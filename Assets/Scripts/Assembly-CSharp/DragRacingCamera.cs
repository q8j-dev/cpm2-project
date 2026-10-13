using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;
using _Car_Parking.Scripts.WindowSystem.Mediators;

public class DragRacingCamera : MonoBehaviour
{
	public enum CameraType
	{
		driverView = 0,
		close = 1,
		middle = 2,
		far = 3,
		garageView = 4,
		spectator = 5,
		freeCamera = 6,
		directionCamera = 7
	}

	[Serializable]
	private class CameraRotationData
	{
		public float x;

		public float y;
	}

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

	private const float MinCameraRenderDistance = 110f;

	private const string SlowMoHitEvent = "Play_Slowmo_Hit";

	private const string SlowMoWhooshEvent = "Play_slow_mo_whoosh";

	private const string SlowMoStateGroup = "State_Finish_Slowmo";

	private const string SlowMoStateOff = "Deactivate";

	private const int SlowMoStopFadeMs = 300;

	private const uint InvalidPlayingId = 0u;

	public CinemachineVirtualCamera virtualCamera;

	public CinemachineVirtualCamera virtualCameraDriverView;

	public CinemachineVirtualCamera virtualCameraGarage;

	public CinemachineVirtualCamera virtualCameraOnFinish;

	public CinemachineVirtualCamera virtualCameraOnTreeStart;

	public CinemachineVirtualCamera virtualCameraOnToLookOtherRace;

	public CinemachineVirtualCamera virtualCameraOnToLookOtherRace2;

	public CinemachineVirtualCamera virtualCameraWinners;

	public CinemachineVirtualCamera virtualCameraLoser;

	public CinemachineVirtualCamera veryFirstCamera;

	public CameraType currentCamera;

	public Vector3 targetOffset;

	public Vector3 CameraSettingOffset;

	public float targetField;

	public float camSettingField = 50f;

	private CinemachineTransposer ct;

	private CinemachineComposer CineComposer;

	private CinemachineOrbitalTransposer ctGarage;

	private CinemachineSameAsFollowTarget driverViewAim;

	public float smoothSpeed = 1f;

	public float smoothSpeed2 = 1f;

	public float smoothSpeed3 = 0.3f;

	private Powertrain _powertrain;

	private Axles axles;

	private VehicleData _vehicleData;

	public CinemachineBasicMultiChannelPerlin noise;

	public CinemachineBasicMultiChannelPerlin noiseDriverView;

	private float noiseAmplTarget;

	private float noiseFreqTarget;

	public NoiseSettings shakeProfile;

	public NoiseSettings handMildProfile;

	public CinemachineTargetGroup targetGroup;

	public CinemachineTargetGroup targetGroupOtherCar;

	public Transform bodyFollow;

	public DragRacingTree dragRacingTree;

	public DragRacingController controller;

	public Transform driverViewPoint;

	public float angle;

	public float LimitAngle = 60f;

	public bool lookForward = true;

	public float speedY = 1f;

	public float speedX = 1f;

	public float SpeedTouch = 4f;

	public float smoothSpeed4 = 0.4f;

	public float thresholdToch = 0.1f;

	private float acceleration;

	private float oldAcceleration;

	private Vector3 oldAcceleration2;

	public Vector3 accelerationMagnitude;

	public Vector3 localDir;

	private Rigidbody carRigidBody;

	private float lastTime;

	private float delta;

	private float lerpedDelta;

	public float speedAcc = 1f;

	public float maxAccOffset = 2f;

	public Vector3 targetRot;

	public Vector3 eulerAngles;

	public float ZoomSpeed = 1f;

	private Camera ourCam;

	private TouchMovePerson _touchMovePerson;

	public bool rotatingByTouchNow;

	private float timeStartTouch;

	public Transform RacingCarToLook;

	public Transform mainCarToLook;

	private float timerUpdateSpectatorCam;

	private Vector3 targetOffsetSpecCam;

	private CinemachineOrbitalTransposer ctSpecCar;

	public float speedSpecCar;

	public int startFrame;

	private float fixedDeltaTime;

	private uint _slowMoPlayingId;

	public float diffAngle;

	public Vector2 posTouch;

	private LayerMask _carMask;

	private LayerMask _ignoreCollisionMask;

	public Transform edgeTracker;

	private bool movedByPlayer;

	public float distanceMag = 1f;

	public float garageDistanceCollider = 1f;

	public float garageZoomDistance = 1f;

	private float _garageBaseY;

	public Vector3 edgeTrackerTargetPos;

	public EventSystem eventSystem;

	public float driverViewLimit = 90f;

	private bool isUITouched;

	private bool edgeReached;

	public float baseRotationSpeedSpectatorView = 2f;

	public float speedYSpectatorView = 0.5f;

	private float targetY;

	private Quaternion targetRotation;

	[SerializeField]
	private List<CameraRotationData> randomPositions;

	[SerializeField]
	private Vector3 PlayerBurnOutOffset = new Vector3(0f, 1.5f, -4.5f);

	private IDisposable _removeCameraTextTypeDisposable;

	private UIReferencesMediator _uiReferencesMediator;

	private OptionalDependency<UIReferencesMediator> _uiReferencesMediatorDep;

	private bool _cameraViewSettingsDefault = true;

	private RacingProvider _racingProvider;

	private CarInputManager _carInputManager;

	private MainPerson _mainPerson;

	private CinemachineBrain _cinemachineBrain;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private IDisposable _setHeadVisibleDisposable;

	private SoundControllerExtension _carInOutSound;

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

	private InputDeviceDetector _inputDeviceDetector;

	private DesktopControlsSettingsSO _controlsSettings;

	private CarCamerasParametersConfig _carCamerasParametersConfig;

	private AlterChaseCameraConfig _chaseFramingConfig;

	private static readonly Vector2 GarageViewLookScale = new Vector2(2f, 1.4f);

	private const float GarageViewGamepadScale = 0.28f;

	private const float GarageZoomSpeed = 4f;

	private const float GarageZoomGamepadScale = 0.1f;

	private const float SpectatorHeightPerLook = 0.15f;

	private const float SpectatorHeightMax = 12f;

	private const float SpectatorZoomSpeed = 4f;

	private const float SpectatorZoomGamepadScale = 0.1f;

	private const float SpectatorZoomNear = -3f;

	private const float SpectatorZoomFar = -12f;

	private float _targetSpectatorZoom;

	private float _spectatorZoomVel;

