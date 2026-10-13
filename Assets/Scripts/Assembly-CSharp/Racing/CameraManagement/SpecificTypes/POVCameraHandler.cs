using System;
using System.Threading;
using CPMEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Infrastructure;

namespace Racing.CameraManagement.SpecificTypes
{
	public class POVCameraHandler : VirtualCameraHandler
	{
		private enum SideLook
		{
			None = 0,
			Left = 1,
			Right = 2
		}

		[Serializable]
		private class ZoomFOVData
		{
			public float Max = default;

			public float Min = default;

			public float Speed = default;

			public bool IsEnabled = default;

			[NonSerialized]
			public float Default;
		}

		[SerializeField]
		private float MaxXAngle = 120f;

		[SerializeField]
		private ZoomFOVData ZoomFOV = default;

		private CinemachinePOV _pov;

		private float _origHMaxSpeed;

		private float _origVMaxSpeed;

		private float _xRecenterDelay;

		private float _yRecenterDelay;

		private bool _isCheckingAngle;

		private bool _isDestroyed;

		private CinemachineVirtualCamera _cinemachineVirtualCamera;

		private MainPerson _mainPerson;

		private IDisposable _switchHeadDisposable;

		private UnityAction<ICinemachineCamera, ICinemachineCamera> _onCameraLiveAction;

		private CancellationTokenSource _activeCameraCts;

		private const float RearHeightFactor = 0.55f;

		private const float RearClearance = 0.05f;

		private const float RearOverhang = 0.6f;

		private const float RearHeightAboveAxle = 1f;

		private const float LookYawSmoothTime = 0.12f;

		[SerializeField]
		private float restDownPitch = 6f;

		[SerializeField]
		private float restPitchDelay = 1f;

		[SerializeField]
		private float restPitchSmoothTime = 0.3f;

		private float _restPitchIdle;

		private float _restPitchVel;

		private const float RecenterMinSpeedKmh = 3f;

		private int _lookActiveFrame = -10;

		private float _povFovTarget = -1f;

		private float _povFovVel;

		private const float PovZoomSmoothTime = 0.12f;

		protected override Vector2 _multiplier => new Vector2(1.35f, 0.78f);

		protected override CameraSettingsContext SettingsContext => CameraSettingsContext.CarFpv;

		protected override bool UseUngatedLook => true;

		protected override float LookRightYaw => 0f;

		protected override float LookLeftYaw => 180f;

		protected override float LookForwardYaw => 270f;

		protected override float LookBackYaw => 90f;

		protected override float CurrentYaw
		{
			get
			{
				if (!(_pov != null))
				{
					return 0f;
				}
				return _pov.m_HorizontalAxis.Value;
			}
			set
			{
				if (_pov != null)
				{
					_pov.m_HorizontalAxis.Value = value;
				}
			}
		}

