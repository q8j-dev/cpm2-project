using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class P2pDealStatusContext : IEventContext
	{
		public string DealID { get; set; }

		public P2pDealStatusType StatusType { get; set; }

		public string SellerFbId { get; set; }

		public string SellerLocalId { get; set; }

		public string BuyerFbId { get; set; }

		public string BuyerLocalId { get; set; }

		public string SenderCarID { get; set; }

		public string ReceiverCarID { get; set; }

		public int Price { get; set; }

		public CurrencyType CurrencyType { get; set; }

		public PurchaseChannel PurchaseChannel { get; set; }

		public P2PRequestActionType RequestActionType { get; set; }
	}
}
