using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	internal class PlayerProfileBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerProfileBackend playerProfileBackend = new PlayerProfileBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(playerProfileBackend, resolver, parameters);
			return playerProfileBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
