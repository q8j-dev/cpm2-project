using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[Preserve]
	internal class RallyCrowdControllerConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyCrowdControllerConfig rallyCrowdControllerConfig = new RallyCrowdControllerConfig();
			Inject(rallyCrowdControllerConfig, resolver, parameters);
			return rallyCrowdControllerConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
