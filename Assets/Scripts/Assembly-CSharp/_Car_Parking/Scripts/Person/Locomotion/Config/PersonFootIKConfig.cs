using RootMotion.FinalIK;
using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	[CreateAssetMenu(fileName = "PersonFootIKConfig", menuName = "Scriptable/Person/PersonFootIKConfig")]
	public class PersonFootIKConfig : ScriptableObject
	{
		[Header("General")]
		public bool Enable = true;

		public Grounding.Quality Quality;

		[Header("Feet")]
		public float MaxStep = 0.5f;

		public float FootSpeed = 2.5f;

		public float FootRadius = 0.15f;

		public float Prediction = 0.05f;

		public float FootRotationWeight = 1f;

		public float FootRotationSpeed = 7f;

		[Range(0f, 90f)]
		public float MaxFootRotationAngle = 45f;

		[Range(0f, 1f)]
		public float BendToTargetWeight = 0.5f;

		public bool AnimatedBendNormal = true;

		[Header("Pelvis")]
		public float PelvisSpeed = 5f;

		[Range(0f, 1f)]
		public float PelvisDamper;

		public float LowerPelvisWeight = 1f;

		public float HeightOffset;

		[Header("Spine")]
		public float SpineBend;

		[Header("Weight Blending")]
		public float WeightLerpSpeed = 6f;

		public float MaxSlopeForIK = 45f;

		[Range(0f, 1f)]
		public float StopGroundWeight = 1f;

		[Header("Descent (direction-aware)")]
		public bool DirectionalDescent = true;

		public float DescendStartDot = 0.15f;

		public float DescendFullDot = 0.5f;

		[Range(0f, 1f)]
		public float DescendFootRotationScale = 1f;

		[Range(0f, 1f)]
		public float DescendMaxStepScale = 1f;

		[Range(0f, 1f)]
		public float DescendPredictionScale = 1f;

		[Range(0f, 1f)]
		public float DescendPelvisDamper;

		public float DescendPelvisSpeed = 5f;

		[Header("Moving Platforms")]
		public bool PlatformIKCompensation = true;

		public float PlatformIKCompensationMax = 0.1f;

		public float PlatformDamperCutoffSpeed = 0.5f;
	}
}
