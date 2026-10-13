using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class DailyTaskContext : IEventContext
	{
		public DailyTaskStatus Status;

		public string TaskId;

		public string PoolId;

		public int? ProgressCurrent;

		public int? ProgressTarget;

		public int? RewardSoft;

		public int? RewardHard;

		public int? TasksCompletedToday;
	}
}
