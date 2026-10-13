using System;
using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CPM.RemoteConfig;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RacingScripts.RaceControllers;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.LapRaceMode.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.RaceInfo;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

public class CircuitRaceSelectMapWindow : WindowScript, IBackableWindow
{
	private const int MinRaceCarsCount = 4;

	private const int MaxRaceCarsCount = 10;

	[SerializeField]
	private TextMeshProUGUI MapTitleText = default;

	[SerializeField]
	private Button StartButton = default;

	[SerializeField]
	private Transform ButtonsParent = default;

	[SerializeField]
	private Transform OnMapButtonsParent = default;

	[SerializeField]
	private SelectMapUIElement SelectMapUIElementPrefab = default;

	[SerializeField]
	private GameObject SelectClassPanel = default;

	[SerializeField]
	private GameObject SelectMapPanel = default;

	[SerializeField]
	private Button BackButton = default;

	[SerializeField]
	private Button BackClassButton = default;

	[SerializeField]
	private Button MapDirection = default;

	[SerializeField]
	private TextMeshProUGUI DirectionText = default;

	[SerializeField]
	private SliderValueHandler ApponentsCountSlider = default;

	[SerializeField]
	private TextMeshProUGUI LapsCount = default;

	[SerializeField]
	private TextMeshProUGUI Distance = default;

	[SerializeField]
	private TextMeshProUGUI TimeDurationText = default;

	[SerializeField]
	private SelectMapUIElement OnMapElementPrefab = default;

	[SerializeField]
	private TextMeshProUGUI CoinsLimit = default;

	[SerializeField]
	private TextMeshProUGUI MoneyLimit = default;

	[SerializeField]
	private CircuitRacingConfig CircuitRacingConfig = default;

	[SerializeField]
	private TextMeshProUGUI DescriptionLeman = default;

	[SerializeField]
	private RaceConfigSO[] RaceConfigs = default;

	[SerializeField]
	private RaceGroupItem[] RaceGroups = default;

	[SerializeField]
	private Toggle QualificationToggle = default;

	[SerializeField]
	private GameObject LapsPanel = default;

	[SerializeField]
	private GameObject TimeDurationPanel = default;

	[SerializeField]
	private ScrollRect GroupsScroll = default;

	[SerializeField]
	private float AutoScrollingDuration = 0.5f;

	[SerializeField]
	private CanvasGroup BudgetPanelCanvasGroup = default;

	[SerializeField]
	private CanvasGroup CoinsLimitCanvasGroup = default;

	[SerializeField]
	private Toggle CWToggle = default;

	[SerializeField]
	private Toggle CCWToggle = default;

	[SerializeField]
	private RaceClassInfoView ClassInfoView = default;

	[SerializeField]
	private CarSelectorPanel CarSelectorPanel = default;

	[SerializeField]
	private RaceInfoDropdown RaceInfoDropdown = default;

	[SerializeField]
	private GameObject LoadingPanel = default;

	private List<SelectMapUIElement> _togglesOnMap;

	private List<SelectMapUIElement> _togglesUI;

	private IPrefabFactory _prefabFactory;

	private IPlayerStorageService _playerStorageService;

	private int _selectedIndex;

	private RacingUIMap _racingUIMap;

	private int _direction;

	private ICarInfoService _carInfoService;

	private IRemoteConfigManager _remoteConfigManager;

	private ConfigValuesStorage _remoteConfigData;

	private bool _isBusy;

	private AllCarsInfo _allCarsInfo;

	private ITimerTuningService _timerTuningService;

	private IPlayerProfileService _playerProfileService;

	private CircuitRaceControlller _circuitRaceControlller;

	private IDisposable _groupPositionChecker = default;

	private LevelScript _levelScript;

	private string _backWindow;

	private InteractableRaceMapHandler _raceMapHandler;

	private bool _cachedIsQualification;

	private bool _isDestroyed;

	private RaceModeController _raceModeController;

	private OptionalDependency<ITimerTuningService> _timerTuningServiceDep;

	private OptionalDependency<LevelScript> _levelScriptDep;

	private OptionalDependency<InteractableRaceMapHandler> _raceMapHandlerDep;

	private OptionalDependency<RaceModeController> _raceModeControllerDep;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.RacingMap;

