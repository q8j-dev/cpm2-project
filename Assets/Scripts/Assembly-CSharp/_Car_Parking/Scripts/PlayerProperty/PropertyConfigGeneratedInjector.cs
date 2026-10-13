using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProperty
{
	[Preserve]
	internal class PropertyConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PropertyConfig propertyConfig = new PropertyConfig();
			Inject(propertyConfig, resolver, parameters);
			return propertyConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
