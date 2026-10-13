using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CPM.Database;
using CPM.Scripts.Garage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VContainer.Unity;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Interior;

namespace _Car_Parking.Scripts.AssetBundles
{
	public class AssetBundleManager : IInitializable
	{
		public const int VersionKoef = 6;

		private const long OneMonthSeconds = 2592000L;

		private const string AssetBundleCacheName = "AssetBundleCache";

		private static readonly Regex InteriorBundleRegex = new Regex("^(\\d+)i(?:_\\d+)?_v(\\d+)$", RegexOptions.Compiled);

		private const string CdnPrefix = "https://olzhas1.b-cdn.net/CPM2";

		private const string AssetFormat = ".unity3d";

		private const string Platform = "iOS";

		private Dictionary<string, AssetBundleInfo> _cachedAssetBundlesNames = new Dictionary<string, AssetBundleInfo>();

		private Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();

		private Dictionary<string, UniTaskCompletionSource<AssetBundle>> _loadingBundles = new Dictionary<string, UniTaskCompletionSource<AssetBundle>>();

		private HashSet<string> _knownInvalidBundles = new HashSet<string>();

		private HashSet<string> _loggedInvalidSkips = new HashSet<string>();

		private string _assetBundlesPath = Path.Combine(Application.persistentDataPath, "AssetBundles");

		public static bool IsBeta { get; set; } = false;

		private static string MainUrl => "https://olzhas1.b-cdn.net/CPM2/iOS";

		private static string BetaUrl => "https://olzhas1.b-cdn.net/CPM2/iOS" + (IsBeta ? "_Beta" : "");

		private string CachePath
		{
			get
			{
				if (!Directory.Exists(_assetBundlesPath))
				{
					Directory.CreateDirectory(_assetBundlesPath);
				}
				return Path.Combine(_assetBundlesPath, "AssetBundleCache.es3");
			}
		}

		public GarageEntity CurrentGarage { get; set; }

		public ISlotService CarSlotsService { get; set; }

		public ICarInfoService CarInfoService { get; set; }

		public void Initialize()
		{
			try
			{
				if (ES3.FileExists(CachePath) && ES3.KeyExists("AssetBundleCache", CachePath))
				{
					_cachedAssetBundlesNames = ES3.Load<Dictionary<string, AssetBundleInfo>>("AssetBundleCache", CachePath);
				}
				else
				{
					_cachedAssetBundlesNames = new Dictionary<string, AssetBundleInfo>();
				}
			}
			catch (Exception)
			{
				if (ES3.FileExists(CachePath))
				{
					ES3.DeleteFile(CachePath);
				}
				_cachedAssetBundlesNames = new Dictionary<string, AssetBundleInfo>();
			}
			Caching.ClearCache();
			UpdateCacheAssetBundlesNames();
			RunStartupCleanupAsync().Forget();
		}

		private async UniTask RunStartupCleanupAsync()
		{
			await RemoveOutdatedInteriorBundles();
			await TryAutomaticallyRemoveUnusedAssetBundles();
		}

		private async UniTask RemoveOutdatedInteriorBundles()
		{
			List<string> list = new List<string>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string key in _cachedAssetBundlesNames.Keys)
			{
				hashSet.Add(key);
				if (IsOutdatedInteriorBundle(key))
				{
					list.Add(key);
				}
			}
			if (Directory.Exists(_assetBundlesPath))
			{
				string[] files = Directory.GetFiles(_assetBundlesPath);
				for (int i = 0; i < files.Length; i++)
				{
					string fileName = Path.GetFileName(files[i]);
					if (!hashSet.Contains(fileName) && IsOutdatedInteriorBundle(fileName))
					{
						list.Add(fileName);
					}
				}
			}
			if (list.Count != 0)
			{
				await RemoveAssets(list);
				SaveCache();
			}
		}

		private static bool IsOutdatedInteriorBundle(string nameWithVersion)
		{
			Match match = InteriorBundleRegex.Match(nameWithVersion);
			if (!match.Success)
			{
				return false;
			}
			if (!int.TryParse(match.Groups[1].Value, out var result))
			{
				return false;
			}
			if (!int.TryParse(match.Groups[2].Value, out var result2))
			{
				return false;
			}
			int num = InteriorVersionStorage.GetVersionOfInterior(result) + 6;
			return result2 != num;
		}

