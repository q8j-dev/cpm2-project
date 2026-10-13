using System;
using System.Collections.Generic;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class RacingRewardAdService : IRacingRewardAdService
	{
		private const string CompetitionPlacement = "Competition";

		private const int UnclaimedAttempts = 5;

		private static readonly TimeSpan UnclaimedRetryDelay = TimeSpan.FromSeconds(2.0);

		private readonly IAdOfferService _adOffers;

		private readonly IAdsRewardsBackend _backend;

		private readonly AdsAnalytic _adsAnalytic;

		private readonly IAdsDailyLimits _dailyLimits;

		public RacingRewardAdService(IAdOfferService adOffers, IAdsRewardsBackend backend, AdsAnalytic adsAnalytic, IAdsDailyLimits dailyLimits)
		{
			_adOffers = adOffers;
			_backend = backend;
			_adsAnalytic = adsAnalytic;
			_dailyLimits = dailyLimits;
		}

		public OfferState CanOffer(string raceId)
		{
			return _adOffers.CanOffer(AdPlacement.Competition, raceId);
		}

		public RacingRewardAmounts SplitWonRewards(int[] rewards, int wonPlaces)
		{
			if (rewards == null || rewards.Length == 0 || wonPlaces <= 0)
			{
				return new RacingRewardAmounts(0, 0);
			}
			int num = Math.Min(wonPlaces, rewards.Length);
			int num2 = rewards.Length - 1;
			bool flag = num == rewards.Length;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!flag || i != num2)
				{
					num3 += rewards[i];
				}
			}
			return new RacingRewardAmounts(flag ? rewards[num2] : 0, num3);
		}

		public bool CanOfferForNewRace()
		{
			OfferState state = _adOffers.CanOffer(AdPlacement.Competition, Guid.NewGuid().ToString("N"));
			int num;
			if (state.IsOfferable && state.Availability != OfferAvailability.ExhaustedToday)
			{
				num = ((state.Availability != OfferAvailability.ExhaustedSession) ? 1 : 0);
				if (num != 0)
				{
					goto IL_005b;
				}
			}
			else
			{
				num = 0;
			}
			_adsAnalytic.Track(AdPlacement.Competition, "RacingRewardAdWindow").Present(state);
			goto IL_005b;
			IL_005b:
			return (byte)num != 0;
		}

		public async UniTask<RacingRewardDoubleResult> TryDoubleRewardAsync(RacingRewardMode mode, string raceId)
		{
			PendingAdRewardPrefs.SaveRaceMode(mode);
			OfferResult offer = await _adOffers.TryShowAsync(AdPlacement.Competition, raceId);
			if (!offer.IsGranted)
			{
				PendingAdRewardPrefs.Clear(AdPlacement.Competition);
				return RacingRewardDoubleResult.NotPaid(offer);
			}
			string text = await WaitForUnclaimedCompetitionAdAsync();
			if (text == null)
			{
				return RacingRewardDoubleResult.NotPaid(offer);
			}
			return await ClaimAsync(offer, mode, text);
		}

		private async UniTask<string> WaitForUnclaimedCompetitionAdAsync()
		{
			for (int attempt = 0; attempt < 5; attempt++)
			{
				if (attempt > 0)
				{
					await UniTask.Delay(UnclaimedRetryDelay);
				}
				IAdsRewardsBackend backend = _backend;
				string debugPlacement = ((attempt == 0) ? "Competition" : "");
				Result<List<UnclaimedAdReward>> result = await backend.GetUnclaimed(default, debugPlacement);
				if (result.IsFail)
				{
					continue;
				}
				foreach (UnclaimedAdReward item in result.Value)
				{
					if (item != null && item.Placement == "Competition" && !string.IsNullOrEmpty(item.AdId))
					{
						return item.AdId;
					}
				}
			}
			return null;
		}

		private async UniTask<RacingRewardDoubleResult> ClaimAsync(OfferResult offer, RacingRewardMode mode, string adId)
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
			for (int attempt = 0; attempt < 2; attempt++)
			{
				Result<List<AdRewardClaimResult>> result = await _backend.Claim(claims);
				if (result.IsFail)
				{
					return RacingRewardDoubleResult.NotPaid(offer);
				}
				AdRewardClaimResult adRewardClaimResult = ((result.Value.Count > 0) ? result.Value[0] : null);
				if (adRewardClaimResult == null)
				{
					return RacingRewardDoubleResult.NotPaid(offer);
				}
				if (adRewardClaimResult.Status == "GRANTED")
				{
					PendingAdRewardPrefs.Clear(AdPlacement.Competition);
					_dailyLimits.RegisterSpent(AdPlacement.Competition);
					int money = ((adRewardClaimResult.Money != 0) ? adRewardClaimResult.Money : (adRewardClaimResult.Claimed?.Money ?? 0));
					int coins = ((adRewardClaimResult.Coins != 0) ? adRewardClaimResult.Coins : (adRewardClaimResult.Claimed?.Coins ?? 0));
					return new RacingRewardDoubleResult(offer, rewardGranted: true, money, coins, adRewardClaimResult.Status, adId);
				}
				if (adRewardClaimResult.Status != "FAILED")
				{
					PendingAdRewardPrefs.Clear(AdPlacement.Competition);
					return RacingRewardDoubleResult.NotPaid(offer, adRewardClaimResult.Status, adId);
				}
			}
			return RacingRewardDoubleResult.NotPaid(offer, "FAILED", adId);
		}
	}
}
