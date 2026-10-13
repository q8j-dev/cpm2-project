using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Shifter.Contracts;
using CPM.Shifter.Logic;
using CPM.Transmission;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.MenuUI.Shifter;
using _Car_Parking.Scripts.UIElement.HUD;

public class AutoShifterController : MonoBehaviour, IShifterMode, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
{
	private enum AutoMode
	{
		Park = 0,
		Reverse = 1,
		Neutral = 2,
		Drive = 3,
		Manual = 4,
		Sport = 5,
		Low = 6
	}

	[SerializeField]
	[Header("Knob & slot RectTransforms")]
	private RectTransform _knob;

	[SerializeField]
	private RectTransform _parkSlot;

	[SerializeField]
	private RectTransform _reverseSlot;

	[SerializeField]
	private RectTransform _neutralSlot;

	[SerializeField]
	private RectTransform _driveSlot;

	[SerializeField]
	private RectTransform _manualSlot;

	[Tooltip("Слот Sport (S) для CVT — на месте M (CVT кажет S вместо M). Назначается в префабе.")]
	[SerializeField]
	private RectTransform _sportSlot;

	[SerializeField]
	[Tooltip("Слот Low (L) для CVT. Назначается в префабе.")]
	private RectTransform _lowSlot;

	[SerializeField]
	[Tooltip("VISUAL slot above M (inside TraceCanvas/Slots). Holds the '+' label. Active only in M mode; fades in/out with knob touch via TraceCanvas CanvasGroup. Position auto-set in LayoutSlots.")]
	private RectTransform _gearUpSlot;

	[SerializeField]
	[Tooltip("VISUAL slot below M (inside TraceCanvas/Slots). Holds the '−' label. Same behavior as _gearUpSlot.")]
	private RectTransform _gearDownSlot;

	[SerializeField]
	[Header("Manual-mode tap buttons (always visible in M, OUTSIDE TraceCanvas)")]
	[Tooltip("Parent GameObject toggled active by UpdateUI when in M mode. Contains _gearUpButton + _gearDownButton.")]
	private GameObject _manualGearPanel;

	[SerializeField]
	[Tooltip("Tap button positioned ABOVE M (inside _manualGearPanel). Position auto-set in LayoutSlots.")]
	private Button _gearUpButton;

	[SerializeField]
	[Tooltip("Tap button positioned BELOW M (inside _manualGearPanel). Position auto-set in LayoutSlots.")]
	private Button _gearDownButton;

	[Header("Slot auto-layout (positions are recomputed each Activate)")]
	[Tooltip("Vertical pitch between adjacent slot centers (P↔R, R↔N, N↔D). Slots are positioned symmetrically around (0,0).")]
	[SerializeField]
	private float _slotPitch = 120f;

	[SerializeField]
	[Tooltip("Horizontal offset of the M slot relative to D (typically negative — M sits LEFT of D).")]
	private float _mSlotOffset = -176f;

	[Tooltip("Gap (px) BETWEEN the knob's edge and the +/− slot/button CENTERS. Final distance from M's center to +/− = knob_half_size + this gap. Default 15 ≈ a thumb's nail width of breathing room — knob no longer overlaps +/−.")]
	[SerializeField]
	private float _gearBumpGap = 15f;

	[SerializeField]
	[Tooltip("Extra padding around the slot bounding rect — how far the knob can travel past the outermost slot during drag.")]
	private float _knobBoundsPadding = 40f;

	[SerializeField]
	[Header("Knob animation")]
	private float _knobSnapDuration = 0.15f;

	[SerializeField]
	private float _knobGrabScale = 1.16f;

	[SerializeField]
	private float _knobScaleDuration = 0.1f;

	[Header("Manual +/- bump animation")]
	[Tooltip("Time for the knob to nudge from M toward the +/- slot on a sequential shift.")]
	[SerializeField]
	private float _gearBumpDuration = 0.08f;

	[Tooltip("Time for the knob to spring back from the +/- slot to M.")]
	[SerializeField]
	private float _gearBumpReturnDuration = 0.12f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How far toward the +/- slot the knob nudges (0 = none, 1 = all the way to the slot).")]
	private float _gearBumpFraction = 0.6f;

