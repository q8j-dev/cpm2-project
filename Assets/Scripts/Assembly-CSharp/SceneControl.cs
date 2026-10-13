using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Scripts.Garage;
using Car_Parking.Scripts.Racing;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using GPS;
using Obi;
using Racing.CameraManagement;
using Traffic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Steam.Perf;
using _Car_Parking.Scripts.Steam.World;

public class SceneControl : MonoBehaviour
{
	[Serializable]
	public class ScenePartSettings
	{
		public float EndDistance = 350f;

		public float StartDistance = 300f;

		public float density = 0.2f;

		public bool nightMode;
	}

	[Serializable]
	public class WaypointData
	{
		public int[] currentTarget;

		public int[] sign;

		public int[] directions;
	}

	[Serializable]
	public class oneEnvironmentColliders
	{
		public Collider[] allColliders;

		public PolygonZone[] PolygonZones;

		public void BakePolygonZonesFromColliders()
		{
			if (allColliders == null)
			{
				PolygonZones = null;
				return;
			}
			List<PolygonZone> list = new List<PolygonZone>(allColliders.Length);
			Collider[] array = allColliders;
			foreach (Collider collider in array)
			{
				if ((bool)collider)
				{
					PolygonZone item = PolygonZone.FromCollider(collider);
					if (item.points != null && item.points.Length >= 3)
					{
						list.Add(item);
					}
				}
			}
			PolygonZones = list.ToArray();
		}

		public float SqrDistanceTo(Vector3 point)
		{
			if (PolygonZones == null)
			{
				return float.MaxValue;
			}
			float num = float.MaxValue;
			PolygonZone[] polygonZones = PolygonZones;
			foreach (PolygonZone polygonZone in polygonZones)
			{
				float num2 = polygonZone.SqrDistance(point);
				if (num2 < num)
				{
					num = num2;
					if (num == 0f)
					{
						return 0f;
					}
				}
			}
			return num;
		}
	}

	private sealed class PreloadJob
	{
		public readonly int sceneIndex;

		private readonly Action wire;

		public PreloadJob(int sceneIndex, Action wire)
		{
			this.sceneIndex = sceneIndex;
			this.wire = wire;
		}

		public void Wire()
		{
			wire?.Invoke();
		}
	}

	private const float SqrLoadDistance = 100f;

	private const float UnloadDistance = 200f;

	private const float SqrUnloadDistance = 40000f;

	private const int DistanceToLoadScene = 10;

	public bool checkEnvironmentForLoad = true;

	public GameObject SuperMarketFront;

	public GameObject SuperMarketOther;

	public Material SunnySkybox;

	public Material currentSkybox;

	public Material nightSkybox;

	public bool MainCityInstance;

	public bool DesertInstance;

	public bool HomeSellingInstance;

	public bool MountainInstance;

	public bool IslandInstance;

	public bool road2Instance;

	public bool road1Instance;

	public bool straightRoadInstance;

	public bool parkingInstance;

	public bool raceTrack;

	public bool city2Instance;

	public bool home2Instance;

	public bool home3Instance;

	public bool highway2Instances;

	public bool richmondInstance;

	public bool offRoadInstances;

	public bool garageInstance;

	public bool mainGarageInstance;

	public bool japanSceneInstance;

	public bool japanSuburbanSceneInstance;

	public bool richIslandInstance;

	public bool richIslandRoadInstance;

	public bool cityMainRacetrackInstance;

	public bool tropicalIslandInstance;

	public bool junctionNurburgringInstance;

	public bool nurburgringInstance;

	public bool italyInstance;

	private bool WillLoad;

	public bool fansLoaded;

	public Vector3[] FanPos;

	public Vector3[] FanEuler;

	public Light[] scene11Lights;

	public Light directionLight11;

	public Material darkSky;

	public Material lightSky;

	public List<GameObject> ForHighQuality = new List<GameObject>();

	public List<GameObject> ForLowQuality = new List<GameObject>();

	public GameObject mainCityPart;

	public ScenePartSettings cityScenePartSettings;

	public ScenePartSettings city2ScenePartSettings;

	public ScenePartSettings mountainScenePartSettings;

	public ScenePartSettings desertScenePartSettings;

	public ScenePartSettings straightRoadScenePartSettings;

	public ScenePartSettings road1ScenePartSettings;

	public ScenePartSettings racingScenePartSettings;

	public ScenePartSettings road2ScenePartSettings;

	public ScenePartSettings parkingSceneSettings;

	public ScenePartSettings Highway2ScenePartSettings;

	public ScenePartSettings offRoadSettings;

	public WaypointData[] waypointsData;

	public GameObject birds;

	public Camera mainCamera;

	public Skybox nearCameraSkybox;

	public AudioSource Whistle;

	public GameObject[] finish400m;

	public GameObject[] finish1600m;

	public GameObject flameRight;

	public GameObject flameLeft;

	private int idExposure;

	private int IdRotation;

	public Transform ropeBegin;

	public Transform ropeEnd;

	public ObiRope MainRope;

	public GameObject HideOnMountain;

	private bool hiden = true;

	public GameObject[] HospitalTrigers;

	public GameObject[] PoliceTrigers;

	public GameObject[] FireStationTrigers;

	public GameObject[] HotelTrigers;

	public GameObject[] AirPortTrigers;

	public GameObject[] CarWashTrigers;

	public GameObject[] SellCarTrigers;

	public GameObject[] RaceTrigers;

	public List<CarLifter> carLifters;

	private float speedFogDensity = 7f;

	private float speedDLcolor = 1f;

	private float speedDLintensity = 1f;

	private int skipRot;

	public GameObject finishHalfRound;

	private float targetF = 5f;

	private float ref3;

	private float ref4;

	private Vector3 v3ref4;

	private ScenePartSettings lastScenePart;

	private float f;

	public int lightCount;

	public List<Light> spotLights;

	private int frameSkipLight;

	public Transform[] racingToRoadPoints;

	public Transform[] newDesertToRoadPoints;

	public Transform newDesertMiddlePoint;

	public Transform[] offRoadToRoadPoints;

	private bool VeryFirstLoad = true;

	public Material lowWater;

	public Material highWater;

	public MeshRenderer water;

	public Transform richmondToRoadSpawn;

	private GameObject parentOfIcons;

	public Transform[] straightRoadPoints;

	public GameObject dogHome1;

	public GameObject[] City2parts;

	public loadCityParts[] LoadCity2Parts;

	public int nowLoadingCityPartIndex;

	public GameObject[] Objects;

	public LoadObject[] LoadObjects;

	public Transform JapanPartInFreeParent;

	public const string JapanPartInFreePath = "FreeParts/JapanPartInFree";

	public Transform CityMainRaceTrackRoadParent;

	private const string CityMainRaceTrackRoadPath = "FreeParts/CityMainRaceTrackRoad";

	private const string CleverFreePartPath = "FreeParts/object2";

	public const bool NewCityMainEnabled = true;

	private int ram = 3000;

	public List<MeshCollider> _meshColliders;

	private IGarageSelectorService _garageSelectorService;

	private FreeDriveControl _freeDriveControl;

	private OptionalDependency<FreeDriveControl> _freeDriveControlDep;

	private SkyboxBlender _skyboxBlender;

	private OptionalDependency<SkyboxBlender> _skyboxBlenderDep;

	private CoreGameManager _coreGameManager;

	private ISceneLoader _sceneLoader;

	private ISlotService _slotService;

	private ManagerUI _managerUI;

	public List<Collider> allColliders = new List<Collider>();

	public List<Collider> allTriggers = new List<Collider>();

	public List<SignHandler> allSing = new List<SignHandler>();

	private CameraManager _cameraManager;

	private OptionalDependency<CameraManager> _cameraManagerDep;

	private GarageController _garageController;

	private OptionalDependency<GarageController> _garageControllerDep;

	private MultiPlayerMenu _multiPlayerMenu;

	private OptionalDependency<MultiPlayerMenu> _multiPlayerMenuDep;

	private SceneCollidersContainer _sceneCollidersContainer;

	private OptionalDependency<SceneCollidersContainer> _sceneCollidersContainerDep;

	public SpawnPointsContainer SpawnPointsContainer;

	private OptionalDependency<SpawnPointsContainer> _spawnPointsContainerDep;

	public SpawnPointsContainer SpecialPointsContainer;

	private GameObject _japanPartInFreeInstance;

	private bool _isLoadingJapanPart;

	private GameObject _cityMainRaceTrackRoadInstance;

	private bool _isLoadingRacePart;

	private IDisposable _removeFreePartJapanDisposable;

	private IDisposable _removeFreePartCityMainRaceTrackDisposable;

	private IDisposable _cleverUnloadDisposable;

