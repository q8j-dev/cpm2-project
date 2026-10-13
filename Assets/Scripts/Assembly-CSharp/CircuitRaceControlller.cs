using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.Rating;
using CPM.Rating.Entities;
using CPM.RemoteConfig;
using CPMEngine;
using Cinemachine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using RacingScripts.RaceControllers;
using UniRx;
using UnityEngine;
using UnityEngine.Splines;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.WindowSystem;

public class CircuitRaceControlller : MonoBehaviour, IRaceController
{
	private const V.placeToSpawn PlaceToSpawn = V.placeToSpawn.Racing;

	private const string QualifiedMapPrefKey = "gpg_pole_qualified_map";

	public static int CurrentMapId;

	public static AdvancedInt CurrentClassId;

	public static bool IsQualification;

	public static int Direction;

	public static int HardCurrency;

	public static int SoftReward;

	private CircuitRacingWaitingWindow _circuitRacingWaitingWindow;

	[SerializeField]
	private Transform[] StartPositions;

	[SerializeField]
	private Transform CheckPointsParent;

	[SerializeField]
	private Transform RaceCarsParent;

	[SerializeField]
	private int AiCarsCount;

	[SerializeField]
	private int LapsCount = 3;

	[SerializeField]
	private int QualificationLapsCount = 1;

	[SerializeField]
	private int WaitAfterFinish = 5;

	[SerializeField]
	private float PenaltyCrashSecs = 0.5f;

	[SerializeField]
	private int PenaltyCrashMeters = 1;

	[SerializeField]
	private float PenaltyOutOfRoadSecs = 2f;

	[SerializeField]
	private GameObject[] StartRedLights;

	[SerializeField]
	private GameObject StartGreenLight;

	[SerializeField]
	private GameObject[] PitStopTriggers;

	[SerializeField]
	private CircuitRaceMapController MapCameraPrefab;

	[SerializeField]
	private Transform GlobalPoint;

	[SerializeField]
	private LemanRaceController LemanRaceController;

	[SerializeField]
	private CircuitAiTester CircuitAiTester;

	public CircuitRacingUIControl CircuitRacingUIControll;

	private IDisposable _subscription;

	private IDisposable _carsSubscription;

	private Transform _startLine;

	public CarCamerasController CamerasController;

	public GameObject ourCar;

	public Transform spawnPoint;

	private RacingCameraController _cameraController;

	public CircuitRacingUIControl ui;

	public CircuitCarsControl circuitCarsControl;

	public float TotalTrackDistance;

	public bool IsCarsLoaded;

	public Action PrepareToRaceAction;

	public RaceMapModel[] RaceMapModels;

	private int currentMapId;

	private GameObject[] raceAiGameObjects;

	private LapCheckpointTrigger[] _lapCheckpointTriggers;

	private VehicleEntityProvider _playersCar;

	private ManagerUI _managerUI;

	private Dictionary<int, float> _chartList;

	private int[] carPoses;

	private List<LapRaceCar> _winners;

	private MenuLevelParent _menuLevelParent;

	private Spline _spline;

	private bool _isRaceFinished;

	private bool _isDestroyed;

	private CancellationTokenSource _cancellationTokenSource;

	private float _targetSpeedAfterFinish;

	private float _minTargetCrashValue;

	private RatingSender _ratingController;

	private DailyTasksManager _dailyTasksManager;

	private IPlayerStorageService _playerStorageService;

	private IPlayerProfileService _playerProfileService;

	private IConnectionManager _connectionManager;

	private IAccountController _accountController;

	private IGuestAccountCreator _guestAccountCreator;

	private ICircuitRacingRewardService _circuitRacingRewardService;

	private CurrencyManagerController _currencyManagerController;

	private ConfigValuesStorage _remoteConfigData;

	private IRemoteConfigManager _remoteConfigManager;

	private InputItemsProvider _controlItemsProvider;

	private CoreGameManager _coreGameManager;

	private FreeDriveProvider _freeDriveProvider;

	private CarInputManager _carInputManager;

	private RacingAnalytic _racingAnalytic;

	private IPrefabFactory _prefabFactory;

	[SerializeField]
	private int[] MaxClassTimes;

	private Transform _cameraTransform;

	private RaceMapModel _currentRaceMapModel;

	public Action<string, string> RaceProgressAction;

	private bool _isRaceStarted;

	private List<IRaceControllerModule> _modules = new List<IRaceControllerModule>();

	private bool inPitStop;

	private bool _isCalculating;

	private LapRaceTrafficLightUI _lapRaceTrafficLightUI;

	private RaceModeController _raceModeController;

	private RaceEnvironment.TimeSettings _defaultTimeSettings;

	[HideInInspector]
	public bool IsUserCrossStartLine;

	[Header("Garage")]
	[SerializeField]
	private Transform[] AiCarsGarageStartPositions;

	private bool _isTestMode;

	private float _startTime;

	private DateTime _startTimeUtc;

	private float _endTime;

	private string _carName;

	private List<float> _testCarLapTime = new List<float>();

	private ITokenService _tokenService;

	private CircuitRacingConfig _circuitRacingConfig;

	private const int MinCount = 2;

	private const float HardFinishGapSecs = 2f;

	private const float MinHardFinishAllTimeSecs = 0.01f;

	private CancellationTokenSource _cancellationTokenSourceRaceStopped;

	private ISteamAchievementService _steamAchievementService;

	private const string SteamQualifiedMapPrefKey = "steam_pole_qualified_map";

	public RaceConfigSO CurrentRaceConfig { get; private set; }

	public RaceState CurrentRaceState { get; set; }

	public RaceCrewTalkSoundController RaceCrewTalkSoundController { get; private set; }

	public Action OnRewardCalculated { get; set; }

	public float ModeDuration => (float)(DateTime.UtcNow - _startTimeUtc).TotalSeconds;

