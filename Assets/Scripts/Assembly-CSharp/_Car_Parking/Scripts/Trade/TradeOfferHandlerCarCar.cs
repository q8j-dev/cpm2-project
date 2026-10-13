using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Database;
using CPM.Rating.Entities;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	public class TradeOfferHandlerCarCar : ITradeOfferHandler, IDisposable
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly ManagerUI _managerUI;

		private readonly SellExchangeFreeHelperAddCar _addCarHelper;

		private readonly SellExchangeFreeHelperDeleteCar _deleteCarHelper;

		private readonly RpcRrService _rpcRrService;

		private readonly Dictionary<string, string> _tuningDatas = new Dictionary<string, string>();

		private readonly PreTradeCarService _preTradeCarService;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly IProgressBackend _progressBackend;

		private readonly ITradeTransactionBackend _tradeTransactionBackend;

		private readonly ISteamAchievementService _steamAchievementService;

		private CancellationTokenSource _mainCTS;

		private CancellationToken _mainCT;

		public TradeItemType OfferItemType => TradeItemType.Car;

		public TradeItemType RequestItemType => TradeItemType.Car;

		public TradeOfferVisibleType OfferVisibleType => TradeOfferVisibleType.Private;

		public TradeOfferHandlerCarCar(FreeDriveProvider freeDriveProvider, ManagerUI managerUI, IAccountController accountController, CurrencyManagerController currencyManagerController, ISlotService slotService, IPoliceSystem policeSystem, ISteamAchievementService steamAchievementService, IDatabaseCarsService databaseCarsService, LockCarButtonController lockCarButtonController, RpcRrService rpcRrService, TradeAnalytic tradeAnalytic, IProgressBackend progressBackend, ITradeTransactionBackend tradeTransactionBackend)
		{
			_steamAchievementService = steamAchievementService;
			_freeDriveProvider = freeDriveProvider;
			_managerUI = managerUI;
			_rpcRrService = rpcRrService;
			_tradeAnalytic = tradeAnalytic;
			_progressBackend = progressBackend;
			_tradeTransactionBackend = tradeTransactionBackend;
			_addCarHelper = new SellExchangeFreeHelperAddCar(slotService, policeSystem);
			_deleteCarHelper = new SellExchangeFreeHelperDeleteCar(slotService, databaseCarsService, lockCarButtonController);
			_preTradeCarService = new PreTradeCarService(freeDriveProvider, accountController, managerUI, currencyManagerController);
			_mainCTS = new CancellationTokenSource();
			_mainCT = _mainCTS.Token;
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _mainCTS);
		}

		public async UniTask<PreTradeResultActionType> PreTradeAction(TradeOffer tradeOffer)
		{
			GetItems(tradeOffer, out var offerItem, out var requestItem);
			if (offerItem == null || requestItem == null)
			{
				return PreTradeResultActionType.Error;
			}
			if (PhotonNetwork.LocalPlayer.ActorNumber != requestItem.CreatorActorNr)
			{
				return PreTradeResultActionType.Error;
			}
			SellExchangeFreeMessage.ErrorTrade errorTrade = SellExchangeFreeHelperCarItemChecker.Check(offerItem, _freeDriveProvider.MultiPlayer);
			if (errorTrade != SellExchangeFreeMessage.ErrorTrade.None)
			{
				return SellExchangeFreeMessage.ShowMessageTradeErrorAndReturnError(errorTrade);
			}
			SellExchangeFreeMessage.ErrorTrade errorTrade2 = SellExchangeFreeHelperCarItemChecker.Check(requestItem, _freeDriveProvider.MultiPlayer);
			if (errorTrade2 != SellExchangeFreeMessage.ErrorTrade.None)
			{
				return SellExchangeFreeMessage.ShowMessageTradeErrorAndReturnError(errorTrade2);
			}
			string text = "";
			MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(tradeOffer.OfferActorNr);
			if ((bool)multiPersonControl)
			{
				text = text + "\"" + multiPersonControl.ThisPlayerName + "\"";
			}
			text += new LocalizedString("FreeDriveControl.WantsToExchangeCar");
			GameObject carByOwnerID = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(tradeOffer.OfferActorNr);
			if ((bool)carByOwnerID && carByOwnerID.TryGetComponent<Powertrain>(out var component))
			{
				component.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
				text += $"({maxHp}hp/{maxTq} Nm)";
			}
			PreTradeResultActionType preTradeResultActionType = await _preTradeCarService.PreTradeAction(tradeOffer, null, text, "", null, _mainCT);
			switch (preTradeResultActionType)
			{
			case PreTradeResultActionType.Accept:
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.BuyerConfirmed);
				_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.Accepted);
				break;
			case PreTradeResultActionType.CarNotFoundAfterAccept:
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.BuyerConfirmed);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.PlayerOffline);
				break;
			}
			return preTradeResultActionType;
		}

		public async UniTask<bool> TradeAction(TradeOffer tradeOffer)
		{
			GetItems(tradeOffer, out var offerItem, out var requestItem);
			if (offerItem == null || requestItem == null)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OfferError);
				return false;
			}
			if (!_freeDriveProvider.MultiPlayer.GetCarByOwnerID(offerItem.CreatorActorNr))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.CarNotFound);
				return false;
			}
			MultiPersonControl sellerMultiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(offerItem.CreatorActorNr);
			if (!sellerMultiPersonControl)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.PersonNotFound);
				return false;
			}
			if (!sellerMultiPersonControl.gameObject.TryGetPhotonView(out var sellerPersonPhotonView))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.PhotonViewNotFound);
				return false;
			}
			Result<bool> result = await _progressBackend.CheckOtherCarLegit(sellerMultiPersonControl.ThisPlayerAccountID, offerItem.CarInstanceId, offerItem.CarID, _mainCT);
			if (result.IsFail || !result.Value)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OtherCarNotLegit);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.ErrorFirebase);
				return false;
			}
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.OfferSent);
			VehicleData myVehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			TradeCreateRequest req = new TradeCreateRequest
			{
				Type = "EXCHANGE",
				CreatorCarUid = myVehicleData.carInfo.CarInstanceId,
				AcceptorUid = sellerMultiPersonControl.ThisPlayerAccountID,
				AcceptorCarUid = offerItem.CarInstanceId
			};
			Result<string> result2 = await _tradeTransactionBackend.CreateTransaction(req, _mainCT);
			if (_mainCT.IsCancellationRequested)
			{
				return false;
			}
			if (result2.IsFail || string.IsNullOrEmpty(result2.Value))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OfferError);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.ErrorFirebase);
				return false;
			}
			string stringTuningData = myVehicleData.carInfo.GetStringTuningData();
			RpcRrResponseExchange rpcRrResponseExchange = await _rpcRrService.Send<RpcRrResponseExchange>(sellerPersonPhotonView.Controller, RpcRrRequestObjectBase.CodeType.Exchange, RpcRrRequestExchange.GetRequestData(tradeOffer.TradeID, result2.Value, stringTuningData), 60f, _mainCT);
			if (_mainCT.IsCancellationRequested)
			{
				return false;
			}
			if (rpcRrResponseExchange.Code == RpcRrResponseObjectBase.CodeType.Success)
			{
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.Completed);
				_tuningDatas.Add(tradeOffer.TradeID, rpcRrResponseExchange.TuningData);
				FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
				int? car_exchange = 1;
				freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, car_exchange), forceSend: true).Forget();
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQIQ", "p2p_car_exchange");
			}
			else
			{
				if (rpcRrResponseExchange.Code == RpcRrResponseObjectBase.CodeType.Fail && rpcRrResponseExchange.Error == RpcRrResponseObjectBase.ErrorType.PlayerLeft)
				{
					_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.PlayerOffline);
				}
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.ResponseFailFromSeller);
			}
			return rpcRrResponseExchange.Code == RpcRrResponseObjectBase.CodeType.Success;
		}

		public async UniTask PostTradeAction(TradeOffer tradeOffer)
		{
			GetItems(tradeOffer, out var offerItem, out var requestItem);
			if (offerItem == null || requestItem == null)
			{
				return;
			}
			GameObject carByOwnerID = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(offerItem.CreatorActorNr);
			if ((bool)carByOwnerID)
			{
				_deleteCarHelper.DeleteCarAfterSellExchange(_freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>());
				await _addCarHelper.BuyMultiplayerCar(carByOwnerID, _tuningDatas.GetValueOrDefault(tradeOffer.TradeID));
				if (!_mainCT.IsCancellationRequested)
				{
					_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.ExchangeCompleted"), 5f).Forget();
				}
			}
		}

		private void GetItems(TradeOffer tradeOffer, out TradeItemCar offerItem, out TradeItemCar requestItem)
		{
			if (tradeOffer == null)
			{
				offerItem = null;
				requestItem = null;
			}
			else
			{
				offerItem = tradeOffer.OfferItem as TradeItemCar;
				requestItem = tradeOffer.RequestItem as TradeItemCar;
			}
		}
	}
}
