using System;
using System.Threading;
using CPM.Database;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Persistence.Systems;
using Photon.Pun;
using Service.Abstract;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.HUD;

public class QuickCarSettings : MonoBehaviour
{
	private const float TimeToSetActiveFillAmoutCircle = 0.2f;

	private const float TimeToSetActiveFalseFillAmoutCircle = 1.2f;

	private const float DistanceToSetActiveDoorsButtons = 4f;

	private const float DelayMoveSideDoor = 1f;

	public GameObject HideButton;

	public GameObject ShowButton;

	public GameObject AnalogClutch;

	public GameObject carDamage;

	public GameObject RepairButton;

	public ButtonUpDown Door;

	public ButtonUpDown DoorPassenger;

	public Image DoorImage;

	public Image DoorFillAmountImage;

	public Button DoorHood;

	public Button DoorTrunk;

	public Button RoofButton;

	public Image DriverDoor;

	public Sprite ExitDoorIcon;

	public Sprite DriverDoorIcon;

	public bool showDoor = true;

	public GameObject joySticks;

	public int oldHState;

	public GameObject hoodButton;

	private MenuLevelParent _menuLevelParent;

	private MultiPlayer _multiPlayer;

	private CoreGameManager _coreGameManager;

	private OptionalDependency<HudColorChanger> _hudColorChanger;

	private OptionalDependency<MultiPlayer> _multiPlayerDep;

	private PlayerComponent _playerComponent;

	private LockCarButtonController _lockCarButtonController;

	private MainPerson _mainPersonDriverPerson;

	private ManagerUI _managerUI;

	private AllCarsInfo _allCarsInfo;

	[SerializeField]
	private Button SideDoorButton;

	[SerializeField]
	private Image SideDoorImage;

	[SerializeField]
	private Sprite SideDoorImageOpen;

	[SerializeField]
	private Sprite SideDoorImageClose;

	private LayerMask _layerMask;

	private LayerMask _ignoreObstacleMask;

	private const float FallbackRadius = 0.4f;

	private const float DistanceToCheckObstacles = 2f;

	private const float DoorClearance = 0.5f;

	private const float DoorCheckDistance = 1f;

	private InputItemsProvider _inputItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private CarInputManager _carInputManager;

