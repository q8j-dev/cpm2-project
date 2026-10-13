using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class PurchaseSystemPurchaseProductsAdderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PurchaseSystemPurchaseProductsAdder purchaseSystemPurchaseProductsAdder = new PurchaseSystemPurchaseProductsAdder((IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters), (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (CheckBackendService)resolver.ResolveOrParameter(typeof(CheckBackendService), "checkBackendService", parameters), (IOfflineModeSystem)resolver.ResolveOrParameter(typeof(IOfflineModeSystem), "offlineModeSystem", parameters));
			Inject(purchaseSystemPurchaseProductsAdder, resolver, parameters);
			return purchaseSystemPurchaseProductsAdder;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
