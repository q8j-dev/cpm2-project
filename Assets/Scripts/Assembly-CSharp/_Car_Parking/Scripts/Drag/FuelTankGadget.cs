using System;
using CPM.Ads.Policy;
using CPM.Database;
using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Ads.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Drag
{
	public class FuelTankGadget : MonoBehaviour
	{
		private VehicleEntityProvider _vehicleEntityMb;

		[SerializeField]
		private TextMeshProUGUI DisplayText = default;

		[SerializeField]
		private Slider GasSlider = default;

		[SerializeField]
		private TextMeshProUGUI CostText = default;

		[SerializeField]
		private Button BuyButton = default;

		[SerializeField]
		private Button AdButton = default;

		[SerializeField]
		private GameObject AdButtonRoot = default;

		[SerializeField]
		private CanvasGroup AdButtonCanvasGroup = default;

		[SerializeField]
		private TMP_Text AdCooldownLabel = default;

		[SerializeField]
		private Toggle[] FuelTypesToggles = default;

		[SerializeField]
		private bool IgnorePrefixText = default;

		[SerializeField]
		private Button MinusButton = default;

		[SerializeField]
		private Button PlusButton = default;

		[SerializeField]
		private Sprite CoinIcon = default;

		[SerializeField]
		private Sprite MoneyIcon = default;

		[SerializeField]
		private Image MoneyIconImage = default;

		[SerializeField]
		private Image MoneyPriceIconImage = default;

		[SerializeField]
		private Image BuyButtonBackground = default;

		[SerializeField]
		private Color MoneyButtonColor = default;

		[SerializeField]
		private Color CoinButtonColor = default;

		private int _maxTankCapacity;

		private int _currentFuel;

		private int _initialFuel;

		private int _fuelToLoad;

		private FuelType _initialBenzin;

		private FuelType _benzin;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private ICarTuningFactory _carTuningFactory;

		private const int E85CoinsPerLiter = 1;

		private const float DisabledAdTextAlpha = 0.4f;

		private const string BuyButtonLocalizationKey = "Mainteancewindow.FuelChange";

		private ObscuredInt _fuelPrice;

		private ObscuredInt _costCoef = 49;

		private static readonly ObscuredFloat PriceInFreeCoef = 2f;

		private int _sliderStep = 1;

		private GameObject _car;

		private string _parentWindow;

		private CurrencyManagerController _currencyManagerController;

		private IPricesService _pricesService;

		private FreeDriveProvider _freeDriveProvider;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private ManagerUI _managerUI;

		private IAdOfferService _adOffers;

		private IClientAdRewardConsumeService _adRewardConsume;

		private bool _rewardedInProgress;

		private string _adContextId;

		private IDisposable _adPoll;

		private AdCooldownTimerView _adCooldownTimer;

		private readonly AdOfferMessages _adMessages = new AdOfferMessages();

		private AdCooldownTimerView AdCooldownTimer => _adCooldownTimer ?? (_adCooldownTimer = new AdCooldownTimerView(AdCooldownLabel));

		public bool CanBuy
		{
			get
			{
				if (_initialBenzin == _benzin)
				{
					return !Mathf.Approximately(_initialFuel, GasSlider.value);
				}
				return true;
			}
		}

		public float FuelToLoad => _fuelToLoad;

		private bool PayWithCoins => _benzin == FuelType.GasE85;

		private OfferState AdOfferState
		{
			get
			{
				if (_adOffers != null)
				{
					return _adOffers.CanOffer(AdPlacement.FuelRefill, _adContextId);
				}
				return OfferState.Hidden;
			}
		}

		public event Action Bought;

		[Inject]
		public void Construct(ICarInfoService carInfoService, ICarTuningFactory carTuningFactory, PlayerDataStorage playerDataStorage, ManagerUI managerUI, CurrencyManagerController currencyManagerController, IPricesService pricesService, FreeDriveProvider freeDriveProvider, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IAdOfferService adOffers, IClientAdRewardConsumeService adRewardConsume)
		{
			_adOffers = adOffers;
			_adRewardConsume = adRewardConsume;
			_managerUI = managerUI;
			_freeDriveProvider = freeDriveProvider;
			_pricesService = pricesService;
			_currencyManagerController = currencyManagerController;
			_carTuningFactory = carTuningFactory;
			_carInfoService = carInfoService;
			_pricesService = pricesService;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		}

		public void Init(string parentWindow, bool onlyCurrentBenzin = false)
		{
			_car = (V.Car ? V.Car : V.LastUsedCar);
			if (!_car || !_car.TryGetComponent<VehicleEntityProvider>(out _vehicleEntityMb))
			{
				return;
			}
			_parentWindow = parentWindow;
			_adContextId = Guid.NewGuid().ToString("N");
			SetBuyButtonText();
			bool flag = true;
			if (FlagsStorage.IsMultiFree)
			{
				flag = _car == _freeDriveProvider.FirstCarMultiPlayer;
			}
			if (flag)
			{
				_vehicleEntityMb.TrySaveMileage();
			}
			_carInfo = (flag ? _carInfoService.GetFromSlot(V.SlotID) : _car.GetComponent<CarInfoContainer>().CarInfo);
			_maxTankCapacity = _carInfo.GetDefaultTuningConfig().ChassisData.FuelTankCapacity;
			_currentFuel = (int)_vehicleEntityMb.CarEntity.MaintenanceInfo.FuelInTank;
			_currentFuel = Mathf.Clamp(_currentFuel, 0, _maxTankCapacity);
			bool flag2 = _vehicleEntityMb.CarEntity.Engine.FuelRail.GetFuelType == FuelType.Diesel;
			for (int i = 0; i < FuelTypesToggles.Length; i++)
			{
				bool flag3 = i == 4;
				bool flag4 = flag2 == flag3;
				if (!flag4)
				{
					FuelTypesToggles[i].isOn = false;
				}
				FuelTypesToggles[i].gameObject.SetActive(flag4);
			}
			_initialBenzin = _vehicleEntityMb.CarEntity.Engine.GetBenzin;
			_benzin = _initialBenzin;
			if (flag2 != (_benzin == FuelType.Diesel))
			{
				_benzin = (flag2 ? FuelType.Diesel : FuelType.Gas95);
			}
			_initialFuel = _currentFuel;
			if ((bool)DisplayText)
			{
				DisplayText.text = $"{_currentFuel} / {_maxTankCapacity}";
			}
			FuelTypesToggles[(int)_benzin].isOn = true;
			if ((bool)GasSlider)
			{
				GasSlider.minValue = 1f;
				GasSlider.maxValue = _maxTankCapacity;
				GasSlider.value = _currentFuel;
				GasSlider.wholeNumbers = true;
				_sliderStep = 1;
			}
			OnToggleChanged((int)_benzin);
			if (onlyCurrentBenzin)
			{
				for (int j = 0; j < FuelTypesToggles.Length; j++)
				{
					FuelTypesToggles[j].interactable = _benzin == (FuelType)j;
					if (!FuelTypesToggles[j].interactable && FuelTypesToggles[j].TryGetComponentInChildren<TMP_Text>(out var component))
					{
						component.color = new Color(0f, 0f, 0f, 0.3f);
					}
				}
			}
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Fuel(0, "", _parentWindow, PurchaseStatus.PURCHASE_CLICKED);
		}

		private void ShowAdvertising()
		{
			ShowAdvertisingAsync().Forget();
		}

		private async UniTaskVoid ShowAdvertisingAsync()
		{
			if (_rewardedInProgress)
			{
				return;
			}
			OfferState adOfferState = AdOfferState;
			if (!adOfferState.CanShowNow)
			{
				if (_adMessages.TryGetBlockedKey(adOfferState, out var key))
				{
					_managerUI.ShowNotificationWindow(new LocalizedString(key), 3f).Forget();
				}
				return;
			}
			int fuelToLoad = _maxTankCapacity - _currentFuel;
			FuelType benzin = _benzin;
			if (fuelToLoad <= 0)
			{
				return;
			}
			_rewardedInProgress = true;
			_managerUI.ShowWaitingWindow(set: true).Forget();
			bool ownSlotCar = _carInfoService.TryGetFromSlot(V.SlotID, out var carInfo) && carInfo == _carInfo;
			if (ownSlotCar)
			{
				PendingAdRewardPrefs.Save(AdPlacement.FuelRefill, V.SlotID, carInfo.CarID);
			}
			try
			{
				if (!(await _adOffers.TryShowAsync(AdPlacement.FuelRefill, _adContextId)).IsGranted)
				{
					if (ownSlotCar)
					{
						PendingAdRewardPrefs.Clear(AdPlacement.FuelRefill);
					}
					return;
				}
				ApplyPurchasedFuel(fuelToLoad, benzin, 0);
				if (ownSlotCar)
				{
					PendingAdRewardPrefs.Clear(AdPlacement.FuelRefill);
				}
				_adRewardConsume.ConsumeInBackground(AdPlacement.FuelRefill);
			}
			finally
			{
				_rewardedInProgress = false;
				if ((bool)_managerUI)
				{
					_managerUI.CloseWaitingWindow();
				}
				if ((bool)this)
				{
					UpdatePaymentView();
					_managerUI.OnBackButtonActivated?.Invoke(obj: true);
				}
			}
		}

		private void OnEnable()
		{
			AddListeners();
			FullRecalculate();
			_adPoll = (from _ in Observable.Interval(TimeSpan.FromSeconds(1.0), Scheduler.MainThreadIgnoreTimeScale)
				where !_rewardedInProgress && _carInfo != null && (bool)_vehicleEntityMb
				select _).Subscribe((long _) =>
			{
				UpdatePaymentView();
			});
		}

		private void OnDisable()
		{
			_adPoll?.Dispose();
			_adPoll = null;
			if ((bool)AdButtonRoot)
			{
				AdButtonRoot.SetActive(value: false);
			}
			RemoveListeners();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Fuel(_fuelPrice, _benzin.ToString(), _parentWindow, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
		}

		private void AddListeners()
		{
			if ((bool)BuyButton)
			{
				BuyButton.onClick.AddListener(BuyGas);
			}
			if ((bool)AdButton)
			{
				AdButton.onClick.AddListener(ShowAdvertising);
			}
			if ((bool)GasSlider)
			{
				GasSlider.onValueChanged.AddListener(SliderGas);
			}
			if ((bool)PlusButton)
			{
				PlusButton.onClick.AddListener(FuelPlusButtonClick);
			}
			if ((bool)MinusButton)
			{
				MinusButton.onClick.AddListener(FuelMinusButtonClick);
			}
			for (int i = 0; i < FuelTypesToggles.Length; i++)
			{
				int index = i;
				FuelTypesToggles[i].onValueChanged.AddListener((bool isOn) =>
				{
					if (isOn)
					{
						OnToggleChanged(index);
					}
				});
			}
		}

		private void RemoveListeners()
		{
			if ((bool)BuyButton)
			{
				BuyButton.onClick.RemoveAllListeners();
			}
			if ((bool)AdButton)
			{
				AdButton.onClick.RemoveAllListeners();
			}
			if ((bool)GasSlider)
			{
				GasSlider.onValueChanged.RemoveAllListeners();
			}
			if ((bool)PlusButton)
			{
				PlusButton.onClick.RemoveAllListeners();
			}
			if ((bool)MinusButton)
			{
				MinusButton.onClick.RemoveAllListeners();
			}
			for (int i = 0; i < FuelTypesToggles.Length; i++)
			{
				FuelTypesToggles[i].onValueChanged.RemoveAllListeners();
			}
		}

		private void FuelPlusButtonClick()
		{
			GasSlider.value += _sliderStep;
		}

		private void FuelMinusButtonClick()
		{
			GasSlider.value -= _sliderStep;
		}

		private void SliderGas(float value)
		{
			FullRecalculate();
		}

		private void FullRecalculate()
		{
			if (_carInfo != null && (bool)_vehicleEntityMb && (bool)GasSlider)
			{
				RecaluculateCurrent();
				CalculateFuelToLoad();
				_fuelPrice = CalcPrice(_costCoef, _fuelToLoad, _maxTankCapacity);
				UpdateDisplayFuel();
				UpdatePaymentView();
			}
		}

		private void UpdatePaymentView()
		{
			if ((bool)AdButton && (bool)AdButtonRoot)
			{
				OfferState adOfferState = AdOfferState;
				AdButtonRoot.SetActive(base.isActiveAndEnabled && PayWithCoins && adOfferState.IsOfferable);
				SetAdButtonEnabled(adOfferState.CanShowNow);
				AdCooldownTimer.SetCooldown(_adMessages.CooldownSeconds(adOfferState));
			}
			if ((bool)BuyButtonBackground)
			{
				BuyButtonBackground.color = (PayWithCoins ? CoinButtonColor : MoneyButtonColor);
			}
			Sprite sprite = (PayWithCoins ? CoinIcon : MoneyIcon);
			if ((bool)MoneyIconImage && (bool)sprite)
			{
				MoneyIconImage.sprite = sprite;
			}
			if ((bool)MoneyPriceIconImage && (bool)sprite)
			{
				MoneyPriceIconImage.sprite = sprite;
			}
		}

		private void SetBuyButtonText()
		{
			if ((bool)BuyButton)
			{
				LocalizedTextMeshPro componentInChildren = BuyButton.GetComponentInChildren<LocalizedTextMeshPro>(includeInactive: true);
				if ((bool)componentInChildren)
				{
					componentInChildren.SetKey("Mainteancewindow.FuelChange");
				}
			}
		}

		private void SetAdButtonEnabled(bool enabled)
		{
			if ((bool)AdButtonCanvasGroup)
			{
				AdButtonCanvasGroup.alpha = (enabled ? 1f : 0.4f);
			}
		}

		private void OnToggleChanged(int index)
		{
			switch (index)
			{
			case 0:
				_benzin = FuelType.Gas95;
				_costCoef = _pricesService.Fuel95Price;
				break;
			case 1:
				_benzin = FuelType.Gas98;
				_costCoef = _pricesService.Fuel98Price;
				break;
			case 2:
				_benzin = FuelType.Gas100;
				_costCoef = _pricesService.Fuel100Price;
				break;
			case 3:
				_benzin = FuelType.GasE85;
				_costCoef = 1;
				break;
			case 4:
				_benzin = FuelType.Diesel;
				_costCoef = _pricesService.FuelDieselPrice;
				break;
			default:
				_benzin = FuelType.Gas95;
				_costCoef = _pricesService.Fuel95Price;
				break;
			}
			FullRecalculate();
		}

		private void RecaluculateCurrent()
		{
			bool flag = true;
			if (FlagsStorage.IsMultiFree)
			{
				flag = _car == _freeDriveProvider.FirstCarMultiPlayer;
			}
			if (flag)
			{
				_currentFuel = ((_carInfo.TuningData.DynamicTuningSettings.FuelType == _benzin) ? ((int)_carInfo.TuningData.Maintenance.FuelInTank) : 0);
			}
			else
			{
				_currentFuel = ((_vehicleEntityMb.CarEntity.Engine.GetBenzin == _benzin) ? ((int)_vehicleEntityMb.CarEntity.MaintenanceInfo.FuelInTank) : 0);
			}
		}

		private void CalculateFuelToLoad()
		{
			_fuelToLoad = (int)GasSlider.value - _currentFuel;
		}

		private ObscuredInt CalcPrice(ObscuredInt coef, float fuelToLoad, float maxTankCapacity)
		{
			int num = (int)fuelToLoad * (int)coef;
			if (FlagsStorage.IsFreeFuelWindow && !PayWithCoins)
			{
				num = (int)((float)num * (float)PriceInFreeCoef);
			}
			num = (int)Mathf.Clamp(num, 0f, maxTankCapacity * (float)(int)coef);
			return num;
		}

		private void BuyGas()
		{
			if (!CanBuy)
			{
				return;
			}
			ObscuredInt fuelPrice = _fuelPrice;
			int fuelToLoad = _fuelToLoad;
			FuelType benzin = _benzin;
			if (!PayWithCoins)
			{
				ConfirmedBuyGas(fuelPrice, fuelToLoad, benzin);
				return;
			}
			string format = new LocalizedString("VisualTuningWindow.Buythisitemfor");
			_managerUI.ShowConfirmWindow(() =>
			{
				ConfirmedBuyGas(fuelPrice, fuelToLoad, benzin);
			}, new LocalizedString("Playerprofilewindow.Message"), string.Format(format, (int)fuelPrice)).Forget();
		}

		private void ConfirmedBuyGas(ObscuredInt fuelPrice, int fuelToLoad, FuelType benzin)
		{
			AdvancedInt price = new AdvancedInt(fuelPrice);
			if ((int)fuelPrice != 0)
			{
				_virtualPurchaseAnalytic.LogPurchaseVirtual_Fuel(fuelPrice, benzin.ToString(), _parentWindow, PurchaseStatus.PURCHASE_CONFIRMED);
			}
			if (benzin == FuelType.GasE85)
			{
				BuyGasForCoins(price, fuelToLoad, benzin).Forget();
				return;
			}
			_currencyManagerController.TryMoneySpend(out var _, price, () =>
			{
				ApplyPurchasedFuel(fuelToLoad, benzin);
			}, LogInsufficientFunds);
		}

		private async UniTaskVoid BuyGasForCoins(AdvancedInt price, int fuelToLoad, FuelType benzin)
		{
			if ((bool)BuyButton)
			{
				BuyButton.interactable = false;
			}
			await _currencyManagerController.TryCoinsSpend(SpendCoinsType.Others, price, () =>
			{
				ApplyPurchasedFuel(fuelToLoad, benzin);
			}, LogInsufficientFunds);
			UpdateBuyButton();
		}

		private void ApplyPurchasedFuel(int fuelToLoad, FuelType benzin)
		{
			ApplyPurchasedFuel(fuelToLoad, benzin, _fuelPrice);
		}

		private void ApplyPurchasedFuel(int fuelToLoad, FuelType benzin, ObscuredInt paidPrice)
		{
			int num = (_currentFuel += fuelToLoad);
			PlayerPrefs.SetInt("buyFuelCount", num);
			_vehicleEntityMb.Refuel(num);
			_carInfo.TuningData.DynamicTuningSettings.FuelType = benzin;
			_carInfo.TuningData.Maintenance.FuelInTank = num;
			_vehicleEntityMb.CarEntity.RecalculateEngine(_vehicleEntityMb, _carTuningFactory, _carInfo);
			UpdateBuyButton();
			SendFuelData();
			Bought?.Invoke();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Fuel(paidPrice, benzin.ToString(), _parentWindow, PurchaseStatus.PURCHASE_COMPLETED);
		}

		private void LogInsufficientFunds()
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Fuel(_fuelPrice, _benzin.ToString(), _parentWindow, PurchaseStatus.INSUFFICIENT_FUNDS);
		}

		private void SendFuelData()
		{
			if (!ScenesHelper.IsMenuSceneActive())
			{
				int num = Mathf.Abs(_currentFuel);
				int num2 = ObscuredPrefs.GetInt("allFuelConsumed");
				ObscuredPrefs.SetInt("allFuelConsumed", num2 + num);
				if ((bool)_freeDriveProvider.FreeDriveDB)
				{
					_freeDriveProvider.FreeDriveDB.fuelConsumed = num;
				}
			}
		}

		private void UpdateDisplayFuel()
		{
			if ((bool)DisplayText)
			{
				DisplayText.text = $"{Mathf.RoundToInt(_currentFuel + _fuelToLoad)} / {_maxTankCapacity}";
			}
			if ((bool)CostText)
			{
				CostText.text = (IgnorePrefixText ? "" : "Cost: <sprite name=money>") + $"{_fuelPrice}";
			}
			UpdateBuyButton();
		}

		private void UpdateBuyButton()
		{
			if ((bool)BuyButton && (bool)_car && _car.TryGetComponent<VehicleEntityProvider>(out _vehicleEntityMb))
			{
				BuyButton.interactable = true;
			}
		}
	}
}
