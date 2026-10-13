using System;
using System.Buffers;
using System.Reflection;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts;

namespace CPM.Database
{
	[Serializable]
	[UnityEngine.Scripting.Preserve]
	[MemoryPackable(GenerateType.Object)]
	public class AdditionalData : ICloneable, IMemoryPackable<AdditionalData>, IMemoryPackFormatterRegister
	{
		[MemoryPack.Internal.Preserve]
		private sealed class AdditionalDataFormatter : MemoryPackFormatter<AdditionalData>
		{
			[MemoryPack.Internal.Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdditionalData value)
			{
				AdditionalData.Serialize(ref writer, ref value);
			}

			[MemoryPack.Internal.Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AdditionalData value)
			{
				AdditionalData.Deserialize(ref reader, ref value);
			}
		}

		public int FlagId;

		public int AccessoryId;

		public bool IsPolice;

		public string WheelText;

		public int WheelTextFont;

		public int WheelTextFontStyle;

		[ColorFormatter.MemoryPackCustomFormatterAttributeColor]
		public Color WheelTextColor;

		[FSODataFormatter.MemoryPackCustomFormatterAttributeFsoData]
		public FsoData Fso;

		public PoliceData Police = new PoliceData();

		public PsData AirSuspension = new PsData();

		private static readonly IMemoryPackFormatter<Color> __WheelTextColorFormatter;

		private static readonly IMemoryPackFormatter<FsoData> __FsoFormatter;

		public object Clone()
		{
			return MemberwiseClone();
		}

		static AdditionalData()
		{
			__WheelTextColorFormatter = typeof(AdditionalData).GetField("WheelTextColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<ColorFormatter.MemoryPackCustomFormatterAttributeColor>().GetFormatter();
			__FsoFormatter = typeof(AdditionalData).GetField("Fso", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<FSODataFormatter.MemoryPackCustomFormatterAttributeFsoData>().GetFormatter();
			RegisterFormatter();
		}

		[MemoryPack.Internal.Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AdditionalData>())
			{
				MemoryPackFormatterProvider.Register(new AdditionalDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AdditionalData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AdditionalData>());
			}
		}

		[MemoryPack.Internal.Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdditionalData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(10, in value.FlagId, in value.AccessoryId, in value.IsPolice);
			writer.WriteString(value.WheelText);
			writer.WriteUnmanaged(in value.WheelTextFont, in value.WheelTextFontStyle);
			writer.WriteValueWithFormatter(__WheelTextColorFormatter, in value.WheelTextColor);
			writer.WriteValueWithFormatter(__FsoFormatter, in value.Fso);
			writer.WritePackable(in value.Police);
			writer.WritePackable(in value.AirSuspension);
		}

		[MemoryPack.Internal.Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AdditionalData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			bool value4;
			int value5;
			int value6;
			Color value7;
			FsoData value8;
			PoliceData value9;
			PsData value10;
			string wheelText;
			if (memberCount == 10)
			{
				if (value != null)
				{
					value2 = value.FlagId;
					value3 = value.AccessoryId;
					value4 = value.IsPolice;
					wheelText = value.WheelText;
					value5 = value.WheelTextFont;
					value6 = value.WheelTextFontStyle;
					value7 = value.WheelTextColor;
					value8 = value.Fso;
					value9 = value.Police;
					value10 = value.AirSuspension;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<bool>(out value4);
					wheelText = reader.ReadString();
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadValueWithFormatter(__WheelTextColorFormatter, ref value7);
					reader.ReadValueWithFormatter(__FsoFormatter, ref value8);
					reader.ReadPackable(ref value9);
					reader.ReadPackable(ref value10);
					goto IL_0245;
				}
				reader.ReadUnmanaged<int, int, bool>(out value2, out value3, out value4);
				wheelText = reader.ReadString();
				reader.ReadUnmanaged<int, int>(out value5, out value6);
				value7 = reader.ReadValueWithFormatter<IMemoryPackFormatter<Color>, Color>(__WheelTextColorFormatter);
				value8 = reader.ReadValueWithFormatter<IMemoryPackFormatter<FsoData>, FsoData>(__FsoFormatter);
				value9 = reader.ReadPackable<PoliceData>();
				value10 = reader.ReadPackable<PsData>();
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AdditionalData), 10, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = false;
					wheelText = null;
					value5 = 0;
					value6 = 0;
					value7 = default;
					value8 = default;
					value9 = null;
					value10 = null;
				}
				else
				{
					value2 = value.FlagId;
					value3 = value.AccessoryId;
					value4 = value.IsPolice;
					wheelText = value.WheelText;
					value5 = value.WheelTextFont;
					value6 = value.WheelTextFontStyle;
					value7 = value.WheelTextColor;
					value8 = value.Fso;
					value9 = value.Police;
					value10 = value.AirSuspension;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<bool>(out value4);
							if (memberCount != 3)
							{
								wheelText = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value6);
										if (memberCount != 6)
										{
											reader.ReadValueWithFormatter(__WheelTextColorFormatter, ref value7);
											if (memberCount != 7)
											{
												reader.ReadValueWithFormatter(__FsoFormatter, ref value8);
												if (memberCount != 8)
												{
													reader.ReadPackable(ref value9);
													if (memberCount != 9)
													{
														reader.ReadPackable(ref value10);
														_ = 10;
													}
												}
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
					goto IL_0245;
				}
			}
			value = new AdditionalData
			{
				FlagId = value2,
				AccessoryId = value3,
				IsPolice = value4,
				WheelText = wheelText,
				WheelTextFont = value5,
				WheelTextFontStyle = value6,
				WheelTextColor = value7,
				Fso = value8,
				Police = value9,
				AirSuspension = value10
			};
			return;
			IL_0245:
			value.FlagId = value2;
			value.AccessoryId = value3;
			value.IsPolice = value4;
			value.WheelText = wheelText;
			value.WheelTextFont = value5;
			value.WheelTextFontStyle = value6;
			value.WheelTextColor = value7;
			value.Fso = value8;
			value.Police = value9;
			value.AirSuspension = value10;
		}
	}
}
