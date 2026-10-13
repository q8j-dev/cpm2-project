using System;
using System.Text.RegularExpressions;
using CPM.Connection;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CarAnimationScripts;
using Car_Parking.Scripts.Drag.CarCompositions;
using CodeStage.AntiCheat.Storage;
using Flags;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using Photon.Pun;
using Racing.CameraManagement;
using Service;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Car.Visual;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.AntiCheat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

public class VehicleData : MonoBehaviour, IBodyKitValuesParent
{
	public const int DistanceToEnableKinematic = 70;

	public const int VelocityYToEnableKinematic = 5;

	public const string RPC_CAR_DISABLE = "DisableCarRPC";

	public const string RPC_CAR_UPDATE_DIRT = "UpdateCarDirtRPC";

	public const string RPC_CAR_UPDATE_DIRT_INTENSITY = "UpdateCarDirtIntensityRPC";

	private readonly ExtraModuleController _extraModuleController = new ExtraModuleController();

	private bool _isVinylsLoaded;

	private float _speedInt;

	private IDisposable _speedSub;

	private SoundController _soundController;

	private int _slotID = -1;

	private Powertrain _powertrain;

	private Axles _axles;

	private Rigidbody _rigidbody;

	private BodyKits _bodyKits;

	private VehicleEntityProvider _vehicleEntityMonoCached;

	private ICarInfoService _carInfoService;

	private CarInfo _defaultCarInfo;

	private CarInfo _lastKnownCarInfo;

	private ColorsService _colorsService;

	private IPricesService _pricesService;

	private MirrorComposition _mirrorComposition;

	private CarInfoProvider _carInfoProvider;

	private VehicleMembersController _vehicleMembersController;

	private CarChildsReferences _carChildsReferences;

	private CarInfoContainer _carInfoContainer;

	private Coroutine _distanceCheckRoutine;

	private bool _isEngineOn;

	private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

	private VinylDrawer _vinylDrawer;

	private CurrencyManagerController _currencyManagerController;

	private IConnectionManager _connectionManager;

	private SoundBankLoadService _soundBankService;

	private DefaultMaterialsConfig _defaultMaterialsConfig;

	private IPrefabFactory _prefabFactory;

	private CarInputManager _carInputManager;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	private MapIconsModel _mapIconModel;

	private CameraManager _cameraManager;

	private WindshieldBrakeService _windshieldBrakeService;

	private OptionalDependency<PlayerDatas.CarLoadingUtility> _carLoadingUtilityDep;

	private OptionalDependency<MapIconsModel> _mapIconModelDep;

	private OptionalDependency<CameraManager> _cameraManagerDep;

	private OptionalDependency<WindshieldBrakeService> _windshieldBrakeServiceDep;

	public GameObject FrontMiddlePark;

	public GameObject RearMiddlePark;

	public int CarId;

	public int CarOwnerID = -1;

	public Vector3 MainModelPosition;

	public bool isTraffiCar;

	public bool isTruck;

	public bool RightHanded;

	public MainCarValues CarValues;

	public CameraDatas[] cameraDatas;

	public Material CarMat;

	public Material StockMaterial;

	public MultiPlayerPosControl mppc;

	public SpacerMove spacerMove1;

	public SpacerMove spacerMove2;

	public Collider[] towTruckRearCollider;

	public AllPersonPoints allPersonPoints;

	public Transform flagPoint;

	public Transform frontPlatePoint;

	public Transform rearPlatePoint;

	public float sizeWheelScale;

	public Vector2 WindowShatterHoleSize = Vector2.zero;

	public Vector2 WindowShatterHolePosition = Vector2.zero;

	public Vector4 WindowWipersTilingOffset = new Vector4(0f, -1.5f, 0.2f, 0.7f);

	public float WindowWipersScale = 1.5f;

	public bool WindowWipersVerticalInvert;

	public bool WindowWipersHorizontalInvert;

	public Vector3 jointPosition;

	public RacingState racingState;

	public float RoofPosition;

	public bool isGarageCar;

	public bool SpacerOpen;

	public bool sellingWorldCar;

	public int idForOnlineCarsArray;

