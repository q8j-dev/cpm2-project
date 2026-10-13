#nullable enable annotations
using System;
using System.Buffers;
using System.Reflection;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;

[Serializable]
[GenerateTypeScript]
[MemoryPackable(GenerateType.Object)]
public class RearWheelData : WheelData, IMemoryPackable<RearWheelData>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RearWheelDataFormatter : MemoryPackFormatter<RearWheelData>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RearWheelData value)
		{
			RearWheelData.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RearWheelData value)
		{
			RearWheelData.Deserialize(ref reader, ref value);
		}
	}

	[Header("Suspension")]
	public SuspensionData SuspensionData;

	private static readonly IMemoryPackFormatter<CarPaintColor> __Color1Formatter;

	private static readonly IMemoryPackFormatter<CarPaintColor> __Color2Formatter;

	static RearWheelData()
	{
		__Color1Formatter = typeof(RearWheelData).GetField("Color1", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
		__Color2Formatter = typeof(RearWheelData).GetField("Color2", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RearWheelData>())
		{
			MemoryPackFormatterProvider.Register(new RearWheelDataFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RearWheelData[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RearWheelData>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TyreType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<TyreType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<WheelDiameter>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<WheelDiameter>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TyreWidth>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<TyreWidth>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TyreProfile>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<TyreProfile>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RimWidth>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<RimWidth>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RearWheelData? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(12);
		writer.WritePackable(in value.BrakesData);
		writer.WriteUnmanaged(in value.Pressure, in value.tyreType, in value.RimModelId, in value.wheelSize, in value.tyreWidth, in value.tyreProfile, in value.rimWidth, in value.SpacerWidth);
		writer.WriteValueWithFormatter(__Color1Formatter, in value.Color1);
		writer.WriteValueWithFormatter(__Color2Formatter, in value.Color2);
		writer.WritePackable(in value.SuspensionData);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref RearWheelData? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		BrakesData value2;
		float value3;
		TyreType value4;
		int value5;
		WheelDiameter value6;
		TyreWidth value7;
		TyreProfile value8;
		RimWidth value9;
		float value10;
		CarPaintColor value11;
		CarPaintColor value12;
		SuspensionData value13;
		if (memberCount == 12)
		{
			if (value != null)
			{
				value2 = value.BrakesData;
				value3 = value.Pressure;
				value4 = value.tyreType;
				value5 = value.RimModelId;
				value6 = value.wheelSize;
				value7 = value.tyreWidth;
				value8 = value.tyreProfile;
				value9 = value.rimWidth;
				value10 = value.SpacerWidth;
				value11 = value.Color1;
				value12 = value.Color2;
				value13 = value.SuspensionData;
				reader.ReadPackable(ref value2);
				reader.ReadUnmanaged<float>(out value3);
				reader.ReadUnmanaged<TyreType>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<WheelDiameter>(out value6);
				reader.ReadUnmanaged<TyreWidth>(out value7);
				reader.ReadUnmanaged<TyreProfile>(out value8);
				reader.ReadUnmanaged<RimWidth>(out value9);
				reader.ReadUnmanaged<float>(out value10);
				reader.ReadValueWithFormatter(__Color1Formatter, ref value11);
				reader.ReadValueWithFormatter(__Color2Formatter, ref value12);
				reader.ReadPackable(ref value13);
				goto IL_029e;
			}
			value2 = reader.ReadPackable<BrakesData>();
			reader.ReadUnmanaged<float, TyreType, int, WheelDiameter, TyreWidth, TyreProfile, RimWidth, float>(out value3, out value4, out value5, out value6, out value7, out value8, out value9, out value10);
			value11 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__Color1Formatter);
			value12 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__Color2Formatter);
			value13 = reader.ReadPackable<SuspensionData>();
		}
		else
		{
			if (memberCount > 12)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RearWheelData), 12, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = null;
				value3 = 0f;
				value4 = TyreType.Touring;
				value5 = 0;
				value6 = (WheelDiameter)0;
				value7 = (TyreWidth)0;
				value8 = (TyreProfile)0;
				value9 = (RimWidth)0;
				value10 = 0f;
				value11 = default;
				value12 = default;
				value13 = null;
			}
			else
			{
				value2 = value.BrakesData;
				value3 = value.Pressure;
				value4 = value.tyreType;
				value5 = value.RimModelId;
				value6 = value.wheelSize;
				value7 = value.tyreWidth;
				value8 = value.tyreProfile;
				value9 = value.rimWidth;
				value10 = value.SpacerWidth;
				value11 = value.Color1;
				value12 = value.Color2;
				value13 = value.SuspensionData;
			}
			if (memberCount != 0)
			{
				reader.ReadPackable(ref value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<float>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<TyreType>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<WheelDiameter>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<TyreWidth>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<TyreProfile>(out value8);
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<RimWidth>(out value9);
											if (memberCount != 8)
											{
												reader.ReadUnmanaged<float>(out value10);
												if (memberCount != 9)
												{
													reader.ReadValueWithFormatter(__Color1Formatter, ref value11);
													if (memberCount != 10)
													{
														reader.ReadValueWithFormatter(__Color2Formatter, ref value12);
														if (memberCount != 11)
														{
															reader.ReadPackable(ref value13);
															_ = 12;
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
				}
			}
			if (value != null)
			{
				goto IL_029e;
			}
		}
		value = new RearWheelData
		{
			BrakesData = value2,
			Pressure = value3,
			tyreType = value4,
			RimModelId = value5,
			wheelSize = value6,
			tyreWidth = value7,
			tyreProfile = value8,
			rimWidth = value9,
			SpacerWidth = value10,
			Color1 = value11,
			Color2 = value12,
			SuspensionData = value13
		};
		return;
		IL_029e:
		value.BrakesData = value2;
		value.Pressure = value3;
		value.tyreType = value4;
		value.RimModelId = value5;
		value.wheelSize = value6;
		value.tyreWidth = value7;
		value.tyreProfile = value8;
		value.rimWidth = value9;
		value.SpacerWidth = value10;
		value.Color1 = value11;
		value.Color2 = value12;
		value.SuspensionData = value13;
	}
}
