using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Common;
using CPM.Database;
using CPM.PlayerData;
using CPM.Transmission;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Cargo;
using Service.Delivery;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Providers;

public class CircuitCarsControl : MonoBehaviour
{
	public const int MinRamSize = 1600;

	public BotTuningSO BotTuningSO;

	[SerializeField]
	private CircuitRaceControlller CircuitRaceControlller;

	public GameObject ourCar;

	public Transform spawnPoint;

	public List<Transform> racerSP;

	public RacingCameraController cameraController;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	public IPrefabFactory _prefabFactory;

	public List<GameObject> allCars;

	public MainCarValues oneValue;

	public LayerMask carRaycastLayer;

	private List<LapRaceCar> _raceCars = new List<LapRaceCar>();

	private LapRaceCar _userLapRaceCar;

	private int _userId;

	private bool _lowMemory;

	private int _localCount;

	private GameObject _testCar;

	private CircuitRacingConfig _circuitRacingConfig;

	private PlayerDataStorage _playerDataStorage;

	private CarOutInControl _carOutInControl;

	private GlobalCollectionsProvider _collectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private RacingProvider _racingProvider;

	private CarInputManager _carInputManager;

	private const float UserMinTurnSpeedCoeficent = 2f;

	private const float UserMaxTurnSpeedCoeficent = 6f;

	private const float RoadProbeRayStartHeight = 5f;

	private const float RoadProbeRayLength = 60f;

	private MainCarValues _userCarValues;

	private int _currentLaneIndex;

	[SerializeField]
	private Transform[] GaragePoints;

	[Header("Bot isolated physics")]
	[Tooltip("Optional per-class frozen physics profiles for race bots. If unset, sane code defaults apply and bots are still isolated from physics/engine/parts edits.")]
	[SerializeField]
	private BotRacePhysicsCatalogSO BotPhysicsCatalog;

	[SerializeField]
	private BotPersonalityCatalogSO BotPersonalityCatalog;

	private bool _isFormula;

	private bool _isDestroyed;

	private AllCarsInfo _allCarsInfo;

	private MainCarValues[] _enabled;

	public List<CircuitCarStearing> CircuitCarStearings = new List<CircuitCarStearing>();

	private readonly UniTaskCompletionSource _carReadyTcs = new UniTaskCompletionSource();

	public UniTask CarReady => _carReadyTcs.Task;

	[Inject]
	public void Construct(PlayerDatas.CarLoadingUtility carLoadingUtility, GlobalCollectionsProvider collectionsProvider, IPrefabFactory prefabFactory, ManagerUI managerUI, CircuitRacingConfig circuitRacingConfig, PlayerDataStorage playerDataStorage, CarOutInControl carOutInControl, FreeDriveProvider freeDriveProvider, RacingProvider racingProvider, CarInputManager carInputManager, AllCarsInfo allCarsInfo)
	{
		_racingProvider = racingProvider;
		_freeDriveProvider = freeDriveProvider;
		_collectionsProvider = collectionsProvider;
		_prefabFactory = prefabFactory;
		_carLoadingUtility = carLoadingUtility;
		_circuitRacingConfig = circuitRacingConfig;
		_playerDataStorage = playerDataStorage;
		_carOutInControl = carOutInControl;
		_carInputManager = carInputManager;
		_allCarsInfo = allCarsInfo;
	}

	private void Start()
	{
		CreateUsersCar(this.GetCancellationTokenOnDestroy()).Forget();
		carRaycastLayer = (int)carRaycastLayer | LayerMask.GetMask("car");
		_lowMemory = IsLowRam();
	}

	private async UniTaskVoid CreateUsersCar(CancellationToken ct)
	{
		try
		{
			ourCar = await CreateUserAndCar(ct);
		}
		catch (OperationCanceledException)
		{
			_carReadyTcs.TrySetCanceled();
			return;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			_carReadyTcs.TrySetCanceled();
			return;
		}
		if (this == null)
		{
			_carReadyTcs.TrySetCanceled();
			return;
		}
		_carReadyTcs.TrySetResult();
		SetFirstOwnCar().Forget();
	}

