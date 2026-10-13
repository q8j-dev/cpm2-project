using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	internal class RewardIdConverter : JsonConverter<string>
	{
		public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				return reader.Value?.ToString();
			}
			if (reader.TokenType == JsonToken.StartObject)
			{
				return JObject.Load(reader).ToString(Formatting.None);
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
		{
			writer.WriteValue(value);
		}
	}
}
