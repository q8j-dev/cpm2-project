using System;
using CPM.Database;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceSystem : PoliceSystemBase, IInitializable, IPoliceSystem, IDisposable
	{
		private readonly IDatabaseCarsService _databaseCarsService;

		[Inject]
		public PoliceSystem(IDatabaseCarsService databaseCarsService, PoliceSystemConfig policeSystemConfig)
			: base(policeSystemConfig)
		{
			_databaseCarsService = databaseCarsService;
		}

		public void Initialize()
		{
		}

		public override bool IsBought(CarInfo carInfo)
		{
			return carInfo.AdditionalData.Police.Bought;
		}

		public override void TryBuy(CarInfo carInfo)
		{
			carInfo.AdditionalData.Police.Bought = true;
			BoughtUpdated(carInfo);
			carInfo.AdditionalData.Police.Set(this, carInfo.CarID, isInstall: false);
			carInfo.Save();
			_databaseCarsService.SaveCurrentCar(saveNow: true);
		}
	}
}
