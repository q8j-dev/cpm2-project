namespace CPMEngine
{
	public sealed class AtBehavior : ITransmissionBehavior
	{
		public void Tick(ITransmissionHost h, float dt)
		{
			AtTransmission.SimulateClutch(h, dt);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			int num = h.TargetGear - 1;
			gear = AtTransmission.GetOptimalGear(h, h.Velocity, h.WDiff, num, h.AccelWant);
			gear = AtTransmission.ApplyLowCap(h, gear, num);
			return gear != num;
		}
	}
}
