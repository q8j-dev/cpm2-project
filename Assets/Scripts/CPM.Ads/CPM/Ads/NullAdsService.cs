using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Ads
{
	public class NullAdsService : IAdsService, IAdsUserIdentity
	{
		public bool IsInitialized => false;

		public bool IsRewardedReady => false;

		public AdFailureCause LastRewardedLoadFailure => AdFailureCause.ManagerDisabled;

		public event Action<AdImpressionInfo> RewardedImpression
		{
			add
			{
			}
			remove
			{
			}
		}

		public UniTask<RewardedAdOutcome> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UniTask.FromResult(new RewardedAdOutcome(RewardedAdResult.NotSupported, AdFailureCause.ManagerDisabled));
		}

		public void SetUserId(string userId)
		{
		}

		public void SetTaggedAudience(int audienceTarget)
		{
		}
	}
}
