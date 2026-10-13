using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceRunSnapshot
	{
		public ObstacleRaceRunStatus Status { get; set; }

		public string RaceId { get; set; }

		public int? PlayersAtStart { get; set; }

		public bool? IsMaster { get; set; }

		public int? TrackSeed { get; set; }

		public string[] TrackPlan { get; set; }

		public int? Place { get; set; }

		public int? DurationMs { get; set; }

		public int? CheckpointCount { get; set; }

		public int? CheckpointEarned { get; set; }

		public int? CheckpointMissed { get; set; }

		public int? RestartCount { get; set; }

		public IReadOnlyList<ObstacleRaceCheckpointRecord> Checkpoints { get; set; }

		public int? ProgressCurrent { get; set; }
	}
}
