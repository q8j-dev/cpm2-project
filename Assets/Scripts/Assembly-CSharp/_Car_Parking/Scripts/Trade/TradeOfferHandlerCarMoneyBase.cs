using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
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
	public abstract class TradeOfferHandlerCarMoneyBase : ITradeOfferHandler, IDisposable
	{
		protected readonly FreeDriveProvider _freeDriveProvider;

		private readonly ManagerUI _managerUI;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly ISlotService _slotService;

		private readonly RpcRrService _rpcRrService;

		private readonly SellExchangeFreeHelperAddCar _addCarHelper;

		private readonly PreTradeCarService _preTradeCarService;

		private readonly Dictionary<string, string> _tuningDatas = new Dictionary<string, string>();

		private readonly VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly IProgressBackend _progressBackend;

		private readonly ITradeTransactionBackend _tradeTransactionBackend;

		private readonly ISteamAchievementService _steamAchievementService;

		private CancellationTokenSource _mainCTS;

		private CancellationToken _mainCT;

		public TradeItemType OfferItemType => TradeItemType.Car;

		public TradeItemType RequestItemType => TradeItemType.Money;

		public abstract TradeOfferVisibleType OfferVisibleType { get; }

		protected abstract RpcRrRequestObjectBase.CodeType RpcRrRequestCodeType { get; }

		public TradeOfferHandlerCarMoneyBase(ManagerUI managerUI, IAccountController accountController, FreeDriveProvider freeDriveProvider, CurrencyManagerController currencyManagerController, ISlotService slotService, RpcRrService rpcRrService, IPoliceSystem policeSystem, ISteamAchievementService steamAchievementService, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IProgressBackend progressBackend, TradeAnalytic tradeAnalytic, ITradeTransactionBackend tradeTransactionBackend)
		{
			_steamAchievementService = steamAchievementService;
			_managerUI = managerUI;
			_freeDriveProvider = freeDriveProvider;
			_currencyManagerController = currencyManagerController;
			_slotService = slotService;
			_rpcRrService = rpcRrService;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_progressBackend = progressBackend;
			_tradeAnalytic = tradeAnalytic;
			_tradeTransactionBackend = tradeTransactionBackend;
			_addCarHelper = new SellExchangeFreeHelperAddCar(slotService, policeSystem);
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
			SellExchangeFreeMessage.ErrorTrade errorTrade = SellExchangeFreeHelperCarItemChecker.Check(offerItem, _freeDriveProvider.MultiPlayer);
			if (errorTrade != SellExchangeFreeMessage.ErrorTrade.None)
			{
				return SellExchangeFreeMessage.ShowMessageTradeErrorAndReturnError(errorTrade);
			}
			_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CLICKED);
			requestItem.Price.Get(out int value);
			PreTradeResultActionType preTradeResultActionType = await _preTradeCarService.PreTradeAction(tradeOffer, value, new LocalizedString("BuyThisCarWindow.PartsCondition"), GetCarTextTitle(tradeOffer), _virtualPurchaseAnalytic, _mainCT);
			if (_mainCT.IsCancellationRequested)
			{
				return PreTradeResultActionType.Error;
			}
			switch (preTradeResultActionType)
			{
			case PreTradeResultActionType.NotEnoughMoney:
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, "BuyThisCarWindow", PurchaseStatus.INSUFFICIENT_FUNDS);
				goto default;
			case PreTradeResultActionType.Reject:
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, "BuyThisCarWindow", PurchaseStatus.PURCHASE_CANCELED_BY_USER);
				goto default;
			case PreTradeResultActionType.CarNotFoundAfterAccept:
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.BuyerConfirmed);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.PlayerOffline);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, "BuyThisCarWindow", PurchaseStatus.PURCHASE_CANCELED_BY_USER);
				goto default;
			default:
				return preTradeResultActionType;
			case PreTradeResultActionType.Accept:
				_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.Accepted);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.BuyerConfirmed);
				if (!(await HaveSlotProcess()) || _mainCT.IsCancellationRequested)
				{
					_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
					return PreTradeResultActionType.Error;
				}
				return PreTradeResultActionType.Accept;
			}
		}

		public async UniTask<bool> TradeAction(TradeOffer tradeOffer)
		{
			GetItems(tradeOffer, out var offerItem, out var requestItem);
			if (offerItem == null || requestItem == null)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OfferError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return false;
			}
			if (!_freeDriveProvider.MultiPlayer.GetCarByOwnerID(offerItem.CreatorActorNr))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.CarNotFound);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return false;
			}
			MultiPersonControl sellerMultiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(offerItem.CreatorActorNr);
			if (!sellerMultiPersonControl)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.PersonNotFound);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return false;
			}
			if (!sellerMultiPersonControl.gameObject.TryGetPhotonView(out var sellerPersonPhotonView))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.PhotonViewNotFound);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return false;
			}
			Result<bool> result = await _progressBackend.CheckOtherCarLegit(sellerMultiPersonControl.ThisPlayerAccountID, offerItem.CarInstanceId, offerItem.CarID, _mainCT);
			if (result.IsFail || !result.Value)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OtherCarNotLegit);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.ErrorFirebase);
				return false;
			}
			requestItem.Price.Get(out int priceCar);
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.OfferSent);
			TradeCreateRequest req = new TradeCreateRequest
			{
				Type = "TRADE",
				AcceptorUid = sellerMultiPersonControl.ThisPlayerAccountID,
				AcceptorCarUid = offerItem.CarInstanceId,
				Money = priceCar
			};
			Result<string> result2 = await _tradeTransactionBackend.CreateTransaction(req, _mainCT);
			if (_mainCT.IsCancellationRequested)
			{
				return false;
			}
			if (result2.IsFail || string.IsNullOrEmpty(result2.Value))
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OfferError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.ErrorFirebase);
				return false;
			}
			RpcRrResponseBuyCar rpcRrResponseBuyCar = await _rpcRrService.Send<RpcRrResponseBuyCar>(sellerPersonPhotonView.Controller, RpcRrRequestCodeType, RpcRrRequestBuyCarBase.GetRequestData(tradeOffer.TradeID, priceCar, result2.Value), 60f, _mainCT);
			if (_mainCT.IsCancellationRequested)
			{
				return false;
			}
			if (rpcRrResponseBuyCar.Code == RpcRrResponseObjectBase.CodeType.Success)
			{
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_COMPLETED);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.Completed);
				_tuningDatas.Add(tradeOffer.TradeID, rpcRrResponseBuyCar.TuningData);
			}
			else
			{
				if (rpcRrResponseBuyCar.Code == RpcRrResponseObjectBase.CodeType.Fail && rpcRrResponseBuyCar.Error == RpcRrResponseObjectBase.ErrorType.PlayerLeft)
				{
					_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.PlayerOffline);
				}
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.ResponseFailFromSeller);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
			}
			return rpcRrResponseBuyCar.Code == RpcRrResponseObjectBase.CodeType.Success;
		}

		public async UniTask PostTradeAction(TradeOffer tradeOffer)
		{
			GetItems(tradeOffer, out var offerItem, out var requestItem);
			if (offerItem == null || requestItem == null)
			{
				return;
			}
			GameObject carByOwnerID = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(offerItem.CreatorActorNr);
			if (!carByOwnerID)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.CarNotFoundPostTrade);
				return;
			}
			_currencyManagerController.TryMoneySpend(out var _, requestItem.Price);
			await _addCarHelper.BuyMultiplayerCar(carByOwnerID, _tuningDatas.GetValueOrDefault(tradeOffer.TradeID));
			if (_mainCT.IsCancellationRequested)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.BuyMultiplayerCarCancel);
				return;
			}
			requestItem.Price.Get(out int value);
			string text = string.Concat(new LocalizedString("FreeDriveControl.BuyCompleted"), $"(-{value})");
			_managerUI.ShowNotificationWindow(text, 5f).Forget();
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQIA", "p2p_car_purchase");
			GooglePlayAchievements.MarkP2pCarBought(offerItem.CarInstanceId);
			_steamAchievementService?.Unlock("ACH_STREET_DEAL");
			_steamAchievementService?.MarkP2pCarBought(offerItem.CarID);
		}

		protected abstract string GetCarTextTitle(TradeOffer tradeOffer);

		private async UniTask<bool> HaveSlotProcess()
		{
			if (_slotService.TryGetEmptySlot(out var _))
			{
				return true;
			}
			await UniTask.Yield(PlayerLoopTiming.Update, _mainCT).SuppressCancellationThrow();
			if (_mainCT.IsCancellationRequested)
			{
				return false;
			}
			int slotId;
			if ((await (await _managerUI.ShowConfirmWindow(null, new LocalizedString("ExchangeCarService.BuySlot"), new LocalizedString("ExchangeCarService.YouDonTHaveEmptySlotsDoYouWant"))).GetResultAsync()).CodeType != WindowScriptAsyncResult<EmptyData>.CodeType.Success)
			{
				List<int> allSlots = _slotService.GetAllSlots();
				slotId = allSlots[allSlots.Count - 1] + 2;
				_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
				return false;
			}
			List<int> allSlots2 = _slotService.GetAllSlots();
			slotId = allSlots2[allSlots2.Count - 1] + 2;
			_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CONFIRMED);
			bool num = await _slotService.TryBuySlot(-1, checkAllGarages: false, LogNegativeResultBuySlot);
			if (!num && (bool)_managerUI)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("ExchangeCarService.CanTBuySlotCarPurchaseCanceled"), 3f).Forget();
			}
			return num;
			void LogNegativeResultBuySlot(int purchaseResultCode)
			{
				switch (purchaseResultCode)
				{
				case 1:
					_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_COMPLETED);
					break;
				case -3:
				case 0:
				case 2:
				case 4:
					_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
					break;
				case -2:
				case 3:
					_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.INSUFFICIENT_FUNDS);
					break;
				default:
					_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.NONE);
					break;
				}
			}
		}

		private void GetItems(TradeOffer tradeOffer, out TradeItemCar offerItem, out TradeItemMoney requestItem)
		{
			if (tradeOffer == null)
			{
				offerItem = null;
				requestItem = null;
			}
			else
			{
				offerItem = tradeOffer.OfferItem as TradeItemCar;
				requestItem = tradeOffer.RequestItem as TradeItemMoney;
			}
		}
	}
}