	public IconMapView MapCarIcon;

	public string generatedID;

	public TriggerScriptTrafficCar trigerScriptTraffic;

	public PhotonView photonView;

	public GameObject Phone;

	public AdvancedBoolean IsTraded = new AdvancedBoolean(v: false);

	public bool IsBeingDestroyed { get; private set; }

	public SoundBankLoadService SoundBankService => _soundBankService;

	public DirtComposition DirtComposition { get; private set; }

	public LightsComposition LightsComposition { get; private set; }

	public WheelComposition WheelComposition { get; private set; }

	public VinylsComposition VinylsComposition { get; private set; }

	public InteriorsComposition InteriorsComposition { get; private set; }

	public ExhaustComposition ExhaustComposition { get; private set; }

	public BodyLoadingComposition BodyLoadingComposition { get; private set; }

	public PlatesComposition PlatesComposition { get; private set; }

	public WindowComposition WindowComposition { get; private set; }

	public ParkTronikCompositions ParkTronikComposition { get; private set; }

	public TrailerComposition TrailerComposition { get; private set; }

	public CarHealthComposition CarHealthComposition { get; private set; }

	public TowingComposition TowingComposition { get; private set; }

	public FlagConstructor FlagConstructor { get; } = new FlagConstructor();

	public bool IsFirstMatCreated { get; set; }

	public SellCarTriggerData СurrentSellCarTrigger { get; set; }

	public Collider Collider
	{
		get
		{
			if (!GetPowertrain)
			{
				return null;
			}
			return GetPowertrain.Collider;
		}
	}

	public bool Towing
	{
		get
		{
			if ((bool)GetPowertrain)
			{
				return GetPowertrain.TowCar;
			}
			return false;
		}
	}

	public bool resetingTowTrigger { get; set; }

	public bool CarOnLift { get; set; }

	public GameObject MainModel { get; set; }

	public MeshRenderer renderBody { get; set; }

	public PoliceCarController PoliceCarController { get; set; }

	public bool IsKitsLoaded
	{
		get
		{
			if ((bool)_bodyKits)
			{
				return _bodyKits.AllBodyKitsWasLoaded.Value;
			}
			return false;
		}
	}

	public float DistanceFromCamera { get; private set; }

	public CarMirrorController CarMirrorController { get; private set; }

	public CarInfoProvider CarInfoProvider => _carInfoProvider;

	public ColorsService ColorsService => _colorsService;

	public Vector3 CarFrontPosition { get; private set; }

	public CarAnimationReferences CarAnimationReferences { get; set; }

	public CarAnimationsController CarAnimationsController { get; set; }

	public FreeDriveProvider FreeDriveProvider { get; private set; }

	public LevelsProvider LevelsProvider { get; set; }

	public RacingProvider RacingProvider { get; set; }

	public GameObject Driver => VehicleMembersController.Driver;

	public Material CarMaterial => CarMat;

	public BodyKits BodyKits => _bodyKits;

	public bool IsMultiplayerAndMine
	{
		get
		{
			if (FlagsStorage.IsMultiFree && (bool)photonView)
			{
				return photonView.IsMine;
			}
			return false;
		}
	}

	public bool IsCarGraphicLoaded { get; set; }

	public bool IsInitialized { get; private set; }

	public ExtraModuleController ExtraModuleController => _extraModuleController;

	public DefaultMaterialsConfig DefaultMaterialsConfig => _defaultMaterialsConfig;

	public CarInputManager GetCarInputManager => _carInputManager;

	public KeepRowComposition KeepRowComposition
	{
		get
		{
			if (!_carInputManager)
			{
				return null;
			}
			return _carInputManager.KeepRowComposition;
		}
	}

	public Axles Axles => _axles;

	public bool BlockSellThisCar { get; set; }

	public bool CarLocked
	{
		get
		{
			return VehicleMembersController?.IsLocked ?? false;
		}
		set
		{
			VehicleMembersController?.SetLocked(value);
		}
	}

	public int slotId
	{
		get
		{
			return _slotID;
		}
		set
		{
			_slotID = value;
			_carInfoProvider.OnSlotIdUpdated();
		}
	}