	[Header("Trace reveal CanvasGroup (fades slot labels in/out with knob touch)")]
	[SerializeField]
	private CanvasGroup _traceCanvasGroup;

	[SerializeField]
	private float _traceRevealDuration = 0.15f;

	[SerializeField]
	private float _traceHideDuration = 0.25f;

	[SerializeField]
	private float _traceHideDelayAfterSnap = 0.2f;

	[Header("Label (P/R/N/D/M plus engaged gear in D/M)")]
	[SerializeField]
	private TextMeshProUGUI _gearLabel;

	private CarInputManager _carInputManager;

	private AutoMode _currentMode;

	private int _lastForwardGear = 1;

	private Powertrain _gearMemoryOwner;

	private TextMeshProUGUI _manualSlotLabel;

	private readonly List<Rect> _allowedZones = new List<Rect>();

	private ShifterGamepadBridge _shifterGamepadBridge;

	private bool _isDragging;

	private bool _isBumping;

	private Tween _knobTween;

	private Tween _knobScaleTween;

	private Vector3 _knobBaseScale = Vector3.one;

	private Tween _traceTween;

	private CancellationTokenSource _gripWheelCts;

	private const float MinHandSequenceSeconds = 0.4f;

	private const int ReverseGear = -1;

	private const int NeutralGear = 0;

	private const int SelectorPark = 1;

	private const int SelectorReverse = 2;

	private const int SelectorNeutral = 3;

	private const int SelectorDrive = 4;

	private const int SelectorManual = 5;

	private const int SelectorSport = 8;

	private const int SelectorLow = 9;

	public bool IsTouching => _isDragging;

	private bool IsSequential
	{
		get
		{
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if (powertrain != null)
			{
				return powertrain.GearboxKind == GearboxKind.Sequential;
			}
			return false;
		}
	}

