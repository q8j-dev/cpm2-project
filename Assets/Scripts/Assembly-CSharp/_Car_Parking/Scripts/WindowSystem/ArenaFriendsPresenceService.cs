using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ArenaFriendsPresenceService : IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks
	{
		private const float PollIntervalSeconds = 10f;

		private readonly PlayerDatas.FriendsUtility _friendsUtility;

		private readonly PhotonReconnectionHandler _reconnect;

		private readonly Dictionary<string, ArenaRoomStatus> _rooms = new Dictionary<string, ArenaRoomStatus>();

		private List<FriendInfo> _friends;

		private ArenaId _targetArena;

		private CancellationTokenSource _cts;

		private bool _running;

		public IReadOnlyList<ArenaFriendPresence> Current { get; private set; } = new List<ArenaFriendPresence>();

		public event Action OnPresenceUpdated;

		public ArenaFriendsPresenceService(PlayerDatas.FriendsUtility friendsUtility, PhotonReconnectionHandler reconnect)
		{
			_friendsUtility = friendsUtility;
			_reconnect = reconnect;
		}

		public void Start(ArenaId targetArena)
		{
			_targetArena = targetArena;
			if (FriendIds().Length == 0)
			{
				Current = new List<ArenaFriendPresence>();
				OnPresenceUpdated?.Invoke();
			}
			else if (!_running)
			{
				_running = true;
				PhotonNetwork.AddCallbackTarget(this);
				if (!PhotonNetwork.IsConnected)
				{
					_reconnect.ReconnectHard(PhotonConnectionProfile.Arena);
				}
				else
				{
					TryJoinLobby();
				}
				_cts = new CancellationTokenSource();
				PollLoop(_cts.Token).Forget();
			}
		}

		public void Stop(bool disconnect = true)
		{
			if (_running)
			{
				_running = false;
				BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
				PhotonNetwork.RemoveCallbackTarget(this);
				_rooms.Clear();
				_friends = null;
				if (disconnect && PhotonNetwork.IsConnected)
				{
					PhotonNetwork.Disconnect();
				}
			}
		}

		public void OnServerChanged()
		{
			_rooms.Clear();
			_friends = null;
			Rebuild();
			if (_running)
			{
				TryJoinLobby();
			}
		}

		private string[] FriendIds()
		{
			List<PrefFriendData> list = _friendsUtility?.Friends;
			if (list == null)
			{
				return Array.Empty<string>();
			}
			return (from f in list
				where !string.IsNullOrEmpty(f.id)
				select f.id).ToArray();
		}

		private void TryJoinLobby()
		{
			if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom && !PhotonNetwork.InLobby)
			{
				PhotonNetwork.JoinLobby();
			}
		}

		private async UniTaskVoid PollLoop(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
					{
						string[] array = FriendIds();
						if (array.Length != 0)
						{
							PhotonNetwork.FindFriends(array);
						}
					}
					await UniTask.Delay(TimeSpan.FromSeconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void Rebuild()
		{
			List<ArenaFriendRoomInput> list = new List<ArenaFriendRoomInput>();
			if (_friends != null)
			{
				foreach (FriendInfo friend in _friends)
				{
					if (friend.IsInRoom && !string.IsNullOrEmpty(friend.Room))
					{
						list.Add(new ArenaFriendRoomInput(friend.UserId, friend.Room));
					}
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<PrefFriendData> list2 = _friendsUtility?.Friends;
			if (list2 != null)
			{
				foreach (PrefFriendData item in list2)
				{
					if (!string.IsNullOrEmpty(item.id))
					{
						dictionary[item.id] = item.Name;
					}
				}
			}
			Current = ArenaFriendPresenceBuilder.Build(list, _rooms, dictionary, _targetArena);
			OnPresenceUpdated?.Invoke();
		}

		public void OnJoinedLobby()
		{
			string[] array = FriendIds();
			if (array.Length != 0)
			{
				PhotonNetwork.FindFriends(array);
			}
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			foreach (RoomInfo room in roomList)
			{
				if (room.RemovedFromList)
				{
					_rooms.Remove(room.Name);
				}
				else
				{
					_rooms[room.Name] = new ArenaRoomStatus(room.PlayerCount, room.MaxPlayers, room.IsOpen);
				}
			}
			Rebuild();
		}

		public void OnLeftLobby()
		{
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
			_friends = friendList;
			Rebuild();
		}

		public void OnCreatedRoom()
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinedRoom()
		{
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		public void OnLeftRoom()
		{
		}

		public void OnConnectedToMaster()
		{
			if (_running)
			{
				TryJoinLobby();
			}
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			_rooms.Clear();
			_friends = null;
			Rebuild();
		}

		public void OnConnected()
		{
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}
	}
}
