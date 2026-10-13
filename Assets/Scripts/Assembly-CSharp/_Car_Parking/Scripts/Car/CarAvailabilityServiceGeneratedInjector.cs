using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Car
{
	[Preserve]
	internal class CarAvailabilityServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarAvailabilityService carAvailabilityService = new CarAvailabilityService((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (CarShopVisualService)resolver.ResolveOrParameter(typeof(CarShopVisualService), "carShopVisualService", parameters), (CarShopPurchaseService)resolver.ResolveOrParameter(typeof(CarShopPurchaseService), "carShopPurchaseService", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters));
			Inject(carAvailabilityService, resolver, parameters);
			return carAvailabilityService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
