using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	public interface IAdsRewardsBackend
	{
		UniTask<Result<List<UnclaimedAdReward>>> GetUnclaimed(CancellationToken ct = default(CancellationToken), string debugPlacement = "");

		UniTask<Result<List<AdRewardClaimResult>>> Claim(List<AdRewardClaimEntry> claims, CancellationToken ct = default(CancellationToken));

		UniTask<Result<List<AdDailyLimit>>> GetLimits(CancellationToken ct = default(CancellationToken));
	}
}
