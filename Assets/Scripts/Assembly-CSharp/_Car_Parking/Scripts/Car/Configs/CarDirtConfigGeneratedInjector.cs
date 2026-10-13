using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Configs
{
	[Preserve]
	internal class CarDirtConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarDirtConfig carDirtConfig = new CarDirtConfig();
			Inject(carDirtConfig, resolver, parameters);
			return carDirtConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
