using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CPM.DailyTasks;
using CPM.Rating;
using CPM.Rating.Entities;
using CPM.Transmission;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Flags;
using TMPro;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem.Mediators;

public class DragRacingUI : MonoBehaviour
{
	public GameObject[] menuList;

	public GameObject[] UIForDrive;

	public GameObject[] UIForWalk;

	public GameObject[] UIForManual;

	public GameObject[] UIForAuto;

	public GameObject winnerMenu;

	public GameObject looserMenu;

	public NextRacePopupWindow nextRacePopupWindow;

	public Transform parentMainScroll;

	public TextMeshProUGUI raceCountText;

	public TextMeshProUGUI yourLoserWindowNickname;

	public TextMeshProUGUI yourWinnerWindowNickname;

	public RawImage yourLoserWindowFlagImage;

	public RawImage yourWinnerWindowFlagImage;

	public Image circleStage;

	public Image circleTimeToStage;

	public Image arrowCircle;

	public GameObject arrowDone;

	public GameObject finishSign;

	public Transform parentResult;

	public TextMeshProUGUI finishTextResult;

	public TextMeshProUGUI falseStartCountLabel;

	public TextMeshProUGUI loseDescription;

	public TextMeshProUGUI loseWinText;

	public GameObject restartRaceButtonOnFinish;

	public GameObject toGarageButtonOnFinish;

	public TextMeshProUGUI winnerPlaceLabel;

	public Image LCfill;

	public GameObject burnOutButton;

	private Powertrain _ownPowertrain;

	private VehicleData _vehicleData;

	public DragRacingCarSettingsUI dragRacingCarSettingsUI;

	public DragRacingController controller;

	public Image steeringAutoTexture;

	private string white = "<#ffffff>";

	private string green = "<#99ff00>";

	private string red = "<#ff0000>";

	public int prevMenu;

	public Slider progressBar;

	public GameObject buttonGarage;

	public GameObject leftUpDown;

	public GameObject rightUpDown;

	public GameObject nowRacingWindow;

	public OneRacerInList oneRacerInListPrefab;

	public OneRacerInList oneRacerInLoserListPrefab;

	public GameObject RacerInListParent;

	public GameObject RacerWinnerInListParent;

	public GameObject RacerLoserListParent;

	public GameObject loadingWindow;

	public GameObject allMenu;

	public bool lastAutoSteeringState;

	private RaceFinishUI _raceFinishUI;

	private static IPrefabFactory _prefabFactory;

	public UINavigationHelper garageNavigationHelper;

	private FlagsDataSO _flagsDataSo;

	public OtherUserRaceController otherUsersRaceController;

	private ManagerUI _managerUI;

	[SerializeField]
	[Header("AIs raceRegion")]
	private UserCard leftBotCard;

	[SerializeField]
	private UserCard rightBotCard;

	[SerializeField]
	private UserCard ownInfoWindow;

	[SerializeField]
	private TextMeshProUGUI startRaceButtonText;

	[SerializeField]
	private TextMeshProUGUI nextRaceText;

	[SerializeField]
	private GameObject testRunButton;

	[SerializeField]
	public GameObject resumeButton;

	[SerializeField]
	private TextMeshProUGUI CityName;

	[SerializeField]
	private TextMeshProUGUI LocationName;

	public GameObject nowRacingNotificationWindow;

	public GameObject firstPlaceCup;

	public GameObject thirdPlaceCup;

	private List<OneRaceResult> RaceResults;

	private int RaceWon;

	private int RaceLose;

	private UIReferencesMediator _uiReferencesMediator;

	private InputItemsProvider _inputItemsProvider;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	private RacingProvider _racingProvider;

	private RacingAnalytic _racingAnalytic;

	private CarInputManager _carInputManager;

	private RatingSender _ratingController;

	private DailyTasksManager _dailyTasksManager;

	private IDisposable _disableAutoSteeringLabelDisposable;

