using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class RaceFinishEvent : DefaultEvent<RaceFinishContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"race_position",
				base.Context.RacePosition
			},
			{
				"race_time",
				base.Context.RaceFinishTime
			},
			{
				"best_lap",
				base.Context.BestLap
			},
			{
				"mode_duration",
				base.Context.ModeDuration
			},
			{
				"penalty",
				base.Context.Penalty
			},
			{
				"count_awards",
				base.Context.CountAwards
			},
			{
				"reward_soft",
				base.Context.RewardSoft
			},
			{
				"reward_hard",
				base.Context.RewardHard
			},
			{
				"exit_reason",
				base.Context.ExitReason
			}
		};

		protected override string Name => "race_finish";

		public RaceFinishEvent(CPMAnalytics analytics, RaceFinishContext context)
			: base(analytics, context)
		{
		}
	}
}
