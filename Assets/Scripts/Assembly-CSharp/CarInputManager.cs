using System;
using CPM.Transmission;
using CPMEngine;
using Car_Parking.Scripts.Drag.CarCompositions;
using UniRx;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CPMEngine;
using _Car_Parking.Scripts.Car.CruiseControl;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam;

public class CarInputManager : MonoBehaviour
{
	private const float MaxSteerWheelAngle = 270f;

	private const float SteerCorrectionFactor = 1f;

	private const float TiltDeadZone = 0.025f;

	private bool TiltSmoothing;

	private readonly OneEuroFilter _tiltFilter = new OneEuroFilter();

	private const float VelocityRatio = 0.009f;

	private const float SteeringRatio = 0.1f;

	private const float BackVelocityRatio = 0.03f;

	private const string ThrottleAxis = "Throttle";

	private const string ThrottleAnalogAxis = "AccelPedal";

	private const string BrakeAxis = "Brake";

	private const string BrakeAnalogAxis = "BrakePedal";

	private const string SteerAxis = "Horizontal";

	private const string SteerWheelAxis = "Steer";

	private const string HandbrakeAxis = "Handbrake";

	private const string ClutchAxis = "Clutch";

	private const string ClutchAnalogAxis = "ClutchPedal";

	private const string ShiftUpButton = "ShiftUp";

	private const string ShiftDownButton = "ShiftDown";

	private const string StartEngineButton = "StartEngine";

	public ReactiveProperty<Controller> RController;

	public DesktopControllerType DesktopControllerType;

	public MobileControllerType MobileControllerType;

	public bool IsEnabled = true;

	public bool IgnoreAllInput;

	public bool IgnoreSteerInput;

	public bool SteerAssistance = true;

	public bool ForceFeedback;

	public bool FollowPoints;

	public bool DriftMode;

	public bool AutoReverse;

	public bool Ready;

	private bool _isCruiseControl;

	public CarOptionsValues CarOptions;

	private bool _activeGamepad;

	private bool _updateKeepRow;

	private bool _steeringWheelIsTouching;

	private bool _oldWheelSystem;

	public float SteerAssistanceMinVelocity = 30f;

	public float TargetSpeed;

	public int SelectorState;

	public int State;

	[Header("Steering")]
	public float SteerAutoInput;

	public float SteerInput;

	public float SteerOutput;

	[Header("Throttle")]
	public float ThrottleAutoInput;

	public float ThrottleInput;

	private float _throttleCruise;

	public float ThrottleOutput;

	[Header("Brake")]
	public float BrakeAutoInput;

	public float BrakeInput;

	public float BrakeOutput;

	[Header("Clutch")]
	public float ClutchInput;

	public float ClutchOutput;

	[Header("Handbrake")]
	public float HandbrakeInput;

	[Header("Starter")]
	public float StarterInput;

	private bool _lc;

	private float _oldSteering;

	private float _deltaSteering;

	private float _steeringVelo;

	private float _calculateSteer;

	private Vector3 _toFollow;

	private float _steerSensitivity;

	private float _steerTime = 0.1f;

	private float _steerReleaseTime = 0.1f;

	private float _veloSteerTime = 0.05f;

	private float _veloSteerReleaseTime = 0.05f;

	private float _throttleTime = 0.25f;

	public static readonly float ThrottleReleaseTime = 0.05f;

	private float _brakesTime = 0.2f;

	public static readonly float BrakesReleaseTime = 0.05f;

	private float _clutchTime = 0.3f;

	public static readonly float ClutchReleaseTime = 0.35f;

	private GamepadHapticService _gamepadHaptics;

	private HapticConfigSO _hapticConfig;

	private bool _hapticsDrivingGroup = true;

	private float _brakeHapticRamp;

	private int _prevGear = int.MinValue;

	private float _lastGearPulseUnscaled;

	private const float GearPulseDebounceSeconds = 0.3f;

	private bool _cachedBrakeKey;

	private bool _isPlayer;

	private bool _onStopCarOnStaged;

	private bool _isAngleTemporarilyDisabled;

	private float _calculateMaxSteerWheelAngle;

	private IDisposable _angleDisabler;

	private IDisposable _disposableResetStopCarOnStaged;

	private Powertrain _powertrain;

	private Rigidbody _rigidbody;

	private VehicleData _vehicleData;

	private VehicleEntityProvider _vehicleEntityProvider;

	private CarAIController _carAIController;

	private Axles _axles;

	private Rigidbody _vehicleRigidbody;

	private CruiseControl _cruiseControl;

	private DriverActionController _driverActionController;

	private CarControlValuesSO _carControlValues;

	private QuickCarSettings _quickCarSettings;

	private LevelsProvider _levelsProvider;

	private DragRacingCamera _dragRacingCamera;

	private SoundController _soundController;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private DesktopControlsSettingsSO _controlsSettings;

