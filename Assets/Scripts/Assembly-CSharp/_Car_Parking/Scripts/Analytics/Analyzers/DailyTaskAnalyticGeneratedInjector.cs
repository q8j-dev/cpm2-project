using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class DailyTaskAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyTaskAnalytic dailyTaskAnalytic = new DailyTaskAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(dailyTaskAnalytic, resolver, parameters);
			return dailyTaskAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
