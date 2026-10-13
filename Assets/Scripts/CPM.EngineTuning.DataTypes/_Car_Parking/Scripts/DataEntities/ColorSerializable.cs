using System;
using CPM.Database;
using Newtonsoft.Json;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.DataEntities
{
	public class ColorSerializable : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			uint num = BitMaskColorPacker.PackColor((ColorSerializableWithAlpha)value);
			serializer.Serialize(writer, num);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return (ColorSerializableWithAlpha)BitMaskColorPacker.UnpackColor(serializer.Deserialize<uint>(reader));
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ColorSerializableWithAlpha);
		}
	}
}
