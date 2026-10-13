using CPMEngine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;

public class JoyStickControl : MonoBehaviour
{
	private enum LookMode
	{
		None = 0,
		ChaseCam = 1,
		Pov = 2,
		PovBack = 3
	}

	private enum LookArrow
	{
		None = 0,
		Left = 1,
		Right = 2,
		Back = 3
	}

	public const float FOVMin = 15f;

	public const float FOVMax = 80f;

	public const float HeightMin = 0f;

	public const float HeightMax = 30f;

	private const float LerpPowerPitchToDefault = 2.25f;

	public GameObject fieldOfView;

	public float perspectiveZoomSpeed = 50f;

	public float speedMove = 1f;

	public CarCamerasController carCamerasController;

	public float timeCheckDoubleTouch;

	[SerializeField]
	private float lookSensitivity = 0.01f;

	[SerializeField]
	private float zoomSensitivity = 50f;

	private CarCameras _carCameras;

	private Camera _cam;

	private GameObject _cachedCar;

	private float _xAngTemp;

	private float _yAngTemp;

	private float _xAngTemp2;

	private float _yAngTemp2;

	private float _joyStickX;

	private float _joyStickY;

	private Vector3 _firstpoint;

	private Vector3 _secondPoint;

	private Vector3 _lastMousePosition;

	[SerializeField]
	private float ScrollZoomStep = 100f;

	private bool _isZooming;

	private bool _isUITouched;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	private CarCameras _carCameras1;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	[SerializeField]
	private float zoomFovGain = 12f;

	[SerializeField]
	private float zoomFovSmoothTime = 0.05f;

	private float _zoomFovTarget = -1f;

	private float _zoomFovVel;

	private bool _lastSpeedFov;

	protected const float MinLookFactor = 0.15f;

	private const float LookRefFov = 50f;

	private const float LookRefScalar = 0.54f;

	private static readonly Vector2 FpvLookSpeedMouse = new Vector2(0.38f, 0.32f);

	private static readonly Vector2 FpvLookSpeedGamepad = new Vector2(0.2f, 0.08f);

	private DesktopControlsSettingsSO _controlsSettings;

	private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

	private RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

