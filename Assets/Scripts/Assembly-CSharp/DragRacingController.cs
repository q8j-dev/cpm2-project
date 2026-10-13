using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Connection;
using CPM.Database;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using SWS;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.WindowSystem;

public class DragRacingController : MonoBehaviour
{
	public delegate void OnRestart();

	public enum RunDistance
	{
		QuaterMile = 0,
		OneMile = 1,
		Lap = 2,
		Rally = 3,
		Pursuit = 4,
		HalfMile = 5
	}

	private class NextRandomCars
	{
		public MainCarValues car1Values;

		public MainCarValues car2Values;
	}

	[Serializable]
	public class Garage
	{
		public PathManager path;

		public Transform carPos;

		public GameObject car;
	}

	public Transform spawnPoint;

	public DragRacingCamera dragRacingCamera;

	public DragRacingUI dragRacingUI;

	public GameObject DragRacingCar1;

	public GameObject DragRacingCar2;

	public GameObject leftCar;

	public GameObject rightCar;

	public PathManager path1;

	public PathManager path2;

	public DragRacingTree dragRacingTree;

	public List<Garage> Garages;

	public int RaceWon;

	public int raceCount;

	public bool raceInProcess;

	public bool raceFor3Place;

	public int timeClass = 12;

	public int carRacingID = -1;

	public GameObject nextRivalCar;

	public bool recordFinishResult;

	public bool focusOnOtherCar;

	public bool autoSteering;

	public int allRacersCount = 16;

	public AllRacingCarClassList allRacingCarClassList;

	public MainCarValues[] nowRacingCarClassList;

	public MainCarValues[] winners;

	public List<GameObject> allCars;

	public podium podium;

	public GameObject ourCar;

	public Dragy dragy;

	public bool alwaysWin = true;

	public DragRacingConfig DragRacingConfig;

	public RunDistance runDistance;

	public GameObject[] for1_4Mile;

	public GameObject[] for1_2Mile;

	public Material defCarMat;

	public MainCarValues testCarToLoad;

	public bool testRun;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	private RaceFinishUI _raceFinishUI;

	private bool camOnFinishShowed;

	private bool _finishResultPending;

	private bool _pendingGarageReturn;

	private IPrefabFactory _prefabFactory;

	public UITimeBannerController UITimeBannerController;

	private ScreenFader _screenFader;

	private float _bestTime = 1000f;

	private int _maxClassTime = 13;

	private readonly int _maxClassCount = 6;

	private NextRandomCars nextRandomCars;

	private IPlayerStorageService _playerStorageService;

	public bool qualification;

	public SoundEventChannel SoundEventChannel;

	private int _lastOwnCarPlaceForWin;

	private IPlayerStatusSystem _playerStatusSystem;

	private int _finishCarsCount;

	private bool _isRightCarWinner;

	private bool _isLeftCarWinner;

	private IDisposable _placePersonOnGarageDisposable;

	private IDisposable _setPosOfNextCarDisposable;

	private IDisposable _randomOtherCarRaceDisposable;

	private IDisposable _prepareNextRandomCarsDataDisposable;

	private IGuestAccountCreator _guestAccountCreator;

	private ManagerUI _managerUI;

	private IDragRacingRewardService _dragRacingRewardService;

	private IConnectionManager _connectionManager;

	private InputItemsProvider _controlItemsProvider;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	private FreeDriveProvider _freeDriveProvider;

	private RacingProvider _racingProvider;

	private RacingAnalytic _racingAnalytic;

	private CarInputManager _carInputManager;

	private AllCarsInfo _allCarsInfo;

	private const int NumberOfRacesWonForFirstPlace = 5;

	private float _dragRacingStartTime;

