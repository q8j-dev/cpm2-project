using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataEntities.Car;

namespace CPM.Database
{
	public interface ICarInfoService
	{
		CarInfo CreateDefault(int carId, string carInstanceID = "");

		CarInfo GetFromSlot(int slotId);

		CarInfo GetFromFirstNotEmptySlot();

		bool TryGetFromSlot(int slotId, out CarInfo carInfo);

		void DeleteFromSlot(CarSlot slotId);

		int GetFirstSlotIdByCarId(int carId);

		void ClearPool();

		Dictionary<int, CarInfoForDatabase> GetAllCars();

		UniTask SaveCarsFromDatabase(List<CarInfoForDatabase> carInfos);

		void CheckAllSlots();
	}
}
