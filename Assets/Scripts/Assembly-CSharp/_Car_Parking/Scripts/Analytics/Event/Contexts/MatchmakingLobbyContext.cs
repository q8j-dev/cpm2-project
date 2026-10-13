using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class MatchmakingLobbyContext : IEventContext
	{
		public MatchmakingLobbyStatus Status { get; set; }

		public string LobbyId { get; set; }

		public LobbyEntryPath? EntryPath { get; set; }

		public LobbyVisibility? Visibility { get; set; }

		public int? PlayersCurrent { get; set; }

		public int? PlayersMin { get; set; }

		public int? PlayersMax { get; set; }

		public LobbyFailReason? FailReason { get; set; }

		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public float? TimeLeft { get; set; }

		public float? TimeLimit { get; set; }

		public string GameEventId { get; set; }

		public string EventVariant { get; set; }

		public string GameMode { get; set; }

		public string Region { get; set; }

		public int? RegionPing { get; set; }

		public string RaceId { get; set; }

		public string ArenaId { get; set; }
	}
}
