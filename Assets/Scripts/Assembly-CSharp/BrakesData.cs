#nullable enable annotations
using System;
using System.Buffers;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[MemoryPackable(GenerateType.Object)]
public class BrakesData : ICloneable, IMemoryPackable<BrakesData>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class BrakesDataFormatter : MemoryPackFormatter<BrakesData>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BrakesData value)
		{
			BrakesData.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref BrakesData value)
		{
			BrakesData.Deserialize(ref reader, ref value);
		}
	}

	private float _thickness = 0.03f;

	private float _mass;

	private float _efficiency = 1f;

	private float _baseDensity = 7800f;

	[Range(180f, 500f)]
	[Tooltip("Diameter in mm")]
	public float Diameter = 280f;

	[Tooltip("Disk material type")]
	public BrakeDiscType DiscType = BrakeDiscType.VENTED;

	[Tooltip("Number of Pistons")]
	[Range(1f, 10f)]
	public int Pistons = 1;

	[Tooltip("Diameter of Pistons in mm")]
	[Range(20f, 48f)]
	public float PistonDiameter = 37f;

	public int CaliperMeshIndex;

	[JsonIgnore]
	private const float HydraulicPressure = 17000000f;

	public const float MaxDiameterMm = 500f;

	public const float MinDiameterMm = 180f;

	[JsonIgnore]
	[MemoryPackIgnore]
	public float GetMass
	{
		get
		{
			CalculateParams();
			float num = (float)Math.PI * Mathf.Pow(GetRadius(), 2f) * _thickness;
			return _baseDensity * num;
		}
	}

	[JsonIgnore]
	[MemoryPackIgnore]
	public BrakeDiscType GetBrakeDiscType => DiscType;

	[MemoryPackIgnore]
	[JsonIgnore]
	public float GetFullBrakeTorque
	{
		get
		{
			float discEfficiency = GetDiscEfficiency(DiscType);
			float num = (float)Math.PI * Mathf.Pow(PistonDiameter / 1000f * 0.5f, 2f) * (float)Pistons;
			float num2 = GetBrakeDiskDiameter() * 0.5f;
			float num3 = 17000000f * num;
			return 2f * num3 * discEfficiency * num2;
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public int GetBrakeDiskTypeIndex()
	{
		return (int)(DiscType - 1);
	}

	public bool ClampToDiscLimits()
	{
		float diameter = Diameter;
		float num = ((diameter < 340f) ? ((diameter < 240f) ? 34f : ((!(diameter < 290f)) ? 42f : 38f)) : ((!(diameter < 390f)) ? 48f : 46f));
		float max = num;
		float num2 = Mathf.Clamp(PistonDiameter, 20f, max);
		diameter = Diameter;
		int a = ((diameter < 340f) ? ((diameter < 240f) ? 2 : ((!(diameter < 290f)) ? 6 : 4)) : ((!(diameter < 390f)) ? 10 : 8));
		int a2 = Mathf.FloorToInt(Diameter * 0.4f / num2) * 2;
		int max2 = Mathf.Clamp(Mathf.Min(a, Mathf.Max(a2, 1)), 1, 10);
		int num3 = Mathf.Clamp(Pistons, 1, max2);
		bool result = !Mathf.Approximately(PistonDiameter, num2) || Pistons != num3;
		PistonDiameter = num2;
		Pistons = num3;
		return result;
	}

	public bool ClampDiameterToWheelSize(int wheelSizeInch)
	{
		float max = Mathf.Min(500f, Mathf.Floor((float)wheelSizeInch * 25.4f * 0.95f));
		float num = Mathf.Clamp(Diameter, 180f, max);
		bool result = !Mathf.Approximately(Diameter, num);
		Diameter = num;
		return result;
	}

	public int GetMinWheelInch()
	{
		return Mathf.CeilToInt(Diameter / 24.13f);
	}

	private void CalculateParams()
	{
		switch (DiscType)
		{
		case BrakeDiscType.SOLID:
			_efficiency = 1f;
			_baseDensity = 7800f;
			_thickness = 0.032f;
			break;
		case BrakeDiscType.VENTED:
			_efficiency = 1.15f;
			_baseDensity = 7800f;
			_thickness = 0.025f;
			break;
		case BrakeDiscType.DRILLED:
			_efficiency = 1.17f;
			_baseDensity = 7800f;
			_thickness = 0.027f;
			break;
		case BrakeDiscType.GROOVED:
			_efficiency = 1.2f;
			_baseDensity = 7800f;
			_thickness = 0.027f;
			break;
		case BrakeDiscType.CARBON:
			_efficiency = 1.23f;
			_baseDensity = 1250f;
			_thickness = 0.05f;
			break;
		case BrakeDiscType.CERAMIC:
			_efficiency = 1.25f;
			_baseDensity = 2600f;
			_thickness = 0.04f;
			break;
		default:
			_efficiency = 1f;
			_baseDensity = 7800f;
			_thickness = 0.03f;
			break;
		}
	}

	private float GetRadius()
	{
		return Diameter * 0.0005f;
	}

	public float GetBrakeDiskDiameter()
	{
		return Diameter * 0.001f;
	}

	private float GetDiscEfficiency(BrakeDiscType type)
	{
		return type switch
		{
			BrakeDiscType.SOLID => 0.42f, 
			BrakeDiscType.VENTED => 0.44f, 
			BrakeDiscType.DRILLED => 0.48f, 
			BrakeDiscType.GROOVED => 0.5f, 
			BrakeDiscType.CARBON => 0.6f, 
			BrakeDiscType.CERAMIC => 0.65f, 
			_ => 0.35f, 
		};
	}

	static BrakesData()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<BrakesData>())
		{
			MemoryPackFormatterProvider.Register(new BrakesDataFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BrakesData[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<BrakesData>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BrakeDiscType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<BrakeDiscType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BrakesData? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader(5, in value.Diameter, in value.DiscType, in value.Pistons, in value.PistonDiameter, in value.CaliperMeshIndex);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref BrakesData? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		float value2;
		BrakeDiscType value3;
		int value4;
		float value5;
		int value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.Diameter;
				value3 = value.DiscType;
				value4 = value.Pistons;
				value5 = value.PistonDiameter;
				value6 = value.CaliperMeshIndex;
				reader.ReadUnmanaged<float>(out value2);
				reader.ReadUnmanaged<BrakeDiscType>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				goto IL_011f;
			}
			reader.ReadUnmanaged<float, BrakeDiscType, int, float, int>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BrakesData), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0f;
				value3 = (BrakeDiscType)0;
				value4 = 0;
				value5 = 0f;
				value6 = 0;
			}
			else
			{
				value2 = value.Diameter;
				value3 = value.DiscType;
				value4 = value.Pistons;
				value5 = value.PistonDiameter;
				value6 = value.CaliperMeshIndex;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<float>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<BrakeDiscType>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<int>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<float>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_011f;
			}
		}
		value = new BrakesData
		{
			Diameter = value2,
			DiscType = value3,
			Pistons = value4,
			PistonDiameter = value5,
			CaliperMeshIndex = value6
		};
		return;
		IL_011f:
		value.Diameter = value2;
		value.DiscType = value3;
		value.Pistons = value4;
		value.PistonDiameter = value5;
		value.CaliperMeshIndex = value6;
	}
}
