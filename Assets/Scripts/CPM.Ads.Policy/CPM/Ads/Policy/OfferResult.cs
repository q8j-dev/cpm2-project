using System;
using System.Collections.Generic;
using CPM.Balance;

namespace CPM.Ads.Policy
{
	public readonly struct OfferResult
	{
		public readonly OfferOutcome Outcome;

		public readonly AdFailureCause Cause;

		public readonly AdFailureBlame Blame;

		public readonly IReadOnlyList<ItemInBox> Rewards;

		public readonly OfferState StateAfter;

		public bool IsGranted => Outcome == OfferOutcome.Granted;

		public OfferResult(OfferOutcome outcome, AdFailureCause cause, AdFailureBlame blame, IReadOnlyList<ItemInBox> rewards, OfferState stateAfter)
		{
			Outcome = outcome;
			Cause = cause;
			Blame = blame;
			Rewards = rewards;
			StateAfter = stateAfter;
		}

		public static OfferResult Blocked(OfferState state)
		{
			return new OfferResult(OfferOutcome.Blocked, AdFailureCause.None, AdFailureBlame.None, Array.Empty<ItemInBox>(), state);
		}

		public static OfferResult Granted(IReadOnlyList<ItemInBox> rewards, OfferState state)
		{
			return new OfferResult(OfferOutcome.Granted, AdFailureCause.None, AdFailureBlame.None, rewards, state);
		}

		public static OfferResult Skipped(OfferState state)
		{
			return new OfferResult(OfferOutcome.Skipped, AdFailureCause.None, AdFailureBlame.None, Array.Empty<ItemInBox>(), state);
		}

		public static OfferResult Failed(AdFailureCause cause, AdFailureBlame blame, OfferState state)
		{
			return new OfferResult(OfferOutcome.Failed, cause, blame, Array.Empty<ItemInBox>(), state);
		}
	}
}
