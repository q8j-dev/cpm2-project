namespace _Car_Parking.Scripts.Person.Locomotion
{
	public struct LocomotionStateSnapshot
	{
		public int Gait;

		public float MoveSpeed;

		public float StrafeDirectionX;

		public float StrafeDirectionZ;

		public float ForwardStrafe;

		public float LocomotionStartDirection;

		public float StartInitialDirection;

		public int StartLatchCount;

		public float InclineAngle;

		public float SlopeLeanAngle;

		public float FallingDuration;

		public float DescendingDuration;

		public bool IsStrafing;

		public bool IsCrouching;

		public bool IsSliding;

		public bool IsJumping;

		public bool IsGrounded;

		public bool IsStopped;

		public bool IsStarting;

		public bool IsWalking;

		public bool IsSprinting;

		public bool IsTurningInPlace;

		public bool IsQuickTurning;

		public bool QuickTurnLeft;

		public bool MovementInputTapped;

		public bool MovementInputPressed;

		public bool MovementInputHeld;

		public bool ShuffleRequested;

		public float CameraRotationOffset;

		public float ShuffleDirectionX;

		public float ShuffleDirectionZ;

		public float LeanValue;

		public float HeadLookX;

		public float HeadLookY;

		public float BodyLookX;

		public float BodyLookY;
	}
}
