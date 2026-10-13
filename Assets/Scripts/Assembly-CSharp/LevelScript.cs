using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPM.PlayerData;
using CPMEngine;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drone;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;

public class LevelScript : MonoBehaviour
{
	private const int DefaultMessageIndex = 5;

	private const int InitialCamIndexDefault = 2;

	private const int levelRaceID400 = 57;

	private const int levelRaceID400_2 = 58;

	private const int levelRaceID1600 = 59;

	private const int levelRaceID1600_2 = 60;

	private const float CarPositionOfferFromParking = 0.68f;

	private const float CarMeasureLiftHeight = 400f;

	private const float CarMeasureLiftDelay = 0.5f;

	private const float ChangePosPeriod = 1f;

	private readonly HashSet<ObscuredInt> RaceLevelList = new HashSet<ObscuredInt> { 57, 58, 59, 60 };

	public LevelDatas[] levelDatas;

	public int LevelToLoadEditor;

	private int _carId;

	public bool WithTrailer;

	public Transform ParkingTriggersParent;

	public bool IsShowNewMessage;

	private Vector3 _carTargetPos;

	private Vector3 _carOffsetPos;

	private Vector3 _carTargetOffsetPosition;

	private bool _isInTargetPos;

	private CarCameras _carCameras;

	[HideInInspector]
	public CarCamerasController carCamerasController;

	[HideInInspector]
	public TargetControll targetControll;

	public Texture2D wheel;

	private int2 _uiGui;

	private CoreGameManager _coreGameManager;

	public GameObject ParkingBuilding;

	public GameObject ParkingBuilding1;

	public GameObject ParkingBuilding2;

	public GameObject Entrance1;

	public GameObject Entrance2;

	public GameObject OtherSceneBuildings;

	private string[] _messages = new string[7];

	private string _levelMessage;

	private ObscuredFloat _timeLeft;

	private ObscuredFloat _levelTime;

	public GameObject ParkHere3DText;

	private Vector3 _carTargetRot;

	private MenuControl _menuControl;

	public bool HasFixedCam;

	public Transform AngleMeasurePoint;

	[HideInInspector]
	public int prevMenuIndex;

	public int IntitialCamIndex = 2;

	public float differenceAngle;

	public float duration;

	private bool _changed;

	private Shader _shaderDef;

	public bool FLeft;

	public bool FRight;

	public bool RLeft;

	public bool RRight;

	public bool IsRestricted;

	public bool InParkingScene;

	public bool OutSideStart;

	public bool DriverOutSideBegin;

	public GameObject FL;

	public GameObject FR;

	public GameObject RL;

	public GameObject RR;

	public GameObject Arrow;

	public GameObject CamChangeButton;

	public bool DoNotChangeCubes;

	public bool ChangeCamToFixed;

	public GameObject ParkingLights;

	public int IndexTrailer;

	public CameraDatas[] cameraDatas;

	private int _startGear = 1;

	public int AttemptNumber;

	public bool EnableOtherSceneStart;

	public bool EnabelParkingSenceStart;

	public bool EnteredParkingCube;

	public int SceneForLoad;

	public int ShowPartsWhenStartPressed = 1;

	public int IndexToLoad;

	public int levelID;

	public float showLeftTime;

	public float currentLeftTime;

	public int showMoney;

	public LevelType LevelType;

	[SerializeField]
	private int[] LevelsFinishPositionsExeption = new int[3] { 63, 64, 35 };

	private bool _rewardByTime = true;

	private int _racingCarId = 1;

	private int _showMenuIndexInvoke;

	private bool _carLifterActive;

	private RaycastHit[] _tempHits = new RaycastHit[5];

	private DateTime _startTime;

	private bool _carPrepared;

	private bool _startRequested;

	private bool _carDamagedDuringLevel;

	public Vector3 localTrailerPos;

	[HideInInspector]
	public Vector3 localTrailerPos2;

	private int _hasLoad;

	private Vector3 _carToLoadPos = new Vector3(-33.08008f, 0.9157104f, -339.5579f);

	private int _carIdToLoad;

	public bool IsLevelPaused;

	private bool _alreadyPressedMenu;

	private PlayerDataStorage _playerDataStorage;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	private QuickCarSettings _quickCarSettings;

	private ICarInfoService _carInfoService;

	private ParkingMenuUI _parkingMenuUI;

	private IDisposable _changingRacingRotDisposable;

	private IDisposable _generateTrafficDisposable;

	private IDisposable _startChangingPosDisposable;

	private IDisposable _resetCarDamageBoolDisposable;

	private IDisposable _updateLeftTimeDisposable;

	private IDisposable _resumeDisposable;

	private IDisposable _openPauseMenuDisposable;

	private IDisposable _setVelocityAfterDisposable;

	private IDisposable _setLoadKinematicDisposable;

	private IDisposable _resetLoadAfterDisposable;

	private IDisposable _checkAgainStateDisposable;

	private IDisposable _hideReverseSingDisposable;

	private IDisposable _openSelMSModeDisposable;

	private IDisposable _openDefSteerWindowDisposable;

	private IDisposable _showOnlyTopLabelDispose;

	private IDisposable _hideArrowDispose;

	private IDisposable _menuSubscription;

	private IDisposable _enableButtonsSubscription;

	private CurrencyManagerController _currencyManagerController;

	private ManagerUI _managerUI;

	private LockCarButtonController _lockCarButtonController;

	private DroneProvider _droneProvider;

	private MenuLevelParent _menuLevelParent;

	private OptionalDependency<TrafficCarControl> _trafficCarControl;

	private GlobalCollectionsProvider _collectionsProvider;

	private InputItemsProvider _controlItemsProvider;

	private LevelSceneControl _levelSceneControl;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsConfig _levelsConfig;

	private ISlotService _slotService;

	private CarInputManager _carInputManager;

	private IPlayerStatusSystem _playerStatusSystem;

	private LevelsAnalytic _levelsAnalytic;

	private AllCarsInfo _allCarsInfo;

	private AssetBundleManager _assetBundleManager;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

	private ISteamAchievementService _steamAchievementService;

	private InputItemsProvider _inputItemsProvider;

	public string FailString { get; set; }

	public GameObject Birds { get; set; }

	public TrafficCarControl TrafficCarControl { get; set; }

	public GameObject FirstCarLevel { get; private set; }

	private GameObject LevelCar
	{
		get
		{
			if (!FirstCarLevel)
			{
				return V.Car;
			}
			return FirstCarLevel;
		}
	}

	public LoadControl LoadControl { get; set; }

	public LevelsProvider LevelsProvider { get; private set; }

	public Transform LastDroppedLoad { get; set; }

	public bool IsCarCameraSettingForLevel { get; private set; }

	public int MoneyWon { get; private set; }

	public float PlayDuration => (float)_levelTime - (float)_timeLeft;

	public float ModeDuration => (float)(DateTime.UtcNow - _startTime).TotalSeconds;

