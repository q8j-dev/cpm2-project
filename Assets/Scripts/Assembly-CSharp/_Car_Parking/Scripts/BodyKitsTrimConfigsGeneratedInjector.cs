using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class BodyKitsTrimConfigsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BodyKitsTrimConfigs bodyKitsTrimConfigs = new BodyKitsTrimConfigs();
			Inject(bodyKitsTrimConfigs, resolver, parameters);
			return bodyKitsTrimConfigs;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
