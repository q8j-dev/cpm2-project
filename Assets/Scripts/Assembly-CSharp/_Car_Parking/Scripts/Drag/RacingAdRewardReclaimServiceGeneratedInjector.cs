using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class RacingAdRewardReclaimServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingAdRewardReclaimService racingAdRewardReclaimService = new RacingAdRewardReclaimService((IAdsRewardsBackend)resolver.ResolveOrParameter(typeof(IAdsRewardsBackend), "backend", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (AdsAnalytic)resolver.ResolveOrParameter(typeof(AdsAnalytic), "adsAnalytic", parameters));
			Inject(racingAdRewardReclaimService, resolver, parameters);
			return racingAdRewardReclaimService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
