using System;
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public struct CarPaintColor : IMemoryPackable<CarPaintColor>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CarPaintColorFormatter : MemoryPackFormatter<CarPaintColor>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarPaintColor value)
			{
				CarPaintColor.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CarPaintColor value)
			{
				CarPaintColor.Deserialize(ref reader, ref value);
			}
		}

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color Color;

		public CarMatType CarMatType;

		public float IridescenceThickness;

		public float IridescencePower;

		private static readonly IMemoryPackFormatter<Color> __ColorFormatter;

		public static CarPaintColor Default => new CarPaintColor
		{
			CarMatType = CarMatType.Glossy,
			Color = Color.white,
			IridescenceThickness = 0f,
			IridescencePower = 0f
		};

		public static CarPaintColor Empty => new CarPaintColor
		{
			CarMatType = CarMatType.Glossy,
			Color = (Vector3.one * -2f).ToColor(),
			IridescenceThickness = 0f,
			IridescencePower = 0f
		};

		public CarPaintColor(Color color)
		{
			CarMatType = CarMatType.Glossy;
			Color = color;
			IridescenceThickness = 0f;
			IridescencePower = 0f;
		}

		public CarPaintColor Clone()
		{
			CarPaintColor result = new CarPaintColor(Color);
			result.CarMatType = CarMatType;
			result.IridescenceThickness = IridescenceThickness;
			result.IridescencePower = IridescencePower;
			return result;
		}

		public override string ToString()
		{
			return $"c={Color} type={CarMatType} t={IridescenceThickness} p={IridescencePower}";
		}

		static CarPaintColor()
		{
			__ColorFormatter = typeof(CarPaintColor).GetField("Color", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CarPaintColor>())
			{
				MemoryPackFormatterProvider.Register(new CarPaintColorFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarPaintColor[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CarPaintColor>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CarMatType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<CarMatType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CarPaintColor value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanaged(in value);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CarPaintColor value)
		{
			reader.ReadUnmanaged<CarPaintColor>(out value);
		}
	}
}
