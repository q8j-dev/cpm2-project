using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class PhotonArenaNetwork : IArenaNetwork, IInRoomCallbacks, IMatchmakingCallbacks, IOnEventCallback, IDisposable
	{
		private readonly PhotonReconnectionHandler _reconnect;

		private ArenaLobbyState _masterState;

		private const string ReadyKey = "arReady";

		private const string PasswordRoomPropKey = "P";

		public bool InRoom => PhotonNetwork.InRoom;

		public bool IsMasterClient => PhotonNetwork.IsMasterClient;

		public int PlayerCount => PhotonNetwork.CurrentRoom?.PlayerCount ?? 0;

		public double ServerTime => PhotonNetwork.Time;

		public double TimeEndDeadline
		{
			get
			{
				ArenaLobbyState arenaLobbyState = ReadLobbyState();
				if (arenaLobbyState != null)
				{
					return arenaLobbyState.Deadline - ServerTime;
				}
				return 0.0;
			}
		}

		public string RoomLobbyId => (ReadRoomProperty("aL") as string) ?? string.Empty;

		public string RoomName => PhotonNetwork.CurrentRoom?.Name ?? string.Empty;

		public bool IsFriendsRoom
		{
			get
			{
				if (ReadRoomProperty("P") is int num)
				{
					return num != 0;
				}
				return false;
			}
		}

		public bool AllPlayersReady
		{
			get
			{
				if (PhotonNetwork.CurrentRoom == null)
				{
					return false;
				}
				Player[] playerList = PhotonNetwork.PlayerList;
				if (playerList.Length == 0)
				{
					return false;
				}
				Player[] array = playerList;
				bool flag = default;
				for (int i = 0; i < array.Length; i++)
				{
					int num;
					if (array[i].CustomProperties.TryGetValue("arReady", out var value))
					{
						if (value is bool)
						{
							flag = (bool)value;
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
					else
					{
						num = 0;
					}
					if (((uint)num & (flag ? 1u : 0u)) == 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public event Action OnJoinedRoom;

		public event Action OnCreatedRoom;

		public event Action OnRoomStateChanged;

		public event Action<byte, object> OnArenaEvent;

		public event Action<short, string> OnEnterRoomFailed;

		public PhotonArenaNetwork(PhotonReconnectionHandler reconnect)
		{
			_reconnect = reconnect;
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public async UniTask ReconnectAndEnterLobbyAsync(PhotonConnectionProfile profile, CancellationToken ct)
		{
			_reconnect.ReconnectHard(profile);
			await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.Server == ServerConnection.MasterServer, PlayerLoopTiming.Update, ct).Timeout(TimeSpan.FromSeconds(30.0));
			if (!PhotonNetwork.InLobby)
			{
				PhotonNetwork.JoinLobby();
			}
			await UniTask.WaitUntil(() => PhotonNetwork.InLobby, PlayerLoopTiming.Update, ct).Timeout(TimeSpan.FromSeconds(15.0));
		}

		public async UniTask EnterLobbyFromRoomAsync(CancellationToken ct)
		{
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.LeaveRoom();
			}
			await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.Server == ServerConnection.MasterServer && !PhotonNetwork.InRoom, PlayerLoopTiming.Update, ct).Timeout(TimeSpan.FromSeconds(30.0));
			if (!PhotonNetwork.InLobby)
			{
				PhotonNetwork.JoinLobby();
			}
			await UniTask.WaitUntil(() => PhotonNetwork.InLobby, PlayerLoopTiming.Update, ct).Timeout(TimeSpan.FromSeconds(15.0));
		}

		public ArenaLobbyState ReadLobbyState()
		{
			if (IsMasterClient && PhotonNetwork.InRoom)
			{
				return _masterState ?? (_masterState = ArenaLobbyState.FromRoomProperties(PhotonNetwork.CurrentRoom?.CustomProperties));
			}
			return ArenaLobbyState.FromRoomProperties(PhotonNetwork.CurrentRoom?.CustomProperties);
		}

		public void WriteLobbyState(ArenaLobbyState state)
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
			{
				_masterState = state;
				PhotonNetwork.CurrentRoom.SetCustomProperties(state.ToRoomProperties());
			}
		}

		public void WriteRoomProperty(string key, object value)
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
			{
				PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { key, value } });
			}
		}

		public object ReadRoomProperty(string key)
		{
			Hashtable hashtable = PhotonNetwork.CurrentRoom?.CustomProperties;
			if (hashtable != null && hashtable.TryGetValue(key, out var value))
			{
				return value;
			}
			return null;
		}

		public IReadOnlyList<ArenaPlayerSnapshot> ReadPlayers()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return Array.Empty<ArenaPlayerSnapshot>();
			}
			Player[] playerList = PhotonNetwork.PlayerList;
			ArenaPlayerSnapshot[] array = new ArenaPlayerSnapshot[playerList.Length];
			for (int i = 0; i < playerList.Length; i++)
			{
				array[i] = new ArenaPlayerSnapshot(playerList[i].ActorNumber, playerList[i].NickName);
			}
			return array;
		}

		public void SetLocalNickName(string name)
		{
			PhotonNetwork.NickName = name;
		}

		public void SetRoomOpen(bool open)
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
			{
				PhotonNetwork.CurrentRoom.IsOpen = open;
			}
		}

		public void SetPlayerReady(bool ready)
		{
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "arReady", ready } });
			}
		}

		public void WriteLocalPlayerProperty(string key, object value)
		{
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { key, value } });
			}
		}

		public object ReadPlayerProperty(int actor, string key)
		{
			Player player = PhotonNetwork.CurrentRoom?.GetPlayer(actor);
			if (player?.CustomProperties != null && player.CustomProperties.TryGetValue(key, out var value))
			{
				return value;
			}
			return null;
		}

		public void LeaveRoom()
		{
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.LeaveRoom();
			}
		}

		public void Disconnect()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
			}
		}

		public void RaiseArenaEvent(byte code, object payload)
		{
			PhotonNetwork.RaiseEvent(code, payload, RaiseEventOptions.Default, SendOptions.SendReliable);
		}

		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			OnJoinedRoom?.Invoke();
		}

		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> _)
		{
		}

		void IMatchmakingCallbacks.OnCreatedRoom()
		{
			OnCreatedRoom?.Invoke();
		}

		void IMatchmakingCallbacks.OnCreateRoomFailed(short c, string m)
		{
			OnEnterRoomFailed?.Invoke(c, m);
		}

		void IMatchmakingCallbacks.OnJoinRoomFailed(short c, string m)
		{
			OnEnterRoomFailed?.Invoke(c, m);
		}

		void IMatchmakingCallbacks.OnJoinRandomFailed(short c, string m)
		{
		}

		void IMatchmakingCallbacks.OnLeftRoom()
		{
			_masterState = null;
		}

		void IInRoomCallbacks.OnPlayerEnteredRoom(Player p)
		{
			OnRoomStateChanged?.Invoke();
		}

		void IInRoomCallbacks.OnPlayerLeftRoom(Player p)
		{
			OnRoomStateChanged?.Invoke();
		}

		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changed)
		{
			OnRoomStateChanged?.Invoke();
		}

		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player t, Hashtable c)
		{
			OnRoomStateChanged?.Invoke();
		}

		void IInRoomCallbacks.OnMasterClientSwitched(Player newMaster)
		{
			_masterState = null;
			OnRoomStateChanged?.Invoke();
		}

		public void OnEvent(EventData e)
		{
			if (e.Code >= 60 && e.Code <= 69)
			{
				OnArenaEvent?.Invoke(e.Code, e.CustomData);
			}
		}
	}
}
