using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace _Car_Parking.Scripts.Analytics.Root.Data
{
	public class AnalyticsRepository<T>
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
		{
			Converters = { (JsonConverter)new StringEnumConverter() }
		};

		private readonly string _key;

		public AnalyticsRepository(string key)
		{
			_key = key;
		}

		public void SaveData(T data)
		{
			string value = JsonConvert.SerializeObject(data, Formatting.Indented, JsonSettings);
			PlayerPrefs.SetString(_key, value);
		}

		public T LoadData()
		{
			return JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(_key));
		}
	}
}
