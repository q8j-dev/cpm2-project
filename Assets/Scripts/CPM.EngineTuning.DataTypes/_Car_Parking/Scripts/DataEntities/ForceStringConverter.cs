using System;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.DataEntities
{
	public class ForceStringConverter : JsonConverter<string>
	{
		public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return reader.Value?.ToString();
		}

		public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
		{
			writer.WriteValue(value);
		}
	}
}
