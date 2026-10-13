using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class VisualTuningExclude
	{
		private static HashSet<ObscuredInt> _f1ExcludedTuningTypes = new HashSet<ObscuredInt> { 2, 8 };

		public static bool IsF1ExcludedType(VisualTuningTypes type)
		{
			return _f1ExcludedTuningTypes.Contains((int)type);
		}

		public static bool IsDisabledFeatureType(VisualTuningTypes type)
		{
			return type == VisualTuningTypes.FsoStrobeLights;
		}
	}
}
