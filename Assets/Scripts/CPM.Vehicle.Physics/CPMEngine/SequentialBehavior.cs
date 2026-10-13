namespace CPMEngine
{
	public sealed class SequentialBehavior : ITransmissionBehavior
	{
		private SequentialFsmState _state;

		private float _fsmTimer;

		private int _pendingGear = 1;

		private int _currentGearAtStart = 1;

		public void Tick(ITransmissionHost h, float dt)
		{
			SequentialTransmission.RunShiftFsm(h, dt, ref _state, ref _fsmTimer, ref _pendingGear, ref _currentGearAtStart);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			gear = h.TargetGear - 1;
			return false;
		}
	}
}
