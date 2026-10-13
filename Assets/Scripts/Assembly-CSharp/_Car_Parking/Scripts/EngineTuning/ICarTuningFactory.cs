using System;
using CPM.Database;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.EngineTuning
{
	public interface ICarTuningFactory
	{
		event InstallPartHandler OnUpdateCar;

		event Action<string, float> MessageNotification;

		event Action<string, float, uint> MessageNotificationWithOtherSound;

		CarEntity AssemblyCarWithTuningData(CarTuningData tuningDataInstance, int carId, out bool needUpdate);

		CarEntity AssemblyCar(CarInfo carInfo, bool checkNulls = false);

		CarEntity AssemblyNewCar(CarTuningData tuningDataInstance, VehicleStockTuningData vehicleStock, int carId);

		VehicleStockTuningData CreateDefault(int carId);

		void SetPartInCar(PartData partData, CarInfo carInfo, int index = 0);

		void BuyPart(PartData partData);

		void SetPartsInCar(CarInfo carInfo, params PartData[] partsData);

		void CheckStatus(EngineInstance engineInstance);

		void SetNewEngineInCar(CarInfo carInfo, EngineKitData engineDefaultConfig, PartData ecu);

		EngineKitData SetOnePart(PartData currentPart, EngineKitData managerUIEditingEngineKitData, bool placeOldPartInInventory = true);
	}
}
