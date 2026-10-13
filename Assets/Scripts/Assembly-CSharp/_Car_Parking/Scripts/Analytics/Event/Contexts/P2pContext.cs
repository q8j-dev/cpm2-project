using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class P2pContext : IEventContext
	{
		public P2PStatus Status { get; set; }

		public string SellerLocalId { get; set; }

		public string SellerFbId { get; set; }

		public string BuyerLocalId { get; set; }

		public string BuyerFbId { get; set; }

		public CurrencyType CurrencyType { get; set; }

		public PurchaseChannel PurchaseChannel { get; set; }

		public string DealId { get; set; }

		public string ItemId { get; set; }

		public int AmountMinor { get; set; }

		public P2PSaleCreditFailReasonType FailReason { get; set; }
	}
}