	private RegistryHandle<BoolEvent> _doorStartCancelHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.DoorStartCancel);

	private RegistryHandle<BoolEvent> _setActionsActiveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetActionsActive);

	private RegistryHandle<BoolEvent> _onHornHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHorn);

	private RegistryHandle<ScriptableEvent> _onHeadlightsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnHeadlights);

	private RegistryHandle<ScriptableEvent> _toggleLeftSignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleLeftSignal);

	private RegistryHandle<ScriptableEvent> _toggleRightSignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleRightSignal);

	private RegistryHandle<ScriptableEvent> _toggleEmergencySignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleEmergencySignal);

	private CancellationTokenSource _doorCts;

	private int _currentGear = 4;

	private bool _doorPerformed;

	private bool _doorAborted;

	private int _doorPressFrame = -1;

	private bool _inputBlockedForAnimation;

	private CancellationTokenSource _inputBlockCts;

	private const float InputBlockTimeoutSeconds = 13f;

	private float _doorPressStart;

	private Powertrain _powerTrain;

	private const float FallbackDoorHeight = 0.8f;

	private const float DoorRayHeight = 0.35f;

	private const float WalkableNormalY = 0.64f;

	private const float DoorDebugDrawSeconds = 5f;

	private const float DoorGroundProbeDistance = 3f;

	private readonly RaycastHit[] _doorRayHits = new RaycastHit[16];

	private readonly Collider[] _doorDebugOverlaps = new Collider[16];

	[field: SerializeField]
	public Button HomeDoorButton { get; private set; }

	[field: SerializeField]
	public Button GarageDoorButton { get; private set; }

	public EventRegistry _eventRegistry { get; private set; }

	public InputMapSetEvent _inputMapSetEvent { get; private set; }

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent, InputItemsProvider inputItemsProvider, FreeDriveProvider freeDriveProvider, CoreGameManager coreGameManager, LockCarButtonController lockCarButtonController, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, ManagerUI managerUI, AllCarsInfo allCarsInfo, CarInputManager carInputManager, OptionalDependency<MultiPlayer> multiPlayer, OptionalDependency<HudColorChanger> hudColorChanger)
	{
		_freeDriveProvider = freeDriveProvider;
		_inputItemsProvider = inputItemsProvider;
		_managerUI = managerUI;
		_lockCarButtonController = lockCarButtonController;
		_hudColorChanger = hudColorChanger;
		_coreGameManager = coreGameManager;
		_multiPlayerDep = multiPlayer;
		_menuLevelParent = menuLevelParent;
		_carInputManager = carInputManager;
		_allCarsInfo = allCarsInfo;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void Awake()
	{
		_multiPlayer = _multiPlayerDep?.Value;
		InitAwake();
	}

	private void Start()
	{
		if (ScenesHelper.IsParkingLevelActive())
		{
			InitStart();
		}
		_layerMask = LayerMask.GetMask("Default", "short distance", "car", "traffic car");
		_ignoreObstacleMask = LayerMask.GetMask("parktronic", "DriverPerson");
		ShowDoor();
		if ((bool)V.Car && V.Car.TryGetComponent<LightController>(out var component))
		{
			UpdateHeadLampIconUI((LightsType)component.HState);
		}
	}

	private void OnDestroy()
	{
		_freeDriveProvider.QuickCarSettings = null;
		if (!ScenesHelper.IsMenuSceneActive())
		{
			onFlashReleased();
		}
		if ((bool)DoorHood && (bool)DoorTrunk)
		{
			DoorTrunk.onClick.RemoveAllListeners();
			DoorHood.onClick.RemoveAllListeners();
		}
		if ((bool)RoofButton)
		{
			RoofButton.onClick.RemoveAllListeners();
		}
		if ((bool)Door)
		{
			Door.RemoveAllListeners();
		}
		if ((bool)DoorPassenger)
		{
			DoorPassenger.RemoveAllListeners();
		}
		if ((bool)SideDoorButton)
		{
			SideDoorButton.onClick.RemoveAllListeners();
		}
	}

	public void InitStart()
	{
		if (!_menuLevelParent)
		{
			_menuLevelParent = V.CoreGameManager.MenuLevelParent;
		}
		if (!_coreGameManager)
		{
			_coreGameManager = V.CoreGameManager;
		}
		_lockCarButtonController.HideKickPassengersButton();
		if ((bool)DoorPassenger)
		{
			DoorPassenger.SetActive(active: false);
		}
		if ((bool)DriverDoor)
		{
			DriverDoor.sprite = ExitDoorIcon;
		}
		PreSetValues();
		if ((bool)V.Car)
		{
			ChangeCarLightMask(V.Car, 1);
			if (V.Car == _freeDriveProvider.FirstCarMultiPlayer)
			{
				_lockCarButtonController.ShowKickPassengersButton(V.Car.GetComponent<VehicleData>());
			}
		}
	}

	public void InitAwake()
	{
		_freeDriveProvider.QuickCarSettings = this;
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			FlagsStorage.PRND = PlayerPrefs.GetInt("PRND", 1) == 1;
		}
		else
		{
			FlagsStorage.PRND = true;
		}
		if ((bool)DoorHood && (bool)DoorTrunk)
		{
			DoorTrunk.onClick.AddListener(TrunkButtonPressed);
			DoorHood.onClick.AddListener(HoodButtonPressed);
		}
		if ((bool)RoofButton)
		{
			RoofButton.onClick.AddListener(RoofButtonPressed);
		}
		if ((bool)Door)
		{
			Door.AddListenerOnPointerDown(TurnOnDoorFillAmountDriverDoor);
		}
		if ((bool)DoorPassenger)
		{
			DoorPassenger.AddListenerOnPointerDown(TurnOnDoorFillAmountPassengerDoor);
		}
		if ((bool)SideDoorButton)
		{
			SideDoorButton.onClick.AddListener(OpenCloseSideDoor);
		}
	}

	public void TurnOnDoorFillAmountDriverDoor()
	{
		if (!V.Car || !V.Car.TryGetComponent<VehicleData>(out var component) || !component.InteriorsComposition.InteriorLoading)
		{
			TurnOnDoorFillAmountAsync(Door, DoorButton).Forget();
		}
	}

	public void TurnOnDoorFillAmountPassengerDoor()
	{
		TurnOnDoorFillAmountAsync(DoorPassenger, DoorButtonPassanger).Forget();
	}

	public async UniTask TurnOnDoorFillAmountAsync(ButtonUpDown doorButton, Action<bool> clickAction)
	{
		_doorAborted = false;
		while ((bool)doorButton)
		{
			if (_doorAborted)
			{
				ResetDoorFillUI();
				break;
			}
			if (doorButton.PressTime == 0f)
			{
				if ((bool)DoorImage)
				{
					DoorImage.gameObject.SetActive(value: false);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.fillAmount = 0f;
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.gameObject.SetActive(value: false);
				}
				clickAction?.Invoke(obj: false);
				break;
			}
			if (doorButton.PressTime >= 0.2f)
			{
				if ((bool)DoorImage)
				{
					DoorImage.gameObject.SetActive(value: true);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.gameObject.SetActive(value: true);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.fillAmount = (doorButton.PressTime - 0.2f) / 1f;
				}
			}
			if (doorButton.PressTime >= 1.2f)
			{
				if ((bool)DoorImage)
				{
					DoorImage.gameObject.SetActive(value: false);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.fillAmount = 0f;
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.gameObject.SetActive(value: false);
				}
				clickAction?.Invoke(obj: true);
				break;
			}
			await UniTask.Yield();
		}
	}

	public void SetGearSelf(int value, bool isParkingGearNeed = false)
	{
		if (!isParkingGearNeed && (value == 0 || value == 1))
		{
			value = 4;
		}
		_inputItemsProvider?.ShifterControl?.SetAutoMode(value);
	}

	public void PreSetValues()
	{
		if (FlagsStorage.InGarageMenu || FlagsStorage.IsMainMenu)
		{
			return;
		}
		int defaultValue = 0;
		if (CPMGraphicsSettingsManager.Instance.Quality >= 2)
		{
			defaultValue = 1;
		}
		if ((bool)V.Car && PlayerPrefs.GetInt("CarDamage", defaultValue) == 1)
		{
			V.Car.GetComponent<VehicleDamage>().isON = true;
		}
		if ((bool)RepairButton)
		{
			bool flag = PlayerPrefs.GetInt("CarDamage", defaultValue) == 1;
			RepairButton.SetActive(flag);
			if ((bool)carDamage)
			{
				carDamage.GetComponent<Toggle>().isOn = flag;
			}
		}
		if ((bool)AnalogClutch)
		{
			AnalogClutch.SetActive(FlagsStorage.IsFreeDriveMode);
		}
	}

	public void SetCarDamage()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleDamage>(out var component))
		{
			component.isON = !component.isON;
			bool isON = component.isON;
			PlayerPrefs.SetInt("CarDamage", isON ? 1 : 0);
			RepairButton.SetActive(isON);
		}
	}

	public void RepairCar()
	{
		if ((bool)V.Car)
		{
			if (FlagsStorage.IsFreeDriveMode)
			{
				_coreGameManager.SetMessage(new LocalizedString("QuickCarSettings.Repaired"), 2f);
			}
			if ((bool)_freeDriveProvider.FreeDriveDB)
			{
				++_freeDriveProvider.FreeDriveDB.carRepair;
			}
			V.Car.GetComponent<VehicleData>().CarHealthComposition.RepairVisual();
		}
	}

	public void TrunkButtonPressed()
	{
		if ((bool)_coreGameManager.CarOutInControl.CarToEnter && (!(_coreGameManager.CarOutInControl.CarToEnter != _freeDriveProvider.FirstCarMultiPlayer) || FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu) && (bool)_coreGameManager.CarOutInControl.CarToEnter && _coreGameManager.CarOutInControl.CarToEnter.TryGetComponent<CarAnimationsController>(out var component) && !component.IsAnimatingTrunk)
		{
			bool isOpen = !component.OpenDoors[CarDoorType.Trunk];
			component.OpenCloseTrunk(isOpen, sendData: true).Forget();
		}
	}

	public void DoorButton(bool justOpenTheDoor = false)
	{
		V.DriverPerson.GetComponent<NewMoveController>().StopMove();
		if (!_playerComponent)
		{
			V.DriverPerson.TryGetComponent<PlayerComponent>(out _playerComponent);
		}
		if (!_mainPersonDriverPerson)
		{
			_mainPersonDriverPerson = V.DriverPerson.GetComponent<MainPerson>();
		}
		PersonAnimationsController personAnimationsController = (_mainPersonDriverPerson ? _mainPersonDriverPerson.PersonAnimationsController : null);
		if (FlagsStorage.OutOfCar)
		{
			if (!_coreGameManager.CarOutInControl.CarToEnter)
			{
				return;
			}
			if (_coreGameManager.CarOutInControl.CarToEnter.TryGetComponent<VehicleEntityProvider>(out var component) && !component.IsMultiplayerCar)
			{
				int2 notValid = Utils.NotValid;
				component.CheckerEngineCheating(out var res);
				if (math.all(res == notValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CheatEngine, 3f, NotificationType.Error);
					return;
				}
				component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
				if (math.all(isIncompatible == Utils.NotValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
					return;
				}
				if (_coreGameManager.CarOutInControl.CarToEnter.TryGetComponent<VehicleData>(out var component2))
				{
					int2 result;
					if (!component2.isGarageCar)
					{
						result = Utils.Valid;
					}
					else
					{
						_allCarsInfo.ResolveCarMeta(component2.CarId, out result);
					}
					if (math.all(result == Utils.NotValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.NfsCar, 3f, NotificationType.Error);
						return;
					}
				}
				if (CheckCarInService(_coreGameManager.CarOutInControl.CarToEnter))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
					return;
				}
			}
			if (justOpenTheDoor)
			{
				personAnimationsController.SitGetOutCarSystem.OpenCloseDoor(_coreGameManager.CarOutInControl.CarToEnter, isDriverDoor: true);
			}
			else
			{
				personAnimationsController.SitGetOutCarSystem.SitIntoCarDriving(_coreGameManager.CarOutInControl.CarToEnter);
			}
			if (_playerComponent != null)
			{
				_playerComponent.enabled = false;
			}
		}
		else
		{
			if (!V.Car)
			{
				return;
			}
			if (!CheckCarOccupied(V.Car.transform, personAnimationsController))
			{
				if (justOpenTheDoor)
				{
					personAnimationsController.SitGetOutCarSystem.OpenCloseDoor(V.Car, isDriverDoor: true);
				}
				else
				{
					personAnimationsController.SitGetOutCarSystem.GetOutCar(V.Car);
				}
				if (_playerComponent != null)
				{
					_playerComponent.enabled = true;
				}
			}
			else
			{
				V.CoreGameManager.SetMessage(new LocalizedString("CarBlock.TheDoorIsBlocked"), 2f);
			}
		}
	}

	private bool CheckCarOccupied(Transform car, PersonAnimationsController personAnimationsController, int placeOverride = -1)
	{
		int num = ((placeOverride >= 0) ? placeOverride : ((!FlagsStorage.OutOfCar) ? ((personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace != -1) ? personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace : 0) : personAnimationsController.SitGetOutCarSystem.GetPlaceForPassenger(car.gameObject)));
		CarDoorType carDoorType = (CarDoorType)num;
		VehicleData component = car.GetComponent<VehicleData>();
		float num2 = ((((carDoorType == CarDoorType.DriverDoor || carDoorType == CarDoorType.PassengerBackDoor1) && component.RightHanded) || ((carDoorType == CarDoorType.PassengerFrontDoor || carDoorType == CarDoorType.PassengerBackDoor2) && !component.RightHanded)) ? 1f : (-1f));
		if (car.TryGetComponent<CarAnimationsController>(out var component2) && (bool)component2.CarAnimationReferences && num < component2.CarAnimationReferences.CountSeats && (bool)component2.CarAnimationsPoints && component2.CarAnimationsPoints.SeatPointsDictionary != null && component2.CarAnimationsPoints.SeatPointsDictionary.TryGetValue(carDoorType, out var value) && (bool)value)
		{
			Vector3 vector = value.position + car.up * 0.35f + value.right * (0.5f * num2);
			Vector3 direction = value.right * num2;
			bool flag = TryFindDoorObstacle(vector, direction, out var obstacle, out var walkableSkipped);
			LogDoorRaycast(car, carDoorType, value, vector, direction, obstacle, flag, walkableSkipped);
			return flag;
		}
		Vector3 vector2 = car.right * (2f * num2);
		vector2 += car.up * 0.8f;
		LogDoorFallback(car, num, carDoorType, car.position + vector2);
		return Physics.CheckSphere(car.position + vector2, 0.4f, _layerMask, QueryTriggerInteraction.Ignore);
	}

	private bool TryFindDoorObstacle(Vector3 from, Vector3 direction, out RaycastHit obstacle, out int walkableSkipped)
	{
		obstacle = default;
		walkableSkipped = 0;
		float num = float.MaxValue;
		int num2 = Physics.RaycastNonAlloc(from, direction, _doorRayHits, 1f, ~(int)_ignoreObstacleMask, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num2; i++)
		{
			RaycastHit raycastHit = _doorRayHits[i];
			if (raycastHit.normal.y > 0.64f)
			{
				walkableSkipped++;
			}
			else if (raycastHit.distance < num)
			{
				num = raycastHit.distance;
				obstacle = raycastHit;
			}
		}
		return obstacle.collider;
	}

	private void LogDoorRaycast(Transform car, CarDoorType doorType, Transform seatPoint, Vector3 from, Vector3 direction, RaycastHit hit, bool blocked, int walkableSkipped)
	{
		Debug.DrawRay(from, direction * 1f, blocked ? Color.red : Color.green, 5f);
		string text = (Physics.Raycast(from, Vector3.down, out var hitInfo, 3f, ~(int)_ignoreObstacleMask, QueryTriggerInteraction.Ignore) ? $"{hitInfo.distance:F2}m above {hitInfo.collider.name}" : "no ground within 3m");
		float num = Vector3.SignedAngle(Vector3.up, car.up, car.forward);
		_ = $"door={doorType} car={car.name} seatPt={seatPoint.name} from={from:F2} dir={direction:F2} " + $"rayHeight={text} carRoll={num:F1} walkableSkipped={walkableSkipped} frame={Time.frameCount}";
		if ((bool)hit.collider)
		{
			Collider collider = hit.collider;
			collider.transform.IsChildOf(car);
			if (!collider.attachedRigidbody)
			{
				_ = collider.transform.root;
			}
			else
			{
				_ = collider.attachedRigidbody.transform;
			}
		}
	}

	private void LogDoorFallback(Transform car, int place, CarDoorType doorType, Vector3 center)
	{
		car.TryGetComponent<CarAnimationsController>(out var component);
		CarAnimationsPoints carAnimationsPoints = (component ? component.CarAnimationsPoints : null);
		Transform value = null;
		bool flag = (bool)carAnimationsPoints && carAnimationsPoints.SeatPointsDictionary != null && carAnimationsPoints.SeatPointsDictionary.TryGetValue(doorType, out value) && (bool)value;
		if ((bool)component && (bool)component.CarAnimationReferences)
		{
			if (place < component.CarAnimationReferences.CountSeats)
			{
				if ((bool)carAnimationsPoints && carAnimationsPoints.SeatPointsDictionary != null && !flag)
				{
					_ = $"no seat point for {doorType} (SeatPoints count={carAnimationsPoints.SeatPoints?.Count})";
				}
			}
			else
			{
				_ = $"place {place} >= CountSeats {component.CarAnimationReferences.CountSeats}";
			}
		}
		float num = float.MaxValue;
		RaycastHit[] array = Physics.RaycastAll(car.position + Vector3.up, Vector3.down, 3f, ~(int)_ignoreObstacleMask, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (!raycastHit.collider.transform.IsChildOf(car) && !(raycastHit.distance >= num))
			{
				num = raycastHit.distance;
				_ = $"{raycastHit.distance - 1f:F2}m above {raycastHit.collider.name}";
			}
		}
		int num2 = Physics.OverlapSphereNonAlloc(center, 0.4f, _doorDebugOverlaps, _layerMask, QueryTriggerInteraction.Ignore);
		for (int j = 0; j < num2; j++)
		{
			_ = _doorDebugOverlaps[j];
		}
	}

	private static string GetDoorDebugPath(Transform t)
	{
		string text = t.name;
		Transform parent = t.parent;
		while ((bool)parent)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	private bool CheckPassengerDoorToggleBlocked(GameObject car, PersonAnimationsController personAnimationsController)
	{
		int doorPlaceForPassenger = personAnimationsController.SitGetOutCarSystem.GetDoorPlaceForPassenger(car);
		if (doorPlaceForPassenger < 0)
		{
			return false;
		}
		bool value = default;
		if ((car.TryGetComponent<CarAnimationsController>(out var component) && component.OpenDoors != null && component.OpenDoors.TryGetValue((CarDoorType)doorPlaceForPassenger, out value)) & value)
		{
			return false;
		}
		return CheckCarOccupied(car.transform, personAnimationsController, doorPlaceForPassenger);
	}

	private bool CheckCarInService(GameObject car)
	{
		if (!car.TryGetComponent<VehicleData>(out var component) || !component.isGarageCar)
		{
			return false;
		}
		ES3File file;
		return EasySaveDataHandler.TryGetFile("TuningTimers/" + component.slotId, out file);
	}

	public void DoorButtonPassanger(bool justOpenTheDoor = false)
	{
		if (!_mainPersonDriverPerson)
		{
			_mainPersonDriverPerson = V.DriverPerson.GetComponent<MainPerson>();
		}
		PersonAnimationsController personAnimationsController = (_mainPersonDriverPerson ? _mainPersonDriverPerson.PersonAnimationsController : null);
		GameObject gameObject = (FlagsStorage.OutOfCar ? _coreGameManager.CarOutInControl.CarToEnter : V.Car);
		if (!gameObject || !personAnimationsController)
		{
			return;
		}
		if ((FlagsStorage.OutOfCar & justOpenTheDoor) ? CheckPassengerDoorToggleBlocked(gameObject, personAnimationsController) : CheckCarOccupied(gameObject.transform, personAnimationsController))
		{
			V.CoreGameManager.SetMessage(new LocalizedString("CarBlock.TheDoorIsBlocked"), 2f);
		}
		else if (FlagsStorage.OutOfCar)
		{
			if (justOpenTheDoor)
			{
				personAnimationsController.SitGetOutCarSystem.OpenCloseDoor(_coreGameManager.CarOutInControl.CarToEnter, isDriverDoor: false);
			}
			else
			{
				personAnimationsController.SitGetOutCarSystem.SitIntoCarPassenger(_coreGameManager.CarOutInControl.CarToEnter);
			}
		}
		else if ((bool)V.Car)
		{
			if (justOpenTheDoor)
			{
				personAnimationsController.SitGetOutCarSystem.OpenCloseDoor(V.Car, isDriverDoor: false);
			}
			else
			{
				personAnimationsController.SitGetOutCarSystem.GetOutCar(V.Car);
			}
		}
	}

	private void OpenCloseSideDoor()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<CarAnimationsController>(out var carAnimationsController) && !carAnimationsController.IsMoveSideDoor)
		{
			bool rightHanded = carAnimationsController.CarAnimationReferences.RightHanded;
			CarDoorType carDoorType = (rightHanded ? CarDoorType.PassengerBackDoor1 : CarDoorType.PassengerBackDoor2);
			carAnimationsController.OpenCloseCarElement(carDoorType, !carAnimationsController.OpenDoors[carDoorType], isSendData: true, isSideDoor: true);
			carAnimationsController.SetMoveSideDoor(isMoving: true);
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				carAnimationsController.SetMoveSideDoor(isMoving: false);
				SideDoorImage.sprite = (carAnimationsController.OpenDoors[carDoorType] ? SideDoorImageClose : SideDoorImageOpen);
			}).AddTo(carAnimationsController);
		}
	}

	public void OpenCloseSideDoor(Image image)
	{
		if ((bool)V.Car && V.Car.TryGetComponent<CarAnimationsController>(out var carAnimationsController) && !carAnimationsController.IsMoveSideDoor)
		{
			bool rightHanded = carAnimationsController.CarAnimationReferences.RightHanded;
			CarDoorType carDoorType = (rightHanded ? CarDoorType.PassengerBackDoor1 : CarDoorType.PassengerBackDoor2);
			carAnimationsController.OpenCloseCarElement(carDoorType, !carAnimationsController.OpenDoors[carDoorType], isSendData: true, isSideDoor: true);
			carAnimationsController.SetMoveSideDoor(isMoving: true);
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				carAnimationsController.SetMoveSideDoor(isMoving: false);
				image.sprite = (carAnimationsController.OpenDoors[carDoorType] ? SideDoorImageClose : SideDoorImageOpen);
			}).AddTo(carAnimationsController);
		}
	}

	public void HoodButtonPressed()
	{
		if (!(_coreGameManager.CarOutInControl.CarToEnter == null) && (!(_coreGameManager.CarOutInControl.CarToEnter != _freeDriveProvider.FirstCarMultiPlayer) || FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu) && (bool)_coreGameManager.CarOutInControl.CarToEnter && _coreGameManager.CarOutInControl.CarToEnter.TryGetComponent<CarAnimationsController>(out var component) && !component.IsAnimatingHood)
		{
			bool isOpen = !component.OpenDoors[CarDoorType.Hood];
			component.OpenCloseCarElement(CarDoorType.Hood, isOpen, isSendData: true);
		}
	}

	public void RoofButtonPressed()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<CarAnimationsController>(out var component) && !component.IsMoveRoof)
		{
			component.OpenCloseRoof(!component.OpenDoors[CarDoorType.Roof], isSendData: true).Forget();
		}
	}

	public void TurnOnOffDoorHood(bool isOn)
	{
		if (isOn)
		{
			if ((bool)DoorHood && !DoorHood.gameObject.activeInHierarchy)
			{
				DoorHood.gameObject.SetActive(value: true);
			}
		}
		else if ((bool)DoorHood && DoorHood.gameObject.activeInHierarchy)
		{
			DoorHood.gameObject.SetActive(value: false);
		}
	}

	public void TurnOnOffDoorTrunk(bool isOn)
	{
		if (isOn)
		{
			if ((bool)DoorTrunk && !DoorTrunk.gameObject.activeInHierarchy)
			{
				DoorTrunk.gameObject.SetActive(value: true);
			}
		}
		else if ((bool)DoorTrunk && DoorTrunk.gameObject.activeInHierarchy)
		{
			DoorTrunk.gameObject.SetActive(value: false);
		}
	}

	public void TurnOnOffRoofButton(bool isOn)
	{
		if (isOn)
		{
			if ((bool)RoofButton && !RoofButton.gameObject.activeInHierarchy)
			{
				RoofButton.gameObject.SetActive(value: true);
			}
		}
		else if ((bool)RoofButton && RoofButton.gameObject.activeInHierarchy)
		{
			RoofButton.gameObject.SetActive(value: false);
		}
	}

	private void SetActiveDoorButton()
	{
		if (!_coreGameManager.CarOutInControl.CarToEnter)
		{
			return;
		}
		if (_coreGameManager.CarOutInControl.CarToEnter.TryGetComponent<CarAnimationsController>(out var component))
		{
			if (!component.CarAnimationReferences)
			{
				return;
			}
			bool flag = IsAnimatingPlayer();
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < component.CarAnimationsPoints.SeatPointsToSit.Count; i++)
			{
				float sqrMagnitude = (V.DriverPerson.transform.position - component.CarAnimationsPoints.SeatPointsToSit[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num = i;
					num2 = sqrMagnitude;
				}
			}
			if (num2 < 4f && !flag)
			{
				if (num == 0)
				{
					if (!Door.gameObject.activeInHierarchy || ((bool)DoorPassenger && DoorPassenger.gameObject.activeInHierarchy))
					{
						if ((bool)Door)
						{
							Door.SetActive(active: true);
						}
						if ((bool)DoorPassenger)
						{
							DoorPassenger.SetActive(active: false);
						}
						if ((bool)DriverDoor)
						{
							DriverDoor.sprite = DriverDoorIcon;
						}
					}
				}
				else if (Door.gameObject.activeInHierarchy || !DoorPassenger || !DoorPassenger.gameObject.activeInHierarchy)
				{
					if ((bool)Door)
					{
						Door.SetActive(active: false);
					}
					if ((bool)DoorPassenger)
					{
						DoorPassenger.SetActive(active: true);
					}
				}
			}
			else if (Door.gameObject.activeInHierarchy || ((bool)DoorPassenger && DoorPassenger.gameObject.activeInHierarchy))
			{
				if ((bool)Door)
				{
					Door.SetActive(active: false);
				}
				if ((bool)DoorPassenger)
				{
					DoorPassenger.SetActive(active: false);
				}
			}
			if (_coreGameManager.CarOutInControl.CarToEnter == _freeDriveProvider.FirstCarMultiPlayer || FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu)
			{
				TurnOnOffDoorHood(component.CarAnimationReferences.IsHoodAnimate && !flag);
				TurnOnOffDoorTrunk(component.CarAnimationReferences.IsTrunkAnimate && !flag);
			}
		}
		else
		{
			SetActiveDoorButtonFromPersonPoints();
		}
	}

	private static bool IsNearDriverSide(GameObject car, Vector3 playerPosition)
	{
		bool rightHanded = car.TryGetComponent<VehicleData>(out var component) && component.RightHanded;
		return IsDriverSide(car.transform.position, car.transform.right, rightHanded, playerPosition);
	}

	public static bool IsDriverSide(Vector3 carPosition, Vector3 carRight, bool rightHanded, Vector3 playerPosition)
	{
		return Vector3.Dot(playerPosition - carPosition, carRight) >= 0f == rightHanded;
	}

	private bool IsAnimatingPlayer()
	{
		if ((bool)_mainPersonDriverPerson && (bool)_mainPersonDriverPerson.PersonAnimationsController)
		{
			if (!_mainPersonDriverPerson.PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
			{
				return _mainPersonDriverPerson.PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
			}
			return true;
		}
		return false;
	}

	public void ShowDoor()
	{
		if (!Door)
		{
			return;
		}
		PersonAnimationsController personAnimationsController = null;
		CarAnimationReferences carAnimationReferences = null;
		if (!_mainPersonDriverPerson)
		{
			_mainPersonDriverPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
		}
		if ((bool)_mainPersonDriverPerson && (bool)_mainPersonDriverPerson.PersonAnimationsController)
		{
			personAnimationsController = _mainPersonDriverPerson.PersonAnimationsController;
			carAnimationReferences = (personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentCar ? personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentCar.CarAnimationReferences : null);
		}
		if (FlagsStorage.OutOfCar)
		{
			SetActiveDoorButton();
			if ((bool)SideDoorButton && SideDoorButton.gameObject.activeInHierarchy)
			{
				SideDoorButton.SetActive(active: false);
			}
			if ((bool)RoofButton && RoofButton.gameObject.activeInHierarchy)
			{
				TurnOnOffRoofButton(isOn: false);
			}
		}
		else
		{
			if ((bool)personAnimationsController && (personAnimationsController.AnimationService.IsAnimatingPlayer() || personAnimationsController.SitGetOutCarSystem.SitGetOutData.DisableDriverButtonInCar))
			{
				if ((bool)Door)
				{
					Door.SetActive(active: false);
				}
				if ((bool)DoorPassenger)
				{
					DoorPassenger.SetActive(active: false);
				}
				if ((bool)carAnimationReferences && carAnimationReferences.IsOpenCloseSideDoor)
				{
					SideDoorButton?.SetActive(active: false);
				}
				if ((bool)carAnimationReferences && carAnimationReferences.IsOpenCloseRoof)
				{
					TurnOnOffRoofButton(isOn: false);
				}
				return;
			}
			if (!FlagsStorage.IsPassenger)
			{
				DriverDoor.sprite = ExitDoorIcon;
				if ((bool)Door)
				{
					Door.SetActive(active: true);
				}
				if ((bool)carAnimationReferences && carAnimationReferences.IsOpenCloseSideDoor)
				{
					SideDoorButton?.SetActive(!personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate);
				}
				if ((bool)carAnimationReferences && carAnimationReferences.IsOpenCloseRoof)
				{
					TurnOnOffRoofButton(!personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate);
				}
				if ((bool)DoorPassenger)
				{
					DoorPassenger.SetActive(active: false);
				}
			}
			else
			{
				if ((bool)Door)
				{
					Door.SetActive(active: false);
				}
				if ((bool)DoorPassenger)
				{
					DoorPassenger.SetActive(active: true);
				}
			}
			TurnOnOffDoorHood(isOn: false);
			TurnOnOffDoorTrunk(isOn: false);
		}
		showDoor = true;
	}

	public void HideDoor()
	{
		if ((bool)Door)
		{
			Door.SetActive(active: false);
		}
		if ((bool)DoorPassenger)
		{
			DoorPassenger.SetActive(active: false);
		}
		TurnOnOffDoorHood(isOn: false);
		TurnOnOffDoorTrunk(isOn: false);
		TurnOnOffRoofButton(isOn: false);
		showDoor = false;
	}

	private bool CurrentCarHasNoTurnSignals()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
		{
			return CarIdStorage.IsCarWithoutTurnSignals(component.CarId);
		}
		return false;
	}

	public void LeftIndicatorOn()
	{
		if (!FreeDriveControl.IsPassengerWithoutControl() && !CurrentCarHasNoTurnSignals() && (bool)V.Car && !FlagsStorage.OutOfCar)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.IndicatorSwitch, base.gameObject);
			ResetAllTc();
			LightController component = V.Car.GetComponent<LightController>();
			if (component.IndicatorState.Value != TurnSignalState.Left)
			{
				component.TurnOnLeftIndicators();
			}
			else
			{
				component.TurnOffAllIndicators();
			}
			SendStateOther();
		}
	}

	public void RightIndicatorOn()
	{
		if (!FreeDriveControl.IsPassengerWithoutControl() && !CurrentCarHasNoTurnSignals() && (bool)V.Car && !FlagsStorage.OutOfCar)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.IndicatorSwitch, base.gameObject);
			ResetAllTc();
			LightController component = V.Car.GetComponent<LightController>();
			if (component.IndicatorState.Value != TurnSignalState.Right)
			{
				component.TurnOnRightIndicators();
			}
			else
			{
				component.TurnOffAllIndicators();
			}
			SendStateOther();
		}
	}

	public void TurnOnEmergency()
	{
		if (!FreeDriveControl.IsPassengerWithoutControl() && !CurrentCarHasNoTurnSignals() && (bool)V.Car && !FlagsStorage.OutOfCar)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.IndicatorSwitch, base.gameObject);
			ResetAllTc();
			LightController component = V.Car.GetComponent<LightController>();
			if (component.IndicatorState.Value != TurnSignalState.Hazard)
			{
				component.EmergencyLightsOn();
			}
			else
			{
				component.TurnOffAllIndicators();
			}
			SendStateOther();
		}
	}

	public void DisableAllLights()
	{
		if ((bool)V.Car || FlagsStorage.OutOfCar)
		{
			LightController component = V.Car.GetComponent<LightController>();
			component.TurnOffAllIndicators();
			component.turnOffHeadLights();
			component.HState = 0;
			UpdateHeadLampIconUI(LightsType.Disabled);
			if (V.Car.TryGetComponent<VehicleData>(out var component2))
			{
				component2.PoliceCarController.PoliceCarSirenControllerInGameplay.StopSiren();
				_menuLevelParent.makePoliceButton.GetComponent<PoliceSirenStatusUpdateButton>().UpdateIcon();
			}
			SendStateOther();
		}
	}

	private void ResetAllTc()
	{
		_hudColorChanger.Value?.SetCurrentColor(_menuLevelParent.LeftIndicatorArrowTC);
		_hudColorChanger.Value?.SetCurrentColor(_menuLevelParent.RightIndicatorArrowTC);
		_hudColorChanger.Value?.SetCurrentColor(_menuLevelParent.EmergencyTC);
	}

	public void TurnOnLights()
	{
		if (!FreeDriveControl.IsPassengerWithoutControl() && (bool)V.Car && !FlagsStorage.OutOfCar && V.Car.TryGetComponent<LightController>(out var component))
		{
			switch (component.HState)
			{
			case 0:
				component.turnOnNearLights();
				component.HState = 1;
				UpdateHeadLampIconUI(LightsType.Near);
				break;
			case 1:
				component.turnOnFarLights();
				component.HState = 2;
				UpdateHeadLampIconUI(LightsType.Far);
				break;
			default:
				component.turnOffHeadLights();
				component.HState = 0;
				UpdateHeadLampIconUI(LightsType.Disabled);
				break;
			}
			SendStateOther();
		}
	}

	private void UpdateHeadLampIconUI(LightsType type)
	{
		if ((bool)_menuLevelParent && (bool)_menuLevelParent.HeadLampIconUI)
		{
			switch (type)
			{
			case LightsType.Near:
				_menuLevelParent.HeadLampIconUI.sprite = _menuLevelParent.nearHeadLampTexture;
				break;
			case LightsType.Far:
				_menuLevelParent.HeadLampIconUI.sprite = _menuLevelParent.farHeadLampTexture;
				break;
			default:
				_menuLevelParent.HeadLampIconUI.sprite = _menuLevelParent.noHeadLightTexture;
				break;
			}
		}
	}

	public void UpdateHeadlightIcon(Image icon)
	{
		if ((bool)icon)
		{
			int num = 0;
			if ((bool)V.Car && V.Car.TryGetComponent<LightController>(out var component))
			{
				num = component.HState;
			}
			switch ((LightsType)num)
			{
			case LightsType.Near:
				icon.sprite = _menuLevelParent.nearHeadLampTexture;
				break;
			case LightsType.Far:
				icon.sprite = _menuLevelParent.farHeadLampTexture;
				break;
			default:
				icon.sprite = _menuLevelParent.noHeadLightTexture;
				break;
			}
		}
	}

	public void onFlashReleased()
	{
		if (FreeDriveControl.IsPassengerWithoutControl() || !V.Car || FlagsStorage.OutOfCar || !V.Car.TryGetComponent<LightController>(out var component))
		{
			return;
		}
		switch (oldHState)
		{
		case 0:
			component.turnOffHeadLights();
			UpdateHeadLampIconUI(LightsType.Disabled);
			component.HState = 0;
			break;
		case 1:
			component.turnOnNearLights();
			component.HState = 1;
			UpdateHeadLampIconUI(LightsType.Near);
			break;
		case 2:
			component.turnOnFarLights();
			component.HState = 2;
			UpdateHeadLampIconUI(LightsType.Far);
			break;
		default:
			if (component.HState == 3)
			{
				component.turnOffHeadLights();
				component.HState = 0;
				UpdateHeadLampIconUI(LightsType.Disabled);
			}
			break;
		}
		SendStateOther();
	}

	public void OnTurnLightsAutoTurnOff()
	{
		ResetAllTc();
		SendStateOther();
	}

	public void SendStateOther()
	{
		if (FlagsStorage.IsMultiFree && (bool)V.Car && !FlagsStorage.InGarageFree && V.Car.TryGetComponent<LightController>(out var component) && (bool)_multiPlayer && V.Car.TryGetComponent<PhotonView>(out var component2))
		{
			string text = ((int)component.IndicatorState.Value).ToString() + component.HState;
			_multiPlayer.SendValueStr(41, component2.ViewID.ToString(), text);
		}
	}

	public void SendStateOtherTo(int actorNumber)
	{
		if (FlagsStorage.IsMultiFree && (bool)V.Car && !FlagsStorage.InGarageFree && V.Car.TryGetComponent<LightController>(out var component) && (bool)_multiPlayer && V.Car.TryGetComponent<PhotonView>(out var component2))
		{
			string text = ((int)component.IndicatorState.Value).ToString() + component.HState;
			_multiPlayer.SendValueStrToPlayer(41, component2.ViewID.ToString(), text, actorNumber);
		}
	}

	public void KeepRow()
	{
		if (!V.Car)
		{
			return;
		}
		if (FlagsStorage.RacingGameMode)
		{
			_coreGameManager.SetMessage(new LocalizedString("QuickCarSettings.KeepLaneAssistantUnavailable"), 3f);
			_freeDriveProvider.OnKeepRowStateChanged?.Invoke(obj: false);
			return;
		}
		FlagsStorage.KeepRow = !FlagsStorage.KeepRow;
		V.Car.GetComponent<VehicleData>();
		if (FlagsStorage.KeepRow)
		{
			if (_carInputManager.KeepRowComposition.SetClosestRow())
			{
				_freeDriveProvider.OnKeepRowStateChanged?.Invoke(obj: true);
				return;
			}
			_coreGameManager.SetMessage(new LocalizedString("QuickCarSettings.KeepLaneAssistantUnavailable"), 3f);
			_freeDriveProvider.OnKeepRowStateChanged?.Invoke(obj: false);
			FlagsStorage.KeepRow = false;
		}
		else
		{
			_freeDriveProvider.OnKeepRowStateChanged?.Invoke(obj: false);
		}
	}

	public void DisableKeepLane(bool message)
	{
		if (message && (bool)V.Car && FlagsStorage.KeepRow)
		{
			_coreGameManager.SetMessage(new LocalizedString("QuickCarSettings.KeepLaneAssistantDisabled"), 3f);
		}
		FlagsStorage.KeepRow = false;
		_freeDriveProvider.OnKeepRowStateChanged?.Invoke(obj: false);
	}

	public void FlashHeadLights()
	{
		if (!FreeDriveControl.IsPassengerWithoutControl() && (bool)V.Car && !FlagsStorage.OutOfCar && V.Car.TryGetComponent<LightController>(out var component))
		{
			UpdateHeadLampIconUI(LightsType.Far);
			component.turnOnFarLights();
			oldHState = component.HState;
			component.HState = 3;
			SendStateOther();
		}
	}

	public void UpdateLightButtonView()
	{
		if ((bool)V.Car && !FlagsStorage.OutOfCar && V.Car.TryGetComponent<LightController>(out var component))
		{
			switch (component.HState)
			{
			case 0:
				UpdateHeadLampIconUI(LightsType.Disabled);
				break;
			case 1:
				UpdateHeadLampIconUI(LightsType.Near);
				break;
			default:
				UpdateHeadLampIconUI(LightsType.Far);
				break;
			}
		}
	}

	public void TurnOnSiren()
	{
		if ((bool)V.Car && V.Car.TryGetComponent<LightController>(out var component))
		{
			VehicleData component2 = V.Car.GetComponent<VehicleData>();
			if (component2.LightsComposition.IsPolice)
			{
				bool flag = component2.LightsComposition.PoliceLights.ToggleSirenByButton();
				component.HState = (flag ? 4 : 5);
				SendStateOther();
			}
		}
	}

	private static int GetOnFootSurfacesMask()
	{
		return LayerMask.GetMask("Road", "Terrain", "Tunnel", "Default", "Garage");
	}

	public void ChangeCarLightMask(GameObject car, int i)
	{
		if (!car)
		{
			Debug.LogError("No car.");
		}
		else
		{
			if (!car.TryGetComponent<LightController>(out var component))
			{
				return;
			}
			if (ScenesHelper.IsParkingLevelActive())
			{
				if (component.HLLeftLight != null)
				{
					component.HLLeftLight.GetComponent<Light>().cullingMask = -1;
				}
				if (component.HLRightLight != null)
				{
					component.HLRightLight.GetComponent<Light>().cullingMask = -1;
				}
			}
			else if (i == 0)
			{
				if (component.HLLeftLight != null)
				{
					component.HLLeftLight.GetComponent<Light>().cullingMask = -1;
				}
				if (component.HLRightLight != null)
				{
					component.HLRightLight.GetComponent<Light>().cullingMask = -1;
				}
			}
			else
			{
				if (component.HLLeftLight != null)
				{
					component.HLLeftLight.GetComponent<Light>().cullingMask = GetOnFootSurfacesMask();
				}
				if (component.HLRightLight != null)
				{
					component.HLRightLight.GetComponent<Light>().cullingMask = GetOnFootSurfacesMask();
				}
			}
		}
	}

	public void ShowResetTrailer(RaycastHit hit)
	{
		ClientController component = hit.transform.GetComponent<ClientController>();
		if ((bool)component && !(component.Owner != V.DriverPerson) && (!PhotonNetwork.IsConnected || component.photonView.IsMine))
		{
			_menuLevelParent.serviceLabels.resetTrailer.gameObject.SetActive(value: true);
			_menuLevelParent.serviceLabels.resetTrailer.onClick.AddListener(component.ResetRotation);
		}
	}

	public void HideResetTrailer()
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			_menuLevelParent.serviceLabels.resetTrailer.gameObject.SetActive(value: false);
			_menuLevelParent.serviceLabels.resetTrailer.onClick.RemoveAllListeners();
		}
	}

	public void ShowHomeDoorButton(bool entering, bool isGarageDoor)
	{
		if (isGarageDoor)
		{
			GarageDoorButton.SetActive(entering);
		}
		else
		{
			HomeDoorButton.SetActive(entering && FlagsStorage.OutOfCar);
		}
	}

	public bool GetSideDoorButtonActiveState()
	{
		if ((bool)SideDoorButton)
		{
			return SideDoorButton.gameObject.activeSelf;
		}
		return false;
	}

	public bool GetCarRoofButtonActiveState()
	{
		if ((bool)RoofButton)
		{
			return RoofButton.gameObject.activeSelf;
		}
		return false;
	}

	public bool GetCarHoodButtonActiveState()
	{
		if ((bool)DoorHood)
		{
			return DoorHood.gameObject.activeSelf;
		}
		return false;
	}

	public bool GetCarTrunkButtonActiveState()
	{
		if ((bool)DoorTrunk)
		{
			return DoorTrunk.gameObject.activeSelf;
		}
		return false;
	}

	public bool GetCarHoodInteractableLive()
	{
		if (HasInteractableDoorCar(out var carAnimationsController))
		{
			return carAnimationsController.CarAnimationReferences.IsHoodAnimate;
		}
		return false;
	}

	public bool GetCarTrunkInteractableLive()
	{
		if (HasInteractableDoorCar(out var carAnimationsController))
		{
			return carAnimationsController.CarAnimationReferences.IsTrunkAnimate;
		}
		return false;
	}

	private bool HasInteractableDoorCar(out CarAnimationsController carAnimationsController)
	{
		carAnimationsController = null;
		GameObject carToEnter = _coreGameManager.CarOutInControl.CarToEnter;
		if ((bool)carToEnter && (carToEnter == _freeDriveProvider.FirstCarMultiPlayer || FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu) && carToEnter.TryGetComponent<CarAnimationsController>(out carAnimationsController))
		{
			return carAnimationsController.CarAnimationReferences;
		}
		return false;
	}

	private void SetActiveDoorButtonFromPersonPoints()
	{
		GameObject carToEnter = _coreGameManager.CarOutInControl.CarToEnter;
		AllPersonPoints allPersonPoints = (((bool)carToEnter && carToEnter.TryGetComponent<VehicleData>(out var component)) ? component.allPersonPoints : null);
		if (!allPersonPoints || allPersonPoints.allTargets == null || allPersonPoints.allTargets.Length == 0)
		{
			if ((bool)Door)
			{
				Door.SetActive(active: true);
			}
			if ((bool)DoorPassenger)
			{
				DoorPassenger.SetActive(active: true);
			}
			if ((bool)DriverDoor)
			{
				DriverDoor.sprite = DriverDoorIcon;
			}
			return;
		}
		Transform transform = allPersonPoints.transform;
		Vector3 position = V.DriverPerson.transform.position;
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < allPersonPoints.allTargets.Length; i++)
		{
			if (allPersonPoints.allTargets[i] != null)
			{
				Vector3 vector = transform.TransformPoint(allPersonPoints.allTargets[i].ParentPosion);
				float sqrMagnitude = (position - vector).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num = i;
					num2 = sqrMagnitude;
				}
			}
		}
		if (num < 0 || !(num2 < 4f) || IsAnimatingPlayer())
		{
			if ((bool)Door)
			{
				Door.SetActive(active: false);
			}
			if ((bool)DoorPassenger)
			{
				DoorPassenger.SetActive(active: false);
			}
		}
		else if (num == 0)
		{
			if ((bool)Door)
			{
				Door.SetActive(active: true);
			}
			if ((bool)DoorPassenger)
			{
				DoorPassenger.SetActive(active: false);
			}
			if ((bool)DriverDoor)
			{
				DriverDoor.sprite = DriverDoorIcon;
			}
		}
		else
		{
			if ((bool)Door)
			{
				Door.SetActive(active: false);
			}
			if ((bool)DoorPassenger)
			{
				DoorPassenger.SetActive(active: true);
			}
		}
	}

	private void ToggleHorn(bool isStarted)
	{
		if (isStarted)
		{
			FlashHeadLights();
		}
		else
		{
			onFlashReleased();
		}
	}

	private void OnDoorStartCancel(bool isStart)
	{
		if (isStart)
		{
			if (HasActiveDoorButton())
			{
				_doorPerformed = false;
				_doorAborted = false;
				_doorPressFrame = Time.frameCount;
				_doorPressStart = Time.time;
				_doorCts?.Cancel();
				_doorCts = new CancellationTokenSource();
				TrackDoorFillAsync(_doorCts.Token).Forget();
			}
			return;
		}
		_doorCts?.Cancel();
		ResetDoorFillUI();
		if (_doorAborted)
		{
			_doorAborted = false;
		}
		else
		{
			if (_doorPerformed)
			{
				return;
			}
			if ((bool)Door && Door.gameObject.activeInHierarchy)
			{
				DoorButton();
				if (IsAnimatingPlayer())
				{
					BlockInputForAnimation();
				}
			}
			else if (!FlagsStorage.IsLevelScene && (bool)DoorPassenger && DoorPassenger.gameObject.activeInHierarchy)
			{
				DoorButtonPassanger();
				if (IsAnimatingPlayer())
				{
					BlockInputForAnimation();
				}
			}
		}
	}

	private float GetCarSpeedForLog()
	{
		if (!V.Car || !V.Car.TryGetComponent<Rigidbody>(out var component))
		{
			return 0f;
		}
		return component.velocity.magnitude;
	}

	private async UniTaskVoid TrackDoorFillAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			if (!HasActiveDoorButton())
			{
				ResetDoorFillUI();
				break;
			}
			float num = Time.time - _doorPressStart;
			if (num >= 0.2f)
			{
				if ((bool)DoorImage)
				{
					DoorImage.gameObject.SetActive(value: true);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.gameObject.SetActive(value: true);
				}
				if ((bool)DoorFillAmountImage)
				{
					DoorFillAmountImage.fillAmount = Mathf.Clamp01((num - 0.2f) / 1f);
				}
			}
			if (num >= 1.2f)
			{
				_doorPerformed = true;
				await UniTask.Yield();
				ResetDoorFillUI();
				if ((bool)DoorPassenger && DoorPassenger.gameObject.activeInHierarchy)
				{
					DoorButtonPassanger(justOpenTheDoor: true);
				}
				else if ((bool)Door && Door.gameObject.activeInHierarchy)
				{
					DoorButton(justOpenTheDoor: true);
				}
				break;
			}
			await UniTask.Yield(ct);
		}
	}

	private bool HasActiveDoorButton()
	{
		if (!Door || !Door.gameObject.activeInHierarchy)
		{
			if ((bool)DoorPassenger)
			{
				return DoorPassenger.gameObject.activeInHierarchy;
			}
			return false;
		}
		return true;
	}

	private void ResetDoorFillUI()
	{
		if ((bool)DoorImage)
		{
			DoorImage.gameObject.SetActive(value: false);
		}
		if ((bool)DoorFillAmountImage)
		{
			DoorFillAmountImage.fillAmount = 0f;
		}
		if ((bool)DoorFillAmountImage)
		{
			DoorFillAmountImage.gameObject.SetActive(value: false);
		}
	}

	private void BlockInputForAnimation()
	{
		if (!_inputBlockedForAnimation)
		{
			_inputBlockedForAnimation = true;
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: false);
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OnAnimationBlockDone));
			CommonExtensions.CancelAndDisposeToken(ref _inputBlockCts);
			_inputBlockCts = new CancellationTokenSource();
			ReleaseInputBlockWhenAnimationEndsAsync(_inputBlockCts.Token).Forget();
		}
	}

	private void OnAnimationBlockDone(bool _)
	{
		ReleaseInputBlockForAnimation("animation done");
	}

	private async UniTaskVoid ReleaseInputBlockWhenAnimationEndsAsync(CancellationToken ct)
	{
		float elapsed = 0f;
		while (elapsed < 13f)
		{
			await UniTask.Yield(PlayerLoopTiming.Update, ct);
			elapsed += Time.unscaledDeltaTime;
			if (!IsAnimatingPlayer())
			{
				ReleaseInputBlockForAnimation("animation ended");
				return;
			}
		}
		ReleaseInputBlockForAnimation("timeout");
	}

	private void ReleaseInputBlockForAnimation(string reason)
	{
		if (_inputBlockedForAnimation)
		{
			_inputBlockedForAnimation = false;
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OnAnimationBlockDone));
			CommonExtensions.CancelAndDisposeToken(ref _inputBlockCts);
			_setActionsActiveHandle.Get(_eventRegistry)?.Raise(value: true);
		}
	}

	private bool IsAutomaticShiftDisabled()
	{
		if (FlagsStorage.IsInputEnabledInThisWindow && !FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger && _powerTrain.IsAutomaticGearbox && (!_menuLevelParent || !_menuLevelParent.isDroneEnabled))
		{
			if (_freeDriveProvider != null && (bool)_freeDriveProvider.MapCamera)
			{
				return _freeDriveProvider.MapCamera.ShowingBigMap;
			}
			return false;
		}
		return true;
	}

	private void OnAutoShiftUp()
	{
		if (!IsAutomaticShiftDisabled())
		{
			int num = ((!_powerTrain.Automatic) ? 6 : Math.Max(1, _currentGear - 1));
			if (num != _currentGear)
			{
				ChangeGearMove(num);
			}
		}
	}

	private void OnAutoShiftDown()
	{
		if (!IsAutomaticShiftDisabled())
		{
			int num = ((!_powerTrain.Automatic) ? 7 : Math.Min(4, _currentGear + 1));
			if (num != _currentGear)
			{
				ChangeGearMove(num);
			}
		}
	}

	private void OnAutoShiftM()
	{
		if (!IsAutomaticShiftDisabled() && _powerTrain.Automatic && _currentGear == 4)
		{
			int moveToGear = 5;
			ChangeGearMove(moveToGear);
		}
	}

	private void OnAutoShiftPRND()
	{
		if (!IsAutomaticShiftDisabled() && !_powerTrain.Automatic && _currentGear == 5)
		{
			int moveToGear = 4;
			ChangeGearMove(moveToGear);
		}
	}

	private void ShiftUpDown(bool shiftUp)
	{
		TrySetPowerTrain();
		if (shiftUp)
		{
			OnAutoShiftUp();
		}
		else
		{
			OnAutoShiftDown();
		}
	}

	private void ToggleShiftMA()
	{
		TrySetPowerTrain();
		if ((bool)_powerTrain)
		{
			ShiftMA(_powerTrain.Automatic);
		}
	}

	private void ShiftMA(bool shiftM)
	{
		TrySetPowerTrain();
		if (shiftM)
		{
			OnAutoShiftM();
		}
		else
		{
			OnAutoShiftPRND();
		}
	}

	private void OnEnable()
	{
		UnsubscribeToActions();
		SubscribeToActions();
	}

	private void SubscribeToActions()
	{
		if (_eventRegistry != null)
		{
			_inputMapSetEvent.Subscribe(CancelPendingActions);
			_doorStartCancelHandle.Get(_eventRegistry).Subscribe(OnDoorStartCancel);
			_onHornHandle.Get(_eventRegistry).Subscribe(ToggleHorn);
			_onHeadlightsHandle.Get(_eventRegistry).Subscribe(TurnOnLights);
			_toggleLeftSignalHandle.Get(_eventRegistry).Subscribe(LeftIndicatorOn);
			_toggleRightSignalHandle.Get(_eventRegistry).Subscribe(RightIndicatorOn);
			_toggleEmergencySignalHandle.Get(_eventRegistry).Subscribe(TurnOnEmergency);
		}
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(TrySetPowerTrain));
	}

	private void CancelPendingActions(InputGroupsEnum _)
	{
		if (Time.frameCount != _doorPressFrame)
		{
			_doorAborted = true;
			CommonExtensions.CancelAndDisposeToken(ref _doorCts);
			_doorPerformed = false;
			ResetDoorFillUI();
			ReleaseInputBlockForAnimation("group switch");
		}
	}

	private void TrySetPowerTrain(bool isIn = true)
	{
		if (!isIn)
		{
			_powerTrain = null;
		}
		else if (!_carInputManager.GetPowertrain || !(_powerTrain == _carInputManager.GetPowertrain))
		{
			_powerTrain = _carInputManager.GetPowertrain;
			_currentGear = 4;
			ChangeGearMove(_currentGear);
		}
	}

	private void OnDisable()
	{
		UnsubscribeToActions();
	}

	private void UnsubscribeToActions()
	{
		if (_eventRegistry != null)
		{
			CommonExtensions.CancelAndDisposeToken(ref _doorCts);
			ReleaseInputBlockForAnimation("teardown");
			_inputMapSetEvent.Unsubscribe(CancelPendingActions);
			_doorStartCancelHandle.Get(_eventRegistry)?.Unsubscribe(OnDoorStartCancel);
			_onHornHandle.Get(_eventRegistry)?.Unsubscribe(ToggleHorn);
			_onHeadlightsHandle.Get(_eventRegistry)?.Unsubscribe(TurnOnLights);
			_toggleLeftSignalHandle.Get(_eventRegistry)?.Unsubscribe(LeftIndicatorOn);
			_toggleRightSignalHandle.Get(_eventRegistry)?.Unsubscribe(RightIndicatorOn);
			_toggleEmergencySignalHandle.Get(_eventRegistry)?.Unsubscribe(TurnOnEmergency);
		}
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(TrySetPowerTrain));
	}

	private void ChangeGearMove(int moveToGear)
	{
		SetGearSelf(moveToGear);
		_carInputManager.SelectorState = ((moveToGear == 6 || moveToGear == 7) ? 5 : moveToGear);
		_currentGear = _carInputManager.SelectorState;
	}
}
