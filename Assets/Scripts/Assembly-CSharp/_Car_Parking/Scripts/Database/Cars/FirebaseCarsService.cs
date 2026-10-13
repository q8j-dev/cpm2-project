using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using CPM.Scripts.VinylDrawer;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using Persistence.Systems;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.Database.Cars
{
	public class FirebaseCarsService : IDatabaseCarsService, IInitializable, IReInitPreMainMenu, IDeleteUserData, IDisposable
	{
		public class DeleteCarInfo
		{
			public ObscuredString CarInstanceId;

			public ObscuredInt SlotId;

			public DeleteCarInfo(CarSlot carSlot)
			{
				CarInstanceId = carSlot.CarFilePath;
				SlotId = carSlot.ID;
			}

			public object[] ToArray()
			{
				return new object[2]
				{
					(int)SlotId,
					(string)CarInstanceId
				};
			}
		}

		private class CarIdForRemoveFromDatabase
		{
			public string CarInstanceId;
		}

		public const string PlayerPrefCarSaveVersion = "CarSaveVersion0";

		private const int CountCarsToSaveWhileSaving = 10;

		private const int CarSaveVersion = 3;

		private const string CarsToSavePrefsKey = "CarsToSave";

		private const string LastSentCarsHashesPrefsKey = "LastSentCarsHashes";

		private const string CarsInGaragePrefsKey = "CarsInGarage";

		public const string GetAllCarsFn = "GetAllCars24_1";

		public const string SaveCarsPartiallyFn = "SaveCar24_1";

		public const string RemoveCarFromDatabaseFn = "RemoveCarFromDatabase24_1";

		private Dictionary<string, string> _carsToSave = new Dictionary<string, string>();

		private List<string> _carsToSaveUnExitedFromGarage = new List<string>();

		private HashSet<string> _deletedCarIDList = new HashSet<string>();

		private int _carHashInGarage;

		private static CarsHashesPool _carsHashesPool = new CarsHashesPool();

		private bool _sendingInProcess;

		private IDisposable _saveCurrentCarTimer;

		private readonly FirebaseWrappedFunctionFactory _factory;

		private readonly IAccountController _accountController;

		private readonly IConnectionManager _connectionManager;

		private readonly RuntimePlayerDataHandler _runtimePlayerDataHandler;

		private readonly IDataVersionsService _dataVersionsService;

		private readonly LocalDataMigrationManager _localDataMigrationManager;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly IDeleterUserDataGetter _deleterUserDataGetter;

		private readonly IProgressBackend _progressBackend;

		private const int TimeoutOverrideLoadCarsInSeconds = 240;

		private HashStatus HashStatus => (HashStatus)(int)_connectionManager.H;

		public Progress Progress { get; set; }

		private bool IsAuthorized => _accountController.HasEmail;

		public event Action StartSaving;

		public event Action EndSaving;

		public event Action<CarSlot> DeleteCarLocalData;

		[Inject]
		public FirebaseCarsService(IAccountController accountController, IConnectionManager connectionManager, RuntimePlayerDataHandler runtimePlayerDataHandler, IDataVersionsService dataVersionsService, LocalDataMigrationManager localDataMigrationManager, PlayerDataStorage playerDataStorage, IDeleterUserDataGetter deleterUserDataGetter, IProgressBackend progressBackend)
		{
			_localDataMigrationManager = localDataMigrationManager;
			_dataVersionsService = dataVersionsService;
			_runtimePlayerDataHandler = runtimePlayerDataHandler;
			_connectionManager = connectionManager;
			_accountController = accountController;
			_playerDataStorage = playerDataStorage;
			_deleterUserDataGetter = deleterUserDataGetter;
			_progressBackend = progressBackend;
			_factory = new FirebaseWrappedFunctionFactory();
			Progress = new Progress();
			_localDataMigrationManager.SubscribeToMigrationCompletion(OnMigrationCompleted);
		}

		private void OnMigrationCompleted()
		{
			OnMigrationCompletedAsync().Forget();
		}

		private async UniTaskVoid OnMigrationCompletedAsync()
		{
			int num = CarIdStorage.MAX_CAR_ID;
			List<int> carIds = new List<int>();
			for (int i = 0; i <= num; i++)
			{
				if (ObscuredPrefs.GetInt("car" + i) == 1)
				{
					carIds.Add(i);
				}
			}
			await UniTask.DelayFrame(60);
			SendAllCarsAsync(carIds.ToArray()).Forget();
		}

		public void Init()
		{
			LoadCachedCars();
		}

		public void DeleteData()
		{
			ObscuredPrefs.DeleteKey("CarsToSave");
			ObscuredPrefs.DeleteKey("LastSentCarsHashes");
			ObscuredPrefs.DeleteKey("CarsInGarage");
			_carsToSave = new Dictionary<string, string>();
			_carsHashesPool = new CarsHashesPool();
			_carsToSaveUnExitedFromGarage = new List<string>();
		}

		public async UniTask<List<T>> GenericGetAllCars<T>()
		{
			FirebaseWrappedFunction firebaseWrappedFunction = _factory.Create("GetAllCars24_1", 240);
			MemoryPackDecoder decoder = new MemoryPackDecoder(_playerDataStorage.AccountID, useBrotli: true);
			FunctionCallResult<List<T>> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<List<T>>(decoder);
			if (functionCallResult.IsSuccess)
			{
				return functionCallResult.Data;
			}
			return new List<T>();
		}

		public void InitCarHashesPool()
		{
			_carsHashesPool = new CarsHashesPool();
		}

		public async UniTask CalculateHashes(List<CarInfoForDatabase> cars)
		{
			int resultCount = cars.Count;
			for (int i = 0; i < resultCount; i++)
			{
				CalculateAndSetHashes(cars[i]);
				Progress.ProgressValue = (float)i / (float)resultCount;
				await UniTask.Delay(10);
			}
			SaveCachedCarsInPrefs();
		}

		private static void CalculateAndSetHashes(CarInfoForDatabase oneCar)
		{
			OneCarHashes oneCarHashes = _carsHashesPool.Get(oneCar.CarInstanceId);
			oneCarHashes.CalculateChangedValues(oneCar, out var _);
			oneCarHashes.ConfirmSuccess();
		}

		private void SaveCarData(CarInfo carInfo, string jsonSerializedOneCar, bool savePrefs)
		{
			if (!(jsonSerializedOneCar == "") && !(_accountController.Email == "0"))
			{
				switch (HashStatus)
				{
				case HashStatus.Incorrect:
					Debug.Log("NotCorrect hash status. Dont save cars.");
					return;
				case HashStatus.Unknown:
					Debug.Log("Unknown hash status. Waiting for new data about hash. Cache cars.");
					break;
				case HashStatus.Correct:
					Debug.Log("Correct hash. Save cars.");
					break;
				}
				if (!carInfo.IsInstanceIdCorrect(tryFixInstanceId: false))
				{
					Debug.LogError("Car instance id is incorrect. Dont save car:" + carInfo.CarInstanceId);
				}
				else if (!CarIdStorage.IsNfcCar(carInfo.CarID))
				{
					TryAddCarToCache(carInfo, jsonSerializedOneCar, savePrefs);
				}
			}
		}

		public async UniTask SendCachedData()
		{
			Debug.Log("try save cached cars");
			await TrySendCachedCars();
		}

		private async UniTask TrySendCachedCars()
		{
			if (HashStatus == HashStatus.Correct)
			{
				if (Application.internetReachability != NetworkReachability.NotReachable)
				{
					StartSaving?.Invoke();
					try
					{
						CheckUnExitedCarsAndAddToCache();
					}
					catch (Exception ex)
					{
						Debug.LogWarning("CheckUnExitedCarsAndAddToCache:" + ex);
					}
					if (await SendCachedCars() != SaveStatus.InProgress)
					{
						EndSaving?.Invoke();
					}
				}
			}
			else
			{
				Debug.Log("Incorrect hash status. Dont save cars." + _accountController.Email + " " + HashStatus);
			}
		}

		public static bool HasCar(string carId)
		{
			return ObscuredPrefs.GetInt("car" + carId) == 1;
		}

		private void CheckUnExitedCarsAndAddToCache()
		{
			if (_carsToSaveUnExitedFromGarage.Count == 0)
			{
				return;
			}
			foreach (string item in _carsToSaveUnExitedFromGarage)
			{
				if (CarInfoService.GetCarInfoByInstanceId(item, out var carInfo))
				{
					SaveCarInCacheToSend(carInfo);
				}
			}
			_carsToSaveUnExitedFromGarage.Clear();
			SaveCarsInGarageInPrefs();
			SaveCachedCarsInPrefs();
		}

		public void SaveCurrentCar(bool saveNow = false, bool saveFullData = false)
		{
			if (V.Car != null)
			{
				if (V.Car.TryGetComponent<VehicleData>(out var component))
				{
					CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(component.carInfo);
					carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
					carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
					SaveCarInCacheToSend(carInfoForDatabase, saveFullData);
					if (saveNow && AutoSaveService.AutoSaveCarAfterBuy)
					{
						TrySendCachedCars().Forget();
					}
				}
			}
			else
			{
				Debug.LogWarning("No V car to save.");
			}
		}

		public void SaveCurrentCarAfterTime()
		{
			_saveCurrentCarTimer?.Dispose();
			_saveCurrentCarTimer = Observable.Timer(TimeSpan.FromSeconds(3.5)).Subscribe((long _) =>
			{
				SaveCurrentCar();
			});
		}

		public void SaveCarInCacheToSend(CarInfoForDatabase car, bool sendFullData = false)
		{
			if (!CarIdStorage.IsValidCarId(car.CarID))
			{
				Debug.LogError(car?.ToString() + " is not a valid car id (out of range)");
				return;
			}
			string jsonSerializedOneCar = CalculateCarJson(car, _playerDataStorage.AccountID, sendFullData);
			SaveCarData(car, jsonSerializedOneCar, savePrefs: true);
		}

		public void ConfirmSendCache(string carInstanceId, bool savePrefs)
		{
			if (_carsHashesPool.Contains(carInstanceId) && _carsToSave.ContainsKey(carInstanceId))
			{
				_carsHashesPool.Get(carInstanceId).ConfirmSuccess();
				_carsToSave.Remove(carInstanceId);
				if (savePrefs)
				{
					SaveCachedCarsInPrefs();
				}
			}
		}

		public void SaveCarInCacheToSend(CarInfo carInfo, bool sendFullData = false)
		{
			CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(carInfo);
			carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
			carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
			SaveCarInCacheToSend(carInfoForDatabase);
		}

		public async UniTask SaveAllCarsInCache(Dictionary<int, CarInfoForDatabase> cars, bool sendFullData = false, bool updateVersions = false)
		{
			foreach (KeyValuePair<int, CarInfoForDatabase> car in cars)
			{
				CarInfoForDatabase carInfo = car.Value;
				if (carInfo == null)
				{
					continue;
				}
				if (!CarIdStorage.IsValidCarId(carInfo.CarID))
				{
					Debug.LogWarning(carInfo?.ToString() + " is not a valid car id (out of range)");
					continue;
				}
				try
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(3.0));
					string jsonSerializedOneCar = CalculateCarJson(carInfo, _playerDataStorage.AccountID, sendFullData);
					SaveCarData(carInfo, jsonSerializedOneCar, savePrefs: false);
				}
				catch (Exception ex)
				{
					Debug.LogError("Save all cars error carID:" + carInfo.CarID + " " + ex);
				}
			}
			SaveCachedCarsInPrefs();
			if (updateVersions)
			{
				ObscuredPrefs.SetInt("VersionsUpdate", 1);
			}
		}

		public void CalculateHashAndSave(CarInfoForDatabase carId)
		{
			OneCarHashes oneCarHashes = _carsHashesPool.Get(carId.CarInstanceId);
			oneCarHashes.CalculateChangedValues(carId, out var _);
			oneCarHashes.ConfirmSuccess();
			SaveCachedCarsInPrefs();
		}

		public void SendAllCarsInBackground(int[] availableCars)
		{
		}

		public async UniTask SendAllCarsAsync(int[] availableCars)
		{
		}

		private async UniTask<SaveStatus> SendCachedCars()
		{
			if (!IsAuthorized)
			{
				Debug.LogWarning("Dont save cars when not authorized.");
				return SaveStatus.Failed;
			}
			if (_sendingInProcess)
			{
				Debug.LogWarning("Sending cars in progress. Dont save again.");
				return SaveStatus.InProgress;
			}
			if (_carsToSave.Count == 0)
			{
				return SaveStatus.Complete;
			}
			_sendingInProcess = true;
			if (ObscuredPrefs.GetInt("CarSaveVersion0") != 3)
			{
				ObscuredPrefs.SetInt("CarSaveVersion0", 3);
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, string> item in _carsToSave)
				{
					list.Add(item.Key);
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (CarInfoService.GetCarInfoByInstanceId(list[i], out var carInfo))
					{
						CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(carInfo);
						carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
						carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
						SaveCarInCacheToSend(carInfoForDatabase, sendFullData: true);
					}
				}
			}
			if (_carsToSave.Count > 0)
			{
				List<KeyValuePair<string, string>> listDict = _carsToSave.ToList();
				int length = listDict.Count;
				int counter = 0;
				bool wasSomeCarDeleted = false;
				for (int j = 0; j < listDict.Count; j++)
				{
					string carID = listDict[j].Key;
					try
					{
						Progress.ProgressValue = (float)j / (float)length;
						string jsonSerializedOneCar = _carsToSave[carID];
						if (!(await SendOneCar(jsonSerializedOneCar)))
						{
							Debug.LogError("Car: " + carID + " didn't saved with error.");
							continue;
						}
						ConfirmSendCache(carID, savePrefs: false);
						await UniTask.Delay(3);
						counter++;
						if (counter >= 10)
						{
							SaveCachedCarsInPrefs();
							counter = 0;
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Error while saving: " + ex);
						_sendingInProcess = false;
						CheckWasCarDeletedInSave(wasSomeCarDeleted);
						return SaveStatus.Failed;
					}
				}
				SaveCachedCarsInPrefs();
				_sendingInProcess = false;
				CheckWasCarDeletedInSave(wasSomeCarDeleted);
				await TryUpdateVersions();
				return SaveStatus.Complete;
			}
			await TryUpdateVersions();
			_sendingInProcess = false;
			return SaveStatus.Complete;
		}

		public async UniTask<List<string>> GetAllCarsJson(Dictionary<int, CarInfoForDatabase> cars, string accountId)
		{
			List<string> allCarsJson = new List<string>();
			foreach (KeyValuePair<int, CarInfoForDatabase> car in cars)
			{
				CarInfoForDatabase carInfo = car.Value;
				if (carInfo == null)
				{
					continue;
				}
				if (!CarIdStorage.IsValidCarId(carInfo.CarID))
				{
					Debug.LogWarning(carInfo?.ToString() + " is not a valid car id (out of range)");
					continue;
				}
				try
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(3.0));
					string item = CalculateCarJson(carInfo, accountId, sendFullData: true);
					allCarsJson.Add(item);
				}
				catch (Exception ex)
				{
					Debug.LogError("Save all cars error carID:" + carInfo.CarID + " " + ex);
				}
			}
			return allCarsJson;
		}

		private async UniTask TryUpdateVersions()
		{
			if (ObscuredPrefs.GetInt("VersionsUpdate") != 0)
			{
				DataClassesVersionStorage storage = await _runtimePlayerDataHandler.GetDataObject<DataClassesVersionStorage>();
				await _dataVersionsService.SaveDataVersions(storage);
				ObscuredPrefs.SetInt("VersionsUpdate", 0);
			}
		}

		public void DeleteCarFromLocalData(CarSlot carSlot, bool deleteFromProgressService = true)
		{
			if (deleteFromProgressService && carSlot != null)
			{
				_progressBackend.RemoveCar(carSlot.CarFilePath).Forget();
			}
			DeleteCarLocalData?.Invoke(carSlot);
		}

		public async UniTask DeleteCarFull(CarSlot carSlot)
		{
			DeleteCarInfo deleteCar = new DeleteCarInfo(carSlot);
			try
			{
				DeleteCarFromLocalData(carSlot);
			}
			catch (Exception ex)
			{
				Debug.LogError("DeleteCarFull Local:" + ex);
			}
			try
			{
				await DeleteCarFromDatabase(deleteCar, ignoreSendingInProgress: true);
			}
			catch (Exception ex2)
			{
				Debug.LogError("DeleteCarFull Database:" + ex2);
			}
		}

		private async UniTask<bool> SendOneCar(string jsonSerializedOneCar)
		{
			return (await _factory.Create("SaveCar24_1").CallAsyncResult<string>(jsonSerializedOneCar)).IsSuccess;
		}

		public async UniTask<SaveStatus> SendCarToDatabase(CarInfo carInfo, bool sendFullData = false)
		{
			if (carInfo == null)
			{
				Debug.LogError("Car info is null. Dont send car to database.");
				return SaveStatus.Failed;
			}
			if (!IsAuthorized)
			{
				Debug.LogWarning("Dont save cars when not authorized.");
				return SaveStatus.Failed;
			}
			if (!CarIdStorage.IsValidCarId(carInfo.CarID))
			{
				Debug.LogError(carInfo?.ToString() + " is not a valid car id (out of range)");
				return SaveStatus.Failed;
			}
			if (HashStatus != HashStatus.Correct)
			{
				return SaveStatus.Failed;
			}
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return SaveStatus.Failed;
			}
			if (!carInfo.IsInstanceIdCorrect(tryFixInstanceId: false))
			{
				Debug.LogError("Car instance id is incorrect. Dont save car:" + carInfo.CarInstanceId);
				return SaveStatus.Failed;
			}
			if (CarIdStorage.IsNfcCar(carInfo.CarID))
			{
				return SaveStatus.Failed;
			}
			try
			{
				CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(carInfo);
				carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
				carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
				string jsonSerializedOneCar = CalculateCarJson(carInfoForDatabase, _playerDataStorage.AccountID, sendFullData);
				bool num = await SendOneCar(jsonSerializedOneCar);
				string carInstanceId = carInfo.CarInstanceId;
				if (!num)
				{
					Debug.LogError("Car: " + carInstanceId + " didn't saved with error.");
					return SaveStatus.Failed;
				}
				_carsHashesPool.Get(carInstanceId).ConfirmSuccess();
				SaveCachedCarsInPrefs();
				return SaveStatus.Complete;
			}
			catch (Exception)
			{
				return SaveStatus.Failed;
			}
		}

		public async UniTask DeleteCarFromDatabase(DeleteCarInfo deleteCar, bool ignoreSendingInProgress = false)
		{
			RemoveCarFromCache(deleteCar.CarInstanceId);
			if (!IsAuthorized)
			{
				return;
			}
			Debug.Log($"DeleteCarFromDatabase from carInstanceId:{deleteCar.CarInstanceId}");
			if (!ignoreSendingInProgress && _sendingInProcess)
			{
				Debug.Log("Try delete when sending in progress. So wait.");
				await UniTask.WaitUntil(() => !_sendingInProcess);
			}
			if (HashStatus != HashStatus.Incorrect)
			{
				string carInstanceIdToRemove = deleteCar.CarInstanceId;
				if (carInstanceIdToRemove == "")
				{
					carInstanceIdToRemove = PlayerDatas.CreateNewCarID(999);
				}
				if ((await _factory.Create("RemoveCarFromDatabase24_1").CallAsyncResult<int>(deleteCar.ToArray())).IsSuccess)
				{
					PlayerDatas.addDeletedCarID(carInstanceIdToRemove);
				}
			}
		}

		private static void CheckWasCarDeletedInSave(bool wasSomeCarDeleted)
		{
			if (wasSomeCarDeleted)
			{
				UpdateCarsListAfterDeleteCars();
			}
		}

		private static void UpdateCarsListAfterDeleteCars()
		{
			V.CoreGameManager.LoadBoughtCars();
			Debug.Log("Some cars were deleted.");
		}

		public bool WasCarDeleted(string carInstanceIdToCheck)
		{
			return false;
		}

		private string CalculateCarJson(CarInfoForDatabase car, string accountId, bool sendFullData = false)
		{
			try
			{
				_carsHashesPool.Get(car.CarInstanceId).CalculateChangedValues(car, out var changedData, sendFullData);
				if (changedData.Count == 0)
				{
					return "";
				}
				return new MemoryPackDictionarySaveCar().Serialize(changedData, accountId);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Cant save. No file.{car} : " + ex);
				return "";
			}
		}

		public void EnterGarage()
		{
			Debug.Log("EnterGarage.");
			if (V.Car != null && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				if (!_carsToSaveUnExitedFromGarage.Contains(component.carInfo.CarInstanceId))
				{
					_carsToSaveUnExitedFromGarage.Add(component.carInfo.CarInstanceId);
				}
				SaveCarsInGarageInPrefs();
			}
		}

		private void SaveCarsInGarageInPrefs()
		{
			string value = JsonConvert.SerializeObject(_carsToSaveUnExitedFromGarage);
			ObscuredPrefs.SetString("CarsInGarage", value);
		}

		public void ExitGarage()
		{
			SaveCurrentCar();
			if (V.Car != null && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				_carsToSaveUnExitedFromGarage.Remove(component.carInfo.CarInstanceId);
			}
		}

		private void SaveCachedCarsInPrefs()
		{
			string value = JsonConvert.SerializeObject(_carsToSave);
			ObscuredPrefs.SetString("CarsToSave", value);
			string value2 = JsonConvert.SerializeObject(_carsHashesPool);
			ObscuredPrefs.SetString("LastSentCarsHashes", value2);
		}

		private void LoadCachedCars()
		{
			string text = ObscuredPrefs.GetString("CarsToSave");
			if (text == "")
			{
				_carsToSave = new Dictionary<string, string>();
				Debug.Log("Cached cars new");
			}
			else
			{
				_carsToSave = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
				PrintCachedCars();
			}
			string text2 = ObscuredPrefs.GetString("LastSentCarsHashes");
			if (text2 == "")
			{
				_carsHashesPool = new CarsHashesPool();
				Debug.Log("Cached pool new");
			}
			else
			{
				_carsHashesPool = JsonConvert.DeserializeObject<CarsHashesPool>(text2);
				Debug.Log("Cached pool loaded");
			}
			string text3 = ObscuredPrefs.GetString("CarsInGarage");
			if (text3 == "")
			{
				_carsToSaveUnExitedFromGarage = new List<string>();
				Debug.Log("jsonCarsInGarage new");
			}
			else
			{
				_carsToSaveUnExitedFromGarage = JsonConvert.DeserializeObject<List<string>>(text3);
				Debug.Log("jsonCarsInGarage");
			}
		}

		private void PrintCachedCars()
		{
			string text = " ";
			foreach (string key in _carsToSave.Keys)
			{
				text = text + key + ",";
			}
		}

		private void TryAddCarToCache(CarInfo carInfo, string jsonSerializedOneCar, bool savePrefs)
		{
			_carsToSave[carInfo.CarInstanceId] = jsonSerializedOneCar;
			if (savePrefs)
			{
				SaveCachedCarsInPrefs();
			}
		}

		private void RemoveCarFromCache(string carInstanceId)
		{
			if (_carsToSave.ContainsKey(carInstanceId))
			{
				_carsToSave.Remove(carInstanceId);
				SaveCachedCarsInPrefs();
			}
			if (_carsHashesPool.Contains(carInstanceId))
			{
				_carsHashesPool.Remove(carInstanceId);
				SaveCachedCarsInPrefs();
			}
			if (_carsToSaveUnExitedFromGarage.Contains(carInstanceId))
			{
				_carsToSaveUnExitedFromGarage.Remove(carInstanceId);
			}
		}

		public void Initialize()
		{
			_deleterUserDataGetter.Add(this);
			Init();
		}

		public void ReInit()
		{
			Init();
		}

		public void Dispose()
		{
			_saveCurrentCarTimer?.Dispose();
		}
	}
}
