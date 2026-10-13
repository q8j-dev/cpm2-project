using UnityEngine;

namespace EasyHaptic_EvilBurgers
{
	public sealed class VibrationEngine
	{
		private AndroidVibrationWrapper androidVibrationWrapper = new AndroidVibrationWrapper();

		private IosVibrationWrapper iosVibrationWrapper = new IosVibrationWrapper();

		[RuntimeInitializeOnLoadMethod]
		public void GlobalInitialize()
		{
		}

		public void PlayCustomVibro(CustomVibrationData customVibration)
		{
		}

		public void PlayTyped(EVibrationType type)
		{
		}

		public bool SupportAmplitudeController()
		{
			return false;
		}
	}
}
