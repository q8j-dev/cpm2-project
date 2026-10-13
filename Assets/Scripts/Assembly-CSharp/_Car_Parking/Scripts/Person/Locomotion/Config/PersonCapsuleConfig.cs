using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	[CreateAssetMenu(fileName = "PersonCapsuleConfig", menuName = "Scriptable/Person/PersonCapsuleConfig")]
	public class PersonCapsuleConfig : ScriptableObject
	{
		[Header("Standing")]
		public float StandingHeight = 1.8f;

		public float StandingCentre = 0.93f;

		[Header("Crouching")]
		public float CrouchingHeight = 1.2f;

		public float CrouchingCentre = 0.6f;

		[Header("Transition")]
		public float CrouchTransitionSpeed = 8f;

		public float SlideTransitionSpeed = 12f;

		[Header("Ceiling")]
		public LayerMask CeilingCheckMask = -1;

		public float CeilingClearance = 0.05f;
	}
}
