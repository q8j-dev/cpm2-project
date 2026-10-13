using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class AlterChaseCameraConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AlterChaseCameraConfig alterChaseCameraConfig = new AlterChaseCameraConfig();
			Inject(alterChaseCameraConfig, resolver, parameters);
			return alterChaseCameraConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
