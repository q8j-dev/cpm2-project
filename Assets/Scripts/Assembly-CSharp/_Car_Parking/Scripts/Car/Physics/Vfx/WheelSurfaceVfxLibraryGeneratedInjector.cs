using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	[Preserve]
	internal class WheelSurfaceVfxLibraryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelSurfaceVfxLibrary wheelSurfaceVfxLibrary = new WheelSurfaceVfxLibrary();
			Inject(wheelSurfaceVfxLibrary, resolver, parameters);
			return wheelSurfaceVfxLibrary;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