	private IDisposable _unloadLaterDisposable;

	private IDisposable _parkingUnloadDisposable;

	private const float ParkingUnloadDelaySeconds = 1.5f;

	private GameObject _cleverInstance;

	private bool _isLoadingClever;

	private Sort2 _sort2;

	private OptionalDependency<Sort2> _sort2Dep;

	private MenuLevelParent _menuLevelParent;

	private GlobalCollectionsProvider _collectionsProvider;

	private HomesConfig _homesConfig;

	private bool _isDestroyed;

	private FreeTimeAttackController _freeTimeAttackController;

	private OptionalDependency<FreeTimeAttackController> _freeTimeAttackControllerDep;

	private CancellationTokenSource _envCheckCts;

	private CancellationTokenSource _birdsCts;

	private static readonly Collider[] ParkingProbeHits = new Collider[64];

	private static readonly Vector3 CarParkingProbeHalfExtents = new Vector3(3f, 2f, 6f);

	private static readonly Vector3 PersonParkingProbeHalfExtents = new Vector3(1.5f, 1.5f, 1.5f);

	public static bool KeepLocationLoadedForGarage = true;

	private const int OutlyingCity2PartsCount = 4;

	private const double SteamActivationBudgetMs = 2.0;

	private const int SteamCleverActivationKey = -1000;

	private readonly Dictionary<int, SteamGradualActivation> _steamActivations = new Dictionary<int, SteamGradualActivation>();

	private readonly List<int> _steamActivationsDone = new List<int>();

	private readonly Dictionary<int, List<GameObject>> _steamUnitCache = new Dictionary<int, List<GameObject>>();

	private bool _steamActivationLoopRunning;

	private const float SQR_STEAM_ENABLE_DISTANCE = 160000f;

	private const float SQR_STEAM_DISABLE_DISTANCE = 490000f;

	private float _steamGracePeriodEndTime;

	private const float SteamGracePeriodSeconds = 10f;

	private readonly HashSet<int> steamEnabledScenes = new HashSet<int>();

	private bool steamPreloadInProgress;

	private bool _worldHiddenForGarage;

	private readonly List<GameObject> _rootsHiddenForGarage = new List<GameObject>();

	private const string WorldBoundariesPath = "trigers and points/All ristricts - WorldBoundaries";

	private const float GarageShadowBlockerHeight = 25f;

	private readonly List<KeyValuePair<Renderer, ShadowCastingMode>> _shadowCastersOffForGarage = new List<KeyValuePair<Renderer, ShadowCastingMode>>();

	private float _lastMainBodyLoadStart;

	private const float MainBodyLoadMinInterval = 0.2f;

	private const float GarageResidencyWaitSeconds = 20f;

	private CharactersService _steamCharacters;

	private const string SteamEnvVisitedMaskKey = "steam_postcard_env_mask";

	private const int SteamEnvironmentsToVisit = 5;

	private ISteamAchievementService _steamAchievementService;

	public Sort2 GetSort => _sort2;

	public MenuGarageNavigationWindow MenuGarageNavigationWindow { private get; set; }

	public static LightmapData[] PreGarageLightmaps { get; private set; }

	public static LightmapsMode PreGarageLightmapsMode { get; private set; }

	public static LightProbes PreGarageLightProbes { get; private set; }

	private static bool SteamSceneSlicingEnabled => SteamPerfFlags.IsOn("sceneSlicing");

	public static bool SteamCleverAsyncEnabled => SteamPerfFlags.IsOn("cleverAsync");

	public bool IsSteamPreloadInProgress => steamPreloadInProgress;

	public bool MpInitialCarsBuilding { get; private set; }

	public static event Action OnRaceSceneLoaded = () =>
	{
	};

	public event Action<int, bool> OnSceneLoadedEvent;

	public event Action<int, bool> HomeBought;

	[Inject]
	public void Construct(CoreGameManager coreGameManager, ISceneLoader sceneLoader, ISlotService slotService, MenuLevelParent menuLevelParent, ManagerUI managerUI, GlobalCollectionsProvider collectionsProvider, IGarageSelectorService garageSelectorService, OptionalDependency<FreeDriveControl> freeDriveControl, OptionalDependency<SkyboxBlender> skyboxBlender, OptionalDependency<CameraManager> cameraManager, OptionalDependency<GarageController> garageController, OptionalDependency<MultiPlayerMenu> multiPlayerMenu, OptionalDependency<SceneCollidersContainer> sceneCollidersContainer, OptionalDependency<SpawnPointsContainer> spawnPointsContainer, OptionalDependency<Sort2> sort2, OptionalDependency<FreeTimeAttackController> freeTimeAttackController, HomesConfig homesConfig, ISteamAchievementService steamAchievementService)
	{
		_steamAchievementService = steamAchievementService;
		_collectionsProvider = collectionsProvider;
		_menuLevelParent = menuLevelParent;
		_sort2Dep = sort2;
		_spawnPointsContainerDep = spawnPointsContainer;
		_sceneCollidersContainerDep = sceneCollidersContainer;
		_multiPlayerMenuDep = multiPlayerMenu;
		_garageControllerDep = garageController;
		_cameraManagerDep = cameraManager;
		_managerUI = managerUI;
		_slotService = slotService;
		_coreGameManager = coreGameManager;
		_skyboxBlenderDep = skyboxBlender;
		_freeDriveControlDep = freeDriveControl;
		_sceneLoader = sceneLoader;
		_garageSelectorService = garageSelectorService;
		_freeTimeAttackControllerDep = freeTimeAttackController;
		_homesConfig = homesConfig;
	}

	private void Awake()
	{
		_sort2 = _sort2Dep?.Value;
		SpawnPointsContainer = _spawnPointsContainerDep?.Value;
		_sceneCollidersContainer = _sceneCollidersContainerDep?.Value;
		_multiPlayerMenu = _multiPlayerMenuDep?.Value;
		_garageController = _garageControllerDep?.Value;
		_cameraManager = _cameraManagerDep?.Value;
		_skyboxBlender = _skyboxBlenderDep?.Value;
		_freeDriveControl = _freeDriveControlDep?.Value;
		_freeTimeAttackController = _freeTimeAttackControllerDep?.Value;
		InitAwake();
	}

	private void OnDestroy()
	{
		RealUnloadJapanFreePart();
		RealUnloadCityMainFreePart();
		RealUnloadCleverFreePart();
		CommonExtensions.DestroyMat(ref SunnySkybox);
		CommonExtensions.DestroyMat(ref currentSkybox);
		CommonExtensions.DestroyMat(ref nightSkybox);
		_isDestroyed = true;
		_envCheckCts?.Cancel();
		_envCheckCts?.Dispose();
		_envCheckCts = null;
		_birdsCts?.Cancel();
		_birdsCts?.Dispose();
		_birdsCts = null;
	}

	private void Start()
	{
		if (ScenesHelper.IsParkingLevelActive())
		{
			InitStart();
		}
	}

	public void InitAwake()
	{
		ram = SystemInfo.systemMemorySize;
		City2parts = new GameObject[LoadCity2Parts.Length];
		Objects = new GameObject[LoadObjects.Length];
		V.SceneControl = this;
		VeryFirstLoad = true;
		idExposure = Shader.PropertyToID("_Exposure");
		IdRotation = Shader.PropertyToID("_Rotation");
		if ((bool)SunnySkybox)
		{
			float value = UnityEngine.Random.Range(0, 360);
			SunnySkybox.SetFloat(IdRotation, value);
			SunnySkybox.SetFloat(idExposure, 2f);
		}
		if ((bool)_sceneCollidersContainer)
		{
			if (_envCheckCts == null)
			{
				_envCheckCts = new CancellationTokenSource();
			}
			EnvironmentCheckLoop(_envCheckCts.Token).Forget();
		}
	}

	private async UniTaskVoid EnvironmentCheckLoop(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			CheckEnvironmentForDestroyBoxCollider();
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
	}

	private async UniTaskVoid BirdsPositionLoop(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			SetBirdsPosition();
			await UniTask.Delay(TimeSpan.FromSeconds(20.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
	}

	public void InitStart()
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			if (_birdsCts == null)
			{
				_birdsCts = new CancellationTokenSource();
			}
			BirdsPositionLoop(_birdsCts.Token).Forget();
		}
		if (SceneManager.GetActiveScene().buildIndex == 13 && PlayerPrefs.GetInt("isSetLight") == 1)
		{
			RenderSettings.ambientMode = AmbientMode.Flat;
		}
		SteamProvidePedestrianCharacters();
		if (SceneManager.GetActiveScene().buildIndex == 13 && SteamWorldPreloadToggle.Enabled && RamDevice.AllowHeavyPreload())
		{
			Debug.Log($"[HeavyPreload] World: enabled (RAM {RamDevice.GetRamMb()}MB > {12288}MB)");
			SteamPreloadWorldScenes().Forget();
		}
		else
		{
			Debug.Log($"[HeavyPreload] World: skipped (toggle {SteamWorldPreloadToggle.Enabled}, RAM {RamDevice.GetRamMb()}MB <= {12288}MB)");
		}
	}

