using Cysharp.Threading.Tasks;
using Services.PriceStyleService;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProperty;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ConfirmApartmentBuyWindow : ConfirmBuyWindowBase
	{
		private CurrencyManagerController _currencyManagerController;

		private HomesConfig _homesConfig;

		private PriceStyleConfig _priceStyleConfig;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private PropertyTriggerService _propertyTriggerService;

		private OptionalDependency<PropertyTriggerService> _propertyTriggerServiceDep;

		private FreeDriveControl _freeDriveControl;

		private OptionalDependency<FreeDriveControl> _freeDriveControlDep;

		private int _buyingHouseIndex = -1;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, HomesConfig homesConfig, PriceStyleConfig priceStyleConfig, VirtualPurchaseAnalytic virtualPurchaseAnalytic, OptionalDependency<PropertyTriggerService> propertyTriggerService, OptionalDependency<FreeDriveControl> freeDriveControl)
		{
			_homesConfig = homesConfig;
			_currencyManagerController = currencyManagerController;
			_priceStyleConfig = priceStyleConfig;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_propertyTriggerServiceDep = propertyTriggerService;
			_freeDriveControlDep = freeDriveControl;
		}

		protected override void Awake()
		{
			base.Awake();
			_propertyTriggerService = _propertyTriggerServiceDep?.Value;
			_freeDriveControl = _freeDriveControlDep?.Value;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (int.TryParse(data, out var result))
			{
				_buyingHouseIndex = result;
			}
			InitUI();
		}

		protected override void OnCancel()
		{
			base.OnCancel();
			OpenPreviousWindowAndClose();
			_propertyTriggerService?.CreateButton().Forget();
			LogApartmentEvent(PurchaseStatus.PURCHASE_CANCELED_BY_USER);
		}

		protected override void OnBuy()
		{
			base.OnBuy();
			LogApartmentEvent(PurchaseStatus.PURCHASE_CONFIRMED);
			BuyApartment().Forget();
		}

		private void InitUI()
		{
			LogApartmentEvent(PurchaseStatus.PURCHASE_CLICKED);
			HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(_buyingHouseIndex);
			SetMainView(homeDataItem.MainImageAddressablePath, new LocalizedString(homeDataItem.Name));
			PriceStyleData byType = _priceStyleConfig.GetByType(homeDataItem.PriceType);
			SetPrice(byType.PriceSprite, homeDataItem.GetPrice.ToString());
		}

		private async UniTask BuyApartment()
		{
			HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(_buyingHouseIndex);
			if ((!_freeDriveControl) ? (await ApartmentsBuyer.BuyHouseFromMenu(_currencyManagerController, homeDataItem, OpenPreviousWindowAndClose)) : (await _freeDriveControl.ProcessApartmentPurchase(homeDataItem)))
			{
				LogApartmentEvent(PurchaseStatus.PURCHASE_COMPLETED);
				if ((bool)_freeDriveControl)
				{
					Close();
				}
			}
			else
			{
				LogApartmentEvent(PurchaseStatus.INSUFFICIENT_FUNDS);
			}
		}

		private void OpenPreviousWindowAndClose()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowApartmentSelectWindow();
			}
			Close();
		}

		private void LogApartmentEvent(PurchaseStatus status)
		{
			HomesConfig.HomeDataItem homeDataItem = _homesConfig.Get(_buyingHouseIndex);
			CurrencyItemType currencyItemType = ((homeDataItem.PriceType != CostType.Coins) ? CurrencyItemType.money : CurrencyItemType.coins);
			string windowId = (ScenesHelper.IsMenuSceneActive() ? "ConfirmApartmentBuyWindow" : "");
			_virtualPurchaseAnalytic.LogPurchaseVirtual_Apartment(homeDataItem.Index, currencyItemType, homeDataItem.GetPrice, windowId, status);
		}
	}
}
