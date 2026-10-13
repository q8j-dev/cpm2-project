namespace _Car_Parking.Scripts.Offers.Reward
{
	public interface IEmoteOfferBannersConfig
	{
		bool TryGetEntry(RewardType type, int packId, out EmoteOfferBannerData data);
	}
}
