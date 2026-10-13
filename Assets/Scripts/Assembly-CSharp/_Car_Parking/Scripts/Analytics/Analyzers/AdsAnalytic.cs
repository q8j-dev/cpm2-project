using System;
using CPM.Ads;
using CPM.Ads.Policy;
using CPM.Balance;
using Photon.Pun;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public sealed class AdsAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		private readonly IAdsService _ads;

		private readonly IBalanceDocumentStore _balance;

		private AdOfferTracker _showing;

		public AdsAnalytic(AnalyticEventFactory analyticEventFactory, IAdsService ads, IBalanceDocumentStore balance)
			: base(analyticEventFactory)
		{
			_ads = ads;
			_balance = balance;
		}

		public void Initialize()
		{
			_ads.RewardedImpression += OnRewardedImpression;
		}

		public void Dispose()
		{
			_ads.RewardedImpression -= OnRewardedImpression;
		}

		public AdOfferTracker Track(AdPlacement placement, string windowId)
		{
			return new AdOfferTracker(this, placement, windowId);
		}

		public void ReportRecoveredReward(AdPlacement placement, string adId, string serverAdId, bool granted, string errorCode = null, string errorMessage = null, int? rewardSoft = null, int? rewardHard = null)
		{
			if (!string.IsNullOrEmpty(adId))
			{
				Send(new AdActionContext
				{
					Status = (granted ? "REWARD_GRANTED" : "REWARD_FAILED"),
					AdId = adId,
					AdPlacement = placement.ToString(),
					AdFormat = "REWARDED",
					AdServerId = serverAdId,
					RewardId = placement.ToString(),
					RewardReceived = (granted ? new bool?(true) : ((bool?)null)),
					RewardSoft = rewardSoft,
					RewardHard = rewardHard,
					ErrorCode = (granted ? null : errorCode),
					ErrorMessage = (granted ? null : errorMessage)
				});
			}
		}

		internal static string CurrentGameMode()
		{
			if (!PhotonNetwork.InRoom)
			{
				return "single_player";
			}
			return "multi_player";
		}

		internal void SetShowing(AdOfferTracker tracker)
		{
			_showing = tracker;
		}

		internal void ClearShowing(AdOfferTracker tracker)
		{
			if (_showing == tracker)
			{
				_showing = null;
			}
		}

		internal void Send(AdActionContext context)
		{
		}

		private void OnRewardedImpression(AdImpressionInfo info)
		{
			_showing?.Impression(info);
		}
	}
}