	public CarInfo carInfo
	{
		get
		{
			if (_carInfoService == null)
			{
				return null;
			}
			IsTraded.Get(out var value);
			if (value || slotId == -1)
			{
				return _defaultCarInfo ?? (_defaultCarInfo = _carInfoService.CreateDefault(CarId));
			}
			if (_carInfoService.TryGetFromSlot(slotId, out var lastKnownCarInfo))
			{
				_lastKnownCarInfo = lastKnownCarInfo;
			}
			return _lastKnownCarInfo;
		}
	}

	public VehicleMembersController VehicleMembersController => _vehicleMembersController ?? (_vehicleMembersController = new VehicleMembersController(this));

	public VehicleEntityProvider GetVehicleEntityMono => _vehicleEntityMonoCached;

	public bool IsRacingCar { get; private set; }

	public CarChildsReferences CarChildsReferences
	{
		get
		{
			if (!_carChildsReferences)
			{
				TryGetComponent<CarChildsReferences>(out _carChildsReferences);
			}
			return _carChildsReferences;
		}
	}

	public CarBodyReferences CarBodyReferences { get; set; }

	public bool IsMineCar
	{
		get
		{
			if (FreeDriveProvider != null && base.gameObject == FreeDriveProvider.FirstCarMultiPlayer)
			{
				return true;
			}
			if (LevelsProvider != null && (bool)LevelsProvider.LevelScript && base.gameObject == LevelsProvider.LevelScript.FirstCarLevel)
			{
				return true;
			}
			return false;
		}
	}

	public Powertrain GetPowertrain
	{
		get
		{
			if (!_powertrain)
			{
				TryGetComponent<Powertrain>(out _powertrain);
			}
			return _powertrain;
		}
	}

	public float GroundOffset
	{
		get
		{
			if (!_axles || _axles.AllWheels.Count <= 0 || !_axles.AllWheels[0] || !_axles.AllWheels[0].WheelModel)
			{
				return 0.4f;
			}
			return 0f - base.transform.InverseTransformPoint(_axles.AllWheels[0].WheelModel.transform.position).y + _axles.AllWheels[0].Wheel.Radius;
		}
	}

	public bool IsMeInCar
	{
		get
		{
			if (FlagsStorage.OutOfCar)
			{
				return false;
			}
			if (!PhotonNetwork.IsConnected)
			{
				return true;
			}
			return V.Car == base.gameObject;
		}
	}

	private MapIconsModel MapIconsModel
	{
		get
		{
			if (_mapIconModel != null)
			{
				return _mapIconModel;
			}
			if ((bool)V.CoreGameManager)
			{
				return V.CoreGameManager.MapIconsModel;
			}
			return null;
		}
	}

	Transform IBodyKitValuesParent.transform => base.transform;

	public event Action<Color> CarColorChanged;

	public event Action OnCarGraphicLoaded;

	public event Action OnInitialized;

	public void SetupAsRacingCar()
	{
		if (!IsRacingCar)
		{
			IsRacingCar = true;
			GetPowertrain.IsRaceCar = true;
			ExhaustComposition?.RespawnParticles();
			VinylsComposition?.TryLoadRacingPreset(CarId);
		}
	}

