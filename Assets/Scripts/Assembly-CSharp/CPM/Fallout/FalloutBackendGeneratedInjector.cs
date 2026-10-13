using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.Fallout
{
	[Preserve]
	internal class FalloutBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FalloutBackend falloutBackend = new FalloutBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(falloutBackend, resolver, parameters);
			return falloutBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
