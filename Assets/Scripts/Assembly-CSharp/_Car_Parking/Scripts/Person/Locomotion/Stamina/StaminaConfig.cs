using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Stamina
{
	[CreateAssetMenu(fileName = "StaminaConfig", menuName = "Scriptable/Person/StaminaConfig")]
	public class StaminaConfig : ScriptableObject
	{
		[Header("Pool")]
		public float MaxStamina = 100f;

		[Header("Costs")]
		public float SprintDrainPerSecond = 20f;

		public float JumpCost = 15f;

		public float SlideCost = 5f;

		[Header("Regen")]
		public float RegenPerSecond = 12f;

		public float RegenDelayAfterUse = 1f;

		[Header("Thresholds")]
		public float MinStaminaToSprint = 5f;

		public float MinStaminaToJump = 10f;

		public float MinStaminaToSlide = 10f;

		public float ExhaustedRecoveryThreshold = 25f;

		[Header("Behaviour")]
		public ZeroStaminaBehaviour ZeroStaminaBehaviour;
	}
}
