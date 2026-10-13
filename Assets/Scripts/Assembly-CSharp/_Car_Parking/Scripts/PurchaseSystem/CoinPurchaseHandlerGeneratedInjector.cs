using System.Collections.Generic;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class CoinPurchaseHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CoinPurchaseHandler coinPurchaseHandler = new CoinPurchaseHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters));
			Inject(coinPurchaseHandler, resolver, parameters);
			return coinPurchaseHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