	public async UniTask<bool> CreateRaceCarsAsync()
	{
		await CreateSeveralRaceCars();
		return true;
	}

	public List<LapRaceCar> GetRaceCars()
	{
		return _raceCars;
	}

	public void StartTest()
	{
		CircuitRaceControlller.PrepareMapForTest();
		CreateOneTestCar();
	}

	private void changeFocusOnCam()
	{
		_localCount++;
		if (_localCount > allCars.Count)
		{
			_localCount = 0;
		}
		if (!allCars[_localCount])
		{
			allCars.RemoveAt(_localCount);
			changeFocusOnCam();
			return;
		}
		GameObject camera = allCars[_localCount];
		if (_localCount == 0)
		{
			camera = V.Car;
		}
		cameraController.setCamera(camera);
		cameraController.SetCameraForRacing();
	}

	private MainCarValues GetRacingCar(int classIndex, int carIndex)
	{
		MainCarValues[] raceCarsConfigs = _circuitRacingConfig.MapDatas[CircuitRaceControlller.GetCurrentMapIndex()].Groups[classIndex].RaceCarsConfigs;
		if (_enabled == null)
		{
			_enabled = raceCarsConfigs.Where((MainCarValues v) => v != null && !_allCarsInfo.IsBannedCar(v.CarID)).ToArray();
		}
		if (_enabled.Length == 0)
		{
			return raceCarsConfigs[carIndex];
		}
		return _enabled[carIndex % _enabled.Length];
	}

	public void SetOwnCarToGarage()
	{
		Powertrain component = ourCar.GetComponent<Powertrain>();
		VehicleData component2 = ourCar.GetComponent<VehicleData>();
		ourCar.transform.rotation = spawnPoint.rotation;
		if (Vector3.Distance(b: ourCar.transform.position, a: spawnPoint.position) > 1f)
		{
			ourCar.transform.position = SnapToRoad(ourCar, spawnPoint.position);
		}
		component2.racingState = RacingState.onGarage;
		component.SetAlignCar(state: true);
	}

	public void SetIgnoreCollisionWithCollider(LapRaceCar car, bool ignore)
	{
		if (!car.VehicleData)
		{
			return;
		}
		foreach (LapRaceCar raceCar in _raceCars)
		{
			if (!(raceCar == car))
			{
				CarColliders.SetIgnore(car.VehicleData, raceCar.VehicleData, ignore);
			}
		}
	}

	private Vector3 SnapToRoad(GameObject car, Vector3 point)
	{
		int mask = LayerMask.GetMask("Road", "Terrain", "Default");
		if (Physics.Raycast(point + Vector3.up * 5f, Vector3.down, out var hitInfo, 60f, mask, QueryTriggerInteraction.Ignore))
		{
			return VectorExtensions.GetPosByWheel(car, hitInfo.point);
		}
		return point + CarYOffset(car);
	}

