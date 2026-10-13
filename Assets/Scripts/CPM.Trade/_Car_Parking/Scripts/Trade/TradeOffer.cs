using System;

namespace _Car_Parking.Scripts.Trade
{
	public class TradeOffer : Offer
	{
		public TradeOfferVisibleType VisibleType { get; private set; }

		public int OfferActorNr { get; private set; }

		public int RequestActorNr { get; private set; }

		public TradeItem OfferItem { get; private set; }

		public TradeItem RequestItem { get; private set; }

		public bool CanCancel
		{
			get
			{
				if (base.State != TradeOfferState.Enabled)
				{
					return base.State == TradeOfferState.Pending;
				}
				return true;
			}
		}

		public bool CanComplete
		{
			get
			{
				if (base.State != TradeOfferState.Enabled)
				{
					return base.State == TradeOfferState.Pending;
				}
				return true;
			}
		}

		public TradeOffer(string tradeId, TradeItem offerItem, TradeItem requestItem, TradeOfferVisibleType visibleType, int offerActorNr, int requestActorNr, float timeLife)
		{
			base.TradeID = tradeId;
			OfferItem = offerItem;
			RequestItem = requestItem;
			VisibleType = visibleType;
			OfferActorNr = offerActorNr;
			RequestActorNr = requestActorNr;
			LifeTimeSeconds = timeLife;
			base.TimeCreated = DateTime.Now;
		}

		public void Reset()
		{
			base.State = TradeOfferState.Enabled;
			base.TimeCreated = DateTime.Now;
		}
	}
}
