using System;
using CPM.Connection.Entities;
using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using Rating.DailyTasks.Scripts;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CPMWorldSale;

namespace CPM.Connection
{
	[Preserve]
	public interface IConnectionDataHandler
	{
		double ServerTimestamp { get; }

		event Action OnCanPlayFalse;

		void SubscribeForRankChange(IRankReceiver rankReceiver);

		void UnsubscribeFromRankChange(IRankReceiver rankReceiver);

		void SubscribeForWWActivity(IWorldSaleActivityDependant dependant);

		void SubscribeForDailyServicesInitialization(IDailyServicesDependant dailyServicesDependant);

		void ProcessUserConnectionData(UserConnectionData connectionData);

		UniTask ReProcessCachedData();

		void ClearCacheData();

		void SubscribeForPendingRewards(IPendingRewardsProcessor pendingRewardsProcessor);
	}
}
