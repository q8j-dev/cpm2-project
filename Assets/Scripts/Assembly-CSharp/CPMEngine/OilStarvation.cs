using UnityEngine;

namespace CPMEngine
{
	public static class OilStarvation
	{
		public const float StarvationThreshold = 0.3f;

		private const float MaxWearMultiplier = 5f;

		public static float GetWearMultiplier(float oilNorm)
		{
			if (oilNorm >= 0.3f)
			{
				return 1f;
			}
			float num = 1f - Mathf.Clamp01(oilNorm) / 0.3f;
			return 1f + 4f * num * num;
		}
	}
}
