using System;
using System.Collections.Generic;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Steamworks;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	public class CarPriceHelperV2 : IInitializable, IDisposable
	{
		private readonly IOfferSystem _offerSystem;

		private readonly AllCarsInfo _allCarsInfo;

		private readonly IPurchaseSystem _purchaseSystem;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		public CarPriceHelperV2(IOfferSystem offerSystem, AllCarsInfo allCarsInfo, IPurchaseSystem purchaseSystem, PurchaseItemsCollection purchaseItemsCollection)
		{
			_offerSystem = offerSystem;
			_allCarsInfo = allCarsInfo;
			_purchaseSystem = purchaseSystem;
			_purchaseItemsCollection = purchaseItemsCollection;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public CombinedPrice GetCarPrice(int carID)
		{
			VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(carID);
			if (!vehicleStockTuningData)
			{
				return new CombinedPrice
				{
					Coin = 20000
				};
			}
			vehicleStockTuningData.GetPrice(out var priceValue);
			CombinedPrice combinedPrice = new CombinedPrice();
			vehicleStockTuningData.GetPriceType(out var type);
			priceValue.Get(out ObscuredInt value);
			if (type)
			{
				combinedPrice.Coin = value;
			}
			else
			{
				combinedPrice.Money = value;
			}
			combinedPrice.RealСurrencyPrice = GetRealСurrencyPrice(carID).Item1;
			return combinedPrice;
		}

		public (RealСurrencyPrice, OfferModel) GetRealСurrencyPrice(int carID)
		{
			string text = "";
			List<OfferModel> offersWithCarId = _offerSystem.GetOffersWithCarId(carID);
			OfferModel offerModel = null;
			for (int i = 0; i < offersWithCarId.Count; i++)
			{
				if (offersWithCarId[i].Payload.RewardCount == 1 && offersWithCarId[i].IsValid)
				{
					offerModel = offersWithCarId[i];
					text = offersWithCarId[i].Payload.FirstBundleID;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(carID);
				if ((bool)vehicleStockTuningData && vehicleStockTuningData.PurchaseItemType != PurchaseItemType.None)
				{
					PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(vehicleStockTuningData.PurchaseItemType);
					if (purchaseItemData != null)
					{
						text = purchaseItemData.PurchaseId;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return (null, null);
			}
			VehicleStockTuningData vehicleStockTuningData2 = _allCarsInfo.GetVehicleStockTuningData(carID);
			if (!vehicleStockTuningData2 || vehicleStockTuningData2.PurchaseItemType == PurchaseItemType.None)
			{
				return (null, null);
			}
			PurchaseItemData purchaseItemData2 = _purchaseItemsCollection.GetPurchaseItemData(vehicleStockTuningData2.PurchaseItemType);
			if (!(purchaseItemData2?.SteamItem))
			{
				return (null, null);
			}
			InventoryDef steamInventoryDef = _purchaseSystem.GetSteamInventoryDef(purchaseItemData2.SteamItem.SteamDefId);
			if (steamInventoryDef == null)
			{
				return (null, null);
			}
			RealСurrencyPrice realСurrencyPrice = new RealСurrencyPrice(text, steamInventoryDef.LocalPrice, SteamInventory.Currency);
			realСurrencyPrice.PriceText = steamInventoryDef.LocalPriceFormatted;
			if (offerModel != null && offerModel.Payload.FirstSteamItemDefId > 0)
			{
				InventoryDef steamInventoryDef2 = _purchaseSystem.GetSteamInventoryDef(offerModel.Payload.FirstSteamItemDefId);
				if (steamInventoryDef2 != null)
				{
					realСurrencyPrice.OldPrice = steamInventoryDef.LocalPrice;
					realСurrencyPrice.OldPriceText = steamInventoryDef.LocalPriceFormatted;
					realСurrencyPrice.Price = steamInventoryDef2.LocalPrice;
					realСurrencyPrice.PriceText = steamInventoryDef2.LocalPriceFormatted;
					realСurrencyPrice.OfferID = offerModel.ID;
					realСurrencyPrice.Discount = offerModel.Payload.Discount;
				}
			}
			return (realСurrencyPrice, offerModel);
		}
	}
}
