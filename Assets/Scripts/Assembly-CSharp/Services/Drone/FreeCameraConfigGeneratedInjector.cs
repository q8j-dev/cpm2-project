using System.Collections.Generic;
using VContainer;

namespace Services.Drone
{
	[Preserve]
	internal class FreeCameraConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeCameraConfig freeCameraConfig = new FreeCameraConfig();
			Inject(freeCameraConfig, resolver, parameters);
			return freeCameraConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
