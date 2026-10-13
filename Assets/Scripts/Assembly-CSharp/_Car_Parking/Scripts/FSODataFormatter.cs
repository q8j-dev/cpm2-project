using CPM.Database;
using MemoryPack;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class FSODataFormatter : MemoryPackFormatter<FsoData>
	{
		public class MemoryPackCustomFormatterAttributeFsoData : MemoryPackCustomFormatterAttribute<FsoData>
		{
			public override IMemoryPackFormatter<FsoData> GetFormatter()
			{
				return new FSODataFormatter();
			}
		}

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FsoData value)
		{
			writer.WriteValue(in value.ModelIndex);
			writer.WriteValue(in value.PositionIndex);
			writer.WriteValue(in value.LeftColor);
			writer.WriteValue(in value.RightColor);
		}

		public override void Deserialize(ref MemoryPackReader reader, ref FsoData value)
		{
			value = default;
			value.ModelIndex = reader.ReadValue<int>();
			value.PositionIndex = reader.ReadValue<int>();
			value.LeftColor = reader.ReadValue<Color>();
			value.RightColor = reader.ReadValue<Color>();
		}
	}
}
