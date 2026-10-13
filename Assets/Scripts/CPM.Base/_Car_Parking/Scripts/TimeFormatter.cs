using System;

namespace _Car_Parking.Scripts
{
	public static class TimeFormatter
	{
		public static TimeSpan RemainingTime(long endUnixSec, long nowUnixSec)
		{
			long num = endUnixSec - nowUnixSec;
			if (num < 0)
			{
				return TimeSpan.Zero;
			}
			double num2 = num;
			TimeSpan maxValue = TimeSpan.MaxValue;
			if (num2 > maxValue.TotalSeconds)
			{
				return TimeSpan.MaxValue;
			}
			return TimeSpan.FromSeconds(num);
		}

		public static string FormatRemainingShort(TimeSpan remaining, in TimeUnitSuffixes suffixes)
		{
			if (remaining.TotalDays >= 1.0)
			{
				return $"{remaining.Days}{suffixes.Day} {remaining.Hours}{suffixes.Hour}";
			}
			if (remaining.TotalHours >= 1.0)
			{
				return $"{remaining.Hours}{suffixes.Hour} {remaining.Minutes}{suffixes.Minute}";
			}
			return $"{remaining.Minutes}{suffixes.Minute} {remaining.Seconds}{suffixes.Second}";
		}

		public static string FormatMinutesSeconds(int totalSeconds)
		{
			if (totalSeconds < 0)
			{
				totalSeconds = 0;
			}
			return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
		}

		public static string FormatCountdownSeconds(float remainingSeconds)
		{
			return FormatMinutesSeconds((remainingSeconds > 0f) ? ((int)Math.Ceiling(remainingSeconds)) : 0);
		}

		public static string FormatElapsed(double seconds)
		{
			if (seconds < 0.0)
			{
				seconds = 0.0;
			}
			int num = (int)(seconds / 60.0);
			double num2 = seconds - (double)(num * 60);
			return $"{num}:{num2:00.0}";
		}

		public static string FormatRemainingFull(TimeSpan remaining, in TimeUnitSuffixes suffixes)
		{
			return $"{remaining.Days}{suffixes.Day} " + $"{remaining.Hours}{suffixes.Hour} " + $"{remaining.Minutes}{suffixes.Minute}";
		}
	}
}
