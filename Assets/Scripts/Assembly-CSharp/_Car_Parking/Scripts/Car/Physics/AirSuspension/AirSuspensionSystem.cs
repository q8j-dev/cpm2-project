using CPM.Database;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Car.Physics.AirSuspension
{
	public class AirSuspensionSystem : AirSuspensionBase, IInitializable
	{
		private readonly IDatabaseCarsService _databaseCarsService;

		private PurchaseItemService _purchaseItemService;

		[Inject]
		public AirSuspensionSystem(IDatabaseCarsService databaseCarsService, PurchaseItemService purchaseItemService)
		{
			_purchaseItemService = purchaseItemService;
			_databaseCarsService = databaseCarsService;
		}

		public void Initialize()
		{
		}

		public override bool IsBought(CarInfo carInfo)
		{
			return carInfo.AdditionalData.AirSuspension.Bought;
		}

		public override int GetPrice()
		{
			return _purchaseItemService.GetPrice(PurchaseItemType.Suspension);
		}

		public override async UniTask<bool> TryBuy(CarInfo carInfo)
		{
			if ((await _purchaseItemService.TryPurchase(PurchaseItemType.Suspension, new
			{
				carInstanceId = carInfo.CarInstanceId
			}, new AdvancedInt(GetPrice()))).IsSuccess)
			{
				AdditionalData additionalData = carInfo.AdditionalData;
				if (additionalData.AirSuspension == null)
				{
					additionalData.AirSuspension = new PsData();
				}
				carInfo.AdditionalData.AirSuspension.Bought = true;
				BoughtUpdated(carInfo);
				carInfo.AdditionalData.AirSuspension.Set(isInstall: false);
				carInfo.Save();
				_databaseCarsService.SaveCurrentCar(saveNow: true);
				return true;
			}
			return false;
		}
	}
}
