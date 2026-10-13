using System.Threading;
using CPM.Account;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;

namespace _Car_Parking.Scripts.Trade
{
	public class PreTradeCarService
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly IAccountController _accountController;

		private readonly ManagerUI _managerUI;

		private readonly CurrencyManagerController _currencyManagerController;

		public PreTradeCarService(FreeDriveProvider freeDriveProvider, IAccountController accountController, ManagerUI managerUI, CurrencyManagerController currencyManagerController)
		{
			_freeDriveProvider = freeDriveProvider;
			_accountController = accountController;
			_managerUI = managerUI;
			_currencyManagerController = currencyManagerController;
		}

		public async UniTask<PreTradeResultActionType> PreTradeAction(TradeOffer tradeOffer, int? price, string windowTitle, string carTextTitle, VirtualPurchaseAnalytic virtualPurchaseAnalytic, CancellationToken cancellationToken)
		{
			if (tradeOffer == null)
			{
				return PreTradeResultActionType.Error;
			}
			if (tradeOffer.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeMessage.ErrorCode.ItsYourCar, "");
				return PreTradeResultActionType.Error;
			}
			if (!(tradeOffer.OfferItem is TradeItemCar tradeItemCarOther))
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return PreTradeResultActionType.Error;
			}
			GameObject carOther = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(tradeItemCarOther.CreatorActorNr);
			if (!carOther)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return PreTradeResultActionType.Error;
			}
			if (!carOther.TryGetPhotonView(out var photonView))
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return PreTradeResultActionType.Error;
			}
			await carOther.GetComponent<MultiPlayerPosControl>().ApplyDataIfNeedAsync(cancellationToken);
			if (cancellationToken.IsCancellationRequested || !carOther)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return PreTradeResultActionType.Error;
			}
			SellExchangeFreeHelperCanBuy.CanBuyCarResult canBuyCarResult = SellExchangeFreeHelperCanBuy.CanBuyCar(carOther, _accountController, _freeDriveProvider, price ?? 100);
			if (canBuyCarResult != SellExchangeFreeHelperCanBuy.CanBuyCarResult.Success)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanBuy.CanBuyCarResultToErrorCode(canBuyCarResult), "");
				return PreTradeResultActionType.Error;
			}
			VehicleData component = carOther.GetComponent<VehicleData>();
			int carID = component.CarId;
			int engineType = component.GetVehicleEntityMono.CarEntity.Engine.GetEngineType;
			WindowScript windowScript = await _managerUI.ShowWindow("BuyThisCarWindow");
			if (cancellationToken.IsCancellationRequested)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				return PreTradeResultActionType.Error;
			}
			BuyThisCarWindow obj = (BuyThisCarWindow)windowScript;
			obj.Init(windowTitle, carTextTitle, carID, engineType, price.GetValueOrDefault(), tradeItemCarOther.ShortTuningData, tradeOffer.TradeID, photonView.Controller);
			WindowScriptAsyncResult<RequestAnswer>.ResultData resultData = await obj.GetResultAsync(cancellationToken);
			if (resultData.CodeType != WindowScriptAsyncResult<RequestAnswer>.CodeType.Success)
			{
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, "BuyThisCarWindow", PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeMessage.ErrorCode.PlayerIsBusy, "");
				return PreTradeResultActionType.Error;
			}
			if (resultData.Payload != RequestAnswer.Confirm)
			{
				return PreTradeResultActionType.Reject;
			}
			virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, "BuyThisCarWindow", PurchaseStatus.PURCHASE_CONFIRMED);
			SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResult canConfirmBuyCarResult = SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCar(_currencyManagerController, price.GetValueOrDefault(), carOther);
			switch (canConfirmBuyCarResult)
			{
			case SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResult.NotEnoughMoney:
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResultToErrorCode(canConfirmBuyCarResult), "");
				return PreTradeResultActionType.NotEnoughMoney;
			default:
				virtualPurchaseAnalytic?.LogBuyCarInMultiplayer(tradeOffer.TradeID, string.Empty, PurchaseStatus.PURCHASE_CANCELED_BY_GAME);
				SellExchangeFreeMessage.ShowErrorMessage(SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResultToErrorCode(canConfirmBuyCarResult), "");
				if (canConfirmBuyCarResult == SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResult.CarNotFound)
				{
					return PreTradeResultActionType.CarNotFoundAfterAccept;
				}
				return PreTradeResultActionType.Error;
			case SellExchangeFreeHelperCanConfirmBuy.CanConfirmBuyCarResult.Success:
				return PreTradeResultActionType.Accept;
			}
		}
	}
}
