using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class LevelsAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LevelsAnalytic levelsAnalytic = new LevelsAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters));
			Inject(levelsAnalytic, resolver, parameters);
			return levelsAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