		protected override void ApplyLook(Vector2 look)
		{
			if (!(_pov == null))
			{
				_lookActiveFrame = Time.frameCount;
				_pov.m_HorizontalRecentering.CancelRecentering();
				_pov.m_VerticalRecentering.CancelRecentering();
				float num = (((bool)_cinemachineVirtualCamera && ZoomFOV.Default > 0f) ? (_cinemachineVirtualCamera.m_Lens.FieldOfView / ZoomFOV.Default) : 1f);
				_pov.m_HorizontalAxis.Value += look.x * num;
				_pov.m_VerticalAxis.Value -= look.y * num;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_pov = _camera.GetCinemachineComponent<CinemachinePOV>();
			if (!(_pov == null))
			{
				_isXRecenter = _pov.m_HorizontalRecentering.m_enabled;
				_isYRecenter = _pov.m_VerticalRecentering.m_enabled;
				_xRecenterDelay = _pov.m_HorizontalRecentering.m_WaitTime;
				_yRecenterDelay = _pov.m_VerticalRecentering.m_WaitTime;
				_origHMaxSpeed = _pov.m_HorizontalAxis.m_MaxSpeed;
				_origVMaxSpeed = _pov.m_VerticalAxis.m_MaxSpeed;
				_cinemachineVirtualCamera = VirtualCamera as CinemachineVirtualCamera;
				if ((bool)_cinemachineVirtualCamera)
				{
					ZoomFOV.Default = _cinemachineVirtualCamera.m_Lens.FieldOfView;
				}
				_onLookFreeHandle.Get(_eventRegistry).Subscribe(OnLookFree);
				_pov.m_HorizontalAxis.m_InputAxisName = string.Empty;
				_pov.m_VerticalAxis.m_InputAxisName = string.Empty;
				_pov.m_HorizontalAxis.m_InputAxisValue = 0f;
				_pov.m_VerticalAxis.m_InputAxisValue = 0f;
			}
		}

		protected override void OnLookOverrideStart()
		{
		}

		protected override void OnLookOverrideEnd()
		{
		}

		protected override void OnLookBackInstantStart()
		{
		}

		protected override void OnLookBackInstantEnd()
		{
		}

		protected override void LateUpdate()
		{
			if (_pov != null)
			{
				bool isCircleMenuGroupActive = base.IsCircleMenuGroupActive;
				_pov.m_HorizontalAxis.m_MaxSpeed = (isCircleMenuGroupActive ? 0f : _origHMaxSpeed);
				_pov.m_VerticalAxis.m_MaxSpeed = (isCircleMenuGroupActive ? 0f : _origVMaxSpeed);
				bool flag = IsCarMoving();
				_pov.m_HorizontalRecentering.m_enabled = flag && _isXRecenter;
				_pov.m_VerticalRecentering.m_enabled = flag && _isYRecenter;
			}
			base.LateUpdate();
			ApplyRestDownPitch();
			SmoothPovZoom();
		}

		protected override void OnLookLeft(bool start)
		{
		}

		protected override void OnLookRight(bool start)
		{
		}

		protected override void OnLookBack(bool start)
		{
		}

		private void UpdateSideLookYaw()
		{
		}

		private void SmoothPovZoom()
		{
			if (ZoomFOV.IsEnabled && (bool)_cinemachineVirtualCamera && !(_povFovTarget < 0f))
			{
				float fieldOfView = _cinemachineVirtualCamera.m_Lens.FieldOfView;
				_cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.SmoothDamp(fieldOfView, _povFovTarget, ref _povFovVel, 0.12f);
			}
		}

		private bool IsCarMoving()
		{
			if (!V.Car || !V.Car.TryGetComponent<VehicleData>(out var component))
			{
				return false;
			}
			Powertrain getPowertrain = component.GetPowertrain;
			if ((bool)getPowertrain)
			{
				return getPowertrain.SpeedKmh > 3f;
			}
			return false;
		}

		private void ApplyRestDownPitch()
		{
			if (_pov == null || !_isActive)
			{
				return;
			}
			if (!IsCarMoving())
			{
				_restPitchIdle = 0f;
				_restPitchVel = 0f;
				return;
			}
			if (Time.frameCount - _lookActiveFrame <= 1)
			{
				_restPitchIdle = 0f;
				_restPitchVel = 0f;
				return;
			}
			_restPitchIdle += Time.deltaTime;
			if (!(_restPitchIdle < restPitchDelay))
			{
				_pov.m_VerticalAxis.Value = Mathf.SmoothDampAngle(_pov.m_VerticalAxis.Value, restDownPitch, ref _restPitchVel, restPitchSmoothTime);
			}
		}

		private bool TryGetRearLookAnchor(VehicleData data, out Vector3 worldPos, out Quaternion worldRot)
		{
			Transform transform = V.Car.transform;
			worldRot = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
			Powertrain getPowertrain = data.GetPowertrain;
			if ((bool)getPowertrain)
			{
				Bounds getWeightBounds = getPowertrain.GetWeightBounds;
				if (getWeightBounds.extents.z > 0.01f)
				{
					Vector3 position = new Vector3(getWeightBounds.center.x, getWeightBounds.center.y + getWeightBounds.extents.y * 0.55f, getWeightBounds.center.z - getWeightBounds.extents.z + 0.05f);
					worldPos = transform.TransformPoint(position);
					return true;
				}
			}
			CarChildsReferences carChildsReferences = data.CarChildsReferences;
			if ((bool)carChildsReferences)
			{
				Transform wheelRL = carChildsReferences.WheelRL;
				Transform wheelRR = carChildsReferences.WheelRR;
				if ((bool)wheelRL && (bool)wheelRR)
				{
					Vector3 vector = (wheelRL.position + wheelRR.position) * 0.5f;
					worldPos = vector - transform.forward * 0.6f + transform.up * 1f;
					return true;
				}
			}
			worldPos = transform.position - transform.forward * 2f + transform.up * 1f;
			return true;
		}

		protected override void OnDestroy()
		{
			_isDestroyed = true;
			_isCheckingAngle = false;
			CancelAndDisposeActiveCameraCts();
			UnsubscribeOnCameraLive();
			_onLookFreeHandle.Get(_eventRegistry)?.Unsubscribe(OnLookFree);
			base.OnDestroy();
			GlobalEventSystem.OnCarForcePositioned -= ForceRecenterCamera;
		}

		public override void SetActive(bool active, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			base.SetActive(active, target, onLiveCallback);
			CancelAndDisposeActiveCameraCts();
			UnsubscribeOnCameraLive();
			if (active)
			{
				_activeCameraCts = new CancellationTokenSource();
				CancellationToken token = _activeCameraCts.Token;
				if ((bool)_cinemachineVirtualCamera)
				{
					_cinemachineVirtualCamera.m_Lens.FieldOfView = ZoomFOV.Default;
					_povFovTarget = ZoomFOV.Default;
					_povFovVel = 0f;
				}
				ForceRecenterCamera(null);
				SubscribeForLiveRecenter(token);
				_isCheckingAngle = true;
				UpdateRotation(token).Forget();
				GlobalEventSystem.OnCarForcePositioned += ForceRecenterCamera;
				if (!_mainPerson)
				{
					_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
				}
				_switchHeadDisposable = Observable.Timer(TimeSpan.FromSeconds(base.VirtualCameraBlend.BlendTime)).Subscribe((long _) =>
				{
					_mainPerson.SwitchHeadVisible(visible: false);
				}).AddTo(this);
			}
			else
			{
				_isCheckingAngle = false;
				CancelAndDisposeActiveCameraCts();
				UnsubscribeOnCameraLive();
				GlobalEventSystem.OnCarForcePositioned -= ForceRecenterCamera;
				_switchHeadDisposable?.Dispose();
				_switchHeadDisposable = null;
				_mainPerson.SwitchHeadVisible(visible: true);
			}
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
			{
				if (active)
				{
					component.InteriorsComposition.ShowHighInterior();
				}
				else
				{
					component.InteriorsComposition.ShowLowInterior();
				}
			}
		}

		private void SubscribeForLiveRecenter(CancellationToken token)
		{
			if (!_cinemachineVirtualCamera)
			{
				return;
			}
			if ((bool)_cameraManager.CurrentBrain && _cameraManager.CurrentBrain.ActiveVirtualCamera is CinemachineVirtualCamera cinemachineVirtualCamera && cinemachineVirtualCamera == _cinemachineVirtualCamera)
			{
				ForceRecenterCamera(null);
				return;
			}
			_onCameraLiveAction = (ICinemachineCamera _, ICinemachineCamera _) =>
			{
				if (!_isActive || token.IsCancellationRequested)
				{
					UnsubscribeOnCameraLive();
				}
				else
				{
					ForceRecenterCamera(null);
					UnsubscribeOnCameraLive();
				}
			};
			_cinemachineVirtualCamera.m_Transitions.m_OnCameraLive.AddListener(_onCameraLiveAction);
		}

		private void CancelAndDisposeActiveCameraCts()
		{
			if (_activeCameraCts != null)
			{
				if (!_activeCameraCts.IsCancellationRequested)
				{
					_activeCameraCts.Cancel();
				}
				_activeCameraCts.Dispose();
				_activeCameraCts = null;
			}
		}

		private void UnsubscribeOnCameraLive()
		{
			if (_onCameraLiveAction != null && (bool)_cinemachineVirtualCamera)
			{
				_cinemachineVirtualCamera.m_Transitions.m_OnCameraLive.RemoveListener(_onCameraLiveAction);
				_onCameraLiveAction = null;
			}
		}

		private void ForceRecenterCamera(VehicleData vehicleData)
		{
			Vector3 eulerAngles = VirtualCamera.Follow.eulerAngles;
			_pov.m_HorizontalAxis.Value = Mathf.DeltaAngle(0f, eulerAngles.y);
			_pov.m_VerticalAxis.Value = Mathf.DeltaAngle(0f, eulerAngles.x);
			_pov.m_HorizontalRecentering.m_WaitTime = (_pov.m_VerticalRecentering.m_WaitTime = 0.01f);
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				_pov.m_HorizontalRecentering.m_WaitTime = _xRecenterDelay;
				_pov.m_VerticalRecentering.m_WaitTime = _yRecenterDelay;
			}).AddTo(this);
		}

