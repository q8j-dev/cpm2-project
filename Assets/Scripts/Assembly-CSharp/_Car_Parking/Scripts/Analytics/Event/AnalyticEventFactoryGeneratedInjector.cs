using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event
{
	[Preserve]
	internal class AnalyticEventFactoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AnalyticEventFactory analyticEventFactory = new AnalyticEventFactory((CPMAnalytics)resolver.ResolveOrParameter(typeof(CPMAnalytics), "analytics", parameters));
			Inject(analyticEventFactory, resolver, parameters);
			return analyticEventFactory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
