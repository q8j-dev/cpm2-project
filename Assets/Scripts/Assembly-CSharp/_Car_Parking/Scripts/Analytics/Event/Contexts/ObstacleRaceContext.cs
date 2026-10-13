using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class ObstacleRaceContext : IEventContext
	{
		public ObstacleRaceStatus Status { get; set; }

		public string RaceId { get; set; }

		public string LobbyId { get; set; }

		public string ArenaId { get; set; }

		public int? RegionPing { get; set; }

		public int? PlayersCurrent { get; set; }

		public bool? IsMaster { get; set; }

		public int? TrackSeed { get; set; }

		public string TrackPlan { get; set; }

		public int? RacePosition { get; set; }

		public int? DurationMs { get; set; }

		public int? CheckpointCount { get; set; }

		public int? CheckpointEarned { get; set; }

		public int? CheckpointMissed { get; set; }

		public int? RestartCount { get; set; }

		public string Checkpoints { get; set; }

		public int? ProgressCurrent { get; set; }

		public LobbyFailReason? FailReason { get; set; }

		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public ObstacleRaceResultStatus? Result { get; set; }

		public int? RewardHard { get; set; }

		public int? RewardSoft { get; set; }
	}
}