	private RegistryHandle<FloatVariable> _throttleHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Throttle);

	private RegistryHandle<FloatVariable> _brakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Brake);

	private RegistryHandle<FloatVariable> _clutchHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Clutch);

	private RegistryHandle<FloatVariable> _steerHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Steer);

	private RegistryHandle<FloatVariable> _handbrakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Handbrake);

	private RegistryHandle<BoolEvent> _starterHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnStartEngine);

	private RegistryHandle<BoolEvent> _startStopBurnoutHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopBurnout);

	private RegistryHandle<GearVariable> _gearHandle = new RegistryHandle<GearVariable>(EventRegistryIDs.Gear);

	private const float CasualPedalThreshold = 0.15f;

	private ForceFeedback _forceFeedback;

	private InputItemsProvider _inputItemsProvider;

	private GameObject _car;

	private float _personForwardInput;

	private float _personBackwardInput;

	private float _personLeftRightInput;

	private bool _steeringChangingByPlayer;

	private bool _steeringChangingByPlayerKeys;

	private bool _throttleChangingByPlayerKeys;

	private bool _brakeChangingByPlayerKeys;

	private bool _clutchChangingByPlayerKeys;

	private bool _autoPilot;

	private DeviceType _deviceType;

	private bool _isDesktop;

	private bool _manualBurnOutActivator;

	private SteerMode _resolvedSteerMode;

	private bool _wasGamepadDetected;

	private DesktopControllerType? _loggedDesktopController;

	private MobileControllerType? _loggedMobileController;

	private int _currentGear;

	private const float AutoSteerReferenceDeltaTime = 1f / 60f;

	public bool IsCruiseControlEnabled
	{
		get
		{
			return _isCruiseControl;
		}
		set
		{
			_isCruiseControl = value;
			if ((bool)_powertrain)
			{
				_powertrain.IsCruiseControlActive = value;
			}
		}
	}

	public float SteeringWheelAngle { get; set; }

	public UGUISteeringWheel UGUISteeringWheel { get; set; }

	public ParkTronicControll ParkTronicControl { get; set; }

	public float GetPersonForward => _personForwardInput;

	public float GetPersonBackward => _personBackwardInput;

	public float GetPersonLeftRight => _personLeftRightInput;

	public Powertrain GetPowertrain => _powertrain;

	public VehicleData GetVehicleData => _vehicleData;

	public Axles GetAxles => _axles;

	public Rigidbody GetRigidbody => _rigidbody;

	public VehicleEntityProvider GetVehicleEntityProvider => _vehicleEntityProvider;

	public KeepRowComposition KeepRowComposition { get; private set; }

	public bool ManualBurnOutActivator => _manualBurnOutActivator;

	public bool OnStopCarOnStaged
	{
		get
		{
			return _onStopCarOnStaged;
		}
		set
		{
			_onStopCarOnStaged = value;
			if (value)
			{
				_disposableResetStopCarOnStaged?.Dispose();
				_disposableResetStopCarOnStaged = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
				{
					ResetStopCarOnStaged();
				}).AddTo(this);
			}
		}
	}

	public Vector3 GetFollowPoint => _toFollow;

	public QuickCarSettings QuickCarSettings
	{
		get
		{
			if (!_quickCarSettings && (bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.QuickCarSettings)
			{
				_quickCarSettings = V.CoreGameManager.FreeDriveProvider.QuickCarSettings;
			}
			return _quickCarSettings;
		}
	}

	public event Action OnInputsCleared;

	public void ApplyCasualControlToPowertrain()
	{
		if ((bool)_car && ((bool)_powertrain || _car.TryGetComponent<Powertrain>(out _powertrain)) && !FlagsStorage.IsCasualControl && _powertrain.Gear == -1)
		{
			_powertrain.Gear = 0;
			_powertrain.IsParkingGear = false;
			_powertrain.RaiseGearChanged();
			SelectorState = 3;
		}
	}

	private void ApplyCasualGearLogic(ref float throttleInput, ref float brakeInput)
	{
		if (!FlagsStorage.IsCasualControl || !_powertrain)
		{
			return;
		}
		bool flag = _powertrain.SpeedMps < 0.5f;
		if (_powertrain.Gear == -1)
		{
			if (throttleInput > 0.15f)
			{
				if (!flag)
				{
					brakeInput = throttleInput;
					throttleInput = 0f;
				}
				else if (brakeInput <= 0.15f)
				{
					_powertrain.Gear = 1;
					_powertrain.IsParkingGear = false;
					_powertrain.RaiseGearChanged();
					SelectorState = 4;
				}
				else
				{
					throttleInput = 0f;
				}
			}
			else
			{
				float num = brakeInput;
				float num2 = throttleInput;
				throttleInput = num;
				brakeInput = num2;
			}
		}
		else if ((brakeInput > 0.15f && throttleInput <= 0.15f) & flag)
		{
			_powertrain.Gear = -1;
			_powertrain.IsParkingGear = false;
			_powertrain.RaiseGearChanged();
			SelectorState = 2;
			float num2 = brakeInput;
			float num = throttleInput;
			throttleInput = num2;
			brakeInput = num;
		}
		else if (((throttleInput > 0.15f && brakeInput <= 0.15f) & flag) && (_powertrain.Gear < 1 || _powertrain.IsParkingGear))
		{
			_powertrain.Gear = 1;
			_powertrain.IsParkingGear = false;
			_powertrain.RaiseGearChanged();
			SelectorState = 4;
		}
	}

	[Inject]
	public void Construct(InputItemsProvider controlItemsProvider, CarControlValuesSO carControlValuesSo, LevelsProvider levelsProvider, EventRegistry eventRegistry, DesktopControlsSettingsSO controlsSettingsAsset, InputMapSetEvent inputMapSetEvent, GamepadHapticService gamepadHaptics, HapticConfigSO hapticConfig, ForceFeedback forceFeedback)
	{
		_carControlValues = carControlValuesSo;
		_levelsProvider = levelsProvider;
		_inputItemsProvider = controlItemsProvider;
		if (KeepRowComposition == null)
		{
			KeepRowComposition keepRowComposition = (KeepRowComposition = new KeepRowComposition());
		}
		_eventRegistry = eventRegistry;
		_controlsSettings = controlsSettingsAsset;
		_inputMapSetEvent = inputMapSetEvent;
		_gamepadHaptics = gamepadHaptics;
		_hapticConfig = hapticConfig;
		_forceFeedback = forceFeedback;
	}

	public void ResetKeepRowComposition()
	{
		KeepRowComposition?.Dispose();
		KeepRowComposition = new KeepRowComposition();
	}

	public void SetFollowPoint(Vector3 point)
	{
		_toFollow = point;
	}

	private void ResetStopCarOnStaged()
	{
		OnStopCarOnStaged = false;
	}

	public void SetDragRaceCamera(DragRacingCamera dragRacingCamera)
	{
		_dragRacingCamera = dragRacingCamera;
	}

	private void Awake()
	{
		MoveElements.GlobalPointMoved += TemporarilyDisableAngleOnKeepRow;
		LoadAssist();
	}

	private void Start()
	{
		_starterHandle.Get(_eventRegistry).Subscribe(ToggleEngine);
		_startStopBurnoutHandle.Get(_eventRegistry).Subscribe(SetBurnOut);
		if (SteamManager.IsInitialized())
		{
			FlagsStorage.IsSteamDeck = SteamManager.IsSteamRunningOnSteamDeck();
		}
	}

	private void InitHUDAnalog()
	{
		if (_inputItemsProvider.ShifterControl != null)
		{
			_inputItemsProvider.ShifterControl.UpdateCar();
		}
	}

	private void TemporarilyDisableAngleOnKeepRow()
	{
		_isAngleTemporarilyDisabled = true;
		SteerInput = 0f;
		_angleDisabler?.Dispose();
		_angleDisabler = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			_isAngleTemporarilyDisabled = false;
		}).AddTo(this);
	}

	public void EnableControl(bool stateInput, bool stateOutput)
	{
		IgnoreSteerInput = stateOutput;
	}

	public void IgnoreSteerControl(bool state)
	{
		IgnoreSteerInput = state;
		FollowPoints = _toFollow != Vector3.zero;
		if (!state)
		{
			ClearSteerInputs();
			FollowPoints = false;
		}
	}

	public void IgnoreAllControl(bool state, bool follow = false)
	{
		IgnoreAllInput = state;
		FollowPoints = follow;
		if (!state)
		{
			ClearAllInputs();
			return;
		}
		if ((bool)UGUISteeringWheel)
		{
			UGUISteeringWheel.ResetWheel();
		}
		SteerInput = 0f;
		_calculateSteer = 0f;
	}

	public void SetDriftMode(bool driftMode)
	{
		if (driftMode)
		{
			_steerTime = 0.01f;
			_steerReleaseTime = 0.01f;
			_veloSteerTime = 0.01f;
			_veloSteerReleaseTime = 0.01f;
		}
		else
		{
			_steerTime = 0.1f;
			_steerReleaseTime = 0.1f;
			_veloSteerTime = 0.05f;
			_veloSteerReleaseTime = 0.05f;
		}
	}

	public void SetSteerWheelAngle(float angle)
	{
		SteeringWheelAngle = angle;
	}

	public void SetSteerTime(float time = 0.1f, float releaseTime = 0.1f)
	{
		_steerTime = Mathf.Max(time, 0.08f);
		_steerReleaseTime = Mathf.Max(releaseTime, 0.01f);
	}

	public void SetVeloSteerTime(float time = 0.05f, float releaseTime = 0.05f)
	{
		_veloSteerTime = Mathf.Max(time, 0.08f);
		_veloSteerReleaseTime = Mathf.Max(releaseTime, 0.01f);
	}

	public static bool SupportsSteerAssist(MobileControllerType type)
	{
		if (type != MobileControllerType.Keys)
		{
			return type == MobileControllerType.Tilt;
		}
		return true;
	}

	public static bool SupportsTiltSmoothing(MobileControllerType type)
	{
		return type == MobileControllerType.Tilt;
	}

	public void InitHelpers()
	{
		if (_isDesktop)
		{
			SteerAssistance = DesktopControllerType == DesktopControllerType.Keyboard && PlayerPrefs.GetInt("Stabilization", 1) == 1;
			DriftMode = DesktopControllerType == DesktopControllerType.Keyboard && PlayerPrefs.GetInt("DriftMode", 1) == 1;
			TiltSmoothing = false;
		}
		else
		{
			MobileControllerType = (MobileControllerType)V.Steering;
			SteerAssistance = SupportsSteerAssist(MobileControllerType) && PlayerPrefs.GetInt("Stabilization", 0) == 1;
			DriftMode = MobileControllerType == MobileControllerType.Keys && PlayerPrefs.GetInt("DriftMode", 0) == 1;
			TiltSmoothing = SupportsTiltSmoothing(MobileControllerType) && PlayerPrefs.GetInt("TiltSmoothing", 0) == 1;
		}
		_activeGamepad = PlayerPrefs.GetInt("gamepad", 0) == 1;
	}

	private SteerMode ResolveSteerMode()
	{
		return SteerModeRules.Resolve(IgnoreSteerInput, FlagsStorage.KeepRow, SteerAssistance, _isDesktop, DesktopControllerType);
	}

	public void AttachVehicle(GameObject newPerson, VehicleData vehicleData)
	{
		if (!newPerson.TryGetComponent<DriverActionController>(out _driverActionController))
		{
			return;
		}
		GameObject gameObject = (vehicleData ? vehicleData.gameObject : _driverActionController.Car);
		if ((bool)gameObject && gameObject != _car)
		{
			_car = gameObject;
			_car.TryGetComponent<Powertrain>(out _powertrain);
			_car.TryGetComponent<Axles>(out _axles);
			_car.TryGetComponent<Rigidbody>(out _vehicleRigidbody);
			_car.TryGetComponent<VehicleEntityProvider>(out _vehicleEntityProvider);
			_car.TryGetComponent<CarAIController>(out _carAIController);
			_car.TryGetComponent<Rigidbody>(out _rigidbody);
			_car.TryGetComponent<SoundController>(out _soundController);
			_vehicleData = vehicleData;
			_vehicleData.BodyLoadingComposition.SetSoundSpoiler(isSet: true);
			if (FlagsStorage.LoadedFromGarage)
			{
				if ((bool)_powertrain)
				{
					_powertrain.Automatic = RestoreAutomaticFromGarage(_powertrain.GearboxKind, _powertrain.AutoModes);
				}
				FlagsStorage.LoadedFromGarage = false;
			}
			KeepRowComposition.ReInit(this, _vehicleData);
			_cruiseControl?.SetCar(_powertrain);
			if ((bool)ParkTronicControl)
			{
				ParkTronicControl.SetCar(_powertrain, _vehicleData);
			}
			if (_forceFeedback != null && (bool)_powertrain)
			{
				_forceFeedback.SetCar(_powertrain);
			}
			if ((bool)_soundController)
			{
				_soundController.Init();
			}
			SyncSteerToVehicle();
			ThrottleInput = 0f;
			BrakeInput = 0f;
			ClutchOutput = 0f;
		}
		if ((bool)_powertrain)
		{
			if (!_powertrain.IsAutomaticGearbox)
			{
				_powertrain.OnGearChanged += UpdateMTGearBoxPos;
			}
			_powertrain.OnGearChanged += _vehicleData.SetCam;
		}
		InitHUDAnalog();
		GetSelectorStateFromCar();
		InitHelpers();
		SetAutopilot(state: false);
		SetPowertrainAssist();
		EnableControl(stateInput: true, stateOutput: false);
		ApplyCasualControlToPowertrain();
	}

	public void AttachVehicle(GameObject newCar)
	{
		_car = newCar;
		_car.TryGetComponent<Powertrain>(out _powertrain);
		_car.TryGetComponent<Axles>(out _axles);
		_car.TryGetComponent<Rigidbody>(out _vehicleRigidbody);
		_car.TryGetComponent<VehicleEntityProvider>(out _vehicleEntityProvider);
		if (_car.TryGetComponent<VehicleData>(out _vehicleData))
		{
			_vehicleData.BodyLoadingComposition.SetSoundSpoiler(isSet: true);
		}
		KeepRowComposition.ReInit(this, _vehicleData);
		_car.TryGetComponent<CarAIController>(out _carAIController);
		_car.TryGetComponent<DriverActionController>(out _driverActionController);
		_car.TryGetComponent<Rigidbody>(out _rigidbody);
		_car.TryGetComponent<SoundController>(out _soundController);
		if (FlagsStorage.LoadedFromGarage)
		{
			if ((bool)_powertrain)
			{
				_powertrain.Automatic = RestoreAutomaticFromGarage(_powertrain.GearboxKind, _powertrain.AutoModes);
			}
			FlagsStorage.LoadedFromGarage = false;
		}
		_cruiseControl?.SetCar(_powertrain);
		AttachCollisionHaptics();
		if ((bool)ParkTronicControl)
		{
			ParkTronicControl.SetCar(_powertrain, _vehicleData);
		}
		if (_forceFeedback != null && (bool)_powertrain)
		{
			_forceFeedback.SetCar(_powertrain);
		}
		if ((bool)_powertrain && !_powertrain.IsAutomaticGearbox)
		{
			_powertrain.OnGearChanged += UpdateMTGearBoxPos;
		}
		if ((bool)_soundController)
		{
			_soundController.Init();
		}
		InitHUDAnalog();
		GetSelectorStateFromCar();
		InitHelpers();
		SetAutopilot(state: false);
		SetPowertrainAssist();
		SteerInput = 0f;
		SyncSteerToVehicle();
		ThrottleInput = 0f;
		BrakeInput = 0f;
		ClutchOutput = 0f;
		EnableControl(stateInput: true, stateOutput: false);
		ApplyCasualControlToPowertrain();
	}

	private void SyncSteerToVehicle()
	{
		if (!_powertrain)
		{
			SteerInput = 0f;
			return;
		}
		float num = (_calculateSteer = (SteerOutput = (SteerInput = Mathf.Clamp(0f - _powertrain.GetSteer(), -1f, 1f))));
		_calculateMaxSteerWheelAngle = 270f * (0.3f + (1f - V.SWsensitivity));
		SteeringWheelAngle = 0f - SteerRemapMath.WidgetAngleFromSteerInput(num, _calculateMaxSteerWheelAngle);
		if ((bool)UGUISteeringWheel)
		{
			UGUISteeringWheel.SyncToSteerInput(num);
		}
	}

	public void ClearCar()
	{
		if ((bool)_car)
		{
			SetAutopilot(state: false);
			if (_inputItemsProvider.ShifterControl != null)
			{
				_inputItemsProvider.ShifterControl.SetActiveObjects(state: false);
			}
			_powertrain.ResetTransmission(resetGear: false);
			_car = null;
			if (!_powertrain.IsAutomaticGearbox)
			{
				_powertrain.OnGearChanged -= UpdateMTGearBoxPos;
			}
			_powertrain.OnGearChanged -= _vehicleData.SetCam;
			if (_forceFeedback != null)
			{
				_forceFeedback.ClearCar();
				_powertrain.EnableForceFeedback = false;
			}
			if ((bool)ParkTronicControl)
			{
				ParkTronicControl.DisableParkTronic();
			}
			StopGamepadHaptics();
			_powertrain = null;
			_axles = null;
			_vehicleRigidbody = null;
			_vehicleEntityProvider = null;
			_vehicleData.BodyLoadingComposition.SetSoundSpoiler(isSet: false);
			KeepRowComposition.ReInit(null, null);
			FlagsStorage.KeepRow = false;
			if ((bool)QuickCarSettings)
			{
				QuickCarSettings.DisableKeepLane(message: false);
			}
			_vehicleData = null;
			_carAIController = null;
			_rigidbody = null;
			_cruiseControl?.ClearCar();
			_soundController.Clear();
			_soundController = null;
			FollowPoints = false;
			ClearAllInputs();
		}
	}

	public void ClearPedals()
	{
		ThrottleInput = 0f;
		ThrottleOutput = 0f;
		BrakeInput = 0f;
		BrakeOutput = 0f;
		ClutchInput = 0f;
		ClutchOutput = 0f;
		InputStorage.BrakeInput = 0f;
		InputStorage.ClutchInput = 0f;
		InputStorage.ThrottleInput = 0f;
		InputStorage.HandbrakeInput = 0f;
		OnInputsCleared?.Invoke();
	}

	private void ClearSteerInputs()
	{
		InputStorage.LeftRightInput = 0f;
		SteerInput = 0f;
		_calculateSteer = 0f;
		SteerOutput = 0f;
		if ((bool)UGUISteeringWheel)
		{
			UGUISteeringWheel.ResetWheel();
		}
	}

	public void ClearAllInputs()
	{
		SteerInput = 0f;
		_calculateSteer = 0f;
		SteerOutput = 0f;
		if ((bool)UGUISteeringWheel)
		{
			UGUISteeringWheel.ResetWheel();
		}
		ThrottleInput = 0f;
		ThrottleOutput = 0f;
		HandbrakeInput = 0f;
		BrakeInput = 0f;
		BrakeOutput = 0f;
		ClutchOutput = 0f;
		OnLeftRightSteerReleased();
		InputStorage.BrakeInput = 0f;
		InputStorage.ClutchInput = 0f;
		InputStorage.ThrottleInput = 0f;
		InputStorage.HandbrakeInput = 0f;
		OnInputsCleared?.Invoke();
	}

	public void SetGearToCar(int gear)
	{
		if ((bool)_powertrain && (bool)QuickCarSettings)
		{
			_powertrain.Gear = gear;
			GetSelectorStateFromCar();
		}
	}

	private void GetSelectorStateFromCar()
	{
		if ((bool)_powertrain && _powertrain.IsAutomaticGearbox)
		{
			int selectorState = SelectorState;
			if (_powertrain.IsParkingGear)
			{
				SelectorState = 1;
			}
			else
			{
				SelectorState = _powertrain.Gear switch
				{
					-1 => 2, 
					0 => 3, 
					_ => 4, 
				};
			}
			if (!_powertrain.Automatic)
			{
				SelectorState = 5;
			}
			if (selectorState != SelectorState)
			{
				_powertrain.RaiseGearChanged();
			}
		}
	}

	public void SetCruiseControl(CruiseControl cruiseControl)
	{
		_cruiseControl = cruiseControl;
	}

	private void KeepRowAssist()
	{
		if (!FlagsStorage.IsFreeDriveMode || !FlagsStorage.KeepRow || FlagsStorage.RacingGameMode)
		{
			return;
		}
		if (SteerInput == 0f)
		{
			if (_updateKeepRow)
			{
				KeepRowComposition.SetClosestRow();
				_updateKeepRow = false;
			}
			Vector3 vector = _car.transform.InverseTransformPoint(_toFollow);
			float num = Mathf.Atan2(vector.x, vector.z);
			if (!_isAngleTemporarilyDisabled && Mathf.Abs(num) > 120f && !FlagsStorage.RacingGameMode && (bool)QuickCarSettings)
			{
				QuickCarSettings.DisableKeepLane(message: true);
			}
			_calculateSteer = Mathf.Clamp(num / _powertrain.MaxSteeringAngle, -1f, 1f);
		}
		else
		{
			_updateKeepRow = true;
		}
	}

	private void AutoControl(ref float throttleInput, ref float brakeInput, ref float steerInput, ref float handbrakeInput)
	{
		if (!FlagsStorage.RacingGameMode && !ScenesHelper.IsMenuSceneActive())
		{
			throttleInput = ((TargetSpeed == 0f) ? ThrottleAutoInput : (1f - _powertrain.SpeedMps / TargetSpeed));
			brakeInput = BrakeAutoInput;
		}
	}

	private void AutoSteer(ref float steerInput)
	{
		if (!ScenesHelper.IsMenuSceneActive() && !ScenesHelper.IsLapRacingSceneActive())
		{
			float num = (_powertrain ? _powertrain.MaxSteeringAngle : 0.6981317f);
			Vector3 vector = _car.transform.InverseTransformPoint(GetFollowPoint + V.GlobalOffset);
			float b = Mathf.Clamp(Mathf.Atan2(vector.x, vector.z) / num, -1f, 1f);
			steerInput = Mathf.Lerp(steerInput, b, 1f / 60f * (130f - _powertrain.SpeedMps));
		}
	}

	private void GetDesktopInput(out float throttleInput, out float brakeInput, out float steerInput, out float handbrakeInput, out float clutchInput)
	{
		throttleInput = 0f;
		brakeInput = 0f;
		steerInput = 0f;
		handbrakeInput = 0f;
		clutchInput = 0f;
		switch (DesktopControllerType)
		{
		case DesktopControllerType.Keyboard:
			throttleInput = _throttleHandle.Get(_eventRegistry).Value;
			brakeInput = _brakeHandle.Get(_eventRegistry).Value;
			steerInput = (IgnoreAllInput ? 0f : _steerHandle.Get(_eventRegistry).Value);
			handbrakeInput = _handbrakeHandle.Get(_eventRegistry).Value;
			clutchInput = _clutchHandle.Get(_eventRegistry).Value;
			ApplyCasualGearLogic(ref throttleInput, ref brakeInput);
			if (!ScenesHelper.IsMenuSceneActive())
			{
				if (IgnoreAllInput)
				{
					AutoControl(ref throttleInput, ref brakeInput, ref steerInput, ref handbrakeInput);
				}
				if (IgnoreSteerInput && FollowPoints)
				{
					AutoSteer(ref steerInput);
				}
			}
			break;
		case DesktopControllerType.SteeringWheel:
			throttleInput = _throttleHandle.Get(_eventRegistry).Value;
			brakeInput = _brakeHandle.Get(_eventRegistry).Value;
			steerInput = (IgnoreAllInput ? 0f : _steerHandle.Get(_eventRegistry).Value);
			handbrakeInput = _handbrakeHandle.Get(_eventRegistry).Value;
			clutchInput = _clutchHandle.Get(_eventRegistry).Value;
			ApplyCasualGearLogic(ref throttleInput, ref brakeInput);
			break;
		default:
			throttleInput = _throttleHandle.Get(_eventRegistry).Value;
			brakeInput = _brakeHandle.Get(_eventRegistry).Value;
			steerInput = (IgnoreAllInput ? 0f : _steerHandle.Get(_eventRegistry).Value);
			handbrakeInput = _handbrakeHandle.Get(_eventRegistry).Value;
			clutchInput = _clutchHandle.Get(_eventRegistry).Value;
			ApplyCasualGearLogic(ref throttleInput, ref brakeInput);
			break;
		}
		if (IgnoreAllInput)
		{
			if (DesktopControllerType != DesktopControllerType.Keyboard && !ScenesHelper.IsMenuSceneActive())
			{
				AutoControl(ref throttleInput, ref brakeInput, ref steerInput, ref handbrakeInput);
			}
			handbrakeInput = 0f;
			clutchInput = 0f;
		}
		if (OnStopCarOnStaged)
		{
			throttleInput = 0f;
			clutchInput = 1f;
			brakeInput = 1f;
		}
		if (_steeringChangingByPlayerKeys)
		{
			steerInput = InputStorage.LeftRightInput;
		}
		if (_throttleChangingByPlayerKeys)
		{
			throttleInput = InputStorage.ThrottleInput;
		}
		if (_brakeChangingByPlayerKeys)
		{
			brakeInput = InputStorage.BrakeInput;
		}
		if (_clutchChangingByPlayerKeys)
		{
			clutchInput = InputStorage.ClutchInput;
		}
		_ = _powertrain.Shifter;
	}

	private void UpdateMTGearBoxPos()
	{
		if (_isDesktop && DesktopControllerType == DesktopControllerType.SteeringWheel)
		{
			_currentGear = GetGear(_gearHandle.Get(_eventRegistry).StoredValue);
			if (_inputItemsProvider.ShifterControl != null && !_inputItemsProvider.ShifterControl.IsTouching)
			{
				_inputItemsProvider.ShifterControl.SetPosByGearJoyStick(_currentGear);
			}
		}
	}

	private int GetGear(GearEnum gear)
	{
		return gear switch
		{
			GearEnum.R => -1, 
			GearEnum.N => 0, 
			GearEnum.G1 => 1, 
			GearEnum.G2 => 2, 
			GearEnum.G3 => 3, 
			GearEnum.G4 => 4, 
			GearEnum.G5 => 5, 
			GearEnum.G6 => 6, 
			GearEnum.G7 => 7, 
			_ => 0, 
		};
	}

	private void GetMobileInput(out float throttleInput, out float brakeInput, out float steerInput, out float handbrakeInput, out float clutchInput)
	{
		if (!IsEnabled)
		{
			steerInput = 0f;
			throttleInput = 0f;
			brakeInput = 0f;
			handbrakeInput = 0f;
			clutchInput = 0f;
		}
		else
		{
			switch (MobileControllerType)
			{
			case MobileControllerType.SteeringWheel:
				_calculateMaxSteerWheelAngle = 270f * (0.3f + (1f - V.SWsensitivity));
				steerInput = (_oldWheelSystem ? Mathf.Clamp((0f - SteeringWheelAngle) / _calculateMaxSteerWheelAngle, -1f, 1f) : CalculateSteeringInput(_vehicleRigidbody.velocity.magnitude, 0f - SteeringWheelAngle));
				break;
			case MobileControllerType.Tilt:
				if (!TiltSmoothing)
				{
					steerInput = (IgnoreAllInput ? 0f : Input.acceleration.normalized.x);
				}
				else if (IgnoreAllInput)
				{
					_tiltFilter.Reset();
					steerInput = 0f;
				}
				else
				{
					steerInput = _tiltFilter.Filter(ReadTiltAxis(), Time.unscaledDeltaTime);
				}
				break;
			default:
				steerInput = InputStorage.LeftRightInput;
				break;
			}
			throttleInput = InputStorage.ThrottleInput;
			brakeInput = InputStorage.BrakeInput;
			handbrakeInput = InputStorage.HandbrakeInput;
			clutchInput = InputStorage.ClutchInput;
		}
		if (!IsEnabled)
		{
			return;
		}
		if (!ScenesHelper.IsMenuSceneActive())
		{
			if (IgnoreAllInput)
			{
				AutoControl(ref throttleInput, ref brakeInput, ref steerInput, ref handbrakeInput);
			}
			if (IgnoreSteerInput && FollowPoints)
			{
				AutoSteer(ref steerInput);
			}
		}
		StateControl(ref throttleInput, ref brakeInput, ref clutchInput);
	}

	private void GetAutoPilotInput(out float throttleInput, out float brakeInput, out float steerInput)
	{
		if (!_carAIController)
		{
			throttleInput = 0f;
			brakeInput = 0f;
			steerInput = 0f;
		}
		throttleInput = _carAIController.TargetThrottle;
		brakeInput = _carAIController.TargetBrake;
		steerInput = _carAIController.TargetSteering;
	}

	private void StateControl(ref float throttleInput, ref float brakeInput, ref float clutchInput)
	{
		if (!FlagsStorage.RacingGameMode)
		{
			return;
		}
		switch (State)
		{
		case 1:
			brakeInput = 1f;
			clutchInput = 1f;
			break;
		case 2:
			if (_vehicleData.racingState == RacingState.staged)
			{
				_powertrain.GetLaunchControlSettings(out var isHave, out var _, out var _, out var _);
				if (isHave)
				{
					if (throttleInput > 0f && brakeInput == 0f)
					{
						throttleInput = 0f;
					}
				}
				else
				{
					throttleInput = 0f;
				}
			}
			if (_vehicleData.racingState == RacingState.Finished)
			{
				brakeInput = 1f;
			}
			if (_powertrain.SpeedMps > 20f)
			{
				brakeInput = 1f;
				clutchInput = 1f;
			}
			break;
		}
		if (OnStopCarOnStaged)
		{
			throttleInput = 0f;
			clutchInput = 1f;
			brakeInput = 1f;
		}
	}

	public void SetAutopilot(bool state)
	{
		if ((bool)_carAIController)
		{
			_carAIController.AiType = AiType.None;
		}
		if (!state)
		{
			ThrottleAutoInput = 0f;
			BrakeAutoInput = 0f;
			SteerAutoInput = 0f;
			_calculateSteer = 0f;
		}
		_powertrain.SetAutoPilot(state);
		_autoPilot = state;
		IgnoreAllInput = state;
	}

	public bool IsAutopilot()
	{
		return _autoPilot;
	}

	private void CalculateSteerInput()
	{
		if (!FlagsStorage.IsInputEnabledInThisWindow)
		{
			return;
		}
		if (FollowPoints)
		{
			_calculateSteer = SteerInput;
			return;
		}
		_steeringChangingByPlayer = SteerInput != 0f;
		DesktopControllerType desktopControllerType = DesktopControllerType;
		if (desktopControllerType != DesktopControllerType.Keyboard && desktopControllerType == DesktopControllerType.SteeringWheel)
		{
			_calculateSteer = SteerInput;
		}
		else
		{
			SmoothSteer(SteerInput, ref _calculateSteer);
		}
	}

	private void Update()
	{
		if (!_car)
		{
			return;
		}
		if (_autoPilot)
		{
			GetAutoPilotInput(out ThrottleInput, out BrakeInput, out SteerInput);
			return;
		}
		float throttleInput = 0f;
		_resolvedSteerMode = ResolveSteerMode();
		GetDesktopInput(out throttleInput, out BrakeInput, out SteerInput, out HandbrakeInput, out ClutchInput);
		if (IsCruiseControlEnabled && throttleInput == 0f)
		{
			ThrottleInput = _throttleCruise;
		}
		else
		{
			ThrottleInput = throttleInput;
		}
		if (_powertrain.Automatic && AutoReverse)
		{
			UpdateAutoReverse(ref ThrottleInput, ref BrakeInput);
		}
	}

	private void FixedUpdate()
	{
		if ((bool)_car)
		{
			TickGamepadHaptics();
			if (_autoPilot)
			{
				SetAutoPilotToCar();
			}
			else
			{
				SetControlToCar();
			}
		}
	}

	private void LateUpdate()
	{
		bool flag = Gamepad.current != null;
		if (_deviceType != UnityEngine.Device.SystemInfo.deviceType || flag != _wasGamepadDetected)
		{
			_deviceType = UnityEngine.Device.SystemInfo.deviceType;
			_isDesktop = _deviceType == DeviceType.Desktop;
			SetController((!_isDesktop) ? Controller.Mobile : Controller.Desktop);
		}
	}

	private void OnMozaBaseConnectionChanged(bool connected)
	{
		SetController((!_isDesktop) ? Controller.Mobile : Controller.Desktop);
	}

	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
		{
			SetController((!_isDesktop) ? Controller.Mobile : Controller.Desktop);
		}
		else
		{
			_ = 2;
		}
	}

	private void LoadAssist()
	{
		CarOptions.AbsLevel = PlayerPrefs.GetInt("ABS", 5);
		CarOptions.EspLevel = PlayerPrefs.GetInt("ESP", 0);
		CarOptions.TrcLevel = PlayerPrefs.GetInt("TCS", 0);
		CarOptions.AnalogClutch = PlayerPrefs.GetInt("AnalogClutch", 0) != 0;
		CarOptions.AnalogThrottle = PlayerPrefs.GetInt("AnalogAccelerator", 0) != 0;
		CarOptions.AnalogBrake = PlayerPrefs.GetInt("AnalogBrake", 0) != 0;
	}

	public void SetPowertrainAssist()
	{
		if ((bool)_powertrain)
		{
			_powertrain.SetAssits(CarOptions);
		}
	}

	public void SetArcadeMode(bool state)
	{
		PlayerPrefs.SetInt("ArcadeMode", state ? 1 : 0);
		CarOptions.ArcadeMode = state;
		SetPowertrainAssist();
	}

	public void SetMTAssist(bool state)
	{
		PlayerPrefs.SetInt("ManualGearboxAssist", state ? 1 : 0);
		CarOptions.MTAssist = state;
		SetPowertrainAssist();
	}

	public void SetAnalogThrottle(bool state)
	{
		PlayerPrefs.SetInt("AnalogAccelerator", state ? 1 : 0);
		CarOptions.AnalogThrottle = state;
		SetPowertrainAssist();
	}

	public void SetAnalogBrake(bool state)
	{
		PlayerPrefs.SetInt("AnalogBrake", state ? 1 : 0);
		CarOptions.AnalogBrake = state;
		SetPowertrainAssist();
	}

	public void SetAnalogClutch(bool state)
	{
		PlayerPrefs.SetInt("AnalogClutch", state ? 1 : 0);
		CarOptions.AnalogClutch = state;
		SetPowertrainAssist();
	}

	public void SetAbsLevel(int level)
	{
		PlayerPrefs.SetInt("ABS", level);
		CarOptions.AbsLevel = level;
		SetPowertrainAssist();
	}

	public void SetEspLevel(int level, bool update = true)
	{
		if (update)
		{
			PlayerPrefs.SetInt("ESP", level);
		}
		CarOptions.EspLevel = level;
		SetPowertrainAssist();
	}

	public void SetTrcLevel(int level, bool update = true)
	{
		if (update)
		{
			PlayerPrefs.SetInt("TCS", level);
		}
		CarOptions.TrcLevel = level;
		SetPowertrainAssist();
	}

	private void SetAutoPilotToCar()
	{
		SmoothPedal(_brakesTime, BrakesReleaseTime, isAnalog: false, BrakeInput, ref BrakeOutput);
		SmoothPedal(_throttleTime, ThrottleReleaseTime, isAnalog: false, ThrottleInput, ref ThrottleOutput);
		ThrottleOutput = Mathf.Clamp01(ThrottleOutput);
		SteerOutput = Mathf.Clamp(SteerInput, -1f, 1f);
		_powertrain.SetSteer(SteerOutput, useAssistent: true);
		_powertrain.Brake = BrakeOutput;
		_powertrain.Throttle = ThrottleOutput;
		_powertrain.ThrottleIntent = ThrottleInput;
	}

	private void SetControlToCar()
	{
		CalculateSteerInput();
		KeepRowAssist();
		if (DesktopControllerType == DesktopControllerType.SteeringWheel)
		{
			ThrottleOutput = ThrottleInput;
			BrakeOutput = BrakeInput;
			ClutchOutput = ClutchInput;
		}
		else
		{
			SmoothPedal(_throttleTime, ThrottleReleaseTime, CarOptions.AnalogThrottle, ThrottleInput, ref ThrottleOutput);
			SmoothPedal(_brakesTime, BrakesReleaseTime, CarOptions.AnalogBrake, BrakeInput, ref BrakeOutput);
			SmoothPedal(_clutchTime, ClutchReleaseTime, CarOptions.AnalogClutch, ClutchInput, ref ClutchOutput);
		}
		ThrottleOutput = Mathf.Clamp01(ThrottleOutput);
		SteerOutput = Mathf.Clamp(_calculateSteer, -1f, 1f);
		if (FlagsStorage.IsInputEnabledInThisWindow)
		{
			_powertrain.SetSteer(SteerOutput, _resolvedSteerMode, DriftMode);
		}
		_powertrain.Brake = BrakeOutput;
		_powertrain.Handbrake = HandbrakeInput;
		_powertrain.Throttle = ThrottleOutput;
		_powertrain.ThrottleIntent = ThrottleInput;
		_powertrain.ClutchPedalInput = ClutchOutput;
		_powertrain.Clutch = ((!_powertrain.AutoClutch) ? ClutchOutput : Mathf.Max(ClutchOutput, _powertrain.Clutch));
		bool flag = _powertrain.AutoClutch && _powertrain.GearType != GearboxType.GearFwd;
		_powertrain.ClutchPedalOverride = ((!_powertrain.AutoClutch) ? 0f : (flag ? Mathf.Max(ClutchInput, HandbrakeInput) : ClutchInput));
		_powertrain.Handbrake = HandbrakeInput;
		if ((_powertrain.ManualHelper || _powertrain.AutoClutch) && HandbrakeInput > 0f && _powertrain.GearType != GearboxType.GearFwd)
		{
			_powertrain.Clutch = Mathf.Max(HandbrakeInput, ClutchOutput);
		}
		if (!_manualBurnOutActivator && _powertrain.IsAutomaticGearbox)
		{
			bool value = ThrottleOutput >= 1f && BrakeOutput >= 1f && _powertrain.Gear == 1;
			_powertrain.SetGearboxState(value);
		}
	}

	private float CalculateSteeringInput(float speed, float steeringWheelRotation)
	{
		float num = steeringWheelRotation / _calculateMaxSteerWheelAngle;
		float num2 = 0f;
		float num3 = 1f;
		if ((bool)_carControlValues)
		{
			num2 = _carControlValues.steeringCurve.Evaluate(speed * 0.002f);
			num3 = _carControlValues.steeringCurveByInput.Evaluate(num);
		}
		float num4 = Mathf.Clamp01(num2 * num3);
		return num * num4;
	}

	public float SmoothSteer(float steerLeftRight, float steering)
	{
		if (steerLeftRight < steering)
		{
			float num = ((steering > 0f) ? (1f / (0.1f + 0.009f * _powertrain.SpeedMps)) : (1f / (0.2f + 0.03f * _powertrain.SpeedMps)));
			if (steerLeftRight < 0f && steering > 0f)
			{
				num *= 1f + Mathf.Abs(Mathf.Abs(steering) - Mathf.Abs(steerLeftRight)) * 1f;
			}
			steering -= num * Time.deltaTime;
			if (steerLeftRight > steering)
			{
				steering = steerLeftRight;
			}
		}
		else if (steerLeftRight > steering)
		{
			float num = ((steering < 0f) ? (1f / (0.1f + 0.009f * _powertrain.SpeedMps)) : (1f / (0.2f + 0.03f * _powertrain.SpeedMps)));
			if (steerLeftRight > 0f && steering < 0f)
			{
				num *= 1f + Mathf.Abs(Mathf.Abs(steering) - Mathf.Abs(steerLeftRight)) * 1f;
			}
			steering += num * Time.deltaTime;
			if (steerLeftRight < steering)
			{
				steering = steerLeftRight;
			}
		}
		return steering;
	}

	private static float ReadTiltAxis()
	{
		int accelerationEventCount = Input.accelerationEventCount;
		if (accelerationEventCount <= 0)
		{
			return Input.acceleration.normalized.x;
		}
		Vector3 zero = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < accelerationEventCount; i++)
		{
			AccelerationEvent accelerationEvent = Input.GetAccelerationEvent(i);
			zero += accelerationEvent.acceleration * accelerationEvent.deltaTime;
			num += accelerationEvent.deltaTime;
		}
		if (num <= 0f)
		{
			return Input.acceleration.normalized.x;
		}
		Vector3 vector = zero / num;
		if (!(vector.sqrMagnitude > 1E-08f))
		{
			return 0f;
		}
		return vector.normalized.x;
	}

	private void SmoothSteer(float input, ref float output)
	{
		_steerSensitivity = Mathf.Clamp(1f - V.SWsensitivity, 0.01f, 1f);
		float num = (DriftMode ? 0.01f : (_veloSteerReleaseTime * _steerSensitivity));
		if (input < output)
		{
			float num2 = ((output > 0f) ? (1f / (_steerReleaseTime * _steerSensitivity + num * _powertrain.SpeedMps)) : (1f / (_steerTime * _steerSensitivity + _veloSteerTime * _powertrain.SpeedMps)));
			if (input < 0f && output > 0f)
			{
				num2 *= 1f + Mathf.Abs(Mathf.Abs(output) - Mathf.Abs(input)) * 1f;
			}
			output -= num2 * Time.deltaTime;
			output = Mathf.Max(output, input);
		}
		else if (input > output)
		{
			float num2 = ((output < 0f) ? (1f / (_steerReleaseTime * _steerSensitivity + num * _powertrain.SpeedMps)) : (1f / (_steerTime * _steerSensitivity + _veloSteerTime * _powertrain.SpeedMps)));
			if (input > 0f && output < 0f)
			{
				num2 *= 1f + Mathf.Abs(Mathf.Abs(output) - Mathf.Abs(input)) * 1f;
			}
			output += num2 * Time.deltaTime;
			output = Mathf.Min(output, input);
		}
	}

	private static void SmoothPedal(float time, float release, bool isAnalog, float input, ref float output)
	{
		if (isAnalog)
		{
			if (input > output)
			{
				float num = 1f / (time + 1E-06f);
				output = Mathf.MoveTowards(output, input, Time.deltaTime * num);
				return;
			}
			float num2 = output / (release + 1E-06f);
			output = Mathf.MoveTowards(output, input, Time.deltaTime * num2);
			if (Mathf.Approximately(input, 0f) && Mathf.Abs(output) < 0.001f)
			{
				output = 0f;
			}
		}
		else
		{
			float num3 = ((input > output) ? (1f / time) : (output / (release + 1E-06f)));
			output = Mathf.MoveTowards(output, input, Time.deltaTime * num3);
		}
	}

	private void SmoothValue(float vel, float target, ref float current)
	{
		if (current < target)
		{
			current += vel * Time.deltaTime;
			if (current > target)
			{
				current = target;
			}
		}
		else if (current > target)
		{
			current -= vel * Time.deltaTime;
			if (current < target)
			{
				current = target;
			}
		}
	}

	private bool IsKeyboardOrKeysMode()
	{
		if (_isDesktop)
		{
			return DesktopControllerType == DesktopControllerType.Keyboard;
		}
		return MobileControllerType == MobileControllerType.Keys;
	}

	private void UpdateAutoReverse(ref float accel, ref float brake)
	{
		if (Mathf.Abs(Vector3.Dot(_powertrain.Velocity, _powertrain.transform.forward)) < 0.1f)
		{
			if (brake > 0.5f)
			{
				_powertrain.Gear = -1;
			}
			if (accel > 0.5f)
			{
				_powertrain.Gear = 1;
			}
		}
		if (_powertrain.Gear == -1)
		{
			float num = brake;
			float num2 = accel;
			accel = num;
			brake = num2;
		}
	}

	private void ResetVehicle(GameObject person, VehicleData vehicleData)
	{
		ClearCar();
	}

	private void OnEnable()
	{
		V.OnCarEntered += AttachVehicle;
		V.OnCarExit += ResetVehicle;
		InputSystem.onDeviceChange += OnDeviceChange;
		MozaWrapper.BaseConnectionChanged += OnMozaBaseConnectionChanged;
		if ((bool)_controlsSettings)
		{
			_controlsSettings.CasualModeChanged += OnCasualModeChanged;
		}
		_inputMapSetEvent?.Subscribe(OnHapticsGroupChanged);
	}

	private void OnDisable()
	{
		V.OnCarEntered -= AttachVehicle;
		V.OnCarExit -= ResetVehicle;
		InputSystem.onDeviceChange -= OnDeviceChange;
		MozaWrapper.BaseConnectionChanged -= OnMozaBaseConnectionChanged;
		_angleDisabler?.Dispose();
		_disposableResetStopCarOnStaged?.Dispose();
		_starterHandle.Get(_eventRegistry)?.Unsubscribe(ToggleEngine);
		_startStopBurnoutHandle.Get(_eventRegistry)?.Unsubscribe(SetBurnOut);
		if ((bool)_controlsSettings)
		{
			_controlsSettings.CasualModeChanged -= OnCasualModeChanged;
		}
		_inputMapSetEvent?.Unsubscribe(OnHapticsGroupChanged);
	}

	private void OnCasualModeChanged(bool _)
	{
		ApplyCasualControlToPowertrain();
	}

	private void OnHapticsGroupChanged(InputGroupsEnum group)
	{
		_hapticsDrivingGroup = group == InputGroupsEnum.CarFree || group == InputGroupsEnum.CarRacing || group == InputGroupsEnum.CarDrag || group == InputGroupsEnum.CarFreeRace || group == InputGroupsEnum.CarRally;
		if (!_hapticsDrivingGroup)
		{
			_gamepadHaptics?.SetContinuous(0f, 0f);
		}
	}

	private void AttachCollisionHaptics()
	{
		if ((bool)_car && _gamepadHaptics != null && !(_hapticConfig == null))
		{
			if (!_car.TryGetComponent<CarCollisionHaptics>(out var component))
			{
				component = _car.AddComponent<CarCollisionHaptics>();
			}
			component.Configure(_gamepadHaptics, _hapticConfig, () => _hapticsDrivingGroup);
		}
	}

	private void TickGamepadHaptics()
	{
		if (_gamepadHaptics == null || _hapticConfig == null)
		{
			return;
		}
		float num = (_powertrain ? Mathf.Abs(_powertrain.SpeedKmh) : 0f);
		PollGearPulse();
		if (!_hapticsDrivingGroup)
		{
			_brakeHapticRamp = 0f;
			_gamepadHaptics.SetContinuous(0f, 0f);
			return;
		}
		bool num2 = HandbrakeInput > 0.1f;
		float num3 = (num2 ? _hapticConfig.HandbrakeMinSpeedKmh : _hapticConfig.BrakeMinSpeedKmh);
		float num4 = Mathf.Clamp01(_brakeHandle.Get(_eventRegistry)?.Value ?? (_powertrain ? _powertrain.Brake : 0f));
		bool num5 = num > num3 && num4 > 0.05f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (num5)
		{
			_brakeHapticRamp = Mathf.Min(1f, _brakeHapticRamp + fixedDeltaTime / Mathf.Max(0.01f, _hapticConfig.BrakeHoldRampSeconds) * Mathf.Lerp(0.4f, 1f, num4));
		}
		else
		{
			_brakeHapticRamp = Mathf.MoveTowards(_brakeHapticRamp, 0f, fixedDeltaTime / Mathf.Max(0.01f, _hapticConfig.BrakeHoldReleaseSeconds));
		}
		float num6 = Mathf.Lerp(_hapticConfig.BrakeHoldMinCeiling, 1f, num4);
		float num7 = _brakeHapticRamp * num6 * _hapticConfig.BrakeLevel;
		if (num2 && num > _hapticConfig.HandbrakeMinSpeedKmh)
		{
			num7 = Mathf.Max(num7, Mathf.Clamp01(HandbrakeInput) * _hapticConfig.HandbrakeLevel);
		}
		_gamepadHaptics.SetContinuous(Mathf.Clamp01(num7), 0f, _hapticConfig.BrakeModulationHz);
	}

	private void PollGearPulse()
	{
		if (_powertrain == null)
		{
			return;
		}
		int gear = _powertrain.Gear;
		if (gear != _prevGear)
		{
			float unscaledTime = Time.unscaledTime;
			bool flag = _prevGear >= 1 && gear >= 1;
			bool num = _prevGear != int.MinValue && !FlagsStorage.IsCasualControl && (flag || unscaledTime - _lastGearPulseUnscaled >= 0.3f);
			_prevGear = gear;
			if (num)
			{
				_lastGearPulseUnscaled = unscaledTime;
				_gamepadHaptics.PulseGearChange();
			}
		}
	}

	private void StopGamepadHaptics()
	{
		_brakeHapticRamp = 0f;
		_prevGear = int.MinValue;
		_gamepadHaptics?.SetContinuous(0f, 0f);
	}

	private void OnDestroy()
	{
		MoveElements.GlobalPointMoved -= TemporarilyDisableAngleOnKeepRow;
		StopGamepadHaptics();
	}

	public void SetController(Controller controllerType)
	{
		switch (controllerType)
		{
		case Controller.Desktop:
			MobileControllerType = MobileControllerType.Keys;
			if (IsSteeringWheel())
			{
				DesktopControllerType = DesktopControllerType.SteeringWheel;
				SteerAssistance = false;
			}
			else
			{
				DesktopControllerType = DesktopControllerType.Keyboard;
			}
			IsEnabled = true;
			break;
		default:
			MobileControllerType = (MobileControllerType)V.Steering;
			IsEnabled = true;
			break;
		}
		if (_loggedDesktopController != DesktopControllerType || _loggedMobileController != MobileControllerType)
		{
			_loggedDesktopController = DesktopControllerType;
			_loggedMobileController = MobileControllerType;
		}
	}

	private bool IsSteeringWheel()
	{
		foreach (InputDevice device in InputSystem.devices)
		{
			string text = device.name?.ToLower() ?? "";
			string text2 = device.description.product?.ToLower() ?? "";
			bool flag = text.Contains("wheel") || text2.Contains("wheel") || text2.Contains("driving") || text2.Contains("g29") || text2.Contains("g920") || text2.Contains("thrustmaster") || text2.Contains("fanatec");
			bool flag2 = text2.Contains("moza") || text.Contains("moza");
			if (flag2 && !flag)
			{
				flag2 = MozaWrapper.IsBaseConnected();
			}
			if (flag | flag2)
			{
				_ = DesktopControllerType;
				_ = 1;
				return true;
			}
		}
		return false;
	}

	public void PressBrake(float value = 1f)
	{
		if (_isDesktop)
		{
			BrakeInput = value;
		}
		else
		{
			InputStorage.BrakeInput = value;
		}
	}

	public void PressThrottle(float value = 1f)
	{
		if (_isDesktop)
		{
			ThrottleInput = value;
		}
		else
		{
			InputStorage.ThrottleInput = value;
		}
	}

	public void PressClutch(float value = 1f)
	{
		if (_isDesktop)
		{
			ClutchInput = value;
		}
		else
		{
			InputStorage.ClutchInput = value;
		}
	}

	public void PressHandbrake(float value = 1f)
	{
		if (_isDesktop)
		{
			HandbrakeInput = value;
		}
		else
		{
			InputStorage.HandbrakeInput = value;
		}
	}

	public void ManualModeGearUp()
	{
		if ((bool)_powertrain)
		{
			_powertrain.GearUp();
			PlaySoundOnGearChange();
			_inputItemsProvider?.ShifterControl?.PlayDriverShiftAnimation();
		}
	}

	public void ManualModeGearDown()
	{
		if ((bool)_powertrain && _powertrain.Gear > 1)
		{
			_powertrain.GearDown();
			PlaySoundOnGearChange();
			_inputItemsProvider?.ShifterControl?.PlayDriverShiftAnimation();
		}
	}

	private void PlaySoundOnGearChange()
	{
		SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
	}

	private static bool RestoreAutomaticFromGarage(GearboxKind kind, AutoShifterModes modes)
	{
		if (!FlagsStorage.IsAutomatic)
		{
			return !ShifterModes.SupportsManual(kind, modes);
		}
		return true;
	}

	public void SetStarter(float starter = 1f)
	{
		if (!_powertrain)
		{
			return;
		}
		StarterInput = starter;
		if (StarterInput >= 1f)
		{
			if (_powertrain.IsEngineWorks || _powertrain.Ignition)
			{
				_powertrain.ShutdownEngine();
			}
			else
			{
				_powertrain.StartEngine(starterSound: true);
			}
		}
	}

	public void SetThrottleForCruiseControl(float throttleNew = 0f)
	{
		_throttleCruise = throttleNew;
	}

	public void SetBurnOut(bool value)
	{
		if (value)
		{
			SetThrottleFromSlider(1f);
			SetBrakeFromSlider(1f);
			_manualBurnOutActivator = true;
		}
		else
		{
			SetThrottleFromSlider(0f);
			SetBrakeFromSlider(0f);
			_manualBurnOutActivator = false;
		}
		_powertrain.SetGearboxState(value, _manualBurnOutActivator);
	}

	public void SetThrottleFromSlider(float throttle)
	{
		if (!IgnoreAllInput)
		{
			_throttleChangingByPlayerKeys = throttle > 0f;
			InputStorage.ThrottleInput = throttle;
		}
	}

	public void SetBrakeFromSlider(float brake)
	{
		if (!IgnoreAllInput)
		{
			_brakeChangingByPlayerKeys = brake > 0f;
			InputStorage.BrakeInput = brake;
		}
	}

	public void SetClutchFromSlider(float clutch)
	{
		if (!IgnoreAllInput)
		{
			_clutchChangingByPlayerKeys = clutch > 0f;
			InputStorage.ClutchInput = clutch;
		}
	}

	public void OnLeftSteerPressed()
	{
		InputStorage.LeftRightInput = -1f;
		_steeringChangingByPlayerKeys = true;
	}

	public void OnRightSteerPressed()
	{
		InputStorage.LeftRightInput = 1f;
		_steeringChangingByPlayerKeys = true;
	}

	public void OnLeftRightSteerReleased()
	{
		InputStorage.LeftRightInput = 0f;
		_steeringChangingByPlayerKeys = false;
	}

	private void ToggleEngine(bool isCancelled = false)
	{
		if (isCancelled)
		{
			return;
		}
		if (_powertrain.IsEngineWorks)
		{
			_powertrain.ShutdownEngine();
			return;
		}
		if (_powertrain.CurrentFuel <= 0f)
		{
			V.CoreGameManager.ManagerUI.ShowFuelDriveUIWindow();
		}
		else
		{
			_gamepadHaptics?.PulseEngineStart(() => (bool)_powertrain && _powertrain.IsEngineWorks);
		}
		_powertrain.StartEngine(starterSound: true);
	}

	public void ResetSteerAndWheelsPosition()
	{
		SteeringWheelAngle = 0f;
		SteerInput = 0f;
		SteerOutput = 0f;
		_calculateSteer = 0f;
		if ((bool)_powertrain)
		{
			_powertrain.SetSteer(0f, useAssistent: false);
		}
	}

	public void SetSteerPositionInstantly(float position)
	{
		float steer = (_calculateSteer = (SteerOutput = (SteerInput = Mathf.Clamp(position, -1f, 1f))));
		if ((bool)_powertrain)
		{
			_powertrain.SetSteer(steer, useAssistent: false);
		}
	}
}
