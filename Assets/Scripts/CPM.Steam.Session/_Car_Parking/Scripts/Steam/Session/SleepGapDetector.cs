using System;

namespace _Car_Parking.Scripts.Steam.Session
{
	public sealed class SleepGapDetector
	{
		public const double DefaultThresholdSeconds = 30.0;

		private readonly double _thresholdSeconds;

		private double? _lastSeconds;

		public SleepGapDetector(double thresholdSeconds)
		{
			if (thresholdSeconds <= 0.0)
			{
				throw new ArgumentOutOfRangeException("thresholdSeconds", thresholdSeconds, "Sleep gap threshold must be positive.");
			}
			_thresholdSeconds = thresholdSeconds;
		}

		public bool Observe(double nowSeconds, out double gapSeconds)
		{
			if (!_lastSeconds.HasValue)
			{
				_lastSeconds = nowSeconds;
				gapSeconds = 0.0;
				return false;
			}
			gapSeconds = nowSeconds - _lastSeconds.Value;
			_lastSeconds = nowSeconds;
			return gapSeconds >= _thresholdSeconds;
		}

		public void Reset()
		{
			_lastSeconds = null;
		}
	}
}
