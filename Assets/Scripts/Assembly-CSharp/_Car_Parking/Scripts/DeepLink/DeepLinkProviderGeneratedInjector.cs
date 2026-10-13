using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.DeepLink
{
	[Preserve]
	internal class DeepLinkProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DeepLinkProvider deepLinkProvider = new DeepLinkProvider();
			Inject(deepLinkProvider, resolver, parameters);
			return deepLinkProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