	[Inject]
	public void Construct(PlayerDatas.VinylsDataUtility vinylsDataUtility, VinylDrawer vinylDrawer, FlagsDataSO flagsDataSo, ICarInfoService carInfoService, CarFactoryService carFactoryService, CurrencyManagerController currencyManagerController, ColorsService colorsService, IConnectionManager connectionManager, DefaultMaterialsConfig defaultMaterialsConfig, ManagerUI managerUI, IPricesService pricesService, MirrorComposition mirrorComposition, IPrefabFactory prefabFactory, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, RacingProvider racingProvider, CarInputManager carInputManager, OptionalDependency<MapIconsModel> mapIconModel, OptionalDependency<PlayerDatas.CarLoadingUtility> carLoadingUtility, OptionalDependency<WindshieldBrakeService> windshieldBrakeService, OptionalDependency<CameraManager> cameraManager, SoundBankLoadService soundBankService)
	{
		_soundBankService = soundBankService;
		_prefabFactory = prefabFactory;
		RacingProvider = racingProvider;
		LevelsProvider = levelsProvider;
		FreeDriveProvider = freeDriveProvider;
		_mirrorComposition = mirrorComposition;
		_pricesService = pricesService;
		_defaultMaterialsConfig = defaultMaterialsConfig;
		_colorsService = colorsService;
		_carInfoService = carInfoService;
		_vinylDrawer = vinylDrawer;
		_vinylsDataUtility = vinylsDataUtility;
		_currencyManagerController = currencyManagerController;
		_connectionManager = connectionManager;
		_carInfoProvider = new CarInfoProvider(this);
		_carInputManager = carInputManager;
		PoliceCarController = prefabFactory.Instantiate(Resources.Load<PoliceCarController>("PoliceSystem/PoliceCar"), base.transform);
		_windshieldBrakeServiceDep = windshieldBrakeService;
		_carLoadingUtilityDep = carLoadingUtility;
		_mapIconModelDep = mapIconModel;
		_cameraManagerDep = cameraManager;
	}

	private void Awake()
	{
		_carLoadingUtility = _carLoadingUtilityDep?.Value;
		_mapIconModel = _mapIconModelDep?.Value;
		_windshieldBrakeService = _windshieldBrakeServiceDep?.Value;
		_cameraManager = _cameraManagerDep?.Value;
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<SoundController>(out _soundController);
		TryGetComponent<MultiPlayerPosControl>(out mppc);
		TryGetComponent<Rigidbody>(out _rigidbody);
		if ((bool)_rigidbody)
		{
			_rigidbody.maxDepenetrationVelocity = 3f;
		}
		TryGetComponent<BodyKits>(out _bodyKits);
		TryGetComponent<Axles>(out _axles);
		TryGetComponent<PhotonView>(out photonView);
		TryGetComponent<VehicleEntityProvider>(out _vehicleEntityMonoCached);
		InitCompositions();
		if ((bool)PoliceCarController)
		{
			PoliceCarController.Init(this);
		}
		if ((bool)allPersonPoints)
		{
			VehicleMembersController.Init(allPersonPoints.allTargets.Length);
		}
		if (WheelComposition != null)
		{
			WheelComposition.TryInitWheelComposition((bool)_powertrain && !_powertrain.OnLineCar);
		}
		TryInitChildren();
		TryInitCarId();
		TryDestroyCharacterJoint();
		base.gameObject.AddComponent<JointHandler>();
		WindowComposition.SubscribeToGraphicLoaded();
		_extraModuleController.Init(this);
		TrySetLayerAndTegCarCollider();
		SetRoofPosition();
		SetCarFrontPosition();
	}

	private void Start()
	{
		base.gameObject.AddComponent<CarDebugTools>();
		LightsComposition?.TryFindJolPolLights();
		TryStartDistanceCheckObservable();
		CreateTriggerMeshCollider();
		CarChildsReferences.FuelTank?.Init(_powertrain);
		MapIconsModel?.RegisterCar(this);
		ParkTronikComposition?.Init();
		_carLoadingUtility?.TrySetUnUsedCar(this);
		TrailerComposition?.Init();
		LightsComposition?.TryDisableLight();
		SetGeneratedID();
		WiseHelper.SetAudioWiseComponents(base.gameObject, !ScenesHelper.IsParkingLevelActive());
		OnCarGraphicLoaded += InteriorsComposition.TryRestoreSavedInterior;
		IsInitialized = true;
		OnInitialized?.Invoke();
	}

	public void InvokeOnCarGraphicLoaded()
	{
		OnCarGraphicLoaded?.Invoke();
	}