	public void SetLightHeight(float height)
	{
		if (height < 0.1f)
		{
			_skyboxBlender.TurnOnBuildingLights();
		}
		else
		{
			_skyboxBlender.TurnOffBuildingLights();
		}
		Vector3 eulerAngles = _coreGameManager.directionalLight.transform.eulerAngles;
		eulerAngles.x = 5f + height * 70f;
		_coreGameManager.directionalLight.transform.eulerAngles = eulerAngles;
	}

	public void SetLightRotation(float rot)
	{
		Vector3 eulerAngles = _coreGameManager.directionalLight.transform.eulerAngles;
		eulerAngles.y = rot * 360f;
		_coreGameManager.directionalLight.transform.eulerAngles = eulerAngles;
	}

	private void Update()
	{
		if (!FlagsStorage.IsFreeDriveMode)
		{
			return;
		}
		if ((bool)HideOnMountain)
		{
			if (!FlagsStorage.CloserMountain)
			{
				if (hiden)
				{
					if ((bool)HideOnMountain)
					{
						HideOnMountain.SetActive(value: true);
					}
					hiden = false;
				}
			}
			else if (!hiden)
			{
				if ((bool)HideOnMountain)
				{
					HideOnMountain.SetActive(value: false);
				}
				hiden = true;
			}
		}
		CheckLightToEnable();
	}

	private void CheckLightToEnable()
	{
		if (CPMGraphicsSettingsManager.Instance.Quality < 2 || spotLights.Count <= 0)
		{
			return;
		}
		if (lightCount >= spotLights.Count)
		{
			lightCount = 0;
		}
		if ((bool)spotLights[lightCount])
		{
			bool flag = Vector3.Distance(base.transform.position, spotLights[lightCount].transform.position) < 60f;
			GameObject gameObject = spotLights[lightCount].gameObject;
			if (gameObject.activeSelf != flag)
			{
				gameObject.SetActive(flag);
			}
		}
		lightCount++;
	}

	private void LoadLevelAsync(bool immediately, int sceneBuildIndex, ref bool instanceBool, Action actionAfter = null)
	{
		if (instanceBool || !TryLoadSceneAsync(immediately, sceneBuildIndex, out var loading))
		{
			return;
		}
		instanceBool = true;
		actionAfter?.Invoke();
		if (loading == null)
		{
			OnSceneLoadedEvent?.Invoke(sceneBuildIndex, arg2: true);
			return;
		}
		loading.completed += (AsyncOperation _) =>
		{
			OnSceneLoadedEvent?.Invoke(sceneBuildIndex, arg2: true);
		};
	}

