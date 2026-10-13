using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Account;
using CPM.Balance;
using CPM.Database;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.Genuine.CodeHash;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Kit;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Persistence.Systems;
using Photon.Pun;
using Racing.CameraManagement;
using Service;
using Service.Cargo;
using Service.Delivery;
using Service.Manager;
using Service.Taxi;
using Traffic;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.BuyNewCar;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Graphics;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.Cars;
using _Car_Parking.Scripts.WindowSystem;

public class CoreGameManager : MonoBehaviour
{
	public delegate void OnButtonClickDelegate(int i);

	private struct KitPanel
	{
		private List<Vector3> _vertices;

		public List<Vector3> Vertices
		{
			get
			{
				return _vertices;
			}
			set
			{
				_vertices = value;
			}
		}

		public GameObject Panel { get; set; }

		public KitPanel(GameObject panel)
		{
			Panel = panel;
			_vertices = new List<Vector3>();
		}

		public void Add(Vector3 vertex)
		{
			_vertices.Add(vertex);
		}
	}

	public struct SellCarValuesRefChangedData
	{
		public SellCarValues NewValue;

		public SellCarValues OldValue;

		public SellCarValuesRefChangedData(SellCarValues newValue, SellCarValues oldValue)
		{
			NewValue = newValue;
			OldValue = oldValue;
		}
	}

	private const int AnalytLevelDone = 11;

	private const int DefaultSellCarId = 59;

	private const int CountOpenedApplicationToShowWindowUpdateAndroid = 10;

	private const string CountOpenedWindowsUpdateAndroidKey = "CountOpenedWindowsUpdateAndroid";

	private const string DeviceNameToShowMessageUpdateAndroid = "samsung";

	private string[] DevicesModelsToShowMessageUpdateAndroid = new string[2] { "a13", "m13" };

	private const float AndroidVersionToNeed = 14f;

	public const string PersonCustomizationCompletedPrefsKey = "PersonCustomizationCompleted";

	private Camera _camera;

	[HideInInspector]
	public CarCameras carCameras;

	[HideInInspector]
	public CarCamerasController carCamerasController;

	[HideInInspector]
	public MenuControl menuControl;

	public bool isFreeDrive;

	public int AllCarNumber;

	public int[] AvailableCars;

	public Transform sellCarPos;

	public GameObject GaragLight;

	public GameObject directionalLight;

	public ObscuredInt SellCarToShowId;

	public ObscuredInt[] CarOrder;

	private ObscuredFloat _moneyPrice;

	private ObscuredInt _coins;

	public GameObject RemoveAdsButton;

	public Material CarMaterialMain;

	public bool Fog = true;

	public Vector3 defaultPosCar;

	public GameObject SellCarRewardShowButton;

	public bool delateAll;

	public bool CarFinished;

	public int[] removedCars;

	public ParticleSystem skidSmoke;

	public ParticleSystem sandSmoke;

	public Transform SecureKeysManager;

	public GameObject mainMenuDesertPos;

	public GameObject NewGameObject;

	private GameObject _rr;

	private GameObject _rr2;

	private int _idSellCarModel;

	private bool _turnOn;

	public TextMesh GarageNameLabel;

	private float _lastUnloadTime;

	private ObscuredString _hashCalculated;

	private bool _levelListLoaded;

	private bool _buying;

	private bool _mustStopLoad;

	private bool _tempLightChangeValue;

	public GameObject globalPoint;

	private bool _isRunning;

	private CancellationTokenSource _lookAtCts;

	private CancellationTokenSource _loadAssetCts;

	public GameObject carImage;

	private IGarageSelectorService _garageSelectorService;

	private IFreeDriverService _freeDriverService;

	private OptionalDependency<GarageController> _garageControllerDep;

	private OptionalDependency<CameraManager> _cameraManagerDep;

	private OptionalDependency<IGarageSelectorService> _garageSelectorServiceDep;

	private OptionalDependency<IFreeDriverService> _freeDriverServiceDep;

	private OptionalDependency<MapIconsModel> _mapIconsModelDep;

	private OptionalDependency<MapIconsDataSo> _mapIconsDataSoDep;

	private PlayerDataStorage _playerDataStorage;

	private IDatabaseSaveLoadService _databaseSaveLoadService;

	private IAccountController _accountController;

	public IPrefabFactory _prefabFactory;

	private readonly Vector2 _buyCarCameraInitRotation = new Vector2(0f, 0f);

	private GaragePositionsConfig _garagePositionsConfig;

	private Transform _tempCamTarget;

	private GameObject _sellCarModel;

	private SellCarValues _sellCarValues;

	private MenuControl _menuControl;

	private ISlotService _slotService;

	private AsyncOperationHandle<GameObject> _asyncOperationHandleSellCar;

	private AsyncOperationHandle<GameObject> _asyncOperationHandleBody;

	private ICarInfoService _carInfoService;

	private GarageController _garageController;

	private CameraManager _cameraManager;

	private IPlayerStorageService _playerStorageService;

	private IDatabaseCarsService _databaseCarsService;

	private CurrencyManagerController _currencyManagerController;

	private LockCarButtonController _lockCarButtonController;

	private LoadWalletFromDataBase _loadWalletFromDataBase;

	private SlotsOnDatabaseCollection _slotsOnDatabaseCollection;

	private SlotsOnDatabaseSaveLoadService _slotsOnDatabaseSaveLoadService;

	private IBuyCarBackendService _buyCarBackendService;

	private CarBuyRequestFactory _carBuyRequestFactory;

	public OnlineCollectionsProvider _onlineCollectionsProvider;

	private GlobalCollectionsProvider _collectionsProvider;

	private InputItemsProvider _controlItemsProvider;

	private AnalyticEventFactory _analyticEventFactory;

	private IPlayerStatusSystem _playerStatusSystem;

	private InterestGroupManager _interestGroupManager;

	private CarInputManager _carInputManager;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private CharactersService _charactersService;

	private IOfflineModeSystem _offlineModeSystem;

	private ICheckCarDataBase _checkCarDataBase;

	public ObscuredString NikNameInputText = "";

	[FormerlySerializedAs("_managerUI")]
	public ManagerUI ManagerUI;

	[FormerlySerializedAs("newCarsWindow")]
	public DealerWindow dealerWindow;

	public bool firstTypingTextFinished;

	private IDisposable _disposableLoadSellCarModel;

	private bool _sellCarLoadInProgress;

	private bool _sellCarLoadQueued;

	private const float SellCarScrollDetectWindow = 0.35f;

	private const float SellCarScrollDebounce = 0.4f;

	private float _lastSellCarRequestTime = float.NegativeInfinity;

	private IDisposable _enableCameraDisposable;

	private IDisposable _setTargetAfterDisposable;

	private IDisposable _onCamStopedMainMenuDisposable;

	private IDisposable _makeSpeedFollowFasterDisposable;

	private IDisposable _startMakeScreenShotDisposable;

	private IDisposable _afterScreenShotDisposable;

	private bool _isDestroyed;

	private CancellationTokenSource _cancellationTokenSource;

	public Action<MultiPersonControl, bool> OnPlayerMuted;

	private AsyncOperationHandle<GameObject>? _handleSellCar;

	private AsyncOperationHandle<GameObject>? _handleBody;

	private const int MaxSyncDataRetries = 60;

	private int _syncDataRetryCount;

	[field: SerializeField]
	public GameObject CameraListener { get; private set; }

	public Camera MainCamera => _camera;

	public AssetBundleManager AssetBundleManager { get; private set; }

	public bool IsSellCarModelReady { get; private set; }

	public GameObject SellCarModel
	{
		get
		{
			return _sellCarModel;
		}
		private set
		{
			_sellCarModel = value;
			if ((bool)_sellCarModel)
			{
				SellCarValues = _sellCarModel.GetComponent<SellCarValues>();
			}
		}
	}

	public SellCarValues SellCarValues
	{
		get
		{
			return _sellCarValues;
		}
		private set
		{
			SellCarValuesRefChangedData obj = new SellCarValuesRefChangedData(value, _sellCarValues);
			_sellCarValues = value;
			OnSellCarValuesRefChanged?.Invoke(obj);
		}
	}

	public ISlotService SlotService => _slotService;

	public ICarInfoService CarInfoService => _carInfoService;

	public GarageController GarageController => _garageController;

	public bool EndlessFuel { get; private set; }

	public GameObject Spark { get; private set; }

	public LevelsProvider LevelsProvider { get; private set; }

	public CarOutInControl CarOutInControl { get; private set; }

	public MenuLevelParent MenuLevelParent { get; private set; }

	public FreeDriveProvider FreeDriveProvider { get; private set; }

	public CarInteractionAnimationsCache CarInteractionAnimationsCache { get; private set; } = new CarInteractionAnimationsCache();

	public MapIconsModel MapIconsModel { get; private set; }

	public MapIconsDataSo MapIconsDataSo { get; private set; }

	public WhooshSoundController WhooshSoundController { get; private set; }

	public SoundBankLoadService SoundBankLoadService { get; private set; }

	public RainSoundState RainSoundState { get; private set; }

	public RainSound RainSound { get; private set; }

	public bool IsPlayerSetuping { get; set; }

	public bool IsFirstStarting { get; set; }

	public bool PersonAnimated { get; set; }

