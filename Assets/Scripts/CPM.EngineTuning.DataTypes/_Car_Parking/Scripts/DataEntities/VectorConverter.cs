using System;
using Newtonsoft.Json;
using UnityEngine;

namespace _Car_Parking.Scripts.DataEntities
{
	public class VectorConverter : JsonConverter
	{
		public class Vector3Serializable
		{
			private const int DigitCount = 4;

			public float X;

			public float Y;

			public float Z;

			public Vector3Serializable(Vector3 vector3)
			{
				X = (float)Math.Round(vector3.x, 4);
				Y = (float)Math.Round(vector3.y, 4);
				Z = (float)Math.Round(vector3.z, 4);
			}

			public Vector3 ToVector3()
			{
				return new Vector3(X, Y, Z);
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Vector3Serializable value2 = new Vector3Serializable((Vector3)value);
			serializer.Serialize(writer, value2);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<Vector3Serializable>(reader).ToVector3();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector3);
		}
	}
}
