using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public static class MouseSteering
	{
		private const float CenterSnap = 0.001f;

		public static float Step(float position, bool held, float deltaX, float maxDisplacement, float returnRate, float deltaTime)
		{
			if (held)
			{
				if (!(maxDisplacement > 0f))
				{
					return position;
				}
				return Mathf.Clamp(position + deltaX / maxDisplacement, -1f, 1f);
			}
			float num = position * Mathf.Exp((0f - Mathf.Max(0f, returnRate)) * Mathf.Max(0f, deltaTime));
			if (!(Mathf.Abs(num) < 0.001f))
			{
				return num;
			}
			return 0f;
		}
	}
}
