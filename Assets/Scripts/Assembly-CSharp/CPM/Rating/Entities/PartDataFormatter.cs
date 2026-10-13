using System;
using MemoryPack;
using _Car_Parking.Scripts.EngineTuning;

namespace CPM.Rating.Entities
{
	public class PartDataFormatter : MemoryPackFormatter<PartData>
	{
		private const float Accuracy = 100f;

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PartData value)
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(2);
			writer.WriteValue(in value.ModelId);
			writer.WriteValue<int>((int)Math.Round(value.Wear * 100f));
		}

		public override void Deserialize(ref MemoryPackReader reader, ref PartData value)
		{
			if (!reader.TryReadObjectHeader(out var _))
			{
				value = null;
				return;
			}
			value = new PartData();
			value.ModelId = reader.ReadValue<int>();
			value.Wear = (float)reader.ReadValue<int>() / 100f;
			value = value.CheckPartData();
		}
	}
}
