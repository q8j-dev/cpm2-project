using Newtonsoft.Json;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class BigOfferCarsWindow : BigOfferWindowBase
	{
		protected override void ButtonBuyClickCallback()
		{
			base.ButtonBuyClickCallback();
			ButtonCloseClickCallback();
			_offerSystem.SetCurrentOfferID(OfferBigBannerView.CurrentOfferModel.ID);
			_purchaseSystem.BuyProductID(OfferBigBannerView.CurrentOfferModel.Payload.FirstBundleID);
		}

		protected override void ButtonViewInShopClickCallback()
		{
			base.ButtonViewInShopClickCallback();
			PrepareToClose();
			_managerUI.CloseWindow("EventViewWindow");
			if (!(_garage == null))
			{
				_garage.LastSelectedCar = OfferBigBannerView.CarId;
			}
			DealerWindowAdditionalData dealerWindowAdditionalData = new DealerWindowAdditionalData();
			dealerWindowAdditionalData.UseFilter = true;
			dealerWindowAdditionalData.ShowOnlySpecial = true;
			dealerWindowAdditionalData.TargetCarId = OfferBigBannerView.CarId;
			_managerUI.ShowNewCarsWindow(JsonConvert.SerializeObject(dealerWindowAdditionalData));
			_coreGameManager.OpenSellCars();
			_managerUI.ShowUserBalanceWindow();
		}
	}
}
