using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	[Preserve]
	internal class ProgressBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ProgressBackend progressBackend = new ProgressBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(progressBackend, resolver, parameters);
			return progressBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
