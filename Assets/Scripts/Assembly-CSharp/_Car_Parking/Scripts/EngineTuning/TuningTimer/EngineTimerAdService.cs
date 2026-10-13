using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	public sealed class EngineTimerAdService : IEngineTimerAdService
	{
		private const double MinRemainingSecondsForOffer = 30.0;

		private readonly ITimerTuningService _timers;

		private readonly IEngineTuningTimerService _engineTimers;

		private readonly IAdOfferService _adOffers;

		private readonly IClientAdRewardConsumeService _consume;

		public EngineTimerAdService(ITimerTuningService timers, IEngineTuningTimerService engineTimers, IAdOfferService adOffers, IClientAdRewardConsumeService consume)
		{
			_timers = timers;
			_engineTimers = engineTimers;
			_adOffers = adOffers;
			_consume = consume;
		}

		public OfferState CanOffer(int slotId)
		{
			if (!TryGetOfferableTimer(slotId, out var info))
			{
				return OfferState.Hidden;
			}
			return _adOffers.CanOffer(AdPlacement.EngineTimerReduce, info.TimerId);
		}

		public async UniTask<OfferResult> TryReduceAsync(int slotId, AdOfferTracker tracker = null)
		{
			if (!TryGetOfferableTimer(slotId, out var info))
			{
				return OfferResult.Blocked(OfferState.Hidden);
			}
			string timerId = info.TimerId;
			OfferResult result = await _adOffers.TryShowAsync(AdPlacement.EngineTimerReduce, timerId);
			tracker?.ShowFinished(result);
			if (!result.IsGranted)
			{
				return result;
			}
			if (!OfferRewards.TryTimeSeconds(result.Rewards, out var seconds))
			{
				Debug.LogError("[Ads] EngineTimerReduce was granted but the balance document carries no Time reward; the reward is lost.");
				tracker?.RewardFailed("GRANT_ERROR", "balance document carries no Time reward");
				return result;
			}
			if (_timers.TryApplyAdReduction(slotId, timerId, seconds))
			{
				tracker?.RewardGranted("Time", seconds);
				_consume.ConsumeInBackground(AdPlacement.EngineTimerReduce);
				return result;
			}
			if (!_timers.TryGetEngineTimerInfo(slotId, out var _))
			{
				Debug.Log("[Ads] EngineTimerReduce landed after the installation had already completed; nothing left to shorten.");
				tracker?.RewardGranted("Time", seconds);
				return result;
			}
			Debug.LogWarning($"[Ads] EngineTimerReduce could not finish slot {slotId} " + "(timer " + timerId + "); the reward is lost.");
			tracker?.RewardFailed("GRANT_ERROR", "the paid-for timer is no longer on the slot");
			return result;
		}

		public OfferState CanOfferForEngine(int engineId)
		{
			if (!TryGetOfferableEngineTimer(engineId, out var info))
			{
				return OfferState.Hidden;
			}
			return _adOffers.CanOffer(AdPlacement.EngineTimerReduce, info.TimerId);
		}

		public async UniTask<OfferResult> TryReduceEngineAsync(int engineId, AdOfferTracker tracker = null)
		{
			if (!TryGetOfferableEngineTimer(engineId, out var info))
			{
				return OfferResult.Blocked(OfferState.Hidden);
			}
			string timerId = info.TimerId;
			OfferResult result = await _adOffers.TryShowAsync(AdPlacement.EngineTimerReduce, timerId);
			tracker?.ShowFinished(result);
			if (!result.IsGranted)
			{
				return result;
			}
			if (!OfferRewards.TryTimeSeconds(result.Rewards, out var seconds))
			{
				Debug.LogError("[Ads] EngineTimerReduce was granted but the balance document carries no Time reward; the reward is lost.");
				tracker?.RewardFailed("GRANT_ERROR", "balance document carries no Time reward");
				return result;
			}
			if (_engineTimers.TryApplyAdReduction(engineId, timerId, seconds))
			{
				tracker?.RewardGranted("Time", seconds);
				_consume.ConsumeInBackground(AdPlacement.EngineTimerReduce);
				return result;
			}
			if (!_engineTimers.TryGetTimerInfo(engineId, out var _))
			{
				Debug.Log("[Ads] EngineTimerReduce landed after the bench assembly had already completed; nothing left to shorten.");
				tracker?.RewardGranted("Time", seconds);
				return result;
			}
			Debug.LogWarning($"[Ads] EngineTimerReduce could not finish engine {engineId} " + "(timer " + timerId + "); the reward is lost.");
			tracker?.RewardFailed("GRANT_ERROR", "the paid-for timer is no longer on the bench");
			return result;
		}

		private bool TryGetOfferableEngineTimer(int engineId, out EngineTimerInfo info)
		{
			if (!_engineTimers.TryGetTimerInfo(engineId, out info))
			{
				return false;
			}
			return info.RemainingSeconds >= 30.0;
		}

		private bool TryGetOfferableTimer(int slotId, out EngineTimerInfo info)
		{
			if (!_timers.TryGetEngineTimerInfo(slotId, out info))
			{
				return false;
			}
			if (!info.IsEngineInstall && !info.HasGearBox)
			{
				return false;
			}
			return info.RemainingSeconds >= 30.0;
		}
	}
}
