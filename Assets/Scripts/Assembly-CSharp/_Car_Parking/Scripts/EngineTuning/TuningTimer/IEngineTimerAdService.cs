using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Analyzers;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public interface IEngineTimerAdService
	{
		OfferState CanOffer(int slotId);

		UniTask<OfferResult> TryReduceAsync(int slotId, AdOfferTracker tracker = null);

		OfferState CanOfferForEngine(int engineId);

		UniTask<OfferResult> TryReduceEngineAsync(int engineId, AdOfferTracker tracker = null);
	}
}
