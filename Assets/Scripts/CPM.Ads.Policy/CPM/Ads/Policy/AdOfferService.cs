using System;
using System.Collections.Generic;
using CPM.Balance;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Ads.Policy
{
	public sealed class AdOfferService : IAdOfferService
	{
		private readonly IAdsService _ads;

		private readonly IAdPlacementCatalog _catalog;

		private readonly IAdPolicyStore _store;

		private readonly IUtcClock _clock;

		private readonly IAdsDailyLimits _dailyLimits;

		private readonly AdPolicyEngine _engine;

		private static readonly TimeSpan ReadinessTtl = TimeSpan.FromSeconds(1.0);

		private int _sessionCount;

		private bool _inFlight;

		private bool _readinessCache;

		private DateTime _readinessCachedAt = DateTime.MinValue;

		private bool AdsHidden => _ads.LastRewardedLoadFailure == AdFailureCause.ManagerDisabled;

		public event Action Changed;

		public AdOfferService(IAdsService ads, IAdPlacementCatalog catalog, IAdPolicyStore store, IUtcClock clock, IAdsDailyLimits dailyLimits)
		{
			_ads = ads;
			_catalog = catalog;
			_store = store;
			_clock = clock;
			_dailyLimits = dailyLimits;
			_engine = new AdPolicyEngine(catalog, store);
		}

		public OfferState CanOffer(AdPlacement placement, string contextId)
		{
			DateTime dateTime = _clock.UtcNow();
			OfferState state = _engine.Evaluate(placement, contextId, dateTime, _sessionCount, AdsHidden, IsAdReady(dateTime));
			return ApplyDailyLimit(placement, state);
		}

		private OfferState ApplyDailyLimit(AdPlacement placement, OfferState state)
		{
			if (state.Availability == OfferAvailability.Hidden)
			{
				return state;
			}
			if (!_catalog.TryGet(placement, out var rules))
			{
				return state;
			}
			bool flag = _dailyLimits.TryGetSpent(placement, out var spent);
			bool flag2 = _dailyLimits.TryGetTotalSpent(out var totalSpent);
			if (!flag && !flag2)
			{
				return state;
			}
			int num = (flag ? Math.Max(0, rules.DailyCap - spent) : int.MaxValue);
			if (flag2)
			{
				num = Math.Min(num, Math.Max(0, _catalog.Global.DailyCap - totalSpent));
			}
			if (num <= 0 && state.Availability != OfferAvailability.ExhaustedContext)
			{
				return new OfferState(OfferAvailability.ExhaustedToday, 0, state.RemainingInContext, 0f, state.Rewards);
			}
			return new OfferState(state.Availability, num, state.RemainingInContext, state.CooldownSecondsLeft, state.Rewards);
		}

		private bool IsAdReady(DateTime now)
		{
			if (now - _readinessCachedAt < ReadinessTtl)
			{
				return _readinessCache;
			}
			bool isRewardedReady = _ads.IsRewardedReady;
			bool num = isRewardedReady != _readinessCache;
			_readinessCache = isRewardedReady;
			_readinessCachedAt = now;
			if (num)
			{
				RaiseChanged();
			}
			return isRewardedReady;
		}

		public async UniTask<OfferResult> TryShowAsync(AdPlacement placement, string contextId)
		{
			if (_inFlight)
			{
				return OfferResult.Blocked(CanOffer(placement, contextId));
			}
			_inFlight = true;
			try
			{
				OfferState state = CanOffer(placement, contextId);
				if (!state.CanShowNow)
				{
					return OfferResult.Blocked(state);
				}
				IReadOnlyList<ItemInBox> rewards = state.Rewards;
				RewardedAdOutcome rewardedAdOutcome = await _ads.ShowRewardedAsync(placement.ToString());
				DateTime nowUtc = _clock.UtcNow();
				_readinessCachedAt = DateTime.MinValue;
				OfferResult result;
				switch (rewardedAdOutcome.Result)
				{
				case RewardedAdResult.Completed:
					_engine.RecordSuccess(placement, contextId, nowUtc);
					_sessionCount++;
					_store.Persist();
					result = OfferResult.Granted(rewards, CanOffer(placement, contextId));
					break;
				case RewardedAdResult.Skipped:
					_engine.RecordNoRewardClose(nowUtc);
					_store.Persist();
					result = OfferResult.Skipped(CanOffer(placement, contextId));
					break;
				default:
					if (rewardedAdOutcome.Cause == AdFailureCause.NoFill)
					{
						_engine.RecordNoFill(placement, nowUtc);
					}
					else
					{
						_engine.RecordNoRewardClose(nowUtc);
					}
					_store.Persist();
					Debug.LogWarning($"[Ads] {placement} impression failed: {rewardedAdOutcome}");
					result = OfferResult.Failed(rewardedAdOutcome.Cause, rewardedAdOutcome.Blame, CanOffer(placement, contextId));
					break;
				}
				RaiseChanged();
				return result;
			}
			finally
			{
				_inFlight = false;
			}
		}

		private void RaiseChanged()
		{
			try
			{
				Changed?.Invoke();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}
}
