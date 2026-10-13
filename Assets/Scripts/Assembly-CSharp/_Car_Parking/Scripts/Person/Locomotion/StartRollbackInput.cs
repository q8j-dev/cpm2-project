namespace _Car_Parking.Scripts.Person.Locomotion
{
	public readonly struct StartRollbackInput
	{
		public readonly bool Enabled;

		public readonly float RollbackSpeed;

		public readonly bool InStartState;

		public readonly float StartNormalizedTime;

		public readonly int StartLatchCount;

		public readonly bool HasMoveInput;

		public readonly bool InputHeld;

		public readonly bool IsGrounded;

		public readonly bool IsJumping;

		public readonly float DeltaTime;

		public StartRollbackInput(bool enabled, float rollbackSpeed, bool inStartState, float startNormalizedTime, int startLatchCount, bool hasMoveInput, bool inputHeld, bool isGrounded, bool isJumping, float deltaTime)
		{
			Enabled = enabled;
			RollbackSpeed = rollbackSpeed;
			InStartState = inStartState;
			StartNormalizedTime = startNormalizedTime;
			StartLatchCount = startLatchCount;
			HasMoveInput = hasMoveInput;
			InputHeld = inputHeld;
			IsGrounded = isGrounded;
			IsJumping = isJumping;
			DeltaTime = deltaTime;
		}
	}
}
