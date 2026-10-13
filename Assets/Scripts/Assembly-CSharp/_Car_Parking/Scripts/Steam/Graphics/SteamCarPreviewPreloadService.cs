using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GraphicsSettings;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	public class SteamCarPreviewPreloadService : IInitializable, IDisposable
	{
		private readonly AllCarsInfo _allCarsInfo;

		private readonly List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

		private readonly HashSet<int> _ready = new HashSet<int>();

		private CancellationTokenSource _cts;

		private bool _started;

		private const int LoadsPerYield = 4;

		[Inject]
		public SteamCarPreviewPreloadService(AllCarsInfo allCarsInfo)
		{
			_allCarsInfo = allCarsInfo;
		}

		public bool IsReady(int carId)
		{
			return _ready.Contains(carId);
		}

		public void Initialize()
		{
			int ramMb = RamDevice.GetRamMb();
			int num = 12288;
			if (!RamDevice.AllowHeavyPreload())
			{
				Debug.Log($"[HeavyPreload] Cars: skipped (RAM {ramMb}MB <= {num}MB)");
				return;
			}
			Debug.Log($"[HeavyPreload] Cars: enabled (RAM {ramMb}MB > {num}MB)");
			_cts = new CancellationTokenSource();
			PreloadAllAsync(_cts.Token).Forget();
		}

		private async UniTaskVoid PreloadAllAsync(CancellationToken ct)
		{
			if (_started || _allCarsInfo?.AllCars == null)
			{
				return;
			}
			_started = true;
			int sinceYield = 0;
			foreach (VehicleStockTuningData allCar in _allCarsInfo.AllCars)
			{
				if (ct.IsCancellationRequested)
				{
					return;
				}
				if (allCar == null)
				{
					continue;
				}
				int id = allCar.Serial;
				if (!_allCarsInfo.IsBannedCar(id))
				{
					int num = sinceYield;
					sinceYield = num + await LoadAndHold($"sell model/{id}.prefab", ct);
					num = sinceYield;
					sinceYield = num + await LoadAndHold($"body/b{id}.prefab", ct);
					if (!CarIdStorage.IsInteriorBakedIntoBody(id))
					{
						num = sinceYield;
						sinceYield = num + await LoadAndHold($"{id}i", ct);
					}
					_ready.Add(id);
					if (sinceYield >= 4)
					{
						sinceYield = 0;
						await UniTask.Yield(ct);
					}
				}
			}
			Debug.Log($"[CarPreload] Done: {_ready.Count} cars preloaded, {_handles.Count} assets resident.");
		}

		private async UniTask<int> LoadAndHold(string key, CancellationToken ct)
		{
			_ = 1;
			try
			{
				AsyncOperationHandle<IList<IResourceLocation>> locHandle = Addressables.LoadResourceLocationsAsync(key);
				await locHandle.WithCancellation(ct);
				bool num = locHandle.Status == AsyncOperationStatus.Succeeded && locHandle.Result != null && locHandle.Result.Count > 0;
				if (locHandle.IsValid())
				{
					Addressables.Release(locHandle);
				}
				if (!num)
				{
					return 1;
				}
				AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
				await handle.WithCancellation(ct);
				if (handle.Status == AsyncOperationStatus.Succeeded)
				{
					_handles.Add(handle);
				}
				else if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch
			{
			}
			return 1;
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
			foreach (AsyncOperationHandle handle in _handles)
			{
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
			}
			_handles.Clear();
			_ready.Clear();
		}
	}
}
