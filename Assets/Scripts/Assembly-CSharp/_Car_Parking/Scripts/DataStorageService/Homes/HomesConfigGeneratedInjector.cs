using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DataStorageService.Homes
{
	[Preserve]
	internal class HomesConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			HomesConfig homesConfig = new HomesConfig();
			Inject(homesConfig, resolver, parameters);
			return homesConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
