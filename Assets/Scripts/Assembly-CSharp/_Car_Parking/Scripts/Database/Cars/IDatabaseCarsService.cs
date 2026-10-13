using System;
using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataEntities.Car;

namespace _Car_Parking.Scripts.Database.Cars
{
	public interface IDatabaseCarsService
	{
		Progress Progress { get; set; }

		event Action StartSaving;

		event Action EndSaving;

		event Action<CarSlot> DeleteCarLocalData;

		void Init();

		UniTask<List<T>> GenericGetAllCars<T>();

		void SaveCurrentCar(bool saveNow = false, bool saveFullData = false);

		void SaveCurrentCarAfterTime();

		void SaveCarInCacheToSend(CarInfoForDatabase index, bool sendFullData = false);

		void SaveCarInCacheToSend(CarInfo carInfo, bool sendFullData = false);

		bool WasCarDeleted(string carCarGeneratedCarInstanceId);

		void EnterGarage();

		void ExitGarage();

		UniTask DeleteCarFromDatabase(FirebaseCarsService.DeleteCarInfo carSlot, bool ignoreSendingInProgress = false);

		UniTask SendCachedData();

		UniTask<List<string>> GetAllCarsJson(Dictionary<int, CarInfoForDatabase> cars, string accountId);

		void SendAllCarsInBackground(int[] availableCars);

		UniTask SaveAllCarsInCache(Dictionary<int, CarInfoForDatabase> cars, bool sendFullData = false, bool updateVersions = false);

		void CalculateHashAndSave(CarInfoForDatabase carId);

		void DeleteCarFromLocalData(CarSlot carSlot, bool deleteFromProgressService = true);

		UniTask SendAllCarsAsync(int[] availableCars);

		UniTask CalculateHashes(List<CarInfoForDatabase> cars);

		void InitCarHashesPool();

		UniTask DeleteCarFull(CarSlot carSlot);

		UniTask<SaveStatus> SendCarToDatabase(CarInfo carInfo, bool sendFullData = false);

		void DeleteData();

		void ConfirmSendCache(string carInstanceId, bool savePrefs);
	}
}
