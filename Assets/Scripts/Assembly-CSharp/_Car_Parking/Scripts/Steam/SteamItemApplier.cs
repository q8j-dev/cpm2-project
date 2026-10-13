using System;
using CPM.Database;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Steam
{
	public class SteamItemApplier : ISteamItemApplier
	{
		private readonly IWalletService _walletService;

		private readonly CommonProvider _commonProvider;

		private readonly AllCarsInfo _allCarsInfo;

		private readonly IGarageBuyService _garageBuyService;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private NewCarVisualData _newCarVisualData;

		public SteamItemApplier(IWalletService walletService, CommonProvider commonProvider, AllCarsInfo allCarsInfo, IGarageBuyService garageBuyService, CurrencyManagerController currencyManagerController, PurchaseItemsCollection purchaseItemsCollection)
		{
			_walletService = walletService;
			_commonProvider = commonProvider;
			_allCarsInfo = allCarsInfo;
			_garageBuyService = garageBuyService;
			_currencyManagerController = currencyManagerController;
			_purchaseItemsCollection = purchaseItemsCollection;
		}

		public async UniTask<bool> Apply(SteamItem item, SteamPurchaseData data)
		{
			if (!item)
			{
				return false;
			}
			bool result;
			try
			{
				switch (item.SteamItemType)
				{
				case SteamItemType.Coins:
					result = await ApplyCoins(item, data);
					break;
				case SteamItemType.Money:
					result = await ApplyMoney(item, data);
					break;
				case SteamItemType.Car:
					result = await ApplyCar(item, data);
					break;
				case SteamItemType.Garage:
					result = await ApplyGarage(item, data);
					break;
				default:
					return false;
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				result = false;
			}
			return result;
		}

		private async UniTask<bool> ApplySimple(SteamPurchasePayload payload, SteamItem item, Action<WalletOperationResult> onSuccess)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(item.PurchaseItemType);
			payload.PaymentProductId = purchaseItemData.PurchaseId;
			ItemData data = new ItemData(purchaseItemData.PurchaseId, payload);
			WalletOperationResult walletOperationResult = await _walletService.PurchaseItemSteam(data);
			if (walletOperationResult.IsSuccess())
			{
				onSuccess?.Invoke(walletOperationResult);
				ManagerUIHelper.SetMessage(new LocalizedString("PurchaserContent.ThankYouForPurchase"), 3f);
				return true;
			}
			ManagerUIHelper.SetMessage(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f);
			return false;
		}

		private UniTask<bool> ApplyCoins(SteamItem item, SteamPurchaseData data)
		{
			SteamPurchasePayloadCoin payload = new SteamPurchasePayloadCoin(data.Itemdefid, data.TransactionID, data.Itemdefid, data.Name);
			return ApplySimple(payload, item, (WalletOperationResult result) =>
			{
				CurrencyManagerController.SetCoins(new AdvancedInt(result.Value));
				CurrencyManagerController.UpdateCoinsVisuals();
			});
		}

		private UniTask<bool> ApplyMoney(SteamItem item, SteamPurchaseData data)
		{
			SteamPurchasePayloadMoney payload = new SteamPurchasePayloadMoney(data.Itemdefid, data.TransactionID, data.Itemdefid, data.Name);
			return ApplySimple(payload, item, (WalletOperationResult result) =>
			{
				_currencyManagerController.SetMoney(new AdvancedInt(result.Value));
			});
		}

		private async UniTask<bool> ApplyCar(SteamItem item, SteamPurchaseData data)
		{
			VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(item.PurchaseItemType);
			if (!vehicleStockTuningData)
			{
				return false;
			}
			int newCarId = vehicleStockTuningData.Serial;
			SlotCollection slotCollection = _commonProvider.SlotService.SlotCollection;
			int newSlotGarageIndex = -1;
			if (!slotCollection.TryGetEmptySlot(out var _))
			{
				newSlotGarageIndex = (slotCollection.IsFull ? (-1) : slotCollection.Index);
			}
			SteamPurchasePayloadNewCar steamPurchasePayloadNewCar = new SteamPurchasePayloadNewCar(data.Itemdefid, data.TransactionID, newCarId, newSlotGarageIndex, data.Itemdefid, data.Name);
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(item.PurchaseItemType);
			steamPurchasePayloadNewCar.PaymentProductId = purchaseItemData.PurchaseId;
			ItemData data2 = new ItemData(purchaseItemData.PurchaseId, steamPurchasePayloadNewCar);
			WalletOperationResult buyResult;
			try
			{
				buyResult = await _walletService.PurchaseItemSteam(data2);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
			if (buyResult == null || !buyResult.IsSuccess())
			{
				return false;
			}
			if (!_commonProvider.SlotService.TryGetEmptySlot(out var _) && (!_commonProvider.SlotService.TryGetFreeSlot(newSlotGarageIndex) || newSlotGarageIndex == -1))
			{
				return false;
			}
			_newCarVisualData = await CarExtension.GetDefaultVisualDataFromPreset(newCarId);
			return CarExtension.BuyCarFromPurchase(newCarId, (_newCarVisualData != null) ? new Action<CarInfo>(RecordStyleForNewPurchasedCar) : null, CarExtension.RecordBodyKitsForNewPurchasedCar, buyResult.Data) == BuyCarFromPurchaseResult.Success;
		}

		private void RecordStyleForNewPurchasedCar(CarInfo carInfo)
		{
			CarExtension.RecordStyleForNewPurchasedCar(carInfo, _newCarVisualData);
		}

		private UniTask<bool> ApplyGarage(SteamItem item, SteamPurchaseData data)
		{
			SteamPurchasePayloadGarage payload = new SteamPurchasePayloadGarage(data.Itemdefid, data.TransactionID, data.Itemdefid, data.Name);
			return ApplySimple(payload, item, (WalletOperationResult result) =>
			{
				_garageBuyService.GarageBuySuccess(item.GarageIndex);
			});
		}
	}
}
