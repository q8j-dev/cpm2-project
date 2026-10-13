using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class DailyTaskEvent : DefaultEvent<DailyTaskContext>
	{
		protected override string Name => "daily_task";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object> { 
				{
					"status",
					base.Context.Status.ToString()
				} };
				if (base.Context.TaskId != null)
				{
					dictionary.Add("task_id", base.Context.TaskId);
				}
				if (base.Context.PoolId != null)
				{
					dictionary.Add("pool_id", base.Context.PoolId);
				}
				if (base.Context.Status == DailyTaskStatus.COMPLETED)
				{
					dictionary.Add("progress_current", base.Context.ProgressCurrent);
				}
				if (base.Context.ProgressTarget.HasValue)
				{
					dictionary.Add("progress_target", base.Context.ProgressTarget.Value);
				}
				if (base.Context.RewardSoft.HasValue)
				{
					dictionary.Add("reward_soft", base.Context.RewardSoft.Value);
				}
				if (base.Context.RewardHard.HasValue)
				{
					dictionary.Add("reward_hard", base.Context.RewardHard.Value);
				}
				if (base.Context.TasksCompletedToday.HasValue)
				{
					dictionary.Add("tasks_completed_today", base.Context.TasksCompletedToday.Value);
				}
				return dictionary;
			}
		}

		public DailyTaskEvent(CPMAnalytics analytics, DailyTaskContext context)
			: base(analytics, context)
		{
		}
	}
}
