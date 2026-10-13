using UnityEngine;

namespace CPM.UI.BoostGauge
{
	public static class BoostGaugeMath
	{
		public static float PositiveArcAngleDeg(float value, float max)
		{
			if (max <= 0f)
			{
				return 180f;
			}
			float num = Mathf.Clamp(value, 0f, max);
			return 180f * (1f - num / max);
		}

		public static float NegativeArcAngleDeg(float value)
		{
			float num = Mathf.Clamp(value, -1f, 0f);
			return 180f - num * 90f;
		}

		public static float PositiveFillAmount(float cur, float max)
		{
			if (max <= 0f)
			{
				return 0f;
			}
			return Mathf.Clamp01(cur / max) * 0.5f;
		}

		public static float NegativeFillAmount(float cur)
		{
			return Mathf.Clamp01(0f - cur) * 0.25f;
		}

		public static int MajorTickCount(float max)
		{
			if (!(max <= 0f))
			{
				return Mathf.FloorToInt(max);
			}
			return 0;
		}

		public static int ScaleMax(float maxBoost, int minScale)
		{
			int b = ((maxBoost <= 0f) ? minScale : Mathf.CeilToInt(maxBoost));
			return Mathf.Max(minScale, b);
		}

		public static bool IsIntegerValue(float value)
		{
			return Mathf.Approximately(value, Mathf.Round(value));
		}
	}
}
