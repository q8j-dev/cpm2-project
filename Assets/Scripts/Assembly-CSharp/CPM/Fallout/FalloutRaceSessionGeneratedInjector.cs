using System.Collections.Generic;
using CommonUtility.Contracts;
using VContainer;

namespace CPM.Fallout
{
	[Preserve]
	internal class FalloutRaceSessionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FalloutRaceSession falloutRaceSession = new FalloutRaceSession((IFalloutBackend)resolver.ResolveOrParameter(typeof(IFalloutBackend), "backend", parameters), (FalloutLimitsCache)resolver.ResolveOrParameter(typeof(FalloutLimitsCache), "falloutLimitsCache", parameters), (IRewardWallet)resolver.ResolveOrParameter(typeof(IRewardWallet), "rewardWallet", parameters), (IReadOnlyList<IFalloutFinishHandler>)resolver.ResolveOrParameter(typeof(IReadOnlyList<IFalloutFinishHandler>), "finishHandlers", parameters));
			Inject(falloutRaceSession, resolver, parameters);
			return falloutRaceSession;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
