using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataEntities.Car;
using Legacy.Database;
using Persistence.Systems;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.DataEntities
{
	public class ExternalSaveableDataAdapter
	{
		private readonly IDatabaseCarsService databaseCarsService;

		private readonly IDataVersionsService dataVersionService;

		private readonly IMigrationManager migrationManager;

		private readonly VersionedDataClassesConfig dataClassesConfig;

		private readonly RuntimePlayerDataHandler runtimePlayerDataHandler;

		private Action<float> OnProgress;

		[Inject]
		public ExternalSaveableDataAdapter(IDatabaseCarsService databaseCarsService, IMigrationManager migrationManager, VersionedDataClassesConfig dataClassesConfig, IDataVersionsService dataVersionService, RuntimePlayerDataHandler runtimePlayerDataHandler)
		{
			this.databaseCarsService = databaseCarsService;
			this.migrationManager = migrationManager;
			this.dataVersionService = dataVersionService;
			this.dataClassesConfig = dataClassesConfig;
			this.runtimePlayerDataHandler = runtimePlayerDataHandler;
		}

		public async UniTask<OneCarsCollection<T>> GetAllCars<T>(Action<float> progressCallback) where T : class
		{
			OnProgress = progressCallback;
			OnProgress?.Invoke(0.05f);
			DataVersionHistory classVersionHistory = GetClassVersionHistory("cars");
			string lastVersion = classVersionHistory.entries[classVersionHistory.entries.Length - 1].version;
			OnProgress?.Invoke(0.07f);
			DataClassesVersionStorage storage = new DataClassesVersionStorage();
			storage.Initialize();
			HashSet<int> migratedCars = new HashSet<int>();
			List<T> cars = await MigrateAndGetCars<T>(migratedCars);
			storage.SetEntry("cars", lastVersion);
			storage.SetEntry("vinyls", lastVersion);
			await runtimePlayerDataHandler.SaveDataObject(storage);
			return new OneCarsCollection<T>
			{
				cars = cars,
				migratedCars = migratedCars
			};
		}

		private async UniTask<List<T>> MigrateAndGetCars<T>(HashSet<int> migratedCars) where T : class
		{
			int ticks = 0;
			List<T> newCars = new List<T>();
			IDisposable loading = Observable.Interval(TimeSpan.FromMilliseconds(500.0)).Subscribe((long _) =>
			{
				OnProgress?.Invoke(Mathf.Clamp(0.1f + (float)ticks * 0.01f, 0f, 0.5f));
				ticks++;
			});
			try
			{
				List<object> cars = await databaseCarsService.GenericGetAllCars<object>();
				if (cars == null || cars.Count == 0)
				{
					throw new MigrationException("cars collection is null or empty", ExceptionCode.MG_C);
				}
				IEnumerable<string> enumerable = cars.Select((object item) => item.ToString());
				OnProgress?.Invoke(0.5f);
				foreach (string item in enumerable)
				{
					T val = await ProcessCar(item, newCars, cars, migratedCars);
					if (val != null)
					{
						newCars.Add(val);
					}
				}
				return newCars;
			}
			catch (MigrationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new MigrationException(ex2.ToString(), ExceptionCode.MG_M_1);
			}
			finally
			{
				loading.Dispose();
			}
		}

		private async UniTask<T> ProcessCar<T>(string car, List<T> newCars, List<object> cars, HashSet<int> migratedCars) where T : class
		{
			T result = null;
			car.TryJson<OneCar>(out var value);
			if (value.floats == null || value.vectors == null)
			{
				if (value.CarID == -1)
				{
					Debug.LogWarning($"CarID == -1. Possibly not a car:{value.CarID}");
					return null;
				}
				if (value.CarID < 0)
				{
					Debug.LogError($"Empty car while migrating. Skip car:{value.CarID}");
					return result;
				}
				Debug.LogWarning($"Empty car while migrating. Create default car:{value.CarID}");
				value = OneCarFactory.CreateDefaultOneCar(value.CarID);
			}
			if (value == null || IsOldVersion(value))
			{
				car.TryJson<OneCarLegacy>(out var legacyCar);
				if (legacyCar == null)
				{
					throw new MigrationException("Cant parse car data from database", ExceptionCode.MG_M_2);
				}
				object obj = legacyCar;
				result = (T)(await migrationManager.MigrateData(obj, obj.GetType(), typeof(T), null, commit: true));
				migratedCars.Add(legacyCar.CarID);
			}
			else
			{
				value.dataVersion = 2;
				result = value as T;
			}
			OnProgress?.Invoke(0.5f + (float)newCars.Count / (float)cars.Count * 0.45f);
			return result;
		}

		private bool IsOldVersion(OneCar oneCar)
		{
			if (oneCar == null)
			{
				return true;
			}
			int num = 0;
			if (int.TryParse(oneCar.dataVersion?.ToString(), out var result))
			{
				num = result;
			}
			if (num != 2)
			{
				return oneCar.floats.Count < 40;
			}
			return false;
		}

		private bool IsLastVersion(DataVersionHistory history, string currentVersion)
		{
			bool num = string.IsNullOrEmpty(currentVersion);
			if (!num && history.entries.All((DataVersionHistory.Entry entry) => entry.version != currentVersion))
			{
				throw new MigrationException("Migration error", ExceptionCode.MG_V);
			}
			if (num && history.entries.Length != 0)
			{
				currentVersion = history.entries[0].version;
			}
			string version = history.entries[history.entries.Length - 1].version;
			return currentVersion == version;
		}

		private DataVersionHistory GetClassVersionHistory(string path)
		{
			DataVersionHistory dataVersionHistory = dataClassesConfig.entryHistories.SingleOrDefault((DataVersionHistory h) => h.id == path);
			if (dataVersionHistory == null)
			{
				throw new Exception("No suitable history found for data at path " + path);
			}
			dataVersionHistory.Initialize();
			return dataVersionHistory;
		}
	}
}
