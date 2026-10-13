using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Realty
{
	[Preserve]
	internal class RealtyConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RealtyConfig realtyConfig = new RealtyConfig();
			Inject(realtyConfig, resolver, parameters);
			return realtyConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
