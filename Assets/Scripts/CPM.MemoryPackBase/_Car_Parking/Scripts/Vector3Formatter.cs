using MemoryPack;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class Vector3Formatter : MemoryPackFormatter<Vector3>
	{
		public class MemoryPackCustomFormatterAttributeVector3 : MemoryPackCustomFormatterAttribute<Vector3>
		{
			public override IMemoryPackFormatter<Vector3> GetFormatter()
			{
				return new Vector3Formatter();
			}
		}

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref Vector3 value)
		{
			writer.WriteValue(in value.x);
			writer.WriteValue(in value.y);
			writer.WriteValue(in value.z);
		}

		public override void Deserialize(ref MemoryPackReader reader, ref Vector3 value)
		{
			value.x = reader.ReadValue<float>();
			value.y = reader.ReadValue<float>();
			value.z = reader.ReadValue<float>();
		}
	}
}
