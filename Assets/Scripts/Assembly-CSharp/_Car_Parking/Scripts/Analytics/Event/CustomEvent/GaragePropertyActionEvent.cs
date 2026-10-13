using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class GaragePropertyActionEvent : DefaultEvent<GaragePropertyActionContext>
	{
		protected override string Name => "garage_property_action";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"action",
				base.Context.Action.ToString()
			},
			{
				"result",
				base.Context.Result.ToString()
			},
			{
				"garage_id",
				base.Context.GarageId
			},
			{
				"garage_source_type",
				base.Context.GarageSourceType.ToString()
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
				"fail_reason",
				base.Context.FailReason.ToString()
			},
			{
				"legacy_sell_block",
				base.Context.LegacySellBlock
			}
		};

		public GaragePropertyActionEvent(CPMAnalytics analytics, GaragePropertyActionContext context)
			: base(analytics, context)
		{
		}
	}
}
