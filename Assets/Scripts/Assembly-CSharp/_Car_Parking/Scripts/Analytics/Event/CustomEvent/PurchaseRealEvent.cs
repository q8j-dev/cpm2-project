using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class PurchaseRealEvent : DefaultEvent<PurchaseRealContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status.ToString()
			},
			{
				"attempt_id",
				base.Context.AttemptId
			},
			{
				"purchase_type",
				ProductTypeToString(base.Context.ProductType)
			},
			{
				"error_origin",
				(int)base.Context.ErrorOrigin
			},
			{
				"error_reason",
				(int)base.Context.ErrorReason
			},
			{
				"sdk_fail_reason",
				(int)base.Context.SdkFailReason
			},
			{
				"is_retry",
				base.Context.IsRetry
			},
			{
				"offer_id",
				base.Context.OfferId
			},
			{
				"steam_item_def_id",
				base.Context.SteamItemDefId
			},
			{
				"product_id",
				base.Context.ProductId
			},
			{
				"price_type",
				base.Context.PriceType
			},
			{
				"price",
				base.Context.Price
			},
			{
				"item_id",
				JsonConvert.SerializeObject(base.Context.Items)
			},
			{
				"transaction_id",
				base.Context.TransactionId
			}
		};

		protected override string Name => "purchase_real";

		public PurchaseRealEvent(CPMAnalytics analytics, PurchaseRealContext context)
			: base(analytics, context)
		{
		}

		private static string ProductTypeToString(ProductType productType)
		{
			return productType switch
			{
				ProductType.Consumable => "CONSUMABLE", 
				ProductType.NonConsumable => "NON_CONSUMABLE", 
				ProductType.Subscription => "SUBSCRIPTION", 
				_ => "UNKNOWN", 
			};
		}
	}
}
