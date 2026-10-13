using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaContext : IArenaContext, IDisposable
	{
		private readonly IArenaNetwork _net;

		private readonly IReadOnlyList<int> _participants;

		public ArenaConfig Config { get; }

		public bool IsMasterClient => _net.IsMasterClient;

		public bool IsFriendsRoom => _net.IsFriendsRoom;

		public IReadOnlyList<int> ParticipantActorNumbers => _participants;

		public double ServerTime => _net.ServerTime;

		public double MatchStartTime => _net.ReadLobbyState().MatchStartTime;

		public bool RestartRequested { get; private set; }

		public string RaceId { get; private set; }

		public event Action OnModeStateChanged;

		public event Action<byte, object> OnModeEvent;

		public ArenaContext(IArenaNetwork net, ArenaConfig config)
		{
			_net = net;
			Config = config;
			_net.OnArenaEvent += HandleArenaEvent;
			_net.OnRoomStateChanged += HandleRoomStateChanged;
			List<int> list = new List<int>();
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player player in playerList)
			{
				list.Add(player.ActorNumber);
			}
			_participants = list;
		}

		public void Dispose()
		{
			_net.OnArenaEvent -= HandleArenaEvent;
			_net.OnRoomStateChanged -= HandleRoomStateChanged;
		}

		public void WriteModeState(string key, object value)
		{
			if (_net.IsMasterClient)
			{
				_net.WriteRoomProperty(key, value);
			}
		}

		public void ClearModeState(string key)
		{
			if (_net.IsMasterClient)
			{
				_net.WriteRoomProperty(key, null);
			}
		}

		public object ReadModeState(string key)
		{
			return _net.ReadRoomProperty(key);
		}

		public void WriteOwnModeState(string key, object value)
		{
			_net.WriteLocalPlayerProperty(key, value);
		}

		public void ClearOwnModeState(string key)
		{
			_net.WriteLocalPlayerProperty(key, null);
		}

		public object ReadPlayerModeState(int actor, string key)
		{
			return _net.ReadPlayerProperty(actor, key);
		}

		public void RaiseModeEvent(byte code, object payload)
		{
			_net.RaiseArenaEvent(code, payload);
		}

		public void Complete()
		{
		}

		public void RequestRestart()
		{
			RestartRequested = true;
		}

		public void SetRaceId(string raceId)
		{
			RaceId = raceId;
		}

		private void HandleArenaEvent(byte code, object data)
		{
			OnModeEvent?.Invoke(code, data);
		}

		private void HandleRoomStateChanged()
		{
			OnModeStateChanged?.Invoke();
		}
	}
}
