using UnityEngine;

namespace EasyHaptic_EvilBurgers
{
	public static class EasyHaptic
	{
		public static VibrationEngine vibrationEngine = new VibrationEngine();

		public static bool hapticOn
		{
			get
			{
				if (PlayerPrefs.GetInt("EasyHapticTurnOn", 1) <= 0)
				{
					return false;
				}
				return true;
			}
			set
			{
				PlayerPrefs.SetInt("EasyHapticTurnOn", value ? 1 : 0);
			}
		}

		public static void Play(EVibrationType vibrationType)
		{
			if (hapticOn)
			{
				vibrationEngine.PlayTyped(vibrationType);
			}
		}

		public static void PlayCustom(CustomVibrationData customVibration)
		{
			if (hapticOn)
			{
				vibrationEngine.PlayCustomVibro(customVibration);
			}
		}
	}
}
