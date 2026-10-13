using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class RealPurchaseAnalytic : AnalyticProviderBase
	{
		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		public RealPurchaseAnalytic(AnalyticEventFactory analyticEventFactory, PurchaseItemsCollection purchaseItemsCollection)
			: base(analyticEventFactory)
		{
			_purchaseItemsCollection = purchaseItemsCollection;
		}

		public void Log(PurchaseProduct purchaseProduct, PurchaseRealErrorOrigin errorOrigin, PurchaseRealErrorReason errorReason, PurchaseRealErrorSdkReason errorSdkReason, bool isRetry, string offerId, List<ItemIdData> itemIds, string transactionId, string attemptId, PurchaseStatus purchaseStatus)
		{
			if (purchaseProduct != null)
			{
				string productId = purchaseProduct.ProductID;
				PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(purchaseProduct.ProductID);
				if (purchaseItemData != null && purchaseItemData.purchaseItemType != PurchaseItemType.None)
				{
					productId = purchaseItemData.AnalyticsProductId;
				}
				LogPurchaseReal(purchaseStatus, errorOrigin, errorReason, errorSdkReason, isRetry, offerId, purchaseProduct.ItemDefId, productId, purchaseProduct.LocalizedPrice, purchaseProduct.IsoCurrencyCode, itemIds, transactionId, attemptId, purchaseProduct.ProductType);
			}
		}

		private void LogPurchaseReal(PurchaseStatus status, PurchaseRealErrorOrigin errorOrigin, PurchaseRealErrorReason errorReason, PurchaseRealErrorSdkReason errorSdkReason, bool isRetry, string offerId, int steamItemDefId, string productId, float price, string priceType, List<ItemIdData> itemIds, string transactionId, string attemptId = "", ProductType purchaseRealType = ProductType.Consumable)
		{
			base.AnalyticEventFactory.CreateEvent<PurchaseRealEvent, PurchaseRealContext>(new PurchaseRealContext
			{
				Status = status,
				AttemptId = attemptId,
				ProductType = purchaseRealType,
				ErrorOrigin = errorOrigin,
				ErrorReason = errorReason,
				IsRetry = isRetry,
				SdkFailReason = errorSdkReason,
				OfferId = offerId,
				SteamItemDefId = steamItemDefId,
				ProductId = productId,
				PriceType = priceType,
				Price = price,
				Items = itemIds,
				TransactionId = transactionId
			}).Send();
		}
	}
}
