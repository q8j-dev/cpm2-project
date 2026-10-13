using MemoryPack;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts
{
	public class ColorFormatter : MemoryPackFormatter<Color>
	{
		public class MemoryPackCustomFormatterAttributeColor : MemoryPackCustomFormatterAttribute<Color>
		{
			public override IMemoryPackFormatter<Color> GetFormatter()
			{
				return new ColorFormatter();
			}
		}

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref Color value)
		{
			writer.WriteValue<uint>(BitMaskColorPacker.PackColor(value));
		}

		public override void Deserialize(ref MemoryPackReader reader, ref Color value)
		{
			value = BitMaskColorPacker.UnpackColor(reader.ReadValue<uint>());
		}
	}
}