	private AutoShifterModes Modes
	{
		get
		{
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if (!(powertrain != null))
			{
				return AutoShifterModes.Manual;
			}
			return powertrain.AutoModes;
		}
	}

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		CaptureKnobBaseScale();
		SteamUIParentChanger.OnChangesApplied += CaptureKnobBaseScale;
	}

	private void OnDestroy()
	{
		SteamUIParentChanger.OnChangesApplied -= CaptureKnobBaseScale;
	}

	private void CaptureKnobBaseScale()
	{
		if (_knob != null)
		{
			_knobBaseScale = _knob.localScale;
		}
	}

	private void OnEnable()
	{
		if ((bool)_gearUpButton)
		{
			_gearUpButton.onClick.AddListener(OnGearUp);
		}
		if ((bool)_gearDownButton)
		{
			_gearDownButton.onClick.AddListener(OnGearDown);
		}
		LayoutSlots();
		SyncToCurrentGear();
		if ((bool)_traceCanvasGroup)
		{
			_traceCanvasGroup.alpha = 0.3f;
		}
	}

	private void LayoutSlots()
	{
		if (IsSequential)
		{
			LayoutSequentialSlots();
			return;
		}
		float num = 1.5f * _slotPitch;
		float num2 = -1.5f * _slotPitch;
		if ((bool)_parkSlot)
		{
			_parkSlot.anchoredPosition = new Vector2(0f, num);
		}
		if ((bool)_reverseSlot)
		{
			_reverseSlot.anchoredPosition = new Vector2(0f, 0.5f * _slotPitch);
		}
		if ((bool)_neutralSlot)
		{
			_neutralSlot.anchoredPosition = new Vector2(0f, -0.5f * _slotPitch);
		}
		if ((bool)_driveSlot)
		{
			_driveSlot.anchoredPosition = new Vector2(0f, num2);
		}
		bool num3 = ModeAvailable(AutoMode.Manual);
		if (num3 && (bool)_manualSlot)
		{
			_manualSlot.anchoredPosition = new Vector2(_mSlotOffset, num2);
			float num4 = HalfMaxDim(_knob);
			float num5 = Mathf.Max(HalfMaxDim(_gearUpButton ? ((RectTransform)_gearUpButton.transform) : null), HalfMaxDim(_gearDownButton ? ((RectTransform)_gearDownButton.transform) : null));
			float num6 = num4 + num5 + _gearBumpGap;
			Vector2 anchoredPosition = new Vector2(_mSlotOffset, num2 + num6);
			Vector2 anchoredPosition2 = new Vector2(_mSlotOffset, num2 - num6);
			if ((bool)_gearUpSlot)
			{
				_gearUpSlot.anchoredPosition = anchoredPosition;
			}
			if ((bool)_gearDownSlot)
			{
				_gearDownSlot.anchoredPosition = anchoredPosition2;
			}
			if ((bool)_gearUpButton)
			{
				((RectTransform)_gearUpButton.transform).anchoredPosition = anchoredPosition;
			}
			if ((bool)_gearDownButton)
			{
				((RectTransform)_gearDownButton.transform).anchoredPosition = anchoredPosition2;
			}
		}
		float num7 = num2;
		if (ModeAvailable(AutoMode.Sport) && (bool)_sportSlot)
		{
			num7 -= _slotPitch;
			_sportSlot.anchoredPosition = new Vector2(0f, num7);
		}
		if (ModeAvailable(AutoMode.Low) && (bool)_lowSlot)
		{
			num7 -= _slotPitch;
			_lowSlot.anchoredPosition = new Vector2(0f, num7);
		}
		_allowedZones.Clear();
		float knobBoundsPadding = _knobBoundsPadding;
		_allowedZones.Add(new Rect(0f - knobBoundsPadding, num7 - knobBoundsPadding, 2f * knobBoundsPadding, num - num7 + 2f * knobBoundsPadding));
		if (num3)
		{
			float num8 = Mathf.Min(0f, _mSlotOffset) - knobBoundsPadding;
			float num9 = Mathf.Max(0f, _mSlotOffset) + knobBoundsPadding;
			_allowedZones.Add(new Rect(num8, num2 - knobBoundsPadding, num9 - num8, 2f * knobBoundsPadding));
		}
	}

	private void LayoutSequentialSlots()
	{
		float num = -0.5f * _slotPitch;
		if ((bool)_parkSlot)
		{
			_parkSlot.anchoredPosition = new Vector2(0f, 1.5f * _slotPitch);
		}
		if ((bool)_reverseSlot)
		{
			_reverseSlot.anchoredPosition = new Vector2(0f, 0.5f * _slotPitch);
		}
		if ((bool)_neutralSlot)
		{
			_neutralSlot.anchoredPosition = new Vector2(0f, num);
		}
		if ((bool)_manualSlot)
		{
			_manualSlot.anchoredPosition = new Vector2(_mSlotOffset, num);
		}
		float num2 = HalfMaxDim(_knob);
		float num3 = Mathf.Max(HalfMaxDim(_gearUpButton ? ((RectTransform)_gearUpButton.transform) : null), HalfMaxDim(_gearDownButton ? ((RectTransform)_gearDownButton.transform) : null));
		float num4 = num2 + num3 + _gearBumpGap;
		Vector2 anchoredPosition = new Vector2(_mSlotOffset, num - num4);
		Vector2 anchoredPosition2 = new Vector2(_mSlotOffset, num + num4);
		if ((bool)_gearUpSlot)
		{
			_gearUpSlot.anchoredPosition = anchoredPosition;
		}
		if ((bool)_gearDownSlot)
		{
			_gearDownSlot.anchoredPosition = anchoredPosition2;
		}
		if ((bool)_gearUpButton)
		{
			((RectTransform)_gearUpButton.transform).anchoredPosition = anchoredPosition;
		}
		if ((bool)_gearDownButton)
		{
			((RectTransform)_gearDownButton.transform).anchoredPosition = anchoredPosition2;
		}
		_allowedZones.Clear();
		float knobBoundsPadding = _knobBoundsPadding;
		_allowedZones.Add(new Rect(0f - knobBoundsPadding, num - knobBoundsPadding, 2f * knobBoundsPadding, 1.5f * _slotPitch - num + 2f * knobBoundsPadding));
		float num5 = Mathf.Min(0f, _mSlotOffset) - knobBoundsPadding;
		float num6 = Mathf.Max(0f, _mSlotOffset) + knobBoundsPadding;
		_allowedZones.Add(new Rect(num5, num - knobBoundsPadding, num6 - num5, 2f * knobBoundsPadding));
	}

	private void OnDisable()
	{
		if ((bool)_gearUpButton)
		{
			_gearUpButton.onClick.RemoveListener(OnGearUp);
		}
		if ((bool)_gearDownButton)
		{
			_gearDownButton.onClick.RemoveListener(OnGearDown);
		}
		_knobTween?.Kill();
		_knobScaleTween?.Kill();
		_traceTween?.Kill();
		if (_knob != null)
		{
			_knob.localScale = _knobBaseScale;
		}
		_isDragging = false;
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
	}

	private void Update()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			ResetGearMemoryOnCarChange(powertrain);
			if (powertrain.Gear > 0)
			{
				_lastForwardGear = powertrain.Gear;
			}
		}
	}

	private void ResetGearMemoryOnCarChange(Powertrain pt)
	{
		if (!(_gearMemoryOwner == pt))
		{
			_gearMemoryOwner = pt;
			_lastForwardGear = 1;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!(_knob == null))
		{
			_isDragging = true;
			RevealTrace();
			UpdateUI();
			ScaleKnob(_knobGrabScale);
			CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
			DriverHandHelpers.Shiftgear();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_isDragging && (bool)_knob)
		{
			_knobTween?.Kill();
			Vector2 point = _knob.anchoredPosition + eventData.delta;
			point = ShifterMath.ClampToAllowedZones(point, _allowedZones);
			_knob.anchoredPosition = point;
			ApplyLabel(FindNearestMode());
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isDragging)
		{
			_isDragging = false;
			ScaleKnob(1f);
			AutoMode currentMode = _currentMode;
			AutoMode autoMode = FindNearestMode();
			SnapKnobTo(autoMode, _knobSnapDuration);
			EnterMode(autoMode);
			if (currentMode != autoMode)
			{
				TryVibrate();
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
			}
			HideTraceWithDelay(_knobSnapDuration + _traceHideDelayAfterSnap);
			ShifterController.FireReleaseGearEvent();
			CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
			_gripWheelCts = new CancellationTokenSource();
			DelayedGripWheel(_gripWheelCts.Token).Forget();
		}
	}

	private async UniTaskVoid DelayedGripWheel(CancellationToken ct)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.4000000059604645), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
		catch (OperationCanceledException)
		{
			return;
		}
		DriverHandHelpers.GripWheel();
	}

	private AutoMode FindNearestMode()
	{
		if (!_knob)
		{
			return _currentMode;
		}
		Vector2 anchoredPosition = _knob.anchoredPosition;
		AutoMode best = _currentMode;
		float bestDistSq = float.MaxValue;
		Consider(_parkSlot, AutoMode.Park, anchoredPosition, ref best, ref bestDistSq);
		Consider(_reverseSlot, AutoMode.Reverse, anchoredPosition, ref best, ref bestDistSq);
		Consider(_neutralSlot, AutoMode.Neutral, anchoredPosition, ref best, ref bestDistSq);
		if (!IsSequential)
		{
			Consider(_driveSlot, AutoMode.Drive, anchoredPosition, ref best, ref bestDistSq);
		}
		if (ModeAvailable(AutoMode.Manual))
		{
			Consider(_manualSlot, AutoMode.Manual, anchoredPosition, ref best, ref bestDistSq);
		}
		if (ModeAvailable(AutoMode.Sport))
		{
			Consider(_sportSlot, AutoMode.Sport, anchoredPosition, ref best, ref bestDistSq);
		}
		if (ModeAvailable(AutoMode.Low))
		{
			Consider(_lowSlot, AutoMode.Low, anchoredPosition, ref best, ref bestDistSq);
		}
		return best;
	}

	private static void Consider(RectTransform slot, AutoMode mode, Vector2 kp, ref AutoMode best, ref float bestDistSq)
	{
		if (!(slot == null))
		{
			Vector2 anchoredPosition = slot.anchoredPosition;
			float num = anchoredPosition.x - kp.x;
			float num2 = anchoredPosition.y - kp.y;
			float num3 = num * num + num2 * num2;
			if (num3 < bestDistSq)
			{
				bestDistSq = num3;
				best = mode;
			}
		}
	}

	private RectTransform SlotFor(AutoMode mode)
	{
		return mode switch
		{
			AutoMode.Park => _parkSlot, 
			AutoMode.Reverse => _reverseSlot, 
			AutoMode.Neutral => _neutralSlot, 
			AutoMode.Drive => _driveSlot, 
			AutoMode.Manual => _manualSlot, 
			AutoMode.Sport => _sportSlot, 
			AutoMode.Low => _lowSlot, 
			_ => null, 
		};
	}

	private void SnapKnobTo(AutoMode mode, float duration)
	{
		RectTransform rectTransform = SlotFor(mode);
		if ((bool)rectTransform && (bool)_knob)
		{
			_knobTween?.Kill();
			if (duration <= 0f)
			{
				_knob.anchoredPosition = rectTransform.anchoredPosition;
			}
			else
			{
				_knobTween = _knob.DOAnchorPos(rectTransform.anchoredPosition, duration).SetEase(Ease.OutQuad);
			}
		}
	}

	private void EnterMode(AutoMode mode)
	{
		switch (mode)
		{
		case AutoMode.Park:
			OnPark();
			break;
		case AutoMode.Reverse:
			OnReverse();
			break;
		case AutoMode.Neutral:
			OnNeutral();
			break;
		case AutoMode.Drive:
			OnDrive();
			break;
		case AutoMode.Manual:
			OnManual();
			break;
		case AutoMode.Sport:
			OnSport();
			break;
		case AutoMode.Low:
			OnLow();
			break;
		}
	}

	private void OnPark()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.Automatic = true;
			powertrain.Gear = 0;
			powertrain.IsParkingGear = true;
			powertrain.SportMode = false;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 1;
			_currentMode = AutoMode.Park;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnReverse()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.IsParkingGear = false;
			powertrain.Automatic = true;
			powertrain.Gear = -1;
			powertrain.SportMode = false;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 2;
			_currentMode = AutoMode.Reverse;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnNeutral()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.IsParkingGear = false;
			powertrain.Automatic = true;
			powertrain.Gear = 0;
			powertrain.SportMode = false;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 3;
			_currentMode = AutoMode.Neutral;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnDrive()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.IsParkingGear = false;
			powertrain.Automatic = true;
			if (powertrain.Gear < 1)
			{
				powertrain.Gear = powertrain.GetOptimalGearForDriveEngagement();
			}
			powertrain.SportMode = false;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 4;
			_currentMode = AutoMode.Drive;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnManual()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			ResetGearMemoryOnCarChange(powertrain);
			powertrain.IsParkingGear = false;
			powertrain.Automatic = false;
			int gear = ((powertrain.Gear > 0) ? powertrain.Gear : ((!IsSequential) ? 1 : _lastForwardGear));
			powertrain.Gear = gear;
			powertrain.SportMode = false;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 5;
			_currentMode = AutoMode.Manual;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnSport()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.IsParkingGear = false;
			powertrain.Automatic = true;
			if (powertrain.Gear < 1)
			{
				powertrain.Gear = powertrain.GetOptimalGearForDriveEngagement();
			}
			powertrain.SportMode = true;
			powertrain.LowMode = false;
			_carInputManager.SelectorState = 4;
			_currentMode = AutoMode.Sport;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	private void OnLow()
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			powertrain.IsParkingGear = false;
			powertrain.Automatic = true;
			if (powertrain.Gear < 1)
			{
				powertrain.Gear = powertrain.GetOptimalGearForDriveEngagement();
			}
			powertrain.SportMode = false;
			powertrain.LowMode = true;
			_carInputManager.SelectorState = 4;
			_currentMode = AutoMode.Low;
			powertrain.RaiseGearChanged();
			UpdateUI();
		}
	}

	public void SetAutoMode(int selectorState)
	{
		switch (selectorState)
		{
		case 1:
			OnPark();
			break;
		case 2:
			OnReverse();
			break;
		case 3:
			OnNeutral();
			break;
		case 4:
			OnDrive();
			break;
		case 5:
			if (ModeAvailable(AutoMode.Manual))
			{
				OnManual();
			}
			break;
		case 6:
		case 7:
			if (ModeAvailable(AutoMode.Manual))
			{
				if (_currentMode != AutoMode.Manual)
				{
					OnManual();
				}
				if (selectorState == 6)
				{
					OnGearUp();
				}
				else
				{
					OnGearDown();
				}
			}
			break;
		case 8:
			if (ModeAvailable(AutoMode.Sport))
			{
				OnSport();
			}
			break;
		case 9:
			if (ModeAvailable(AutoMode.Low))
			{
				OnLow();
			}
			break;
		}
	}

	private void OnGearUp()
	{
		if (_currentMode != AutoMode.Manual)
		{
			return;
		}
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			int num = powertrain.Gear + 1;
			if (num > powertrain.NumGears)
			{
				num = powertrain.NumGears;
			}
			if (num != powertrain.Gear)
			{
				ApplyGear(num);
				TryVibrate();
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
				PlayDriverShiftAnimation();
				BumpKnob(_gearUpSlot);
			}
		}
	}

	private void OnGearDown()
	{
		if (_currentMode != AutoMode.Manual)
		{
			return;
		}
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			int num = Mathf.Max(1, powertrain.Gear - 1);
			if (num != powertrain.Gear)
			{
				ApplyGear(num);
				TryVibrate();
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
				PlayDriverShiftAnimation();
				BumpKnob(_gearDownSlot);
			}
		}
	}

	private void BumpKnob(RectTransform bumpSlot)
	{
		if ((bool)_knob && (bool)_manualSlot && (bool)bumpSlot && !_isDragging)
		{
			_knobTween?.Kill();
			Vector2 anchoredPosition = _manualSlot.anchoredPosition;
			Vector2 endValue = anchoredPosition + (bumpSlot.anchoredPosition - anchoredPosition) * _gearBumpFraction;
			_knob.anchoredPosition = anchoredPosition;
			_isBumping = true;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_knob.DOAnchorPos(endValue, _gearBumpDuration).SetEase(Ease.OutQuad));
			sequence.Append(_knob.DOAnchorPos(anchoredPosition, _gearBumpReturnDuration).SetEase(Ease.InQuad));
			sequence.OnComplete(() =>
			{
				_isBumping = false;
			});
			sequence.OnKill(() =>
			{
				_isBumping = false;
			});
			_knobTween = sequence;
		}
	}

	private void ApplyGear(int gear)
	{
		if ((bool)_carInputManager?.GetPowertrain)
		{
			_carInputManager.GetPowertrain.Gear = gear;
			UpdateLabel();
		}
	}

	public void Activate()
	{
		LayoutSlots();
		SyncToCurrentGear();
		if (!_shifterGamepadBridge)
		{
			_shifterGamepadBridge = GetComponentInParent<ShifterGamepadBridge>();
		}
		if ((bool)_shifterGamepadBridge)
		{
			_shifterGamepadBridge.SubscribeToActions(isManual: false);
		}
	}

	public void Deactivate()
	{
		_knobTween?.Kill();
		_traceTween?.Kill();
		if ((bool)_traceCanvasGroup)
		{
			_traceCanvasGroup.alpha = 0.3f;
		}
	}

	public void SyncToCurrentGear()
	{
		SyncFromPowertrain(_knobSnapDuration);
	}

	public void ResetToNeutral()
	{
	}

	public void SetPosByGear(int gear)
	{
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		AutoMode mode = (_currentMode = ((gear < 0) ? AutoMode.Reverse : ((gear != 0) ? ((_currentMode == AutoMode.Manual) ? AutoMode.Manual : ((powertrain != null && powertrain.LowMode) ? AutoMode.Low : ((powertrain != null && powertrain.SportMode) ? AutoMode.Sport : AutoMode.Drive))) : ((_currentMode != AutoMode.Park) ? AutoMode.Neutral : AutoMode.Park))));
		if (!_isDragging)
		{
			SnapKnobTo(mode, 0f);
		}
		UpdateUI();
	}

	public void SetGearTraceAlpha(float a)
	{
		if ((bool)_traceCanvasGroup)
		{
			_traceCanvasGroup.alpha = Mathf.Clamp01(a);
		}
	}

	public void SetGearLabelText(string text)
	{
		if ((bool)_gearLabel)
		{
			_gearLabel.text = text;
		}
	}

	public void PlayDriverShiftAnimation()
	{
		CommonExtensions.CancelAndDisposeToken(ref _gripWheelCts);
		DriverHandHelpers.Shiftgear();
		_gripWheelCts = new CancellationTokenSource();
		DelayedGripWheel(_gripWheelCts.Token).Forget();
	}

	private void SyncFromPowertrain(float snapDuration)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		Powertrain powertrain = _carInputManager?.GetPowertrain;
		if (!(powertrain == null))
		{
			ResetGearMemoryOnCarChange(powertrain);
			if (!powertrain.Automatic)
			{
				_currentMode = AutoMode.Manual;
			}
			else if (powertrain.IsParkingGear)
			{
				_currentMode = AutoMode.Park;
			}
			else if (powertrain.LowMode && powertrain.Gear > 0)
			{
				_currentMode = AutoMode.Low;
			}
			else if (powertrain.SportMode && powertrain.Gear > 0)
			{
				_currentMode = AutoMode.Sport;
			}
			else
			{
				int gear = powertrain.Gear;
				_currentMode = ((gear < 0) ? AutoMode.Reverse : ((gear == 0) ? AutoMode.Neutral : (IsSequential ? AutoMode.Manual : AutoMode.Drive)));
			}
			ApplyModeVisibility();
			UpdateUI();
			if (!_isDragging && !_isBumping)
			{
				SnapKnobTo(_currentMode, snapDuration);
			}
		}
	}

	private string SequentialGearText()
	{
		int valueOrDefault = (_carInputManager?.GetPowertrain?.Gear).GetValueOrDefault();
		return ((valueOrDefault > 0) ? valueOrDefault : _lastForwardGear).ToString();
	}

	private bool ModeAvailable(AutoMode m)
	{
		return m switch
		{
			AutoMode.Manual => IsSequential || Modes.HasFlag(AutoShifterModes.Manual), 
			AutoMode.Sport => Modes.HasFlag(AutoShifterModes.Sport), 
			AutoMode.Low => Modes.HasFlag(AutoShifterModes.Low), 
			_ => true, 
		};
	}

	private void ApplyModeVisibility()
	{
		if ((bool)_manualSlot)
		{
			_manualSlot.gameObject.SetActive(ModeAvailable(AutoMode.Manual));
		}
		if ((bool)_sportSlot)
		{
			_sportSlot.gameObject.SetActive(ModeAvailable(AutoMode.Sport));
		}
		if ((bool)_lowSlot)
		{
			_lowSlot.gameObject.SetActive(ModeAvailable(AutoMode.Low));
		}
		if ((bool)_driveSlot)
		{
			_driveSlot.gameObject.SetActive(!IsSequential);
		}
	}

	private void UpdateUI()
	{
		bool flag = ModeAvailable(AutoMode.Manual);
		bool flag2 = _currentMode == AutoMode.Manual;
		bool flag3 = _isDragging || !flag2;
		bool flag4 = _isDragging | flag2;
		if ((bool)_parkSlot)
		{
			_parkSlot.gameObject.SetActive(flag3);
		}
		if ((bool)_reverseSlot)
		{
			_reverseSlot.gameObject.SetActive(flag3);
		}
		if ((bool)_neutralSlot)
		{
			_neutralSlot.gameObject.SetActive(flag3);
		}
		if ((bool)_driveSlot)
		{
			_driveSlot.gameObject.SetActive(flag3 && !IsSequential);
		}
		if ((bool)_sportSlot)
		{
			_sportSlot.gameObject.SetActive(flag3 && ModeAvailable(AutoMode.Sport));
		}
		if ((bool)_lowSlot)
		{
			_lowSlot.gameObject.SetActive(flag3 && ModeAvailable(AutoMode.Low));
		}
		if ((bool)_manualSlot)
		{
			_manualSlot.gameObject.SetActive(flag & flag4);
		}
		if ((bool)_manualGearPanel)
		{
			_manualGearPanel.SetActive(flag & flag2);
		}
		if ((bool)_gearUpSlot)
		{
			_gearUpSlot.gameObject.SetActive(flag & flag4);
		}
		if ((bool)_gearDownSlot)
		{
			_gearDownSlot.gameObject.SetActive(flag & flag4);
		}
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		ApplyLabel(_currentMode);
		if (!(_manualSlot != null))
		{
			return;
		}
		if (_manualSlotLabel == null)
		{
			TextMeshProUGUI componentInChildren = _manualSlot.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
			if (componentInChildren != _gearLabel)
			{
				_manualSlotLabel = componentInChildren;
			}
		}
		if (_manualSlotLabel != null)
		{
			_manualSlotLabel.text = (IsSequential ? SequentialGearText() : "M");
		}
	}

	private void ApplyLabel(AutoMode mode)
	{
		if (!(_gearLabel == null))
		{
			TextMeshProUGUI gearLabel = _gearLabel;
			gearLabel.text = mode switch
			{
				AutoMode.Park => "P", 
				AutoMode.Reverse => "R", 
				AutoMode.Neutral => "N", 
				AutoMode.Drive => "D", 
				AutoMode.Manual => IsSequential ? SequentialGearText() : "M", 
				AutoMode.Sport => "S", 
				AutoMode.Low => "L", 
				_ => string.Empty, 
			};
			Color color = mode switch
			{
				AutoMode.Reverse => Color.red, 
				AutoMode.Neutral => Color.green, 
				_ => ThemedLabelColor(), 
			};
			_gearLabel.color = color;
			_gearLabel.faceColor = color;
		}
	}

	private void TryVibrate()
	{
		if (!V.Car)
		{
			return;
		}
		try
		{
			float intensity = 0.1f;
			Utils.Vibrate(0.1f, intensity);
		}
		catch (Exception ex)
		{
			Debug.LogError("AutoShifter vibration error: " + ex.Message);
		}
	}

	private static float HalfMaxDim(RectTransform rt)
	{
		if (rt == null)
		{
			return 0f;
		}
		Vector2 size = rt.rect.size;
		return Mathf.Max(size.x, size.y) * 0.5f;
	}

	private static Color ThemedLabelColor()
	{
		Color result = ((PlayerPrefs.GetInt("DarkButtons", 0) == 1) ? HudColorChanger.DarkColor : HudColorChanger.DefaultColor);
		result.a = 0.6f;
		return result;
	}

	private void ScaleKnob(float scale)
	{
		if ((bool)_knob)
		{
			_knobScaleTween?.Kill();
			_knobScaleTween = _knob.DOScale(_knobBaseScale * scale, _knobScaleDuration).SetEase(Ease.OutQuad);
		}
	}

	private void RevealTrace()
	{
		if (!(_traceCanvasGroup == null))
		{
			_traceTween?.Kill();
			_traceTween = _traceCanvasGroup.DOFade(1f, _traceRevealDuration).SetEase(Ease.OutQuad);
		}
	}

	private void HideTraceWithDelay(float delay)
	{
		if (!(_traceCanvasGroup == null))
		{
			_traceTween?.Kill();
			_traceTween = _traceCanvasGroup.DOFade(0.3f, _traceHideDuration).SetEase(Ease.InQuad).SetDelay(delay);
		}
	}
}
