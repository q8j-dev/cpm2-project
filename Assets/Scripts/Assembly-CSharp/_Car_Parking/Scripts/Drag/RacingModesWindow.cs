using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.RaceInfo;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drag
{
	public class RacingModesWindow : WindowScript, IBackableWindow
	{
		private readonly Color _startButtonColor = new Color(0.76f, 0.04f, 0f);

		private readonly Color _selectButtonColor = new Color(0.45f, 0.56f, 0.6f);

		private readonly Color _selectRallyColor = new Color(1f, 0.7294f, 0f);

		[Header("Drag racing")]
		[SerializeField]
		private GameObject DragRacingStatusStatistics = default;

		[SerializeField]
		private GameObject AutoDragClassesPanel = default;

		[SerializeField]
		private GameObject ManualDragClassesPanel = default;

		[SerializeField]
		private Toggle AutomaticDragModeToggle = default;

		[SerializeField]
		private Toggle ManualDragModeToggle = default;

		[SerializeField]
		private List<RaceWonPlacesItem> DragClassRecordsUis = default;

		[SerializeField]
		private List<RaceWonPlacesItem> ManualDragClassRecordsUis = default;

		[Header("Circuit racing")]
		[SerializeField]
		private GameObject CircuitRacingStatusStatistics = default;

		[SerializeField]
		private List<RaceWonPlacesItem> CircuitRaceWonPlacesItems = default;

		[Header("Other")]
		[SerializeField]
		private Image RaceModeImage = default;

		[SerializeField]
		private TextMeshProUGUI RacingTitleText = default;

		[SerializeField]
		private TextMeshProUGUI RacingModeDescriptionText = default;

		[SerializeField]
		private Toggle CircuitRacingModeToggle = default;

		[SerializeField]
		private Toggle DragRacingModeToggle = default;

		[SerializeField]
		private Toggle RallyRacingModeToggle = default;

		[SerializeField]
		private NewMarkIcon RallyNewMarkIcon = default;

		[SerializeField]
		private Button StartButton = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private Sprite[] Sprites = default;

		[SerializeField]
		private RectTransform RightPanlelRectTransform = default;

		[SerializeField]
		private CarSelectorPanel CarSelectorPanel = default;

		[SerializeField]
		private CircuitRacingConfig CircuitRacingConfig = default;

		[SerializeField]
		private GameObject LoadingPanel = default;

		private ITimerTuningService _timerTuningService;

		private ICarInfoService _carInfoService;

		private AllCarsInfo _allCarsInfo;

		private RacingMode _racingMode;

		private IPlayerStorageService _playerStorageService;

		private bool _isBusy;

		private RacingUIMap _racingUIMap;

		private bool _isCheckIcons;

		private InteractableRaceMapHandler _raceMapHandler;

		private IPrefabFactory _prefabFactory;

		private bool _isDestroyed;

		private InputMapSetEvent _inputMapSetEvent;

		[Inject]
		public void Construct(ITimerTuningService timerTuningService, ICarInfoService carInfoService, AllCarsInfo allCarsInfo, IPlayerStorageService playerStorageService, IPrefabFactory prefabFactory, InputMapSetEvent inputMapSetEvent)
		{
			_timerTuningService = timerTuningService;
			_carInfoService = carInfoService;
			_allCarsInfo = allCarsInfo;
			_playerStorageService = playerStorageService;
			_prefabFactory = prefabFactory;
			_inputMapSetEvent = inputMapSetEvent;
		}

		private async UniTaskVoid CreateRaceMapImage()
		{
			LoadingPanel.SetActive(value: true);
			InteractableRaceMapHandler component = (await Addressables.LoadAssetAsync<GameObject>("InteractableRaceMapHandler").BindTo(V.CoreGameManager.ManagerUI.gameObject)).GetComponent<InteractableRaceMapHandler>();
			if (!_isDestroyed)
			{
				_raceMapHandler = _prefabFactory.Instantiate(component, _managerUI.GetWindowsParent());
				PrepareMapTouch();
				_inputMapSetEvent.TrySetGroup(InputGroupsEnum.RacingMap);
				LoadingPanel.SetActive(value: false);
			}
		}

		private void PrepareMapTouch()
		{
			_racingUIMap = _raceMapHandler.RacingUIMap;
			_racingUIMap.RacingModesWindow = this;
			_raceMapHandler.gameObject.transform.SetSiblingIndex(0);
			_racingUIMap.EnableMainCamera(state: false);
			_racingUIMap.ObjectRoot.gameObject.SetActive(value: true);
			_racingUIMap.RaceModePoints.SetActive(value: true);
			_raceMapHandler.InitMapElements(_racingUIMap.RacingTrackedObjects, _racingUIMap.RacingRoadElements);
			_racingUIMap.SnapToTarget(1, withFOV: true);
			InitIcons();
		}

		protected override void Start()
		{
			InitBackButton();
			base.Start();
			Time.timeScale = 0f;
			_raceMapHandler = base.transform.parent.GetComponentInChildren<InteractableRaceMapHandler>();
			if (!_raceMapHandler)
			{
				CreateRaceMapImage().Forget();
			}
			else
			{
				PrepareMapTouch();
			}
			SetUIElements(RacingMode.RacingTracks);
			BackButton.onClick.AddListener(Back);
			Init();
			_isCheckIcons = true;
			SubscribeListeners();
			CarSelectorPanel.ParseCarValues();
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component))
			{
				AutomaticDragModeToggle.isOn = component.IsAutomaticGearbox;
				ManualDragModeToggle.isOn = !component.IsAutomaticGearbox;
			}
			if ((bool)_racingUIMap)
			{
				_racingUIMap.RacingModesWindow = this;
			}
			if (_managerUI.IsRacingCarSelect() || FlagsStorage.IsMainMenu)
			{
				SelectCircuitRacingMode(state: true);
			}
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
		}

		protected override void OnEnable()
		{
			BackButton.gameObject.SetActive(value: false);
			base.OnEnable();
		}

		private void InitIcons()
		{
			if ((bool)_racingUIMap)
			{
				_racingUIMap.RaceModePoints.gameObject.SetActive(value: true);
				_racingUIMap.CircuitPoints.gameObject.SetActive(value: false);
			}
		}

		private void SubscribeListeners()
		{
			StartButton.onClick.AddListener(RunGame);
			CircuitRacingModeToggle.onValueChanged.AddListener(SelectCircuitRacingMode);
			DragRacingModeToggle.onValueChanged.AddListener(SelectDragRacingMode);
			RallyRacingModeToggle.onValueChanged.AddListener(SelectRallyRacingMode);
			AutomaticDragModeToggle.onValueChanged.AddListener(SelectAutomaticGear);
			ManualDragModeToggle.onValueChanged.AddListener(SelectManualGear);
			CarSelectorPanel.SelectClicked += ShowSelectCarWindow;
		}

		private void ShowSelectCarWindow()
		{
			DestroyMap();
			_racingUIMap.EnableMainCamera(state: true);
			_managerUI.SetPreviousWindow("SelectRaceModeWindow");
			_managerUI.ShowCircuitCarSelectWindow();
			Close();
		}

		private void OnDisable()
		{
			if ((bool)_racingUIMap && (bool)_racingUIMap.ObjectRoot)
			{
				_racingUIMap.ObjectRoot.gameObject.SetActive(value: false);
			}
		}

		public void Init()
		{
			CalculateDragRacingRecords();
			CalculateManualDragRacingRecords();
			CalculateCircuitRacingRecords();
		}

		public void SelectRacingModeToggle(RacingMode racingMode)
		{
			(racingMode switch
			{
				RacingMode.DragRacing => DragRacingModeToggle, 
				RacingMode.Rally => RallyRacingModeToggle, 
				_ => CircuitRacingModeToggle, 
			}).isOn = true;
		}

		public void SelectRallyRacingMode(bool state)
		{
			if (state)
			{
				SetUIElements(RacingMode.Rally);
				CircuitRacingStatusStatistics.SetActive(value: false);
				DragRacingStatusStatistics.SetActive(value: false);
				CarSelectorPanel.gameObject.SetActive(value: false);
				V.CurrentRaceModeType = RacingModeType.Rally;
				_racingMode = RacingMode.Rally;
				if ((bool)_racingUIMap)
				{
					_racingUIMap.SnapToTarget(2);
					_racingUIMap.SetVisibleRoad(2);
				}
			}
		}

		public void SelectDragRacingMode(bool state)
		{
			if (state)
			{
				CarSelectorPanel.gameObject.SetActive(value: true);
				DragRacingModeToggle.SetIsOnWithoutNotify(value: true);
				DragRacingStatusStatistics.SetActive(value: true);
				CircuitRacingStatusStatistics.SetActive(value: false);
				SetUIElements(RacingMode.DragRacing);
				_racingUIMap.SnapToTarget(0);
				_racingUIMap.SetVisibleRoad(0);
			}
		}

		public void SelectCircuitRacingMode(bool state)
		{
			if (state)
			{
				CarSelectorPanel.gameObject.SetActive(value: true);
				CircuitRacingModeToggle.SetIsOnWithoutNotify(value: true);
				DragRacingStatusStatistics.SetActive(value: false);
				CircuitRacingStatusStatistics.SetActive(value: true);
				SetUIElements(RacingMode.RacingTracks);
				if ((bool)_racingUIMap)
				{
					_racingUIMap.SnapToTarget(1);
					_racingUIMap.SetVisibleRoad(1);
				}
			}
		}

		private void SelectAutomaticGear(bool isOn)
		{
			if (isOn)
			{
				ShowManualDragClassesPanel(isActive: false);
			}
		}

		private void SelectManualGear(bool isOn)
		{
			if (isOn)
			{
				ShowManualDragClassesPanel(isActive: true);
			}
		}

		private void ShowManualDragClassesPanel(bool isActive)
		{
			ManualDragClassesPanel.SetActive(isActive);
			AutoDragClassesPanel.SetActive(!isActive);
		}

		private void SetUIElements(RacingMode racingMode)
		{
			UIHelper.DelayedRebuildLayout(RightPanlelRectTransform).Forget();
			_racingMode = racingMode;
			switch (racingMode)
			{
			case RacingMode.DragRacing:
				RaceModeImage.sprite = Sprites[0];
				RacingTitleText.text = new LocalizedString("Selectracemodewindow.DragRacing");
				RacingModeDescriptionText.text = new LocalizedString("Selectracemodewindow.DragRacingDescription");
				StartButton.image.color = _startButtonColor;
				break;
			case RacingMode.RacingTracks:
				RaceModeImage.sprite = Sprites[1];
				RacingTitleText.text = new LocalizedString("Selectracemodewindow.RacingTracks");
				RacingModeDescriptionText.text = new LocalizedString("Selectracemodewindow.CircuitRacingDescription");
				StartButton.image.color = _selectButtonColor;
				break;
			case RacingMode.Rally:
				RaceModeImage.sprite = Sprites[2];
				RacingTitleText.text = new LocalizedString("Selectracemodewindow.Rally");
				RacingModeDescriptionText.text = new LocalizedString("FreeDragRacingWindow.RallyDescription");
				StartButton.image.color = _selectRallyColor;
				break;
			}
		}

		private void DestroyMap()
		{
			_raceMapHandler.Reset();
			_racingUIMap.Reset();
			Object.Destroy(_raceMapHandler.gameObject);
		}

		private void RunGame()
		{
			if (math.all(CheckCar() == Utils.NotValid))
			{
				return;
			}
			if (_racingMode == RacingMode.RacingTracks)
			{
				_managerUI.ShowCircuitMapWindow();
				Close();
				return;
			}
			DestroyMap();
			if (_isBusy)
			{
				return;
			}
			_isBusy = true;
			DestroyRagdollAndHeadMove();
			if (_racingMode == RacingMode.DragRacing)
			{
				if ((bool)V.DriverPerson)
				{
					Object.DontDestroyOnLoad(V.DriverPerson);
				}
				if ((bool)V.Car)
				{
					Object.DontDestroyOnLoad(V.Car);
				}
			}
			FlagsStorage.RacingGameMode = true;
			if (_racingMode == RacingMode.Rally)
			{
				StartButton.interactable = false;
				ShowRallyMenuWindowAsync().Forget();
				return;
			}
			_managerUI.ShowWaitingWindow(set: true).Forget();
			SceneManager.LoadSceneAsync((_racingMode == RacingMode.DragRacing) ? 32 : 41);
			if (_racingMode == RacingMode.Rally)
			{
				RallyNewMarkIcon.HideAndSave();
			}
			Close();
		}

		private async UniTask ShowRallyMenuWindowAsync()
		{
			await _managerUI.ShowWaitingWindowNotTransparent(set: true, backButtonState: false, alwaysOnTop: true, updateBackButton: false);
			_managerUI.ShowSelectRallyLocationsMenuWindow();
			Close();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_isDestroyed = true;
			Time.timeScale = 1f;
		}

		private int2 CheckCar()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out var component) && component.CarEntity.GetValue().MaintenanceData.FuelInTank < 10f)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("FuelTankEntity.RefuelCar"), 2f);
				return Utils.NotValid;
			}
			if ((bool)V.LastUsedCar)
			{
				CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
				if (V.LastUsedCar.TryGetComponent<VehicleEntityProvider>(out component))
				{
					component.CheckerEngineCheating(out var res);
					if (math.all(res == Utils.NotValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.CheatEngine, 3f, NotificationType.Error);
						return Utils.NotValid;
					}
					component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
					if (math.all(isIncompatible == Utils.NotValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
						return Utils.NotValid;
					}
				}
				_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
				if (math.all(result == Utils.NotValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CantDriveThisCar, 3f, NotificationType.Error);
					return Utils.NotValid;
				}
				if (component.CarEntity.GetValue().IsBroken)
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarIsBroken, 3f, NotificationType.Error);
					return Utils.NotValid;
				}
				if (_timerTuningService.IsCarBlocked(V.SlotID))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
					return Utils.NotValid;
				}
			}
			return Utils.Valid;
		}

		private void DestroyRagdollAndHeadMove()
		{
			DriverActionController component = V.DriverPerson.GetComponent<DriverActionController>();
			if ((bool)component && (bool)component.PersonHeadMove)
			{
				Object.Destroy(component.PersonHeadMove);
			}
		}

		private void CalculateDragRacingRecords()
		{
			PlayerStorage data = _playerStorageService.Data;
			if (data.RacingProgress == null)
			{
				data.RacingProgress = new RacingProgress();
			}
			foreach (RacingClassRecord classRecord in _playerStorageService.Data.RacingProgress.RacingClassRecords ?? new List<RacingClassRecord>())
			{
				if (classRecord != null)
				{
					RaceWonPlacesItem raceWonPlacesItem = DragClassRecordsUis.Find((RaceWonPlacesItem dragClass) => dragClass.TimeClass == classRecord.TimeClass);
					if ((bool)raceWonPlacesItem)
					{
						raceWonPlacesItem.SetPlacesCount(classRecord.FirstPlace, classRecord.SecondPlace, classRecord.ThirdPlace);
					}
				}
			}
		}

		private void CalculateManualDragRacingRecords()
		{
			PlayerStorage data = _playerStorageService.Data;
			if (data.RacingProgress == null)
			{
				data.RacingProgress = new RacingProgress();
			}
			foreach (RacingClassRecord classRecord in _playerStorageService.Data.RacingProgress.ManualRacingClassRecords ?? new List<RacingClassRecord>())
			{
				if (classRecord != null)
				{
					RaceWonPlacesItem raceWonPlacesItem = ManualDragClassRecordsUis.Find((RaceWonPlacesItem dragClass) => dragClass.TimeClass == classRecord.TimeClass);
					if ((bool)raceWonPlacesItem)
					{
						raceWonPlacesItem.SetPlacesCount(classRecord.FirstPlace, classRecord.SecondPlace, classRecord.ThirdPlace);
					}
				}
			}
		}

		private void CalculateCircuitRacingRecords()
		{
			int mapCount = CircuitRacingConfig.MapDatas.Length;
			Dictionary<int, List<CircuitRacingRecord>> trackRecords = RaceRecords.GetTrackRecords(_playerStorageService, mapCount);
			for (int i = 0; i < CircuitRaceWonPlacesItems.Count; i++)
			{
				RaceWonPlacesItem wonPlacesItem = CircuitRaceWonPlacesItems[i];
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (List<CircuitRacingRecord> value in trackRecords.Values)
				{
					if (value.Count <= i)
					{
						value.Add(new CircuitRacingRecord
						{
							BestTime = 0f,
							ClassIndex = i,
							FirstPlace = 0,
							SecondPlace = 0,
							ThirdPlace = 0
						});
					}
					CircuitRacingRecord circuitRacingRecord = value.First((CircuitRacingRecord r) => r.ClassIndex == wonPlacesItem.TimeClass);
					num += circuitRacingRecord.FirstPlace;
					num2 += circuitRacingRecord.SecondPlace;
					num3 += circuitRacingRecord.ThirdPlace;
				}
				wonPlacesItem.SetPlacesCount(num, num2, num3);
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_racingUIMap.EnableMainCamera(state: true);
			DestroyMap();
			_managerUI.SetCarSelect(active: false);
			_managerUI.ShowGameModeWindow();
			Close();
		}
	}
}
