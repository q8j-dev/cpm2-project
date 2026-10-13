using System;
using System.Collections.Generic;
using Photon.Realtime;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatExtraDataController : IDisposable, IInitializable
	{
		private readonly Dictionary<int, bool> _mutedPlayers = new Dictionary<int, bool>();

		private readonly Dictionary<int, bool> _blockedPlayers = new Dictionary<int, bool>();

		public void Initialize()
		{
			_mutedPlayers.Clear();
			_blockedPlayers.Clear();
			V.OnPlayerDisconnected += OnPlayerDisconnected;
		}

		public void Dispose()
		{
			_mutedPlayers.Clear();
			_blockedPlayers.Clear();
			V.OnPlayerDisconnected -= OnPlayerDisconnected;
		}

		public bool IsMuted(int actorNumber)
		{
			bool value;
			return _mutedPlayers.TryGetValue(actorNumber, out value) & value;
		}

		public bool IsBlocked(int actorNumber)
		{
			bool value;
			return _blockedPlayers.TryGetValue(actorNumber, out value) & value;
		}

		public void SetMuted(int actorNumber, bool value)
		{
			if (value)
			{
				_mutedPlayers[actorNumber] = true;
			}
			else
			{
				_mutedPlayers.Remove(actorNumber);
			}
		}

		public void SetBlocked(int actorNumber, bool value)
		{
			if (value)
			{
				_blockedPlayers[actorNumber] = true;
			}
			else
			{
				_blockedPlayers.Remove(actorNumber);
			}
		}

		private void OnPlayerDisconnected(Player player)
		{
			_mutedPlayers.Remove(player.ActorNumber);
			_blockedPlayers.Remove(player.ActorNumber);
		}
	}
}
