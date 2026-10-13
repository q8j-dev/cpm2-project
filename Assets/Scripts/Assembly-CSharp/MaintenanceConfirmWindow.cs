using System;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.UIElement;

public class MaintenanceConfirmWindow : MonoBehaviour
{
	private const float WheelBarStep = 0.05f;

	private const int DelayStep = 10;

	[SerializeField]
	private MaintenanceUIWindow MaintenanceUIWindow;

	[SerializeField]
	private TextMeshProUGUI TitleText;

	[SerializeField]
	private Button CancelButton;

	[SerializeField]
	private Button SetDefaultButton;

	[SerializeField]
	private Button ConfirmButton;

	[SerializeField]
	private Button AddButton;

	[SerializeField]
	private Sprite TireSprite;

	[SerializeField]
	private Sprite TirePressure;

	[SerializeField]
	private Sprite OilSprite;

	[SerializeField]
	private Sprite FuelSprite;

	[SerializeField]
	private Sprite CarWashSprite;

	[SerializeField]
	[Header("ConfirmPanel")]
	private GameObject ConfirmPanel;

	[SerializeField]
	private Image ConfirmDetailImage;

	[SerializeField]
	private TextMeshProUGUI ConfirmDetailText;

	[SerializeField]
	private Image ProgressImage;

	[SerializeField]
	[Header("ChangingTirePanel")]
	private GameObject ChangingTirePanel;

	[SerializeField]
	private Toggle FrontWheelToggle;

	[SerializeField]
	private Toggle RearWheelToggle;

	[SerializeField]
	[Header("TirePressurePanel")]
	private GameObject TirePressurePanel;

	[SerializeField]
	private SliderValueHandler FrontWheelSlider;

	[SerializeField]
	private SliderValueHandler RearWheelSlider;

	[Header("OilPanel")]
	[SerializeField]
	private GameObject OilPanel;

	[Header("CarWashPanel")]
	[SerializeField]
	private GameObject CarWashPanel;

	[SerializeField]
	[Header("FuelPanel")]
	private GameObject FuelPanel;

	[SerializeField]
	private FuelTankGadget FuelTankGadget;

	private VehicleEntityProvider _vehicleEntityProvider;

	private Powertrain _powertrain;

	private Axles _axles;

	private Action _confirmAction;

	private Action _setDefaultAction;

	private ManagerUI _managerUI;

	private ICarInfoService _carInfoService;

	private CarInfo _carInfo;

	private MaintenanceStates _maintenanceStates;

	private VehicleData _vehicleData;

	private VehicleStockTuningData _carTuningDataConfig;

	private CurrencyManagerController _currencyManagerController;

	private IPricesService _pricesService;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private LocalizedTextMeshPro _confirmButtonLabel;

	[Inject]
	public void Construct(ManagerUI managerUI, ICarInfoService carInfoService, CurrencyManagerController currencyManagerController, IPricesService pricesService, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
	{
		_pricesService = pricesService;
		_currencyManagerController = currencyManagerController;
		_managerUI = managerUI;
		_carInfoService = carInfoService;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
	}

	private void Start()
	{
		_vehicleData = V.Car.GetComponent<VehicleData>();
		SetDefaultButton.onClick.RemoveAllListeners();
		SetDefaultButton.onClick.AddListener(SetDefault);
	}

	private void ResetPanels()
	{
		ConfirmButton.SetActive(active: true);
		CancelButton.SetActive(active: true);
		ConfirmButton.interactable = true;
		CancelButton.interactable = true;
		ConfirmPanel.SetActive(value: false);
		ChangingTirePanel.SetActive(value: false);
		TirePressurePanel.SetActive(value: false);
		OilPanel.SetActive(value: false);
		FuelPanel.SetActive(value: false);
		CarWashPanel.SetActive(value: false);
		SetDefaultButton.SetActive(active: false);
		AddButton.transform.parent.gameObject.SetActive(value: false);
		ConfirmButton.onClick.RemoveAllListeners();
		ConfirmButton.onClick.AddListener(ShowConfirmPanel);
	}

	public async UniTaskVoid Prepare()
	{
		_powertrain = V.Car.GetComponent<Powertrain>();
		_axles = V.Car.GetComponent<Axles>();
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		CancelButton.onClick.AddListener(OnCancel);
		_vehicleEntityProvider = V.Car.GetComponent<VehicleEntityProvider>();
		_vehicleData = V.Car.GetComponent<VehicleData>();
		_carTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(_vehicleData.CarId);
	}

	public void Init(MaintenanceStates maintenanceStates)
	{
		if (maintenanceStates != MaintenanceStates.Fuel)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(maintenanceStates, 0, PurchaseStatus.PURCHASE_CLICKED);
		}
		_maintenanceStates = maintenanceStates;
		base.gameObject.SetActive(value: true);
		ResetPanels();
		switch (maintenanceStates)
		{
		case MaintenanceStates.TirePressure:
			ShowTirePressurePanel();
			break;
		case MaintenanceStates.Oil:
			ShowOilPanel();
			break;
		case MaintenanceStates.TireCondition:
			ShowChangingTirePanel();
			break;
		case MaintenanceStates.CarWash:
			ShowCarWashPanel();
			break;
		case MaintenanceStates.Fuel:
			ShowFuelPanel();
			break;
		}
	}