	private Vector3 CarYOffset(GameObject car)
	{
		float num;
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

	private void SetOwnCarForCircuitRacing(GameObject car)
	{
		car.transform.rotation = spawnPoint.transform.rotation;
		car.transform.position = SnapToRoad(car, spawnPoint.transform.position);
		PlayerDatas.CreateSkid(car);
		VehicleData component = ourCar.GetComponent<VehicleData>();
		Powertrain getPowertrain = component.GetPowertrain;
		component.SetEngineState(toStart: true);
		getPowertrain.SetKinematic(value: false);
		getPowertrain.SetAlignCar(state: false);
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

	public async UniTask CreateQualificationCar(Transform position)
	{
		for (int i = 0; i < _raceCars.Count(); i++)
		{
			_raceCars[i].SetActive(_raceCars[i] == _userLapRaceCar);
		}
		PublishUserToStartPoint(position, ourCar, _raceCars.Count - 1).Forget();
		_userLapRaceCar.SetLaps(CircuitRaceControlller.GetRaceMapModel().RaceConfig.QualificationLapsCount);
	}

	public async UniTask CreateSeveralRaceCars()
	{
		CircuitRaceControlller.CurrentClassId.Get(out int value);
		_isFormula = value == 6;
		int num = V.Car.GetComponent<VehicleEntityProvider>().CarEntity.PerformancePoints;
		Transform[] startPositions = CircuitRaceControlller.GetStartPositions();
		List<MainCarValues> carsValues = GetClassCarValues(startPositions.Length, num);
		int userIndex = -1;
		for (int i = 0; i < carsValues.Count; i++)
		{
			if (carsValues[i].IsUser)
			{
				userIndex = i;
				break;
			}
		}
		float num2 = 0f;
		int num3 = 0;
		for (int j = 0; j < carsValues.Count; j++)
		{
			if (!carsValues[j].IsUser && carsValues[j].PerformanceScore > 0)
			{
				num2 += (float)carsValues[j].PerformanceScore;
				num3++;
			}
		}
		num2 = ((num3 > 0) ? (num2 / (float)num3) : 0f);
		float ppPaceScale = ((num2 > 0f && num > 0) ? Mathf.Clamp(Mathf.Sqrt((float)num / num2), 1f, 2f) : 1f);
		List<UniTask> freezeTasks = new List<UniTask>();
		for (int k = 0; k < carsValues.Count; k++)
		{
			if (!carsValues[k].IsUser)
			{
				bool isRivalSlot = userIndex >= 0 && Mathf.Abs(k - userIndex) == 1;
				LapRaceCar lapRaceCar = await CreateTestRacerCar(GaragePoints[k], k, carsValues[k], isRivalSlot, ppPaceScale);
				if ((bool)lapRaceCar)
				{
					freezeTasks.Add(FreezeAfterSettleAsync(lapRaceCar));
				}
			}
		}
		await UniTask.WhenAll(freezeTasks);
		await PublishUserToStartPoint(spawnPoint, ourCar, carsValues.Count);
		CreateBoxAroundForFormula(ourCar);
		if (ourCar.TryGetComponent<DriverController>(out var component) && !component.DriverPerson)
		{
			V.CoreGameManager.SetupCarDriver(ourCar);
		}
	}

	private void CreateBoxAroundForFormula(GameObject car)
	{
		if (_isFormula)
		{
			Powertrain component = car.GetComponent<Powertrain>();
			BoxCollider boxCollider = car.AddComponent<BoxCollider>();
			Bounds getWeightBounds = component.GetWeightBounds;
			boxCollider.center = new Vector3(getWeightBounds.center.x, getWeightBounds.center.y + 0.2f, getWeightBounds.center.z);
			boxCollider.size = new Vector3(getWeightBounds.size.x, getWeightBounds.size.y / 2f, getWeightBounds.size.z);
		}
	}

	public void RestartCars()
	{
		for (int i = 0; i < _raceCars.Count(); i++)
		{
			StopCar(_raceCars[i].gameObject);
			_raceCars[i].Restart();
		}
	}

	public void MoveCarsToSpawnPoints()
	{
		Transform[] startPositions = CircuitRaceControlller.GetStartPositions();
		for (int i = 0; i < _raceCars.Count; i++)
		{
			_raceCars[i].gameObject.SetActive(value: true);
			Transform obj = _raceCars[i].transform;
			Transform transform = startPositions[i];
			obj.position = transform.position + CarYOffset(_raceCars[i].gameObject);
			obj.LookAt(CircuitRaceControlller.GetRaceMapModel().StartArc.transform);
			obj.rotation = transform.rotation;
			_raceCars[i].GetNearestBezierKnotIndex(_raceCars[i].transform.position);
			_raceCars[i].SetWheelsTemperature();
			if (_raceCars[i].TryGetComponent<CircuitCarStearing>(out var component))
			{
				component.SetGridPosition(i % 5, i / 5);
			}
		}
	}

	public void SetKinematicCars(bool state)
	{
		for (int i = 0; i < _raceCars.Count(); i++)
		{
			Rigidbody rigidbody = _raceCars[i].GetRigidbody();
			if (state)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
			rigidbody.isKinematic = state;
		}
	}

	private List<MainCarValues> GetClassCarValues(int carsCount, int usersPoint)
	{
		CircuitRaceControlller.CurrentClassId.Get(out int value);
		_userCarValues = ScriptableObject.CreateInstance<MainCarValues>();
		_userCarValues.PerformanceScore = usersPoint;
		_userCarValues.IsUser = true;
		List<MainCarValues> list = new List<MainCarValues>();
		if (value == 6)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				int carIndex = component.CarId switch
				{
					155 => 0, 
					165 => 1, 
					_ => 0, 
				};
				for (int i = 0; i < carsCount - 1; i++)
				{
					list.Add(GetRacingCar(value, carIndex));
				}
			}
		}
		else
		{
			for (int j = 0; j < carsCount - 1; j++)
			{
				list.Add(GetRacingCar(value, j));
			}
		}
		_enabled = null;
		return list;
	}

