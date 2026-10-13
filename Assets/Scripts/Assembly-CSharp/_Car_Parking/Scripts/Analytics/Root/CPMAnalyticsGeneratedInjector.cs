using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.Analytics.Root.Data;

namespace _Car_Parking.Scripts.Analytics.Root
{
	[Preserve]
	internal class CPMAnalyticsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CPMAnalytics cPMAnalytics = new CPMAnalytics((IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters), (AnalyticConfig)resolver.ResolveOrParameter(typeof(AnalyticConfig), "analyticConfig", parameters));
			Inject(cPMAnalytics, resolver, parameters);
			return cPMAnalytics;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
