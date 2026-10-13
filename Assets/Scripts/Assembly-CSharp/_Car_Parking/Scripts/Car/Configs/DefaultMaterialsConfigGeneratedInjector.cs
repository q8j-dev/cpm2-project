using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Configs
{
	[Preserve]
	internal class DefaultMaterialsConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DefaultMaterialsConfig defaultMaterialsConfig = new DefaultMaterialsConfig();
			Inject(defaultMaterialsConfig, resolver, parameters);
			return defaultMaterialsConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