	private async UniTaskVoid CreateOneTestCar()
	{
		DestroyTestCar();
		Transform[] startPositions = CircuitRaceControlller.GetStartPositions();
		Transform sp = startPositions[0];
		float offset = 2f;
		GameObject racerCar = await CreateRaceCar(oneValue, sp.position + Vector3.up * offset);
		await UniTask.Delay(2000);
		racerCar.layer = LayerViaName.CarLayer;
		racerCar.transform.SetPositionAndRotation(sp.position + new Vector3(UnityEngine.Random.Range(1f, 3f), offset, UnityEngine.Random.Range(1f, 3f)), sp.rotation);
		racerCar.GetComponent<RacingCarController>().DontSwitchLOD = true;
		CircuitCarStearing component = racerCar.GetComponent<CircuitCarStearing>();
		component.SetMinSpeedCoefficient(oneValue.MinSpeedTurnCoefficient, oneValue.MaxSpeedTurnCoefficient);
		component.ChangeSteeringState(CarSteeringState.Race);
		component.SetSplineLength();
		component.SetMinSpeed();
		_testCar = racerCar;
		CircuitRaceControlller.StartTestMode(racerCar);
		racerCar.transform.LookAt(CircuitRaceControlller.GetStartLine());
		cameraController.setCamera(racerCar);
		cameraController.SetCameraForRacing();
	}

