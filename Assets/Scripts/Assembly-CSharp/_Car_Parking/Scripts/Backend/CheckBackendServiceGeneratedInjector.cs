using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Backend
{
	[Preserve]
	internal class CheckBackendServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CheckBackendService checkBackendService = new CheckBackendService((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(checkBackendService, resolver, parameters);
			return checkBackendService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
