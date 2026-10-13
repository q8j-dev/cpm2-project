using System.Collections.Generic;
using CPM.Ads.Policy;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	internal class EngineTimerAdServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineTimerAdService engineTimerAdService = new EngineTimerAdService((ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timers", parameters), (IEngineTuningTimerService)resolver.ResolveOrParameter(typeof(IEngineTuningTimerService), "engineTimers", parameters), (IAdOfferService)resolver.ResolveOrParameter(typeof(IAdOfferService), "adOffers", parameters), (IClientAdRewardConsumeService)resolver.ResolveOrParameter(typeof(IClientAdRewardConsumeService), "consume", parameters));
			Inject(engineTimerAdService, resolver, parameters);
			return engineTimerAdService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
