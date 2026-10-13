using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CPM.Database;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Racing.CameraManagement;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Interior;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drag
{
	public class GarageMenuWindow : WindowScript, IBackableWindow
	{
		private const string LocalizeInteriorButtonKeyDownload = "Garagewindow.Downloadinterior";

		private const string LocalizeInteriorButtonKeyInstall = "Garagewindow.InstallInterior";

		private const string LocalizeInteriorButtonKeyReinstall = "Garagewindow.ReinstallInterior";

		private const int DefaultInteriorNumber = 1;

		public Button SettingButton;

		public Button newCarsButton;

		[SerializeField]
		private Button ButtonInterior;

		[SerializeField]
		private TMP_Text InteriorButtonText;

		public Button RideCarButton;

		[SerializeField]
		private OccupiedCarSlotsUI OccupiedCarSlotsUI;

		[SerializeField]
		private TextMeshProUGUI CarTitleText;

		[SerializeField]
		private Slider HorsePowerSlider;

		[SerializeField]
		private TextMeshProUGUI HorsePowerSliderValueText;

		[SerializeField]
		private Slider TorqueSlider;

		[SerializeField]
		private TextMeshProUGUI TorqueValueText;

		[SerializeField]
		private Slider WeightSlider;

		[SerializeField]
		private TextMeshProUGUI WeightValueText;

		[SerializeField]
		private Slider GearsSlider;

		[SerializeField]
		private TextMeshProUGUI GearsValueText;

		[SerializeField]
		private TextMeshProUGUI MilliageText;

		[SerializeField]
		private float GearsMaxValue;

		[SerializeField]
		private float WeightMaxValue;

		[SerializeField]
		private float TorqueMaxValue;

		[SerializeField]
		private float HpMaxValue;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		[SerializeField]
		private ScrollRect ScrollRect;

		[SerializeField]
		private RectTransform ContentPanel;

		[SerializeField]
		private Button WalkButton;

		[SerializeField]
		private CanvasGroup SettingsCanvasGroup;

		[SerializeField]
		private CanvasGroup PoliceCanvasGroup;

		[SerializeField]
		private CanvasGroup ManageSlotsCanvasGroup;

		private CoreGameManager _coreGameManager;

		private ISlotService _slotService;

		private IDatabaseCarsService _carsService;

		private GarageController _garageController;

		private Action<GameObject, VehicleData> _updateSpecTextOnCarEnter;

		private Tween _tween;

		private ITimerTuningService _timerTuningService;

		private ICarInfoService _carInfoService;

		private AllCarsInfo _allCarsInfo;

		private IFreeDriverService _freeDriverService;

		private CameraManager _cameraManager;

		private CarSelectorByTouch _carSelector;

		private AssetBundleManager _assetBundleManager;

		private IGarageSelectorService _garageSelectorService;

		private const bool DeleteNfs = false;

		private VehicleData _vehicleData;

		private bool _destroyed;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

		public static int SelectedSlotId { get; set; } = -1;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, ManagerUI managerUI, ICarInfoService carInfoService, ISlotService slotService, IDatabaseCarsService carsService, EventRegistry eventRegistry, GarageController garageController, ITimerTuningService timerTuningService, AllCarsInfo allCarsInfo, IFreeDriverService freeDriverService, CameraManager cameraManager, CarSelectorByTouch carSelector, AssetBundleManager assetBundleManager, IGarageSelectorService garageSelectorService)
		{
			_timerTuningService = timerTuningService;
			_carsService = carsService;
			_slotService = slotService;
			_coreGameManager = coreGameManager;
			_managerUI = managerUI;
			_garageController = garageController;
			_allCarsInfo = allCarsInfo;
			_freeDriverService = freeDriverService;
			_cameraManager = cameraManager;
			_carInfoService = carInfoService;
			_carSelector = carSelector;
			_eventRegistry = eventRegistry;
			_assetBundleManager = assetBundleManager;
			_garageSelectorService = garageSelectorService;
		}

		protected override void Awake()
		{
			base.Awake();
			OccupiedCarSlotsUI.Init(_garageSelectorService.CurrentGarageIndex);
		}

		private void CheckCarsNfc()
		{
			List<int> allSlots = _slotService.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				CarInfo fromSlot = _carInfoService.GetFromSlot(allSlots[i]);
				if (fromSlot != null)
				{
					_allCarsInfo.ResolveCarMeta(fromSlot.CarID, out var result);
					if (math.all(result == Utils.NotValid))
					{
						DeleteCar(allSlots[i]);
					}
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			bool flag = _coreGameManager.HasFirstCar();
			InitBackButton();
			_managerUI.IncreaseTimerButtonShow?.Invoke(obj: true);
			GearsSlider.maxValue = GearsMaxValue;
			HorsePowerSlider.maxValue = HpMaxValue;
			TorqueSlider.maxValue = TorqueMaxValue;
			WeightSlider.maxValue = WeightMaxValue;
			newCarsButton.onClick.AddListener(OpenNewCarsWindow);
			WalkButton.onClick.AddListener(OnFreeWalk);
			if (flag)
			{
				SettingButton.onClick.AddListener(LoadShop);
				RideCarButton.onClick.AddListener(RideCarInMenuGarage);
			}
			V.OnCarEntered += OnCarChanged;
			if ((bool)V.Car)
			{
				_vehicleData = V.Car.GetComponent<VehicleData>();
				OnCarChanged(V.Car, _vehicleData);
			}
			InitInteriorButton();
			SettingsCanvasGroup.alpha = (flag ? 1f : 0.3f);
			PoliceCanvasGroup.alpha = (flag ? 1f : 0.3f);
			ManageSlotsCanvasGroup.alpha = 1f;
			WalkButton.SetActive(!flag);
			RideCarButton.SetActive(flag);
			SnapTo();
		}

		private void InitInteriorButton()
		{
			ButtonInterior.gameObject.SetActive(value: false);
		}

		private async UniTask InstallInterior()
		{
			ButtonInterior.interactable = false;
			int interior = ((_vehicleData.carInfo.InstalledBodyKits.InteriorId == 0) ? 1 : _vehicleData.carInfo.InstalledBodyKits.InteriorId);
			await _vehicleData.InteriorsComposition.TrySetInterior(interior, save: true);
			if (!_destroyed)
			{
				InitInteriorButton();
			}
		}

		private async UniTask ReinstallInterior()
		{
			ButtonInterior.interactable = false;
			_vehicleData.InteriorsComposition.ResetInterior();
			await UniTask.Yield();
			await _vehicleData.InteriorsComposition.TrySetInterior(_vehicleData.carInfo.InstalledBodyKits.InteriorId, save: false);
			if (!_destroyed)
			{
				InitInteriorButton();
			}
		}

		private async UniTask DownloadInterior()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("GarageMenuWindow.NoInternetConnection"), 2f);
				return;
			}
			ButtonInterior.interactable = false;
			int interior = ((_vehicleData.carInfo.InstalledBodyKits.InteriorId == 0) ? 1 : _vehicleData.carInfo.InstalledBodyKits.InteriorId);
			int version = InteriorVersionStorage.GetVersionOfInterior(_vehicleData.CarId) + 6;
			string text = ((interior > 1) ? $"{_vehicleData.CarId}i_{interior}" : $"{_vehicleData.CarId}i");
			string message = new LocalizedString("SetCar.Downloadinghasstarted.Onceitisfinished");
			_coreGameManager.SetMessage(message, 3f);
			AssetBundle assetBundle = await _assetBundleManager.DownloadInteriorAssetBundle(text, version, this.GetCancellationTokenOnDestroy());
			if (!_destroyed && !(assetBundle == null))
			{
				await _vehicleData.InteriorsComposition.TrySetInterior(interior, save: true);
				if (!_destroyed)
				{
					InitInteriorButton();
				}
			}
		}

		protected override void OnDestroy()
		{
			V.OnCarEntered -= OnCarChanged;
			ButtonInterior.onClick.RemoveAllListeners();
			_destroyed = true;
			base.OnDestroy();
		}

		protected override void OnEnable()
		{
			_carSelector.Selected += SnapTo;
			V.OnCarEntered += _updateSpecTextOnCarEnter;
			_carSelector.enabled = true;
			base.OnEnable();
			_onThrottleHandle.Get(_eventRegistry).Subscribe(RideCarForSubscription);
		}

		private void OnDisable()
		{
			_carSelector.Selected -= SnapTo;
			V.OnCarEntered -= _updateSpecTextOnCarEnter;
			_carSelector.enabled = false;
			_onThrottleHandle.Get(_eventRegistry)?.Unsubscribe(RideCarForSubscription);
		}

		private void OpenNewCarsWindow()
		{
			if (!_cameraManager.firstCam.activeSelf)
			{
				_coreGameManager.OpenSellCars();
				_managerUI.ShowNewCarsWindow();
				Close();
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			V.CoreGameManager.CheckDeviceForShowUpdateAndroidWindow();
			_managerUI.ShowMainMenuWindow();
			Close();
		}

		private void LoadShop()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleEntityProvider>(out var _))
			{
				_managerUI.ShowCarTuningWindow();
				Close();
			}
		}

		public async void DeleteCar(int slotId)
		{
			CarSlot slot = _slotService.GetSlot(slotId);
			_carsService.DeleteCarFull(slot);
			if (_slotService.OccupiedSlotCount > 0)
			{
				ShowUI(state: true);
				return;
			}
			await Task.Delay(10);
			_managerUI.ShowMainMenuWindow();
			_freeDriverService.SetupFreeDriver();
		}

		private void OnFreeWalk()
		{
			_managerUI.ShowFreeWalkWindow();
			FlagsStorage.FreeWalk = true;
			_cameraManager.ActivateCamera(RacingCameraType.None);
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
			if (V.Car.TryGetComponent<Powertrain>(out var component2))
			{
				component2.ResetTransmission();
				component2.StartEngine(starterSound: true);
			}
			Close();
		}

		private void OnCarChanged(GameObject car, VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
			InitInteriorButton();
		}

		private void SnapTo()
		{
			_garageController.LastSelecterCarInGarage = SelectedSlotId;
			if (_garageController.LastSelecterCarInGarage != -1 && _garageController.LastSelecterCarInGarage <= ContentPanel.childCount)
			{
				float value = 1f - (float)_garageController.LastSelecterCarInGarage / (float)ContentPanel.childCount;
				value = Mathf.Clamp01(value);
				ScrollRect.verticalNormalizedPosition = value;
			}
		}
	}
}
