using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Common;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Car_Parking.Scripts.Racing;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using GPS;
using Gley.TrafficSystem;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WorldInteraction;

public class FreeDriveControl : MonoBehaviour
{
	private const int InvitationReceivedIdEmpty = -99;

	private const float ShadowMaxDistance = 50f;

	public const float CarSpawnYOffset = 0.2f;

	public bool isSelectingCar;

	public bool isHomeLoaded;

	public float MultiRaceTimeLeft;

	public Action<GameObject> OnCarToRoad;

	public ChatControl chatControl;

	public GameObject toRoadButton;

	public GameObject EnterNickName;

	public GameObject TunnelSoundTrigger;

	public GameObject TunnelSoundTrigger2;

	public bool startChangingDayTime;

	public int CarIDToLoadEditor;

	public int actionIndex;

	public GetFuelWindow GetFuel;

	public LayerMask carLayer;

	private int countWaitOfRacing;

	public int current_tar_index = 200;

	public int current_tar_indexMountain;

	public int current_tar_City2;

	public Vector3[] targetsMainLoop;

	public Vector3[] targetsCity2;

	public Vector3[] targetsMountainBig;

	public int DirectionMountain;

	public int DirectionMainLoop;

	public int DirectionCity2;

	private float _smallest;

	private float _smallest2;

	public float smallestCity2;

	public Transform mainPoint;

	public GameObject StraightRoadTrigger;

	public List<string> playerToIgnore = new List<string>();

	public bool fristSpawnCar;

	public bool IsChasing;

	public bool InvitationJustSend;

	public bool InvitationHost;

	public Button inviteToRaceButton;

	private ManagerUI _managerUI;

	private IPrefabFactory _prefabFactory;

	[SerializeField]
	private GameObject prefabCircleNotification;

	public FreeDragRacingWindow newNegotiationWindow;

	public GameObject notificationCircleRacing;

	public GameObject questionWindowRacing;

	public TrafficComponent TrafficComponent;

	public GameObject[] TrafficLights;

	private CarCamerasController _carCamerasController;

	private LevelScript _levelScript;

	private CoreGameManager _coreGameManager;

	private MultiPlayerPosControl _multiPlayerPosControl;

	private PlayerDatas.FriendsUtility _friendsUtility;

	private PlayerDataStorage _playerDataStorage;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	private GarageController _garageController;

	private CurrencyManagerController _currencyManagerController;

	private HomesConfig _homesConfig;

	private ParkingMenuUI _parkingMenuUI;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private OptionalDependency<FreeTimeAttackController> _timeAttackControllerDep;

	private IEarnMoneyService _earnMoneyService;

	private IPricesService _pricesService;

	private const float DelayToRoadButtonClick = 1f;

	private bool _isTouchProcessedToInvite;

	private bool _isInitStart;

	private string _lastRevivedFriendIDToAdd;

	private string _lastRevivedFriendNameToAdd;

	private bool _isDestroyed;

	private Button _toRoadButton;

	private PolicePenalizeForSpeeding _policePenalizeForSpeeding;

	private PoliceSystemConfig _policeConfig;

	private Sort2 _sort2;

	private IAccountController _accountController;

	private IGarageSelectorService _garageSelectorService;

	private IDisposable _wwiseFilterOffDisposable;

	private IDisposable _onTimeOutInviteDisposable;

	private IDisposable _resetInviteButtonDisposable;

	private IDisposable _resetAfterDisposable;

	private IDisposable _resetRigidBodyDisposable;

	private IDisposable _enableToRoadButtonDisposable;

	private IDisposable _updateCurrentRowTick;

	private MenuLevelParent _menuLevelParent;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private GlobalCollectionsProvider _collectionsProvider;

	private InputItemsProvider _controlItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private ToRoadRouter _toRoadRouter;

	private LevelsProvider _levelsProvider;

	private CarInputManager _carInputManager;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private SellCarTrigger _lastSellCarTrigger;

	private FreeTimeAttackController _timeAttackController;

	private IBusinessService _businessService;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private ISteamAchievementService _steamAchievementService;

