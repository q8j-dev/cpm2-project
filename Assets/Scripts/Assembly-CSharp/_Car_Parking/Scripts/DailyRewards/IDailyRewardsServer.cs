using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.DailyRewards
{
	public interface IDailyRewardsServer : IDeleteUserData
	{
		IReadOnlyList<DailyRewardData> Rewards { get; }

		int Count { get; }

		event Action OnDailyRewardsChanged;

		UniTask<FunctionStatus> AwardRewardAsync(DailyRewardData dailyRewardData, ISlotService slotService, string accountID);

		UniTask<CombinedPrice> GetCarPriceAsync(int day);

		UniTask<bool> SyncRewardsAsync();

		UniTask<FunctionStatus> ExchangeRewardCarAsync(int day);

		void Clear();

		void SetRewards(List<DailyRewardData> rewards);
	}
}
