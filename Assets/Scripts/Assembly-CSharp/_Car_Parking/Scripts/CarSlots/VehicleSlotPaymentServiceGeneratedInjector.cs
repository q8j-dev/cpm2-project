using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	internal class VehicleSlotPaymentServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VehicleSlotPaymentService vehicleSlotPaymentService = new VehicleSlotPaymentService((PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters));
			Inject(vehicleSlotPaymentService, resolver, parameters);
			return vehicleSlotPaymentService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
