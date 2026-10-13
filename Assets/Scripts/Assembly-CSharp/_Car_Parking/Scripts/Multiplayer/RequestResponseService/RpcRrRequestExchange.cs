using CPM.Database;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
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
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrRequestExchange : RpcRrRequestObjectBase
	{
		protected const string Success = "1";

		protected const string Error = "0";

		protected const string ErrorEncrypt = "2";

		protected const string ErrorOffer = "3";

		protected const string ErrorInFirebase = "4";

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly ITradeService _tradeService;

		private readonly ManagerUI _managerUI;

		private readonly SellExchangeFreeHelperAddCar _addCarHelper;

		private readonly SellExchangeFreeHelperDeleteCar _deleteCarHelper;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private readonly IProgressBackend _progressBackend;

		private readonly ITradeTransactionBackend _tradeTransactionBackend;

		private ISteamAchievementService _steamAchievementService;

		public override CodeType Code => CodeType.Exchange;

		public static string GetRequestData(string tradeOfferID, string transactionId, string tuningData)
		{
			return WalletCypher.Encode(tradeOfferID + "?" + transactionId + "?" + tuningData, "1");
		}

		public RpcRrRequestExchange(FreeDriveProvider freeDriveProvider, ITradeService tradeService, ManagerUI managerUI, ISlotService slotService, IPoliceSystem policeSystem, IDatabaseCarsService databaseCarsService, LockCarButtonController lockCarButtonController, TradeAnalytic tradeAnalytic, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IProgressBackend progressBackend, ISteamAchievementService steamAchievementService, ITradeTransactionBackend tradeTransactionBackend)
		{
			_freeDriveProvider = freeDriveProvider;
			_tradeService = tradeService;
			_managerUI = managerUI;
			_tradeAnalytic = tradeAnalytic;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_progressBackend = progressBackend;
			_tradeTransactionBackend = tradeTransactionBackend;
			_addCarHelper = new SellExchangeFreeHelperAddCar(slotService, policeSystem);
			_deleteCarHelper = new SellExchangeFreeHelperDeleteCar(slotService, databaseCarsService, lockCarButtonController);
			_steamAchievementService = steamAchievementService;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			requestData = WalletCypher.DecodeWithOtherUID(requestData, sender.ActorNumber);
			string[] array = requestData.Split("?"[0]);
			if (array == null || array.Length != 3)
			{
				return "2";
			}
			string tradeOfferID = array[0];
			string transactionId = array[1];
			string tuningData = array[2];
			TradeOffer tradeOffer = _tradeService.GetOffer(tradeOfferID);
			if (tradeOffer == null || !tradeOffer.CanComplete)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.InternalError);
				return "3";
			}
			TradeItemCar tradeItemCar = tradeOffer.OfferItem as TradeItemCar;
			TradeItemCar tradeItemOtherCar = tradeOffer.RequestItem as TradeItemCar;
			if (tradeItemCar == null || tradeItemOtherCar == null)
			{
				return "3";
			}
			SellExchangeFreeMessage.ErrorTrade num = SellExchangeFreeHelperCarItemChecker.Check(tradeItemCar, _freeDriveProvider.MultiPlayer);
			SellExchangeFreeMessage.ErrorTrade errorTrade = SellExchangeFreeHelperCarItemChecker.Check(tradeItemOtherCar, _freeDriveProvider.MultiPlayer);
			if (num != SellExchangeFreeMessage.ErrorTrade.None || errorTrade != SellExchangeFreeMessage.ErrorTrade.None)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			if (tradeItemCar.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber || tradeItemOtherCar.CreatorActorNr != sender.ActorNumber)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(tradeItemOtherCar.CreatorActorNr);
			if (!multiPersonControl)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			Result<bool> result = await _progressBackend.CheckOtherCarLegit(multiPersonControl.ThisPlayerAccountID, tradeItemOtherCar.CarInstanceId, tradeItemOtherCar.CarID);
			if (result.IsFail || !result.Value)
			{
				SellExchangeFreeMessage.ShowMessageTradeError(SellExchangeFreeMessage.ErrorTrade.OtherCarNotLegit);
				_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.ErrorFirebase);
				return "3";
			}
			GameObject otherCar = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(tradeItemOtherCar.CreatorActorNr);
			if (!otherCar)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			VehicleData otherVehicleData = otherCar.GetComponent<VehicleData>();
			await otherCar.GetComponent<MultiPlayerPosControl>().ApplyDataIfNeedAsync();
			if (!otherCar || !otherVehicleData)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOfferID, P2pDealStatusType.OfferSent);
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOfferID, P2pDealStatusType.BuyerConfirmed);
			MultiPersonControl multiPersonControl2 = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(sender.ActorNumber);
			VehicleData vehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			TradeConfirmRequest req = new TradeConfirmRequest
			{
				Type = "EXCHANGE",
				TransactionId = transactionId,
				CreatorUid = multiPersonControl2.ThisPlayerAccountID,
				CreatorCarUid = otherVehicleData.CarInfoProvider.CarInfo.CarInstanceId,
				AcceptorCarUid = vehicleData.carInfo.CarInstanceId
			};
			TradeOfferState oldTradeState = tradeOffer.State;
			tradeOffer.State = TradeOfferState.Processing;
			Result<bool> result2 = await _tradeTransactionBackend.ConfirmTransaction(req);
			tradeOffer.State = oldTradeState;
			if (result2.IsFail || !result2.Value)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.FirebaseError);
				return "4";
			}
			_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.Accepted);
			_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.COMMITED, P2PSaleCreditFailReasonType.Unknown);
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.Completed);
			string stringTuningData = vehicleData.carInfo.GetStringTuningData();
			_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.ExchangeCompleted"), 5f).Forget();
			_deleteCarHelper.DeleteCarAfterSellExchange(vehicleData);
			_addCarHelper.BuyMultiplayerCar(otherCar, tuningData).Forget();
			_tradeService.CompleteOffer(tradeOffer.TradeID);
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			int? car_exchange = 1;
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, car_exchange), forceSend: true).Forget();
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQIQ", "p2p_car_exchange");
			_steamAchievementService?.Unlock("ACH_EVEN_TRADE");
			return "1?" + stringTuningData;
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			string[] array = executedRequestData.Split("?"[0]);
			if (array == null || array.Length == 0)
			{
				return new RpcRrResponseExchange(RpcRrResponseObjectBase.CodeType.Fail, "");
			}
			if (array.Length == 2 && array[0] == "1")
			{
				return new RpcRrResponseExchange(RpcRrResponseObjectBase.CodeType.Success, array[1]);
			}
			return new RpcRrResponseExchange(RpcRrResponseObjectBase.CodeType.Fail, "");
		}
	}
}
