using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class CommonProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CommonProvider commonProvider = new CommonProvider();
			Inject(commonProvider, resolver, parameters);
			return commonProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
