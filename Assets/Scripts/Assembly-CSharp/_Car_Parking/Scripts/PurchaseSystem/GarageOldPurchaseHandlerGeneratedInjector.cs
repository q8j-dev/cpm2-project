using System.Collections.Generic;
using CPM.Connection;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class GarageOldPurchaseHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageOldPurchaseHandler garageOldPurchaseHandler = new GarageOldPurchaseHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters), (PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters));
			Inject(garageOldPurchaseHandler, resolver, parameters);
			return garageOldPurchaseHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