	private RegistryHandle<BoolEvent> _toggleMapHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ToggleMap);

	private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

	private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

	private OpenWorldAnalytic _openWorldAnalytic;

	private IGameFactManager _gameFactManager;

	private const float RoadReadyTimeoutSeconds = 5f;

	private const float RoadReadyAbsoluteCapSeconds = 40f;

	private const float RoadProbeRayStartHeight = 5f;

	private const float RoadProbeRayLength = 60f;

	private const int RoadProbePollMs = 100;

	public int LastInvitationReceivedId { get; set; } = -99;

	public float DistanceCarsRacingMulti { get; private set; }

	public bool JustPenalized { get; private set; }

	public bool IsToRoadButtonCkicked { get; private set; }

	public PolicePenalizeForSpeeding PolicePenalizeForSpeeding => _policePenalizeForSpeeding;

	public CurrencyManagerController CurrencyManagerController => _currencyManagerController;

	private bool CanSetHomePositions
	{
		get
		{
			if ((bool)V.SceneControl && (bool)V.DriverPerson && (bool)V.CoreGameManager.CarOutInControl)
			{
				if (!V.Car)
				{
					return V.LastUsedCar;
				}
				return true;
			}
			return false;
		}
	}

	public FreeTimeAttackController TimeAttackController => _timeAttackController;

	public ParkingMenuUI ParkingMenuUI => _parkingMenuUI;

	public event Action OnBigMapHide;

	public event Action<int, bool> OnFastFoodBuildBought;

	[Inject]
	public void Construct(PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, MenuLevelParent menuLevelParent, PlayerDatas.CarLoadingUtility carLoadingUtility, GlobalCollectionsProvider collectionsProvider, OnlineCollectionsProvider onlineCollectionsProvider, GarageController garageController, ManagerUI managerUI, IPrefabFactory prefabFactory, CurrencyManagerController currencyManagerController, HomesConfig homesConfig, IEarnMoneyService earnMoneyService, IPricesService pricesService, PoliceSystemConfig policeConfig, Sort2 sort2, IAccountController accountController, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, ISteamAchievementService steamAchievementService, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, LevelsProvider levelsProvider, IGarageSelectorService garageSelectorService, CarInputManager carInputManager, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IBusinessService businessService, OpenWorldAnalytic openWorldAnalytic, OptionalDependency<ParkingMenuUI> parkingMenuUI, OptionalDependency<FreeTimeAttackController> timeAttackController, IGameFactManager gameFactManager, ToRoadRouter toRoadRouter)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_toRoadRouter = toRoadRouter;
		_controlItemsProvider = controlItemsProvider;
		_collectionsProvider = collectionsProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_menuLevelParent = menuLevelParent;
		_sort2 = sort2;
		_pricesService = pricesService;
		_earnMoneyService = earnMoneyService;
		_parkingMenuUIDep = parkingMenuUI;
		_homesConfig = homesConfig;
		_currencyManagerController = currencyManagerController;
		_garageController = garageController;
		_friendsUtility = friendsUtility;
		_playerDataStorage = playerDataStorage;
		_carLoadingUtility = carLoadingUtility;
		_managerUI = managerUI;
		_prefabFactory = prefabFactory;
		_policeConfig = policeConfig;
		_accountController = accountController;
		_garageSelectorService = garageSelectorService;
		_carInputManager = carInputManager;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		_timeAttackControllerDep = timeAttackController;
		_businessService = businessService;
		_openWorldAnalytic = openWorldAnalytic;
		_gameFactManager = gameFactManager;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
		_steamAchievementService = steamAchievementService;
	}

	private void Awake()
	{
		_parkingMenuUI = _parkingMenuUIDep?.Value;
		_timeAttackController = _timeAttackControllerDep?.Value;
		_freeDriveProvider.FreeDriveControl = this;
		if (_gameFactManager.IsTrue(GameFact.Carless))
		{
			FlagsStorage.OutOfCar = true;
		}
		InitAwake();
	}

	public void InitAwake()
	{
		_collectionsProvider.ResetGasPistolMoves();
		FlagsStorage.IsInputEnabledInThisWindow = true;
		FlagsStorage.SolverCheckStarted = false;
		if (!GPS.GPSTracker.scriptableObject)
		{
			LoadSO().Forget();
		}
		InvitationJustSend = false;
		if (toRoadButton.TryGetComponent<Button>(out _toRoadButton))
		{
			_toRoadButton.onClick.AddListener(OnEnableToRoadButtonClick);
		}
		_policePenalizeForSpeeding = new PolicePenalizeForSpeeding(_policeConfig);
	}

	private void OnEnableToRoadButtonClick()
	{
		IsToRoadButtonCkicked = true;
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			IsToRoadButtonCkicked = false;
		}).AddTo(this);
	}

	public void InitStart()
	{
		_isInitStart = true;
		if (!GPS.GPSTracker.scriptableObject)
		{
			LoadSO().Forget();
		}
		TryGetComponent<LevelScript>(out _levelScript);
		TryGetComponent<CoreGameManager>(out _coreGameManager);
		TryGetComponent<CarCamerasController>(out _carCamerasController);
		TryGetComponent<ChatControl>(out chatControl);
		V.CurrentEnvironment = V.PlaceToSpawn;
		_menuLevelParent.SetTextForRacing();
		LoadCurrentEnvironment();
		_currencyManagerController.UpdateLabelsEvents();
		FlagsStorage.CamFollowCar = true;
		FlagsStorage.MoveByTouch = false;
		FlagsStorage.IsEnabledJoy = true;
		startChangingDayTime = false;
		outTunnel();
		if (!FlagsStorage.IsMultiFree)
		{
			if (_gameFactManager.IsTrue(GameFact.Carless) || !_coreGameManager.HasFirstCar())
			{
				PreparePedestrian(V.PlaceToSpawn);
			}
			else
			{
				PrepareCar(V.PlaceToSpawn).Forget();
			}
			OnFirtsCarSpawn();
			_levelsProvider.LevelScript.OpenLoadingMenu();
			CheckSceneLoadAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
		else
		{
			_levelScript.ShowConnectingMenu();
		}
		GetComponent<Camera>().enabled = false;
		CancelInvite();
		_toggleMapHandle.Get(_eventRegistry).Subscribe(ToggleMap);
		_toRoadHandle.Get(_eventRegistry).Subscribe(ToRoadForSubscription);
		_cancelHandle.Get(_eventRegistry).Subscribe(CancelInviteDelegate);
		StartUpdateCurrentRow();
		inviteToRaceButton.onClick.AddListener(InviteToRace);
		V.OnSkyboxChanged?.Invoke();
		ApplyPendingEnvironmentAsync(this.GetCancellationTokenOnDestroy()).Forget();
		Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			_openWorldAnalytic.StartEvent(FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single);
		}).AddTo(this);
		_isInitStart = false;
	}

	private void ToRoadForSubscription()
	{
		if (GetToRoadButtonActiveState())
		{
			_toRoadButton.onClick.Invoke();
		}
	}

	private void CancelInviteDelegate()
	{
		if (isSelectingCar)
		{
			CancelInvite();
		}
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		_freeDriveProvider.FreeDriveControl = null;
		_policePenalizeForSpeeding.Dispose();
		isHomeLoaded = false;
		FlagsStorage.ChasingRaceModeStarted = false;
		if ((bool)_toRoadButton)
		{
			_toRoadButton.onClick.RemoveListener(OnEnableToRoadButtonClick);
		}
		_toggleMapHandle.Get(_eventRegistry)?.Unsubscribe(ToggleMap);
		_toRoadHandle.Get(_eventRegistry)?.Unsubscribe(ToRoadForSubscription);
		_cancelHandle.Get(_eventRegistry)?.Unsubscribe(CancelInviteDelegate);
	}

	private static async UniTaskVoid LoadSO()
	{
		ResourceRequest rr = Resources.LoadAsync("GPS/Scriptables/WayPoints");
		while (!rr.isDone)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(8.0));
		}
		GPS.GPSTracker.scriptableObject = rr.asset as WayPointsSO;
	}

	public void OnFirtsCarSpawn()
	{
		if (V.CurrentEnvironment == V.placeToSpawn.City2 && !FlagsStorage.KeepCityParts)
		{
			V.SceneControl.LoadPartsCity2();
		}
	}

	private async UniTaskVoid ApplyPendingEnvironmentAsync(CancellationToken ct)
	{
		try
		{
			await UniTask.NextFrame(ct);
			RaceEnvironment.ConsumePendingRequest();
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTask CheckSceneLoadAsync(CancellationToken ct = default(CancellationToken))
	{
		_ = 2;
		try
		{
			await FlagsStorage.FirstMainSceneLoaded.Where((bool loaded) => loaded).First().ToUniTask(useFirstValue: false, ct);
			while (V.SceneControl == null || V.SceneControl.IsSteamPreloadInProgress)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.Realtime, PlayerLoopTiming.Update, ct);
			}
			LoadAdditionalScenesFromHomeConfig();
			EnableCamWithDelay().Forget();
			Time.timeScale = 1f;
			if ((bool)V.Car)
			{
				V.CurrentCamIndex = PlayerPrefs.GetInt("LastCam", 1);
				await UniTask.WaitUntil(() => (bool)V.DriverPerson || _isDestroyed, PlayerLoopTiming.Update, ct);
				if (!_isDestroyed)
				{
					_carCamerasController.SetCamera(V.CurrentCamIndex, V.Car.transform, newTarget: true);
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async UniTaskVoid EnableCamWithDelay()
	{
		if (FlagsStorage.StartInHome)
		{
			await SetToHome(60);
			await UniTask.DelayFrame(10);
			isHomeLoaded = true;
		}
		else
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5));
			isHomeLoaded = false;
		}
		EnableCam();
	}

	private void LoadAdditionalScenesFromHomeConfig()
	{
		if (FlagsStorage.StartInHome)
		{
			int index = ObscuredPrefs.GetInt("homeSelected");
			HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(index);
			if (homeDataItem.StartData != null && homeDataItem.StartData.AdditionalScene != SceneType.None)
			{
				V.SceneControl.LoadEnvironmentByIndex((int)homeDataItem.StartData.AdditionalScene, immediately: true);
			}
		}
	}

	private async UniTask SetToHome(int delayFrame)
	{
		await UniTask.DelayFrame(delayFrame);
		int homeToUse = ObscuredPrefs.GetInt("homeSelected");
		HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(homeToUse);
		if (homeDataItem.StartData != null && homeDataItem.StartData.SoundFilter != null && homeDataItem.StartData.SoundFilter.IsValid())
		{
			homeDataItem.StartData.SoundFilter.SetValue();
		}
		if (homeToUse == 0)
		{
			SetHomePositions(homeToUse).Forget();
			V.SceneControl.LoadIsland(immediately: true, () =>
			{
				SetHomePositions(homeToUse).Forget();
			});
			return;
		}
		if (homeToUse == 3)
		{
			SetHomePositions(homeToUse).Forget();
			return;
		}
		V.SceneControl.LoadCity2(immediately: true);
		SetHomePositions(homeToUse).Forget();
		await UniTask.DelayFrame(10);
		SetHomePositions(homeToUse).Forget();
		if (homeToUse != 2)
		{
			LoadCityPart(homeToUse);
		}
	}

	private void LoadCityPart(int homeToUse)
	{
		SetHomePositions(homeToUse).Forget();
	}

	private async UniTaskVoid SetHomePositions(int homeToUse)
	{
		if (!CanSetHomePositions)
		{
			return;
		}
		await UniTask.WaitForFixedUpdate();
		GameObject gameObject = (V.Car ? V.Car : V.LastUsedCar);
		if (!V.DriverPerson || !gameObject)
		{
			return;
		}
		V.DriverPerson.transform.position = _homesConfig.Get(homeToUse).Positions.PersonPosition;
		_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car, enableCollidersImmediately: false, isChangePos: false);
		V.LastUsedCar.transform.position = _homesConfig.Get(homeToUse).Positions.CarPosition;
		V.DriverPerson.transform.position = _homesConfig.Get(homeToUse).Positions.PersonPosition;
		V.LastUsedCar.transform.eulerAngles = Vector3.zero;
		base.transform.LookAt(V.LastUsedCar.transform);
		for (int i = 0; i < 4; i++)
		{
			await UniTask.WaitForFixedUpdate();
			if (!V.DriverPerson)
			{
				break;
			}
			V.DriverPerson.transform.position = _homesConfig.Get(homeToUse).Positions.PersonPosition;
		}
	}

	private void EnableCam()
	{
		ShowMainDrivingUI();
		GetComponent<Camera>().enabled = true;
		_levelsProvider.LevelScript.Resume();
		if (!V.Car)
		{
			if (_gameFactManager.IsTrue(GameFact.Carless))
			{
				ShowWalkingUI();
			}
			return;
		}
		switch (V.Steering)
		{
		case 0:
			if ((bool)_carInputManager.UGUISteeringWheel)
			{
				_carInputManager.UGUISteeringWheel.SetActive(active: true);
			}
			break;
		case 3:
			_menuLevelParent.LeftRight.SetActive(value: true);
			break;
		}
	}

	public void ShowWalkingUI()
	{
		FlagsStorage.OutOfCar = true;
		_coreGameManager.SetSteering();
		if ((bool)_menuLevelParent)
		{
			if ((bool)_menuLevelParent.ParkingMenuUI)
			{
				_menuLevelParent.ParkingMenuUI.ShowWalkElements(canShow: true);
			}
			if ((bool)_menuLevelParent.InsideCamControllParent)
			{
				_menuLevelParent.InsideCamControllParent.SetActive(value: false);
			}
		}
	}

	private void LoadCurrentEnvironment()
	{
		FlagsStorage.FirstMainSceneLoaded.Value = true;
		if (V.CurrentEnvironment == V.placeToSpawn.MainCity)
		{
			FlagsStorage.FirstMainSceneLoaded.Value = false;
			V.SceneControl.LoadMainCity(immediately: false);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Nurburgring)
		{
			V.SceneControl.LoadNurburgring(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Italy)
		{
			V.SceneControl.LoadItaly(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Highway)
		{
			V.SceneControl.LoadHighway2(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Desert)
		{
			V.SceneControl.LoadDesert(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Mountain)
		{
			FlagsStorage.FirstMainSceneLoaded.Value = false;
			V.SceneControl.LoadMountain(immediately: false);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Racing)
		{
			V.SceneControl.LoadRaceTrack(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.City2)
		{
			City2Load();
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.JapanSerpentine)
		{
			V.SceneControl.LoadJapanScene(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.JapanSuburban)
		{
			V.SceneControl.LoadJapanSuburbanScene(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Island)
		{
			V.SceneControl.LoadIsland(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.Offroad)
		{
			V.SceneControl.LoadOffRoad(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.TropicalIsland)
		{
			V.SceneControl.LoadTropicalIsland(immediately: true);
		}
		else if (V.CurrentEnvironment == V.placeToSpawn.GarageEntrance)
		{
			if (_garageSelectorService.CurrentGarageIndex == 0)
			{
				V.CurrentEnvironment = V.placeToSpawn.MainCity;
			}
			else if (_garageSelectorService.CurrentGarageIndex == 1)
			{
				V.CurrentEnvironment = V.placeToSpawn.Desert;
			}
			LoadCurrentEnvironment();
		}
	}

	private void City2Load()
	{
		FlagsStorage.FirstMainSceneLoaded.Value = false;
		V.SceneControl.LoadCity2(immediately: true);
		if (FlagsStorage.KeepCityParts)
		{
			V.SceneControl.LoadAllCityParts().Forget();
		}
		else
		{
			FlagsStorage.FirstMainSceneLoaded.Value = true;
		}
	}

	public void TryCancelInvitation()
	{
		if (isSelectingCar)
		{
			CancelInvite();
			inviteToRaceButton.SetActive(active: true);
		}
		else if (InvitationJustSend)
		{
			CancelButtonNegWindow();
		}
	}

	public void InviteToRace()
	{
		if (_timeAttackController != null && _timeAttackController.IsRaceInProgress())
		{
			_coreGameManager.SetMessage(new LocalizedString("FreeRace.WaitTimeAttack"), 3f);
		}
		else
		{
			if (V.DriverPerson.TryGetComponentInChildren<PersonAnimationsController>(out var component) && (component.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate || component.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate))
			{
				return;
			}
			if (FlagsStorage.InGarageFree)
			{
				_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.CanTDoItInGarage"), 3f);
			}
			else if ((int)CurrencyManagerController.Money < (int)_pricesService.MultiplayerRaceLose)
			{
				_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouDonTHaveEnoughMoney"), 3f);
			}
			else if (!FlagsStorage.OutOfCar)
			{
				if (!notificationCircleRacing && !newNegotiationWindow && !questionWindowRacing)
				{
					if (FlagsStorage.IsPassenger)
					{
						_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouAreJustPassenger"), 4f);
					}
					else
					{
						StartSelectToRace();
					}
				}
			}
			else
			{
				_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouDonTHaveACar"), 4f);
			}
		}
	}

	private void StartSelectToRace()
	{
		if (!isSelectingCar)
		{
			_parkingMenuUI.ShowInvitationPanel(isActive: true);
			inviteToRaceButton.gameObject.SetActive(value: false);
			isSelectingCar = true;
			_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.ClickToTheCarToInvite"), 4f);
			_onTimeOutInviteDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
			{
				onTimeOutInvite();
			}).AddTo(this);
		}
	}

	private void Update()
	{
		_policePenalizeForSpeeding.Update();
		if (FlagsStorage.IsMultiFree && PhotonNetwork.IsConnected && FlagsStorage.ChasingRaceModeStarted)
		{
			if ((bool)_freeDriveProvider.RacingCar2)
			{
				ChasingRaceLogic();
			}
			else
			{
				OnWin();
			}
		}
	}

	public void AimAtCar(GameObject otherCar)
	{
		VehicleData component;
		if (!isSelectingCar)
		{
			PolicePenalizeForSpeeding?.AimAtCar(otherCar);
		}
		else if (otherCar.TryGetComponent<VehicleData>(out component) && !component.Towing)
		{
			if ((bool)component.Driver)
			{
				_ = otherCar.GetComponent<PhotonView>().Owner.NickName;
				SendInvitation(otherCar);
				CancelInvite();
			}
			else
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.ThereIsNoDriver"), 3f);
			}
		}
	}

	private void PolicePlayer()
	{
		if (FlagsStorage.NowPolice && FlagsStorage.RaycastForSpeeding && Input.touchCount > 0 && Physics.Raycast(V.CoreGameManager.GetComponent<Camera>().ScreenPointToRay(Input.touches[Input.touchCount - 1].position), out var hitInfo, float.PositiveInfinity, carLayer, QueryTriggerInteraction.Ignore))
		{
			GameObject gameObject = hitInfo.transform.gameObject;
			if (gameObject.CompareTag("Car") && gameObject != V.Car)
			{
				PenalizeForSpeeding(gameObject);
			}
		}
	}

	private void InvitePlayer()
	{
		if (!isSelectingCar || Input.touchCount <= 0)
		{
			return;
		}
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began && !_isTouchProcessedToInvite)
		{
			Ray ray = V.CoreGameManager.GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
			LayerMask layerMask = -1;
			if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore))
			{
				GameObject gameObject = hitInfo.transform.gameObject;
				if (gameObject.CompareTag("Car") && gameObject != V.Car)
				{
					if ((bool)gameObject.GetComponent<VehicleData>().Driver && !gameObject.GetComponent<VehicleData>().Towing)
					{
						SendInvitation(gameObject);
						CancelInvite();
					}
					else
					{
						V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.ThereIsNoDriver"), 3f);
					}
				}
			}
			_isTouchProcessedToInvite = true;
		}
		TouchPhase phase = touch.phase;
		if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
		{
			_isTouchProcessedToInvite = false;
		}
	}

	private void PenalizeForSpeeding(GameObject car)
	{
		if (car.GetComponent<MultiPlayerPosControl>().speeding)
		{
			if (!V.Car.GetComponent<VehicleData>().LightsComposition.PoliceLights.IsSirenEnabled)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.TurnOnSiren"), 3f);
				return;
			}
			if (Vector3.Distance(car.transform.position, V.DriverPerson.transform.position) > 50f)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.CloserToCar"), 3f);
				return;
			}
			PhotonView component = car.GetComponent<VehicleData>().Driver.GetComponent<PhotonView>();
			int ownerActorNr = component.OwnerActorNr;
			_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(34, ownerActorNr.ToString(), _playerDataStorage.PlayerID.ToString(), component.Owner);
			V.SceneControl.Whistle.transform.position = V.Car.transform.position;
			V.SceneControl.Whistle.Play();
		}
	}

	public void inTunnel()
	{
		TunnelSoundTrigger.SetActive(value: true);
		TunnelSoundTrigger2.SetActive(value: true);
		QualitySettings.shadowDistance = 90f;
	}

	public void outTunnel()
	{
		TunnelSoundTrigger.SetActive(value: false);
		TunnelSoundTrigger2.SetActive(value: false);
		QualitySettings.shadowDistance = 50f;
	}

	private void ChasingRaceLogic()
	{
		if (!IsChasing)
		{
			DistanceCarsRacingMulti = Vector3.Distance(_freeDriveProvider.RacingCar2.transform.position, V.Car.transform.position);
		}
		else
		{
			if (!_multiPlayerPosControl)
			{
				_multiPlayerPosControl = _freeDriveProvider.RacingCar2.GetComponent<MultiPlayerPosControl>();
			}
			DistanceCarsRacingMulti = _multiPlayerPosControl.distanceMultiRace;
		}
		MultiRaceTimeLeft = 0f - Time.deltaTime + MultiRaceTimeLeft;
		if (Time.frameCount % 5 == 0)
		{
			_freeDriveProvider.DragRacingControlMultiplayer.distanceTextChasing.text = DistanceCarsRacingMulti.ToString("F1") + "m";
			_freeDriveProvider.DragRacingControlMultiplayer.timeTextChasing.text = MultiRaceTimeLeft.ToString("F1") + "s";
			float num;
			float num2;
			if (IsChasing)
			{
				num = 1f - DistanceCarsRacingMulti / 250f;
				_freeDriveProvider.DragRacingControlMultiplayer.distanceFill.color = Color.Lerp(Color.red, Color.green, num);
				if (num < 0.2f)
				{
					bool activeSelf = _freeDriveProvider.DragRacingControlMultiplayer.distanceTextChasing.gameObject.activeSelf;
					_freeDriveProvider.DragRacingControlMultiplayer.distanceTextChasing.gameObject.SetActive(!activeSelf);
				}
				else
				{
					_freeDriveProvider.DragRacingControlMultiplayer.distanceTextChasing.gameObject.SetActive(value: true);
				}
				num2 = 1f - MultiRaceTimeLeft / 60f;
				_freeDriveProvider.DragRacingControlMultiplayer.timeFill.color = Color.Lerp(Color.red, Color.green, num2);
			}
			else
			{
				num = DistanceCarsRacingMulti / 250f;
				_freeDriveProvider.DragRacingControlMultiplayer.distanceFill.color = Color.Lerp(Color.red, Color.green, num);
				num2 = 1f - MultiRaceTimeLeft / 60f;
				_freeDriveProvider.DragRacingControlMultiplayer.timeFill.color = Color.Lerp(Color.green, Color.red, num2);
				if ((double)num2 > 0.8)
				{
					bool activeSelf2 = _freeDriveProvider.DragRacingControlMultiplayer.timeTextChasing.gameObject.activeSelf;
					_freeDriveProvider.DragRacingControlMultiplayer.timeTextChasing.gameObject.SetActive(!activeSelf2);
				}
				else
				{
					_freeDriveProvider.DragRacingControlMultiplayer.timeTextChasing.gameObject.SetActive(value: true);
				}
			}
			_freeDriveProvider.DragRacingControlMultiplayer.distanceFill.fillAmount = num;
			_freeDriveProvider.DragRacingControlMultiplayer.timeFill.fillAmount = num2;
		}
		if (DistanceCarsRacingMulti > 250f)
		{
			if (IsChasing)
			{
				OnLoose();
			}
			else
			{
				OnWin();
			}
		}
		if (MultiRaceTimeLeft < 0f)
		{
			if (IsChasing)
			{
				OnWin();
			}
			else
			{
				OnLoose();
			}
		}
	}

	public void OnLoose()
	{
		ObscuredInt multiplayerPursuitLose = _pricesService.MultiplayerPursuitLose;
		V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("FreeDriveControl.YouLostTheRaceNTospend"), $"\n-{multiplayerPursuitLose}$"), 4f);
		AdvancedInt price = new AdvancedInt(multiplayerPursuitLose);
		_currencyManagerController.TryMoneySpend(out var _, price);
		EndRace();
	}

	public void OnWin()
	{
		ObscuredInt multiplayerPursuitWin = _earnMoneyService.MultiplayerPursuitWin;
		V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("FreeDriveControl.YouWonTheRaceNReward"), $"\n+{multiplayerPursuitWin}$"), 4f);
		_currencyManagerController.AddMoney(new AdvancedInt(multiplayerPursuitWin));
		NewRacingMultiplayerController newRacingMultiplayerController = (_freeDriveProvider.DragRacingControlMultiplayer ? _freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController : null);
		if ((bool)newRacingMultiplayerController && newRacingMultiplayerController.runDistance == DragRacingController.RunDistance.Pursuit)
		{
			if (IsChasing)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQMA", "pursuit_race");
			}
			else
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQLw", "pursuit_race");
			}
			_steamAchievementService?.Unlock(IsChasing ? "ACH_CAUGHT_IN_SIXTY" : "ACH_BREAKAWAY");
		}
		if (!IsChasing && (bool)_freeDriveProvider.RacingCar2)
		{
			PhotonView component = _freeDriveProvider.RacingCar2.GetComponent<PhotonView>();
			GetComponent<MultiPlayer>().SendValueStrToPlayer(10, component.OwnerActorNr.ToString(), "", component.Owner);
		}
		EndRace();
	}

	public void SendInvitation(GameObject carToInvite)
	{
		_onTimeOutInviteDisposable?.Dispose();
		PhotonView component = carToInvite.GetComponent<PhotonView>();
		int ownerActorNr = component.OwnerActorNr;
		_freeDriveProvider.RacingCar2 = carToInvite;
		NewRacingMultiplayerController newRacingMultiplayerController = _freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController;
		int closestTree = newRacingMultiplayerController.getClosestTree(V.DriverPerson.transform.position);
		newRacingMultiplayerController.CurrentRacingTree = newRacingMultiplayerController.TreeList[closestTree];
		GetComponent<MultiPlayer>().SendValueStrToPlayer<RpcInvitationReceived>(ownerActorNr.ToString(), closestTree.ToString(), component.Owner);
		chatControl.ShowPartiallyChat();
		MultiPlayer multiPlayer = _freeDriveProvider.MultiPlayer;
		if ((bool)multiPlayer)
		{
			multiPlayer.RaiseRawPositionToAll();
			if ((bool)V.Car && V.Car.TryGetComponent<MultiPlayerPosControl>(out var component2))
			{
				component2.ForceSendNextStream();
			}
		}
		inviteToRaceButton.SetActive(active: false);
		_resetInviteButtonDisposable?.Dispose();
		_resetInviteButtonDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
		{
			ResetInviteButton();
		}).AddTo(this);
		countWaitOfRacing = 0;
		InvitationJustSend = true;
		LastInvitationReceivedId = ownerActorNr;
		_coreGameManager.SetMessage(new LocalizedString("FreeDriveControl.InvitationSent"), 4f);
	}

	public void ResetInviteButton()
	{
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			countWaitOfRacing++;
			if (countWaitOfRacing < 10)
			{
				_resetInviteButtonDisposable?.Dispose();
				_resetInviteButtonDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
				{
					ResetInviteButton();
				}).AddTo(this);
				return;
			}
			if (FlagsStorage.RacingGameMode || FlagsStorage.ChasingRaceModeStarted || (bool)newNegotiationWindow)
			{
				return;
			}
			MonoBehaviour.print("make car 2 null");
			_freeDriveProvider.RacingCar2 = null;
			countWaitOfRacing = 0;
		}
		_resetInviteButtonDisposable?.Dispose();
		InvitationJustSend = false;
		MonoBehaviour.print(" inviteToRaceButton is true");
		inviteToRaceButton.SetActive(active: true);
	}

	public void InvitationReceived()
	{
		foreach (string item in playerToIgnore)
		{
			if (item == LastInvitationReceivedId.ToString())
			{
				CancelInvoke("InvitationReject");
				if (PhotonUtility.TryGetPlayer(LastInvitationReceivedId, out var player))
				{
					GetComponent<MultiPlayer>().SendValueStrToPlayer(8, LastInvitationReceivedId.ToString(), "", player);
					_freeDriveProvider.RacingCar2 = null;
				}
				else
				{
					GetComponent<MultiPlayer>().SendValueStr(8, LastInvitationReceivedId.ToString(), "");
					_freeDriveProvider.RacingCar2 = null;
				}
				return;
			}
		}
		inviteToRaceButton.SetActive(active: false);
		MonoBehaviour.print("ShowSubmitCircle");
		V.CoreGameManager.ManagerUI.ShowSubmitCircle();
	}

	public void PenalizedReset()
	{
		JustPenalized = true;
		_resetAfterDisposable?.Dispose();
		_resetAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
		{
			ResetAfter();
		}).AddTo(this);
	}

	private void ResetAfter()
	{
		JustPenalized = false;
	}

	public void InvitationReject(bool checkHaveReceived = false)
	{
		if (!checkHaveReceived || LastInvitationReceivedId != -99)
		{
			CancelInvoke("InvitationReject");
			GetComponent<MultiPlayer>().SendValueStr(8, LastInvitationReceivedId.ToString(), "");
			_freeDriveProvider.RacingCar2 = null;
			inviteToRaceButton.SetActive(active: true);
			_levelScript.Resume(ignoreBrakes: false, ignoreThrottle: true);
			ClearInvitationId();
		}
	}

	public void ClearInvitationId()
	{
		LastInvitationReceivedId = -99;
	}

	public void InvitationRejectIgnore(bool withIgnore = true)
	{
		if (withIgnore)
		{
			playerToIgnore.Add(LastInvitationReceivedId.ToString());
		}
		InvitationReject();
	}

	public void CancelButtonNegWindow()
	{
		CancelInvoke("InvitationReject");
		if (PhotonUtility.TryGetPlayer(LastInvitationReceivedId, out var player))
		{
			GetComponent<MultiPlayer>().SendValueStrToPlayer(14, LastInvitationReceivedId.ToString(), "", player);
		}
		else
		{
			GetComponent<MultiPlayer>().SendValueStr(14, LastInvitationReceivedId.ToString(), "");
		}
		MonoBehaviour.print("make car 2 null");
		_freeDriveProvider.RacingCar2 = null;
		MonoBehaviour.print(" inviteToRaceButton is true");
		inviteToRaceButton.SetActive(active: true);
		_levelScript.Resume();
	}

	public void SendAllCarDataOfExchangeCar(int idNewOwner)
	{
		_ = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>().ViewID;
	}

	public void ShowFinishMenu()
	{
		_managerUI.CloseWindow("DailyTasksFull");
		_managerUI.ClosePhoneWindow();
		_managerUI.CloseWindow("PlayersListWindow");
		_managerUI.CloseSeveralWindows();
		_levelScript.SetActiveFalseEachMenu(11);
		_levelScript.CleanScreen();
		chatControl.HideAllChat();
		Time.timeScale = 1f;
	}

	public void ShowPlayersList()
	{
		if (!V.DriverPerson.TryGetComponent<MainPerson>(out var component) || !component.PersonAnimationsController || !component.PersonAnimationsController.AnimationService.IsAnimatingPlayer())
		{
			_managerUI.ShowPlayersList();
			_levelScript.CleanScreen();
			if (FlagsStorage.IsMultiFree || ScenesHelper.IsMenuSceneActive())
			{
				Audio.SetPauseSoundFilter(on: true);
			}
			if ((bool)_controlItemsProvider.ThrottleSliderControl)
			{
				_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
			}
		}
	}

	public async UniTask ShowProfile(string playerId)
	{
		MultiPersonControl multiPersonControl = _onlineCollectionsProvider.GetPlayerByPlayerID(playerId);
		if (!multiPersonControl)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.Playerlefttheroom"), 2f).Forget();
			return;
		}
		PlayerProfileWindow playerProfileWindow = ((!FlagsStorage.IsSpectatorMode) ? (await _managerUI.ShowPlayerProfileWindow()) : (_managerUI.GetWindow("PlayerProfileWindow") as PlayerProfileWindow));
		if ((bool)playerProfileWindow)
		{
			playerProfileWindow.OpenInChat(multiPersonControl);
		}
	}

	private void ShowOtherWindows()
	{
		_managerUI.CloseSeveralWindows();
		_levelScript.UnCleanScreen();
	}

	private void ToggleMap(bool isOn)
	{
		if (_freeDriveProvider.MapCamera.ShowingBigMap && !isOn)
		{
			HideBigMap();
		}
		else if (!FlagsStorage.InGarageFree && !FlagsStorage.InGarageMenu)
		{
			ShowBigMap();
		}
	}

	public void ShowBigMap()
	{
		if (!FlagsStorage.IsMainMenu)
		{
			_managerUI.CloseNotificationWindow();
			_levelScript.CleanScreen();
			_levelScript.SetActiveFalseEachMenu(15);
			Time.timeScale = 1f;
			Audio.ResetAllVolume();
			StopWalking();
			_freeDriveProvider.MapCamera.ShowBigMap();
			_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Map, setRoot: true);
		}
	}

	private void StopWalking()
	{
		if (FlagsStorage.OutOfCar && (bool)V.DriverPerson)
		{
			if (V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
			{
				component.StopMove();
			}
			if (V.DriverPerson.TryGetComponent<PersonLocomotion>(out var component2))
			{
				component2.ResetToRest(resetStamina: false);
			}
		}
	}

	public void HideBigMap()
	{
		if (_freeDriveProvider.MapCamera.ShowingBigMap)
		{
			_freeDriveProvider.MapCamera.HideBigMap();
			_levelScript.Resume(ignoreBrakes: true);
			OnBigMapHide?.Invoke();
			_inputMapSetEvent.TrySetRootGroup();
		}
	}

	public async UniTask ShowNegotiationWindow()
	{
		_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.fromTrigger = false;
		_freeDriveProvider.DragRacingControlMultiplayer.Reset();
		_levelScript.SetActiveFalseEachMenu(10);
		_levelScript.CleanScreen();
		chatControl.HideAllChat();
		Time.timeScale = 1f;
		_managerUI.CloseSeveralWindows();
		_managerUI.ShowFreeDragRacingWindow();
		while (newNegotiationWindow == null)
		{
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		}
		newNegotiationWindow.ShowChooseDistance();
	}

	public void ShowGetFuelMenu()
	{
		if (!(_freeDriveProvider.FirstCarMultiPlayer != V.Car))
		{
			_parkingMenuUI.Hide();
			FlagsStorage.IsFreeFuelWindow = true;
			_managerUI.ShowFuelDriveUIWindow();
		}
	}

	public void ShowMoneyPurchaseMenu()
	{
		_managerUI.ShowMoneyPurchaseWindow();
	}

	public void ShowMoneyPurchaseMenu2()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.NoMoney!"), 3f, NotificationType.Error);
	}

	public void ClosePurchaseMenu()
	{
		if (_levelsProvider.LevelScript.prevMenuIndex == 12)
		{
			ShowGetFuelMenu();
		}
		else
		{
			_levelsProvider.LevelScript.Resume();
		}
	}

	public void AcceptChasing()
	{
		CancelInvoke("InvitationReject");
		if (!FlagsStorage.ChasingRaceModeStarted)
		{
			_levelScript.Resume();
			Player player = PhotonNetwork.CurrentRoom.Players[LastInvitationReceivedId];
			if (player != null)
			{
				GetComponent<MultiPlayer>().SendValueStrToPlayer(9, LastInvitationReceivedId.ToString(), "", player.UserId);
			}
			else
			{
				GetComponent<MultiPlayer>().SendValueStr(9, LastInvitationReceivedId.ToString(), "");
			}
			V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.RaceStartedNfollowCarFor1Min"), 6f);
			StartChasingRace();
			IsChasing = true;
			MonoBehaviour.print("make true is chasing");
		}
	}

	public void InvitationAccept()
	{
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			ShowNegotiationWindow();
			if (PhotonUtility.TryGetPlayer(LastInvitationReceivedId, out var player))
			{
				GetComponent<MultiPlayer>().SendValueStrToPlayer(13, LastInvitationReceivedId.ToString(), "", player);
			}
			else
			{
				GetComponent<MultiPlayer>().SendValueStr(13, LastInvitationReceivedId.ToString(), "");
			}
			InvitationHost = false;
		}
	}

	public void InvitationAccepted()
	{
		if (!FlagsStorage.ChasingRaceModeStarted)
		{
			IsChasing = false;
			StartChasingRace();
			V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("FreeDriveControl.RaceStartedNoverTakeBy"), " ", 250f.ToString(), new LocalizedString("FreeDriveControl.MIn1Min")), 6f);
		}
	}

	public void StartChasingRace()
	{
		ShowMainDrivingUI();
		isSelectingCar = false;
		_freeDriveProvider.DragRacingControlMultiplayer.forChasing.SetActive(!FlagsStorage.RacingGameMode);
		GPSTracker.CancelGPSDraw();
		inviteToRaceButton.SetActive(active: false);
		_resetInviteButtonDisposable?.Dispose();
		InvitationJustSend = false;
		chatControl.HideAllChat();
		DistanceCarsRacingMulti = 0f;
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			_multiPlayerPosControl = _freeDriveProvider.RacingCar2.GetComponent<MultiPlayerPosControl>();
			_multiPlayerPosControl.distanceMultiRace = 0f;
		}
		_enableToRoadButtonDisposable?.Dispose();
		toRoadButton.SetActive(value: false);
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFreeRace, setRoot: true);
	}

	public void CloseFinishMenu()
	{
		if (PhotonUtility.TryGetPlayer(LastInvitationReceivedId, out var player))
		{
			GetComponent<MultiPlayer>().SendValueStrToPlayer(19, LastInvitationReceivedId.ToString(), "", player);
		}
		else
		{
			GetComponent<MultiPlayer>().SendValueStr(19, LastInvitationReceivedId.ToString(), "");
		}
		ShowMainDrivingUI();
		if ((bool)newNegotiationWindow)
		{
			newNegotiationWindow.Close();
		}
		_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.OnRaceFinished();
	}

	public void EndRace()
	{
		isSelectingCar = false;
		_freeDriveProvider.DragRacingControlMultiplayer.forChasing.SetActive(value: false);
		FlagsStorage.ChasingRaceModeStarted = false;
		toRoadButton.SetActive(value: true);
		MonoBehaviour.print("make car 2 null");
		_freeDriveProvider.RacingCar2 = null;
		ResetInviteButton();
		if ((bool)V.Car && V.Car.TryGetComponent<MultiPlayerPosControl>(out var component))
		{
			component.ForceSendNextStream();
		}
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFree, setRoot: true);
	}

	private async UniTaskVoid RestoreFreeDriveInterior(VehicleData vehicleData)
	{
		CancellationToken ct = vehicleData.GetCancellationTokenOnDestroy();
		await UniTask.WaitUntil(() => !vehicleData || vehicleData.IsCarGraphicLoaded, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
		if (!ct.IsCancellationRequested && (bool)vehicleData)
		{
			if (CPMGraphicsSettingsManager.Instance.AdditionalEffects != CPMGraphicsSettings.AdditionalEffects.Low)
			{
				await vehicleData.InteriorsComposition.ForceSetSavedInterior(ct);
			}
			else
			{
				vehicleData.InteriorsComposition.TryRestoreSavedInterior();
			}
		}
	}

	public void PreparePedestrian(V.placeToSpawn spawnEnvironment)
	{
		(Transform[] points, int IndexArrayPostionCar) spawnPoints = V.SceneControl.SpawnPointsContainer.GetSpawnPoints(spawnEnvironment);
		Transform[] item = spawnPoints.points;
		V.CurrentIndexArrayPositionCar = spawnPoints.IndexArrayPostionCar;
		Transform randomSpawnPoint = item.GetRandomSpawnPoint();
		V.Car = null;
		GameObject gameObject = V.CoreGameManager.EnsurePlayerPerson(null);
		if (FlagsStorage.IsFreeDriveMode && (bool)V.CoreGameManager.globalPoint)
		{
			gameObject.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
		gameObject.transform.position = randomSpawnPoint.position + Vector3.up;
		gameObject.transform.eulerAngles = randomSpawnPoint.eulerAngles;
		if (gameObject.TryGetComponent<Collider>(out var component))
		{
			component.enabled = true;
		}
		if (gameObject.TryGetComponent<Rigidbody>(out var component2))
		{
			component2.constraints = RigidbodyConstraints.FreezeRotation;
			component2.isKinematic = true;
		}
		FlagsStorage.OutOfCar = true;
		V.PersonCamIndex = PlayerPrefs.GetInt("PersonCamIndex", 0);
		_carCamerasController.SetCamera(V.PersonCamIndex, gameObject.transform, newTarget: true);
		StartSceneTraffic();
		EnablePedestrianPhysicsWhenReady(gameObject);
	}

	private void EnablePedestrianPhysicsWhenReady(GameObject person)
	{
		FlagsStorage.FirstMainSceneLoaded.Where((bool loaded) => loaded).First().Delay(TimeSpan.FromSeconds(0.5))
			.Subscribe((bool _) =>
			{
				if ((bool)person && person.TryGetComponent<Rigidbody>(out var component))
				{
					component.isKinematic = false;
				}
			})
			.AddTo(this);
	}

	public async UniTaskVoid PrepareCar(V.placeToSpawn spawnEnvironment)
	{
		Vector3 pos = Vector3.zero;
		Vector3 r = Vector3.zero;
		bool usedGarageExitPoint = false;
		float positionOffset = 1f;
		if (!FlagsStorage.IsMultiFree)
		{
			if (spawnEnvironment == V.placeToSpawn.GarageEntrance)
			{
				Transform transform = null;
				int num = 0;
				if (_garageSelectorService != null)
				{
					num = _garageSelectorService.CurrentGarageIndex;
				}
				if ((bool)_garageController && _garageController.FreeExitPointsByGarages != null && num >= 0 && num < _garageController.FreeExitPointsByGarages.Count)
				{
					transform = _garageController.FreeExitPointsByGarages[num];
					_ = (bool)transform;
				}
				spawnEnvironment = num switch
				{
					0 => V.placeToSpawn.MainCity, 
					1 => V.placeToSpawn.Desert, 
					_ => spawnEnvironment, 
				};
				if (transform != null)
				{
					pos = transform.position;
					usedGarageExitPoint = true;
					r = transform.eulerAngles + (FlagsStorage.EnteredForwardInGarageTunnel ? Vector3.zero : new Vector3(0f, 180f, 0f));
					V.CurrentIndexArrayPositionCar = 860;
				}
				else if (V.SceneControl != null && V.SceneControl.SpawnPointsContainer != null)
				{
					(Transform[] points, int IndexArrayPostionCar) spawnPoints = V.SceneControl.SpawnPointsContainer.GetSpawnPoints(spawnEnvironment);
					Transform[] item = spawnPoints.points;
					V.CurrentIndexArrayPositionCar = spawnPoints.IndexArrayPostionCar;
					Transform randomSpawnPoint = item.GetRandomSpawnPoint();
					pos = randomSpawnPoint.position;
					r = randomSpawnPoint.eulerAngles;
				}
			}
			else
			{
				(Transform[] points, int IndexArrayPostionCar) spawnPoints2 = V.SceneControl.SpawnPointsContainer.GetSpawnPoints(spawnEnvironment);
				Transform[] item2 = spawnPoints2.points;
				V.CurrentIndexArrayPositionCar = spawnPoints2.IndexArrayPostionCar;
				Transform randomSpawnPoint2 = item2.GetRandomSpawnPoint();
				pos = randomSpawnPoint2.position;
				r = randomSpawnPoint2.eulerAngles;
				if (spawnEnvironment == V.placeToSpawn.Highway)
				{
					current_tar_index = 447;
					current_tar_City2 = 400;
				}
			}
			V.CurrentIndexArrayPositionCar -= 15;
			if ((bool)V.SceneControl && FlagsStorage.StartInHome)
			{
				int index = ObscuredPrefs.GetInt("homeSelected");
				pos = _homesConfig.Get(index).Positions.CarPosition;
				r = _homesConfig.Get(index).Positions.CarEulerAngles;
			}
			pos.y += positionOffset;
		}
		CarSlot slot = V.CoreGameManager.SlotService.GetSlot(V.SlotID);
		if (slot == null || slot.IsEmpty)
		{
			foreach (CarSlot allSlotsInfo in V.CoreGameManager.SlotService.AllSlotsInfos)
			{
				if (allSlotsInfo != null && !allSlotsInfo.IsEmpty)
				{
					V.SlotID = allSlotsInfo.ID;
					break;
				}
			}
		}
		if (!FlagsStorage.IsMultiFree)
		{
			V.CoreGameManager.EnsurePlayerPerson(null);
		}
		V.Car = (await _carLoadingUtility.LoadCarFromSlot(V.SlotID, pos, Quaternion.Euler(Vector3.zero), tryUnloadCurrent: false, waitBodyLoad: false, FlagsStorage.IsMultiFree)).Item2;
		if (this == null || !V.Car || V.CoreGameManager == null)
		{
			return;
		}
		if (!ScenesHelper.IsMenuSceneActive() && !ScenesHelper.IsParkingLevelActive() && V.Car.TryGetComponent<Powertrain>(out var component) && !component.IsEngineWorks)
		{
			component.StartEngine(starterSound: false);
		}
		if (!FlagsStorage.IsMultiFree)
		{
			Vector3 vector = (usedGarageExitPoint ? Vector3.zero : (Vector3.up * (V.Car.GetComponent<VehicleData>().GroundOffset + 0.2f)));
			V.Car.transform.position += Vector3.down * positionOffset + vector;
		}
		GameObject car = V.Car;
		StartSceneTraffic();
		if (FlagsStorage.IsFreeDriveMode)
		{
			V.Car.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
		car.GetComponent<VehicleData>().KeepRowComposition.CurrentTargetIndex = V.CurrentIndexArrayPositionCar;
		if (!FlagsStorage.IsMultiFree)
		{
			_freeDriveProvider.FirstCarMultiPlayer = V.Car;
			V.Car.transform.eulerAngles = r;
		}
		else
		{
			car.GetComponent<MultiPlayerPosControl>().InterestGroup = InterestGroupManager.CarInterestGroup;
		}
		if ((bool)_freeDriveProvider.QuickCarSettings)
		{
			_freeDriveProvider.QuickCarSettings.PreSetValues();
		}
		Rigidbody component2 = V.Car.GetComponent<Rigidbody>();
		CarTriggers carTriggers = V.Car.AddComponent<CarTriggers>();
		carTriggers.LevelScript = _levelScript;
		carTriggers.FreeDriveProvider = _freeDriveProvider;
		carTriggers.LevelsProvider = _levelsProvider;
		if (!V.Car.GetComponent<Dragy>())
		{
			V.Car.AddComponent<Dragy>();
		}
		PlayerDatas.CreateSkid(V.Car);
		_freeDriveProvider.CarCameras.NewCarEntered(V.Car);
		V.CurrentCamIndex = PlayerPrefs.GetInt("LastCam", 2);
		_carCamerasController.SetCamera(V.CurrentCamIndex, V.Car.transform, newTarget: true);
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component3))
		{
			RestoreFreeDriveInterior(component3).Forget();
		}
		component2.isKinematic = true;
		fristSpawnCar = true;
		V.CoreGameManager.SetupCarDriver(V.Car);
		bool showWaitingWindow = NeedDelayedEnableRbInScene(spawnEnvironment) && FlagsStorage.IsMultiFree;
		EnableRigidBodyWhenRoadReady(5f, showWaitingWindow).Forget();
		_managerUI.ScreenFade.FadeIn();
		if ((bool)V.SceneControl)
		{
			switch (spawnEnvironment)
			{
			case V.placeToSpawn.MainCity:
			case V.placeToSpawn.GarageEntrance:
				V.SceneControl.LoadEnvironmentByIndex(0);
				break;
			case V.placeToSpawn.Highway:
				V.SceneControl.LoadEnvironmentByIndex(1);
				break;
			case V.placeToSpawn.Desert:
				V.SceneControl.LoadEnvironmentByIndex(2);
				break;
			case V.placeToSpawn.Mountain:
				V.SceneControl.LoadEnvironmentByIndex(3);
				break;
			case V.placeToSpawn.Racing:
				V.SceneControl.LoadEnvironmentByIndex(4);
				break;
			case V.placeToSpawn.City2:
				V.SceneControl.LoadEnvironmentByIndex(5);
				break;
			case V.placeToSpawn.Offroad:
				V.SceneControl.LoadEnvironmentByIndex(11);
				break;
			case V.placeToSpawn.JapanSerpentine:
				V.SceneControl.LoadEnvironmentByIndex(13);
				break;
			case V.placeToSpawn.JapanSuburban:
				V.SceneControl.LoadEnvironmentByIndex(14);
				break;
			case V.placeToSpawn.TropicalIsland:
				V.SceneControl.LoadEnvironmentByIndex(17);
				break;
			case V.placeToSpawn.Nurburgring:
				V.SceneControl.LoadEnvironmentByIndex(18);
				break;
			case V.placeToSpawn.Italy:
				V.SceneControl.LoadEnvironmentByIndex(20);
				break;
			}
		}
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQCw", "free_drive_spawn");
		_steamAchievementService?.Unlock("ACH_IGNITION_ON");
	}

	private bool NeedDelayedEnableRbInScene(V.placeToSpawn placeToSpawn)
	{
		if (placeToSpawn != V.placeToSpawn.Racing)
		{
			return placeToSpawn == V.placeToSpawn.JapanSerpentine;
		}
		return true;
	}

	public void SwitchLightControllers(bool isOn)
	{
		GameObject[] trafficLights = TrafficLights;
		for (int i = 0; i < trafficLights.Length; i++)
		{
			trafficLights[i].SetActive(isOn);
		}
	}

	private void StartSceneTraffic()
	{
		if (ScenesHelper.IsFreeDriveSceneActive())
		{
			bool flag = PlayerPrefs.GetInt("CarQuantity", 10) > 0;
			if ((!FlagsStorage.IsMultiFree && SystemInfo.systemMemorySize > 1100) & flag)
			{
				InitTrafficWhenCameraReady().Forget();
			}
			else
			{
				TrafficComponent.InitTrafficLightsOnly().Forget();
			}
			SwitchLightControllers(isOn: false);
		}
	}

	private async UniTaskVoid InitTrafficWhenCameraReady()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		Camera sceneCamera = GetComponent<Camera>();
		await UniTask.WaitUntil(() => sceneCamera.enabled, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		GameObject gameObject = (V.Car ? V.Car : V.LastUsedCar);
		TrafficComponent.Init(sceneCamera.transform).Forget();
		if ((bool)gameObject && !gameObject.GetComponent<PlayerComponent>())
		{
			gameObject.AddComponent<PlayerComponent>();
		}
	}

	private async UniTaskVoid EnableRigidBodyWhenRoadReady(float timeoutSeconds, bool showWaitingWindow = false)
	{
		if (showWaitingWindow)
		{
			await _managerUI.ShowWaitingWindowNotTransparent(set: true);
		}
		CancellationToken ct = this.GetCancellationTokenOnDestroy();
		int roadMask = LayerMask.GetMask("Road", "Terrain", "Default");
		float start = Time.realtimeSinceStartup;
		float absoluteStart = start;
		bool roadReady = false;
		try
		{
			while ((bool)V.Car)
			{
				roadReady = Physics.Raycast(V.Car.transform.position + Vector3.up * 5f, Vector3.down, 60f, roadMask, QueryTriggerInteraction.Ignore);
				if (roadReady)
				{
					break;
				}
				if (SceneLoadTracker.HasPendingLoads)
				{
					start = Time.realtimeSinceStartup;
				}
				if (Time.realtimeSinceStartup - start >= timeoutSeconds || Time.realtimeSinceStartup - absoluteStart >= 40f)
				{
					break;
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0), DelayType.Realtime, PlayerLoopTiming.Update, ct);
			}
			if ((bool)V.Car)
			{
				_ = roadReady;
				ResetRigidBody();
			}
		}
		finally
		{
			if (showWaitingWindow && (bool)_managerUI)
			{
				_managerUI.CloseNotTransparentWaitingWindow();
			}
		}
	}

	private void ResetRigidBody()
	{
		if ((bool)V.Car)
		{
			V.Car.GetComponent<Rigidbody>().isKinematic = false;
		}
		fristSpawnCar = false;
	}

	private void onTimeOutInvite()
	{
		_onTimeOutInviteDisposable?.Dispose();
		isSelectingCar = false;
		inviteToRaceButton.SetActive(active: true);
		_parkingMenuUI.ShowInvitationPanel(isActive: false);
	}

	public void CancelInvite()
	{
		_freeDriveProvider.DragRacingControlMultiplayer.forChasing.SetActive(value: false);
		_parkingMenuUI.ShowInvitationPanel(isActive: false, _isInitStart);
		isSelectingCar = false;
		FlagsStorage.ChasingRaceModeStarted = false;
		_onTimeOutInviteDisposable?.Dispose();
	}

	public void ShowMainDrivingUI()
	{
		_levelScript.SetActiveFalseEachMenu(1);
		_levelScript.UnCleanScreen();
		_levelScript.Resume();
		if (FlagsStorage.IsChatAllowed)
		{
			chatControl.CreateChatWindow();
		}
	}

	private void WwiseFilterOff()
	{
		AudioWrapper.SetState("state_filter", "out");
		AudioWrapper.SetState("apartment", "outo");
		AudioWrapper.SetState("Amb", "SoundOn");
		AudioWrapper.SetState("State_Room", "out_room");
		AmbienceStateZones.ReapplyOccupiedZones();
	}

	public void ToRoad()
	{
		ExecuteToRoad(ToRoadSource.Button);
	}

	public void ToRoad(ToRoadSource source)
	{
		ExecuteToRoad(source);
	}

	public void CarToRoad(GameObject car, ToRoadSource source = ToRoadSource.OutOfBounds)
	{
		ExecuteToRoad(source, car);
	}

	private void ExecuteToRoad(ToRoadSource source, GameObject car = null)
	{
		if (FlagsStorage.IsInGarageTunnel || IsAnimating())
		{
			return;
		}
		if ((bool)car || !FlagsStorage.OutOfCar)
		{
			car = (car ? car : GetCurrentCar());
			if (!car)
			{
				return;
			}
			if (FlagsStorage.IsPassenger && !CanControlCar(car))
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FreeDriveControl.YouAreJustPassenger"), 2f);
				return;
			}
			if (FlagsStorage.RaceStarted || InvitationJustSend || !CanControlCar(car))
			{
				return;
			}
		}
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<WorldInteractionController>(out var component))
		{
			component.CancelForTeleport();
		}
		ScheduleWwiseFilterOff();
		ToRoadRequest request = new ToRoadRequest(V.DriverPerson, car, source);
		ToRoadResult result = _toRoadRouter.Resolve(in request);
		if (result.Type == ToRoadResult.Kind.Place)
		{
			ApplyToRoadResult(in request, in result);
			GPSTracker.RecalcPath();
		}
	}

	private bool IsAnimating()
	{
		bool num = (bool)V.DriverPerson && V.DriverPerson.TryGetComponentInChildren<PersonAnimationsController>(out var component) && (component.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate || component.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate);
		bool flag = (bool)V.Car && V.Car.TryGetComponent<CarAnimationsController>(out var component2) && (bool)component2.CarAnimationReferences && component2.CarAnimationReferences.IsAnimatingSeats;
		return num | flag;
	}

	private GameObject GetCurrentCar()
	{
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component))
		{
			return component.currentCar;
		}
		return null;
	}

	public static bool CanControlCar(GameObject car)
	{
		if (!car.TryGetComponent<VehicleData>(out var component))
		{
			return false;
		}
		bool flag = component.Driver;
		bool flag2 = !component.photonView || component.photonView.IsMine;
		if (FlagsStorage.IsPassenger)
		{
			return !flag & flag2;
		}
		return true;
	}

	public static bool IsPassengerWithoutControl()
	{
		if (FlagsStorage.IsPassenger && (bool)V.Car)
		{
			return !CanControlCar(V.Car);
		}
		return false;
	}

	private void ScheduleWwiseFilterOff()
	{
		_wwiseFilterOffDisposable?.Dispose();
		_wwiseFilterOffDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			WwiseFilterOff();
		}).AddTo(this);
	}

	private void ApplyToRoadResult(in ToRoadRequest request, in ToRoadResult result)
	{
		if (request.MovesPerson)
		{
			ApplyToPerson(request.Person, in result);
		}
		else
		{
			ApplyToCar(request.Car, in result);
		}
	}

	private void ApplyToPerson(GameObject person, in ToRoadResult result)
	{
		if (!person)
		{
			return;
		}
		if (person.TryGetComponent<DriverActionController>(out var component) && (bool)component.ragdollCpm)
		{
			component.ragdollCpm.ReleaseForTeleport();
		}
		if (person.TryGetComponent<PersonLocomotion>(out var component2))
		{
			component2.Teleport(result.Position, result.Rotation);
			component2.ResetStamina();
		}
		else
		{
			person.transform.SetPositionAndRotation(result.Position, result.Rotation);
			if (person.TryGetComponent<Rigidbody>(out var component3))
			{
				component3.position = result.Position;
				component3.rotation = result.Rotation;
				component3.velocity = Vector3.zero;
			}
		}
		Physics.SyncTransforms();
		if (result.LoadEnvironment)
		{
			LoadEnvironmentBySphere(V.SceneControl.getEnvironmentByColliders(person.transform.position));
		}
	}

	private void ApplyToCar(GameObject car, in ToRoadResult result)
	{
		if (!car)
		{
			return;
		}
		OnCarToRoad?.Invoke(car);
		StraightRoadTrigger.SetActive(value: false);
		if (!V.Car && car == _freeDriveProvider.FirstCarMultiPlayer)
		{
			V.LastUsedCar = (V.Car = _freeDriveProvider.FirstCarMultiPlayer);
		}
		ConfigurableJoint component = car.GetComponent<ConfigurableJoint>();
		ClientController clientController = (((bool)component && (bool)component.connectedBody) ? component.connectedBody.GetComponent<ClientController>() : null);
		bool flag = car.TryGetComponent<VehicleData>(out var component2) && (bool)component2.Driver;
		if (car.TryGetComponent<Powertrain>(out var component3))
		{
			component3.ResetTransmission(resetGear: false);
			if (!flag)
			{
				component3.Throttle = 0f;
			}
		}
		_carInputManager.ClearAllInputs();
		car.transform.SetPositionAndRotation(result.Position, result.Rotation);
		if (result.LoadEnvironment)
		{
			LoadEnvironmentBySphere(V.SceneControl.getEnvironmentByColliders(car.transform.position));
		}
		if (clientController != null)
		{
			clientController.Attach(car.transform);
		}
		toRoadButton.SetActive(value: false);
		_enableToRoadButtonDisposable?.Dispose();
		_enableToRoadButtonDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			EnableToRoadButton();
		}).AddTo(this);
		_managerUI.HideActionButton();
	}

	public void LoadEnvironmentBySphere(int index)
	{
		switch (index)
		{
		case 0:
			V.SceneControl.LoadMainCity(immediately: true);
			break;
		case 1:
			V.SceneControl.LoadHighway2(immediately: true);
			break;
		case 2:
			V.SceneControl.LoadDesert(immediately: true);
			break;
		case 3:
			V.SceneControl.LoadMountain(immediately: true);
			break;
		case 4:
			V.SceneControl.LoadRaceTrack(immediately: true);
			break;
		case 5:
			V.SceneControl.LoadCity2(immediately: true);
			break;
		case 6:
			V.SceneControl.LoadIsland(immediately: true);
			break;
		case 7:
			V.SceneControl.LoadRoad1(immediately: true);
			break;
		case 8:
			V.SceneControl.LoadRoad2(immediately: true);
			break;
		case 9:
			V.SceneControl.LoadStraightRoad(immediately: true);
			break;
		case 10:
			V.SceneControl.LoadHome2(immediately: true);
			break;
		case 11:
			V.SceneControl.LoadOffRoad(immediately: true);
			break;
		case 12:
			V.SceneControl.LoadRichmond(immediately: true);
			break;
		case 13:
			V.SceneControl.LoadJapanScene(immediately: true);
			break;
		case 14:
			V.SceneControl.LoadJapanSuburbanScene(immediately: true);
			break;
		case 15:
			V.SceneControl.LoadRichIsland(immediately: true);
			break;
		case 21:
			V.SceneControl.LoadRichIslandRoad(immediately: true);
			break;
		case 20:
			V.SceneControl.LoadItaly(immediately: true);
			break;
		case 16:
			V.SceneControl.LoadCityMainRaceTrack(immediately: true);
			break;
		case 17:
			V.SceneControl.LoadTropicalIsland(immediately: true);
			break;
		case 18:
			V.SceneControl.LoadNurburgring(immediately: true);
			break;
		case 19:
			V.SceneControl.LoadJunctionNurburgring(immediately: true);
			break;
		}
	}

	public void DisableToRoadButton()
	{
		if ((bool)toRoadButton)
		{
			toRoadButton.SetActive(value: false);
		}
	}

	public void EnableToRoadButton()
	{
		if ((bool)toRoadButton)
		{
			toRoadButton.SetActive(value: true);
		}
	}

	public void ShowActionButton(int index, string text = "", CostType costType = CostType.Money)
	{
		actionIndex = index;
		bool isPurchase = false;
		switch (index)
		{
		case 1:
			text = (CarLifter.GetClosestLifter(base.transform).Lifting ? new LocalizedString("CarLifter.Down") : new LocalizedString("CarLifter.Lift"));
			break;
		case 4:
			ShowMoneyPurchaseMenu2();
			_managerUI.HideActionButton();
			isPurchase = true;
			break;
		case 6:
			isPurchase = true;
			break;
		}
		_managerUI.ShowActionButton(text, isPurchase, costType);
	}

	public void HideActionButton()
	{
		_managerUI.HideActionButton();
	}

	public void OnActionButtonPressed()
	{
		if (actionIndex == 1)
		{
			CarLifter carLifter = ClosestLifter();
			if ((bool)carLifter)
			{
				carLifter.ChangeMoveDirection();
			}
		}
		else if (actionIndex == 4)
		{
			ShowMoneyPurchaseMenu();
		}
		else if (actionIndex == 7)
		{
			ShowGetFuelMenu();
		}
	}

	public async UniTask<bool> ProcessApartmentPurchase(HomesConfig.HomeDataItem homeData)
	{
		LogBuyApartment(actionIndex, homeData, isBought: false, PurchaseStatus.PURCHASE_CONFIRMED);
		bool result = await ApartmentsBuyer.BuyHomeInFree(_currencyManagerController, homeData);
		LogBuyApartment(actionIndex, homeData, result);
		return result;
	}

	public bool BuyFastFoodBuilding(int id)
	{
		if (_businessService.HasBought(id))
		{
			return false;
		}
		bool num = _businessService.BuyBusiness(id);
		if (num)
		{
			Action<int, bool> action = OnFastFoodBuildBought;
			if (action == null)
			{
				return num;
			}
			action(id, arg2: true);
		}
		return num;
	}

	public void StartUpdateCurrentRow()
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			targetsMountainBig = _sort2.MountainRoadRoundBigV;
			targetsMainLoop = _sort2.row4TrafficArrayV;
			targetsCity2 = _sort2.row3City2;
			if (V.CurrentEnvironment == V.placeToSpawn.MainCity)
			{
				current_tar_index = 830;
				current_tar_indexMountain = 150;
				current_tar_City2 = 1;
			}
			else if (V.CurrentEnvironment == V.placeToSpawn.Highway)
			{
				current_tar_index = 830;
				current_tar_indexMountain = 150;
				current_tar_City2 = 350;
			}
			else if (V.CurrentEnvironment == V.placeToSpawn.Desert)
			{
				current_tar_City2 = 1;
			}
			else if (V.CurrentEnvironment == V.placeToSpawn.Mountain)
			{
				current_tar_index = 1000;
				current_tar_indexMountain = 150;
				current_tar_City2 = 1;
			}
			else if (V.CurrentEnvironment == V.placeToSpawn.City2)
			{
				current_tar_index = 830;
				current_tar_indexMountain = 150;
				current_tar_City2 = 630;
			}
			_updateCurrentRowTick?.Dispose();
			_updateCurrentRowTick = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(50.0)).Subscribe((long _) =>
			{
				UpdateCurrentRow();
			}).AddTo(this);
		}
	}

	private void UpdateCurrentRow()
	{
		if (!V.DriverPerson)
		{
			return;
		}
		_smallest = FindClose();
		_smallest2 = FindCloseMountain();
		smallestCity2 = FindCloseCity2();
		if (_smallest < 20f && _smallest2 < 20f)
		{
			if (V.CurrentEnvironment == V.placeToSpawn.Mountain && FlagsStorage.CloserMountain)
			{
				TargetToInnerLoop();
			}
			else
			{
				TargetToOuterLoop();
			}
			return;
		}
		float num = Mathf.Min(_smallest, _smallest2, smallestCity2);
		if (num == _smallest)
		{
			TargetToOuterLoop();
		}
		else if (num == _smallest2)
		{
			TargetToInnerLoop();
		}
		else
		{
			TargetToCity2();
		}
	}

	private void TargetToOuterLoop()
	{
		V.CurrentIndexArrayPositionCar = current_tar_index;
		V.DirectionCar = DirectionMainLoop;
		FlagsStorage.CloserMountain = false;
		FlagsStorage.CloserCity2 = false;
	}

	private void TargetToInnerLoop()
	{
		V.CurrentIndexArrayPositionCar = current_tar_indexMountain;
		V.DirectionCar = DirectionMountain;
		FlagsStorage.CloserMountain = true;
		FlagsStorage.CloserCity2 = false;
	}

	private void TargetToCity2()
	{
		V.CurrentIndexArrayPositionCar = current_tar_City2;
		V.DirectionCar = DirectionCity2;
		FlagsStorage.CloserMountain = false;
		FlagsStorage.CloserCity2 = true;
	}

	private float FindCloseMountain()
	{
		float num = 100000f;
		if (!V.DriverPerson)
		{
			mainPoint = base.transform;
		}
		else
		{
			mainPoint = V.DriverPerson.transform;
		}
		if (current_tar_indexMountain < targetsMountainBig.Length - 1 && current_tar_indexMountain > 1)
		{
			float num2 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMountainBig[current_tar_indexMountain].x, targetsMountainBig[current_tar_indexMountain].z));
			float num3 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMountainBig[current_tar_indexMountain - 1].x, targetsMountainBig[current_tar_indexMountain - 1].z));
			float num4 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMountainBig[current_tar_indexMountain + 1].x, targetsMountainBig[current_tar_indexMountain + 1].z));
			num = Mathf.Min(num2, num3, num4);
			if (num != num2)
			{
				if (num == num3)
				{
					current_tar_indexMountain--;
					DirectionMountain = -1;
					if (current_tar_indexMountain <= 1)
					{
						current_tar_indexMountain = targetsMountainBig.Length - 2;
					}
				}
				else if (num == num4)
				{
					current_tar_indexMountain++;
					if (current_tar_indexMountain > targetsMountainBig.Length - 2)
					{
						current_tar_indexMountain = 2;
					}
					DirectionMountain = 1;
				}
			}
		}
		return num;
	}

	private float FindClose()
	{
		if (!V.DriverPerson)
		{
			mainPoint = base.transform;
		}
		else
		{
			mainPoint = V.DriverPerson.transform;
		}
		float num = 200f;
		if (current_tar_index <= 1)
		{
			current_tar_index = 2;
		}
		if (current_tar_index < targetsMainLoop.Length - 1 && current_tar_index > 1)
		{
			float num2 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMainLoop[current_tar_index].x, targetsMainLoop[current_tar_index].z));
			float num3 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMainLoop[current_tar_index - 1].x, targetsMainLoop[current_tar_index - 1].z));
			float num4 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsMainLoop[current_tar_index + 1].x, targetsMainLoop[current_tar_index + 1].z));
			num = Mathf.Min(num2, num3, num4);
			if (num != num2)
			{
				if (num == num3)
				{
					current_tar_index--;
					DirectionMainLoop = -1;
				}
				else if (num == num4)
				{
					if (current_tar_index < targetsMainLoop.Length - 2)
					{
						current_tar_index++;
					}
					DirectionMainLoop = 1;
				}
			}
		}
		return num;
	}

	private float FindCloseCity2()
	{
		if (!V.DriverPerson)
		{
			mainPoint = base.transform;
		}
		else
		{
			mainPoint = V.DriverPerson.transform;
		}
		float num = 200f;
		if (current_tar_City2 > 617 && current_tar_City2 < 626 && !FlagsStorage.KeepRow)
		{
			if (DirectionCity2 > 0)
			{
				current_tar_City2 = 628;
			}
			else
			{
				current_tar_City2 = 616;
			}
		}
		if (current_tar_City2 <= 1)
		{
			current_tar_City2 = 2;
		}
		if (current_tar_City2 < targetsCity2.Length - 1 && current_tar_City2 > 1)
		{
			float num2 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsCity2[current_tar_City2].x, targetsCity2[current_tar_City2].z));
			float num3 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsCity2[current_tar_City2 - 1].x, targetsCity2[current_tar_City2 - 1].z));
			float num4 = Vector2.Distance(new Vector2(mainPoint.localPosition.x, mainPoint.localPosition.z), new Vector2(targetsCity2[current_tar_City2 + 1].x, targetsCity2[current_tar_City2 + 1].z));
			num = Mathf.Min(num2, num3, num4);
			if (num != num2)
			{
				if (num == num3)
				{
					current_tar_City2--;
					DirectionCity2 = -1;
				}
				else if (num == num4)
				{
					if (current_tar_City2 < targetsCity2.Length - 2)
					{
						current_tar_City2++;
					}
					DirectionCity2 = 1;
				}
			}
		}
		return num;
	}

	public void UnloadFromTowCar()
	{
		if (!V.Car)
		{
			return;
		}
		VehicleData component = V.Car.GetComponent<VehicleData>();
		if (component.CarId != 101)
		{
			return;
		}
		component.SpacerOpen = !component.SpacerOpen;
		component.spacerMove1.ExtendSpacer(component.SpacerOpen);
		component.spacerMove2.ExtendSpacer(component.SpacerOpen);
		if (FlagsStorage.IsMultiFree && !FlagsStorage.IsPassenger && !component.isGarageCar)
		{
			int num = 0;
			if (component.SpacerOpen)
			{
				num = 1;
			}
			PhotonView component2 = V.Car.GetComponent<PhotonView>();
			_freeDriveProvider.MultiPlayer.SendValueStr(40, component2.ViewID.ToString(), num.ToString());
		}
		if (component.SpacerOpen && (bool)component.TowingComposition.LoadCar && !component.isGarageCar)
		{
			component.TowingComposition.UnTowMultiplayer(component.TowingComposition.LoadCar);
		}
	}

	private CarLifter ClosestLifter()
	{
		CarLifter result = null;
		if (FlagsStorage.IsFreeDriveMode)
		{
			int num = int.MaxValue;
			foreach (CarLifter carLifter in V.SceneControl.carLifters)
			{
				if ((bool)carLifter)
				{
					int num2 = (int)Vector3.Distance(base.transform.position, carLifter.transform.position);
					if (num2 < num)
					{
						num = num2;
						result = carLifter;
					}
				}
			}
		}
		return result;
	}

	private void OnLowMemory()
	{
		_menuLevelParent.OpenMainMenu();
	}

	public void ActivateCarLift()
	{
		ShowActionButton(1);
		CarLifter.ClosestLifter.StartWaitUntilCarMovesAway();
	}

	private void LogBuyApartment(int apartmentId, HomesConfig.HomeDataItem homeData, bool isBought, PurchaseStatus purchaseStatus = PurchaseStatus.NONE)
	{
		if (purchaseStatus == PurchaseStatus.NONE)
		{
			purchaseStatus = (isBought ? PurchaseStatus.PURCHASE_COMPLETED : PurchaseStatus.INSUFFICIENT_FUNDS);
		}
		CurrencyItemType currencyItemType = ((homeData.PriceType == CostType.Money) ? CurrencyItemType.money : CurrencyItemType.coins);
		_virtualPurchaseAnalytic.LogPurchaseVirtual_Apartment(apartmentId, currencyItemType, homeData.GetPrice, _virtualPurchaseAnalytic.BuyApartmentInWorld, purchaseStatus);
	}

	public bool GetToRoadButtonActiveState()
	{
		if ((bool)toRoadButton)
		{
			return toRoadButton.activeSelf;
		}
		return false;
	}
}
