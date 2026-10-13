using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl
{
	public static class HeightOffset
	{
		private const float ItalyYOffset = 100f;

		private static float GetYOffset(Vector3 localPos)
		{
			if (!V.SceneControl)
			{
				return 0f;
			}
			if (V.SceneControl.getEnvironmentByColliders(localPos + V.GlobalOffset) == 20)
			{
				return 100f;
			}
			return 0f;
		}

		public static float ApplyOnSend(Vector3 localPos, float y)
		{
			return y - GetYOffset(localPos);
		}

		public static float ApplyOnReceive(Vector3 localPos, float y)
		{
			return y + GetYOffset(localPos);
		}
	}
}