	private bool _spectatorZoomInit;

	[SerializeField]
	private SteamSpeedCameraConfig _speedFovConfig;

	private float _driverOffsetX;

	private float _driverOffsetY;

	[SerializeField]
	private bool deubg;

	private LookMode _lookActiveMode;

	private LookArrow _activeLookArrow;

	private bool _lookHeld;

	private bool _lookFree;

	private bool _lookOverrideActive;

	private Tween _lookOverrideTween;

	private float _snapYawAngle;

	private Vector3 _snapPovEuler;

	private Vector3 _snapPovLocalPosition;

	private Transform _rearTransform;

	private DriverController _povBackDriver;

	private const float LookTransitionTime = 0.18f;

	private static readonly Vector2 Multiplier = new Vector2(1.08f, 0.756f);

	private static readonly Vector2 MultiplierPit = new Vector2(1.08f, 0.756f);

	private static readonly Vector2 MultiplierChase = new Vector2(2.2f, 1.5f);

	private static readonly Vector2 MultiplierFpv = new Vector2(1.8f, 1.2f);

	private const float DriverViewRecenterDelay = 1f;

	private const float DriverViewRecenterSmoothTime = 0.3f;

	private const float DriverViewRestPitch = 6f;

	private const float RecenterMinSpeedKmh = 3f;

	private float _driverViewIdleTime;

	private float _driverPitchVel;

	private float _driverYawVel;

	protected const float MinLookFactor = 0.15f;

	private const float ZoomSpeedAlter = 100f;

	private const float GarageZoomMin = 2f;

	private const float GarageZoomMax = 10f;

	private const float CarZoomMin = -10f;

	private const float CarZoomMax = -1f;

	private const float DriverViewZoomMin = 20f;

	private const float DriverViewZoomMax = 90f;

	private const float ZoomSmoothTime = 0.12f;

	private const float DriverViewZoomSpeed = 25f;

	private const float CarFovZoomSpeed = 15f;

	private const float ChaseFovMin = 30f;

	private const float ChaseFovMax = 75f;

	private float _targetChaseFov;

	private float _carZoomVel;

	private float _targetGarageZoom;

	private float _garageZoomVel;

	private float _targetFovZoom;

	private float _fovZoomVel;

	private bool _carZoomInit;

	private bool _garageZoomInit;

	private bool _fovZoomInit;

	private const float RearHeightFactor = 0.55f;

	private const float RearClearance = 0.05f;

	private const float RearOverhang = 0.6f;

	private const float RearHeightAboveAxle = 1f;