	private InputMapSetEvent _inputMapSetEvent;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _backToCarHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BackToCar);

	private ISteamAchievementService _steamAchievementService;

	private Button _toCarButton;

	public long PassedInModeTime => (long)((Time.time - _dragRacingStartTime) * 1000f);

	public bool IsRandomCarsRace { get; private set; }

	public int Place
	{
		get
		{
			if (RaceWon == 5)
			{
				if (raceFor3Place)
				{
					if (!dragy.lost)
					{
						return 3;
					}
					return 4;
				}
				if (dragy.lost)
				{
					return 2;
				}
				return 1;
			}
			return 99;
		}
	}

	public GameObject OtherCar => GetRivalCar(ourCar);

	public static event OnRestart Restarted;

	private void Awake()
	{
		if (V.LastUsedCar == null)
		{
			SceneManager.LoadScene(1);
			return;
		}
		if (!Application.isEditor)
		{
			alwaysWin = false;
		}
		_racingProvider.DragRacingController = this;
		qualification = true;
		autoSteering = PlayerPrefs.GetInt("autoSteering", 1) == 1;
		dragRacingUI.steeringAutoTexture.color = (autoSteering ? Color.green : Color.white);
		SetRunDistance();
	}

	private void Start()
	{
		SoundEventChannel.InvokeDragSpeakerEvent("Openning");
		raceInProcess = false;
		ourCar = V.LastUsedCar;
		if (ourCar.TryGetComponent<VehicleData>(out var component))
		{
			component.CarValues.driver = V.DriverPerson;
			component.CarValues.Class = Utils.GetClassVehicle(component.GetVehicleEntityMono.CarEntity.PerformancePoints, component.CarId);
		}
		dragy = ourCar.GetComponent<Dragy>();
		_playerStatusSystem.OnGameModePlayed();
		_coreGameManager.CarOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		OnStart();
		_backToCarHandle.Get(_eventRegistry).Subscribe(OnBackToCarInput);
	}

	private void OnDestroy()
	{
		_placePersonOnGarageDisposable?.Dispose();
		_placePersonOnGarageDisposable = null;
		_carInputManager.IgnoreSteerControl(state: false);
		_carInputManager.ResetKeepRowComposition();
		_racingProvider.DragRacingController = null;
		FlagsStorage.SingeDragRacingMode = false;
		_backToCarHandle.Get(_eventRegistry)?.Unsubscribe(OnBackToCarInput);
	}

	private void FillAllGarageOnStart()
	{
		for (int i = 2; i < Garages.Count; i++)
		{
			GameObject car = CreateOtherRacingCarSelectedClass();
			PushToGarage(Garages[i], car);
		}
	}

	[Inject]
	public void Construct(RaceFinishUI raceFinishUI, PlayerDatas.CarLoadingUtility carLoadingUtility, IPrefabFactory prefabFactory, ScreenFader screenFader, IPlayerStorageService playerStorageService, SoundEventChannel soundEventChannel, IDragRacingRewardService dragRacingRewardService, ManagerUI managerUI, IConnectionManager connectionManager, InputItemsProvider controlItemsProvider, CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, FreeDriveProvider freeDriveProvider, RacingProvider racingProvider, IPlayerStatusSystem playerStatusSystem, RacingAnalytic racingAnalytic, CarInputManager carInputManager, IGuestAccountCreator guestAccountCreator, AllCarsInfo allCarsInfo, InputMapSetEvent inputMapSetEvent, EventRegistry eventRegistry, ISteamAchievementService steamAchievementService)
	{
		_racingProvider = racingProvider;
		_freeDriveProvider = freeDriveProvider;
		_menuLevelParent = menuLevelParent;
		_coreGameManager = coreGameManager;
		_controlItemsProvider = controlItemsProvider;
		_connectionManager = connectionManager;
		_dragRacingRewardService = dragRacingRewardService;
		_managerUI = managerUI;
		SoundEventChannel = soundEventChannel;
		_playerStorageService = playerStorageService;
		_screenFader = screenFader;
		_prefabFactory = prefabFactory;
		_carLoadingUtility = carLoadingUtility;
		_raceFinishUI = raceFinishUI;
		_carInputManager = carInputManager;
		_playerStatusSystem = playerStatusSystem;
		_racingAnalytic = racingAnalytic;
		_guestAccountCreator = guestAccountCreator;
		_allCarsInfo = allCarsInfo;
		if ((bool)dragRacingCamera)
		{
			_carInputManager.SetDragRaceCamera(dragRacingCamera);
		}
		_inputMapSetEvent = inputMapSetEvent;
		_eventRegistry = eventRegistry;
		_steamAchievementService = steamAchievementService;
	}

	private MainCarValues[] FilterBannedCars(MainCarValues[] source)
	{
		if (source == null || source.Length == 0 || !_allCarsInfo)
		{
			return source;
		}
		MainCarValues[] array = source.Where((MainCarValues v) => (bool)v && !_allCarsInfo.IsBannedCar(v.CarID)).ToArray();
		if (array.Length != 0)
		{
			return array;
		}
		return source;
	}

	private void SetRunDistance()
	{
		PlayerDatas.DisableEnableAll(runDistance == RunDistance.QuaterMile, for1_4Mile);
		PlayerDatas.DisableEnableAll(runDistance == RunDistance.OneMile, for1_2Mile);
		Transform transform = dragRacingCamera.virtualCameraOnFinish.transform;
		transform.position = runDistance switch
		{
			RunDistance.QuaterMile => for1_4Mile[1].transform.position, 
			RunDistance.OneMile => for1_2Mile[1].transform.position, 
			_ => dragRacingCamera.virtualCameraOnFinish.transform.position, 
		};
	}

	private void OnStart()
	{
		timeClass = UnityEngine.Random.Range(8, 13);
		FlagsStorage.RacingGameMode = true;
		FlagsStorage.SingeDragRacingMode = true;
		FlagsStorage.IsMainMenu = false;
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(32);
		SceneManager.MoveGameObjectToScene(ourCar, sceneByBuildIndex);
		Observable.NextFrame().Subscribe((Unit _) =>
		{
			_screenFader.FadeIn();
			if ((bool)ourCar)
			{
				SetOwnCarForDragRacing(ourCar);
				dragRacingCamera.setCamera(ourCar);
			}
			StartNewRace();
			FillAllGarageOnStart();
			dragRacingCamera.StartMoveVeryFirstCam().Forget();
		}).AddTo(this);
	}

	public bool AllCarBodyReady()
	{
		foreach (Garage garage in Garages)
		{
			if ((bool)garage.car && !garage.car.GetComponent<VehicleData>().MainModel)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsLeftCar(GameObject car)
	{
		return leftCar == car;
	}

	public void ChangeClass(int newClass)
	{
		StartNewRace();
	}

	private void SetNewTimeClass(int newClass)
	{
		timeClass = newClass;
		_bestTime = 1000f;
		if ((bool)dragy)
		{
			dragy.lost = false;
			dragy.reason = Dragy.LostReason.none;
			dragy.falseStartCount = 0;
		}
		RaceWon = 0;
		nextRivalCar = null;
		winners = new MainCarValues[3];
		nowRacingCarClassList = FilterBannedCars(allRacingCarClassList.GetClassList(timeClass, 16));
		raceFor3Place = false;
		allRacersCount = nowRacingCarClassList.Length;
		allRacersCount = 32;
	}

	public void StartNewRace()
	{
		_finishResultPending = false;
		_pendingGarageReturn = false;
		for (int i = 0; i < winners.Length; i++)
		{
			MainCarValues mainCarValues = winners[i];
			if ((bool)mainCarValues && i != _lastOwnCarPlaceForWin)
			{
				UnityEngine.Object.Destroy(mainCarValues.driver);
			}
		}
		SetNewTimeClass(timeClass);
		OnGarageOpen();
		podium.ClearPodium();
		Restarted?.Invoke();
		if (dragRacingUI.lastAutoSteeringState)
		{
			dragRacingUI.lastAutoSteeringState = false;
			SetSteering(auto: false);
		}
		dragRacingTree.OnStarNewRace();
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<Rigidbody>(out var component))
		{
			component.isKinematic = false;
			component.constraints = RigidbodyConstraints.FreezeRotation;
		}
		_dragRacingStartTime = Time.time;
		_racingAnalytic.LogDragRacingStart();
	}

	private void SetOwnCarForDragRacing(GameObject car)
	{
		car.transform.SetPositionAndRotation(spawnPoint.transform.position + CarYOffset(car), spawnPoint.transform.rotation);
		PlayerDatas.CreateSkid(car);
		if (ourCar.TryGetComponent<VehicleData>(out var component))
		{
			component.SetEngineState(toStart: true);
			component.RacingProvider = _racingProvider;
			component.GetPowertrain.SetKinematic(value: false);
		}
	}

	public Vector3 CarYOffset(GameObject car)
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

	private GameObject CreateOtherRacingCarSelectedClass()
	{
		int num = 10;
		int num2;
		do
		{
			num2 = UnityEngine.Random.Range(1, nowRacingCarClassList.Length);
		}
		while (InWinnerList(nowRacingCarClassList[num2]) && num-- > 0);
		if (carRacingID != -1)
		{
			num2 = carRacingID;
		}
		return CreateRaceCar(nowRacingCarClassList[num2]);
	}

	public async UniTaskVoid OnWon()
	{
		List<MainCarValues> winnersList = new List<MainCarValues>();
		UITimeBannerController.DisableElements();
		fillWinnerList(Place);
		SetWinnerPersons();
		winnersList.AddRange(winners);
		await UniTask.Delay(500);
		dragRacingCamera.ShowWinnerCamera();
		dragRacingUI.SetUIForGarage(nextRivalCar);
		dragRacingUI.onWon();
		ProcessWinnersResult();
		int num = UnityEngine.Random.Range(2, nowRacingCarClassList.Length);
		winnersList.Add(nowRacingCarClassList[num]);
		raceInProcess = false;
		UITimeBannerController.FillDragRacingTable(winnersList, runDistance);
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEw", "drag_race");
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui, setRoot: true);
		_steamAchievementService?.Unlock("ACH_CHECKERED_HELLO");
		if ((bool)dragy && dragy.falseStartCount == 0)
		{
			GooglePlayAchievements.MarkCleanSlateDrag("drag_race");
			_steamAchievementService?.MarkCleanSlateDrag();
		}
	}

	public void OnCarAccident()
	{
		if ((bool)ourCar && ourCar.TryGetComponent<VehicleData>(out var component) && component.racingState == RacingState.OnRacing && (bool)dragy)
		{
			dragy.lost = true;
			dragy.reason = Dragy.LostReason.carAccident;
			OnLose().Forget();
		}
	}

	public async UniTaskVoid OnLose()
	{
		UITimeBannerController.DisableElements();
		MonoBehaviour.print("on lose");
		fillWinnerListOnLose();
		List<MainCarValues> needMainCarValues = winners.ToList();
		MainCarValues carValues = ourCar.GetComponent<VehicleData>().CarValues;
		carValues.car = V.Car;
		carValues.IsUser = true;
		int num = UnityEngine.Random.Range(2, nowRacingCarClassList.Length);
		needMainCarValues.Add(nowRacingCarClassList[num]);
		needMainCarValues.Add(carValues);
		await UniTask.Delay(100);
		SetWinnerPersons();
		await UniTask.Delay(500);
		if (Place > 3)
		{
			SetOwnCarToGarage();
		}
		dragRacingUI.SetUIForGarage(nextRivalCar);
		dragRacingUI.onLose();
		ProcessWinnersResult();
		dragRacingCamera.ShowLoseCamera();
		raceInProcess = false;
		UITimeBannerController.FillDragRacingTable(needMainCarValues, runDistance);
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui, setRoot: true);
	}

	private void SetOwnCarToGarage()
	{
		Garage garage = ToGarage(ourCar);
		_setPosOfNextCarDisposable?.Dispose();
		if ((bool)nextRivalCar)
		{
			_setPosOfNextCarDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				SetPosOfNextCar();
			}).AddTo(this);
		}
		Vector3 position = garage.carPos.position;
		ourCar.transform.rotation = garage.carPos.rotation;
		Vector3 position2 = ourCar.transform.position;
		if (Vector3.Distance(position, position2) > 1f)
		{
			ourCar.transform.position = position + CarYOffset(ourCar);
		}
		if (ourCar.TryGetComponent<VehicleData>(out var component))
		{
			component.racingState = RacingState.onGarage;
		}
		dragRacingCamera.SetCameraForGarage();
		dragRacingTree.StopCar(ourCar);
		if (ourCar.TryGetComponent<Powertrain>(out var component2))
		{
			component2.SetAlignCar(state: true);
		}
		if (ourCar == DragRacingCar2)
		{
			DragRacingCar2 = null;
		}
	}

	public void OnGarageOpen()
	{
		IsRandomCarsRace = false;
		if ((RaceWon == 5 && Place != 4) || (dragy.lost && RaceWon == 4))
		{
			OnWon().Forget();
			return;
		}
		if (dragy.lost)
		{
			OnLose().Forget();
			return;
		}
		if (!nextRivalCar && !dragy.lost)
		{
			nowRacingCarClassList = allRacingCarClassList.EliminateRandom(nowRacingCarClassList, allRacersCount, ourCar);
			dragRacingUI.UpdateProgressBar(allRacersCount);
			if (allRacersCount != 1)
			{
				nextRivalCar = CreateOtherRacingCarSelectedClass();
			}
		}
		testRun = false;
		SetOwnCarToGarage();
		if (!DragRacingCar2)
		{
			PrepareNextRandomCarsData();
		}
		PlacePersonOnGarage(V.DriverPerson, ourCar);
		dragRacingUI.SetUIForGarage(nextRivalCar);
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Garage, setRoot: true);
	}

	private void SetPosOfNextCar()
	{
		onGarage(nextRivalCar, setToGaragePosition: true);
	}

	public void OnFinishResultShown()
	{
		_finishResultPending = false;
		if (_pendingGarageReturn)
		{
			_pendingGarageReturn = false;
			onGarage(ourCar, setToGaragePosition: true);
		}
	}

	public void OnRaceNowButtonPressed()
	{
		if (V.DriverPerson.GetComponent<TouchMovePerson>().enabled)
		{
			return;
		}
		if (qualification)
		{
			StartQualificationRun();
			return;
		}
		bool uIForRacing = false;
		if (ourCar.TryGetComponent<Powertrain>(out var component))
		{
			uIForRacing = component.IsAutomaticGearbox;
		}
		dragRacingUI.SetUIForRacing(uIForRacing);
		dragRacingCamera.SetCameraForRacing();
		RaceWithNextCar();
		ClearAllCars(rightCar, leftCar);
		SoundEventChannel.InvokeDragSpeakerEvent("Pre_Start");
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarDrag, setRoot: true);
	}

	public void SetSteering(bool auto)
	{
		if ((bool)ourCar)
		{
			_carInputManager.FollowPoints = auto;
			_carInputManager.IgnoreSteerControl(auto);
		}
		PlayerPrefs.SetInt("autoSteering", auto ? 1 : 0);
		autoSteering = auto;
		dragRacingUI.SetAutoSteering(auto);
		_menuLevelParent.SetSteering();
	}

	private void SetCarToRacing(GameObject car, bool setToPosition)
	{
		DragRacingTree.PositionOnTree positionOnTree = DragRacingTree.PositionOnTree.left;
		bool flag = false;
		if (setToPosition)
		{
			DragRacingTree.PositionOnTree physicalSide = dragRacingTree.GetPhysicalSide(car);
			if (physicalSide != DragRacingTree.PositionOnTree.None && (physicalSide != DragRacingTree.PositionOnTree.left || !dragRacingTree.CarLeft || !(dragRacingTree.CarLeft != car)) && (physicalSide != DragRacingTree.PositionOnTree.right || !dragRacingTree.CarRight || !(dragRacingTree.CarRight != car)))
			{
				positionOnTree = physicalSide;
				flag = true;
			}
		}
		if (!flag)
		{
			if (!dragRacingTree.CarLeft && !dragRacingTree.CarRight)
			{
				positionOnTree = ((UnityEngine.Random.Range(0, 2) != 0) ? DragRacingTree.PositionOnTree.right : DragRacingTree.PositionOnTree.left);
			}
			else
			{
				if ((bool)dragRacingTree.CarLeft && (bool)dragRacingTree.CarRight)
				{
					return;
				}
				if (!dragRacingTree.CarLeft)
				{
					positionOnTree = DragRacingTree.PositionOnTree.left;
				}
				if (!dragRacingTree.CarRight)
				{
					positionOnTree = DragRacingTree.PositionOnTree.right;
				}
			}
		}
		Vector3[] pathPoints;
		if (positionOnTree == DragRacingTree.PositionOnTree.left)
		{
			pathPoints = path1.GetPathPoints();
			leftCar = car;
		}
		else
		{
			pathPoints = path2.GetPathPoints();
			rightCar = car;
		}
		dragRacingTree.SetCarToSide(car, positionOnTree);
		bool num = car == ourCar;
		Powertrain component = car.GetComponent<Powertrain>();
		dragRacingTree.StopCar(car);
		if (car.TryGetComponent<Dragy>(out var component2))
		{
			component2.Reset();
		}
		else
		{
			car.GetOrAddComponent<Dragy>();
		}
		component.SetAlignCar(state: false);
		CarTriggers carTriggers = car.GetComponent<CarTriggers>();
		if (!carTriggers)
		{
			carTriggers = car.AddComponent<CarTriggers>();
		}
		carTriggers.FreeDriveProvider = _freeDriveProvider;
		carTriggers.RacingProvider = _racingProvider;
		if (car.TryGetComponent<VehicleData>(out var component3))
		{
			component3.racingState = RacingState.rollingToStart;
			component3.RacingProvider = _racingProvider;
		}
		if (setToPosition)
		{
			car.transform.SetPositionAndRotation(pathPoints[2] + CarYOffset(car), Quaternion.identity);
			car.transform.LookAt(pathPoints[3] + CarYOffset(car));
			SyncRigidbodyToTransform(car);
		}
		CarAIController component4;
		if (num)
		{
			component3.KeepRowComposition.RacingTargets = pathPoints;
			component3.KeepRowComposition.CurrentTargetIndex = 1;
			_carInputManager.State = 2;
			_carInputManager.TargetSpeed = 10f;
			SetSteering(autoSteering);
			_carInputManager.KeepRowComposition.FollowOnRacing = ((positionOnTree == DragRacingTree.PositionOnTree.left) ? dragRacingTree.leftSideTarget : dragRacingTree.rightSideTarget);
			camOnFinishShowed = false;
		}
		else if (car.TryGetComponent<CarAIController>(out component4))
		{
			component4.targets = pathPoints;
			component4.CurrentTarIndex = 2;
			component4.Sign = 1;
			component4.TargetSpeed = 15 + UnityEngine.Random.Range(-3, 3);
			component4.AiType = AiType.DragRacingCar;
		}
		else
		{
			component4 = car.AddComponent<CarAIController>();
			component4.targets = pathPoints;
			component4.CurrentTarIndex = 2;
			component4.Sign = 1;
			component4.TargetSpeed = 15 + UnityEngine.Random.Range(-3, 3);
			component4.AiType = AiType.DragRacingCar;
		}
	}

	private GameObject CreateRaceCar(MainCarValues mainCarValues)
	{
		if (!mainCarValues)
		{
			return null;
		}
		GameObject gameObject = _carLoadingUtility.LoadDragRacingCar(mainCarValues, Vector3.zero);
		allCars.Add(gameObject);
		if (gameObject.TryGetComponent<DriverController>(out var component))
		{
			PlayerDatas.CreateSkid(gameObject);
			mainCarValues.driver = (component.DriverPerson = CreateRacerPerson());
			_coreGameManager.CarOutInControl.InCar(component.DriverPerson, gameObject).Forget();
		}
		if (gameObject.TryGetComponent<VehicleData>(out var component2))
		{
			component2.RacingProvider = _racingProvider;
		}
		return gameObject;
	}

	private GameObject CreateRacerPerson()
	{
		GameObject obj = Resources.Load<GameObject>("person0");
		GameObject obj2 = _prefabFactory.Instantiate(obj);
		if (obj2.TryGetComponent<MainPerson>(out var component))
		{
			component.racingPerson = true;
		}
		return obj2;
	}

	public GameObject GetRivalCar(GameObject car)
	{
		if (!(DragRacingCar1 == car))
		{
			return DragRacingCar1;
		}
		return DragRacingCar2;
	}

	private int GetClassByTime(float time)
	{
		timeClass = Mathf.Clamp((int)Mathf.Floor(time), 8, 13);
		return timeClass;
	}

	public void PlayDragSpeakerFinish(GameObject car)
	{
		if ((testRun || qualification) && !IsRandomCarsRace)
		{
			return;
		}
		_finishCarsCount++;
		if (_finishCarsCount != 2)
		{
			_isLeftCarWinner = car == dragRacingTree.CarLeft;
			_isRightCarWinner = car == dragRacingTree.CarRight;
			return;
		}
		bool flag = ourCar == dragRacingTree.CarLeft || ourCar == dragRacingTree.CarRight;
		if (dragRacingTree.CarLeft.GetComponent<Dragy>().reactionTime < 0f || dragRacingTree.CarRight.GetComponent<Dragy>().reactionTime < 0f)
		{
			if (!flag || !RaceAgain(dragRacingTree.CarLeft, dragRacingTree.CarRight))
			{
				SoundEventChannel.InvokeDragSpeakerEvent("FalseStart_Lose");
			}
			else
			{
				SoundEventChannel.InvokeDragSpeakerEvent("FalseStart_Restart");
			}
		}
		else if (_isLeftCarWinner)
		{
			SoundEventChannel.InvokeDragSpeakerEvent("WinLeft");
		}
		else if (_isRightCarWinner)
		{
			SoundEventChannel.InvokeDragSpeakerEvent("WinRight");
		}
		_finishCarsCount = 0;
	}

	public void OnCarFinish(CarComponents car)
	{
		VehicleData vehicleData = car.VehicleData;
		PlayDragSpeakerFinish(car.GameObject);
		dragRacingTree.OnCarFinished(car);
		Dragy component = vehicleData.GetComponent<Dragy>();
		if (runDistance != RunDistance.QuaterMile)
		{
			_ = component.halfMileResult;
		}
		else
		{
			_ = component.quarterMileResult;
		}
		if (vehicleData.racingState != RacingState.OnRacing)
		{
			return;
		}
		vehicleData.racingState = RacingState.Finished;
		CarAIController carAIController = car.CarAIController;
		bool flag = false;
		GameObject rivalCar = GetRivalCar(car.GameObject);
		if ((bool)rivalCar)
		{
			flag = RaceAgain(car.GameObject, rivalCar);
		}
		if (flag)
		{
			vehicleData.racingState = RacingState.toRaceAgain;
		}
		else
		{
			if (recordFinishResult)
			{
				RecordFinishResults();
			}
			vehicleData.racingState = RacingState.toGarage;
		}
		if (car.GameObject == ourCar)
		{
			_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
			_carInputManager.TargetSpeed = 1000f;
			dragRacingUI.ShowFinishSign();
			dragRacingUI.AddResult(component);
			SaveClassBestTime();
			if (!flag)
			{
				nextRivalCar = null;
			}
		}
		if ((bool)carAIController)
		{
			carAIController.TargetSpeed = 10f;
		}
		if (car.GameObject != ourCar && rivalCar != ourCar)
		{
			return;
		}
		_finishResultPending = true;
		if (testRun)
		{
			if (vehicleData.racingState == RacingState.OnRacing)
			{
				return;
			}
			int num = 0;
			if (component.reactionTime >= 0f && qualification)
			{
				num = (timeClass = GetClassByTime(component.quarterMileResult));
				SetNewTimeClass(timeClass);
				SoundEventChannel.InvokeDragSpeakerEvent($"Q_s{timeClass}");
			}
			if (car.GameObject == leftCar)
			{
				_raceFinishUI.ShowResults(car.GameObject, null, flag, num);
			}
			else
			{
				_raceFinishUI.ShowResults(null, car.GameObject, flag, num);
			}
			if (num != 0)
			{
				qualification = false;
			}
		}
		else if (rivalCar.GetComponent<VehicleData>().racingState != RacingState.OnRacing && vehicleData.racingState != RacingState.OnRacing)
		{
			_raceFinishUI.ShowResults(leftCar, rightCar, flag);
		}
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui);
	}

	private void SaveClassBestTime()
	{
		float num = dragy.reactionTime + dragy.quarterMileResult;
		float num2 = ((timeClass != 8) ? timeClass : 0);
		if (_bestTime > num && dragy.quarterMileResult >= num2 && !(dragy.reactionTime < 0f))
		{
			_bestTime = num;
		}
	}

	private void SaveWinsResult(int place)
	{
		Powertrain component = ourCar.GetComponent<Powertrain>();
		int gearboxType = (component.IsAutomaticGearbox ? 1 : 0);
		RacingClassRecord racingClassRecord = (component.IsAutomaticGearbox ? GetClassRecords() : GetManualRacingClassRecords()).Find((RacingClassRecord record) => record.TimeClass == timeClass);
		if (racingClassRecord == null)
		{
			return;
		}
		if (racingClassRecord.BestTime <= 0f || racingClassRecord.BestTime > _bestTime)
		{
			racingClassRecord.BestTime = _bestTime;
		}
		switch (place)
		{
		case 1:
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQMQ", "drag_tournament");
			if (runDistance == RunDistance.QuaterMile)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQFA", "drag_tournament");
			}
			_steamAchievementService?.Unlock("ACH_BRACKET_BOSS");
			if (runDistance == RunDistance.QuaterMile)
			{
				_steamAchievementService?.Unlock("ACH_QUATER_MILE_HERO");
			}
			racingClassRecord.FirstPlace++;
			if (racingClassRecord.FirstPlace == 1)
			{
				DragReward(timeClass, 1, gearboxType).Forget();
			}
			else
			{
				ShowNotificationCoinRewardWinOnce();
			}
			break;
		case 2:
			racingClassRecord.SecondPlace++;
			if (racingClassRecord.SecondPlace == 1)
			{
				DragReward(timeClass, 2, gearboxType).Forget();
			}
			else
			{
				ShowNotificationCoinRewardWinOnce();
			}
			break;
		case 3:
			racingClassRecord.ThirdPlace++;
			if (racingClassRecord.ThirdPlace == 1)
			{
				DragReward(timeClass, 3, gearboxType).Forget();
			}
			else
			{
				ShowNotificationCoinRewardWinOnce();
			}
			break;
		}
		_playerStorageService.Save();
	}

	private void ShowNotificationCoinRewardWinOnce()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("DragRacingController.CONGRATULATIONS!You'vealreadygottherewardforthisplaceinthecurrentclass"), 5f).Forget();
	}

	private async UniTask DragReward(int currentClass, int place, int gearboxType)
	{
		_connectionManager.CorrectHash.Get(out var value);
		if (!value)
		{
			return;
		}
		int classId = currentClass - 8;
		if (await _guestAccountCreator.CreateGuestAccountIfNeeds())
		{
			WalletOperationResult walletOperationResult = await _dragRacingRewardService.GetCoinForDrag(classId, place, gearboxType);
			if (walletOperationResult.IsSuccess())
			{
				GetterCoinsManager.AddCoinsToPrefs(new AdvancedInt(walletOperationResult.Value));
				string text = new LocalizedString("DragRacingController.CongratulationsResultValueCoin");
				_managerUI.ShowNotificationWindow(text.Replace("{result.Value}", walletOperationResult.Value.ToString()), 3f).Forget();
			}
		}
	}

	private List<RacingClassRecord> GetClassRecords()
	{
		PlayerStorage data = _playerStorageService.Data;
		if (data.RacingProgress == null)
		{
			data.RacingProgress = new RacingProgress();
		}
		List<RacingClassRecord> list = _playerStorageService.Data.RacingProgress.RacingClassRecords;
		if (list == null || list.Count == 0)
		{
			list = new List<RacingClassRecord>();
			for (int i = 0; i < _maxClassCount; i++)
			{
				RacingClassRecord newRacingClassRecord = GetNewRacingClassRecord();
				_maxClassTime--;
				list.Add(newRacingClassRecord);
			}
			_playerStorageService.Data.RacingProgress.RacingClassRecords = list;
		}
		if (_playerStorageService.Data.RacingProgress.RacingClassRecords.Count == _maxClassCount - 1)
		{
			list.Add(GetNewRacingClassRecord());
			_playerStorageService.Data.RacingProgress.RacingClassRecords = list;
		}
		return list;
	}

	private List<RacingClassRecord> GetManualRacingClassRecords()
	{
		PlayerStorage data = _playerStorageService.Data;
		if (data.RacingProgress == null)
		{
			data.RacingProgress = new RacingProgress();
		}
		List<RacingClassRecord> list = _playerStorageService.Data.RacingProgress.ManualRacingClassRecords;
		if (list == null || list.Count == 0)
		{
			list = new List<RacingClassRecord>();
			for (int i = 0; i < _maxClassCount; i++)
			{
				RacingClassRecord newRacingClassRecord = GetNewRacingClassRecord();
				_maxClassTime--;
				list.Add(newRacingClassRecord);
			}
			_playerStorageService.Data.RacingProgress.ManualRacingClassRecords = list;
		}
		if (_playerStorageService.Data.RacingProgress.ManualRacingClassRecords.Count == _maxClassCount - 1)
		{
			list.Add(GetNewRacingClassRecord());
			_playerStorageService.Data.RacingProgress.ManualRacingClassRecords = list;
		}
		return list;
	}

	private RacingClassRecord GetNewRacingClassRecord()
	{
		return new RacingClassRecord
		{
			TimeClass = _maxClassTime,
			FirstPlace = 0,
			SecondPlace = 0,
			ThirdPlace = 0,
			BestTime = 0f
		};
	}

	private void ProcessWinnersResult()
	{
		for (int i = 0; i < winners.Length; i++)
		{
			if (winners[i].car == ourCar)
			{
				SaveWinsResult(i + 1);
			}
		}
	}

	private async UniTaskVoid OnBothCarFinished()
	{
		nextRandomCars = new NextRandomCars();
		if (dragRacingCamera.virtualCameraOnToLookOtherRace.enabled)
		{
			_randomOtherCarRaceDisposable?.Dispose();
			_randomOtherCarRaceDisposable = Observable.Timer(TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
			{
				RandomOtherCarRace();
			}).AddTo(this);
			await UniTask.Delay(1000);
			if (dragRacingCamera != null && dragRacingCamera.virtualCameraOnTreeStart != null)
			{
				dragRacingCamera.virtualCameraOnTreeStart.enabled = true;
			}
			if (dragRacingUI != null)
			{
				dragRacingUI.ShowFinishOtherCars();
			}
			DestroyRacingCar(leftCar);
			DestroyRacingCar(rightCar);
		}
		else
		{
			PrepareNextRandomCarsData();
		}
	}

	private void RecordFinishResults()
	{
		if (testRun)
		{
			return;
		}
		Dragy component = leftCar.GetComponent<Dragy>();
		Dragy component2 = rightCar.GetComponent<Dragy>();
		float num = ((runDistance == RunDistance.QuaterMile) ? component.quarterMileResult : component.halfMileResult);
		float num2 = ((runDistance == RunDistance.QuaterMile) ? component2.quarterMileResult : component2.halfMileResult);
		if (num == 0f || num2 == 0f)
		{
			return;
		}
		recordFinishResult = false;
		bool flag = timeClass == 8;
		if ((num < (float)timeClass || num2 < (float)timeClass) && !flag)
		{
			if (num < (float)timeClass && num2 < (float)timeClass)
			{
				if (num < num2)
				{
					component.lost = true;
					component.reason = Dragy.LostReason.fasterClass;
				}
				else
				{
					component2.lost = true;
					component2.reason = Dragy.LostReason.fasterClass;
				}
			}
			else
			{
				if (num < (float)timeClass)
				{
					component.lost = true;
					component.reason = Dragy.LostReason.fasterClass;
				}
				if (num2 < (float)timeClass)
				{
					component2.lost = true;
					component2.reason = Dragy.LostReason.fasterClass;
				}
			}
		}
		else if ((component.falseStartCount <= 1 || component2.falseStartCount <= 1) && (component.falseStartCount > 1 || component2.falseStartCount > 1))
		{
			if (component.falseStartCount > 1)
			{
				component.lost = true;
				component.reason = Dragy.LostReason.falseStart;
			}
			else if (component2.falseStartCount > 1)
			{
				component2.lost = true;
				component2.reason = Dragy.LostReason.falseStart;
			}
		}
		else
		{
			float num3 = num;
			if (component.reactionTime > 0f)
			{
				num3 += component.reactionTime;
			}
			float num4 = num2;
			if (component2.reactionTime > 0f)
			{
				num4 += component2.reactionTime;
			}
			if (num3 < num4)
			{
				component2.lost = true;
				component2.reason = Dragy.LostReason.slow;
			}
			else
			{
				component.lost = true;
				component.reason = Dragy.LostReason.slow;
			}
		}
		if ((component2.reason == Dragy.LostReason.fasterClass || component.reason == Dragy.LostReason.fasterClass) && !RaceAgain(leftCar, rightCar))
		{
			SoundEventChannel.InvokeDragSpeakerEvent("Breakout");
		}
		if (ourCar == leftCar || ourCar == rightCar)
		{
			allRacersCount /= 2;
			if (!dragy.lost && dragy.reactionTime >= 0f && dragy.reason != Dragy.LostReason.outOfRoad)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQJA", "drag_race");
			}
			if (!dragy.lost && dragy.reactionTime >= 0f && dragy.reactionTime < 0.05f && dragy.reason != Dragy.LostReason.outOfRoad)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQOA", "drag_race");
			}
			if (!dragy.lost && dragy.reactionTime >= 0f && dragy.reason != Dragy.LostReason.outOfRoad)
			{
				_steamAchievementService?.Unlock("ACH_TREE_WHISPERER");
			}
			if (!dragy.lost && dragy.reactionTime >= 0f && dragy.reactionTime < 0.05f && dragy.reason != Dragy.LostReason.outOfRoad)
			{
				_steamAchievementService?.Unlock("ACH_BLINK_AND_YOU_MISSED_IT");
			}
			RecordProgress();
			if (RaceWon == 5 && !raceFor3Place)
			{
				SoundEventChannel.InvokeDragSpeakerEvent(_isLeftCarWinner ? "WinFinal_Left" : "WinFinal_Right");
			}
		}
		OnBothCarFinished().Forget();
	}

	private void RecordProgress()
	{
		if (RaceWon == 4)
		{
			if (dragy.lost)
			{
				if (raceFor3Place)
				{
					AddWinner(GetRivalCar(ourCar), 3);
				}
				else
				{
					AddWinner(GetRivalCar(ourCar), 1);
					AddWinner(ourCar, 2);
				}
			}
			else if (raceFor3Place)
			{
				AddWinner(GetRivalCar(ourCar), 2);
				AddWinner(ourCar, 3);
			}
			else
			{
				AddWinner(GetRivalCar(ourCar), 2);
				AddWinner(ourCar, 1);
			}
		}
		if (RaceWon == 3)
		{
			if (dragy.lost)
			{
				AddWinner(GetRivalCar(ourCar), UnityEngine.Random.Range(1, 2));
				AddWinner(ourCar, 3);
				raceFor3Place = true;
				dragy.lost = false;
				dragy.lostButFor3place = true;
			}
			else
			{
				AddWinner(GetRivalCar(ourCar), 3);
			}
		}
		if (alwaysWin && dragy.lost)
		{
			dragy.lost = false;
		}
		if (!dragy.lost)
		{
			RaceWon++;
			raceInProcess = true;
		}
		else if (RaceWon == 4)
		{
			RaceWon++;
		}
	}

	private void fillWinnerListOnLose()
	{
		List<MainCarValues> list = new List<MainCarValues>();
		MainCarValues[] array = nowRacingCarClassList;
		foreach (MainCarValues mainCarValues in array)
		{
			if (!InWinnerList(mainCarValues))
			{
				list.Add(mainCarValues);
			}
		}
		int num = 0;
		array = winners;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				int index = UnityEngine.Random.Range(1, list.Count);
				MainCarValues mainCarValues2 = UnityEngine.Object.Instantiate(list[index]);
				winners[num] = mainCarValues2;
				winners[num].car = CreateRaceCar(mainCarValues2);
			}
			num++;
		}
	}

	private void AddWinner(GameObject car, int place)
	{
		place--;
		MainCarValues carValues = car.GetComponent<VehicleData>().CarValues;
		carValues.car = car;
		winners[place] = carValues;
	}

	private bool InWinnerList(MainCarValues v)
	{
		MainCarValues[] array = winners;
		foreach (MainCarValues mainCarValues in array)
		{
			if ((bool)mainCarValues && mainCarValues == v)
			{
				return true;
			}
		}
		return false;
	}

	private bool InWinnerList(GameObject car)
	{
		MainCarValues[] array = winners;
		foreach (MainCarValues mainCarValues in array)
		{
			if ((bool)mainCarValues && mainCarValues.car == car)
			{
				return true;
			}
		}
		return false;
	}

	public void fillWinnerList(int ownCarPlace)
	{
		ownCarPlace--;
		if (ownCarPlace < 3)
		{
			winners[ownCarPlace] = ourCar.GetComponent<VehicleData>().CarValues;
			winners[ownCarPlace].IsUser = true;
		}
		_lastOwnCarPlaceForWin = ownCarPlace;
		for (int i = 0; i < 3; i++)
		{
			if (winners[i] == null)
			{
				winners[i] = nowRacingCarClassList[i + 1];
			}
			if (!winners[i].car)
			{
				winners[i].car = CreateRaceCar(winners[i]);
			}
		}
	}

	public void onGarage(GameObject car, bool setToGaragePosition)
	{
		if (car == ourCar && _finishResultPending)
		{
			_pendingGarageReturn = true;
			return;
		}
		VehicleData component = car.GetComponent<VehicleData>();
		if (component.racingState == RacingState.onGarage)
		{
			return;
		}
		component.racingState = RacingState.onGarage;
		CarAIController component2 = car.GetComponent<CarAIController>();
		if ((bool)component2)
		{
			component2.TargetSpeed = 0f;
		}
		if (setToGaragePosition)
		{
			Garage garage = GetOwnGarage(car) ?? GetEmptyGarage();
			if (garage != null)
			{
				PushToGarage(garage, car);
			}
		}
		if (DragRacingCar1 == car)
		{
			DragRacingCar1 = null;
		}
		if (DragRacingCar2 == car)
		{
			DragRacingCar2 = null;
		}
		GameObject driver = car.GetComponent<VehicleData>().Driver;
		PlacePersonOnGarage(driver, car);
		if (car == ourCar)
		{
			OnGarageOpen();
		}
	}

	private void PushToGarage(Garage garage, GameObject car)
	{
		garage.car = car;
		CarAIController component = car.GetComponent<CarAIController>();
		car.GetComponent<VehicleData>().racingState = RacingState.onGarage;
		if ((bool)component)
		{
			component.targets = garage.path.GetPathPoints();
		}
		if (car.TryGetComponent<Powertrain>(out var component2))
		{
			component2.PositionIntegration = false;
			component2.FixVehicle(stop: false);
		}
		Vector3 position = garage.carPos.position;
		car.transform.rotation = garage.carPos.rotation;
		car.transform.position = position + CarYOffset(ourCar);
		SyncRigidbodyToTransform(car);
		dragRacingTree.StopCar(car);
	}

	private static void SyncRigidbodyToTransform(GameObject car)
	{
		if (car.TryGetComponent<Powertrain>(out var component))
		{
			component.PositionIntegration = false;
			component.FixVehicle(stop: false);
		}
		if (car.TryGetComponent<Rigidbody>(out var component2))
		{
			if (!component2.isKinematic)
			{
				component2.velocity = Vector3.zero;
				component2.angularVelocity = Vector3.zero;
			}
			component2.position = car.transform.position;
			component2.rotation = car.transform.rotation;
		}
	}

	public Garage ToGarage(GameObject car)
	{
		VehicleData component = car.GetComponent<VehicleData>();
		Garage garage = GetOwnGarage(car);
		if (component.racingState == RacingState.toRaceAgain)
		{
			return garage;
		}
		if (garage == null)
		{
			garage = GetEmptyGarage();
			if (garage == null)
			{
				MonoBehaviour.print("more garage please.");
			}
		}
		if (garage != null)
		{
			garage.car = car;
			bool num = car == ourCar;
			CarAIController component2 = car.GetComponent<CarAIController>();
			if (num)
			{
				component.KeepRowComposition.RacingTargets = garage.path.GetPathPoints();
				component.KeepRowComposition.CurrentTargetIndex = 1;
				_carInputManager.TargetSpeed = 10f;
				_carInputManager.State = 2;
			}
			else
			{
				component2.targets = garage.path.GetPathPoints();
				component2.CurrentTarIndex = 2;
				component2.Sign = 1;
				component2.TargetSpeed = 15 + UnityEngine.Random.Range(-3, 3);
			}
		}
		if (car == DragRacingCar1)
		{
			DragRacingCar1 = null;
		}
		if (car == DragRacingCar2)
		{
			DragRacingCar2 = null;
		}
		return garage;
	}

	private Garage GetEmptyGarage()
	{
		foreach (Garage garage in Garages)
		{
			if (garage.car == null)
			{
				return garage;
			}
		}
		return deleteCarInGarage();
	}

	private Garage GetOwnGarage(GameObject car)
	{
		if (car == V.LastUsedCar)
		{
			return Garages[0];
		}
		foreach (Garage garage in Garages)
		{
			if (garage.car == car)
			{
				return garage;
			}
		}
		return null;
	}

	private bool alreadyInGarage(GameObject car)
	{
		foreach (Garage garage in Garages)
		{
			if (garage.car == car)
			{
				return true;
			}
		}
		return false;
	}

	private Garage deleteCarInGarage()
	{
		foreach (Garage garage in Garages)
		{
			if (InWinnerList(garage.car) || !(garage.car != ourCar) || !(garage.car != nextRivalCar))
			{
				continue;
			}
			VehicleData component = garage.car.GetComponent<VehicleData>();
			if (component.racingState == RacingState.onGarage)
			{
				if ((bool)component.CarValues.driver)
				{
					UnityEngine.Object.Destroy(component.CarValues.driver);
				}
				UnityEngine.Object.Destroy(garage.car);
				return garage;
			}
		}
		return null;
	}

	public void ToRaceAgain(GameObject car)
	{
		if (car.GetComponent<VehicleData>().racingState == RacingState.toRaceAgain)
		{
			SetCarToRacing(car, setToPosition: false);
		}
	}

	private void PrepareNextRandomCarsData()
	{
		_prepareNextRandomCarsDataDisposable?.Dispose();
		if ((bool)dragRacingTree.CarLeft || (bool)dragRacingTree.CarRight)
		{
			return;
		}
		nextRandomCars = new NextRandomCars();
		int input = UnityEngine.Random.Range(8, 14);
		MainCarValues[] array = FilterBannedCars(allRacingCarClassList.GetClassList(input));
		int num = UnityEngine.Random.Range(0, array.Length);
		if (carRacingID != -1)
		{
			num = carRacingID;
		}
		MainCarValues car1Values = UnityEngine.Object.Instantiate(array[num]);
		nextRandomCars.car1Values = car1Values;
		int num2;
		do
		{
			num2 = UnityEngine.Random.Range(0, array.Length);
		}
		while (carRacingID == -1 && num2 == num);
		if (carRacingID != -1)
		{
			num2 = carRacingID;
		}
		car1Values = UnityEngine.Object.Instantiate(array[num2]);
		car1Values.timeClass = input;
		nextRandomCars.car2Values = car1Values;
		Resources.LoadAsync<GameObject>($"car{nextRandomCars.car1Values.CarID}");
		Resources.LoadAsync<GameObject>($"car{nextRandomCars.car2Values.CarID}");
		dragRacingUI.ShowRandomCarRaceNotification(nextRandomCars.car1Values, nextRandomCars.car2Values);
		if (!dragRacingCamera.virtualCameraOnToLookOtherRace.enabled)
		{
			_prepareNextRandomCarsDataDisposable = Observable.Timer(TimeSpan.FromSeconds(30.0)).Subscribe((long _) =>
			{
				PrepareNextRandomCarsData();
			}).AddTo(this);
		}
	}

	public void ShowRandomCarRacePressed()
	{
		IsRandomCarsRace = true;
		dragRacingCamera.virtualCameraOnToLookOtherRace.enabled = true;
		dragRacingUI.nowRacingNotificationWindow.SetActive(value: false);
		RandomOtherCarRace();
	}

	public void RandomOtherCarRace()
	{
		if (dragRacingTree.CarLeft == nextRivalCar || dragRacingTree.CarRight == nextRivalCar)
		{
			return;
		}
		int num = 0;
		foreach (GameObject allCar in allCars)
		{
			if ((bool)allCar)
			{
				num++;
			}
		}
		if (num <= 15)
		{
			if (!nextRandomCars.car1Values)
			{
				PrepareNextRandomCarsData();
			}
			if (dragRacingCamera.virtualCameraOnToLookOtherRace.enabled)
			{
				DestroyCarsOnRacingTree();
				dragRacingTree.ResetTree();
				DragRacingCar2 = CreateRaceCar(nextRandomCars.car1Values);
				SetCarToRacing(DragRacingCar2, setToPosition: true);
				DragRacingCar1 = CreateRaceCar(nextRandomCars.car2Values);
				SetCarToRacing(DragRacingCar1, setToPosition: true);
				dragRacingUI.ShowOtherRacingCar();
				SoundEventChannel.InvokeDragSpeakerEvent("Pre_Start");
				_inputMapSetEvent.TrySetGroup(InputGroupsEnum.DragSpectator);
			}
		}
	}

	public void RaceWithTestCar()
	{
		_placePersonOnGarageDisposable?.Dispose();
		_placePersonOnGarageDisposable = null;
		testRun = false;
		bool uIForRacing = false;
		if (ourCar.TryGetComponent<Powertrain>(out var component))
		{
			uIForRacing = component.IsAutomaticGearbox;
		}
		dragRacingUI.SetUIForRacing(uIForRacing);
		dragRacingCamera.SetCameraForRacing();
		DestroyCarsOnRacingTree();
		_coreGameManager.CarOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		dragRacingTree.ClearRacingPath();
		dragRacingTree.ResetTree();
		SetCarToRacing(ourCar, setToPosition: true);
		DragRacingCar2 = ourCar;
		DragRacingCar1 = CreateRaceCar(testCarToLoad);
		nextRivalCar = DragRacingCar1;
		SetCarToRacing(DragRacingCar1, setToPosition: true);
		MainCarValues carValues = DragRacingCar1.GetComponent<VehicleData>().CarValues;
		_coreGameManager.CarOutInControl.InCar(carValues.driver, nextRivalCar).Forget();
		dragRacingCamera.RacingCarToLook = DragRacingCar1.transform;
		dragRacingCamera.SetCameraForRacing();
		ClearAllCars(rightCar, leftCar);
		SpeedometerUI speedometerUI = _controlItemsProvider.SpeedometerUI;
		if ((bool)speedometerUI)
		{
			speedometerUI.SetNewCar(DragRacingCar1);
		}
	}

	public void RaceWithNextCar()
	{
		_placePersonOnGarageDisposable?.Dispose();
		_placePersonOnGarageDisposable = null;
		DestroyCarsOnRacingTree();
		_coreGameManager.CarOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		dragRacingTree.ClearRacingPath();
		dragRacingTree.ResetTree();
		SetCarToRacing(ourCar, setToPosition: true);
		DragRacingCar2 = ourCar;
		DragRacingCar1 = nextRivalCar;
		SetCarToRacing(DragRacingCar1, setToPosition: true);
		MainCarValues carValues = nextRivalCar.GetComponent<VehicleData>().CarValues;
		_coreGameManager.CarOutInControl.InCar(carValues.driver, nextRivalCar).Forget();
		dragRacingCamera.RacingCarToLook = DragRacingCar1.transform;
	}

	public void StartTestRun()
	{
		_finishResultPending = false;
		_pendingGarageReturn = false;
		_placePersonOnGarageDisposable?.Dispose();
		_placePersonOnGarageDisposable = null;
		bool uIForRacing = false;
		if (ourCar.TryGetComponent<Powertrain>(out var component))
		{
			uIForRacing = component.IsAutomaticGearbox;
		}
		dragRacingUI.SetUIForRacing(uIForRacing);
		dragRacingCamera.SetCameraForRacing();
		testRun = true;
		DestroyCarsOnRacingTree();
		_coreGameManager.CarOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		dragRacingTree.ClearRacingPath();
		dragRacingTree.ResetTree();
		SetCarToRacing(ourCar, setToPosition: true);
		DragRacingCar2 = ourCar;
		dragRacingCamera.RacingCarToLook = null;
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarDrag, setRoot: true);
	}

	public void StartQualificationRun()
	{
		qualification = true;
		StartTestRun();
	}

	public void DestroyRacingCar(GameObject car)
	{
		if ((bool)car && car != ourCar && car != nextRivalCar && !InWinnerList(car))
		{
			VehicleData component = car.GetComponent<VehicleData>();
			if ((bool)component && (bool)component.CarValues && (bool)component.CarValues.driver)
			{
				UnityEngine.Object.Destroy(component.CarValues.driver);
			}
			if ((bool)component && component.WheelComposition != null && (bool)component.WheelComposition.SkidMark)
			{
				UnityEngine.Object.Destroy(component.WheelComposition.SkidMark);
			}
			UnityEngine.Object.Destroy(car);
		}
	}

	private void DestroyCarsOnRacingTree()
	{
		if ((bool)dragRacingTree.CarLeft)
		{
			DestroyRacingCar(dragRacingTree.CarLeft);
		}
		if ((bool)dragRacingTree.CarRight)
		{
			DestroyRacingCar(dragRacingTree.CarRight);
		}
	}

	public void ResetCarPositions()
	{
		_finishResultPending = false;
		_pendingGarageReturn = false;
		dragRacingTree.ResetTree();
		if (qualification)
		{
			GameObject car = ((DragRacingCar1 == ourCar) ? DragRacingCar1 : DragRacingCar2);
			SetCarToRacing(car, setToPosition: true);
		}
		else
		{
			dragRacingTree.ResetTree();
			if ((bool)DragRacingCar1)
			{
				SetCarToRacing(DragRacingCar1, setToPosition: true);
			}
			if ((bool)DragRacingCar2)
			{
				SetCarToRacing(DragRacingCar2, setToPosition: true);
			}
		}
		dragRacingUI.closeResultWindow();
		SoundEventChannel.InvokeDragSpeakerEvent("Pre_Start");
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarDrag);
	}

	public void OnPlayerOutCar()
	{
		_placePersonOnGarageDisposable?.Dispose();
		_placePersonOnGarageDisposable = null;
		_controlItemsProvider.InputTouch.leftJoy.SetActive(value: true);
		_controlItemsProvider.InputTouch.rightJoy.SetActive(value: true);
		dragRacingUI.SetUIForWalking();
		dragRacingCamera.setCameraForPersonWalking();
		_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, ourCar);
		V.DriverPerson.GetComponent<TouchMovePerson>().enabled = true;
		if (alreadyInGarage(V.LastUsedCar))
		{
			Garage ownGarage = GetOwnGarage(V.LastUsedCar);
			Vector3 vector = V.LastUsedCar.transform.position + V.LastUsedCar.transform.forward * 4f;
			float y = ownGarage.carPos.position.y;
			RaycastHit hitInfo2;
			if (Physics.Raycast(new Vector3(vector.x, ownGarage.carPos.position.y + 3f, vector.z), Vector3.down, out var hitInfo, 10f, LayerMask.GetMask("map"), QueryTriggerInteraction.Ignore))
			{
				y = hitInfo.point.y + 0.1f;
			}
			else if (Physics.Raycast(new Vector3(V.LastUsedCar.transform.position.x, ownGarage.carPos.position.y + 3f, V.LastUsedCar.transform.position.z), Vector3.down, out hitInfo2, 10f, LayerMask.GetMask("map"), QueryTriggerInteraction.Ignore))
			{
				vector = V.LastUsedCar.transform.position;
				y = hitInfo2.point.y + 0.1f;
			}
			vector = new Vector3(vector.x, y, vector.z);
			V.DriverPerson.transform.position = vector;
		}
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.PersonDrag, setRoot: true);
	}

	public void OutOfCars()
	{
		foreach (GameObject allCar in allCars)
		{
			if (!(allCar == null))
			{
				GameObject driver = allCar.GetComponent<VehicleData>().Driver;
				if ((bool)driver)
				{
					_coreGameManager.CarOutInControl.OutCar(driver, allCar);
				}
			}
		}
		_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, ourCar);
	}

	private bool RaceAgain(GameObject car1, GameObject car2)
	{
		Dragy component = car1.GetComponent<Dragy>();
		Dragy component2 = car2.GetComponent<Dragy>();
		if (car1 != ourCar && car2 != ourCar)
		{
			return false;
		}
		if (component.reactionTime < 0f && component2.reactionTime < 0f && component.falseStartCount > 1 && component2.falseStartCount > 1)
		{
			component.falseStartCount--;
			component2.falseStartCount--;
			return true;
		}
		if ((component.reactionTime < 0f || component2.reactionTime < 0f) && component.falseStartCount < 2 && component2.falseStartCount < 2)
		{
			return true;
		}
		return false;
	}

	public void BackToCar()
	{
		_coreGameManager.CarOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		OnGarageOpen();
	}

	private void OnBackToCarInput()
	{
		if (!_toCarButton)
		{
			_toCarButton = FindToCarButton();
		}
		if ((bool)_toCarButton && _toCarButton.isActiveAndEnabled && _toCarButton.interactable)
		{
			BackToCar();
		}
	}

	private Button FindToCarButton()
	{
		GameObject[] uIForWalk = dragRacingUI.UIForWalk;
		foreach (GameObject gameObject in uIForWalk)
		{
			if ((bool)gameObject && gameObject.TryGetComponent<Button>(out var component) && CallsBackToCar(component))
			{
				return component;
			}
		}
		return null;
	}

	private static bool CallsBackToCar(Button button)
	{
		for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
		{
			if (button.onClick.GetPersistentMethodName(i) == "BackToCar")
			{
				return true;
			}
		}
		return false;
	}

	public void PlacePersonOnGarage(GameObject person, GameObject car)
	{
		if (!person)
		{
			return;
		}
		Rigidbody rigidbody = car.GetComponent<Rigidbody>();
		if (!rigidbody)
		{
			UnityEngine.Object.Destroy(person);
			return;
		}
		_placePersonOnGarageDisposable = (from _ in Observable.EveryUpdate()
			where (bool)rigidbody && rigidbody.velocity.magnitude <= 0.1f
			select _).First().SelectMany((long _) =>
		{
			if ((bool)car.GetComponent<VehicleData>().Driver)
			{
				_coreGameManager.CarOutInControl.OutCar(person, car);
			}
			Vector3 position = car.transform.position;
			person.transform.position = position + car.transform.right * 2f;
			person.transform.LookAt(position + car.transform.forward * 5f);
			return Observable.Timer(TimeSpan.FromSeconds(2.0));
		}).Subscribe((long __) =>
		{
			if (!podium.gameObject.activeSelf)
			{
				V.DriverPerson.GetComponent<TouchMovePerson>().enabled = false;
				V.DriverPerson.GetComponent<NewMoveController>().playingRandom = false;
				if ((bool)person)
				{
					person.GetComponent<DriverActionController>().ik.enabled = true;
				}
			}
		})
			.AddTo(this);
	}

	public void SetWinnerPersons()
	{
		podium.SetPersonForPodiums(winners[0].car, winners[1].car, winners[2].car).Forget();
	}

	public void ClearAllCars(GameObject exclude1, GameObject exclude2)
	{
	}
}