		private void UpdateCacheAssetBundlesNames()
		{
			List<string> list = new List<string>();
			foreach (string key in _cachedAssetBundlesNames.Keys)
			{
				if (!File.Exists(Path.Combine(_assetBundlesPath, key)))
				{
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				_cachedAssetBundlesNames.Remove(item);
			}
		}

		private async UniTask TryAutomaticallyRemoveUnusedAssetBundles()
		{
			List<string> list = new List<string>();
			long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			foreach (string key in _cachedAssetBundlesNames.Keys)
			{
				if (num - _cachedAssetBundlesNames[key].LastUsedData > 2592000)
				{
					list.Add(key);
				}
			}
			await RemoveAssets(list);
			SaveCache();
		}

		public async UniTask RemoveUnusedDownloadedAssets()
		{
			await RemoveUnusedDownloadedInteriorsAssets();
		}

		private async UniTask RemoveUnusedDownloadedInteriorsAssets()
		{
			List<int> allSlots = CarSlotsService.GetAllSlots();
			List<string> list = new List<string>();
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < allSlots.Count; i++)
			{
				if (CarInfoService.TryGetFromSlot(allSlots[i], out var carInfo) && carInfo.InstalledBodyKits.InteriorId > 0)
				{
					int versionOfInterior = InteriorVersionStorage.GetVersionOfInterior(carInfo.CarID);
					versionOfInterior += 6;
					string arg = ((carInfo.InstalledBodyKits.InteriorId > 1) ? $"{carInfo.CarID}i_{carInfo.InstalledBodyKits.InteriorId}" : $"{carInfo.CarID}i");
					string item = $"{arg}_v{versionOfInterior}";
					hashSet.Add(item);
				}
			}
			foreach (string key in _cachedAssetBundlesNames.Keys)
			{
				if (!hashSet.Contains(key))
				{
					list.Add(key);
				}
			}
			await RemoveAssets(list);
			SaveCache();
		}

