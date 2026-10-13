using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class PurchaseVirtualEvent : DefaultEvent<PurchaseVirtualContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status.ToString()
			},
			{
				"currency_type",
				base.Context.CurrencyType
			},
			{
				"purchase_type",
				base.Context.PurchaseType
			},
			{
				"sub_type",
				base.Context.SubType
			},
			{
				"purchase_channel",
				base.Context.PurchaseChannel
			},
			{
				"seller_type",
				base.Context.SellerType
			},
			{
				"seller_user_id",
				base.Context.SellerUserId
			},
			{
				"listing_id",
				base.Context.ListingId
			},
			{
				"deal_id",
				base.Context.DealId
			},
			{
				"price",
				base.Context.Price
			},
			{
				"window_id",
				base.Context.WindowId
			},
			{
				"item_id",
				base.Context.ItemId
			}
		};

		protected override string Name => "purchase_virtual";

		public PurchaseVirtualEvent(CPMAnalytics analytics, PurchaseVirtualContext context)
			: base(analytics, context)
		{
		}
	}
}
