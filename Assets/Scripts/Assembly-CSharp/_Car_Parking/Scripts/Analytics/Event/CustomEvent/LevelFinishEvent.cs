using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class LevelFinishEvent : DefaultEvent<LevelFinishContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"level_id",
				base.Context.LevelId
			},
			{
				"play_duration",
				base.Context.PlayDuration
			},
			{
				"mode_duration",
				base.Context.ModeDuration
			},
			{
				"reward_soft",
				base.Context.RewardSoft
			},
			{
				"best_completion_time",
				base.Context.BestCompletionTime
			},
			{
				"attempt_number",
				base.Context.AttemptNumber
			},
			{
				"finish_reason",
				base.Context.FinishReason
			}
		};

		protected override string Name => "level_finish";

		public LevelFinishEvent(CPMAnalytics analytics, LevelFinishContext context)
			: base(analytics, context)
		{
		}
	}
}
