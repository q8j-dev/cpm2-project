using VContainer;

namespace CPM.Connection
{
	public class PhotonReconnectionHandler
	{
		private readonly IConnectionManager connectionManager;

		private readonly PhotonConnectionHandler photonConnectionHandler;

		[Inject]
		public PhotonReconnectionHandler(IConnectionManager connectionManager, PhotonConnectionHandler photonConnectionHandler)
		{
			this.connectionManager = connectionManager;
			this.photonConnectionHandler = photonConnectionHandler;
		}

		public void ReconnectSoft(PhotonConnectionProfile profile)
		{
			if (!photonConnectionHandler.HasCachedConfig)
			{
				ReconnectHard(profile);
			}
			else
			{
				photonConnectionHandler.Reconnect(profile);
			}
		}

		public void ReconnectHard(PhotonConnectionProfile profile)
		{
			connectionManager.ReconnectPhoton(profile).Forget();
		}
	}
}
