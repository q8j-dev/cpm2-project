using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using Service;
using Service.Abstract;
using Service.Cargo;
using Service.Delivery;
using Service.Manager;
using Service.Taxi;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.NewUI;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneWindow : WindowScript, IBackableWindow
	{
		public class AdditionalData
		{
			public bool FadeIn;
		}

		private const Ease NotificationEase = Ease.OutBack;

		private const float Duration = 0.5f;

		private const float NotificationDefaultYPos = 450f;

		private const float NotificationWaitTime = 3f;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		[SerializeField]
		[Header("Phone buttons")]
		private Button Taxi;

		[SerializeField]
		private Button Delivery;

		[SerializeField]
		private Button Cargo;

		[SerializeField]
		private Button Drone;

		[SerializeField]
		private Button Draggy;

		[SerializeField]
		private Button DailyTasks;

		[SerializeField]
		private Button Bus;

		[SerializeField]
		private Button PhotoMode;

		[SerializeField]
		private Button Suspension;

		[SerializeField]
		private Button Activity;

		[Header("Other buttons")]
		[SerializeField]
		private Button Accept;

		[SerializeField]
		private Button Cancel;

		[SerializeField]
		private Button Reset;

		[SerializeField]
		private Button CloseButton;

		[Header("Texts")]
		[SerializeField]
		private TextMeshProUGUI AcceptText;

		[SerializeField]
		private TextMeshProUGUI AppName;

		[SerializeField]
		private TextMeshProUGUI TimeText;

		[SerializeField]
		private TextMeshProUGUI DateText;

		[SerializeField]
		private TextMeshProUGUI AppNameInNotification;

		[SerializeField]
		private TextMeshProUGUI NotificationText;

		[Header("Images")]
		[SerializeField]
		private Image AppTopImage;

		[SerializeField]
		private Image AppIconInNotification;

		[Header("App view elements to hide/show")]
		[SerializeField]
		private GameObject AppView;

		[SerializeField]
		private GameObject ServiceColumns;

		[SerializeField]
		private GameObject DraggyColumns;

		[SerializeField]
		private GameObject Background;

		[Header("Accept button state change sprites")]
		[SerializeField]
		private Sprite NormalAccept;

		[SerializeField]
		private Sprite DisabledAccept;

		[Header("App view header background sprites")]
		[SerializeField]
		private Sprite TaxiSprite;

		[SerializeField]
		private Sprite DeliverySprite;

		[SerializeField]
		private Sprite CargoSprite;

		[SerializeField]
		private Sprite DraggySprite;

		[Header("App Icons")]
		[SerializeField]
		private Sprite TaxiIcon;

		[SerializeField]
		private Sprite DeliveryIcon;

		[SerializeField]
		private Sprite CargoIcon;

		[SerializeField]
		private Sprite DailyTasksIcon;

		[SerializeField]
		private Sprite CarControlIcon;

		[SerializeField]
		private Sprite DroneIcon;

		[SerializeField]
		[Header("Prefabs")]
		private ServiceOrderUI ServiceOrderUIPrefab;

		[SerializeField]
		private DraggyUIElement DraggyUIElementPrefab;

		[Header("Transforms")]
		[SerializeField]
		private RectTransform Notification;

		[SerializeField]
		private Transform OrderParent;

		private Tween _tween;

		[Header("FullScreenPanels")]
		[SerializeField]
		private CarControlUIPanel СarControlUIPanel;

		[SerializeField]
		private PhoneHorizontalVerticalRotator PhoneHorizontalVerticalRotator;

		[SerializeField]
		[Tooltip("Faded from zero up to their authored alpha when the window is opened with FadeIn.")]
		[Header("Fade in")]
		private Fader[] FadeInFaders;

		[SerializeField]
		private float FadeDuration = 0.3f;

		private bool _isOpeningPhotoMode;

		private bool _notificationShowed;

		private List<ServiceOrderUI> _orders = new List<ServiceOrderUI>();

		private CoreGameManager _coreGameManager;

		private Service.Taxi.ServiceController _taxiServiceController;

		private Service.Delivery.ServiceController _deliveryServiceController;

		private Service.Cargo.ServiceController _cargoServiceController;

		private DraggyUIElement _draggyUIElement;

		private MenuLevelParent _menuLevelParent;

		private InputItemsProvider _controlItemsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private LevelsProvider _levelsProvider;

		private ParkingMenuUI _parkingMenuUI;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		private MainPerson _mainPerson;

		private GameObject _appsPanel;

		private bool _isDestroyed;

		[field: SerializeField]
		public RawImage Map { get; private set; }

		public TYPE ServiceType { get; private set; }

		public int CurrentTask { get; private set; }

		public bool IsMapDrag
		{
			get
			{
				return _freeDriveProvider.MapCamera.beganTouching;
			}
			set
			{
				_freeDriveProvider.MapCamera.beganTouching = value;
			}
		}

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, OptionalDependency<ParkingMenuUI> parkingMenuUI)
		{
			_levelsProvider = levelsProvider;
			_freeDriveProvider = freeDriveProvider;
			_controlItemsProvider = controlItemsProvider;
			_menuLevelParent = menuLevelParent;
			_coreGameManager = coreGameManager;
			_parkingMenuUIDep = parkingMenuUI;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (!string.IsNullOrEmpty(data))
			{
				AdditionalData additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
				if (additionalData != null && additionalData.FadeIn)
				{
					FadeIn();
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_parkingMenuUI = _parkingMenuUIDep?.Value;
		}

		protected override void Start()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ChangeBalancePanelActiveState?.Invoke(obj: false);
				DailyTasks.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Daily Task", DailyTasksIcon);
				});
				Taxi.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Taxi", TaxiIcon);
				});
				Delivery.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Delivery", DeliveryIcon);
				});
				Cargo.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Cargo", CargoSprite);
				});
				Draggy.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Draggy", DraggySprite);
				});
				Drone.onClick.AddListener(() =>
				{
					ShowNotAvalibleInGarage("Drone", DroneIcon);
				});
			}
			else
			{
				_taxiServiceController = (Service.Taxi.ServiceController)_taskManager.TaxiManager.serviceController;
				_deliveryServiceController = (Service.Delivery.ServiceController)_taskManager.DeliveryManager.serviceController;
				_cargoServiceController = (Service.Cargo.ServiceController)_taskManager.CargoManager.serviceController;
				_taxiServiceController.PhoneWindow = this;
				_deliveryServiceController.PhoneWindow = this;
				_cargoServiceController.PhoneWindow = this;
				DailyTasks.onClick.AddListener(OpenDailyTasks);
				Taxi.onClick.AddListener(OpenTaxi);
				Delivery.onClick.AddListener(OpenDelivery);
				Cargo.onClick.AddListener(OpenCargo);
				Bus.onClick.AddListener(OpenBus);
				Draggy.onClick.AddListener(OpenDraggy);
				Drone.onClick.AddListener(OpenDrone);
				Accept.onClick.AddListener(StartTask);
				Cancel.onClick.AddListener(CancelTask);
				Reset.onClick.AddListener(ResetDraggy);
			}
			CloseButton.onClick.AddListener(CloseWindow);
			Suspension.onClick.AddListener(ShowSuspension);
			PhotoMode.onClick.AddListener(OpenPhotoMode);
			Activity.onClick.AddListener(OpenActivity);
			SetDateTime();
			SetMap();
			Map.gameObject.SetActive(value: false);
			CloseOtherUI();
			if (!FlagsStorage.IsMultiFree && !V.CoreGameManager.ManagerUI.GetWindow("WaitingWindow"))
			{
				Time.timeScale = 0f;
			}
			_taskManager.OnFinish += OnFinish;
			_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
			_mainPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
			_appsPanel = Taxi.transform.parent.gameObject;
			base.Start();
			InitBackButton();
		}

		public void Back()
		{
			if (AppView.activeSelf)
			{
				CloseAppView();
				ReRegisterBackButton().Forget();
			}
			else
			{
				CloseWindow();
			}
		}

		private async UniTaskVoid ReRegisterBackButton()
		{
			await UniTask.Yield();
			if ((bool)this)
			{
				InitBackButton();
			}
		}

		private void CloseAppView()
		{
			ServiceType = TYPE.NONE;
			ClearOrders();
			AppView.SetActive(value: false);
			Map.gameObject.SetActive(value: false);
			SetServiceMapInputGroup(mapVisible: false);
		}

		private void SetServiceMapInputGroup(bool mapVisible)
		{
			InputMapSetEvent.TrySetGroup((!mapVisible) ? InputGroupsEnum.Ui : InputGroupsEnum.RacingMap);
		}

		private void OnSuspensionClosed()
		{
			_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
			ShowMap(state: true);
			_appsPanel.SetActive(value: true);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnDestroy()
		{
			_isDestroyed = true;
			_tween?.Kill();
			_taskManager.OnFinish -= OnFinish;
			_isOpeningPhotoMode = false;
			Activity.onClick.RemoveListener(OpenActivity);
			base.OnDestroy();
		}

		private void FadeIn()
		{
			if (FadeInFaders == null)
			{
				return;
			}
			Fader[] fadeInFaders = FadeInFaders;
			foreach (Fader fader in fadeInFaders)
			{
				if ((bool)fader)
				{
					fader.FadeInFromZero(FadeDuration);
				}
			}
		}

		private void DisableApps()
		{
			DailyTasks.interactable = false;
			Taxi.interactable = false;
			Delivery.interactable = false;
			Cargo.interactable = false;
			Bus.interactable = false;
			Draggy.interactable = false;
			Drone.interactable = false;
			Suspension.interactable = false;
			PhotoMode.interactable = false;
		}

		private void CloseOtherUI()
		{
			if ((bool)_levelsProvider.LevelScript)
			{
				_levelsProvider.LevelScript.SetActiveFalseEachMenu(17);
			}
		}

		private void ShowNotAvalibleInGarage(string title, Sprite icon)
		{
			ShowNotification(new LocalizedString("MainMenu.ServiceNotAvalibleInGarage"), title, icon);
		}

		private void CloseWindow()
		{
			ResumeService();
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowMenuGarageControllerHUD("PhoneWindow");
			}
			Time.timeScale = 1f;
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			Close();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				if (ScenesHelper.IsMenuSceneActive() && !_isOpeningPhotoMode)
				{
					_managerUI.ChangeBalancePanelActiveState?.Invoke(obj: true);
				}
			}
		}

		private void ResumeService()
		{
			GPSTracker.ClearPath(forget: true);
			if ((bool)_freeDriveProvider.MapCamera)
			{
				_freeDriveProvider.MapCamera.HideBigMap();
			}
			if (!_levelsProvider.LevelScript || _levelsProvider.LevelScript.GetComponent<Camera>().enabled || !FlagsStorage.IsFreeDriveMode)
			{
				if ((bool)_levelsProvider.LevelScript)
				{
					_levelsProvider.LevelScript.SetActiveFalseEachMenu(1);
					_levelsProvider.LevelScript.UnCleanScreen();
				}
				InputStorage.ThrottleInput = 0f;
				InputStorage.BrakeInput = 0f;
				InputStorage.ClutchInput = 0f;
				if (SceneManager.GetActiveScene().buildIndex == 13)
				{
					_menuLevelParent.HideEmoji();
				}
			}
		}

		private void SetDateTime()
		{
			TimeText.text = DateTime.Now.ToString("hh\\:mm");
			DateText.text = DateTime.Today.ToString("dddd, dd MMMM", CultureInfo.InvariantCulture);
		}

		private void StartTask()
		{
			if (FlagsStorage.InGarageFree)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("PhoneWindow.Youcan'tstarttaskingarage"), 3f).Forget();
				return;
			}
			switch (ServiceType)
			{
			case TYPE.TAXI:
				_taxiServiceController.ClickStartButton(CurrentTask);
				break;
			case TYPE.DELIVERY:
				_deliveryServiceController.ClickStartButton(CurrentTask);
				break;
			case TYPE.CARGO:
				_cargoServiceController.ClickStartButton(CurrentTask);
				break;
			case (TYPE)3:
				break;
			}
		}

		private void SetMap()
		{
			if ((bool)_freeDriveProvider.MapCamera)
			{
				Map.gameObject.SetActive(value: true);
				_freeDriveProvider.MapCamera.serviceMap = Map;
				_freeDriveProvider.MapCamera.ShowServiceMap();
			}
		}

		private void ShowCancelButton(bool isEnabled)
		{
			Accept.SetActive(!isEnabled);
			Cancel.SetActive(isEnabled);
		}

		private void CancelTask()
		{
			_coreGameManager.CancelTask();
			switch (ServiceType)
			{
			case TYPE.TAXI:
				ServiceType = TYPE.NONE;
				OpenTaxi();
				break;
			case TYPE.DELIVERY:
				ServiceType = TYPE.NONE;
				_taskManager.Drop();
				OpenDelivery();
				break;
			case TYPE.CARGO:
				ServiceType = TYPE.NONE;
				OpenCargo();
				break;
			case (TYPE)3:
				break;
			}
		}

		private void ShowCompleteCurrentServiceNotification()
		{
			string text = new LocalizedString("PhoneWindow.YouHaveAnOngoingTaskPleaseComp");
			switch (_taskManager.Task.Type)
			{
			case TYPE.TAXI:
				ShowNotification(text, new LocalizedString("PhoneWindow.Taxi"), TaxiIcon);
				break;
			case TYPE.DELIVERY:
				ShowNotification(text, new LocalizedString("PhoneWindow.Delivery"), DeliveryIcon);
				break;
			case TYPE.CARGO:
				ShowNotification(text, new LocalizedString("PhoneWindow.Cargo"), CargoIcon);
				break;
			case (TYPE)3:
				break;
			}
		}

		private void ShowNotification(string text, string notifyName, Sprite icon)
		{
			if (_notificationShowed)
			{
				return;
			}
			_notificationShowed = true;
			Notification.gameObject.SetActive(value: true);
			AppIconInNotification.sprite = icon;
			AppNameInNotification.text = notifyName;
			NotificationText.text = text;
			Notification.anchoredPosition = new Vector2(0f, 450f);
			_tween = Notification.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					Notification.DOAnchorPosY(450f, 0.5f).SetEase(Ease.OutBack).SetDelay(3f)
						.SetUpdate(isIndependentUpdate: true)
						.OnComplete(() =>
						{
							_notificationShowed = false;
						});
				});
		}

		private void OpenTaxi()
		{
			if (FlagsStorage.IsPassenger)
			{
				ShowNotification(new LocalizedString("PhoneWindow.YouAreAPassenger"), new LocalizedString("PhoneWindow.Taxi"), TaxiIcon);
			}
			else if (CarIdStorage.CantUseTaxi(V.CarId))
			{
				ShowNotification(new LocalizedString("PhoneWindow.TaxiNotAvailableForThisCar"), new LocalizedString("PhoneWindow.Taxi"), TaxiIcon);
			}
			else
			{
				if (ServiceType == TYPE.TAXI)
				{
					return;
				}
				Map.gameObject.SetActive(value: true);
				SetServiceMapInputGroup(mapVisible: true);
				ServiceType = TYPE.TAXI;
				SetAppView(new LocalizedString("PhoneWindow.TaxiApp"), TaxiSprite);
				if (_taskManager.Task != null)
				{
					if (_taskManager.Task.Type == ServiceType)
					{
						ShowActiveOrder(_taxiServiceController.CurrentOrder);
					}
					else
					{
						ShowCompleteCurrentServiceNotification();
					}
				}
				else if (!V.Car)
				{
					string text = new LocalizedString("PhoneWindow.YouBetterGetTaxiForYourselfMan");
					ShowNotification(text, new LocalizedString("PhoneWindow.Taxi"), TaxiIcon);
				}
				else
				{
					_coreGameManager.OpenTaxi();
					Reset.SetActive(active: false);
					ShowNonInteractableAcceptButton();
				}
			}
		}

		private void OnFinish()
		{
			switch (ServiceType)
			{
			case TYPE.TAXI:
				_coreGameManager.OpenTaxi();
				break;
			case TYPE.DELIVERY:
				_coreGameManager.Delivery();
				break;
			case TYPE.CARGO:
				_coreGameManager.Cargo();
				break;
			}
			Reset.SetActive(active: false);
			ShowNonInteractableAcceptButton();
		}

		private void ShowActiveOrder(Service.Abstract.Order activeOrder)
		{
			AddActiveOrder(activeOrder);
			Reset.SetActive(active: false);
			ShowCancelButton(isEnabled: true);
		}

		private void SetAppView(string appName, Sprite appSprite, bool isService = true)
		{
			AppName.text = appName;
			AppTopImage.sprite = appSprite;
			AppView.SetActive(value: true);
			ServiceColumns.SetActive(isService);
			DraggyColumns.SetActive(!isService);
			ClearOrders();
			ReleaseThrottle();
			InitBackButton();
		}

		private void ReleaseThrottle()
		{
			if ((bool)_controlItemsProvider.ThrottleSliderControl)
			{
				_controlItemsProvider.ThrottleSliderControl.OnThrottleReleased();
			}
		}

		private void ClearOrders()
		{
			if ((bool)_draggyUIElement)
			{
				UnityEngine.Object.Destroy(_draggyUIElement.gameObject);
			}
			if (_orders == null || _orders.Count == 0)
			{
				return;
			}
			foreach (ServiceOrderUI order in _orders)
			{
				UnityEngine.Object.Destroy(order.gameObject);
			}
			_orders.Clear();
		}

		private void OpenDelivery()
		{
			ShowNotification(new LocalizedString("PhoneWindow.DeliveryNotAvailableYet"), new LocalizedString("PhoneWindow.Delivery"), DeliveryIcon);
		}

		private void OpenCargo()
		{
			ShowNotification(new LocalizedString("PhoneWindow.CargoNotAvailableYet"), new LocalizedString("PhoneWindow.Cargo"), CargoIcon);
		}

		private void OpenDraggy()
		{
			ServiceType = TYPE.NONE;
			Map.gameObject.SetActive(value: false);
			SetServiceMapInputGroup(mapVisible: false);
			SetAppView(new LocalizedString("PhoneWindow.DraggyApp"), DraggySprite, isService: false);
			Cancel.SetActive(active: false);
			Reset.SetActive(active: true);
			SetDraggy();
		}

		private void OpenDrone()
		{
			_menuLevelParent.ActiveDrone();
			_managerUI.ShowDroneControlWindow();
			if ((bool)_mainPerson)
			{
				_mainPerson.SwitchHeadVisible(visible: true);
			}
			Close();
		}

		private void OpenDailyTasks()
		{
			if (ManagerKeysForPrefs.HasEmail)
			{
				Map.gameObject.SetActive(value: false);
				_managerUI.ShowDailyTasksFull();
				Close();
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("PhoneWindow.YouAreNotAuthorized"), 3f, NotificationType.Error).Forget();
			}
		}

		private void OpenBus()
		{
		}

		private void OpenPhotoMode()
		{
			if (!base.IsCloseCanceled)
			{
				_isOpeningPhotoMode = true;
				_managerUI.ShowPhotoModeWindow();
				if ((bool)_mainPerson)
				{
					_mainPerson.EnterPhotoModeHeadVisible();
				}
				base.Close();
			}
		}

		private void OpenActivity()
		{
			OpenActivityAsync().Forget();
		}

		private async UniTaskVoid OpenActivityAsync()
		{
			Activity.interactable = false;
			if ((bool)PhoneHorizontalVerticalRotator)
			{
				await PhoneHorizontalVerticalRotator.RotateAsync(this.GetCancellationTokenOnDestroy());
			}
			if (!_isDestroyed)
			{
				await _managerUI.ShowWindow("PhoneActivityWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, JsonConvert.SerializeObject(new PhoneGameActivityWindow.AdditionalData
				{
					OpenSource = PhoneGameActivityWindow.OpenSourceType.Phone,
					FadeIn = true
				}));
				base.Close();
			}
		}

		private void SetDraggy()
		{
			GameObject gameObject = V.Car;
			if (!gameObject)
			{
				gameObject = V.LastUsedCar;
			}
			if ((bool)gameObject)
			{
				Dragy component = gameObject.GetComponent<Dragy>();
				_draggyUIElement = UnityEngine.Object.Instantiate(DraggyUIElementPrefab, OrderParent);
				_draggyUIElement.Init(component);
			}
		}

		private void ResetDraggy()
		{
			GameObject gameObject = V.Car;
			if (!gameObject)
			{
				gameObject = V.LastUsedCar;
			}
			if ((bool)gameObject && gameObject.TryGetComponent<Dragy>(out var component))
			{
				component.kmh100Result = 0f;
				component.kmh100slope = 0f;
				component.kmh100200Result = 0f;
				component.kmh100200Slope = 0f;
				component.mph60Result = 0f;
				component.mph60Slope = 0f;
				component.m402Result = 0f;
				component.m402Slope = 0f;
				if ((bool)_draggyUIElement)
				{
					_draggyUIElement.Init(component);
				}
			}
		}

		private void ShowSuspension()
		{
			if (CarIdStorage.CantControlSuspension(V.CarId))
			{
				ShowNotification(new LocalizedString("PhoneWindow.SuspensionNotAvailableForThisCar"), new LocalizedString("Phone.SuspensionControl"), CarControlIcon);
				return;
			}
			if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.SetActive(active: false);
			}
			ShowMap(state: false);
			Time.timeScale = 1f;
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			СarControlUIPanel.Show();
			CarControlUIPanel сarControlUIPanel = СarControlUIPanel;
			сarControlUIPanel.BackButtonPressed = (Action)Delegate.Remove(сarControlUIPanel.BackButtonPressed, new Action(OnSuspensionClosed));
			CarControlUIPanel сarControlUIPanel2 = СarControlUIPanel;
			сarControlUIPanel2.BackButtonPressed = (Action)Delegate.Combine(сarControlUIPanel2.BackButtonPressed, new Action(OnSuspensionClosed));
			if (AppView.activeSelf)
			{
				CloseAppView();
			}
			_appsPanel.SetActive(value: false);
		}

		private void AddActiveOrder(Service.Abstract.Order order)
		{
			ServiceOrderUI serviceOrderUI = UnityEngine.Object.Instantiate(ServiceOrderUIPrefab, OrderParent);
			serviceOrderUI.Init(order);
			_orders.Clear();
			_orders.Add(serviceOrderUI);
		}

		public void ShowNonInteractableAcceptButton()
		{
			ShowCancelButton(isEnabled: false);
			EnableAcceptButton(isEnabled: false);
		}

		public void EnableAcceptButton(bool isEnabled)
		{
			if ((bool)Accept)
			{
				Accept.interactable = isEnabled;
				Accept.image.sprite = (isEnabled ? NormalAccept : DisabledAccept);
				AcceptText.color = (isEnabled ? Color.white : Color.gray);
			}
		}

		public void OnOrderSelected(int id)
		{
			CurrentTask = id;
			if (!Accept.interactable)
			{
				EnableAcceptButton(isEnabled: true);
			}
			foreach (ServiceOrderUI order in _orders)
			{
				if (order.Id != id)
				{
					order.Select(isSelected: false);
				}
			}
		}

		public void AddOrder(Location departure, Location destination, uint price, float distance, string destinationName, string orderName, int id, int typeId = 0)
		{
			ServiceOrderUI serviceOrderUI = UnityEngine.Object.Instantiate(ServiceOrderUIPrefab, OrderParent);
			serviceOrderUI.Init(departure, destination, price, distance, destinationName, orderName, id, this, typeId);
			_orders.Add(serviceOrderUI);
		}

		public Service.Abstract.Order GetCurrentOrder()
		{
			return (from o in _orders
				where o.Id == CurrentTask
				select o.Order).FirstOrDefault();
		}

		public void ShowMap(bool state)
		{
			Background.SetActive(state);
		}
	}
}