	protected override bool ChangeRootGameInputGroup => true;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, IPlayerStorageService playerStorageService, ICarInfoService carInfoService, IRemoteConfigManager remoteConfigManager, AllCarsInfo allCarsInfo, IPlayerProfileService playerProfileService, RacingAnalytic racingAnalytic, OptionalDependency<ITimerTuningService> timerTuningService, OptionalDependency<RaceModeController> raceModeControllerDep, OptionalDependency<LevelScript> levelScript, OptionalDependency<InteractableRaceMapHandler> raceMapHandler)
	{
		_playerProfileService = playerProfileService;
		_remoteConfigManager = remoteConfigManager;
		_carInfoService = carInfoService;
		_prefabFactory = prefabFactory;
		_playerStorageService = playerStorageService;
		_timerTuningServiceDep = timerTuningService;
		_allCarsInfo = allCarsInfo;
		_carInfoService = carInfoService;
		_levelScriptDep = levelScript;
		_raceMapHandlerDep = raceMapHandler;
		_raceModeControllerDep = raceModeControllerDep;
	}

	protected override void Awake()
	{
		base.Awake();
		_timerTuningService = _timerTuningServiceDep?.Value;
		_raceModeController = _raceModeControllerDep?.Value;
		if ((bool)_raceModeController)
		{
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
		}
		_levelScript = _levelScriptDep?.Value;
		_raceMapHandler = _raceMapHandlerDep?.Value;
	}

	public override void SetAdditionalData(string data)
	{
		base.SetAdditionalData(data);
		_backWindow = data;
	}

	private void OnCWDirectionSelected(bool state)
	{
		if (state)
		{
			ChangeDirection(-1);
		}
	}

	private void OnCCWDirectionSelected(bool state)
	{
		if (state)
		{
			ChangeDirection(1);
		}
	}

	private void ChangeDirection(int direction)
	{
		CircuitRaceControlller.Direction = direction;
	}

	private async UniTaskVoid CreateRaceMapImage()
	{
		LoadingPanel.SetActive(value: true);
		InteractableRaceMapHandler component = (await Addressables.LoadAssetAsync<GameObject>("InteractableRaceMapHandler").BindTo(V.CoreGameManager.ManagerUI.gameObject)).GetComponent<InteractableRaceMapHandler>();
		if (!_isDestroyed)
		{
			_raceMapHandler = _prefabFactory.Instantiate(component, _managerUI.GetWindowsParent());
			_raceMapHandler.gameObject.transform.SetSiblingIndex(0);
			_racingUIMap = _raceMapHandler.RacingUIMap;
			_racingUIMap.EnableMainCamera(state: false);
			PrepareMapIcons();
			LoadingPanel.SetActive(value: false);
			InputMapSetEvent.TrySetPreviousGroup();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BackButton.SetActive(active: false);
	}

	protected override void Start()
	{
		if (_raceModeController != null)
		{
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
		}
		_cachedIsQualification = CircuitRaceControlller.IsQualification;
		CircuitRaceControlller.Direction = 1;
		_raceMapHandler = base.transform.parent.GetComponentInChildren<InteractableRaceMapHandler>();
		if (!_raceMapHandler)
		{
			CreateRaceMapImage().Forget();
		}
		if (!FlagsStorage.IsMainMenu)
		{
			CarSelectorPanel.gameObject.SetActive(value: false);
			RaceState raceState = _circuitRaceControlller.GetRaceState();
		}
		else
		{
			Time.timeScale = 0f;
			PrepareMapIcons();
		}
		StartButton.onClick.AddListener(StartGame);
		CreateButtons();
		BackButton.onClick.AddListener(Back);
		BackClassButton.onClick.AddListener(BackToMap);
		MapDirection.onClick.AddListener(SetDirection);
		InitMapIcons();
		OnMapSelected(3);
		SetDirection();
		SetSlider();
		SetRewardsLimitText().Forget();
		CarSelectorPanel.ParseCarValues();
		CarSelectorPanel.SelectClicked += ShowSelectCarWindow;
		DelayedChangeToggle().Forget();
		base.Start();
		InitBackButton();
	}

	private void PrepareMapIcons()
	{
		if ((bool)_raceMapHandler)
		{
			_racingUIMap = _raceMapHandler.RacingUIMap;
			_raceMapHandler.InitMapElements(_racingUIMap.CircuitTrackedObjects, _racingUIMap.CircuitRoadElements);
			_racingUIMap.CurrentRoadElements[1].SetObjectState(state: false);
			_racingUIMap.CircuitPoints.SetActive(value: true);
			_direction = 1;
			_racingUIMap.gameObject.SetActive(value: true);
			_racingUIMap.ObjectRoot.gameObject.SetActive(value: true);
			if ((bool)_racingUIMap)
			{
				_racingUIMap.CircuitRaceSelectMapWindow = this;
				_racingUIMap.RaceModePoints.gameObject.SetActive(value: false);
				_racingUIMap.CircuitPoints.gameObject.SetActive(value: true);
			}
		}
	}

	private async UniTaskVoid DelayedChangeToggle()
	{
		await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		QualificationToggle.isOn = CircuitRaceControlller.IsQualification;
	}

	private async UniTask SetRewardsLimitText()
	{
		CoinsLimit.text = "0/0";
		MoneyLimit.text = "0/0";
		_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
		_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
		long circuitMoneyEarned = _playerProfileService.CircuitMoneyEarned;
		MoneyLimit.text = $"{circuitMoneyEarned}/{_remoteConfigData.raceRewardMoneyLimit}";
		CoinsLimit.text = $"{_playerProfileService.CircuitCoinsEarned}/{_remoteConfigData.raceRewardCoinsLimit}";
		bool flag = Application.internetReachability != NetworkReachability.NotReachable;
		if ((bool)CoinsLimitCanvasGroup)
		{
			CoinsLimitCanvasGroup.alpha = (flag ? 1f : 0.1f);
		}
	}

	private void SetSlider()
	{
		ApponentsCountSlider.Init(4f, 10f, 1f, isRoundValue: true);
		ApponentsCountSlider.SetTextWithoutNotify(10f);
	}

	private void InitMapIcons()
	{
		RectTransform[] array = new RectTransform[_togglesOnMap.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _togglesOnMap[i].GetComponent<RectTransform>();
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		if (FlagsStorage.IsMainMenu)
		{
			_managerUI.ShowSelectRacigModeWindow();
		}
		else
		{
			CircuitRaceControlller.IsQualification = _cachedIsQualification;
			ResetMapHandler();
			if ((bool)_circuitRaceControlller && _circuitRaceControlller.GetRaceState() == RaceState.RaceFinished)
			{
				_levelScript?.UnCleanScreen();
				_ = _managerUI.ShowUserBalanceOnlyWindow();
				LapRaceLeaderboardUIPanel lapRaceLeaderboardUI = _circuitRaceControlller.CircuitRacingUIControll.GetLapRaceLeaderboardUI();
				if ((bool)lapRaceLeaderboardUI)
				{
					lapRaceLeaderboardUI.SetActive(active: true);
				}
			}
			if (string.IsNullOrEmpty(_backWindow))
			{
				Close();
			}
			else if (_backWindow == WindowScriptType.CircuitRacePauseWindow.ToString())
			{
				_ = _managerUI.ShowWindow(_backWindow, alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			}
			else
			{
				_ = _managerUI.ShowWindow(_backWindow);
			}
		}
		Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			if (!FlagsStorage.IsMainMenu)
			{
				_racingUIMap.EnableMainCamera(state: true);
			}
			base.Close();
		}
	}

	private void ResetMapHandler()
	{
		_racingUIMap.ObjectRoot.gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(_raceMapHandler.gameObject);
	}

	private void BackToMap()
	{
		_raceMapHandler.enabled = true;
		SelectMapPanel.SetActive(value: true);
		SelectClassPanel.SetActive(value: false);
	}

	private void CreateButtons()
	{
		QualificationToggle.onValueChanged.AddListener(SetQualification);
		_togglesOnMap = new List<SelectMapUIElement>();
		_togglesUI = new List<SelectMapUIElement>();
		for (int i = 0; i < RaceConfigs.Length; i++)
		{
			SelectMapUIElement selectMapUIElement = _prefabFactory.Instantiate(SelectMapUIElementPrefab, ButtonsParent);
			string trackName = (RaceConfigs[i].IsLeman ? ((string)new LocalizedString("LapRaceLeaderboardUIPanel.LeMans")) : string.Format(new LocalizedString("LapRaceLeaderboardUIPanel.Track"), RaceConfigs[i].TrackName));
			selectMapUIElement.Init(i, trackName, this);
			SelectMapUIElement selectMapUIElement2 = _prefabFactory.Instantiate(OnMapElementPrefab, OnMapButtonsParent);
			selectMapUIElement2.Init(i, "", this);
			_togglesUI.Add(selectMapUIElement);
			_togglesOnMap.Add(selectMapUIElement2);
		}
		CWToggle.onValueChanged.AddListener(OnCWDirectionSelected);
		CCWToggle.onValueChanged.AddListener(OnCCWDirectionSelected);
	}

	private void SetQualification(bool state)
	{
		CircuitRaceControlller.IsQualification = state;
		BudgetPanelCanvasGroup.DOFade(state ? 0.1f : 1f, 0.2f).SetUpdate(isIndependentUpdate: true);
	}

	private void StartGame()
	{
		if (!FlagsStorage.IsMainMenu)
		{
			if ((bool)_levelScript && !FlagsStorage.IsMainMenu)
			{
				_levelScript.UnCleanScreen();
			}
			_circuitRaceControlller.GoGarageWithoutFinish(showMenu: false);
			_circuitRaceControlller.StartRaceOrQualification();
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
			Audio.ResetAllVolume();
			DestroyMap();
			Close();
		}
		else
		{
			VehicleEntityProvider component = V.Car.GetComponent<VehicleEntityProvider>();
			VehicleClass classVehicle = Utils.GetClassVehicle(component.CarEntity.PerformancePoints, component.GetVehicleData.CarId);
			SelectClassIndex(new AdvancedInt((int)classVehicle));
		}
	}

	public void ShowSelectClassWindow(int mapIndex)
	{
		_groupPositionChecker?.Dispose();
		_raceMapHandler.enabled = false;
		CircuitRaceControlller.CurrentMapId = mapIndex;
		SelectClassPanel.SetActive(value: true);
		int mapCount = CircuitRacingConfig.MapDatas.Length;
		List<CircuitRacingRecord> list = RaceRecords.GetTrackRecords(_playerStorageService, mapCount)[mapIndex];
		CircuitRaceConfigItem[] groups = CircuitRacingConfig.MapDatas[mapIndex].Groups;
		VehicleEntityProvider component = V.Car.GetComponent<VehicleEntityProvider>();
		VehicleClass classVehicle = Utils.GetClassVehicle(component.CarEntity.PerformancePoints, component.GetVehicleData.CarId);
		for (int i = 0; i < RaceGroups.Length; i++)
		{
			if (list.Count <= i)
			{
				list.Add(new CircuitRacingRecord
				{
					BestTime = 0f,
					ClassIndex = i,
					FirstPlace = 0,
					SecondPlace = 0,
					ThirdPlace = 0
				});
			}
			RaceGroups[i].Init(list[i], groups[i].Rewards, i, this);
			if (classVehicle != (VehicleClass)i)
			{
				RaceGroups[i].Block();
				continue;
			}
			Transform component2 = RaceGroups[i].GetComponent<Transform>();
			if (((Vector3)RectTransformUtility.WorldToScreenPoint(null, component2.position)).x >= (float)Screen.width)
			{
				GroupsScroll.DOHorizontalNormalizedPos(1f, AutoScrollingDuration).SetUpdate(isIndependentUpdate: true);
			}
		}
	}

	private void SetCurrentClassInfo()
	{
		VehicleEntityProvider component = V.Car.GetComponent<VehicleEntityProvider>();
		VehicleClass vehicleClass = Utils.GetClassVehicle(component.CarEntity.PerformancePoints, component.GetVehicleData.CarId);
		int mapCount = CircuitRacingConfig.MapDatas.Length;
		List<CircuitRacingRecord> list = RaceRecords.GetTrackRecords(_playerStorageService, mapCount)[_selectedIndex];
		CircuitRacingRecord circuitRacingRecord = list.First((CircuitRacingRecord c) => c.ClassIndex == (int)vehicleClass);
		CircuitRaceConfigItem circuitRaceConfigItem = CircuitRacingConfig.MapDatas[_selectedIndex].Groups.First((CircuitRaceConfigItem c) => c.Class == vehicleClass);
		int[] winCounts = new int[3] { circuitRacingRecord.FirstPlace, circuitRacingRecord.SecondPlace, circuitRacingRecord.ThirdPlace };
		ClassInfoView.SetBestTime(circuitRacingRecord.BestTime);
		ClassInfoView.SetRewards(circuitRaceConfigItem.Rewards, winCounts);
		RaceInfoDropdown.SetAllInfo(list);
	}

	private void ShowSelectCarWindow()
	{
		DestroyMap();
		_racingUIMap.EnableMainCamera(state: true);
		_managerUI.SetPreviousWindow("CircuitRaceSelectMapWindow");
		_managerUI.ShowCircuitCarSelectWindow();
		Close();
	}

	private void SetMapUIValues(int mapIndex)
	{
		RaceConfigSO raceConfigSO = RaceConfigs[mapIndex];
		LapsPanel.SetActive(!raceConfigSO.IsLeman);
		TimeDurationText.text = raceConfigSO.LemansMinutesCount.ToString() + new LocalizedString("AchievementsWindow.Min");
		TimeDurationPanel.SetActive(raceConfigSO.IsLeman);
		LapsCount.text = raceConfigSO.LapsCount.ToString();
		float num = raceConfigSO.LapDistance;
		if (num > 0f)
		{
			num /= 1000f;
		}
		Distance.text = $"{Utils.GetConvertedMileage(num):F1}{Utils.CurrentMetric}";
		if ((bool)DescriptionLeman)
		{
			DescriptionLeman.transform.parent.gameObject.SetActive(raceConfigSO.IsLeman);
		}
	}

	private void OnDisable()
	{
		_racingUIMap.ObjectRoot.gameObject.SetActive(value: false);
	}

	public void OnMapSelected(int mapId)
	{
		CircuitRaceControlller.CurrentMapId = mapId;
		SetMapUIValues(mapId);
		if ((bool)_racingUIMap)
		{
			_racingUIMap.SnapToTarget(mapId, withFOV: true);
			_racingUIMap.SetVisibleRoad(mapId);
		}
		_selectedIndex = mapId;
		CircuitRaceControlller.CurrentMapId = mapId;
		MapTitleText.text = (RaceConfigs[mapId].IsLeman ? ((string)new LocalizedString("LapRaceLeaderboardUIPanel.LeMans")) : string.Format(new LocalizedString("LapRaceLeaderboardUIPanel.Track"), RaceConfigs[mapId].TrackName));
		_togglesUI[mapId].SetOnWithoutNotify(isOn: true);
		_togglesOnMap[mapId].SetOnWithoutNotify(isOn: true);
		SetCurrentClassInfo();
	}

	private void SetDirection()
	{
		bool flag = _direction == -1;
		_direction = (flag ? 1 : (-1));
		DirectionText.text = (flag ? "Left" : "Right");
	}

	public void SelectClassIndex(AdvancedInt index)
	{
		CircuitRaceControlller.CurrentClassId = index;
		if (!StartCircuitRace())
		{
			_racingUIMap.EnableMainCamera(state: true);
			_raceMapHandler.Reset();
			_racingUIMap.Reset();
			UnityEngine.Object.Destroy(_raceMapHandler.gameObject);
			_managerUI.ShowUserBalanceWindow();
			_managerUI.ShowMainMenuWindow();
		}
		Close();
	}

	private bool StartCircuitRace()
	{
		if (_isBusy)
		{
			return true;
		}
		_isBusy = true;
		if ((bool)V.LastUsedCar)
		{
			CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
			if (V.LastUsedCar.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				int2 notValid = Utils.NotValid;
				component.CheckerEngineCheating(out var res);
				if (math.all(res == notValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CheatEngine, 3f, NotificationType.Error);
					return false;
				}
				component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
				if (math.all(isIncompatible == notValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
					return false;
				}
			}
			_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
			if (math.all(result == Utils.NotValid))
			{
				_managerUI.ShowCarBlockNotification(NotificationMessages.CantDriveThisCar, 3f, NotificationType.Error);
				return false;
			}
			if (component.CarEntity.GetValue().IsBroken)
			{
				_managerUI.ShowCarBlockNotification(NotificationMessages.CarIsBroken, 3f, NotificationType.Error);
				return false;
			}
			_timerTuningService.IsCarBlocked(V.SlotID).Get(out var value);
			if ((_timerTuningService != null) & value)
			{
				_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
				return false;
			}
		}
		DestroyRagdollAndHeadMove();
		FlagsStorage.RacingGameMode = true;
		V.CurrentRaceModeType = RacingModeType.CircuitRacing;
		_managerUI.ShowWaitingWindow(set: true).Forget();
		DestroyMap();
		SceneManager.LoadSceneAsync(41);
		return true;
	}

	private void DestroyMap()
	{
		_racingUIMap.Reset();
		_raceMapHandler.Reset();
		UnityEngine.Object.Destroy(_raceMapHandler.gameObject);
	}

	private void DestroyRagdollAndHeadMove()
	{
		DriverActionController component = V.DriverPerson.GetComponent<DriverActionController>();
		if ((bool)component && (bool)component.PersonHeadMove)
		{
			UnityEngine.Object.Destroy(component.PersonHeadMove);
		}
	}

	protected override void OnDestroy()
	{
		_isDestroyed = true;
		if (FlagsStorage.IsMainMenu)
		{
			Time.timeScale = 1f;
		}
		CarSelectorPanel.SelectClicked -= ShowSelectCarWindow;
		base.OnDestroy();
	}
}
