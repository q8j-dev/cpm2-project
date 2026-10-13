using UnityEngine;

namespace _Car_Parking.Scripts.Car.TachometerCustom
{
	public static class TachometerInterpolator
	{
		private static float Interpolate(TachometerPoint point1, TachometerPoint point2, float x)
		{
			return Mathf.Lerp(point1.Value, point2.Value, Mathf.InverseLerp(point1.RPM, point2.RPM, x));
		}

		public static float Interpolate(TachometerPoint[] points, float x)
		{
			if (x < 0f)
			{
				return 0f;
			}
			for (int i = 0; i < points.Length - 1; i++)
			{
				if (points[i].RPM <= x && x <= points[i + 1].RPM)
				{
					return Interpolate(points[i], points[i + 1], x);
				}
			}
			if (points.Length == 0)
			{
				return 0f;
			}
			return points[points.Length - 1].Value;
		}
	}
}
