using System.Collections.Generic;
using CPM.Common;
using VContainer;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	[Preserve]
	internal class AnalyticsBackgroundFlusherGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AnalyticsBackgroundFlusher analyticsBackgroundFlusher = new AnalyticsBackgroundFlusher((AppStateHandler)resolver.ResolveOrParameter(typeof(AppStateHandler), "appStateHandler", parameters), (CPMAnalytics)resolver.ResolveOrParameter(typeof(CPMAnalytics), "analytics", parameters));
			Inject(analyticsBackgroundFlusher, resolver, parameters);
			return analyticsBackgroundFlusher;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
