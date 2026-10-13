using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace CPM.Connection
{
	[Preserve]
	internal class ConnectionHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ConnectionHandler connectionHandler = new ConnectionHandler((PhotonConnectionHandler)resolver.ResolveOrParameter(typeof(PhotonConnectionHandler), "photonConnectionHandler", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters), (RemoteConfigDataHandler)resolver.ResolveOrParameter(typeof(RemoteConfigDataHandler), "remoteConfigDataHandler", parameters), (CheckBackendService)resolver.ResolveOrParameter(typeof(CheckBackendService), "checkBackendService", parameters));
			Inject(connectionHandler, resolver, parameters);
			return connectionHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
