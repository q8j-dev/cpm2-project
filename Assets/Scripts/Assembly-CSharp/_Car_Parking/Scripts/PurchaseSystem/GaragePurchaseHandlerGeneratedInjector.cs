using System.Collections.Generic;
using CPM.Connection;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class GaragePurchaseHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GaragePurchaseHandler garagePurchaseHandler = new GaragePurchaseHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters), (PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters));
			Inject(garagePurchaseHandler, resolver, parameters);
			return garagePurchaseHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
