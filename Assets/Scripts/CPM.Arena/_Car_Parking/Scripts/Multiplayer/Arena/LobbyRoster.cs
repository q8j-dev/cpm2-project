using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class LobbyRoster : IDisposable
	{
		private readonly IArenaNetwork _net;

		private readonly Dictionary<int, LobbyPlayer> _players = new Dictionary<int, LobbyPlayer>();

		public IReadOnlyCollection<LobbyPlayer> Players => _players.Values;

		public LobbyRoster(IArenaNetwork net)
		{
			_net = net;
			_net.OnJoinedRoom += OnJoinedRoom;
			_net.OnRoomStateChanged += OnRoomStateChanged;
		}

		public void Dispose()
		{
			_net.OnJoinedRoom -= OnJoinedRoom;
			_net.OnRoomStateChanged -= OnRoomStateChanged;
		}

		public bool TryGet(int actor, out LobbyPlayer player)
		{
			return _players.TryGetValue(actor, out player);
		}

		public string GetName(int actor)
		{
			if (!_players.TryGetValue(actor, out var value))
			{
				return null;
			}
			return value.Name;
		}

		private void OnJoinedRoom()
		{
			Rebuild();
		}

		private void OnRoomStateChanged()
		{
			if (_net.ReadLobbyState().Phase < ArenaPhase.Countdown)
			{
				Rebuild();
			}
		}

		private void Rebuild()
		{
			_players.Clear();
			foreach (ArenaPlayerSnapshot item in _net.ReadPlayers())
			{
				_players[item.Actor] = new LobbyPlayer(item.Actor, item.Name);
			}
		}
	}
}
