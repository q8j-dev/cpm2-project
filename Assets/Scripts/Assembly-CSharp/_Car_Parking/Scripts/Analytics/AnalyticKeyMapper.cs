using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

namespace _Car_Parking.Scripts.Analytics
{
	public static class AnalyticKeyMapper
	{
		private static readonly Dictionary<CPMGraphicsSettings.ColorCorrectionLut, string> _colorCorrectionLutMapping = new Dictionary<CPMGraphicsSettings.ColorCorrectionLut, string> { 
		{
			CPMGraphicsSettings.ColorCorrectionLut.Off,
			"None"
		} };

		private static readonly Dictionary<CPMGraphicsSettings.RealtimeRefections, string> _realtimeReflectionsMapping = new Dictionary<CPMGraphicsSettings.RealtimeRefections, string>
		{
			{
				CPMGraphicsSettings.RealtimeRefections.Off,
				"False"
			},
			{
				CPMGraphicsSettings.RealtimeRefections.Medium,
				"True"
			}
		};

		private static readonly Dictionary<TonemappingMode, string> _tonemappingModeMapping = new Dictionary<TonemappingMode, string>
		{
			{
				TonemappingMode.None,
				"Off"
			},
			{
				TonemappingMode.ACES,
				"Aces"
			}
		};

		public static string GetMappedValue(CPMGraphicsSettings.ColorCorrectionLut value)
		{
			if (!_colorCorrectionLutMapping.TryGetValue(value, out var value2))
			{
				return value.ToString();
			}
			return value2;
		}

		public static string GetMappedValue(CPMGraphicsSettings.RealtimeRefections value)
		{
			if (!_realtimeReflectionsMapping.TryGetValue(value, out var value2))
			{
				return value.ToString();
			}
			return value2;
		}

		public static string GetMappedValue(TonemappingMode value)
		{
			if (!_tonemappingModeMapping.TryGetValue(value, out var value2))
			{
				return value.ToString();
			}
			return value2;
		}
	}
}
