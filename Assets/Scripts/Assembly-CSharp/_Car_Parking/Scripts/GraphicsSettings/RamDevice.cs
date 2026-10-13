using UnityEngine.Device;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class RamDevice
	{
		public const int LowRam = 4000;

		public const int MediumRam = 6000;

		public const int HighRam = 8000;

		public const int HeavyPreloadRamThreshold = 12288;

		public static T GetValueOnRamSettingsIosOnly<T>(T lowValue, T mediumValue, T highValue)
		{
			return highValue;
		}

		public static T GetValueOnRamSettings<T>(T lowValue, T mediumValue, T highValue)
		{
			if (GetRam() < 4000)
			{
				return lowValue;
			}
			if (GetRam() < 6000)
			{
				return mediumValue;
			}
			return highValue;
		}

		public static bool AllowHeavyPreload()
		{
			return GetRam() > 12288;
		}

		public static int GetRamMb()
		{
			return GetRam();
		}

		private static int GetRam()
		{
			return SystemInfo.systemMemorySize;
		}
	}
}
