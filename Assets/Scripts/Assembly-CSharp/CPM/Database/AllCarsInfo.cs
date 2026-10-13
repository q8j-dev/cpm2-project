using System.Collections.Generic;
using CPM.RemoteConfig;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace CPM.Database
{
	[CreateAssetMenu(fileName = "AllCarsInfo", menuName = "CarPartsTuning/AllCarsInfo")]
	public class AllCarsInfo : SerializedScriptableObject
	{
		[OdinSerialize]
		private Dictionary<int, CarInfo> _allCarsInfo = new Dictionary<int, CarInfo>();

		public List<VehicleStockTuningData> AllCars;

		[SerializeField]
		private List<long> _carPreloadKeys = new List<long>();

		[SerializeField]
		private long _carPreloadStamp;

		[HideInInspector]
		[SerializeField]
		private List<ObscuredInt> NfsCarIds;

		private IRemoteConfigManager _remoteConfigManager;

		private ConfigValuesStorage _remoteConfigData;

		private List<ObscuredString> _nonTradableCarInstances = new List<ObscuredString>();

		private List<ObscuredInt> _nonTradableCarIds = new List<ObscuredInt>();

		private List<ObscuredInt> _bannedCarIds = new List<ObscuredInt>();

		private bool HasPreloadProfile => _carPreloadStamp != 0;

		[Inject]
		public void Construct(IRemoteConfigManager remoteConfigManager)
		{
			_remoteConfigManager = remoteConfigManager;
			Init().Forget();
		}

		private async UniTaskVoid Init()
		{
			_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
			_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
			if (!string.IsNullOrEmpty(_remoteConfigData.nfsCarIds) && _remoteConfigData.nfsCarIds.TryJson<int[]>(out var value))
			{
				int[] array = value;
				foreach (int num in array)
				{
					_nonTradableCarIds.Add(num);
				}
			}
			if (!string.IsNullOrEmpty(_remoteConfigData.nfsCarInstances))
			{
				string[] array2 = _remoteConfigData.nfsCarInstances.Split(',');
				foreach (string text in array2)
				{
					_nonTradableCarInstances.Add(text);
				}
			}
			if (!string.IsNullOrEmpty(_remoteConfigData.banCarsIds) && _remoteConfigData.banCarsIds.TryJson<int[]>(out var value2))
			{
				int[] array = value2;
				foreach (int num2 in array)
				{
					_bannedCarIds.Add(num2);
				}
			}
		}

		public VehicleStockTuningData GetVehicleStockTuningData(int carId)
		{
			for (int i = 0; i < AllCars.Count; i++)
			{
				if ((int)AllCars[i].Serial == carId)
				{
					return AllCars[i];
				}
			}
			return null;
		}

		public VehicleStockTuningData GetVehicleStockTuningData(PurchaseItemType purchaseItemType)
		{
			for (int i = 0; i < AllCars.Count; i++)
			{
				if (AllCars[i].PurchaseItemType == purchaseItemType)
				{
					return AllCars[i];
				}
			}
			return null;
		}

		public CarInfo GetCarInfo(int carId)
		{
			if (_allCarsInfo.TryGetValue(carId, out var value))
			{
				return value;
			}
			Debug.LogWarning($"[AllCarsInfo] GetCarInfo: carId={carId} not found in catalog of {_allCarsInfo.Count} entries (returning null)");
			return null;
		}

		private static long GetPreloadKey(int carId)
		{
			return (long)(((((ulong)((-3750763034362895579L ^ (uint)(carId * 2654435761u)) * 1099511628211L) ^ ((ulong)(uint)carId << 19)) * 1099511628211L) ^ 0x51B39C4A72E5D681L) * 1099511628211L);
		}

		private static long GetPreloadStamp(List<long> hashes)
		{
			long num = -6502262239297832519L;
			for (int i = 0; i < hashes.Count; i++)
			{
				num = (num * 31) ^ hashes[i];
			}
			return num ^ ((long)hashes.Count * 1374496523L);
		}

		private bool IsPreloadProfileStale()
		{
			if (!HasPreloadProfile)
			{
				return true;
			}
			return GetPreloadStamp(_carPreloadKeys) != _carPreloadStamp;
		}

		public bool IsCarPreloadCached(ObscuredInt carId)
		{
			if (!HasPreloadProfile)
			{
				return false;
			}
			long preloadKey = GetPreloadKey(carId);
			for (int i = 0; i < _carPreloadKeys.Count; i++)
			{
				if (_carPreloadKeys[i] == preloadKey)
				{
					return true;
				}
			}
			return false;
		}

		public void ResolveCarMeta(ObscuredInt carId, out int2 result)
		{
			if (IsPreloadProfileStale())
			{
				result = Utils.NotValid;
			}
			else
			{
				result = (IsCarPreloadCached(carId) ? Utils.NotValid : Utils.Valid);
			}
		}

		public bool IsBannedCar(ObscuredInt carId)
		{
			if (CarIdStorage.IsSteamExcludedCar(carId))
			{
				return true;
			}
			return _bannedCarIds.Contains(carId);
		}

		public int2 CheckTradeCarNotAllowed(ObscuredInt carId)
		{
			if (!_nonTradableCarIds.Contains(carId))
			{
				return Utils.Valid;
			}
			return Utils.NotValid;
		}

		public int2 CheckTradeCarNotAllowed(ObscuredString carInstance)
		{
			if (!_nonTradableCarInstances.Contains(carInstance))
			{
				return Utils.Valid;
			}
			return Utils.NotValid;
		}
	}
}
