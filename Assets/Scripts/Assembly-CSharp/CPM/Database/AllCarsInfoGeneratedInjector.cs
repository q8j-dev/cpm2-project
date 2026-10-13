using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace CPM.Database
{
	[Preserve]
	internal class AllCarsInfoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllCarsInfo allCarsInfo = new AllCarsInfo();
			Inject(allCarsInfo, resolver, parameters);
			return allCarsInfo;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllCarsInfo obj = (AllCarsInfo)instance;
			IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
			obj.Construct(remoteConfigManager);
		}
	}
}
