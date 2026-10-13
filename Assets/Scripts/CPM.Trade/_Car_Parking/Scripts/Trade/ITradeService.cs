using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Trade
{
	public interface ITradeService
	{
		bool Enabled { get; }

		IReadOnlyCollection<TradeOffer> TradeOffers { get; }

		TradeOffer GetOffer(string tradeId);

		void ClearOffers();

		bool AddOffer(TradeOffer offer);

		bool CancelOrDeclineOffer(string tradeId, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer = true);

		void CancelOrDeclineAllOffers(TradeOfferCancelReason cancelReason, bool notifyOtherPlayer, TradeOfferVisibleFilter visibleFilter = TradeOfferVisibleFilter.All);

		void CancelOrDeclineAllOffersByActorNr(int actorNr, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer);

		bool CancelOffer(string tradeId, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer = true, bool isCanceled = true);

		void CompleteOffer(string tradeId);

		UniTask<bool> TryTradeAsync(string tradeId);

		List<TradeOffer> GetMyActiveOffers();
	}
}
