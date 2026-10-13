using System.Collections.Generic;
using VContainer;

namespace CPM.Connection
{
	[Preserve]
	internal class PhotonReconnectionHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotonReconnectionHandler photonReconnectionHandler = new PhotonReconnectionHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (PhotonConnectionHandler)resolver.ResolveOrParameter(typeof(PhotonConnectionHandler), "photonConnectionHandler", parameters));
			Inject(photonReconnectionHandler, resolver, parameters);
			return photonReconnectionHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
