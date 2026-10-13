using UnityEngine;

namespace CPMEngine
{
	public static class WeatherPhysics
	{
		private static float _wetness;

		public const float WetEffectThreshold = 0.5f;

		public const float WetCoolingMul = 3.5f;

		public static float Wetness
		{
			get
			{
				return _wetness;
			}
			set
			{
				_wetness = Mathf.Clamp01(value);
			}
		}

		public static bool IsWet => _wetness > 0.001f;

		public static bool IsWetForEffects => _wetness >= 0.5f;

		public static Surface GetWetEquivalent(Surface surface)
		{
			return surface switch
			{
				Surface.Dryasphalt => Surface.Wetasphalt, 
				Surface.OutOfRoad => Surface.Damp, 
				_ => surface, 
			};
		}

		public static float GetWetGripMul(Surface surface)
		{
			return surface switch
			{
				Surface.Grass => 0.6f, 
				Surface.DirtRoad => 0.65f, 
				Surface.Offroad => 0.7f, 
				Surface.Gravel => 0.85f, 
				Surface.Sand => 1.05f, 
				Surface.Wood => 0.6f, 
				Surface.Kerb => 0.7f, 
				Surface.Concrete => 0.75f, 
				Surface.Glue => 0.6f, 
				Surface.Snow => 0.9f, 
				Surface.Winterroad => 0.85f, 
				Surface.Ice => 0.85f, 
				_ => 1f, 
			};
		}

		public static float GetWetRollingFrictionMul(Surface surface)
		{
			return surface switch
			{
				Surface.Dryasphalt => 1.5f, 
				Surface.Concrete => 1.4f, 
				Surface.Kerb => 1.3f, 
				Surface.Wood => 1.1f, 
				Surface.Glue => 1.2f, 
				Surface.OutOfRoad => 2.4f, 
				Surface.DirtRoad => 1.5f, 
				Surface.Offroad => 1.5f, 
				Surface.Grass => 1.4f, 
				Surface.Gravel => 1.3f, 
				Surface.Snow => 1.2f, 
				Surface.Winterroad => 1.1f, 
				Surface.Sand => 0.9f, 
				_ => 1f, 
			};
		}

		public static float ApplyWetRollingFriction(float dryValue, Surface surface)
		{
			if (_wetness <= 0f)
			{
				return dryValue;
			}
			return dryValue * (1f + (GetWetRollingFrictionMul(surface) - 1f) * _wetness);
		}

		public static float GetWetCoolingMul(Surface surface)
		{
			return surface switch
			{
				Surface.Indoor => 1f, 
				Surface.None => 1f, 
				_ => 3.5f, 
			};
		}

		public static float ApplyWetCooling(float dryCoolingRate, Surface surface)
		{
			if (_wetness <= 0f)
			{
				return dryCoolingRate;
			}
			return dryCoolingRate * (1f + (GetWetCoolingMul(surface) - 1f) * _wetness);
		}
	}
}
