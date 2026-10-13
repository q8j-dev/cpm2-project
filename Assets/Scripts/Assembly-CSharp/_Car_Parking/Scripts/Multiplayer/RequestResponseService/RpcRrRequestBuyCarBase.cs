using CPM.Database;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public abstract class RpcRrRequestBuyCarBase : RpcRrRequestObjectBase
	{
		protected const string Success = "1";

		protected const string Error = "0";

		protected const string ErrorEncrypt = "2";

		protected const string ErrorOffer = "3";

		protected const string ErrorInFirebase = "4";

		protected readonly FreeDriveProvider _freeDriveProvider;

		protected readonly CommonProvider _commonProvider;

		private readonly ITradeService _tradeService;

		private readonly ManagerUI _managerUI;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly SellExchangeFreeHelperDeleteCar _deleteCarHelper;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private readonly ITradeTransactionBackend _tradeTransactionBackend;

		private readonly ISteamAchievementService _steamAchievementService;

		public static string GetRequestData(string tradeOfferID, int priceCar, string transactionId)
		{
			return WalletCypher.Encode(string.Format("{0}{1}{2}{3}{4}", tradeOfferID, "?", priceCar, "?", transactionId), "1");
		}

		protected RpcRrRequestBuyCarBase(FreeDriveProvider freeDriveProvider, ITradeService tradeService, ISteamAchievementService steamAchievementService, ManagerUI managerUI, ISlotService slotService, CurrencyManagerController currencyManagerController, IDatabaseCarsService databaseCarsService, LockCarButtonController lockCarButtonController, CommonProvider commonProvider, TradeAnalytic tradeAnalytic, VirtualPurchaseAnalytic virtualPurchaseAnalytic, ITradeTransactionBackend tradeTransactionBackend)
		{
			_freeDriveProvider = freeDriveProvider;
			_tradeService = tradeService;
			_steamAchievementService = steamAchievementService;
			_managerUI = managerUI;
			_currencyManagerController = currencyManagerController;
			_commonProvider = commonProvider;
			_tradeAnalytic = tradeAnalytic;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_tradeTransactionBackend = tradeTransactionBackend;
			_deleteCarHelper = new SellExchangeFreeHelperDeleteCar(slotService, databaseCarsService, lockCarButtonController);
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			requestData = WalletCypher.DecodeWithOtherUID(requestData, sender.ActorNumber);
			string[] array = requestData.Split("?");
			if (array == null || array.Length != 3)
			{
				return "2";
			}
			string tradeOfferID = array[0];
			if (!int.TryParse(array[1], out var priceCar))
			{
				return "3";
			}
			string transactionId = array[2];
			if (priceCar <= 0)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				return "3";
			}
			TradeOffer tradeOffer = _tradeService.GetOffer(tradeOfferID);
			if (tradeOffer == null || !tradeOffer.CanComplete)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.InternalError);
				return "3";
			}
			TradeItemCar tradeItemCar = tradeOffer.OfferItem as TradeItemCar;
			TradeItemMoney tradeItemMoney = tradeOffer.RequestItem as TradeItemMoney;
			if (tradeItemCar == null || tradeItemMoney == null)
			{
				return "3";
			}
			if (SellExchangeFreeHelperCarItemChecker.Check(tradeItemCar, _freeDriveProvider.MultiPlayer) != SellExchangeFreeMessage.ErrorTrade.None)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOfferID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return "3";
			}
			tradeItemMoney.Price.Get(out int value);
			if (value != priceCar)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOfferID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return "3";
			}
			if (tradeItemCar.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return "3";
			}
			string text = ExtraCheckCanSellCar(tradeOffer, sender);
			if (!string.IsNullOrEmpty(text))
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.ValidatorError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOfferID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return text;
			}
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOfferID, P2pDealStatusType.OfferSent);
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOfferID, P2pDealStatusType.BuyerConfirmed);
			MultiPersonControl multiPersonControl = _freeDriveProvider.MultiPlayer.GetMultiPersonControl(sender.ActorNumber);
			VehicleData vehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			TradeConfirmRequest req = new TradeConfirmRequest
			{
				Type = "TRADE",
				TransactionId = transactionId,
				CreatorUid = multiPersonControl.ThisPlayerAccountID,
				AcceptorCarUid = vehicleData.carInfo.CarInstanceId,
				Money = priceCar
			};
			TradeOfferState oldTradeState = tradeOffer.State;
			tradeOffer.State = TradeOfferState.Processing;
			Result<bool> result = await _tradeTransactionBackend.ConfirmTransaction(req);
			tradeOffer.State = oldTradeState;
			if (result.IsFail || !result.Value)
			{
				_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.REVERSED, P2PSaleCreditFailReasonType.FirebaseError);
				_virtualPurchaseAnalytic.LogBuyCarInMultiplayer(tradeOfferID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return "4";
			}
			_tradeAnalytic.LogCarTradeRequestStatus(tradeOffer.TradeID, CarTradeRequestStatusType.Accepted);
			_tradeAnalytic.LogP2pDealStatusEvent(tradeOffer.TradeID, P2pDealStatusType.Completed);
			_tradeAnalytic.LogP2pSale(tradeOfferID, P2PStatus.COMMITED, P2PSaleCreditFailReasonType.Unknown);
			string stringTuningData = vehicleData.carInfo.GetStringTuningData();
			bool num = IsTunedAboveStock(vehicleData);
			int getPerformance = CarTuningFactory.GetDefaultCarTuningDataConfig(vehicleData.carInfo.CarID).GetPerformance;
			if (vehicleData.carInfo.TuningData.PerformancePoints != getPerformance)
			{
				_steamAchievementService?.Unlock("ACH_TUNED_RESALE");
			}
			string text2 = new LocalizedString("CarChangeLogic.CarSoldForCarprice");
			_managerUI.ShowNotificationWindow(text2.Replace("{0}", priceCar.ToString()), 5f).Forget();
			_currencyManagerController.AddMoney(new AdvancedInt(priceCar));
			_deleteCarHelper.DeleteCarAfterSellExchange(vehicleData);
			_commonProvider.TradeService.CompleteOffer(tradeOffer.TradeID);
			ExtraActionAfterSellCar(tradeOffer);
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			int? car_trade = 1;
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, car_trade), forceSend: true).Forget();
			if (num)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQLQ", "p2p_car_sale");
			}
			return "1?" + stringTuningData;
		}

		private static bool IsTunedAboveStock(VehicleData vehicleData)
		{
			VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(vehicleData.carInfo.CarID);
			if ((bool)defaultCarTuningDataConfig)
			{
				return vehicleData.carInfo.TuningData.PerformancePoints != defaultCarTuningDataConfig.GetPerformance;
			}
			return false;
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			string[] array = executedRequestData.Split("?"[0]);
			if (array == null || array.Length == 0)
			{
				return new RpcRrResponseBuyCar(RpcRrResponseObjectBase.CodeType.Fail, "");
			}
			if (array.Length == 2 && array[0] == "1")
			{
				return new RpcRrResponseBuyCar(RpcRrResponseObjectBase.CodeType.Success, array[1]);
			}
			return new RpcRrResponseBuyCar(RpcRrResponseObjectBase.CodeType.Fail, "");
		}

		protected abstract string ExtraCheckCanSellCar(TradeOffer tradeOffer, Player sender);

		protected abstract void ExtraActionAfterSellCar(TradeOffer tradeOffer);
	}
}
