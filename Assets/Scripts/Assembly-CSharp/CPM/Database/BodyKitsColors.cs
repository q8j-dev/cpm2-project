using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public class BodyKitsColors : IMemoryPackable<BodyKitsColors>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class BodyKitsColorsFormatter : MemoryPackFormatter<BodyKitsColors>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BodyKitsColors value)
			{
				BodyKitsColors.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref BodyKitsColors value)
			{
				BodyKitsColors.Deserialize(ref reader, ref value);
			}
		}

		public List<BodyKitColor> Colors = new List<BodyKitColor>();

		public bool TryGet(BodyKitType type, out CarPaintColor color)
		{
			color = CarPaintColor.Empty;
			if (Colors == null)
			{
				return false;
			}
			foreach (BodyKitColor color2 in Colors)
			{
				if (color2.BodyKitName == type)
				{
					color = color2.Color;
					return true;
				}
			}
			return false;
		}

		public CarPaintColor Get(BodyKitType type, CarPaintColor defaultColor)
		{
			if (Colors == null)
			{
				return defaultColor;
			}
			foreach (BodyKitColor color in Colors)
			{
				if (color.BodyKitName == type)
				{
					return color.Color;
				}
			}
			return defaultColor;
		}

		public void Set(BodyKitType bodyKit, CarPaintColor lastColor)
		{
			for (int i = 0; i < Colors.Count; i++)
			{
				if (Colors[i].BodyKitName == bodyKit)
				{
					Colors[i].Color = lastColor.Clone();
					return;
				}
			}
			Colors.Add(new BodyKitColor
			{
				BodyKitName = bodyKit,
				Color = lastColor.Clone()
			});
		}

		public void Remove(BodyKitType bodyKit)
		{
			for (int i = 0; i < Colors.Count; i++)
			{
				if (Colors[i].BodyKitName == bodyKit)
				{
					Colors.RemoveAt(i);
					break;
				}
			}
		}

		public BodyKitsColors Clone()
		{
			BodyKitsColors bodyKitsColors = new BodyKitsColors();
			for (int i = 0; i < Colors.Count; i++)
			{
				bodyKitsColors.Colors.Add(Colors[i].Clone());
			}
			return bodyKitsColors;
		}

		static BodyKitsColors()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<BodyKitsColors>())
			{
				MemoryPackFormatterProvider.Register(new BodyKitsColorsFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<BodyKitsColors[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<BodyKitsColors>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<BodyKitColor>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<BodyKitColor>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BodyKitsColors? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			ListFormatter.SerializePackable(ref writer, value.Colors);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref BodyKitsColors? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			List<BodyKitColor> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.Colors;
					ListFormatter.DeserializePackable(ref reader, ref value2);
					goto IL_006a;
				}
				value2 = ListFormatter.DeserializePackable<BodyKitColor>(ref reader);
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BodyKitsColors), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.Colors : null);
				if (memberCount != 0)
				{
					ListFormatter.DeserializePackable(ref reader, ref value2);
					_ = 1;
				}
				if (value != null)
				{
					goto IL_006a;
				}
			}
			value = new BodyKitsColors
			{
				Colors = value2
			};
			return;
			IL_006a:
			value.Colors = value2;
		}
	}
}
