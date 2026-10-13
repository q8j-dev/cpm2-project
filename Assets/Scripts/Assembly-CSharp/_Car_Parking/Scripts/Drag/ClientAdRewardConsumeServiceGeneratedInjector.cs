using System.Collections.Generic;
using CPM.Ads.Policy;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class ClientAdRewardConsumeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ClientAdRewardConsumeService clientAdRewardConsumeService = new ClientAdRewardConsumeService((IAdsRewardsBackend)resolver.ResolveOrParameter(typeof(IAdsRewardsBackend), "backend", parameters), (IAdsDailyLimits)resolver.ResolveOrParameter(typeof(IAdsDailyLimits), "dailyLimits", parameters));
			Inject(clientAdRewardConsumeService, resolver, parameters);
			return clientAdRewardConsumeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