		private async UniTaskVoid UpdateRotation(CancellationToken token)
		{
			Vector3 oldRotation = VirtualCamera.Follow.eulerAngles;
			while (_isCheckingAngle && !token.IsCancellationRequested && !_isDestroyed)
			{
				if ((bool)VirtualCamera && (bool)VirtualCamera.Follow)
				{
					Vector3 vector = VirtualCamera.Follow.eulerAngles - oldRotation;
					vector.x = Mathf.DeltaAngle(0f, vector.x);
					vector.y = Mathf.DeltaAngle(0f, vector.y);
					_pov.m_HorizontalAxis.Value += vector.y;
					_pov.m_VerticalAxis.Value += vector.x;
					oldRotation = VirtualCamera.Follow.eulerAngles;
				}
				await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
			}
		}

		public override void SetRecenter(bool isActive)
		{
			if (isActive)
			{
				_pov.m_HorizontalRecentering.m_enabled = _isXRecenter;
				_pov.m_VerticalRecentering.m_enabled = _isYRecenter;
			}
			else
			{
				_pov.m_HorizontalRecentering.m_enabled = false;
				_pov.m_VerticalRecentering.m_enabled = false;
			}
		}

		public override void CameraZoom(float delta)
		{
			if (ZoomFOV.IsEnabled && (bool)_cinemachineVirtualCamera)
			{
				if (_povFovTarget < 0f)
				{
					_povFovTarget = _cinemachineVirtualCamera.m_Lens.FieldOfView;
				}
				_povFovTarget = Mathf.Clamp(_povFovTarget + delta * ZoomFOV.Speed, ZoomFOV.Min, ZoomFOV.Max);
			}
			else
			{
				base.CameraZoom(delta);
			}
		}

		private bool IsHorizontalAngleWithinLimit()
		{
			if (!_pov.LookAtTarget)
			{
				return false;
			}
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			Vector3 normalized2 = Vector3.ProjectOnPlane(_pov.LookAtTarget.forward, Vector3.up).normalized;
			return Vector3.Angle(normalized, normalized2) <= MaxXAngle;
		}
	}
}
