using CPM.PlayerData;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class TradeAnalytic : AnalyticProviderBase
	{
		private readonly CommonProvider _commonProvider;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly PlayerDataStorage _playerDataStorage;

		public TradeAnalytic(AnalyticEventFactory analyticEventFactory, CommonProvider commonProvider, FreeDriveProvider freeDriveProvider, PlayerDataStorage playerDataStorage)
			: base(analyticEventFactory)
		{
			_commonProvider = commonProvider;
			_freeDriveProvider = freeDriveProvider;
			_playerDataStorage = playerDataStorage;
		}

		public void LogP2pSale(string tradeOfferId, P2PStatus status, P2PSaleCreditFailReasonType failReason)
		{
			TradeOffer offer = _commonProvider.TradeService.GetOffer(tradeOfferId);
			if (offer != null && offer.OfferItem is TradeItemCar tradeItemCar)
			{
				PurchaseChannel purchaseChannel = PurchaseChannel.P2PDirect;
				if (offer.VisibleType == TradeOfferVisibleType.Public)
				{
					purchaseChannel = PurchaseChannel.MultiplayerMarket;
				}
				string firebaseId = MultiPlayerAccountHelper.GetFirebaseId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string localId = MultiPlayerAccountHelper.GetLocalId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string firebaseId2 = MultiPlayerAccountHelper.GetFirebaseId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string localId2 = MultiPlayerAccountHelper.GetLocalId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				int value = 0;
				if (offer.RequestItem is TradeItemMoney tradeItemMoney)
				{
					tradeItemMoney.Price.Get(out value);
				}
				base.AnalyticEventFactory.CreateEvent<P2pSaleCreditEvent, P2pContext>(new P2pContext
				{
					Status = status,
					SellerFbId = firebaseId,
					SellerLocalId = localId,
					BuyerFbId = firebaseId2,
					BuyerLocalId = localId2,
					DealId = tradeOfferId,
					ItemId = tradeItemCar.CarInstanceId,
					CurrencyType = CurrencyType.SOFT,
					PurchaseChannel = purchaseChannel,
					AmountMinor = value,
					FailReason = failReason
				}).Send();
			}
		}

		public void LogP2pDealStatusEvent(string tradeOfferId, P2pDealStatusType statusType)
		{
			TradeOffer offer = _commonProvider.TradeService.GetOffer(tradeOfferId);
			if (offer != null && offer.VisibleType != TradeOfferVisibleType.Public && offer.OfferItem is TradeItemCar tradeItemCar)
			{
				string firebaseId = MultiPlayerAccountHelper.GetFirebaseId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string localId = MultiPlayerAccountHelper.GetLocalId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string firebaseId2 = MultiPlayerAccountHelper.GetFirebaseId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				string localId2 = MultiPlayerAccountHelper.GetLocalId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
				int value = 0;
				if (offer.RequestItem is TradeItemMoney tradeItemMoney)
				{
					tradeItemMoney.Price.Get(out value);
				}
				string receiverCarID = "";
				if (offer.RequestItem is TradeItemCar tradeItemCar2)
				{
					receiverCarID = tradeItemCar2.CarInstanceId;
				}
				PurchaseChannel purchaseChannel = PurchaseChannel.P2PDirect;
				if (offer.VisibleType == TradeOfferVisibleType.Public)
				{
					purchaseChannel = PurchaseChannel.MultiplayerMarket;
				}
				P2PRequestActionType requestActionType = P2PRequestActionType.Sell;
				if (offer.RequestItem.ItemType == TradeItemType.Car)
				{
					requestActionType = P2PRequestActionType.Exchange;
				}
				P2pDealStatusContext context = new P2pDealStatusContext
				{
					DealID = tradeOfferId,
					StatusType = statusType,
					SellerFbId = firebaseId,
					SellerLocalId = localId,
					BuyerFbId = firebaseId2,
					BuyerLocalId = localId2,
					SenderCarID = tradeItemCar.CarInstanceId,
					ReceiverCarID = receiverCarID,
					Price = value,
					CurrencyType = CurrencyType.SOFT,
					PurchaseChannel = purchaseChannel,
					RequestActionType = requestActionType
				};
				base.AnalyticEventFactory.CreateEvent<P2pDealStatusEvent, P2pDealStatusContext>(context).Send();
			}
		}

		public void LogCarTradeRequestStatus(string tradeOfferId, CarTradeRequestStatusType statusType)
		{
			TradeOffer offer = _commonProvider.TradeService.GetOffer(tradeOfferId);
			if (offer == null || offer.VisibleType == TradeOfferVisibleType.Public || !(offer.OfferItem is TradeItemCar tradeItemCar))
			{
				return;
			}
			string firebaseId = MultiPlayerAccountHelper.GetFirebaseId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
			string localId = MultiPlayerAccountHelper.GetLocalId(offer.OfferActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
			string firebaseId2 = MultiPlayerAccountHelper.GetFirebaseId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
			string localId2 = MultiPlayerAccountHelper.GetLocalId(offer.RequestActorNr, _freeDriveProvider.MultiPlayer, _playerDataStorage);
			P2PDeliveryMode deliveryMode = P2PDeliveryMode.Normal;
			if (PhotonNetwork.LocalPlayer.ActorNumber == offer.OfferActorNr)
			{
				if (_commonProvider.ChatExtraDataController != null && _commonProvider.ChatExtraDataController.IsBlocked(offer.RequestActorNr))
				{
					deliveryMode = P2PDeliveryMode.Blocked;
				}
				else if (_commonProvider.ChatExtraDataController != null && _commonProvider.ChatExtraDataController.IsMuted(offer.RequestActorNr))
				{
					deliveryMode = P2PDeliveryMode.Muted;
				}
			}
			else
			{
				MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(offer.OfferActorNr, checkVDriverPerson: true);
				if ((bool)multiPersonControl)
				{
					if (multiPersonControl.IsBlocked)
					{
						deliveryMode = P2PDeliveryMode.Blocked;
					}
					else if (multiPersonControl.isMuted)
					{
						deliveryMode = P2PDeliveryMode.Muted;
					}
				}
			}
			string receiverCarInstanceId = "";
			if (offer.RequestItem is TradeItemCar tradeItemCar2)
			{
				receiverCarInstanceId = tradeItemCar2.CarInstanceId;
			}
			P2PRequestActionType requestAction = P2PRequestActionType.Sell;
			if (offer.RequestItem.ItemType == TradeItemType.Car)
			{
				requestAction = P2PRequestActionType.Exchange;
			}
			int value = 0;
			if (offer.RequestItem is TradeItemMoney tradeItemMoney)
			{
				tradeItemMoney.Price.Get(out value);
			}
			CarTradeRequestStatusContext context = new CarTradeRequestStatusContext
			{
				DealId = tradeOfferId,
				Status = statusType,
				TTlSec = Mathf.RoundToInt(30f),
				SenderFbId = firebaseId,
				SenderLocalId = localId,
				ReceiverFbId = firebaseId2,
				ReceiverLocalId = localId2,
				SenderCarInstanceId = tradeItemCar.CarInstanceId,
				ReceiverCarInstanceId = receiverCarInstanceId,
				DeliveryMode = deliveryMode,
				RequestAction = requestAction,
				Price = value,
				CurrencyType = CurrencyType.SOFT
			};
			base.AnalyticEventFactory.CreateEvent<CarTradeRequestStatusEvent, CarTradeRequestStatusContext>(context).Send();
		}
	}
}
