using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public class RacingCameraPreset
	{
		[Header("Close view")]
		public Vector3 CloseOffset;

		public float CloseFOV;

		public float CloseSlopeUp;

		public float CloseSlopeDown;

		[Header("Middle view")]
		public Vector3 MiddleOffset;

		public float MiddleFOV;

		public float MiddleSlopeUp;

		public float MiddleSlopeDown;

		[Header("Far view")]
		public Vector3 FarOffset;

		public float FarFOV;

		public float FarSlopeUp;

		public float FarSlopeDown;

		[Header("Shared")]
		public Vector3 ComposerOffset;

		[Header("Camera Follow")]
		public float VelocityLookSpeed;

		public float VerticalAngleMin;

		public float VerticalAngleMax;

		public float RotateResetDelay;

		public bool LookAtVelocity;

		public float RotationLerpSpeed;

		public float VerticalFollowSpeed;

		[Header("Acceleration Effect")]
		public float AccelerationEffect;

		public float MaxPullback;

		public float AccelerationSmoothSpeed;

		public float MaxOffsetMoveSpeed;

		public float AccelerationClamp;

		[Header("FOV by Speed")]
		public float StartFOVSpeed;

		public float EndFOVSpeed;

		public float FieldAdjustment;

		public float SpeedFOV;

		[Header("Height by Speed")]
		public float MaxHeightBoost;

		public float HeightBoostMaxSpeed;

		[Header("Camera Collision")]
		public LayerMask CollideAgainstLayers;

		public float CameraRadius;

		[Tooltip("Obstruction must last this long (sec) before the camera reacts. Kills the jerk when clipping bridge railings and other thin roadside geometry.")]
		public float MinimumOcclusionTime;

		[Tooltip("How gradually (sec) the camera returns to its normal position after being pushed by an obstacle.")]
		public float CollisionDamping;

		[Tooltip("How gradually (sec) the camera moves to correct for an occlusion.")]
		public float CollisionDampingWhenOccluded;

		[Tooltip("Seconds spent absorbing an unpredicted jolt in the cockpit view. 0 disables the filter. Steady driving is predicted from the car's own velocity, so unlike transposer damping this costs no lag at speed.")]
		[Header("Driver View Shock Filter")]
		public float DriverViewShockFilterTime;

		[Tooltip("Hard cap, in metres, on how far the cockpit camera may trail the seat while absorbing a jolt.")]
		public float DriverViewShockFilterClamp;

		[Header("Camera Shake")]
		public bool ShakeOnKerb;

		public bool ShakeOnOffRoad;

		public float ShakeMultiplier;

		public float ShakeTransitionSpeed;
	}
}