	private async UniTask<LapRaceCar> CreateTestRacerCar(Transform sp, int index, MainCarValues carValues, bool isRivalSlot = false, float ppPaceScale = 1f)
	{
		string nickName = ((carValues.gender == 0) ? ChatFilter.maleRandomName[UnityEngine.Random.Range(0, ChatFilter.maleRandomName.Length)] : ChatFilter.femaleRandomName[UnityEngine.Random.Range(0, ChatFilter.femaleRandomName.Length)]);
		Vector3 pos = sp.position + Vector3.up * 2f;
		GameObject racerCar = await CreateRaceCar(carValues, pos);
		if (racerCar.TryGetComponent<VehicleData>(out var component) && (bool)component.CarValues)
		{
			component.CarValues.NickName = nickName;
		}
		racerCar.transform.position = pos;
		racerCar.transform.rotation = sp.rotation;
		await UniTask.Delay(10);
		CircuitCarStearing component2 = racerCar.GetComponent<CircuitCarStearing>();
		Powertrain component3 = racerCar.GetComponent<Powertrain>();
		BotRacePhysicsProfileSO profile = null;
		BotPersonality value = BotPersonality.Neutral;
		if (BotPhysicsCatalog != null || BotPersonalityCatalog != null)
		{
			CircuitRaceControlller.CurrentClassId.Get(out int value2);
			if (BotPhysicsCatalog != null)
			{
				profile = BotPhysicsCatalog.GetProfileForClass(value2);
			}
			if (BotPersonalityCatalog != null)
			{
				value = BotPersonalityCatalog.Resolve(index, value2, isRivalSlot);
			}
		}
		component2.EnableRaceBotPhysics(component3, profile, index, value, ppPaceScale);
		if ((bool)component3)
		{
			component3.SetRaceBotWheelInwardOffset(carValues.WheelInwardOffset);
		}
		component2.SetMinSpeedCoefficient(carValues.MinSpeedTurnCoefficient, carValues.MaxSpeedTurnCoefficient);
		LapRaceCar lapRaceCar = racerCar.GetComponent<LapRaceCar>();
		if (!lapRaceCar)
		{
			lapRaceCar = _prefabFactory.InstantiateComponent<LapRaceCar>(racerCar);
		}
		lapRaceCar.InitRaceCar(CircuitRaceControlller, index, CircuitRaceControlller.GetRaceLapsCount(), carValues.CarID, _collectionsProvider);
		lapRaceCar.SetName(nickName);
		Rigidbody rigidbody = lapRaceCar.GetRigidbody();
		if ((bool)rigidbody)
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}
		_raceCars.Add(lapRaceCar);
		CreateBoxAroundForFormula(racerCar);
		return lapRaceCar;
	}

	private static async UniTask FreezeAfterSettleAsync(LapRaceCar lapRaceCar)
	{
		await UniTask.Delay(200);
		if ((bool)lapRaceCar)
		{
			lapRaceCar.FreezeCar(state: true);
		}
	}

	public void SetCarsLapsCount(int lapCount)
	{
		for (int i = 0; i < _raceCars.Count(); i++)
		{
			_raceCars[i].SetLaps(lapCount);
		}
	}

	private async UniTask<GameObject> CreateRaceCar(MainCarValues mainCarValues, Vector3 startPosition)
	{
		if (!mainCarValues)
		{
			return null;
		}
		GameObject gameObject = await _carLoadingUtility.LoadCircuitRacingCar(mainCarValues, startPosition);
		CircuitCarStearing component = gameObject.GetComponent<CircuitCarStearing>();
		component.Init(CircuitRaceControlller);
		CircuitCarStearings.Add(component);
		component.Direction = CircuitRaceControlller.Direction;
		component.SetRaceConfig(CircuitRaceControlller.CurrentRaceConfig);
		allCars.Add(gameObject);
		if (!_lowMemory && gameObject.TryGetComponent<DriverController>(out var component2))
		{
			PlayerDatas.CreateSkid(gameObject);
			GameObject gameObject2 = (component2.DriverPerson = CreateRacerPerson());
			if (gameObject.TryGetComponent<VehicleData>(out var component3) && (bool)component3.CarValues)
			{
				component3.CarValues.driver = gameObject2;
			}
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.DestroyComponent<CapsuleCollider>();
			gameObject2.DestroyComponent<DriverActionController>();
			gameObject2.DestroyComponent<PhotonView>();
			gameObject2.DestroyComponent<AudioSource>();
			gameObject2.DestroyComponent<MultiPersonControl>();
			gameObject2.DestroyComponent<NewMoveController>();
			gameObject2.DestroyComponent<Rigidbody>();
			gameObject2.DestroyComponent<TouchMovePerson>();
			gameObject2.DestroyComponent<Emoji>();
			gameObject2.DestroyComponent<BannerManager>();
			gameObject2.DestroyComponent<TaxiController>();
			gameObject2.DestroyComponent<DeliveryController>();
			gameObject2.DestroyComponent<CargoController>();
			gameObject2.DestroyComponent<ChatWorldMessagesCreator>();
			gameObject2.DestroyComponent<StepSounds>();
			if (gameObject2.TryGetComponent<MainPerson>(out var component4))
			{
				component4.InitRacingDriver(component2);
			}
		}
		if (Application.isEditor)
		{
			gameObject.AddComponent<RealtimeAnalyze>();
		}
		return gameObject;
	}

	public void SetCarsDirection()
	{
		bool flag = false;
		_ = CircuitRaceControlller.Direction;
		_ = 0;
		for (int i = 0; i < _raceCars.Count(); i++)
		{
			_raceCars[i].SetDirection(GetNeedLaneIndex(flag));
			flag = !flag;
		}
	}

	private int GetNeedLaneIndex(bool state)
	{
		if (state)
		{
			if (CircuitRaceControlller.Direction <= 0)
			{
				return -1;
			}
			return 1;
		}
		if (CircuitRaceControlller.Direction <= 0)
		{
			return 1;
		}
		return -1;
	}

	public void SetCarToRoad(LapRaceCar lapRaceCar, Vector3 position, Quaternion rotation)
	{
		lapRaceCar.transform.position = position + CarYOffset(lapRaceCar.gameObject);
		lapRaceCar.transform.rotation = rotation;
		lapRaceCar.GetRigidbody().velocity = Vector3.zero;
		lapRaceCar.GetRigidbody().angularVelocity = Vector3.zero;
	}

	public async UniTaskVoid SetFirstOwnCar()
	{
		SetOwnCarForCircuitRacing(ourCar);
	}

	private async UniTask<GameObject> CreateUserAndCar(CancellationToken ct)
	{
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
		V.Car = (await _carLoadingUtility.LoadCarFromSlot(V.SlotID, spawnPoint.transform.position, spawnPoint.transform.rotation, tryUnloadCurrent: false, waitBodyLoad: false, FlagsStorage.IsMultiFree)).Item2;
		ct.ThrowIfCancellationRequested();
		if (this == null)
		{
			return null;
		}
		V.LastUsedCar = V.Car;
		if ((bool)V.Car)
		{
			V.Car.transform.position = SnapToRoad(V.Car, spawnPoint.transform.position);
		}
		return V.Car;
	}

	private async UniTask PublishUserToStartPoint(Transform placeTransform, GameObject car, int index)
	{
		_userId = index;
		CircuitCarStearing circuitCarStearing = car.GetComponent<CircuitCarStearing>();
		if (!circuitCarStearing)
		{
			circuitCarStearing = car.AddComponent<CircuitCarStearing>();
			circuitCarStearing.Init(CircuitRaceControlller);
			circuitCarStearing.SetMinSpeedCoefficient(2f, 6f);
			circuitCarStearing.enabled = false;
			circuitCarStearing.SetRaceConfig(CircuitRaceControlller.CurrentRaceConfig);
			circuitCarStearing.ChangeSteeringState(CarSteeringState.Race);
			CarAIController carAIController = circuitCarStearing.GetComponent<CarAIController>();
			if (!carAIController)
			{
				carAIController = circuitCarStearing.gameObject.AddComponent<CarAIController>();
			}
			circuitCarStearing.Direction = CircuitRaceControlller.Direction;
			carAIController.Sign = 1;
			carAIController.TargetSpeed = 0f;
			carAIController.SteerAssistent = true;
			carAIController.AiType = AiType.CircuitRacingCar;
			carAIController.randomOffset = UnityEngine.Random.insideUnitSphere * 0.7f;
			carAIController.randomOffset.y = 0f;
			carAIController.circuitCarSteering = circuitCarStearing;
			circuitCarStearing.AiController = carAIController;
			circuitCarStearing.MinSpeedThreshold = 150f;
			circuitCarStearing.BrakeSpeedInPercent = 0.1f;
			circuitCarStearing.MinAngleThreshold = 0.1f;
			circuitCarStearing.NextTargetCoefficient = 1f;
		}
		else
		{
			circuitCarStearing.Direction = CircuitRaceControlller.Direction;
		}
		CircuitCarStearings.Add(circuitCarStearing);
		circuitCarStearing.SetMinSpeed();
		LapRaceCar lapRaceCar = car.GetComponent<LapRaceCar>();
		if (!lapRaceCar)
		{
			lapRaceCar = _prefabFactory.InstantiateComponent<LapRaceCar>(car);
		}
		if (car.TryGetComponent<CarTriggers>(out var component))
		{
			component.FreeDriveProvider = _freeDriveProvider;
			component.RacingProvider = _racingProvider;
		}
		VehicleData component2 = car.GetComponent<VehicleData>();
		component2.RacingProvider = _racingProvider;
		int lapCount = (CircuitRaceControlller.IsQualification ? CircuitRaceControlller.GetQualificationRaceLapsCount() : CircuitRaceControlller.GetRaceLapsCount());
		lapRaceCar.InitRaceCar(CircuitRaceControlller, index, lapCount, component2.CarId, _collectionsProvider, isUser: true);
		lapRaceCar.SetName(_playerDataStorage.PlayerName);
		Rigidbody rigidbody = lapRaceCar.GetRigidbody();
		if ((bool)rigidbody)
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}
		if (!_userLapRaceCar)
		{
			_raceCars.Add(lapRaceCar);
		}
		_userLapRaceCar = lapRaceCar;
		await UniTask.Yield();
		lapRaceCar.transform.rotation = placeTransform.rotation;
		lapRaceCar.transform.position = SnapToRoad(car, placeTransform.position);
		lapRaceCar.FreezeCar(state: true);
		lapRaceCar.ResetTransmission();
		lapRaceCar.transform.rotation = placeTransform.rotation;
	}

	public LapRaceCar GetUserLapRaceCar()
	{
		return _userLapRaceCar;
	}

	public int GetUserId()
	{
		return _userId;
	}

	public void SetCarToRacing(GameObject car)
	{
		_carOutInControl.InCar(V.DriverPerson, ourCar).Forget();
		bool num = car == ourCar;
		Powertrain component = car.GetComponent<Powertrain>();
		StopCar(car);
		if (car.TryGetComponent<Dragy>(out var component2))
		{
			component2.Reset();
		}
		else
		{
			car.AddComponent<Dragy>();
		}
		if (car.GetComponent<CarTriggers>() == null)
		{
			car.AddComponent<CarTriggers>();
		}
		if (car.TryGetComponent<VehicleData>(out var component3))
		{
			component3.racingState = RacingState.OnRacing;
		}
		component.SetAlignCar(state: false);
		CarAIController component4;
		if (num)
		{
			_carInputManager.State = 3;
			_carInputManager.TargetSpeed = 10f;
			CircuitRaceControlller.SetSteering();
			bool flag = component3.CarValues.Gearbox == GearboxKind.MT;
			if (flag)
			{
				component.Automatic = false;
			}
			component.AutoClutch = !flag;
		}
		else if (car.TryGetComponent<CarAIController>(out component4))
		{
			component4.CurrentTarIndex = 2;
			component4.Sign = 1;
			component4.AiType = AiType.DragRacingCar;
		}
		else
		{
			car.AddComponent<CarAIController>();
		}
	}

	public void StopCar(GameObject carGameObject)
	{
		CarComponents carComponents = new CarComponents(carGameObject);
		MonoBehaviour.print("stop car");
		Powertrain powertrain = carComponents.Powertrain;
		if (carComponents.GameObject == ourCar)
		{
			_carInputManager.PressThrottle(0f);
			_carInputManager.OnStopCarOnStaged = true;
		}
		CarAIController carAIController = carComponents.CarAIController;
		if ((bool)carAIController && carAIController.AiType != AiType.None)
		{
			carAIController.onStopCarOnStaged = true;
			carAIController.TargetSpeed = 0f;
		}
		powertrain.ResetTransmission(resetGear: false);
	}

	public void SetAiSpeed(GameObject car, float speed)
	{
		_carInputManager.OnStopCarOnStaged = true;
		_carInputManager.TargetSpeed = speed;
	}

	public void DestroyAiCars()
	{
		for (int i = 0; i < _raceCars.Count; i++)
		{
			if (!_raceCars[i].IsUser())
			{
				DriverController component = _raceCars[i].GetComponent<DriverController>();
				if ((bool)component && (bool)component.DriverPerson)
				{
					UnityEngine.Object.Destroy(component.DriverPerson);
				}
				UnityEngine.Object.Destroy(_raceCars[i].gameObject);
			}
		}
		_raceCars = new List<LapRaceCar>();
		CircuitCarStearings = new List<CircuitCarStearing>();
		PlayerDatas.TryUnloadResource();
	}

	public void DestroyTestCar()
	{
		if ((bool)_testCar)
		{
			DriverController component = _testCar.GetComponent<DriverController>();
			if ((bool)component && (bool)component.DriverPerson)
			{
				UnityEngine.Object.Destroy(component.DriverPerson);
			}
			UnityEngine.Object.Destroy(_testCar);
		}
	}

	private bool IsLowRam()
	{
		return SystemInfo.systemMemorySize <= 1600;
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		_carReadyTcs.TrySetCanceled();
	}

	public void SetConfigToStearing(RaceConfigSO raceConfig)
	{
		for (int i = 0; i < CircuitCarStearings.Count; i++)
		{
			CircuitCarStearings[i].SetRaceConfig(raceConfig);
		}
	}
}
