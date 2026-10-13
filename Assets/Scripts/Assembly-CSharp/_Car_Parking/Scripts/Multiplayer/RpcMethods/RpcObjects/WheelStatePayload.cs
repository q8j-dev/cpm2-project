using System.Globalization;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public static class WheelStatePayload
	{
		public struct Data
		{
			public float FrontLeftSpring;

			public float FrontRightSpring;

			public float RearLeftSpring;

			public float RearRightSpring;

			public int FrontLeftSurface;

			public int FrontRightSurface;

			public int RearLeftSurface;

			public int RearRightSurface;
		}

		public static string Pack(float fl, float fr, float rl, float rr, int sfl, int sfr, int srl, int srr)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:F2}?{1:F2}?{2:F2}?{3:F2}?{4}?{5}?{6}?{7}", fl, fr, rl, rr, sfl, sfr, srl, srr);
		}

		public static bool TryParse(string raw, out Data payload)
		{
			payload = default;
			if (string.IsNullOrEmpty(raw))
			{
				return false;
			}
			string[] array = raw.Split('?');
			if (array.Length < 8)
			{
				return false;
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (!float.TryParse(array[0], NumberStyles.Float, invariantCulture, out payload.FrontLeftSpring))
			{
				return false;
			}
			if (!float.TryParse(array[1], NumberStyles.Float, invariantCulture, out payload.FrontRightSpring))
			{
				return false;
			}
			if (!float.TryParse(array[2], NumberStyles.Float, invariantCulture, out payload.RearLeftSpring))
			{
				return false;
			}
			if (!float.TryParse(array[3], NumberStyles.Float, invariantCulture, out payload.RearRightSpring))
			{
				return false;
			}
			if (!int.TryParse(array[4], NumberStyles.Integer, invariantCulture, out payload.FrontLeftSurface))
			{
				return false;
			}
			if (!int.TryParse(array[5], NumberStyles.Integer, invariantCulture, out payload.FrontRightSurface))
			{
				return false;
			}
			if (!int.TryParse(array[6], NumberStyles.Integer, invariantCulture, out payload.RearLeftSurface))
			{
				return false;
			}
			if (!int.TryParse(array[7], NumberStyles.Integer, invariantCulture, out payload.RearRightSurface))
			{
				return false;
			}
			return true;
		}
	}
}
