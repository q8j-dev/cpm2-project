using System;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Trade
{
	public interface ITradeOfferHandler : IDisposable
	{
		TradeItemType OfferItemType { get; }

		TradeItemType RequestItemType { get; }

		TradeOfferVisibleType OfferVisibleType { get; }

		UniTask<PreTradeResultActionType> PreTradeAction(TradeOffer tradeOffer);

		UniTask<bool> TradeAction(TradeOffer tradeOffer);

		UniTask PostTradeAction(TradeOffer tradeOffer);
	}
}
