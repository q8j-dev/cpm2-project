using CPM.Account;
using CPM.Database;
using CPM.Database.Constructors;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	public static class SellExchangeFreeHelperCanSellOrExchange
	{
		public enum CanSellCarResult
		{
			Success = 1,
			IncompatibleParts = 2,
			CheatEngine = 3,
			YouSoldThisCar = 4,
			YouCantSellThisCar = 5,
			YouAlreadyAreSellingThisCar = 6,
			YouCantSellYourLastCar = 7,
			YouMustHaveAnAccount = 8,
			SitInCar = 9,
			OtherPlayerInGarage = 10,
			PriceShouldBeBiggerMsg = 11,
			CantInGarage = 12,
			YouAreInRaceNow = 13,
			OtherCarInCarSale = 14,
			TradeServiceNotAvailable = 15,
			ItsYourself = 16,
			Error = 99
		}

		public class CanSellCarResultData
		{
			public CanSellCarResult CanSellCarResult;

			public string Data;

			public CanSellCarResultData(CanSellCarResult canSellCarResult, string data = "")
			{
				CanSellCarResult = canSellCarResult;
				Data = data;
			}
		}

		public static async UniTask<CanSellCarResultData> CanSellCarInProfile(GameObject car, FreeDriveProvider freeDriveProvider, IAccountController accountController, AllCarsInfo allCarsInfo, ISlotService slotService, int carPrice, GameObject otherPerson, ITradeService tradeService)
		{
			CanSellCarResultData canSellCarResultData = await CanSellCarBase(car, freeDriveProvider, accountController, allCarsInfo, slotService, carPrice, tradeService);
			if (canSellCarResultData.CanSellCarResult != CanSellCarResult.Success)
			{
				return canSellCarResultData;
			}
			if (!otherPerson)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (!otherPerson.TryGetPhotonView(out var photonView))
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (IsSelf(otherPerson, photonView))
			{
				return new CanSellCarResultData(CanSellCarResult.ItsYourself);
			}
			MultiPersonControl component = otherPerson.GetComponent<MultiPersonControl>();
			if ((bool)component && (bool)component.FirstAndOwnCar && component.FirstAndOwnCar.TryGetComponent<VehicleData>(out var component2))
			{
				if (component2.isGarageCar)
				{
					return new CanSellCarResultData(CanSellCarResult.OtherPlayerInGarage);
				}
				if (component2.СurrentSellCarTrigger != null)
				{
					return new CanSellCarResultData(CanSellCarResult.OtherCarInCarSale);
				}
			}
			return new CanSellCarResultData(CanSellCarResult.Success);
		}

		public static async UniTask<CanSellCarResultData> CanSetCarToSellTrigger(SellCarTrigger sellCarTrigger, GameObject car, FreeDriveProvider freeDriveProvider, IAccountController accountController, AllCarsInfo allCarsInfo, ISlotService slotService, ITradeService tradeService)
		{
			CanSellCarResultData canSellCarResultData = await CanSellCarBase(car, freeDriveProvider, accountController, allCarsInfo, slotService, 999, tradeService);
			if (canSellCarResultData.CanSellCarResult != CanSellCarResult.Success)
			{
				return canSellCarResultData;
			}
			if (V.Car != car)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (!sellCarTrigger || (bool)sellCarTrigger.SellingCar)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (!V.Car || FlagsStorage.IsPassenger || V.Car != freeDriveProvider.FirstCarMultiPlayer)
			{
				return new CanSellCarResultData(CanSellCarResult.SitInCar);
			}
			if (car.GetComponent<VehicleData>().CarOwnerID != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return new CanSellCarResultData(CanSellCarResult.SitInCar);
			}
			return new CanSellCarResultData(CanSellCarResult.Success);
		}

		public static async UniTask<CanSellCarResultData> CanExchangeCar(GameObject car, ISlotService slotService, AllCarsInfo allCarsInfo, IAccountController accountController, FreeDriveProvider freeDriveProvider, GameObject otherPerson, ITradeService tradeService)
		{
			CanSellCarResultData canSellCarResultData = await CanSellOrExchangeCarBase(car, freeDriveProvider, accountController, allCarsInfo, tradeService);
			if (canSellCarResultData.CanSellCarResult != CanSellCarResult.Success)
			{
				return canSellCarResultData;
			}
			if (!otherPerson)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (!otherPerson.TryGetPhotonView(out var photonView))
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (IsSelf(otherPerson, photonView))
			{
				return new CanSellCarResultData(CanSellCarResult.ItsYourself);
			}
			MultiPersonControl component = otherPerson.GetComponent<MultiPersonControl>();
			if ((bool)component && (bool)component.FirstAndOwnCar && component.FirstAndOwnCar.TryGetComponent<VehicleData>(out var component2))
			{
				if (component2.isGarageCar)
				{
					return new CanSellCarResultData(CanSellCarResult.OtherPlayerInGarage);
				}
				if (component2.СurrentSellCarTrigger != null)
				{
					return new CanSellCarResultData(CanSellCarResult.OtherCarInCarSale);
				}
			}
			return new CanSellCarResultData(CanSellCarResult.Success);
		}

		public static bool IsSelf(GameObject otherPerson, PhotonView otherPersonPhotonView)
		{
			if (!otherPerson)
			{
				return false;
			}
			if (otherPerson == V.DriverPerson)
			{
				return true;
			}
			if (otherPerson.TryGetComponent<MultiPersonControl>(out var component) && component.ownPlayer)
			{
				return true;
			}
			if ((bool)otherPersonPhotonView && PhotonNetwork.LocalPlayer != null)
			{
				return otherPersonPhotonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber;
			}
			return false;
		}

		public static bool PlayerInRaceNow()
		{
			if (!V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null)
			{
				return false;
			}
			FreeDriveProvider freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
			if ((bool)freeDriveProvider.FirstCarMultiPlayer && freeDriveProvider.FirstCarMultiPlayer.TryGetComponent<FreeRaceCar>(out var _))
			{
				return true;
			}
			if ((bool)freeDriveProvider.FirstCarMultiPlayer && (bool)freeDriveProvider.DragRacingControlMultiplayer && (bool)freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController && (bool)freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacingTree && (freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacingTree.ownCar == freeDriveProvider.FirstCarMultiPlayer || freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.CurrentRacingTree.otherCar == freeDriveProvider.FirstCarMultiPlayer))
			{
				return true;
			}
			return false;
		}

		private static async UniTask<CanSellCarResultData> CanSellCarBase(GameObject car, FreeDriveProvider freeDriveProvider, IAccountController accountController, AllCarsInfo allCarsInfo, ISlotService slotService, int sellPrice, ITradeService tradeService)
		{
			CanSellCarResultData canSellCarResultData = await CanSellOrExchangeCarBase(car, freeDriveProvider, accountController, allCarsInfo, tradeService);
			if (canSellCarResultData.CanSellCarResult != CanSellCarResult.Success)
			{
				return canSellCarResultData;
			}
			if (slotService.OccupiedSlotCountInAllGarages <= 1)
			{
				return new CanSellCarResultData(CanSellCarResult.YouCantSellYourLastCar);
			}
			if (sellPrice < 1)
			{
				return new CanSellCarResultData(CanSellCarResult.PriceShouldBeBiggerMsg);
			}
			return new CanSellCarResultData(CanSellCarResult.Success);
		}

		private static async UniTask<CanSellCarResultData> CanSellOrExchangeCarBase(GameObject car, FreeDriveProvider freeDriveProvider, IAccountController accountController, AllCarsInfo allCarsInfo, ITradeService tradeService)
		{
			if (!tradeService.Enabled)
			{
				return new CanSellCarResultData(CanSellCarResult.TradeServiceNotAvailable);
			}
			if (PlayerInRaceNow())
			{
				return new CanSellCarResultData(CanSellCarResult.YouAreInRaceNow);
			}
			if (!car)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (car != freeDriveProvider.FirstCarMultiPlayer)
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (FlagsStorage.InGarageFree)
			{
				return new CanSellCarResultData(CanSellCarResult.CantInGarage);
			}
			if (!car.TryGetComponent<VehicleData>(out var vehicleData))
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (!accountController.HasEmail)
			{
				return new CanSellCarResultData(CanSellCarResult.YouMustHaveAnAccount);
			}
			CarInfo carInfo = vehicleData.carInfo;
			VehicleEntityProvider component = car.GetComponent<VehicleEntityProvider>();
			if ((bool)component)
			{
				int2 notValid = Utils.NotValid;
				component.CheckerPartsInEngine(out var partsLine, out var isIncompatible);
				if (math.all(isIncompatible == notValid))
				{
					return new CanSellCarResultData(CanSellCarResult.IncompatibleParts, partsLine);
				}
				component.CheckerEngineCheating(out var res);
				if (math.all(res == notValid))
				{
					return new CanSellCarResultData(CanSellCarResult.CheatEngine);
				}
			}
			vehicleData.IsTraded.Get(out var value);
			if (value)
			{
				return new CanSellCarResultData(CanSellCarResult.YouSoldThisCar);
			}
			if (CarIdStorage.IsRewardExclusiveCar(vehicleData.CarId))
			{
				return new CanSellCarResultData(CanSellCarResult.YouCantSellThisCar);
			}
			allCarsInfo.ResolveCarMeta(vehicleData.CarId, out var result);
			if (math.all(result == Utils.NotValid) || math.all(allCarsInfo.CheckTradeCarNotAllowed(vehicleData.CarId) == Utils.NotValid) || math.all(allCarsInfo.CheckTradeCarNotAllowed(carInfo.CarInstanceId) == Utils.NotValid))
			{
				return new CanSellCarResultData(CanSellCarResult.YouCantSellThisCar);
			}
			if (await accountController.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.CarSell))
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			if (vehicleData.СurrentSellCarTrigger != null)
			{
				return new CanSellCarResultData(CanSellCarResult.YouAlreadyAreSellingThisCar);
			}
			if (!CarUniqueIdGenerator.IsCarInstanceIdCorrect(vehicleData.carInfo.CarInstanceId, vehicleData.CarId))
			{
				return new CanSellCarResultData(CanSellCarResult.Error);
			}
			return new CanSellCarResultData(CanSellCarResult.Success);
		}

		public static SellExchangeFreeMessage.ErrorCode CanSellCarResultToErrorCode(CanSellCarResult canSellCarResult)
		{
			return canSellCarResult switch
			{
				CanSellCarResult.IncompatibleParts => SellExchangeFreeMessage.ErrorCode.IncompatibleParts, 
				CanSellCarResult.CheatEngine => SellExchangeFreeMessage.ErrorCode.CheatEngine, 
				CanSellCarResult.YouSoldThisCar => SellExchangeFreeMessage.ErrorCode.YouSoldThisCar, 
				CanSellCarResult.YouCantSellThisCar => SellExchangeFreeMessage.ErrorCode.YouCantSellThisCar, 
				CanSellCarResult.YouAlreadyAreSellingThisCar => SellExchangeFreeMessage.ErrorCode.YouAlreadyAreSellingThisCar, 
				CanSellCarResult.YouCantSellYourLastCar => SellExchangeFreeMessage.ErrorCode.YouCantSellYourLastCar, 
				CanSellCarResult.YouMustHaveAnAccount => SellExchangeFreeMessage.ErrorCode.YouMustHaveAnAccount, 
				CanSellCarResult.SitInCar => SellExchangeFreeMessage.ErrorCode.SitInCar, 
				CanSellCarResult.OtherPlayerInGarage => SellExchangeFreeMessage.ErrorCode.OtherPlayerInGarage, 
				CanSellCarResult.PriceShouldBeBiggerMsg => SellExchangeFreeMessage.ErrorCode.PriceShouldBeBiggerMsg, 
				CanSellCarResult.CantInGarage => SellExchangeFreeMessage.ErrorCode.CantInGarage, 
				CanSellCarResult.YouAreInRaceNow => SellExchangeFreeMessage.ErrorCode.YouAreInRaceNow, 
				CanSellCarResult.OtherCarInCarSale => SellExchangeFreeMessage.ErrorCode.OtherCarInCarSale, 
				CanSellCarResult.TradeServiceNotAvailable => SellExchangeFreeMessage.ErrorCode.TradeServiceNotAvailable, 
				CanSellCarResult.ItsYourself => SellExchangeFreeMessage.ErrorCode.ItsYourCar, 
				CanSellCarResult.Error => SellExchangeFreeMessage.ErrorCode.Error, 
				_ => SellExchangeFreeMessage.ErrorCode.Error, 
			};
		}
	}
}
