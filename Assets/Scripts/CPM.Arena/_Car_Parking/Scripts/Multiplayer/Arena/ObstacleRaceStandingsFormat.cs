using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceStandingsFormat
	{
		public const string Unknown = "—";

		public static string Elapsed(double? seconds)
		{
			if (!seconds.HasValue)
			{
				return "—";
			}
			return TimeFormatter.FormatElapsed(seconds.Value);
		}

		public static string Remaining(float? meters)
		{
			if (!meters.HasValue)
			{
				return "—";
			}
			return $"{Mathf.Max(0f, meters.Value):0} m";
		}

		public static string Gap(float? localMeters, float? otherMeters)
		{
			if (!localMeters.HasValue || !otherMeters.HasValue)
			{
				return "—";
			}
			float num = localMeters.Value - otherMeters.Value;
			string arg = ((num >= 0f) ? "+" : "−");
			return $"{arg}{Mathf.Abs(num):0} m";
		}
	}
}
