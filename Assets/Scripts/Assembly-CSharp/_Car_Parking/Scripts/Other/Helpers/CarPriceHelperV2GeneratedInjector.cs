using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class CarPriceHelperV2GeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarPriceHelperV2 carPriceHelperV = new CarPriceHelperV2((IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters));
			Inject(carPriceHelperV, resolver, parameters);
			return carPriceHelperV;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
