using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class RacingAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingAnalytic racingAnalytic = new RacingAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters));
			Inject(racingAnalytic, resolver, parameters);
			return racingAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
