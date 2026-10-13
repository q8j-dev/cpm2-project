using CPM.Account;
using CPM.Database.Constructors;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public static class SellExchangeFreeHelperCanBuy
	{
		public enum CanBuyCarResult
		{
			Success = 1,
			YouMustHaveAnAccount = 2,
			ItsYourCar = 3,
			BlockSellThisCar = 4,
			CarInstanceIdIncorrect = 5,
			Error = 99
		}

		public static CanBuyCarResult CanBuyCar(GameObject car, IAccountController accountController, FreeDriveProvider freeDriveProvider, int priceCar)
		{
			if (!accountController.HasEmail)
			{
				return CanBuyCarResult.YouMustHaveAnAccount;
			}
			if (car == freeDriveProvider.FirstCarMultiPlayer)
			{
				return CanBuyCarResult.ItsYourCar;
			}
			VehicleData component = car.GetComponent<VehicleData>();
			if (component.BlockSellThisCar)
			{
				return CanBuyCarResult.BlockSellThisCar;
			}
			if (priceCar <= 0)
			{
				return CanBuyCarResult.Error;
			}
			if (!CarUniqueIdGenerator.IsCarInstanceIdCorrect(component.CarInfoProvider.CarInfo.CarInstanceId, component.CarId))
			{
				return CanBuyCarResult.CarInstanceIdIncorrect;
			}
			return CanBuyCarResult.Success;
		}

		public static SellExchangeFreeMessage.ErrorCode CanBuyCarResultToErrorCode(CanBuyCarResult canBuyCarResult)
		{
			return canBuyCarResult switch
			{
				CanBuyCarResult.ItsYourCar => SellExchangeFreeMessage.ErrorCode.ItsYourCar, 
				CanBuyCarResult.YouMustHaveAnAccount => SellExchangeFreeMessage.ErrorCode.YouMustHaveAnAccountToBuy, 
				CanBuyCarResult.BlockSellThisCar => SellExchangeFreeMessage.ErrorCode.BlockSellThisCar, 
				CanBuyCarResult.CarInstanceIdIncorrect => SellExchangeFreeMessage.ErrorCode.Error, 
				CanBuyCarResult.Error => SellExchangeFreeMessage.ErrorCode.Error, 
				_ => SellExchangeFreeMessage.ErrorCode.Error, 
			};
		}
	}
}
