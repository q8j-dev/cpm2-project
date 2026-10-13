using System;

namespace _Car_Parking.Scripts.Trade
{
	public class Offer
	{
		private TradeOfferState _tradeOfferState;

		protected float LifeTimeSeconds;

		public string TradeID { get; protected set; }

		public DateTime TimeCreated { get; protected set; }

		public float SecondUntilExpiration
		{
			get
			{
				if (LifeTimeSeconds > 0f)
				{
					return LifeTimeSeconds - (float)(DateTime.Now - TimeCreated).TotalSeconds;
				}
				return 100f;
			}
		}

		public TradeOfferState State
		{
			get
			{
				return _tradeOfferState;
			}
			set
			{
				_tradeOfferState = value;
				OnStateChangedEvent?.Invoke(value);
			}
		}

		public event Action<TradeOfferState> OnStateChangedEvent;
	}
}
