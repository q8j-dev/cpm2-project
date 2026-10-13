using Unity.Mathematics;
using UnityEngine;

namespace CPMEngine
{
	public static class AirSuspensionMath
	{
		private const float MinStoredClearanceMm = 1f;

		private const float LegacyMetreCeiling = 1f;

		public const float MinWheelSpringLen = 0.0254f;

		public static float CornerLoad(float mass, float frontPercent, bool front)
		{
			float num = Mathf.Clamp01(frontPercent * 0.01f);
			if (!front)
			{
				num = 1f - num;
			}
			return Mathf.Max(0f, mass) * 9.80665f * num * 0.5f;
		}

		public static float Sag(float cornerLoad, float springCoef)
		{
			if (springCoef <= 0f)
			{
				return 0f;
			}
			return Mathf.Max(0f, cornerLoad) / springCoef;
		}

		public static float SpringLenToClearance(float springLen, float sag)
		{
			return Mathf.Max(0f, springLen - sag);
		}

		public static float ClearanceToSpringLen(float clearance, float sag, float springLen)
		{
			return Mathf.Min(Mathf.Max(0f, springLen), Mathf.Max(0f, clearance) + Mathf.Max(0f, sag));
		}

		public static float ReadStoredLimitMm(float stored, float sag, float springLen)
		{
			if (float.IsNaN(stored))
			{
				return -1f;
			}
			if (stored < 0f)
			{
				return -1f;
			}
			if (stored > 0f && stored < 1f)
			{
				if (stored >= springLen)
				{
					return -1f;
				}
				float num = Mathf.Max(0f, stored - sag) * 1000f;
				if (!(num < 1f))
				{
					return Mathf.Round(num);
				}
				return -1f;
			}
			return Mathf.Round(stored);
		}

		public static float ReadStoredSetpointMm(float stored, float sag)
		{
			if (float.IsNaN(stored))
			{
				return -1f;
			}
			if (stored < 0f)
			{
				return -1f;
			}
			if (stored > 0f && stored < 1f)
			{
				float num = Mathf.Max(0f, stored - sag) * 1000f;
				if (!(num < 1f))
				{
					return Mathf.Round(num);
				}
				return 0f;
			}
			return Mathf.Round(stored);
		}

		public static float2 WholeMillimetreLimits(float2 limits)
		{
			float num = Mathf.Ceil(limits.x * 1000f - 0.001f) * 0.001f;
			float num2 = Mathf.Floor(limits.y * 1000f + 0.001f) * 0.001f;
			if (!(num2 < num))
			{
				return new float2(num, num2);
			}
			return limits;
		}

		public static float2 CommonRoom(float hFront, float2 limitsFront, float hRear, float2 limitsRear)
		{
			float y = Mathf.Max(0f, Mathf.Min(limitsFront.y - hFront, limitsRear.y - hRear));
			return new float2(Mathf.Max(0f, Mathf.Min(hFront - limitsFront.x, hRear - limitsRear.x)), y);
		}

		public static float ClampCommonDelta(float delta, float hFront, float2 limitsFront, float hRear, float2 limitsRear)
		{
			float2 float5 = CommonRoom(hFront, limitsFront, hRear, limitsRear);
			return Mathf.Clamp(delta, 0f - float5.x, float5.y);
		}

		public static float2 EffectiveLimits(float minSpringLen, float maxSpringLen, float sag, float2 preference)
		{
			float num = SpringLenToClearance(maxSpringLen, sag);
			float num2 = Mathf.Min(Mathf.Max(SpringLenToClearance(minSpringLen, sag), Mathf.Max(0f, 0.0254f - sag)), num);
			float num3 = ((preference.y < 0f) ? num : Mathf.Clamp(preference.y, num2, num));
			return new float2(Mathf.Clamp((preference.x < 0f) ? num2 : preference.x, num2, num3), num3);
		}

		public static float2 ResolveSpringLenLimits(float minLimit, float maxLimit, float fittedSpringLen)
		{
			if (maxLimit <= 0f)
			{
				return new float2(0f, Mathf.Max(0f, fittedSpringLen));
			}
			return new float2(Mathf.Clamp(minLimit, 0f, maxLimit), maxLimit);
		}
	}
}
