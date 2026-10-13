using System;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

namespace _Car_Parking.Scripts.DataEntities
{
	public class MaintenanceDataConverter : JsonConverter
	{
		private const uint FlMask = 255u;

		private const uint FrMask = 65280u;

		private const uint RlMask = 16711680u;

		private const uint RrMask = 4278190080u;

		private const int DataLength = 5;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			MaintenanceInfo maintenanceInfo = (MaintenanceInfo)value;
			float[] array = new float[5];
			uint num = 0u;
			num |= (uint)(maintenanceInfo.FrontTyreHealth[0] * 100f);
			num |= (uint)(maintenanceInfo.FrontTyreHealth[1] * 100f) << 8;
			num |= (uint)(maintenanceInfo.RearTyreHealth[0] * 100f) << 16;
			num |= (uint)(maintenanceInfo.RearTyreHealth[1] * 100f) << 24;
			array[0] = (int)maintenanceInfo.OilLife;
			array[1] = Mathf.Round(maintenanceInfo.OilCapacity * 10f) / 10f;
			array[2] = (int)maintenanceInfo.FuelInTank;
			array[3] = num;
			array[4] = (float)(int)(maintenanceInfo.Mileage * 10f) / 10f;
			serializer.Serialize(writer, array);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			float[] array = serializer.Deserialize<float[]>(reader);
			if (array == null || array.Length != 5)
			{
				return default(MaintenanceInfo);
			}
			uint num = (uint)array[3];
			float num2 = (float)(num & 0xFF) * 0.01f;
			float num3 = (float)((num & 0xFF00) >> 8) * 0.01f;
			float num4 = (float)((num & 0xFF0000) >> 16) * 0.01f;
			float num5 = (float)((num & 0xFF000000u) >> 24) * 0.01f;
			return new MaintenanceInfo
			{
				OilLife = array[0],
				OilCapacity = array[1],
				FuelInTank = array[2],
				FrontTyreHealth = new float[2] { num2, num3 },
				RearTyreHealth = new float[2] { num4, num5 },
				Mileage = array[4]
			};
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(MaintenanceInfo);
		}
	}
}
