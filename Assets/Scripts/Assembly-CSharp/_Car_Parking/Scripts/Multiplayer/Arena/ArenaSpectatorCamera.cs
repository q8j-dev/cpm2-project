using UnityEngine;
using UnityEngine.EventSystems;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaSpectatorCamera : MonoBehaviour
	{
		private const float Distance = 4.5f;

		private const float Height = 2f;

		private const float MinDistance = 1.2f;

		private const float WallPadding = 0.3f;

		private const int PersonLayer = 17;

		private const float PivotSmoothTime = 0.16f;

		private const float PivotSnapDistance = 8f;

		private const float DistancePullIn = 20f;

		private const float DistancePushOut = 3f;

		private const float DragToDegrees = 0.15f;

		private const float DefaultPitch = 8f;

		private const float PitchMin = -10f;

		private const float PitchMax = 45f;

		private Transform _target;

		private float _yaw;

		private float _pitch = 8f;

		private bool _dragBlocked;

		private Vector2 _lastMousePosition;

		private EventRegistry _eventRegistry;

		private DesktopControlsSettingsSO _controlsSettings;

		private RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

		private const float LookEpsilon = 1E-05f;

		private Vector3 _pivot;

		private Vector3 _pivotVelocity;

		private float _distance = 4.5f;

		public bool HasTarget => _target;

		private void Awake()
		{
			base.enabled = false;
		}

		public void BindInput(EventRegistry eventRegistry, DesktopControlsSettingsSO controlsSettings)
		{
			_eventRegistry = eventRegistry;
			_controlsSettings = controlsSettings;
		}

		public void Attach(Transform target)
		{
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<TouchMovePerson>(out var component))
			{
				component.UpdateCamera = false;
			}
			base.enabled = true;
			SetTarget(target);
		}

		public void SetTarget(Transform target)
		{
			_target = target;
			if ((bool)_target)
			{
				_yaw = _target.eulerAngles.y;
				_pitch = 8f;
				_pivot = TargetPivot();
				_pivotVelocity = Vector3.zero;
				_distance = ClearDistance();
				Apply();
			}
		}

		public void Detach()
		{
			_target = null;
			base.enabled = false;
		}

		private void LateUpdate()
		{
			if ((bool)_target)
			{
				ApplyLookFree();
				SmoothPivot();
				SmoothDistance();
				Apply();
			}
		}

		private void SmoothPivot()
		{
			Vector3 vector = TargetPivot();
			if ((vector - _pivot).sqrMagnitude > 64f)
			{
				_pivot = vector;
				_pivotVelocity = Vector3.zero;
			}
			else
			{
				_pivot = Vector3.SmoothDamp(_pivot, vector, ref _pivotVelocity, 0.16f, float.PositiveInfinity, Time.deltaTime);
			}
		}

		private void SmoothDistance()
		{
			float num = ClearDistance();
			float num2 = ((num < _distance) ? 20f : 3f);
			_distance = Mathf.Lerp(_distance, num, 1f - Mathf.Exp((0f - num2) * Time.deltaTime));
		}

		private void Apply()
		{
			base.transform.SetPositionAndRotation(_pivot - LookDirection() * _distance, Quaternion.Euler(_pitch, _yaw, 0f));
		}

		private void ApplyLookFree()
		{
			if (_eventRegistry == null)
			{
				return;
			}
			Vector2Variable vector2Variable = _lookFreeHandle.Get(_eventRegistry);
			if ((bool)vector2Variable)
			{
				Vector2 value = vector2Variable.Value;
				if ((bool)_controlsSettings)
				{
					Vector2 lookScale = _controlsSettings.GetLookScale(CameraSettingsContext.Person);
					value.x *= lookScale.x;
					value.y *= lookScale.y;
				}
				if (!(value.sqrMagnitude < 1E-05f))
				{
					_yaw += value.x;
					_pitch = Mathf.Clamp(_pitch - value.y, -10f, 45f);
				}
			}
		}

		private void ApplyDrag()
		{
			if (TryReadDrag(out var delta))
			{
				_yaw += delta.x * 0.15f;
				_pitch = Mathf.Clamp(_pitch - delta.y * 0.15f, -10f, 45f);
			}
		}

		private bool TryReadDrag(out Vector2 delta)
		{
			delta = Vector2.zero;
			if (Input.touchCount == 0 && !Input.GetMouseButton(0))
			{
				_dragBlocked = false;
				return false;
			}
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began)
				{
					_dragBlocked = IsPointerOverUI(touch.fingerId);
				}
				if (_dragBlocked || touch.phase != TouchPhase.Moved)
				{
					return false;
				}
				delta = touch.deltaPosition;
				return true;
			}
			if (Input.GetMouseButtonDown(0))
			{
				_dragBlocked = IsPointerOverUI(-1);
				_lastMousePosition = Input.mousePosition;
			}
			if (_dragBlocked)
			{
				return false;
			}
			Vector2 vector = Input.mousePosition;
			delta = vector - _lastMousePosition;
			_lastMousePosition = vector;
			return true;
		}

		private static bool IsPointerOverUI(int pointerId)
		{
			if (!UnityEngine.EventSystems.EventSystem.current)
			{
				return false;
			}
			if (pointerId >= 0)
			{
				return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId);
			}
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
		}

		private float ClearDistance()
		{
			if (!Physics.Raycast(_pivot, -LookDirection(), out var hitInfo, 4.5f, -1, QueryTriggerInteraction.Ignore) || hitInfo.transform.gameObject.layer == 17)
			{
				return 4.5f;
			}
			return Mathf.Max(1.2f, hitInfo.distance - 0.3f);
		}

		private Vector3 LookDirection()
		{
			return Quaternion.Euler(_pitch, _yaw, 0f) * Vector3.forward;
		}

		private Vector3 TargetPivot()
		{
			return _target.position + Vector3.up * 2f;
		}
	}
}
