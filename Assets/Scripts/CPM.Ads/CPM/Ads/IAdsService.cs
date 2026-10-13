using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Ads
{
	public interface IAdsService
	{
		bool IsInitialized { get; }

		bool IsRewardedReady { get; }

		AdFailureCause LastRewardedLoadFailure { get; }

		event Action<AdImpressionInfo> RewardedImpression;

		UniTask<RewardedAdOutcome> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default(CancellationToken));

		void SetTaggedAudience(int audienceTarget);
	}
}
