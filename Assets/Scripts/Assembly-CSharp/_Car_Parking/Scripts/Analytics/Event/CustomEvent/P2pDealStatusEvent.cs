using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class P2pDealStatusEvent : DefaultEvent<P2pDealStatusContext>
	{
		protected override string Name => "p2p_deal_status";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"deal_id",
						base.Context.DealID
					},
					{
						"status",
						base.Context.StatusType
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
						"sender_car_id",
						base.Context.SenderCarID
					},
					{
						"receiver_car_id",
						base.Context.ReceiverCarID
					},
					{
						"purchase_channel",
						base.Context.PurchaseChannel
					},
					{
						"request_action",
						base.Context.RequestActionType
					}
				};
				if (base.Context.RequestActionType == P2PRequestActionType.Sell)
				{
					dictionary.Add("offer_price_minor", base.Context.Price);
					dictionary.Add("offer_currency_type", base.Context.CurrencyType);
				}
				return dictionary;
			}
		}

		public P2pDealStatusEvent(CPMAnalytics analytics, P2pDealStatusContext context)
			: base(analytics, context)
		{
		}
	}
}
