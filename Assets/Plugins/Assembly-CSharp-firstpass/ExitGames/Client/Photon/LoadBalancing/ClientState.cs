using System;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum ClientState
	{
		PeerCreated = 0,
		Authenticating = 1,
		Authenticated = 2,
		JoinedLobby = 3,
		DisconnectingFromMasterserver = 4,
		ConnectingToGameserver = 5,
		ConnectedToGameserver = 6,
		Joining = 7,
		Joined = 8,
		Leaving = 9,
		DisconnectingFromGameserver = 10,
		ConnectingToMasterserver = 11,
		Disconnecting = 12,
		Disconnected = 13,
		ConnectedToMasterserver = 14,
		[Obsolete("Renamed to ConnectedToMasterserver.")]
		ConnectedToMaster = ConnectedToMasterserver,
		ConnectingToNameServer = 15,
		ConnectedToNameServer = 16,
		DisconnectingFromNameServer = 17
	}
}
