using System;
using CPM.Connection.Entities;
using CodeStage.AntiCheat.Storage;
using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using Rating.DailyTasks.Scripts;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CPMWorldSale;

namespace CPM.Connection
{
	public class ConnectionDataHandler : IConnectionDataHandler
	{
		private IRankReceiver rankReceiver;

		private IWorldSaleActivityDependant worldSaleActivityDependant;

		private UserConnectionData cachedData;

		private IDailyServicesDependant dailyServicesDependant;

		private readonly IConnectionManager connectionManager;

		private IPendingRewardsProcessor pendingRewardsProcessor;

		public double ServerTimestamp { get; private set; }

		public event Action OnCanPlayFalse;

		[Inject]
		public ConnectionDataHandler(IConnectionManager connectionManager)
		{
			this.connectionManager = connectionManager;
			connectionManager.SubscribeForConnectionDataHandle(this);
			cachedData = null;
		}

		public void SubscribeForPendingRewards(IPendingRewardsProcessor pendingRewardsProcessor)
		{
			this.pendingRewardsProcessor = pendingRewardsProcessor;
		}

		~ConnectionDataHandler()
		{
			cachedData = null;
		}

		public void SubscribeForRankChange(IRankReceiver rankReceiver)
		{
			this.rankReceiver = rankReceiver;
			if (cachedData != null && IsAlive(rankReceiver))
			{
				rankReceiver.OnRankReceived(ref cachedData.level);
			}
		}

		public void UnsubscribeFromRankChange(IRankReceiver rankReceiver)
		{
			if (this.rankReceiver == rankReceiver)
			{
				this.rankReceiver = null;
			}
		}

		private static bool IsAlive(object subscriber)
		{
			if (subscriber != null)
			{
				if (subscriber is UnityEngine.Object obj)
				{
					return obj;
				}
				return true;
			}
			return false;
		}

		public void SubscribeForWWActivity(IWorldSaleActivityDependant dependant)
		{
			worldSaleActivityDependant = dependant;
		}

		public void SubscribeForDailyServicesInitialization(IDailyServicesDependant dailyServicesDependant)
		{
			this.dailyServicesDependant = dailyServicesDependant;
		}

		public void ProcessUserConnectionData(UserConnectionData connectionData)
		{
			if (connectionData == null)
			{
				return;
			}
			ServerTimestamp = connectionData.timestamp;
			cachedData = connectionData;
			if (IsAlive(rankReceiver))
			{
				rankReceiver.OnRankReceived(ref connectionData.level);
			}
			connectionManager.CorrectHash.Get(out var value);
			if (!value || connectionData.reloadRequired)
			{
				return;
			}
			if (connectionData.reports > 50)
			{
				ObscuredPrefs.SetInt("Banned", 1);
			}
			else
			{
				ObscuredPrefs.SetInt("Banned", 0);
			}
			if (connectionData.sellingCarWW == 1)
			{
				worldSaleActivityDependant.ProcessWorldSaleSubscription(connectionData.wwReward);
				if (connectionData.wwReward > 0)
				{
					connectionData.wwReward = 0;
					connectionData.sellingCarWW = 0;
				}
			}
			dailyServicesDependant?.Initialize(ServerTimestamp);
			if (connectionData.pendingRewards != null && connectionData.pendingRewards.Length != 0)
			{
				pendingRewardsProcessor.ProcessRewards(connectionData.pendingRewards);
				connectionData.pendingRewards = null;
			}
		}

		public async UniTask ReProcessCachedData()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.0));
			if (cachedData != null)
			{
				ProcessUserConnectionData(cachedData);
			}
		}

		public void ClearCacheData()
		{
			cachedData = null;
		}
	}
}
