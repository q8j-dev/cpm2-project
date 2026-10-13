namespace CPM.Ads.Policy
{
	public static class AdTimerReductionRules
	{
		public const double ToleranceSeconds = 1.0;

		public static double ClampReduction(double reductionSeconds, double baseDuration)
		{
			if (double.IsNaN(reductionSeconds) || reductionSeconds < 0.0)
			{
				return 0.0;
			}
			if (double.IsNaN(baseDuration) || baseDuration <= 0.0)
			{
				return 0.0;
			}
			if (!(reductionSeconds > baseDuration))
			{
				return reductionSeconds;
			}
			return baseDuration;
		}

		public static double ExpectedDuration(double baseDuration, double clampedReduction)
		{
			double num = baseDuration - clampedReduction;
			if (!(num < 0.0))
			{
				return num;
			}
			return 0.0;
		}

		public static bool NeedsRepair(double actualDuration, double expectedDuration)
		{
			return actualDuration < expectedDuration - 1.0;
		}
	}
}
