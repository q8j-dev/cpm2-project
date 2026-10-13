using UnityEngine;

namespace _Car_Parking.Scripts
{
	[CreateAssetMenu(menuName = "CPM/Alter Chase Camera Config", fileName = "AlterChaseCameraConfig")]
	public class AlterChaseCameraConfig : ScriptableObject
	{
		[Header("Chase distance stagger (m, horizontal to the anchor)")]
		public float NearDistance = 3.5f;

		public float MiddleDistance = 5f;

		public float FarDistance = 7f;

		[Header("Base FOV for the non-POV cameras (free-drive base)")]
		public float BaseFov = 50f;

		[Header("Garage default view")]
		[Tooltip("Degrees to tilt the garage camera UP from the derived free-drive chase tilt (~11.3° down). 0 = exact chase tilt.")]
		public float GarageTiltUpDeg;

		[Tooltip("Metres added to the garage default distance on top of MiddleDistance (the zoom correction is approximate — use this to match the free-drive framing by eye). Negative = closer.")]
		public float GarageDistanceAdd;

		[Header("Look left/right/back action yaw (deg, car-relative)")]
		public float LookLeftYaw = -90f;

		public float LookRightYaw = 90f;

		public float LookBackYaw = 180f;

		public float LookForwardYaw;

		public Vector3 FollowOffset(float distance, float height, float pitch, float followDeltaY, float d)
		{
			float num = height + pitch;
			float y = followDeltaY + num + d * ((0f - pitch) / distance);
			return new Vector3(0f, y, 0f - d);
		}

		public Vector3 ComposerOffset(float height, float pitch, float aimDeltaY)
		{
			return new Vector3(0f, aimDeltaY + height + pitch, 0f);
		}

		public float TiltDeg(float distance, float pitch)
		{
			return Mathf.Atan2(0f - pitch, distance) * 57.29578f;
		}

		public static Transform FindAnchor(Transform carRoot)
		{
			if (!carRoot)
			{
				return null;
			}
			Transform transform = carRoot.Find("CarCommonCameraTarget");
			if (!transform)
			{
				return carRoot;
			}
			return transform;
		}
	}
}
