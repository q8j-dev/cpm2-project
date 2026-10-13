using UnityEngine;

namespace CPMEngine
{
	public static class IdleGovernor
	{
		public const float MaxAuthority = 0.5f;

		public const float Floor = 0.01f;

		public const float SubIdleBand = 100f;

		public static float IdleHoldStep(float currentW, float idleW, float stallFloorW, float maxHoldTorque, float inertia, float dt)
		{
			float num = Mathf.Max(currentW, stallFloorW);
			if (num >= idleW)
			{
				return num;
			}
			if (inertia <= 0f || dt <= 0f || maxHoldTorque <= 0f)
			{
				return num;
			}
			return Mathf.Min(idleW, num + maxHoldTorque * dt / inertia);
		}

		public static float StallGuardFloorRpm(float minStarterRpm, float idleRpm, float stallFraction, float marginRpm)
		{
			return Mathf.Min(Mathf.Max(minStarterRpm, idleRpm * Mathf.Clamp01(stallFraction)) + marginRpm, idleRpm);
		}

		public static float ThrottleFloor(float currentRpm, float idleRpm, float scale)
		{
			float num = idleRpm * 2f;
			if (currentRpm > num)
			{
				return 0.01f;
			}
			float num2 = (num - currentRpm) / num;
			float num3 = Mathf.Clamp01((idleRpm - currentRpm) / 100f);
			float num4 = num2 * 0.25f + num2 * 0.5f * num3;
			return Mathf.Min(0.01f + num4 * Mathf.Clamp01(scale), 0.5f);
		}
	}
}
