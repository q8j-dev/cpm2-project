using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.Connection
{
	[Preserve]
	internal class FirebaseConnectionServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseConnectionService firebaseConnectionService = new FirebaseConnectionService((CheckBackendService)resolver.ResolveOrParameter(typeof(CheckBackendService), "checkBackendService", parameters));
			Inject(firebaseConnectionService, resolver, parameters);
			return firebaseConnectionService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
