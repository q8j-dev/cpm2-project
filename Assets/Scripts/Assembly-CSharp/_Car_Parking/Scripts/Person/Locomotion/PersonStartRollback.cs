namespace _Car_Parking.Scripts.Person.Locomotion
{
	public sealed class PersonStartRollback
	{
		private const float CompleteNormalizedTime = 0.01f;

		private const float RollbackTimeout = 1f;

		private int _latch = -1;

		private float _elapsed;

		public StartRollbackPhase Phase { get; private set; }

		public float PlaybackSpeed { get; private set; } = 1f;

		public bool CompletedThisFrame { get; private set; }

		public bool Uncommitted
		{
			get
			{
				if (Phase != StartRollbackPhase.Uncommitted)
				{
					return Phase == StartRollbackPhase.RollingBack;
				}
				return true;
			}
		}

		public bool OwnsStart
		{
			get
			{
				if (!Uncommitted)
				{
					return Phase == StartRollbackPhase.Finished;
				}
				return true;
			}
		}

		public bool Active => Phase == StartRollbackPhase.RollingBack;

		public void Reset()
		{
			Phase = StartRollbackPhase.Idle;
			PlaybackSpeed = 1f;
			CompletedThisFrame = false;
			_elapsed = 0f;
			_latch = -1;
		}

		public void Tick(in StartRollbackInput input)
		{
			CompletedThisFrame = false;
			if (!input.Enabled || !input.InStartState || !input.IsGrounded || input.IsJumping)
			{
				Phase = StartRollbackPhase.Idle;
				PlaybackSpeed = 1f;
				_elapsed = 0f;
				_latch = -1;
				return;
			}
			if (_latch != input.StartLatchCount)
			{
				_latch = input.StartLatchCount;
				Phase = StartRollbackPhase.Uncommitted;
				_elapsed = 0f;
			}
			else if (Phase == StartRollbackPhase.Idle)
			{
				Phase = StartRollbackPhase.Uncommitted;
				_elapsed = 0f;
			}
			switch (Phase)
			{
			case StartRollbackPhase.Uncommitted:
				TickUncommitted(in input);
				break;
			case StartRollbackPhase.RollingBack:
				TickRollingBack(in input);
				break;
			case StartRollbackPhase.Finished:
				PlaybackSpeed = 1f;
				if (input.HasMoveInput)
				{
					Phase = StartRollbackPhase.Uncommitted;
				}
				break;
			default:
				PlaybackSpeed = 1f;
				break;
			}
		}

		private void TickUncommitted(in StartRollbackInput input)
		{
			PlaybackSpeed = 1f;
			if (input.InputHeld)
			{
				Phase = StartRollbackPhase.Committed;
			}
			else if (!input.HasMoveInput)
			{
				Phase = StartRollbackPhase.RollingBack;
				PlaybackSpeed = 0f - input.RollbackSpeed;
				_elapsed = 0f;
			}
		}

		private void TickRollingBack(in StartRollbackInput input)
		{
			if (input.HasMoveInput)
			{
				Phase = StartRollbackPhase.Uncommitted;
				PlaybackSpeed = 1f;
				_elapsed = 0f;
				return;
			}
			_elapsed += input.DeltaTime;
			PlaybackSpeed = 0f - input.RollbackSpeed;
			if (!(input.StartNormalizedTime > 0.01f) || !(_elapsed < 1f))
			{
				CompletedThisFrame = true;
				Phase = StartRollbackPhase.Finished;
				PlaybackSpeed = 1f;
				_elapsed = 0f;
			}
		}
	}
}
