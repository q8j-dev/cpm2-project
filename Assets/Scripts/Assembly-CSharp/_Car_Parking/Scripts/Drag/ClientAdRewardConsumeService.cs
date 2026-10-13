using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class ClientAdRewardConsumeService : IClientAdRewardConsumeService
	{
		private const int UnclaimedAttempts = 5;

		private static readonly TimeSpan UnclaimedRetryDelay = TimeSpan.FromSeconds(2.0);

		private readonly IAdsRewardsBackend _backend;

		private readonly IAdsDailyLimits _dailyLimits;

		public ClientAdRewardConsumeService(IAdsRewardsBackend backend, IAdsDailyLimits dailyLimits)
		{
			_backend = backend;
			_dailyLimits = dailyLimits;
		}

		public void ConsumeInBackground(AdPlacement placement)
		{
			ConsumeAsync(placement).Forget();
		}

		public async UniTask<bool> ConsumeAsync(AdPlacement placement, CancellationToken ct = default(CancellationToken))
		{
			_ = 1;
			try
			{
				string text = await WaitForUnclaimedAsync(placement, ct);
				if (text == null)
				{
					return false;
				}
				Result<List<AdRewardClaimResult>> result = await _backend.Claim(new List<AdRewardClaimEntry>
				{
					new AdRewardClaimEntry
					{
						AdId = text,
						Type = "None",
						Id = placement.ToString()
					}
				}, ct);
				if (result.IsFail || result.Value.Count == 0 || result.Value[0] == null)
				{
					return false;
				}
				if (result.Value[0].Status == "CONSUMED")
				{
					_dailyLimits.RegisterSpent(placement);
					return true;
				}
				return false;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private async UniTask<string> WaitForUnclaimedAsync(AdPlacement placement, CancellationToken ct)
		{
			string wanted = placement.ToString();
			for (int attempt = 0; attempt < 5; attempt++)
			{
				if (attempt > 0)
				{
					await UniTask.Delay(UnclaimedRetryDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				Result<List<UnclaimedAdReward>> result = await _backend.GetUnclaimed(ct, (attempt == 0) ? wanted : "");
				if (result.IsFail)
				{
					continue;
				}
				foreach (UnclaimedAdReward item in result.Value)
				{
					if (item != null && item.Placement == wanted && !string.IsNullOrEmpty(item.AdId))
					{
						return item.AdId;
					}
				}
			}
			return null;
		}
	}
}
