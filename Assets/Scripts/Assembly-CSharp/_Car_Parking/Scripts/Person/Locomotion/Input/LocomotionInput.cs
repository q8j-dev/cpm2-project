using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Input
{
	public struct LocomotionInput
	{
		public Vector2 Move;

		public bool SprintHeld;

		public bool SprintTierHeld;

		public bool CrouchHeld;

		public Vector2 Look;

		public float CameraYaw;

		public bool HasMove;

		public bool MovementInputTapped;

		public bool MovementInputPressed;

		public bool MovementInputHeld;

		public bool ShuffleRequested;
	}
}
