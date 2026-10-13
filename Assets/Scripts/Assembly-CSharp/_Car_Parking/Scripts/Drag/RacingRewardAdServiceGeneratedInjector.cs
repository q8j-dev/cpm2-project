using System.Collections.Generic;
using CPM.Ads.Policy;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class RacingRewardAdServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingRewardAdService racingRewardAdService = new RacingRewardAdService((IAdOfferService)resolver.ResolveOrParameter(typeof(IAdOfferService), "adOffers", parameters), (IAdsRewardsBackend)resolver.ResolveOrParameter(typeof(IAdsRewardsBackend), "backend", parameters), (AdsAnalytic)resolver.ResolveOrParameter(typeof(AdsAnalytic), "adsAnalytic", parameters), (IAdsDailyLimits)resolver.ResolveOrParameter(typeof(IAdsDailyLimits), "dailyLimits", parameters));
			Inject(racingRewardAdService, resolver, parameters);
			return racingRewardAdService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
