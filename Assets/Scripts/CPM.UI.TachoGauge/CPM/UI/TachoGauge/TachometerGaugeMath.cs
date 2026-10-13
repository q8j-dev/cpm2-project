using System;
using UnityEngine;

namespace CPM.UI.TachoGauge
{
	public static class TachometerGaugeMath
	{
		public const float DenseScaleMaxRpm = 11000f;

		public const float DenseDigitsMaxRpm = 12000f;

		public static int DigitStepThousands(float maxRpm)
		{
			if (!(maxRpm > 12000f))
			{
				return 1;
			}
			return 2;
		}

		public static int QuarterIndexStep(float maxRpm)
		{
			if (!(maxRpm > 11000f))
			{
				return 1;
			}
			return 2;
		}

		public static int MajorStepCount(float maxRpm)
		{
			if (maxRpm <= 0f)
			{
				return 0;
			}
			int num = DigitStepThousands(maxRpm);
			return Mathf.CeilToInt(maxRpm / (1000f * (float)num)) * num;
		}

		public static float TickAngleRad(float valueThousands, float startAngleDeg, float endAngleDeg, int majorStepCount)
		{
			float num = startAngleDeg * ((float)Math.PI / 180f);
			float num2 = endAngleDeg * ((float)Math.PI / 180f);
			if (num < 0f)
			{
				num += (float)Math.PI * 2f;
			}
			if (num2 < 0f)
			{
				num2 += (float)Math.PI * 2f;
			}
			if (num > num2)
			{
				num2 += (float)Math.PI * 2f;
			}
			float num3 = ((majorStepCount <= 0) ? 0f : ((num2 - num) / (float)majorStepCount));
			return num - valueThousands * num3;
		}

		public static bool IsMajor(int quarterIndex)
		{
			return quarterIndex % 4 == 0;
		}

		public static bool IsHalf(int quarterIndex)
		{
			return quarterIndex % 4 == 2;
		}

		public static float TickWidth(int quarterIndex, float lineWidth)
		{
			if (!IsMajor(quarterIndex))
			{
				return lineWidth * 0.5f;
			}
			return lineWidth;
		}

		public static float TickLength(int quarterIndex, float lineRadius, float lineLength, float digitsRadius)
		{
			if (!IsHalf(quarterIndex))
			{
				return lineLength;
			}
			return Mathf.Abs(lineRadius - digitsRadius);
		}

		public static float TickCenterRadius(int quarterIndex, float lineRadius, float lineLength, float digitsRadius)
		{
			float num = TickLength(quarterIndex, lineRadius, lineLength, digitsRadius);
			float num2 = ((digitsRadius >= lineRadius) ? 1f : (-1f));
			return lineRadius + num2 * num * 0.5f;
		}

		public static float DigitCenterRadius(float digitsRadius, float lineRadius, float digitHeight)
		{
			float num = ((digitsRadius >= lineRadius) ? 1f : (-1f));
			return digitsRadius + num * digitHeight * 0.5f;
		}
	}
}
