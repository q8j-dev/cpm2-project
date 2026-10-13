using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	internal class CarSlotsObserverGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarSlotsObserver carSlotsObserver = new CarSlotsObserver((ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters));
			Inject(carSlotsObserver, resolver, parameters);
			return carSlotsObserver;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
