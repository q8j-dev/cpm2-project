using System;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class DragRacingCarSettingsUI : MonoBehaviour
{
	public GameObject TireListWindow;

	public GameObject mainWindow;

	public Slider pressureFrontSlider;

	public Slider pressureRearSlider;

	public Slider healthFrontRightSlider;

	public Slider healthFrontLeftSlider;

	public Slider healthRearRightSlider;

	public Slider healthRearLeftSlider;

	public TextMeshProUGUI FrontRight;

	public TextMeshProUGUI FrontLeft;

	public TextMeshProUGUI RearRight;

	public TextMeshProUGUI RearLeft;

	public WheelProvider FR;

	public WheelProvider FL;

	public WheelProvider RR;

	public WheelProvider RL;

	public TextMeshProUGUI frontInstalledTireTypeLabel;

	public TextMeshProUGUI rearInstalledTireTypeLabel;

	public Slider fuelProgressBar;

	public TextMeshProUGUI fuelProgressLabel;

	[SerializeField]
	private Button FuelPlusButton;

	[SerializeField]
	private Button FuelMinusButton;

	[SerializeField]
	private Button FrontPressurePlusButton;

	[SerializeField]
	private Button FrontPressureMinusButton;

	[SerializeField]
	private Button RearPressurePlusButton;

	[SerializeField]
	private Button RearPressureMinusButton;

	private VehicleData _vehicleData;

	private VehicleStockTuningData _carTuningDataConfig;

	private ICarInfoService _carInfoService;

	private ManagerUI _managerUI;

	private CarInfo _carInfo;

	private Powertrain _powertrain;

	private VehicleEntityProvider _vehicleEntityMono;

	private bool _frontTireChange;

	private const float WheelBarStep = 0.05f;

	private float _fuelTankCapacity;

	private int _displayedFuelTenths = -1;

	[Inject]
	public void Construct(ICarInfoService carInfoService, ManagerUI managerUI)
	{
		_carInfoService = carInfoService;
		_managerUI = managerUI;
	}

	public void OnOpen()
	{
		_powertrain = V.LastUsedCar.GetComponent<Powertrain>();
		_vehicleEntityMono = V.LastUsedCar.GetComponent<VehicleEntityProvider>();
		_vehicleData = V.LastUsedCar.GetComponent<VehicleData>();
		FR = _powertrain.GetAxles.FrontAxle.RightWheel;
		FL = _powertrain.GetAxles.FrontAxle.LeftWheel;
		RR = _powertrain.GetAxles.RearAxle.RightWheel;
		RL = _powertrain.GetAxles.RearAxle.LeftWheel;
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		_carTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(_vehicleData.CarId);
		TyreType tyreType = _carInfo.TuningData.FrontWheelData.tyreType;
		TyreType tyreType2 = _carInfo.TuningData.RearWheelData.tyreType;
		pressureFrontSlider.minValue = _carTuningDataConfig.Tuning.FrontWheelDataLimits.GetMinWheelPressure(tyreType);
		pressureFrontSlider.maxValue = _carTuningDataConfig.Tuning.FrontWheelDataLimits.GetMaxWheelPressure(_carTuningDataConfig.Tuning.FrontWheelData.Pressure, tyreType);
		pressureRearSlider.minValue = _carTuningDataConfig.Tuning.RearWheelDataLimits.GetMinWheelPressure(tyreType2);
		pressureRearSlider.maxValue = _carTuningDataConfig.Tuning.RearWheelDataLimits.GetMaxWheelPressure(_carTuningDataConfig.Tuning.RearWheelData.Pressure, tyreType2);
		_powertrain.GetTyrePressure(AxleType.Front, out var pressure);
		pressureFrontSlider.value = Utils.Pa2Bar(pressure);
		_powertrain.GetTyrePressure(AxleType.Rear, out pressure);
		pressureRearSlider.value = Utils.Pa2Bar(pressure);
		healthFrontRightSlider.value = FR.Wheel.TyreHealth;
		healthFrontRightSlider.fillRect.GetComponent<Image>().color = ((RL.Wheel.TyreHealth < 0.2f) ? Color.red : Color.green);
		healthFrontLeftSlider.value = FL.Wheel.TyreHealth;
		healthFrontLeftSlider.fillRect.GetComponent<Image>().color = ((RL.Wheel.TyreHealth < 0.2f) ? Color.red : Color.green);
		healthRearRightSlider.value = RR.Wheel.TyreHealth;
		healthRearRightSlider.fillRect.GetComponent<Image>().color = ((RL.Wheel.TyreHealth < 0.2f) ? Color.red : Color.green);
		healthRearLeftSlider.value = RL.Wheel.TyreHealth;
		healthRearLeftSlider.fillRect.GetComponent<Image>().color = ((RL.Wheel.TyreHealth < 0.2f) ? Color.red : Color.green);
		frontInstalledTireTypeLabel.text = GetTyreTypeName(_powertrain.GetTyreType(AxleType.Front));
		rearInstalledTireTypeLabel.text = GetTyreTypeName(_powertrain.GetTyreType(AxleType.Rear));
		_fuelTankCapacity = _carTuningDataConfig.Tuning.ChassisData.FuelTankCapacity;
		_displayedFuelTenths = -1;
		RefreshFuelView();
		CloseTireSelectWindow();
		AddListeners();
	}

	private string GetTyreTypeName(TyreType type)
	{
		return type switch
		{
			TyreType.Winter => new LocalizedString("DragRacingCarSettingsUI.Snow"), 
			TyreType.Offroad => new LocalizedString("DragRacingCarSettingsUI.Offroad"), 
			TyreType.Touring => new LocalizedString("DragRacingCarSettingsUI.Touring"), 
			TyreType.SemiSlick => new LocalizedString("DragRacingCarSettingsUI.SemiSlick"), 
			TyreType.Slick => new LocalizedString("DragRacingCarSettingsUI.Slick"), 
			TyreType.Buggy => new LocalizedString("DragRacingCarSettingsUI.Buggy"), 
			_ => new LocalizedString("DragRacingCarSettingsUI.Touring"), 
		};
	}

	public void onFrontChangeTirePressed()
	{
		mainWindow.SetActive(value: false);
		TireListWindow.SetActive(value: true);
		_frontTireChange = true;
	}

	public void onRearChangeTirePressed()
	{
		mainWindow.SetActive(value: false);
		TireListWindow.SetActive(value: true);
		_frontTireChange = false;
	}

	public void CloseTireSelectWindow()
	{
		mainWindow.SetActive(value: true);
		TireListWindow.SetActive(value: false);
	}

	public void OnTireSelected(GameObject tireType)
	{
		TyreType type = (TyreType)int.Parse(tireType.name);
		_powertrain.ChangeTyre((!_frontTireChange) ? AxleType.Rear : AxleType.Front, type);
		OnOpen();
	}

	public void OnFrontPressureChanged(float value)
	{
		_powertrain.SetTyrePressure(AxleType.Front, value);
		_carInfo.TuningData.FrontWheelData.Pressure = value;
		_carInfo.Save();
	}

	public void OnRearPressureChanged(float value)
	{
		_powertrain.SetTyrePressure(AxleType.Rear, value);
		_carInfo.TuningData.RearWheelData.Pressure = value;
		_carInfo.Save();
	}

	private void OpenFuelWindow()
	{
		FlagsStorage.IsFreeFuelWindow = false;
		_managerUI.ShowWindow(WindowScriptType.FuelDriveUIWindow.ToString(), alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
	}

	private void RefreshFuelView()
	{
		if ((bool)_vehicleEntityMono && !(_fuelTankCapacity <= 0f))
		{
			float getFuelInTank = _vehicleEntityMono.GetFuelInTank;
			int num = Mathf.RoundToInt(getFuelInTank * 10f);
			if (num != _displayedFuelTenths)
			{
				_displayedFuelTenths = num;
				fuelProgressBar.SetValueWithoutNotify(getFuelInTank / _fuelTankCapacity);
				fuelProgressLabel.text = $"{getFuelInTank:f1} L";
			}
		}
	}

	private void Update()
	{
		if (Time.frameCount % 10 == 0)
		{
			RefreshFuelView();
			if (!(FL == null))
			{
				FrontLeft.text = Math.Round(pressureFrontSlider.value, 2).ToString("F2") + " bar\n" + FL.Temperature.ToString("N0") + "°";
				FrontRight.text = Math.Round(pressureFrontSlider.value, 2).ToString("F2") + " bar\n" + FR.Temperature.ToString("N0") + "°";
				RearLeft.text = Math.Round(pressureRearSlider.value, 2).ToString("F2") + " bar\n" + RL.Temperature.ToString("N0") + "°";
				RearRight.text = Math.Round(pressureRearSlider.value, 2).ToString("F2") + " bar\n" + RL.Temperature.ToString("N0") + "°";
			}
		}
	}

	private void AddListeners()
	{
		RemoveListeners();
		FuelPlusButton.onClick.AddListener(OpenFuelWindow);
		FuelMinusButton.onClick.AddListener(OpenFuelWindow);
		FrontPressurePlusButton.onClick.AddListener(PlusFrontPressure);
		FrontPressureMinusButton.onClick.AddListener(MinusFrontPressure);
		RearPressurePlusButton.onClick.AddListener(PlusRearPressure);
		RearPressureMinusButton.onClick.AddListener(MinusRearPressure);
		pressureFrontSlider.onValueChanged.AddListener(OnFrontPressureChanged);
		pressureRearSlider.onValueChanged.AddListener(OnRearPressureChanged);
	}

	private void RemoveListeners()
	{
		FuelPlusButton.onClick.RemoveListener(OpenFuelWindow);
		FuelMinusButton.onClick.RemoveListener(OpenFuelWindow);
		FrontPressurePlusButton.onClick.RemoveListener(PlusFrontPressure);
		FrontPressureMinusButton.onClick.RemoveListener(MinusFrontPressure);
		RearPressurePlusButton.onClick.RemoveListener(PlusRearPressure);
		RearPressureMinusButton.onClick.RemoveListener(MinusRearPressure);
		pressureFrontSlider.onValueChanged.RemoveListener(OnFrontPressureChanged);
		pressureRearSlider.onValueChanged.RemoveListener(OnRearPressureChanged);
	}

	private void PlusFrontPressure()
	{
		pressureFrontSlider.value += 0.05f;
	}

	private void MinusFrontPressure()
	{
		pressureFrontSlider.value -= 0.05f;
	}

	private void PlusRearPressure()
	{
		pressureRearSlider.value += 0.05f;
	}

	private void MinusRearPressure()
	{
		pressureRearSlider.value -= 0.05f;
	}
}
