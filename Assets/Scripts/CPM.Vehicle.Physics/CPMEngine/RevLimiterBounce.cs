using System;
using UnityEngine;

namespace CPMEngine
{
	public static class RevLimiterBounce
	{
		private const float PeakOverRpm = 150f;

		public static float CeilingRpm(float limitRpm, float depthRpm, float freqHz, ref float phase, float dt)
		{
			phase += dt * freqHz;
			phase -= Mathf.Floor(phase);
			float num = 0.5f - 0.5f * Mathf.Cos(phase * 2f * (float)Math.PI);
			return limitRpm + 150f - depthRpm * num;
		}
	}
}