	private void ShowFuelPanel()
	{
		_confirmAction = null;
		SetTitleText(new LocalizedString("MaintenanceConfirmWindow.Fuel"), new LocalizedString("CarBlock.CarInService"), FuelSprite);
		SetConfirmButtonText("Mainteancewindow.FuelChange");
		ConfirmButton.onClick.RemoveAllListeners();
		FuelTankGadget.Bought -= ShowConfirmPanel;
		FuelTankGadget.Bought += ShowConfirmPanel;
		FuelPanel.SetActive(value: true);
		FuelTankGadget.Init("");
	}

	private void SetTitleText(string titleText, string confirmationTitle, Sprite sprite)
	{
		TitleText.text = titleText;
		ConfirmDetailText.text = confirmationTitle;
		ConfirmDetailImage.sprite = sprite;
	}

	private void SetConfirmButtonText(string localizationKey)
	{
		if (!_confirmButtonLabel)
		{
			_confirmButtonLabel = ConfirmButton.GetComponentInChildren<LocalizedTextMeshPro>(includeInactive: true);
		}
		_confirmButtonLabel.SetKey(localizationKey);
	}

	private void ShowConfirmPanel()
	{
		if (_maintenanceStates != MaintenanceStates.TireCondition || FrontWheelToggle.isOn || RearWheelToggle.isOn)
		{
			ResetPanels();
			ConfirmPanel.SetActive(value: true);
			ConfirmButton.SetActive(active: false);
			CancelButton.SetActive(active: false);
			StartProgress().Forget();
		}
	}

	private void ShowChangingTirePanel()
	{
		ResetPanels();
		SetTitleText(new LocalizedString("MaintenanceConfirmWindow.changingtires"), new LocalizedString("CarBlock.CarInService"), TireSprite);
		SetConfirmButtonText("Mainteancewindow.Change");
		ChangingTirePanel.SetActive(value: true);
		FrontWheelToggle.SetIsOnWithoutNotify(value: false);
		RearWheelToggle.SetIsOnWithoutNotify(value: false);
		_confirmAction = SetTireHealth;
	}

