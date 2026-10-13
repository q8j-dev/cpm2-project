using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class LSDValues
	{
		public LSDType Type;

		public float AccelBias = 1f;

		public float BrakeBias = 1f;

		public float MaxBias = 1f;

		public float TorqueBiasRatio = 1f;

		public float MaxResistance = 30f;

		public float LockPercent = 10f;

		private static readonly Dictionary<LSDType, (float Min, float Max)> LockRangePerType = new Dictionary<LSDType, (float, float)>
		{
			{
				LSDType.Open,
				(0f, 20f)
			},
			{
				LSDType.Torsen,
				(15f, 50f)
			},
			{
				LSDType.Way1,
				(30f, 65f)
			},
			{
				LSDType.Way15,
				(35f, 70f)
			},
			{
				LSDType.Way2,
				(45f, 80f)
			},
			{
				LSDType.Electronic,
				(30f, 75f)
			},
			{
				LSDType.ActiveM,
				(50f, 95f)
			},
			{
				LSDType.Locked,
				(100f, 100f)
			}
		};

		private static readonly Dictionary<LSDType, (float Min, float Max)> AccelBiasRangePerType = new Dictionary<LSDType, (float, float)>
		{
			{
				LSDType.Way1,
				(1.2f, 2f)
			},
			{
				LSDType.Way15,
				(1.2f, 2f)
			},
			{
				LSDType.Way2,
				(1.2f, 2f)
			}
		};

		private static readonly Dictionary<LSDType, (float Min, float Max)> BrakeBiasRangePerType = new Dictionary<LSDType, (float, float)> { 
		{
			LSDType.Way15,
			(1.1f, 1.5f)
		} };

		private static readonly Dictionary<LSDType, (float Min, float Max)> MaxBiasRangePerType = new Dictionary<LSDType, (float, float)>
		{
			{
				LSDType.Electronic,
				(1f, 2f)
			},
			{
				LSDType.ActiveM,
				(1f, 2f)
			}
		};

		private static readonly Dictionary<LSDType, (float Min, float Max)> TorqueBiasRatioRangePerType = new Dictionary<LSDType, (float, float)> { 
		{
			LSDType.Torsen,
			(2f, 6f)
		} };

		private static readonly Dictionary<LSDType, (float Min, float Max)> MaxResistanceRangePerType = new Dictionary<LSDType, (float, float)> { 
		{
			LSDType.ActiveM,
			(20f, 100f)
		} };

		public static (float Min, float Max) GetLockPercentRange(LSDType type)
		{
			if (!LockRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 0f, Max: 100f);
			}
			return value;
		}

		public static float ClampLockPercent(float value, LSDType type)
		{
			var (min, max) = GetLockPercentRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static (float Min, float Max) GetAccelBiasRange(LSDType type)
		{
			if (!AccelBiasRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 1.2f, Max: 2f);
			}
			return value;
		}

		public static (float Min, float Max) GetBrakeBiasRange(LSDType type)
		{
			if (!BrakeBiasRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 1.1f, Max: 1.5f);
			}
			return value;
		}

		public static (float Min, float Max) GetMaxBiasRange(LSDType type)
		{
			if (!MaxBiasRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 1f, Max: 2f);
			}
			return value;
		}

		public static (float Min, float Max) GetTorqueBiasRatioRange(LSDType type)
		{
			if (!TorqueBiasRatioRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 2f, Max: 6f);
			}
			return value;
		}

		public static (float Min, float Max) GetMaxResistanceRange(LSDType type)
		{
			if (!MaxResistanceRangePerType.TryGetValue(type, out (float, float) value))
			{
				return (Min: 20f, Max: 100f);
			}
			return value;
		}

		public static float ClampAccelBias(float value, LSDType type)
		{
			var (min, max) = GetAccelBiasRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static float ClampBrakeBias(float value, LSDType type)
		{
			var (min, max) = GetBrakeBiasRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static float ClampMaxBias(float value, LSDType type)
		{
			var (min, max) = GetMaxBiasRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static float ClampTorqueBiasRatio(float value, LSDType type)
		{
			var (min, max) = GetTorqueBiasRatioRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static float ClampMaxResistance(float value, LSDType type)
		{
			var (min, max) = GetMaxResistanceRange(type);
			return Mathf.Clamp(value, min, max);
		}

		public static float LockPercentFromBias(float bias)
		{
			float num = Mathf.Max(bias, 1f);
			return (num - 1f) / (num + 1f) * 100f;
		}

		public static float BiasFromLockPercent(float lockPercent)
		{
			float num = Mathf.Clamp(lockPercent, 0f, 99f) / 100f;
			return (1f + num) / (1f - num);
		}
	}
}
