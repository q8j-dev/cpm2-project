using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public static class WorldPoseSyncCodec
	{
		public static Vector3 ToInvariant(Vector3 world, Vector3 globalOffset)
		{
			return world - globalOffset;
		}

		public static Vector3 ToWorld(Vector3 invariant, Vector3 globalOffset)
		{
			return invariant + globalOffset;
		}

		public static float Yaw(Quaternion facing)
		{
			return facing.eulerAngles.y;
		}

		public static Quaternion Facing(float yaw)
		{
			return Quaternion.Euler(0f, yaw, 0f);
		}
	}
}
