using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Backend
{
	[Preserve]
	internal class BackendClientGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BackendClient backendClient = new BackendClient((IHttpClient)resolver.ResolveOrParameter(typeof(IHttpClient), "httpClient", parameters), (BackendClient.Config)resolver.ResolveOrParameter(typeof(BackendClient.Config), "config", parameters));
			Inject(backendClient, resolver, parameters);
			return backendClient;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
