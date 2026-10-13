using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Preserve]
	internal class RallyPropControllerConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyPropControllerConfig rallyPropControllerConfig = new RallyPropControllerConfig();
			Inject(rallyPropControllerConfig, resolver, parameters);
			return rallyPropControllerConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
