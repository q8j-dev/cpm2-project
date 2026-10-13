using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class SnowmanEvent : DefaultEvent<SnowmanContext>
	{
		protected override string Name => "snowman_event";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status
			},
			{
				"game_event_id",
				base.Context.GameEventId
			},
			{
				"count_total",
				base.Context.CountTotal
			},
			{
				"count_done",
				base.Context.CountDone
			},
			{
				"time_left",
				base.Context.TimeLeft
			},
			{
				"time_limit",
				base.Context.TimeLimit
			},
			{
				"reward_id",
				base.Context.RewardId
			},
			{
				"reward_received",
				base.Context.RewardReceived
			},
			{
				"map_id",
				base.Context.MapId
			},
			{
				"zone_id",
				base.Context.ZoneId
			},
			{
				"attempt_number",
				base.Context.AttemptNumber
			},
			{
				"car_id",
				AnalyticsHelper.GetCurrentCarId()
			},
			{
				"car_class",
				base.Context.CarClass
			},
			{
				"mode_type",
				base.Context.ModeType
			}
		};

		public SnowmanEvent(CPMAnalytics analytics, SnowmanContext context)
			: base(analytics, context)
		{
		}
	}
}
