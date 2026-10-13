using System;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.DataEntities
{
	public class CarPaintColorConverter : JsonConverter
	{
		public static object[] CarPaintColorToArray(CarPaintColor carPaintColor)
		{
			object[] obj = new object[2] { carPaintColor.Color, null };
			float r = (float)carPaintColor.CarMatType / 16f;
			Color color = new Color(r, carPaintColor.IridescencePower, carPaintColor.IridescenceThickness);
			obj[1] = color;
			return obj;
		}

		public static CarPaintColor ColorsToCarPaintColor(Color color1, Color otherData)
		{
			CarMatType carMatType = (CarMatType)Mathf.Round(otherData.r * 16f);
			float g = otherData.g;
			float b = otherData.b;
			return new CarPaintColor
			{
				CarMatType = carMatType,
				Color = color1,
				IridescencePower = g,
				IridescenceThickness = b
			};
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			object[] value2 = CarPaintColorToArray((CarPaintColor)value);
			serializer.Serialize(writer, value2);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Color[]? array = serializer.Deserialize<Color[]>(reader);
			Color color = array[0];
			Color otherData = array[1];
			return ColorsToCarPaintColor(color, otherData);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(CarPaintColor);
		}
	}
}
