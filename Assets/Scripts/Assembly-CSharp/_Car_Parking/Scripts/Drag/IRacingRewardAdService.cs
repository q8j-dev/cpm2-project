using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Drag
{
	public interface IRacingRewardAdService
	{
		OfferState CanOffer(string raceId);

		bool CanOfferForNewRace();

		RacingRewardAmounts SplitWonRewards(int[] rewards, int wonPlaces);

		UniTask<RacingRewardDoubleResult> TryDoubleRewardAsync(RacingRewardMode mode, string raceId);
	}
}
