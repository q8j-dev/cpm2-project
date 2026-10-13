using System;
using CPM.Balance;
using UnityEngine;

namespace CPM.Ads.Policy
{
	public sealed class AdPolicyEngine
	{
		private static readonly ItemInBox[] NoRewards = Array.Empty<ItemInBox>();

		private readonly IAdPlacementCatalog _catalog;

		private readonly IAdPolicyStore _store;

		public AdPolicyEngine(IAdPlacementCatalog catalog, IAdPolicyStore store)
		{
			_catalog = catalog;
			_store = store;
		}

		public OfferState Evaluate(AdPlacement placement, string contextId, DateTime nowUtc, int sessionCount, bool adsHidden, bool adReady)
		{
			if (!_catalog.SystemEnabled | adsHidden)
			{
				return Hidden();
			}
			if (!_catalog.TryGet(placement, out var rules) || !rules.Enabled)
			{
				return Hidden();
			}
			if (rules.PerContextCap > 0 && string.IsNullOrEmpty(contextId))
			{
				Debug.LogError($"AdPolicyEngine: {placement} has a per-context cap but was asked " + "with an empty contextId; hiding the offer.");
				return Hidden();
			}
			RollDayIfNeeded(nowUtc);
			GlobalRules global = _catalog.Global;
			PlacementEntry orAddPlacement = AdPolicyStateOps.GetOrAddPlacement(_store.State, placement);
			int num = CountForContext(orAddPlacement, contextId);
			int remainingInContext = Math.Max(0, rules.PerContextCap - num);
			if (remainingInContext <= 0)
			{
				return State(OfferAvailability.ExhaustedContext);
			}
			if (sessionCount >= global.SessionCap)
			{
				return State(OfferAvailability.ExhaustedSession);
			}
			float num2 = SecondsUntil(nowUtc, orAddPlacement.NoFillUntilTicks);
			if (num2 > 0f)
			{
				return State(OfferAvailability.NoFill, num2);
			}
			float val = SecondsUntil(nowUtc, Offset(_store.State.LastSuccessTicks, global.SuccessCooldownSec));
			float val2 = SecondsUntil(nowUtc, Offset(_store.State.LastNoRewardCloseTicks, global.TechnicalCooldownSec));
			float num3 = Math.Max(val, val2);
			if (num3 > 0f)
			{
				return State(OfferAvailability.Cooldown, num3);
			}
			if (!adReady)
			{
				return State(OfferAvailability.NotReady);
			}
			return State(OfferAvailability.Ready);
			OfferState State(OfferAvailability availability, float cooldownLeft = 0f)
			{
				return new OfferState(availability, 0, remainingInContext, cooldownLeft, rules.Rewards);
			}
		}

		public void RollDayIfNeeded(DateTime nowUtc)
		{
			if (TryGetWindowStart(_store.State.DayWindowStartTicks, out var windowStart) && nowUtc < windowStart.AddDays(1.0))
			{
				return;
			}
			_store.State.DayWindowStartTicks = nowUtc.Date.Ticks;
			_store.State.GlobalDayCount = 0;
			foreach (PlacementEntry placement in _store.State.Placements)
			{
				placement.DayCount = 0;
				placement.Contexts.Clear();
			}
		}

		public void RecordSuccess(AdPlacement placement, string contextId, DateTime nowUtc)
		{
			RollDayIfNeeded(nowUtc);
			PlacementEntry orAddPlacement = AdPolicyStateOps.GetOrAddPlacement(_store.State, placement);
			orAddPlacement.DayCount++;
			AdPolicyStateOps.GetOrAddContext(orAddPlacement, contextId ?? string.Empty).Count++;
			_store.State.GlobalDayCount++;
			_store.State.LastSuccessTicks = nowUtc.Ticks;
		}

		public void RecordNoRewardClose(DateTime nowUtc)
		{
			RollDayIfNeeded(nowUtc);
			_store.State.LastNoRewardCloseTicks = nowUtc.Ticks;
		}

		public void RecordNoFill(AdPlacement placement, DateTime nowUtc)
		{
			RollDayIfNeeded(nowUtc);
			AdPolicyStateOps.GetOrAddPlacement(_store.State, placement).NoFillUntilTicks = nowUtc.AddSeconds(_catalog.Global.NoFillLockSec).Ticks;
		}

		private static OfferState Hidden()
		{
			return new OfferState(OfferAvailability.Hidden, 0, 0, 0f, NoRewards);
		}

		private static int CountForContext(PlacementEntry entry, string contextId)
		{
			if (string.IsNullOrEmpty(contextId))
			{
				return 0;
			}
			foreach (ContextEntry context in entry.Contexts)
			{
				if (context.Id == contextId)
				{
					return context.Count;
				}
			}
			return 0;
		}

		private static bool TryGetWindowStart(long ticks, out DateTime windowStart)
		{
			DateTime minValue = DateTime.MinValue;
			if (ticks >= minValue.Ticks)
			{
				minValue = DateTime.MaxValue;
				if (ticks <= minValue.Ticks)
				{
					windowStart = new DateTime(ticks, DateTimeKind.Utc);
					return true;
				}
			}
			windowStart = default;
			return false;
		}

		private static long Offset(long ticks, int seconds)
		{
			if (ticks != 0L)
			{
				return ticks + TimeSpan.FromSeconds(seconds).Ticks;
			}
			return 0L;
		}

		private static float SecondsUntil(DateTime nowUtc, long deadlineTicks)
		{
			if (deadlineTicks <= 0)
			{
				return 0f;
			}
			double num = (double)(deadlineTicks - nowUtc.Ticks) / 10000000.0;
			if (!(num > 0.0))
			{
				return 0f;
			}
			return (float)num;
		}
	}
}
