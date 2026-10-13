using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

public static class TyreTypeRanges
{
	public readonly struct Range
	{
		public readonly WheelDiameter MinDiameter;

		public readonly WheelDiameter MaxDiameter;

		public readonly RimWidth MinRimWidth;

		public readonly RimWidth MaxRimWidth;

		public readonly TyreWidth MinWidth;

		public readonly TyreWidth MaxWidth;

		public readonly TyreProfile MinProfile;

		public readonly TyreProfile MaxProfile;

		public readonly float MinPressure;

		public readonly float MaxPressure;

		public Range(WheelDiameter dMin, WheelDiameter dMax, RimWidth rwMin, RimWidth rwMax, TyreWidth wMin, TyreWidth wMax, TyreProfile pMin, TyreProfile pMax, float pressMin, float pressMax)
		{
			MinDiameter = dMin;
			MaxDiameter = dMax;
			MinRimWidth = rwMin;
			MaxRimWidth = rwMax;
			MinWidth = wMin;
			MaxWidth = wMax;
			MinProfile = pMin;
			MaxProfile = pMax;
			MinPressure = pressMin;
			MaxPressure = pressMax;
		}
	}

	public static readonly WheelDiameter[] AllDiameters = (WheelDiameter[])Enum.GetValues(typeof(WheelDiameter));

	public static readonly TyreWidth[] AllTyreWidths = (TyreWidth[])Enum.GetValues(typeof(TyreWidth));

	public static readonly TyreProfile[] AllTyreProfiles = (TyreProfile[])Enum.GetValues(typeof(TyreProfile));

	public static readonly RimWidth[] AllRimWidths = (RimWidth[])Enum.GetValues(typeof(RimWidth));

	public static readonly TyreType[] AllTyreTypes = (TyreType[])Enum.GetValues(typeof(TyreType));

	public static readonly WheelDiameter[] DiametersDescending = SortDescending(AllDiameters);

	public static readonly TyreWidth[] TyreWidthsDescending = SortDescending(AllTyreWidths);

	public static readonly TyreProfile[] ProfilesDescending = SortDescending(AllTyreProfiles);

	private static readonly Dictionary<TyreType, Range> _ranges = new Dictionary<TyreType, Range>
	{
		{
			TyreType.Touring,
			new Range(WheelDiameter.R13, WheelDiameter.R24, RimWidth.W4, RimWidth.W125, TyreWidth.W155, TyreWidth.W355, TyreProfile.P20, TyreProfile.P80, 1.6f, 9f)
		},
		{
			TyreType.Winter,
			new Range(WheelDiameter.R13, WheelDiameter.R22, RimWidth.W4, RimWidth.W9, TyreWidth.W155, TyreWidth.W315, TyreProfile.P40, TyreProfile.P80, 1.6f, 4f)
		},
		{
			TyreType.SemiSlick,
			new Range(WheelDiameter.R14, WheelDiameter.R24, RimWidth.W55, RimWidth.W135, TyreWidth.W195, TyreWidth.W375, TyreProfile.P25, TyreProfile.P65, 1.4f, 4f)
		},
		{
			TyreType.Slick,
			new Range(WheelDiameter.R13, WheelDiameter.R24, RimWidth.W6, RimWidth.W150, TyreWidth.W195, TyreWidth.W445, TyreProfile.P20, TyreProfile.P80, 1.3f, 3.5f)
		},
		{
			TyreType.Vintage,
			new Range(WheelDiameter.R13, WheelDiameter.R18, RimWidth.W3, RimWidth.W7, TyreWidth.W135, TyreWidth.W215, TyreProfile.P60, TyreProfile.P100, 1.6f, 3.5f)
		},
		{
			TyreType.Offroad,
			new Range(WheelDiameter.R14, WheelDiameter.R26, RimWidth.W5, RimWidth.W13, TyreWidth.W205, TyreWidth.W355, TyreProfile.P55, TyreProfile.P80, 0.8f, 8f)
		},
		{
			TyreType.Buggy,
			new Range(WheelDiameter.R14, WheelDiameter.R18, RimWidth.W5, RimWidth.W10, TyreWidth.W205, TyreWidth.W325, TyreProfile.P60, TyreProfile.P80, 0.7f, 2.5f)
		},
		{
			TyreType.SoftSlick,
			new Range(WheelDiameter.R13, WheelDiameter.R24, RimWidth.W6, RimWidth.W150, TyreWidth.W195, TyreWidth.W445, TyreProfile.P20, TyreProfile.P80, 1.3f, 3.5f)
		},
		{
			TyreType.HardSlick,
			new Range(WheelDiameter.R13, WheelDiameter.R24, RimWidth.W6, RimWidth.W150, TyreWidth.W195, TyreWidth.W445, TyreProfile.P20, TyreProfile.P80, 1.3f, 3.5f)
		},
		{
			TyreType.DragSlick,
			new Range(WheelDiameter.R13, WheelDiameter.R20, RimWidth.W7, RimWidth.W150, TyreWidth.W245, TyreWidth.W445, TyreProfile.P50, TyreProfile.P80, 0.7f, 2f)
		}
	};

	public const float MinTyreRimDeltaInches = -3.5f;

	public const float MaxTyreRimDeltaInches = 3.5f;

	private const float MmPerInch = 25.4f;

