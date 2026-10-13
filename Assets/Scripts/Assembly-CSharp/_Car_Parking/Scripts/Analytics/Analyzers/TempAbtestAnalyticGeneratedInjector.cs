using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class TempAbtestAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TempAbtestAnalytic tempAbtestAnalytic = new TempAbtestAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(tempAbtestAnalytic, resolver, parameters);
			return tempAbtestAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
