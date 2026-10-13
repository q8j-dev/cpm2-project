using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class ObstacleRaceEvent : DefaultEvent<ObstacleRaceContext>
	{
		protected override string Name => "obstacle_race";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object> { 
				{
					"status",
					base.Context.Status.GetEnumMemberValue()
				} };
				AddIfPresent(dictionary, "race_id", base.Context.RaceId);
				AddIfPresent(dictionary, "lobby_id", base.Context.LobbyId);
				AddIfPresent(dictionary, "arena_id", base.Context.ArenaId);
				AddIfPresent(dictionary, "track_plan", base.Context.TrackPlan);
				AddIfPresent(dictionary, "checkpoints", base.Context.Checkpoints);
				AddIfPresent(dictionary, "error_code", base.Context.ErrorCode);
				AddIfPresent(dictionary, "error_message", base.Context.ErrorMessage);
				if (base.Context.RegionPing.HasValue)
				{
					dictionary.Add("region_ping", base.Context.RegionPing.Value);
				}
				if (base.Context.PlayersCurrent.HasValue)
				{
					dictionary.Add("players_current", base.Context.PlayersCurrent.Value);
				}
				if (base.Context.IsMaster.HasValue)
				{
					dictionary.Add("is_master", base.Context.IsMaster.Value);
				}
				if (base.Context.TrackSeed.HasValue)
				{
					dictionary.Add("track_seed", base.Context.TrackSeed.Value);
				}
				if (base.Context.RacePosition.HasValue)
				{
					dictionary.Add("race_position", base.Context.RacePosition.Value);
				}
				if (base.Context.DurationMs.HasValue)
				{
					dictionary.Add("duration_ms", base.Context.DurationMs.Value);
				}
				if (base.Context.CheckpointCount.HasValue)
				{
					dictionary.Add("checkpoint_count", base.Context.CheckpointCount.Value);
				}
				if (base.Context.CheckpointEarned.HasValue)
				{
					dictionary.Add("checkpoint_earned", base.Context.CheckpointEarned.Value);
				}
				if (base.Context.CheckpointMissed.HasValue)
				{
					dictionary.Add("checkpoint_missed", base.Context.CheckpointMissed.Value);
				}
				if (base.Context.RestartCount.HasValue)
				{
					dictionary.Add("restart_count", base.Context.RestartCount.Value);
				}
				if (base.Context.ProgressCurrent.HasValue)
				{
					dictionary.Add("progress_current", base.Context.ProgressCurrent.Value);
				}
				if (base.Context.FailReason.HasValue)
				{
					dictionary.Add("fail_reason", base.Context.FailReason.Value.GetEnumMemberValue());
				}
				if (base.Context.Result.HasValue)
				{
					dictionary.Add("result", base.Context.Result.Value.GetEnumMemberValue());
				}
				if (base.Context.RewardHard.HasValue)
				{
					dictionary.Add("reward_hard", base.Context.RewardHard.Value);
				}
				if (base.Context.RewardSoft.HasValue)
				{
					dictionary.Add("reward_soft", base.Context.RewardSoft.Value);
				}
				return dictionary;
			}
		}

		private static void AddIfPresent(Dictionary<string, object> parameters, string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				parameters.Add(key, value);
			}
		}

		public ObstacleRaceEvent(CPMAnalytics analytics, ObstacleRaceContext context)
			: base(analytics, context)
		{
		}
	}
}
