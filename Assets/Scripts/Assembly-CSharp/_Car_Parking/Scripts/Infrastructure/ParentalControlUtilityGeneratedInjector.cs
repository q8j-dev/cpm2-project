using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class ParentalControlUtilityGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ParentalControlUtility parentalControlUtility = new ParentalControlUtility((IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(parentalControlUtility, resolver, parameters);
			return parentalControlUtility;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
