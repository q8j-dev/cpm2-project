using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class CarTradeMoreInfoAnalytic : AnalyticProviderBase, IInRoomCallbacks, IDisposable
	{
		private readonly ITradeService _tradeService;

		private readonly Dictionary<string, int> _openAmount = new Dictionary<string, int>();

		private readonly IPlayerStorageService _playerStorageService;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly OnlineCollectionsProvider _onlineCollectionsProvider;

		public CarTradeMoreInfoAnalytic(ITradeService tradeService, IPlayerStorageService playerStorageService, FreeDriveProvider freeDriveProvider, AnalyticEventFactory analyticEventFactory, OnlineCollectionsProvider onlineCollectionsProvider)
			: base(analyticEventFactory)
		{
			_tradeService = tradeService;
			_playerStorageService = playerStorageService;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public void LogPreviewOpened(string tradeOfferId, bool logFromOfferCreator)
		{
			TradeOffer offer = _tradeService.GetOffer(tradeOfferId);
			if (offer != null && offer.OfferItem is TradeItemCar tradeItemCar)
			{
				int valueOrDefault = _openAmount.GetValueOrDefault(tradeItemCar.CarInstanceId, 0);
				valueOrDefault++;
				_openAmount[tradeItemCar.CarInstanceId] = valueOrDefault;
				CreateEvent(tradeOfferId, logFromOfferCreator, CarTradeMoreInfoStatusType.PreviewOpened);
			}
		}

		public void LogMoreInfoClicked(string tradeOfferId, bool logFromOfferCreator)
		{
			CreateEvent(tradeOfferId, logFromOfferCreator, CarTradeMoreInfoStatusType.MoreInfoClicked);
		}

		public void LogMoreInfoCanceled(string tradeOfferId, bool logFromOfferCreator)
		{
			CreateEvent(tradeOfferId, logFromOfferCreator, CarTradeMoreInfoStatusType.CanceledByUser);
		}

		public void LogPurchased(string tradeOfferId, bool logFromOfferCreator)
		{
			CreateEvent(tradeOfferId, logFromOfferCreator, CarTradeMoreInfoStatusType.Purchased);
		}

		public void LogInsufficientHard(string tradeOfferId, bool logFromOfferCreator)
		{
			CreateEvent(tradeOfferId, logFromOfferCreator, CarTradeMoreInfoStatusType.InsufficientHard);
		}

		private void CreateEvent(string tradeOfferId, bool logFromOfferCreator, CarTradeMoreInfoStatusType statusType)
		{
			TradeOffer offer = _tradeService.GetOffer(tradeOfferId);
			if (offer == null)
			{
				return;
			}
			TradeItemCar tradeItemCar = (logFromOfferCreator ? (offer.RequestItem as TradeItemCar) : (offer.OfferItem as TradeItemCar));
			if (tradeItemCar != null)
			{
				int valueOrDefault = _openAmount.GetValueOrDefault(tradeItemCar.CarInstanceId, 0);
				string ownerUserLocalId = "NotFound";
				MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(tradeItemCar.CreatorActorNr, checkVDriverPerson: true);
				if ((bool)multiPersonControl)
				{
					ownerUserLocalId = multiPersonControl.ThisPlayerID;
				}
				int priceAmount = BuyThisCarWindow.CarDetailsPrice;
				CurrencyType priceCurrency = CurrencyType.HARD;
				if (statusType == CarTradeMoreInfoStatusType.PreviewOpened)
				{
					priceAmount = 0;
					priceCurrency = CurrencyType.FREE;
				}
				CarTradeMoreInfoContext context = new CarTradeMoreInfoContext
				{
					DealId = offer.TradeID,
					Status = statusType,
					ViewerUserLocalId = _playerStorageService.LocalID,
					OwnerUserLocalId = ownerUserLocalId,
					TargetCarIdCarInstanceId = tradeItemCar.CarInstanceId,
					PriceAmount = priceAmount,
					PriceCurrency = priceCurrency,
					IsRepeatViewPurchase = (valueOrDefault > 1)
				};
				base.AnalyticEventFactory.CreateEvent<CarTradeMoreInfoEvent, CarTradeMoreInfoContext>(context).Send();
			}
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			GameObject carByOwnerID = _onlineCollectionsProvider.GetCarByOwnerID(otherPlayer.ActorNumber);
			if ((bool)carByOwnerID && carByOwnerID.TryGetComponent<VehicleData>(out var component) && component.CarInfoProvider.IsReady)
			{
				_openAmount.Remove(component.CarInfoProvider.CarInfo.CarInstanceId);
			}
			RemoveOldCarInstanceIDs();
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
		}

		private void RemoveOldCarInstanceIDs()
		{
			List<string> list = new List<string>();
			foreach (string key in _openAmount.Keys)
			{
				MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
				foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
				{
					if ((bool)multiPlayerPosControl && multiPlayerPosControl.VehicleData.CarInfoProvider.IsReady && multiPlayerPosControl.VehicleData.CarInfoProvider.CarInfo.CarInstanceId == key)
					{
						list.Add(key);
						break;
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				_openAmount.Remove(list[j]);
			}
		}
	}
}