	private IDisposable _disableFinishSignDisposable;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private RegistryHandle<ScriptableEvent> _keepLaneHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.KeepLane);

	private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

	[Inject]
	public void Construct(RaceFinishUI raceFinishUI, IPrefabFactory prefabFactory, ManagerUI managerUI, FlagsDataSO flagsDataSo, UIReferencesMediator uiReferencesMediator, InputItemsProvider inputItemsProvider, CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, RacingProvider racingProvider, RacingAnalytic racingAnalytic, CarInputManager carInputManager, RatingSender ratingController, DailyTasksManager dailyTasksManager, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent)
	{
		_dailyTasksManager = dailyTasksManager;
		_ratingController = ratingController;
		_racingProvider = racingProvider;
		_menuLevelParent = menuLevelParent;
		_coreGameManager = coreGameManager;
		_inputItemsProvider = inputItemsProvider;
		_flagsDataSo = flagsDataSo;
		_raceFinishUI = raceFinishUI;
		_prefabFactory = prefabFactory;
		_managerUI = managerUI;
		_uiReferencesMediator = uiReferencesMediator;
		_racingAnalytic = racingAnalytic;
		_carInputManager = carInputManager;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void Awake()
	{
		SmoothListerVolume(this.GetCancellationTokenOnDestroy()).Forget();
		loadingWindow.SetActive(value: false);
		allMenu.SetActive(value: true);
		RaceResults = new List<OneRaceResult>();
		ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public void AddResult(Dragy dragy)
	{
		OneRaceResult oneRaceResult = new OneRaceResult();
		oneRaceResult.RT = dragy.reactionTime;
		oneRaceResult.quaterMilResult = dragy.quarterMileResult;
		oneRaceResult.exitSpeed = dragy.exitSpeed;
		_ = oneRaceResult.RT;
		_ = 0f;
		if (!controller.testRun)
		{
			if (dragy.lost)
			{
				RaceLose++;
			}
			else
			{
				RaceWon++;
			}
		}
		RaceResults.Add(oneRaceResult);
		ownInfoWindow.FillLastResultData(oneRaceResult);
		ownInfoWindow.ShowUserStatistics(RaceWon, RaceLose);
	}

	public void SetOwnInfoCurrentMileageType()
	{
		ownInfoWindow.SetCurrentMileageType();
	}

	private void Start()
	{
		disableFinishSign();
		parentResult.gameObject.SetActive(value: false);
		_ownPowertrain = V.Car.GetComponent<Powertrain>();
		_vehicleData = V.Car.GetComponent<VehicleData>();
		controller = GetComponent<DragRacingController>();
		ownInfoWindow.Init(_vehicleData.CarValues, _ownPowertrain, setFlag: false);
		_keepLaneHandle.Get(_eventRegistry).Subscribe(OnSteeringAutoChanged);
		_onMenuHandle.Get(_eventRegistry).Subscribe(ShowSettings);
	}

	private void OnDestroy()
	{
		_keepLaneHandle.Get(_eventRegistry)?.Unsubscribe(OnSteeringAutoChanged);
		_onMenuHandle.Get(_eventRegistry)?.Unsubscribe(ShowSettings);
	}

	public void UpdateProgressBar(int racerCount)
	{
	}

	public void ShowFinishOtherCars()
	{
		VehicleData component = controller.leftCar.GetComponent<VehicleData>();
		VehicleData component2 = controller.rightCar.GetComponent<VehicleData>();
		Dragy component3 = controller.leftCar.GetComponent<Dragy>();
		Dragy component4 = controller.rightCar.GetComponent<Dragy>();
		_ = component.CarValues;
		_ = component2.CarValues;
		nowRacingWindow.transform.GetChild(0).gameObject.SetActive(value: true);
		Transform child = nowRacingWindow.transform.GetChild(0).GetChild(0);
		float num = ((controller.runDistance == DragRacingController.RunDistance.QuaterMile) ? component3.quarterMileResult : component3.halfMileResult);
		if (num == 0f)
		{
			child.gameObject.SetActive(value: false);
		}
		else
		{
			child.gameObject.SetActive(value: true);
			otherUsersRaceController.leftCarCard.FillResultPanel(component3, num);
		}
		Transform child2 = nowRacingWindow.transform.GetChild(0).GetChild(1);
		num = ((controller.runDistance == DragRacingController.RunDistance.QuaterMile) ? component4.quarterMileResult : component4.halfMileResult);
		if (num == 0f)
		{
			child2.gameObject.SetActive(value: false);
		}
		else
		{
			child2.gameObject.SetActive(value: true);
			otherUsersRaceController.rightCarCard.FillResultPanel(component4, num);
		}
		otherUsersRaceController.ShowResult();
	}

	public void ShowOtherRacingCar()
	{
		if ((bool)controller.leftCar && (bool)controller.rightCar)
		{
			Powertrain component = controller.leftCar.GetComponent<Powertrain>();
			Powertrain component2 = controller.rightCar.GetComponent<Powertrain>();
			MainCarValues carValues = controller.leftCar.GetComponent<VehicleData>().CarValues;
			MainCarValues carValues2 = controller.rightCar.GetComponent<VehicleData>().CarValues;
			otherUsersRaceController.Init(carValues, component, carValues2, component2);
			component.CalcMaxPowerMaxTorque(out var _, out var _, out var _, out var _, out var _);
			SetActiveFalseEachMenu(4);
			controller.dragRacingCamera.LookAtOtherRacing();
		}
	}

	public void ShowRandomCarRaceNotification(MainCarValues car1, MainCarValues car2)
	{
		leftBotCard.Init(car1, null);
		rightBotCard.Init(car2, null);
		if (!garageNavigationHelper.ButtonTouched)
		{
			garageNavigationHelper.HideAll();
		}
		else
		{
			GameObject[] childsWindows = garageNavigationHelper.childsWindows;
			for (int i = 0; i < childsWindows.Length; i++)
			{
				if (childsWindows[i].activeSelf)
				{
					nowRacingNotificationWindow.SetActive(value: false);
					return;
				}
			}
		}
		nowRacingNotificationWindow.SetActive(value: true);
	}

	private void HideNowRaceWindow()
	{
		nowRacingNotificationWindow.SetActive(value: false);
	}

	public void ShowNextCar()
	{
		MonoBehaviour.print("show next car");
	}

	public void SetAutoSteering(bool auto)
	{
		TextMeshProUGUI component = steeringAutoTexture.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		component.gameObject.SetActive(value: true);
		_ = _vehicleData.CarValues.Gearbox;
		_disableAutoSteeringLabelDisposable?.Dispose();
		_disableAutoSteeringLabelDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			DisableAutoSteeringLabel();
		}).AddTo(this);
		if (auto)
		{
			steeringAutoTexture.color = Color.green;
			component.text = string.Concat(new LocalizedString("DragRacingUI.AutoSteering"), green, new LocalizedString("DragRacingUI.On"));
		}
		else
		{
			steeringAutoTexture.color = Color.white;
			component.text = string.Concat(new LocalizedString("DragRacingUI.AutoSteering"), red, new LocalizedString("DragRacingUI.Off"));
		}
	}

	public void OnSteeringAutoChanged()
	{
		_racingProvider.DragRacingController.autoSteering = !_racingProvider.DragRacingController.autoSteering;
		_racingProvider.DragRacingController.SetSteering(_racingProvider.DragRacingController.autoSteering);
		SetAutoSteering(_racingProvider.DragRacingController.autoSteering);
	}

	private void DisableAutoSteeringLabel()
	{
		steeringAutoTexture.transform.GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(value: false);
	}

	public void HideCircle()
	{
		circleStage.transform.parent.gameObject.SetActive(value: false);
	}

	public void ShowCircle()
	{
		circleStage.transform.parent.gameObject.SetActive(value: true);
	}

	public void onOwnCarStaged()
	{
		circleTimeToStage.gameObject.SetActive(value: false);
		buttonGarage.SetActive(value: false);
	}

	public void OnOtherCarStaged()
	{
		circleTimeToStage.gameObject.SetActive(_vehicleData.racingState == RacingState.rollingToStart);
		buttonGarage.SetActive(value: false);
	}

	public void setCircle(float distance, float stageRange)
	{
		if (distance < 5f)
		{
			ShowCircle();
		}
		else
		{
			HideCircle();
		}
		if (!circleStage.gameObject.activeSelf)
		{
			return;
		}
		if (math.abs(distance) < stageRange)
		{
			arrowDone.SetActive(value: true);
			arrowCircle.gameObject.SetActive(value: false);
			circleStage.fillAmount = 1f;
			circleStage.color = Color.green;
			return;
		}
		bool num = distance > 0f;
		arrowDone.SetActive(value: false);
		arrowCircle.gameObject.SetActive(value: true);
		if (distance < 0f)
		{
			distance += stageRange;
			distance = math.abs(distance);
			circleStage.fillAmount = 1f - distance / 5f;
		}
		else
		{
			distance -= stageRange;
			if (distance < 5f)
			{
				circleStage.fillAmount = 1f - distance / 5f;
			}
		}
		if (num)
		{
			arrowCircle.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			arrowCircle.color = Color.green;
			circleStage.color = Color.green;
		}
		else
		{
			arrowCircle.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			arrowCircle.color = Color.red;
			circleStage.color = Color.red;
		}
	}

	public void setCircleStage(float timeLeft)
	{
		if (circleTimeToStage.gameObject.activeInHierarchy && _vehicleData.racingState == RacingState.rollingToStart)
		{
			float num = 1f - timeLeft / 30f;
			if (num <= 0f && controller.dragy.reason != Dragy.LostReason.timeOut)
			{
				controller.OnGarageOpen();
				circleTimeToStage.SetActive(active: false);
			}
			else
			{
				num = math.clamp(num, 0f, 1f);
				circleTimeToStage.fillAmount = num;
			}
		}
	}

	public void OnChangeClass(int newClass)
	{
		controller.ChangeClass(newClass);
	}

	public async void SetUIForGarage(GameObject car)
	{
		MonoBehaviour.print("SetUIForGarage");
		SetActiveFalseEachMenu(0);
		ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
		startRaceButtonText.text = (controller.qualification ? new LocalizedString("DragRacingUI.StartQualification") : new LocalizedString("DragRacingUI.Start"));
		nextRaceText.text = (controller.qualification ? new LocalizedString("DragRacingUI.Qualification") : new LocalizedString("DragRacingUI.NextRace"));
		testRunButton.SetActive(!controller.qualification);
		if (car != null)
		{
			VehicleData vehicleData = car.GetComponent<VehicleData>();
			if (controller.qualification)
			{
				vehicleData = _vehicleData;
			}
			nextRacePopupWindow.Init(vehicleData.CarValues, vehicleData.GetPowertrain);
			parentMainScroll.gameObject.SetActive(value: true);
		}
		else
		{
			parentMainScroll.gameObject.SetActive(value: false);
		}
		SetRaceCountUI(_racingProvider.DragRacingController.RaceWon);
		buttonGarage.SetActive(value: true);
		dragRacingCarSettingsUI.OnOpen();
		UpdateRacerList();
		await Task.Delay(1000);
		if (!garageNavigationHelper.ButtonTouched && RaceResults.Count > 0)
		{
			garageNavigationHelper.FocusOnWindowByIndex(0);
			garageNavigationHelper.ButtonTouched = false;
		}
	}

	public async UniTask SmoothListerVolume(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			AudioListener.volume = 0f;
			while (AudioListener.volume <= 1f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				AudioListener.volume += 0.01f;
				if (AudioListener.volume >= 1f)
				{
					AudioListener.volume = 1f;
					break;
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void SetUIForRacing(bool isAutomatic = false)
	{
		PlayerDatas.DisableEnableAll(isEnable: true, UIForDrive);
		PlayerDatas.DisableEnableAll(isEnable: false, UIForWalk);
		SetActiveFalseEachMenu(1);
		circleTimeToStage.gameObject.SetActive(value: false);
		_menuLevelParent.SetSteering();
	}

	public void ShowSettings()
	{
		if (_inputMapSetEvent.CurrentInputGroup != InputGroupsEnum.PersonDrag)
		{
			_managerUI.ShowSettingsWindow();
			HideCurrentMenu(isActive: false);
		}
	}

	public void SetTransmissionUI()
	{
		MainCarValues carValues = _vehicleData.CarValues;
		Powertrain component = carValues.car.GetComponent<Powertrain>();
		carValues.Gearbox = (component.IsAutomaticGearbox ? GearboxKind.AT : GearboxKind.MT);
		bool num = carValues.Gearbox == GearboxKind.MT;
		PlayerDatas.DisableEnableAll(num, UIForManual);
		PlayerDatas.DisableEnableAll(!num, UIForAuto);
		FlagsStorage.PRND = !num;
	}

	public void SetUIForWalking()
	{
		PlayerDatas.DisableEnableAll(isEnable: false, UIForDrive);
		PlayerDatas.DisableEnableAll(isEnable: true, UIForWalk);
		SetActiveFalseEachMenu(1);
	}

	public void Back()
	{
		SetActiveFalseEachMenu(prevMenu);
	}

	public void SetUIForPause()
	{
		SetActiveFalseEachMenu(2);
	}

	public void SetRaceCountUI(int raceWon)
	{
		Dragy component = V.LastUsedCar.GetComponent<Dragy>();
		winnerMenu.SetActive(value: false);
		looserMenu.SetActive(value: false);
		showLastRace();
		if (component.lost)
		{
			if (raceWon != 4)
			{
				return;
			}
			raceWon++;
		}
		firstPlaceCup.SetActive(value: true);
		thirdPlaceCup.SetActive(value: false);
		string text = new LocalizedString("DragRacingUI.raceisleft");
		switch (raceWon)
		{
		case 0:
			raceCountText.text = text.Replace("{0}", "5");
			break;
		case 1:
			raceCountText.text = text.Replace("{0}", "4");
			break;
		case 2:
			raceCountText.text = text.Replace("{0}", "3");
			break;
		case 3:
			raceCountText.text = text.Replace("{0}", "2");
			break;
		case 4:
			if (!_racingProvider.DragRacingController.raceFor3Place)
			{
				raceCountText.text = new LocalizedString("DragRacingUI.LastRaceFor1thPlace");
				break;
			}
			raceCountText.text = new LocalizedString("DragRacingUI.LastRaceFor3thPlace");
			firstPlaceCup.SetActive(value: false);
			thirdPlaceCup.SetActive(value: true);
			break;
		default:
			_ = 5;
			break;
		}
		if (component.falseStartCount == 0)
		{
			falseStartCountLabel.text = new LocalizedString("DragRacingUI.YouCanHaveOnly1FalseStart");
			falseStartCountLabel.color = Color.white;
		}
		else
		{
			falseStartCountLabel.text = new LocalizedString("DragRacingUI.DoNotFalseStartOtherwiseYouWil");
			falseStartCountLabel.color = Color.red;
		}
		string text2 = controller.timeClass + "s";
		if (controller.timeClass == 8)
		{
			text2 = new LocalizedString("DragRacingUI.Unlim");
		}
		TextMeshProUGUI[] racingClassTexts = _uiReferencesMediator.RacingClassTexts;
		foreach (TextMeshProUGUI textMeshProUGUI in racingClassTexts)
		{
			if ((bool)textMeshProUGUI)
			{
				textMeshProUGUI.text = text2;
			}
		}
	}

	public async void onWon()
	{
		ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
		parentMainScroll.gameObject.SetActive(value: false);
		loseWinText.text = new LocalizedString("DragRacingUI.Win");
		winnerPlaceLabel.text = string.Concat(new LocalizedString("DragRacingUI.CongratulationsYouAre"), controller.Place.ToString());
		ShowWinnerList();
		float time = 0f;
		int sec = 100;
		while (time < 4000f)
		{
			time += (float)sec;
			if (Input.touchCount > 0)
			{
				break;
			}
			await Task.Delay(sec);
		}
		looserMenu.SetActive(value: true);
	}

	private void Update()
	{
		if ((bool)_ownPowertrain)
		{
			_ownPowertrain.GetLaunchControlSettings(out var _, out var _, out var lcTimer, out var lcHoldTime);
			bool flag = (lcTimer > 0f || lcTimer > lcHoldTime) && _ownPowertrain.SpeedMps < 1f;
			LCfill.gameObject.SetActive(flag);
			bool num = _vehicleData.racingState == RacingState.rollingToStart;
			bool flag2 = _ownPowertrain.SpeedMps < 5f;
			GearboxType gearType = _ownPowertrain.GearType;
			bool flag3 = gearType != GearboxType.GearAwd && gearType != GearboxType.Gear4Wd;
			bool flag4 = _ownPowertrain.Gear > 0;
			bool flag5 = (num & flag2 & flag3 & flag4) && _ownPowertrain.IsEngineWorks;
			burnOutButton.SetActive(flag5);
			if (!flag5 && _vehicleData.racingState == RacingState.OnRacing && _carInputManager.ManualBurnOutActivator)
			{
				_carInputManager.SetBurnOut(value: false);
			}
			if (flag && lcTimer >= 0f)
			{
				LCfill.fillAmount = (lcHoldTime - lcTimer) / lcHoldTime;
			}
		}
	}

	public async void onLose()
	{
		ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
		Dragy component = V.LastUsedCar.GetComponent<Dragy>();
		parentMainScroll.gameObject.SetActive(value: false);
		string text = "";
		if (component.reason == Dragy.LostReason.slow)
		{
			text = new LocalizedString("DragRacingUI.YouCanWinNextTime");
		}
		else if (component.reason == Dragy.LostReason.falseStart)
		{
			text = new LocalizedString("DragRacingUI.YouLostFalseStart");
		}
		else if (component.reason == Dragy.LostReason.fasterClass)
		{
			text = new LocalizedString("DragRacingUI.YouCarFasterThan");
			text = string.Format(text, _racingProvider.DragRacingController.timeClass);
		}
		else if (component.reason == Dragy.LostReason.carAccident)
		{
			text = new LocalizedString("DragRacingUI.YouHadACarAccidentDisqualified");
		}
		else if (component.reason == Dragy.LostReason.timeOut)
		{
			text = new LocalizedString("DragRacingUI.StageIsTimeOutDisqualified");
		}
		else if (component.reason == Dragy.LostReason.outOfRoad)
		{
			text = new LocalizedString("DragRacingUI.OutOfRoadDisqualified");
		}
		loseWinText.text = new LocalizedString("UserCard.Lose");
		loseDescription.text = text;
		controller.fillWinnerList(16);
		ShowLoserList();
		float time = 0f;
		int sec = 100;
		while (time < 4000f)
		{
			time += (float)sec;
			if (Input.touchCount > 0)
			{
				break;
			}
			await UniTask.Delay(sec);
		}
		if ((bool)this && (bool)looserMenu)
		{
			looserMenu.SetActive(value: true);
		}
	}

	public void Restart()
	{
		_racingProvider.DragRacingController.StartNewRace();
	}

	public void showLastRace()
	{
		V.LastUsedCar.GetComponent<Dragy>();
	}

	public async void OpenMainMenuFromDragRacing()
	{
		controller.podium.StopCrowdSound();
		if (FlagsStorage.OutOfCar)
		{
			await _coreGameManager.CarOutInControl.InCar(V.DriverPerson, V.LastUsedCar);
		}
		if (controller.RaceWon > 0 && controller.raceInProcess)
		{
			HideCurrentMenu(isActive: false);
			_managerUI.ShowConfirmWindow(() =>
			{
				LoadMainMenu();
			}, new LocalizedString("DragRacingUI.DoYouWantToExit"), new LocalizedString("DragRacingUI.AllYourProgressWillBeLost"), () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
				HideCurrentMenu(isActive: true);
			}).Forget();
		}
		else
		{
			LoadMainMenu();
		}
	}

	private void LoadMainMenu()
	{
		SaveTime();
		_racingAnalytic.LogDragRacingFinish();
		Application.LoadLevel(1);
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
		string text = rating.ToJson();
		if (text != "}")
		{
			await _ratingController.SendRating(text);
			_dailyTasksManager.UpdateData(rating).Forget();
		}
	}

	public void SetActiveFalseEachMenu(int i)
	{
		if (prevMenu != i)
		{
			ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
			prevMenu = V.MenuIndex;
		}
		V.MenuIndex = i;
		GameObject[] array = menuList;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == menuList[i])
			{
				gameObject.SetActive(value: true);
				SmoothListerVolume(this.GetCancellationTokenOnDestroy()).Forget();
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
		InputStorage.ThrottleInput = 0f;
		InputStorage.BrakeInput = 0f;
	}

	public void SetHideAllMenu(int i)
	{
		if (prevMenu != i)
		{
			ShowLoadingAsync(this.GetCancellationTokenOnDestroy()).Forget();
			prevMenu = V.MenuIndex;
		}
		V.MenuIndex = i;
		GameObject[] array = menuList;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
		InputStorage.ThrottleInput = 0f;
		InputStorage.BrakeInput = 0f;
	}

	private async UniTask ShowLoadingAsync(CancellationToken ct = default(CancellationToken))
	{
		if (loadingWindow.activeSelf)
		{
			return;
		}
		CanvasGroup canvasGroup = loadingWindow.GetComponent<CanvasGroup>();
		try
		{
			loadingWindow.SetActive(value: true);
			canvasGroup.DOFade(0.8f, 1f);
			allMenu.SetActive(value: false);
			await UniTask.Delay(TimeSpan.FromSeconds(0.800000011920929), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			canvasGroup.DOFade(0f, 1f);
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			canvasGroup.DOKill();
			canvasGroup.alpha = 0f;
			loadingWindow.SetActive(value: false);
			allMenu.SetActive(value: true);
		}
	}

	public void ShowFinishSign()
	{
		if (!parentResult.gameObject.activeSelf)
		{
			finishSign.gameObject.SetActive(value: true);
			finishSign.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1f, 0.5f);
			_disableFinishSignDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				disableFinishSign();
			}).AddTo(this);
		}
	}

	public void closeResultWindow()
	{
		if (lastAutoSteeringState)
		{
			lastAutoSteeringState = false;
			controller.SetSteering(auto: false);
		}
		SetActiveFalseEachMenu(1);
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarDrag);
	}

	public void disableFinishSign()
	{
		finishSign.gameObject.SetActive(value: false);
		finishSign.transform.GetChild(0).GetComponent<Image>().fillAmount = 0f;
		buttonGarage.SetActive(value: true);
	}

	public void OnBurnOutPressed()
	{
		_carInputManager.SetBurnOut(value: true);
	}

	public void OnBurnOutReleasedReleased()
	{
		_carInputManager.SetBurnOut(value: false);
	}

	public void UpdateRacerList()
	{
		CreateRacersListAsync(controller.nowRacingCarClassList, RacerInListParent.transform, isLoserMenu: false, this.GetCancellationTokenOnDestroy()).Forget();
	}

	public void ShowWinnerList()
	{
		CreateRacersListAsync(controller.winners, RacerLoserListParent.transform, isLoserMenu: true, this.GetCancellationTokenOnDestroy()).Forget();
	}

	public void ShowLoserList()
	{
		CreateRacersListAsync(controller.winners, RacerLoserListParent.transform, isLoserMenu: true, this.GetCancellationTokenOnDestroy()).Forget();
	}

	private async UniTask CreateRacersListAsync(MainCarValues[] values, Transform parent, bool isLoserMenu = false, CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			foreach (Transform item in parent.transform)
			{
				if ((bool)item)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
			int num = 0;
			for (int i = 0; i < values.Length; i++)
			{
				OneRacerInList obj = (isLoserMenu ? oneRacerInLoserListPrefab : oneRacerInListPrefab);
				_prefabFactory.Instantiate(obj, parent).Init(values[i], i + 1);
				if (num > 5)
				{
					await UniTask.Yield(ct);
					num = 0;
				}
				else
				{
					num++;
				}
			}
			await UniTask.Yield(ct);
			LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void HideCurrentMenu(bool isActive)
	{
		menuList[V.MenuIndex].SetActive(isActive);
	}
}
