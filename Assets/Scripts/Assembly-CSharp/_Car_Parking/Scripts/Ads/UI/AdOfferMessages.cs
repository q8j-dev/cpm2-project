using CPM.Ads.Policy;

namespace _Car_Parking.Scripts.Ads.UI
{
	public sealed class AdOfferMessages
	{
		private const string LimitReachedKey = "Advertising.LimitReached";

		private const string AdLoadingKey = "FuelDriveUI.AdStillLoading";

		private const string NoAdsAvailableKey = "FuelDriveUI.AdNoneAvailableNow";

		public float CooldownSeconds(OfferState state)
		{
			if (state.Availability != OfferAvailability.Cooldown)
			{
				return 0f;
			}
			return state.CooldownSecondsLeft;
		}

		public bool TryGetBlockedKey(OfferState state, out string key)
		{
			switch (state.Availability)
			{
			case OfferAvailability.ExhaustedContext:
			case OfferAvailability.ExhaustedSession:
			case OfferAvailability.ExhaustedToday:
				key = "Advertising.LimitReached";
				return true;
			case OfferAvailability.NotReady:
				key = "FuelDriveUI.AdStillLoading";
				return true;
			case OfferAvailability.NoFill:
				key = "FuelDriveUI.AdNoneAvailableNow";
				return true;
			default:
				key = null;
				return false;
			}
		}
	}
}
