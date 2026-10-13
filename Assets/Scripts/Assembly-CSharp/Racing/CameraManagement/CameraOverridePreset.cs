using Cinemachine;
using UnityEngine;

namespace Racing.CameraManagement
{
	[CreateAssetMenu(fileName = "NewCameraPreset", menuName = "Configs/Camera Override Preset")]
	public class CameraOverridePreset : ScriptableObject
	{
		public int Index;

		public string Description;

		public float MinimumObstacleDetectTime;

		public CinemachineCollider.ResolutionStrategy ObstacleAvoidStrategy;

		public float TimeToTryRestPos;

		public float PosSetDamping;

		public float ObstacleAvoidDamping;

		public float ZoomDistance;

		[Header("Position")]
		[Space]
		public BoundsRelativePos XPosSide = BoundsRelativePos.Left;

		public float XPositionOffset;

		public BoundsRelativePos YPosSide = BoundsRelativePos.YCenter;

		public float YPositionOffset;

		public BoundsRelativePos ZPosSide;

		public float ZPositionOffset;

		public Vector3 GetLocalOffset(Bounds bounds)
		{
			return new Vector3(GetPos(bounds, XPosSide) + XPositionOffset, GetPos(bounds, YPosSide) + YPositionOffset, GetPos(bounds, ZPosSide) + ZPositionOffset);
		}

		public void ApplyCineColliderSettings(CinemachineCollider cineCollider)
		{
			cineCollider.m_MinimumOcclusionTime = MinimumObstacleDetectTime;
			cineCollider.m_Strategy = ObstacleAvoidStrategy;
			cineCollider.m_SmoothingTime = TimeToTryRestPos;
			cineCollider.m_Damping = PosSetDamping;
			cineCollider.m_DampingWhenOccluded = ObstacleAvoidDamping;
		}

		private float GetPos(Bounds bounds, BoundsRelativePos pos)
		{
			return pos switch
			{
				BoundsRelativePos.Front => bounds.extents.x, 
				BoundsRelativePos.Back => 0f - bounds.extents.x, 
				BoundsRelativePos.Left => 0f - bounds.extents.z, 
				BoundsRelativePos.Right => bounds.extents.z, 
				BoundsRelativePos.Top => bounds.extents.y, 
				BoundsRelativePos.Bottom => 0f - bounds.extents.y, 
				BoundsRelativePos.XCenter => bounds.center.x, 
				BoundsRelativePos.YCenter => bounds.center.y, 
				BoundsRelativePos.ZCenter => bounds.center.z, 
				_ => 0f, 
			};
		}
	}
}
