using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class GlobalCollectionsProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GlobalCollectionsProvider globalCollectionsProvider = new GlobalCollectionsProvider();
			Inject(globalCollectionsProvider, resolver, parameters);
			return globalCollectionsProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
