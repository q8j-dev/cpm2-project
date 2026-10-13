using MemoryPack;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

namespace CPM.Rating.Entities
{
	public class MaintenanceInfoFormatter : MemoryPackFormatter<MaintenanceInfo>
	{
		public class MemoryPackCustomFormatterAttributeMaintenanceInfo : MemoryPackCustomFormatterAttribute<MaintenanceInfo>
		{
			public override IMemoryPackFormatter<MaintenanceInfo> GetFormatter()
			{
				return new MaintenanceInfoFormatter();
			}
		}

		private const uint FlMask = 255u;

		private const uint FrMask = 65280u;

		private const uint RlMask = 16711680u;

		private const uint RrMask = 4278190080u;

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MaintenanceInfo value)
		{
			uint num = 0u;
			num |= (uint)(value.FrontTyreHealth[0] * 100f);
			num |= (uint)(value.FrontTyreHealth[1] * 100f) << 8;
			num |= (uint)(value.RearTyreHealth[0] * 100f) << 16;
			num |= (uint)(value.RearTyreHealth[1] * 100f) << 24;
			writer.WriteValue<int>((int)value.OilLife);
			writer.WriteValue<float>(Mathf.Round(value.OilCapacity * 10f) / 10f);
			writer.WriteValue<int>((int)value.FuelInTank);
			writer.WriteValue(in num);
			writer.WriteValue<float>((float)(int)(value.Mileage * 10f) / 10f);
		}

		public override void Deserialize(ref MemoryPackReader reader, ref MaintenanceInfo value)
		{
			int num = reader.ReadValue<int>();
			float oilCapacity = reader.ReadValue<float>();
			int num2 = reader.ReadValue<int>();
			uint num3 = reader.ReadValue<uint>();
			float num4 = (float)(num3 & 0xFF) * 0.01f;
			float num5 = (float)((num3 & 0xFF00) >> 8) * 0.01f;
			float num6 = (float)((num3 & 0xFF0000) >> 16) * 0.01f;
			float num7 = (float)((num3 & 0xFF000000u) >> 24) * 0.01f;
			float mileage = reader.ReadValue<float>();
			value = new MaintenanceInfo
			{
				OilLife = num,
				OilCapacity = oilCapacity,
				FuelInTank = num2,
				FrontTyreHealth = new float[2] { num4, num5 },
				RearTyreHealth = new float[2] { num6, num7 },
				Mileage = mileage
			};
		}
	}
}
