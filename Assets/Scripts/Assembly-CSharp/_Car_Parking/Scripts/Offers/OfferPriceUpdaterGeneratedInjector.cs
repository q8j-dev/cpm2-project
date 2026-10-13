using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	internal class OfferPriceUpdaterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OfferPriceUpdater offerPriceUpdater = new OfferPriceUpdater((IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters), (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters));
			Inject(offerPriceUpdater, resolver, parameters);
			return offerPriceUpdater;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
