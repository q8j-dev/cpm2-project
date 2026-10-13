using System;

namespace _Car_Parking.Scripts.Trade
{
	public class TradeOfferHandlersHolder : IDisposable
	{
		private readonly ITradeOfferHandler[] _handlers;

		public TradeOfferHandlersHolder(ITradeOfferHandler[] handlers)
		{
			_handlers = handlers;
		}

		public void Dispose()
		{
			for (int i = 0; i < _handlers.Length; i++)
			{
				_handlers[i].Dispose();
			}
		}

		public ITradeOfferHandler GetHandler(TradeOffer tradeOffer)
		{
			return GetHandler(tradeOffer.OfferItem.ItemType, tradeOffer.RequestItem.ItemType, tradeOffer.VisibleType);
		}

		public ITradeOfferHandler GetHandler(TradeItemType offerType, TradeItemType requestType, TradeOfferVisibleType offerVisibleType)
		{
			for (int i = 0; i < _handlers.Length; i++)
			{
				if (_handlers[i].OfferItemType == offerType && _handlers[i].RequestItemType == requestType && _handlers[i].OfferVisibleType == offerVisibleType)
				{
					return _handlers[i];
				}
			}
			return null;
		}
	}
}
