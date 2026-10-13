using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Car.Visual;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class WindshieldBrakeVfxPoolGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WindshieldBrakeVfxPool windshieldBrakeVfxPool = new WindshieldBrakeVfxPool((WindshieldBrakeVfx)resolver.ResolveOrParameter(typeof(WindshieldBrakeVfx), "prefab", parameters));
			Inject(windshieldBrakeVfxPool, resolver, parameters);
			return windshieldBrakeVfxPool;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
