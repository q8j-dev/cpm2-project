using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.Garage;
using CPMEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;

public class MaintenanceUIWindow : WindowScript, IBackableWindow
{
	private static readonly int MaskValue = Shader.PropertyToID("_MaskValue");

	[SerializeField]
	private GameObject TiresGraphPanel = default;

	[SerializeField]
	private GameObject OilGraphPanel = default;

	[SerializeField]
	private GameObject CarWashPanel = default;

	[SerializeField]
	private GameObject FuelPanel = default;

	[SerializeField]
	private GameObject PricePanel = default;

	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	[SerializeField]
	private TextMeshProUGUI DescriptionText = default;

	[SerializeField]
	private Slider SliderTop = default;

	[SerializeField]
	private Slider SliderDown = default;

	[SerializeField]
	private TextMeshProUGUI SliderTopText = default;

	[SerializeField]
	private Image SliderTopFillImage = default;

	[SerializeField]
	private TextMeshProUGUI SliderDownText = default;

	[SerializeField]
	private Image SliderDownFillImage = default;

	[SerializeField]
	private TextMeshProUGUI PriceText = default;

	[SerializeField]
	private ForceRebuildLayoutImmediate ForceRebuildLayoutPrice = default;

	[SerializeField]
	private Button ConfirmButton = default;

	[SerializeField]
	private TextMeshProUGUI SliderTopTitleText = default;

	[SerializeField]
	private TextMeshProUGUI SliderDownTitleText = default;

	[SerializeField]
	private MaintenanceConfirmWindow MaintenanceConfirmWindow = default;

	[SerializeField]
	private GameObject MainPanel = default;

	[Header("TiresPanel")]
	[SerializeField]
	private Slider FrontLeftWheelSlider = default;

	[SerializeField]
	private Slider FrontRightWheelSlider = default;

	[SerializeField]
	private Slider RearLeftWheelSlider = default;

	[SerializeField]
	private Slider RearRightWheelSlider = default;

	[SerializeField]
	private TextMeshProUGUI FrontLeftWheelSliderText = default;

	[SerializeField]
	private TextMeshProUGUI FrontRightWheelSliderText = default;

	[SerializeField]
	private TextMeshProUGUI RearLeftWheelSliderText = default;

	[SerializeField]
	private TextMeshProUGUI RearRightWheelSliderText = default;

	[SerializeField]
	[Header("OilPanel")]
	private Slider OilSlider = default;

	[Header("Toggles")]
	[SerializeField]
	private UIPartCategoryChecker TireConditionToggle = default;

	[SerializeField]
	private UIPartCategoryChecker TirePressureToggle = default;

	[SerializeField]
	private UIPartCategoryChecker OilToggle = default;

	[SerializeField]
	private UIPartCategoryChecker CarWashToggle = default;

	[SerializeField]
	private UIPartCategoryChecker FuelToggle = default;

	[SerializeField]
	[Header("Color")]
	private Color ErrorColor = default;

	[SerializeField]
	private Color WarningColor = default;

	[SerializeField]
	private Color NormalColor = default;

	private VehicleEntityProvider _vehicleEntityProvider;

	private CarInfo _carInfo;

	private ICarInfoService _carInfoService;

	private Powertrain _powertrain;

	private MaintenanceStates _currentMaintenanceState;

	private VehicleData _vehicleData;

	private VehicleStockTuningData _defaultCarTuningConfig;

	private IPricesService _pricesService;

	private GarageController _garageController;

	private LocalizedTextMeshPro _confirmButtonLabel;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

