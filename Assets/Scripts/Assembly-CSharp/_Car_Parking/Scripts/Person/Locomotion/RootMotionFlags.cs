namespace _Car_Parking.Scripts.Person.Locomotion
{
	public readonly struct RootMotionFlags
	{
		public readonly bool StopActive;

		public readonly bool StopSettled;

		public readonly float StopBlend;

		public readonly bool StartActive;

		public readonly bool StartSettled;

		public readonly float StartBlend;

		public readonly bool StartTurnActive;

		public readonly bool TurnStartActive;

		public readonly bool CrouchTurnStart;

		public readonly bool StartExitActive;

		public readonly float StartExitBlend;

		public readonly bool LocoActive;

		public readonly bool LocoSettled;

		public readonly float LocoBlend;

		public readonly bool QuickTurnActive;

		public readonly bool QuickTurnSettled;

		public readonly float QuickTurnBlend;

		public readonly bool QuickTurnExitActive;

		public readonly float QuickTurnExitBlend;

		public readonly bool QuickTurnReverseActive;

		public readonly bool QuickTurnCommitActive;

		public readonly float QuickTurnProgress;

		public readonly bool EmoteActive;

		public readonly bool StartUncommitted;

		public readonly bool StartRollbackActive;

		public RootMotionFlags(bool stopActive, bool stopSettled, float stopBlend, bool startActive, bool startSettled, float startBlend, bool startTurnActive, bool turnStartActive, bool crouchTurnStart, bool startExitActive, float startExitBlend, bool locoActive, bool locoSettled, float locoBlend, bool quickTurnActive, bool quickTurnSettled, float quickTurnBlend, bool quickTurnExitActive, float quickTurnExitBlend, bool quickTurnReverseActive, bool quickTurnCommitActive, float quickTurnProgress, bool emoteActive, bool startUncommitted, bool startRollbackActive)
		{
			StopActive = stopActive;
			StopSettled = stopSettled;
			StopBlend = stopBlend;
			StartActive = startActive;
			StartSettled = startSettled;
			StartBlend = startBlend;
			StartTurnActive = startTurnActive;
			TurnStartActive = turnStartActive;
			CrouchTurnStart = crouchTurnStart;
			StartExitActive = startExitActive;
			StartExitBlend = startExitBlend;
			LocoActive = locoActive;
			LocoSettled = locoSettled;
			LocoBlend = locoBlend;
			QuickTurnActive = quickTurnActive;
			QuickTurnSettled = quickTurnSettled;
			QuickTurnBlend = quickTurnBlend;
			QuickTurnExitActive = quickTurnExitActive;
			QuickTurnExitBlend = quickTurnExitBlend;
			QuickTurnReverseActive = quickTurnReverseActive;
			QuickTurnCommitActive = quickTurnCommitActive;
			QuickTurnProgress = quickTurnProgress;
			EmoteActive = emoteActive;
			StartUncommitted = startUncommitted;
			StartRollbackActive = startRollbackActive;
		}
	}
}
