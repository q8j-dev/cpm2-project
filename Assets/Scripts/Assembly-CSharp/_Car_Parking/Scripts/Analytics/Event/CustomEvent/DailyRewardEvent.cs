using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class DailyRewardEvent : DefaultEvent<DailyRewardContext>
	{
		protected override string Name => "daily_reward";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object> { 
				{
					"status",
					base.Context.Status
				} };
				if (base.Context.DayNumber.HasValue)
				{
					dictionary.Add("day_number", base.Context.DayNumber.Value);
				}
				if (base.Context.CarId.HasValue)
				{
					dictionary.Add("car_id", base.Context.CarId.Value);
				}
				if (base.Context.RewardSoft.HasValue)
				{
					dictionary.Add("reward_soft", base.Context.RewardSoft.Value);
				}
				if (base.Context.RewardHard.HasValue)
				{
					dictionary.Add("reward_hard", base.Context.RewardHard.Value);
				}
				if (base.Context.RewardType.HasValue)
				{
					dictionary.Add("reward_type", base.Context.RewardType.Value);
				}
				if (base.Context.HadFreeSlot.HasValue)
				{
					dictionary.Add("had_free_slot", base.Context.HadFreeSlot.Value);
				}
				if (base.Context.CurrentStreak.HasValue)
				{
					dictionary.Add("current_streak", base.Context.CurrentStreak.Value);
				}
				return dictionary;
			}
		}

		public DailyRewardEvent(CPMAnalytics analytics, DailyRewardContext context)
			: base(analytics, context)
		{
		}
	}
}
