using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public static class PersonAnimatorParams
	{
		public static readonly int CurrentGait = Animator.StringToHash("CurrentGait");

		public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

		public static readonly int FallingDuration = Animator.StringToHash("FallingDuration");

		public static readonly int IsStopped = Animator.StringToHash("IsStopped");

		public static readonly int IsStarting = Animator.StringToHash("IsStarting");

		public static readonly int IsWalking = Animator.StringToHash("IsWalking");

		public static readonly int IsSprinting = Animator.StringToHash("IsSprinting");

		public static readonly int IsCrouching = Animator.StringToHash("IsCrouching");

		public static readonly int IsJumping = Animator.StringToHash("IsJumping");

		public static readonly int DoJump = Animator.StringToHash("DoJump");

		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

		public static readonly int IsQuickTurning = Animator.StringToHash("IsQuickTurning");

		public static readonly int QuickTurnLeft = Animator.StringToHash("QuickTurnLeft");

		public static readonly int MovementInputTapped = Animator.StringToHash("MovementInputTapped");

		public static readonly int MovementInputPressed = Animator.StringToHash("MovementInputPressed");

		public static readonly int MovementInputHeld = Animator.StringToHash("MovementInputHeld");

		public static readonly int IsStrafing = Animator.StringToHash("IsStrafing");

		public static readonly int StrafeDirectionX = Animator.StringToHash("StrafeDirectionX");

		public static readonly int StrafeDirectionZ = Animator.StringToHash("StrafeDirectionZ");

		public static readonly int ForwardStrafe = Animator.StringToHash("ForwardStrafe");

		public static readonly int IdleBreakIndex = Animator.StringToHash("IdleBreakIndex");

		public static readonly int PlayIdleBreak = Animator.StringToHash("PlayIdleBreak");
	}
}
