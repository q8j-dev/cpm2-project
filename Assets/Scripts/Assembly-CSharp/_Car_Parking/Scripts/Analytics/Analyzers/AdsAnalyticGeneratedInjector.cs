using System.Collections.Generic;
using CPM.Ads;
using CPM.Balance;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class AdsAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AdsAnalytic adsAnalytic = new AdsAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (IAdsService)resolver.ResolveOrParameter(typeof(IAdsService), "ads", parameters), (IBalanceDocumentStore)resolver.ResolveOrParameter(typeof(IBalanceDocumentStore), "balance", parameters));
			Inject(adsAnalytic, resolver, parameters);
			return adsAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
