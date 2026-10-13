using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	[Preserve]
	internal class SummaryOfEventsConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SummaryOfEventsConfig summaryOfEventsConfig = new SummaryOfEventsConfig();
			Inject(summaryOfEventsConfig, resolver, parameters);
			return summaryOfEventsConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
