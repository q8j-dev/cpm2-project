using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Car.Visual
{
	[Preserve]
	internal class WindshieldBrakeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WindshieldBrakeService windshieldBrakeService = new WindshieldBrakeService((WindshieldBrakeVfxPool)resolver.ResolveOrParameter(typeof(WindshieldBrakeVfxPool), "windshieldBrakeVfxPool", parameters));
			Inject(windshieldBrakeService, resolver, parameters);
			return windshieldBrakeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
