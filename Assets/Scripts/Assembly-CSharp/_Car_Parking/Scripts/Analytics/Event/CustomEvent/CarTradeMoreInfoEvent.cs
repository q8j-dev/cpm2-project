using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	[Preserve]
	public class CarTradeMoreInfoEvent : DefaultEvent<CarTradeMoreInfoContext>
	{
		protected override string Name => "car_trade_more_info";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
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
				"viewer_user_id",
				base.Context.ViewerUserLocalId
			},
			{
				"owner_user_id",
				base.Context.OwnerUserLocalId
			},
			{
				"target_car_id",
				base.Context.TargetCarIdCarInstanceId
			},
			{
				"price_amount",
				base.Context.PriceAmount
			},
			{
				"price_currency",
				base.Context.PriceCurrency
			},
			{
				"is_repeat_view_purchase",
				base.Context.IsRepeatViewPurchase
			}
		};

		public CarTradeMoreInfoEvent(CPMAnalytics analytics, CarTradeMoreInfoContext context)
			: base(analytics, context)
		{
		}
	}
}
