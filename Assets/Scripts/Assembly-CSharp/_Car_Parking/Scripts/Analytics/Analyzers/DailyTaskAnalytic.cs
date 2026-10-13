using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class DailyTaskAnalytic : AnalyticProviderBase
	{
		public DailyTaskAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogCompleted(string taskId, string poolId, int? progressCurrent, int progressTarget, int rewardSoft, int rewardHard)
		{
			base.AnalyticEventFactory.CreateEvent<DailyTaskEvent, DailyTaskContext>(new DailyTaskContext
			{
				Status = DailyTaskStatus.COMPLETED,
				TaskId = taskId,
				PoolId = poolId,
				ProgressCurrent = progressCurrent,
				ProgressTarget = progressTarget,
				RewardSoft = ClampReward(rewardSoft),
				RewardHard = ClampReward(rewardHard)
			}).Send();
		}

		public void LogClaimed(string taskId, string poolId, int progressTarget, int rewardSoft, int rewardHard, int tasksCompletedToday)
		{
			base.AnalyticEventFactory.CreateEvent<DailyTaskEvent, DailyTaskContext>(new DailyTaskContext
			{
				Status = DailyTaskStatus.CLAIMED,
				TaskId = taskId,
				PoolId = poolId,
				ProgressTarget = progressTarget,
				RewardSoft = ClampReward(rewardSoft),
				RewardHard = ClampReward(rewardHard),
				TasksCompletedToday = tasksCompletedToday
			}).Send();
		}

		private int ClampReward(int reward)
		{
			return Mathf.Max(0, reward);
		}
	}
}
