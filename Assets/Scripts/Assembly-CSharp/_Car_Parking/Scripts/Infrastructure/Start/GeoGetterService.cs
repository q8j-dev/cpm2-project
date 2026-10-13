using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	public class GeoGetterService : IDisposable, IInitializable, ITickable
	{
		private const int RequestTimeout = 800;

		private const string HeaderApiKey = "X-Api-Key";

		private const string PrefsKeyGeoData = "GetGeoData";

		private const string ApiUrl = "https://geo.ogames.kz/v1";

		private const string ApiKey = "320b93f3e7f4410aa52ce24da363ad04";

		private bool _isDestroyed;

		private readonly ServerTimeService _serverTimeService;

		private GeoData _geoData;

		public GeoData GetGeoData()
		{
			return _geoData;
		}

		public GeoGetterService(ServerTimeService serverTimeService)
		{
			_serverTimeService = serverTimeService;
		}

		public void Initialize()
		{
			GetGeoRequest().Forget();
		}

		private async UniTask GetGeoRequest()
		{
			if (_geoData != null)
			{
				return;
			}
			UnityWebRequest request = UnityWebRequest.Get("https://geo.ogames.kz/v1");
			request.timeout = 800;
			request.SetRequestHeader("X-Api-Key", "320b93f3e7f4410aa52ce24da363ad04");
			await request.SendWebRequest();
			if (!_isDestroyed)
			{
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
				{
					_geoData = new GeoData();
					RegionInfo regionInfo = new RegionInfo(CultureInfo.CurrentCulture.Name);
					_geoData.country = regionInfo.EnglishName;
					_geoData.countryCode = regionInfo.TwoLetterISORegionName;
				}
				else
				{
					string text = request.downloadHandler.text;
					_geoData = JsonUtility.FromJson<GeoData>(text);
					_geoData.lastUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					_serverTimeService.SyncTimeFromServer(_geoData.serverUnixTime);
				}
				SaveCountryData();
			}
		}

		private void SaveCountryData()
		{
			string value = JsonUtility.ToJson(_geoData);
			PlayerPrefs.SetString("GetGeoData", value);
			PlayerPrefs.Save();
		}

		public void Dispose()
		{
			_isDestroyed = true;
		}

		public void Tick()
		{
		}
	}
}
