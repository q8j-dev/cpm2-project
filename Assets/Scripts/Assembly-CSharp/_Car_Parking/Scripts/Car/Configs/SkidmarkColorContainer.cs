using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Configs
{
	public static class SkidmarkColorContainer
	{
		private static readonly Dictionary<Surface, SkidmarkSurfaceColor> SurfaceEntries = new Dictionary<Surface, SkidmarkSurfaceColor>();

		private static bool _ready;

		public static void Construct(SkidmarkColorConfigSO config)
		{
			if (!config)
			{
				Debug.LogError("[SkidmarkColorContainer] SkidmarkColorConfigSO is null. Все скидмарки будут чёрными.");
				return;
			}
			SurfaceEntries.Clear();
			foreach (SkidmarkSurfaceColor entry in config.Entries)
			{
				SurfaceEntries[entry.Surface] = entry;
			}
			_ready = true;
		}

		public static Color GetColor(Surface surface)
		{
			if (!_ready)
			{
				Debug.LogWarning("[SkidmarkColorContainer] GetColor called before Construct or config is missing.");
				return Color.black;
			}
			if (!SurfaceEntries.TryGetValue(surface, out var value))
			{
				return Color.black;
			}
			return value.Color;
		}

		public static float GetDigOpacity(Surface surface)
		{
			if (!_ready || !SurfaceEntries.TryGetValue(surface, out var value))
			{
				return 0f;
			}
			return value.DigOpacity;
		}

		public static float GetSlipFloor(Surface surface)
		{
			if (!_ready || !SurfaceEntries.TryGetValue(surface, out var value))
			{
				return 0f;
			}
			return value.SlipFloor;
		}

		public static float GetSlipCeiling(Surface surface)
		{
			if (!_ready || !SurfaceEntries.TryGetValue(surface, out var value))
			{
				return 1f;
			}
			return value.SlipCeiling;
		}
	}
}
