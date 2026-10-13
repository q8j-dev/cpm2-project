using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IArenaContext
	{
		bool IsMasterClient { get; }

		IReadOnlyList<int> ParticipantActorNumbers { get; }

		ArenaConfig Config { get; }

		bool IsFriendsRoom { get; }

		string RaceId { get; }

		double ServerTime { get; }

		double MatchStartTime { get; }

		event Action OnModeStateChanged;

		event Action<byte, object> OnModeEvent;

		void WriteModeState(string key, object value);

		void ClearModeState(string key);

		object ReadModeState(string key);

		void WriteOwnModeState(string key, object value);

		void ClearOwnModeState(string key);

		object ReadPlayerModeState(int actor, string key);

		void RaiseModeEvent(byte code, object payload);

		void Complete();

		void RequestRestart();
	}
}
