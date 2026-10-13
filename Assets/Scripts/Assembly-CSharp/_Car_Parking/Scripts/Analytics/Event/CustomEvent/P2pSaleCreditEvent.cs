using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class P2pSaleCreditEvent : DefaultEvent<P2pContext>
	{
		protected override string Name => "p2p_sale_credit";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status
			},
			{
				"seller_user_fb_id",
				base.Context.SellerFbId
			},
			{
				"seller_user_id",
				base.Context.SellerLocalId
			},
			{
				"buyer_user_id",
				base.Context.BuyerLocalId
			},
			{
				"buyer_user_fb_id",
				base.Context.BuyerFbId
			},
			{
				"currency_type",
				base.Context.CurrencyType
			},
			{
				"purchase_channel",
				base.Context.PurchaseChannel
			},
			{
				"deal_id",
				base.Context.DealId
			},
			{
				"item_id",
				base.Context.ItemId
			},
			{
				"gross_amount_minor",
				base.Context.AmountMinor
			},
			{
				"fail_reason",
				base.Context.FailReason
			}
		};

		public P2pSaleCreditEvent(CPMAnalytics analytics, P2pContext context)
			: base(analytics, context)
		{
		}
	}
}
