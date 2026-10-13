namespace CPMEngine
{
	public sealed class MtBehavior : ITransmissionBehavior
	{
		public void Tick(ITransmissionHost h, float dt)
		{
			if (h.AutoClutch)
			{
				h.IdleGovernorScale = 1f;
				AtTransmission.SimulateClutch(h, dt);
				return;
			}
			h.TcMult = 1f;
			h.CurrentGear = h.TargetGear;
			h.TccLocked = false;
			MtTransmission.ApplyStallPolicy(h);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			int num = h.TargetGear - 1;
			gear = AtTransmission.GetOptimalGear(h, h.Velocity, h.WDiff, num, h.AccelWant);
			return gear != num;
		}
	}
}
