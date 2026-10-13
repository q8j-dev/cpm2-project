using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public static class SellExchangeFreeHelperCanConfirmBuy
	{
		public enum CanConfirmBuyCarResult
		{
			Success = 1,
			NotEnoughMoney = 2,
			YouCanTBuyACarInTheGarage = 3,
			BlockSellThisCar = 4,
			PingIsBig = 5,
			CarNotFound = 6,
			Error = 99
		}

		private const int MaxPing = 500;

		public static CanConfirmBuyCarResult CanConfirmBuyCar(CurrencyManagerController currencyManagerController, int price, GameObject car)
		{
			if (!car)
			{
				return CanConfirmBuyCarResult.CarNotFound;
			}
			if (!currencyManagerController.IsMoneyEnough(price))
			{
				return CanConfirmBuyCarResult.NotEnoughMoney;
			}
			if (FlagsStorage.InGarageFree)
			{
				return CanConfirmBuyCarResult.YouCanTBuyACarInTheGarage;
			}
			if (car.GetComponent<VehicleData>().BlockSellThisCar)
			{
				return CanConfirmBuyCarResult.BlockSellThisCar;
			}
			if (PhotonNetwork.GetPing() > 500)
			{
				return CanConfirmBuyCarResult.PingIsBig;
			}
			return CanConfirmBuyCarResult.Success;
		}

		public static SellExchangeFreeMessage.ErrorCode CanConfirmBuyCarResultToErrorCode(CanConfirmBuyCarResult canConfirmBuyCarResult)
		{
			return canConfirmBuyCarResult switch
			{
				CanConfirmBuyCarResult.NotEnoughMoney => SellExchangeFreeMessage.ErrorCode.NotEnoughMoney, 
				CanConfirmBuyCarResult.YouCanTBuyACarInTheGarage => SellExchangeFreeMessage.ErrorCode.YouCanTBuyACarInTheGarage, 
				CanConfirmBuyCarResult.BlockSellThisCar => SellExchangeFreeMessage.ErrorCode.BlockSellThisCar, 
				CanConfirmBuyCarResult.PingIsBig => SellExchangeFreeMessage.ErrorCode.Error, 
				CanConfirmBuyCarResult.CarNotFound => SellExchangeFreeMessage.ErrorCode.CarNotFoundAfterAccept, 
				CanConfirmBuyCarResult.Error => SellExchangeFreeMessage.ErrorCode.Error, 
				_ => SellExchangeFreeMessage.ErrorCode.Error, 
			};
		}
	}
}
