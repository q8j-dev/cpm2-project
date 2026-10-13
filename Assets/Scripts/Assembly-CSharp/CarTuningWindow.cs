using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.Garage;
using CPMEngine;
using Racing.CameraManagement;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.SOAP_Registry;

public class CarTuningWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Button backButton;

	[Header("Toggles")]
	[SerializeField]
	private Toggle tuningButton;

	[SerializeField]
	private Toggle exteriorButton;

	[SerializeField]
	private Toggle dynoRunButton;

	[SerializeField]
	private TextMeshProUGUI MileageText;

	[SerializeField]
	private TextMeshProUGUI TorqueText;

	[SerializeField]
	private TextMeshProUGUI TorqueTitleText;

	[SerializeField]
	private TextMeshProUGUI HPText;

	[SerializeField]
	private TextMeshProUGUI HPTitleText;

	[SerializeField]
	private TextMeshProUGUI WeightText;

	[SerializeField]
	private TextMeshProUGUI GearsText;

	[SerializeField]
	private TextMeshProUGUI TransmissionText;

	[Header("Buttons for windows")]
	[SerializeField]
	private UIPartCategoryChecker EngineButton;

	[SerializeField]
	private Button SuspensionButton;

	[SerializeField]
	private UIPartCategoryChecker GearboxButton;

	[SerializeField]
	private UIPartCategoryChecker MaintenanceButtonChecker;

	[Header("Other setttings")]
	public VisualButtonHandler OnPanelOpened;

	public int openPanel = 1;

	private CanvasGroup[] _panels;

	[SerializeField]
	private TextMeshProUGUI AllWeightText;

	private ITimerTuningService _timerTuningService;

	private ExteriorTuning _exteriorTuning;

	private ICarInfoService _carInfoService;

	private GarageController _garageController;

	private CarInfo _carInfo;

	private VehicleEntityProvider _vehicleData;

	private Powertrain _powertrain;

	private IDatabaseCarsService _databaseCarsService;

	private CameraManager _cameraManager;

	private AllCarsInfo _allCarsInfo;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(ManagerUI managerUI, ICarInfoService carInfoService, ITimerTuningService timerTuningService, ExteriorTuning exteriorTuning, IDatabaseCarsService databaseCarsService, CameraManager cameraManager, AllCarsInfo allCarsInfo, GarageController garageController)
	{
		_cameraManager = cameraManager;
		_databaseCarsService = databaseCarsService;
		_managerUI = managerUI;
		_carInfoService = carInfoService;
		_timerTuningService = timerTuningService;
		_exteriorTuning = exteriorTuning;
		_allCarsInfo = allCarsInfo;
		_garageController = garageController;
	}

	private void ShowVisualTuningWindow(bool state)
	{
		if (state)
		{
			_managerUI.ShowBlackPanel(state: false);
			_managerUI.ScreenFade.FadeOut(0.75f, 0f);
			_exteriorTuning.OpenVisualTuning(openFromUI: true);
			_managerUI.ShowVisualTuningWindow();
		}
	}

	private void OpenSuspensionButton()
	{
		_managerUI.ShowBlackPanel(state: false);
		_managerUI.ShowSuspensionWindow();
	}

	private void ShowMaintenanceWindow()
	{
		_managerUI.ShowBlackPanel(state: false);
		_managerUI.ShowMaintenanceWindow();
	}

	private void OpenWheelsTuningMenu()
	{
		_managerUI.ShowWheelsTuningWindow();
	}

	private void OpenGearboxTuningMenu()
	{
		_managerUI.ShowBlackPanel(state: true);
		_managerUI.ShowGearboxTuningWindow();
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		TorqueText.gameObject.SetActive(value: false);
		TorqueTitleText.gameObject.SetActive(value: false);
		HPText.gameObject.SetActive(value: false);
		HPTitleText.gameObject.SetActive(value: false);
		_managerUI.EditingEngineKitData = null;
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		_vehicleData = V.LastUsedCar.GetComponent<VehicleEntityProvider>();
		_powertrain = V.LastUsedCar.GetComponent<Powertrain>();
		backButton.onClick.AddListener(Back);
		EngineButton.UIPartCategoryButton.onClick.AddListener(OpenEngineTuningMenu);
		SuspensionButton.onClick.AddListener(OpenSuspensionButton);
		GearboxButton.UIPartCategoryButton.onClick.AddListener(OpenGearboxTuningMenu);
		MaintenanceButtonChecker.UIPartCategoryButton.onClick.AddListener(ShowMaintenanceWindow);
		tuningButton.SetIsOnWithoutNotify(value: true);
		exteriorButton.onValueChanged.AddListener(ShowVisualTuningWindow);
		dynoRunButton.onValueChanged.AddListener(GoToDynoRun);
		CheckMaintenance();
		CheckEngineAndGearboxStatuses();
		_databaseCarsService.EnterGarage();
		UpdateSpecText();
	}

	private void UpdateSpecText()
	{
		if (V.LastUsedCar.TryGetComponent<VehicleEntityProvider>(out var component))
		{
			int gears = component.CarEntity.GearBox.Gears;
			GearsText.text = gears.ToString();
			float mileage = component.CarEntity.MaintenanceInfo.Mileage;
			MileageText.text = $"{Utils.GetConvertedMileage(mileage):0.00} {Utils.CurrentMetric}";
			WeightText.text = Utils.GetMass(component.CarEntity.ChassisData.Weight + component.CarEntity.GetWeight(), round: true);
		}
		else
		{
			GearsText.text = string.Empty;
			MileageText.text = string.Empty;
		}
		if (V.LastUsedCar.TryGetComponent<Powertrain>(out var component2))
		{
			if (TransmissionText.TryGetComponent<LocalizedTextMeshPro>(out var component3))
			{
				component3.enabled = false;
			}
			TransmissionText.text = (component2.IsAutomaticGearbox ? new LocalizedString("Cartuningwindow.Automatic") : new LocalizedString("Cartuningwindow.Manual"));
		}
	}

	private void CheckMaintenance()
	{
		List<float> list = new List<float>();
		float pressure = _vehicleData.GetStockTuningData.Tuning.FrontWheelData.Pressure;
		float pressure2 = _vehicleData.GetStockTuningData.Tuning.RearWheelData.Pressure;
		_vehicleData.CarEntity.GetOilData(out var capacity, out var oilLife);
		float num = ((_vehicleData.CarEntity.Engine.RotorHousing != null) ? _vehicleData.CarEntity.Engine.RotorHousing.GetParams.OilCapacity : _vehicleData.CarEntity.Engine.Block.Part.OilCapacity);
		list.Add(capacity * 100f / num);
		list.Add(oilLife);
		list.Add((1f - _carInfo.Colors.DirtAmount) * 100f);
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.FrontAxle, out capacity, out oilLife);
		list.Add(Mathf.Round((capacity + oilLife) / 2f * 100f));
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.RearAxle, out capacity, out oilLife);
		list.Add(Mathf.Round((capacity + oilLife) / 2f * 100f));
		float num2 = math.round(_carInfo.TuningData.Maintenance.FuelInTank);
		list.Add(num2 * 100f / (float)_carInfo.GetDefaultTuningConfig().ChassisData.FuelTankCapacity);
		MaintenanceButtonChecker.Check(list);
		list.Clear();
		_powertrain.GetTyrePressure(AxleType.Front, out var pressure3);
		num2 = Utils.Pa2Bar(pressure3);
		MaintenanceButtonChecker.CheckRange(num2, pressure, 0.3f, 0.1f);
		_powertrain.GetTyrePressure(AxleType.Rear, out pressure3);
		num2 = Utils.Pa2Bar(pressure3);
		MaintenanceButtonChecker.CheckRange(num2, pressure2, 0.3f, 0.1f);
	}

	private void CheckEngineAndGearboxStatuses()
	{
		List<PartData> partDatas = new List<PartData>
		{
			_carInfo.TuningData.EngineKitData.Charger,
			_carInfo.TuningData.EngineKitData.Block,
			_carInfo.TuningData.EngineKitData.Camshaft1,
			_carInfo.TuningData.EngineKitData.Camshaft2,
			_carInfo.TuningData.EngineKitData.Crankshaft,
			_carInfo.TuningData.EngineKitData.Flywheel,
			_carInfo.TuningData.EngineKitData.Intercooler,
			_carInfo.TuningData.EngineKitData.Pistons,
			_carInfo.TuningData.EngineKitData.Radiator,
			_carInfo.TuningData.EngineKitData.AirFilter,
			_carInfo.TuningData.EngineKitData.ConnectionRod,
			_carInfo.TuningData.EngineKitData.CylinderHead,
			_carInfo.TuningData.EngineKitData.ExhaustHeader,
			_carInfo.TuningData.EngineKitData.ExhaustPipe,
			_carInfo.TuningData.EngineKitData.FuelInjectors,
			_carInfo.TuningData.EngineKitData.FuelPump,
			_carInfo.TuningData.EngineKitData.FuelRail,
			_carInfo.TuningData.EngineKitData.IntakeManifold,
			_carInfo.TuningData.EngineKitData.ThrottleValve
		};
		EngineButton.Check(partDatas);
		partDatas = new List<PartData>
		{
			_carInfo.TuningData.GearBox,
			_carInfo.TuningData.EngineKitData.Clutch
		};
		GearboxButton.Check(partDatas);
	}

	private void OpenEngineTuningMenu()
	{
		_managerUI.ShowBlackPanel(state: true);
		CarTuningData tuningData = _carInfoService.GetFromSlot(V.SlotID).TuningData;
		_managerUI.EditingEngineKitData = tuningData.EngineKitData;
		_managerUI.ShowEngineUIWindow();
	}

	public new void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.NavigationModeSet(isEnabled: false);
			base.Close();
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowBlackPanel(state: false);
		_databaseCarsService.ExitGarage();
		_managerUI.ShowGarageWindow();
		Close();
	}

	private void GoToDynoRun(bool state)
	{
		if (!state)
		{
			return;
		}
		_managerUI.ShowBlackPanel(state: false);
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
				if (math.all(isIncompatible == Utils.NotValid))
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
		_managerUI.ScreenFade.FadeOut(0.75f, 0f);
		_managerUI.ShowDynoWindow();
		V.CoreGameManager.menuControl.OpenDyno();
		if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component2))
		{
			_garageController.ClearSpaceForCar(component2);
		}
	}
}
