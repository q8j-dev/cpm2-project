using System;
using CPM.Database;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.DataEntities
{
	public class BodyKitColorConverter : JsonConverter
	{
		public class BodyKitColorSerializable
		{
			public int t;

			public Color c;

			public Color o;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			BodyKitColor bodyKitColor = (BodyKitColor)value;
			BodyKitColorSerializable bodyKitColorSerializable = new BodyKitColorSerializable();
			bodyKitColorSerializable.t = (int)bodyKitColor.BodyKitName;
			object[] array = CarPaintColorConverter.CarPaintColorToArray(bodyKitColor.Color);
			bodyKitColorSerializable.c = (Color)array[0];
			bodyKitColorSerializable.o = (Color)array[1];
			serializer.Serialize(writer, bodyKitColorSerializable);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			BodyKitColorSerializable bodyKitColorSerializable = serializer.Deserialize<BodyKitColorSerializable>(reader);
			if (bodyKitColorSerializable == null)
			{
				return null;
			}
			return new BodyKitColor
			{
				BodyKitName = (BodyKitType)bodyKitColorSerializable.t,
				Color = CarPaintColorConverter.ColorsToCarPaintColor(bodyKitColorSerializable.c, bodyKitColorSerializable.o)
			};
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(BodyKitColor);
		}
	}
}