	public bool IsUserFinishSequenceStarted { get; private set; }

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, PlayerDatas.CarLoadingUtility carLoadingUtility, ManagerUI managerUI, MenuLevelParent menuLevelParent, CurrencyManagerController currencyManagerController, IPlayerStorageService playerStorageService, Camera camera, IConnectionManager connectionManager, IAccountController accountController, ICircuitRacingRewardService circuitRacingRewardService, ITokenService tokenService, CircuitRacingConfig circuitRacingConfig, IRemoteConfigManager remoteConfigManager, CarInputManager carInputManager, RatingSender ratingController, DailyTasksManager dailyTasksManager, InputItemsProvider controlItemsProvider, CoreGameManager coreGameManager, FreeDriveProvider freeDriveProvider, RacingAnalytic racingAnalytic, RacingCameraController racingCameraController, CircuitRacingWaitingWindow circuitRacingWaitingWindow, LapRaceTrafficLightUI lapRaceTrafficLightUI, IGuestAccountCreator guestAccountCreator, RaceModeController raceModeController, IPlayerProfileService playerProfileService, ISteamAchievementService steamAchievementService)
	{
		_playerProfileService = playerProfileService;
		_prefabFactory = prefabFactory;
		_dailyTasksManager = dailyTasksManager;
		_ratingController = ratingController;
		_freeDriveProvider = freeDriveProvider;
		_coreGameManager = coreGameManager;
		_controlItemsProvider = controlItemsProvider;
		_remoteConfigManager = remoteConfigManager;
		_circuitRacingConfig = circuitRacingConfig;
		_tokenService = tokenService;
		_currencyManagerController = currencyManagerController;
		_menuLevelParent = menuLevelParent;
		_managerUI = managerUI;
		_playerStorageService = playerStorageService;
		_cameraTransform = camera.transform;
		_connectionManager = connectionManager;
		_accountController = accountController;
		_circuitRacingRewardService = circuitRacingRewardService;
		_racingAnalytic = racingAnalytic;
		_carInputManager = carInputManager;
		_cameraController = racingCameraController;
		_circuitRacingWaitingWindow = circuitRacingWaitingWindow;
		_lapRaceTrafficLightUI = lapRaceTrafficLightUI;
		_guestAccountCreator = guestAccountCreator;
		_raceModeController = raceModeController;
		_steamAchievementService = steamAchievementService;
	}

	private void OnDestroy()
	{
		RaceModeController raceModeController = _raceModeController;
		raceModeController.ShowMainMenuAction = (Action)Delegate.Remove(raceModeController.ShowMainMenuAction, new Action(GoMainMenu));
		RaceEnvironment.RestoreTimeSettings(_defaultTimeSettings);
		FlagsStorage.SingeDragRacingMode = false;
		_cancellationTokenSource?.Dispose();
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceRaceStopped);
		_isDestroyed = true;
		_currentRaceMapModel = null;
		RaceMapModels = null;
		if (RaceCrewTalkSoundController != null)
		{
			RaceCrewTalkSoundController.StopRaceCrewTalkSoundEvent();
		}
		V.CoreGameManager = null;
		RaceCrewTalkSoundController?.Dispose();
		PlayerDatas.UnloadUnusedAssets("CircuitRace dispose");
	}

	public void StartTestMode(GameObject car)
	{
		_isTestMode = true;
		_startTime = 0f;
		_endTime = 0f;
		_carName = car.name;
		_testCarLapTime = new List<float>();
		SetDelayedSpeedometer(car).Forget();
	}

	private async UniTaskVoid SetDelayedSpeedometer(GameObject car)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(2.0));
		SpeedometerUI speedometerUI = _controlItemsProvider?.SpeedometerUI;
		if ((bool)speedometerUI)
		{
			speedometerUI.SetNewCar(car);
			speedometerUI.SetTestMode(state: true);
		}
	}

	public RaceState GetRaceState()
	{
		return CurrentRaceState;
	}

	public void MarkUserFinishSequenceStarted()
	{
		IsUserFinishSequenceStarted = true;
	}

	public void ShowPitStopTriggers(bool state)
	{
		GameObject[] pitStopTriggers = PitStopTriggers;
		for (int i = 0; i < pitStopTriggers.Length; i++)
		{
			pitStopTriggers[i].SetActive(state);
		}
	}

	public void CheckMaintenance(LapRaceCar lapRaceCar, int wheelHealthPercent, int fuelCapacityPercent)
	{
		MaintenanceStatus maintenanceStatus = CheckMaintenanceStatus(fuelCapacityPercent);
		if (maintenanceStatus != lapRaceCar.CurrentFuelStatus)
		{
			lapRaceCar.CurrentFuelStatus = maintenanceStatus;
			if (maintenanceStatus != MaintenanceStatus.Good)
			{
				CircuitRacingUIControll.SetMaintenanceMessage(MaintenanceType.Fuel, maintenanceStatus);
				RaceCrewTalkSoundController.PlayPitStopSoundEvent(MaintenanceType.Fuel);
				return;
			}
		}
		MaintenanceStatus maintenanceStatus2 = CheckMaintenanceStatus(wheelHealthPercent);
		if (maintenanceStatus2 != lapRaceCar.CurrentWheelStatus)
		{
			lapRaceCar.CurrentWheelStatus = maintenanceStatus2;
			if (maintenanceStatus2 != MaintenanceStatus.Good)
			{
				CircuitRacingUIControll.SetMaintenanceMessage(MaintenanceType.Wheel, maintenanceStatus2);
				RaceCrewTalkSoundController.PlayPitStopSoundEvent(MaintenanceType.Wheel);
			}
		}
	}

	private MaintenanceStatus CheckMaintenanceStatus(int percentValue)
	{
		MaintenanceStatus result = MaintenanceStatus.Good;
		if (percentValue > 25 && percentValue < 50)
		{
			result = MaintenanceStatus.Warning;
		}
		if (percentValue > 10 && percentValue < 25)
		{
			result = MaintenanceStatus.Error;
		}
		if (percentValue <= 10)
		{
			result = MaintenanceStatus.Critical;
		}
		return result;
	}

	public void StartNextTrack()
	{
		if (CurrentRaceState == RaceState.RaceStarted || CurrentRaceState == RaceState.RaceFinished || CurrentRaceState == RaceState.RaceStaged)
		{
			Cancel();
		}
		circuitCarsControl.DestroyTestCar();
		UnsubscribeMethods();
		if (CurrentMapId >= RaceMapModels.Length - 1)
		{
			CurrentMapId = 0;
		}
		else
		{
			CurrentMapId++;
		}
		SetMapIndex(CurrentMapId);
		StartRace();
	}

	public int GetCurrentMapIndex()
	{
		return CurrentMapId;
	}

	public Transform GetCameraTransform()
	{
		return _cameraTransform;
	}

	public void Init(int id)
	{
		CircuitRacingUIControll.ResetTimer();
		CircuitRacingUIControll.ShowToRoad(state: false);
		LemanRaceController.Reset();
		RaceMapModel raceMapModel = RaceMapModels[id];
		CurrentRaceConfig = raceMapModel.RaceConfig;
		circuitCarsControl.SetConfigToStearing(raceMapModel.RaceConfig);
		currentMapId = id;
		_cancellationTokenSource?.Dispose();
		_cancellationTokenSource = new CancellationTokenSource();
		_currentRaceMapModel = raceMapModel;
		if (raceMapModel.RaceConfig.IsLeman)
		{
			LemanRaceController.Init(raceMapModel);
		}
		_lapCheckpointTriggers = raceMapModel.CheckPointsParent.GetComponentsInChildren<LapCheckpointTrigger>();
		QualificationLapsCount = raceMapModel.RaceConfig.QualificationLapsCount;
		StartPositions = ((Direction > 0) ? raceMapModel.StartPositions : raceMapModel.ReverseStartPositions);
		CheckPointsParent = raceMapModel.CheckPointsParent;
		LapsCount = raceMapModel.RaceConfig.LapsCount;
		circuitCarsControl.SetCarsLapsCount(LapsCount);
		WaitAfterFinish = raceMapModel.RaceConfig.WaitAfterFinish;
		PenaltyCrashSecs = raceMapModel.RaceConfig.PenaltyCrashSecs;
		StartRedLights = raceMapModel.RedLights;
		StartGreenLight = raceMapModel.GreenLights;
		_startLine = _lapCheckpointTriggers[0].transform;
		_targetSpeedAfterFinish = raceMapModel.RaceConfig.TargetSpeedAfterFinish;
		PenaltyOutOfRoadSecs = raceMapModel.RaceConfig.PenaltyOutOfRoadDistance;
		_minTargetCrashValue = raceMapModel.RaceConfig.MinTargetCrashValue;
		_spline = ((Direction > 0) ? raceMapModel.RaceConfig.Spline : raceMapModel.RaceConfig.ReversedSpline());
		TotalTrackDistance = raceMapModel.RaceConfig.LapDistance;
		InitCheckPoints();
		PrepareMap(id);
		raceMapModel.SetCheckpointsDirection(Direction);
		ShowPitStopTriggers(raceMapModel.RaceConfig.IsLeman);
		EnableAll(isUser: true);
		_isRaceStarted = false;
	}

	public int[] GetCurrentRewards()
	{
		CurrentClassId.Get(out int value);
		int[] rewards = _circuitRacingConfig.MapDatas[CurrentMapId].Groups[value].Rewards;
		int num = rewards.Length;
		int[] array = new int[num];
		int num2 = (int)_playerProfileService.CircuitMoneyEarned;
		int num3 = (int)_playerProfileService.CircuitCoinsEarned;
		for (int num4 = num - 1; num4 >= 0; num4--)
		{
			if (num4 == 0)
			{
				int num5 = _remoteConfigData.raceRewardCoinsLimit - num3;
				if (rewards[num4] > num5)
				{
					array[num4] = num5;
				}
				else
				{
					array[num4] = rewards[num4];
				}
				num3 += array[num4];
			}
			else
			{
				int num6 = _remoteConfigData.raceRewardMoneyLimit - num2;
				if (rewards[num4] > num6)
				{
					array[num4] = num6;
				}
				else
				{
					array[num4] = rewards[num4];
				}
				num2 += array[num4];
			}
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] < 0)
			{
				array[i] = 0;
			}
		}
		return array;
	}

	public RaceMapModel GetRaceMapModel()
	{
		return _currentRaceMapModel;
	}

	public bool IsCurrentMapLeman()
	{
		if (_currentRaceMapModel == null)
		{
			return false;
		}
		return _currentRaceMapModel.RaceConfig.IsLeman;
	}

	public void PrepareMapForTest()
	{
		_managerUI.ShowCircuitPlayWindow();
		int num = currentMapId;
		RaceConfigSO raceConfig = RaceMapModels[num].RaceConfig;
		Direction = raceConfig.Direction;
		CurrentRaceConfig = raceConfig;
		StartPositions = ((Direction > 0) ? RaceMapModels[num].StartPositions : RaceMapModels[num].ReverseStartPositions);
		PrepareMap(num);
		RaceMapModels[num].SetDirectionForTest(Direction);
		_startLine = RaceMapModels[num].StarLine;
	}

	private void PrepareMap(int index)
	{
		for (int i = 0; i < RaceMapModels.Length; i++)
		{
			bool active = i == index;
			RaceMapModels[i].SetActive(active);
			GameObject[] obstaclesForEnable = RaceMapModels[i].ObstaclesForEnable;
			for (int j = 0; j < obstaclesForEnable.Length; j++)
			{
				obstaclesForEnable[j].SetActive(value: false);
			}
		}
		if (!_isTestMode)
		{
			GameObject[] obstaclesForEnable2 = RaceMapModels[CurrentMapId].ObstaclesForEnable;
			for (int k = 0; k < obstaclesForEnable2.Length; k++)
			{
				obstaclesForEnable2[k].SetActive(value: true);
			}
		}
	}

	public Transform GetStartLine()
	{
		return _startLine;
	}

	public Spline GetSpline()
	{
		return _spline;
	}

	public float GetPenaltyCrashSecsVar()
	{
		return PenaltyCrashSecs;
	}

	public float GetPenaltyOutOfRoadVar()
	{
		return PenaltyOutOfRoadSecs;
	}

	public float GetPenaltyCrashMetersVar()
	{
		return PenaltyCrashMeters;
	}

	public float GetMinCrashValue()
	{
		return _minTargetCrashValue;
	}

	public float GetOutOfRoadPenaltyVar()
	{
		return PenaltyOutOfRoadSecs;
	}

	public void RegisterAiCar()
	{
	}

	public LapCheckpointTrigger[] GetCheckPoints()
	{
		return _lapCheckpointTriggers;
	}

	private async UniTaskVoid GetCarsPositions()
	{
		foreach (LapRaceCar raceCar in circuitCarsControl.GetRaceCars())
		{
			raceCar.CalculatePos();
			await UniTask.NextFrame();
			if (_isDestroyed)
			{
				return;
			}
		}
	}

	private bool IsCanceled(CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return true;
		}
		return false;
	}

	private async UniTaskVoid DelayedStartRace(CancellationToken cancellationToken)
	{
		IsCarsLoaded = false;
		IsUserCrossStartLine = false;
		inPitStop = false;
		IsCanceled(cancellationToken);
		_cameraController.ChangeCameraFollowType(_cameraController.virtualCamera, CinemachineTransposer.BindingMode.LockToTarget);
		_currentRaceMapModel.SetUIMapElements(state: false);
		_lapRaceTrafficLightUI.SetActive(active: true);
		_lapRaceTrafficLightUI.ResetRedLights();
		_chartList = new Dictionary<int, float>();
		carPoses = new int[StartPositions.Length];
		string lapsCount = "/" + LapsCount;
		CircuitRacingUIControll.SetProgress("0", lapsCount, "", "");
		CircuitRacingUIControll.ShowRaceHudElemetns(state: false);
		InitCheckPoints();
		StartRideOnTrack().Forget();
		if (!IsQualification)
		{
			await DelayedCreateCars();
			bool flag = IsCanceled(cancellationToken);
			if (_isDestroyed | flag)
			{
				return;
			}
			_subscription = Observable.Interval(TimeSpan.FromSeconds(0.5)).DelaySubscription(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				CheckList();
			})
				.AddTo(this);
		}
		else
		{
			CurrentRaceState = RaceState.RaceStaged;
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
			await circuitCarsControl.CreateQualificationCar(StartPositions[0]);
			bool flag = IsCanceled(cancellationToken);
			if (_isDestroyed | flag)
			{
				return;
			}
			await UniTask.Delay(TimeSpan.FromSeconds(1.0));
			flag = IsCanceled(cancellationToken);
			if (_isDestroyed | flag)
			{
				return;
			}
			_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
		}
		DelayedStartRaceAfterCarsCreated(cancellationToken);
	}

	private async UniTaskVoid DelayedStartRaceAfterCarsCreated(CancellationToken cancellationToken)
	{
		bool flag = IsCanceled(cancellationToken);
		if (_isDestroyed | flag)
		{
			return;
		}
		_lapRaceTrafficLightUI.Reset();
		for (int i = 0; i < StartRedLights.Length; i++)
		{
			RaceCrewTalkSoundController.PlayRedLightSound();
			_lapRaceTrafficLightUI.SetRedLight(i);
			StartRedLights[i].gameObject.SetActive(value: true);
			if (i == StartRedLights.Length - 1 && !IsQualification)
			{
				PrepareToRaceAction?.Invoke();
				circuitCarsControl.SetKinematicCars(state: true);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(1.25));
			flag = IsCanceled(cancellationToken);
			CurrentRaceState = RaceState.RaceStarted;
			if (_isDestroyed | flag)
			{
				return;
			}
			StartRedLights[i].gameObject.SetActive(value: false);
		}
		_cameraController.ChangeCameraFollowType(_cameraController.virtualCamera, CinemachineTransposer.BindingMode.LockToTarget);
		_lapRaceTrafficLightUI.ResetRedLights();
		CircuitRacingUIControll.SetSmallMap(RaceMapModels[CurrentMapId].MapTexture);
		CircuitRacingUIControll.ClearMap();
		CircuitRacingUIControll.ResetTimer();
		await UniTask.Delay(TimeSpan.FromSeconds(1.25));
		flag = IsCanceled(cancellationToken);
		if (_isDestroyed | flag)
		{
			return;
		}
		if (IsQualification)
		{
			CircuitRacingUIControll.ShowChartList(state: true);
			CircuitRacingUIControll.ShowTimerPanel();
		}
		_isRaceStarted = true;
		foreach (LapRaceCar raceCar in circuitCarsControl.GetRaceCars())
		{
			raceCar.FreezeCar(state: false);
			if (!IsQualification)
			{
				raceCar.ChangeCarState(CarSteeringState.Race, notResetWaypoint: true);
			}
			if (raceCar != circuitCarsControl.GetUserLapRaceCar())
			{
				CircuitRacingUIControll.SetCarOnMap(raceCar.transform);
			}
		}
		RaceCrewTalkSoundController.PlayGreenLightSound();
		_lapRaceTrafficLightUI.SetGreenLight(state: true);
		circuitCarsControl.SetKinematicCars(state: false);
		StartGreenLight.SetActive(value: true);
		if (_currentRaceMapModel.RaceConfig.IsLeman)
		{
			LemanRaceController.InitPitStopTriggers();
		}
		CircuitRacingUIControll.SetOwnCarOnMap(circuitCarsControl.GetUserLapRaceCar().GetMiniMapTransform());
		CircuitRacingUIControll.ShowToRoad(state: true);
		RaceCrewTalkSoundController.PlayStartRaceSoundEvent();
		for (int j = 0; j < _modules.Count; j++)
		{
			_modules[j].RaceStarted();
		}
	}

	private void InitCheckPoints()
	{
		for (int i = 0; i < _lapCheckpointTriggers.Length; i++)
		{
			_lapCheckpointTriggers[i].Init(i);
		}
	}

	private void Awake()
	{
		RaceCrewTalkSoundController = _prefabFactory.Create<RaceCrewTalkSoundController>(Array.Empty<object>());
		RaceCrewTalkSoundController.Init(this);
	}

	public int GetRaceLapsCount()
	{
		return LapsCount;
	}

	public int GetQualificationRaceLapsCount()
	{
		return QualificationLapsCount;
	}

	public int GetTriggersCount()
	{
		return _lapCheckpointTriggers.Length;
	}

	public void SetMapIndex(int index)
	{
		CurrentMapId = index;
		for (int i = 0; i < RaceMapModels.Length; i++)
		{
			RaceMapModels[i].SetUIMapElements(state: false);
		}
		RaceMapModels[index].SetUIMapElements(state: true);
	}

	public void StartRaceOrQualification()
	{
		Time.timeScale = 1f;
		FlagsStorage.IsInputEnabledInThisWindow = true;
		if (IsQualification)
		{
			StartQualification();
			_managerUI.ShowCircuitPlayWindow();
		}
		else
		{
			StartRace();
		}
	}

	private void StartRace()
	{
		IsUserFinishSequenceStarted = false;
		circuitCarsControl.SetCarsDirection();
		_racingAnalytic.LogRaceStart((RaceType)CurrentMapId);
		_startTimeUtc = DateTime.UtcNow;
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceRaceStopped);
		_cancellationTokenSourceRaceStopped = new CancellationTokenSource();
		_chartList = new Dictionary<int, float>();
		_managerUI.ShowCircuitPlayWindow();
		Init(CurrentMapId);
		StartGreenLight.SetActive(value: false);
		DelayedStartRace(_cancellationTokenSourceRaceStopped.Token).Forget();
		_cameraController.ResetDynamicRotation().Forget();
	}

	public void ResetUserCameraInertia()
	{
		_cameraController.ResetCameraInertia();
	}

	public void StartQualification()
	{
		IsUserFinishSequenceStarted = false;
		_racingAnalytic.LogRaceStart((RaceType)CurrentMapId);
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceRaceStopped);
		_cancellationTokenSourceRaceStopped = new CancellationTokenSource();
		Init(CurrentMapId);
		IsQualification = true;
		StartGreenLight.SetActive(value: false);
		DelayedStartRace(_cancellationTokenSourceRaceStopped.Token).Forget();
	}

	private async UniTask DelayedCreateCars()
	{
		circuitCarsControl.MoveCarsToSpawnPoints();
		CurrentRaceState = RaceState.RaceStaged;
		_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
		circuitCarsControl.RestartCars();
		if (_isDestroyed)
		{
			return;
		}
		foreach (LapRaceCar raceCar in circuitCarsControl.GetRaceCars())
		{
			_chartList.Add(raceCar.GetRaceCarId(), 0f);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(1.0));
		_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
	}

	public async UniTaskVoid DelayedStartRaceMode()
	{
		if ((bool)_circuitRacingWaitingWindow)
		{
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
		}
		await UniTask.WaitUntil(() => !ScenesHelper.IsMenuSceneActive()).TimeoutWithoutException(TimeSpan.FromSeconds(8.0));
		PlayerDatas.UnloadResourcesForce();
		await UniTask.DelayFrame(3);
		StartRaceMode();
	}

	private void StartRaceMode()
	{
		CPMGraphicsSettingsManager.Instance.RenderPropsDistanceController.SetSettings(GraphicsDistanceController.PriorityType.RacingCircuit, 65f, GraphicsDistanceController.SetType.SetIfLarger);
		CPMGraphicsSettingsManager.Instance.RenderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.RacingCircuit, 120f, GraphicsDistanceController.SetType.SetIfLarger);
		CurrentRaceState = RaceState.GarageActive;
		RaceEnvironment.Apply(WeatherType.Clear, DayTimeType.Day);
		if (CurrentRaceConfig == null && RaceMapModels.Length != 0)
		{
			CurrentRaceConfig = RaceMapModels[0].RaceConfig;
		}
		DelayedInit();
		if (FirebaseAuth.DefaultInstance.CurrentUser != null)
		{
			_tokenService.LoadToken().Forget();
		}
		SetRemoteConfigData().Forget();
		_modules.Add(new PlayerNoFuel(this, () =>
		{
			TryDisqualificationAsync(DisqualificationType.NoFuel).Forget();
		}));
		_modules.Add(new PlayerCarBroken(this, () =>
		{
			TryDisqualificationAsync(DisqualificationType.CarBroken).Forget();
		}));
	}

	private void Start()
	{
		_defaultTimeSettings = RaceEnvironment.CaptureTimeSettings();
		RaceModeController raceModeController = _raceModeController;
		raceModeController.ShowMainMenuAction = (Action)Delegate.Combine(raceModeController.ShowMainMenuAction, new Action(GoMainMenu));
		StartAsync().Forget();
	}

	private async UniTask StartAsync()
	{
		await InitDriverF1IfNeedsAsync();
		DelayedStartRaceMode().Forget();
		await _raceModeController.InitializeScene(V.placeToSpawn.Racing);
	}

	private void Update()
	{
		for (int i = 0; i < _modules.Count; i++)
		{
			_modules[i].Update();
		}
	}

	private async UniTask SetRemoteConfigData()
	{
		_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
		_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
	}

	private async UniTaskVoid DelayedInit()
	{
		OnStart();
		if (!(await circuitCarsControl.CarReady.SuppressCancellationThrow()) && !_isDestroyed)
		{
			OnGarageOpen();
			await circuitCarsControl.CreateSeveralRaceCars();
			IsCarsLoaded = true;
			_cameraController.Init();
			_cameraController.PrepareCarReferences();
			_cameraController.SetCameraForGarage(V.LastUsedCar);
			_circuitRacingWaitingWindow.DelayedDisableWindow(1.5f).Forget();
			await _cameraController.StartMoveVeryFirstCam();
			CircuitRacingUIControll.SetUIForGarage();
			if (!_isDestroyed)
			{
				_cameraController.SetCameraForGarage(V.LastUsedCar);
			}
		}
	}

	private void OnStart()
	{
		FlagsStorage.RacingGameMode = true;
		FlagsStorage.SingeDragRacingMode = true;
		FlagsStorage.IsMainMenu = false;
	}

	public async UniTaskVoid StartRideOnTrack()
	{
		_cameraController.PrepareCarReferences();
		_cameraController.DisableAllCamera();
		await UniTask.Yield();
		_cameraController.setCamera(circuitCarsControl.ourCar);
		await UniTask.Yield();
		circuitCarsControl.SetCarToRacing(circuitCarsControl.ourCar);
		_cameraController.SetCameraForRacing();
	}

	public void EnableAutoPilot(bool state)
	{
		_carInputManager.SetAutopilot(state);
	}

	public void SetSteering()
	{
		if ((bool)circuitCarsControl.ourCar)
		{
			_carInputManager.IgnoreSteerControl(state: false);
		}
		_menuLevelParent.SetSteering();
	}

	public Transform[] GetStartPositions()
	{
		if (StartPositions == null || StartPositions.Length == 0)
		{
			StartPositions = ((Direction > 0) ? RaceMapModels[0].StartPositions : RaceMapModels[0].ReverseStartPositions);
		}
		return StartPositions;
	}

	public void OnGarageOpen()
	{
		circuitCarsControl.SetOwnCarToGarage();
		PlacePersonOnGarage(V.DriverPerson, circuitCarsControl.ourCar);
	}

	private void PlacePersonOnGarage(GameObject person, GameObject car)
	{
		if (!person)
		{
			return;
		}
		Powertrain powertrain = car.GetComponent<Powertrain>();
		if (!powertrain)
		{
			UnityEngine.Object.Destroy(person);
			return;
		}
		(from _ in Observable.EveryUpdate()
			where (bool)powertrain && powertrain.Velocity.magnitude <= 0.1f
			select _).First().SelectMany((long _) =>
		{
			if ((bool)car.GetComponent<VehicleData>().Driver)
			{
				_coreGameManager.CarOutInControl.OutCar(person, car, enableCollidersImmediately: true, isChangePos: true, isSendRpc: false);
			}
			Vector3 position = car.transform.position;
			person.transform.position = position + car.transform.right * 2f;
			person.transform.LookAt(position + car.transform.forward * 5f);
			return Observable.Timer(TimeSpan.FromSeconds(2.0));
		}).Subscribe((long __) =>
		{
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
			{
				component.enabled = false;
				component.playingRandom = false;
			}
			if ((bool)person)
			{
				person.GetComponent<DriverActionController>().ik.enabled = true;
			}
		})
			.AddTo(this);
	}

	public void UserRideWrongWay(bool state)
	{
		CircuitRacingUIControll.SetWrongWay(state);
	}

	public void UserOutOfRoad(bool state)
	{
		CircuitRacingUIControll.SetOutOfRoad(state);
	}

	public void RegisterCheckPoint(GameObject car, int checkPointId, bool isFinishLine = false)
	{
		if (!_isTestMode)
		{
			LapRaceCar component = car.transform.parent.GetComponent<LapRaceCar>();
			if (!component)
			{
				return;
			}
			if (isFinishLine)
			{
				if (component.LastTriggerId == 1 || component.IsCheatFinish())
				{
					component.IsCrossWrongFinishSide = true;
					return;
				}
			}
			else if (_lapCheckpointTriggers != null && checkPointId != _lapCheckpointTriggers.Length - 1 && checkPointId > 0)
			{
				component.IsCrossWrongFinishSide = false;
				return;
			}
			component.LastTriggerId = checkPointId;
			if (component.IsFinishRace())
			{
				return;
			}
			if (component.IsUser() && component.GetCurrentLap() != -1 && component.IsWrongWay)
			{
				if (isFinishLine)
				{
					component.IsCrossWrongFinishSide = true;
				}
				return;
			}
			if (isFinishLine && !component.IsCrossWrongFinishSide)
			{
				OnFinishLap(component);
			}
			if (component.IsUser())
			{
				if (!component.IsFinishRace())
				{
					DrawNextCheckPoint(checkPointId);
				}
				component.IsCrossWrongFinishSide = false;
			}
		}
		else
		{
			if (!isFinishLine || !car.name.Contains("Clone"))
			{
				return;
			}
			if (_startTime == 0f)
			{
				_startTime = Time.time;
			}
			else if (_endTime == 0f)
			{
				_endTime = Time.time - _startTime;
				_testCarLapTime.Add(_endTime);
				string text = "";
				for (int i = 0; i < _testCarLapTime.Count; i++)
				{
					text = text + _testCarLapTime[i] + " ||| ";
				}
				_startTime = Time.time;
				_endTime = 0f;
			}
		}
	}

	private void DrawNextCheckPoint(int i)
	{
		int num = i + 1;
		if (_lapCheckpointTriggers.Length <= num)
		{
			num = 0;
		}
	}

	private void CheckList()
	{
		if (circuitCarsControl.GetUserLapRaceCar().IsWrongWay)
		{
			return;
		}
		List<LapRaceCar> raceCars = circuitCarsControl.GetRaceCars();
		raceCars = (from car in raceCars
			orderby car.GetCurrentLap() descending, car.GetTotalProgress() descending
			select car).ToList();
		int num = 0;
		int userIndex = 0;
		for (int num2 = 0; num2 < raceCars.Count; num2++)
		{
			raceCars[num2].place = num2;
			if (raceCars[num2].IsUser())
			{
				userIndex = num2;
				num = num2;
			}
		}
		int num3 = circuitCarsControl.GetUserLapRaceCar().GetCurrentLap() + 1;
		string arg = new LocalizedString("FreeDragRacingWindow.Lap");
		string arg2 = new LocalizedString("Circuitraceselectmapwindow.Place");
		string lapProgress = $"{arg} {num3}";
		string placeProgress = $"{arg2} {num + 1}";
		if (_currentRaceMapModel.RaceConfig.IsLeman)
		{
			CircuitRacingUIControll.SetPlaceProgress(placeProgress, "/" + raceCars.Count);
		}
		else
		{
			CircuitRacingUIControll.SetProgress(lapProgress, "/" + LapsCount, placeProgress, "/" + raceCars.Count);
		}
		if (!circuitCarsControl.GetUserLapRaceCar().IsFinishRace())
		{
			CircuitRacingUIControll.SetRealtimeChartList(raceCars, userIndex);
		}
	}

	public async UniTaskVoid OnFinishLap(LapRaceCar lapRaceCar)
	{
		if (lapRaceCar.IsUser())
		{
			while (FlagsStorage.IsPhotoMode)
			{
				await UniTask.Yield();
			}
		}
		float penaltyTime = lapRaceCar.GetPenaltyTime();
		float crashPenaltySecs = lapRaceCar.GetCrashPenaltySecs();
		if (lapRaceCar.GetCurrentLap() == -1)
		{
			if (lapRaceCar.IsUser())
			{
				if (_currentRaceMapModel.RaceConfig.IsLeman)
				{
					int lemansMinutesCount = _currentRaceMapModel.RaceConfig.LemansMinutesCount;
					LemanRaceController.SetTime(lemansMinutesCount);
				}
				CircuitRacingUIControll.ShowRaceHudElemetns(state: true);
				if (!IsQualification)
				{
					CircuitRacingUIControll.ShowChartList(state: true);
				}
				IsUserCrossStartLine = true;
				SubscribeMethods();
			}
			lapRaceCar.StartRace();
		}
		else
		{
			if (lapRaceCar.IsUser() && CheckParent())
			{
				return;
			}
			float totalSeconds = lapRaceCar.OnLapFinish() + penaltyTime + crashPenaltySecs;
			if (lapRaceCar.IsUser())
			{
				CircuitRacingUIControll.ShowTimerPanel();
				CircuitRacingUIControll.ResetTimer(isPlaySound: true);
			}
			string secs = Utils.FormatTime(totalSeconds);
			if (_currentRaceMapModel.RaceConfig.IsLeman)
			{
				if (lapRaceCar.IsLemanFinished)
				{
					return;
				}
				if (lapRaceCar.place == 0)
				{
					if (LemanRaceController.IsTimeExpired())
					{
						if (!lapRaceCar.IsUser())
						{
							RaceCrewTalkSoundController.PlayFinishLemanSoundEvent(lapRaceCar, isTimeExpired: true);
						}
						LemanRaceController.FinishRace(lapRaceCar, secs);
						LemanRaceController.LeaderFinishLeman();
						return;
					}
					if (LemanRaceController.IsNeedAddBonusTime())
					{
						RaceCrewTalkSoundController.PlayFinishLemanSoundEvent(lapRaceCar, isTimeExpired: false);
						OnLemanLeaderFinished(lapRaceCar);
						return;
					}
				}
				if (!LemanRaceController.IsTimeExpired())
				{
					EnableAll(lapRaceCar.IsUser());
				}
				else if (lapRaceCar.IsHasBonusLap)
				{
					if (lapRaceCar.IsUser())
					{
						CircuitRacingUIControll.SetLap(new LocalizedString("CircuitRaceController.FinalLap"), "");
					}
				}
				else
				{
					LemanRaceController.FinishRace(lapRaceCar, secs);
				}
			}
			else if (lapRaceCar.IsFinalLap())
			{
				await FinishRace(lapRaceCar);
			}
			else
			{
				EnableAll(lapRaceCar.IsUser());
			}
		}
	}

	private bool CheckParent()
	{
		int num = 0;
		if (_currentRaceMapModel.CheckerParent.childCount < 1)
		{
			return false;
		}
		foreach (Transform item in _currentRaceMapModel.CheckerParent)
		{
			if (!item.gameObject.activeSelf)
			{
				num++;
			}
		}
		if (_currentRaceMapModel.CheckerParent.childCount / 2 <= num)
		{
			return false;
		}
		circuitCarsControl.GetUserLapRaceCar().CurrentDisqualificationType = DisqualificationType.UnCorrectFinish;
		RemoveUser();
		return true;
	}

	public void RemoveUser()
	{
		IsUserFinishSequenceStarted = true;
		UnsubscribeMethods();
		LapRaceCar userLapRaceCar = circuitCarsControl.GetUserLapRaceCar();
		userLapRaceCar.FreezeCar(state: true);
		userLapRaceCar.FinishRace();
		RaceCrewTalkSoundController.PlayRaceCrewTalkSoundEvent(CrewTalkSoundType.You_DQ);
		CircuitRacingUIControll.ShowDisqualifiedWindow();
		ModuleFinished();
	}

	private void EnableAll(bool isUser)
	{
		if (!isUser)
		{
			return;
		}
		foreach (Transform item in _currentRaceMapModel.CheckerParent)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private void SubscribeMethods()
	{
		_carsSubscription = Observable.Interval(TimeSpan.FromSeconds(0.5)).DelaySubscription(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			GetCarsPositions();
		})
			.AddTo(this);
	}

	private void UnsubscribeMethods()
	{
		_subscription?.Dispose();
		_carsSubscription?.Dispose();
	}

	private void OnLemanLeaderFinished(LapRaceCar lapRaceCar)
	{
		LemanRaceController.SetPenaltyLap(lapRaceCar);
		LemanRaceController.AddBonusLap(lapRaceCar.GetRaceCarId());
	}

	private async UniTask FinishRace(LapRaceCar lapRaceCar)
	{
		if (lapRaceCar.IsUser())
		{
			IsUserFinishSequenceStarted = true;
		}
		while (FlagsStorage.IsPhotoMode)
		{
			await UniTask.Yield();
		}
		if (lapRaceCar.IsUser())
		{
			CircuitRacingUIControll.ShowChartList(state: false);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(2.0));
		if (_isDestroyed || !lapRaceCar)
		{
			return;
		}
		lapRaceCar.FinishRace();
		if (lapRaceCar.IsUser())
		{
			if (IsQualification)
			{
				CalculateQualificationResult();
				return;
			}
			if (lapRaceCar.IsUser())
			{
				circuitCarsControl.SetIgnoreCollisionWithCollider(lapRaceCar, ignore: true);
			}
			CalculateRaceResults();
		}
		else
		{
			OnRacerFinishRaceAfterUser(lapRaceCar);
		}
	}

	private void CalculateQualificationResult()
	{
		CircuitRacingUIControll.OnQualificationFinish(circuitCarsControl.GetUserLapRaceCar());
		_isRaceStarted = false;
		ObscuredPrefs.SetInt("gpg_pole_qualified_map", CurrentMapId);
		PlayerPrefs.SetInt("steam_pole_qualified_map", CurrentMapId);
	}

	private async UniTaskVoid CalculateRaceResults()
	{
		CircuitRacingUIControll.ShowChartList(state: false);
		CurrentRaceState = RaceState.RaceFinished;
		ModuleFinished();
		CircuitRacingUIControll.ShowRaceHudElemetns(state: false);
		List<LapRaceCar> lapRaceCars = circuitCarsControl.GetRaceCars();
		int waitTime = WaitAfterFinish;
		float penaltyTime = circuitCarsControl.GetUserLapRaceCar().GetPenaltyTime();
		float crashPenaltySecs = circuitCarsControl.GetUserLapRaceCar().GetCrashPenaltySecs();
		bool userFinishedWithZeroPenalty = penaltyTime == 0f && crashPenaltySecs == 0f;
		if (userFinishedWithZeroPenalty)
		{
			waitTime = 1;
		}
		await CircuitRacingUIControll.OnRaceFinish(lapRaceCars);
		if (_isDestroyed)
		{
			return;
		}
		if (circuitCarsControl.GetUserLapRaceCar().place != lapRaceCars.Count - 1)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("CircuitRaceController.WaitingOtherCars"), 3f).Forget();
		}
		CheckAllUsersFinished();
		await DelayedFinish(waitTime, _cancellationTokenSource.Token);
		if (_isDestroyed)
		{
			return;
		}
		_carsSubscription?.Dispose();
		if (!CircuitRacingUIControll.IsLeaderBoardPanelActive())
		{
			return;
		}
		lapRaceCars = lapRaceCars.OrderBy((LapRaceCar car) => car.GetTotalTime()).ToList();
		for (int num = 0; num < lapRaceCars.Count; num++)
		{
			LapRaceCar lapRaceCar = lapRaceCars[num];
			lapRaceCar.place = num;
			if (lapRaceCar.IsUser())
			{
				RaceCrewTalkSoundController.PlayFinishRaceSoundEvent(lapRaceCar);
			}
		}
		bool flag = circuitCarsControl.GetUserLapRaceCar().place == 0;
		if (userFinishedWithZeroPenalty & flag)
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQJQ", "circuit_race");
		}
		if (ObscuredPrefs.GetInt("gpg_pole_qualified_map", -1) == CurrentMapId)
		{
			if (flag)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQMg", "circuit_race");
			}
			ObscuredPrefs.DeleteKey("gpg_pole_qualified_map");
		}
		if (flag)
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEw", "circuit_race");
		}
		if (userFinishedWithZeroPenalty)
		{
			GooglePlayAchievements.MarkCleanSlateCircuit("circuit_race");
		}
		if (userFinishedWithZeroPenalty & flag)
		{
			_steamAchievementService?.Unlock("ACH_CLEAN_LINE");
		}
		if (flag && PlayerPrefs.GetInt("steam_pole_qualified_map", -1) == CurrentMapId)
		{
			_steamAchievementService?.Unlock("ACH_POLE_TO_PODIUM");
		}
		_steamAchievementService?.Unlock("ACH_CHECKERED_HELLO");
		_steamAchievementService?.MarkCleanSlateCircuit();
		CircuitRacingUIControll.SortUsers();
		_winners = lapRaceCars.GetRange(0, 3);
		Calculate(circuitCarsControl.GetUserLapRaceCar().place).Forget();
		ShowPitStopTriggers(state: false);
		_isRaceStarted = false;
		foreach (LapRaceCar item in lapRaceCars)
		{
			if (item.IsUser())
			{
				_racingAnalytic.LogRaceFinish(item, ModeDuration, "completed");
			}
		}
	}

	public void ShowPitStopMenu()
	{
		CircuitRacingUIControll.ShowPitStop(state: true);
	}

	public void StopPitStopMenu()
	{
		CircuitRacingUIControll.ShowPitStop(state: false);
	}

	public void UnlockSplashAndDash()
	{
		_steamAchievementService?.Unlock("ACH_SPLASH_AND_DASH");
	}

	public void UnlockRunningOnFumes()
	{
		_steamAchievementService?.Unlock("ACH_RUNNING_ON_FUMES");
	}

	private void OnRacerFinishRaceAfterUser(LapRaceCar lapRaceCar)
	{
		if ((bool)lapRaceCar || circuitCarsControl.GetUserLapRaceCar().IsFinishRace())
		{
			CircuitRacingUIControll.OnOneRaceCarFinishRace(lapRaceCar);
			CheckAllUsersFinished();
		}
	}

	private void CheckAllUsersFinished()
	{
		bool isRaceFinished = true;
		foreach (LapRaceCar raceCar in circuitCarsControl.GetRaceCars())
		{
			if (!raceCar.IsFinishRace())
			{
				isRaceFinished = false;
				break;
			}
		}
		_isRaceFinished = isRaceFinished;
	}

	public bool IsRaceFinished()
	{
		return _isRaceFinished;
	}

	private async UniTask DelayedFinish(int userWaitTime, CancellationToken cancellationToken)
	{
		for (int i = 0; i < userWaitTime; i++)
		{
			if (_isRaceFinished)
			{
				break;
			}
			if (userWaitTime == i + 1)
			{
				HardFinishNotFinishedCars();
				break;
			}
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				break;
			}
		}
	}

	private void HardFinishNotFinishedCars()
	{
		if (_isRaceFinished)
		{
			return;
		}
		foreach (var item in GetCarsToHardFinish())
		{
			HardFinishRace(item.Car, item.TotalTime);
			if (_isDestroyed)
			{
				break;
			}
			OnRacerFinishRaceAfterUser(item.Car);
		}
	}

	private List<(LapRaceCar Car, float TotalTime)> GetCarsToHardFinish()
	{
		List<LapRaceCar> raceCars = circuitCarsControl.GetRaceCars();
		float num = 0f;
		foreach (LapRaceCar item in raceCars)
		{
			if ((bool)item && item.IsFinishRace())
			{
				float totalTime = item.GetTotalTime();
				if (totalTime > num)
				{
					num = totalTime;
				}
			}
		}
		LapRaceCar userLapRaceCar = circuitCarsControl.GetUserLapRaceCar();
		if ((bool)userLapRaceCar && userLapRaceCar.IsFinishRace())
		{
			float totalTime2 = userLapRaceCar.GetTotalTime();
			if (totalTime2 > num)
			{
				num = totalTime2;
			}
		}
		float num2 = ((_startTimeUtc == default(DateTime)) ? 0f : ModeDuration);
		if (num < num2)
		{
			num = num2;
		}
		List<LapRaceCar> list = (from car in raceCars
			where (bool)car && !car.IsFinishRace() && !car.IsUser()
			orderby car.place
			select car).ToList();
		List<(LapRaceCar, float)> list2 = new List<(LapRaceCar, float)>(list.Count);
		for (int num3 = 0; num3 < list.Count; num3++)
		{
			list2.Add((list[num3], num + 2f * (float)(num3 + 1)));
		}
		return list2;
	}

	private void HardFinishRace(LapRaceCar lapRaceCar, float targetTotalTime)
	{
		if (!lapRaceCar.IsUser())
		{
			float num = targetTotalTime - lapRaceCar.GetPenaltyTime() - lapRaceCar.GetCrashPenaltySecs() - (float)lapRaceCar.place;
			if (num < 0.01f)
			{
				num = 0.01f;
			}
			lapRaceCar.SetAllTime(num);
			lapRaceCar.FinishRace();
			lapRaceCar.FreezeCar(state: true);
			circuitCarsControl.StopCar(lapRaceCar.gameObject);
		}
	}

	public async UniTask WaitForAllRacesToFinishAsync(List<LapRaceCar> raceModels, CancellationToken cancellationToken)
	{
		try
		{
			await UniTask.WhenAll(raceModels.Select((LapRaceCar carModel) => WaitUntilFinishedAsync(carModel, cancellationToken)));
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask WaitUntilFinishedAsync(LapRaceCar raceModel, CancellationToken cancellationToken)
	{
		await UniTask.WaitUntil(() => raceModel.IsFinishRace());
	}

	private void HardCompleteRace()
	{
		foreach (var item in GetCarsToHardFinish())
		{
			HardFinishRace(item.Car, item.TotalTime);
		}
		List<LapRaceCar> raceCars = circuitCarsControl.GetRaceCars();
		raceCars = raceCars.OrderBy((LapRaceCar car) => car.GetTotalTime()).ToList();
		for (int num = 0; num < raceCars.Count; num++)
		{
			raceCars[num].place = num;
		}
		CircuitRacingUIControll.SortUsers();
		_winners = raceCars.GetRange(0, 3);
		_isRaceFinished = true;
	}

	public void OnFinishRace()
	{
		_lapRaceTrafficLightUI.Reset();
		CheckAllUsersFinished();
		if (!_isRaceFinished)
		{
			HardCompleteRace();
		}
		bool flag = _winners.FirstOrDefault((LapRaceCar car) => car.IsUser()) != null;
		int place = 0;
		if (flag)
		{
			for (int num = 0; num < _winners.Count; num++)
			{
				if (_winners[num].IsUser())
				{
					place = num;
					break;
				}
			}
		}
		else
		{
			place = 4;
		}
		foreach (LapRaceCar winner in _winners)
		{
			winner.FreezeCar(state: true);
			circuitCarsControl.StopCar(winner.gameObject);
		}
		CircuitRacingUIControll.ShowResult(place);
		if (flag)
		{
			OnWon(_winners);
		}
		else
		{
			OnLoose(_winners);
		}
	}

	public void SetWinnerPersons(List<LapRaceCar> winners)
	{
	}

	private void OnWon(List<LapRaceCar> winners)
	{
		SetWinnerPersons(winners);
	}

	private void SaveResult(int place)
	{
		List<CircuitRacingRecord> source = GetTrackRecords()[CurrentMapId];
		CurrentClassId.Get(out int classId);
		CircuitRacingRecord circuitRacingRecord = source.FirstOrDefault((CircuitRacingRecord record) => record.ClassIndex == classId);
		if (circuitRacingRecord == null)
		{
			return;
		}
		if (circuitRacingRecord.BestTime <= 0f || circuitRacingRecord.BestTime > circuitCarsControl.GetUserLapRaceCar().GetTotalTime())
		{
			circuitRacingRecord.BestTime = circuitCarsControl.GetUserLapRaceCar().GetTotalTime();
		}
		if (place <= 2 && place >= 0)
		{
			switch (place)
			{
			case 0:
				circuitRacingRecord.FirstPlace++;
				break;
			case 1:
				circuitRacingRecord.SecondPlace++;
				break;
			case 2:
				circuitRacingRecord.ThirdPlace++;
				break;
			}
			_playerStorageService.Save();
		}
	}

	public async UniTask Calculate(int place)
	{
		int hardCurrency = 0;
		int softCurrency = 0;
		if (_isCalculating)
		{
			OnRewardCalculated?.Invoke();
			return;
		}
		_isCalculating = true;
		HardCurrency = 0;
		SoftReward = 0;
		int num = 0;
		if (_currentRaceMapModel.CheckerParent.childCount < 1)
		{
			_isCalculating = false;
			OnRewardCalculated?.Invoke();
			return;
		}
		foreach (Transform item in _currentRaceMapModel.CheckerParent)
		{
			if (!item.gameObject.activeSelf)
			{
				num++;
			}
		}
		if (_currentRaceMapModel.CheckerParent.childCount / 2 >= num)
		{
			_isCalculating = false;
			OnRewardCalculated?.Invoke();
			return;
		}
		_connectionManager.CorrectHash.Get(out var value);
		if (!value)
		{
			_isCalculating = false;
			OnRewardCalculated?.Invoke();
			return;
		}
		int moneyEarnedAtStart = (int)_playerProfileService.CircuitMoneyEarned;
		int moneyEarned = moneyEarnedAtStart;
		CurrentClassId.Get(out int value2);
		CircuitRaceConfigItem groupConfig = _circuitRacingConfig.MapDatas[CurrentMapId].Groups[value2];
		if (place == 0 && _playerProfileService.CircuitCoinsEarned < _remoteConfigData.raceRewardCoinsLimit)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				string token = _tokenService.Token;
				bool firebaseUserWasNull = FirebaseAuth.DefaultInstance.CurrentUser == null;
				if (!(await _guestAccountCreator.CreateGuestAccountIfNeeds()))
				{
					OnRewardCalculated?.Invoke();
					_isCalculating = false;
					return;
				}
				if (string.IsNullOrEmpty(token) & firebaseUserWasNull)
				{
					await _tokenService.LoadToken();
					token = _tokenService.Token;
				}
				if (token != null && !string.IsNullOrEmpty(token))
				{
					WalletOperationResult walletOperationResult = await _circuitRacingRewardService.GetCircuitRaceReward(CurrentMapId, CurrentClassId, place, token);
					if (!string.IsNullOrEmpty(walletOperationResult.Token))
					{
						_tokenService.Token = walletOperationResult.Token;
					}
					int num2 = 0;
					if (walletOperationResult.IsSuccess())
					{
						string text = "";
						int num3 = 0;
						if (walletOperationResult.Value > 0)
						{
							_playerProfileService.AddCircuitCoinsEarned(walletOperationResult.Value);
							num3 += walletOperationResult.Value;
							GetterCoinsManager.AddCoinsToPrefs(new AdvancedInt(walletOperationResult.Value));
							hardCurrency += walletOperationResult.Value;
							_currencyManagerController.UpdateCoinsAnimated();
							text += string.Format("+{0} {1}", walletOperationResult.Value, new LocalizedString("HomesConfig.Coins"));
						}
						if (moneyEarned < _remoteConfigData.raceRewardMoneyLimit)
						{
							int num4 = ((groupConfig.Rewards.Length > 1) ? groupConfig.Rewards[1] : 0);
							int num5 = ((groupConfig.Rewards.Length > 2) ? groupConfig.Rewards[2] : 0);
							int num6 = num4 + num5;
							int num7 = _remoteConfigData.raceRewardMoneyLimit - moneyEarned;
							if (num6 > num7)
							{
								num6 = num7;
								num2 = 2;
								_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), num2).Forget();
							}
							_currencyManagerController.AddMoney(new AdvancedInt(num6));
							softCurrency += num6;
							moneyEarned += num6;
							num3 += num6;
							text += string.Format("+{0} {1}  ", num6, new LocalizedString("HomesConfig.Money"));
						}
						else
						{
							num2 = 2;
							_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), num2).Forget();
						}
						if (num3 > 0)
						{
							ShowRewardNotification(text, num2).Forget();
						}
						_playerProfileService.AddCircuitMoneyEarned(moneyEarned - moneyEarnedAtStart);
					}
					else
					{
						if (walletOperationResult.Success != 2)
						{
							_isCalculating = false;
							OnRewardCalculated?.Invoke();
							return;
						}
						if (moneyEarned < _remoteConfigData.raceRewardMoneyLimit)
						{
							if (place < groupConfig.Rewards.Length)
							{
								int num8 = ((groupConfig.Rewards.Length > 2) ? groupConfig.Rewards[2] : 0);
								int num9 = ((groupConfig.Rewards.Length > 1) ? groupConfig.Rewards[1] : 0);
								int num10 = num8 + num9;
								int num11 = _remoteConfigData.raceRewardMoneyLimit - moneyEarned;
								if (num10 > num11)
								{
									num10 = num11;
									num2 = 2;
									_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), num2).Forget();
								}
								_currencyManagerController.AddMoney(new AdvancedInt(num10));
								softCurrency += num10;
								moneyEarned += num10;
								string arg = new LocalizedString("HomesConfig.Money");
								ShowRewardNotification($"+{num10} {arg}", num2).Forget();
								_playerProfileService.AddCircuitMoneyEarned(moneyEarned - moneyEarnedAtStart);
							}
						}
						else
						{
							_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), 2f).Forget();
						}
					}
				}
			}
			else if (moneyEarned < _remoteConfigData.raceRewardMoneyLimit)
			{
				if (place < groupConfig.Rewards.Length)
				{
					int num12 = 3;
					_managerUI.ShowNotificationWindow(string.Format("{0}", new LocalizedString("CircuitRaceController.CantGetCoinWithoutInternet")), num12).Forget();
					int num13 = groupConfig.Rewards[1] + groupConfig.Rewards[1] + groupConfig.Rewards[2];
					int num14 = _remoteConfigData.raceRewardMoneyLimit - moneyEarned;
					if (num13 > num14)
					{
						num13 = num14;
						_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), num12).Forget();
					}
					_currencyManagerController.AddMoney(new AdvancedInt(num13));
					moneyEarned += num13;
					softCurrency += num13;
					string arg2 = new LocalizedString("HomesConfig.Money");
					ShowRewardNotification($"+{num13} {arg2}", num12).Forget();
					_playerProfileService.AddCircuitMoneyEarned(moneyEarned - moneyEarnedAtStart);
				}
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), 2f).Forget();
			}
		}
		else
		{
			if (place < 0 || place >= 3)
			{
				_isCalculating = false;
				OnRewardCalculated?.Invoke();
				return;
			}
			if (moneyEarned < _remoteConfigData.raceRewardMoneyLimit)
			{
				int num15 = 0;
				if (place < groupConfig.Rewards.Length)
				{
					int num16 = groupConfig.Rewards[2];
					if (place < 2)
					{
						num16 += groupConfig.Rewards[1];
					}
					int num17 = _remoteConfigData.raceRewardMoneyLimit - moneyEarned;
					if (num16 > num17)
					{
						num16 = num17;
						num15 = 2;
						_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), num15).Forget();
					}
					_currencyManagerController.AddMoney(new AdvancedInt(num16));
					softCurrency += num16;
					moneyEarned += num16;
					string arg3 = new LocalizedString("HomesConfig.Money");
					ShowRewardNotification($"+{num16} {arg3}", num15).Forget();
					_playerProfileService.AddCircuitMoneyEarned(moneyEarned - moneyEarnedAtStart);
				}
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("Circuitraceresultwindow.BudgetLimit"), 2f).Forget();
			}
		}
		SaveResult(place);
		_isCalculating = false;
		OnRewardCalculated?.Invoke();
		HardCurrency = hardCurrency;
		SoftReward = softCurrency;
	}

	private async UniTask ShowRewardNotification(string moneyText, int waitTime = 0)
	{
		if (waitTime > 0)
		{
			await UniTask.Delay(waitTime);
		}
		if (!_isDestroyed)
		{
			string text = new LocalizedString("LevelScript.Congratulations");
			_managerUI.ShowNotificationWindow(text + " " + moneyText, 3f).Forget();
		}
	}

	private void OnLoose(List<LapRaceCar> winners)
	{
		SetWinnerPersons(winners);
	}

	private void Cancel()
	{
		_cancellationTokenSource?.Cancel();
	}

	public void RestartRace()
	{
		if ((bool)_circuitRacingWaitingWindow)
		{
			_circuitRacingWaitingWindow.gameObject.SetActive(value: true);
		}
		if ((bool)CircuitRacingUIControll.CircuitPlayWindow)
		{
			CircuitRacingUIControll.CircuitPlayWindow.RestartUI();
		}
		RaceState currentRaceState = CurrentRaceState;
		if (currentRaceState == RaceState.RaceStarted || currentRaceState == RaceState.RaceFinished || currentRaceState == RaceState.RaceStaged)
		{
			Cancel();
		}
		circuitCarsControl.DestroyTestCar();
		UnsubscribeMethods();
		CircuitRacingUIControll.SetProgress("0", "/0", "0", "/0");
		StartRace();
		if ((bool)_freeDriveProvider.QuickCarSettings)
		{
			_freeDriveProvider.QuickCarSettings.RepairCar();
		}
	}

	public void GoGarage()
	{
		_lapRaceTrafficLightUI.Reset();
		CurrentRaceState = RaceState.PodiumActive;
		Cancel();
		ShowPitStopTriggers(state: false);
		OnFinishRace();
		CircuitRacingUIControll.SetProgress("0", "/0", "0", "/0");
		UnsubscribeMethods();
		_managerUI.ShowCircuitRaceMenuWindow();
	}

	public void GoGarageWithoutFinish(bool showMenu = true)
	{
		Cancel();
		CircuitRacingUIControll.SetProgress("0", "/0", "0", "/0");
		UnsubscribeMethods();
		if (showMenu)
		{
			_managerUI.ShowCircuitRaceMenuWindow();
		}
	}

	public void GoMainGarage()
	{
		_lapRaceTrafficLightUI.SetActive(active: false);
		Cancel();
		ShowPitStopTriggers(state: false);
		UnsubscribeMethods();
		DelayedInit();
		if ((bool)_freeDriveProvider.QuickCarSettings)
		{
			_freeDriveProvider.QuickCarSettings.RepairCar();
		}
	}

	public void GoMainMenu()
	{
		DelayedGoMainMenu().Forget();
		if (CurrentRaceState == RaceState.GarageActive || CurrentRaceState == RaceState.RaceStaged)
		{
			return;
		}
		foreach (LapRaceCar raceCar in circuitCarsControl.GetRaceCars())
		{
			if (raceCar.IsUser())
			{
				_racingAnalytic.LogRaceFinish(raceCar, ModeDuration, "quit_manual");
			}
		}
	}

	private async UniTaskVoid DelayedGoMainMenu()
	{
		SaveTime();
		_isRaceFinished = true;
		PlayerDatas.UnloadResourcesForce();
		await UniTask.DelayFrame(3);
		_menuLevelParent.OpenMainMenu();
	}

	private void SaveTime()
	{
		RatingData ratingData = new RatingData();
		float num = (float)V.AnalyticWheres.multiFreeDrive + (float)V.AnalyticWheres.singleFree + V.AnalyticWheres.level + (float)V.AnalyticWheres.DragRacing + (float)V.AnalyticWheres.CircuitRacing;
		if (num > 1f)
		{
			ratingData.time = (int)num;
		}
		V.AnalyticWheres.Save();
		SendTaskInternal(ratingData).Forget();
	}

	private async UniTaskVoid SendTaskInternal(RatingData rating)
	{
		if (_accountController.Authorized)
		{
			string text = rating.ToJson();
			if (text != "}")
			{
				await _ratingController.SendRating(text);
				_dailyTasksManager.UpdateData(rating).Forget();
			}
		}
	}

	public void OnPlayerOutCar()
	{
		_controlItemsProvider.InputTouch.leftJoy.SetActive(value: true);
		_controlItemsProvider.InputTouch.rightJoy.SetActive(value: true);
		_cameraController.SetCameraForPersonWalking();
		_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, ourCar);
		V.DriverPerson.GetComponent<TouchMovePerson>().enabled = true;
	}

	public Dictionary<int, List<CircuitRacingRecord>> GetTrackRecords()
	{
		PlayerStorage data = _playerStorageService.Data;
		if (data.CircuitRacingProgress == null)
		{
			data.CircuitRacingProgress = new CircuitRacingProgress();
		}
		int num = _circuitRacingConfig.MapDatas.Length;
		Dictionary<int, List<CircuitRacingRecord>> dictionary = _playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords;
		if (dictionary == null || dictionary.Count == 0 || dictionary.Count < num)
		{
			dictionary = new Dictionary<int, List<CircuitRacingRecord>>();
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(i, GetRaceRecords());
			}
			if (dictionary.Count < num)
			{
				for (int j = dictionary.Count - 1; j < num; j++)
				{
					dictionary.Add(j, GetRaceRecords());
				}
			}
			_playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords = dictionary;
		}
		return dictionary;
	}

	private List<CircuitRacingRecord> GetRaceRecords()
	{
		List<CircuitRacingRecord> list = new List<CircuitRacingRecord>();
		foreach (VehicleClass value in Enum.GetValues(typeof(VehicleClass)))
		{
			CircuitRacingRecord item = new CircuitRacingRecord
			{
				ClassIndex = (int)value,
				FirstPlace = 0,
				SecondPlace = 0,
				ThirdPlace = 0,
				BestTime = 0f
			};
			list.Add(item);
		}
		return list;
	}

	private async UniTaskVoid TryDisqualificationAsync(DisqualificationType disqualificationType)
	{
		while (FlagsStorage.IsPhotoMode)
		{
			await UniTask.Yield();
		}
		CircuitRacingUIControll.HideLapRaceTrafficLight();
		UnsubscribeMethods();
		circuitCarsControl.GetUserLapRaceCar().FreezeCar(state: true);
		_managerUI.CloseCircuitPlayWindow();
		string localizationKey = "CircuitRace.DisqualificationMessage.Common";
		switch (disqualificationType)
		{
		case DisqualificationType.NoFuel:
			localizationKey = "CircuitRace.DisqualificationMessage.NoFuel";
			break;
		case DisqualificationType.CarBroken:
			localizationKey = "CircuitRace.DisqualificationMessage.CarBroken";
			break;
		}
		LemanRaceController.StopLemanCountdown();
		V.CoreGameManager.SetMessage(new LocalizedString(localizationKey), 3f, NotificationType.Error);
		await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: true);
		if (!_isDestroyed)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<LapRaceCar>(out var component))
			{
				component.CurrentDisqualificationType = disqualificationType;
			}
			RemoveUser();
		}
	}

	private void ModuleFinished()
	{
		for (int i = 0; i < _modules.Count; i++)
		{
			_modules[i].RaceFinished();
		}
	}

	public void ResetRace()
	{
		UnsubscribeMethods();
	}

	public bool IsLemanTimeExpired()
	{
		return LemanRaceController.IsTimeExpired();
	}

	public void SetInPitstop(bool _enable)
	{
		inPitStop = _enable;
	}

	public bool IsInPitstop()
	{
		return inPitStop;
	}

	private async UniTask InitDriverF1IfNeedsAsync()
	{
		CurrentClassId.Get(out int value);
		if (!V.DriverPerson || value != 6)
		{
			return;
		}
		UnityEngine.Object.Destroy(V.DriverPerson);
		V.DriverPerson = null;
		V.CoreGameManager.SetupCarDriver(V.Car);
		await UniTask.Yield();
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var mainPerson))
		{
			await UniTask.WaitUntil(() => _isDestroyed || !mainPerson || (bool)mainPerson.skin);
			if (!_isDestroyed && (bool)mainPerson)
			{
				mainPerson.TryUseHelmet();
				await UniTask.DelayFrame(10);
			}
		}
	}

	public void Resume()
	{
		if (CurrentRaceState == RaceState.GarageActive)
		{
			_managerUI.ShowCircuitRaceMenuWindow();
		}
		else if ((bool)CircuitRacingUIControll)
		{
			_managerUI.ShowWindow("CircuitRacePauseWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
		}
	}
}
