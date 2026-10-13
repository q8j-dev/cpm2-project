using System.Collections.Generic;
using CPM.Account;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	internal class VehicleSlotManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VehicleSlotManager vehicleSlotManager = new VehicleSlotManager((IStorageService)resolver.ResolveOrParameter(typeof(IStorageService), "storageService", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters), (VehicleSlotPaymentService)resolver.ResolveOrParameter(typeof(VehicleSlotPaymentService), "vehicleSlotPaymentService", parameters), (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters), (AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters));
			Inject(vehicleSlotManager, resolver, parameters);
			return vehicleSlotManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
