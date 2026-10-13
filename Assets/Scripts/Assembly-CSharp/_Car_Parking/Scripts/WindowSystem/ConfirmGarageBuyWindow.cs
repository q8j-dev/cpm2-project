using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services.PriceStyleService;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Garage.GarageSellService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProperty;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ConfirmGarageBuyWindow : ConfirmBuyWindowBase
	{
		[UnityEngine.Scripting.Preserve]
		public struct AdditionalData
		{
			public int GarageIndex;

			public string PreviousWindow;
		}

		[SerializeField]
		private TextMeshProUGUI Capacity;

		[SerializeField]
		private TextMeshProUGUI Location;

		[SerializeField]
		private RealCurrencyView GaragePriceView;

		private GaragePositionsConfig _garageConfig;

		private PriceStyleConfig _priceStyleConfig;

		private IGarageBuyService _garageBuyService;

		private IGarageSellService _garageSellService;

		private IPurchaseSystem _purchaseSystem;

		private PropertyTriggerService _propertyTriggerService;

		private OptionalDependency<PropertyTriggerService> _propertyTriggerServiceDep;

		private int _currentGarage = -1;

		private AdditionalData _additionalData;

		private ParkingMenuUI _parkingMenuUI;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(GaragePositionsConfig garageConfig, PriceStyleConfig priceStyleConfig, IGarageBuyService garageBuyService, IPurchaseSystem purchaseSystem, IGarageSellService garageSellService, OptionalDependency<ParkingMenuUI> parkingMenuUI, OptionalDependency<PropertyTriggerService> propertyTriggerService)
		{
			_parkingMenuUIDep = parkingMenuUI;
			_garageConfig = garageConfig;
			_priceStyleConfig = priceStyleConfig;
			_garageBuyService = garageBuyService;
			_purchaseSystem = purchaseSystem;
			_propertyTriggerServiceDep = propertyTriggerService;
			_garageSellService = garageSellService;
		}

		protected override void Awake()
		{
			base.Awake();
			_parkingMenuUI = _parkingMenuUIDep?.Value;
			_propertyTriggerService = _propertyTriggerServiceDep?.Value;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (!string.IsNullOrEmpty(data))
			{
				_additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
				InitUI(_additionalData.GarageIndex);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_garageBuyService.GarageBought += OnGarageBought;
			_purchaseSystem.OnInitializedCompletedEvent += PurchaseSystemInitializedCallback;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_purchaseSystem.OnInitializedCompletedEvent -= PurchaseSystemInitializedCallback;
			if (_garageBuyService != null)
			{
				_garageBuyService.GarageBought -= OnGarageBought;
			}
		}

		private void PurchaseSystemInitializedCallback()
		{
			UpdatePrice();
		}

		private void OnGarageBought(int _)
		{
			OpenPreviousWindowAndClose();
			RateAppHelper.RateAppAsync();
		}

		private void InitUI(int garageIndex)
		{
			_currentGarage = garageIndex;
			GarageData garageData = _garageConfig.Get(garageIndex);
			Capacity.text = garageData.MaxCarSpotCount.ToString();
			Location.text = new LocalizedString(garageData.LocationKey);
			SetMainView(garageData.GarageIconAddressablePath, new LocalizedString(garageData.NameLocalizedKey));
			UpdatePrice();
		}

		protected override void OnCancel()
		{
			base.OnCancel();
			_propertyTriggerService?.CreateButton().Forget();
			OpenPreviousWindowAndClose();
		}

		protected override void OnBuy()
		{
			base.OnBuy();
			BuyGarage().Forget();
		}

		private async UniTask BuyGarage()
		{
			await ManagerUIHelper.ShowWaitingWindow(set: true);
			PurchaseItemType garagePurchaseType = await _garageSellService.GetGaragePurchaseItemType();
			ManagerUIHelper.CloseWaitingWindow().Forget();
			await UniTask.Yield();
			_purchaseSystem.BuyProductID(garagePurchaseType);
		}

		private void OpenPreviousWindowAndClose()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				if (string.IsNullOrEmpty(_additionalData.PreviousWindow))
				{
					_managerUI.ShowGarageSelectWindow();
				}
				else
				{
					_managerUI.ShowWindow(_additionalData.PreviousWindow).Forget();
				}
			}
			else if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.Show();
			}
			Close();
		}

		private void UpdatePrice()
		{
			PurchaseItemType purchaseItemType = PurchaseItemType.Garage1;
			PurchaseProduct purchaseProduct = _purchaseSystem.GetPurchaseProduct(purchaseItemType);
			GarageData garageData = _garageConfig.Get(_currentGarage);
			PriceStyleData byType = _priceStyleConfig.GetByType(garageData.PriceType);
			SetPrice(byType.PriceSprite, (!FlagsStorage.IsPurchaserInitialized) ? garageData.GetPrice.ToString() : (purchaseProduct?.LocalizedPriceText ?? ""), showPriceIcon: false);
			GaragePriceView.UpdateText(purchaseProduct?.LocalizedPriceText ?? "");
		}
	}
}
