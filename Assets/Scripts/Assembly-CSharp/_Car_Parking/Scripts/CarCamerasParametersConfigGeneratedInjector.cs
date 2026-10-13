using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class CarCamerasParametersConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarCamerasParametersConfig carCamerasParametersConfig = new CarCamerasParametersConfig();
			Inject(carCamerasParametersConfig, resolver, parameters);
			return carCamerasParametersConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
