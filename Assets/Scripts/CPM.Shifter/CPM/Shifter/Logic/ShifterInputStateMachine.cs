using System;
using System.Collections.Generic;
using System.Diagnostics;
using CPM.Shifter.Contracts;
using UnityEngine;

namespace CPM.Shifter.Logic
{
	public class ShifterInputStateMachine
	{
		public const float ClutchEngageThreshold = 0.7f;

		public const float SnapBackDuration = 0.15f;

		private const float DragSnapDuration = 0.15f;

		private const float TapMaxMovementSq = 400f;

		private IManualHShifterView _view;

		private Func<float> _clutch;

		private bool _isTouching;

		private float _maxClutchDuringGrab;

		private int _lastPreviewGear = int.MinValue;

		private bool _isGamepadGrab;

		private int _lastReachedGear;

		private int _snappedSlot = int.MinValue;

		private Vector2 _knobAtPointerDown;

		private Vector2 _fingerOffset;

		private readonly Dictionary<int, Vector2> _slotsCache = new Dictionary<int, Vector2>(8);

		private bool _wasOutsideBbox;

		public bool IsTouching => _isTouching;

		public bool ClampFingerToBounds { get; set; }

		public event Action<int> OnGearRequested;

		public event Action<int> OnPreviewGearChanged;

		public event Action OnGrabStarted;

		public event Action OnGrabReleased;

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private static void Log(string s)
		{
			UnityEngine.Debug.Log("<color=#00C8FF>[ShifterSM]</color> " + s);
		}

		public void Bind(IManualHShifterView view, Func<float> clutchProvider)
		{
			_view = view;
			_clutch = clutchProvider;
		}

		public void Unbind()
		{
			_view = null;
			_clutch = null;
			_isTouching = false;
			_isGamepadGrab = false;
			_lastReachedGear = 0;
			_lastPreviewGear = int.MinValue;
			_snappedSlot = int.MinValue;
			_maxClutchDuringGrab = 0f;
		}

		public void PointerDown(bool gamepad = false)
		{
			if (_view != null && _view.IsLayoutReady)
			{
				_isTouching = true;
				_isGamepadGrab = gamepad;
				_lastReachedGear = 0;
				_lastPreviewGear = int.MinValue;
				_snappedSlot = int.MinValue;
				_wasOutsideBbox = false;
				_knobAtPointerDown = _view.KnobAnchoredPos;
				_fingerOffset = Vector2.zero;
				_maxClutchDuringGrab = _clutch?.Invoke() ?? 0f;
				OnGrabStarted?.Invoke();
			}
		}

		public void Drag(Vector2 delta, float neutralBandHalfHeight)
		{
			if (!_isTouching || _view == null)
			{
				return;
			}
			if (_clutch != null)
			{
				float num = _clutch();
				if (num > _maxClutchDuringGrab)
				{
					_maxClutchDuringGrab = num;
				}
			}
			_fingerOffset += delta;
			if (ClampFingerToBounds)
			{
				Vector2 knobAnchoredPos = ShifterMath.ClampToAllowedZones(_view.KnobAnchoredPos + delta, _view.AllowedZones);
				_view.SetKnobAnchoredPos(knobAnchoredPos);
				Dictionary<int, Vector2> slotPixelPositions = SnapshotActiveSlots();
				int num2 = ShifterMath.NearestSlot(knobAnchoredPos, slotPixelPositions, neutralBandHalfHeight);
				if (num2 != _lastPreviewGear)
				{
					_lastPreviewGear = num2;
					OnPreviewGearChanged?.Invoke(num2);
				}
				return;
			}
			Vector2 vector = _knobAtPointerDown + _fingerOffset;
			if (ShifterMath.IsOutsideZonesBoundingBox(vector, _view.AllowedZones))
			{
				if (!_wasOutsideBbox)
				{
					_wasOutsideBbox = true;
				}
				int num3 = NearestSlotGearTo(vector);
				if (num3 != _snappedSlot)
				{
					_snappedSlot = num3;
					_view.MoveKnobTo(_view.GetSlotAnchoredPos(num3), 0.15f);
				}
			}
			else
			{
				if (_wasOutsideBbox)
				{
					_wasOutsideBbox = false;
				}
				_snappedSlot = int.MinValue;
				Vector2 knobAnchoredPos2 = ShifterMath.ClampToAllowedZones(vector, _view.AllowedZones);
				_view.SetKnobAnchoredPos(knobAnchoredPos2);
			}
			Dictionary<int, Vector2> slotPixelPositions2 = SnapshotActiveSlots();
			int num4 = ShifterMath.NearestSlot(vector, slotPixelPositions2, neutralBandHalfHeight);
			if (num4 != 0)
			{
				_lastReachedGear = num4;
			}
			if (num4 != _lastPreviewGear)
			{
				_lastPreviewGear = num4;
				OnPreviewGearChanged?.Invoke(num4);
			}
		}

