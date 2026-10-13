using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class ClientAdRewardRecoveryServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClientAdRewardRecoveryService clientAdRewardRecoveryService = new ClientAdRewardRecoveryService((IAdsRewardsBackend)resolver.ResolveOrParameter(typeof(IAdsRewardsBackend), "backend", parameters), (ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timers", parameters), (IEngineTuningTimerService)resolver.ResolveOrParameter(typeof(IEngineTuningTimerService), "engineTimers", parameters), (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IRacingAdRewardReclaimService)resolver.ResolveOrParameter(typeof(IRacingAdRewardReclaimService), "racingAdRewardReclaim", parameters), (AdsAnalytic)resolver.ResolveOrParameter(typeof(AdsAnalytic), "adsAnalytic", parameters));
			Inject(clientAdRewardRecoveryService, resolver, parameters);
			return clientAdRewardRecoveryService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