	private static readonly HashSet<CameraType> DrivingCamTypes = new HashSet<CameraType>
	{
		CameraType.driverView,
		CameraType.close,
		CameraType.middle,
		CameraType.far
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

	private float LookOverrideCurrentYaw
	{
		get
		{
			switch (currentCamera)
			{
			case CameraType.driverView:
				return _driverOffsetY;
			case CameraType.close:
			case CameraType.middle:
			case CameraType.far:
				return Mathf.DeltaAngle(V.Car.transform.eulerAngles.y, targetRot.y);
			default:
				return 0f;
			}
		}
		set
		{
			switch (currentCamera)
			{
			case CameraType.driverView:
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
			case CameraType.close:
			case CameraType.middle:
			case CameraType.far:
				if ((bool)bodyFollow && (bool)V.LastUsedCar)
				{
					targetRot = V.Car.transform.eulerAngles;
					targetRot.y += value;
					targetRot.z = 0f;
					targetRot.x = 0f;
					bodyFollow.eulerAngles = targetRot;
				}
				break;
			case CameraType.garageView:
			case CameraType.spectator:
			case CameraType.freeCamera:
			case CameraType.directionCamera:
				break;
			}
		}
	}

	[Inject]
	public void Construct(RacingProvider racingProvider, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, CarInputManager carInputManager, OptionalDependency<UIReferencesMediator> uiReferencesMediator, SoundControllerExtension carInOutSound)
	{
		_carInOutSound = carInOutSound;
		_racingProvider = racingProvider;
		_uiReferencesMediatorDep = uiReferencesMediator;
		_carInputManager = carInputManager;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void SetIgnoreCollisionCameras()
	{
		if ((bool)virtualCamera)
		{
			virtualCamera.GetComponent<CinemachineCollider>().m_TransparentLayers = _ignoreCollisionMask;
		}
		if ((bool)virtualCameraGarage)
		{
			virtualCameraGarage.GetComponent<CinemachineCollider>().m_TransparentLayers = _ignoreCollisionMask;
		}
	}

	private void Awake()
	{
		_uiReferencesMediator = _uiReferencesMediatorDep?.Value;
		string[] layerNames = new string[2] { "Load", "parktronic" };
		_carMask = LayerMask.GetMask("car");
		_ignoreCollisionMask = LayerMask.GetMask(layerNames);
		SpeedTouch = 0.5f;
		fixedDeltaTime = Time.fixedDeltaTime;
		smoothSpeed4 = 2f;
		ourCam = GetComponent<Camera>();
		ct = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		CineComposer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
		ctGarage = virtualCameraGarage.GetCinemachineComponent<CinemachineOrbitalTransposer>();
		_garageBaseY = ctGarage.m_FollowOffset.y;
		ctSpecCar = virtualCameraOnToLookOtherRace.GetCinemachineComponent<CinemachineOrbitalTransposer>();
		driverViewAim = virtualCamera.GetCinemachineComponent<CinemachineSameAsFollowTarget>();
		noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noiseDriverView = virtualCameraDriverView.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		if (!(V.Car == null))
		{
			_powertrain = V.Car.GetComponent<Powertrain>();
			axles = V.Car.GetComponent<Axles>();
			_vehicleData = V.Car.GetComponent<VehicleData>();
			carRigidBody = V.Car.GetComponent<Rigidbody>();
			_touchMovePerson = V.DriverPerson.GetComponent<TouchMovePerson>();
			_touchMovePerson.CamPosLerpSpeed = 7f;
			startFrame = Time.frameCount;
			DisableAllCamera();
			RandomUpdateCameraOnGaragePos().Forget();
			veryFirstCamera.enabled = true;
			SetIgnoreCollisionCameras();
			CPMGraphicsSettingsManager.Instance.RenderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.DragRacing, 110f, GraphicsDistanceController.SetType.SetIfLarger);
			_mainPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
			_cinemachineBrain = GetComponent<CinemachineBrain>();
			SubscribeToCameraActions();
		}
	}

	public async UniTask StartMoveVeryFirstCam()
	{
		veryFirstCamera.enabled = true;
		CinemachineDollyCart dollyCart = veryFirstCamera.Follow.GetComponent<CinemachineDollyCart>();
		dollyCart.enabled = false;
		dollyCart.m_Position = 0f;
		dollyCart.m_Speed = 0f;
		CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
		dragRacingTree.BlinkRightSideMats(destroyToken).Forget();
		bool readyAllBody = false;
		int count = 0;
		while (!readyAllBody)
		{
			if (!veryFirstCamera || !veryFirstCamera.enabled)
			{
				return;
			}
			readyAllBody = controller.AllCarBodyReady();
			count++;
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
		}
		if (count < 3)
		{
			await UniTask.Delay(300, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
		}
		if (!dollyCart)
		{
			return;
		}
		dollyCart.enabled = true;
		while ((bool)dollyCart && dollyCart.m_Position < dollyCart.m_Path.PathLength && (bool)veryFirstCamera && veryFirstCamera.enabled)
		{
			float num = dollyCart.m_Position / dollyCart.m_Path.PathLength;
			dollyCart.m_Speed = ((num > 0.48f) ? (2f + 5f * (2f - num)) : 40f);
			if (Input.touchCount > 0)
			{
				break;
			}
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, destroyToken);
		}
		if ((bool)veryFirstCamera)
		{
			veryFirstCamera.enabled = false;
		}
	}

	private void OnDestroy()
	{
		AudioWrapper.StopAll(base.gameObject);
		UnsubscribeFromCameraActions();
	}

	public void LookAtOtherRacing()
	{
		if ((bool)controller.leftCar && (bool)controller.rightCar)
		{
			DisableAllCamera();
			currentCamera = CameraType.spectator;
			virtualCameraOnToLookOtherRace2.enabled = false;
			virtualCameraOnToLookOtherRace.enabled = true;
			virtualCameraOnToLookOtherRace.GetComponent<CinemachineVirtualCamera>().LookAt = targetGroupOtherCar.transform;
			virtualCameraOnToLookOtherRace.GetComponent<CinemachineVirtualCamera>().m_Follow = bodyFollow;
			CinemachineTargetGroup.Target[] array = new CinemachineTargetGroup.Target[2];
			array[0].target = controller.leftCar.transform;
			array[0].weight = 1f;
			array[1].target = controller.rightCar.transform;
			array[1].weight = 1f;
			targetGroupOtherCar.m_Targets = array;
			ctSpecCar.m_FollowOffset.y = 2f;
		}
	}

	public async void setCamera(GameObject target)
	{
		CinemachineTargetGroup.Target[] t = new CinemachineTargetGroup.Target[2];
		if ((bool)target.GetComponent<VehicleData>())
		{
			VehicleData vehicleDynamics = target.GetComponent<VehicleData>();
			while (vehicleDynamics.Collider == null)
			{
				MonoBehaviour.print("waiting center of mass");
				await UniTask.Delay(100);
			}
			mainCarToLook = _vehicleData.Collider.transform;
		}
		mainCarToLook = target.transform;
		t[0].target = target.transform;
		t[0].weight = 1f;
		targetGroup.m_Targets = t;
		virtualCamera.enabled = true;
		virtualCameraGarage.enabled = false;
		virtualCameraOnToLookOtherRace.enabled = false;
		virtualCamera.LookAt = targetGroup.transform;
		virtualCamera.Follow = bodyFollow.transform;
		virtualCameraGarage.LookAt = edgeTracker;
		virtualCameraGarage.Follow = bodyFollow.transform;
		targetRotation = bodyFollow.rotation;
		if ((bool)target.GetComponent<DriverController>().camInsideView)
		{
			virtualCameraDriverView.Follow = target.GetComponent<DriverController>().camInsideView.transform;
			GameObject gameObject = new GameObject("DriverViewOrigin");
			driverViewPoint = gameObject.transform;
			driverViewPoint.position = virtualCameraDriverView.Follow.position;
			driverViewPoint.localRotation = virtualCameraDriverView.Follow.localRotation;
			driverViewPoint.parent = virtualCameraDriverView.Follow.parent;
		}
	}

	private async UniTaskVoid RandomUpdateCameraOnGaragePos()
	{
		do
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
			if (veryFirstCamera == null)
			{
				break;
			}
			if (!veryFirstCamera.enabled)
			{
				if (currentCamera == CameraType.garageView && !movedByPlayer)
				{
					CameraRotationData cameraRotationData = randomPositions[UnityEngine.Random.Range(0, randomPositions.Count)];
					float x = cameraRotationData.x;
					float y = cameraRotationData.y;
					Vector3 vector = new Vector3(x, y, 0f);
					targetRot = vector;
					rotatingByTouchNow = true;
					timeStartTouch = Time.time;
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(10000.0));
			}
		}
		while (FlagsStorage.RacingGameMode);
	}

	private void moveBodyByTouchOnDriverView()
	{
		MoveBodyByTouchOnDriverViewAlter(virtualCameraDriverView.Follow);
	}

