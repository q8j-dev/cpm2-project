using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Car_Parking.Scripts.WorldInteraction
{
	[Serializable]
	public class WorldPoseData
	{
		[HideInInspector]
		[FormerlySerializedAs("Type")]
		public int Id;

		public Vector3 BodyOffset;

		public Vector3 BodyEulerOffset;

		[Range(0f, 1f)]
		public float BodyWeight = 1f;

		public bool UseFeet = true;

		public Vector3 LeftFootOffset;

		public Vector3 LeftFootEuler;

		public Vector3 RightFootOffset;

		public Vector3 RightFootEuler;

		[Range(0f, 1f)]
		public float LeftLegMappingWeight = 1f;

		[Range(0f, 1f)]
		public float RightLegMappingWeight = 1f;

		[Range(0f, 1f)]
		public float FootWeight = 1f;

		[Range(0f, 1f)]
		public float FootRotationWeight;

		public bool UseShoulders;

		public Vector3 LeftShoulderOffset;

		public Vector3 RightShoulderOffset;

		[Range(0f, 1f)]
		public float ShoulderWeight = 1f;

		public bool UseThighs;

		public Vector3 LeftThighOffset;

		public Vector3 RightThighOffset;

		[Range(0f, 1f)]
		public float ThighWeight = 1f;

		public bool UseArmBend;

		public Vector3 LeftElbowOffset;

		public Vector3 RightElbowOffset;

		[Range(0f, 1f)]
		public float ArmBendWeight = 1f;

		public bool UseLegBend = true;

		public Vector3 LeftKneeOffset;

		public Vector3 RightKneeOffset;

		[Range(0f, 1f)]
		public float LegBendWeight = 1f;

		public bool UseHands;

		public Vector3 LeftHandOffset;

		public Vector3 LeftHandEuler;

		public Vector3 RightHandOffset;

		public Vector3 RightHandEuler;

		[Range(0f, 1f)]
		public float LeftArmMappingWeight = 1f;

		[Range(0f, 1f)]
		public float RightArmMappingWeight = 1f;

		[Range(0f, 1f)]
		public float HandWeight = 1f;

		[Range(0f, 1f)]
		public float HandRotationWeight;

		[Range(0f, 1f)]
		public float MaintainHeadRot;

		public bool UseHeadLook;

		public Vector3 HeadLookOffset;

		[Range(0f, 1f)]
		public float HeadLookWeight = 0.85f;

		public float ApproachDistance;

		public WorldPoseData Clone()
		{
			return (WorldPoseData)MemberwiseClone();
		}
	}
}
