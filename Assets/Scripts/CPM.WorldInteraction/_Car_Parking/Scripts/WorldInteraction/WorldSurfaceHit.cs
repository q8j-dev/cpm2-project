using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public struct WorldSurfaceHit
	{
		public bool Valid;

		public Vector3 SurfacePoint;

		public Quaternion Facing;

		public Vector3 ApproachPoint;

		public WorldPoseData Pose;

		public WorldInteractionMarker Marker;

		public Collider Collider;

		public static readonly WorldSurfaceHit None = new WorldSurfaceHit
		{
			Valid = false
		};
	}
}
