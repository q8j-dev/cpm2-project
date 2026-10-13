using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Car
{
	public static class BadCarsChecker
	{
		public const int CarInfoToSet = 132;

		private const string StockMaterialNameToSet = "vaz2107";

		public static void CarInfoCheck(CarInfo carInfo, MultiPlayerPosControl multiPlayerPosControl, CarInfoContainer carInfoContainer, BadCarConfig badCarConfig, bool isCarLegit = true)
		{
			if (FlagsStorage.IsMultiFree && carInfo != null && (!isCarLegit || IsCarBad(carInfo.CarID)))
			{
				carInfo.CarID = 132;
				VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(132);
				carInfo.TuningData = defaultCarTuningDataConfig.Tuning.GetTuningData();
				carInfo.InstalledBodyKits = new InstalledBodyKits();
				if (multiPlayerPosControl.TryGetComponent<VehicleData>(out var component) && (bool)badCarConfig)
				{
					badCarConfig.Apply(component, carInfo, 132);
					component.BodyLoadingComposition.ReloadLOD();
				}
				carInfoContainer.VinylsData = default;
			}
		}

		private static bool IsCarBad(int carId)
		{
			return CarIdStorage.IsNfcCar(carId);
		}

		public static void CarDataCheck(VehicleData vehicleData)
		{
			if (FlagsStorage.IsMultiFree && !(vehicleData == null) && !(vehicleData.gameObject == V.Car) && IsCarBad(vehicleData.CarId))
			{
				Color color = new Color(1f, 0.31f, 0.89f);
				vehicleData.CarId = 132;
				if ((bool)vehicleData.StockMaterial)
				{
					vehicleData.StockMaterial.name = "vaz2107";
				}
				if ((bool)vehicleData.CarMat)
				{
					ColorsService.SetCarPaintColor(vehicleData.CarMat, new CarPaintColor(color));
				}
				vehicleData.BlockSellThisCar = true;
			}
		}
	}
}