		private int NearestSlotGearTo(Vector2 pos)
		{
			int result = 0;
			float num = (_view.GetSlotAnchoredPos(0) - pos).sqrMagnitude;
			for (int i = -1; i <= 7; i++)
			{
				if (i != 0 && _view.IsSlotActive(i))
				{
					float sqrMagnitude = (_view.GetSlotAnchoredPos(i) - pos).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						result = i;
					}
				}
			}
			return result;
		}

		public void PointerUp(float neutralBandHalfHeight)
		{
			if (!_isTouching || _view == null)
			{
				return;
			}
			_isTouching = false;
			OnGrabReleased?.Invoke();
			if (_isGamepadGrab)
			{
				_isGamepadGrab = false;
				if (_fingerOffset.sqrMagnitude < 400f)
				{
					_view.MoveKnobTo(_knobAtPointerDown, 0.15f);
					int num = NearestSlotGearTo(_knobAtPointerDown);
					if (_lastPreviewGear != num)
					{
						_lastPreviewGear = num;
						OnPreviewGearChanged?.Invoke(num);
					}
				}
				else if (ClampFingerToBounds)
				{
					Dictionary<int, Vector2> slotPixelPositions = SnapshotActiveSlots();
					int num2 = ShifterMath.NearestSlot(_view.KnobAnchoredPos, slotPixelPositions, neutralBandHalfHeight);
					int gear = ((_maxClutchDuringGrab >= 0.7f) ? num2 : 0);
					Snap(gear);
				}
				else if (_maxClutchDuringGrab >= 0.7f && _lastReachedGear != 0)
				{
					Snap(_lastReachedGear);
				}
				else
				{
					Snap(0);
				}
				return;
			}
			Dictionary<int, Vector2> slotPixelPositions2 = SnapshotActiveSlots();
			int num3 = ShifterMath.NearestSlot(_view.KnobAnchoredPos, slotPixelPositions2, neutralBandHalfHeight);
			if (num3 == 0)
			{
				Snap(0);
			}
			else if (_maxClutchDuringGrab >= 0.7f)
			{
				Snap(num3);
			}
			else if (_fingerOffset.sqrMagnitude < 400f)
			{
				_view.MoveKnobTo(_knobAtPointerDown, 0.15f);
				int num4 = NearestSlotGearTo(_knobAtPointerDown);
				if (_lastPreviewGear != num4)
				{
					_lastPreviewGear = num4;
					OnPreviewGearChanged?.Invoke(num4);
				}
			}
			else
			{
				Snap(0);
			}
		}

		private void Snap(int gear)
		{
			OnGearRequested?.Invoke(gear);
			if (_view != null)
			{
				_view.MoveKnobTo(_view.GetSlotAnchoredPos(gear), 0.15f);
			}
		}

		private Dictionary<int, Vector2> SnapshotActiveSlots()
		{
			_slotsCache.Clear();
			for (int i = -1; i <= 7; i++)
			{
				if (i != 0 && _view.IsSlotActive(i))
				{
					_slotsCache[i] = _view.GetSlotAnchoredPos(i);
				}
			}
			return _slotsCache;
		}
	}
}
