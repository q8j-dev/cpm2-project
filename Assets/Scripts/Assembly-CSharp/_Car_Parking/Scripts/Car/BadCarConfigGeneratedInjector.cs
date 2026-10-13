using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car
{
	[Preserve]
	internal class BadCarConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BadCarConfig badCarConfig = new BadCarConfig();
			Inject(badCarConfig, resolver, parameters);
			return badCarConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
