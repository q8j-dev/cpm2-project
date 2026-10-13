using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.VinylsAi;

namespace CPM.Database
{
	public class CarInfoService : ICarInfoService, IInitializable, IDisposable, IDeleteUserData
	{
		private class EmptyPathInSlotException : Exception
		{
		}

		public const int DefaultCarId = 13;

		private readonly ICarTuningFactory _carTuningFactory;

		private readonly ISlotService _slotService;

		private readonly Dictionary<string, CarInfo> _pool = new Dictionary<string, CarInfo>();

		private IDatabaseCarsService _databaseCarsService;

		private readonly IDeleterUserDataGetter _deleterUserDataGetter;

		[Inject]
		public CarInfoService(ICarTuningFactory carTuningFactory, ISlotService slotService, IDatabaseCarsService databaseCarsService, IDeleterUserDataGetter deleterUserDataGetter)
		{
			_databaseCarsService = databaseCarsService;
			_slotService = slotService;
			_carTuningFactory = carTuningFactory;
			_deleterUserDataGetter = deleterUserDataGetter;
		}

		public CarInfo CreateDefault(int carId, string carInstanceID = "")
		{
			VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId);
			if (!defaultCarTuningDataConfig)
			{
				carId = 13;
				defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId);
			}
			CarInfoMain carInfoDefault = defaultCarTuningDataConfig.CarInfoDefault;
			VehicleStockTuningData vehicleStockTuningData = _carTuningFactory.CreateDefault(carId);
			CarInfo carInfo = new CarInfo(carInfoDefault);
			carInfo.CarInstanceId = (string.IsNullOrEmpty(carInstanceID) ? GenerateCarInstanceID(carId) : carInstanceID);
			carInfo.TuningData = vehicleStockTuningData.Tuning.GetTuningData();
			carInfo.BoughtBodyKits = new BoughtBodyKits();
			carInfo.IsInstanceIdCorrect(tryFixInstanceId: true);
			carInfo.TuningData.DynamicTuningSettings.FuelType = vehicleStockTuningData.Tuning.GetBaseEngine().FuelRail.GetFT;
			AdditionalData additionalData = carInfo.AdditionalData;
			if (additionalData.Police == null)
			{
				additionalData.Police = new PoliceData();
			}
			VehiclePresetTuningData[] presets = vehicleStockTuningData.Presets;
			if (presets != null && presets.Length > 0 && (bool)vehicleStockTuningData.Presets[0])
			{
				carInfo.TuningData.FrontWheelData = vehicleStockTuningData.Presets[0].FrontWheelData.DeepClone();
				carInfo.TuningData.RearWheelData = vehicleStockTuningData.Presets[0].RearWheelData.DeepClone();
				if ((bool)vehicleStockTuningData.Presets[0].GearBox)
				{
					carInfo.TuningData.GearBox = vehicleStockTuningData.Presets[0].GearBox.CreateNewPartDataFromPart();
				}
			}
			VehicleStockTuningData defaultCarTuningDataConfig2 = CarTuningFactory.GetDefaultCarTuningDataConfig(carInfo.CarID);
			carInfo.CheckForNulls(defaultCarTuningDataConfig2);
			carInfo.TuningData.CheckForNaNs(defaultCarTuningDataConfig2);
			return carInfo;
		}

		public int GetFirstSlotIdByCarId(int carId)
		{
			int num = _slotService.SlotCollection.Count;
			for (int i = 0; i < num; i++)
			{
				CarSlot carSlot = _slotService.SlotCollection[i];
				if (!string.IsNullOrEmpty(carSlot.CarFilePath))
				{
					CarInfo fromSlot = GetFromSlot(carSlot.ID);
					if (fromSlot != null && fromSlot.CarID == carId)
					{
						return carSlot.ID;
					}
				}
			}
			return -1;
		}

		public CarInfo GetFromSlot(int slotId)
		{
			CarSlot slot = _slotService.GetSlot(slotId);
			if (slot == null)
			{
				throw new Exception($"No slot found with id:{slotId}!");
			}
			if (slot.CarFilePath.IsNullOrEmpty())
			{
				throw new EmptyPathInSlotException();
			}
			if (_pool.TryGetValue(slot.CarFilePath, out var value))
			{
				return value;
			}
			string carInfoPath = DirectoryHelper.GetCarInfoPath(slot.CarFilePath);
			CarInfo carInfo = new CarInfo();
			if (EasySaveDataHandler.TryGetFile(carInfoPath, out var file))
			{
				carInfo.Read(file);
				if (carInfo.CarInstanceId != slot.CarFilePath)
				{
					Debug.LogWarning("[CarInfoService] CarInstanceId/'" + carInfo.CarInstanceId + "' != slot.CarFilePath/'" + slot.CarFilePath + "'; syncing to slot path so Save round-trips.");
					carInfo.CarInstanceId = slot.CarFilePath;
				}
				AdditionalData additionalData = carInfo.AdditionalData;
				if (additionalData.Police == null)
				{
					additionalData.Police = new PoliceData();
				}
				_pool.Add(slot.CarFilePath, carInfo);
				return carInfo;
			}
			throw new Exception($"No car file found in slot:{slotId}!");
		}

		public CarInfo GetFromFirstNotEmptySlot()
		{
			int notEmptySlotId = _slotService.GetNotEmptySlotId();
			return GetFromSlot(notEmptySlotId);
		}

		public bool TryGetFromSlot(int slotId, out CarInfo carInfo)
		{
			CarSlot slot = _slotService.GetSlot(slotId);
			if (slot == null)
			{
				carInfo = null;
				return false;
			}
			if (_pool.TryGetValue(slot.CarFilePath, out carInfo))
			{
				return true;
			}
			if (EasySaveDataHandler.TryGetFile(DirectoryHelper.GetCarInfoPath(slot.CarFilePath), out var file))
			{
				CarInfo carInfo2 = new CarInfo();
				carInfo2.Read(file);
				if (carInfo2.CarInstanceId != slot.CarFilePath)
				{
					Debug.LogWarning("[CarInfoService] CarInstanceId/'" + carInfo2.CarInstanceId + "' != slot.CarFilePath/'" + slot.CarFilePath + "'; syncing to slot path so Save round-trips.");
					carInfo2.CarInstanceId = slot.CarFilePath;
				}
				_pool.Add(slot.CarFilePath, carInfo2);
				carInfo = carInfo2;
				return true;
			}
			carInfo = null;
			return false;
		}

		public static bool GetCarInfoByInstanceId(string carInstanceId, out CarInfo carInfo)
		{
			if (EasySaveDataHandler.TryGetFile(DirectoryHelper.GetCarInfoPath(carInstanceId), out var file))
			{
				CarInfo carInfo2 = new CarInfo();
				carInfo2.Read(file);
				carInfo = carInfo2;
				return true;
			}
			carInfo = null;
			return false;
		}

		public void DeleteFromSlot(CarSlot slot)
		{
			VinylsCensorSaver.ClearLocalData(slot.CarFilePath);
			try
			{
				EasySaveDataHandler.DeleteFile(DirectoryHelper.GetCarInfoPath(slot.CarFilePath));
			}
			catch (Exception ex)
			{
				Debug.LogError("Delete car data:" + ex);
			}
			try
			{
				EasySaveDataHandler.DeleteFile(DirectoryHelper.GetCarVinylsPath(slot.CarFilePath));
			}
			catch (Exception ex2)
			{
				Debug.LogError("Delete car v data:" + ex2);
			}
			try
			{
				EasySaveDataHandler.DeleteFile(DirectoryHelper.GetPlateVinylsPath(slot.CarFilePath));
			}
			catch (Exception ex3)
			{
				Debug.LogError("Delete car p data:" + ex3);
			}
			try
			{
				EasySaveDataHandler.DeleteDirectory("cars/" + slot.CarFilePath);
			}
			catch (Exception ex4)
			{
				Debug.LogError("Delete car directory :" + ex4);
			}
			_pool.Remove(slot.CarFilePath);
			_slotService.ClearSlot(slot.ID);
		}

		public void ClearPool()
		{
			_pool.Clear();
		}

		public void DeleteData()
		{
			ClearPool();
		}

		public Dictionary<int, CarInfoForDatabase> GetAllCars()
		{
			List<int> allSlots = _slotService.GetAllSlots();
			Dictionary<int, CarInfoForDatabase> dictionary = new Dictionary<int, CarInfoForDatabase>();
			foreach (int item in allSlots)
			{
				CarInfo carInfo = null;
				try
				{
					carInfo = GetFromSlot(item);
				}
				catch (Exception message)
				{
					Debug.Log(message);
				}
				if (carInfo != null)
				{
					CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(carInfo);
					carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
					carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
					dictionary[item] = carInfoForDatabase;
				}
			}
			return dictionary;
		}

		public async UniTask SaveCarsFromDatabase(List<CarInfoForDatabase> carInfos)
		{
			_slotService.IsAllSlotsEmpty();
			List<CarInfoForDatabase> savedCarInfos = new List<CarInfoForDatabase>();
			for (int i = 0; i < carInfos.Count; i++)
			{
				try
				{
					CarInfoForDatabase carInfoForDatabase = carInfos[i];
					if (!carInfoForDatabase.IsInstanceIdCorrect(tryFixInstanceId: false))
					{
						Debug.LogError("Incorrect car info id from database. " + carInfoForDatabase.CarInstanceId + ". Skip saving.");
						continue;
					}
					VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carInfoForDatabase.CarID);
					carInfoForDatabase.CheckForNulls(defaultCarTuningDataConfig);
					carInfoForDatabase.Save();
					PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(carInfoForDatabase.Vinyls, DirectoryHelper.GetCarVinylsPath(carInfoForDatabase.CarInstanceId));
					PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(carInfoForDatabase.Window, DirectoryHelper.GetWindowVinylsPath(carInfoForDatabase.CarInstanceId));
					savedCarInfos.Add(carInfoForDatabase);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(3.0));
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds(3.0));
			CheckAllSlots();
			try
			{
				await _databaseCarsService.CalculateHashes(savedCarInfos);
			}
			catch (Exception e)
			{
				ReleaseDebug.LogError(e);
			}
			Debug.Log("Save cars from database completed.");
		}

		public void CheckAllSlots()
		{
			for (int i = 0; i < _slotService.SlotCount; i++)
			{
				try
				{
					GetFromSlot(i);
				}
				catch (EmptyPathInSlotException)
				{
				}
				catch (Exception)
				{
					_slotService.ClearSlot(i);
					PlayerPrefs.SetInt("ForceSaveSlots", 1);
				}
			}
		}

		private string GenerateCarInstanceID(int carId)
		{
			return CarUniqueIdGenerator.CreateNewCarID(carId);
		}

		public void Initialize()
		{
			_databaseCarsService.DeleteCarLocalData += DeleteCarFromCarInstanceId;
			_deleterUserDataGetter.Add(this);
		}

		public void Dispose()
		{
			_databaseCarsService.DeleteCarLocalData -= DeleteCarFromCarInstanceId;
			_deleterUserDataGetter.Remove(this);
		}

		private void DeleteCarFromCarInstanceId(CarSlot carSlot)
		{
			DeleteFromSlot(carSlot);
		}
	}
}
