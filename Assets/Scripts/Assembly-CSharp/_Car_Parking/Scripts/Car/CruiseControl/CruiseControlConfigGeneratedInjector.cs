using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	[Preserve]
	internal class CruiseControlConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CruiseControlConfig cruiseControlConfig = new CruiseControlConfig();
			Inject(cruiseControlConfig, resolver, parameters);
			return cruiseControlConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
