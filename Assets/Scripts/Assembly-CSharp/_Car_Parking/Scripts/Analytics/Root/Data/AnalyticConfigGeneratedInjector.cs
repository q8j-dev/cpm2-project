using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Analytics.Root.Data
{
	[Preserve]
	internal class AnalyticConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AnalyticConfig analyticConfig = new AnalyticConfig();
			Inject(analyticConfig, resolver, parameters);
			return analyticConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
