using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class DailyRewardContext : IEventContext
	{
		public DailyRewardStatus Status;

		public int? DayNumber;

		public int? CarId;

		public int? RewardSoft;

		public int? RewardHard;

		public DailyRewardType? RewardType;

		public int? HadFreeSlot;

		public int? CurrentStreak;
	}
}
