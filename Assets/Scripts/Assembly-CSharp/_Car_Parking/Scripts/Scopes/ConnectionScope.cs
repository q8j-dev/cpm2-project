using CPM.Account;
using CPM.Connection;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	public static class ConnectionScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<ConnectionHandler>(Lifetime.Singleton).As<IAuthorizationDependant>().As<IConnectionManager>()
				.As<IAuthorizationFailureDependant>()
				.As<IConnectionManagerBase>()
				.AsSelf();
			builder.Register<RemoteConfigDataHandler>(Lifetime.Singleton);
			builder.Register<PhotonConnectionHandler>(Lifetime.Singleton);
			builder.Register<PhotonReconnectionHandler>(Lifetime.Singleton);
			builder.Register<ConnectionDataHandler>(Lifetime.Singleton).As<IConnectionDataHandler>();
		}
	}
}
