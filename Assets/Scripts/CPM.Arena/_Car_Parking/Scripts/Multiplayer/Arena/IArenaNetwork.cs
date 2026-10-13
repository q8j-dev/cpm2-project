using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Connection;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IArenaNetwork
	{
		bool InRoom { get; }

		bool IsMasterClient { get; }

		int PlayerCount { get; }

		double ServerTime { get; }

		double TimeEndDeadline { get; }

		string RoomLobbyId { get; }

		string RoomName { get; }

		bool IsFriendsRoom { get; }

		bool AllPlayersReady { get; }

		event Action OnJoinedRoom;

		event Action OnCreatedRoom;

		event Action OnRoomStateChanged;

		event Action<byte, object> OnArenaEvent;

		event Action<short, string> OnEnterRoomFailed;

		UniTask ReconnectAndEnterLobbyAsync(PhotonConnectionProfile profile, CancellationToken ct);

		UniTask EnterLobbyFromRoomAsync(CancellationToken ct);

		ArenaLobbyState ReadLobbyState();

		void WriteLobbyState(ArenaLobbyState state);

		void SetRoomOpen(bool open);

		void WriteRoomProperty(string key, object value);

		object ReadRoomProperty(string key);

		IReadOnlyList<ArenaPlayerSnapshot> ReadPlayers();

		void SetLocalNickName(string name);

		void SetPlayerReady(bool ready);

		void WriteLocalPlayerProperty(string key, object value);

		object ReadPlayerProperty(int actor, string key);

		void LeaveRoom();

		void Disconnect();

		void RaiseArenaEvent(byte code, object payload);
	}
}
