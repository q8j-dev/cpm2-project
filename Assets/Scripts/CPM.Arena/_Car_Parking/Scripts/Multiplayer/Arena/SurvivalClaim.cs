using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct SurvivalClaim
	{
		public const string Key = "orE";

		public readonly int Actor;

		public readonly double Elapsed;

		public SurvivalClaim(int actor, double elapsed)
		{
			Actor = actor;
			Elapsed = elapsed;
		}

		public static object Encode(double elapsed)
		{
			return Math.Max(0.0, elapsed);
		}

		public static bool TryParse(object raw, out double elapsed)
		{
			if (raw is double num && !double.IsNaN(num))
			{
				elapsed = num;
				return true;
			}
			elapsed = 0.0;
			return false;
		}
	}
}
