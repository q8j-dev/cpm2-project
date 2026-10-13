using System;
using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	public class NewCarsPurchaseHandler : IPurchaseHandler, IDisposable
	{
		private readonly AllCarsInfo _allCarsInfo;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private readonly PurchaseItemService _purchaseItemService;

		private NewCarVisualData _newCarVisualData;

		private readonly CommonProvider _commonProvider;

		private readonly List<IPurchaseCheckProductToBuy> _purchaseChecksProductToBuy = new List<IPurchaseCheckProductToBuy>();

		public NewCarsPurchaseHandler(IConnectionManager connectionManager, AllCarsInfo allCarsInfo, PurchaseItemsCollection purchaseItemsCollection, PurchaseItemService purchaseItemService, CommonProvider commonProvider)
		{
			_allCarsInfo = allCarsInfo;
			_purchaseItemsCollection = purchaseItemsCollection;
			_purchaseItemService = purchaseItemService;
			_commonProvider = commonProvider;
			_purchaseChecksProductToBuy.Add(new NewCarPurchaseCheckerGarageIsFully(showMessageBeforeBuySlot: false));
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerServerAvailable());
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerHashCheck(connectionManager));
		}

		public void Dispose()
		{
		}

		public bool IsHandlerForProduct(string productID)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(productID);
			if (purchaseItemData == null)
			{
				return false;
			}
			return _allCarsInfo.GetVehicleStockTuningData(purchaseItemData.purchaseItemType);
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

		public async UniTask<PurchaseItemResultStatus> ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, string attemptId)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(purchaseProduct.ProductID);
			if (purchaseItemData == null)
			{
				return PurchaseItemResultStatus.Error;
			}
			VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(purchaseItemData.purchaseItemType);
			if (!vehicleStockTuningData)
			{
				return PurchaseItemResultStatus.Error;
			}
			int newCarId = vehicleStockTuningData.Serial;
			int newSlotGarageIndex = -1;
			if (!_commonProvider.SlotService.TryGetEmptySlot(out var _))
			{
				newSlotGarageIndex = _commonProvider.SlotService.GetFirstGarageIndexNotFull();
			}
			PurchasePayloadNewCar payLoad = new PurchasePayloadNewCar(args.purchasedProduct.definition.id, args.purchasedProduct.transactionID, attemptId, newCarId, newSlotGarageIndex);
			PurchaseItemResult buyResult;
			try
			{
				buyResult = await _purchaseItemService.TryPurchase(purchaseItemData.purchaseItemType, payLoad, new AdvancedInt(0));
			}
			catch (Exception exception)
			{
				buyResult = PurchaseItemResult.GetFailResult();
				Debug.LogException(exception);
			}
			if (buyResult == null)
			{
				buyResult = PurchaseItemResult.GetFailResult();
			}
			try
			{
				if (!buyResult.IsSuccess)
				{
					return ServerPurchaseStatusMapper.ToResultStatus(buyResult.StatusCode);
				}
				if (!_commonProvider.SlotService.TryGetEmptySlot(out var _) && (!_commonProvider.SlotService.TryGetFreeSlot(newSlotGarageIndex) || newSlotGarageIndex == -1))
				{
					return PurchaseItemResultStatus.Error;
				}
				_newCarVisualData = await CarExtension.GetDefaultVisualDataFromPreset(newCarId);
				return (CarExtension.BuyCarFromPurchase(newCarId, (_newCarVisualData != null) ? new Action<CarInfo>(RecordStyleForNewPurchasedCar) : null, CarExtension.RecordBodyKitsForNewPurchasedCar, buyResult.Data) == BuyCarFromPurchaseResult.Success) ? PurchaseItemResultStatus.Success : PurchaseItemResultStatus.Error;
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
				return PurchaseItemResultStatus.Error;
			}
		}

		public void RecordStyleForNewPurchasedCar(CarInfo carInfo)
		{
			CarExtension.RecordStyleForNewPurchasedCar(carInfo, _newCarVisualData);
		}

		public string GetOfferId(string productID)
		{
			return "";
		}

		public List<ItemIdData> GetItemIds(string productID)
		{
			List<ItemIdData> list = new List<ItemIdData>();
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(productID);
			if (purchaseItemData == null)
			{
				return list;
			}
			VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(purchaseItemData.purchaseItemType);
			if (!vehicleStockTuningData)
			{
				return list;
			}
			int num = vehicleStockTuningData.Serial;
			list.Add(new ItemIdData
			{
				ItemId = num.ToString(),
				Amount = 1
			});
			return list;
		}

		public async UniTask PurchaseFailedAsync(PurchaseProduct purchaseProduct, Product product, PurchaseFailureReason failureReason)
		{
		}
	}
}
