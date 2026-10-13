using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class MigrationAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MigrationAnalytic migrationAnalytic = new MigrationAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters));
			Inject(migrationAnalytic, resolver, parameters);
			return migrationAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
