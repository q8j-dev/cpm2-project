using MemoryPack;
using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts
{
	public class CarPaintColorFormatter : MemoryPackFormatter<CarPaintColor>
	{
		public class MemoryPackCustomFormatterAttributeCarPaintColor : MemoryPackCustomFormatterAttribute<CarPaintColor>
		{
			public override IMemoryPackFormatter<CarPaintColor> GetFormatter()
			{
				return new CarPaintColorFormatter();
			}
		}

		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarPaintColor value)
		{
			writer.WriteValue(in value.Color);
			float r = (float)value.CarMatType / 16f;
			writer.WriteValue<Color>(new Color(r, value.IridescencePower, value.IridescenceThickness));
		}

		public override void Deserialize(ref MemoryPackReader reader, ref CarPaintColor value)
		{
			value.Color = reader.ReadValue<Color>();
			Color color = reader.ReadValue<Color>();
			value.CarMatType = (CarMatType)Mathf.Round(color.r * 16f);
			value.IridescencePower = color.g;
			value.IridescenceThickness = color.b;
		}
	}
}
