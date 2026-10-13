using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	[Preserve]
	internal class WheelSurfaceVfxServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelSurfaceVfxService wheelSurfaceVfxService = new WheelSurfaceVfxService((IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters));
			Inject(wheelSurfaceVfxService, resolver, parameters);
			return wheelSurfaceVfxService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
