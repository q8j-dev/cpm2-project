using System;
using Newtonsoft.Json;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.DataEntities
{
	public class PartDataConverter : JsonConverter
	{
		public class PartDataSerializable
		{
			public int ID;

			public int W;
		}

		private Func<PartData, PartData> _partDataChecker;

		public const float Accuracy = 100f;

		public PartDataConverter(Func<PartData, PartData> partDataChecker)
		{
			_partDataChecker = partDataChecker;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			PartData partData = (PartData)value;
			PartDataSerializable partDataSerializable = new PartDataSerializable();
			partDataSerializable.ID = partData.ModelId;
			int w = (int)Math.Round(partData.Wear * 100f);
			partDataSerializable.W = w;
			serializer.Serialize(writer, partDataSerializable);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			PartDataSerializable partDataSerializable = serializer.Deserialize<PartDataSerializable>(reader);
			if (partDataSerializable == null)
			{
				return null;
			}
			float wear = (float)partDataSerializable.W / 100f;
			PartData arg = new PartData
			{
				ModelId = partDataSerializable.ID,
				Wear = wear
			};
			return _partDataChecker(arg);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(PartData);
		}
	}
}
