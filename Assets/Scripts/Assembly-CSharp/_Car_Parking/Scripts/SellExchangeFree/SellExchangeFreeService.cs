using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	[Preserve]
	public class SellExchangeFreeService : ISellExchangeFreeService, IDisposable
	{
		private readonly ManagerUI _managerUI;

		private readonly RpcRrService _rpcRrService;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly AllCarsInfo _allCarsInfo;

		private readonly ISlotService _slotService;

		private readonly IAccountController _accountController;

		private readonly MultiPlayer _multiPlayer;

		private readonly CarSellTriggersCollection _carSellTriggersCollection;

		private readonly ITradeService _tradeService;

		private readonly TradeServiceFactory _tradeServiceFactory;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly IP2PAntiSpam _p2pAntiSpam;

		private readonly CarSellTriggerExitHandler _carSellTriggerExitHandler;

		private GameObject _buyCarOnTrigger;

		private CancellationTokenSource _sellCarOnTriggerCts;

		private CancellationTokenSource _buyCarCarOnTriggerCts;

		private CancellationTokenSource _mainCts;

		public SellExchangeFreeService(ManagerUI managerUI, RpcRrService rpcRrService, FreeDriveProvider freeDriveProvider, AllCarsInfo allCarsInfo, ISlotService slotService, IAccountController accountController, MultiPlayer multiPlayer, CarSellTriggersCollection carSellTriggersCollection, ITradeService tradeService, TradeServiceFactory tradeServiceFactory, TradeAnalytic tradeAnalytic, IP2PAntiSpam p2PAntiSpam, CarSellTriggerExitHandler carSellTriggerExitHandler)
		{
			_managerUI = managerUI;
			_rpcRrService = rpcRrService;
			_freeDriveProvider = freeDriveProvider;
			_allCarsInfo = allCarsInfo;
			_slotService = slotService;
			_accountController = accountController;
			_multiPlayer = multiPlayer;
			_carSellTriggersCollection = carSellTriggersCollection;
			_tradeService = tradeService;
			_tradeServiceFactory = tradeServiceFactory;
			_tradeAnalytic = tradeAnalytic;
			_p2pAntiSpam = p2PAntiSpam;
			_carSellTriggerExitHandler = carSellTriggerExitHandler;
			_mainCts = new CancellationTokenSource();
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _sellCarOnTriggerCts);
			CommonExtensions.CancelAndDisposeToken(ref _buyCarCarOnTriggerCts);
			CommonExtensions.CancelAndDisposeToken(ref _mainCts);
		}

		public async UniTask TrySetCarOnSellTriggerAsync(SellCarTrigger sellCarTrigger, GameObject car)
		{
			SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultData canSellCarResultData = await SellExchangeFreeHelperCanSellOrExchange.CanSetCarToSellTrigger(sellCarTrigger, car, _freeDriveProvider, _accountController, _allCarsInfo, _slotService, _tradeService);
			if (canSellCarResultData.CanSellCarResult != SellExchangeFreeHelperCanSellOrExchange.CanSellCarResult.Success)
			{
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultToErrorCode(canSellCarResultData.CanSellCarResult), canSellCarResultData.Data);
			}
			else
			{
				if (!(await CheckMyOffers()))
				{
					return;
				}
				CommonExtensions.CancelAndDisposeToken(ref _sellCarOnTriggerCts);
				_sellCarOnTriggerCts = new CancellationTokenSource();
				CancellationToken cancellationToken = _sellCarOnTriggerCts.Token;
				WindowScript windowScript = await _managerUI.OpenSellCarWindow();
				if (!windowScript)
				{
					return;
				}
				if (cancellationToken.IsCancellationRequested)
				{
					windowScript.Close();
					return;
				}
				WindowScriptAsyncResult<SellCarWindow.CarPriceData>.ResultData selCarWindowResult = await ((SellCarWindow)windowScript).GetResultAsync(cancellationToken);
				if (selCarWindowResult.CodeType == WindowScriptAsyncResult<SellCarWindow.CarPriceData>.CodeType.Success)
				{
					VehicleData component = car.GetComponent<VehicleData>();
					PhotonView photonView = car.GetPhotonView();
					TradeItemCar tradeItemCar = await _tradeServiceFactory.CreateItemCarAsync(photonView.CreatorActorNr, component.carInfo.TuningData.CalcShortTuningStates(), cancellationToken);
					TradeItemMoney tradeItemMoney = new TradeItemMoney(new AdvancedInt(selCarWindowResult.Payload.CarPrice));
					TradeOffer tradeOffer = _tradeServiceFactory.CreateOfferOnTrigger(tradeItemCar, tradeItemMoney, PhotonNetwork.LocalPlayer.ActorNumber);
					if (_tradeService.AddOffer(tradeOffer))
					{
						RpcCarEnteredCarSellTrigger.Send(_multiPlayer, _freeDriveProvider.FirstCarMultiPlayer, selCarWindowResult.Payload.CarModel, selCarWindowResult.Payload.CarPrice, sellCarTrigger.Index, tradeOffer.TradeID);
						SellCarPlaced(selCarWindowResult.Payload.CarModel, _freeDriveProvider.FirstCarMultiPlayer, sellCarTrigger.Index, tradeOffer.TradeID);
					}
				}
			}
		}

		public async UniTask TrySellCarToOtherPlayerAsync(int price, GameObject otherPerson)
		{
			if (price <= 0 || price > (int)SellCarWindow.MaxPrice)
			{
				ManagerUIHelper.SetMessage(((string)new LocalizedString("SellCarWindow.PriceShouldBeCorrect")).Replace("{MaxPrice}", SellCarWindow.MaxPrice.ToString()), 3f);
				return;
			}
			SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultData canSellCarResultData = await SellExchangeFreeHelperCanSellOrExchange.CanSellCarInProfile(_freeDriveProvider.FirstCarMultiPlayer, _freeDriveProvider, _accountController, _allCarsInfo, _slotService, price, otherPerson, _tradeService);
			if (canSellCarResultData.CanSellCarResult != SellExchangeFreeHelperCanSellOrExchange.CanSellCarResult.Success)
			{
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultToErrorCode(canSellCarResultData.CanSellCarResult), canSellCarResultData.Data);
			}
			else
			{
				if (!(await CheckAntiSpam()) || !(await CheckMyOffers()))
				{
					return;
				}
				ShowNotificationWindow(new LocalizedString("PlayerProfile.Sending.."), 4f);
				PhotonView otherPersonPhotonView = otherPerson.GetPhotonView();
				VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
				PhotonView photonView = _freeDriveProvider.FirstCarMultiPlayer.GetPhotonView();
				TradeItemCar tradeItemCar = await _tradeServiceFactory.CreateItemCarAsync(photonView.CreatorActorNr, component.carInfo.TuningData.CalcShortTuningStates());
				TradeItemMoney tradeItemMoney = new TradeItemMoney(new AdvancedInt(price));
				TradeOffer tradeOffer = _tradeServiceFactory.CreateOfferP2PSell(tradeItemCar, tradeItemMoney, PhotonNetwork.LocalPlayer.ActorNumber, otherPersonPhotonView.CreatorActorNr);
				if (!_tradeService.AddOffer(tradeOffer))
				{
					return;
				}
				GlobalEventSystem.OnNewOffer?.Invoke(tradeOffer);
				_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.RequestSent);
				RpcRrResponseSendBuyCarOffer rpcRrResponseSendBuyCarOffer = await _rpcRrService.Send<RpcRrResponseSendBuyCarOffer>(otherPersonPhotonView.Controller, RpcRrRequestObjectBase.CodeType.SendBuyCarOffer, RpcRrRequestSendBuyCarOffer.GetRequestData(_freeDriveProvider.FirstCarMultiPlayer, price, tradeOffer.TradeID), 20f);
				if (rpcRrResponseSendBuyCarOffer.Code == RpcRrResponseObjectBase.CodeType.Success)
				{
					if (rpcRrResponseSendBuyCarOffer.StatusCode == RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.SuccessAndBlocked)
					{
						_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.BlockedByIgnore);
					}
					else if (rpcRrResponseSendBuyCarOffer.StatusCode != RpcRrResponseSendBuyCarOffer.SendBuyCarOfferStatusCode.Success)
					{
						SellExchangeFreeMessage.ShowErrorMessage(RpcRrResponseSendBuyCarOffer.StatusCodeToErrorCode(rpcRrResponseSendBuyCarOffer.StatusCode), "");
						_tradeService.CancelOffer(tradeOffer.TradeID, TradeOfferCancelReason.None, notifyOtherPlayer: false, isCanceled: false);
						return;
					}
					_p2pAntiSpam.NewRequestAdded();
					ShowNotificationWindow(new LocalizedString("PlayerProfile.CarSelloffersent"), 4f);
				}
			}
		}

		public async UniTask TryExchangeCarToOtherPlayerAsync(GameObject otherPerson)
		{
			CancellationToken cancellationToken = _mainCts.Token;
			SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultData canSellCarResultData = await SellExchangeFreeHelperCanSellOrExchange.CanExchangeCar(_freeDriveProvider.FirstCarMultiPlayer, _slotService, _allCarsInfo, _accountController, _freeDriveProvider, otherPerson, _tradeService);
			if (canSellCarResultData.CanSellCarResult != SellExchangeFreeHelperCanSellOrExchange.CanSellCarResult.Success)
			{
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanSellOrExchange.CanSellCarResultToErrorCode(canSellCarResultData.CanSellCarResult), canSellCarResultData.Data);
			}
			else
			{
				if (!(await CheckAntiSpam()) || !(await CheckMyOffers()))
				{
					return;
				}
				ShowNotificationWindow(new LocalizedString("PlayerProfile.Sending.."), 4f);
				PhotonView otherPersonPhotonView = otherPerson.GetPhotonView();
				RpcRrResponseShortCarTuningData responseShortCarTuning = await _rpcRrService.Send<RpcRrResponseShortCarTuningData>(otherPersonPhotonView.Controller, RpcRrRequestObjectBase.CodeType.CarTuningDataInt, RpcRrRequestShortCarTuningData.GetRequestData(), 30f, cancellationToken);
				if (cancellationToken.IsCancellationRequested || responseShortCarTuning.Code != RpcRrResponseObjectBase.CodeType.Success)
				{
					return;
				}
				VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
				PhotonView photonView = _freeDriveProvider.FirstCarMultiPlayer.GetPhotonView();
				TradeItemCar tradeItemMyCar = await _tradeServiceFactory.CreateItemCarAsync(photonView.CreatorActorNr, component.carInfo.TuningData.CalcShortTuningStates(), cancellationToken);
				TradeItemCar tradeItemCar = await _tradeServiceFactory.CreateItemCarAsync(otherPersonPhotonView.CreatorActorNr, 0, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				TradeOffer tradeOffer = _tradeServiceFactory.CreateOfferP2PExchange(tradeItemMyCar, tradeItemCar, PhotonNetwork.LocalPlayer.ActorNumber, otherPersonPhotonView.CreatorActorNr);
				if (!_tradeService.AddOffer(tradeOffer))
				{
					return;
				}
				tradeOffer.State = TradeOfferState.Pending;
				VehicleData component2 = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(otherPersonPhotonView.CreatorActorNr).GetComponent<VehicleData>();
				int otherCarID = component2.CarId;
				int otherEngineType = component2.GetVehicleEntityMono.CarEntity.Engine.GetEngineType;
				WindowScript windowScript = await _managerUI.ShowWindow("BuyThisCarWindow");
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				BuyThisCarWindow obj = (BuyThisCarWindow)windowScript;
				obj.Init(new LocalizedString("CarChangeLogic.ExchangeToThisCar"), "", otherCarID, otherEngineType, 0, responseShortCarTuning.ShortTuningData, tradeOffer.TradeID, otherPersonPhotonView.Controller, logFromOfferCreator: true);
				WindowScriptAsyncResult<RequestAnswer>.ResultData resultData = await obj.GetResultAsync(cancellationToken);
				if (resultData.CodeType != WindowScriptAsyncResult<RequestAnswer>.CodeType.Success)
				{
					return;
				}
				if (resultData.Payload != RequestAnswer.Confirm)
				{
					_tradeService.CancelOrDeclineOffer(tradeOffer.TradeID, TradeOfferCancelReason.None, notifyOtherPlayer: false);
					return;
				}
				tradeOffer.Reset();
				GlobalEventSystem.OnNewOffer?.Invoke(tradeOffer);
				_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.RequestSent);
				RpcRrResponseSendExchangeCarOffer rpcRrResponseSendExchangeCarOffer = await _rpcRrService.Send<RpcRrResponseSendExchangeCarOffer>(otherPersonPhotonView.Controller, RpcRrRequestObjectBase.CodeType.SendExchangeCarOffer, RpcRrRequestSendExchangeCarOffer.GetRequestData(_freeDriveProvider.FirstCarMultiPlayer, tradeOffer.TradeID), 20f);
				if (rpcRrResponseSendExchangeCarOffer.Code != RpcRrResponseObjectBase.CodeType.Success)
				{
					_tradeService.CancelOffer(tradeOffer.TradeID, TradeOfferCancelReason.None, notifyOtherPlayer: false, isCanceled: false);
					return;
				}
				if (rpcRrResponseSendExchangeCarOffer.StatusCode == RpcRrResponseSendExchangeCarOffer.SendBuyCarOfferStatusCode.SuccessAndBlocked)
				{
					_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.BlockedByIgnore);
				}
				else if (rpcRrResponseSendExchangeCarOffer.StatusCode != RpcRrResponseSendExchangeCarOffer.SendBuyCarOfferStatusCode.Success)
				{
					SellExchangeFreeMessage.ShowErrorMessage(RpcRrResponseSendExchangeCarOffer.StatusCodeToErrorCode(rpcRrResponseSendExchangeCarOffer.StatusCode), "");
					_tradeService.CancelOffer(tradeOffer.TradeID, TradeOfferCancelReason.None, notifyOtherPlayer: false, isCanceled: false);
					return;
				}
				_p2pAntiSpam.NewRequestAdded();
				ShowNotificationWindow(new LocalizedString("PlayerProfile.CarSelloffersent"), 4f);
			}
		}

		public void SellCarPlaced(string carModel, GameObject car, int numberSellCarTrigger, string tradeOfferID)
		{
			SellCarTriggerData sellCarTriggerData = _carSellTriggersCollection.GetSellCarTriggerData(numberSellCarTrigger);
			if (sellCarTriggerData.IsOccupied)
			{
				if (car == _freeDriveProvider.FirstCarMultiPlayer)
				{
					ShowNotificationWindow(new LocalizedString("MenuLevelParent.Thisplacenotempty"), 5f);
				}
			}
			else
			{
				car.GetComponent<VehicleData>().СurrentSellCarTrigger = sellCarTriggerData;
				sellCarTriggerData.Init(carModel, tradeOfferID);
			}
		}

		public void SellCarExit(int place)
		{
			SellCarExit(_carSellTriggersCollection.GetSellCarTriggerData(place));
		}

		public void SellCarExit(SellCarTriggerData sellCarTriggerData)
		{
			if (sellCarTriggerData == null)
			{
				return;
			}
			TradeOffer offer = _tradeService.GetOffer(sellCarTriggerData.TradeOfferId);
			if (offer == null)
			{
				return;
			}
			GameObject carByOwnerID = _multiPlayer.GetCarByOwnerID(offer.OfferActorNr, needToCheckFirstCar: true);
			if (!carByOwnerID)
			{
				_carSellTriggerExitHandler.Clear(sellCarTriggerData.Index);
				sellCarTriggerData.Clear();
				return;
			}
			if (carByOwnerID == _freeDriveProvider.FirstCarMultiPlayer)
			{
				CommonExtensions.CancelAndDisposeToken(ref _sellCarOnTriggerCts);
			}
			if ((bool)_buyCarOnTrigger && _buyCarOnTrigger == carByOwnerID)
			{
				CommonExtensions.CancelAndDisposeToken(ref _buyCarCarOnTriggerCts);
			}
			VehicleData component = carByOwnerID.GetComponent<VehicleData>();
			if ((bool)component && component.СurrentSellCarTrigger != null && carByOwnerID == _freeDriveProvider.FirstCarMultiPlayer)
			{
				RpcCarExitCarSellTrigger.Send(_freeDriveProvider.MultiPlayer, sellCarTriggerData.Index);
			}
			if ((bool)component)
			{
				component.СurrentSellCarTrigger = null;
			}
			_tradeService.CancelOrDeclineOffer(sellCarTriggerData.TradeOfferId, TradeOfferCancelReason.None, notifyOtherPlayer: false);
			_carSellTriggerExitHandler.Clear(sellCarTriggerData.Index);
			sellCarTriggerData.Clear();
		}

		private async UniTask<bool> CheckAntiSpam()
		{
			if (_p2pAntiSpam.SecondsToNextRequest < 0)
			{
				return true;
			}
			string message = string.Format(new LocalizedString("Trade.WaitForNextOffer"), _p2pAntiSpam.SecondsToNextRequest);
			ShowNotificationWindow(message, 4f);
			return false;
		}

		private async UniTask<bool> CheckMyOffers()
		{
			List<TradeOffer> myActiveOffers = _tradeService.GetMyActiveOffers();
			bool flag = false;
			foreach (TradeOffer item in myActiveOffers)
			{
				if (item.OfferItem.ItemType == TradeItemType.Car && item.VisibleType == TradeOfferVisibleType.Private)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return true;
			}
			if ((await (await _managerUI.ShowConfirmWindow(null, new LocalizedString("Trade.HaveOfferTitle"), new LocalizedString("Trade.HaveOfferDescription"))).GetResultAsync()).CodeType == WindowScriptAsyncResult<EmptyData>.CodeType.Success)
			{
				foreach (TradeOffer item2 in myActiveOffers)
				{
					if (item2.OfferItem.ItemType == TradeItemType.Car && item2.VisibleType == TradeOfferVisibleType.Private)
					{
						_tradeService.CancelOrDeclineOffer(item2.TradeID, TradeOfferCancelReason.Manual);
					}
				}
				return true;
			}
			return false;
		}

		private void ShowNotificationWindow(string message, float duration)
		{
			if ((bool)_managerUI)
			{
				_managerUI.ShowNotificationWindow(message, duration).Forget();
			}
		}
	}
}
