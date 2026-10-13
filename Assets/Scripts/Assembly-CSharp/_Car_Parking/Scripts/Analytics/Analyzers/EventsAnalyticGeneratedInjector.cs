using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class EventsAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventsAnalytic eventsAnalytic = new EventsAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(eventsAnalytic, resolver, parameters);
			return eventsAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