	private static bool TryLoadSceneAsync(bool immediately, int sceneBuildIndex, out AsyncOperation loading)
	{
		loading = null;
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
		if (sceneByBuildIndex.isLoaded)
		{
			return true;
		}
		if (sceneByBuildIndex.IsValid())
		{
			return false;
		}
		if (!immediately)
		{
			loading = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
			if (loading != null)
			{
				SceneLoadTracker.Begin();
				loading.completed += (AsyncOperation _) =>
				{
					SceneLoadTracker.End();
				};
			}
		}
		else
		{
			SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Additive);
		}
		return true;
	}

	public void LoadDesert(bool immediately)
	{
		LoadLevelAsync(immediately, 15, ref DesertInstance, () =>
		{
			V.CurrentEnvironment = V.placeToSpawn.Desert;
		});
	}

	public void LoadRaceTrack(bool immediately)
	{
		LoadLevelAsync(immediately, 23, ref raceTrack);
	}

	public void LoadMainGarage(bool immediately)
	{
		_sceneLoader.LoadSceneAsync(_garageSelectorService.CurrentSceneIndex, LoadSceneMode.Additive, (IContainerBuilder builder) =>
		{
			if ((bool)_cameraManager && (bool)_managerUI && (bool)_garageController && (bool)_multiPlayerMenu && builder != null)
			{
				object[] array = new object[4] { _cameraManager, _managerUI, _garageController, _multiPlayerMenu };
				foreach (object instance in array)
				{
					builder.RegisterInstance(instance).AsSelf();
				}
			}
		});
	}

	public void UnloadMainGarage()
	{
		SceneManager.UnloadSceneAsync(_garageSelectorService.CurrentSceneIndex);
	}

	public void LoadMountain(bool immediately)
	{
		LoadLevelAsync(immediately, 16, ref MountainInstance, () =>
		{
			V.CurrentEnvironment = V.placeToSpawn.Mountain;
			GetAfterMountainHighQualityObjAsync(this.GetCancellationTokenOnDestroy()).Forget();
		});
	}

	public void LoadStraightRoad(bool immediately)
	{
		LoadLevelAsync(immediately, 24, ref straightRoadInstance);
	}

	public void LoadOffRoad(bool immediately)
	{
		if (!offRoadInstances)
		{
			LoadLevelAsync(immediately, 29, ref offRoadInstances);
		}
	}

	public void LoadParkingBuilding(bool immediately)
	{
		_parkingUnloadDisposable?.Dispose();
		_parkingUnloadDisposable = null;
		LoadLevelAsync(immediately, 25, ref parkingInstance);
	}

	public void RequestUnloadParkingBuilding()
	{
		if (!parkingInstance)
		{
			return;
		}
		_parkingUnloadDisposable?.Dispose();
		_parkingUnloadDisposable = Observable.Timer(TimeSpan.FromSeconds(1.5)).Subscribe((long _) =>
		{
			_parkingUnloadDisposable = null;
			if (!IsAnyPlayerObjectInsideParking())
			{
				UnloadParkingBuilding();
			}
		}).AddTo(this);
	}

	private bool IsAnyPlayerObjectInsideParking()
	{
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(25);
		if (!sceneByBuildIndex.isLoaded)
		{
			return false;
		}
		GameObject gameObject = (V.Car ? V.Car : V.LastUsedCar);
		if ((bool)gameObject && IsInsideParking(gameObject.transform.position, CarParkingProbeHalfExtents, sceneByBuildIndex))
		{
			return true;
		}
		if ((bool)V.DriverPerson)
		{
			return IsInsideParking(V.DriverPerson.transform.position, PersonParkingProbeHalfExtents, sceneByBuildIndex);
		}
		return false;
	}

	private static bool IsInsideParking(Vector3 position, Vector3 halfExtents, Scene parkingScene)
	{
		int num = Physics.OverlapBoxNonAlloc(position, halfExtents, ParkingProbeHits, Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
		if (num >= ParkingProbeHits.Length)
		{
			return true;
		}
		for (int i = 0; i < num; i++)
		{
			Collider collider = ParkingProbeHits[i];
			if ((bool)collider && collider.gameObject.scene == parkingScene)
			{
				return true;
			}
		}
		return false;
	}

	public void LoadMainCity(bool immediately)
	{
		LoadLevelAsync(immediately, 14, ref MainCityInstance, () =>
		{
			V.CurrentEnvironment = V.placeToSpawn.MainCity;
			GetAfterCityObjAsync(this.GetCancellationTokenOnDestroy()).Forget();
			TryLoadCityMainRaceTrackFreePart();
		});
	}

	public async UniTaskVoid LoadAllCityParts()
	{
		await UniTask.Delay(200, ignoreTimeScale: true);
		for (int i = 0; i < LoadCity2Parts.Length - 4; i++)
		{
			GlobalEventSystem.ActivateCity2Part(isPart: true, i, show: true);
		}
		FlagsStorage.FirstMainSceneLoaded.Value = true;
	}

	public void LoadCity2(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 26, ref city2Instance, actionAfter);
	}

	public void LoadHome2(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 28, ref home2Instance, actionAfter);
	}

	public void LoadHome3(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 30, ref home3Instance, actionAfter);
	}

	public void LoadTropicalIsland(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 43, ref tropicalIslandInstance, actionAfter);
	}

	public void LoadNurburgring(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 44, ref nurburgringInstance, actionAfter);
	}

	public void LoadJunctionNurburgring(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 45, ref junctionNurburgringInstance, actionAfter);
	}

	public void onMiddleHighway2()
	{
	}

	public void LoadHighway2(bool immediately)
	{
		LoadLevelAsync(immediately, 27, ref highway2Instances);
	}

	public void LoadIsland(bool immediately, Action actionAfter = null)
	{
		LoadLevelAsync(immediately, 17, ref IslandInstance, () =>
		{
			actionAfter?.Invoke();
			CreateDog(0);
		});
	}

	public void LoadRichmond(bool immediately)
	{
	}

	public void LoadJapanScene(bool immediately)
	{
		LoadLevelAsync(immediately, 35, ref japanSceneInstance, TryLoadJapanFreePart);
	}

	public void LoadJapanSuburbanScene(bool immediately)
	{
		LoadLevelAsync(immediately, 36, ref japanSuburbanSceneInstance, TryLoadJapanFreePart);
	}

	public void TryLoadJapanFreePart()
	{
		_removeFreePartJapanDisposable?.Dispose();
		if (!_isLoadingJapanPart && !_japanPartInFreeInstance)
		{
			_isLoadingJapanPart = true;
			GameObject original = Resources.Load<GameObject>("FreeParts/JapanPartInFree");
			_japanPartInFreeInstance = UnityEngine.Object.Instantiate(original, JapanPartInFreeParent);
			if (!_isLoadingJapanPart)
			{
				UnityEngine.Object.Destroy(_japanPartInFreeInstance);
				return;
			}
			_japanPartInFreeInstance.transform.localPosition = new Vector3(0f, 0f, 0f);
			_japanPartInFreeInstance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			_japanPartInFreeInstance.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void TryUnloadJapanFreePart()
	{
		if (!japanSceneInstance && !japanSuburbanSceneInstance)
		{
			_removeFreePartJapanDisposable?.Dispose();
			_removeFreePartJapanDisposable = Observable.Timer(TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
			{
				RealUnloadJapanFreePart();
			}).AddTo(this);
		}
	}

	public void TryLoadCityMainRaceTrackFreePart()
	{
	}

	public void TryUnloadCityMainRaceTrackFreePart()
	{
		if (!MainCityInstance && !cityMainRacetrackInstance)
		{
			_removeFreePartCityMainRaceTrackDisposable?.Dispose();
			_removeFreePartCityMainRaceTrackDisposable = Observable.Timer(TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
			{
				RealUnloadCityMainFreePart();
			}).AddTo(this);
		}
	}

	private void RealUnloadJapanFreePart()
	{
		if ((bool)_japanPartInFreeInstance)
		{
			UnityEngine.Object.Destroy(_japanPartInFreeInstance);
		}
		_isLoadingJapanPart = false;
	}

	private void RealUnloadCityMainFreePart()
	{
		if ((bool)_cityMainRaceTrackRoadInstance)
		{
			UnityEngine.Object.Destroy(_cityMainRaceTrackRoadInstance);
		}
		_isLoadingRacePart = false;
	}

	public void LoadRichIsland(bool immediately)
	{
	}

	public void LoadRichIslandRoad(bool immediately)
	{
	}

	public void LoadItaly(bool immediately)
	{
		LoadLevelAsync(immediately, 47, ref italyInstance);
	}

	public void LoadCityMainRaceTrack(bool immediately)
	{
	}

	public void LoadRoad2(bool immediately)
	{
		LoadLevelAsync(immediately, 18, ref road2Instance);
	}

	public void LoadRoad1(bool immediately)
	{
		LoadLevelAsync(immediately, 19, ref road1Instance);
	}

	public async UniTask PrepareScenesForLoadGarage()
	{
		PreGarageLightmaps = LightmapSettings.lightmaps;
		PreGarageLightmapsMode = LightmapSettings.lightmapsMode;
		PreGarageLightProbes = LightmapSettings.lightProbes;
		if (KeepLocationLoadedForGarage)
		{
			HideWorldForGarage();
			return;
		}
		Scene scene = ScenesHelper.GetAllLoadedScenes().FirstOrDefault((Scene scene2) => scene2.buildIndex == ScenesHelper.GetFreeFromGarageIndex(_garageSelectorService.CurrentSceneIndex));
		if (scene != default(Scene))
		{
			await SceneManager.UnloadSceneAsync(scene);
		}
		UnloadRoad2();
	}

	private void UnLoadLevel(int sceneBuildIndex, ref bool instanceBool, Action actionAfter = null)
	{
		if (!instanceBool)
		{
			return;
		}
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
		if (!sceneByBuildIndex.IsValid())
		{
			instanceBool = false;
			return;
		}
		SceneManager.UnloadSceneAsync(sceneByBuildIndex);
		_unloadLaterDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			UnloadLater();
		}).AddTo(this);
		instanceBool = false;
		ropeEnd.transform.parent = _menuLevelParent.transform;
		actionAfter?.Invoke();
		OnSceneLoadedEvent?.Invoke(sceneBuildIndex, arg2: false);
	}

	public void UnLoadOffRoad()
	{
		UnLoadLevel(29, ref offRoadInstances);
	}

	public void UnloadDesert()
	{
		UnLoadLevel(15, ref DesertInstance, () =>
		{
		});
	}

	public void UnLoadHighway2()
	{
		UnLoadLevel(27, ref highway2Instances);
	}

	public void UnLoadCity2()
	{
		UnLoadLevel(26, ref city2Instance);
	}

	public void UnloadHome2()
	{
		UnLoadLevel(28, ref home2Instance);
	}

	public void UnloadMainCity()
	{
		UnLoadLevel(14, ref MainCityInstance, () =>
		{
			TryUnloadCityMainRaceTrackFreePart();
		});
		if (home3Instance)
		{
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(30));
			home3Instance = false;
		}
		UnloadParkingBuilding();
	}

	public void UnloadRaceTrack()
	{
		if (raceTrack)
		{
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(23));
			raceTrack = false;
			ropeEnd.transform.parent = _menuLevelParent.transform;
		}
	}

	public void UnloadStraightRoad()
	{
		UnLoadLevel(24, ref straightRoadInstance);
	}

	public void UnloadParkingBuilding()
	{
		UnLoadLevel(25, ref parkingInstance);
	}

	public void UnloadMountain()
	{
		UnLoadLevel(16, ref MountainInstance);
	}

	public void UnloadIsland()
	{
		UnLoadLevel(17, ref IslandInstance);
	}

	public void UnloadRichmond()
	{
		UnLoadLevel(31, ref richmondInstance);
	}

	public void UnloadCityMainRaceTrack()
	{
		UnLoadLevel(38, ref cityMainRacetrackInstance, () =>
		{
			TryUnloadCityMainRaceTrackFreePart();
		});
	}

	public void UnloadJapanScene()
	{
		UnLoadLevel(35, ref japanSceneInstance, () =>
		{
			TryUnloadJapanFreePart();
		});
	}

	public void UnloadJapanSuburbanScene()
	{
		UnLoadLevel(36, ref japanSuburbanSceneInstance, () =>
		{
			TryUnloadJapanFreePart();
		});
	}

	public void UnloadRichIslandScene()
	{
		UnLoadLevel(37, ref richIslandInstance);
	}

	public void UnloadRichIslandRoadScene()
	{
		UnLoadLevel(46, ref richIslandRoadInstance);
	}

	public void UnloadItalyScene()
	{
		UnLoadLevel(47, ref italyInstance);
	}

	public void UnloadRoad2()
	{
		UnLoadLevel(18, ref road2Instance);
	}

	public void UnloadRoad1()
	{
		UnLoadLevel(19, ref road1Instance);
	}

	public void UnLoadTropicalIsland()
	{
		UnLoadLevel(43, ref tropicalIslandInstance);
	}

	public void UnLoadJunctionNurburgring()
	{
		UnLoadLevel(45, ref junctionNurburgringInstance);
	}

	public void UnLoadNurburgring()
	{
		UnLoadLevel(44, ref nurburgringInstance);
	}

	public void OnMiddleRoad1()
	{
	}

	public void OnMiddleRoad2()
	{
	}

	public void OnMiddleStraightRoad()
	{
	}

	public void OnMiddleCity()
	{
	}

	public void OnMiddleMountain()
	{
	}

	public void OnMiddleDesert()
	{
	}

	public void OnMiddleRacing()
	{
	}

	public void UnloadAll()
	{
		UnloadMainCity();
		UnloadMountain();
		UnloadStraightRoad();
		UnloadIsland();
		UnloadRichmond();
		UnloadJapanScene();
		UnloadJapanSuburbanScene();
		UnloadRichIslandScene();
		UnloadRichIslandRoadScene();
		UnloadItalyScene();
		UnloadRoad2();
		UnloadRoad1();
		UnloadDesert();
		UnloadCityMainRaceTrack();
		UnloadParkingBuilding();
		UnloadHome2();
		UnLoadCity2();
		UnLoadHighway2();
		UnLoadOffRoad();
		UnLoadTropicalIsland();
		UnLoadJunctionNurburgring();
		UnLoadNurburgring();
	}

	public void CreateDog(int home)
	{
		if (ObscuredPrefs.GetInt("homeOpen" + home) != 0 && home != 0 && dogHome1 == null)
		{
			dogHome1 = UnityEngine.Object.Instantiate(Resources.Load("Other/dog0", typeof(GameObject))) as GameObject;
			Vector3 dogPosition = _homesConfig.Get(home).Positions.DogPosition;
			dogHome1.transform.position = dogPosition;
			dogHome1.GetComponent<DogMove>().initialPoint = dogPosition;
			dogHome1.transform.parent = V.GlobalPoint;
		}
	}

	public void HomeBuy(int home)
	{
		if (ObscuredPrefs.GetInt("homeOpen" + home) != 0)
		{
			HomeBought?.Invoke(home, arg2: true);
		}
	}

	public void UnloadLater()
	{
		_unloadLaterDisposable?.Dispose();
		V.CoreGameManager.ClearAssets();
	}

	public void ShowSuperMarketOther()
	{
		SuperMarketFront.gameObject.SetActive(value: true);
		SuperMarketOther.gameObject.SetActive(value: true);
	}

	public void OnQualityChanged()
	{
		if (SceneManager.GetActiveScene().buildIndex == 11)
		{
			ChangeLightsMode();
			GetComponent<Camera>().farClipPlane = 800f;
			QualitySettings.shadowDistance = 120f;
		}
		if (!FlagsStorage.IsFreeDriveMode)
		{
			return;
		}
		bool flag = CPMGraphicsSettingsManager.Instance.Quality < 2;
		foreach (Light spotLight in spotLights)
		{
			if ((bool)spotLight)
			{
				spotLight.gameObject.SetActive(!flag);
			}
		}
		if ((bool)lowWater && (bool)highWater)
		{
			water.material = ((CPMGraphicsSettingsManager.Instance.Quality >= 2) ? highWater : lowWater);
		}
		_collectionsProvider.ActivateQualitySets();
	}

	public void MakeDayScene11(bool isDay)
	{
		Light[] array = scene11Lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = !isDay;
		}
		ChangeLightsMode();
	}

	private void ChangeLightsMode()
	{
		if (CPMGraphicsSettingsManager.Instance.Quality >= 2)
		{
			Light[] array = scene11Lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].renderMode = LightRenderMode.ForcePixel;
			}
		}
		else
		{
			Light[] array = scene11Lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].renderMode = LightRenderMode.ForceVertex;
			}
		}
		base.gameObject.GetComponent<Camera>().farClipPlane = 800f;
		QualitySettings.shadowDistance = 120f;
	}

	public void ForHighQualityEnable(bool enable)
	{
		PlayerDatas.DisableEnableAll(enable, ForHighQuality.ToArray());
		ForLowQualityEnable(!enable);
	}

	public void ForLowQualityEnable(bool enable)
	{
		PlayerDatas.DisableEnableAll(enable, ForLowQuality.ToArray());
	}

	public void SetBirdsPosition()
	{
		if (!V.DriverPerson || FlagsStorage.IsMainMenu || !_sort2 || !((birds.transform.position - V.DriverPerson.transform.position).sqrMagnitude > mainCamera.farClipPlane * mainCamera.farClipPlane))
		{
			return;
		}
		int num = GPS.GPSTracker.FindRandomClosestPoint(V.DriverPerson.transform.localPosition);
		if (num == -1)
		{
			return;
		}
		WayPointNode wayPointNode = GPS.GPSTracker.nodes[num];
		if (wayPointNode == null)
		{
			return;
		}
		Vector3 vector = wayPointNode.position;
		for (int i = 0; i < 25; i++)
		{
			if (wayPointNode.next.IsNullOrEmpty())
			{
				break;
			}
			wayPointNode = GPS.GPSTracker.nodes[(int)wayPointNode.next[0]];
		}
		vector = new Vector3(wayPointNode.position.x, wayPointNode.position.y, wayPointNode.position.z) + V.GlobalOffset + Vector3.up * 10f;
		birds.transform.position = vector;
	}

	private async UniTask GetAfterMountainHighQualityObjAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(16);
			while (!sceneByBuildIndex.isLoaded)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(16);
			}
			FlagsStorage.FirstMainSceneLoaded.Value = true;
			GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				if (gameObject.name == "hq")
				{
					ForHighQuality.Add(gameObject);
					gameObject.SetActive(CPMGraphicsSettingsManager.Instance.Quality > 1);
				}
				else if (gameObject.name == "hide")
				{
					HideOnMountain = gameObject;
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask GetAfterCityObjAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(14);
			while (!sceneByBuildIndex.isLoaded)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(14);
			}
			FlagsStorage.FirstMainSceneLoaded.Value = true;
			GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				if (gameObject.name == "shadow")
				{
					ForLowQuality.Add(gameObject);
					gameObject.SetActive(CPMGraphicsSettingsManager.Instance.Quality <= 1);
				}
				else if (gameObject.name == "NEW CITY")
				{
					mainCityPart = gameObject;
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void ShowHideCityParts(bool show)
	{
		if ((bool)mainCityPart)
		{
			mainCityPart.gameObject.SetActive(show);
		}
		else if (show)
		{
			LoadMainCity(immediately: true);
		}
		RenderSettings.fog = !show;
		if (show)
		{
			_freeDriveControl.outTunnel();
			return;
		}
		RenderSettings.fogEndDistance = 90f;
		RenderSettings.fogStartDistance = 30f;
	}

	public void SetScenePart(ScenePartSettings sceneSettings)
	{
	}

	public void onSceneSettingEnter(string trigerName)
	{
		switch (trigerName)
		{
		case "Road1Settings":
			SetScenePart(road1ScenePartSettings);
			break;
		case "Road2Settings":
			SetScenePart(road2ScenePartSettings);
			break;
		case "DesertSettings":
			SetScenePart(desertScenePartSettings);
			break;
		case "RacingSettings":
			SetScenePart(racingScenePartSettings);
			break;
		case "StraightRoadSettings":
			SetScenePart(straightRoadScenePartSettings);
			break;
		case "CitySettings":
			SetScenePart(cityScenePartSettings);
			break;
		case "MountainSettings":
			SetScenePart(mountainScenePartSettings);
			break;
		case "Highway2Settings":
			SetScenePart(Highway2ScenePartSettings);
			break;
		case "City2Settings":
			SetScenePart(city2ScenePartSettings);
			break;
		case "offRoadSettings":
			SetScenePart(offRoadSettings);
			break;
		}
	}

	private Vector3 GetLocalPlayerPosition()
	{
		if (FlagsStorage.OutOfCar && !V.Car)
		{
			return V.DriverPerson.transform.position;
		}
		GameObject cachedCar = _coreGameManager.carCamerasController.CachedCar;
		if ((bool)cachedCar)
		{
			return cachedCar.transform.position;
		}
		if ((bool)V.Car)
		{
			return V.Car.transform.position;
		}
		return V.DriverPerson.transform.position;
	}

	public int getEnvironmentByColliders(Vector3 mainPos)
	{
		if (!V.DriverPerson)
		{
			return 0;
		}
		if (_sceneCollidersContainer == null || _sceneCollidersContainer.allEnvironmentCollider == null)
		{
			return -1;
		}
		mainPos -= V.GlobalOffset;
		int num = 0;
		int result = -1;
		float num2 = float.MaxValue;
		oneEnvironmentColliders[] allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		for (int i = 0; i < allEnvironmentCollider.Length; i++)
		{
			float num3 = allEnvironmentCollider[i].SqrDistanceTo(mainPos);
			if (num3 <= num2)
			{
				num2 = num3;
				result = num;
				if (num2 == 0f)
				{
					return result;
				}
			}
			num++;
		}
		return result;
	}

	public void LoadPartsCity2()
	{
		if (!V.DriverPerson)
		{
			return;
		}
		if (city2Instance)
		{
			Vector3 localPlayerPosition = GetLocalPlayerPosition();
			for (int i = 0; i < LoadCity2Parts.Length; i++)
			{
				if (!LoadCity2Parts[i])
				{
					continue;
				}
				if (i < LoadCity2Parts.Length - 4)
				{
					GlobalEventSystem.ActivateCity2Part(isPart: true, i, show: true);
					continue;
				}
				float num = Vector3.Distance(LoadCity2Parts[i].collider.ClosestPoint(localPlayerPosition), localPlayerPosition);
				if (num < 1000f)
				{
					GlobalEventSystem.ActivateCity2Part(isPart: true, i, show: true);
				}
				else if (num > 1100f)
				{
					GlobalEventSystem.ActivateCity2Part(isPart: true, i, show: false);
				}
			}
		}
		LoadCity2Object();
	}

	private void LoadCity2Object()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < LoadObjects.Length; i++)
		{
			if (!LoadObjects[i])
			{
				continue;
			}
			float num = Vector3.Distance(LoadObjects[i].collider.ClosestPoint(position), position);
			if (num < LoadObjects[i].LoadDistance)
			{
				if (i == 2)
				{
					if (SteamCleverAsyncEnabled)
					{
						SteamSetCleverPartActive(active: true);
					}
					else if (!_cleverInstance && !_isLoadingClever)
					{
						TryLoadCleverFreePart();
					}
				}
				else
				{
					GlobalEventSystem.ActivateCity2Part(isPart: false, i, show: true);
				}
			}
			else
			{
				if (!(num > LoadObjects[i].disableDistance))
				{
					continue;
				}
				if (i == 2)
				{
					if (SteamCleverAsyncEnabled)
					{
						SteamSetCleverPartActive(active: false);
					}
					else if (_isLoadingClever && _cleverUnloadDisposable == null)
					{
						_cleverUnloadDisposable = Observable.Timer(TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
						{
							RealUnloadCleverFreePart();
						}).AddTo(this);
					}
				}
				else
				{
					GlobalEventSystem.ActivateCity2Part(isPart: false, i, show: false);
				}
			}
		}
	}

	public void TryLoadCleverFreePart()
	{
		_cleverUnloadDisposable?.Dispose();
		_cleverUnloadDisposable = null;
		_isLoadingClever = true;
		GameObject original = Resources.Load<GameObject>("FreeParts/object2");
		_cleverInstance = UnityEngine.Object.Instantiate(original);
		if (_isDestroyed)
		{
			RealUnloadCleverFreePart();
			return;
		}
		_cleverInstance.transform.position += V.GlobalPoint.position;
		_cleverInstance.transform.parent = V.GlobalPoint.transform;
		_cleverInstance.SetActive(value: true);
	}

	private void RealUnloadCleverFreePart()
	{
		_cleverUnloadDisposable?.Dispose();
		_cleverUnloadDisposable = null;
		_isLoadingClever = false;
		if ((bool)_cleverInstance)
		{
			Addressables.ReleaseInstance(_cleverInstance);
		}
		_cleverInstance = null;
	}

	public List<int> GetSceneIndexByPosition(Vector3 position)
	{
		position -= V.GlobalOffset;
		List<int> list = new List<int>();
		int num = 0;
		oneEnvironmentColliders[] allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		for (int i = 0; i < allEnvironmentCollider.Length; i++)
		{
			if (allEnvironmentCollider[i].SqrDistanceTo(position) < 100f && !FlagsStorage.InGarageFree && !FlagsStorage.InGarageMenu && !FlagsStorage.IsInGarageTunnel)
			{
				list.Add(((SceneType)num).ToBuildIndex());
			}
			num++;
		}
		return list;
	}

	public void CheckEnvironmentForDestroyBoxCollider()
	{
		if (!checkEnvironmentForLoad || _menuLevelParent.isDroneEnabled || !V.DriverPerson || !_coreGameManager || !_coreGameManager.carCamerasController || _sceneCollidersContainer == null || _sceneCollidersContainer.allEnvironmentCollider == null)
		{
			return;
		}
		Vector3 point = GetLocalPlayerPosition() - V.GlobalOffset;
		bool flag = FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu || FlagsStorage.IsInGarageTunnel;
		int num = 0;
		oneEnvironmentColliders[] allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		for (int i = 0; i < allEnvironmentCollider.Length; i++)
		{
			float num2 = allEnvironmentCollider[i].SqrDistanceTo(point);
			if (num2 < 100f && !flag)
			{
				LoadEnvironmentByIndex(num);
				TrackEnvironmentVisited(num);
				TrackEnvironmentVisitedForSteam(num);
			}
			else if (num2 > 40000f)
			{
				DestroyEnvironmentByIndex(num);
			}
			num++;
		}
		LoadPartsCity2();
	}

	private void TrackEnvironmentVisited(int environmentIndex)
	{
		GooglePlayAchievements.ReportMaskProgress("CgkIuKqW0PcZEAIQKQ", "gpg_postcard_env_mask", environmentIndex, AchievementProgress.EffectiveVisitedEnvironmentsTarget, "environment_load");
	}

	public void LoadEnvironmentByIndex(int index, bool immediately = false)
	{
		switch (index)
		{
		case 0:
			LoadMainCity(immediately);
			break;
		case 1:
			LoadHighway2(immediately);
			break;
		case 2:
			LoadDesert(immediately);
			break;
		case 3:
			LoadMountain(immediately);
			break;
		case 4:
			LoadRaceTrack(immediately);
			if (_freeTimeAttackController != null)
			{
				_freeTimeAttackController.EnableComponents(state: true);
			}
			break;
		case 5:
			LoadCity2(immediately);
			break;
		case 6:
			LoadIsland(immediately);
			break;
		case 7:
			LoadRoad1(immediately);
			break;
		case 8:
			LoadRoad2(immediately);
			break;
		case 9:
			LoadStraightRoad(immediately);
			break;
		case 10:
			LoadHome2(immediately);
			break;
		case 11:
			LoadOffRoad(immediately);
			break;
		case 12:
			LoadRichmond(immediately);
			break;
		case 13:
			LoadJapanScene(immediately);
			break;
		case 14:
			LoadJapanSuburbanScene(immediately);
			break;
		case 15:
			LoadRichIsland(immediately);
			break;
		case 21:
			LoadRichIslandRoad(immediately);
			break;
		case 20:
			LoadItaly(immediately);
			break;
		case 16:
			LoadCityMainRaceTrack(immediately);
			break;
		case 17:
			LoadTropicalIsland(immediately);
			break;
		case 18:
			LoadNurburgring(immediately);
			break;
		case 19:
			LoadJunctionNurburgring(immediately);
			break;
		}
	}

	public void DestroyEnvironmentByIndex(int index)
	{
	}

	private void SteamEnqueueActivation(int key, SteamGradualActivation job)
	{
		_steamActivations[key] = job;
		if (!_steamActivationLoopRunning)
		{
			_steamActivationLoopRunning = true;
			SteamActivationLoop(this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	private async UniTaskVoid SteamActivationLoop(CancellationToken cancellationToken)
	{
		while (true)
		{
			await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
			if (_steamActivations.Count == 0)
			{
				continue;
			}
			int key = 0;
			SteamGradualActivation steamGradualActivation = null;
			foreach (KeyValuePair<int, SteamGradualActivation> steamActivation in _steamActivations)
			{
				if (steamGradualActivation == null || (steamActivation.Value.Activating && !steamGradualActivation.Activating))
				{
					key = steamActivation.Key;
					steamGradualActivation = steamActivation.Value;
				}
			}
			steamGradualActivation.Step(2.0);
			if (steamGradualActivation.IsDone)
			{
				_steamActivations.Remove(key);
			}
		}
	}

	private void SteamToggleGradually(int key, bool active, List<GameObject> roots, string label)
	{
		if (_steamActivations.TryGetValue(key, out var value))
		{
			if (!value.IsDone)
			{
				if (value.Activating != active)
				{
					value.Reverse();
				}
				return;
			}
			_steamActivations.Remove(key);
			if (value.Activating == active)
			{
				return;
			}
		}
		List<GameObject> units = SteamGetUnits(key, roots);
		SteamEnqueueActivation(key, active ? SteamGradualActivation.BeginActivation(roots, units, label) : SteamGradualActivation.BeginDeactivation(roots, units, label));
	}

	private List<GameObject> SteamGetUnits(int key, List<GameObject> roots)
	{
		if (!_steamUnitCache.TryGetValue(key, out var value))
		{
			value = SteamGradualActivation.CollectUnits(roots);
			_steamUnitCache[key] = value;
		}
		return value;
	}

	private void SteamCacheSceneUnits(int buildIndex, Scene scene)
	{
		if (SteamSceneSlicingEnabled)
		{
			SteamGetUnits(buildIndex, SteamSceneRoots(scene));
		}
	}

	private static List<GameObject> SteamSceneRoots(Scene scene)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			if (gameObject.name != "FreeDrivePartScope")
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	private void SteamToggleSceneGradually(int buildIndex, Scene scene, bool active)
	{
		SteamToggleGradually(buildIndex, active, SteamSceneRoots(scene), scene.name);
	}

	private bool SteamCompleteActivation(int key)
	{
		if (!_steamActivations.TryGetValue(key, out var value))
		{
			return false;
		}
		_steamActivations.Remove(key);
		if (value.IsDone && !value.Activating)
		{
			return false;
		}
		if (!value.Activating)
		{
			value.Reverse();
		}
		value.Complete();
		return true;
	}

	private void SteamCancelActivation(int key)
	{
		if (_steamActivations.TryGetValue(key, out var value))
		{
			value.RestoreAfterRootsDisabled();
			_steamActivations.Remove(key);
		}
	}

	private void SteamFinishPendingDeactivations()
	{
		_steamActivationsDone.Clear();
		foreach (KeyValuePair<int, SteamGradualActivation> steamActivation in _steamActivations)
		{
			if (!steamActivation.Value.Activating)
			{
				steamActivation.Value.Complete();
				_steamActivationsDone.Add(steamActivation.Key);
			}
		}
		foreach (int item in _steamActivationsDone)
		{
			_steamActivations.Remove(item);
		}
	}

	public void SteamSetCleverPartActive(bool active)
	{
		SteamGradualActivation value;
		if (!_cleverInstance)
		{
			if (active)
			{
				SteamLoadCleverPart();
			}
		}
		else if (((_steamActivations.TryGetValue(-1000, out value) && !value.IsDone) ? value.Activating : _cleverInstance.activeSelf) != active)
		{
			SteamToggleGradually(-1000, active, new List<GameObject> { _cleverInstance }, "Clever");
		}
	}

	private void SteamPreloadCleverPart()
	{
		SteamWatchCleverFlag();
		SteamLoadCleverPart();
		if ((bool)_cleverInstance)
		{
			_cleverInstance.SetActive(value: false);
		}
	}

	private void SteamWatchCleverFlag()
	{
	}

	private void SteamLoadCleverPart()
	{
		if (!_cleverInstance && !_isDestroyed && (bool)V.GlobalPoint)
		{
			GameObject gameObject = Resources.Load<GameObject>("FreeParts/object2");
			if ((bool)gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.transform.position += V.GlobalPoint.position;
				gameObject2.transform.parent = V.GlobalPoint.transform;
				_steamUnitCache.Remove(-1000);
				SteamGetUnits(-1000, new List<GameObject> { gameObject2 });
				_cleverInstance = gameObject2;
			}
		}
	}

	public void HideWorldForGarage()
	{
		if (_worldHiddenForGarage)
		{
			return;
		}
		_worldHiddenForGarage = true;
		_ = Time.realtimeSinceStartup;
		SteamFinishPendingDeactivations();
		SceneType[] array = (SceneType[])Enum.GetValues(typeof(SceneType));
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].ToBuildIndex();
			if (num < 0)
			{
				continue;
			}
			Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(num);
			if (!sceneByBuildIndex.isLoaded)
			{
				continue;
			}
			GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				if (gameObject.activeSelf && !(gameObject.name == "FreeDrivePartScope"))
				{
					gameObject.SetActive(value: false);
					_rootsHiddenForGarage.Add(gameObject);
				}
			}
		}
		Transform transform = (V.GlobalPoint ? V.GlobalPoint.Find("trigers and points/All ristricts - WorldBoundaries") : null);
		if ((bool)transform && transform.gameObject.activeSelf)
		{
			transform.gameObject.SetActive(value: false);
			_rootsHiddenForGarage.Add(transform.gameObject);
		}
		DisableRaisedWorldShadowsAsync().Forget();
	}

	public void ShowWorldAfterGarage()
	{
		if (!_worldHiddenForGarage)
		{
			return;
		}
		_ = Time.realtimeSinceStartup;
		foreach (GameObject item in _rootsHiddenForGarage)
		{
			if ((bool)item)
			{
				item.SetActive(value: true);
			}
		}
		_ = _rootsHiddenForGarage.Count;
		_rootsHiddenForGarage.Clear();
		_worldHiddenForGarage = false;
		foreach (KeyValuePair<Renderer, ShadowCastingMode> item2 in _shadowCastersOffForGarage)
		{
			if ((bool)item2.Key)
			{
				item2.Key.shadowCastingMode = item2.Value;
			}
		}
		_ = _shadowCastersOffForGarage.Count;
		_shadowCastersOffForGarage.Clear();
	}

	private async UniTaskVoid DisableRaisedWorldShadowsAsync()
	{
		await UniTask.DelayFrame(2);
		if (_worldHiddenForGarage)
		{
			_ = Time.realtimeSinceStartup;
			DisableRaisedWorldShadows();
		}
	}

	private void DisableRaisedWorldShadows()
	{
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(13);
		if (!sceneByBuildIndex.isLoaded)
		{
			return;
		}
		GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			if (!gameObject.activeSelf)
			{
				continue;
			}
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(includeInactive: false);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.shadowCastingMode != ShadowCastingMode.Off && !(renderer.bounds.min.y < 25f))
				{
					_shadowCastersOffForGarage.Add(new KeyValuePair<Renderer, ShadowCastingMode>(renderer, renderer.shadowCastingMode));
					renderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
		}
	}

	public bool TryStartMainBodyLoad()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - _lastMainBodyLoadStart < 0.2f)
		{
			return false;
		}
		_lastMainBodyLoadStart = realtimeSinceStartup;
		return true;
	}

	public void SetMpInitialCarsBuilding(bool value)
	{
		MpInitialCarsBuilding = value;
	}

	private static async UniTask SteamWaitForGarageResidency(CancellationToken cancellationToken)
	{
		float deadline = Time.realtimeSinceStartup + 20f;
		while (Time.realtimeSinceStartup < deadline && !Detector.Resident.IsResident && (Detector.Resident.IsPreparing || !(Time.realtimeSinceStartup > deadline - 20f + 2f)))
		{
			await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested)
			{
				break;
			}
		}
	}

	private async UniTaskVoid SteamPreloadWorldScenes()
	{
		CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
		checkEnvironmentForLoad = false;
		steamPreloadInProgress = true;
		SteamBlockLoadedFlagAsync(cancellationToken).Forget();
		SteamPrewarmPedestrians();
		List<PreloadJob> allJobs = new List<PreloadJob>
		{
			new PreloadJob(27, () =>
			{
				LoadHighway2(immediately: false);
			}),
			new PreloadJob(26, () =>
			{
				LoadCity2(immediately: false);
			}),
			new PreloadJob(15, () =>
			{
				LoadDesert(immediately: false);
			}),
			new PreloadJob(16, () =>
			{
				LoadMountain(immediately: false);
			}),
			new PreloadJob(17, () =>
			{
				LoadIsland(immediately: false);
			}),
			new PreloadJob(19, () =>
			{
				LoadRoad1(immediately: false);
			}),
			new PreloadJob(18, () =>
			{
				LoadRoad2(immediately: false);
			}),
			new PreloadJob(24, () =>
			{
				LoadStraightRoad(immediately: false);
			}),
			new PreloadJob(29, () =>
			{
				LoadOffRoad(immediately: false);
			}),
			new PreloadJob(23, () =>
			{
				LoadRaceTrack(immediately: false);
			}),
			new PreloadJob(35, () =>
			{
				LoadJapanScene(immediately: false);
			}),
			new PreloadJob(36, () =>
			{
				LoadJapanSuburbanScene(immediately: false);
			}),
			new PreloadJob(47, () =>
			{
				LoadItaly(immediately: false);
			}),
			new PreloadJob(43, () =>
			{
				LoadTropicalIsland(immediately: false);
			}),
			new PreloadJob(45, () =>
			{
				LoadJunctionNurburgring(immediately: false);
			}),
			new PreloadJob(44, () =>
			{
				LoadNurburgring(immediately: false);
			}),
			new PreloadJob(14, () =>
			{
				LoadMainCity(immediately: false);
			})
		};
		SteamLowLoadingPriority.Acquire();
		try
		{
			foreach (PreloadJob item in allJobs)
			{
				await SteamLoadAndDeactivate(item, cancellationToken);
			}
			foreach (PreloadJob item2 in allJobs)
			{
				item2.Wire();
			}
			await SteamWaitForAddressableSubLoads(cancellationToken);
			if (SteamCleverAsyncEnabled)
			{
				SteamPreloadCleverPart();
			}
			await SteamPrewarmTraffic(cancellationToken);
			await SteamWaitForGarageResidency(cancellationToken);
		}
		finally
		{
			SteamLowLoadingPriority.Release();
		}
		_steamGracePeriodEndTime = 0f;
		checkEnvironmentForLoad = true;
		if ((bool)MoveElements._object)
		{
			MoveElements._object.CheckDistance();
		}
		SteamRefreshProximity();
		SteamRefreshEmergencyEnable();
		await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
		if (!cancellationToken.IsCancellationRequested)
		{
			steamPreloadInProgress = false;
			FlagsStorage.FirstMainSceneLoaded.Value = true;
			SteamProximityLoop(cancellationToken).Forget();
			SteamEmergencyEnableLoop(cancellationToken).Forget();
		}
	}

	private async UniTask SteamPrewarmTraffic(CancellationToken cancellationToken)
	{
		if (!FlagsStorage.IsMultiFree && SystemInfo.systemMemorySize > 1100 && (bool)_freeDriveControl && (bool)_freeDriveControl.TrafficComponent && PlayerPrefs.GetInt("CarQuantity", 50) > 0)
		{
			await _freeDriveControl.TrafficComponent.SteamPrewarmVehiclesAsync(cancellationToken);
		}
	}

	[Inject]
	public void SteamConstructPedestrians(CharactersService charactersService)
	{
		_steamCharacters = charactersService;
	}

	private void SteamProvidePedestrianCharacters()
	{
		PeopleTraffic instance = PeopleTraffic.Instance;
		if ((bool)instance && _steamCharacters != null)
		{
			instance.SteamProvideCharacters(_steamCharacters);
		}
	}

	private void SteamPrewarmPedestrians()
	{
		SteamProvidePedestrianCharacters();
		PeopleTraffic instance = PeopleTraffic.Instance;
		if ((bool)instance && instance.CurrentDensity > 0)
		{
			instance.SteamBuildPedestriansAsync().Forget();
		}
	}

	private void SteamEnableAllScenes()
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.isLoaded)
			{
				SteamSetSceneRootsActive(sceneAt.buildIndex, active: true);
			}
		}
	}

	private async UniTask SteamLoadAndDeactivate(PreloadJob job, CancellationToken ct)
	{
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(job.sceneIndex);
		if (sceneByBuildIndex.isLoaded)
		{
			steamEnabledScenes.Add(job.sceneIndex);
			SteamCacheSceneUnits(job.sceneIndex, sceneByBuildIndex);
			SteamSetSceneRootsActive(job.sceneIndex, active: false);
			return;
		}
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(job.sceneIndex, LoadSceneMode.Additive);
		if (asyncOperation != null)
		{
			asyncOperation.allowSceneActivation = false;
			while (asyncOperation.progress < 0.9f)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			await UniTask.Yield(PlayerLoopTiming.Update, ct);
			asyncOperation.allowSceneActivation = true;
			await asyncOperation.WithCancellation(ct);
			await UniTask.Yield(PlayerLoopTiming.Update, ct);
			steamEnabledScenes.Add(job.sceneIndex);
			SteamCacheSceneUnits(job.sceneIndex, SceneManager.GetSceneByBuildIndex(job.sceneIndex));
			SteamSetSceneRootsActive(job.sceneIndex, active: false);
		}
	}

	private async UniTaskVoid SteamBlockLoadedFlagAsync(CancellationToken cancellationToken)
	{
		while (steamPreloadInProgress)
		{
			FlagsStorage.FirstMainSceneLoaded.Value = false;
			await UniTask.Yield(PlayerLoopTiming.PreUpdate, cancellationToken);
			if (steamPreloadInProgress)
			{
				FlagsStorage.FirstMainSceneLoaded.Value = false;
				await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, cancellationToken);
				if (steamPreloadInProgress)
				{
					FlagsStorage.FirstMainSceneLoaded.Value = false;
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
					continue;
				}
				break;
			}
			break;
		}
	}

	private async UniTask SteamWaitForAddressableSubLoads(CancellationToken ct)
	{
		for (float elapsed = 0f; elapsed < 10f; elapsed += 0.2f)
		{
			bool num = !_isLoadingJapanPart || _japanPartInFreeInstance != null;
			bool flag = !_isLoadingRacePart || _cityMainRaceTrackRoadInstance != null;
			if (num & flag)
			{
				break;
			}
			await UniTask.Delay(200, DelayType.Realtime, PlayerLoopTiming.Update, ct);
		}
	}

	private async UniTaskVoid SteamProximityLoop(CancellationToken cancellationToken)
	{
		while (true)
		{
			await UniTask.Delay(500, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			SteamLowLoadingPriority.HealOrphaned();
			try
			{
				SteamRefreshProximity();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private async UniTaskVoid SteamEmergencyEnableLoop(CancellationToken cancellationToken)
	{
		while (true)
		{
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			try
			{
				SteamRefreshEmergencyEnable();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private void SteamRefreshEmergencyEnable()
	{
		if (_worldHiddenForGarage || !_sceneCollidersContainer || !V.DriverPerson)
		{
			return;
		}
		CarCamerasController carCamerasController = _coreGameManager.carCamerasController;
		if (!carCamerasController)
		{
			return;
		}
		GameObject cachedCar = carCamerasController.CachedCar;
		if (!FlagsStorage.OutOfCar && !cachedCar && !V.Car)
		{
			return;
		}
		Vector3 point = (FlagsStorage.OutOfCar ? V.DriverPerson.transform.position : (cachedCar ? cachedCar.transform.position : V.Car.transform.position)) - V.GlobalOffset;
		int num = 0;
		oneEnvironmentColliders[] allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		for (int i = 0; i < allEnvironmentCollider.Length; i++)
		{
			if (allEnvironmentCollider[i].SqrDistanceTo(point) < 100f)
			{
				SteamSetSceneRootsActive(((SceneType)num).ToBuildIndex(), active: true);
			}
			num++;
		}
	}

	private void SteamRefreshProximity()
	{
		if (_worldHiddenForGarage || !_sceneCollidersContainer || !V.DriverPerson)
		{
			return;
		}
		CarCamerasController carCamerasController = _coreGameManager.carCamerasController;
		if (!carCamerasController)
		{
			return;
		}
		GameObject cachedCar = carCamerasController.CachedCar;
		if (!FlagsStorage.OutOfCar && cachedCar == null && V.Car == null)
		{
			return;
		}
		Vector3 point = (FlagsStorage.OutOfCar ? V.DriverPerson.transform.position : (cachedCar ? cachedCar.transform.position : V.Car.transform.position)) - V.GlobalOffset;
		float num = float.MaxValue;
		oneEnvironmentColliders[] allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		for (int i = 0; i < allEnvironmentCollider.Length; i++)
		{
			float num2 = allEnvironmentCollider[i].SqrDistanceTo(point);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (num > 1000000f)
		{
			SteamEnableAllScenes();
			return;
		}
		int num3 = 0;
		allEnvironmentCollider = _sceneCollidersContainer.allEnvironmentCollider;
		foreach (oneEnvironmentColliders obj in allEnvironmentCollider)
		{
			int buildIndex = ((SceneType)num3).ToBuildIndex();
			float num4 = obj.SqrDistanceTo(point);
			if (num4 < 160000f)
			{
				SteamSetSceneRootsActive(buildIndex, active: true, gradual: true);
			}
			else if (num4 > 490000f && Time.realtimeSinceStartup > _steamGracePeriodEndTime)
			{
				SteamSetSceneRootsActive(buildIndex, active: false, gradual: true);
			}
			num3++;
		}
	}

	private void SteamSetSceneRootsActive(int buildIndex, bool active, bool gradual = false)
	{
		if (steamEnabledScenes.Contains(buildIndex) == active)
		{
			if (active && !gradual)
			{
				SteamCompleteActivation(buildIndex);
			}
			return;
		}
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(buildIndex);
		if (!sceneByBuildIndex.isLoaded)
		{
			return;
		}
		if (gradual && SteamSceneSlicingEnabled)
		{
			SteamToggleSceneGradually(buildIndex, sceneByBuildIndex, active);
			if (active)
			{
				steamEnabledScenes.Add(buildIndex);
			}
			else
			{
				steamEnabledScenes.Remove(buildIndex);
			}
			return;
		}
		if (active && SteamCompleteActivation(buildIndex))
		{
			steamEnabledScenes.Add(buildIndex);
			return;
		}
		GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			if (!(gameObject.name == "FreeDrivePartScope"))
			{
				gameObject.SetActive(active);
			}
		}
		if (!active)
		{
			SteamCancelActivation(buildIndex);
		}
		if (active)
		{
			steamEnabledScenes.Add(buildIndex);
		}
		else
		{
			steamEnabledScenes.Remove(buildIndex);
		}
	}

	private void TrackEnvironmentVisitedForSteam(int environmentIndex)
	{
		if (environmentIndex < 0 || environmentIndex >= 31)
		{
			return;
		}
		int num = PlayerPrefs.GetInt("steam_postcard_env_mask", 0);
		int num2 = 1 << environmentIndex;
		if ((num & num2) == 0)
		{
			num |= num2;
			PlayerPrefs.SetInt("steam_postcard_env_mask", num);
			if (CountSetBits(num) >= 5)
			{
				_steamAchievementService?.Unlock("ACH_POSTCARD_HUNTER");
			}
		}
	}

	private static int CountSetBits(int value)
	{
		int num = 0;
		while (value != 0)
		{
			num += value & 1;
			value >>= 1;
		}
		return num;
	}
}