		public void UpdateInteriorsAssetsData()
		{
			List<int> allSlots = CarSlotsService.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				if (CarInfoService.TryGetFromSlot(allSlots[i], out var carInfo) && carInfo.InstalledBodyKits.InteriorId > 0)
				{
					int versionOfInterior = InteriorVersionStorage.GetVersionOfInterior(carInfo.CarID);
					versionOfInterior += 6;
					string name = ((carInfo.InstalledBodyKits.InteriorId > 1) ? $"{carInfo.CarID}i_{carInfo.InstalledBodyKits.InteriorId}" : $"{carInfo.CarID}i");
					TryAddNewAssetOrUpdateInfo(name, versionOfInterior);
				}
			}
			SaveCache();
		}

		private void SaveCache()
		{
			string cachePath = CachePath;
			string text = cachePath + ".tmp";
			try
			{
				if (ES3.FileExists(text))
				{
					ES3.DeleteFile(text);
				}
				ES3.Save("AssetBundleCache", _cachedAssetBundlesNames, text);
				if (File.Exists(cachePath))
				{
					File.Delete(cachePath);
				}
				File.Move(text, cachePath);
			}
			catch (Exception)
			{
				if (ES3.FileExists(cachePath))
				{
					ES3.DeleteFile(cachePath);
				}
				if (ES3.FileExists(text))
				{
					ES3.DeleteFile(text);
				}
				ES3.Save("AssetBundleCache", _cachedAssetBundlesNames, cachePath);
			}
		}

		public float GetSizeAllDownloadedAssets()
		{
			float num = 0f;
			foreach (AssetBundleInfo value in _cachedAssetBundlesNames.Values)
			{
				num += value.SizeMb;
			}
			return num;
		}

		private async UniTask RemoveAllCarsInteriors()
		{
			await RemoveAssets(_cachedAssetBundlesNames.Keys.ToList());
			if (CurrentGarage != null && CurrentGarage.GetSpawnedCars().Count > 0)
			{
				foreach (VehicleData value in CurrentGarage.GetSpawnedCars().Values)
				{
					if ((bool)value && !IsAddressableInterior(value))
					{
						value.InteriorsComposition.ResetInterior();
					}
				}
			}
			else
			{
				if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && !IsAddressableInterior(component))
				{
					component.InteriorsComposition.ResetInterior();
				}
				if ((bool)V.LastUsedCar && V.LastUsedCar != V.Car && V.LastUsedCar.TryGetComponent<VehicleData>(out var component2) && !IsAddressableInterior(component2))
				{
					component2.InteriorsComposition.ResetInterior();
				}
			}
			SaveCache();
		}

		private static bool IsAddressableInterior(VehicleData vehicleData)
		{
			int currentInteriorId = vehicleData.InteriorsComposition.CurrentInteriorId;
			return CarIdStorage.LoadsBuiltInInteriorViaAddressables(vehicleData.CarId, currentInteriorId);
		}

		public async UniTask RemoveAllDownloadedAssets()
		{
			await RemoveAllCarsInteriors();
		}

		public void TryAddNewAssetOrUpdateInfo(string name, int version)
		{
			string text = $"{name}_v{version}";
			string text2 = Path.Combine(_assetBundlesPath, text);
			if (File.Exists(text2))
			{
				_cachedAssetBundlesNames.TryAdd(text, new AssetBundleInfo());
				float sizeMb = (float)new FileInfo(text2).Length / 1024f / 1024f;
				_cachedAssetBundlesNames[text].SizeMb = sizeMb;
				_cachedAssetBundlesNames[text].LastUsedData = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				SaveCache();
			}
		}

		public async UniTask RemoveCachedAssetBundle(string name, int version)
		{
			string item = $"{name}_v{version}";
			await RemoveAssets(new List<string> { item });
			SaveCache();
		}

		private async UniTask RemoveAssets(List<string> namesWithVersion)
		{
			foreach (string item in namesWithVersion)
			{
				if (_loadedBundles.TryGetValue(item, out var value))
				{
					_loadedBundles.Remove(item);
					if (value != null)
					{
						try
						{
							value.Unload(unloadAllLoadedObjects: true);
						}
						catch (Exception)
						{
						}
					}
				}
				string path = Path.Combine(_assetBundlesPath, item ?? "");
				if (File.Exists(path))
				{
					File.Delete(path);
					_cachedAssetBundlesNames.Remove(item);
				}
			}
			AsyncOperation asyncOperation = PlayerDatas.UnloadUnusedAssets("AssetBundleManager");
			if (asyncOperation != null)
			{
				await asyncOperation;
			}
			PlayerDatas.CollectGarbage("AssetBundleManager");
		}

		public void TryAddToLoadedAssetBundles(string name, int version, AssetBundle bundle)
		{
			_loadedBundles = _loadedBundles.Where((KeyValuePair<string, AssetBundle> kvp) => kvp.Value != null).ToDictionary((KeyValuePair<string, AssetBundle> kvp) => kvp.Key, (KeyValuePair<string, AssetBundle> kvp) => kvp.Value);
			_loadedBundles.TryAdd($"{name}_v{version}", bundle);
		}

		public void TryUnloadAssetBundle(string key)
		{
			if (_loadedBundles.TryGetValue(key, out var value))
			{
				_loadedBundles.Remove(key);
				value.Unload(unloadAllLoadedObjects: false);
			}
		}

		public bool IsCachedAssetBundle(string name, int version)
		{
			return File.Exists(Path.Combine(_assetBundlesPath, $"{name}_v{version}"));
		}

		private AssetBundle TryGetLoadedAssetBundle(string name, int version)
		{
			return _loadedBundles.GetValueOrDefault($"{name}_v{version}");
		}

		public async UniTask<AssetBundle> TryGetAssetBundleFromCache(string name, int version, CancellationToken cancellationToken)
		{
			string nameWithVersion = $"{name}_v{version}";
			string path = Path.Combine(_assetBundlesPath, nameWithVersion);
			if (!File.Exists(path))
			{
				return null;
			}
			if (_loadingBundles.TryGetValue(nameWithVersion, out var value))
			{
				return await AwaitTcs(value, name, version, cancellationToken);
			}
			UniTaskCompletionSource<AssetBundle> tcs = new UniTaskCompletionSource<AssetBundle>();
			_loadingBundles[nameWithVersion] = tcs;
			AssetBundle assetBundle = null;
			try
			{
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
				if ((await request.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow()).Item1)
				{
					return null;
				}
				if ((bool)request.assetBundle)
				{
					TryAddToLoadedAssetBundles(name, version, request.assetBundle);
					TryAddNewAssetOrUpdateInfo(name, version);
					assetBundle = request.assetBundle;
					return assetBundle;
				}
				DeleteCorruptedBundleFile(path, nameWithVersion);
				return null;
			}
			catch (Exception)
			{
				DeleteCorruptedBundleFile(path, nameWithVersion);
				return null;
			}
			finally
			{
				_loadingBundles.Remove(nameWithVersion);
				tcs.TrySetResult(assetBundle);
			}
		}

		private void DeleteCorruptedBundleFile(string path, string nameWithVersion)
		{
			bool flag = !File.Exists(path);
			if (!flag)
			{
				try
				{
					File.Delete(path);
					flag = true;
				}
				catch (Exception)
				{
				}
			}
			if (flag && _cachedAssetBundlesNames.Remove(nameWithVersion))
			{
				SaveCache();
			}
		}

		public void RemoveLoadedCorruptedAssetBundle(string name, int version)
		{
			string text = $"{name}_v{version}";
			string path = Path.Combine(_assetBundlesPath, text);
			if (_loadedBundles.TryGetValue(text, out var value))
			{
				_loadedBundles.Remove(text);
				if (value != null)
				{
					try
					{
						value.Unload(unloadAllLoadedObjects: true);
					}
					catch (Exception)
					{
					}
				}
			}
			_knownInvalidBundles.Add(text);
			DeleteCorruptedBundleFile(path, text);
		}

		public bool IsBundleMarkedInvalid(string name, int version)
		{
			return IsBundleMarkedInvalid($"{name}_v{version}");
		}

		public bool IsBundleMarkedInvalid(string nameWithVersion)
		{
			return _knownInvalidBundles.Contains(nameWithVersion);
		}

		private void TryDeleteOldVersionAssetBundle(string name, int version)
		{
			string assetBundlesPath = _assetBundlesPath;
			string value = name + "_v";
			string text = $"{name}_v{version}";
			if (!Directory.Exists(assetBundlesPath))
			{
				return;
			}
			string[] files = Directory.GetFiles(assetBundlesPath);
			foreach (string path in files)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				if (fileNameWithoutExtension.StartsWith(value) && !(fileNameWithoutExtension == text))
				{
					if (_cachedAssetBundlesNames.ContainsKey(fileNameWithoutExtension))
					{
						_cachedAssetBundlesNames.Remove(fileNameWithoutExtension);
					}
					File.Delete(path);
				}
			}
			SaveCache();
		}

		public async UniTask<AssetBundle> DownloadAssetBundle(string url, string name, int version, CancellationToken token)
		{
			string nameWithVersion = $"{name}_v{version}";
			if (IsBundleMarkedInvalid(nameWithVersion))
			{
				return null;
			}
			string path = Path.Combine(_assetBundlesPath, nameWithVersion);
			if (_loadingBundles.TryGetValue(nameWithVersion, out var value))
			{
				return await AwaitTcs(value, name, version, token);
			}
			UniTaskCompletionSource<AssetBundle> tcs = new UniTaskCompletionSource<AssetBundle>();
			_loadingBundles[nameWithVersion] = tcs;
			AssetBundle assetBundle = null;
			try
			{
				using (UnityWebRequest request = UnityWebRequest.Get(url))
				{
					request.downloadHandler = new DownloadHandlerFile(path);
					try
					{
						await request.SendWebRequest();
					}
					catch (Exception)
					{
						DeleteCorruptedBundleFile(path, nameWithVersion);
						return null;
					}
					if (request.result != UnityWebRequest.Result.Success)
					{
						DeleteCorruptedBundleFile(path, nameWithVersion);
						return null;
					}
					AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
					await bundleRequest;
					assetBundle = bundleRequest.assetBundle;
					if (!assetBundle)
					{
						_knownInvalidBundles.Add(nameWithVersion);
						DeleteCorruptedBundleFile(path, nameWithVersion);
					}
				}
				if (assetBundle != null)
				{
					TryDeleteOldVersionAssetBundle(name, version);
					TryAddToLoadedAssetBundles(name, version, assetBundle);
					TryAddNewAssetOrUpdateInfo(name, version);
				}
				return assetBundle;
			}
			finally
			{
				_loadingBundles.Remove(nameWithVersion);
				tcs.TrySetResult(assetBundle);
			}
		}

		public async UniTask<AssetBundle> GetInteriorAssetBundle(string name, int version, CancellationToken token)
		{
			string key = $"{name}_v{version}";
			if (_loadingBundles.TryGetValue(key, out var value))
			{
				return await AwaitTcs(value, name, version, token);
			}
			AssetBundle assetBundle = TryGetLoadedAssetBundle(name, version);
			if (!assetBundle)
			{
				assetBundle = await TryGetAssetBundleFromCache(name, version, token);
			}
			return assetBundle;
		}

		public async UniTask<AssetBundle> DownloadInteriorAssetBundle(string name, int version, CancellationToken token)
		{
			string text = $"{name}_v{version}";
			if (IsBundleMarkedInvalid(text))
			{
				_loggedInvalidSkips.Add(text);
				return null;
			}
			AssetBundle assetBundle = await GetInteriorAssetBundle(name, version, token);
			if (assetBundle != null)
			{
				return assetBundle;
			}
			string url = BetaUrl + "/" + name + ".unity3d";
			string urlMain = MainUrl + "/" + name + ".unity3d";
			assetBundle = await DownloadAssetBundle(url, name, version, token);
			if (!assetBundle)
			{
				assetBundle = await DownloadAssetBundle(urlMain, name, version, token);
			}
			return assetBundle;
		}

		private async UniTask<AssetBundle> AwaitTcs(UniTaskCompletionSource<AssetBundle> tcs, string name, int version, CancellationToken token)
		{
			var (flag, assetBundle) = await tcs.Task.AttachExternalCancellation(token).SuppressCancellationThrow();
			if (flag || assetBundle == null)
			{
				return null;
			}
			TryAddNewAssetOrUpdateInfo(name, version);
			return assetBundle;
		}
	}
}
