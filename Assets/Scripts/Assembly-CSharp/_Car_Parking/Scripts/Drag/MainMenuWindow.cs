using System;
using System.Collections.Generic;
using System.Threading;
using CPM.DailyTasks;
using CPM.Database;
using CPM.RemoteConfig;
using CPM.Scripts.Garage;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Racing.CameraManagement;
using Steamworks.Data;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CPMRating.AchievementsService;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drag
{
	public class MainMenuWindow : WindowScript, IBackableWindow
	{
		public const string NeedToUpdateShowKey = "NeedToUpdateShow";

		public Button ExitButton;

		public Button StartButton;

		public Button ParkingButton;

		public Button DriverSelectButton;

		public Transform DriverNotice;

		public Button InstagramButton;

		public Button DailyRewardsButton;

		public Button ActivitiesButton;

		[FormerlySerializedAs("RealtyButton")]
		public Button PropertyButton;

		public SocialsConfig SocialsConfig;

		private SocialsConfig.SocialData _currentSocial;

		private ITimerTuningService _timerTuningService;

		private IAutoSaveService _autoSaveService;

		private INotificationService _notificationService;

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		[SerializeField]
		private DailyTaskWindowManager DailyTaskWindowManager;

		[SerializeField]
		private Button NeedToUpdateButton;

		[SerializeField]
		private GameObject RealtyBacklight;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		private CurrencyManagerController _currencyManagerController;

		private IEngineTuningTimerService _engineTuningTimerService;

		private IRemoteConfigManager _remoteConfigManager;

		private MenuControl _menuControl;

		private ISlotService _slotService;

		private CarSelectorByTouch _carSelector;

		private IAchievementsService _achievementsService;

		private DataBase _dataBase;

		private ICarInfoService _carInfoService;

		private GarageController _garageController;

		private AllCarsInfo _allCarsInfo;

		private ISteamAchievementService _steamAchievementService;

		private ISteamPurchaseRestoreService _steamPurchaseRestoreService;

		private EventRegistry _eventRegistry;

		private CameraManager _cameraManager;

		private CoreGameManager _coreGameManager;

		private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

		protected override bool ChangeRootGameInputGroup => true;

		protected override void Awake()
		{
			base.Awake();
			StartButton.onClick.AddListener(ShowGameModes);
			ParkingButton.onClick.AddListener(LoadParking);
			DriverSelectButton.onClick.AddListener(LoadDriver);
			DailyRewardsButton.onClick.AddListener(OpenDailyRewards);
			InstagramButton.onClick.AddListener(() =>
			{
				InstagramClick(_currentSocial.prefsKey, _currentSocial.url);
			});
			PropertyButton.onClick.AddListener(OpenProperty);
		}

		public void Back()
		{
			_managerUI.ShowSettingsWindow();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		[Inject]
		public void Construct(ITimerTuningService timerTuningService, IAutoSaveService autoSaveService, CurrencyManagerController currencyManagerController, IEngineTuningTimerService engineTuningTimerService, IRemoteConfigManager remoteConfigManager, ISlotService slotService, MenuControl menuControl, CarSelectorByTouch carSelector, INotificationService notificationService, ICharacterEquipmentDataService characterEquipmentDataService, IAchievementsService achievementsService, DataBase dataBase, ISteamAchievementService steamAchievementService, ISteamPurchaseRestoreService steamPurchaseRestoreService, GarageController garageController, AllCarsInfo allCarsInfo, EventRegistry eventRegistry, ICarInfoService carInfoService, CameraManager cameraManager, CoreGameManager coreGameManager)
		{
			_achievementsService = achievementsService;
			_menuControl = menuControl;
			_remoteConfigManager = remoteConfigManager;
			_engineTuningTimerService = engineTuningTimerService;
			_currencyManagerController = currencyManagerController;
			_autoSaveService = autoSaveService;
			_timerTuningService = timerTuningService;
			_slotService = slotService;
			_carSelector = carSelector;
			_notificationService = notificationService;
			_characterEquipmentDataService = characterEquipmentDataService;
			_dataBase = dataBase;
			_steamAchievementService = steamAchievementService;
			_steamPurchaseRestoreService = steamPurchaseRestoreService;
			_garageController = garageController;
			_allCarsInfo = allCarsInfo;
			_eventRegistry = eventRegistry;
			_carInfoService = carInfoService;
			_cameraManager = cameraManager;
			_coreGameManager = coreGameManager;
		}

		protected override void OnEnable()
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			_managerUI.OnSettingsButtonActivated?.Invoke(obj: true);
			_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: true);
			_managerUI.SetBackground?.Invoke(obj: true);
			_autoSaveService.TrySendSaveData(AutoSaveService.AutoSaveFrom.MainMenu);
			int num = ObscuredPrefs.GetInt("GarageButtonImportantClicks");
			RealtyBacklight.SetActive(num == 1 && _slotService.OccupiedSlotCount == _slotService.MaxSlotCount);
			_carSelector.enabled = true;
			ActivitiesButton.onClick.AddListener(OnActivitiesButtonClicked);
			OnEnableCanvasGroup().Forget();
			Debug.Log("enabled this main menu", this);
			base.OnEnable();
			_onThrottleHandle.Get(_eventRegistry).Subscribe(RideCarForSubscription);
		}

		private void OnDisable()
		{
			ActivitiesButton.onClick.RemoveListener(OnActivitiesButtonClicked);
			_managerUI.OnSettingsButtonActivated?.Invoke(obj: false);
			_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: false);
			_carSelector.enabled = false;
			if ((bool)CanvasGroup)
			{
				CanvasGroup.alpha = 0f;
			}
			_onThrottleHandle.Get(_eventRegistry)?.Unsubscribe(RideCarForSubscription);
		}

		protected override void Start()
		{
			base.Start();
			_managerUI.ClearWindows();
			_managerUI.OnBackButtonActivated?.Invoke(obj: false);
			_managerUI.IncreaseTimerButtonShow?.Invoke(obj: true);
			UpdateButtons();
			_timerTuningService.IsPause = false;
			_engineTuningTimerService.IsOnPause = false;
			DailyTaskWindowManager.ShowCachedDailyTasks();
			ShowNeedToUpdateButton().Forget();
			DriverNotice.gameObject.SetActive(_notificationService.HasNotice(_characterEquipmentDataService.Gender) && _notificationService.HasNotification);
			TryShowPlayerReviewWindow(base.destroyCancellationToken).Forget();
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			InitBackButton();
			CheckAchievmentsProgress().Forget();
			TryShowUnrestoredPurchaseMessage();
		}

		private void RideCarForSubscription(bool isStarted)
		{
			if (isStarted)
			{
				if (_coreGameManager.HasFirstCar())
				{
					RideCarInMenuGarage();
				}
				else
				{
					OnFreeWalk();
				}
			}
		}

		private void OnFreeWalk()
		{
			_managerUI.ShowFreeWalkWindow();
			FlagsStorage.FreeWalk = true;
			_cameraManager.ActivateCamera(RacingCameraType.None);
		}

		private void RideCarInMenuGarage()
		{
			if (_garageController.IsSwitchingCar)
			{
				return;
			}
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
						return;
					}
					component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
					if (math.all(isIncompatible == notValid))
					{
						_managerUI.ShowCarBlockNotification(NotificationMessages.IncompatibleParts, 3f, NotificationType.Error, partsLine);
						return;
					}
				}
				_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
				if (math.all(result == Utils.NotValid))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CantDriveThisCar, 3f, NotificationType.Error);
					return;
				}
				if (component.CarEntity.GetValue().IsBroken)
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarIsBroken, 3f, NotificationType.Error);
					return;
				}
				if (_timerTuningService.IsCarBlocked(V.SlotID))
				{
					_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
					return;
				}
			}
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 1f);
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 1f);
			_managerUI.ShowMenuGarageControllerHUD();
			_managerUI.OnActivateBalancePanel?.Invoke(obj: false);
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component2))
			{
				component2.ResetTransmission();
				component2.StartEngine(starterSound: true);
			}
			Close();
		}

		private void TryShowUnrestoredPurchaseMessage()
		{
			if (_steamPurchaseRestoreService != null && _steamPurchaseRestoreService.HasUnrestoredPurchaseNoSlot)
			{
				_steamPurchaseRestoreService.ClearUnrestoredPurchaseFlag();
				_managerUI.ShowConfirmWindow(null, string.Empty, new LocalizedString("Steampurchase.RestoredPurchase"), null, string.Empty, null, new ConfirmWindow.AdditionalData
				{
					CancelButtonVisible = false
				}).Forget();
			}
		}

		private async UniTaskVoid CheckAchievmentsProgress()
		{
			CheckAllAchievmentsProgress();
		}

		private void CheckAllAchievmentsProgress()
		{
			IEnumerable<Achievement> enumerable = _steamAchievementService?.GetAchievements();
			if (enumerable == null)
			{
				return;
			}
			List<Achievement> list = new List<Achievement>();
			foreach (Achievement item in enumerable)
			{
				if (!item.State)
				{
					list.Add(item);
				}
			}
			if (list.Count == 1 && list[0].Identifier == "ACH_STEAM_JOURNEY")
			{
				_steamAchievementService?.Unlock("ACH_STEAM_JOURNEY");
			}
		}

		private async UniTask TryShowPlayerReviewWindow(CancellationToken cancellationToken)
		{
		}

		private async UniTask OnEnableCanvasGroup()
		{
			await UniTask.Yield();
			await UniTask.Yield();
			if ((bool)this && (bool)CanvasGroup)
			{
				CanvasGroup.alpha = 1f;
			}
		}

		private void LoadParking()
		{
			_managerUI.ShowGarageWindow();
			Close();
		}

		private void OpenProperty()
		{
			_managerUI.ShowPropertyWindow();
		}

		private void ShowGameModes()
		{
			_managerUI.ShowGameModeWindow();
			Close();
		}

		private void LoadDriver()
		{
			_menuControl.ShowSelectDriver(JsonConvert.SerializeObject(new PersonWindowAdditionalData
			{
				ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu
			}));
			Close();
		}

		private void UpdateButtons()
		{
			_currentSocial = null;
			foreach (SocialsConfig.SocialData social in SocialsConfig.socials)
			{
				if (!ButtonWasPressed(social.prefsKey))
				{
					_currentSocial = social;
					break;
				}
			}
			if (_currentSocial != null)
			{
				InstagramButton.gameObject.SetActive(value: true);
				InstagramButton.GetComponentInChildren<RawImage>().texture = _currentSocial.buttonTexture;
			}
			else
			{
				InstagramButton.gameObject.SetActive(value: false);
			}
		}

		private static bool ButtonWasPressed(string key)
		{
			return PlayerPrefs.GetInt(key, 0) == 1;
		}

		public void InstagramClick(string prefsKey, string url)
		{
			PlayerPrefs.SetInt(prefsKey, 1);
			AddMoney();
			Application.OpenURL(url);
			base.gameObject.SetActive(value: false);
			Observable.Timer(TimeSpan.FromSeconds(SocialsConfig.timeToShowButtonAgain)).Subscribe((long _) =>
			{
				UpdateButtons();
			}).AddTo(this);
		}

		private void AddMoney()
		{
			_currencyManagerController.AddMoney(new AdvancedInt(SocialsConfig.reward));
		}

		private void OpenDailyRewards()
		{
			_menuControl.OpenDailyRewardsMenu();
		}

		private async UniTask ShowNeedToUpdateButton()
		{
			_remoteConfigManager.GetDefaultConfigStorage();
			ConfigValuesStorage configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage();
			if (configValuesStorage.needToUpdateGame == 2)
			{
				NeedToUpdateButton.SetActive(active: true);
			}
			else if (configValuesStorage.needToUpdateGame == 3 && PlayerPrefs.GetInt("NeedToUpdateShow", 1) == 1)
			{
				NeedToUpdateButton.SetActive(active: true);
				NeedToUpdateButton.onClick.AddListener(() =>
				{
					NeedToUpdateButton.SetActive(active: false);
				});
			}
		}

		private void OnActivitiesButtonClicked()
		{
			_managerUI.CloseBalanceWindow();
			_managerUI.ShowWindow("PhoneActivityWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(new PhoneGameActivityWindow.AdditionalData
			{
				OpenSource = PhoneGameActivityWindow.OpenSourceType.MainMenu
			})).Forget();
			base.Close();
		}
	}
}
