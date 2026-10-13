using System.Collections.Generic;
using System.Linq;
using CPM.PlayerData;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class VirtualPurchaseAnalytic : AnalyticProviderBase
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly CommonProvider _commonProvider;

		private readonly PlayerDataStorage _playerDataStorage;

		private Dictionary<PersonWindowCategoryType, PurchaseType> _personCategoryByPurchaseTypes = new Dictionary<PersonWindowCategoryType, PurchaseType>
		{
			{
				PersonWindowCategoryType.Skin,
				PurchaseType.Skin
			},
			{
				PersonWindowCategoryType.Animation,
				PurchaseType.Animation
			},
			{
				PersonWindowCategoryType.Cloth,
				PurchaseType.Cloth
			}
		};

		private Dictionary<int, string> _purchasePersonSubTypes = new Dictionary<int, string>
		{
			{ -1, "animation" },
			{ 0, "hair" },
			{ 1, "face" },
			{ 2, "beard" },
			{ 3, "caps" },
			{ 4, "mask" },
			{ 5, "top" },
			{ 6, "gloves" },
			{ 7, "bag" },
			{ 8, "pants" },
			{ 9, "shoes" },
			{ 10, "glasses" }
		};

		public string BuyApartmentInWorld => "BuyApartmentWorldButton";

		public VirtualPurchaseAnalytic(AnalyticEventFactory analyticEventFactory, FreeDriveProvider freeDriveProvider, CommonProvider commonProvider, PlayerDataStorage playerDataStorage)
			: base(analyticEventFactory)
		{
			_freeDriveProvider = freeDriveProvider;
			_commonProvider = commonProvider;
			_playerDataStorage = playerDataStorage;
		}

		public void LogPurchaseVirtual_NewCars(bool mustBuySlot, CombinedPrice combinedPrice, PurchaseStatus purchaseStatus)
		{
			ISlotService slotService = AnalyticsDependencies.Get<ISlotService>();
			if (slotService != null && slotService.SlotCount != slotService.MaxSlotCount)
			{
				int num = 0;
				int price;
				if (mustBuySlot)
				{
					price = VehicleSlotPaymentService.Price;
					string itemId = (slotService.GetAllSlots().Last() + 2).ToString();
					LogPurchaseVirtual(purchaseStatus, CurrencyItemType.coins, PurchaseType.Slot, "", PurchaseChannel.GarageStore, SellerType.System, "", "", "", price, "NewCarsWindow", itemId);
					num = VehicleSlotPaymentService.Price;
				}
				CurrencyItemType currencyItemType = (combinedPrice.IsHaveMoney ? CurrencyItemType.money : CurrencyItemType.coins);
				price = ((currencyItemType != CurrencyItemType.coins) ? ((int)combinedPrice.Money) : ((int)combinedPrice.Coin - num));
				if (price > 0)
				{
					V.CoreGameManager.SellCarValues.CarID.Get(out int value);
					string itemId = value.ToString();
					LogPurchaseVirtual(purchaseStatus, currencyItemType, PurchaseType.Car, "", PurchaseChannel.GarageStore, SellerType.System, "", "", "", price, "NewCarsWindow", itemId);
				}
			}
		}

		public void LogPurchaseMoreInfo(string tradeOfferId, PurchaseStatus purchaseStatus)
		{
			TradeOffer offer = _commonProvider.TradeService.GetOffer(tradeOfferId);
			if (offer != null && offer.OfferItem is TradeItemCar tradeItemCar)
			{
				string localId = MultiPlayerAccountHelper.GetLocalId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				LogPurchaseVirtual(purchaseStatus, CurrencyItemType.coins, PurchaseType.MoreInfoAboutTradeCar, string.Empty, PurchaseChannel.P2PDirect, SellerType.Player, localId, string.Empty, tradeOfferId, BuyThisCarWindow.CarDetailsPrice, "BuyThisCarWindow", tradeItemCar.CarInstanceId);
			}
		}

		public void LogBuyCarInMultiplayer(string tradeOfferId, string windowId, PurchaseStatus purchaseStatus)
		{
			TradeOffer offer = _commonProvider.TradeService.GetOffer(tradeOfferId);
			if (offer == null)
			{
				return;
			}
			PurchaseChannel purchaseChannel = PurchaseChannel.P2PDirect;
			if (offer.VisibleType == TradeOfferVisibleType.Public)
			{
				purchaseChannel = PurchaseChannel.MultiplayerMarket;
			}
			int value = 0;
			if (offer.RequestItem is TradeItemMoney tradeItemMoney)
			{
				tradeItemMoney.Price.Get(out value);
			}
			string itemId = "";
			string listingId = "";
			if (offer.OfferItem is TradeItemCar tradeItemCar)
			{
				itemId = tradeItemCar.CarInstanceId;
				GameObject carByOwnerID = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(offer.OfferActorNr, needToCheckFirstCar: true);
				if ((bool)carByOwnerID && carByOwnerID.TryGetComponent<VehicleData>(out var component) && component.СurrentSellCarTrigger != null)
				{
					listingId = component.СurrentSellCarTrigger.Index.ToString();
				}
			}
			string localId = MultiPlayerAccountHelper.GetLocalId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.money, PurchaseType.Car, "", purchaseChannel, SellerType.Player, localId, listingId, offer.TradeID, value, windowId, itemId);
		}

		public void LogPurchaseVirtual_Engine(string engineId, int price, PurchaseStatus purchaseStatus)
		{
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.money, PurchaseType.CarTuning, "Engine_New", PurchaseChannel.GarageStore, SellerType.System, "", "", "", price, "DefaultEnginesMarketWindow", engineId);
		}

		public void LogPurchaseVirtual_InventoryItem(string itemId, int price, string subType, PurchaseStatus purchaseStatus)
		{
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.money, PurchaseType.CarTuning, subType, PurchaseChannel.GarageStore, SellerType.System, "", "", "", price, "MarketInventoryWindow", itemId);
		}

		public void LogPurchaseVirtual_CarSlot(int price, int slotId, PurchaseStatus purchaseStatus)
		{
			PurchaseChannel purchaseChannel = (FlagsStorage.IsMainMenu ? PurchaseChannel.GarageStore : PurchaseChannel.Other);
			string windowId = (FlagsStorage.IsMainMenu ? "GarageWindow" : "BuyThisCarWindow");
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.coins, PurchaseType.Slot, "", purchaseChannel, SellerType.System, "", "", "", price, windowId, slotId.ToString());
		}

		public void LogPurchaseVirtual_Maintenance(MaintenanceStates maintenanceStates, int price, PurchaseStatus purchaseStatus)
		{
			PurchaseChannel purchaseChannel = (FlagsStorage.IsMainMenu ? PurchaseChannel.GarageStore : PurchaseChannel.Other);
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.money, PurchaseType.CarMaintenance, "", purchaseChannel, SellerType.System, "", "", "", price, "MainteanceWindow", maintenanceStates.ToString());
		}

		public void LogPurchaseVirtual_Fuel(int price, string fuelId, string windowId, PurchaseStatus purchaseStatus)
		{
			PurchaseChannel purchaseChannel = (FlagsStorage.IsMainMenu ? PurchaseChannel.GarageStore : PurchaseChannel.Other);
			LogPurchaseVirtual(purchaseStatus, CurrencyItemType.money, PurchaseType.Fuel, "", purchaseChannel, SellerType.System, "", "", "", price, windowId, fuelId);
		}

		public void LogPurchaseVirtual_PersonCustomization(int type, int price, int itemId, PurchaseStatus purchaseStatus)
		{
			if (price > 0)
			{
				LogPurchaseVirtual(purchaseStatus, CurrencyItemType.coins, ConvertPersonWindowCategory(type), ConvertPurchasePersonSubTypes(type), PurchaseChannel.GarageStore, SellerType.System, "", "", "", price, "PersonCustomizationWindow", itemId.ToString());
			}
		}

		public void LogPurchaseVirtual_Apartment(int apartmentId, CurrencyItemType currencyItemType, int price, string windowId, PurchaseStatus purchaseStatus)
		{
			PurchaseChannel purchaseChannel = (FlagsStorage.IsMainMenu ? PurchaseChannel.GarageStore : PurchaseChannel.Other);
			LogPurchaseVirtual(purchaseStatus, currencyItemType, PurchaseType.Apartment, "", purchaseChannel, SellerType.System, "", "", "", price, windowId, apartmentId.ToString());
		}

		private void LogPurchaseVirtual(PurchaseStatus status, CurrencyItemType currencyType, PurchaseType purchaseType, string subType, PurchaseChannel purchaseChannel, SellerType sellerType, string sellerUserId, string listingId, string dealId, int price, string windowId, string itemId)
		{
			base.AnalyticEventFactory.CreateEvent<PurchaseVirtualEvent, PurchaseVirtualContext>(new PurchaseVirtualContext
			{
				Status = status,
				CurrencyType = CurrencyConverter.Convert(currencyType),
				PurchaseType = purchaseType,
				SubType = subType,
				PurchaseChannel = purchaseChannel,
				SellerType = sellerType.ToString(),
				SellerUserId = sellerUserId,
				ListingId = listingId,
				DealId = dealId,
				Price = price,
				WindowId = windowId,
				ItemId = itemId
			}).Send();
		}

		private PurchaseType ConvertPersonWindowCategory(int type)
		{
			switch (type)
			{
			case -1:
				return PurchaseType.Animation;
			case 0:
			case 1:
			case 2:
				return PurchaseType.Skin;
			default:
				return PurchaseType.Cloth;
			}
		}

		private string ConvertPurchasePersonSubTypes(int personSubType)
		{
			return _purchasePersonSubTypes[personSubType];
		}
	}
}
