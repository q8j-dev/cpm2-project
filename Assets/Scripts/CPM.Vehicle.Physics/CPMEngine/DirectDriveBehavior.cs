namespace CPMEngine
{
	public sealed class DirectDriveBehavior : ITransmissionBehavior
	{
		public void Tick(ITransmissionHost h, float dt)
		{
			AtTransmission.SimulateClutch(h, dt);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			gear = h.TargetGear - 1;
			return false;
		}
	}
}
