using System;
using System.Collections.Generic;
using CPM.Connection;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	public class GaragePurchaseHandler : IPurchaseHandler, IDisposable
	{
		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private readonly IGarageBuyService _garageBuyService;

		private readonly PurchaseItemService _purchaseItemService;

		private readonly List<IPurchaseCheckProductToBuy> _purchaseChecksProductToBuy = new List<IPurchaseCheckProductToBuy>();

		public GaragePurchaseHandler(IConnectionManager connectionManager, PurchaseItemsCollection purchaseItemsCollection, IGarageBuyService garageBuyService, PurchaseItemService purchaseItemService)
		{
			_purchaseItemsCollection = purchaseItemsCollection;
			_garageBuyService = garageBuyService;
			_purchaseItemService = purchaseItemService;
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerHashCheck(connectionManager));
		}

		public void Dispose()
		{
		}

		public bool IsHandlerForProduct(string productID)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(productID);
			if (purchaseItemData == null || purchaseItemData.RealPurchaseData == null)
			{
				return false;
			}
			return purchaseItemData.RealPurchaseData.Type == RealPurchaseType.Garage;
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
			if (GarageBuyService.BlockGarage2Purchase)
			{
				return PreBuyActionResult.BlockGarage2Purchase;
			}
			_garageBuyService.SetBlockGarage2Purchase(value: false).Forget();
			return PreBuyActionResult.Success;
		}

		public async UniTask<PurchaseItemResultStatus> ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, string attemptId)
		{
			if (GarageBuyService.BlockGarage2Purchase)
			{
				return PurchaseItemResultStatus.Error;
			}
			PurchaseItemData item = _purchaseItemsCollection.GetPurchaseItemData(purchaseProduct.ProductID);
			if (item == null || item.RealPurchaseData == null || item.RealPurchaseData.Type != RealPurchaseType.Garage)
			{
				return PurchaseItemResultStatus.Error;
			}
			_garageBuyService.SetBlockGarage2Purchase(value: true).Forget();
			PurchasePayloadGarage payLoad = new PurchasePayloadGarage(args.purchasedProduct.definition.id, args.purchasedProduct.transactionID, attemptId);
			PurchaseItemResult buyResult = null;
			try
			{
				buyResult = await _purchaseItemService.TryPurchase(item.purchaseItemType, payLoad, new AdvancedInt(0));
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
				ManagerUIHelper.SetMessage(new LocalizedString("PurchaserContent.ThankYouForPurchase"), 3f);
				_garageBuyService.GarageBuySuccess(item.RealPurchaseData.GarageIndex);
			}
			else if (buyResult.StatusCode == -10)
			{
				ManagerUIHelper.SetMessage("Purchase failed. Please contact game support.", 3f, NotificationType.Error);
			}
			return ServerPurchaseStatusMapper.ToResultStatus(buyResult.StatusCode);
		}

		public string GetOfferId(string productID)
		{
			return "";
		}

		public List<ItemIdData> GetItemIds(string productID)
		{
			string itemId = _purchaseItemsCollection.GetPurchaseItemData(productID)?.PurchaseId ?? "Garage";
			return new List<ItemIdData>
			{
				new ItemIdData
				{
					ItemId = itemId,
					Amount = 1
				}
			};
		}

		public async UniTask PurchaseFailedAsync(PurchaseProduct purchaseProduct, Product product, PurchaseFailureReason failureReason)
		{
		}
	}
}
