using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Steam
{
	[Preserve]
	internal class SteamItemCatalogGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamItemCatalog steamItemCatalog = new SteamItemCatalog((PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters));
			Inject(steamItemCatalog, resolver, parameters);
			return steamItemCatalog;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
