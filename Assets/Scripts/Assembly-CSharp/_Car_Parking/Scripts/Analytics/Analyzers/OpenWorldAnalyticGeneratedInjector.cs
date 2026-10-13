using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class OpenWorldAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OpenWorldAnalytic openWorldAnalytic = new OpenWorldAnalytic((LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters), (AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(openWorldAnalytic, resolver, parameters);
			return openWorldAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
