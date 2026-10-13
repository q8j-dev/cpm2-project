using System;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;

namespace Racing.CameraManagement
{
	public abstract class CameraHandlerBase<T> : MonoBehaviour, ICameraHandler where T : CinemachineVirtualCameraBase
	{
		private enum LookDir
		{
			None = 0,
			Left = 1,
			Right = 2,
			Back = 3
		}

		[SerializeField]
		protected CinemachineVirtualCameraBase VirtualCamera;

		public float MinZoomDistance = -3f;

		public float MaxZoomDistance = 5f;

		[SerializeField]
		protected float ZoomSpeed = 0.1f;

		[SerializeField]
		protected bool IsZoomInverted = true;

		[SerializeField]
		protected bool IsZoomOrbitDependant;

		[SerializeField]
		private RacingCameraType RacingCameraType;

		[SerializeField]
		private CinemachineBlendDefinition BlendMode = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);

		public float _zoomDistance;

		protected CinemachineCameraOffset _cameraOffset;

		protected CameraManager _cameraManager;

		protected bool _isActive;

		protected bool _isXRecenter;

		protected bool _isYRecenter;

		protected T _camera;

		private UnityAction<ICinemachineCamera, ICinemachineCamera> _pendingOnLive;

		protected EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		protected SteeringWheelValueEvent _steeringWheelValueEvent;

		private InputDeviceDetector _inputDeviceDetector;

		protected DesktopControlsSettingsSO _controlsSettings;

		protected float _zoomMultiplier = 100f;

		protected const float MinLookFactor = 0.15f;

		protected RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

		protected RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

		protected RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