	private void moveBodyFollowDriverView()
	{
		if (currentCamera == CameraType.driverView)
		{
			MainPerson component = V.DriverPerson.GetComponent<MainPerson>();
			Vector3 position = component.ik.references.head.position;
			Vector2 b = new Vector2(position.z, position.x);
			position = component.ik.references.root.position;
			float num = Vector2.Distance(new Vector2(position.z, position.x), b);
			_touchMovePerson.CameraDistance = (0f - num) * 5f - 0.1f;
			virtualCameraOnTreeStart.enabled = false;
		}
		if (rotatingByTouchNow)
		{
			return;
		}
		targetField = camSettingField;
		Transform follow = virtualCameraDriverView.Follow;
		follow.transform.position = driverViewPoint.transform.position;
		lookForward = true;
		if (_vehicleData.racingState == RacingState.staged || _vehicleData.racingState == RacingState.preLaunch || Time.time - dragRacingTree.launchTime < 2f)
		{
			follow.transform.LookAt(dragRacingTree.toLookForCam);
			follow.transform.position = driverViewPoint.transform.position + driverViewPoint.transform.forward * 0.6f;
			targetField = 30f;
			lookForward = false;
		}
		else if (_vehicleData.racingState == RacingState.OnRacing)
		{
			if ((bool)RacingCarToLook && controller.autoSteering && Vector3.Distance(RacingCarToLook.transform.position, mainCarToLook.position) < 40f)
			{
				follow.transform.LookAt(RacingCarToLook.transform);
				lookForward = false;
			}
		}
		else if (_vehicleData.racingState == RacingState.rollingToStart && (bool)RacingCarToLook && RacingCarToLook.GetComponent<Powertrain>().GearboxState == GearboxState.Burnout && Vector3.Distance(RacingCarToLook.transform.position, mainCarToLook.position) < 10f)
		{
			follow.transform.LookAt(RacingCarToLook.transform);
			lookForward = false;
		}
		angle = Quaternion.Angle(follow.localRotation, Quaternion.identity);
		if (angle > LimitAngle)
		{
			lookForward = true;
		}
		if (lookForward)
		{
			follow.transform.LookAt(mainCarToLook.position + mainCarToLook.forward * 10f);
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
		if (currentCamera == CameraType.driverView)
		{
			camSettingField += num * ZoomSpeed * 0.1f;
			return;
		}
		if (currentCamera == CameraType.spectator)
		{
			ctSpecCar.m_FollowOffset.z -= num * ZoomSpeed * 0.1f;
			ctSpecCar.m_FollowOffset.z = Mathf.Clamp(ctSpecCar.m_FollowOffset.z, -12f, -3f);
			return;
		}
		garageZoomDistance += num * ZoomSpeed * 0.1f;
		CameraSettingOffset.z -= num * ZoomSpeed * 0.1f;
		CameraSettingOffset.z = Mathf.Clamp(CameraSettingOffset.z, -10f, -1f);
		garageZoomDistance = Mathf.Clamp(garageZoomDistance, -2f, 10f);
	}

	private void moveBodyByTouchOnGarage()
	{
		MoveBodyByTouchOnGarageAlter(bodyFollow);
	}

	private void moveBodyByTouchRacing()
	{
		MoveBodyByTouchRacingAlter(bodyFollow);
	}

	private void moveBodyGarageView()
	{
		if (!bodyFollow || !V.LastUsedCar)
		{
			return;
		}
		if (rotatingByTouchNow)
		{
			Vector3 position = ourCam.transform.position;
			Vector3 direction = _vehicleData.Collider.transform.position - position;
			if (Physics.Raycast(position, direction, out var hitInfo, 300f, _carMask, QueryTriggerInteraction.Ignore))
			{
				edgeTrackerTargetPos = hitInfo.point;
			}
		}
		float value = garageZoomDistance + garageDistanceCollider;
		value = Mathf.Clamp(value, 1f, 10f);
		float f = eulerAngles.x * ((float)Math.PI / 180f);
		ctGarage.m_FollowOffset = new Vector3(0f, Mathf.Sin(f) * value + _garageBaseY * Mathf.Cos(f), Mathf.Cos(f) * value - Mathf.Sin(f) * _garageBaseY);
		targetField = camSettingField;
		targetOffset = CameraSettingOffset + accelerationMagnitude;
		virtualCameraOnTreeStart.enabled = false;
		float num = Vector3.Distance(edgeTracker.transform.position, edgeTrackerTargetPos);
		if (edgeReached)
		{
			edgeReached = num < 0.5f;
		}
		if (!edgeReached)
		{
			float num2 = Mathf.Clamp(num, 0.5f, 3f) * 10f;
			edgeTracker.transform.position = Vector3.Lerp(edgeTracker.transform.position, edgeTrackerTargetPos, Time.deltaTime * num2);
			garageDistanceCollider = Vector3.Distance(edgeTracker.transform.position, _vehicleData.Collider.transform.position) * distanceMag;
			garageDistanceCollider = Mathf.Clamp(garageDistanceCollider, -7f, 7f);
			if (num < 0.2f)
			{
				edgeReached = true;
			}
		}
		bodyFollow.transform.position = edgeTracker.transform.position;
	}

	private void moveBodyFollow()
	{
		if (!bodyFollow || !V.LastUsedCar)
		{
			return;
		}
		targetField = camSettingField;
		targetOffset = CameraSettingOffset + accelerationMagnitude;
		virtualCameraOnTreeStart.enabled = false;
		targetGroup.m_Targets[1].weight = 0f;
		if ((_vehicleData.racingState == RacingState.staged || _vehicleData.racingState == RacingState.preLaunch || Time.time - dragRacingTree.launchTime < 0.5f) && !V.DriverPerson.GetComponent<TouchMovePerson>().enabled)
		{
			virtualCameraOnTreeStart.enabled = true;
			virtualCameraOnTreeStart.transform.position = driverViewPoint.transform.position + driverViewPoint.transform.forward * 0.3f;
			virtualCameraOnTreeStart.transform.LookAt(dragRacingTree.toLookForCam);
			targetField = 30f;
			return;
		}
		bodyFollow.transform.position = mainCarToLook.position;
		if (rotatingByTouchNow || !_racingProvider.DragRacingController.autoSteering)
		{
			return;
		}
		if (_vehicleData.racingState == RacingState.rollingToStart)
		{
			if (_powertrain.GearboxState == GearboxState.Burnout)
			{
				smoothSpeed = 0.1f;
				if (_powertrain.GearType == GearboxType.GearFwd)
				{
					bodyFollow.transform.LookAt(_powertrain.GetAxles.RearAxle.LeftWheel.transform);
				}
				else
				{
					bodyFollow.transform.LookAt(mainCarToLook.right * 3f + mainCarToLook.position);
				}
				targetOffset = PlayerBurnOutOffset;
				targetField = 40f;
			}
			else if ((bool)RacingCarToLook && controller.autoSteering && RacingCarToLook.GetComponent<Powertrain>().GearboxState == GearboxState.Burnout && Vector3.Distance(RacingCarToLook.transform.position, mainCarToLook.position) < 10f)
			{
				bodyFollow.transform.LookAt(RacingCarToLook.transform);
			}
			else
			{
				bodyFollow.transform.LookAt(dragRacingTree.toLookForCam);
			}
			return;
		}
		if (_vehicleData.racingState == RacingState.OnRacing && (bool)RacingCarToLook && controller.autoSteering)
		{
			float num = Vector3.Distance(RacingCarToLook.transform.position, mainCarToLook.position);
			if (num < 40f && Time.time - dragRacingTree.launchTime > 3f)
			{
				Vector3 position = RacingCarToLook.transform.position;
				position.y = bodyFollow.transform.position.y - 2f;
				bodyFollow.transform.LookAt(position);
				targetField = 80f - num;
				targetGroup.m_Targets[1].weight = 1f;
				return;
			}
		}
		bodyFollow.transform.rotation = mainCarToLook.rotation;
		eulerAngles = bodyFollow.transform.eulerAngles;
		targetRot = eulerAngles;
	}

	private void FixedUpdate()
	{
		CheckAccelerationCar();
	}

	private void CheckAccelerationCar()
	{
		if (!(carRigidBody == null))
		{
			float magnitude = carRigidBody.velocity.magnitude;
			delta = (0f - (magnitude - oldAcceleration) / Time.deltaTime) * 0.1f;
			if (magnitude < 10f)
			{
				delta = 0f;
			}
			delta = Mathf.Clamp(delta, 0f - maxAccOffset, maxAccOffset);
			oldAcceleration = magnitude;
			Vector3 normalized = carRigidBody.velocity.normalized;
			localDir = bodyFollow.transform.InverseTransformDirection(normalized);
			lerpedDelta = Mathf.MoveTowards(lerpedDelta, delta, Time.deltaTime * speedAcc);
			accelerationMagnitude = localDir * lerpedDelta;
		}
	}

	private void MoveSpectatorCam()
	{
		if ((bool)controller.leftCar)
		{
			VehicleData component = controller.leftCar.GetComponent<VehicleData>();
			if (component.racingState != RacingState.toGarage)
			{
				bool flag = component.racingState == RacingState.rollingToStart || component.racingState == RacingState.preLaunch || component.racingState == RacingState.staged;
				virtualCameraOnTreeStart.enabled = flag;
				virtualCameraOnToLookOtherRace.enabled = !flag;
				if (flag && (bool)dragRacingTree && (bool)dragRacingTree.toLookForCam)
				{
					virtualCameraOnTreeStart.LookAt = dragRacingTree.toLookForCam;
				}
			}
			float num = Vector3.Distance(controller.leftCar.transform.position, controller.rightCar.transform.position);
			float weight = Mathf.Clamp(1f - num / 30f, 0f, 0.6f);
			targetGroupOtherCar.m_Targets[1].weight = weight;
			bodyFollow.position = controller.leftCar.transform.position;
		}
		if (isUITouched)
		{
			return;
		}
		if (Input.touchCount > 0)
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				float num2 = baseRotationSpeedSpectatorView * (Screen.dpi / 160f);
				if (touch.phase == TouchPhase.Moved)
				{
					float num3 = touch.deltaPosition.x * num2 * Time.deltaTime;
					targetY = touch.deltaPosition.y * speedYSpectatorView * Time.deltaTime + targetY;
					targetY = Mathf.Clamp(targetY, 0f, 12f);
					targetRotation *= Quaternion.Euler(Vector3.up * num3);
				}
			}
			else if (Input.touchCount == 2)
			{
				zooming();
			}
		}
		MoveSpectatorCamAlter();
		ctSpecCar.m_FollowOffset.y = Mathf.Lerp(ctSpecCar.m_FollowOffset.y, targetY, 3f * Time.deltaTime);
		bodyFollow.rotation = targetRotation;
	}

	private void Update()
	{
		if (Time.frameCount - startFrame >= 10)
		{
			CheckForUITouch();
			if (currentCamera == CameraType.spectator)
			{
				MoveSpectatorCam();
			}
			else if (currentCamera == CameraType.driverView)
			{
				moveBodyByTouchOnDriverView();
				moveBodyFollowDriverView();
			}
			else if (currentCamera == CameraType.garageView)
			{
				moveBodyByTouchOnGarage();
				moveBodyGarageView();
			}
			else
			{
				moveBodyByTouchRacing();
				moveBodyFollow();
			}
			ShakeEffectCam();
		}
	}

	private void CheckForUITouch()
	{
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began && eventSystem.IsPointerOverGameObject())
			{
				isUITouched = true;
			}
		}
		else
		{
			isUITouched = false;
		}
	}

	private void ShakeEffectCam()
	{
		if ((bool)axles)
		{
			bool flag = axles.RearAxle.LeftWheel.Wheel.GetSmoke() || axles.RearAxle.RightWheel.Wheel.GetSmoke() || axles.FrontAxle.LeftWheel.Wheel.GetSmoke() || axles.FrontAxle.RightWheel.Wheel.GetSmoke();
			_powertrain.GetLaunchControlSettings(out var _, out var lcRpm, out var _, out var _);
			if (_powertrain.IsActiveLaunchControlNow && _powertrain.CurrentRpm.AlmostEquals(lcRpm, 200f))
			{
				noiseAmplTarget = 0.1f;
				noiseFreqTarget = 12f;
				noise.m_NoiseProfile = shakeProfile;
			}
			else if (flag)
			{
				noiseAmplTarget = 0.5f;
				noiseFreqTarget = 3f;
				noise.m_NoiseProfile = handMildProfile;
			}
			else if (_powertrain.CurrentRpm.AlmostEquals(_powertrain.RedlineRpm, 200f) || (_powertrain.IsShifting && _powertrain.CurrentRpm > 4000f))
			{
				noiseAmplTarget = 0.1f;
				noiseFreqTarget = 8f;
				noise.m_NoiseProfile = shakeProfile;
			}
			else
			{
				noiseAmplTarget = 0.5f + _powertrain.SpeedMps / 200f;
				noiseFreqTarget = 0.5f + _powertrain.SpeedMps / 200f;
				noise.m_NoiseProfile = handMildProfile;
			}
			noiseDriverView.m_NoiseProfile = noise.m_NoiseProfile;
			ct.m_FollowOffset = Vector3.MoveTowards(ct.m_FollowOffset, targetOffset, smoothSpeed);
			float num = targetField;
			num += SpeedCameraFx.FovAdd(_powertrain.SpeedMps, _speedFovConfig);
			virtualCamera.m_Lens.FieldOfView = Mathf.MoveTowards(virtualCamera.m_Lens.FieldOfView, num, smoothSpeed2);
			virtualCameraDriverView.m_Lens.FieldOfView = virtualCamera.m_Lens.FieldOfView;
			virtualCameraOnTreeStart.m_Lens.FieldOfView = virtualCamera.m_Lens.FieldOfView;
			if (currentCamera == CameraType.driverView)
			{
				noiseDriverView.m_AmplitudeGain = Mathf.MoveTowards(noiseDriverView.m_AmplitudeGain, noiseAmplTarget * 0.3f, smoothSpeed3);
				noiseDriverView.m_FrequencyGain = Mathf.MoveTowards(noiseDriverView.m_FrequencyGain, noiseFreqTarget * 0.3f, smoothSpeed3);
			}
			else
			{
				noise.m_AmplitudeGain = Mathf.Min(Mathf.MoveTowards(noise.m_AmplitudeGain, noiseAmplTarget, smoothSpeed3), 0.3f);
				noise.m_FrequencyGain = Mathf.MoveTowards(noise.m_FrequencyGain, noiseFreqTarget, smoothSpeed3);
			}
		}
	}

	public void OnCloseFinish()
	{
		Time.timeScale = 0f;
		virtualCameraOnFinish.enabled = true;
		ResetCamAsync(this.GetCancellationTokenOnDestroy()).Forget();
		controller.dragRacingUI.menuList[1].SetActive(value: false);
	}

	private async UniTask ResetCamAsync(CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			StopSlowMoAudio();
			_slowMoPlayingId = AudioWrapper.PostEvent("Play_Slowmo_Hit", base.gameObject);
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), DelayType.Realtime, PlayerLoopTiming.Update, ct);
			Time.timeScale = 0.05f;
			Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
			await UniTask.Delay(TimeSpan.FromSeconds(4.0), DelayType.Realtime, PlayerLoopTiming.Update, ct);
			AudioWrapper.PostEvent("Play_slow_mo_whoosh", base.gameObject);
			controller.dragRacingUI.SmoothListerVolume(ct).Forget();
			virtualCameraOnFinish.enabled = false;
			controller.dragRacingUI.menuList[1].SetActive(value: true);
		}
		finally
		{
			StopSlowMoAudio();
			Time.timeScale = 1f;
			Time.fixedDeltaTime = fixedDeltaTime;
		}
	}

	private void StopSlowMoAudio()
	{
		if (_slowMoPlayingId != 0)
		{
			AudioWrapper.StopPlayingId(_slowMoPlayingId, 300);
			_slowMoPlayingId = 0u;
		}
		AudioWrapper.SetState("State_Finish_Slowmo", "Deactivate");
	}

	public void SetCameraForRacing()
	{
		veryFirstCamera.enabled = false;
		virtualCamera.enabled = true;
		virtualCameraGarage.enabled = false;
		virtualCameraOnFinish.enabled = false;
		currentCamera = (CameraType)PlayerPrefs.GetInt("CarCameraRacing", 1);
		ChangeCameraOnCar(currentCamera);
		rotatingByTouchNow = false;
		if ((bool)controller.nextRivalCar)
		{
			targetGroup.m_Targets[1].target = controller.nextRivalCar.transform;
			targetGroup.m_Targets[1].weight = 0f;
		}
		if ((bool)driverViewPoint)
		{
			driverViewPoint.transform.position = V.LastUsedCar.GetComponent<DriverController>().camInsideView.transform.position;
		}
	}

	public void setCameraForPersonWalking()
	{
		DisableAllCamera();
		currentCamera = (CameraType)PlayerPrefs.GetInt("PersonCameraRacing", 1);
		ChangeCameraOnPerson(currentCamera);
	}

	public void DisableAllCamera()
	{
		veryFirstCamera.enabled = false;
		virtualCamera.enabled = false;
		virtualCameraGarage.enabled = false;
		virtualCameraDriverView.enabled = false;
		virtualCameraOnFinish.enabled = false;
		virtualCameraOnToLookOtherRace.enabled = false;
		virtualCameraOnToLookOtherRace2.enabled = false;
		virtualCameraWinners.enabled = false;
		virtualCameraLoser.enabled = false;
		_carInOutSound.SetOutCarSound();
	}

	public void SetCameraForGarage()
	{
		DisableAllCamera();
		virtualCameraGarage.enabled = true;
		targetRot = bodyFollow.transform.eulerAngles;
		garageZoomDistance = 3f;
		currentCamera = CameraType.garageView;
		bodyFollow.transform.position = V.LastUsedCar.transform.position;
		edgeTracker.transform.position = V.LastUsedCar.transform.position;
		edgeTrackerTargetPos = edgeTracker.transform.position;
		movedByPlayer = false;
		eulerAngles = bodyFollow.transform.eulerAngles;
		targetRot = eulerAngles;
	}

	public void ShowWinnerCamera()
	{
		DisableAllCamera();
		virtualCameraWinners.enabled = true;
		virtualCameraWinners.Follow.GetComponent<CinemachineDollyCart>().enabled = true;
		virtualCameraWinners.Follow.GetComponent<CinemachineDollyCart>().m_Position = 0f;
	}

	public void ShowLoseCamera()
	{
		DisableAllCamera();
		virtualCameraLoser.enabled = true;
		virtualCameraLoser.Follow.GetComponent<CinemachineDollyCart>().enabled = true;
		virtualCameraLoser.Follow.GetComponent<CinemachineDollyCart>().m_Position = 0f;
	}

	public void SetNextCam()
	{
		int num = (int)currentCamera;
		if (_cameraViewSettingsDefault)
		{
			num++;
			if (num > 3)
			{
				num = 0;
			}
		}
		CameraType next = (currentCamera = (CameraType)num);
		if (FlagsStorage.OutOfCar)
		{
			ChangeCameraOnPerson(next);
		}
		else
		{
			ChangeCameraOnCar(next);
		}
	}

	private void ShowHideInterior(CameraType next)
	{
		if (next == CameraType.driverView)
		{
			if (!_vehicleData.InteriorsComposition.Interior)
			{
				_vehicleData.InteriorsComposition.TryRestoreSavedInterior();
			}
			else
			{
				_vehicleData.InteriorsComposition.ShowHighInterior();
			}
		}
		else if (CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low)
		{
			_vehicleData.InteriorsComposition.ShowLowInterior();
		}
	}

	public void ChangeCameraOnCar(CameraType next)
	{
		currentCamera = next;
		ShowHideInterior(next);
		_setHeadVisibleDisposable?.Dispose();
		_setHeadVisibleDisposable = null;
		_cameraViewSettingsDefault = true;
		if ((bool)_racingProvider.DragRacingController && (bool)_racingProvider.DragRacingController.ourCar)
		{
			bodyFollow.transform.rotation = _racingProvider.DragRacingController.ourCar.transform.rotation * Quaternion.Euler(-1.5f, -15f, 0f);
			targetRot = bodyFollow.transform.eulerAngles;
		}
		if (next == CameraType.driverView)
		{
			if ((bool)_racingProvider.DragRacingController && (bool)_racingProvider.DragRacingController.ourCar && _racingProvider.DragRacingController.ourCar.TryGetComponent<DriverController>(out var component))
			{
				component.camInsideView.transform.localRotation = Quaternion.identity;
			}
			virtualCameraDriverView.enabled = true;
			virtualCamera.enabled = false;
			_carInOutSound.UpdateInCarSound(_vehicleData.CarId);
			_setHeadVisibleDisposable = Observable.Timer(TimeSpan.FromSeconds(_cinemachineBrain.m_DefaultBlend.m_Time)).Subscribe((long _) =>
			{
				TrySwitchVisibleHeadPerson(isVisible: false);
			}).AddTo(this);
		}
		else
		{
			virtualCameraDriverView.enabled = false;
			virtualCamera.enabled = true;
			_carInOutSound.SetOutCarSound();
			TrySwitchVisibleHeadPerson(isVisible: true);
		}
		if (next == CameraType.close)
		{
			targetOffset = new Vector3(0f, 1.8f, -7f);
			CineComposer.m_TrackedObjectOffset = new Vector3(0f, 0.5f, 0f);
			targetField = 45f;
			camSettingField = targetField;
			CameraSettingOffset = targetOffset;
		}
		if (next == CameraType.middle)
		{
			targetOffset = new Vector3(0f, 2f, -9f);
			CineComposer.m_TrackedObjectOffset = new Vector3(0f, 0.5f, 0f);
			targetField = 40f;
			camSettingField = targetField;
			CameraSettingOffset = targetOffset;
		}
		if (next == CameraType.far)
		{
			targetOffset = new Vector3(0f, 5f, -11f);
			CineComposer.m_TrackedObjectOffset = new Vector3(0f, 1f, 0f);
			targetField = 50f;
			camSettingField = targetField;
			CameraSettingOffset = targetOffset;
		}
		ApplyChaseViewOverridesAlter(next);
		PlayerPrefs.SetInt("CarCameraRacing", (int)next);
		TrySetCameraText(next);
	}

	private void TrySwitchVisibleHeadPerson(bool isVisible)
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

	public void ChangeCameraOnPerson(CameraType next)
	{
		TouchMovePerson component = V.DriverPerson.GetComponent<TouchMovePerson>();
		Vector3 cameraPivot = new Vector3(0f, 1.6f, 1f);
		float fieldOfView = 50f;
		float cameraDistance = 0f;
		if (next == CameraType.driverView)
		{
			cameraPivot = new Vector3(0f, 1.5f, 0f);
			cameraDistance = 0f;
			fieldOfView = 45f;
		}
		if (next == CameraType.close)
		{
			cameraPivot = new Vector3(0f, 1.8f, 0f);
			cameraDistance = 2f;
			fieldOfView = 50f;
		}
		if (next == CameraType.middle)
		{
			cameraPivot = new Vector3(0f, 2f, 0f);
			fieldOfView = 55f;
			cameraDistance = 4f;
		}
		if (next == CameraType.far)
		{
			cameraPivot = new Vector3(0f, 2f, 0f);
			fieldOfView = 45f;
			cameraDistance = 7f;
		}
		component.CameraPivot = cameraPivot;
		component.CameraDistance = cameraDistance;
		ourCam.fieldOfView = fieldOfView;
		PlayerPrefs.SetInt("PersonCameraRacing", (int)next);
		_carInOutSound.SetOutCarSound();
	}

	private void TrySetCameraText(CameraType next)
	{
		if (_uiReferencesMediator != null)
		{
			_uiReferencesMediator.CameraTypeText.SetActive(active: true);
			_uiReferencesMediator.CameraTypeText.text = $"#{(int)(next + 1)} {Utils.GetCameraTypeName(next)}";
			_removeCameraTextTypeDisposable?.Dispose();
			_removeCameraTextTypeDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				_uiReferencesMediator.CameraTypeText.text = "";
				_uiReferencesMediator.CameraTypeText.SetActive(active: false);
			}).AddTo(this);
		}
	}

	[Inject]
	public void ConstructControlsSettings(SteeringWheelValueEvent steeringWheelValueEvent, InputDeviceDetector inputDeviceDetector, CarCamerasParametersConfig carCamerasParametersConfig, AlterChaseCameraConfig chaseFramingConfig, OptionalDependency<DesktopControlsSettingsSO> dep)
	{
		_steeringWheelValueEvent = steeringWheelValueEvent;
		_inputDeviceDetector = inputDeviceDetector;
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

	private void ApplyChaseViewOverridesAlter(CameraType next)
	{
		if ((next == CameraType.close || next == CameraType.middle || next == CameraType.far) && (bool)_carCamerasParametersConfig && (bool)_chaseFramingConfig && (bool)V.LastUsedCar)
		{
			VehicleData vehicleData = (_vehicleData ? _vehicleData : V.LastUsedCar.GetComponent<VehicleData>());
			bool isTrack = (bool)vehicleData && vehicleData.isTruck;
			bool isTall = (bool)vehicleData && CarIdStorage.TallCars.Contains(vehicleData.CarId);
			_carCamerasParametersConfig.GetChaseParams(1, isTrack, isTall, out var distance, out var height, out var pitch);
			Transform transform = AlterChaseCameraConfig.FindAnchor(V.LastUsedCar.transform);
			float num = (transform ? (transform.position.y - V.LastUsedCar.transform.position.y) : 0f);
			float d = next switch
			{
				CameraType.close => _chaseFramingConfig.NearDistance, 
				CameraType.far => _chaseFramingConfig.FarDistance, 
				_ => _chaseFramingConfig.MiddleDistance, 
			};
			targetOffset = _chaseFramingConfig.FollowOffset(distance, height, pitch, num, d);
			CameraSettingOffset = targetOffset;
			CineComposer.m_TrackedObjectOffset = _chaseFramingConfig.ComposerOffset(height, pitch, num);
			targetField = _chaseFramingConfig.BaseFov;
			camSettingField = targetField;
		}
	}

	private void SubscribeToCameraActions()
	{
		_changeCameraHandle.Get(_eventRegistry).Subscribe(SetNextCam);
		_lookLeftHandle.Get(_eventRegistry).Subscribe(OnLookLeft);
		_lookRightHandle.Get(_eventRegistry).Subscribe(OnLookRight);
		_lookBackHandle.Get(_eventRegistry).Subscribe(OnLookBack);
		_inputMapSetEvent.Subscribe(OnInputGroupChanged);
		_onLookHandle.Get(_eventRegistry).Subscribe(OnLookHeldChanged);
		_onLookFreeHandle.Get(_eventRegistry).Subscribe(OnLookFreeChanged);
	}

	private void UnsubscribeFromCameraActions()
	{
		_onLookFreeHandle.Get(_eventRegistry).Unsubscribe(OnLookFreeChanged);
		_onLookHandle.Get(_eventRegistry).Unsubscribe(OnLookHeldChanged);
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
				float num = (IsGamepadActive ? 0.4f : 4f);
				_targetGarageZoom -= value * num;
				_targetGarageZoom = Mathf.Clamp(_targetGarageZoom, 2f, 10f);
			}
			garageZoomDistance = Mathf.SmoothDamp(garageZoomDistance, _targetGarageZoom, ref _garageZoomVel, 0.12f);
			Vector2 value2 = _lookHandle.Get(_eventRegistry).Value;
			float value3 = (garageZoomDistance - 2f) / 8f;
			value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(MultiplierPit, value2);
			value2 = Vector2.Scale(value2, GarageViewLookScale);
			if (IsGamepadActive)
			{
				value2 *= 0.28f;
			}
			value2 = ScaleUserLook(value2, CameraSettingsContext.Garage);
			if (_lookHeld)
			{
				movedByPlayer = true;
				rotatingByTouchNow = true;
				timeStartTouch = Time.time;
				_cameraViewSettingsDefault = false;
				targetRot.y += value2.x;
				targetRot.x -= value2.y;
				targetRot.x = Mathf.Clamp(targetRot.x, 0f, 70f);
				targetRot.z = 0f;
			}
			float num2 = Vector3.Distance(bodyFollow.transform.position, edgeTrackerTargetPos);
			float t = smoothSpeed4;
			if (currentCamera == CameraType.garageView)
			{
				t = (3f + num2) * smoothSpeed4;
			}
			eulerAngles = new Vector3(Mathf.Lerp(eulerAngles.x, targetRot.x, t), Mathf.Lerp(eulerAngles.y, targetRot.y, t), Mathf.Lerp(eulerAngles.z, targetRot.z, t));
			movingNow.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		}
	}

	private void MoveSpectatorCamAlter()
	{
		if (!ctSpecCar)
		{
			return;
		}
		float value = _zoomHandle.Get(_eventRegistry).Value;
		if (!_spectatorZoomInit)
		{
			_targetSpectatorZoom = ctSpecCar.m_FollowOffset.z;
			_spectatorZoomInit = true;
		}
		if (value != 0f)
		{
			float num = (IsGamepadActive ? 0.4f : 4f);
			_targetSpectatorZoom = Mathf.Clamp(_targetSpectatorZoom + value * num, -12f, -3f);
		}
		ctSpecCar.m_FollowOffset.z = Mathf.SmoothDamp(ctSpecCar.m_FollowOffset.z, _targetSpectatorZoom, ref _spectatorZoomVel, 0.12f);
		Vector2 value2 = _lookHandle.Get(_eventRegistry).Value;
		if (!(value2 == Vector2.zero))
		{
			value2 = Vector2.Scale(Vector2.Scale(MultiplierPit, value2), GarageViewLookScale);
			if (IsGamepadActive)
			{
				value2 *= 0.28f;
			}
			value2 = ScaleUserLook(value2, CameraSettingsContext.Garage);
			targetRotation *= Quaternion.Euler(0f, value2.x, 0f);
			targetY = Mathf.Clamp(targetY - value2.y * 0.15f, 0f, 12f);
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
		Vector2 value2 = _lookFreeHandle.Get(_eventRegistry).Value;
		float value3 = (CameraSettingOffset.z - -10f) / 9f;
		value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(MultiplierChase, value2);
		value2 = ScaleUserLook(value2, CameraSettingsContext.Car);
		if (!_lookFree)
		{
			rotatingByTouchNow = false;
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
			_targetFovZoom -= value * 25f;
			_targetFovZoom = Mathf.Clamp(_targetFovZoom, 20f, 90f);
			_cameraViewSettingsDefault = false;
			PlayerPrefs.SetFloat($"racingCamFOV_{(int)currentCamera}", _targetFovZoom);
		}
		camSettingField = Mathf.SmoothDamp(camSettingField, _targetFovZoom, ref _fovZoomVel, 0.12f);
		Vector2 value2 = _lookFreeHandle.Get(_eventRegistry).Value;
		float value3 = (camSettingField - 20f) / 70f;
		value2 = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value3)) * Vector2.Scale(MultiplierFpv, value2);
		value2 = ScaleUserLook(value2, CameraSettingsContext.CarFpv);
		if (_lookFree)
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

	private void OnLookFreeChanged(bool held)
	{
		_lookFree = held;
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
			if (arrow == LookArrow.Back && currentCamera == CameraType.driverView)
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
		if (getPowertrain != null)
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
		if (carChildsReferences != null)
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
			if (currentCamera == CameraType.driverView)
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
			if (_lookActiveMode == LookMode.Pov && currentCamera == CameraType.driverView && component?.camInsideView != null)
			{
				component.camInsideView.transform.localEulerAngles = _snapPovEuler;
			}
			else if (_lookActiveMode == LookMode.ChaseCam && currentCamera != CameraType.driverView)
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
		if (_lookActiveMode == LookMode.PovBack && currentCamera == CameraType.driverView && (bool)_rearTransform)
		{
			if (V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView && V.Car.TryGetComponent<VehicleData>(out var component2) && TryGetRearLookAnchor(component2, out var worldPos, out var worldRot))
			{
				_ = deubg;
				component.camInsideView.transform.SetPositionAndRotation(worldPos, worldRot);
			}
		}
		else if (_lookActiveMode == LookMode.Pov && currentCamera == CameraType.driverView && V.Car.TryGetComponent<DriverController>(out component3) && (bool)component3.camInsideView)
		{
			Vector3 localEulerAngles = component3.camInsideView.transform.localEulerAngles;
			component3.camInsideView.transform.localEulerAngles = new Vector3(localEulerAngles.x, LookOverrideCurrentYaw, localEulerAngles.z);
		}
	}
}
