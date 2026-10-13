using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class OnlineCollectionsProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OnlineCollectionsProvider onlineCollectionsProvider = new OnlineCollectionsProvider();
			Inject(onlineCollectionsProvider, resolver, parameters);
			return onlineCollectionsProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
