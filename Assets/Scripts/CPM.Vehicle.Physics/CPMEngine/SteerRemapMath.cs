using UnityEngine;

namespace CPMEngine
{
	public static class SteerRemapMath
	{
		public static float WidgetAngleFromSteerInput(float steerInput, float maxWidgetAngle)
		{
			return Mathf.Clamp(steerInput, -1f, 1f) * maxWidgetAngle;
		}

		public static float SteerInputFromWidgetAngle(float widgetAngle, float maxWidgetAngle)
		{
			if (maxWidgetAngle <= 0f)
			{
				return 0f;
			}
			return Mathf.Clamp(widgetAngle / maxWidgetAngle, -1f, 1f);
		}
	}
}
