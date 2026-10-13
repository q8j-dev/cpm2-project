using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class NewCarWindowConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NewCarWindowConfig newCarWindowConfig = new NewCarWindowConfig();
			Inject(newCarWindowConfig, resolver, parameters);
			return newCarWindowConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
