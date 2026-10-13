#nullable enable annotations
using System;
using System.Buffers;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[Serializable]
[MemoryPackable(GenerateType.Object)]
[GenerateTypeScript]
public class SteerSuspensionData : SuspensionData, IMemoryPackable<SteerSuspensionData>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SteerSuspensionDataFormatter : MemoryPackFormatter<SteerSuspensionData>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SteerSuspensionData value)
		{
			SteerSuspensionData.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SteerSuspensionData value)
		{
			SteerSuspensionData.Deserialize(ref reader, ref value);
		}
	}

	[Range(-12f, 30f)]
	public float Caster = 2f;

	[Range(0f, 100f)]
	public float Ackerman;

	[Range(28f, 75f)]
	public int MaxSteerAngle = 33;

	static SteerSuspensionData()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SteerSuspensionData>())
		{
			MemoryPackFormatterProvider.Register(new SteerSuspensionDataFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SteerSuspensionData[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SteerSuspensionData>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SuspensionType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SuspensionType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SteerSuspensionData? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader(11, in value.SuspensionType, in value.SpringLength, in value.SpringRate, in value.Bump, in value.Rebound, in value.AntiSwayBarForce, in value.Camber, in value.ToeIn, in value.Caster, in value.Ackerman, in value.MaxSteerAngle);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SteerSuspensionData? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		SuspensionType value2;
		float value3;
		float value4;
		float value5;
		float value6;
		float value7;
		float value8;
		float value9;
		float value10;
		float value11;
		int value12;
		if (memberCount == 11)
		{
			if (value != null)
			{
				value2 = value.SuspensionType;
				value3 = value.SpringLength;
				value4 = value.SpringRate;
				value5 = value.Bump;
				value6 = value.Rebound;
				value7 = value.AntiSwayBarForce;
				value8 = value.Camber;
				value9 = value.ToeIn;
				value10 = value.Caster;
				value11 = value.Ackerman;
				value12 = value.MaxSteerAngle;
				reader.ReadUnmanaged<SuspensionType>(out value2);
				reader.ReadUnmanaged<float>(out value3);
				reader.ReadUnmanaged<float>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				reader.ReadUnmanaged<float>(out value7);
				reader.ReadUnmanaged<float>(out value8);
				reader.ReadUnmanaged<float>(out value9);
				reader.ReadUnmanaged<float>(out value10);
				reader.ReadUnmanaged<float>(out value11);
				reader.ReadUnmanaged<int>(out value12);
				goto IL_0249;
			}
			reader.ReadUnmanaged<SuspensionType, float, float, float, float, float, float, float, float, float, int>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9, out value10, out value11, out value12);
		}
		else
		{
			if (memberCount > 11)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SteerSuspensionData), 11, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = SuspensionType.Mcpherson;
				value3 = 0f;
				value4 = 0f;
				value5 = 0f;
				value6 = 0f;
				value7 = 0f;
				value8 = 0f;
				value9 = 0f;
				value10 = 0f;
				value11 = 0f;
				value12 = 0;
			}
			else
			{
				value2 = value.SuspensionType;
				value3 = value.SpringLength;
				value4 = value.SpringRate;
				value5 = value.Bump;
				value6 = value.Rebound;
				value7 = value.AntiSwayBarForce;
				value8 = value.Camber;
				value9 = value.ToeIn;
				value10 = value.Caster;
				value11 = value.Ackerman;
				value12 = value.MaxSteerAngle;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<SuspensionType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<float>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<float>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<float>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<float>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<float>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<float>(out value8);
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<float>(out value9);
											if (memberCount != 8)
											{
												reader.ReadUnmanaged<float>(out value10);
												if (memberCount != 9)
												{
													reader.ReadUnmanaged<float>(out value11);
													if (memberCount != 10)
													{
														reader.ReadUnmanaged<int>(out value12);
														_ = 11;
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
				goto IL_0249;
			}
		}
		value = new SteerSuspensionData
		{
			SuspensionType = value2,
			SpringLength = value3,
			SpringRate = value4,
			Bump = value5,
			Rebound = value6,
			AntiSwayBarForce = value7,
			Camber = value8,
			ToeIn = value9,
			Caster = value10,
			Ackerman = value11,
			MaxSteerAngle = value12
		};
		return;
		IL_0249:
		value.SuspensionType = value2;
		value.SpringLength = value3;
		value.SpringRate = value4;
		value.Bump = value5;
		value.Rebound = value6;
		value.AntiSwayBarForce = value7;
		value.Camber = value8;
		value.ToeIn = value9;
		value.Caster = value10;
		value.Ackerman = value11;
		value.MaxSteerAngle = value12;
	}
}