	private void SetTireHealth()
	{
		AdvancedInt advancedInt = new AdvancedInt(_pricesService.ChangeTiresGaragePrice);
		if (advancedInt != 0)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.ChangeTiresGaragePrice, PurchaseStatus.PURCHASE_CONFIRMED);
		}
		_currencyManagerController.TryMoneySpend(out var _, advancedInt, () =>
		{
			if (FrontWheelToggle.isOn)
			{
				_axles.SetTyreHealth(_axles.FrontAxle, 1f, _axles.FrontAxle.LeftWheel.TyreType);
				_vehicleEntityProvider.CarEntity.SetNewFrontTyres();
			}
			if (RearWheelToggle.isOn)
			{
				_axles.SetTyreHealth(_axles.RearAxle, 1f, _axles.RearAxle.LeftWheel.TyreType);
				if (_axles.Rear1Axle.Wheels.Count > 0)
				{
					_axles.SetTyreHealth(_axles.Rear1Axle, 1f, _axles.Rear1Axle.LeftWheel.TyreType);
				}
				if (_axles.Rear2Axle.Wheels.Count > 0)
				{
					_axles.SetTyreHealth(_axles.Rear2Axle, 1f, _axles.Rear2Axle.LeftWheel.TyreType);
				}
				_vehicleEntityProvider.CarEntity.SetNewRearTyres();
			}
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.ChangeTiresGaragePrice, PurchaseStatus.PURCHASE_COMPLETED);
		}, () =>
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.ChangeTiresGaragePrice, PurchaseStatus.INSUFFICIENT_FUNDS);
		});
	}

	private void ShowTirePressurePanel()
	{
		ResetPanels();
		ConfirmButton.interactable = false;
		SetDefaultButton.SetActive(active: true);
		SetTitleText(new LocalizedString("MaintenanceConfirmWindow.changingtirespressure"), new LocalizedString("CarBlock.CarInService"), TirePressure);
		SetConfirmButtonText("Mainteancewindow.TyreChange");
		TirePressurePanel.SetActive(value: true);
		TyreType tyreType = _carInfo.TuningData.FrontWheelData.tyreType;
		TyreType tyreType2 = _carInfo.TuningData.RearWheelData.tyreType;
		float pressure = _carTuningDataConfig.Tuning.FrontWheelData.Pressure;
		float pressure2 = _carTuningDataConfig.Tuning.RearWheelData.Pressure;
		float minWheelPressure = _carTuningDataConfig.Tuning.FrontWheelDataLimits.GetMinWheelPressure(tyreType);
		float maxWheelPressure = _carTuningDataConfig.Tuning.FrontWheelDataLimits.GetMaxWheelPressure(pressure, tyreType);
		float minWheelPressure2 = _carTuningDataConfig.Tuning.RearWheelDataLimits.GetMinWheelPressure(tyreType2);
		float maxWheelPressure2 = _carTuningDataConfig.Tuning.RearWheelDataLimits.GetMaxWheelPressure(pressure2, tyreType2);
		_powertrain.GetTyrePressure(AxleType.Front, out var pressure3);
		FrontWheelSlider.Init(minWheelPressure, maxWheelPressure, 0.05f);
		float value = Utils.Pa2Bar(pressure3);
		FrontWheelSlider.SetValue(value);
		_powertrain.GetTyrePressure(AxleType.Rear, out pressure3);
		value = Utils.Pa2Bar(pressure3);
		RearWheelSlider.Init(minWheelPressure2, maxWheelPressure2, 0.05f);
		RearWheelSlider.SetValue(value);
		FrontWheelSlider.AddListener(OnSliderValueChanged);
		RearWheelSlider.AddListener(OnSliderValueChanged);
		_confirmAction = SetTiresPressure;
		_setDefaultAction = SetDefaulPressure;
	}

	private void OnSliderValueChanged(float value)
	{
		ConfirmButton.interactable = true;
	}

	private void SetTiresPressure()
	{
		AdvancedInt advancedInt = new AdvancedInt(_pricesService.WheelPressurePrice);
		if (advancedInt != 0)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.WheelPressurePrice, PurchaseStatus.PURCHASE_CONFIRMED);
		}
		_currencyManagerController.TryMoneySpend(out var _, advancedInt, () =>
		{
			float value = FrontWheelSlider.GetValue();
			float value2 = RearWheelSlider.GetValue();
			_powertrain.SetTyrePressure(AxleType.Front, value);
			_powertrain.SetTyrePressure(AxleType.Rear, value2);
			_carInfo.TuningData.FrontWheelData.Pressure = value;
			_carInfo.TuningData.RearWheelData.Pressure = value2;
			_carInfo.Save();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.WheelPressurePrice, PurchaseStatus.PURCHASE_COMPLETED);
		}, () =>
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.WheelPressurePrice, PurchaseStatus.INSUFFICIENT_FUNDS);
		});
	}

	private void SetDefaulPressure()
	{
		RearWheelSlider.SetValue(MaintenanceUIWindow.GetDefaultTirePressure(isFront: false));
		FrontWheelSlider.SetValue(MaintenanceUIWindow.GetDefaultTirePressure(isFront: true));
	}

	private void ShowOilPanel()
	{
		ResetPanels();
		SetTitleText(new LocalizedString("Mainteancewindow.CHANgingoil"), new LocalizedString("CarBlock.CarInService"), OilSprite);
		SetConfirmButtonText("Mainteancewindow.OilChange");
		OilPanel.SetActive(value: true);
		_confirmAction = ChangeOil;
	}

	private void ChangeOil()
	{
		AdvancedInt advancedInt = new AdvancedInt(_pricesService.OilPrice);
		if (advancedInt != 0)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.OilPrice, PurchaseStatus.PURCHASE_CONFIRMED);
		}
		_currencyManagerController.TryMoneySpend(out var _, advancedInt, () =>
		{
			if (_vehicleEntityProvider.CarEntity.Engine.RotorHousing != null && (bool)_vehicleEntityProvider.CarEntity.Engine.RotorHousing.Part)
			{
				_vehicleEntityProvider.CarEntity.ChangeOil(_vehicleEntityProvider.CarEntity.Engine.RotorHousing.Part.OilCapacity);
			}
			else
			{
				_vehicleEntityProvider.CarEntity.ChangeOil(_vehicleEntityProvider.CarEntity.Engine.Block.Part.OilCapacity);
			}
			_vehicleEntityProvider.TrySaveMileage();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.OilPrice, PurchaseStatus.PURCHASE_COMPLETED);
		}, () =>
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.OilPrice, PurchaseStatus.INSUFFICIENT_FUNDS);
		});
	}

	public void FillTank()
	{
		ShowConfirmPanel();
	}

	private void CleanCar()
	{
		AdvancedInt advancedInt = new AdvancedInt(_pricesService.CarWashGaragePrice);
		if (advancedInt != 0)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.CarWashGaragePrice, PurchaseStatus.PURCHASE_CLICKED);
		}
		_currencyManagerController.TryMoneySpend(out var _, advancedInt, () =>
		{
			_carInfo.Colors.DirtAmount = 0f;
			_vehicleData.CarMat.SetFloat("_MaskValue", _carInfo.Colors.DirtAmount);
			_vehicleData.DirtComposition.ResetCarDirt();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.CarWashGaragePrice, PurchaseStatus.PURCHASE_CONFIRMED);
		}, () =>
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, _pricesService.CarWashGaragePrice, PurchaseStatus.INSUFFICIENT_FUNDS);
		});
	}

	private void ShowCarWashPanel()
	{
		SetTitleText(new LocalizedString("MaintenanceConfirmWindow.washingcar"), new LocalizedString("CarBlock.CarInService"), CarWashSprite);
		SetConfirmButtonText("Mainteancewindow.WashChange");
		CarWashPanel.SetActive(value: true);
		_confirmAction = CleanCar;
	}

	private async UniTaskVoid StartProgress()
	{
		for (int i = 0; i < 100; i++)
		{
			await UniTask.Delay(10);
			ProgressImage.fillAmount = (float)i * 0.1f;
		}
		_confirmAction?.Invoke();
		if ((bool)MaintenanceUIWindow)
		{
			ResetPanels();
			Close();
		}
	}

	private void SetDefault()
	{
		_setDefaultAction?.Invoke();
	}

	private void OnCancel()
	{
		if (_maintenanceStates != MaintenanceStates.Fuel)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Maintenance(_maintenanceStates, 0, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
		}
		Close();
	}

	private void Close()
	{
		if ((bool)FuelTankGadget)
		{
			FuelTankGadget.Bought -= ShowConfirmPanel;
		}
		base.gameObject.SetActive(value: false);
		MaintenanceUIWindow.CloseConfirmationWindow();
	}
}
