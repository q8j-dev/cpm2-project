using System;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class CarTradeMoreInfoContext : IEventContext
	{
		public string DealId { get; set; }

		public CarTradeMoreInfoStatusType Status { get; set; }

		public string ViewerUserLocalId { get; set; }

		public string OwnerUserLocalId { get; set; }

		public string TargetCarIdCarInstanceId { get; set; }

		public int PriceAmount { get; set; }

		public CurrencyType PriceCurrency { get; set; }

		public bool IsRepeatViewPurchase { get; set; }
	}
}
