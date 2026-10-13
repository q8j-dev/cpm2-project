using CPM.Ads.Policy;

namespace _Car_Parking.Scripts.Drag
{
	public readonly struct RacingRewardDoubleResult
	{
		public readonly OfferResult Offer;

		public readonly bool RewardGranted;

		public readonly int Money;

		public readonly int Coins;

		public readonly string ClaimStatus;

		public readonly string ServerAdId;

		public RacingRewardDoubleResult(OfferResult offer, bool rewardGranted, int money, int coins, string claimStatus, string serverAdId = null)
		{
			Offer = offer;
			RewardGranted = rewardGranted;
			Money = money;
			Coins = coins;
			ClaimStatus = claimStatus;
			ServerAdId = serverAdId;
		}

		public static RacingRewardDoubleResult NotPaid(OfferResult offer, string claimStatus = null, string serverAdId = null)
		{
			return new RacingRewardDoubleResult(offer, rewardGranted: false, 0, 0, claimStatus, serverAdId);
		}
	}
}
