using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class RacingAdRewardReclaimService : IRacingAdRewardReclaimService
	{
		private readonly IAdsRewardsBackend _backend;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly AdsAnalytic _adsAnalytic;

		public RacingAdRewardReclaimService(IAdsRewardsBackend backend, CurrencyManagerController currencyManagerController, AdsAnalytic adsAnalytic)
		{
			_backend = backend;
			_currencyManagerController = currencyManagerController;
			_adsAnalytic = adsAnalytic;
		}

		public async UniTask ReclaimAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			_ = 1;
			try
			{
				if (PendingAdRewardPrefs.GetRaceMode() == RacingRewardMode.None)
				{
					return;
				}
				Result<List<UnclaimedAdReward>> result = await _backend.GetUnclaimed(cancellationToken);
				if (!result.IsFail)
				{
					UnclaimedAdReward unclaimedAdReward = FindOldestCompetitionReward(result.Value);
					if (unclaimedAdReward != null)
					{
						await ReclaimByAdIdAsync(unclaimedAdReward.AdId, cancellationToken);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
		}

		public async UniTask ReclaimAsync(string adId, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (!string.IsNullOrEmpty(adId))
				{
					await ReclaimByAdIdAsync(adId, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
		}

		private async UniTask ReclaimByAdIdAsync(string adId, CancellationToken cancellationToken)
		{
			RacingRewardMode raceMode = PendingAdRewardPrefs.GetRaceMode();
			if (await ClaimAsync(adId, raceMode, cancellationToken))
			{
				await _currencyManagerController.UpdateCoinsFromDatabase();
			}
		}

		private static UnclaimedAdReward FindOldestCompetitionReward(List<UnclaimedAdReward> rewards)
		{
			foreach (UnclaimedAdReward reward in rewards)
			{
				if (reward != null && !string.IsNullOrEmpty(reward.AdId) && reward.Placement == "Competition")
				{
					return reward;
				}
			}
			return null;
		}

		private async UniTask<bool> ClaimAsync(string adId, RacingRewardMode mode, CancellationToken cancellationToken)
		{
			List<AdRewardClaimEntry> claims = new List<AdRewardClaimEntry>
			{
				new AdRewardClaimEntry
				{
					AdId = adId,
					Type = mode.ToString(),
					Id = "Competition"
				}
			};
			Result<List<AdRewardClaimResult>> result = await _backend.Claim(claims, cancellationToken);
			if (result.IsFail)
			{
				return false;
			}
			AdRewardClaimResult adRewardClaimResult = ((result.Value.Count > 0) ? result.Value[0] : null);
			if (adRewardClaimResult == null || adRewardClaimResult.Status == "FAILED")
			{
				return false;
			}
			string adId2 = PendingAdRewardPrefs.GetAdId(AdPlacement.Competition);
			PendingAdRewardPrefs.Clear(AdPlacement.Competition);
			if (adRewardClaimResult.Status == "GRANTED")
			{
				int num = ((adRewardClaimResult.Money != 0) ? adRewardClaimResult.Money : (adRewardClaimResult.Claimed?.Money ?? 0));
				int num2 = ((adRewardClaimResult.Coins != 0) ? adRewardClaimResult.Coins : (adRewardClaimResult.Claimed?.Coins ?? 0));
				_adsAnalytic.ReportRecoveredReward(AdPlacement.Competition, adId2, adId, granted: true, null, null, (num > 0) ? new int?(num) : ((int?)null), (num2 > 0) ? new int?(num2) : ((int?)null));
				return true;
			}
			_adsAnalytic.ReportRecoveredReward(AdPlacement.Competition, adId2, adId, granted: false, AdRewardErrorCode.FromClaimStatus(adRewardClaimResult.Status), adRewardClaimResult.Status);
			return false;
		}
	}
}
