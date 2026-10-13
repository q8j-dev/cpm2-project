using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class RewardGranterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RewardGranter rewardGranter = new RewardGranter((IReadOnlyList<IGrantHandler>)resolver.ResolveOrParameter(typeof(IReadOnlyList<IGrantHandler>), "handlers", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters));
			Inject(rewardGranter, resolver, parameters);
			return rewardGranter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
