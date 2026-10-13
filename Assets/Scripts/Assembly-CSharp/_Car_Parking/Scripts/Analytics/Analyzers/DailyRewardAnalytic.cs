using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DailyRewards;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class DailyRewardAnalytic : AnalyticProviderBase
	{
		public DailyRewardAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogShown(int currentStreak)
		{
			base.AnalyticEventFactory.CreateEvent<DailyRewardEvent, DailyRewardContext>(new DailyRewardContext
			{
				Status = DailyRewardStatus.SHOWN,
				CurrentStreak = currentStreak
			}).Send();
		}

		public void LogClaimed(int dayNumber, DailyRewardData.TypeReward type, int rewardSoft, int rewardHard)
		{
			base.AnalyticEventFactory.CreateEvent<DailyRewardEvent, DailyRewardContext>(new DailyRewardContext
			{
				Status = DailyRewardStatus.CLAIMED,
				DayNumber = dayNumber,
				RewardType = MapType(type),
				RewardSoft = rewardSoft,
				RewardHard = rewardHard
			}).Send();
		}

		public void LogPlaced(int dayNumber, DailyRewardData.TypeReward type, int carId, bool hadFreeSlot)
		{
			base.AnalyticEventFactory.CreateEvent<DailyRewardEvent, DailyRewardContext>(new DailyRewardContext
			{
				Status = DailyRewardStatus.PLACED,
				DayNumber = dayNumber,
				RewardType = MapType(type),
				CarId = carId,
				HadFreeSlot = (hadFreeSlot ? 1 : 0)
			}).Send();
		}

		public void LogExchanged(int dayNumber, int carId, bool hadFreeSlot, int rewardSoft)
		{
			base.AnalyticEventFactory.CreateEvent<DailyRewardEvent, DailyRewardContext>(new DailyRewardContext
			{
				Status = DailyRewardStatus.EXCHANGED,
				DayNumber = dayNumber,
				CarId = carId,
				HadFreeSlot = (hadFreeSlot ? 1 : 0),
				RewardSoft = rewardSoft
			}).Send();
		}

		private static DailyRewardType MapType(DailyRewardData.TypeReward type)
		{
			return type switch
			{
				DailyRewardData.TypeReward.Money => DailyRewardType.MONEY, 
				DailyRewardData.TypeReward.Coin => DailyRewardType.COIN, 
				DailyRewardData.TypeReward.Car => DailyRewardType.CAR, 
				_ => DailyRewardType.NONE, 
			};
		}
	}
}
