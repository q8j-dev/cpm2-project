using CPM.Database;
using MemoryPack;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts
{
	public class ColorSerializableWithAlphaFormatter : MemoryPackFormatter<ColorSerializableWithAlpha>
	{
		public class MemoryPackCustomFormatterAttributeColorSerializableWithAlpha : MemoryPackCustomFormatterAttribute<ColorSerializableWithAlpha>
		{
			public override IMemoryPackFormatter<ColorSerializableWithAlpha> GetFormatter()
			{
				return new ColorSerializableWithAlphaFormatter();
			}
		}

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ColorSerializableWithAlpha value)
		{
			writer.WriteValue<uint>(BitMaskColorPacker.PackColor(value));
		}

		public override void Deserialize(ref MemoryPackReader reader, ref ColorSerializableWithAlpha value)
		{
			value = BitMaskColorPacker.UnpackColor(reader.ReadValue<uint>());
		}
	}
}
