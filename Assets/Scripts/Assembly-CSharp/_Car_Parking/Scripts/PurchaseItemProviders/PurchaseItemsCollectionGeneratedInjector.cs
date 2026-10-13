using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PurchaseItemProviders
{
	[Preserve]
	internal class PurchaseItemsCollectionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PurchaseItemsCollection purchaseItemsCollection = new PurchaseItemsCollection();
			Inject(purchaseItemsCollection, resolver, parameters);
			return purchaseItemsCollection;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
