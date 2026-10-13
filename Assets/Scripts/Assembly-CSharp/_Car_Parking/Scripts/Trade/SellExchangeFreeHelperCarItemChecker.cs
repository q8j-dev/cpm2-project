using UnityEngine;
using _Car_Parking.Scripts.SellExchangeFree;

namespace _Car_Parking.Scripts.Trade
{
	public class SellExchangeFreeHelperCarItemChecker
	{
		public static SellExchangeFreeMessage.ErrorTrade Check(TradeItemCar tradeItemCar, MultiPlayer multiPlayer)
		{
			if (tradeItemCar == null || !multiPlayer)
			{
				return SellExchangeFreeMessage.ErrorTrade.CarItemIsNull;
			}
			GameObject carByOwnerID = multiPlayer.GetCarByOwnerID(tradeItemCar.CreatorActorNr, needToCheckFirstCar: true);
			if (!carByOwnerID)
			{
				return SellExchangeFreeMessage.ErrorTrade.CarNotFoundInScene;
			}
			if (!carByOwnerID.TryGetComponent<VehicleData>(out var component))
			{
				return SellExchangeFreeMessage.ErrorTrade.VehicleDataMissing;
			}
			if (component.CarId != tradeItemCar.CarID)
			{
				return SellExchangeFreeMessage.ErrorTrade.OfferErrorCarsNotValid;
			}
			return SellExchangeFreeMessage.ErrorTrade.None;
		}
	}
}
