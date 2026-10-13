using Newtonsoft.Json;

namespace _Car_Parking.Scripts.Multiplayer.MultiplayerData.Other
{
	public class ConverterUtils
	{
		private static JsonSerializerSettings _settings;

		public static JsonSerializerSettings GetJsonSerializerSettings()
		{
			if (_settings == null)
			{
				_settings = new JsonSerializerSettings();
			}
			return _settings;
		}

		public static object[] TryGetObjectArray(object data)
		{
			if (data is object[])
			{
				return (object[])data;
			}
			return JsonConvert.DeserializeObject<object[]>(data.ToString(), GetJsonSerializerSettings());
		}
	}
}
