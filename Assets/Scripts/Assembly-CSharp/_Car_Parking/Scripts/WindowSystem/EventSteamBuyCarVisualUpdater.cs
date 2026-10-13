using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class EventSteamBuyCarVisualUpdater : ICarShopVisualUpdater
	{
		private const EventType PurchaseEvent = EventType.PromoSteamPurchase;

		private const string BuyLabelKey = "CarShop.Explore";

		private readonly CarAvailabilityService _carAvailabilityService;

		public EventSteamBuyCarVisualUpdater(CarAvailabilityService carAvailabilityService)
		{
			_carAvailabilityService = carAvailabilityService;
		}

		public void UpdateCard(DealerUIElement card, int carId)
		{
			if (IsPurchaseEventCar(carId))
			{
				if ((bool)card.SideIcon)
				{
					card.SideIcon.gameObject.SetActive(value: true);
				}
				if ((bool)card.PriceRoot)
				{
					card.PriceRoot.SetActive(value: false);
				}
				if ((bool)card.NewCarMark)
				{
					card.NewCarMark.SetActive(value: false);
				}
			}
		}

		public void UpdateBuyButton(ButtonBuyNewCar button, int carId)
		{
			if (IsPurchaseEventCar(carId))
			{
				if ((bool)button.PriceRoot)
				{
					button.PriceRoot.SetActive(value: false);
				}
				if ((bool)button.Label)
				{
					button.Label.SetKey("CarShop.Explore");
				}
			}
		}

		private bool IsPurchaseEventCar(int carId)
		{
			return _carAvailabilityService.IsCarUnlockedByEvent(carId, EventType.PromoSteamPurchase);
		}
	}
}
