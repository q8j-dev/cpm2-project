using System.Collections.Generic;
using CPM.Ads.Policy;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class AdsDailyLimitsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AdsDailyLimitsService adsDailyLimitsService = new AdsDailyLimitsService((IAdsRewardsBackend)resolver.ResolveOrParameter(typeof(IAdsRewardsBackend), "backend", parameters), (IUtcClock)resolver.ResolveOrParameter(typeof(IUtcClock), "clock", parameters));
			Inject(adsDailyLimitsService, resolver, parameters);
			return adsDailyLimitsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