	[Inject]
	public void Construct(ICarInfoService carInfoService, ManagerUI managerUI, IPricesService pricesService, GarageController garageController)
	{
		_pricesService = pricesService;
		_managerUI = managerUI;
		_carInfoService = carInfoService;
		_garageController = garageController;
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		V.Car.TryGetComponent<Powertrain>(out _powertrain);
		V.Car.TryGetComponent<VehicleData>(out _vehicleData);
		V.Car.TryGetComponent<VehicleEntityProvider>(out _vehicleEntityProvider);
		_garageController.EnableCarPhysics(_vehicleData, isEnabled: true);
		MaintenanceConfirmWindow.Prepare().Forget();
		TireConditionToggle.UIPartCategoryToggle.onValueChanged.AddListener(OnTireConditionPress);
		TirePressureToggle.UIPartCategoryToggle.onValueChanged.AddListener(OnTirePressurePress);
		OilToggle.UIPartCategoryToggle.onValueChanged.AddListener(OnOilPress);
		CarWashToggle.UIPartCategoryToggle.onValueChanged.AddListener(OnCarWashPress);
		FuelToggle.UIPartCategoryToggle.onValueChanged.AddListener(OnFuelPress);
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		Init();
		CheckMaintenance();
		_defaultCarTuningConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(_vehicleData.CarId);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowCarTuningWindow();
		Close();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)_vehicleData && (bool)_garageController)
		{
			_garageController.DisableCarPhysicsDelayed(_vehicleData, 2f);
		}
	}

	private void Init()
	{
		MainPanel.SetActive(value: true);
		switch (_currentMaintenanceState)
		{
		case MaintenanceStates.TireCondition:
			InitTireCondition();
			break;
		case MaintenanceStates.TirePressure:
			InitTirePressure();
			break;
		case MaintenanceStates.Oil:
			InitOil();
			break;
		case MaintenanceStates.CarWash:
			InitCarWash();
			break;
		case MaintenanceStates.Fuel:
			InitFuel();
			break;
		}
	}

	private void OnFuelPress(bool state)
	{
		if (state && _currentMaintenanceState != MaintenanceStates.Fuel)
		{
			InitFuel();
		}
	}

	private void OnTireConditionPress(bool state)
	{
		if (state && _currentMaintenanceState != MaintenanceStates.TireCondition)
		{
			InitTireCondition();
		}
	}

	private void OnTirePressurePress(bool state)
	{
		if (state && _currentMaintenanceState != MaintenanceStates.TirePressure)
		{
			InitTirePressure();
		}
	}

	private void OnOilPress(bool state)
	{
		if (state && _currentMaintenanceState != MaintenanceStates.Oil)
		{
			InitOil();
		}
	}

	private void OnCarWashPress(bool state)
	{
		if (state && _currentMaintenanceState != MaintenanceStates.CarWash)
		{
			InitCarWash();
		}
	}

	private void CheckMaintenance()
	{
		ResetStates();
		CheckOil();
		CheckCarWash();
		CheckTireHealth();
		CheckTyrePressure();
		CheckFuel();
	}

	private void ResetStates()
	{
		OilToggle.ResetState();
		CarWashToggle.ResetState();
		TirePressureToggle.ResetState();
		TireConditionToggle.ResetState();
		FuelToggle.ResetState();
	}

	private void CheckOil()
	{
		List<float> list = new List<float>();
		_vehicleEntityProvider.CarEntity.GetOilData(out var capacity, out var oilLife);
		float num = ((_vehicleEntityProvider.CarEntity.Engine.RotorHousing != null && _vehicleEntityProvider.CarEntity.Engine.RotorHousing.Part != null) ? _vehicleEntityProvider.CarEntity.Engine.RotorHousing.Part.OilCapacity : _vehicleEntityProvider.CarEntity.Engine.Block.Part.OilCapacity);
		list.Add(capacity * 100f / num);
		list.Add(oilLife);
		OilToggle.Check(list);
	}

	private void CheckCarWash()
	{
		List<float> list = new List<float>();
		_carInfo.Colors.DirtAmount = _vehicleData.CarMat.GetFloat(MaskValue);
		list.Add((1f - _carInfo.Colors.DirtAmount) * 100f);
		CarWashToggle.Check(list);
	}

	private void CheckTireHealth()
	{
		List<float> list = new List<float>();
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.FrontAxle, out var left, out var right);
		list.Add(Mathf.Round((left + right) / 2f * 100f));
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.RearAxle, out left, out right);
		list.Add(Mathf.Round((left + right) / 2f * 100f));
		TireConditionToggle.Check(list);
	}

	private void CheckTyrePressure()
	{
		float pressure = _carInfo.GetDefaultTuningConfig().FrontWheelData.Pressure;
		float pressure2 = _carInfo.GetDefaultTuningConfig().RearWheelData.Pressure;
		_powertrain.GetTyrePressure(AxleType.Front, out var pressure3);
		float values = Utils.Pa2Bar(pressure3);
		TirePressureToggle.CheckRange(values, pressure, 0.3f, 0.1f);
		_powertrain.GetTyrePressure(AxleType.Rear, out pressure3);
		values = Utils.Pa2Bar(pressure3);
		TirePressureToggle.CheckRange(values, pressure2, 0.3f, 0.1f);
	}

	private void CheckFuel()
	{
		List<float> list = new List<float>();
		float num = math.round(_vehicleEntityProvider.CarEntity.MaintenanceInfo.FuelInTank);
		list.Add(num * 100f / (float)_carInfo.GetDefaultTuningConfig().ChassisData.FuelTankCapacity);
		FuelToggle.Check(list);
	}

	private bool SetButtonState(float maxValue, float value, float threshold = 0.9f)
	{
		bool flag = Mathf.Round(maxValue) * threshold > Mathf.Round(value);
		ConfirmButton.interactable = flag;
		return flag;
	}

	private void InitTireCondition()
	{
		_currentMaintenanceState = MaintenanceStates.TireCondition;
		ResetPanels();
		SetMinMaxValues(SliderTop, 0f, 100f);
		SetMinMaxValues(SliderDown, 0f, 100f);
		TiresGraphPanel.SetActive(value: true);
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: true);
		SliderDownTitleText.transform.parent.gameObject.SetActive(value: true);
		TitleText.text = new LocalizedString("MaintenanceUIWindow.TiresCondition");
		DescriptionText.text = new LocalizedString("MaintenanceUIWindow.IndicatesWearLevelOfTiresAffec");
		SetConfirmButtonText("Mainteancewindow.Change");
		SliderTopTitleText.text = new LocalizedString("MaintenanceUIWindow.FrontTiresCondition");
		SliderDownTitleText.text = new LocalizedString("MaintenanceUIWindow.RearTiresCondition");
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.FrontAxle, out var left, out var right);
		SetMinMaxValues(FrontLeftWheelSlider, 0f, 100f);
		FillSlider(FrontLeftWheelSlider, FrontLeftWheelSliderText, left, "%", roundToPercent: true);
		SetMinMaxValues(FrontRightWheelSlider, 0f, 100f);
		FillSlider(FrontRightWheelSlider, FrontRightWheelSliderText, right, "%", roundToPercent: true);
		float value = Mathf.Round((left + right) / 2f * 100f);
		bool num = SetButtonState(100f, value);
		Color color = GetColor(value, 1f);
		FillSlider(SliderTop, SliderTopText, value, "%");
		SetColors(color, SliderTopText, SliderTopFillImage);
		_powertrain.GetAxles.GetTyreHealth(_powertrain.GetAxles.RearAxle, out left, out right);
		SetMinMaxValues(RearLeftWheelSlider, 0f, 100f);
		FillSlider(RearLeftWheelSlider, RearLeftWheelSliderText, left, "%", roundToPercent: true);
		SetMinMaxValues(RearRightWheelSlider, 0f, 100f);
		FillSlider(RearRightWheelSlider, RearRightWheelSliderText, right, "%", roundToPercent: true);
		value = Mathf.Round((left + right) / 2f * 100f);
		if (!num)
		{
			SetButtonState(100f, value);
		}
		color = GetColor(value, 100f);
		FillSlider(SliderDown, SliderDownText, value, "%");
		SetColors(color, SliderDownText, SliderDownFillImage);
		SetPriceText();
		ConfirmButton.onClick.AddListener(() =>
		{
			MainPanel.SetActive(value: false);
			MaintenanceConfirmWindow.Init(MaintenanceStates.TireCondition);
		});
	}

	private Color GetPressureColor(float value, float optimalValue, float minWarningPercent, float optimalMinPercent)
	{
		float num = optimalValue - optimalValue * minWarningPercent;
		float num2 = optimalValue + optimalValue * minWarningPercent;
		float num3 = optimalValue - optimalValue * optimalMinPercent;
		if (optimalValue + optimalValue * optimalMinPercent > value && num3 < value)
		{
			return NormalColor;
		}
		if (num2 > value && num < value)
		{
			return WarningColor;
		}
		return ErrorColor;
	}

	private Color GetColor(float value, float maxValue)
	{
		float num = maxValue * 0.2f;
		float num2 = maxValue * 0.5f;
		if (value < num2 && value > num)
		{
			return WarningColor;
		}
		if (value < num)
		{
			return ErrorColor;
		}
		return NormalColor;
	}

	private void SetMinMaxValues(Slider slider, float minVal, float maxVal)
	{
		slider.minValue = minVal;
		slider.maxValue = maxVal;
	}

	private void SetConfirmButtonText(string localizationKey)
	{
		if (!_confirmButtonLabel)
		{
			_confirmButtonLabel = ConfirmButton.GetComponentInChildren<LocalizedTextMeshPro>(includeInactive: true);
		}
		_confirmButtonLabel.SetKey(localizationKey);
	}

	private void InitTirePressure()
	{
		float pressure = _vehicleEntityProvider.GetStockTuningData.Tuning.FrontWheelData.Pressure;
		float pressure2 = _vehicleEntityProvider.GetStockTuningData.Tuning.RearWheelData.Pressure;
		TyreType tyreType = _carInfo.TuningData.FrontWheelData.tyreType;
		TyreType tyreType2 = _carInfo.TuningData.RearWheelData.tyreType;
		float minWheelPressure = _defaultCarTuningConfig.Tuning.FrontWheelDataLimits.GetMinWheelPressure(tyreType);
		float minWheelPressure2 = _defaultCarTuningConfig.Tuning.RearWheelDataLimits.GetMinWheelPressure(tyreType2);
		float maxWheelPressure = _defaultCarTuningConfig.Tuning.FrontWheelDataLimits.GetMaxWheelPressure(pressure, tyreType);
		float maxWheelPressure2 = _defaultCarTuningConfig.Tuning.RearWheelDataLimits.GetMaxWheelPressure(pressure2, tyreType2);
		_currentMaintenanceState = MaintenanceStates.TirePressure;
		ResetPanels();
		SetMinMaxValues(SliderTop, minWheelPressure, maxWheelPressure);
		SetMinMaxValues(SliderDown, minWheelPressure2, maxWheelPressure2);
		TiresGraphPanel.SetActive(value: true);
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: true);
		SliderDownTitleText.transform.parent.gameObject.SetActive(value: true);
		TitleText.text = new LocalizedString("MaintenanceUIWindow.TiresPressure");
		DescriptionText.text = new LocalizedString("MaintenanceUIWindow.MeasuresAirPressureInTiresImpa");
		SetConfirmButtonText("Mainteancewindow.TyreChange");
		SliderTopTitleText.text = new LocalizedString("MaintenanceUIWindow.FrontTiresPressure");
		SliderDownTitleText.text = new LocalizedString("MaintenanceUIWindow.RearTiresPressure");
		_powertrain.GetTyrePressure(AxleType.Front, out var pressure3);
		float value = Utils.Pa2Bar(pressure3);
		SetMinMaxValues(FrontLeftWheelSlider, minWheelPressure, maxWheelPressure);
		SetMinMaxValues(FrontRightWheelSlider, minWheelPressure, maxWheelPressure);
		FillSlider(FrontLeftWheelSlider, FrontLeftWheelSliderText, value, new LocalizedString("MaintenanceUIWindow.Bar"));
		FillSlider(FrontRightWheelSlider, FrontRightWheelSliderText, value, new LocalizedString("MaintenanceUIWindow.Bar"));
		Color pressureColor = GetPressureColor(value, pressure, 0.3f, 0.1f);
		SetColors(pressureColor, SliderTopText, SliderTopFillImage);
		FillSlider(SliderTop, SliderTopText, value, new LocalizedString("MaintenanceUIWindow.Bar"));
		_powertrain.GetTyrePressure(AxleType.Rear, out pressure3);
		value = Utils.Pa2Bar(pressure3);
		SetMinMaxValues(RearLeftWheelSlider, minWheelPressure2, maxWheelPressure2);
		SetMinMaxValues(RearRightWheelSlider, minWheelPressure2, maxWheelPressure2);
		FillSlider(RearLeftWheelSlider, RearLeftWheelSliderText, value, new LocalizedString("MaintenanceUIWindow.Bar"));
		FillSlider(RearRightWheelSlider, RearRightWheelSliderText, value, new LocalizedString("MaintenanceUIWindow.Bar"));
		pressureColor = GetPressureColor(value, pressure2, 0.3f, 0.1f);
		FillSlider(SliderDown, SliderDownText, Utils.Pa2Bar(pressure3), new LocalizedString("MaintenanceUIWindow.Bar"));
		SetColors(pressureColor, SliderDownText, SliderDownFillImage);
		SetPriceText();
		ConfirmButton.onClick.AddListener(() =>
		{
			MainPanel.SetActive(value: false);
			MaintenanceConfirmWindow.Init(MaintenanceStates.TirePressure);
		});
	}

	private void InitFuel()
	{
		_currentMaintenanceState = MaintenanceStates.Fuel;
		ConfirmButton.interactable = true;
		ResetPanels();
		PricePanel.SetActive(value: false);
		FuelPanel.SetActive(value: true);
		int num = Mathf.RoundToInt(_vehicleEntityProvider.CarEntity.MaintenanceInfo.FuelInTank);
		int fuelTankCapacity = _carInfo.GetDefaultTuningConfig().ChassisData.FuelTankCapacity;
		SetMinMaxValues(SliderTop, 0f, fuelTankCapacity);
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: true);
		TitleText.text = new LocalizedString("MaintenanceConfirmWindow.Fuel");
		DescriptionText.text = new LocalizedString("MaintenanceUIWindow.IndicatesTheAmountOfFuelInTheT");
		SetConfirmButtonText("Mainteancewindow.FuelChange");
		SliderTopTitleText.text = new LocalizedString("MaintenanceUIWindow.TankFullness");
		FillSlider(SliderTop, SliderTopText, num, "");
		Color color = GetColor(num, fuelTankCapacity);
		SetColors(color, SliderTopText, SliderTopFillImage);
		SetPriceText();
		ConfirmButton.onClick.AddListener(() =>
		{
			MainPanel.SetActive(value: false);
			MaintenanceConfirmWindow.Init(MaintenanceStates.Fuel);
		});
	}

	private void FillSlider(Slider slider, TextMeshProUGUI valueText, float value, string suffix, bool roundToPercent = false)
	{
		if (roundToPercent)
		{
			value = Mathf.Round(value * 100f);
		}
		slider.value = value;
		valueText.text = Math.Round(value, 2) + suffix;
	}

	public float GetDefaultTirePressure(bool isFront)
	{
		if (!isFront)
		{
			return _defaultCarTuningConfig.Tuning.RearWheelData.Pressure;
		}
		return _defaultCarTuningConfig.Tuning.FrontWheelData.Pressure;
	}

	private void SetColors(Color color, TextMeshProUGUI valueText, Image sliderFillImage)
	{
		valueText.color = color;
		sliderFillImage.color = color;
	}

	private void InitOil()
	{
		_currentMaintenanceState = MaintenanceStates.Oil;
		ResetPanels();
		OilGraphPanel.SetActive(value: true);
		float num = _vehicleEntityProvider.CarEntity.GetMaxOilCapacity;
		_vehicleEntityProvider.CarEntity.GetOilData(out var capacity, out var oilLife);
		if (!float.IsFinite(capacity))
		{
			capacity = 0f;
		}
		if (!float.IsFinite(oilLife))
		{
			oilLife = 0f;
		}
		if (!float.IsFinite(num) || num <= 0f)
		{
			num = 1f;
		}
		SetMinMaxValues(OilSlider, 0f, num);
		OilSlider.value = capacity;
		SetMinMaxValues(SliderTop, 0f, 100f);
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: true);
		TitleText.text = new LocalizedString("MaintenanceUIWindow.OilCondition");
		DescriptionText.text = new LocalizedString("MaintenanceUIWindow.QualityOfEngineOilCrucialForEn");
		SetConfirmButtonText("Mainteancewindow.OilChange");
		SliderTopTitleText.text = new LocalizedString("MaintenanceUIWindow.OilLifeValue");
		Color color = GetColor(oilLife, 100f);
		FillSlider(SliderTop, SliderTopText, Mathf.Round(oilLife), "%");
		SetColors(color, SliderTopText, SliderTopFillImage);
		if (!SetButtonState(num, capacity, 0.99f))
		{
			SetButtonState(100f, oilLife, 0.99f);
		}
		SetPriceText();
		ConfirmButton.onClick.AddListener(() =>
		{
			MainPanel.SetActive(value: false);
			MaintenanceConfirmWindow.Init(MaintenanceStates.Oil);
		});
	}

	private void InitCarWash()
	{
		_currentMaintenanceState = MaintenanceStates.CarWash;
		ResetPanels();
		CarWashPanel.SetActive(value: true);
		TitleText.text = new LocalizedString("MaintenanceUIWindow.CarCleanliness");
		DescriptionText.text = new LocalizedString("MaintenanceUIWindow.ExteriorCleanlinessLevelAffect");
		SetConfirmButtonText("Mainteancewindow.WashChange");
		SliderTopTitleText.text = new LocalizedString("MaintenanceUIWindow.CarCleanliness");
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: true);
		SetMinMaxValues(SliderTop, 0f, 100f);
		float num = 1f - _carInfo.Colors.DirtAmount;
		Color color = GetColor(num, 1f);
		FillSlider(SliderTop, SliderTopText, num, "%", roundToPercent: true);
		SetColors(color, SliderTopText, SliderTopFillImage);
		SetButtonState(100f, num * 100f);
		SetPriceText();
		ConfirmButton.onClick.AddListener(() =>
		{
			MainPanel.SetActive(value: false);
			MaintenanceConfirmWindow.Init(MaintenanceStates.CarWash);
		});
	}

	private void ResetPanels()
	{
		ConfirmButton.interactable = true;
		ConfirmButton.onClick.RemoveAllListeners();
		TiresGraphPanel.SetActive(value: false);
		OilGraphPanel.SetActive(value: false);
		CarWashPanel.SetActive(value: false);
		FuelPanel.SetActive(value: false);
		SliderTopTitleText.transform.parent.gameObject.SetActive(value: false);
		SliderDownTitleText.transform.parent.gameObject.SetActive(value: false);
		PricePanel.SetActive(value: true);
	}

	public void CloseConfirmationWindow()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		Init();
		CheckMaintenance();
	}

	private void SetPriceText()
	{
		string text = "";
		switch (_currentMaintenanceState)
		{
		case MaintenanceStates.TireCondition:
			text = $"{_pricesService.ChangeTiresGaragePrice}$";
			break;
		case MaintenanceStates.TirePressure:
			text = $"{_pricesService.WheelPressurePrice}$";
			break;
		case MaintenanceStates.Fuel:
			text = "";
			break;
		case MaintenanceStates.Oil:
			text = $"{_pricesService.OilPrice}$";
			break;
		case MaintenanceStates.CarWash:
			text = $"{_pricesService.CarWashGaragePrice}$";
			break;
		}
		PriceText.text = text;
		ForceRebuildLayoutPrice.RebuildLayout();
	}
}
