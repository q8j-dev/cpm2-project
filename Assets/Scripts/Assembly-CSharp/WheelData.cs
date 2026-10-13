#nullable enable annotations
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using CPMEngine;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;

[GenerateTypeScript]
[MemoryPackable(GenerateType.Object)]
public class WheelData : ICloneable, IMemoryPackable<WheelData>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class WheelDataFormatter : MemoryPackFormatter<WheelData>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WheelData value)
		{
			WheelData.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref WheelData value)
		{
			WheelData.Deserialize(ref reader, ref value);
		}
	}

	[Header("Brakes")]
	public BrakesData BrakesData;

	[Header("Type and model")]
	[Range(0.5f, 11f)]
	[Tooltip("Tyre pressure in bar")]
	public float Pressure = 2.2f;

	public TyreType tyreType;

	public int RimModelId;

	[Header("Size")]
	public WheelDiameter wheelSize = WheelDiameter.R13;

	public TyreWidth tyreWidth = TyreWidth.W155;

	public TyreProfile tyreProfile = TyreProfile.P75;

	public RimWidth rimWidth = RimWidth.W4;

	public const float MaxSpacerWidthMm = 150f;

	[Range(0f, 150f)]
	[Header("Spacers")]
	public float SpacerWidth;

	[CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor]
	[Header("Colors")]
	public CarPaintColor Color1 = CarPaintColor.Default;

	[CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor]
	public CarPaintColor Color2 = CarPaintColor.Default;

	private static readonly IMemoryPackFormatter<CarPaintColor> __Color1Formatter;

	private static readonly IMemoryPackFormatter<CarPaintColor> __Color2Formatter;

	[JsonIgnore]
	[MemoryPackIgnore]
	public string GetTyreName => $"{tyreWidth}/{tyreProfile}ZR{wheelSize} {GetTyreType()}";

	public object Clone()
	{
		return MemberwiseClone();
	}

	private float GetDiameter()
	{
		return Utils.Inch2Meter((float)wheelSize);
	}

	private float GetWidth()
	{
		return Utils.Mm2Meter(tyreWidth);
	}

	private float GetOptimalPressure()
	{
		return Utils.Bar2kPa(Pressure);
	}

	private float GetProfile()
	{
		return Utils.Mm2Meter((float)tyreWidth / 100f * (float)tyreProfile);
	}

	public float GetWheelRadius()
	{
		return GetDiameter() * 0.5f + GetProfile();
	}

	public static float CalculateWheelRadius(WheelDiameter wheelSize, TyreWidth tyreWidth, TyreProfile tyreProfile)
	{
		float num = Utils.Inch2Meter((float)wheelSize);
		float num2 = Utils.Mm2Meter((float)tyreWidth / 100f * (float)tyreProfile);
		return num * 0.5f + num2;
	}

	public static List<string> Sanitize(WheelData data)
	{
		List<string> list = new List<string>();
		if (data == null)
		{
			return list;
		}
		if (!Enum.IsDefined(typeof(TyreType), data.tyreType))
		{
			list.Add($"tyreType: {(int)data.tyreType} → {TyreType.Touring} (undefined enum)");
			data.tyreType = TyreType.Touring;
		}
		if (!float.IsFinite(data.Pressure))
		{
			float pressure = data.Pressure;
			data.Pressure = TyreTypeRanges.PressureFloor(data.tyreType);
			list.Add($"Pressure: {pressure} → {data.Pressure:0.00} bar (non-finite)");
		}
		if (!Enum.IsDefined(typeof(WheelDiameter), data.wheelSize))
		{
			list.Add($"wheelSize: {(int)data.wheelSize} → R13 (undefined enum)");
			data.wheelSize = WheelDiameter.R13;
		}
		if (!Enum.IsDefined(typeof(TyreWidth), data.tyreWidth))
		{
			list.Add($"tyreWidth: {(int)data.tyreWidth} → 155 (undefined enum)");
			data.tyreWidth = TyreWidth.W155;
		}
		if (!Enum.IsDefined(typeof(TyreProfile), data.tyreProfile))
		{
			list.Add($"tyreProfile: {(int)data.tyreProfile} → 75 (undefined enum)");
			data.tyreProfile = TyreProfile.P75;
		}
		if (!Enum.IsDefined(typeof(RimWidth), data.rimWidth))
		{
			list.Add($"rimWidth: {(int)data.rimWidth} → 40 (undefined enum)");
			data.rimWidth = RimWidth.W4;
		}
		if (!float.IsFinite(data.SpacerWidth) || data.SpacerWidth < 0f)
		{
			list.Add($"SpacerWidth: {data.SpacerWidth} → 0 (non-finite or negative)");
			data.SpacerWidth = 0f;
		}
		else if (data.SpacerWidth > 150f)
		{
			list.Add($"SpacerWidth: {data.SpacerWidth:F1} → {150f:F0} (above max)");
			data.SpacerWidth = 150f;
		}
		return list;
	}

	public static List<string> ClampToLimits(WheelData data, float maxRadius, int brakeDiscMinInch, bool isF1Car)
	{
		List<string> list = new List<string>();
		if (data == null)
		{
			return list;
		}
		WheelDiameter wheelDiameter = data.wheelSize;
		RimWidth rimWidth = data.rimWidth;
		TyreWidth tyreWidth = data.tyreWidth;
		TyreProfile tyreProfile = data.tyreProfile;
		float spacerWidth = data.SpacerWidth;
		TyreType tyreType = data.tyreType;
		float b = data.BrakesData?.Diameter ?? 0f;
		float pressure = data.Pressure;
		list.AddRange(Sanitize(data));
		if (data.BrakesData != null)
		{
			float diameter = data.BrakesData.Diameter;
			if (data.BrakesData.ClampDiameterToWheelSize((int)data.wheelSize))
			{
				list.Add($"brakeDiameter: {diameter:F0}mm → {data.BrakesData.Diameter:F0}mm (out of [{180f:F0}, {500f:F0}] or > 95% of R{(int)data.wheelSize})");
			}
		}
		if (!isF1Car && data.tyreWidth == TyreWidth.W455)
		{
			TyreWidth tyreWidth2 = data.tyreWidth;
			data.tyreWidth = TyreWidth.W445;
			list.Add($"tyreWidth: {(int)tyreWidth2} → {(int)data.tyreWidth} (W455 is F1-only)");
		}
		if (!isF1Car)
		{
			TyreType tyreType2 = data.tyreType;
			WheelDiameter wheelDiameter2 = data.wheelSize;
			data.wheelSize = TyreTypeRanges.ClampToRange(tyreType2, data.wheelSize);
			if (wheelDiameter2 != data.wheelSize)
			{
				list.Add($"wheelSize: R{(int)wheelDiameter2} → R{(int)data.wheelSize} (out of {tyreType2} range)");
			}
			RimWidth rimWidth2 = data.rimWidth;
			data.rimWidth = TyreTypeRanges.ClampToRange(tyreType2, data.rimWidth);
			if (rimWidth2 != data.rimWidth)
			{
				list.Add($"rimWidth: {(int)rimWidth2} → {(int)data.rimWidth} (out of {tyreType2} range)");
			}
			TyreWidth tyreWidth3 = data.tyreWidth;
			data.tyreWidth = TyreTypeRanges.ClampToRange(tyreType2, data.tyreWidth);
			if (tyreWidth3 != data.tyreWidth)
			{
				list.Add($"tyreWidth: {(int)tyreWidth3} → {(int)data.tyreWidth} (out of {tyreType2} range)");
			}
			TyreWidth tyreWidth4 = data.tyreWidth;
			data.tyreWidth = TyreTypeRanges.ClampTyreWidthToRim(data.tyreWidth, data.rimWidth);
			if (tyreWidth4 != data.tyreWidth)
			{
				list.Add($"tyreWidth: {(int)tyreWidth4} → {(int)data.tyreWidth} (out of rim {(float)data.rimWidth / 10f:0.0}\" fitment envelope)");
			}
			TyreProfile tyreProfile2 = data.tyreProfile;
			data.tyreProfile = TyreTypeRanges.ClampToRange(tyreType2, data.tyreProfile, data.wheelSize);
			if (tyreProfile2 != data.tyreProfile)
			{
				list.Add($"tyreProfile: P{(int)tyreProfile2} → P{(int)data.tyreProfile} (out of {tyreType2} range or below R{(int)data.wheelSize} floor)");
			}
			float pressure2 = data.Pressure;
			data.Pressure = TyreTypeRanges.ClampPressureToType(tyreType2, data.Pressure);
			if (!Mathf.Approximately(pressure2, data.Pressure))
			{
				list.Add($"Pressure: {pressure2:0.00} → {data.Pressure:0.00} bar (out of {tyreType2} range)");
			}
		}
		if (brakeDiscMinInch > 0 && (int)data.wheelSize < brakeDiscMinInch)
		{
			WheelDiameter wheelDiameter3 = data.wheelSize;
			WheelDiameter wheelDiameter4 = data.wheelSize;
			int num = int.MaxValue;
			WheelDiameter[] allDiameters = TyreTypeRanges.AllDiameters;
			foreach (WheelDiameter wheelDiameter5 in allDiameters)
			{
				int num2 = (int)wheelDiameter5;
				if (num2 >= brakeDiscMinInch && num2 < num)
				{
					wheelDiameter4 = wheelDiameter5;
					num = num2;
				}
			}
			if (num == int.MaxValue)
			{
				allDiameters = TyreTypeRanges.AllDiameters;
				foreach (WheelDiameter wheelDiameter6 in allDiameters)
				{
					if (wheelDiameter6 > wheelDiameter4)
					{
						wheelDiameter4 = wheelDiameter6;
					}
				}
			}
			if (wheelDiameter3 != wheelDiameter4)
			{
				data.wheelSize = wheelDiameter4;
				list.Add($"wheelSize: R{(int)wheelDiameter3} → R{(int)wheelDiameter4} (brake disc requires min R{brakeDiscMinInch})");
			}
		}
		if (maxRadius > 0f && !float.IsPositiveInfinity(maxRadius) && data.GetWheelRadius() > maxRadius)
		{
			WheelDiameter wheelDiameter7 = data.wheelSize;
			TyreWidth tyreWidth5 = data.tyreWidth;
			TyreProfile tyreProfile3 = data.tyreProfile;
			if (ClampToRadius(data, maxRadius))
			{
				list.Add($"radius: R{(int)wheelDiameter7}/{(int)tyreWidth5}/P{(int)tyreProfile3} → " + $"R{(int)data.wheelSize}/{(int)data.tyreWidth}/P{(int)data.tyreProfile} " + $"(exceeded MaxWheelRadius={maxRadius:F3}m)");
			}
		}
		float a = data.BrakesData?.Diameter ?? 0f;
		if (data.wheelSize == wheelDiameter && data.rimWidth == rimWidth && data.tyreWidth == tyreWidth && data.tyreProfile == tyreProfile && data.tyreType == tyreType && Mathf.Approximately(data.SpacerWidth, spacerWidth) && Mathf.Approximately(a, b) && Mathf.Approximately(data.Pressure, pressure))
		{
			list.Clear();
		}
		return list;
	}

	public static bool ClampToRadius(WheelData data, float maxRadius)
	{
		if (data == null)
		{
			return false;
		}
		if (maxRadius <= 0f || float.IsPositiveInfinity(maxRadius))
		{
			return false;
		}
		if (data.GetWheelRadius() <= maxRadius)
		{
			return false;
		}
		bool result = false;
		TyreProfile[] profilesDescending = TyreTypeRanges.ProfilesDescending;
		foreach (TyreProfile tyreProfile in profilesDescending)
		{
			if (tyreProfile < data.tyreProfile)
			{
				data.tyreProfile = tyreProfile;
				result = true;
				if (CalculateWheelRadius(data.wheelSize, data.tyreWidth, data.tyreProfile) <= maxRadius)
				{
					return true;
				}
			}
		}
		TyreWidth[] tyreWidthsDescending = TyreTypeRanges.TyreWidthsDescending;
		foreach (TyreWidth tyreWidth in tyreWidthsDescending)
		{
			if (tyreWidth < data.tyreWidth)
			{
				data.tyreWidth = tyreWidth;
				result = true;
				if (CalculateWheelRadius(data.wheelSize, data.tyreWidth, data.tyreProfile) <= maxRadius)
				{
					return true;
				}
			}
		}
		WheelDiameter[] diametersDescending = TyreTypeRanges.DiametersDescending;
		foreach (WheelDiameter wheelDiameter in diametersDescending)
		{
			if (wheelDiameter < data.wheelSize)
			{
				data.wheelSize = wheelDiameter;
				result = true;
				if (CalculateWheelRadius(data.wheelSize, data.tyreWidth, data.tyreProfile) <= maxRadius)
				{
					return true;
				}
			}
		}
		return result;
	}

	private TyreType GetTyreType()
	{
		return tyreType;
	}

	public void GetFullTyreSpecs(out float radius, out float width, out int prof, out float pressure, out TyreType tyretype)
	{
		radius = GetWheelRadius();
		width = GetWidth();
		tyretype = GetTyreType();
		prof = (int)tyreProfile;
		pressure = GetOptimalPressure();
	}

	private float GetRimWidth()
	{
		return Utils.Inch2Meter((float)rimWidth) / 10f;
	}

	private float GetRimRadius()
	{
		return GetDiameter() * 0.5f;
	}

	static WheelData()
	{
		__Color1Formatter = typeof(WheelData).GetField("Color1", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
		__Color2Formatter = typeof(WheelData).GetField("Color2", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetCustomAttribute<CarPaintColorFormatter.MemoryPackCustomFormatterAttributeCarPaintColor>().GetFormatter();
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<WheelData>())
		{
			MemoryPackFormatterProvider.Register(new WheelDataFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<WheelData[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<WheelData>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WheelData? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(11);
		writer.WritePackable(in value.BrakesData);
		writer.WriteUnmanaged(in value.Pressure, in value.tyreType, in value.RimModelId, in value.wheelSize, in value.tyreWidth, in value.tyreProfile, in value.rimWidth, in value.SpacerWidth);
		writer.WriteValueWithFormatter(__Color1Formatter, in value.Color1);
		writer.WriteValueWithFormatter(__Color2Formatter, in value.Color2);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref WheelData? value)
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
		if (memberCount == 11)
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
				goto IL_0269;
			}
			value2 = reader.ReadPackable<BrakesData>();
			reader.ReadUnmanaged<float, TyreType, int, WheelDiameter, TyreWidth, TyreProfile, RimWidth, float>(out value3, out value4, out value5, out value6, out value7, out value8, out value9, out value10);
			value11 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__Color1Formatter);
			value12 = reader.ReadValueWithFormatter<IMemoryPackFormatter<CarPaintColor>, CarPaintColor>(__Color2Formatter);
		}
		else
		{
			if (memberCount > 11)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(WheelData), 11, memberCount);
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
				goto IL_0269;
			}
		}
		value = new WheelData
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
			Color2 = value12
		};
		return;
		IL_0269:
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
	}
}
