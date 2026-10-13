using System;
using System.Threading;
using CPM.Common;
using CPM.Database;
using CPM.PlayerData;
using CPM.Rating.Entities;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem;

public class DragRacingControlMultiplayer : MonoBehaviour
{
	public const float MaxDistance2Win = 250f;

	public int selectedOwn = -1;

	public int selectedGuest = -1;

	public Transform car1StartPosCity;

	public Transform car2StartPosCity;

	public Transform car1StartPosHW;

	public Transform car2StartPosHW;

	public Transform car1StartPosDes;

	public Transform car2StartPosDes;

	public Transform car1StartPosMountain;

	public Transform car2StartPosMountain;

	public Transform car1StartPosMountainBig;

	public Transform car2StartPosMountainBig;

	public Transform car1StartPosRacingPlace;

	public Transform car2StartPosRacingPlace;

	public Transform car1StartPosRacingLoop1;

	public Transform car2StartPosRacingLoop1;

	public Transform car1StartPosRacingLoop2;

	public Transform car2StartPosRacingLoop2;

	public Transform car1StartPosOffroadLoop1;

	public Transform car2StartPosOffroadLoop1;

	public Transform car1StartPosOffroadSprint;

	public Transform car2StartPosOffroadSprint;

	public Transform car1StartPosCity2;

	public Transform car2StartPosCity2;

	public float timer;

	public float TimerOverAll;

	public float carTimerToFinish;

	public float carTimer2ToFinish;

	public float timeTo100;

	public float timeTo200;

	public float timeTo500;

	public float maxSpeed1;

	private string white = "[ffffff]";

	private string green = "[99ff00]";

	private string end_color = "[-]";

	private string red = "[ff0000]";

	private string orange = "[F4FA58]";

	public int AnswerReplay;

	public int AnswerReplayGuest;

	public bool sendValue;

	public int prevCamIndex = -1;

	private CarCamerasController carCamerasController;

	public bool freeControlRacing;

	public ObscuredInt CheckPointOverAll;

	public ObscuredInt checkPointCount;

	public ObscuredInt loopOverAll;

	public ObscuredInt loopCount;

	public GameObject offroadRacingObjects;

	public int[] arr;

	public GameObject racingUpDownButton;

	public PlayerDataStorage playerDataStorage;

	private int _initialCarIgnoreLayerMask;

	public NewRacingMultiplayerController newRacingMultiplayerController;

	public GameObject forChasing;

	public TextMeshProUGUI distanceTextChasing;

	public Image distanceFill;

	public TextMeshProUGUI timeTextChasing;

	public Image timeFill;

	private CurrencyManagerController _currencyManagerController;

	private CoreGameManager _coreGameManager;

	private IPricesService _pricesService;

	private IEarnMoneyService _earnMoneyService;

	private Sort2 _sort2;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private FreeDriveControl _freeDriveControl;

	private InputItemsProvider _controlItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private MenuLevelParent _menuLevelParent;

	private CarInputManager _carInputManager;

	private FreeTimeAttackController _timeAttackController;

	private IDisposable _recordResultDisposable;

	private IDisposable _enableAfterDisposable;

