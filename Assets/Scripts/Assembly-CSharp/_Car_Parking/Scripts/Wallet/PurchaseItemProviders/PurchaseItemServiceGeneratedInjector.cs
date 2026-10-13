using System.Collections.Generic;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	[Preserve]
	internal class PurchaseItemServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PurchaseItemService purchaseItemService = new PurchaseItemService((CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletService", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters));
			Inject(purchaseItemService, resolver, parameters);
			return purchaseItemService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
