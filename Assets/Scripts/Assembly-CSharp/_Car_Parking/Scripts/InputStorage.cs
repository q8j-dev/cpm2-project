using UnityEngine;

namespace _Car_Parking.Scripts
{
	public static class InputStorage
	{
		public static float ClutchInput;

		public static float ThrottleInput;

		public static float BrakeInput;

		public static float HandbrakeInput;

		public static float LeftRightInput;

		public static float LeftJoyInputX;

		public static float LeftJoyInputY;

		public static float RightJoyInputX;

		public static float RightJoyInputY;

		public static bool JumpRequested;

		public static bool CrouchRequested;

		public static bool PoseExitRequested;

		public static bool SprintHeld;

		public static bool LeftJoyIsDrag;

		public static bool SprintLocked;

		public static bool SprintLockArmed;

		public static bool SprintTierActive;

		public static bool DeflectionSprintMode;

		public static bool SprintLockCancelRequested;

		public static float SprintLockArmProgress;

		public static bool SprintLockAiming;

		public static Vector2 LeftJoyTouchScreenPos;

		public static bool GamepadActive;
	}
}
