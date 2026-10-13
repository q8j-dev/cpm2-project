using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class PurchaseVirtualContext : IEventContext
	{
		public PurchaseStatus Status;

		public CurrencyType CurrencyType;

		public PurchaseType PurchaseType;

		public string SubType;

		public PurchaseChannel PurchaseChannel;

		public string SellerType;

		public string SellerUserId;

		public string ListingId;

		public string DealId;

		public float Price;

		public string WindowId;

		public string ItemId;
	}
}
