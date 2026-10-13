using System;
using System.Collections.Generic;
using CPMEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;

public class RacingCameraController : MonoBehaviour
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

	private const string FixedCameraNameLocalizedKey = "DragRacingCamera.fixed";

	private readonly Vector3 _startGarageCameraBodyFollowRotation = new Vector3(10f, 20f, 0f);

	public GameObject PathSetTarget;

	public CinemachineVirtualCamera virtualCamera;

	public CinemachineBasicMultiChannelPerlin noise;

	public CinemachineBasicMultiChannelPerlin noiseDriverView;

	public CinemachineVirtualCamera virtualCameraDriverView;

	public CinemachineVirtualCamera virtualCameraGarage;

	public CinemachineVirtualCamera veryFirstCamera;

	public CinemachineVirtualCamera ReverseCamera;

	public CinemachineTargetGroup targetGroup;

	public Transform mainCarToLook;

	public Transform bodyFollow;

	public DragRacingCamera.CameraType currentCamera;

	private TouchMovePerson _touchMovePerson;

	private Powertrain drivetrain;

	private Axles axles;

	private VehicleData carData;

	private Rigidbody carRigidBody;

	public Transform ourCam;

	private CinemachineOrbitalTransposer ctGarage;

	public float camSettingField = 50f;

	private Quaternion targetRotation;

	public Transform driverViewPoint;

	public EventSystem eventSystem;

	private CinemachineTransposer _cinemachineTransposer;

	public float ZoomSpeed = 1f;

	public float driverViewLimit = 90f;

	private float _driverOffsetX;

	private float _driverOffsetY;

	private CarInputManager _carInputManager;

	private RacingProvider _racingProvider;

	[Header("CheckCameraShakeSettings")]
	public float accTrehsoldX = 5f;

	public float accTrehsoldZ = 5f;

	public Vector3 currentAcceleration;

	public Vector3 localAcceleration;

	public float speedDependentCorrection = 0.025f;

	public float highSpeedDependentCorrection = 2f;

	public float highSpeedStartEffect = 40f;

	public Action<string, bool> OnChangeCameraStateAndText;

	[SerializeField]
	private int VelocityAverageWindow = 5;

	private Queue<Vector3> _velocitySamples = new Queue<Vector3>();

	private Vector3 _velocitySum = Vector3.zero;

	private Vector3 _lastAveragedVelocity = Vector3.zero;

	private bool _hasAveragedPrev;

	private MainPerson _mainPerson;

	private CinemachineBrain _cinemachineBrain;

	private IDisposable _setHeadVisibleDisposable;

	private bool _isInitialized;

	private RaceModeDataSO _raceModeDataSo;

	private int _prevTouchCount;

	private Vector3 _lockedTargetRot;

	private int _rotationFingerId = -1;

	private Vector2 _touchStartScreenPos;

	public RacingTouchInput RacingTouchInput;

	private SoundControllerExtension _carInOutSound;

	private bool isUITouched;

	public bool rotatingByTouchNow;

	public bool movedByPlayer;

	public Vector3 posTouch;

	public float timeStartTouch;

	public float SpeedTouch = 4f;

	public Vector3 targetRot;

	public Vector3 eulerAngles;

	private bool _cameraViewSettingsDefault;

	public float thresholdToch;

	public float speedY = -0.5f;

	public float speedX = 0.5f;

	public float smoothSpeed4 = 2f;

	public Vector3 edgeTrackerTargetPos;

	private bool _farReturnInProgress;

	private bool _isDestoyed;

	private IDisposable _removeCameraTextTypeDisposable;

	private CinemachineComposer CineComposer;

	public float distanceMag = 1f;

	public float garageDistanceCollider = 1f;

	public float garageZoomDistance = 1f;

	private float _garageBaseY;

	private bool edgeReached;

	private float targetField;

	public Transform edgeTracker;

	private LayerMask _carMask;

	public Vector3 targetOffset;

	public Vector3 CameraSettingOffset;

	[SerializeField]
	private float _returnCameraToMainSpeed = 0.8f;

	private float _maxSpeed;

	public float accelerationEffect = 0.1f;

	public bool lookAtVelo = true;

	public float maxPullback2 = 4.4f;

	public float smoothAcceleration;

	public float accelerationSmoothSpeed = 8f;

	public bool activeFOV;

	private float _maxOffsetMoveSpeed = 1f;

	private float _accelerationClamp = 20f;

	[Header("Camera View Configs")]
	public CameraViewConfig CloseView = new CameraViewConfig
	{
		Offset = new Vector3(0f, 1.8f, -6f),
		FOV = 40f,
		ComposerOffset = new Vector3(0f, 0.5f, 0f)
	};

	public CameraViewConfig MiddleView = new CameraViewConfig
	{
		Offset = new Vector3(0f, 1.8f, -9f),
		FOV = 40f,
		ComposerOffset = new Vector3(0f, 0.5f, 0f)
	};

	public CameraViewConfig FarView = new CameraViewConfig
	{
		Offset = new Vector3(0f, 2.1f, -7.5f),
		FOV = 45f,
		ComposerOffset = new Vector3(0f, 0.5f, 0f)
	};

	[Header("Rally Camera Follow")]
	public float rallyFollowSpeed = 3f;

	public float rallyAngleSpeedMultiplier = 0.05f;

	[Range(0f, 1f)]
	public float rallyPitchFollow = 1f;

	public float rallyPitchMaxDegrees = 45f;

	public float rallyPitchSpeed = 8f;

	private float _rallyPitch;

	[Header("Look Return")]
	public float lookHoldSeconds = 2.5f;

	public float lookReturnSpeed = 1.2f;

	public float lookReturnEaseSeconds = 1.2f;

	private float _lookReturnBlend = 1f;

	private float _targetAmplitude;

	private float _targetGain;

	private float _currentAmplitude;

	private float _currentGain;

	[SerializeField]
	private float _shakeTransitionSpeed = 5f;

	private float _shakeMultiplier = 1f;

	private bool _shakeOnKerb = true;

	private bool _shakeOnOffRoad = true;

	private float _shakeTimer;

	public bool _isShaking;

	[SerializeField]
	private float defaultAmplitude = 0.01f;

	[SerializeField]
	private float defaultGain = 0.01f;

	public float fieldAdjustment = 0.6f;

	public float speed_fiedCorection = 0.08f;

	public float speedFOV = 6f;

	public float startFOVmove = 40f;

	public float endFOVmove = 85f;

	private static readonly DragRacingCamera.CameraType[] _cameraCycle = new DragRacingCamera.CameraType[4]
	{
		DragRacingCamera.CameraType.driverView,
		DragRacingCamera.CameraType.close,
		DragRacingCamera.CameraType.middle,
		DragRacingCamera.CameraType.far
	};

	private float _cockpitShockFilterTime;

	private float _cockpitShockFilterClamp;

	private Transform _cockpitBaseFollow;

	private Vector3 _cockpitBaseLocalPos;

	private Vector3 _cockpitFilteredWorldPos;

	private bool _cockpitFilterValid;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

	private RegistryHandle<BoolEvent> _lookLeftHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookLeft);

	private RegistryHandle<BoolEvent> _lookRightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookRight);

	private RegistryHandle<BoolEvent> _lookBackHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookBack);

	private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

	private RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

	private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

	private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

	private RegistryHandle<BoolEvent> _onLookFreeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLookFree);

	private SteeringWheelValueEvent _steeringWheelValueEvent;

	private DesktopControlsSettingsSO _controlsSettings;

	private CarCamerasParametersConfig _carCamerasParametersConfig;

	private AlterChaseCameraConfig _chaseFramingConfig;

	[SerializeField]
	private SteamSpeedCameraConfig _speedFovConfig;

	private LookMode _lookActiveMode;

	private LookArrow _activeLookArrow;

	private bool _lookHeld;

	private bool _lookFreeHeld;

	private bool _lookOverrideActive;

	private Tween _lookOverrideTween;

	private float _snapYawAngle;

	private Vector3 _snapPovEuler;

	private Vector3 _snapPovLocalPosition;

	private Transform _rearTransform;

	private DriverController _povBackDriver;

	private const float LookTransitionTime = 0.18f;

	private static readonly Vector2 Multiplier = new Vector2(0.864f, 0.605f);

	private static readonly Vector2 MultiplierPitMouse = new Vector2(6f, 4f);

	private static readonly Vector2 MultiplierPitGamepad = new Vector2(3.6f, 2.4f);

	private static readonly Vector2 MultiplierChaseMouse = new Vector2(2.8f, 1.9f);

	private static readonly Vector2 MultiplierChaseGamepad = new Vector2(3.08f, 2.09f);

	private static readonly Vector2 MultiplierFpv = new Vector2(1.5f, 0.8f);

	protected const float MinLookFactor = 0.15f;

	private const float DriverViewRecenterDelay = 1f;

	private const float DriverViewRecenterSmoothTime = 0.3f;

	private const float DriverViewRestPitch = 6f;

	private const float RecenterMinSpeedKmh = 3f;

	private float _driverViewIdleTime;

	private float _driverPitchVel;

	private float _driverYawVel;

	private const float ZoomSpeedAlter = 100f;

	private const float GarageZoomMin = 2f;

	private const float GarageZoomMax = 10f;

	private const float CarZoomMin = -10f;

	private const float CarZoomMax = -1f;

	private const float DriverViewZoomMin = 20f;

	private const float DriverViewZoomMax = 90f;

	private const float ZoomSmoothTime = 0.12f;

	private const float GarageZoomSpeed = 3f;

	private const float DriverViewZoomSpeed = 15f;

	private const float CarFovZoomSpeed = 15f;

	private const float ChaseFovMin = 30f;

	private const float ChaseFovMax = 75f;

	private float _targetChaseFov;

	private float _carZoomVel;

	private float _targetGarageZoom;

	private float _garageZoomVel;

	private float _targetFovZoom;

	private float _fovZoomVel;

	private float _speedFovSmoothed;

	private bool _carZoomInit;

	private bool _garageZoomInit;

	private bool _fovZoomInit;

	private const float RearHeightFactor = 0.55f;

	private const float RearClearance = 0.05f;

	private const float RearOverhang = 0.6f;

	private const float RearHeightAboveAxle = 1f;

	[SerializeField]
	private bool deubg;

	private static readonly HashSet<DragRacingCamera.CameraType> DrivingCamTypes = new HashSet<DragRacingCamera.CameraType>
	{
		DragRacingCamera.CameraType.driverView,
		DragRacingCamera.CameraType.close,
		DragRacingCamera.CameraType.middle,
		DragRacingCamera.CameraType.far
	};

	private CameraViewConfig CurrentViewConfig => currentCamera switch
	{
		DragRacingCamera.CameraType.close => CloseView, 
		DragRacingCamera.CameraType.middle => MiddleView, 
		DragRacingCamera.CameraType.far => FarView, 
		_ => CloseView, 
	};

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

	private static bool IsGamepadNow
	{
		get
		{
			if (InputDeviceDetector.Instance != null)
			{
				return InputDeviceDetector.Instance.CurrentDeviceType != InputDeviceType.KeyboardMouse;
			}
			return false;
		}
	}

	private float LookLeftYaw
	{
		get
		{
			if (!_chaseFramingConfig)
			{
				return -90f;
			}
			return _chaseFramingConfig.LookLeftYaw;
		}
	}

	private float LookRightYaw
	{
		get
		{
			if (!_chaseFramingConfig)
			{
				return 90f;
			}
			return _chaseFramingConfig.LookRightYaw;
		}
	}

	private float LookBackYaw
	{
		get
		{
			if (!_chaseFramingConfig)
			{
				return 180f;
			}
			return _chaseFramingConfig.LookBackYaw;
		}
	}

	private float LookForwardYaw
	{
		get
		{
			if (!_chaseFramingConfig)
			{
				return 0f;
			}
			return _chaseFramingConfig.LookForwardYaw;
		}
	}

	private bool IsDrivingCam => DrivingCamTypes.Contains(currentCamera);

	private bool IsLiveOnBrain => true;

	private float LookOverrideCurrentYaw
	{
		get
		{
			switch (currentCamera)
			{
			case DragRacingCamera.CameraType.driverView:
				return _driverOffsetY;
			case DragRacingCamera.CameraType.close:
			case DragRacingCamera.CameraType.middle:
			case DragRacingCamera.CameraType.far:
				return Mathf.DeltaAngle(V.Car.transform.eulerAngles.y, targetRot.y);
			default:
				return 0f;
			}
		}
		set
		{
			switch (currentCamera)
			{
			case DragRacingCamera.CameraType.driverView:
				if ((bool)bodyFollow && (bool)V.LastUsedCar && (bool)mainCarToLook)
				{
					Transform follow = virtualCameraDriverView.Follow;
					if ((bool)follow)
					{
						_driverOffsetY = value;
						Vector3 vector = mainCarToLook.eulerAngles;
						follow.eulerAngles = new Vector3(vector.x + _driverOffsetX, vector.y + _driverOffsetY, vector.z);
					}
				}
				break;
			case DragRacingCamera.CameraType.close:
			case DragRacingCamera.CameraType.middle:
			case DragRacingCamera.CameraType.far:
				if ((bool)bodyFollow && (bool)V.LastUsedCar)
				{
					targetRot = V.Car.transform.eulerAngles;
					targetRot.y += value;
					targetRot.z = 0f;
					targetRot.x = 0f;
					bodyFollow.eulerAngles = targetRot;
				}
				break;
			case DragRacingCamera.CameraType.garageView:
			case DragRacingCamera.CameraType.spectator:
			case DragRacingCamera.CameraType.freeCamera:
			case DragRacingCamera.CameraType.directionCamera:
				break;
			}
		}
	}

	[Inject]
	public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, CarInputManager carInputManager, RacingProvider racingProvider, RaceModeDataSO raceModeDataSo, SoundControllerExtension carInOutSound)
	{
		_carInOutSound = carInOutSound;
		_carInputManager = carInputManager;
		_racingProvider = racingProvider;
		_eventRegistry = eventRegistry;
		_raceModeDataSo = raceModeDataSo;
		if (raceModeDataSo != null)
		{
			ApplyPreset(raceModeDataSo.RaceModeData.CameraPreset);
		}
		_inputMapSetEvent = inputMapSetEvent;
	}

	public async UniTaskVoid Init()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(1.0));
		if (!_isDestoyed && (bool)V.DriverPerson)
		{
			_touchMovePerson = V.DriverPerson.GetComponent<TouchMovePerson>();
		}
	}

	public void PrepareCarReferences()
	{
		if ((bool)V.LastUsedCar)
		{
			_isInitialized = true;
			drivetrain = V.LastUsedCar.GetComponent<Powertrain>();
			axles = V.LastUsedCar.GetComponent<Axles>();
			carData = V.LastUsedCar.GetComponent<VehicleData>();
			_maxSpeed = carData.GetPowertrain.CalcTopSpeed();
			carRigidBody = V.LastUsedCar.GetComponent<Rigidbody>();
		}
	}

	private void SetCinemachineFirstPath(CinemachineDollyCart dollyCart)
	{
		CinemachinePath cinemachinePath = PathSetTarget.GetComponent<CinemachinePath>() ?? PathSetTarget.AddComponent<CinemachinePath>();
		CineMachinePathData cineMachinePathData = _raceModeDataSo.RaceModeData.CineMachinePathData;
		if (cineMachinePathData != null)
		{
			cinemachinePath.m_Waypoints = cineMachinePathData.Waypoints;
			cinemachinePath.m_Looped = cineMachinePathData.Loopped;
			cinemachinePath.m_Resolution = cineMachinePathData.Resolution;
			dollyCart.m_Path = cinemachinePath;
		}
	}

	public async UniTask StartMoveVeryFirstCam()
	{
		veryFirstCamera.enabled = true;
		CinemachineDollyCart dollyCart = veryFirstCamera.Follow.GetComponent<CinemachineDollyCart>();
		SetCinemachineFirstPath(dollyCart);
		if (dollyCart.m_Path == null)
		{
			veryFirstCamera.enabled = false;
			return;
		}
		dollyCart.enabled = false;
		dollyCart.m_Position = 0f;
		dollyCart.m_Speed = 0f;
		await UniTask.Delay(400);
		dollyCart.enabled = true;
		while (dollyCart.m_Position < dollyCart.m_Path.PathLength)
		{
			float num = dollyCart.m_Position / dollyCart.m_Path.PathLength;
			dollyCart.m_Speed = ((num > 0.48f) ? (2f + 5f * (2f - num)) : 40f);
			if (_lookHeld || _lookFreeHeld)
			{
				break;
			}
			await UniTask.Delay(100);
		}
		veryFirstCamera.enabled = false;
	}

	public async void setCamera(GameObject target)
	{
		CinemachineTargetGroup.Target[] t = new CinemachineTargetGroup.Target[2];
		if (target.TryGetComponent<VehicleData>(out var carDynamics))
		{
			while (!carDynamics.Collider)
			{
				MonoBehaviour.print("waiting center of mass");
				await UniTask.Delay(100);
			}
			mainCarToLook = carDynamics.Collider.transform;
		}
		t[0].target = target.transform;
		t[0].weight = 1f;
		targetGroup.m_Targets = t;
		virtualCamera.enabled = true;
		virtualCameraGarage.enabled = false;
		virtualCamera.LookAt = targetGroup.transform;
		virtualCamera.Follow = bodyFollow.transform;
		noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noiseDriverView = virtualCameraDriverView.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		virtualCameraGarage.LookAt = edgeTracker;
		virtualCameraGarage.Follow = bodyFollow.transform;
		targetRotation = bodyFollow.rotation;
		ReverseCamera.enabled = false;
		ReverseCamera.LookAt = targetGroup.transform;
		ReverseCamera.Follow = bodyFollow.transform;
		if (target.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
		{
			virtualCameraDriverView.Follow = component.camInsideView.transform;
			GameObject gameObject = new GameObject("DriverViewOrigin");
			driverViewPoint = gameObject.transform;
			driverViewPoint.position = virtualCameraDriverView.Follow.position;
			driverViewPoint.localRotation = virtualCameraDriverView.Follow.localRotation;
			driverViewPoint.parent = virtualCameraDriverView.Follow.parent;
		}
	}

	private void Start()
	{
		ctGarage = virtualCameraGarage.GetCinemachineComponent<CinemachineOrbitalTransposer>();
		_garageBaseY = ctGarage.m_FollowOffset.y;
		CineComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
		_cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		_racingProvider.RacingCameraController = this;
		_cinemachineBrain = GetComponent<CinemachineBrain>();
		_ = new string[2] { "Load", "parktronic" };
		_carMask = LayerMask.GetMask("car");
		SpeedTouch = 0.5f;
		smoothSpeed4 = 2f;
		if (V.CurrentRaceModeType == RacingModeType.CircuitRacing)
		{
			CircuitRaceControlller.CurrentClassId.Get(out int value);
			if (value == 6)
			{
				SetColliderLayer(virtualCamera, LayerBitmask.PedestrainLayer);
				SetColliderLayer(ReverseCamera, LayerBitmask.PedestrainLayer);
			}
		}
		_mainPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
		SubscribeToCameraActions();
	}

	public void MarkCameraSettingsChanged()
	{
		_cameraViewSettingsDefault = false;
	}

	public void SetColliderLayer(CinemachineVirtualCamera cam, int layerIndex)
	{
		CinemachineCollider component = cam.GetComponent<CinemachineCollider>();
		component.m_TransparentLayers = (int)component.m_TransparentLayers | layerIndex;
	}

	private void ApplyGarageRotationDelta(Vector2 delta)
	{
		if (!(delta.magnitude <= thresholdToch))
		{
			_cameraViewSettingsDefault = false;
			timeStartTouch = Time.time;
			targetRot.y += speedY * delta.x;
			targetRot.x += speedX * delta.y;
			targetRot.x = Mathf.Clamp(targetRot.x, 0f, 70f);
			targetRot.z = 0f;
		}
	}

	private void MoveBodyByTouchOnGarage()
	{
		if ((bool)bodyFollow && (bool)V.LastUsedCar)
		{
			Transform movingNow = bodyFollow;
			MoveBodyByTouchOnGarageAlter(movingNow);
		}
	}

	private void RayCastCarColliderForGarageCamera(Vector3 origin, Vector3 direction)
	{
		if (Physics.Raycast(origin, direction, out var hitInfo, 300f, _carMask, QueryTriggerInteraction.Ignore))
		{
			edgeTrackerTargetPos = hitInfo.point;
		}
	}

	private void MoveBodyGarageView()
	{
		if (!bodyFollow || !V.LastUsedCar || !carData)
		{
			return;
		}
		Collider collider = carData.Collider;
		if (collider == null)
		{
			return;
		}
		if (rotatingByTouchNow)
		{
			RayCastCarColliderForGarageCamera(virtualCameraGarage.transform.position, collider.transform.position - virtualCameraGarage.transform.position);
		}
		float value = garageZoomDistance + garageDistanceCollider;
		value = Mathf.Clamp(value, 1f, 10f);
		float f = eulerAngles.x * ((float)Math.PI / 180f);
		ctGarage.m_FollowOffset = new Vector3(0f, Mathf.Sin(f) * value + _garageBaseY * Mathf.Cos(f), Mathf.Cos(f) * value - Mathf.Sin(f) * _garageBaseY);
		targetField = camSettingField;
		float num = Vector3.Distance(edgeTracker.transform.position, edgeTrackerTargetPos);
		if (edgeReached)
		{
			edgeReached = num < 0.5f;
		}
		if (!edgeReached)
		{
			float num2 = Mathf.Clamp(num, 0.5f, 3f) * 10f;
			edgeTracker.transform.position = Vector3.Lerp(edgeTracker.transform.position, edgeTrackerTargetPos, Time.deltaTime * num2);
			garageDistanceCollider = Vector3.Distance(edgeTracker.transform.position, collider.transform.position) * distanceMag;
			garageDistanceCollider = Mathf.Clamp(garageDistanceCollider, -7f, 7f);
			if (num < 0.2f)
			{
				edgeReached = true;
			}
		}
		bodyFollow.transform.position = edgeTracker.transform.position;
	}

	public void SetCameraShake(float targetAmplitude, float targetGain, float duration, float speed = 1f)
	{
		if (noise == null)
		{
			noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			defaultAmplitude = noise.m_AmplitudeGain;
			defaultGain = noise.m_FrequencyGain;
			if (noise == null)
			{
				return;
			}
		}
		if (currentCamera != DragRacingCamera.CameraType.far)
		{
			_targetAmplitude = Mathf.Abs(targetAmplitude) * speed;
			_targetGain = Mathf.Abs(targetGain) * speed;
			_shakeTimer = Mathf.Abs(duration);
			_isShaking = true;
		}
	}

	private void UpdateCameraShake()
	{
		if (!noise)
		{
			if ((bool)virtualCamera)
			{
				noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			}
			if (!noise)
			{
				return;
			}
		}
		if (_isShaking)
		{
			_shakeTimer -= Time.deltaTime;
			if (_shakeTimer <= 0f)
			{
				_isShaking = false;
				_targetAmplitude = 0f;
				_targetGain = 0f;
			}
			_currentAmplitude = Mathf.Lerp(_currentAmplitude, _targetAmplitude, Time.deltaTime * _shakeTransitionSpeed);
			_currentGain = Mathf.Lerp(_currentGain, _targetGain, Time.deltaTime * _shakeTransitionSpeed);
			noise.m_AmplitudeGain = _currentAmplitude;
			noise.m_FrequencyGain = _currentGain;
			if ((bool)noiseDriverView)
			{
				noiseDriverView.m_AmplitudeGain = _currentAmplitude;
				noiseDriverView.m_FrequencyGain = _currentGain * 1.5f;
			}
		}
		else if (_currentAmplitude > defaultAmplitude || _currentGain > defaultGain)
		{
			_currentAmplitude = Mathf.Lerp(_currentAmplitude, 0f, Time.deltaTime * _shakeTransitionSpeed);
			_currentGain = Mathf.Lerp(_currentGain, 0f, Time.deltaTime * _shakeTransitionSpeed);
			noise.m_AmplitudeGain = _currentAmplitude;
			noise.m_FrequencyGain = _currentGain;
			if ((bool)noiseDriverView)
			{
				noiseDriverView.m_AmplitudeGain = _currentAmplitude;
				noiseDriverView.m_FrequencyGain = _currentGain;
			}
		}
	}

	public void StopCameraShake()
	{
		if ((bool)noise)
		{
			_isShaking = false;
			_targetAmplitude = defaultAmplitude;
			_targetGain = defaultGain;
		}
	}

	private void MoveBodyFollow()
	{
		if (!bodyFollow || !V.LastUsedCar || !mainCarToLook)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float speedMps = carData.GetPowertrain.SpeedMps;
		if (speedMps > endFOVmove)
		{
			speedMps = endFOVmove;
		}
		if (speedMps > startFOVmove)
		{
			num = (speedMps - startFOVmove) * fieldAdjustment;
			num2 = (speedMps - startFOVmove) * speed_fiedCorection;
		}
		num = SpeedCameraFx.FovAdd(carData.GetPowertrain.SpeedMps, _speedFovConfig);
		targetField = camSettingField + num;
		bodyFollow.transform.position = mainCarToLook.position;
		if (currentCamera == DragRacingCamera.CameraType.far)
		{
			_cinemachineTransposer.m_FollowOffset = CameraSettingOffset;
			virtualCamera.m_Lens.FieldOfView = camSettingField + SpeedCameraFx.FovAdd(carData.GetPowertrain.SpeedMps, _speedFovConfig);
			if (!rotatingByTouchNow)
			{
				Vector3 forward = mainCarToLook.forward;
				forward.y = 0f;
				if (forward.sqrMagnitude > 0.001f)
				{
					bodyFollow.transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(StepRallyPitch(), 0f, 0f);
				}
			}
			return;
		}
		if (rotatingByTouchNow || _lookOverrideActive)
		{
			virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, targetField, speedFOV * Time.deltaTime);
			_lookReturnBlend = 0f;
			float x = bodyFollow.transform.eulerAngles.x;
			_rallyPitch = ((x > 180f) ? (x - 360f) : x);
			return;
		}
		float b = Vector3.Dot(currentAcceleration, V.LastUsedCar.transform.forward);
		smoothAcceleration = Mathf.Lerp(smoothAcceleration, b, Time.deltaTime * accelerationSmoothSpeed);
		smoothAcceleration = Mathf.Clamp(smoothAcceleration, 0f - _accelerationClamp, _accelerationClamp);
		float num3 = Mathf.Atan2(mainCarToLook.forward.y, new Vector2(mainCarToLook.forward.x, mainCarToLook.forward.z).magnitude) * 57.29578f;
		CameraViewConfig currentViewConfig = CurrentViewConfig;
		float num4 = ((num3 > 0f) ? currentViewConfig.SlopeUp : currentViewConfig.SlopeDown);
		float y = Mathf.Abs(num3) * num4;
		float z = (0f - smoothAcceleration) * accelerationEffect + num2;
		targetOffset = CameraSettingOffset + new Vector3(0f, y, z);
		if (Mathf.Abs(targetOffset.z) < maxPullback2)
		{
			targetOffset.z = ((targetOffset.z > 0f) ? maxPullback2 : (0f - maxPullback2));
		}
		_cinemachineTransposer.m_FollowOffset = Vector3.MoveTowards(_cinemachineTransposer.m_FollowOffset, targetOffset, Mathf.Abs(smoothAcceleration) * _maxOffsetMoveSpeed);
		if (lookAtVelo)
		{
			LookAtVelocity();
		}
		else
		{
			Vector3 forward2 = mainCarToLook.forward;
			forward2.y = 0f;
			if (forward2.sqrMagnitude > 0.001f)
			{
				Quaternion b2 = Quaternion.LookRotation(forward2, Vector3.up);
				bodyFollow.transform.rotation = Quaternion.Lerp(bodyFollow.transform.rotation, b2, Time.deltaTime * _returnCameraToMainSpeed);
			}
		}
		eulerAngles = bodyFollow.transform.eulerAngles;
		targetRot = eulerAngles;
	}

	public async UniTaskVoid ResetDynamicRotation()
	{
		await UniTask.Delay(100);
		MonoBehaviour.print("ResetDynamicRotation");
		bodyFollow.rotation = mainCarToLook.rotation * Quaternion.Euler(10f, 0f, 0f);
		eulerAngles = bodyFollow.eulerAngles;
		targetRot = bodyFollow.eulerAngles;
	}

	public void ResetCameraInertia()
	{
		_velocitySamples.Clear();
		_velocitySum = Vector3.zero;
		_lastAveragedVelocity = Vector3.zero;
		_hasAveragedPrev = false;
		currentAcceleration = Vector3.zero;
		localAcceleration = Vector3.zero;
		smoothAcceleration = 0f;
		_cockpitFilterValid = false;
	}

	private void LookAtVelocity()
	{
		Vector3 forward = mainCarToLook.forward;
		forward.y = 0f;
		if (!(forward.sqrMagnitude < 0.001f))
		{
			Quaternion b = Quaternion.LookRotation(forward, Vector3.up);
			Quaternion a = Quaternion.Euler(0f, bodyFollow.transform.eulerAngles.y, 0f);
			float num = Quaternion.Angle(a, b);
			float b2 = rallyFollowSpeed + num * rallyAngleSpeedMultiplier;
			_lookReturnBlend = Mathf.MoveTowards(_lookReturnBlend, 1f, Time.deltaTime / Mathf.Max(lookReturnEaseSeconds, 0.01f));
			b2 = Mathf.Lerp(lookReturnSpeed, b2, _lookReturnBlend);
			Quaternion quaternion = Quaternion.Slerp(a, b, Time.deltaTime * b2);
			bodyFollow.transform.rotation = quaternion * Quaternion.Euler(StepRallyPitch(), 0f, 0f);
		}
	}

	private float StepRallyPitch()
	{
		float b = Mathf.Clamp((0f - Mathf.Asin(Mathf.Clamp(mainCarToLook.forward.y, -1f, 1f))) * 57.29578f * rallyPitchFollow, 0f - rallyPitchMaxDegrees, rallyPitchMaxDegrees);
		_rallyPitch = Mathf.Lerp(_rallyPitch, b, Time.deltaTime * rallyPitchSpeed);
		return _rallyPitch;
	}

	private void CheckForUITouch()
	{
		if (Input.touchCount <= 0)
		{
			isUITouched = false;
		}
		else
		{
			isUITouched = Input.GetTouch(0).phase == TouchPhase.Began && eventSystem.IsPointerOverGameObject();
		}
	}

	private void MoveBodyByTouchRacing()
	{
		if ((bool)bodyFollow && (bool)V.LastUsedCar)
		{
			Transform movingNow = bodyFollow;
			if (rotatingByTouchNow && (bool)carRigidBody && currentCamera != DragRacingCamera.CameraType.far && Time.time - timeStartTouch > lookHoldSeconds)
			{
				rotatingByTouchNow = false;
			}
			MoveBodyByTouchRacingAlter(movingNow);
		}
	}

	private void zooming()
	{
		Touch touch = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);
		Vector2 vector = touch.position - touch.deltaPosition;
		Vector2 vector2 = touch2.position - touch2.deltaPosition;
		float magnitude = (vector - vector2).magnitude;
		float magnitude2 = (touch.position - touch2.position).magnitude;
		float num = magnitude - magnitude2;
		if (currentCamera == DragRacingCamera.CameraType.driverView)
		{
			camSettingField += num * ZoomSpeed * 0.1f;
			_cameraViewSettingsDefault = false;
			PlayerPrefs.SetFloat($"racingCamFOV_{(int)currentCamera}", camSettingField);
		}
		else
		{
			garageZoomDistance += num * ZoomSpeed * 0.1f;
			CameraSettingOffset.z -= num * ZoomSpeed * 0.1f;
			CameraSettingOffset.z = Mathf.Clamp(CameraSettingOffset.z, -10f, -1f);
			garageZoomDistance = Mathf.Clamp(garageZoomDistance, -2f, 10f);
			SaveCurrentCamZoom();
		}
	}

	public void DisableAllCamera()
	{
		veryFirstCamera.enabled = false;
		virtualCamera.enabled = false;
		virtualCameraGarage.enabled = false;
		virtualCameraDriverView.enabled = false;
		ReverseCamera.enabled = false;
		_carInOutSound.SetOutCarSound();
	}

	public void ChangeCameraFollowType(CinemachineVirtualCamera camera, CinemachineTransposer.BindingMode bindingMode)
	{
		CinemachineTransposer cinemachineComponent = camera.GetCinemachineComponent<CinemachineTransposer>();
		if (cinemachineComponent != null)
		{
			cinemachineComponent.m_BindingMode = bindingMode;
		}
	}

	public async UniTaskVoid CameraReversed(bool isPressed)
	{
		if (_isDestoyed || !ReverseCamera || ReverseCamera.enabled == isPressed)
		{
			return;
		}
		if (isPressed)
		{
			ReverseCamera.enabled = true;
			return;
		}
		ReverseCamera.enabled = false;
		await UniTask.Yield();
		if (!_isDestoyed)
		{
			ChangeCameraOnCar(currentCamera);
		}
	}

	public void SetCameraForGarage(GameObject car)
	{
		DisableAllCamera();
		virtualCameraGarage.enabled = true;
		garageZoomDistance = 3f;
		garageDistanceCollider = 0f;
		edgeReached = true;
		ctGarage.m_FollowOffset.z = garageZoomDistance;
		currentCamera = DragRacingCamera.CameraType.garageView;
		Vector3 position = ((carData?.flagPoint != null) ? carData.flagPoint.transform.position : car.transform.position);
		bodyFollow.transform.position = position;
		edgeTracker.transform.position = position;
		edgeTrackerTargetPos = position;
		movedByPlayer = false;
		eulerAngles = _startGarageCameraBodyFollowRotation;
		targetRot = eulerAngles;
		RayCastCarColliderForGarageCamera(virtualCameraGarage.transform.position, carData.Collider.transform.position - virtualCameraGarage.transform.position);
	}

	public void SetCameraForRacing()
	{
		DisableAllCamera();
		virtualCamera.enabled = true;
		currentCamera = (DragRacingCamera.CameraType)PlayerPrefs.GetInt("CarCameraRacing", 1);
		ChangeCameraOnCar(currentCamera);
		rotatingByTouchNow = false;
		if ((bool)driverViewPoint)
		{
			driverViewPoint.transform.position = V.LastUsedCar.GetComponent<DriverController>().camInsideView.transform.position;
		}
	}

	private void ShowHideInterior(DragRacingCamera.CameraType next)
	{
		if (next == DragRacingCamera.CameraType.driverView)
		{
			if (!carData.InteriorsComposition.Interior)
			{
				carData.InteriorsComposition.TryRestoreSavedInterior();
			}
			else
			{
				carData.InteriorsComposition.ShowHighInterior();
			}
		}
		else if (CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
		{
			carData.InteriorsComposition.ShowLowInterior();
		}
	}

	public void ChangeCameraOnCar(DragRacingCamera.CameraType next)
	{
		currentCamera = next;
		ShowHideInterior(next);
		_setHeadVisibleDisposable?.Dispose();
		_setHeadVisibleDisposable = null;
		_cameraViewSettingsDefault = true;
		lookAtVelo = true;
		rotatingByTouchNow = false;
		_farReturnInProgress = false;
		_cockpitBaseFollow = null;
		_cockpitFilterValid = false;
		if ((bool)mainCarToLook)
		{
			Vector3 forward = mainCarToLook.forward;
			forward.y = 0f;
			if (forward.sqrMagnitude > 0.001f)
			{
				targetRot = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
			}
		}
		if (next == DragRacingCamera.CameraType.driverView)
		{
			_driverOffsetX = 0f;
			_driverOffsetY = 0f;
			if ((bool)V.Car && V.Car.TryGetComponent<DriverController>(out var component) && component.camInsideView != null && driverViewPoint != null)
			{
				component.camInsideView.transform.localPosition = driverViewPoint.localPosition;
				component.camPosInitialInsideView = driverViewPoint.localPosition;
				component.camPosDefaultInsideView = driverViewPoint.localPosition;
			}
			if ((bool)V.Car && V.Car.TryGetComponent<DriverController>(out var component2))
			{
				component2.camInsideView.transform.localRotation = Quaternion.identity;
			}
			ReverseCamera.Follow = virtualCameraDriverView.Follow;
			virtualCameraDriverView.enabled = true;
			virtualCamera.enabled = false;
			_carInOutSound.UpdateInCarSound(carData.CarId);
			float num = PlayerPrefs.GetFloat($"racingCamFOV_{(int)next}", 0f);
			camSettingField = ((num != 0f) ? num : 50f);
			_setHeadVisibleDisposable = Observable.Timer(TimeSpan.FromSeconds(_cinemachineBrain.m_DefaultBlend.m_Time)).Subscribe((long _) =>
			{
				TrySwitchVisibleHeadPerson(isVisible: false);
			}).AddTo(this);
		}
		else
		{
			ReverseCamera.Follow = bodyFollow.transform;
			virtualCameraDriverView.enabled = false;
			virtualCamera.enabled = true;
			_carInOutSound.SetOutCarSound();
			TrySwitchVisibleHeadPerson(isVisible: true);
		}
		if (next == DragRacingCamera.CameraType.close)
		{
			ApplyViewConfig(CloseView, next);
		}
		if (next == DragRacingCamera.CameraType.middle)
		{
			ApplyViewConfig(MiddleView, next);
		}
		if (next == DragRacingCamera.CameraType.far)
		{
			ApplyViewConfig(FarView, next);
			lookAtVelo = false;
			StopCameraShake();
		}
		PlayerPrefs.SetInt("CarCameraRacing", (int)next);
		TrySetCameraText(next);
	}

	private void ApplyViewConfig(CameraViewConfig cfg, DragRacingCamera.CameraType type)
	{
		cfg = DeriveChaseViewConfig(cfg, type);
		targetOffset = cfg.Offset;
		CameraSettingOffset = cfg.Offset;
		camSettingField = cfg.FOV;
		CineComposer.m_TrackedObjectOffset = cfg.ComposerOffset;
		float num = PlayerPrefs.GetFloat($"racingCamZ_{(int)type}", 0f);
		if (num != 0f)
		{
			CameraSettingOffset.z = num;
		}
		if (_cinemachineTransposer != null)
		{
			Vector3 cameraSettingOffset = CameraSettingOffset;
			if (mainCarToLook != null)
			{
				float num2 = Mathf.Atan2(mainCarToLook.forward.y, new Vector2(mainCarToLook.forward.x, mainCarToLook.forward.z).magnitude) * 57.29578f;
				float num3 = ((num2 > 0f) ? cfg.SlopeUp : cfg.SlopeDown);
				cameraSettingOffset.y += Mathf.Abs(num2) * num3;
			}
			_cinemachineTransposer.m_FollowOffset = cameraSettingOffset;
		}
	}

	private void SaveCurrentCamZoom()
	{
		_cameraViewSettingsDefault = false;
		PlayerPrefs.SetFloat($"racingCamZ_{(int)currentCamera}", CameraSettingOffset.z);
	}

	public void TrySwitchVisibleHeadPerson(bool isVisible)
	{
		if (!_mainPerson)
		{
			_mainPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
		}
		if ((bool)_mainPerson)
		{
			_mainPerson.SwitchHeadVisible(isVisible);
		}
	}

	public void SetNextCam()
	{
		AbortLookOverride(restoreSnapshot: false);
		if (_cameraViewSettingsDefault)
		{
			int num = Array.IndexOf(_cameraCycle, currentCamera);
			num = (num + 1) % _cameraCycle.Length;
			currentCamera = _cameraCycle[num];
		}
		else
		{
			PlayerPrefs.DeleteKey($"racingCamZ_{(int)currentCamera}");
			PlayerPrefs.DeleteKey($"racingCamFOV_{(int)currentCamera}");
		}
		ChangeCameraOnCar(currentCamera);
	}

	private void Update()
	{
		if (_isInitialized && !_isDestoyed && (bool)virtualCamera && (bool)virtualCameraDriverView && (bool)virtualCameraGarage)
		{
			CheckForUITouch();
			if (currentCamera == DragRacingCamera.CameraType.driverView)
			{
				MoveBodyByTouchOnDriverView();
				MoveBodyFollowDriverView();
				ApplyCockpitShockFilter(virtualCameraDriverView.Follow);
			}
			else if (currentCamera == DragRacingCamera.CameraType.garageView)
			{
				MoveBodyByTouchOnGarage();
				MoveBodyGarageView();
			}
			else
			{
				MoveBodyByTouchRacing();
				MoveBodyFollow();
			}
			UpdateCameraShake();
		}
	}

	public void SetTouchInputEnabled(bool value)
	{
		if (RacingTouchInput != null)
		{
			RacingTouchInput.SetInputEnabled(value);
		}
	}

	private void MoveBodyByTouchOnDriverView()
	{
		if ((bool)bodyFollow && (bool)V.LastUsedCar && !(mainCarToLook == null))
		{
			Transform follow = virtualCameraDriverView.Follow;
			MoveBodyByTouchOnDriverViewAlter(follow);
		}
	}

	private void ApplyCockpitShockFilter(Transform follow)
	{
		if (_cockpitShockFilterTime <= 0f || !carRigidBody || !follow || !follow.parent)
		{
			_cockpitFilterValid = false;
			return;
		}
		if (_cockpitBaseFollow != follow)
		{
			_cockpitBaseFollow = follow;
			_cockpitBaseLocalPos = follow.localPosition;
			_cockpitFilterValid = false;
		}
		follow.localPosition = _cockpitBaseLocalPos;
		Vector3 position = follow.position;
		float deltaTime = Time.deltaTime;
		if (!_cockpitFilterValid || deltaTime <= 0f)
		{
			_cockpitFilteredWorldPos = position;
			_cockpitFilterValid = true;
			return;
		}
		Vector3 vector = _cockpitFilteredWorldPos + carRigidBody.velocity * deltaTime;
		Vector3 vector2 = position - vector;
		_cockpitFilteredWorldPos = vector + vector2 * (1f - Mathf.Exp((0f - deltaTime) / _cockpitShockFilterTime));
		Vector3 vector3 = _cockpitFilteredWorldPos - position;
		float num = Mathf.Max(0f, _cockpitShockFilterClamp);
		if (vector3.sqrMagnitude > num * num)
		{
			vector3 = vector3.normalized * num;
			_cockpitFilteredWorldPos = position + vector3;
		}
		follow.localPosition = _cockpitBaseLocalPos + follow.parent.InverseTransformVector(vector3);
	}

	private void ApplyDriverViewLook(Transform movingNow)
	{
		movingNow.rotation = mainCarToLook.rotation * Quaternion.Euler(_driverOffsetX, _driverOffsetY, 0f);
	}

	private float NormalizeAngle(float angle)
	{
		angle %= 360f;
		if (angle > 180f)
		{
			angle -= 360f;
		}
		if (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	private void MoveBodyFollowDriverView()
	{
		if (currentCamera == DragRacingCamera.CameraType.driverView)
		{
			if (!_mainPerson)
			{
				_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
			}
			Vector3 position = _mainPerson.ik.references.head.position;
			Vector2 b = new Vector2(position.z, position.x);
			position = _mainPerson.ik.references.root.position;
			float num = Vector2.Distance(new Vector2(position.z, position.x), b);
			_touchMovePerson.CameraDistance = (0f - num) * 5f - 0.1f;
		}
		virtualCameraDriverView.m_Lens.FieldOfView = camSettingField;
		if (!rotatingByTouchNow)
		{
			targetField = camSettingField;
		}
	}

	private void TrySetCameraText(DragRacingCamera.CameraType next)
	{
		string arg = ((next == DragRacingCamera.CameraType.far) ? ((string)new LocalizedString("DragRacingCamera.fixed")) : Utils.GetCameraTypeName(next));
		OnChangeCameraStateAndText?.Invoke($"#{(int)(next + 1)} {arg}", arg2: true);
		_removeCameraTextTypeDisposable?.Dispose();
		_removeCameraTextTypeDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			OnChangeCameraStateAndText?.Invoke("", arg2: false);
		}).AddTo(this);
	}

	public void SetCameraForPersonWalking()
	{
		DisableAllCamera();
		currentCamera = (DragRacingCamera.CameraType)PlayerPrefs.GetInt("PersonCameraRacing", 1);
		ChangeCameraOnPerson(currentCamera);
	}

	public void ChangeCameraOnPerson(DragRacingCamera.CameraType next)
	{
		Vector3 cameraPivot = new Vector3(0f, 1.6f, 1f);
		float fieldOfView = 50f;
		float cameraDistance = 0f;
		if (next == DragRacingCamera.CameraType.driverView)
		{
			cameraPivot = new Vector3(0f, 1.5f, 0f);
			cameraDistance = 0f;
			fieldOfView = 45f;
		}
		if (next == DragRacingCamera.CameraType.close)
		{
			cameraPivot = new Vector3(0f, 2f, 0f);
			cameraDistance = 2f;
			fieldOfView = 55f;
		}
		if (next == DragRacingCamera.CameraType.middle)
		{
			cameraPivot = new Vector3(0f, 2.2f, 0f);
			fieldOfView = 55f;
			cameraDistance = 4f;
		}
		if (next == DragRacingCamera.CameraType.far)
		{
			cameraPivot = new Vector3(0f, 2.5f, 0f);
			fieldOfView = 55f;
			cameraDistance = 7f;
		}
		_touchMovePerson.CameraPivot = cameraPivot;
		_touchMovePerson.CameraDistance = cameraDistance;
		ourCam.GetComponent<Camera>().fieldOfView = fieldOfView;
		PlayerPrefs.SetInt("PersonCameraRacing", (int)next);
		_carInOutSound.SetOutCarSound();
	}

	private void FixedUpdate()
	{
		if (!_isDestoyed && (bool)virtualCamera)
		{
			CheckSetCameraShake();
		}
	}

	private Vector3 UpdateAndGetAveragedVelocity(Vector3 newVelocity)
	{
		_velocitySamples.Enqueue(newVelocity);
		_velocitySum += newVelocity;
		if (VelocityAverageWindow < 1)
		{
			VelocityAverageWindow = 1;
		}
		while (_velocitySamples.Count > VelocityAverageWindow)
		{
			Vector3 vector = _velocitySamples.Dequeue();
			_velocitySum -= vector;
		}
		if (_velocitySamples.Count <= 0)
		{
			return newVelocity;
		}
		return _velocitySum / _velocitySamples.Count;
	}

	private void CheckSetCameraShake()
	{
		if (carRigidBody == null)
		{
			return;
		}
		Vector3 velocity = carRigidBody.velocity;
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector3 vector = UpdateAndGetAveragedVelocity(velocity);
		if (_hasAveragedPrev)
		{
			currentAcceleration = (vector - _lastAveragedVelocity) / fixedDeltaTime;
		}
		else
		{
			currentAcceleration = Vector3.zero;
			_hasAveragedPrev = true;
		}
		localAcceleration = carRigidBody.transform.InverseTransformDirection(currentAcceleration);
		_lastAveragedVelocity = vector;
		if (_isShaking)
		{
			return;
		}
		float magnitude = velocity.magnitude;
		bool flag = magnitude > 5f;
		if ((_shakeOnKerb & flag) && OnKerb())
		{
			SetCameraShake(0.3f * _shakeMultiplier, 12f, 0.1f, magnitude * speedDependentCorrection);
			return;
		}
		if (drivetrain.CurrentRpm > drivetrain.RedlineRpm)
		{
			SetCameraShake(0.2f * _shakeMultiplier, 40f, 0.2f);
			return;
		}
		if ((_shakeOnOffRoad & flag) && OutOfRoad())
		{
			SetCameraShake(1f * _shakeMultiplier, 1f, 0.2f, magnitude * speedDependentCorrection);
			return;
		}
		float num = Mathf.Abs(localAcceleration.x);
		float num2 = Mathf.Abs(localAcceleration.z);
		if (num > accTrehsoldX)
		{
			SetCameraShake(0.02f * localAcceleration.x * _shakeMultiplier, 0.1f * localAcceleration.x, 0.1f, magnitude * speedDependentCorrection);
		}
		else if (num2 > accTrehsoldZ)
		{
			if (localAcceleration.z > 0f)
			{
				SetCameraShake(0.02f * localAcceleration.z * _shakeMultiplier, 0.5f * localAcceleration.z, 0.2f);
			}
			else
			{
				SetCameraShake(0.05f * localAcceleration.z * _shakeMultiplier, 0.2f * localAcceleration.z, 0.2f);
			}
		}
		else if (flag && magnitude > highSpeedStartEffect)
		{
			float num3 = magnitude - highSpeedStartEffect;
			SetCameraShake(0.01f * _shakeMultiplier, 0.03f, 0.2f, num3 * highSpeedDependentCorrection);
		}
	}

	private bool OutOfRoad()
	{
		foreach (WheelProvider allWheel in axles.AllWheels)
		{
			Surface wheelSurface = allWheel.WheelSurface;
			if (wheelSurface != Surface.Dryasphalt && wheelSurface != Surface.Wetasphalt)
			{
				return true;
			}
		}
		return false;
	}

	private bool OnKerb()
	{
		foreach (WheelProvider allWheel in axles.AllWheels)
		{
			if (allWheel.WheelSurface == Surface.Kerb)
			{
				return true;
			}
		}
		return false;
	}

	public void ApplyPreset(RacingCameraPreset p)
	{
		CloseView = new CameraViewConfig
		{
			Offset = p.CloseOffset,
			FOV = p.CloseFOV,
			ComposerOffset = p.ComposerOffset,
			SlopeUp = p.CloseSlopeUp,
			SlopeDown = p.CloseSlopeDown
		};
		MiddleView = new CameraViewConfig
		{
			Offset = p.MiddleOffset,
			FOV = p.MiddleFOV,
			ComposerOffset = p.ComposerOffset,
			SlopeUp = p.MiddleSlopeUp,
			SlopeDown = p.MiddleSlopeDown
		};
		FarView = new CameraViewConfig
		{
			Offset = p.FarOffset,
			FOV = p.FarFOV,
			ComposerOffset = p.ComposerOffset,
			SlopeUp = p.FarSlopeUp,
			SlopeDown = p.FarSlopeDown
		};
		rallyFollowSpeed = p.VelocityLookSpeed;
		rallyAngleSpeedMultiplier = p.RotationLerpSpeed;
		lookAtVelo = p.LookAtVelocity;
		_returnCameraToMainSpeed = p.RotationLerpSpeed;
		accelerationEffect = p.AccelerationEffect;
		maxPullback2 = p.MaxPullback;
		accelerationSmoothSpeed = p.AccelerationSmoothSpeed;
		_maxOffsetMoveSpeed = p.MaxOffsetMoveSpeed;
		startFOVmove = p.StartFOVSpeed;
		endFOVmove = p.EndFOVSpeed;
		fieldAdjustment = p.FieldAdjustment;
		speedFOV = p.SpeedFOV;
		_shakeTransitionSpeed = p.ShakeTransitionSpeed;
		_shakeMultiplier = p.ShakeMultiplier;
		_shakeOnKerb = p.ShakeOnKerb;
		_shakeOnOffRoad = p.ShakeOnOffRoad;
		_cockpitShockFilterTime = p.DriverViewShockFilterTime;
		_cockpitShockFilterClamp = p.DriverViewShockFilterClamp;
		_accelerationClamp = p.AccelerationClamp;
		CinemachineCollider component = virtualCamera.GetComponent<CinemachineCollider>();
		if (component != null)
		{
			component.m_CollideAgainst = p.CollideAgainstLayers;
			component.m_CameraRadius = p.CameraRadius;
			component.m_MinimumOcclusionTime = p.MinimumOcclusionTime;
			component.m_Damping = p.CollisionDamping;
			component.m_DampingWhenOccluded = p.CollisionDampingWhenOccluded;
		}
	}

	public void CaptureToPreset(RacingCameraPreset p)
	{
		p.CloseOffset = CloseView.Offset;
		p.CloseFOV = CloseView.FOV;
		p.CloseSlopeUp = CloseView.SlopeUp;
		p.CloseSlopeDown = CloseView.SlopeDown;
		p.MiddleOffset = MiddleView.Offset;
		p.MiddleFOV = MiddleView.FOV;
		p.MiddleSlopeUp = MiddleView.SlopeUp;
		p.MiddleSlopeDown = MiddleView.SlopeDown;
		p.FarOffset = FarView.Offset;
		p.FarFOV = FarView.FOV;
		p.FarSlopeUp = FarView.SlopeUp;
		p.FarSlopeDown = FarView.SlopeDown;
		p.ComposerOffset = CloseView.ComposerOffset;
		p.VelocityLookSpeed = rallyFollowSpeed;
		p.RotationLerpSpeed = rallyAngleSpeedMultiplier;
		p.LookAtVelocity = lookAtVelo;
		p.AccelerationEffect = accelerationEffect;
		p.MaxPullback = maxPullback2;
		p.AccelerationSmoothSpeed = accelerationSmoothSpeed;
		p.MaxOffsetMoveSpeed = _maxOffsetMoveSpeed;
		p.StartFOVSpeed = startFOVmove;
		p.EndFOVSpeed = endFOVmove;
		p.FieldAdjustment = fieldAdjustment;
		p.SpeedFOV = speedFOV;
		p.ShakeTransitionSpeed = _shakeTransitionSpeed;
	}

	private void OnDestroy()
	{
		_isDestoyed = true;
		_racingProvider.RacingCameraController = null;
		UnsubscribeFromCameraActions();
	}

	[Inject]
	public void ConstructControlsSettings(SteeringWheelValueEvent steeringWheelValueEvent, CarCamerasParametersConfig carCamerasParametersConfig, AlterChaseCameraConfig chaseFramingConfig, OptionalDependency<DesktopControlsSettingsSO> dep)
	{
		_steeringWheelValueEvent = steeringWheelValueEvent;
		_carCamerasParametersConfig = carCamerasParametersConfig;
		_chaseFramingConfig = chaseFramingConfig;
		if (dep != null && dep.HasValue)
		{
			_controlsSettings = dep.Value;
		}
	}

	private Vector2 ScaleUserLook(Vector2 look, CameraSettingsContext ctx)
	{
		if (!_controlsSettings)
		{
			return look;
		}
		return Vector2.Scale(look, _controlsSettings.GetLookScale(ctx));
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

	private CameraViewConfig DeriveChaseViewConfig(CameraViewConfig cfg, DragRacingCamera.CameraType type)
	{
		if (type != DragRacingCamera.CameraType.close && type != DragRacingCamera.CameraType.middle && type != DragRacingCamera.CameraType.far)
		{
			return cfg;
		}
		if (!_carCamerasParametersConfig || !_chaseFramingConfig || !V.LastUsedCar)
		{
			return cfg;
		}
		bool isTrack = (bool)carData && carData.isTruck;
		bool isTall = (bool)carData && CarIdStorage.TallCars.Contains(carData.CarId);
		_carCamerasParametersConfig.GetChaseParams(1, isTrack, isTall, out var distance, out var height, out var pitch);
		Transform transform = AlterChaseCameraConfig.FindAnchor(V.LastUsedCar.transform);
		float followDeltaY = (((bool)transform && (bool)mainCarToLook) ? (transform.position.y - mainCarToLook.position.y) : 0f);
		float aimDeltaY = (((bool)transform && (bool)targetGroup) ? (transform.position.y - targetGroup.transform.position.y) : 0f);
		float d = ((type == DragRacingCamera.CameraType.close) ? _chaseFramingConfig.NearDistance : _chaseFramingConfig.MiddleDistance);
		cfg.Offset = _chaseFramingConfig.FollowOffset(distance, height, pitch, followDeltaY, d);
		cfg.ComposerOffset = _chaseFramingConfig.ComposerOffset(height, pitch, aimDeltaY);
		cfg.FOV = _chaseFramingConfig.BaseFov;
		maxPullback2 = Mathf.Min(maxPullback2, _chaseFramingConfig.NearDistance - 0.1f);
		return cfg;
	}

	private void SubscribeToCameraActions()
	{
		_changeCameraHandle.Get(_eventRegistry).Subscribe(SetNextCam);
		_lookLeftHandle.Get(_eventRegistry).Subscribe(OnLookLeft);
		_lookRightHandle.Get(_eventRegistry).Subscribe(OnLookRight);
		_lookBackHandle.Get(_eventRegistry).Subscribe(OnLookBack);
		_inputMapSetEvent.Subscribe(OnInputGroupChanged);
		_onLookHandle.Get(_eventRegistry).Subscribe(OnLookHeldChanged);
		_onLookFreeHandle.Get(_eventRegistry).Subscribe(OnLookFreeHeldChanged);
	}

	private void UnsubscribeFromCameraActions()
	{
		_onLookFreeHandle.Get(_eventRegistry)?.Unsubscribe(OnLookFreeHeldChanged);
		_onLookHandle.Get(_eventRegistry)?.Unsubscribe(OnLookHeldChanged);
		_lookLeftHandle.Get(_eventRegistry)?.Unsubscribe(OnLookLeft);
		_lookRightHandle.Get(_eventRegistry)?.Unsubscribe(OnLookRight);
		_lookBackHandle.Get(_eventRegistry)?.Unsubscribe(OnLookBack);
		_changeCameraHandle.Get(_eventRegistry)?.Unsubscribe(SetNextCam);
		_inputMapSetEvent?.Unsubscribe(OnInputGroupChanged);
		_lookOverrideTween?.Kill();
		ClearPovBackSuppression();
	}

	private void MoveBodyByTouchOnGarageAlter(Transform movingNow)
	{
		if ((bool)bodyFollow && (bool)V.LastUsedCar && !isUITouched && (bool)movingNow)
		{
			float value = _zoomHandle.Get(_eventRegistry).Value;
			if (!_garageZoomInit)
			{
				_targetGarageZoom = garageZoomDistance;
				_garageZoomInit = true;
			}
			if (value != 0f)
			{
				_targetGarageZoom -= value * 3f;
				_targetGarageZoom = Mathf.Clamp(_targetGarageZoom, 2f, 10f);
			}
			garageZoomDistance = Mathf.SmoothDamp(garageZoomDistance, _targetGarageZoom, ref _garageZoomVel, 0.12f);
			Vector2 value2 = _lookHandle.Get(_eventRegistry).Value;
			float value3 = (garageZoomDistance - 2f) / 8f;
			value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(IsGamepadNow ? MultiplierPitGamepad : MultiplierPitMouse, value2);
			value2 = ScaleUserLook(value2, CameraSettingsContext.Garage);
			if (_lookHeld)
			{
				movedByPlayer = true;
				rotatingByTouchNow = true;
				timeStartTouch = Time.time;
				_cameraViewSettingsDefault = false;
				targetRot.y += speedY * value2.x;
				targetRot.x += speedX * value2.y;
				targetRot.x = Mathf.Clamp(targetRot.x, 0f, 70f);
				targetRot.z = 0f;
			}
			float num = Vector3.Distance(bodyFollow.transform.position, edgeTrackerTargetPos);
			float t = smoothSpeed4;
			if (currentCamera == DragRacingCamera.CameraType.garageView)
			{
				t = (3f + num) * smoothSpeed4;
			}
			eulerAngles = new Vector3(Mathf.Lerp(eulerAngles.x, targetRot.x, t), Mathf.Lerp(eulerAngles.y, targetRot.y, t), Mathf.Lerp(eulerAngles.z, targetRot.z, t));
			movingNow.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		}
	}

	private void MoveBodyByTouchRacingAlter(Transform movingNow)
	{
		if (_lookOverrideActive || !movingNow)
		{
			return;
		}
		float value = _zoomHandle.Get(_eventRegistry).Value;
		if (!_carZoomInit)
		{
			_targetChaseFov = camSettingField;
			_carZoomInit = true;
		}
		if (value != 0f)
		{
			_targetChaseFov -= value * 15f;
			_targetChaseFov = Mathf.Clamp(_targetChaseFov, 30f, 75f);
			_cameraViewSettingsDefault = false;
			PlayerPrefs.SetFloat($"racingCamFOV_{(int)currentCamera}", _targetChaseFov);
		}
		camSettingField = Mathf.SmoothDamp(camSettingField, _targetChaseFov, ref _carZoomVel, 0.12f);
		if ((bool)virtualCamera)
		{
			float b = (((bool)carData && (bool)carData.GetPowertrain) ? SpeedCameraFx.FovAdd(carData.GetPowertrain.SpeedMps, _speedFovConfig) : 0f);
			float num = (_speedFovConfig ? _speedFovConfig.FovLerpSpeed : 3f);
			_speedFovSmoothed = Mathf.Lerp(_speedFovSmoothed, b, Time.deltaTime * num);
			virtualCamera.m_Lens.FieldOfView = camSettingField + _speedFovSmoothed;
		}
		Vector2 value2 = _lookFreeHandle.Get(_eventRegistry).Value;
		float value3 = (CameraSettingOffset.z - -10f) / 9f;
		value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(IsGamepadNow ? MultiplierChaseGamepad : MultiplierChaseMouse, value2);
		value2 = ScaleUserLook(value2, CameraSettingsContext.Car);
		bool flag = currentCamera == DragRacingCamera.CameraType.far;
		if (flag)
		{
			rotatingByTouchNow = false;
		}
		if (!_lookFreeHeld | flag)
		{
			if (flag || Time.time - timeStartTouch > lookHoldSeconds)
			{
				rotatingByTouchNow = false;
			}
		}
		else
		{
			if (!rotatingByTouchNow)
			{
				eulerAngles = bodyFollow.transform.eulerAngles;
				eulerAngles.x = ((eulerAngles.x > 180f) ? (eulerAngles.x - 360f) : eulerAngles.x);
				targetRot = eulerAngles;
				rotatingByTouchNow = true;
			}
			timeStartTouch = Time.time;
			_cameraViewSettingsDefault = false;
			targetRot.y += value2.x;
			targetRot.x -= value2.y;
			if (targetRot.x > 180f)
			{
				targetRot.x -= 360f;
			}
			if (targetRot.x < -180f)
			{
				targetRot.x += 360f;
			}
			targetRot.x = Mathf.Clamp(targetRot.x, -20f, 70f);
			targetRot.z = 0f;
		}
		movingNow.eulerAngles = targetRot;
	}

	private void MoveBodyByTouchOnDriverViewAlter(Transform movingNow)
	{
		if (_lookOverrideActive || !movingNow)
		{
			return;
		}
		float value = _zoomHandle.Get(_eventRegistry).Value;
		if (!_fovZoomInit)
		{
			_targetFovZoom = camSettingField;
			_fovZoomInit = true;
		}
		if (value != 0f)
		{
			_targetFovZoom -= value * 15f;
			_targetFovZoom = Mathf.Clamp(_targetFovZoom, 20f, 90f);
			_cameraViewSettingsDefault = false;
			PlayerPrefs.SetFloat($"racingCamFOV_{(int)currentCamera}", _targetFovZoom);
		}
		camSettingField = Mathf.SmoothDamp(camSettingField, _targetFovZoom, ref _fovZoomVel, 0.12f);
		Vector2 value2 = _lookFreeHandle.Get(_eventRegistry).Value;
		float value3 = (camSettingField - 20f) / 70f;
		value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(MultiplierFpv, value2);
		value2 = ScaleUserLook(value2, CameraSettingsContext.CarFpv);
		if (_lookFreeHeld)
		{
			rotatingByTouchNow = true;
			timeStartTouch = Time.time;
			_cameraViewSettingsDefault = false;
			_driverOffsetY += value2.x;
			_driverOffsetX -= value2.y;
			_driverOffsetX = Mathf.Clamp(_driverOffsetX, -2f, 80f);
			_driverOffsetY = Mathf.Clamp(_driverOffsetY, 0f - driverViewLimit, driverViewLimit);
			_driverViewIdleTime = 0f;
			_driverPitchVel = 0f;
			_driverYawVel = 0f;
		}
		else if (!IsCarMoving())
		{
			_driverViewIdleTime = 0f;
		}
		else
		{
			_driverViewIdleTime += Time.deltaTime;
			if (_driverViewIdleTime >= 1f)
			{
				_driverOffsetX = Mathf.SmoothDampAngle(_driverOffsetX, 6f, ref _driverPitchVel, 0.3f);
				_driverOffsetY = Mathf.SmoothDampAngle(_driverOffsetY, 0f, ref _driverYawVel, 0.3f);
			}
		}
		Vector3 vector = mainCarToLook.eulerAngles;
		movingNow.eulerAngles = new Vector3(vector.x + _driverOffsetX, vector.y + _driverOffsetY, vector.z);
	}

	private void OnLookHeldChanged(bool held)
	{
		_lookHeld = held;
	}

	private void OnLookFreeHeldChanged(bool held)
	{
		_lookFreeHeld = held;
	}

	private void OnLookLeft(bool s)
	{
		HandleArrow(s, LookArrow.Left, LookLeftYaw);
	}

	private void OnLookRight(bool s)
	{
		HandleArrow(s, LookArrow.Right, LookRightYaw);
	}

	private void OnLookBack(bool s)
	{
		HandleArrow(s, LookArrow.Back, LookBackYaw);
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
		if (arrow != LookArrow.Back || IsDrivingCam)
		{
			if (_lookActiveMode == LookMode.PovBack && arrow != LookArrow.Back)
			{
				EndLookBackPovTeleport();
			}
			_activeLookArrow = arrow;
			if (arrow == LookArrow.Back && currentCamera == DragRacingCamera.CameraType.driverView)
			{
				StartLookBackPovTeleport();
			}
			else
			{
				StartLookOverride(yaw);
			}
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
				_ = deubg;
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
				_ = deubg;
				return true;
			}
		}
		worldPos = transform.position - transform.forward * 2f + transform.up * 1f;
		_ = deubg;
		return true;
	}

	private void StartLookBackPovTeleport()
	{
		if (!V.Car || !V.Car.TryGetComponent<DriverController>(out var component) || component.camInsideView == null || !V.Car.TryGetComponent<VehicleData>(out var component2))
		{
			return;
		}
		_rearTransform = V.Car.transform;
		if (TryGetRearLookAnchor(component2, out var worldPos, out var worldRot))
		{
			_ = deubg;
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
			if (V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
			{
				component.camInsideView.transform.localPosition = _snapPovLocalPosition;
				component.camInsideView.transform.localEulerAngles = _snapPovEuler;
			}
			if (WheelConnected)
			{
				_driverOffsetY = 0f;
			}
			_lookOverrideActive = false;
			_lookActiveMode = LookMode.None;
			_rearTransform = null;
		}
	}

	private void StartLookOverride(float targetYawCarRelative)
	{
		if (!IsDrivingCam || !V.Car)
		{
			return;
		}
		_lookOverrideTween?.Kill();
		if (!_lookOverrideActive)
		{
			if (currentCamera == DragRacingCamera.CameraType.driverView)
			{
				if (!V.Car.TryGetComponent<DriverController>(out var component) || component.camInsideView == null)
				{
					return;
				}
				_lookActiveMode = LookMode.Pov;
				_snapPovEuler = component.camInsideView.transform.localEulerAngles;
				LookOverrideCurrentYaw = _snapPovEuler.y;
				if (!deubg)
				{
				}
			}
			else
			{
				_lookActiveMode = LookMode.ChaseCam;
				_snapYawAngle = LookOverrideCurrentYaw;
				LookOverrideCurrentYaw = _snapYawAngle;
				_ = deubg;
			}
			_lookOverrideActive = true;
		}
		float endValue = LookOverrideCurrentYaw + Mathf.DeltaAngle(LookOverrideCurrentYaw, targetYawCarRelative);
		_ = deubg;
		_lookOverrideTween = DOTween.To(() => LookOverrideCurrentYaw, (float v) =>
		{
			LookOverrideCurrentYaw = v;
			_ = deubg;
		}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			_ = deubg;
		});
	}

	private void EndLookOverride()
	{
		if (_lookOverrideActive)
		{
			_lookOverrideTween?.Kill();
			float target = (WheelConnected ? LookForwardYaw : ((_lookActiveMode == LookMode.Pov) ? _snapPovEuler.y : _snapYawAngle));
			float endValue = LookOverrideCurrentYaw + Mathf.DeltaAngle(LookOverrideCurrentYaw, target);
			_ = deubg;
			_lookOverrideTween = DOTween.To(() => LookOverrideCurrentYaw, (float v) =>
			{
				LookOverrideCurrentYaw = v;
				_ = deubg;
			}, endValue, 0.18f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
			{
				_lookOverrideActive = false;
				_lookActiveMode = LookMode.None;
				_ = deubg;
			});
		}
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
		_activeLookArrow = LookArrow.None;
		if (!V.Car || !V.Car.TryGetComponent<DriverController>(out var component))
		{
			return;
		}
		if (_lookActiveMode == LookMode.PovBack && component?.camInsideView != null)
		{
			component.camInsideView.transform.localPosition = _snapPovLocalPosition;
			component.camInsideView.transform.localEulerAngles = _snapPovEuler;
		}
		else if (restoreSnapshot && (bool)V.Car)
		{
			if (_lookActiveMode == LookMode.Pov && currentCamera == DragRacingCamera.CameraType.driverView && component?.camInsideView != null)
			{
				component.camInsideView.transform.localEulerAngles = _snapPovEuler;
			}
			else if (_lookActiveMode == LookMode.ChaseCam && currentCamera != DragRacingCamera.CameraType.driverView)
			{
				LookOverrideCurrentYaw = _snapYawAngle;
			}
		}
		_lookOverrideActive = false;
		_lookActiveMode = LookMode.None;
		_rearTransform = null;
	}

	private void LateUpdate()
	{
		if (!_lookOverrideActive || !V.Car)
		{
			return;
		}
		_ = deubg;
		DriverController component3;
		if (_lookActiveMode == LookMode.PovBack && currentCamera == DragRacingCamera.CameraType.driverView && (bool)_rearTransform)
		{
			if (V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView && V.Car.TryGetComponent<VehicleData>(out var component2) && TryGetRearLookAnchor(component2, out var worldPos, out var worldRot))
			{
				_ = deubg;
				component.camInsideView.transform.SetPositionAndRotation(worldPos, worldRot);
			}
		}
		else if (_lookActiveMode == LookMode.Pov && currentCamera == DragRacingCamera.CameraType.driverView && V.Car.TryGetComponent<DriverController>(out component3) && (bool)component3.camInsideView)
		{
			Vector3 localEulerAngles = component3.camInsideView.transform.localEulerAngles;
			component3.camInsideView.transform.localEulerAngles = new Vector3(localEulerAngles.x, LookOverrideCurrentYaw, localEulerAngles.z);
		}
	}
}