	public event Action OnLevelFailMenuOpen;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, MenuLevelParent menuLevelParent, CurrencyManagerController currencyManagerController, GlobalCollectionsProvider collectionsProvider, LockCarButtonController lockCarButtonController, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, LevelsConfig levelsConfig, LevelsProvider levelsProvider, ISlotService slotService, CarInputManager carInputManager, IPlayerStatusSystem playerStatusSystem, LevelsAnalytic levelsAnalytic, AssetBundleManager assetBundleManager, AllCarsInfo allCarsInfo, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, OptionalDependency<PlayerDatas.CarLoadingUtility> carLoadingUtility, OptionalDependency<QuickCarSettings> quickCarSettings, OptionalDependency<ICarInfoService> carInfoService, OptionalDependency<ParkingMenuUI> parkingMenuUI, OptionalDependency<ManagerUI> managerUI, OptionalDependency<DroneProvider> droneProvider, OptionalDependency<TrafficCarControl> trafficCarControl, InputItemsProvider inputItemsProvider, ISteamAchievementService steamAchievementService)
	{
		LevelsProvider = levelsProvider;
		_levelsConfig = levelsConfig;
		_freeDriveProvider = freeDriveProvider;
		_controlItemsProvider = controlItemsProvider;
		_collectionsProvider = collectionsProvider;
		_trafficCarControl = trafficCarControl;
		_menuLevelParent = menuLevelParent;
		_droneProvider = droneProvider.Value;
		_lockCarButtonController = lockCarButtonController;
		_managerUI = managerUI.Value;
		_currencyManagerController = currencyManagerController;
		_playerDataStorage = playerDataStorage;
		_parkingMenuUI = parkingMenuUI.Value;
		_carLoadingUtility = carLoadingUtility.Value;
		_quickCarSettings = quickCarSettings.Value;
		_carInfoService = carInfoService.Value;
		_slotService = slotService;
		_playerStatusSystem = playerStatusSystem;
		_levelsAnalytic = levelsAnalytic;
		_carInputManager = carInputManager;
		_assetBundleManager = assetBundleManager;
		_allCarsInfo = allCarsInfo;
		_inputItemsProvider = inputItemsProvider;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
		_steamAchievementService = steamAchievementService;
	}

	private void LoadLevelDataFromConfig()
	{
		if (LevelsProvider.CurrentLevelData == null)
		{
			return;
		}
		_carId = LevelsProvider.CurrentLevelData.CarId;
		LevelType = LevelsProvider.CurrentLevelData.Type;
		_hasLoad = LevelsProvider.CurrentLevelData.HasLoad;
		_racingCarId = LevelsProvider.CurrentLevelData.RacingCarId;
		_rewardByTime = LevelsProvider.CurrentLevelData.RewardByTime;
		_carTargetPos = (LevelsProvider.CurrentLevelData.SetCarTargetPos ? LevelsProvider.CurrentLevelData.CarTargetPos : (LevelsProvider.CurrentLevelData.ParkingTriggerParentPos + Vector3.down * 0.68f));
		_carTargetRot = (LevelsProvider.CurrentLevelData.SetCarTargetRot ? LevelsProvider.CurrentLevelData.CarTargetRot : (LevelsProvider.CurrentLevelData.ParkingTriggerParentRot + Vector3.up * 180f));
		_levelTime = LevelsProvider.CurrentLevelData.LevelTimeObscured;
		HasFixedCam = LevelsProvider.CurrentLevelData.HasFixedCam;
		_messages = LevelsProvider.CurrentLevelData.Messages;
		InParkingScene = LevelsProvider.CurrentLevelData.InParkingScene;
		DriverOutSideBegin = LevelsProvider.CurrentLevelData.DriverOutSideBegin;
		OutSideStart = LevelsProvider.CurrentLevelData.OutSideStart;
		ChangeCamToFixed = LevelsProvider.CurrentLevelData.ChangeCamToFixed;
		WithTrailer = LevelsProvider.CurrentLevelData.WithTrailer;
		IndexTrailer = LevelsProvider.CurrentLevelData.TrailerIndex;
		cameraDatas = LevelsProvider.CurrentLevelData.CameraData;
		_startGear = LevelsProvider.CurrentLevelData.StartGear;
		EnabelParkingSenceStart = LevelsProvider.CurrentLevelData.EnableParkingSceneStart;
		EnableOtherSceneStart = LevelsProvider.CurrentLevelData.EnableOtherSceneStart;
		_carToLoadPos = LevelsProvider.CurrentLevelData.TowCarPos;
		_carIdToLoad = LevelsProvider.CurrentLevelData.TowCarId;
		IntitialCamIndex = ((LevelsProvider.CurrentLevelData.InitialCamIndex != -1) ? LevelsProvider.CurrentLevelData.InitialCamIndex : 2);
		IsCarCameraSettingForLevel = LevelsProvider.CurrentLevelData.IsCarCameraSettingForLevel;
		if (LevelsProvider.CurrentLevelData.ParkingToUse != 0 && (bool)ParkingBuilding1 && (bool)ParkingBuilding2)
		{
			if (LevelsProvider.CurrentLevelData.ParkingToUse == 1)
			{
				ParkingBuilding = ParkingBuilding1;
				ParkingBuilding2.SetActive(value: false);
				Entrance1.SetActive(value: true);
				Entrance2.SetActive(value: false);
			}
			else
			{
				ParkingBuilding = ParkingBuilding2;
				ParkingBuilding1.SetActive(value: false);
				Entrance1.SetActive(value: false);
				Entrance2.SetActive(value: true);
			}
		}
		ShowPartsWhenStartPressed = LevelsProvider.CurrentLevelData.ShowPartsWhenStartPressed;
	}

	private void SetDataForLevel()
	{
		_uiGui = Utils.NotValid;
		LoadLevelDataFromConfig();
		LevelDatas[] array = levelDatas;
		if (array != null && array.Length > 0)
		{
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				for (int i = 0; i < levelDatas.Length; i++)
				{
					if (levelDatas[i] != null)
					{
						if (levelDatas[i].ObjectsForDisable != null)
						{
							levelDatas[i].EnableDisableObjects(enable: true, levelDatas[i].ObjectsForDisable);
						}
						if (levelDatas[i].ObjectsForEnable != null)
						{
							levelDatas[i].EnableDisableObjects(enable: false, levelDatas[i].ObjectsForEnable);
						}
					}
				}
				if (LevelsProvider.CurrentLevelData == null)
				{
					LevelsProvider.CurrentLevelData = ScriptableObject.CreateInstance<LevelDataScript>();
					LevelsProvider.CurrentLevelData.IndexInScene = IndexToLoad;
					LevelsProvider.CurrentLevelData.SceneForLoad = SceneForLoad;
					LevelsProvider.CurrentLevelData.ID = levelID;
				}
				if (Application.isEditor)
				{
					MonoBehaviour.print($"{LevelsProvider.CurrentLevelData.ID} id");
					MonoBehaviour.print($"{LevelsProvider.CurrentLevelData.SceneForLoad} SceneForLoad");
				}
				int num = (LevelsProvider.CurrentLevelData ? LevelsProvider.CurrentLevelData.IndexInScene : 0);
				if (num < levelDatas.Length)
				{
					levelDatas[num].LoadLevelDataForLevel(this);
					levelDatas[num].ForLevelParticularMethods();
				}
				if ((bool)ParkingTriggersParent)
				{
					ParkingTriggersParent.SetPositionAndRotation(LevelsProvider.CurrentLevelData.ParkingTriggerParentPos, Quaternion.Euler(LevelsProvider.CurrentLevelData.ParkingTriggerParentRot));
				}
			}
			if (EnableOtherSceneStart && (bool)OtherSceneBuildings)
			{
				OtherSceneBuildings.SetActive(value: true);
			}
			if (EnabelParkingSenceStart && (bool)ParkingBuilding)
			{
				ParkingBuilding.SetActive(value: true);
			}
			LevelDataScript currentLevelData = LevelsProvider.CurrentLevelData;
			currentLevelData.RelativeTrailerAngle = LevelsProvider.CurrentLevelData.ID switch
			{
				23 => 82.11111f, 
				24 => 262f, 
				_ => LevelsProvider.CurrentLevelData.RelativeTrailerAngle, 
			};
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				if ((bool)Birds)
				{
					Birds.transform.position = ParkingTriggersParent.position + Vector3.up * 5f;
				}
			}).AddTo(this);
		}
		_carLifterActive = false;
		FlagsStorage.RacingGameMode = false;
		localTrailerPos2 = localTrailerPos;
		if (LevelsProvider.CurrentLevelData == null || ScenesHelper.IsLapRacingSceneActive() || ScenesHelper.IsDragRacingSceneActive() || ScenesHelper.IsFreeDriveSceneActive())
		{
			return;
		}
		switch (LevelsProvider.CurrentLevelData.SceneForLoad)
		{
		case 11:
			_levelSceneControl.LoadDesert();
			break;
		case 12:
		{
			int iD = LevelsProvider.CurrentLevelData.ID;
			bool flag = iD == 65 || iD == 66 || iD == 67 || iD == 68;
			if (V.SceneControl != null)
			{
				V.SceneControl.MakeDayScene11(!flag);
			}
			break;
		}
		case 20:
			_levelSceneControl.LoadMainCity();
			break;
		case 21:
			_levelSceneControl.LoadMountain();
			break;
		case 22:
			_levelSceneControl.LoadRoad2();
			break;
		}
		TimeProvider.skyboxSelectedRuntime = 0;
		V.OnSkyboxChanged?.Invoke();
		GlobalEventSystem.RequestTimeController()?.SnapSunToTime();
		_generateTrafficDisposable?.Dispose();
		_generateTrafficDisposable = Observable.NextFrame().Subscribe((Unit _) =>
		{
			Traffic();
		}).AddTo(this);
	}

	private void Traffic()
	{
		_trafficCarControl.Value.DestroyAllTrafficCar();
		if ((bool)LevelsProvider.CurrentLevelData)
		{
			LevelType = LevelsProvider.CurrentLevelData.Type;
		}
		switch (LevelType)
		{
		case LevelType.Chasing:
			if (!_trafficCarControl.Value.CarToFollow)
			{
				_trafficCarControl.Value.CreateChasingCar(LevelsProvider.CurrentLevelData.RacingCarId);
			}
			break;
		case LevelType.Parking:
			_trafficCarControl.Value.CreateLevelTraffic();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case LevelType.Sprint:
		case LevelType.DragRace:
		case LevelType.CircuitRacing:
			break;
		}
	}

	public void RestartScene()
	{
		_managerUI.ShowWaitingWindowDark(set: true).Forget();
		if ((bool)LoadControl)
		{
			LoadControl.ResetParentBack();
			LoadControl.DisableLoad();
		}
		if ((bool)V.Car)
		{
			UnityEngine.Object.Destroy(V.Car);
		}
		if ((bool)LevelsProvider.RacingCarLevel)
		{
			if (LevelsProvider.RacingCarLevel.TryGetComponent<VehicleData>(out var component) && (bool)component.Driver)
			{
				UnityEngine.Object.Destroy(component.Driver);
			}
			UnityEngine.Object.Destroy(LevelsProvider.RacingCarLevel);
			LevelsProvider.RacingCarLevel = null;
		}
		if ((bool)FirstCarLevel)
		{
			UnityEngine.Object.Destroy(FirstCarLevel);
		}
		UnityEngine.Object.Destroy(V.DriverPerson);
		V.DriverPerson = null;
		if ((bool)LevelsProvider.Trailer)
		{
			UnityEngine.Object.Destroy(LevelsProvider.Trailer);
		}
		V.Car = null;
		PostAwake();
	}

	public void Init()
	{
		if (ScenesHelper.IsFreeDriveSceneActive())
		{
			_startTime = DateTime.UtcNow;
		}
		_menuLevelParent.isDroneEnabled = false;
		localTrailerPos.y = 0.4f;
		InitAwake();
		if (TryGetComponent<Button>(out var component))
		{
			component.onClick.AddListener(OnActionButtonPressed);
		}
		if (ScenesHelper.IsParkingLevelActive())
		{
			LocalizationManager.LocalizationChanged += LocalizeMessage;
		}
		_playerStatusSystem.OnGameModePlayed();
		_changeCameraHandle.Get(_eventRegistry).Subscribe(ChangeCam);
		Audio.ResetAllVolume();
	}

	private void OnDestroy()
	{
		_tempHits = null;
		if (LevelsProvider.LevelScript == this)
		{
			LevelsProvider.LevelScript = null;
		}
		if (ScenesHelper.IsParkingLevelActive())
		{
			LocalizationManager.LocalizationChanged -= LocalizeMessage;
		}
		if (TryGetComponent<Button>(out var component))
		{
			component.onClick.RemoveListener(OnActionButtonPressed);
		}
		_carInputManager.IgnoreSteerControl(state: false);
		_changeCameraHandle.Get(_eventRegistry).Unsubscribe(ChangeCam);
	}

	private void InitAwake()
	{
		TryGetComponent<CarCameras>(out _carCameras);
		TryGetComponent<CarCamerasController>(out carCamerasController);
		TryGetComponent<CoreGameManager>(out _coreGameManager);
		TryGetComponent<TargetControll>(out targetControll);
		TryGetComponent<LevelSceneControl>(out _levelSceneControl);
		LevelsProvider.LevelScript = this;
		PostAwake();
	}

	private void PostAwake()
	{
		if (ScenesHelper.IsParkingLevelActive())
		{
			FlagsStorage.OutOfCar = false;
		}
		duration = 0f;
		SetDataForLevel();
		FlagsStorage.LevelStarted = false;
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		if (buildIndex != 41 && buildIndex != 13)
		{
			FlagsStorage.IsMultiFree = false;
			PlayAlone();
			StopChangingPos();
			_startRequested = false;
			PrepareCar().Forget();
			_ = LoadTrailer();
			OpenFirstMenu();
			V.CurrentCamIndex = IntitialCamIndex;
			FlagsStorage.CamFollowCar = false;
			_carCameras.FollowPosRot = false;
		}
	}

	public void PlayAlone()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private void SetVelocityAfter()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component))
		{
			component.ResetTransmission();
		}
		_carInputManager.ClearAllInputs();
	}

	private void UpdateBestLabels()
	{
	}

	public void RestartLevelMultiParking()
	{
	}

	private async UniTaskVoid PrepareCar()
	{
		_carPrepared = false;
		if (!V.Car)
		{
			LevelType levelType = LevelType;
			int num = ((levelType == LevelType.DragRace || levelType == LevelType.Chasing) ? ((int)V.SlotID) : _carInfoService.GetFirstSlotIdByCarId(_carId));
			levelType = LevelType;
			if (levelType == LevelType.DragRace || levelType == LevelType.Chasing)
			{
				CarSlot slot = _slotService.GetSlot(num);
				if (slot == null || slot.IsEmpty)
				{
					num = _slotService.GetNotEmptySlotId();
					V.SlotID = num;
				}
			}
			bool loadDefault = false;
			if (num != -1)
			{
				V.Car = (await _carLoadingUtility.LoadCarFromSlot(num, LevelsProvider.CurrentLevelData.CarInitialPos, Quaternion.Euler(Vector3.zero), tryUnloadCurrent: false)).Item2;
				if (V.Car.TryGetComponent<VehicleEntityProvider>(out var component) && component.VehicleEngineResult.IsBroken)
				{
					if (V.Car.TryGetComponent<VehicleData>(out var cd))
					{
						await UniTask.WaitUntil(() => cd.IsCarGraphicLoaded && cd.IsKitsLoaded).TimeoutWithoutException(TimeSpan.FromSeconds(10.0));
					}
					_carLoadingUtility.UnloadCar(V.Car);
					loadDefault = true;
				}
			}
			else
			{
				loadDefault = true;
			}
			if (loadDefault)
			{
				if ((bool)LevelsProvider.CurrentLevelData.CarConfig)
				{
					_carId = LevelsProvider.CurrentLevelData.CarConfig.CarID;
				}
				V.Car = (await _carLoadingUtility.LoadDefaultCar(_carId, LevelsProvider.CurrentLevelData.CarInitialPos, Quaternion.Euler(Vector3.zero), tryUnloadCurrent: false)).Item2;
				if ((bool)LevelsProvider.CurrentLevelData.CarConfig)
				{
					_carLoadingUtility.LoadCarConfig(V.Car, LevelsProvider.CurrentLevelData.CarConfig);
				}
			}
			FirstCarLevel = V.Car;
			V.CarId = _carId;
			FirstCarLevel.transform.eulerAngles = LevelsProvider.CurrentLevelData.CarInitRot;
			if (FirstCarLevel.TryGetComponent<VehicleEntityProvider>(out var component2))
			{
				component2.IsLvlCar = true;
			}
		}
		if (!FirstCarLevel)
		{
			FirstCarLevel = V.Car;
		}
		_coreGameManager.SetupCarDriver(V.Car);
		if (V.Car.TryGetComponent<VehicleData>(out var vehicleData))
		{
			Rigidbody carBody = V.Car.GetComponent<Rigidbody>();
			carBody.isKinematic = true;
			V.Car.transform.position += Vector3.up * 400f;
			await UniTask.Delay(TimeSpan.FromSeconds(0.5));
			_carOffsetPos = (vehicleData.GroundOffset - 0.1f) * Vector3.up;
			ToInitialPos();
			carBody.isKinematic = false;
			vehicleData.BodyLoadingComposition.ShowMainModelOnly();
			SetupInteriorForLevelCar(vehicleData).Forget();
		}
		if (LevelsProvider.CurrentLevelData.Type == LevelType.Parking && !_coreGameManager.isFreeDrive)
		{
			DeactivateLodOnLoaded(vehicleData);
			if (!FlagsStorage.LevelStarted && !_startRequested)
			{
				StopChangingPos();
				_startChangingPosDisposable = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					StartChangingPos();
				}).AddTo(this);
			}
			if (_hasLoad > 0)
			{
				_setLoadKinematicDisposable?.Dispose();
				_setLoadKinematicDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					SetLoadKinematic();
				}).AddTo(this);
			}
		}
		else
		{
			ToInitialPos();
		}
		if ((bool)_carInputManager.UGUISteeringWheel)
		{
			_carInputManager.UGUISteeringWheel.enabled = false;
		}
		FlagsStorage.OutOfCar = false;
		FirstCarLevel.TryGetComponent<VehicleData>(out var component3);
		CarTriggers carTriggers = V.Car.AddComponent<CarTriggers>();
		carTriggers.LevelScript = this;
		carTriggers.FreeDriveProvider = _freeDriveProvider;
		carTriggers.LevelsAnalytic = _levelsAnalytic;
		carTriggers.LevelsProvider = LevelsProvider;
		PlayerDatas.CreateSkid(V.Car);
		if (FirstCarLevel.TryGetComponent<Powertrain>(out var dr))
		{
			dr.IsAutomaticGearbox = true;
			dr.Automatic = true;
			dr.StartEngine(starterSound: false);
		}
		if (LevelType == LevelType.DragRace)
		{
			V.CurrentEnvironment = V.placeToSpawn.MainCity;
			LevelsProvider.RacingCarLevel = _carLoadingUtility.LoadRacingCarLevel(_racingCarId, LevelsProvider.CurrentLevelData.RacingCarInitialPos, targetControll.targets1V3[1], 110);
			PlayerDatas.CreateSkid(LevelsProvider.RacingCarLevel);
			component3.KeepRowComposition.RacingTargets = targetControll.targets1V3[0];
			LevelsProvider.RacingCarLevel.transform.SetPositionAndRotation(LevelsProvider.CurrentLevelData.RacingCarInitialPos, Quaternion.Euler(LevelsProvider.CurrentLevelData.RacingCarInitialRot));
			FlagsStorage.RacingGameMode = true;
			_changingRacingRotDisposable?.Dispose();
			_changingRacingRotDisposable = Observable.Interval(TimeSpan.FromSeconds(0.15000000596046448)).Subscribe((long _) =>
			{
				if ((bool)LevelsProvider.RacingCarLevel)
				{
					Vector3 eulerAngles = LevelsProvider.RacingCarLevel.transform.eulerAngles;
					eulerAngles.y = 180f;
					LevelsProvider.RacingCarLevel.transform.rotation = Quaternion.Euler(eulerAngles);
				}
			}).AddTo(this);
			StopChangingPos();
		}
		if (LevelsProvider.CurrentLevelData.TowCarLevel)
		{
			Vector3 localPosition = component3.GetPowertrain.TrailerJoint.localPosition;
			(CarInfo, GameObject) tuple = await _carLoadingUtility.LoadDefaultCar(_carIdToLoad, localPosition, Quaternion.identity, tryUnloadCurrent: false);
			tuple.Item2.GetComponent<MultiPlayerPosControl>().enabled = false;
			_levelSceneControl.CarTowLoad = tuple.Item2;
			PlayerDatas.TowCar(FirstCarLevel, _levelSceneControl.CarTowLoad);
			await UniTask.WaitForFixedUpdate();
		}
		if (LevelsProvider.CurrentLevelData.ID == 56)
		{
			dr.ShutdownEngine();
		}
		_carPrepared = true;
	}

	private void DeactivateLodOnLoaded(VehicleData vehicleData)
	{
		if (V.Car.TryGetComponent<CarLODControl>(out var component))
		{
			CarLODControl carLODControl = component;
			bool flag = (component.IsActiveOnLoaded = false);
			carLODControl.enabled = flag;
			vehicleData?.BodyLoadingComposition.ShowMainModelOnly();
		}
	}

	private async UniTaskVoid LoadTrailer()
	{
		if (!LevelsProvider.CurrentLevelData.LoadTrailer)
		{
			return;
		}
		await UniTask.WaitUntil(() => V.Car, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		GameObject car = V.Car;
		GameObject gameObject = await PlayerDatas.LoadTrailerFromResource(car.transform.position + car.transform.forward * -2f, car.transform.eulerAngles, IndexTrailer);
		LevelsProvider.Trailer = gameObject;
		if ((bool)car)
		{
			if ((bool)gameObject && car.TryGetComponent<CarChildsReferences>(out var component))
			{
				component.Trailer = gameObject.transform;
			}
			LevelsProvider.Trailer.transform.parent = car.transform;
			if (localTrailerPos == Vector3.zero)
			{
				localTrailerPos2 = LevelsProvider.Trailer.transform.localPosition;
			}
			if (LevelsProvider.Trailer.TryGetComponent<TrailerEntity>(out var component2))
			{
				Vector3 trailerSpawnLocalPosition = GetTrailerSpawnLocalPosition(car, LevelsProvider.Trailer);
				LevelsProvider.Trailer.transform.localPosition = trailerSpawnLocalPosition;
				localTrailerPos = trailerSpawnLocalPosition;
				localTrailerPos2 = trailerSpawnLocalPosition;
				component2.Install(V.Car.GetComponent<Powertrain>());
			}
		}
	}

	private void DisableBrokenCarCollider(GameObject car)
	{
		if (car.TryGetComponent<VehicleData>(out var component))
		{
			component.Collider.enabled = false;
		}
	}

	private async UniTask SetupInteriorForLevelCar(VehicleData vehicleData)
	{
		if (!vehicleData)
		{
			return;
		}
		if (IsLowGraphics() && V.CurrentCamIndex != 0)
		{
			await UniTask.WaitUntil(() => !IsLowGraphics() || V.CurrentCamIndex == 0, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			if (!vehicleData)
			{
				return;
			}
		}
		await vehicleData.InteriorsComposition.ForceSetSavedInterior(this.GetCancellationTokenOnDestroy());
	}

	private bool IsLowGraphics()
	{
		if ((bool)CPMGraphicsSettingsManager.Instance)
		{
			return CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low;
		}
		return false;
	}

	public void CleanScreen(bool canMute = true)
	{
		GlobalEventSystem.OnCleanUI?.Invoke(obj: true);
		if (_menuLevelParent.isDroneEnabled)
		{
			_droneProvider?.ShowDroneUI(state: false);
		}
		else if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Hide();
		}
		if (!FlagsStorage.IsMultiFree & canMute)
		{
			Audio.MuteExcludeUI();
		}
		Time.timeScale = ((SceneManager.GetActiveScene().buildIndex == 13 && FlagsStorage.IsMultiFree) ? 1 : 0);
	}

	public void CleanForPassangerView()
	{
		CleanForPersonView();
		SpeedometerUI speedometerUI = _inputItemsProvider.SpeedometerUI;
		if ((bool)speedometerUI)
		{
			speedometerUI.ShowDash();
		}
		if ((bool)_controlItemsProvider.InputTouch)
		{
			_controlItemsProvider.InputTouch.leftJoy.SetActive(value: false);
			_controlItemsProvider.InputTouch.rightJoy.SetActive(value: false);
		}
	}

	public void CleanForPersonView(bool isEnableTouch = true)
	{
		if (_controlItemsProvider.ShifterControl != null)
		{
			_controlItemsProvider.ShifterControl.HideObjects();
		}
		if ((bool)_freeDriveProvider.FreeDriveControl && _controlItemsProvider.ShifterControl != null)
		{
			_controlItemsProvider.ShifterControl.SetActiveObjects(state: false);
		}
		if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.CleanForPersonView(isOut: true);
			_parkingMenuUI.CloseCameraSettingsWindowWithButton();
		}
		if (((bool)_controlItemsProvider.InputTouch && (bool)_controlItemsProvider.InputTouch.leftJoy) & isEnableTouch)
		{
			_controlItemsProvider.InputTouch.leftJoy.SetActive(value: true);
			_controlItemsProvider.InputTouch.rightJoy.SetActive(value: true);
		}
		if ((bool)_menuLevelParent.IndicatorsParent)
		{
			_menuLevelParent.IndicatorsParent.SetActive(value: false);
		}
	}

	public void UnCleanScreen(bool saveGearState = false, bool closeSeveralWindows = true)
	{
		GlobalEventSystem.OnCleanUI?.Invoke(obj: false);
		SpeedometerUI speedometerUI = _inputItemsProvider.SpeedometerUI;
		if ((bool)speedometerUI)
		{
			speedometerUI.ShowDash();
		}
		if (_menuLevelParent.isDroneEnabled)
		{
			_droneProvider?.ShowDroneUI(state: true);
		}
		else if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Show(showPartiallyChat: true, closeSeveralWindows);
			_parkingMenuUI.CleanForPersonView(isOut: false);
		}
		Audio.ResetAllVolume();
		Time.timeScale = 1f;
		if (_controlItemsProvider.ShifterControl != null && !saveGearState)
		{
			_controlItemsProvider.ShifterControl.ShowObjects(fromControlUi: false);
		}
		if (FlagsStorage.OutOfCar)
		{
			CleanForPersonView();
		}
		else if (FlagsStorage.IsPassenger)
		{
			CleanForPassangerView();
		}
	}

	public void LockPosition(Rigidbody rb, bool locked)
	{
		if ((bool)rb)
		{
			rb.constraints = (locked ? ((RigidbodyConstraints)10) : RigidbodyConstraints.None);
			rb.freezeRotation = locked;
			if (!locked)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}
	}

	public async UniTask StartLevel()
	{
		_startRequested = true;
		StopChangingPos();
		await UniTask.WaitUntil(() => _carPrepared);
		AttemptNumber++;
		_uiGui = Utils.NotValid;
		await UniTask.WaitUntil(() => FirstCarLevel);
		SetVelocityAfter();
		await UniTask.Yield();
		if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Show();
			await UniTask.Yield();
			_parkingMenuUI.ShowAnimationToggle(DriverOutSideBegin);
			_parkingMenuUI.ShowWalkElements(DriverOutSideBegin);
		}
		if ((DriverOutSideBegin && !V.DriverPerson) || !FirstCarLevel)
		{
			return;
		}
		if (!DriverOutSideBegin && FlagsStorage.OutOfCar)
		{
			await _coreGameManager.CarOutInControl.InCar(V.DriverPerson, FirstCarLevel);
		}
		if (!V.Car)
		{
			V.Car = FirstCarLevel;
			if (!V.Car)
			{
				return;
			}
		}
		await StartLevel2();
		if (!V.Car)
		{
			return;
		}
		if (V.Car.TryGetComponent<CarLODControl>(out var component))
		{
			component.enabled = true;
		}
		if (V.Car.TryGetComponent<VehicleData>(out var component2))
		{
			component2.CarHealthComposition.RepairVisual();
		}
		if (LevelType == LevelType.CircuitRacing)
		{
			FlagsStorage.EndlessCrashTime = true;
			_managerUI.ShowNotificationWindow("3...", 0.7f).Forget();
			await UniTask.Delay(1000);
			_managerUI.ShowNotificationWindow("2...", 0.7f).Forget();
			await UniTask.Delay(1000);
			_managerUI.ShowNotificationWindow("1...", 0.7f).Forget();
			await UniTask.Delay(1000);
			_managerUI.ShowNotificationWindow(new LocalizedString("LevelScript.Go"), 0.7f).Forget();
			FlagsStorage.EndlessCrashTime = false;
		}
		if (LevelType == LevelType.DragRace)
		{
			_changingRacingRotDisposable?.Dispose();
			RacingLevelSet();
		}
		else
		{
			IObservable<long> observable = Observable.Timer(TimeSpan.FromSeconds(1.0));
			if (LevelsProvider.CurrentLevelData.LoadTrailer)
			{
				observable.Subscribe((long _) =>
				{
					Rigidbody rigidbody = (_carInputManager ? _carInputManager.GetRigidbody : null);
					if ((bool)rigidbody)
					{
						rigidbody.isKinematic = false;
					}
				}).AddTo(V.Car);
			}
			Observable.Interval(TimeSpan.FromSeconds(0.05000000074505806)).TakeUntil(observable).Subscribe((long _) =>
			{
				Powertrain powertrain = (_carInputManager ? _carInputManager.GetPowertrain : null);
				if ((bool)powertrain)
				{
					powertrain.Handbrake = 1f;
				}
			})
				.AddTo(V.Car);
			if ((bool)LoadControl)
			{
				if (_hasLoad != 0)
				{
					LoadControl.ResetPosition(_hasLoad - 1, V.Car, makeKinematic: false);
					int iD = LevelsProvider.CurrentLevelData.ID;
					if (iD == 81 || iD == 84 || iD == 86)
					{
						LoadControl.ResetPosition(_hasLoad, LevelsProvider.Trailer, makeKinematic: false);
					}
				}
				else
				{
					LoadControl.DisableLoad();
				}
			}
			_carInputManager.IgnoreSteerControl(state: false);
		}
		_carLifterActive = true;
		if (LevelType != LevelType.CircuitRacing && LevelType != LevelType.DragRace)
		{
			_startTime = DateTime.UtcNow;
		}
	}

	private static void CancelPendingSeatGetOutAnimations()
	{
		if (!V.DriverPerson || !V.DriverPerson.TryGetComponentInChildren<PersonAnimationsController>(out var component))
		{
			return;
		}
		SeatGetOutCarSystem sitGetOutCarSystem = component.SitGetOutCarSystem;
		if (sitGetOutCarSystem?.SitGetOutData != null)
		{
			if (sitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
			{
				sitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			}
			else if (sitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate)
			{
				sitGetOutCarSystem.CarAnimationStopper.CancelGetOut();
			}
		}
	}

	private void RacingLevelSet()
	{
		FlagsStorage.RacingGameMode = true;
		_uiGui = Utils.NotValid;
		if (V.Car.TryGetComponent<VehicleData>(out var component))
		{
			component.KeepRowComposition.RacingTargets = targetControll.targets1V3[0];
			component.KeepRowComposition.CurrentTargetIndex = 85;
			component.SetupAsRacingCar();
		}
		_carInputManager.State = 2;
		_carInputManager.IgnoreSteerControl(state: true);
		_carInputManager.Ready = false;
		V.CoreGameManager.SetSteering();
		if ((bool)LevelsProvider.RacingCarLevel)
		{
			if (LevelsProvider.RacingCarLevel.TryGetComponent<Powertrain>(out var component2))
			{
				component2.IsAutomaticGearbox = true;
				component2.ResetTransmission(resetGear: false);
				component2.StartEngine(starterSound: false);
			}
			if (LevelsProvider.RacingCarLevel.TryGetComponent<CarAIController>(out var component3))
			{
				component3.TargetSpeed = 5f;
				component3.StopCar = false;
				component3.KeepHighRpm = false;
				component3.SteerAssistent = false;
			}
			LevelsProvider.RacingCarLevel.transform.SetPositionAndRotation(LevelsProvider.CurrentLevelData.RacingCarInitialPos, Quaternion.Euler(LevelsProvider.CurrentLevelData.RacingCarInitialRot));
			if (LevelsProvider.RacingCarLevel.TryGetComponent<LightController>(out var component4))
			{
				component4.EmergencyLightsOn();
			}
		}
		ShowMessage("LevelScript.MoveSlowlyToTheStartLine", isImportant: true);
		FlagsStorage.RaceStarted = false;
		FlagsStorage.Car2Finished = false;
		if ((bool)LevelsProvider.TreeControl)
		{
			LevelsProvider.TreeControl.ResetLights();
		}
		FlagsStorage.IsReadyMultRaceCar2 = false;
		_freeDriveProvider.QuickCarSettings.HideDoor();
	}

	public void OnRaceStarted()
	{
		ShowMessage("LevelScript.Go", isImportant: true);
		if ((bool)FirstCarLevel && FirstCarLevel.TryGetComponent<Powertrain>(out var component))
		{
			component.FixVehicle(stop: false);
		}
		_carInputManager.TargetSpeed = 100000f;
		_carInputManager.State = 3;
		_carInputManager.Ready = false;
		if ((bool)LevelsProvider.RacingCarLevel && LevelsProvider.RacingCarLevel.TryGetComponent<CarAIController>(out var component2))
		{
			component2.TargetSpeed = 240f;
			component2.StopCar = false;
			component2.KeepHighRpm = false;
			component2.SteerAssistent = false;
		}
		FlagsStorage.RaceStarted = true;
	}

	private void EnableSceneAndParking()
	{
		if ((bool)ParkingBuilding && (bool)OtherSceneBuildings)
		{
			switch (ShowPartsWhenStartPressed)
			{
			case 1:
				OtherSceneBuildings.SetActive(value: true);
				ParkingBuilding.SetActive(value: false);
				break;
			case 2:
				OtherSceneBuildings.SetActive(value: false);
				ParkingBuilding.SetActive(value: true);
				break;
			}
		}
		if ((bool)OtherSceneBuildings && ShowPartsWhenStartPressed == 0)
		{
			OtherSceneBuildings.SetActive(value: true);
		}
		_ = (bool)ParkingBuilding;
	}

	private async UniTask StartLevel2()
	{
		StopChangingPos();
		CancelPendingSeatGetOutAnimations();
		HideArrow();
		if (FlagsStorage.OutOfCar || V.Car == LevelsProvider.RacingCarLevel)
		{
			if (LevelType == LevelType.DragRace)
			{
				FlagsStorage.IsPassenger = false;
			}
			_coreGameManager.CarOutInControl.InCar(V.DriverPerson, FirstCarLevel).Forget();
		}
		if ((bool)CamChangeButton)
		{
			CamChangeButton.SetActive(!LevelsProvider.CurrentLevelData.HasFixedCam);
		}
		SetActiveFalseEachMenu(1);
		EnableSceneAndParking();
		FlagsStorage.CamFollowCar = true;
		FlagsStorage.MoveByTouch = false;
		FlagsStorage.UpdatedToReverse = false;
		FlagsStorage.LoadCollidedInvoked = false;
		LastDroppedLoad = null;
		if (LevelsProvider.CurrentLevelData.Type != LevelType.Chasing)
		{
			_menuLevelParent.LabelTop.text = new LocalizedString(_messages[5]);
			_levelMessage = _messages[5];
		}
		else
		{
			_menuLevelParent.LabelTop.SetText(new LocalizedString(_messages[0]));
			_levelMessage = _messages[0];
			if ((bool)_menuLevelParent.RewardedVideWatchButton)
			{
				_menuLevelParent.RewardedVideWatchButton.SetActive(value: false);
			}
		}
		if (V.CurrentCamIndex == 3 || V.CurrentCamIndex == 4)
		{
			carCamerasController.SetCamera(1, V.Car.transform, newTarget: true);
			V.CurrentCamIndex = 1;
		}
		else
		{
			carCamerasController.SetCamera(V.CurrentCamIndex, V.Car.transform, newTarget: true);
		}
		if (V.CurrentCamIndex == 5)
		{
			carCamerasController.CamStaticMode(on: true);
		}
		SetCarForLevel();
		_menuLevelParent.LabelTopParent.SetActive(value: true);
		TryHideTopLabel(LevelsProvider, _menuLevelParent);
		_menuLevelParent.LabelMiddleParent.SetActive(value: false);
		_timeLeft = _levelTime;
		_updateLeftTimeDisposable?.Dispose();
		_updateLeftTimeDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Repeat().Subscribe((long _) =>
		{
			UpdateLeftTime();
		})
			.AddTo(this);
		if (IsRestricted)
		{
			ShowMessage(_messages[6], isImportant: true);
			IsRestricted = false;
		}
		FlagsStorage.UpdatedToReverse = false;
		RLeft = false;
		RRight = false;
		FRight = false;
		FLeft = false;
		FlagsStorage.IsCarDamaged = false;
		_carDamagedDuringLevel = false;
		_carInputManager.ClearAllInputs();
		if ((bool)_controlItemsProvider.ThrottleSliderControl)
		{
			_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
		}
		if ((bool)_controlItemsProvider.BrakeSliderControl)
		{
			_controlItemsProvider.BrakeSliderControl.OnBrakeReleased();
		}
		if ((bool)LevelsProvider.Trailer)
		{
			SetTrailer();
		}
		if ((bool)_levelSceneControl && (bool)_levelSceneControl.CarTowLoad)
		{
			_levelSceneControl.CarTowLoad.transform.parent = null;
		}
		if (LevelsProvider.CurrentLevelData.ID == 88)
		{
			V.Car.GetComponent<Powertrain>().SetCheckEngine(value: true, sendRpc: false);
		}
		if (LevelsProvider.CurrentLevelData.Type == LevelType.Chasing)
		{
			_trafficCarControl.Value.CarToFollow.transform.SetPositionAndRotation(_trafficCarControl.Value.CarToFollowPos, new Quaternion(0f, 180f, 0f, 0f));
			if (_trafficCarControl.Value.CarToFollow.TryGetComponent<Powertrain>(out var component))
			{
				component.StartEngine(starterSound: false);
				component.Velocity = Vector3.zero;
				component.AngularVelocity = Vector3.zero;
			}
			if (_trafficCarControl.Value.CarToFollow.TryGetComponent<CarAIController>(out var component2))
			{
				component2.targets = targetControll.targets1V3[0];
				component2.CurrentTarIndex = 4;
				component2.CurrentWay = 0;
				component2.StopCar = false;
			}
			_freeDriveProvider.QuickCarSettings.HideDoor();
		}
		_timeLeft = _levelTime;
		UnCleanScreen();
		SetParkingTriggerPos(V.Car);
		bool num = PlayerPrefs.GetInt("ShowDefaultSteering", 0) == 0;
		if ((bool)V.Car)
		{
			Powertrain component3 = V.Car.GetComponent<Powertrain>();
			if (LevelsProvider.CurrentLevelData.ID == 56)
			{
				component3.ShutdownEngine();
			}
		}
		if (num)
		{
			_openDefSteerWindowDisposable?.Dispose();
			_openDefSteerWindowDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				OpenDefaultStreeringWindow();
			}).AddTo(this);
		}
		if (_controlItemsProvider.ShifterControl != null)
		{
			_controlItemsProvider.ShifterControl.ShowObjects(fromControlUi: false);
		}
		if (DriverOutSideBegin)
		{
			_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car, enableCollidersImmediately: true);
			if (V.DriverPerson.TryGetComponent<Rigidbody>(out var rb))
			{
				rb.isKinematic = false;
			}
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (!V.DriverPerson)
			{
				return;
			}
			V.DriverPerson.transform.position = LevelsProvider.CurrentLevelData.DriverStartPos;
			V.DriverPerson.transform.eulerAngles = LevelsProvider.CurrentLevelData.DriverStartRot;
			if ((bool)rb)
			{
				RigidbodyInterpolation interpolation = rb.interpolation;
				rb.interpolation = RigidbodyInterpolation.None;
				rb.position = LevelsProvider.CurrentLevelData.DriverStartPos;
				rb.rotation = Quaternion.Euler(LevelsProvider.CurrentLevelData.DriverStartRot);
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				Physics.SyncTransforms();
				rb.interpolation = interpolation;
			}
			GetComponent<PersonCameraControll>().x = LevelsProvider.CurrentLevelData.DriverStartRot.x;
			GetComponent<PersonCameraControll>().y = LevelsProvider.CurrentLevelData.DriverStartRot.y;
			Resume();
		}
		PlayerDatas.LookAtCarAudience(V.DriverPerson, _collectionsProvider.GetAllNpcPeople());
		HideReverseSing();
		if (LevelsProvider.CurrentLevelData.ID == 56)
		{
			ShowMessage("LevelScript.SitIntoACar", isImportant: true);
			levelDatas[LevelsProvider.CurrentLevelData.IndexInScene].ObjectsForEnable[1].gameObject.SetActive(value: true);
		}
		if (LevelsProvider.CurrentLevelData.TowCarLevel)
		{
			_levelSceneControl.CarTowLoad.SetActive(value: true);
			PlayerDatas.TowCar(V.Car, _levelSceneControl.CarTowLoad);
			DisableBrokenCarCollider(_levelSceneControl.CarTowLoad);
		}
		_managerUI.HideActionButton();
		FlagsStorage.LevelStarted = true;
		InputGroupsEnum newGroup = (FlagsStorage.OutOfCar ? InputGroupsEnum.Person : InputGroupsEnum.CarFree);
		_inputMapSetEvent.TrySetGroup(newGroup, setRoot: true);
	}

	private void LocalizeMessage()
	{
		_menuLevelParent.LabelTop.SetText(new LocalizedString(_levelMessage));
	}

	private static void TryHideTopLabel(LevelsProvider levelsProvider, MenuLevelParent menuLevelParent)
	{
		if (!levelsProvider.CurrentLevelData.HideTopLabel)
		{
			return;
		}
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			CanvasGroup canvasGroup = menuLevelParent.LabelTopParent.GetComponent<CanvasGroup>();
			canvasGroup.DOFade(0f, 1f).OnComplete(() =>
			{
				menuLevelParent.LabelTopParent.SetActive(value: false);
				canvasGroup.alpha = 1f;
			});
		}).AddTo(menuLevelParent.LabelTopParent);
	}

	private void SetParkingTriggerPos(GameObject car)
	{
		if (DoNotChangeCubes)
		{
			return;
		}
		VehicleData component = car.GetComponent<VehicleData>();
		Powertrain component2 = car.GetComponent<Powertrain>();
		if ((bool)component && (bool)component2 && (bool)component.Collider)
		{
			Vector3 extents = component2.GetWeightBounds.extents;
			float num = extents.x;
			float num2 = extents.z;
			if (num < num2)
			{
				float num3 = num2;
				float num4 = num;
				num = num3;
				num2 = num4;
			}
			if ((bool)FR)
			{
				float y = FR.transform.localPosition.y;
				FR.transform.localPosition = new Vector3(0f - num2, y, 0f - num);
				FL.transform.localPosition = new Vector3(num2, y, 0f - num);
				RR.transform.localPosition = new Vector3(0f - num2, y, num);
				RL.transform.localPosition = new Vector3(num2, y, num);
			}
		}
	}

	private Vector3 CarYOffset(GameObject car)
	{
		float num = 0f;
		if (car.TryGetComponent<Axles>(out var component))
		{
			num = car.transform.position.y - component.AllWheels[0].transform.position.y;
			if ((bool)component.AllWheels[0].WheelModel)
			{
				num = car.transform.position.y - component.AllWheels[0].WheelModel.transform.position.y;
			}
			float radius = component.AllWheels[0].Wheel.Radius;
			num += radius;
		}
		else
		{
			num = -0.4f;
		}
		return new Vector3(0f, num, 0f);
	}

	private void UpdateLeftTime()
	{
		_menuLevelParent.timeLeftLabel.SetText($"{_timeLeft:N1}");
		if (LevelsProvider.CurrentLevelData.Type == LevelType.Chasing)
		{
			if ((bool)_trafficCarControl.Value.CarToFollow && !(Vector3.Distance(V.DriverPerson.transform.position, _trafficCarControl.Value.CarToFollow.transform.position) <= 70f))
			{
				FailString = new LocalizedString("LevelScript.DriveFaster");
				OpenFailMenu();
			}
		}
		else if (LevelType == LevelType.DragRace)
		{
			float num = (LevelsProvider.RacingCarLevel ? Vector3.Distance(LevelsProvider.RacingCarLevel.transform.position, LevelsProvider.CurrentLevelData.RacingCarInitialPos) : 0f);
			float num2 = (FirstCarLevel ? Vector3.Distance(FirstCarLevel.transform.position, LevelsProvider.CurrentLevelData.CarInitialPos) : 0f);
			float num3 = num2 - num;
			_menuLevelParent.timeLeftLabel.SetText((num3 >= 0f) ? string.Format("{0}+{1:N0}m", "<color=#99FF00>", num2 - num) : string.Format("{0}{1:N0}m", "<color=#FF0000>", num2 - num));
		}
	}

	public void ShowMessage(string mes, bool isImportant)
	{
		if (!_menuLevelParent.LabelTop)
		{
			return;
		}
		_levelMessage = mes;
		_menuLevelParent.LabelTop.SetText(new LocalizedString(mes));
		_menuLevelParent.LabelMiddle.SetText(new LocalizedString(mes));
		if (isImportant)
		{
			_menuLevelParent.LabelTopParent.SetActive(value: false);
			_menuLevelParent.LabelMiddleParent.SetActive(value: true);
			_showOnlyTopLabelDispose?.Dispose();
			_showOnlyTopLabelDispose = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				ShowOnlyTopLabel();
			}).AddTo(this);
		}
		else
		{
			_menuLevelParent.LabelTopParent.SetActive(value: true);
			_menuLevelParent.LabelMiddleParent.SetActive(value: false);
		}
	}

	public void ResetTopMessage()
	{
		_menuLevelParent.LabelTop.text = new LocalizedString(_messages[5]);
	}

	private void ShowOnlyTopLabel()
	{
		_menuLevelParent.LabelTopParent.SetActive(value: true);
		_menuLevelParent.LabelMiddleParent.SetActive(value: false);
		_menuLevelParent.LabelTopParent.SetActive(LevelsProvider.CurrentLevelData.Type != LevelType.Chasing);
	}

	public void OpenPauseMenu()
	{
		SetActiveFalseEachMenu(2);
		CleanScreen();
		PlayerPrefs.SetFloat("SWsensitivity", V.SWsensitivity);
		if (FlagsStorage.IsMultiFree)
		{
			string playerName = _playerDataStorage.PlayerName;
			if ((playerName == "" || playerName == "nickname...") && (bool)_freeDriveProvider.FreeDriveControl.EnterNickName)
			{
				_freeDriveProvider.FreeDriveControl.EnterNickName.SetActive(value: true);
			}
		}
	}

	public void Resume(bool ignoreBrakes = false, bool ignoreThrottle = false)
	{
		SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
		IsLevelPaused = false;
		Camera component = GetComponent<Camera>();
		if (FlagsStorage.IsFreeDriveMode)
		{
			_freeDriveProvider.MapCamera.HideBigMap();
		}
		if (!component || component.enabled || !FlagsStorage.IsFreeDriveMode)
		{
			SetActiveFalseEachMenu((!_menuLevelParent.isDroneEnabled) ? 1 : 20, ignoreThrottle);
			_carInputManager.PressThrottle(0f);
			if (!ignoreBrakes)
			{
				_carInputManager.PressBrake(0f);
				_carInputManager.PressHandbrake(0f);
			}
			_managerUI.ClosePhoneWindow();
			UnCleanScreen(ignoreBrakes);
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				_menuLevelParent.HideEmoji();
				_inputMapSetEvent.TrySetRootGroup();
			}
		}
	}

	private void SetCarForLevel()
	{
		if (FlagsStorage.OutOfCar && !DriverOutSideBegin)
		{
			_coreGameManager.CarOutInControl.InCar(V.DriverPerson, FirstCarLevel).Forget();
		}
		else if (FlagsStorage.IsPassenger && !DriverOutSideBegin)
		{
			_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, FirstCarLevel);
			_coreGameManager.CarOutInControl.InCar(V.DriverPerson, FirstCarLevel).Forget();
		}
		_carInputManager.SetGearToCar(_startGear);
		ToInitialPos();
		if ((bool)_carInputManager.UGUISteeringWheel)
		{
			_carInputManager.UGUISteeringWheel.enabled = true;
			_carInputManager.UGUISteeringWheel.ResetWheel();
		}
		ResetFreeSpaceSensors();
	}

	private void ResetFreeSpaceSensors()
	{
		foreach (ParkingFreeSpaceSensor allFreeSpaceSensor in _collectionsProvider.GetAllFreeSpaceSensors())
		{
			allFreeSpaceSensor.CheckForSensor();
		}
	}

	public void OpenFailMenu()
	{
		SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.LevelNotCompleteSwitch, base.gameObject);
		OnLevelFailMenuOpen?.Invoke();
		_managerUI.CloseFreeAnimationWindow();
		_managerUI.HideActionButton();
		_menuLevelParent.FailLabel.SetText(FailString);
		SetVelocityAfter();
		ShowMenuAfter(5);
		EnableButtonsAfter();
		SetActiveFalseEachMenu(5);
		SmoothShowFailMenu();
		_menuLevelParent.FailLabel.gameObject.SetActive(value: false);
		CleanScreen();
		_updateLeftTimeDisposable?.Dispose();
		if ((bool)_menuLevelParent.endlessTimeCrashWindow && !FlagsStorage.EndlessCrashTime)
		{
			_menuLevelParent.endlessTimeCrashWindow.SetActive(value: false);
		}
		FlagsStorage.LevelStarted = false;
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Garage, setRoot: true);
	}

	public void SmoothShowFailMenu()
	{
		ShowMenuAfter(5);
		_menuLevelParent.FailLabel.gameObject.SetActive(value: false);
	}

	public void OpenOtherLevelsMenu()
	{
		SetActiveFalseEachMenu(6);
	}

	private void OnApplicationQuit()
	{
		V.CoreGameManager.SendEventLevelCurrentQuitLevel();
		duration = 0f;
	}

	public void OpenLoadingMenu()
	{
		CleanScreen();
		SetActiveFalseEachMenu(7);
	}

	public void ShowConnectingMenu()
	{
		CleanScreen();
		SetActiveFalseEachMenu(8);
	}

	public void OpenSettingMenu()
	{
		CleanScreen();
		SetActiveFalseEachMenu(3);
	}

	public void OpenFirstMenu()
	{
		SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
		FlagsStorage.IsEnabledJoy = true;
		FlagsStorage.LevelStarted = false;
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Garage, setRoot: true);
		SetActiveFalseEachMenu(0);
		CleanScreen();
		Time.timeScale = 1f;
		_messages[5] = LevelsProvider.CurrentLevelData.Title;
		if ((bool)_menuLevelParent.TitleLabel)
		{
			_menuLevelParent.TitleLabel.SetText(new LocalizedString(_messages[5]));
		}
		HideReverseSing();
		if (!(LevelsProvider.CurrentLevelData.InitialCamPos == Vector3.zero))
		{
			base.transform.SetPositionAndRotation(LevelsProvider.CurrentLevelData.InitialCamPos, Quaternion.Euler(LevelsProvider.CurrentLevelData.InitialCamRot));
			if (LevelsProvider.CurrentLevelData.FieldOfView != 0f)
			{
				GetComponent<Camera>().fieldOfView = LevelsProvider.CurrentLevelData.FieldOfView;
			}
		}
	}

	public void NextLevel()
	{
		duration = 0f;
		showLeftTime = 0.1f;
		LoadNextScene();
		Time.timeScale = 1f;
	}

	private void LoadNextScene()
	{
		if ((bool)V.AnalyticWheres)
		{
			V.AnalyticWheres.Save();
		}
		_menuLevelParent.DoneLabel.SetText(new LocalizedString("LevelLoader.Loading"));
		LevelDataScript nextSceneData = PlayerDatas.GetNextSceneData(LevelsProvider.CurrentLevelData.ID, _levelsConfig, _allCarsInfo);
		if (nextSceneData == null)
		{
			_menuLevelParent.OpenMainMenu();
			return;
		}
		LevelsProvider.CurrentLevelData = nextSceneData;
		if (LevelsProvider.CurrentLevelData.SceneForLoad == SceneManager.GetActiveScene().buildIndex)
		{
			RestartScene();
		}
		else
		{
			SwitchSceneWithCurtain(LevelsProvider.CurrentLevelData.SceneForLoad);
		}
	}

	private void SwitchSceneWithCurtain(int buildIndex)
	{
		ManagerUI managerUI = _managerUI;
		UniTask.Void(async () =>
		{
			try
			{
				await managerUI.ShowWaitingWindowDark(set: true);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Curtain] шторка не показана: " + ex.Message);
			}
			AsyncSceneSwitcher.Load(buildIndex);
		});
	}

	public void WatchAndContinue1()
	{
		_menuLevelParent.CancelShowRewardButton.SetActive(value: false);
		EnableCancelButton();
	}

	public void DoubleMoneyButton()
	{
		showLeftTime = 0f;
		_menuLevelParent.menus[4].SetActive(value: false);
		WatchAndContinue1();
	}

	public void DoubleMoney()
	{
		if (math.all(_uiGui == Utils.Valid))
		{
			showLeftTime = 0f;
			ObscuredInt obscuredInt = Mathf.RoundToInt((float)_timeLeft * 60f);
			_currencyManagerController.AddMoney(new AdvancedInt(obscuredInt));
			_menuLevelParent.MoneyWonLevelLabel.SetText($"+{obscuredInt}");
			Time.timeScale = 0f;
		}
	}

	private void EnableCancelButton()
	{
		_menuLevelParent.CancelShowRewardButton.SetActive(value: true);
	}

	public void WatchAndContinue2()
	{
		_updateLeftTimeDisposable?.Dispose();
		_updateLeftTimeDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Repeat().Subscribe((long _) =>
		{
			UpdateLeftTime();
		})
			.AddTo(this);
		Resume();
		FlagsStorage.LevelStarted = true;
		_inputMapSetEvent.TrySetRootGroup();
		_resetCarDamageBoolDisposable?.Dispose();
		_resetCarDamageBoolDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			ResetCarDamageBool();
		}).AddTo(this);
		if ((bool)V.Car)
		{
			if ((bool)LevelsProvider.Trailer)
			{
				Powertrain component = LevelsProvider.Trailer.GetComponent<Powertrain>();
				component.AngularVelocity = Vector3.zero;
				component.Velocity = Vector3.zero;
			}
			_setVelocityAfterDisposable?.Dispose();
			_setVelocityAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				SetVelocityAfter();
			}).AddTo(this);
		}
		_carInputManager.ClearPedals();
		_carCameras.FollowPosRot = false;
		FlagsStorage.LoadCollidedInvoked = false;
		if (LevelsProvider.CurrentLevelData.Type != LevelType.Chasing)
		{
			_timeLeft = (float)_timeLeft + 60f;
			_levelTime = (float)_levelTime + 60f;
		}
		if ((bool)LoadControl && _hasLoad != 0)
		{
			LoadControl.ResetPosition(_hasLoad - 1, V.Car, makeKinematic: false);
			if (WithTrailer)
			{
				LoadControl.ResetPosition(_hasLoad, LevelsProvider.Trailer, makeKinematic: false);
			}
			_resetLoadAfterDisposable?.Dispose();
			_resetLoadAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				ResetLoadAfter();
			}).AddTo(this);
		}
	}

	private void ResetLoadAfter()
	{
		LoadControl.ResetPosition(_hasLoad - 1, V.Car, makeKinematic: false);
		int iD = LevelsProvider.CurrentLevelData.ID;
		if (iD == 81 || iD == 84 || iD == 86)
		{
			LoadControl.ResetPosition(_hasLoad, LevelsProvider.Trailer, makeKinematic: false);
		}
	}

	public void CancelRewardVideo()
	{
		MonoBehaviour.print(" CancelRewardVideo");
		if (V.MenuIndex == 4)
		{
			_menuLevelParent.menus[4].SetActive(value: true);
		}
	}

	private void ResetCarDamageBool()
	{
		_carDamagedDuringLevel |= FlagsStorage.IsCarDamaged;
		FlagsStorage.IsCarDamaged = false;
	}

	private bool WasCarDamagedDuringLevel()
	{
		if (!_carDamagedDuringLevel)
		{
			return FlagsStorage.IsCarDamaged;
		}
		return true;
	}

	public void OpenMainMenu()
	{
		if (_alreadyPressedMenu)
		{
			return;
		}
		_alreadyPressedMenu = true;
		if ((bool)V.AnalyticWheres)
		{
			V.AnalyticWheres.Save();
		}
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			if ((bool)_freeDriveProvider.FreeDriveControl && (bool)_freeDriveProvider.FreeDriveControl.EnterNickName)
			{
				_freeDriveProvider.FreeDriveControl.EnterNickName.SetActive(value: false);
			}
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
			}
		}
		V.CoreGameManager.SendEventLevelCurrentQuitLevel();
		duration = 0f;
		SceneManager.LoadScene(1);
	}

	private void DoneLabelUpdate(float leftTime)
	{
		showMoney = (int)leftTime * 30;
		_menuLevelParent.MoneyWonLevelLabel.gameObject.SetActive(value: true);
		_menuLevelParent.DoneLabel.SetText($"{(float)_levelTime - (float)_timeLeft:N1} Sec");
	}

	private float GetBestResult()
	{
		if (!LevelsProvider.CurrentLevelData)
		{
			return 0f;
		}
		MonoBehaviour.print($"bestResultID{LevelsProvider.CurrentLevelData.ID} :{LevelsProvider.CurrentLevelData.ID}");
		return ObscuredPrefs.GetFloat($"bestResultID{LevelsProvider.CurrentLevelData.ID}");
	}

	public void OpenLevelDoneMenu()
	{
		SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.LevelCompleteSwitch, base.gameObject);
		CleanScreen();
		_managerUI.HideActionButton();
		float value = (float)_levelTime - (float)_timeLeft;
		value = Mathf.Clamp(value, 0f, 300f);
		ObscuredInt obscuredInt = Mathf.Clamp((int)(float)_timeLeft * 30, 0, 10000);
		MoneyWon = obscuredInt;
		float bestResult = GetBestResult();
		_menuLevelParent.MoneyWonLevelLabel.SetText(string.Empty);
		_uiGui = Utils.Valid;
		if (!_rewardByTime)
		{
			if (LevelType == LevelType.Chasing)
			{
				if (bestResult == 0f)
				{
					SetLevelReward();
				}
				_menuLevelParent.topDoneLabel.SetText(string.Empty);
				_menuLevelParent.DoneLabel.SetText("<color=#99FF00>" + new LocalizedString("LevelScript.ColorstagsLevelsgreenGreatDriv"));
			}
			else
			{
				SetLevelReward();
			}
		}
		else
		{
			DoneLabelUpdate(_timeLeft);
			MonoBehaviour.print($"{bestResult} and {value}");
			if (bestResult >= value || bestResult == 0f)
			{
				if (LevelsProvider.CurrentLevelData.ID == 61 && bestResult >= value)
				{
					obscuredInt = 0;
				}
				_currencyManagerController.AddMoney(new AdvancedInt(obscuredInt));
				_menuLevelParent.MoneyWonLevelLabel.SetText($"+${obscuredInt}");
				_menuLevelParent.topDoneLabel.SetText(new LocalizedString("LevelScript.PersonalBestTime"));
				_menuLevelParent.topDoneLabel2.SetText(new LocalizedString("LevelScript.Congratulations"));
			}
			else
			{
				_menuLevelParent.topDoneLabel.SetText(new LocalizedString("LevelScript.NotYourBestTime"));
				_menuLevelParent.topDoneLabel2.SetText(new LocalizedString("LevelScript.YouCanDoItFaster"));
				_menuLevelParent.MoneyWonLevelLabel.SetText("+$0");
			}
		}
		PlayerDatas.RecordBestReuslt((float)_levelTime - (float)_timeLeft, LevelsProvider);
		LevelType levelType = LevelType;
		if (levelType == LevelType.Chasing || levelType == LevelType.DragRace)
		{
			SetActiveFalseEachMenu(4);
		}
		else
		{
			ShowMessage("LevelLoader.Done", isImportant: true);
			ShowMenuAfter(4);
			if (LevelsProvider.CurrentLevelData.ID != 55)
			{
				SetSmoothCamMove(V.Car.transform.position, LevelsProvider.CurrentLevelData.InitialCamPos);
			}
		}
		FlagsStorage.LevelStarted = false;
		if (LevelType == LevelType.Parking && !WasCarDamagedDuringLevel())
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEg", "parking_level");
			GooglePlayAchievements.MarkCleanSlateParking("parking_level");
		}
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Garage, setRoot: true);
		_steamAchievementService?.Unlock("ACH_PERFECTLY_PARKED");
		if (!FlagsStorage.IsCarDamaged)
		{
			_steamAchievementService?.MarkCleanSlateParking();
		}
		V.CoreGameManager.SendEventLevelDone((float)_levelTime - (float)_timeLeft);
		SendDoneInOneDay();
		_levelsAnalytic.LogFinishLevel("complete");
	}

	private void ShowMenuAfter(int menuShowIndex)
	{
		_menuSubscription?.Dispose();
		_menuSubscription = Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224), Scheduler.MainThreadIgnoreTimeScale).Subscribe((long _) =>
		{
			SetActiveFalseEachMenu(menuShowIndex);
			_menuLevelParent.FailLabel.SetText(FailString);
			_menuLevelParent.FailLabel.gameObject.SetActive(value: true);
		}).AddTo(this);
	}

	private void EnableButtonsAfter()
	{
		_enableButtonsSubscription?.Dispose();
		_enableButtonsSubscription = Observable.Timer(TimeSpan.FromSeconds(0.6000000238418579), Scheduler.MainThreadIgnoreTimeScale).Subscribe((long _) =>
		{
			if ((bool)_menuLevelParent.endlessTimeCrashWindow && !FlagsStorage.EndlessCrashTime)
			{
				_menuLevelParent.endlessTimeCrashWindow.SetActive(value: true);
			}
		}).AddTo(this);
	}

	private void SendDoneInOneDay()
	{
	}

	public void OpenSelectionManualShiftingMode()
	{
		_menuLevelParent.EnableObjectInArray(_menuLevelParent.ManualModeSprites, PlayerPrefs.GetInt("ManualShiftingMode", -1));
		SetActiveFalseEachMenu(8);
		CleanScreen();
	}

	public void OpenDefaultStreeringWindow()
	{
		PlayerPrefs.SetInt("ShowDefaultSteering", 1);
		SetActiveFalseEachMenu(9);
		CleanScreen();
	}

	private void SetLevelReward()
	{
		if (!FlagsStorage.IsFreeDriveMode && (bool)LevelsProvider.CurrentLevelData && math.all(_uiGui == Utils.Valid))
		{
			_menuLevelParent.DoneLabel.text = "<color=#99FF00>" + new LocalizedString("LevelScript.ColorstagsLevelsgreen1GreatDri");
			AdvancedInt advancedInt = new AdvancedInt(LevelsProvider.CurrentLevelData.Reward);
			_menuLevelParent.MoneyWonLevelLabel.SetText($"+{advancedInt}");
			_currencyManagerController.AddMoney(advancedInt);
			_uiGui = Utils.NotValid;
		}
	}

	private void SetTrailer()
	{
		if ((bool)LevelsProvider.Trailer)
		{
			Debug.Log("Set Trailer");
			Vector3 trailerSpawnLocalPosition = GetTrailerSpawnLocalPosition(V.Car, LevelsProvider.Trailer);
			LevelsProvider.Trailer.transform.SetLocalPositionAndRotation(trailerSpawnLocalPosition, Quaternion.identity);
			Powertrain component = LevelsProvider.Trailer.GetComponent<Powertrain>();
			component.Velocity = Vector3.zero;
			component.AngularVelocity = Vector3.zero;
		}
	}

	private Vector3 GetTrailerSpawnLocalPosition(GameObject car, GameObject trailer)
	{
		if (!car || !trailer)
		{
			return localTrailerPos;
		}
		if (!car.TryGetComponent<Powertrain>(out var component) || !component.TrailerJoint)
		{
			return localTrailerPos;
		}
		if (!trailer.TryGetComponent<TrailerEntity>(out var component2) || !component2.JackBall)
		{
			return localTrailerPos;
		}
		return component.TrailerJoint.localPosition - component2.JackBall.localPosition;
	}

	private void ToTargetPos()
	{
		SetTrailer();
		int mask = LayerMask.GetMask("Default", "Road");
		Vector3 vector = _carTargetPos;
		int num = Physics.RaycastNonAlloc(_carTargetPos + Vector3.up * 2f, Vector3.down, _tempHits, 10f, mask);
		if (num > 0 && !LevelsFinishPositionsExeption.Contains(LevelsProvider.CurrentLevelData.ID))
		{
			Vector3 vector2 = ((!_tempHits[0].collider.isTrigger) ? _tempHits[0].point : Vector3.zero);
			for (int i = 0; i < num; i++)
			{
				if (_tempHits[i].point.y > vector2.y && !_tempHits[i].collider.isTrigger)
				{
					vector2 = _tempHits[i].point;
				}
			}
			vector = vector2;
		}
		SetVelocityAfter();
		PlaceLevelCar(vector + _carOffsetPos, Quaternion.Euler(_carTargetRot));
		_isInTargetPos = true;
	}

	private void ToInitialPos()
	{
		SetTrailer();
		PlaceLevelCar(LevelsProvider.CurrentLevelData.CarInitialPos + _carOffsetPos, Quaternion.Euler(LevelsProvider.CurrentLevelData.CarInitRot));
		_isInTargetPos = false;
	}

	private void PlaceLevelCar(Vector3 position, Quaternion rotation)
	{
		GameObject levelCar = LevelCar;
		if (!levelCar)
		{
			Debug.LogError("LevelScript: no car to reposition");
			return;
		}
		levelCar.transform.SetPositionAndRotation(position, rotation);
		if (levelCar.TryGetComponent<Rigidbody>(out var component) && !component.isKinematic)
		{
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		Physics.SyncTransforms();
	}

	private void StartChangingPos()
	{
		if (!LevelsProvider.CurrentLevelData)
		{
			StopChangingPos();
		}
		else if (_isInTargetPos)
		{
			ToInitialPos();
		}
		else
		{
			ToTargetPos();
		}
	}

	private void StopChangingPos()
	{
		_startChangingPosDisposable?.Dispose();
		_startChangingPosDisposable = null;
	}

	private void SetLoadKinematic()
	{
		if ((bool)LoadControl)
		{
			LoadControl.ResetPosition(_hasLoad - 1, V.Car, makeKinematic: true);
			if (LevelsProvider.CurrentLevelData.LoadTrailer)
			{
				LoadControl.ResetPosition(_hasLoad, LevelsProvider.Trailer, makeKinematic: true);
			}
		}
	}

	private void Update()
	{
		if (FlagsStorage.LevelStarted)
		{
			if ((LevelType != LevelType.DragRace && !FlagsStorage.EndlessCrashTime) || LevelType == LevelType.Chasing || (LevelType == LevelType.DragRace && FlagsStorage.RaceStarted))
			{
				_timeLeft = (float)_timeLeft - Time.deltaTime;
			}
			if ((float)_timeLeft < 10f && (float)_timeLeft > 9.8f)
			{
				ShowMessage("LevelScript.LessThan10sLeft", isImportant: true);
			}
			if ((float)_timeLeft < 0f)
			{
				if (LevelsProvider.CurrentLevelData.Type == LevelType.Chasing)
				{
					OpenLevelDoneMenu();
				}
				else
				{
					FailString = new LocalizedString("LevelScript.TimeIsOver");
					_levelsAnalytic.LogFinishLevel("timeout");
					OpenFailMenu();
				}
			}
			if (IsShowNewMessage && !FlagsStorage.OutOfCar)
			{
				ShowArrow();
				bool flag = true;
				bool isImportant = false;
				string mes = "";
				float num = 0f;
				if (LevelsProvider.CurrentLevelData.IsReverseParking && (bool)V.Car && (bool)AngleMeasurePoint)
				{
					Vector3 to = AngleMeasurePoint.position - V.Car.transform.position;
					num = Vector3.Angle(V.Car.transform.forward, to);
				}
				if (!LevelsProvider.CurrentLevelData.IsReverseParking || (LevelsProvider.CurrentLevelData.IsReverseParking && num < 110f))
				{
					bool flag2 = (RRight && RLeft && FRight && !FLeft) || (RRight && RLeft && !FRight && FLeft) || (!RRight && RLeft && FRight && FLeft) || (!RRight && RLeft && FRight && FLeft);
					if (RRight && RLeft && !FRight && !FLeft)
					{
						mes = _messages[0];
					}
					else if (!RRight && !RLeft && FRight && FLeft)
					{
						mes = _messages[1];
					}
					else if (!RRight && RLeft && !FRight && FLeft)
					{
						mes = _messages[2];
					}
					else if (RRight && !RLeft && FRight && !FLeft)
					{
						mes = _messages[3];
					}
					else if (!RRight && RLeft && FRight && !FLeft)
					{
						mes = _messages[4];
						isImportant = true;
					}
					else if (RRight && !RLeft && !FRight && FLeft)
					{
						mes = _messages[5];
						isImportant = true;
					}
					else if (flag2)
					{
						mes = _messages[4];
						isImportant = true;
					}
					else
					{
						flag = false;
					}
					if (RRight && RLeft && FRight && FLeft && !FlagsStorage.IsCarDamaged)
					{
						bool flag3 = true;
						if ((bool)LevelsProvider.Trailer && LevelsProvider.CurrentLevelData.RelativeTrailerAngle != 0f)
						{
							float y = LevelsProvider.Trailer.transform.eulerAngles.y;
							differenceAngle = Mathf.Abs(Mathf.Abs(y) - LevelsProvider.CurrentLevelData.RelativeTrailerAngle);
							if (differenceAngle > 5f)
							{
								flag3 = false;
								isImportant = true;
								mes = "LevelScript.ParkATrailerStraight";
								flag = true;
							}
						}
						if (V.Car.GetComponent<Powertrain>().SpeedMps > 0.5f && LevelsProvider.CurrentLevelData.CheckForStop)
						{
							isImportant = true;
							mes = "LevelScript.Stophere!";
							flag = true;
							_checkAgainStateDisposable?.Dispose();
							_checkAgainStateDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
							{
								CheckAgainState();
							}).AddTo(this);
						}
						else if (flag3)
						{
							OpenLevelDoneMenu();
						}
					}
					bool active = !RRight && !RLeft && !FRight && !FLeft && V.CurrentCamIndex != 6;
					if ((bool)ParkHere3DText)
					{
						ParkHere3DText.SetActive(active);
					}
				}
				else
				{
					if (LevelsProvider.CurrentLevelData.IsReverseParking && num > 110f)
					{
						isImportant = false;
						mes = "LevelScript.MakeAReverseParking";
						flag = true;
						if (EnteredParkingCube)
						{
							EnteredParkingCube = false;
							ShowReverseSign();
						}
					}
					HideArrow();
				}
				if (flag)
				{
					ShowMessage(mes, isImportant);
				}
				IsShowNewMessage = false;
			}
		}
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			duration += Time.deltaTime;
		}
	}

	private void ShowReverseSign()
	{
		if ((bool)_menuLevelParent.ReverseSign)
		{
			_menuLevelParent.ReverseSign.SetActive(value: true);
			_hideReverseSingDisposable?.Dispose();
			_hideReverseSingDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				HideReverseSing();
			}).AddTo(this);
		}
	}

	private void HideReverseSing()
	{
		if ((bool)_menuLevelParent.ReverseSign)
		{
			_menuLevelParent.ReverseSign.SetActive(value: false);
		}
	}

	public void Back()
	{
		switch (prevMenuIndex)
		{
		case 0:
		case 1:
			OpenFirstMenu();
			break;
		case 2:
			OpenPauseMenu();
			break;
		case 5:
			OpenFailMenu();
			break;
		case 3:
		case 4:
			break;
		}
	}

	private void CheckAgainState()
	{
		IsShowNewMessage = true;
	}

	public void SetActiveFalseEachMenu(int i, bool ignoreThrottle = false)
	{
		if ((bool)_controlItemsProvider.ThrottleSliderControl && !ignoreThrottle)
		{
			_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
		}
		prevMenuIndex = V.MenuIndex;
		V.MenuIndex = i;
		if (!_menuLevelParent)
		{
			return;
		}
		for (int j = 0; j < _menuLevelParent.menus.Length; j++)
		{
			GameObject gameObject = _menuLevelParent.menus[j];
			if ((bool)gameObject)
			{
				bool flag = i == j;
				if (i != -1 && gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
				else if (gameObject.activeSelf && !flag)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
	}

	public void ChangeCam()
	{
		if (_freeDriveProvider != null && (bool)_freeDriveProvider.MapCamera && _freeDriveProvider.MapCamera.ShowingBigMap)
		{
			return;
		}
		if (V.CurrentCamIndex == 5 && (bool)_menuLevelParent.customLightSettingsMenu)
		{
			_ = _menuLevelParent.customLightSettingsMenu.activeSelf;
		}
		if ((bool)_carCameras)
		{
			_carCameras.camChangedByTouch = false;
		}
		if ((bool)_carCameras && !_carCameras.camChangedByTouch)
		{
			V.CurrentCamIndex++;
		}
		else
		{
			_ = V.CurrentCamIndex;
		}
		if (!HasFixedCam)
		{
			if (V.CurrentCamIndex > 5)
			{
				V.CurrentCamIndex = 0;
			}
		}
		else
		{
			V.CurrentCamIndex = IntitialCamIndex;
		}
		if (V.CurrentCamIndex == 4)
		{
			V.CurrentCamIndex = 5;
		}
		int num = 5;
		VehicleData component;
		if (V.CurrentCamIndex == num)
		{
			_lockCarButtonController.HideKickPassengersButton();
			_lockCarButtonController.SetActiveRoot(isActive: false);
		}
		else if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out component))
		{
			_lockCarButtonController.SetActiveRoot(isActive: true);
			_lockCarButtonController.ShowKickPassengersButton(component);
		}
		if ((bool)carCamerasController)
		{
			carCamerasController.SetCamera(V.CurrentCamIndex, FlagsStorage.OutOfCar ? V.DriverPerson.transform : V.Car.transform, FlagsStorage.OutOfCar);
		}
	}

	public void SetCameraByIndex(int index)
	{
		if (_freeDriveProvider != null && (bool)_freeDriveProvider.MapCamera && _freeDriveProvider.MapCamera.ShowingBigMap)
		{
			return;
		}
		if (FlagsStorage.IsSpectatorMode)
		{
			if ((bool)carCamerasController && (bool)carCamerasController.CachedCar)
			{
				V.CurrentCamIndex = index;
				carCamerasController.SetCamera(index, carCamerasController.CachedCar.transform, newTarget: false);
			}
			return;
		}
		Transform transform = ((!FlagsStorage.OutOfCar) ? (V.Car ? V.Car.transform : null) : (V.DriverPerson ? V.DriverPerson.transform : null));
		if ((bool)transform)
		{
			V.CurrentCamIndex = index;
			VehicleData component;
			if (V.CurrentCamIndex == 5)
			{
				_lockCarButtonController.HideKickPassengersButton();
				_lockCarButtonController.SetActiveRoot(isActive: false);
			}
			else if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out component))
			{
				_lockCarButtonController.SetActiveRoot(isActive: true);
				_lockCarButtonController.ShowKickPassengersButton(component);
			}
			if ((bool)carCamerasController)
			{
				carCamerasController.SetCamera(V.CurrentCamIndex, transform, FlagsStorage.OutOfCar);
			}
		}
	}

	private void ShowArrow()
	{
		if (!V.Car || !Arrow || V.Car.GetComponent<Powertrain>().SpeedMps > 5f)
		{
			return;
		}
		Arrow.SetActive(value: true);
		Vector3 position = Vector3.zero;
		Vector3 vector = Vector3.zero;
		bool[] array = new bool[4] { FLeft, FRight, RLeft, RRight };
		Transform[] array2 = new Transform[4] { FL.transform, FR.transform, RL.transform, RR.transform };
		Vector3 vector2;
		switch (array.Count((bool b) => b))
		{
		case 2:
			position = (FL.transform.position + FR.transform.position + RR.transform.position + RL.transform.position) * 0.25f;
			if (RRight)
			{
				if (RLeft && !FRight && !FLeft)
				{
					vector2 = (FL.transform.position + FR.transform.position) * 0.5f;
				}
				else
				{
					if (RLeft || !FRight || FLeft)
					{
						goto IL_0285;
					}
					vector2 = (RL.transform.position + FL.transform.position) * 0.5f;
				}
			}
			else if (!RLeft && FRight && FLeft)
			{
				vector2 = (RL.transform.position + RR.transform.position) * 0.5f;
			}
			else
			{
				if (!RLeft || FRight || !FLeft)
				{
					goto IL_0285;
				}
				vector2 = (RR.transform.position + FR.transform.position) * 0.5f;
			}
			goto IL_0288;
		case 3:
			{
				if (!array[0])
				{
					vector = array2[0].position;
					position = array2[1].position;
				}
				else if (!array[1])
				{
					vector = array2[1].position;
					position = array2[0].position;
				}
				else if (!array[2])
				{
					vector = array2[2].position;
					position = array2[3].position;
				}
				else if (!array[3])
				{
					vector = array2[3].position;
					position = array2[2].position;
				}
				break;
			}
			IL_0288:
			vector = vector2;
			if (vector == Vector3.zero)
			{
				position = Vector3.zero;
			}
			break;
			IL_0285:
			vector2 = vector;
			goto IL_0288;
		}
		Arrow.transform.position = position;
		Arrow.transform.LookAt(new Vector3(vector.x, position.y, vector.z));
		_hideArrowDispose?.Dispose();
		_hideArrowDispose = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			HideArrow();
		}).AddTo(this);
	}

	private void HideArrow()
	{
		if ((bool)Arrow)
		{
			Arrow.SetActive(value: false);
		}
	}

	public void OnActionButtonPressed()
	{
		if (LevelsProvider.CurrentLevelData.ID == 64)
		{
			CarLifter.ClosestLifter.ChangeMoveDirection();
		}
	}

	public void OnActionEntered()
	{
		if (LevelsProvider.CurrentLevelData.ID == 64 && _carLifterActive)
		{
			string localizationKey = (((bool)CarLifter.ClosestLifter && CarLifter.ClosestLifter.Lifting) ? new LocalizedString("CarLifter.Down") : new LocalizedString("CarLifter.Lift"));
			_ = _managerUI.ShowActionButton(new LocalizedString(localizationKey));
		}
	}

	public void OnActionExit()
	{
		if (LevelsProvider.CurrentLevelData.ID == 64)
		{
			_managerUI.HideActionButton();
		}
	}

	public void SetSmoothCamMove(Vector3 rotPos, Vector3 pos)
	{
		if ((bool)V.Car)
		{
			_freeDriveProvider.CarCameras.posTargetFollow.position = _freeDriveProvider.CarCameras.transform.position;
			_freeDriveProvider.CarCameras.rotTargetFollow.position = V.Car.transform.position;
			_freeDriveProvider.CarCameras.FollowPosRot = true;
			_freeDriveProvider.CarCameras.targetPositon = pos;
			_freeDriveProvider.CarCameras.targetPositionOfRotation = rotPos;
		}
	}

	public void EnableParking(bool isEnabled)
	{
		ParkingBuilding.SetActive(isEnabled);
		_levelSceneControl.SetLightsParking(isEnabled);
	}
}
