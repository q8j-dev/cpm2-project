using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class InputItemsProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InputItemsProvider inputItemsProvider = new InputItemsProvider();
			Inject(inputItemsProvider, resolver, parameters);
			return inputItemsProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
