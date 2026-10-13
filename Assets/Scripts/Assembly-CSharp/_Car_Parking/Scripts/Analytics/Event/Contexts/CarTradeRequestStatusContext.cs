using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class CarTradeRequestStatusContext : IEventContext
	{
		public string DealId { get; set; }

		public CarTradeRequestStatusType Status { get; set; }

		public int TTlSec { get; set; }

		public string SenderFbId { get; set; }

		public string SenderLocalId { get; set; }

		public string ReceiverFbId { get; set; }

		public string ReceiverLocalId { get; set; }

		public string SenderCarInstanceId { get; set; }

		public string ReceiverCarInstanceId { get; set; }

		public P2PDeliveryMode DeliveryMode { get; set; }

		public P2PRequestActionType RequestAction { get; set; }

		public int Price { get; set; }

		public CurrencyType CurrencyType { get; set; }
	}
}
