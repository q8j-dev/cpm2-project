using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class CarTradeRequestStatusEvent : DefaultEvent<CarTradeRequestStatusContext>
	{
		protected override string Name => "car_trade_request_status";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"deal_id",
						base.Context.DealId
					},
					{
						"status",
						base.Context.Status
					},
					{
						"ttl_sec",
						base.Context.TTlSec
					},
					{
						"sender_user_fb_id",
						base.Context.SenderFbId
					},
					{
						"receiver_user_fb_id",
						base.Context.ReceiverFbId
					},
					{
						"sender_user_id",
						base.Context.SenderLocalId
					},
					{
						"receiver_user_id",
						base.Context.ReceiverLocalId
					},
					{
						"sender_car_id",
						base.Context.SenderCarInstanceId
					},
					{
						"receiver_car_id",
						base.Context.ReceiverCarInstanceId
					},
					{
						"delivery_mode",
						base.Context.DeliveryMode
					},
					{
						"request_action",
						base.Context.RequestAction
					}
				};
				if (base.Context.RequestAction == P2PRequestActionType.Sell)
				{
					dictionary.Add("offer_price_minor", base.Context.Price);
					dictionary.Add("offer_currency_type", base.Context.CurrencyType);
				}
				return dictionary;
			}
		}

		public CarTradeRequestStatusEvent(CPMAnalytics analytics, CarTradeRequestStatusContext context)
			: base(analytics, context)
		{
		}
	}
}