		protected RegistryHandle<BoolEvent> _onLookFreeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLookFree);

		protected RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

		private RegistryHandle<BoolEvent> _lookLeftHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookLeft);

		private RegistryHandle<BoolEvent> _lookRightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookRight);

		private RegistryHandle<BoolEvent> _lookBackHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookBack);

		private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

		protected bool _lookOverrideActive;

		private Tween _lookOverrideTween;

		private float _snapYaw;

		private bool _isLookBackInstant;

		private LookDir _activeLookDir;

		private bool _isLookFree;

		private const float LookTransitionTime = 0.18f;

		private static readonly HashSet<RacingCameraType> DrivingCamTypes = new HashSet<RacingCameraType>
		{
			RacingCameraType.ForwardView,
			RacingCameraType.InCar
		};

		private CancellationTokenSource _debugLoopCts;

		private const float DebugYawTarget = 360f;

		private const int DebugMaxFrames = 600;

		protected bool IsCircleMenuGroupActive
		{
			get
			{
				if (_inputMapSetEvent != null)
				{
					InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
					return currentInputGroup == InputGroupsEnum.CarCircleMenu || currentInputGroup == InputGroupsEnum.PersonCircleMenu || currentInputGroup == InputGroupsEnum.CarFreeRaceCircleMenu || currentInputGroup == InputGroupsEnum.CarPassengerCircleMenu;
				}
				return false;
			}
		}

		protected virtual Vector2 _multiplier => new Vector2(0.036f, 0.000156f);

		protected virtual CameraSettingsContext SettingsContext => CameraSettingsContext.None;

		protected virtual bool UseUngatedLook => false;

		protected bool IsDrivingCam => DrivingCamTypes.Contains(VirtualCameraType);

		private bool WheelConnected
		{
			get
			{
				if ((bool)_steeringWheelValueEvent)
				{
					return _steeringWheelValueEvent.StoredValue;
				}
				return false;
			}
		}

		protected virtual float CurrentYaw
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		protected virtual float LookLeftYaw => -90f;

		protected virtual float LookRightYaw => 90f;

		protected virtual float LookForwardYaw => 0f;

		protected virtual float LookBackYaw => 180f;

		protected virtual float DebugTrackedYaw
		{
			get
			{
				if (!VirtualCamera)
				{
					return 0f;
				}
				return VirtualCamera.transform.eulerAngles.y;
			}
		}

		protected virtual bool DebugIsPitchAtBottom => false;

		protected virtual Vector2 DebugFullCircleSimulatedInput => new Vector2(-15f, 0f);

		protected virtual Vector2 DebugTopToBottomSimulatedInput => new Vector2(0f, -15f);

		protected virtual float GamepadLookScale => 1f;

		private bool IsGamepadActive
		{
			get
			{
				if (_inputDeviceDetector != null)
				{
					return _inputDeviceDetector.CurrentDeviceType != InputDeviceType.KeyboardMouse;
				}
				return false;
			}
		}

		public float ZoomDistance => _zoomDistance;

		public RacingCameraType VirtualCameraType => RacingCameraType;

		public CinemachineBlendDefinition VirtualCameraBlend => BlendMode;

		public abstract ref CinemachineVirtualCameraBase.TransitionParams TransitionParams { get; }

		public virtual bool ChangeTargetOnActivate { get; } = true;

		protected bool IsLiveOnBrain
		{
			get
			{
				if (!_isActive)
				{
					return false;
				}
				CinemachineBrain cinemachineBrain = (_cameraManager ? _cameraManager.CurrentBrain : null);
				if (!cinemachineBrain)
				{
					return _isActive;
				}
				ICinemachineCamera activeVirtualCamera = cinemachineBrain.ActiveVirtualCamera;
				if (activeVirtualCamera == null)
				{
					return false;
				}
				if (activeVirtualCamera == _camera)
				{
					return true;
				}
				if (cinemachineBrain.IsBlending && cinemachineBrain.ActiveBlend != null)
				{
					return cinemachineBrain.ActiveBlend.CamB == _camera;
				}
				return false;
			}
		}

		protected virtual void OnLookBackInstantStart()
		{
		}

		protected virtual void OnLookBackInstantEnd()
		{
		}

		protected virtual void OnLookOverrideStart()
		{
		}

		protected virtual void OnLookOverrideEnd()
		{
		}

		protected virtual void DebugMoveToPitchTop()
		{
		}

		[Inject]
		public void ConstructControlsSettings(CameraManager cameraManager, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, SteeringWheelValueEvent steeringWheelValueEvent, OptionalDependency<InputDeviceDetector> inputDeviceDetectorDep, OptionalDependency<DesktopControlsSettingsSO> desktopControlsSettingsSOdep)
		{
			_cameraManager = cameraManager;
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_steeringWheelValueEvent = steeringWheelValueEvent;
			if (inputDeviceDetectorDep.HasValue)
			{
				_inputDeviceDetector = inputDeviceDetectorDep.Value;
			}
			if (desktopControlsSettingsSOdep.HasValue)
			{
				_controlsSettings = desktopControlsSettingsSOdep.Value;
			}
		}

		protected virtual void Awake()
		{
			if (VirtualCamera == null)
			{
				VirtualCamera = GetComponent<CinemachineVirtualCameraBase>();
			}
			_camera = VirtualCamera as T;
			_cameraOffset = VirtualCamera.GetComponent<CinemachineCameraOffset>();
			if (IsZoomOrbitDependant)
			{
				UnityEngine.Object.Destroy(_cameraOffset);
				_cameraOffset = null;
			}
			_cameraManager.RegisterCamera(this);
			_lookLeftHandle.Get(_eventRegistry).Subscribe(OnLookLeft);
			_lookRightHandle.Get(_eventRegistry).Subscribe(OnLookRight);
			_lookBackHandle.Get(_eventRegistry).Subscribe(OnLookBack);
			_changeCameraHandle.Get(_eventRegistry).Subscribe(OnChangeCamera);
			_inputMapSetEvent.Subscribe(OnInputGroupChanged);
		}

		protected virtual void OnDestroy()
		{
			ClearPendingOnLive();
			if (_cameraManager != null)
			{
				_cameraManager.UnregisterCamera(this);
			}
			_debugLoopCts?.Cancel();
			_debugLoopCts?.Dispose();
			_debugLoopCts = null;
			_lookLeftHandle.Get(_eventRegistry)?.Unsubscribe(OnLookLeft);
			_lookRightHandle.Get(_eventRegistry)?.Unsubscribe(OnLookRight);
			_lookBackHandle.Get(_eventRegistry)?.Unsubscribe(OnLookBack);
			_changeCameraHandle.Get(_eventRegistry)?.Unsubscribe(OnChangeCamera);
			_inputMapSetEvent?.Unsubscribe(OnInputGroupChanged);
			_lookOverrideTween?.Kill();
		}

		private void ClearPendingOnLive()
		{
			if (_pendingOnLive != null)
			{
				if ((bool)VirtualCamera)
				{
					TransitionParams.m_OnCameraLive.RemoveListener(_pendingOnLive);
				}
				_pendingOnLive = null;
			}
		}

		public virtual void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			_isActive = isActive;
			ClearPendingOnLive();
			if (isActive && onLiveCallback != null)
			{
				if ((bool)_cameraManager.CurrentBrain && _cameraManager.CurrentBrain.ActiveVirtualCamera is T val && val == _camera)
				{
					onLiveCallback(_camera);
				}
				else
				{
					UnityAction<ICinemachineCamera, ICinemachineCamera> eventAction = null;
					eventAction = (ICinemachineCamera _, ICinemachineCamera _) =>
					{
						TransitionParams.m_OnCameraLive.RemoveListener(eventAction);
						if (_pendingOnLive == eventAction)
						{
							_pendingOnLive = null;
						}
						onLiveCallback(_camera);
					};
					_pendingOnLive = eventAction;
					TransitionParams.m_OnCameraLive.AddListener(eventAction);
				}
			}
			if (isActive)
			{
				if (ChangeTargetOnActivate)
				{
					CinemachineVirtualCameraBase virtualCamera = VirtualCamera;
					Transform lookAt = (VirtualCamera.Follow = (target ? target : V.Car.transform));
					virtualCamera.LookAt = lookAt;
				}
				VirtualCamera.m_Priority = 10;
				return;
			}
			if ((bool)_cameraOffset)
			{
				_cameraOffset.m_Offset.z = 0f;
				_zoomDistance = 0f;
			}
			VirtualCamera.m_Priority = 0;
			if (ChangeTargetOnActivate && !CinemachineCore.Instance.IsLive(VirtualCamera))
			{
				VirtualCamera.LookAt = null;
				VirtualCamera.Follow = null;
			}
		}

		public void ChangeTarget(Transform target)
		{
			if (ChangeTargetOnActivate)
			{
				VirtualCamera.Follow = target;
				VirtualCamera.LookAt = target;
			}
		}

		protected virtual void ApplyAlterSettings()
		{
			if (_camera.TryGetComponent<CinemachineFreeLook>(out var component))
			{
				component.m_XAxis.m_AccelTime = 0f;
				component.m_XAxis.m_DecelTime = 0f;
				component.m_XAxis.m_MaxSpeed = 0f;
				component.m_XAxis.m_InputAxisName = "";
				component.m_YAxis.m_AccelTime = 0f;
				component.m_YAxis.m_DecelTime = 0f;
				component.m_YAxis.m_MaxSpeed = 0f;
				component.m_YAxis.m_InputAxisName = "";
				for (int i = 0; i < 3; i++)
				{
					CinemachineVirtualCamera rig = component.GetRig(i);
					if (!(rig == null))
					{
						CinemachineOrbitalTransposer cinemachineComponent = rig.GetCinemachineComponent<CinemachineOrbitalTransposer>();
						if (cinemachineComponent != null)
						{
							cinemachineComponent.m_XDamping = 0f;
							cinemachineComponent.m_YDamping = 0f;
							cinemachineComponent.m_ZDamping = 0f;
						}
						CinemachineComposer cinemachineComponent2 = rig.GetCinemachineComponent<CinemachineComposer>();
						if (cinemachineComponent2 != null)
						{
							cinemachineComponent2.m_HorizontalDamping = 0f;
							cinemachineComponent2.m_VerticalDamping = 0f;
							cinemachineComponent2.m_LookaheadTime = 0f;
							cinemachineComponent2.m_LookaheadSmoothing = 0f;
							cinemachineComponent2.m_DeadZoneWidth = 0f;
							cinemachineComponent2.m_DeadZoneHeight = 0f;
						}
					}
				}
			}
			if (_camera.TryGetComponent<CinemachineVirtualCamera>(out var component2))
			{
				CinemachineTransposer cinemachineComponent3 = component2.GetCinemachineComponent<CinemachineTransposer>();
				if (cinemachineComponent3 != null)
				{
					cinemachineComponent3.m_XDamping = 0f;
					cinemachineComponent3.m_YDamping = 0f;
					cinemachineComponent3.m_ZDamping = 0f;
				}
				CinemachineFramingTransposer cinemachineComponent4 = component2.GetCinemachineComponent<CinemachineFramingTransposer>();
				if (cinemachineComponent4 != null)
				{
					cinemachineComponent4.m_XDamping = 0f;
					cinemachineComponent4.m_YDamping = 0f;
					cinemachineComponent4.m_ZDamping = 0f;
					cinemachineComponent4.m_LookaheadTime = 0f;
					cinemachineComponent4.m_LookaheadSmoothing = 0f;
				}
				CinemachineComposer cinemachineComponent5 = component2.GetCinemachineComponent<CinemachineComposer>();
				if (cinemachineComponent5 != null)
				{
					cinemachineComponent5.m_HorizontalDamping = 0f;
					cinemachineComponent5.m_VerticalDamping = 0f;
					cinemachineComponent5.m_LookaheadTime = 0f;
					cinemachineComponent5.m_LookaheadSmoothing = 0f;
				}
			}
		}

		protected void UpdateZoom()
		{
			float num = _zoomHandle.Get(_eventRegistry).Value * _zoomMultiplier;
			if (num.Abs() > 1E-05f)
			{
				CameraZoom(0f - num);
			}
		}

		protected void UpdateLook()
		{
			if (IsCircleMenuGroupActive)
			{
				return;
			}
			Vector2 b = Vector2.zero;
			if (UseUngatedLook)
			{
				if (_isLookFree)
				{
					b = _lookFreeHandle.Get(_eventRegistry).Value;
				}
			}
			else
			{
				b = _lookHandle.Get(_eventRegistry).Value;
			}
			float value = (_zoomDistance - MinZoomDistance) / (MaxZoomDistance - MinZoomDistance);
			b = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value)) * Vector2.Scale(_multiplier, b);
			if (IsGamepadActive)
			{
				b *= GamepadLookScale;
			}
			if ((bool)_controlsSettings)
			{
				Vector2 lookScale = _controlsSettings.GetLookScale(SettingsContext);
				b.x *= lookScale.x;
				b.y *= lookScale.y;
			}
			if (b.sqrMagnitude > 1E-05f)
			{
				ApplyLook(b);
			}
		}

		protected virtual void ApplyLook(Vector2 look)
		{
		}

		protected virtual void OnLookFree(bool start)
		{
			_isLookFree = start;
		}

		protected virtual void OnLookLeft(bool start)
		{
			HandleLookDir(start, LookDir.Left, LookLeftYaw);
		}

		protected virtual void OnLookRight(bool start)
		{
			HandleLookDir(start, LookDir.Right, LookRightYaw);
		}

		protected virtual void OnLookBack(bool start)
		{
			HandleLookDir(start, LookDir.Back, LookBackYaw);
		}

		private void HandleLookDir(bool start, LookDir dir, float yaw)
		{
			if (IsDrivingCam && IsLiveOnBrain)
			{
				if (start)
				{
					BeginLook(dir, yaw);
				}
				else
				{
					EndLook(dir);
				}
			}
		}

		private void BeginLook(LookDir dir, float yaw)
		{
			_lookOverrideTween?.Kill();
			if (!_lookOverrideActive)
			{
				_snapYaw = CurrentYaw;
				_lookOverrideActive = true;
				OnLookOverrideStart();
			}
			if (_isLookBackInstant && dir != LookDir.Back)
			{
				OnLookBackInstantEnd();
				_isLookBackInstant = false;
			}
			_activeLookDir = dir;
			if (dir == LookDir.Back)
			{
				_isLookBackInstant = true;
				OnLookBackInstantStart();
				return;
			}
			float endValue = CurrentYaw + Mathf.DeltaAngle(CurrentYaw, yaw);
			_lookOverrideTween = DOTween.To(() => CurrentYaw, (float v) =>
			{
				CurrentYaw = v;
			}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true);
		}

		private void EndLook(LookDir dir)
		{
			if (_activeLookDir != dir)
			{
				return;
			}
			_lookOverrideTween?.Kill();
			_activeLookDir = LookDir.None;
			if (_isLookBackInstant)
			{
				OnLookBackInstantEnd();
				CurrentYaw = (WheelConnected ? LookForwardYaw : _snapYaw);
				_isLookBackInstant = false;
				_lookOverrideActive = false;
				OnLookOverrideEnd();
				return;
			}
			float endValue = CurrentYaw + Mathf.DeltaAngle(CurrentYaw, LookForwardYaw);
			_lookOverrideTween = DOTween.To(() => CurrentYaw, (float v) =>
			{
				CurrentYaw = v;
			}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
			{
				_lookOverrideActive = false;
				OnLookOverrideEnd();
			});
		}

		private void ForceEndLook()
		{
			if (_lookOverrideActive)
			{
				_lookOverrideTween?.Kill();
				if (_isLookBackInstant)
				{
					OnLookBackInstantEnd();
					CurrentYaw = _snapYaw;
					_isLookBackInstant = false;
				}
				_activeLookDir = LookDir.None;
				_lookOverrideActive = false;
				OnLookOverrideEnd();
			}
		}

		private void OnChangeCamera()
		{
			ForceEndLook();
		}

		private void OnInputGroupChanged(InputGroupsEnum _)
		{
			if (_lookOverrideActive)
			{
				if (_isLookBackInstant || !IsLiveOnBrain)
				{
					ForceEndLook();
				}
				else
				{
					EndLook(_activeLookDir);
				}
			}
		}

		protected void HandleLookEvent(bool start, float yaw)
		{
			if (!IsDrivingCam || !IsLiveOnBrain)
			{
				return;
			}
			_lookOverrideTween?.Kill();
			if (start)
			{
				if (!_lookOverrideActive)
				{
					_snapYaw = CurrentYaw;
					_lookOverrideActive = true;
					OnLookOverrideStart();
				}
				float endValue = CurrentYaw + Mathf.DeltaAngle(CurrentYaw, yaw);
				_lookOverrideTween = DOTween.To(() => CurrentYaw, (float v) =>
				{
					CurrentYaw = v;
				}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
				{
				});
			}
			else
			{
				float endValue2 = CurrentYaw + Mathf.DeltaAngle(CurrentYaw, LookForwardYaw);
				_lookOverrideTween = DOTween.To(() => CurrentYaw, (float v) =>
				{
					CurrentYaw = v;
				}, endValue2, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
				{
					_lookOverrideActive = false;
					OnLookOverrideEnd();
				});
			}
		}

		public void DebugRotateFullCircle()
		{
			RestartDebugLoop(DebugFullCircleLoop);
		}

		public void DebugRotateTopToBottom()
		{
			RestartDebugLoop(DebugTopToBottomLoop);
		}

		private void RestartDebugLoop(Func<CancellationToken, UniTask> loop)
		{
			_debugLoopCts?.Cancel();
			_debugLoopCts?.Dispose();
			_debugLoopCts = new CancellationTokenSource();
			loop(_debugLoopCts.Token).Forget();
		}

		private async UniTask DebugFullCircleLoop(CancellationToken ct)
		{
			float prevYaw = DebugTrackedYaw;
			float accumulated = 0f;
			int frames = 0;
			_ = Time.realtimeSinceStartupAsDouble;
			Vector2 input = Vector2.Scale(b: DebugFullCircleSimulatedInput, a: _multiplier);
			while (accumulated < 360f && frames < 600 && !ct.IsCancellationRequested)
			{
				ApplyLook(input);
				await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
				float debugTrackedYaw = DebugTrackedYaw;
				accumulated += Mathf.Abs(Mathf.DeltaAngle(prevYaw, debugTrackedYaw));
				prevYaw = debugTrackedYaw;
				frames++;
			}
			if (!ct.IsCancellationRequested)
			{
				_ = Time.realtimeSinceStartupAsDouble;
				_ = accumulated;
				_ = 360f;
			}
		}

		private async UniTask DebugTopToBottomLoop(CancellationToken ct)
		{
			DebugMoveToPitchTop();
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
			int frames = 0;
			_ = Time.realtimeSinceStartupAsDouble;
			Vector2 input = DebugTopToBottomSimulatedInput;
			input = Time.deltaTime * Vector2.Scale(_multiplier, input);
			while (!DebugIsPitchAtBottom && frames < 600 && !ct.IsCancellationRequested)
			{
				ApplyLook(input);
				await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
				frames++;
			}
			if (!ct.IsCancellationRequested)
			{
				_ = Time.realtimeSinceStartupAsDouble;
				_ = DebugIsPitchAtBottom;
			}
		}

		public abstract void SetRecenter(bool isActive);

		public void CameraZoom()
		{
		}

		public virtual void CameraZoom(float delta)
		{
			float num = delta * ZoomSpeed * 0.1f;
			_zoomDistance = Mathf.Clamp(_zoomDistance + num, MinZoomDistance, MaxZoomDistance);
			if ((bool)_cameraOffset)
			{
				_cameraOffset.m_Offset.z = _zoomDistance;
			}
		}

		public virtual void SetZoomValue(float value)
		{
			_zoomDistance = Mathf.Clamp(value, MinZoomDistance, MaxZoomDistance);
			if ((bool)_cameraOffset)
			{
				_cameraOffset.m_Offset.z = _zoomDistance;
			}
		}
	}
}