	private RegistryHandle<BoolEvent> _onLookFreeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLookFree);

	private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

	private RegistryHandle<BoolEvent> _lookLeftHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookLeft);

	private RegistryHandle<BoolEvent> _lookRightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookRight);

	private RegistryHandle<BoolEvent> _lookBackHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookBack);

	private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

	[Inject]
	private SteeringWheelValueEvent _steeringWheelValueEvent;

	private LookMode _lookActiveMode;

	private LookArrow _activeLookArrow;

	private bool _lookOverrideActive;

	private float _lookOverrideCurrentYaw;

	private Tween _lookOverrideTween;

	private float _snapYawAngle;

	private float _directionLookIdleTime;

	private float _directionYawVel;

	private float _directionHeightVel;

	private float _fpvLookIdleTime;

	private float _fpvPitchVel;

	private float _fpvYawVel;

	private bool _fpvWasDriverView;

	private Vector3 _snapPovEuler;

	private Vector3 _snapPovLocalPosition;

	private Transform _rearTransform;

	private DriverController _povBackDriver;

	private const float LookTransitionTime = 0.18f;

	private const float DirectionRecenterSmoothTime = 0.3f;

	private const float DirectionRecenterDelay = 1f;

	private const float RecenterMinSpeedKmh = 3f;

	private const float SpeedXNorm = 1f;

	private const float SpeedYNorm = 1f;

	private const float RearHeightFactor = 0.55f;

	private const float RearClearance = 0.05f;

	private const float RearOverhang = 0.6f;

	private const float RearHeightAboveAxle = 1f;

	public GameObject CachedCar
	{
		get
		{
			if (!_cachedCar)
			{
				return V.Car;
			}
			return _cachedCar;
		}
		set
		{
			_cachedCar = value;
		}
	}

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

	[Inject]
	public void ConstructControlsSettings(OptionalDependency<DesktopControlsSettingsSO> settings)
	{
		if (settings != null && settings.HasValue)
		{
			_controlsSettings = settings.Value;
		}
	}

	private bool IsCarMoving()
	{
		if (!CachedCar || !CachedCar.TryGetComponent<VehicleData>(out var component))
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

	[Inject]
	public void Construct(CoreGameManager coreGameManager, EventRegistry eventRegistry, MenuLevelParent menuLevelParent, InputMapSetEvent inputMapSetEvent)
	{
		_menuLevelParent = menuLevelParent;
		_coreGameManager = coreGameManager;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void Start()
	{
		if ((bool)_menuLevelParent)
		{
			_menuLevelParent.JoyStickControl = this;
		}
		float num = Screen.width;
		perspectiveZoomSpeed = perspectiveZoomSpeed * 1500f / num;
		speedMove = 5000f / num;
		_carCameras = _coreGameManager.carCameras;
		_cam = Camera.main;
		carCamerasController = _coreGameManager.carCamerasController;
		if ((bool)fieldOfView)
		{
			fieldOfView.SetActive(value: false);
		}
		SubscribeLookDirection();
	}

	private void ToggleRotation(bool isStarted)
	{
		if (!isStarted)
		{
			_joyStickX = 0f;
			_joyStickY = 0f;
		}
	}

	private void OnDestroy()
	{
		UnsubscribeLookDirection();
	}

	public void ResetPoints()
	{
		_firstpoint = Vector3.zero;
		_secondPoint = Vector3.zero;
	}

	private void Update()
	{
		if (!_menuLevelParent.isDroneEnabled && (bool)V.Car && FlagsStorage.IsEnabledJoy && Time.timeScale > 0f)
		{
			NewUpdateMethod();
		}
	}

	private void NewUpdateMethod()
	{
		float value = _zoomHandle.Get(_eventRegistry).Value;
		bool flag = (bool)_carCameras && _carCameras.SpeedFovControlsFov;
		float num = (flag ? _carCameras.ZoomFov : _cam.fieldOfView);
		if (_zoomFovTarget < 0f || flag != _lastSpeedFov)
		{
			_zoomFovTarget = num;
			_zoomFovVel = 0f;
		}
		_lastSpeedFov = flag;
		if (Mathf.Abs(value) > 1E-05f)
		{
			_zoomFovTarget = Mathf.Clamp(_zoomFovTarget - value * zoomFovGain, 15f, 80f);
		}
		float zoomFov = Mathf.Clamp(Mathf.SmoothDamp(num, _zoomFovTarget, ref _zoomFovVel, zoomFovSmoothTime), 15f, 80f);
		if (flag)
		{
			_carCameras.ZoomFov = zoomFov;
		}
		else
		{
			_cam.fieldOfView = zoomFov;
		}
		Vector2 vector = _lookFreeHandle.Get(_eventRegistry)?.Value ?? Vector2.zero;
		if ((bool)_carCameras && _carCameras.SteamLookMode == CarCameras.SteamLook.Default && V.CurrentCamIndex == 2 && !_lookOverrideActive && !FlagsStorage.MovingInsideCamPos)
		{
			if (vector != Vector2.zero)
			{
				_directionLookIdleTime = 0f;
				_directionYawVel = 0f;
				_directionHeightVel = 0f;
			}
			else
			{
				_directionLookIdleTime += Time.deltaTime;
			}
			if (_directionLookIdleTime >= 1f)
			{
				_carCameras.yawAngle = Mathf.SmoothDampAngle(_carCameras.yawAngle, 0f, ref _directionYawVel, 0.3f);
				if (_carCameras.DefaultHeight > 0.01f)
				{
					_carCameras.height = Mathf.SmoothDamp(_carCameras.height, _carCameras.DefaultHeight, ref _directionHeightVel, 0.3f);
					_carCameras.pitchAngle = 0f - _carCameras.height + _carCameras.pitchAngleOffset;
				}
			}
		}
		if ((bool)_carCameras && _carCameras.driverView && (bool)V.Car && !_lookOverrideActive && CachedCar.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
		{
			Transform transform = component.camInsideView.transform;
			if (!_fpvWasDriverView)
			{
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.x = _carCameras.FpvDefaultPitch;
				transform.localEulerAngles = localEulerAngles;
				_fpvWasDriverView = true;
			}
			if (vector != Vector2.zero)
			{
				_fpvLookIdleTime = 0f;
				_fpvPitchVel = 0f;
				_fpvYawVel = 0f;
			}
			else if (!IsCarMoving())
			{
				_fpvLookIdleTime = 0f;
				_fpvPitchVel = 0f;
				_fpvYawVel = 0f;
			}
			else
			{
				_fpvLookIdleTime += Time.deltaTime;
				if (_fpvLookIdleTime >= 1f)
				{
					Vector3 localEulerAngles2 = transform.localEulerAngles;
					localEulerAngles2.x = Mathf.SmoothDampAngle(localEulerAngles2.x, _carCameras.FpvDefaultPitch, ref _fpvPitchVel, 0.3f);
					localEulerAngles2.y = Mathf.SmoothDampAngle(localEulerAngles2.y, 0f, ref _fpvYawVel, 0.3f);
					transform.localEulerAngles = localEulerAngles2;
				}
			}
		}
		else if ((bool)_carCameras && !_carCameras.driverView)
		{
			_fpvWasDriverView = false;
		}
		if (vector == Vector2.zero)
		{
			return;
		}
		float num2;
		if (_carCameras.driverView)
		{
			num2 = 0.54f * (_cam.fieldOfView / 50f);
		}
		else
		{
			float value2 = (_cam.fieldOfView - 15f) / 65f;
			num2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value2));
		}
		CameraSettingsContext ctx = ((!_carCameras.driverView) ? CameraSettingsContext.Car : CameraSettingsContext.CarFpv);
		Vector2 lookScale = _controlsSettings.GetLookScale(ctx);
		vector = num2 * Vector2.Scale(lookScale, vector);
		_joyStickX = vector.x;
		_joyStickY = 0f - vector.y;
		int num3;
		float x;
		if (_carCameras.driverView)
		{
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance != null)
			{
				num3 = ((instance.CurrentDeviceType != InputDeviceType.KeyboardMouse) ? 1 : 0);
				if (num3 != 0)
				{
					x = FpvLookSpeedGamepad.x;
					goto IL_04d3;
				}
			}
			else
			{
				num3 = 0;
			}
			x = FpvLookSpeedMouse.x;
			goto IL_04d3;
		}
		goto IL_051b;
		IL_051b:
		if (!FlagsStorage.MovingInsideCamPos && !_lookOverrideActive)
		{
			ChangeCamWithJoyStick();
		}
		return;
		IL_04d3:
		float num4 = x;
		float num5 = ((num3 != 0) ? FpvLookSpeedGamepad.y : FpvLookSpeedMouse.y);
		_joyStickX *= speedMove * num4;
		_joyStickY *= speedMove * num5;
		goto IL_051b;
	}

	private void SubscribeLookDirection()
	{
		_onLookHandle.Get(_eventRegistry).Subscribe(ToggleRotation);
		_onLookFreeHandle.Get(_eventRegistry).Subscribe(ToggleRotation);
		_lookLeftHandle.Get(_eventRegistry).Subscribe(OnLookLeft);
		_lookRightHandle.Get(_eventRegistry).Subscribe(OnLookRight);
		_lookBackHandle.Get(_eventRegistry).Subscribe(OnLookBack);
		_changeCameraHandle.Get(_eventRegistry).Subscribe(OnChangeCamera);
		_inputMapSetEvent.Subscribe(OnInputGroupChanged);
	}

	private void UnsubscribeLookDirection()
	{
		_onLookFreeHandle.Get(_eventRegistry)?.Unsubscribe(ToggleRotation);
		_onLookHandle.Get(_eventRegistry)?.Unsubscribe(ToggleRotation);
		_lookLeftHandle.Get(_eventRegistry)?.Unsubscribe(OnLookLeft);
		_lookRightHandle.Get(_eventRegistry)?.Unsubscribe(OnLookRight);
		_lookBackHandle.Get(_eventRegistry)?.Unsubscribe(OnLookBack);
		_changeCameraHandle.Get(_eventRegistry)?.Unsubscribe(OnChangeCamera);
		_inputMapSetEvent?.Unsubscribe(OnInputGroupChanged);
		_lookOverrideTween?.Kill();
		ClearPovBackSuppression();
	}

	private void OnLookLeft(bool s)
	{
		HandleArrow(s, LookArrow.Left, -90f);
	}

	private void OnLookRight(bool s)
	{
		HandleArrow(s, LookArrow.Right, 90f);
	}

	private void OnLookBack(bool s)
	{
		HandleArrow(s, LookArrow.Back, 180f);
	}

	private void HandleArrow(bool start, LookArrow arrow, float yaw)
	{
		if (start)
		{
			BeginArrow(arrow, yaw);
		}
		else
		{
			EndArrow(arrow);
		}
	}

	private void BeginArrow(LookArrow arrow, float yaw)
	{
		if (_lookActiveMode == LookMode.PovBack && arrow != LookArrow.Back)
		{
			EndLookBackPovTeleport();
		}
		_activeLookArrow = arrow;
		if (arrow == LookArrow.Back && _carCameras != null && _carCameras.driverView)
		{
			StartLookBackPovTeleport();
		}
		else
		{
			StartLookOverride(yaw);
		}
	}

	private void EndArrow(LookArrow arrow)
	{
		if (_activeLookArrow == arrow)
		{
			_activeLookArrow = LookArrow.None;
			if (_lookActiveMode == LookMode.PovBack)
			{
				EndLookBackPovTeleport();
			}
			else
			{
				EndLookOverride();
			}
		}
	}

	private bool TryGetRearLookAnchor(VehicleData data, out Vector3 worldPos, out Quaternion worldRot)
	{
		Transform transform = CachedCar.transform;
		worldRot = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
		Powertrain getPowertrain = data.GetPowertrain;
		if (getPowertrain != null)
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
		if (carChildsReferences != null)
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

	private void StartLookBackPovTeleport()
	{
		if (FlagsStorage.FixedCameraAndLookAt || !V.Car || !CachedCar.TryGetComponent<DriverController>(out var component) || component.camInsideView == null || !CachedCar.TryGetComponent<VehicleData>(out var component2))
		{
			return;
		}
		_rearTransform = CachedCar.transform;
		if (TryGetRearLookAnchor(component2, out var worldPos, out var worldRot))
		{
			if (!_lookOverrideActive)
			{
				_snapPovEuler = component.camInsideView.transform.localEulerAngles;
				_snapPovLocalPosition = component.camInsideView.transform.localPosition;
				_lookOverrideActive = true;
			}
			else if (_lookActiveMode == LookMode.Pov)
			{
				_lookOverrideTween?.Kill();
				_snapPovLocalPosition = component.camInsideView.transform.localPosition;
			}
			_lookActiveMode = LookMode.PovBack;
			component.SuppressInsideViewReturn = true;
			_povBackDriver = component;
			component.camInsideView.transform.SetPositionAndRotation(worldPos, worldRot);
		}
	}

	private void ClearPovBackSuppression()
	{
		if ((bool)_povBackDriver)
		{
			_povBackDriver.SuppressInsideViewReturn = false;
		}
		_povBackDriver = null;
	}

	private void EndLookBackPovTeleport()
	{
		if (!_lookOverrideActive || _lookActiveMode != LookMode.PovBack)
		{
			return;
		}
		_lookOverrideTween?.Kill();
		ClearPovBackSuppression();
		if ((bool)V.Car)
		{
			if (CachedCar.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
			{
				component.camInsideView.transform.localPosition = _snapPovLocalPosition;
				component.camInsideView.transform.localEulerAngles = _snapPovEuler;
			}
			_lookOverrideActive = false;
			_lookActiveMode = LookMode.None;
			_rearTransform = null;
		}
	}

	private void StartLookOverride(float targetYawCarRelative)
	{
		if (FlagsStorage.FixedCameraAndLookAt || (_carCameras != null && _carCameras.SteamLookMode == CarCameras.SteamLook.Fixed) || !V.Car)
		{
			return;
		}
		_lookOverrideTween?.Kill();
		if (!_lookOverrideActive)
		{
			if (_carCameras.driverView)
			{
				if (!CachedCar.TryGetComponent<DriverController>(out var component) || component.camInsideView == null)
				{
					return;
				}
				_lookActiveMode = LookMode.Pov;
				_snapPovEuler = component.camInsideView.transform.localEulerAngles;
				_lookOverrideCurrentYaw = _snapPovEuler.y;
			}
			else
			{
				_lookActiveMode = LookMode.ChaseCam;
				_snapYawAngle = _carCameras.yawAngle;
				_lookOverrideCurrentYaw = _snapYawAngle;
			}
			_lookOverrideActive = true;
		}
		float endValue = _lookOverrideCurrentYaw + Mathf.DeltaAngle(_lookOverrideCurrentYaw, targetYawCarRelative);
		_lookOverrideTween = DOTween.To(() => _lookOverrideCurrentYaw, (float v) =>
		{
			_lookOverrideCurrentYaw = v;
		}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
		});
	}

	private void EndLookOverride()
	{
		if (_lookOverrideActive)
		{
			_lookOverrideTween?.Kill();
			float target = ((_lookActiveMode == LookMode.Pov) ? _snapPovEuler.y : (WheelConnected ? 0f : _snapYawAngle));
			float endValue = _lookOverrideCurrentYaw + Mathf.DeltaAngle(_lookOverrideCurrentYaw, target);
			_lookOverrideTween = DOTween.To(() => _lookOverrideCurrentYaw, (float v) =>
			{
				_lookOverrideCurrentYaw = v;
			}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
			{
				_lookOverrideActive = false;
				_lookActiveMode = LookMode.None;
			});
		}
	}

	private void OnChangeCamera()
	{
		AbortLookOverride(restoreSnapshot: false);
	}

	private void OnInputGroupChanged(InputGroupsEnum _)
	{
		AbortLookOverride(restoreSnapshot: true);
	}

	private void AbortLookOverride(bool restoreSnapshot)
	{
		if (!_lookOverrideActive)
		{
			return;
		}
		_lookOverrideTween?.Kill();
		ClearPovBackSuppression();
		if (!V.Car)
		{
			return;
		}
		CachedCar.TryGetComponent<DriverController>(out var component);
		if (_lookActiveMode == LookMode.PovBack && component?.camInsideView != null)
		{
			component.camInsideView.transform.localPosition = _snapPovLocalPosition;
			component.camInsideView.transform.localEulerAngles = _snapPovEuler;
		}
		else if (restoreSnapshot && (bool)V.Car)
		{
			if (_lookActiveMode == LookMode.Pov && _carCameras.driverView && component?.camInsideView != null)
			{
				component.camInsideView.transform.localEulerAngles = _snapPovEuler;
			}
			else if (_lookActiveMode == LookMode.ChaseCam && !_carCameras.driverView)
			{
				_carCameras.yawAngle = _snapYawAngle;
			}
		}
		_lookOverrideActive = false;
		_lookActiveMode = LookMode.None;
		_activeLookArrow = LookArrow.None;
		_rearTransform = null;
	}

	private void LateUpdate()
	{
		if (!_lookOverrideActive || !V.Car)
		{
			return;
		}
		if (FlagsStorage.FixedCameraAndLookAt)
		{
			if (_lookActiveMode == LookMode.PovBack)
			{
				EndLookBackPovTeleport();
			}
			else
			{
				EndLookOverride();
			}
		}
		else if (_lookActiveMode == LookMode.PovBack && _carCameras.driverView && (bool)_rearTransform)
		{
			if (CachedCar.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView && CachedCar.TryGetComponent<VehicleData>(out var component2) && TryGetRearLookAnchor(component2, out var worldPos, out var worldRot))
			{
				component.camInsideView.transform.SetPositionAndRotation(worldPos, worldRot);
			}
		}
		else if (_lookActiveMode == LookMode.Pov && _carCameras.driverView)
		{
			if (CachedCar.TryGetComponent<DriverController>(out var component3) && (bool)component3.camInsideView)
			{
				Vector3 localEulerAngles = component3.camInsideView.transform.localEulerAngles;
				component3.camInsideView.transform.localEulerAngles = new Vector3(localEulerAngles.x, _lookOverrideCurrentYaw, localEulerAngles.z);
			}
		}
		else if (_lookActiveMode == LookMode.ChaseCam && !_carCameras.driverView)
		{
			_carCameras.yawAngle = _lookOverrideCurrentYaw;
		}
	}

	private void OldUpdateMethod()
	{
		CheckForUITouch();
		if (Input.touchCount == 0)
		{
			_joyStickY = 0f;
			_joyStickX = 0f;
			_carCameras.beganTouching = false;
			_isZooming = false;
		}
		else
		{
			MoveByTouch();
		}
		if (!FlagsStorage.MovingInsideCamPos)
		{
			ChangeCamWithJoyStick();
		}
	}

	private void CheckForUITouch()
	{
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && (bool)EventSystem.current && EventSystem.current.IsPointerOverGameObject())
		{
			_isUITouched = true;
		}
		else
		{
			_isUITouched = false;
		}
	}

	private void MoveByTouch()
	{
		if (_isUITouched)
		{
			return;
		}
		if (timeCheckDoubleTouch > 0f)
		{
			timeCheckDoubleTouch -= Time.deltaTime;
		}
		if (Input.touchCount == 2)
		{
			_firstpoint = Input.GetTouch(0).position;
			_secondPoint = Input.GetTouch(1).position;
			float num = _firstpoint.x / (float)Screen.width * 100f;
			float num2 = _secondPoint.x / (float)Screen.width * 100f;
			_joyStickX = 0f;
			_joyStickY = 0f;
			if ((num < 73f && num > 30f && num2 < 73f && num2 > 30f) || _isZooming)
			{
				Zooming();
				_carCameras.beganTouching = false;
				_secondPoint = _firstpoint;
				_xAngTemp = _xAngTemp2;
				_yAngTemp = _yAngTemp2;
			}
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				_joyStickX = 0f;
				_joyStickY = 0f;
				_carCameras.beganTouching = false;
			}
		}
		else
		{
			if (Input.touchCount != 1)
			{
				return;
			}
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				_firstpoint = Input.GetTouch(0).position;
				timeCheckDoubleTouch = 2f;
				_carCameras.WidthPercent = _firstpoint.x / (float)Screen.width * 100f;
				_xAngTemp = _joyStickX;
				_yAngTemp = _joyStickY;
				_isZooming = false;
				if (_carCameras.WidthPercent < 73f && _carCameras.WidthPercent > 30f && !EventSystem.current.currentSelectedGameObject && !FlagsStorage.MovingInsideCamPos)
				{
					_carCameras.beganTouching = true;
				}
			}
			if (Input.GetTouch(0).phase == TouchPhase.Moved && _carCameras.beganTouching)
			{
				_xAngTemp2 = _xAngTemp;
				_yAngTemp2 = _yAngTemp;
				_secondPoint = Input.GetTouch(0).position;
				_joyStickX = _xAngTemp + (_secondPoint.x - _firstpoint.x) / (float)Screen.width * 2f;
				_joyStickY = _yAngTemp - (_secondPoint.y - _firstpoint.y) / (float)Screen.height * 2f;
				timeCheckDoubleTouch = 0f;
			}
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				_joyStickX = 0f;
				_joyStickY = 0f;
				_carCameras.beganTouching = false;
			}
		}
	}

	private void MoveByMouse()
	{
	}

	private void ChangeCamWithJoyStick()
	{
		if (_joyStickY == 0f && _joyStickX == 0f)
		{
			return;
		}
		if (_carCameras.driverView)
		{
			if ((bool)CachedCar && CachedCar.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
			{
				Transform obj = component.camInsideView.transform;
				Vector3 localEulerAngles = obj.localEulerAngles;
				localEulerAngles.x += _joyStickY * 4f;
				localEulerAngles.y += _joyStickX * 4f;
				_carCameras.camChangedByTouch = true;
				obj.localEulerAngles = localEulerAngles;
			}
		}
		else if (!FlagsStorage.FixedCameraAndLookAt)
		{
			if (_carCameras.SteamLookMode != CarCameras.SteamLook.Fixed)
			{
				_carCameras.camChangedByTouch = true;
				float a = _carCameras.pitchAngle - (0f - _carCameras.height + _carCameras.pitchAngleOffset);
				_carCameras.height += _joyStickY * 0.22f * speedMove;
				_carCameras.height = Mathf.Clamp(_carCameras.height, 0f, 30f);
				_carCameras.pitchAngle = 0f - _carCameras.height + _carCameras.pitchAngleOffset + Mathf.Lerp(a, 0f, 2.25f * Time.deltaTime);
				if (_carCameras.SteamLookMode == CarCameras.SteamLook.Free)
				{
					_carCameras.freeOrbitYaw += _joyStickX * 4f * speedMove;
				}
				else
				{
					_carCameras.yawAngle += _joyStickX * 4f * speedMove;
				}
			}
		}
		else if (FlagsStorage.CamStaticMode)
		{
			Transform transform = _coreGameManager.transform;
			float num = 20f;
			if (!_menuLevelParent.focusToCar.isOn)
			{
				Vector3 localEulerAngles2 = transform.localEulerAngles;
				localEulerAngles2.x += _joyStickY * 4f;
				localEulerAngles2.y += _joyStickX * 4f;
				transform.localEulerAngles = localEulerAngles2;
			}
			else
			{
				Vector3 pos = transform.position - transform.right * (_joyStickX * 2f) + new Vector3(0f, _joyStickY * 2f, 0f);
				pos.y = Mathf.Clamp(pos.y, CachedCar.transform.position.y, CachedCar.transform.position.y + 20f);
				pos.x = Mathf.Clamp(pos.x, CachedCar.transform.position.x - num, CachedCar.transform.position.x + num);
				pos.z = Mathf.Clamp(pos.z, CachedCar.transform.position.z - num, CachedCar.transform.position.z + num);
				transform.position = carCamerasController.MakeRaycast(pos);
			}
		}
	}

	private void Zooming()
	{
		Touch touch = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);
		_isZooming = true;
		Vector2 vector = touch.position - touch.deltaPosition;
		Vector2 vector2 = touch2.position - touch2.deltaPosition;
		float magnitude = (vector - vector2).magnitude;
		float magnitude2 = (touch.position - touch2.position).magnitude;
		float num = magnitude - magnitude2;
		_cam.fieldOfView += num * perspectiveZoomSpeed;
		_cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, 15f, 80f);
	}

	private void ZoomingByMouse(float scrollZoomStep)
	{
		_cam.fieldOfView += scrollZoomStep * perspectiveZoomSpeed;
		_cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, 15f, 80f);
	}
}
