using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public interface IPurchaseHandler : IDisposable
	{
		bool IsHandlerForProduct(string productID);

		UniTask<PreBuyActionResult> PreBuyAction(string productID);

		UniTask<PurchaseItemResultStatus> ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, string attemptId);

		string GetOfferId(string productID);

		List<ItemIdData> GetItemIds(string productID);

		UniTask PurchaseFailedAsync(PurchaseProduct purchaseProduct, Product product, PurchaseFailureReason failureReason);
	}
}
