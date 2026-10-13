using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	internal class OfferBigBannerViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:OfferBigBannerView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OfferBigBannerView obj = (OfferBigBannerView)instance;
			IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
			obj.ConstructBase(purchaseSystem);
		}
	}
}
