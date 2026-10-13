using CPM.Database;
using MemoryPack;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts
{
	public class BodyKitColorFormatter : MemoryPackFormatter<BodyKitColor>
	{
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BodyKitColor value)
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(2);
			int value2 = (int)value.BodyKitName;
			writer.WriteValue(in value2);
			writer.WriteValue(in value.Color);
		}

		public override void Deserialize(ref MemoryPackReader reader, ref BodyKitColor value)
		{
			value = new BodyKitColor();
			if (reader.TryReadObjectHeader(out var _))
			{
				value.BodyKitName = (BodyKitType)reader.ReadValue<int>();
				value.Color = reader.ReadValue<CarPaintColor>();
			}
		}
	}
}
