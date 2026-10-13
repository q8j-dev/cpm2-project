using System.Collections.Generic;
using CPM.Connection;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class PhotonArenaNetworkGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotonArenaNetwork photonArenaNetwork = new PhotonArenaNetwork((PhotonReconnectionHandler)resolver.ResolveOrParameter(typeof(PhotonReconnectionHandler), "reconnect", parameters));
			Inject(photonArenaNetwork, resolver, parameters);
			return photonArenaNetwork;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