	private void InitCompositions()
	{
		if (DirtComposition == null)
		{
			DirtComposition dirtComposition = (DirtComposition = new DirtComposition(this));
		}
		if (LightsComposition == null)
		{
			LightsComposition lightsComposition = (LightsComposition = new LightsComposition(this));
		}
		if (WheelComposition == null)
		{
			WheelComposition wheelComposition = (WheelComposition = new WheelComposition(this, _currencyManagerController, FreeDriveProvider));
		}
		if (VinylsComposition == null)
		{
			VinylsComposition vinylsComposition = (VinylsComposition = new VinylsComposition(this, _vinylDrawer, _vinylsDataUtility));
		}
		if (InteriorsComposition == null)
		{
			InteriorsComposition interiorsComposition = (InteriorsComposition = new InteriorsComposition(this, _connectionManager, _defaultMaterialsConfig, FreeDriveProvider, _cameraManager, _soundBankService));
		}
		if (ExhaustComposition == null)
		{
			ExhaustComposition exhaustComposition = (ExhaustComposition = new ExhaustComposition(this));
		}
		if (BodyLoadingComposition == null)
		{
			BodyLoadingComposition bodyLoadingComposition = (BodyLoadingComposition = new BodyLoadingComposition(this));
		}
		if (PlatesComposition == null)
		{
			PlatesComposition platesComposition = (PlatesComposition = new PlatesComposition(this, frontPlatePoint, rearPlatePoint));
		}
		if (WindowComposition == null)
		{
			WindowComposition windowComposition = (WindowComposition = new WindowComposition(this, _colorsService, _windshieldBrakeService));
		}
		CarMirrorController = (isTraffiCar ? null : new CarMirrorController(this, _mirrorComposition));
		ParkTronikComposition = new ParkTronikCompositions(this, _powertrain, _prefabFactory);
		TrailerComposition = new TrailerComposition(this);
		CarHealthComposition = new CarHealthComposition(this);
		if (TowingComposition == null)
		{
			TowingComposition towingComposition = (TowingComposition = new TowingComposition(this));
		}
		if (_vehicleMembersController == null)
		{
			_vehicleMembersController = new VehicleMembersController(this);
		}
	}

	public void StopCar()
	{
		Powertrain component = V.Car.GetComponent<Powertrain>();
		_carInputManager.PressThrottle(0f);
		_carInputManager.TargetSpeed = 0f;
		_carInputManager.PressBrake(0.6f);
		component.Velocity /= 3f;
	}

	private void OnEnable()
	{
		DirtComposition?.TrySetDirtUpdate(isActive: true);
		CPMGraphicsSettingsManager.Instance.OnQualityChanged += ExhaustComposition.OnQualityChanged;
	}

	private void OnDisable()
	{
		DirtComposition?.TrySetDirtUpdate(isActive: false);
		CPMGraphicsSettingsManager.Instance.OnQualityChanged -= ExhaustComposition.OnQualityChanged;
	}

	private void DistanceCheck()
	{
		if ((bool)V.CoreGameManager)
		{
			DistanceFromCamera = (V.CoreGameManager.transform.position - base.transform.position).magnitude;
			_rigidbody.isKinematic = Mathf.Abs(_rigidbody.velocity.y) < 5f && FlagsStorage.OutOfCar && DistanceFromCamera > 70f;
		}
	}

	private void OnDestroy()
	{
		IsBeingDestroyed = true;
		_extraModuleController?.Dispose();
		VehicleMembersController?.Dispose();
		VinylsComposition?.DestroyAllRenderTextures();
		if (IsFirstMatCreated)
		{
			CommonExtensions.DestroyMat(ref CarMat);
		}
		WindowComposition?.DestroyMat();
		WheelComposition?.Dispose();
		PlatesComposition?.DestroyMats();
		LightsComposition?.Dispose();
		CarMirrorController?.Dispose();
		MapIconsModel?.UnregisterCar(this);
		BodyLoadingComposition?.Dispose();
		InteriorsComposition?.Dispose();
		if (InteriorsComposition != null)
		{
			OnCarGraphicLoaded -= InteriorsComposition.TryRestoreSavedInterior;
		}
		mppc = null;
		if ((bool)CarValues)
		{
			CarValues.car = null;
			CarValues.driver = null;
			if (CarValues.RuntimeInstance)
			{
				UnityEngine.Object.Destroy(CarValues);
			}
		}
	}

	public void OnCarColorChanged(Color color)
	{
		CarColorChanged?.Invoke(color);
	}

