#nullable enable annotations
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace CPM.Database
{
	[MemoryPackable(GenerateType.Object)]
	public class BodyKitColor : IMemoryPackable<BodyKitColor>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class BodyKitColorFormatter : MemoryPackFormatter<BodyKitColor>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BodyKitColor value)
			{
				BodyKitColor.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref BodyKitColor value)
			{
				BodyKitColor.Deserialize(ref reader, ref value);
			}
		}

		public BodyKitType BodyKitName;

		[CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor]
		public CarPaintColor Color;

		private static readonly IMemoryPackFormatter<CarPaintColor> __ColorFormatter;

		public BodyKitColor Clone()
		{
			return new BodyKitColor
			{
				BodyKitName = BodyKitName,
				Color = Color.Clone()
			};
		}

		static BodyKitColor()
		{
			__ColorFormatter = typeof(BodyKitColor).GetField("Color", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<BodyKitColor>())
			{
				MemoryPackFormatterProvider.Register(new BodyKitColorFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<BodyKitColor[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<BodyKitColor>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<BodyKitType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<BodyKitType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BodyKitColor? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(2, in value.BodyKitName);
			writer.WriteValueWithFormatter(__ColorFormatter, in value.Color);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref BodyKitColor? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			BodyKitType value2;
			CarPaintColor value3;
			if (memberCount == 2)
			{
				if (value != null)
				{
					value2 = value.BodyKitName;
					value3 = value.Color;
					reader.ReadUnmanaged<BodyKitType>(out value2);
					reader.ReadValueWithFormatter(__ColorFormatter, ref value3);
					goto IL_00b0;
				}
				reader.ReadUnmanaged<BodyKitType>(out value2);
				value3 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__ColorFormatter);
			}
			else
			{
				if (memberCount > 2)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BodyKitColor), 2, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = BodyKitType.Spoiler;
					value3 = default;
				}
				else
				{
					value2 = value.BodyKitName;
					value3 = value.Color;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<BodyKitType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadValueWithFormatter(__ColorFormatter, ref value3);
						_ = 2;
					}
				}
				if (value != null)
				{
					goto IL_00b0;
				}
			}
			value = new BodyKitColor
			{
				BodyKitName = value2,
				Color = value3
			};
			return;
			IL_00b0:
			value.BodyKitName = value2;
			value.Color = value3;
		}
	}
}
