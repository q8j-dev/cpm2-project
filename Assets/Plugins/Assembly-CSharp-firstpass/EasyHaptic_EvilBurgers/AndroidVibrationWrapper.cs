namespace EasyHaptic_EvilBurgers
{
	public sealed class AndroidVibrationWrapper
	{
		public void Initialize()
		{
		}

		public void PlayHaptic(EVibrationType type)
		{
		}

		public void AndroidOneShotVibration(long milliseconds, int amplitude)
		{
		}

		public void AndroidWaveformVibration(long[] millisecondsPattern, int[] amplitudesPattern, int repeat = -1)
		{
		}

		private void InitMainActivity()
		{
		}

		private void InitVibration()
		{
		}

		public static int GetApiLevel()
		{
			return -1;
		}

		public void StopVibration()
		{
		}

		public bool HasAmplitudeControl()
		{
			return false;
		}

		public bool HasVibrator()
		{
			return false;
		}
	}
}
