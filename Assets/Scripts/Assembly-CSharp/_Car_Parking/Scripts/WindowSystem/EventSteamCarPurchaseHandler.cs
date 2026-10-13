using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class EventSteamCarPurchaseHandler : ICarPurchaseHandler
	{
		private const EventType PurchaseEvent = EventType.PromoSteamPurchase;

		private readonly CarAvailabilityService _carAvailabilityService;

		private readonly ManagerUI _managerUI;

		public EventSteamCarPurchaseHandler(CarAvailabilityService carAvailabilityService, ManagerUI managerUI)
		{
			_carAvailabilityService = carAvailabilityService;
			_managerUI = managerUI;
		}

		public bool TryHandle(CarPurchaseContext context)
		{
			if (context == null || !_carAvailabilityService.IsCarUnlockedByEvent(context.CarId, EventType.PromoSteamPurchase))
			{
				return false;
			}
			context.HideShop?.Invoke();
			EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(EventType.PromoSteamPurchase))
			{
				PreviousWindow = "NewCarsWindow"
			};
			_managerUI.ShowWindow("EventViewWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, JsonConvert.SerializeObject(value)).Forget();
			return true;
		}
	}
}