	private void SetGeneratedID()
	{
		if (!(V.Car != base.gameObject))
		{
			string text = ObscuredPrefs.GetString($"carID{CarId}");
			if (text == "")
			{
				text = ObsoletePlayerDatas.CreateNewCarID(CarId);
				ObscuredPrefs.SetString($"carID{CarId}", text);
			}
			generatedID = text;
		}
	}

	public void SetEngineState(bool toStart)
	{
		if (toStart)
		{
			GetPowertrain.StartEngine(starterSound: false);
		}
		else
		{
			GetPowertrain.ShutdownEngine();
		}
	}

	private void TryStartDistanceCheckObservable()
	{
		if (!isTraffiCar && ((FlagsStorage.IsLevelScene && V.Car == base.gameObject) || (FlagsStorage.IsFreeDriveMode && !FlagsStorage.InGarageFree && !FlagsStorage.IsMultiFree && !PhotonNetwork.IsConnected)))
		{
			Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				DistanceCheck();
			}).AddTo(this);
		}
	}

	private void TryInitCarId()
	{
		if (!isTraffiCar)
		{
			string s = Regex.Replace(base.gameObject.name, "[^0-9]", "");
			CarId = int.Parse(s);
		}
	}

	private void TrySetLayerAndTegCarCollider()
	{
		if (!isTraffiCar && (bool)Collider)
		{
			Collider.transform.tag = "Car";
			Collider.gameObject.layer = LayerViaName.CarLayer;
		}
	}

	private void SetRoofPosition()
	{
		if ((bool)Collider)
		{
			Vector3 max = Collider.bounds.max;
			RoofPosition = base.transform.InverseTransformPoint(max).y;
		}
	}

	private void SetCarFrontPosition()
	{
		if ((bool)Collider)
		{
			CarFrontPosition = base.transform.InverseTransformPoint(Collider.bounds.center + base.transform.forward * Collider.bounds.extents.z);
		}
	}

	private void TryDestroyCharacterJoint()
	{
		if (FlagsStorage.IsFreeDriveMode && TryGetComponent<CharacterJoint>(out var component))
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private void TryInitChildren()
	{
		if ((bool)CarChildsReferences && !isTraffiCar)
		{
			CarMirrorController.InitCameras();
			if (!CarChildsReferences.Exhaust.IsNullOrEmpty())
			{
				ExhaustComposition.Install();
			}
			if (V.Car != base.gameObject)
			{
				CarMirrorController.SetMirror(val: false);
			}
			if (!CarChildsReferences.FuelSlot)
			{
				GameObject obj = new GameObject("fuel slot");
				obj.transform.parent = base.gameObject.transform;
				obj.transform.localPosition = new Vector3(1.5f, 1f, -2f);
				GameObject gameObject = obj;
				CarChildsReferences.FuelSlot = gameObject.transform;
			}
		}
	}

	public void SwitchOnOffAdditionalObjects(bool isActive)
	{
		if ((bool)BodyLoadingComposition.CarRetractableDisplay)
		{
			BodyLoadingComposition.CarRetractableDisplay.SetActive(isActive);
			BodyLoadingComposition.CarRetractableDisplay.SwitchOnOffCarDisplayAnimation(_isEngineOn);
		}
	}

	public void EnableCarDisplay(bool isEngineOn)
	{
		_isEngineOn = isEngineOn;
		_extraModuleController.EnableCarDisplay(isEngineOn);
		if ((bool)BodyLoadingComposition.CarRetractableDisplay)
		{
			BodyLoadingComposition.CarRetractableDisplay.SwitchOnOffCarDisplayAnimation(_isEngineOn);
		}
	}

	public void CleanupMapIcon()
	{
		MapIconsModel?.UnregisterCar(this);
	}

	public void ResetTowTruckTrigger()
	{
		resetingTowTrigger = false;
	}

	private void CreateTriggerMeshCollider()
	{
		if ((bool)Collider)
		{
			Collider collider = UnityEngine.Object.Instantiate(Collider, Collider.transform.parent);
			collider.name = "Trigger Mesh Collider For Grass Sounds";
			collider.isTrigger = true;
			collider.tag = "Untagged";
			collider.gameObject.layer = LayerViaName.ParktronicLayer;
			collider.gameObject.AddComponent<SoundObjectTrigger>().Init(_soundController, GetPowertrain, LevelsProvider);
		}
	}

	public void OnDoorStateChanged(bool isDoorOpen)
	{
		if ((bool)PoliceCarController.BodyKitsUpdater)
		{
			PoliceCarController.BodyKitsUpdater.ToggleTopLights(!isDoorOpen);
			if (PhotonNetwork.IsConnected && photonView.ViewID != 0)
			{
				photonView.RpcToDefaultGroup("OnDoorStateChangedRPC", RpcTarget.All, isDoorOpen);
			}
		}
	}

	[PunRPC]
	private void DisableCarRPC(bool isActive)
	{
		mppc.forceDisableCar = !isActive;
		isGarageCar = true;
	}

	[PunRPC]
	private void UpdateCarDirtRPC(int combined, PhotonMessageInfo info)
	{
		var (dirtInt, value) = DirtComposition.SplitIntensityAndColor(combined);
		DirtComposition.UpdateDirtMultiplayer(dirtInt, info.photonView.ViewID, value).Forget();
	}

	[PunRPC]
	private void UpdateCarDirtIntensityRPC(byte dirtInt, PhotonMessageInfo info)
	{
		DirtComposition.UpdateDirtMultiplayer((float)(int)dirtInt * 0.01f, info.photonView.ViewID).Forget();
	}

	[PunRPC]
	private void OnDoorStateChangedRPC(bool isDoorOpen)
	{
		if ((bool)photonView && photonView.ViewID != 0 && PhotonNetwork.CurrentRoom != null && !photonView.IsMine && photonView.OwnerActorNr == CarOwnerID && (bool)PoliceCarController.BodyKitsUpdater)
		{
			PoliceCarController.BodyKitsUpdater.ToggleTopLights(!isDoorOpen);
		}
	}

	public void StopCarCompletely(float timeToGainControlBack)
	{
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;
		_rigidbody.isKinematic = true;
		Observable.Timer(TimeSpan.FromSeconds(timeToGainControlBack)).Subscribe((long _) =>
		{
			_rigidbody.isKinematic = false;
		}).AddTo(base.gameObject);
	}

	public IPricesService GetPricesService()
	{
		return _pricesService;
	}

	public void SetCam()
	{
		if (FlagsStorage.RacingGameMode)
		{
			return;
		}
		if (GetPowertrain.Gear == -1)
		{
			if (!FlagsStorage.UpdatedToReverse && V.CurrentCamIndex < 3 && V.CurrentCamIndex != 0 && !FlagsStorage.IsFreeDriveMode)
			{
				V.PrevIndexOfCam = V.CurrentCamIndex;
				if ((bool)LevelsProvider.LevelScript)
				{
					LevelsProvider.LevelScript.carCamerasController.SetCamera(3, V.Car.transform, newTarget: true);
				}
				V.CurrentCamIndex = 3;
				FlagsStorage.ChangeCamAuto = true;
				FlagsStorage.UpdatedToReverse = true;
			}
			if ((FlagsStorage.ShowCarMirror || CPMGraphicsSettingsManager.Instance.Quality > 0) && V.CurrentCamIndex == 0)
			{
				CarMirrorController.ShowRearCamDisplay();
			}
		}
		else
		{
			FlagsStorage.UpdatedToReverse = false;
			if (FlagsStorage.ChangeCamAuto && (bool)LevelsProvider.LevelScript && (bool)LevelsProvider.LevelScript.carCamerasController)
			{
				LevelsProvider.LevelScript.carCamerasController.SetCamera(V.PrevIndexOfCam, V.Car.transform, newTarget: false);
				V.CurrentCamIndex = V.PrevIndexOfCam;
				FlagsStorage.ChangeCamAuto = false;
			}
			CarMirrorController.HideRearCamDisplay();
		}
	}

	public void TryResetCarOwner()
	{
		if ((bool)photonView)
		{
			photonView.TransferOwnership(CarOwnerID);
		}
	}
}
