using System;
using System.Buffers;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[MemoryPackable(GenerateType.Object)]
[GenerateTypeScript]
public class SuspensionData : ICloneable, IMemoryPackable<SuspensionData>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SuspensionDataFormatter : MemoryPackFormatter<SuspensionData>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SuspensionData value)
		{
			SuspensionData.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SuspensionData value)
		{
			SuspensionData.Deserialize(ref reader, ref value);
		}
	}

	[MemoryPackIgnore]
	[Header("Suspension State")]
	public SuspensionState SuspensionState;

	[Header("Strut Type")]
	public SuspensionType SuspensionType;

	[Header("Spring")]
	[Range(3f, 24f)]
	[Tooltip("Length in inch")]
	public float SpringLength = 10f;

	[Tooltip("Spring Rate in Kgf/mm")]
	[Range(1f, 50f)]
	public float SpringRate = 5f;

	[Header("ShockAbsorber")]
	[Tooltip("Bump in N/m")]
	[Range(800f, 15000f)]
	public float Bump = 2500f;

	[Tooltip("Rebound in N/m")]
	[Range(1000f, 26000f)]
	public float Rebound = 3000f;

	[Range(0f, 70000f)]
	[Tooltip("Force in N")]
	[Header("AntiSwayBar")]
	public float AntiSwayBarForce = 10000f;

	[Range(1f, -15f)]
	public float Camber;

	public float ToeIn = 0.1f;

	public object Clone()
	{
		return MemberwiseClone();
	}

	public void GetShockAbsorberParams(out float2 rate)
	{
		rate = new float2(Bump, Rebound);
	}

	public void GetSpringParams(out float2 specs)
	{
		specs = new float2(Utils.Inch2Meter(SpringLength), Utils.Kgfmm2NM(SpringRate));
	}

	static SuspensionData()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SuspensionData>())
		{
			MemoryPackFormatterProvider.Register(new SuspensionDataFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SuspensionData[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SuspensionData>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SuspensionType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SuspensionType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SuspensionData? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader(8, in value.SuspensionType, in value.SpringLength, in value.SpringRate, in value.Bump, in value.Rebound, in value.AntiSwayBarForce, in value.Camber, in value.ToeIn);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SuspensionData? value)
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
		if (memberCount == 8)
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
				reader.ReadUnmanaged<SuspensionType>(out value2);
				reader.ReadUnmanaged<float>(out value3);
				reader.ReadUnmanaged<float>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				reader.ReadUnmanaged<float>(out value7);
				reader.ReadUnmanaged<float>(out value8);
				reader.ReadUnmanaged<float>(out value9);
				goto IL_01b7;
			}
			reader.ReadUnmanaged<SuspensionType, float, float, float, float, float, float, float>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
		}
		else
		{
			if (memberCount > 8)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SuspensionData), 8, memberCount);
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
				goto IL_01b7;
			}
		}
		value = new SuspensionData
		{
			SuspensionType = value2,
			SpringLength = value3,
			SpringRate = value4,
			Bump = value5,
			Rebound = value6,
			AntiSwayBarForce = value7,
			Camber = value8,
			ToeIn = value9
		};
		return;
		IL_01b7:
		value.SuspensionType = value2;
		value.SpringLength = value3;
		value.SpringRate = value4;
		value.Bump = value5;
		value.Rebound = value6;
		value.AntiSwayBarForce = value7;
		value.Camber = value8;
		value.ToeIn = value9;
	}
}
