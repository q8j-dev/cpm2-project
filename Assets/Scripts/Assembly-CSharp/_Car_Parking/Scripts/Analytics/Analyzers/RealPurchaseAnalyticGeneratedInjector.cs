using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class RealPurchaseAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RealPurchaseAnalytic realPurchaseAnalytic = new RealPurchaseAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters));
			Inject(realPurchaseAnalytic, resolver, parameters);
			return realPurchaseAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
