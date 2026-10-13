using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public static class SellExchangeFreeMessage
	{
		public enum ErrorCode
		{
			IncompatibleParts = 2,
			CheatEngine = 3,
			YouSoldThisCar = 4,
			YouCantSellThisCar = 5,
			YouAlreadyAreSellingThisCar = 6,
			YouCantSellYourLastCar = 7,
			YouMustHaveAnAccount = 8,
			ItsYourCar = 9,
			YouMustHaveAnAccountToBuy = 10,
			BlockSellThisCar = 11,
			PlayerIsBusy = 12,
			NotEnoughMoney = 13,
			YouCanTBuyACarInTheGarage = 14,
			ErrorWhileBuyCar = 15,
			ErrorWhileExchangeCar = 16,
			SitInCar = 17,
			OtherPlayerInGarage = 18,
			PriceShouldBeBiggerMsg = 19,
			CantInGarage = 20,
			OtherPlayerIsInTrade = 21,
			YouAreInRaceNow = 22,
			OtherCarInCarSale = 23,
			CarNotFoundAfterAccept = 24,
			ErrorTradeNone = 25,
			ErrorTradeOfferError = 26,
			ErrorTradeCarNotFound = 27,
			ErrorTradePersonNotFound = 28,
			ErrorTradePhotonViewNotFound = 29,
			ErrorTradeOtherCarNotLegit = 30,
			ErrorTradeResponseFailFromSeller = 31,
			ErrorTradeCarNotFoundPostTrade = 32,
			ErrorTradeBuyMultiplayerCarCancel = 33,
			ErrorTradeCarNotFoundInScene = 34,
			ErrorTradeVehicleDataMissing = 35,
			ErrorTradeOfferErrorCarsNotValid = 36,
			ErrorTradeCarItemIsNull = 37,
			TradeServiceNotAvailable = 38,
			Error = 99
		}

		public enum ErrorTrade
		{
			None = -1,
			OfferError = 1,
			CarNotFound = 2,
			PersonNotFound = 3,
			PhotonViewNotFound = 4,
			OtherCarNotLegit = 5,
			ResponseFailFromSeller = 6,
			CarNotFoundPostTrade = 7,
			BuyMultiplayerCarCancel = 8,
			CarNotFoundInScene = 9,
			VehicleDataMissing = 10,
			OfferErrorCarsNotValid = 11,
			CarItemIsNull = 12
		}

		public static PreTradeResultActionType ShowMessageTradeErrorAndReturnError(ErrorTrade errorTrade)
		{
			ShowMessageTradeError(errorTrade);
			return PreTradeResultActionType.Error;
		}

		public static void ShowMessageTradeError(ErrorTrade errorTrade)
		{
			ShowErrorMessage(ErrorTradeToErrorCode(errorTrade), "");
		}

		public static void ShowErrorMessage(ErrorCode canSellCarResult, string data)
		{
			string text = "";
			float duration = 2f;
			NotificationType type = NotificationType.Success;
			switch (canSellCarResult)
			{
			case ErrorCode.YouCantSellYourLastCar:
				text = "SellCarTrigger.Youcan'tsellyourlastcar=)";
				break;
			case ErrorCode.YouAlreadyAreSellingThisCar:
				text = "SellCarTrigger.Youalreadyaresellingthiscar=)";
				break;
			case ErrorCode.YouCantSellThisCar:
				text = "SellCarTrigger.Youcan'tsellthiscar!";
				duration = 3f;
				type = NotificationType.Error;
				break;
			case ErrorCode.YouSoldThisCar:
				text = "SellCarTrigger.Yousoldthiscar";
				break;
			case ErrorCode.CheatEngine:
				text = "CarBlock.Yourcarinstalledwithanincompatibleengine.Pleasereplaceitwithacompatibleone!";
				duration = 3f;
				type = NotificationType.Error;
				break;
			case ErrorCode.IncompatibleParts:
				text = "CarBlock.Incompatiblepartsareinstalledintheengine";
				duration = 3f;
				type = NotificationType.Error;
				break;
			case ErrorCode.YouMustHaveAnAccount:
				text = "GetBalancesResult.Youmusthaveanaccounttosellcars!";
				duration = 3f;
				type = NotificationType.Error;
				break;
			case ErrorCode.ItsYourCar:
				text = "FreeDriveControl.Itisyourcar;)";
				break;
			case ErrorCode.YouMustHaveAnAccountToBuy:
				text = "BuyThisCarWindow.YouMustHaveAnAccountToBuyACar";
				break;
			case ErrorCode.BlockSellThisCar:
				text = "FreeDriveControl.Youcan'tbuythiscar";
				break;
			case ErrorCode.PlayerIsBusy:
				text = "MultiPlayerPosControl.PlayerIsBusy";
				duration = 5f;
				break;
			case ErrorCode.NotEnoughMoney:
				text = "ExchangeCarService.NotEnoughMoney";
				duration = 3f;
				break;
			case ErrorCode.YouCanTBuyACarInTheGarage:
				text = "ExchangeCarService.YouCanTBuyACarInTheGarage";
				duration = 3f;
				break;
			case ErrorCode.ErrorWhileBuyCar:
				text = "CarChangeLogic.ErrorWhileBuyCar";
				break;
			case ErrorCode.ErrorWhileExchangeCar:
				text = "CarChangeLogic.ErrorWhileExchangeCar";
				break;
			case ErrorCode.SitInCar:
				text = "CarBlock.Youmustbeinyourcar!";
				break;
			case ErrorCode.OtherPlayerInGarage:
				text = "RejectReason.InGarage";
				break;
			case ErrorCode.PriceShouldBeBiggerMsg:
				text = "PlayerProfile.Priceshouldbebiggerthan0!";
				break;
			case ErrorCode.CantInGarage:
				text = "PlayerProfile.Can'tdoitingarage";
				break;
			case ErrorCode.YouAreInRaceNow:
				text = "Trade.CompleteRaceForOffer";
				break;
			case ErrorCode.OtherCarInCarSale:
				text = "MultiPlayerPosControl.PlayerIsBusy";
				break;
			case ErrorCode.OtherPlayerIsInTrade:
				text = "RejectReason.IsInTrade";
				break;
			case ErrorCode.CarNotFoundAfterAccept:
				text = "FreeDriveControl.Playerlefttheroom";
				break;
			case ErrorCode.ErrorTradeOfferError:
				text = "TradeError.OfferError";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeCarNotFound:
				text = "TradeError.CarNotFound";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradePersonNotFound:
				text = "TradeError.PersonNotFound";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradePhotonViewNotFound:
				text = "TradeError.PhotonViewNotFound";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeOtherCarNotLegit:
				text = "TradeError.OtherCarNotLegit";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeResponseFailFromSeller:
				text = "TradeError.ResponseFailFromSeller";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeCarNotFoundPostTrade:
				text = "TradeError.CarNotFoundPostTrade";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeBuyMultiplayerCarCancel:
				text = "TradeError.BuyMultiplayerCarCancel";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeCarNotFoundInScene:
				text = "TradeError.CarNotFoundInScene";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeVehicleDataMissing:
				text = "TradeError.VehicleDataMissing";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeOfferErrorCarsNotValid:
				text = "TradeError.OfferErrorCarsNotValid";
				duration = 3f;
				break;
			case ErrorCode.ErrorTradeCarItemIsNull:
				text = "TradeError.CarItemIsNull";
				duration = 3f;
				break;
			case ErrorCode.TradeServiceNotAvailable:
				text = "TradeError.TradeServiceNotAvailable";
				duration = 3f;
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = new LocalizedString(text);
				if (!string.IsNullOrEmpty(data))
				{
					text2 = text2 + " \n" + data;
				}
				ManagerUIHelper.SetMessage(text2, duration, type);
			}
		}

		private static ErrorCode ErrorTradeToErrorCode(ErrorTrade errorTrade)
		{
			return errorTrade switch
			{
				ErrorTrade.None => ErrorCode.ErrorTradeNone, 
				ErrorTrade.OfferError => ErrorCode.ErrorTradeOfferError, 
				ErrorTrade.CarNotFound => ErrorCode.ErrorTradeCarNotFound, 
				ErrorTrade.PersonNotFound => ErrorCode.ErrorTradePersonNotFound, 
				ErrorTrade.PhotonViewNotFound => ErrorCode.ErrorTradePhotonViewNotFound, 
				ErrorTrade.OtherCarNotLegit => ErrorCode.ErrorTradeOtherCarNotLegit, 
				ErrorTrade.ResponseFailFromSeller => ErrorCode.ErrorTradeResponseFailFromSeller, 
				ErrorTrade.CarNotFoundPostTrade => ErrorCode.ErrorTradeCarNotFoundPostTrade, 
				ErrorTrade.BuyMultiplayerCarCancel => ErrorCode.ErrorTradeBuyMultiplayerCarCancel, 
				ErrorTrade.CarNotFoundInScene => ErrorCode.ErrorTradeCarNotFoundInScene, 
				ErrorTrade.VehicleDataMissing => ErrorCode.ErrorTradeVehicleDataMissing, 
				ErrorTrade.OfferErrorCarsNotValid => ErrorCode.ErrorTradeOfferErrorCarsNotValid, 
				ErrorTrade.CarItemIsNull => ErrorCode.ErrorTradeCarItemIsNull, 
				_ => ErrorCode.Error, 
			};
		}
	}
}
