using System.Collections.Generic;
using VContainer;

namespace CPM.Ads.Policy
{
	[Preserve]
	internal class AdOfferServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AdOfferService adOfferService = new AdOfferService((IAdsService)resolver.ResolveOrParameter(typeof(IAdsService), "ads", parameters), (IAdPlacementCatalog)resolver.ResolveOrParameter(typeof(IAdPlacementCatalog), "catalog", parameters), (IAdPolicyStore)resolver.ResolveOrParameter(typeof(IAdPolicyStore), "store", parameters), (IUtcClock)resolver.ResolveOrParameter(typeof(IUtcClock), "clock", parameters), (IAdsDailyLimits)resolver.ResolveOrParameter(typeof(IAdsDailyLimits), "dailyLimits", parameters));
			Inject(adOfferService, resolver, parameters);
			return adOfferService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