	public event Action<SellCarValuesRefChangedData> OnSellCarValuesRefChanged;

	public event OnButtonClickDelegate OnChangeCarButtonClick;

	public event OnButtonClickDelegate OnOpen;

	public event OnButtonClickDelegate ButtonPressed;

	public event OnButtonClickDelegate BuyButtonPressed;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, MenuLevelParent menuLevelParent, IDatabaseSaveLoadService databaseSaveLoadService, OnlineCollectionsProvider onlineCollectionsProvider, IAccountController accountController, GlobalCollectionsProvider collectionsProvider, IPrefabFactory prefabFactory, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, ISlotService slotService, ICarInfoService carInfoService, ManagerUI managerUI, IPlayerStorageService playerStorageService, IDatabaseCarsService databaseCarsService, GaragePositionsConfig garagePositionsConfig, InputItemsProvider controlItemsProvider, CurrencyManagerController currencyManagerController, LockCarButtonController lockCarButtonController, AllCarsInfo allCarsInfo, SlotsOnDatabaseSaveLoadService slotsOnDatabaseSaveLoadService, IBuyCarBackendService buyCarBackendService, CarBuyRequestFactory carBuyRequestFactory, IPlayerStatusSystem playerStatusSystem, AnalyticEventFactory analyticEventFactory, InterestGroupManager interestGroupManager, ICharacterEquipmentDataService characterEquipmentDataService, CharactersService charactersService, IOfflineModeSystem offlineModeSystem, CarInputManager carInputManager, ICheckCarDataBase checkCarDataBase, AssetBundleManager assetBundleManager, OptionalDependency<MapIconsModel> mapIconsModel, OptionalDependency<MapIconsDataSo> mapIconsDataSo, OptionalDependency<GarageController> garageController, OptionalDependency<CameraManager> cameraManager, OptionalDependency<IGarageSelectorService> garageSelectorService, OptionalDependency<IFreeDriverService> freeDriverService, SoundBankLoadService soundBankLoadService, RainSoundState rainSoundState, RainSound rainSound)
	{
		_interestGroupManager = interestGroupManager;
		LevelsProvider = levelsProvider;
		FreeDriveProvider = freeDriveProvider;
		_controlItemsProvider = controlItemsProvider;
		_collectionsProvider = collectionsProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		MenuLevelParent = menuLevelParent;
		_carBuyRequestFactory = carBuyRequestFactory;
		_buyCarBackendService = buyCarBackendService;
		_slotsOnDatabaseSaveLoadService = slotsOnDatabaseSaveLoadService;
		_lockCarButtonController = lockCarButtonController;
		_currencyManagerController = currencyManagerController;
		_databaseCarsService = databaseCarsService;
		_playerStorageService = playerStorageService;
		_cameraManagerDep = cameraManager;
		_garageControllerDep = garageController;
		_carInfoService = carInfoService;
		_slotService = slotService;
		_playerDataStorage = playerDataStorage;
		_databaseSaveLoadService = databaseSaveLoadService;
		_accountController = accountController;
		_prefabFactory = prefabFactory;
		ManagerUI = managerUI;
		_garagePositionsConfig = garagePositionsConfig;
		_garageSelectorServiceDep = garageSelectorService;
		_freeDriverServiceDep = freeDriverService;
		_mapIconsModelDep = mapIconsModel;
		_mapIconsDataSoDep = mapIconsDataSo;
		_analyticEventFactory = analyticEventFactory;
		_playerStatusSystem = playerStatusSystem;
		_characterEquipmentDataService = characterEquipmentDataService;
		_charactersService = charactersService;
		_offlineModeSystem = offlineModeSystem;
		_carInputManager = carInputManager;
		_checkCarDataBase = checkCarDataBase;
		AssetBundleManager = assetBundleManager;
		SoundBankLoadService = soundBankLoadService;
		RainSoundState = rainSoundState;
		RainSound = rainSound;
	}

	public GameObject EnsurePlayerPerson(GameObject car)
	{
		int gender = _characterEquipmentDataService.Gender;
		if (FlagsStorage.IsMultiFree)
		{
			if (V.DriverPerson != null)
			{
				return V.DriverPerson;
			}
			int ownCarViewId = ((car != null && car.TryGetComponent<PhotonView>(out var component)) ? component.ViewID : 0);
			Vector3 position = ((car != null) ? car.transform.position : Vector3.zero);
			V.DriverPerson = _charactersService.CreateNetworkedPerson(ownCarViewId, _playerDataStorage, position);
			return V.DriverPerson;
		}
		if (car != null && car == LevelsProvider.RacingCarLevel)
		{
			GameObject obj = _charactersService.CreateLocalPerson(gender);
			obj.GetComponent<MainPerson>().IsRacingPerson = true;
			return obj;
		}
		if (V.DriverPerson == null)
		{
			V.DriverPerson = _charactersService.CreateLocalPerson(gender);
			if (V.DriverPerson.TryGetComponent<MainPerson>(out var component2))
			{
				component2.HideSkinUntilSeated = true;
			}
		}
		return V.DriverPerson;
	}

	public void SetupCarDriver(GameObject car, bool withPerson = true)
	{
		if ((bool)car && car.TryGetComponent<DriverController>(out var component))
		{
			GameObject person = (withPerson ? EnsurePlayerPerson(car) : null);
			component.SetupDriverWhenReady(person);
			if ((bool)MenuLevelParent && (bool)MenuLevelParent.ParkingMenuUI)
			{
				MenuLevelParent.ParkingMenuUI.SwitchTurnSignal();
			}
		}
	}

	private void Awake()
	{
		_garageController = _garageControllerDep?.Value;
		_cameraManager = _cameraManagerDep?.Value;
		_garageSelectorService = _garageSelectorServiceDep?.Value;
		_freeDriverService = _freeDriverServiceDep?.Value;
		MapIconsModel = _mapIconsModelDep?.Value;
		MapIconsDataSo = _mapIconsDataSoDep?.Value;
		_camera = GetComponent<Camera>();
		V.CoreGameManager = this;
		CPMGraphicsSettingsManager.Instance.SetGraphicsOnStart(_camera, this, GetComponent<LevelSceneControl>(), LevelsProvider);
		InitAwake();
		if (TryGetComponent<LevelScript>(out var component))
		{
			component.Init();
		}
	}

	private void Start()
	{
		_onlineCollectionsProvider.ResetAllCollections();
		if (SceneManager.GetActiveScene().buildIndex == 1)
		{
			V.SceneControl.LoadMainGarage(immediately: true);
			Audio.ResetAllVolume();
		}
		if (ScenesHelper.IsMenuSceneActive() || ScenesHelper.IsParkingLevelActive())
		{
			InitStart();
		}
		_cancellationTokenSource = new CancellationTokenSource();
	}

	private string GetVersionAndroid()
	{
		string[] array = SystemInfo.operatingSystem.Split(' ');
		if (array.Length >= 3)
		{
			return array[2];
		}
		return string.Empty;
	}

	public void CheckDeviceForShowUpdateAndroidWindow()
	{
	}

	public async UniTaskVoid UnloadResources(CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			await UniTask.DelayFrame(200, PlayerLoopTiming.Update, cancellationToken);
			ClearAssets();
		}
		catch (Exception)
		{
		}
	}

	public void ClearAssets()
	{
		if (Time.time - _lastUnloadTime < 2f)
		{
			return;
		}
		AsyncOperation asyncOperation = PlayerDatas.UnloadUnusedAssets("ClearAssets");
		if (asyncOperation != null)
		{
			asyncOperation.completed += (AsyncOperation _) =>
			{
				PlayerDatas.CollectGarbage("ClearAssets");
			};
			_lastUnloadTime = Time.time;
		}
	}

	public bool GetAvailableMemoryInGB(out float memory)
	{
		try
		{
			using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			using AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			using AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getSystemService", new object[1] { "activity" });
			AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
			androidJavaObject2.Call("getMemoryInfo", androidJavaObject3);
			float num = (float)androidJavaObject3.Get<long>("availMem") / 1.0737418E+09f;
			memory = num;
			return true;
		}
		catch
		{
			memory = 0f;
			return false;
		}
	}

	private void InitAwake()
	{
		V.PlayerPointLOD = base.transform;
		EasySaveDataHandler.Initialize();
		_carInfoService.ClearPool();
		_playerStorageService.Initialize();
		_carInputManager.OnLeftRightSteerReleased();
		if (SceneManager.GetActiveScene().buildIndex == 1)
		{
			_collectionsProvider.ResetRenderTextures();
		}
		if ((bool)globalPoint)
		{
			V.GlobalPoint = globalPoint.transform;
		}
		FlagsStorage.IsMainMenu = false;
		FlagsStorage.IsLevelScene = false;
		FlagsStorage.IsFreeDriveMode = false;
		if ((bool)V.SandSmoke)
		{
			sandSmoke = ParkParticlesAtGlobalPoint(UnityEngine.Object.Instantiate(V.SandSmoke).GetComponent<ParticleSystem>());
		}
		if ((bool)V.SparkPrefab)
		{
			Spark = UnityEngine.Object.Instantiate(V.SparkPrefab);
		}
		GameObject gameObject = (V.SkidSmoke ? V.SkidSmoke : Resources.Load<GameObject>("Other/SkidSmoke"));
		if ((bool)gameObject)
		{
			skidSmoke = ParkParticlesAtGlobalPoint(UnityEngine.Object.Instantiate(gameObject).GetComponent<ParticleSystem>());
		}
		CarOutInControl = base.gameObject.GetComponent<CarOutInControl>();
		if (!CarOutInControl)
		{
			CarOutInControl = _prefabFactory.InstantiateComponent<CarOutInControl>(base.gameObject);
		}
		CarOutInControl.LockCarButtonController = _lockCarButtonController;
		CarOutInControl.ControlItemsProvider = _controlItemsProvider;
		CarOutInControl.FreeDriveProvider = FreeDriveProvider;
		CarOutInControl.LevelsProvider = LevelsProvider;
		base.gameObject.AddComponent<PersonCameraControll>();
		if (!GetComponent<AnalyticWheres>())
		{
			base.gameObject.AddComponent<AnalyticWheres>();
		}
		Time.timeScale = 1f;
		SetValues();
		FlagsStorage.IsFreeDriveMode = false;
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			V.GlobalOffset = Vector3.zero;
			FlagsStorage.IsMultiFree = false;
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
			}
		}
		else
		{
			FlagsStorage.IsFreeDriveMode = true;
			_checkCarDataBase.Enabled = true;
			_playerStatusSystem.OnGameModePlayed();
			LevelsProvider.CurrentLevelData = ScriptableObject.CreateInstance<LevelDataScript>();
			LevelsProvider.CurrentLevelData.SceneForLoad = 13;
			LevelsProvider.CurrentLevelData.ID = 43;
		}
		AllCarNumber = _garagePositionsConfig.OrderedCarIds.Count - 1;
		if (ScenesHelper.IsMenuSceneActive())
		{
			FlagsStorage.KeepCityParts = true;
			_turnOn = false;
			FlagsStorage.IsMainMenu = true;
			V.GlobalOffset = Vector3.zero;
			TryParseGarage();
			FlagsStorage.IsLevelScene = false;
			FlagsStorage.MoveByTouch = true;
			FlagsStorage.KeepRow = false;
			V.RoomPassword = -1;
			V.CurrentCamIndex = 1;
			FlagsStorage.CreateForFriendOnly = false;
			LoadBoughtCars();
			VehicleStockTuningData[] array = Resources.LoadAll<VehicleStockTuningData>("Configs/CarTuningData/");
			CarOrder = (from tuningData in array
				where !tuningData.Nfs
				select new
				{
					carId = tuningData.Serial,
					Index = _garagePositionsConfig.OrderedCarIds.IndexOf(tuningData.Serial)
				} into x
				orderby (x.Index != -1) ? x.Index : int.MaxValue
				select x.carId).ToArray();
			CarIdStorage.SetNfcCarsList(array);
			SellCarToShowId = ObscuredPrefs.GetInt("lastBoughtId");
			SellCarToShowId = Mathf.Clamp(SellCarToShowId, 0, CarOrder.Length - 1);
			VehicleStockTuningData vehicleStockTuningData = array.First((VehicleStockTuningData x) => (int)x.Serial == (int)CarOrder[(int)SellCarToShowId]);
			if (!vehicleStockTuningData || (bool)vehicleStockTuningData.IsRewardCar)
			{
				PlayerPrefs.SetInt("lastBoughtId", 0);
				SellCarToShowId = 0;
			}
			FlagsStorage.CamFollowCar = true;
			FlagsStorage.SmoothFollow = true;
			FlagsStorage.MoveByTouch = true;
			FlagsStorage.FixedCameraAndLookAt = true;
			FlagsStorage.IsMultiFree = false;
			if ((bool)NewGameObject && (bool)mainMenuDesertPos)
			{
				NewGameObject.transform.position = mainMenuDesertPos.transform.position;
				NewGameObject.transform.eulerAngles = mainMenuDesertPos.transform.eulerAngles;
			}
			if (MenuLevelParent.IOSPlayer)
			{
				iPhoneSpeaker.SetSpeakerOnStart();
			}
		}
		else
		{
			FlagsStorage.IsMainMenu = false;
		}
		if (SceneManager.GetActiveScene().buildIndex == 13 || SceneManager.GetActiveScene().buildIndex == 1 || FlagsStorage.RacingGameMode)
		{
			FlagsStorage.IsLevelScene = false;
		}
		else
		{
			FlagsStorage.IsLevelScene = true;
		}
		_onlineCollectionsProvider.ResetAllCollections();
		_collectionsProvider.ResetMapIcons();
		UpdateSideCarTraffic();
		if (SceneManager.GetActiveScene().buildIndex == 1 && !FlagsStorage.LoginEventSend)
		{
			FlagsStorage.LoginEventSend = true;
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_analyticEventFactory.CreateEvent<LoginEvent, DefaultContext>(new DefaultContext()).Send();
			}).AddTo(this);
		}
		if (FlagsStorage.IsMultiFree)
		{
			_interestGroupManager.Initialize();
		}
		else
		{
			_interestGroupManager.Dispose();
		}
		WhooshSoundController = new WhooshSoundController();
		ParticleSystem ParkParticlesAtGlobalPoint(ParticleSystem particles)
		{
			if ((bool)particles && (bool)globalPoint)
			{
				particles.transform.SetParent(globalPoint.transform, worldPositionStays: false);
			}
			return particles;
		}
	}

	private void TryParseGarage()
	{
		for (int i = 0; i <= (int)CarIdStorage.MAX_CAR_ID; i++)
		{
			if (PlayerPrefs.GetInt($"GarageCar{i}", 0) == 1)
			{
				return;
			}
		}
		int num = 0;
		for (int j = 0; j <= (int)CarIdStorage.MAX_CAR_ID; j++)
		{
			if (ObscuredPrefs.GetInt($"car{j}") == 1)
			{
				PlayerPrefs.SetInt($"GarageCar{j}", 1);
				num++;
			}
			if (num == 5)
			{
				break;
			}
		}
	}

	private void EnableCamera()
	{
		if (!HasFirstCar())
		{
			if (!_slotService.HaveCarInAnyGarage && _offlineModeSystem.Disabled)
			{
				if (sellCarPos == null)
				{
					return;
				}
				OnVeryFirstStart().Forget();
			}
			else if ((bool)menuControl)
			{
				_freeDriverService?.CreateDriver();
			}
		}
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			TrySyncData();
		}).AddTo(this);
		if (AvailableCars.Length > 3 && PlayerPrefs.GetInt("offerSignIn", 0) == 0)
		{
			if (ManagerKeysForPrefs.Email == "0")
			{
				menuControl.ShowSignInRequiredWindow(2);
			}
			PlayerPrefs.SetInt("offerSignIn", 1);
		}
	}

	private void TrySyncData()
	{
		if (!InternetHelper.HasInternetConnection() || ManagerKeysForPrefs.AccountId == "0")
		{
			return;
		}
		if (!_accountController.Authorized)
		{
			if (++_syncDataRetryCount <= 60)
			{
				Debug.Log("Account not ready. Wait to sync with database");
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					TrySyncData();
				}).AddTo(this);
			}
		}
		else
		{
			_loadWalletFromDataBase = new LoadWalletFromDataBase(_currencyManagerController);
			_loadWalletFromDataBase.Load();
		}
	}

	private async UniTask SetupPersonFirstTime()
	{
		IsPlayerSetuping = true;
		if ((bool)V.DriverPerson)
		{
			UnityEngine.Object.Destroy(V.DriverPerson);
		}
		ManagerUI.ShowFirstGameWindow();
		await UniTask.WaitUntil(() => !IsPlayerSetuping);
	}

	public async UniTask OnVeryFirstStart()
	{
		if (PlayerPrefs.GetInt("PersonCustomizationCompleted", 0) != 1)
		{
			await SetupPersonFirstTime();
		}
		if (_isDestroyed)
		{
			return;
		}
		if ((bool)V.DriverPerson)
		{
			UnityEngine.Object.Destroy(V.DriverPerson);
		}
		V.DriverPerson = _prefabFactory.Instantiate(Resources.Load<GameObject>($"person{_characterEquipmentDataService.Gender}"));
		V.DriverPerson.transform.position = new Vector3(V.DriverPerson.transform.position.x, -100f, V.DriverPerson.transform.position.z);
		V.DriverPerson.GetComponent<TouchMovePerson>().enabled = false;
		V.CoreGameManager.OpenSellCars(useMiddleCameraChangeDirection: false);
		LoadSellCarModel().Forget();
		while (!V.CoreGameManager.SellCarModel)
		{
			await UniTask.Delay(100);
			if (_isDestroyed)
			{
				return;
			}
		}
		if ((bool)carImage)
		{
			carImage.SetActive(value: false);
		}
		ManagerUI.ShowUserBalanceWindow();
		ManagerUI.ShowNewCarsWindow();
		_cameraManager.firstCam.SetActive(value: false);
		ManagerUI.CloseBootLoadingWindow();
	}

	public void InitStart()
	{
		_enableCameraDisposable?.Dispose();
		_enableCameraDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			EnableCamera();
		}).AddTo(this);
		if (Application.platform == RuntimePlatform.Android)
		{
			OnStart();
		}
		if ((bool)globalPoint)
		{
			base.transform.parent = globalPoint.transform;
			V.GlobalPoint = globalPoint.transform;
		}
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			V.SceneLoaded();
		}
	}

	public void SendEventBoughCar(int carID)
	{
		Analytics.CustomEvent("Car Buy", new Dictionary<string, object> { 
		{
			"Car " + carID,
			1
		} });
	}

	public void SendEventLevelDone(float timeDone)
	{
		if ((bool)LevelsProvider.CurrentLevelData && SceneManager.GetActiveScene().buildIndex != 13 && SceneManager.GetActiveScene().buildIndex != 1)
		{
			Analytics.CustomEvent("Done " + 11, new Dictionary<string, object> { 
			{
				"Level " + LevelsProvider.CurrentLevelData.ID,
				timeDone
			} });
		}
	}

	public void SendEventLevelDoneInOneDay(int day, float overAll)
	{
		Analytics.CustomEvent("OneDay", new Dictionary<string, object> { 
		{
			"OneDay " + day,
			overAll
		} });
	}

	public void SendEventLevelCurrentQuitLevel()
	{
		if (LevelsProvider.CurrentLevelData != null && SceneManager.GetActiveScene().buildIndex != 13 && SceneManager.GetActiveScene().buildIndex != 1)
		{
			Analytics.CustomEvent("Quite", new Dictionary<string, object> { 
			{
				"From " + LevelsProvider.CurrentLevelData.ID,
				1
			} });
		}
	}

	public int GetArrayIndexInCarOreder(int carid)
	{
		int num = 0;
		ObscuredInt[] carOrder = CarOrder;
		for (int i = 0; i < carOrder.Length; i++)
		{
			if ((int)carOrder[i] == carid)
			{
				return num;
			}
			num++;
		}
		return 0;
	}

	public int GetArrayIndexCar(int carid)
	{
		int num = 0;
		for (int i = 0; i < AvailableCars.Length; i++)
		{
			if (AvailableCars[i] == carid)
			{
				return num;
			}
			num++;
		}
		return 0;
	}

	public void LoadBoughtCars()
	{
		ObscuredPrefs.SetInt("car" + 13, 1);
		int num = 0;
		int slotCountPrefs = _slotService.SlotCollection.SlotCountPrefs;
		for (int i = 0; i <= slotCountPrefs; i++)
		{
			if (ObscuredPrefs.GetInt("car" + i) == 1)
			{
				num++;
			}
		}
		AvailableCars = new int[num];
		if (ObscuredPrefs.GetInt("car" + ObscuredPrefs.GetInt("LastCarId", 13)) == 0)
		{
			ObscuredPrefs.SetInt("LastCarId", 13);
		}
	}

	public void SetLanguage(GameObject parent)
	{
		string text = parent.name;
		Debug.Log("TrySetLanguage: " + text);
	}

	private void SetValues()
	{
		GameObject gameObject = GameObject.Find("SecureKeysManager");
		if (!gameObject && (bool)SecureKeysManager)
		{
			UnityEngine.Object.Instantiate(SecureKeysManager, Vector3.zero, Quaternion.identity).name = "SecureKeysManager";
		}
		if ((bool)gameObject)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject.gameObject);
		}
		EndlessFuel = ObscuredPrefs.GetInt("EndlessFuel") == 1;
		FlagsStorage.EndlessCrashTime = ObscuredPrefs.GetInt("endlessCrashTime") == 1;
		PlayerPrefs.SetInt("CarDamage", 1);
		V.UnloadResourceCounter = 0;
		FlagsStorage.IsPassenger = false;
		FlagsStorage.RacingGameMode = false;
		FlagsStorage.RaceStarted = false;
		CarFinished = false;
		FlagsStorage.Car2Finished = false;
		_carInputManager.ClearPedals();
		FlagsStorage.LevelStarted = false;
		FlagsStorage.CamStaticMode = false;
		FlagsStorage.IsReadyMultRaceCar2 = false;
		if (ScenesHelper.IsMenuSceneActive())
		{
			FlagsStorage.IsMainMenu = true;
			FlagsStorage.InGarageFree = false;
			V.PlaceToSpawn = V.placeToSpawn.MainCity;
		}
		else
		{
			FlagsStorage.IsMainMenu = false;
		}
		carCameras = GetComponent<CarCameras>();
		carCamerasController = GetComponent<CarCamerasController>();
		menuControl = GetComponent<MenuControl>();
		SetAllPlayerData();
		Audio.LoadVolumeFromPrefs(setAllVolume: true);
		Audio.LoadAmbianceVolumeFromPrefs(setVolume: true);
		Audio.LoadCarVolumeFromPrefs(setVolume: true);
		Audio.LoadUIVolumeFromPrefs(setVolume: true);
		Audio.LoadSfxVolumeFromPrefs(setVolume: true);
		Audio.LoadTiresVolumeFromPrefs(setVolume: true);
		Audio.LoadMusicVolumeFromPrefs(setVolume: true);
		if ((bool)GarageNameLabel)
		{
			GarageNameLabel.text = base.name;
		}
		PlayerDatas.SetValues();
		Camera component = GetComponent<Camera>();
		V.SceneControl = GetComponent<SceneControl>();
		if (!FlagsStorage.IsMainMenu)
		{
			component.farClipPlane = ObscuredPrefs.GetFloat("farClipPlane", 250f);
		}
		if (Fog)
		{
			RenderSettings.fogEndDistance = component.farClipPlane;
			RenderSettings.fogStartDistance = component.farClipPlane - component.farClipPlane * 0.1f;
		}
		V.DetailDistance = ObscuredPrefs.GetFloat("detailDistance", V.DetailDistance);
		V.SWsensitivity = PlayerPrefs.GetFloat("SWsensitivity", 0.5f);
		if (SceneManager.GetActiveScene().buildIndex == 1)
		{
			FlagsStorage.OutOfCar = false;
		}
		SetSteering();
	}

	private void SetAllPlayerData()
	{
		string text = ObscuredPrefs.GetString("PlayerName");
		_playerStorageService.Name = text;
		if (string.IsNullOrEmpty(text))
		{
			text = PlayerStorageServiceExtension.GetRandomPlayerName();
			IAccountController accountController = _accountController;
			if (accountController != null && accountController.AccountType == AccountType.NewSystemGuest)
			{
				text = PlayerStorageServiceExtension.GetRandomGuestName();
			}
		}
		text = PlayerDatas.CheckNameValid(text);
		_playerDataStorage.SetBasicPlayerData(text, _playerStorageService.LocalID);
	}

	public void AddPostProcessing()
	{
		int defaultValue = 1;
		if (Application.platform == RuntimePlatform.IPhonePlayer && FlagsStorage.IsFreeDriveMode)
		{
			defaultValue = 0;
		}
		int num = PlayerPrefs.GetInt("TargetFrameRateMode", defaultValue);
		if (SystemInfo.batteryLevel < 20f && num >= 2 && SystemInfo.batteryStatus == BatteryStatus.Discharging)
		{
			num = 1;
		}
	}

	public void UpdateSideCarTraffic()
	{
		bool isActive = PlayerPrefs.GetInt("sideCarTraffic", 0) == 1;
		if ((bool)PeopleTraffic.Instance)
		{
			PeopleTraffic.Instance.UpdateSideCarTrafficState(isActive);
		}
	}

	public void SetSteering()
	{
		MenuLevelParent.SetSteering();
	}

	public void SetMessage(string message, float timeToShow, NotificationType type = NotificationType.Success, bool showBackground = true, float sparkFadeInDuration = -1f, float sparkFadeOutDuration = -1f)
	{
		ManagerUI.ShowNotificationWindow(message, timeToShow, type, 0u, null, withSign: false, showBackground, sparkFadeInDuration, sparkFadeOutDuration).Forget();
	}

	public void onCarStack()
	{
		OnCarStopped();
		_carInputManager.PressBrake();
		_carInputManager.TargetSpeed = 0f;
	}

	public void OnCarStopped()
	{
		CancelInvoke("onCarStack");
		if (!_turnOn)
		{
			_turnOn = true;
			if (V.MenuIndex != 24)
			{
				carCameras.distance = (V.Car.GetComponent<VehicleData>().isTruck ? 7 : 5);
				carCameras.distanceDamping = 100f;
				carCameras.xAngle = 151.9f;
				carCameras.yAngle = 13f;
				carCameras.rotationDamping = 0.2f;
				carCameras.height = 0.55f;
			}
			FlagsStorage.CamFollowCar = true;
			FlagsStorage.MoveByTouch = true;
			_setTargetAfterDisposable?.Dispose();
			_setTargetAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				SetTargetAfrter();
			}).AddTo(this);
			Audio.ResetAllVolume();
		}
	}

	public void onCamStopedMainMenu()
	{
		if (V.Car.GetComponent<Rigidbody>().velocity.magnitude > 0.5f)
		{
			_onCamStopedMainMenuDisposable?.Dispose();
			_onCamStopedMainMenuDisposable = Observable.Timer(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long _) =>
			{
				onCamStopedMainMenu();
			}).AddTo(this);
			return;
		}
		FlagsStorage.FixedCameraAndLookAt = false;
		if (FreeDriveProvider.CarCameras.Target == V.Car.transform)
		{
			FreeDriveProvider.CarCameras.targetMainMenu.transform.position = base.transform.position;
			base.transform.LookAt(V.Car.transform);
			Vector3 eulerAngles = base.transform.eulerAngles;
			FreeDriveProvider.CarCameras.xAngle = eulerAngles.y;
			FreeDriveProvider.CarCameras.y = eulerAngles.z;
			FreeDriveProvider.CarCameras.speedFollow = 0.5f;
			_makeSpeedFollowFasterDisposable?.Dispose();
			_makeSpeedFollowFasterDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				MakeSpeedFollowFaster();
			}).AddTo(this);
		}
	}

	private void MakeSpeedFollowFaster()
	{
		FreeDriveProvider.CarCameras.speedFollow = 2f;
	}

	public void SetTargetAfrter()
	{
		if (V.MenuIndex != 4 && V.MenuIndex != 24 && V.MenuIndex != 26)
		{
			carCameras.Target = V.Car.transform;
		}
		float currentDistance = Vector3.Distance(V.Car.transform.position, base.transform.position);
		carCameras.isDistanceDamping = true;
		carCameras.currentDistance = currentDistance;
	}

	public void FromSellCars()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && (bool)component.CarBodyReferences && (bool)component.CarBodyReferences.Body)
		{
			component.CarBodyReferences.Body.tag = "Car";
		}
		if ((bool)SellCarModel)
		{
			UnityEngine.Object.Destroy(SellCarModel);
		}
		CPMGraphicsSettingsManager.Instance.PerformFirstLaunchFixes();
	}

	public bool HasFirstCar()
	{
		SlotCollection slotCollection = _slotService.SlotCollection;
		int id = (V.SlotID = slotCollection.LastSlotId);
		if (slotCollection.IsSlotEmpty(id))
		{
			id = (V.SlotID = slotCollection.GetFirstNotEmptySlot());
			slotCollection.LastSlotId = id;
		}
		CarSlot carSlot = slotCollection[V.SlotID];
		if (carSlot != null)
		{
			return !carSlot.IsEmpty;
		}
		return false;
	}

	public bool HaveCarInAnyGarage()
	{
		return _slotService.HaveCarInAnyGarage;
	}

	private Transform GetSellPosition()
	{
		int currentGarageIndex = _garageSelectorService.CurrentGarageIndex;
		sellCarPos.SetLocalPositionAndRotation(_garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].SellCarPoint.Position, _garagePositionsConfig.GaragePositionsDatas[currentGarageIndex].SellCarPoint.Rotation);
		if (!_tempCamTarget)
		{
			_tempCamTarget = new GameObject("TempCamTarget").transform;
		}
		_tempCamTarget.position = sellCarPos.position + sellCarPos.up;
		return sellCarPos;
	}

	public void OpenSellCars(bool useMiddleCameraChangeDirection = true)
	{
		sellCarPos = GetSellPosition();
		if (!_cameraManager.firstCam.activeSelf)
		{
			_cameraManager.CustomMoveCameraToTarget.PlayCustomBlendToSellCar().Forget();
		}
		else
		{
			_cameraManager.ActivateCamera(RacingCameraType.BuyCar, _tempCamTarget, null, null, useFade: true, stopCustomMove: false);
		}
		_idSellCarModel = CarOrder[(int)SellCarToShowId];
		OnOpen?.Invoke(SellCarToShowId);
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && (bool)component.CarBodyReferences && (bool)component.CarBodyReferences.Body)
		{
			component.CarBodyReferences.Body.tag = "Untagged";
		}
	}

	private async UniTask CheckBodyLoad(int sellCarModelId)
	{
		if (!(_rr != null) || !(_rr2 != null))
		{
			return;
		}
		if (_mustStopLoad)
		{
			_mustStopLoad = false;
			return;
		}
		Vector3 position = sellCarPos.transform.position;
		Vector3 eulerAngles = sellCarPos.eulerAngles;
		position.y += 0.28f;
		SellCarModel = _prefabFactory.Instantiate(_rr);
		SellCarModel.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, eulerAngles.y, 0f));
		SellCarModel.SetActive(value: false);
		IsSellCarModelReady = false;
		carCameras.Target = SellCarModel.transform;
		_cameraManager.ChangeTarget(SellCarModel.transform);
		SellCarValues component = SellCarModel.GetComponent<SellCarValues>();
		if (_rr2 != null)
		{
			GameObject gameObject = (component.body = _prefabFactory.Instantiate(_rr2, SellCarModel.transform));
			gameObject.transform.SetLocalPositionAndRotation(component.pos, Quaternion.Euler(component.rot));
			component.OnMainBodyLoaded();
			CarBodyReferences component2 = gameObject.GetComponent<CarBodyReferences>();
			bool num = CarIdStorage.IsInteriorBakedIntoBody(sellCarModelId);
			if (num && (bool)component2 && (bool)component2.OldInterior)
			{
				component2.OldInterior.SetActive(active: false);
			}
			if (!num && (bool)component2 && CarIdStorage.LoadsBuiltInInteriorViaAddressables(sellCarModelId))
			{
				SteamPreviewInteriorLoader.LoadInto(gameObject, component2, sellCarModelId).Forget();
			}
			SetupSellCarBodyKits(component, sellCarModelId);
			if ((bool)component2 && (bool)component2.Diods && component2.Diods.TryGetComponent<MeshRenderer>(out var component3))
			{
				Shader shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
				if ((bool)shader)
				{
					component3.material.shader = shader;
				}
			}
			HDHeadlighsController componentInChildren = SellCarModel.GetComponentInChildren<HDHeadlighsController>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				componentInChildren.Init();
			}
			GameObject awaitedModel = SellCarModel;
			int num2;
			if (!_cameraManager.firstCam.activeSelf)
			{
				ICameraHandler currentCamera = _cameraManager.CurrentCamera;
				num2 = ((currentCamera != null && currentCamera.VirtualCameraType == RacingCameraType.GarageLookAround) ? 1 : 0);
			}
			else
			{
				num2 = 1;
			}
			bool switchToLookAround = (byte)num2 != 0;
			if ((bool)component && (bool)component.BuyCarBodyKits)
			{
				await component.BuyCarBodyKits.WaitForBodyKitsAsync(this.GetCancellationTokenOnDestroy());
			}
			if (!awaitedModel || awaitedModel != SellCarModel)
			{
				return;
			}
			IsSellCarModelReady = true;
			if ((bool)SellCarModel)
			{
				SellCarModel.SetActive(value: true);
			}
			if (switchToLookAround)
			{
				_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, V.CoreGameManager.SellCarModel.transform, null, null, useFade: true, stopCustomMove: false);
			}
			else
			{
				await UniTask.WaitUntil(() => !_cameraManager.CustomMoveCameraToTarget.Moving);
				if (_isDestroyed || ((bool)_cameraManager && _cameraManager.CustomMoveCameraToTarget != null && _cameraManager.CustomMoveCameraToTarget.CancellationTokenSource != null && _cameraManager.CustomMoveCameraToTarget.CancellationTokenSource.IsCancellationRequested))
				{
					return;
				}
			}
		}
		if ((bool)carImage)
		{
			carImage.SetActive(value: false);
		}
	}

	private void SetupSellCarBodyKits(SellCarValues sellCarValues, int sellCarModelId)
	{
		if ((bool)SellCarModel)
		{
			BuyCarBodyKits obj = Resources.Load("UI/BuyBodyKitsForNewCar", typeof(BuyCarBodyKits)) as BuyCarBodyKits;
			BuyCarBodyKits buyCarBodyKits = _prefabFactory.Instantiate(obj, SellCarModel.transform.position, SellCarModel.transform.rotation, SellCarModel.transform);
			if ((bool)sellCarValues)
			{
				sellCarValues.SetBuyBodyKits(buyCarBodyKits);
				buyCarBodyKits.transform.SetLocalPositionAndRotation(sellCarValues.pos, Quaternion.Euler(sellCarValues.rot));
			}
			buyCarBodyKits.Init(sellCarModelId, sellCarValues ? sellCarValues.BuyBodyKitsUIButtonPositionGetter : null);
		}
	}

	private async UniTask LoadAssetAsync(GameObject car, CancellationToken ct = default(CancellationToken))
	{
		ResourceRequest request = Resources.LoadAsync("car" + car.name.Split('(')[0], typeof(GameObject));
		while (!request.isDone)
		{
			await UniTask.WaitForFixedUpdate(ct);
		}
		GameObject carModel = (GameObject)request.asset;
		Dictionary<string, List<ObscuredInt>> prices = new Dictionary<string, List<ObscuredInt>>();
		if (carModel.GetComponent<BodyKits>() == null)
		{
			if (carImage != null)
			{
				carImage.SetActive(value: false);
			}
			return;
		}
		BodyKits component = carModel.GetComponent<BodyKits>();
		Dictionary<string, Pos> positions = new Dictionary<string, Pos>();
		Transform[] spoilers = component.spoilers;
		foreach (Transform transform in spoilers)
		{
			if (transform != null)
			{
				positions.Add(transform.name, new Pos(GetNum(transform.name), transform));
			}
		}
		spoilers = component.airIntakeRoof;
		foreach (Transform transform2 in spoilers)
		{
			if (transform2 != null)
			{
				positions.Add("r" + transform2.name, new Pos(GetNum(transform2.name), transform2));
			}
		}
		spoilers = component.airIntakesHood;
		foreach (Transform transform3 in spoilers)
		{
			if (transform3 != null)
			{
				positions.Add("h" + transform3.name, new Pos(GetNum(transform3.name), transform3));
			}
		}
		request = Resources.LoadAsync("UI/Canvas", typeof(GameObject));
		while (!request.isDone)
		{
			await UniTask.WaitForFixedUpdate(ct);
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(request.asset, Camera.main.transform.position, Camera.main.transform.rotation);
		Canvas canvas;
		try
		{
			gameObject.transform.SetParent(car.transform);
			canvas = gameObject.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.WorldSpace;
			RectTransform component2 = canvas.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(2f, 2f);
			canvas.worldCamera = Camera.main;
			component2.localRotation = Quaternion.Euler(Vector3.zero);
			component2.localPosition = Vector3.zero;
		}
		catch (Exception)
		{
			if (carImage != null)
			{
				carImage.SetActive(value: false);
			}
			return;
		}
		if (canvas == null)
		{
			if (carImage != null)
			{
				carImage.SetActive(value: false);
			}
			return;
		}
		request = Resources.LoadAsync("UI/Button", typeof(GameObject));
		while (!request.isDone)
		{
			await UniTask.WaitForFixedUpdate(ct);
		}
		List<KitPanel> panels = new List<KitPanel>();
		List<string> list = new List<string>();
		KitController kitController;
		try
		{
			foreach (Transform item in car.transform)
			{
				if (Contains(item.name, new string[8] { "spoiler", "fbumper", "rbumper", "skirt", "hood", "hair", "rair", "fender" }))
				{
					list.Add(item.name.Substring(0, 4));
				}
			}
			kitController = car.AddComponent<KitController>();
			kitController.Set(carModel, list, positions, prices, car.name);
		}
		catch (Exception)
		{
			if (carImage != null)
			{
				carImage.SetActive(value: false);
			}
			return;
		}
		try
		{
			foreach (Transform item2 in car.transform)
			{
				if (Contains(item2.name, new string[4] { "fbumper", "hood", "rbumper", "spoiler" }))
				{
					GameObject panel = UnityEngine.Object.Instantiate((GameObject)request.asset, canvas.transform, worldPositionStays: true);
					panel.name = item2.name;
					panels.Add(new KitPanel(panel));
					BoxCollider obj = item2.gameObject.AddComponent<BoxCollider>();
					GetVertex(item2.gameObject, ref panels);
					if (item2.name.Contains("hood"))
					{
						panel.transform.SetPositionAndRotation((panels.Last().Vertices[0] - panels.Last().Vertices[1]) / 2f + panels.Last().Vertices[1] + new Vector3(0.2f, 0.08f, -0.1f), Quaternion.identity);
					}
					else if (item2.name.Contains("rbumper"))
					{
						panel.transform.SetPositionAndRotation((panels.Last().Vertices[2] - panels.Last().Vertices[7]) / 2f + panels.Last().Vertices[7] + new Vector3(-0.2f, 0f, 0.1f), Quaternion.identity);
					}
					else if (item2.name.Contains("fbumper"))
					{
						panel.transform.SetPositionAndRotation((panels.Last().Vertices[0] - panels.Last().Vertices[5]) / 2f + panels.Last().Vertices[5] + new Vector3(0.2f, 0f, -0.1f), Quaternion.identity);
					}
					else
					{
						panel.transform.SetPositionAndRotation((panels.Last().Vertices[1] - panels.Last().Vertices[0]) / 2f + panels.Last().Vertices[0] + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
					}
					UnityEngine.Object.Destroy(obj);
					Button component3 = panel.transform.GetChild(0).GetComponent<Button>();
					Button component4 = panel.transform.GetChild(1).GetComponent<Button>();
					kitController.AddButton(component3);
					kitController.AddButton(component4);
					component3.onClick.AddListener(() =>
					{
						if (Input.touchCount <= 1)
						{
							kitController.Next(car.transform, panel.name.Substring(0, 4));
							ButtonPressed?.Invoke(1);
						}
					});
					component4.onClick.AddListener(() =>
					{
						if (Input.touchCount <= 1)
						{
							kitController.Prev(car.transform, panel.name.Substring(0, 4));
							ButtonPressed?.Invoke(1);
						}
					});
				}
				else
				{
					if (!Contains(item2.name, new string[2] { "fender", "skirt" }))
					{
						continue;
					}
					for (int num = 0; num < 2; num++)
					{
						GameObject panel2 = UnityEngine.Object.Instantiate((GameObject)request.asset, canvas.transform, worldPositionStays: true);
						panel2.name = item2.name;
						panels.Add(new KitPanel(panel2));
						BoxCollider obj2 = item2.gameObject.AddComponent<BoxCollider>();
						GetVertex(item2.gameObject, ref panels);
						if (num == 0)
						{
							panel2.transform.SetPositionAndRotation((panels.Last().Vertices[0] - panels.Last().Vertices[6]) / 2f + panels.Last().Vertices[6] + new Vector3(-0.2f, 0f, -0.1f), Quaternion.identity);
						}
						else
						{
							panel2.transform.SetPositionAndRotation((panels.Last().Vertices[1] - panels.Last().Vertices[5]) / 2f + panels.Last().Vertices[5] + new Vector3(0.2f, 0f, 0.1f), Quaternion.identity);
						}
						UnityEngine.Object.Destroy(obj2);
						Button component5 = panel2.transform.GetChild(0).GetComponent<Button>();
						Button component6 = panel2.transform.GetChild(1).GetComponent<Button>();
						kitController.AddButton(component5);
						kitController.AddButton(component6);
						component5.onClick.AddListener(() =>
						{
							if (Input.touchCount <= 1)
							{
								kitController.Next(car.transform, panel2.name.Substring(0, 4));
								ButtonPressed?.Invoke(1);
							}
						});
						component6.onClick.AddListener(() =>
						{
							if (Input.touchCount <= 1)
							{
								kitController.Prev(car.transform, panel2.name.Substring(0, 4));
								ButtonPressed?.Invoke(1);
							}
						});
					}
				}
			}
		}
		catch (Exception)
		{
			if (carImage != null)
			{
				carImage.SetActive(value: false);
			}
			return;
		}
		if (_lookAtCts != null)
		{
			_lookAtCts.Cancel();
			_lookAtCts.Dispose();
		}
		_lookAtCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		LookAtAsync(panels, _lookAtCts.Token).Forget();
		if (carImage != null)
		{
			carImage.SetActive(value: false);
		}
	}

	private int GetNum(string num)
	{
		string text = string.Empty;
		for (int i = 0; i < num.Length; i++)
		{
			if (char.IsDigit(num[i]))
			{
				text += num[i];
			}
		}
		if (text.Length <= 0)
		{
			return -1;
		}
		return int.Parse(text);
	}

	private async UniTask LookAtAsync(List<KitPanel> panels, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			_isRunning = true;
			while (true)
			{
				if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
				{
					foreach (KitPanel panel in panels)
					{
						try
						{
							RawImage image = panel.Panel.transform.GetComponent<RawImage>();
							Fade(ref image, FADE.OFF);
							RawImage image2 = panel.Panel.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
							Fade(ref image2, FADE.OFF);
							RawImage image3 = panel.Panel.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
							Fade(ref image3, FADE.OFF);
						}
						catch (Exception)
						{
							break;
						}
					}
				}
				else
				{
					foreach (KitPanel panel2 in panels)
					{
						try
						{
							RawImage image4 = panel2.Panel.transform.GetComponent<RawImage>();
							Fade(ref image4, FADE.ON);
							RawImage image5 = panel2.Panel.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
							Fade(ref image5, FADE.ON);
							RawImage image6 = panel2.Panel.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
							Fade(ref image6, FADE.ON);
						}
						catch (Exception)
						{
							break;
						}
					}
				}
				try
				{
					foreach (KitPanel panel3 in panels)
					{
						Vector3 worldPosition = 2f * panel3.Panel.transform.position - Camera.main.transform.position;
						panel3.Panel.transform.LookAt(worldPosition);
					}
				}
				catch (Exception)
				{
					break;
				}
				await UniTask.WaitForFixedUpdate(ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void Fade(ref RawImage image, FADE fade)
	{
		Color color = image.color;
		switch (fade)
		{
		case FADE.OFF:
			if (image.color.a > 0.01f)
			{
				image.color = new Color(color.r, color.g, color.b, color.a / (1f + 10f * Time.deltaTime));
			}
			break;
		case FADE.ON:
			if (color.a < 0.5f)
			{
				image.color = new Color(color.r, color.g, color.b, color.a * (1f + 20f * Time.deltaTime));
			}
			break;
		}
	}

	private bool Contains(string name, string[] strs)
	{
		foreach (string value in strs)
		{
			if (name.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	private void GetVertex(GameObject obj, ref List<KitPanel> panels)
	{
		BoxCollider component = obj.GetComponent<BoxCollider>();
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(component.size.x, component.size.y, component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(0f - component.size.x, component.size.y, 0f - component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(component.size.x, component.size.y, 0f - component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(0f - component.size.x, component.size.y, component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(component.size.x, 0f - component.size.y, component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(0f - component.size.x, 0f - component.size.y, component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(component.size.x, 0f - component.size.y, 0f - component.size.z) / 2f));
		panels.Last().Add(obj.transform.TransformPoint(component.center + new Vector3(0f - component.size.x, 0f - component.size.y, 0f - component.size.z) / 2f));
	}

	public void ChangeSellModel(ref Node node, Transform canvas, MODE mode, CREATE createMode)
	{
		if (_buying)
		{
			_buying = false;
		}
		else if (_idSellCarModel != (int)CarOrder[node.index] || mode == MODE.CHECK)
		{
			_idSellCarModel = CarOrder[node.index];
			SellCarToShowId = node.index;
		}
	}

	public void LoadSellCarFromOutside(int id, bool immediate = false)
	{
		_disposableLoadSellCarModel?.Dispose();
		_idSellCarModel = id;
		_mustStopLoad = false;
		float num = 0.5f;
		float num2 = Time.unscaledTime - _lastSellCarRequestTime;
		_lastSellCarRequestTime = Time.unscaledTime;
		num = ((num2 < 0.35f) ? 0.4f : 0.15f);
		float num3 = (immediate ? 0f : num);
		_disposableLoadSellCarModel = Observable.Timer(TimeSpan.FromSeconds(num3)).Subscribe((long _) =>
		{
			LoadSellCarModelSafe().Forget();
		}).AddTo(this);
	}

	private async UniTaskVoid LoadSellCarModelSafe()
	{
		if (_sellCarLoadInProgress)
		{
			_sellCarLoadQueued = true;
			return;
		}
		_sellCarLoadInProgress = true;
		try
		{
			do
			{
				_sellCarLoadQueued = false;
				await LoadSellCarModel();
			}
			while (_sellCarLoadQueued && (bool)this);
		}
		finally
		{
			_sellCarLoadInProgress = false;
		}
	}

	public async UniTask<bool> BuyCarById(SellCarValues sellCarValues, int validatedCarId)
	{
		_ = 1;
		try
		{
			await ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false);
			return await BuyThisVehicle(sellCarValues, validatedCarId);
		}
		catch (Exception message)
		{
			Debug.Log(message);
			return false;
		}
		finally
		{
			ManagerUIHelper.CloseWaitingWindow();
		}
	}

	public void ResetCarId()
	{
		_idSellCarModel = 59;
		SellCarToShowId = 59;
	}

	private async UniTask LoadSellCarModel()
	{
		try
		{
			int sellCarModelID = _idSellCarModel;
			if ((bool)SellCarModel)
			{
				if (SellCarModel.GetComponent<SellCarValues>().CarID == sellCarModelID)
				{
					_cameraManager.ChangeTarget(SellCarModel.transform);
					if ((bool)carImage)
					{
						carImage.SetActive(value: false);
					}
					return;
				}
				if (_isRunning)
				{
					if (_lookAtCts != null)
					{
						_lookAtCts.Cancel();
						_lookAtCts.Dispose();
						_lookAtCts = null;
					}
					_isRunning = false;
				}
				UnityEngine.Object.Destroy(SellCarModel);
			}
			if (_handleSellCar.HasValue && _handleSellCar.Value.IsValid())
			{
				Addressables.Release(_handleSellCar.Value);
			}
			if (_handleBody.HasValue && _handleBody.Value.IsValid())
			{
				Addressables.Release(_handleBody.Value);
			}
			FreeDriveProvider.CarCameras.distance = ((sellCarModelID == 70 || sellCarModelID == 170 || sellCarModelID == 171) ? 8 : 6);
			AsyncOperationHandle<GameObject> handleSellCar = Addressables.LoadAssetAsync<GameObject>($"sell model/{sellCarModelID}.prefab");
			AsyncOperationHandle<GameObject> handleBody = Addressables.LoadAssetAsync<GameObject>($"body/b{sellCarModelID}.prefab");
			_handleSellCar = handleSellCar;
			_handleBody = handleBody;
			await UniTask.WhenAll(handleSellCar.WithCancellation(this.GetCancellationTokenOnDestroy()), handleBody.WithCancellation(this.GetCancellationTokenOnDestroy()));
			if ((bool)this)
			{
				GameObject result = handleSellCar.Result;
				GameObject result2 = handleBody.Result;
				_rr = result;
				_rr2 = result2;
				if (_idSellCarModel == sellCarModelID)
				{
					CheckBodyLoad(sellCarModelID).Forget();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			dealerWindow.EClose(clear: true);
		}
	}

	public void HideShowWatchANdGEt()
	{
		SellCarRewardShowButton.SetActive((int)SellCarToShowId < 21);
	}

	public void SellCarNextCar()
	{
		OnChangeCarButtonClick?.Invoke(1);
		if ((int)SellCarToShowId > AllCarNumber)
		{
			SellCarToShowId = AllCarNumber;
		}
		else
		{
			_idSellCarModel = CarOrder[(int)SellCarToShowId];
		}
		HideShowWatchANdGEt();
	}

	public void SellCarPreviousCar()
	{
		OnChangeCarButtonClick?.Invoke(-1);
		if ((int)SellCarToShowId < 0)
		{
			SellCarToShowId = 0;
		}
		else
		{
			_idSellCarModel = CarOrder[(int)SellCarToShowId];
		}
		HideShowWatchANdGEt();
	}

	public void SetSWsensetivity()
	{
		V.SWsensitivity = PlayerPrefs.GetFloat("SWsensitivity", 1f);
		PlayerPrefs.SetFloat("SWsensitivity", V.SWsensitivity);
	}

	private async UniTask<bool> BuyThisVehicle(SellCarValues sellCarValues, int validatedCarId)
	{
		if (!_slotService.SlotCollection.TryGetEmptySlot(out var slot))
		{
			Debug.LogError("No slot");
			return false;
		}
		sellCarValues.CarID.Get(out int carID);
		if (carID != validatedCarId)
		{
			return false;
		}
		carID = validatedCarId;
		ObscuredPrefs.SetInt("lastBoughtId", carID);
		CombinedPrice price = sellCarValues.GetPrice();
		if (CarPriceProvider.IsRealCurrencyCar(validatedCarId) && !price.IsRealСurrency)
		{
			return false;
		}
		if (!CarPriceProvider.IsRealCurrencyCar(validatedCarId))
		{
			CarPriceProvider.Get(validatedCarId, out var _, out var isCoinPrice);
			isCoinPrice.Get(out var value);
			ObscuredInt obscuredInt = price.Money;
			if ((bool)sellCarValues.BuyCarBodyKits)
			{
				obscuredInt = (int)obscuredInt - (int)sellCarValues.BuyCarBodyKits.GetPrice().Money;
			}
			if (!(value ? ((int)obscuredInt <= 0) : ((int)obscuredInt > 0)))
			{
				return false;
			}
		}
		_moneyPrice = price.Money;
		ObscuredInt obscuredInt2 = Mathf.Abs((int)_coins + (int)price.Coin);
		if ((int)obscuredInt2 < 0)
		{
			obscuredInt2 = 0;
			ObscuredPrefs.DeleteAll();
			PlayerPrefs.DeleteAll();
		}
		AdvancedBoolean advancedBoolean = new AdvancedBoolean((int)obscuredInt2 <= 0) || _currencyManagerController.IsEnoughCoins(obscuredInt2, withMessage: true);
		AdvancedBoolean advancedBoolean2 = _currencyManagerController.IsEnoughMoney((int)(float)_moneyPrice, withMessage: true);
		if ((int)price.Money <= 0 && (int)price.Coin <= 0)
		{
			return false;
		}
		if (advancedBoolean2 && advancedBoolean)
		{
			if ((int)obscuredInt2 <= 0 && (float)_moneyPrice <= 0f)
			{
				Debug.LogError("Price is not correct. Don't buy this car.");
				return false;
			}
			bool withStyle = sellCarValues.indexStyle != 0;
			AdvancedInt moneyKitsCount = sellCarValues.BuyCarBodyKits.GetMoneyKitsCount();
			AdvancedInt coinsPrice = sellCarValues.BuyCarBodyKits.GetCoinsPrice();
			if (!InternetHelper.HasInternetConnection())
			{
				V.CoreGameManager.SetMessage(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, NotificationType.Error);
				return false;
			}
			CarBackendRequest data = _carBuyRequestFactory.CreateRequest(carID, withStyle, moneyKitsCount, coinsPrice, sellCarValues.BuyCarBodyKits.GetBodyKitsForBuyCars(), sellCarValues.indexStyle, slot.ID);
			(AdvancedBoolean, string) obj = await _buyCarBackendService.TryBuy(data);
			obj.Item1.Get(out var value2);
			string item = obj.Item2;
			if (string.IsNullOrEmpty(item) || (!item.Contains($"car{carID}_") && item != $"car{carID}"))
			{
				value2 = false;
			}
			if (value2)
			{
				_buying = true;
				BuyButtonPressed?.Invoke(1);
				LoadBoughtCars();
				V.SlotID = slot.ID;
				if (AddPurchasedCarToSlot(slot, carID, dealerWindow.RecordStyleForNewPurchasedCar, sellCarValues.BuyCarBodyKits ? new Action<CarInfo>(sellCarValues.BuyCarBodyKits.RecordBodyKitsForNewPurchasedCar) : null, item) == null)
				{
					_buying = false;
					BuyButtonPressed?.Invoke(0);
					SetMessage(new LocalizedString("DataBase.UnstableInternetConnectionTryR"), 3f, NotificationType.Error);
					return false;
				}
				FromSellCars();
				PlayerDatas.SetColor($"CarDirtColor{carID}", Color.white);
				PlayerPrefs.SetFloat($"CarDirtIntensity{carID}", 0f);
				AnalyticsHelper.SaveLastCarId(carID);
				ObscuredPrefs.Save();
				if (SlotService.OccupiedSlotCountInAllGarages == 1)
				{
					ManagerUI.ShowMainMenuWindow();
				}
				else
				{
					ManagerUI.ShowGarageWindow();
				}
				return true;
			}
		}
		BuyButtonPressed?.Invoke(0);
		_buying = false;
		return false;
	}

	public CarInfo AddPurchasedCarToSlot(CarSlot slot, int carId, Action<CarInfo> addVinylsCallback, Action<CarInfo> addBodyKitsCallback, string serverInstanceId)
	{
		if (!IsServerIssuedInstanceId(carId, serverInstanceId))
		{
			return null;
		}
		return AddNewCarToSlot(slot, carId, addVinylsCallback, addBodyKitsCallback, serverInstanceId);
	}

	public static bool IsServerIssuedInstanceId(int carId, string instanceId)
	{
		if (string.IsNullOrEmpty(instanceId))
		{
			return false;
		}
		foreach (char c in instanceId)
		{
			switch (c)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case '_':
				continue;
			}
			if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
			{
				return false;
			}
		}
		if (!(instanceId == $"car{carId}"))
		{
			return instanceId.StartsWith($"car{carId}_", StringComparison.Ordinal);
		}
		return true;
	}

	public CarInfo AddNewCarToSlot(CarSlot slot, int carId, Action<CarInfo> addVinylsCallback = null, Action<CarInfo> addBodyKitsCallback = null, string carInstanceID = "")
	{
		ObscuredPrefs.SetInt("car" + carId, 1);
		_slotService.GetSlotCollectionBySlotId(slot.ID).LastSlotId = slot.ID;
		CarInfo carInfo = _carInfoService.CreateDefault(carId, carInstanceID);
		if (CarIdStorage.LoadsBuiltInInteriorViaAddressables(carId))
		{
			carInfo.InstalledBodyKits.InteriorId = 1;
			if (!carInfo.BoughtBodyKits.Interiors.Contains(1))
			{
				carInfo.BoughtBodyKits.Interiors.Add(1);
			}
		}
		if (addBodyKitsCallback == null)
		{
			SeedForcedNonDefaultBodyKits(carInfo, carId);
		}
		carInfo.Save();
		addVinylsCallback?.Invoke(carInfo);
		addBodyKitsCallback?.Invoke(carInfo);
		_databaseCarsService.SaveCarInCacheToSend(carInfo, sendFullData: true);
		_databaseCarsService.ConfirmSendCache(carInfo.CarInstanceId, savePrefs: true);
		_slotService.SetSlot(slot.ID, carInfo.CarInstanceId);
		return carInfo;
	}

	private void SeedForcedNonDefaultBodyKits(CarInfo carInfo, int carId)
	{
		GameObject gameObject = Resources.Load<GameObject>($"car{carId}");
		if ((bool)gameObject && gameObject.TryGetComponent<BodyKits>(out var component))
		{
			SeedForcedKit(carInfo, BodyKitType.FrontBumper, component.BumperCountFront);
			SeedForcedKit(carInfo, BodyKitType.RearBumper, component.BumperCountRear);
			SeedForcedKit(carInfo, BodyKitType.Skirt, component.SkirtsCount);
			SeedForcedKit(carInfo, BodyKitType.Hood, component.HoodCount);
			SeedForcedKit(carInfo, BodyKitType.Fender, component.FenderCount);
		}
	}

	private static void SeedForcedKit(CarInfo carInfo, BodyKitType type, int count)
	{
		if (BodyKitForcedDefaults.ShouldForceNonDefault(type) && count > 1)
		{
			carInfo.InstalledBodyKits.GetInstalledBodyKitIndexRef(type) = 1;
			carInfo.BoughtBodyKits.AddBoughtBodyKits(type, 1);
		}
	}

	public void SetSteeringWheel()
	{
		V.Steering = 0;
		SetSteering();
	}

	public void SetSteeringTilt()
	{
		V.Steering = 1;
		SetSteering();
	}

	public void SetSteeringAnalog()
	{
		PlayerPrefs.SetInt("steering", 2);
		V.Steering = 2;
		SetSteering();
	}

	public void SetSteeringArrow()
	{
		V.Steering = 3;
		SetSteering();
	}

	public void UnloadCurrentSellCar()
	{
		_mustStopLoad = true;
		if ((bool)SellCarModel)
		{
			UnityEngine.Object.Destroy(SellCarModel);
		}
	}

	public void UpdateOverAllProgress()
	{
		int num = 0;
		for (int i = 0; i < 200; i++)
		{
			if (ObscuredPrefs.HasKey($"bestResultID{i}"))
			{
				num++;
			}
		}
	}

	public void MakeScreenShot()
	{
		_tempLightChangeValue = FlagsStorage.LightTransformChangeMode;
		_startMakeScreenShotDisposable?.Dispose();
		_startMakeScreenShotDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
		{
			StartMakeScreenShot();
		}).AddTo(this);
		MenuLevelParent.gameObject.SetActive(value: false);
		MenuLevelParent.screenshotManager.transform.parent = null;
		_carInputManager.EnableControl(stateInput: false, stateOutput: true);
		AudioWrapper.PostEvent(SoundEventsDataHelper.PlaySfxScreenShotSwitch, base.gameObject);
	}

	private void StartMakeScreenShot()
	{
		MenuLevelParent.screenshotManager.Capture();
		_afterScreenShotDisposable?.Dispose();
		_afterScreenShotDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
		{
			AfterScreenShot();
		}).AddTo(this);
	}

	private void AfterScreenShot()
	{
		FlagsStorage.LightTransformChangeMode = _tempLightChangeValue;
		MenuLevelParent.gameObject.SetActive(value: true);
		_carInputManager.EnableControl(stateInput: true, stateOutput: false);
		MenuLevelParent.screenshotManager.transform.parent = MenuLevelParent.gameObject.transform;
	}

	public void OnStart()
	{
	}

	private void OnStart2(HashGeneratorResult result)
	{
		if (result.Success)
		{
			_ = result.SummaryHash;
		}
	}

	private void OnDestroy()
	{
		if (_handleSellCar.HasValue && _handleSellCar.Value.IsValid())
		{
			Addressables.Release(_handleSellCar.Value);
		}
		if (_handleBody.HasValue && _handleBody.Value.IsValid())
		{
			Addressables.Release(_handleBody.Value);
		}
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			V.ResetOnSceneLoaded();
		}
		V.CoreGameManager = null;
		_isDestroyed = true;
		CancellationTokenSource cancellationTokenSource = _cancellationTokenSource;
		if (cancellationTokenSource != null && cancellationTokenSource.Token.CanBeCanceled)
		{
			_cancellationTokenSource.Cancel();
		}
		if (_lookAtCts != null)
		{
			_lookAtCts.Cancel();
			_lookAtCts.Dispose();
			_lookAtCts = null;
		}
		if (_loadAssetCts != null)
		{
			_loadAssetCts.Cancel();
			_loadAssetCts.Dispose();
			_loadAssetCts = null;
		}
		WhooshSoundController.Dispose();
	}

	public void OpenTaxi()
	{
		Service.Taxi.ServiceController obj = (Service.Taxi.ServiceController)Global.TaskManager.TaxiManager.serviceController;
		obj.SetActive(active: true);
		obj.CreateOrders();
	}

	public void Delivery()
	{
		Service.Delivery.ServiceController obj = (Service.Delivery.ServiceController)Global.TaskManager.DeliveryManager.serviceController;
		obj.SetActive(active: true);
		obj.CreateOrders();
	}

	public void Cargo()
	{
		Service.Cargo.ServiceController obj = (Service.Cargo.ServiceController)Global.TaskManager.CargoManager.serviceController;
		obj.SetActive(active: true);
		obj.CreateOrders();
	}

	public void CancelTask()
	{
		Global.TaskManager.Clear();
		PhotonView photonView = PhotonView.Get(Global.TaskManager);
		PhotonView photonView2 = PhotonView.Get(V.DriverPerson);
		if (PhotonNetwork.IsConnected)
		{
			photonView.RpcToDefaultGroup("ChangeState", RpcTarget.Others, photonView2.ViewID, (byte)2);
		}
		TaskManager taskManager = Global.TaskManager;
		Service.Taxi.ServiceController serviceController = (Service.Taxi.ServiceController)taskManager.TaxiManager.serviceController;
		Service.Delivery.ServiceController serviceController2 = (Service.Delivery.ServiceController)taskManager.DeliveryManager.serviceController;
		Service.Cargo.ServiceController obj = (Service.Cargo.ServiceController)taskManager.CargoManager.serviceController;
		serviceController?.CreateOrders();
		serviceController2?.CreateOrders();
		obj?.CreateOrders();
	}
}
