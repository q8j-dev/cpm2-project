using System;
using System.Collections.Generic;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	public class CoinPurchaseHandler : IPurchaseHandler, IDisposable
	{
		private readonly List<IPurchaseCheckProductToBuy> _purchaseChecksProductToBuy = new List<IPurchaseCheckProductToBuy>();

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private readonly PurchaseItemService _purchaseItemService;

		public CoinPurchaseHandler(IConnectionManager connectionManager, PurchaseItemsCollection purchaseItemsCollection, CommonProvider commonProvider, PurchaseItemService purchaseItemService)
		{
			_purchaseItemsCollection = purchaseItemsCollection;
			_purchaseItemService = purchaseItemService;
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerHashCheck(connectionManager));
		}

		public void Dispose()
		{
		}

		public async UniTask<PreBuyActionResult> PreBuyAction(string productID)
		{
			for (int i = 0; i < _purchaseChecksProductToBuy.Count; i++)
			{
				PreBuyActionResult preBuyActionResult = await _purchaseChecksProductToBuy[i].CheckProductToBuyAsync(productID);
				if (preBuyActionResult != PreBuyActionResult.Success)
				{
					return preBuyActionResult;
				}
			}
			return PreBuyActionResult.Success;
		}

		public bool IsHandlerForProduct(string productID)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(productID);
			if (purchaseItemData == null || purchaseItemData.RealPurchaseData == null)
			{
				return false;
			}
			return purchaseItemData.RealPurchaseData.Type == RealPurchaseType.Coin;
		}

		public async UniTask<PurchaseItemResultStatus> ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, string attemptId)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(purchaseProduct.ProductID);
			if (purchaseItemData == null || purchaseItemData.RealPurchaseData == null || purchaseItemData.RealPurchaseData.Type != RealPurchaseType.Coin)
			{
				return PurchaseItemResultStatus.Error;
			}
			PurchasePayloadCoin payLoad = new PurchasePayloadCoin(args.purchasedProduct.definition.id, args.purchasedProduct.transactionID, attemptId);
			PurchaseItemResult buyResult = null;
			try
			{
				buyResult = await _purchaseItemService.TryPurchase(purchaseItemData.purchaseItemType, payLoad, new AdvancedInt(0));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			if (buyResult == null)
			{
				buyResult = PurchaseItemResult.GetFailResult();
			}
			if (buyResult.IsSuccess)
			{
				CurrencyManagerController.SetCoins(new AdvancedInt(buyResult.Value));
			}
			PurchaseItemResultStatus purchaseItemResultStatus = ServerPurchaseStatusMapper.ToResultStatus(buyResult.StatusCode);
			ManagerUIHelper.SetMessage((buyResult.IsSuccess || purchaseItemResultStatus == PurchaseItemResultStatus.ItemWasBought) ? new LocalizedString("PurchaserContent.ThankYouForPurchase") : new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f);
			if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.CoinPurchaseWindow)
			{
				V.CoreGameManager.FreeDriveProvider.CoinPurchaseWindow.OnPurchaseDone();
			}
			return purchaseItemResultStatus;
		}

		public string GetOfferId(string productID)
		{
			return "";
		}

		public List<ItemIdData> GetItemIds(string productID)
		{
			List<ItemIdData> list = new List<ItemIdData>();
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(productID);
			if (purchaseItemData == null || purchaseItemData.RealPurchaseData == null || purchaseItemData.RealPurchaseData.Type != RealPurchaseType.Coin)
			{
				return list;
			}
			list.Add(new ItemIdData
			{
				ItemId = "HARD",
				Amount = purchaseItemData.RealPurchaseData.Amount
			});
			return list;
		}

		public async UniTask PurchaseFailedAsync(PurchaseProduct purchaseProduct, Product product, PurchaseFailureReason failureReason)
		{
		}
	}
}
