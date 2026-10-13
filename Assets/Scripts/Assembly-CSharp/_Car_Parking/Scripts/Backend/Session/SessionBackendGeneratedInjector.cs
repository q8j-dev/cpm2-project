using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Backend.Session
{
	[Preserve]
	internal class SessionBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SessionBackend sessionBackend = new SessionBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(sessionBackend, resolver, parameters);
			return sessionBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