	private static T[] SortDescending<T>(T[] src) where T : Enum
	{
		T[] obj = (T[])src.Clone();
		Array.Sort(obj, (T a, T b) => Convert.ToInt32(b).CompareTo(Convert.ToInt32(a)));
		return obj;
	}

	public static Range Get(TyreType type)
	{
		if (!_ranges.TryGetValue(type, out var value))
		{
			return default;
		}
		return value;
	}

	public static bool IsDiameterInRange(TyreType type, WheelDiameter d)
	{
		Range range = Get(type);
		if (d >= range.MinDiameter)
		{
			return d <= range.MaxDiameter;
		}
		return false;
	}

	public static bool IsRimWidthInRange(TyreType type, RimWidth rw)
	{
		Range range = Get(type);
		if (rw >= range.MinRimWidth)
		{
			return rw <= range.MaxRimWidth;
		}
		return false;
	}

	public static bool IsWidthInRange(TyreType type, TyreWidth w)
	{
		Range range = Get(type);
		if (w >= range.MinWidth)
		{
			return w <= range.MaxWidth;
		}
		return false;
	}

	public static bool IsProfileInRange(TyreType type, TyreProfile p)
	{
		Range range = Get(type);
		if (p >= range.MinProfile)
		{
			return p <= range.MaxProfile;
		}
		return false;
	}

	public static TyreProfile MinProfileForDiameter(WheelDiameter d)
	{
		if (d <= WheelDiameter.R16)
		{
			if (d <= WheelDiameter.R14)
			{
				return TyreProfile.P50;
			}
			return TyreProfile.P40;
		}
		if (d <= WheelDiameter.R18)
		{
			return TyreProfile.P30;
		}
		return TyreProfile.P20;
	}

	public static bool IsProfileInRange(TyreType type, TyreProfile p, WheelDiameter d)
	{
		if (!IsProfileInRange(type, p))
		{
			return false;
		}
		return p >= MinProfileForDiameter(d);
	}

	public static float PressureFloor(TyreType type)
	{
		return Get(type).MinPressure;
	}

	public static bool IsPressureInRange(TyreType type, float bar)
	{
		Range range = Get(type);
		if (bar >= range.MinPressure)
		{
			return bar <= range.MaxPressure;
		}
		return false;
	}

	public static float PressureCeiling(TyreType type, float stockBar, float perCarCapBar)
	{
		Range range = Get(type);
		float num = Mathf.Clamp(stockBar, range.MinPressure, range.MaxPressure);
		float b = num * 1.4f;
		float a = range.MaxPressure;
		if (perCarCapBar >= num)
		{
			a = Mathf.Min(a, perCarCapBar);
		}
		return Mathf.Max(num, Mathf.Min(a, b));
	}

	public static float ClampPressureToType(TyreType type, float bar)
	{
		Range range = Get(type);
		return Mathf.Clamp(bar, range.MinPressure, range.MaxPressure);
	}

	public static WheelDiameter ClampToRange(TyreType type, WheelDiameter d)
	{
		Range range = Get(type);
		if (d < range.MinDiameter)
		{
			return range.MinDiameter;
		}
		if (d > range.MaxDiameter)
		{
			return range.MaxDiameter;
		}
		return d;
	}

	public static RimWidth ClampToRange(TyreType type, RimWidth rw)
	{
		Range range = Get(type);
		if (rw < range.MinRimWidth)
		{
			return range.MinRimWidth;
		}
		if (rw > range.MaxRimWidth)
		{
			return range.MaxRimWidth;
		}
		return rw;
	}

	public static TyreWidth ClampToRange(TyreType type, TyreWidth w)
	{
		Range range = Get(type);
		if (w < range.MinWidth)
		{
			return range.MinWidth;
		}
		if (w > range.MaxWidth)
		{
			return range.MaxWidth;
		}
		return w;
	}

	public static TyreProfile ClampToRange(TyreType type, TyreProfile p)
	{
		Range range = Get(type);
		if (p < range.MinProfile)
		{
			return range.MinProfile;
		}
		if (p > range.MaxProfile)
		{
			return range.MaxProfile;
		}
		return p;
	}

	public static TyreProfile ClampToRange(TyreType type, TyreProfile p, WheelDiameter d)
	{
		TyreProfile tyreProfile = ClampToRange(type, p);
		TyreProfile tyreProfile2 = MinProfileForDiameter(d);
		if (tyreProfile >= tyreProfile2)
		{
			return tyreProfile;
		}
		return tyreProfile2;
	}

	public static bool IsTyreWidthCompatibleWithRim(TyreWidth tyre, RimWidth rim)
	{
		float num = (float)tyre / 25.4f;
		float num2 = (float)rim / 10f;
		float num3 = num - num2;
		if (num3 >= -3.5f)
		{
			return num3 <= 3.5f;
		}
		return false;
	}

	public static TyreWidth ClampTyreWidthToRim(TyreWidth tyre, RimWidth rim)
	{
		if (IsTyreWidthCompatibleWithRim(tyre, rim))
		{
			return tyre;
		}
		TyreWidth result = tyre;
		int num = int.MaxValue;
		TyreWidth[] allTyreWidths = AllTyreWidths;
		foreach (TyreWidth tyreWidth in allTyreWidths)
		{
			if (IsTyreWidthCompatibleWithRim(tyreWidth, rim))
			{
				int num2 = Math.Abs(tyreWidth - tyre);
				if (num2 < num)
				{
					num = num2;
					result = tyreWidth;
				}
			}
		}
		return result;
	}
}
