using System;
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;

namespace CPM.Database
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public class Colors : ICloneable, IMemoryPackable<Colors>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ColorsFormatter : MemoryPackFormatter<Colors>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref Colors value)
			{
				Colors.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref Colors value)
			{
				Colors.Deserialize(ref reader, ref value);
			}
		}

		[CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor]
		public CarPaintColor CarColor;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color DirtColor;

		public float DirtAmount;

		[ColorSerializableWithAlphaFormatter.MemoryPackCustomFormatterAttributeColorSerializableWithAlpha]
		public ColorSerializableWithAlpha WindowColor;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color WindowSpecularColor;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color DiodColor = Color.white;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color SmokeColor = Color.white;

		public BodyKitsColors BodyKits = new BodyKitsColors();

		private static readonly IMemoryPackFormatter<CarPaintColor> __CarColorFormatter;

		private static readonly IMemoryPackFormatter<Color> __DirtColorFormatter;

		private static readonly IMemoryPackFormatter<ColorSerializableWithAlpha> __WindowColorFormatter;

		private static readonly IMemoryPackFormatter<Color> __WindowSpecularColorFormatter;

		private static readonly IMemoryPackFormatter<Color> __DiodColorFormatter;

		private static readonly IMemoryPackFormatter<Color> __SmokeColorFormatter;

		public object Clone()
		{
			Colors obj = MemberwiseClone() as Colors;
			obj.BodyKits = BodyKits.Clone();
			return obj;
		}

		static Colors()
		{
			__CarColorFormatter = typeof(Colors).GetField("CarColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
			__DirtColorFormatter = typeof(Colors).GetField("DirtColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			__WindowColorFormatter = typeof(Colors).GetField("WindowColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorSerializableWithAlphaFormatter.MemoryPackCustomFormatterAttributeColorSerializableWithAlpha>().GetFormatter();
			__WindowSpecularColorFormatter = typeof(Colors).GetField("WindowSpecularColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			__DiodColorFormatter = typeof(Colors).GetField("DiodColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			__SmokeColorFormatter = typeof(Colors).GetField("SmokeColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<Colors>())
			{
				MemoryPackFormatterProvider.Register(new ColorsFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Colors[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<Colors>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref Colors? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(8);
			writer.WriteValueWithFormatter(__CarColorFormatter, in value.CarColor);
			writer.WriteValueWithFormatter(__DirtColorFormatter, in value.DirtColor);
			writer.WriteUnmanaged(in value.DirtAmount);
			writer.WriteValueWithFormatter(__WindowColorFormatter, in value.WindowColor);
			writer.WriteValueWithFormatter(__WindowSpecularColorFormatter, in value.WindowSpecularColor);
			writer.WriteValueWithFormatter(__DiodColorFormatter, in value.DiodColor);
			writer.WriteValueWithFormatter(__SmokeColorFormatter, in value.SmokeColor);
			writer.WritePackable(in value.BodyKits);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref Colors? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			CarPaintColor value2;
			Color value3;
			float value4;
			ColorSerializableWithAlpha value5;
			Color value6;
			Color value7;
			Color value8;
			BodyKitsColors value9;
			if (memberCount == 8)
			{
				if (value != null)
				{
					value2 = value.CarColor;
					value3 = value.DirtColor;
					value4 = value.DirtAmount;
					value5 = value.WindowColor;
					value6 = value.WindowSpecularColor;
					value7 = value.DiodColor;
					value8 = value.SmokeColor;
					value9 = value.BodyKits;
					reader.ReadValueWithFormatter(__CarColorFormatter, ref value2);
					reader.ReadValueWithFormatter(__DirtColorFormatter, ref value3);
					reader.ReadUnmanaged<float>(out value4);
					reader.ReadValueWithFormatter(__WindowColorFormatter, ref value5);
					reader.ReadValueWithFormatter(__WindowSpecularColorFormatter, ref value6);
					reader.ReadValueWithFormatter(__DiodColorFormatter, ref value7);
					reader.ReadValueWithFormatter(__SmokeColorFormatter, ref value8);
					reader.ReadPackable(ref value9);
					goto IL_0241;
				}
				value2 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__CarColorFormatter);
				value3 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Color>, Color>(__DirtColorFormatter);
				reader.ReadUnmanaged<float>(out value4);
				value5 = reader.ReadValueWithFormatter<IMemoryPackFormatter<ColorSerializableWithAlpha>, ColorSerializableWithAlpha>(__WindowColorFormatter);
				value6 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Color>, Color>(__WindowSpecularColorFormatter);
				value7 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Color>, Color>(__DiodColorFormatter);
				value8 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Color>, Color>(__SmokeColorFormatter);
				value9 = reader.ReadPackable<BodyKitsColors>();
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(Colors), 8, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = default;
					value3 = default;
					value4 = 0f;
					value5 = default;
					value6 = default;
					value7 = default;
					value8 = default;
					value9 = null;
				}
				else
				{
					value2 = value.CarColor;
					value3 = value.DirtColor;
					value4 = value.DirtAmount;
					value5 = value.WindowColor;
					value6 = value.WindowSpecularColor;
					value7 = value.DiodColor;
					value8 = value.SmokeColor;
					value9 = value.BodyKits;
				}
				if (memberCount != 0)
				{
					reader.ReadValueWithFormatter(__CarColorFormatter, ref value2);
					if (memberCount != 1)
					{
						reader.ReadValueWithFormatter(__DirtColorFormatter, ref value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<float>(out value4);
							if (memberCount != 3)
							{
								reader.ReadValueWithFormatter(__WindowColorFormatter, ref value5);
								if (memberCount != 4)
								{
									reader.ReadValueWithFormatter(__WindowSpecularColorFormatter, ref value6);
									if (memberCount != 5)
									{
										reader.ReadValueWithFormatter(__DiodColorFormatter, ref value7);
										if (memberCount != 6)
										{
											reader.ReadValueWithFormatter(__SmokeColorFormatter, ref value8);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value9);
												_ = 8;
											}
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0241;
				}
			}
			value = new Colors
			{
				CarColor = value2,
				DirtColor = value3,
				DirtAmount = value4,
				WindowColor = value5,
				WindowSpecularColor = value6,
				DiodColor = value7,
				SmokeColor = value8,
				BodyKits = value9
			};
			return;
			IL_0241:
			value.CarColor = value2;
			value.DirtColor = value3;
			value.DirtAmount = value4;
			value.WindowColor = value5;
			value.WindowSpecularColor = value6;
			value.DiodColor = value7;
			value.SmokeColor = value8;
			value.BodyKits = value9;
		}
	}
}
