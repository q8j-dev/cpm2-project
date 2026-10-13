using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	public class OfferCarVisualDataDownloader : IOfferCarVisualDataDownloader
	{
		private readonly Dictionary<string, NewCarVisualData> _offerCarVisualDataCache = new Dictionary<string, NewCarVisualData>();

		private readonly Dictionary<int, AllVinylPreset> _allVinylPresetCache = new Dictionary<int, AllVinylPreset>();

		public async UniTask<NewCarVisualData> DownloadOfferCarVisualDataAsync(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			if (_offerCarVisualDataCache.TryGetValue(url, out var value))
			{
				return value;
			}
			using UnityWebRequest request = UnityWebRequest.Get(url);
			await request.SendWebRequest().ToUniTask();
			if (request.result == UnityWebRequest.Result.Success)
			{
				NewCarVisualData newCarVisualData = JsonConvert.DeserializeObject<NewCarVisualData>(request.downloadHandler.text);
				if (newCarVisualData != null)
				{
					_offerCarVisualDataCache[url] = newCarVisualData;
				}
				return newCarVisualData;
			}
			return null;
		}

		public async UniTask<AllVinylPreset> GetVinylPreset(int carID, List<OfferModel> offerModels)
		{
			if (offerModels == null || offerModels.Count == 0 || offerModels[0].Payload.RewardItems.Length == 0)
			{
				return null;
			}
			string uRL = offerModels[0].Payload.RewardItems[0].URL;
			if (string.IsNullOrEmpty(uRL))
			{
				return null;
			}
			if (_allVinylPresetCache.TryGetValue(carID, out var value))
			{
				return value;
			}
			NewCarVisualData newCarVisualData = await DownloadOfferCarVisualDataAsync(uRL);
			if (newCarVisualData == null)
			{
				return null;
			}
			AllVinylPreset allVinylPreset = ScriptableObject.CreateInstance<AllVinylPreset>();
			allVinylPreset.CarID = carID;
			allVinylPreset.allPresetList = new List<OneVinylsPreset>();
			OneVinylsPreset oneVinylsPreset = new OneVinylsPreset();
			oneVinylsPreset.vynilOneCarList = newCarVisualData.VynilOneCar;
			oneVinylsPreset.carColor = newCarVisualData.CarPaintColor.Color;
			allVinylPreset.allPresetList.Add(oneVinylsPreset);
			_allVinylPresetCache.Add(carID, allVinylPreset);
			return allVinylPreset;
		}
	}
}
