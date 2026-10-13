using System;
using System.Collections.Generic;
using CPM.Balance;

namespace CPM.Ads.Policy
{
	public readonly struct OfferState
	{
		public readonly OfferAvailability Availability;

		public readonly int RemainingToday;

		public readonly int RemainingInContext;

		public readonly float CooldownSecondsLeft;

		public readonly IReadOnlyList<ItemInBox> Rewards;

		public static OfferState Hidden { get; } = new OfferState(OfferAvailability.Hidden, 0, 0, 0f, Array.Empty<ItemInBox>());

		public bool IsOfferable => Availability != OfferAvailability.Hidden;

		public bool CanShowNow => Availability == OfferAvailability.Ready;

		public OfferState(OfferAvailability availability, int remainingToday, int remainingInContext, float cooldownSecondsLeft, IReadOnlyList<ItemInBox> rewards)
		{
			Availability = availability;
			RemainingToday = remainingToday;
			RemainingInContext = remainingInContext;
			CooldownSecondsLeft = cooldownSecondsLeft;
			Rewards = rewards;
		}
	}
}
