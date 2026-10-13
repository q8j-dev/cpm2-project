using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	[CreateAssetMenu(menuName = "WorldInteraction/Settings", fileName = "WorldInteractionSettings")]
	public class WorldInteractionSettings : ScriptableObject
	{
		[Header("Search")]
		public float SearchRadius = 10f;

		public float SearchDistance = 5f;

		public Vector3 SearchBoxHalfExtents = new Vector3(0.5f, 0.8f, 0.1f);

		public float FlatnessProbeRadius = 0.3f;

		public float MeshSnapMaxDistance = 0.5f;

		public Vector3 ClearanceHalfExtents = new Vector3(0.25f, 0.5f, 0.25f);

		public LayerMask SurfaceMask = -1;

		public LayerMask FootProbeMask = -1;

		[Header("Lean geometry")]
		public float LeanHipHeight = 0.3f;

		public float LeanChestHeight = 1.2f;

		public float LeanSearchHeight = 1.4f;

		public float LeanFaceAboveHip = 0.1f;

		public float LeanStandOff = 0.35f;

		[Header("Approach")]
		public float ApproachSpeed = 0.5f;

		public float ArriveDistance = 0.3f;

		public float ArriveAngle = 6f;

		public float RotateSpeed = 6f;

		public float ApproachTimeout = 2f;

		[Header("IK")]
		public float IkWeightLerpSpeed = 6f;

		[Header("Surface tracking")]
		public float SurfaceMoveExitDistance = 0.2f;

		[Header("Foot probe")]
		public float FootProbeUp = 1.5f;

		public float FootProbeDown = 1.5f;

		public float FootSurfaceClearance = 0.05f;

		public float SeatEdgeInset = 0.12f;

		[Header("Ground check (in-place + horizontal poses)")]
		public float GroundCheckUp = 0.3f;

		public float GroundCheckDistance = 0.4f;
	}
}
