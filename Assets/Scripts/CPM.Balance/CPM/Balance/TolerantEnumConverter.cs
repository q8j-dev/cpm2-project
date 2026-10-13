using System;
using Newtonsoft.Json;
using UnityEngine;

namespace CPM.Balance
{
	public sealed class TolerantEnumConverter : JsonConverter
	{
		private const string UnknownMemberName = "Unknown";

		public override bool CanConvert(Type objectType)
		{
			return (Nullable.GetUnderlyingType(objectType) ?? objectType).IsEnum;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == JsonToken.String)
			{
				string text = (string)reader.Value;
				string[] names = Enum.GetNames(enumType);
				foreach (string text2 in names)
				{
					if (string.Equals(text2, text, StringComparison.OrdinalIgnoreCase))
					{
						return Enum.Parse(enumType, text2);
					}
				}
				return Fallback(enumType, text);
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				object obj = Enum.ToObject(enumType, Convert.ToInt64(reader.Value));
				if (!Enum.IsDefined(enumType, obj))
				{
					return Fallback(enumType, obj.ToString());
				}
				return obj;
			}
			return Fallback(enumType, reader.Value?.ToString());
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}

		private static object Fallback(Type enumType, string raw)
		{
			string[] names = Enum.GetNames(enumType);
			foreach (string text in names)
			{
				if (text == "Unknown")
				{
					return Enum.Parse(enumType, text);
				}
			}
			Debug.LogError("TolerantEnumConverter: '" + raw + "' is not a member of " + enumType.Name + " and " + enumType.Name + " has no Unknown member; falling back to 0.");
			return Enum.ToObject(enumType, 0);
		}
	}
}