	private InputMapSetEvent _inputMapSetEvent;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _cancelRaceHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CancelRace);

	public int lastReplay;

	[HideInInspector]
	public int lastInput = -1;

	public float MultiRaceCar100Km { get; set; }

	public float MultiRaceCar200Km { get; set; }

	public float MultiRaceCar500Km { get; set; }

	public float MultiRaceCarMaxSpeed { get; set; }

	public string PlayerNameMultiCar2 { get; set; }

	public float TimeMultiRaceCarFinish { get; set; }

	public float MultiRaceOverallRace { get; set; }

	public float MultiRaceWon { get; set; }

	public int SelectedDistance { get; set; }

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, CurrencyManagerController currencyManagerController, IPricesService pricesService, IEarnMoneyService earnMoneyService, Sort2 sort2, CoreGameManager coreGameManager, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveControl freeDriveControl, InputMapSetEvent inputMapSetEvent, EventRegistry eventRegistry, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, MenuLevelParent menuLevelParent, CarInputManager carInputManager, FreeTimeAttackController timeAttackController)
	{
		_timeAttackController = timeAttackController;
		_menuLevelParent = menuLevelParent;
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_inputMapSetEvent = inputMapSetEvent;
		_eventRegistry = eventRegistry;
		_controlItemsProvider = controlItemsProvider;
		_freeDriveControl = freeDriveControl;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_coreGameManager = coreGameManager;
		_sort2 = sort2;
		_earnMoneyService = earnMoneyService;
		_pricesService = pricesService;
		_currencyManagerController = currencyManagerController;
		this.playerDataStorage = playerDataStorage;
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		_freeDriveProvider.DragRacingControlMultiplayer = this;
		_menuLevelParent.exitRaceButton.SetActive(value: false);
		carCamerasController = GetComponent<CarCamerasController>();
	}

	private void OnDestroy()
	{
		_freeDriveProvider.DragRacingControlMultiplayer = null;
		_cancelRaceHandle.Get(_eventRegistry)?.Unsubscribe(OnCancelRaceInput);
	}

	public void SetCurrentLoopLabel(int currentLoop, int overallLoop)
	{
		MonoBehaviour.print(currentLoop + " " + overallLoop);
		if (overallLoop <= 1 || currentLoop > overallLoop)
		{
			newRacingMultiplayerController.lapCountLabel.gameObject.SetActive(value: false);
			return;
		}
		newRacingMultiplayerController.lapCountLabel.gameObject.SetActive(value: true);
		newRacingMultiplayerController.lapCountLabel.text = currentLoop + "/" + overallLoop;
	}

	public void onReachedStartLine()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonView.Get(V.Car.GetComponent<PhotonView>()).RpcToDefaultGroup("SendValueStr", RpcTarget.Others, (byte)16, _freeDriveControl.LastInvitationReceivedId.ToString(), "");
		}
	}

	public void onReachedFinishLine()
	{
		if (_coreGameManager.CarFinished || newRacingMultiplayerController.runDistance == DragRacingController.RunDistance.QuaterMile || newRacingMultiplayerController.runDistance == DragRacingController.RunDistance.OneMile || newRacingMultiplayerController.runDistance == DragRacingController.RunDistance.HalfMile)
		{
			return;
		}
		VehicleData component = V.LastUsedCar.GetComponent<VehicleData>();
		MonoBehaviour.print("onReachedFinishLine");
		if ((int)CheckPointOverAll != 0)
		{
			if ((int)checkPointCount == 0)
			{
				return;
			}
			if ((int)checkPointCount < (int)CheckPointOverAll)
			{
				float num = ((int)CheckPointOverAll - (int)checkPointCount) * 30;
				MonoBehaviour.print("Not all check points passed! " + num);
				_coreGameManager.SetMessage(string.Concat(new LocalizedString("MultiDragRacingControll.NotAllCheckPointsPassedN"), num.ToString()), 5f);
				NewRacingMultiplayerController obj = newRacingMultiplayerController;
				obj.fineToAdd = (float)obj.fineToAdd + num;
			}
		}
		++loopCount;
		if ((int)loopOverAll != 0 && (int)loopCount < (int)loopOverAll)
		{
			SetCurrentLoopLabel((int)loopCount + 1, loopOverAll);
			newRacingMultiplayerController.CurrentRacingTree.EnableCheckPoints();
			checkPointCount = 0;
			return;
		}
		SetCurrentLoopLabel((int)loopCount + 1, 0);
		_carInputManager.State = 1;
		component.racingState = RacingState.Finished;
		_coreGameManager.CarFinished = true;
		_ = newRacingMultiplayerController.OnCarFinishLapRace(V.LastUsedCar);
		_recordResultDisposable = Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
		{
			RecordResult();
		}).AddTo(this);
	}

	private bool checkCar()
	{
		if (!V.Car)
		{
			return false;
		}
		bool result = true;
		if (V.Car != _freeDriveProvider.FirstCarMultiPlayer)
		{
			return false;
		}
		V.Car.GetComponent<Powertrain>();
		V.Car.GetComponent<VehicleData>();
		V.Car.GetComponent<Rigidbody>();
		return result;
	}

	private void RecordResult()
	{
		int num = 0;
		if (carTimerToFinish - TimeMultiRaceCarFinish < 0f || TimeMultiRaceCarFinish == 0f)
		{
			_currencyManagerController.AddMoney(new AdvancedInt(_earnMoneyService.MultiplayerRaceWin));
			ObscuredPrefs.SetInt("RaceWin", ObscuredPrefs.GetInt("RaceWin") + 1);
			num = 1;
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			int? race_win = 1;
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, race_win), forceSend: true).Forget();
		}
		else
		{
			num = 0;
			AdvancedInt price = new AdvancedInt(_pricesService.MultiplayerRaceLose);
			_currencyManagerController.TryMoneySpend(out var _, price, showShop: false, spendAnyway: true);
			ObscuredPrefs.SetInt("RaceLose", ObscuredPrefs.GetInt("RaceLose") + 1);
		}
		if (checkCar())
		{
			V.Car.GetComponent<Powertrain>();
			if (getRaceType() != -1 && num == 1 && !UtilsVehicles.IsBlackVehicle())
			{
				++_freeDriveProvider.FreeDriveDB.dragWins;
			}
		}
	}

	private int getRaceType()
	{
		return (int)newRacingMultiplayerController.runDistance;
	}

	private void SettingOnlineCarsForDragRacing()
	{
		VehicleData component = V.Car.GetComponent<VehicleData>();
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			GameObject loadCar = component.TowingComposition.LoadCar;
			if (!multiPlayerPosControl || !(multiPlayerPosControl.gameObject != V.Car))
			{
				continue;
			}
			Rigidbody component2 = multiPlayerPosControl.GetComponent<Rigidbody>();
			if (multiPlayerPosControl.gameObject != _freeDriveProvider.RacingCar2)
			{
				if (multiPlayerPosControl.gameObject != loadCar)
				{
					component2.isKinematic = true;
					multiPlayerPosControl.forceDisableCar = true;
					multiPlayerPosControl.FarFromPerson = true;
				}
				continue;
			}
			if ((bool)V.Car && V.Car == component2.gameObject)
			{
				component2.isKinematic = false;
			}
			multiPlayerPosControl.forceDisableCar = false;
			multiPlayerPosControl.FarFromPerson = false;
			CarColliders.SetIgnore(component, multiPlayerPosControl.GetComponent<VehicleData>(), ignore: true);
		}
		Physics.IgnoreLayerCollision(17, 31, ignore: true);
	}

	private void ResetBackOnlineCar()
	{
		GameObject gameObject = V.Car;
		if (!gameObject)
		{
			gameObject = V.LastUsedCar;
		}
		VehicleData component = gameObject.GetComponent<VehicleData>();
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl)
			{
				Rigidbody component2 = multiPlayerPosControl.GetComponent<Rigidbody>();
				if ((bool)V.Car && V.Car == component2.gameObject)
				{
					component2.isKinematic = false;
				}
				multiPlayerPosControl.forceDisableCar = false;
				multiPlayerPosControl.FarFromPerson = false;
				if (multiPlayerPosControl.gameObject == _freeDriveProvider.RacingCar2)
				{
					CarColliders.SetIgnore(component, multiPlayerPosControl.GetComponent<VehicleData>(), ignore: false);
				}
			}
		}
		Physics.IgnoreLayerCollision(17, 31, ignore: false);
	}

	private async UniTask FinishMenuAsync(float delay, bool isUpdate, CancellationToken ct = default(CancellationToken))
	{
		int num = ObscuredPrefs.GetInt("RaceWin");
		int num2 = ObscuredPrefs.GetInt("RaceLose");
		int num3 = num + num2;
		string text = carTimerToFinish + "?" + timeTo100 + "?" + timeTo200 + "?" + timeTo500 + "?" + maxSpeed1 + "?" + timeTo200 + "?" + num + "?" + num3;
		PhotonView.Get(V.Car.GetComponent<PhotonView>()).RpcToDefaultGroup("SendValueStr", RpcTarget.Others, (byte)17, playerDataStorage.PlayerName, text);
		ShowResult(carTimerToFinish, carTimer2ToFinish, timeTo100, timeTo500, maxSpeed1, isUpdate, timeTo200);
		_levelsProvider.LevelScript.CleanScreen();
		if (V.CurrentCamIndex != 5)
		{
			prevCamIndex = carCamerasController.i;
			V.CurrentCamIndex = 5;
			carCamerasController.SetCamera(5, V.Car.transform, newTarget: false);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		_freeDriveProvider.FreeDriveControl.ShowFinishMenu();
		GetComponent<CarCamerasController>().SetPostionForCamStaticMode();
		FlagsStorage.FixedCameraAndLookAt = true;
		_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
	}

	public void StartRacing2()
	{
		_coreGameManager.SetMessage(new LocalizedString("MultiDragRacingControll.Go"), 2f);
		loopCount = 1;
		_carInputManager.State = 3;
		Reset();
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			_freeDriveProvider.RacingCar2.GetComponent<MultiPlayerPosControl>().EnableCar();
		}
	}

	private async UniTask PreSetCarSceneForDragRace()
	{
		Rigidbody component = V.Car.GetComponent<Rigidbody>();
		Powertrain component2 = V.Car.GetComponent<Powertrain>();
		VehicleData component3 = V.Car.GetComponent<VehicleData>();
		component.angularVelocity = Vector3.zero;
		component.velocity = Vector3.zero;
		_freeDriveProvider.QuickCarSettings.SetGearSelf(4);
		component2.Gear = 1;
		_menuLevelParent.exitRaceButton.SetActive(value: false);
		freeControlRacing = false;
		CheckPointOverAll = 0;
		loopOverAll = 0;
		V.placeToSpawn placeToSpawn = newRacingMultiplayerController.CurrentRacingTree.PlaceToSpawn;
		if (newRacingMultiplayerController.CurrentRacingTree.Races != null && newRacingMultiplayerController.CurrentRacingTree.Races.Count > 0)
		{
			V.SceneControl.LoadEnvironmentByIndex((int)SceneTypeExtensions.FromPlaceToSpawn(placeToSpawn), immediately: true);
			component3.KeepRowComposition.RacingTargets = null;
			component3.KeepRowComposition.CurrentTargetIndex = 0;
			component3.KeepRowComposition.RaceToAddForFollow = 1;
			offroadRacingObjects.SetActive(placeToSpawn == V.placeToSpawn.Offroad);
			DragRacingMultiplayerTree.RaceEntry currentRaceEntry = newRacingMultiplayerController.CurrentRacingTree.CurrentRaceEntry;
			if (currentRaceEntry != null)
			{
				CheckPointOverAll = DragRacingMultiplayerTree.CountUniqueCheckpoints(currentRaceEntry.Checkpoints);
				loopOverAll = ((currentRaceEntry.Type == DragRacingController.RunDistance.Lap) ? currentRaceEntry.LoopOverAll : 0);
			}
		}
		else
		{
			switch (placeToSpawn)
			{
			case V.placeToSpawn.MainCity:
				V.SceneControl.LoadMainCity(immediately: true);
				component3.KeepRowComposition.CurrentTargetIndex = 860;
				component3.KeepRowComposition.RaceToAddForFollow = -1;
				if (_freeDriveProvider.FreeDriveControl.InvitationHost)
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row5TrafficArrayV;
					break;
				}
				component3.KeepRowComposition.CurrentTargetIndex = 635;
				component3.KeepRowComposition.RacingTargets = _sort2.row6TrafficArrayV;
				break;
			case V.placeToSpawn.Highway:
				V.SceneControl.LoadHighway2(immediately: true);
				component3.KeepRowComposition.RaceToAddForFollow = -1;
				component3.KeepRowComposition.CurrentTargetIndex = 449;
				if (_freeDriveProvider.FreeDriveControl.InvitationHost)
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row2City2;
				}
				else
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row3City2;
				}
				break;
			case V.placeToSpawn.Desert:
				V.SceneControl.LoadDesert(immediately: true);
				if (SelectedDistance == 0)
				{
					freeControlRacing = true;
					CheckPointOverAll = 7;
				}
				else if (SelectedDistance == 1)
				{
					if ((bool)newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree)
					{
						newRacingMultiplayerController.CurrentRacingTree = newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree;
					}
					freeControlRacing = true;
					CheckPointOverAll = 41;
				}
				component3.KeepRowComposition.RaceToAddForFollow = -1;
				component3.KeepRowComposition.CurrentTargetIndex = 133;
				if (_freeDriveProvider.FreeDriveControl.InvitationHost)
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row4TrafficArrayV;
				}
				else
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row5TrafficArrayV;
				}
				break;
			case V.placeToSpawn.Mountain:
				V.SceneControl.LoadMountain(immediately: true);
				if (SelectedDistance == 0)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 16;
					if (_freeDriveProvider.FreeDriveControl.InvitationHost)
					{
						component3.KeepRowComposition.RacingTargets = _sort2.MountainRoad402_2V;
					}
					else
					{
						component3.KeepRowComposition.RacingTargets = _sort2.MountainRoad402_1V;
					}
				}
				else if (SelectedDistance == 1)
				{
					if ((bool)newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree)
					{
						newRacingMultiplayerController.CurrentRacingTree = newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree;
					}
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					if (_freeDriveProvider.FreeDriveControl.InvitationHost)
					{
						component3.KeepRowComposition.RacingTargets = _sort2.MountainRoadRoundBigV;
					}
					else
					{
						component3.KeepRowComposition.RacingTargets = _sort2.MountainRoadRoundBigV2;
					}
				}
				break;
			case V.placeToSpawn.Racing:
				V.SceneControl.LoadRaceTrack(immediately: true);
				if (SelectedDistance == 0)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 15;
					if (_freeDriveProvider.FreeDriveControl.InvitationHost)
					{
						component3.KeepRowComposition.RacingTargets = _sort2.RacingPlaceRoad4022;
					}
					else
					{
						component3.KeepRowComposition.RacingTargets = _sort2.RacingPlaceRoad4021;
					}
				}
				else if (SelectedDistance == 1)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 13;
					loopOverAll = 1;
				}
				else if (SelectedDistance == 2)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 5;
					loopOverAll = 2;
				}
				break;
			case V.placeToSpawn.City2:
				V.SceneControl.LoadCity2(immediately: true);
				component3.KeepRowComposition.CurrentTargetIndex = 600;
				component3.KeepRowComposition.RaceToAddForFollow = -1;
				if (_freeDriveProvider.FreeDriveControl.InvitationHost)
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row4City2;
				}
				else
				{
					component3.KeepRowComposition.RacingTargets = _sort2.row5City2;
				}
				break;
			case V.placeToSpawn.Offroad:
				V.SceneControl.LoadOffRoad(immediately: true);
				offroadRacingObjects.SetActive(value: true);
				if (SelectedDistance == 0)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 13;
					loopOverAll = 2;
				}
				else if (SelectedDistance == 1)
				{
					if ((bool)newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree)
					{
						newRacingMultiplayerController.CurrentRacingTree = newRacingMultiplayerController.CurrentRacingTree.otherDistanceTree;
					}
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 16;
					loopOverAll = 0;
				}
				break;
			case V.placeToSpawn.JapanSerpentine:
				V.SceneControl.LoadJapanScene(immediately: true);
				if (SelectedDistance == 0)
				{
					freeControlRacing = true;
					CheckPointOverAll = 5;
					loopOverAll = 0;
				}
				else if (SelectedDistance == 1)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 5;
					loopOverAll = 0;
				}
				break;
			case V.placeToSpawn.JapanSuburban:
				V.SceneControl.LoadJapanScene(immediately: true);
				if (SelectedDistance == 0)
				{
					freeControlRacing = true;
					CheckPointOverAll = 5;
					loopOverAll = 0;
				}
				else if (SelectedDistance == 1)
				{
					component3.KeepRowComposition.RaceToAddForFollow = 1;
					component3.KeepRowComposition.CurrentTargetIndex = 45;
					freeControlRacing = true;
					CheckPointOverAll = 5;
					loopOverAll = 0;
				}
				break;
			}
		}
		_timeAttackController.TryHideStartTriggersInLocation(newRacingMultiplayerController.CurrentRacingTree.PlaceToSpawn);
		newRacingMultiplayerController.CurrentRacingTree.Activate();
		await UniTask.NextFrame();
		newRacingMultiplayerController.EnablePropsForRacingPlace(newRacingMultiplayerController.CurrentRacingTree);
		_carInputManager.IgnoreSteerControl(state: false);
		_ = _freeDriveProvider.FreeDriveControl.InvitationHost;
	}

	public void ReplayYesRestart()
	{
		AnswerReplay = 1;
		if (PhotonUtility.TryGetPlayer(_freeDriveControl.LastInvitationReceivedId, out var player))
		{
			_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(18, 2.ToString(), "", player);
		}
		CheckReplayResult();
	}

	public void ReplayNORestart()
	{
		AnswerReplay = 0;
		if (PhotonUtility.TryGetPlayer(_freeDriveControl.LastInvitationReceivedId, out var player))
		{
			_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(18, 3.ToString(), "", player);
		}
		CheckReplayResult();
	}

	public async void ReplayReceived(int i)
	{
		int waitCount = 0;
		if (lastReplay == -1)
		{
			waitCount = 11;
		}
		lastReplay = i;
		for (; waitCount <= 20; waitCount++)
		{
			await UniTask.Delay(10);
			if (i != lastReplay)
			{
				MonoBehaviour.print("cancelled");
				return;
			}
		}
		if (i == 2)
		{
			AnswerReplayGuest = 1;
			_freeDriveProvider.FreeDriveControl.newNegotiationWindow.otherCarRepliedYES();
		}
		else
		{
			AnswerReplayGuest = 0;
			_freeDriveProvider.FreeDriveControl.newNegotiationWindow.otherCarRepliedNO();
		}
		CheckReplayResult();
	}

	private void CheckReplayResult()
	{
		if (AnswerReplay == AnswerReplayGuest && AnswerReplay == 1)
		{
			Reset();
			OnRestartRace();
			if ((bool)newRacingMultiplayerController)
			{
				newRacingMultiplayerController.NotifyReplayRestart();
			}
			if (prevCamIndex != -1)
			{
				carCamerasController.SetCamera(prevCamIndex, V.Car.transform, newTarget: false);
				FlagsStorage.CamStaticMode = false;
				FlagsStorage.FixedCameraAndLookAt = false;
			}
		}
	}

	public void Reset()
	{
		selectedOwn = -1;
		selectedGuest = -1;
		resetValue();
	}

	public void resetValue()
	{
		maxSpeed1 = 0f;
		timeTo100 = 0f;
		timeTo200 = 0f;
		timeTo500 = 0f;
		freeControlRacing = false;
		TimerOverAll = 0f;
		carTimerToFinish = 0f;
		carTimer2ToFinish = 0f;
		checkPointCount = 0;
		loopCount = 0;
		_coreGameManager.CarFinished = false;
		FlagsStorage.Car2Finished = false;
		FlagsStorage.RaceStarted = false;
		TimeMultiRaceCarFinish = 0f;
		MultiRaceCar100Km = 0f;
		MultiRaceCar200Km = 0f;
		MultiRaceCar500Km = 0f;
		MultiRaceCarMaxSpeed = 0f;
		MultiRaceWon = 0f;
		MultiRaceOverallRace = 0f;
		AnswerReplay = -1;
		AnswerReplayGuest = -1;
		sendValue = true;
	}

	public void FinishRace()
	{
		MonoBehaviour.print("finish race");
		newRacingMultiplayerController.EnablePropsForRacingPlace(state: false);
		offroadRacingObjects.SetActive(value: false);
		FlagsStorage.RacingGameMode = false;
		FlagsStorage.RaceStarted = false;
		_coreGameManager.CarFinished = false;
		FlagsStorage.Car2Finished = false;
		Reset();
		SetCarToFreeDrive();
		ResetBackOnlineCar();
		_freeDriveProvider.RacingCar2 = null;
		V.LastUsedCar.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		_freeDriveProvider.FreeDriveControl.EndRace();
		SetCurrentLoopLabel(0, 0);
		_menuLevelParent.exitRaceButton.SetActive(value: false);
		FlagsStorage.FixedCameraAndLookAt = false;
		if (prevCamIndex != -1)
		{
			carCamerasController.SetCamera(prevCamIndex, V.LastUsedCar.transform, newTarget: false);
		}
		if ((bool)racingUpDownButton)
		{
			racingUpDownButton.gameObject.SetActive(value: false);
		}
		_carInputManager.Ready = false;
	}

	private void OnCancelRaceInput()
	{
		if (FlagsStorage.RacingGameMode && (bool)_freeDriveProvider.RacingCar2)
		{
			ExitRace();
		}
	}

	public void ExitRace()
	{
		if (checkCar())
		{
			Powertrain component = V.Car.GetComponent<Powertrain>();
			int raceType = getRaceType();
			component.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var _, out var _, out var _);
			if (raceType != -1)
			{
				_freeDriveProvider.LeaderBoard.UpdateRacing(0, raceType, maxHp, 0f, this.GetCancellationTokenOnDestroy()).Forget();
			}
		}
		if (PhotonNetwork.IsConnected)
		{
			GameObject gameObject = V.Car;
			if (!gameObject)
			{
				gameObject = V.LastUsedCar;
			}
			PhotonView.Get(gameObject.GetComponent<PhotonView>()).RpcToDefaultGroup("SendValueStr", RpcTarget.Others, (byte)44, _freeDriveControl.LastInvitationReceivedId.ToString(), "");
		}
		ObscuredInt money = CurrencyManagerController.Money;
		if ((int)money > 0)
		{
			money = Mathf.Min(money, _pricesService.MultiplayerRaceLose);
			CoreGameManager coreGameManager = _coreGameManager;
			string text = new LocalizedString("MultiDragRacingControll.RaceWastedN");
			ObscuredInt obscuredInt = money;
			coreGameManager.SetMessage(text + obscuredInt.ToString(), 5f);
		}
		else
		{
			_coreGameManager.SetMessage(new LocalizedString("MultiDragRacingControll.RaceWasted"), 5f);
		}
		AdvancedInt price = new AdvancedInt(_pricesService.MultiplayerRaceLose);
		_currencyManagerController.TryMoneySpend(out var _, price, showShop: false, spendAnyway: true);
		newRacingMultiplayerController.OnRaceFinished();
	}

	private void SetCarToFreeDrive()
	{
		_carInputManager.IgnoreSteerControl(state: false);
		_carInputManager.State = 3;
	}

	private void OnRestartRace()
	{
		if ((bool)_freeDriveProvider.FreeDriveControl.newNegotiationWindow)
		{
			_freeDriveProvider.FreeDriveControl.newNegotiationWindow.Close();
		}
		_ = RaceBegin();
	}

	private async UniTask RaceBegin()
	{
		GPSTracker.CancelGPSDraw();
		MonoBehaviour.print("race begin");
		await PreSetCarSceneForDragRace();
		FlagsStorage.RacingGameMode = true;
		FlagsStorage.ChasingRaceModeStarted = false;
		_freeDriveProvider.FreeDriveControl.InvitationJustSend = false;
		await UniTask.NextFrame();
		SettingOnlineCarsForDragRacing();
		_freeDriveProvider.FreeDriveControl.chatControl.ShowPartiallyChat();
		if ((bool)V.Car)
		{
			V.Car.GetComponent<MultiPlayerPosControl>().ForceSendNextStream();
		}
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			MultiPlayerPosControl component = _freeDriveProvider.RacingCar2.GetComponent<MultiPlayerPosControl>();
			component.EnableCar();
			component.ForceSendNextStream();
		}
		_enableAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			enableAfter();
		}).AddTo(this);
		if ((bool)racingUpDownButton)
		{
			racingUpDownButton.gameObject.SetActive(value: true);
		}
		_menuLevelParent.exitRaceButton.SetActive(value: true);
		newRacingMultiplayerController.OnRaceStarted();
		if (!newRacingMultiplayerController.CurrentRacingTree || !newRacingMultiplayerController.CurrentRacingTree.otherCar)
		{
			_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.ExitRace();
			_coreGameManager.SetMessage(new LocalizedString("MultiPlayerPosControl.PlayerCanceledRace"), 3f);
			return;
		}
		_coreGameManager.SetMessage(new LocalizedString("MultiDragRacingControll.MoveSlowlyToTheStartLine"), 3f);
		SetCurrentLoopLabel(1, loopOverAll);
		_freeDriveProvider.FreeDriveControl.ShowMainDrivingUI();
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFreeRace, setRoot: true);
	}

	private void enableAfter()
	{
		if ((bool)_freeDriveProvider.RacingCar2)
		{
			_freeDriveProvider.RacingCar2.GetComponent<MultiPlayerPosControl>().EnableCar();
		}
	}

	private void Start()
	{
		Reset();
		newRacingMultiplayerController.DisableAllCheckPoints();
		_cancelRaceHandle.Get(_eventRegistry).Subscribe(OnCancelRaceInput);
	}

	public void SetOwnDistance(int i)
	{
		selectedOwn = i;
		CheckChoosedDistance();
	}

	public void EnableGuestDistance(int newInput)
	{
		selectedGuest = newInput;
		CheckChoosedDistance();
		FreeDragRacingWindow newNegotiationWindow = _freeDriveProvider.FreeDriveControl.newNegotiationWindow;
		if ((bool)newNegotiationWindow)
		{
			newNegotiationWindow.SetEnemyDistance(newInput);
		}
	}

	private async void CheckChoosedDistance()
	{
		if (selectedOwn == -1 || selectedGuest == -1 || selectedGuest != selectedOwn)
		{
			return;
		}
		int waitCount = 0;
		lastInput = selectedGuest;
		for (; waitCount <= 10; waitCount++)
		{
			await UniTask.Delay(100);
			if (selectedGuest != lastInput || selectedGuest != selectedOwn)
			{
				MonoBehaviour.print("cancelled");
				return;
			}
		}
		SelectedDistance = selectedOwn;
		newRacingMultiplayerController.SetDistance();
		if (newRacingMultiplayerController.runDistance == DragRacingController.RunDistance.Pursuit)
		{
			if ((bool)_freeDriveProvider.RacingCar2)
			{
				_freeDriveProvider.FreeDriveControl.MultiRaceTimeLeft = 60f;
				if (_freeDriveProvider.FreeDriveControl.InvitationHost)
				{
					_freeDriveProvider.FreeDriveControl.InvitationAccepted();
				}
				else
				{
					_freeDriveProvider.FreeDriveControl.AcceptChasing();
				}
				FlagsStorage.ChasingRaceModeStarted = true;
			}
		}
		else if (_freeDriveProvider.FreeDriveControl.InvitationHost)
		{
			int num = ((newRacingMultiplayerController.LastResolvedRaceIndex >= 0) ? newRacingMultiplayerController.LastResolvedRaceIndex : selectedOwn);
			sendValueStr(54, _freeDriveControl.LastInvitationReceivedId.ToString(), num.ToString());
			_ = RaceBegin();
		}
		if ((bool)_freeDriveProvider.FreeDriveControl.newNegotiationWindow)
		{
			_freeDriveProvider.FreeDriveControl.newNegotiationWindow.Close();
		}
		_freeDriveProvider.FreeDriveControl.ShowMainDrivingUI();
	}

	public void StartRaceByHostDistance(int value)
	{
		DragRacingMultiplayerTree currentRacingTree = newRacingMultiplayerController.CurrentRacingTree;
		if (currentRacingTree != null && currentRacingTree.Races != null && value >= 0 && value < currentRacingTree.Races.Count)
		{
			newRacingMultiplayerController.ApplyResolvedRaceIndex(value);
			SelectedDistance = currentRacingTree.MenuSlotForType(currentRacingTree.Races[value].Type);
		}
		else
		{
			SelectedDistance = value;
			newRacingMultiplayerController.SetDistance();
		}
		_ = RaceBegin();
	}

	public void EnableOwnToggleDistacne(int i)
	{
		selectedOwn = i;
		if (PhotonUtility.TryGetPlayer(_freeDriveControl.LastInvitationReceivedId, out var player))
		{
			_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(12, "", i.ToString(), player);
		}
		SetOwnDistance(i);
	}

	public void sendValueStr(int a, string b, string name)
	{
		PhotonView photonView = PhotonView.Get(V.Car.GetComponent<PhotonView>());
		byte b2 = (byte)a;
		photonView.RpcToDefaultGroup("SendValueStr", RpcTarget.Others, b2, b, name);
	}

	public void ShowResult(float car1FinisthTime, float car2FinisthTime, float timeTo100, float timeTo500, float maxSpeed1, bool updateResalt, float timeTo200)
	{
	}

	private string formatTime(float time)
	{
		int num = (int)(time / 3600f);
		num.ToString();
		if (num < 10)
		{
			_ = "0" + num;
		}
		int num2 = (int)(time % 3600f) / 60;
		string text = num2.ToString();
		if (num2 < 10)
		{
			text = "0" + num2;
		}
		float num3 = time % 3600f % 60f;
		string text2 = num3.ToString("F");
		if (num3 < 10f)
		{
			text2 = "0" + num3.ToString("F");
		}
		if (num2 == 0)
		{
			return text2;
		}
		return text + ":" + text2;
	}
}
