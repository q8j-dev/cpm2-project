using UnityEngine;

namespace CPMEngine
{
	public sealed class AmtBehavior : ITransmissionBehavior
	{
		private AmtFsmState _fsmState;

		private float _fsmTimer;

		private int _pendingTargetGear = 1;

		private int _currentGearAtStart = 1;

		public void Tick(ITransmissionHost h, float dt)
		{
			AmtTransmission.RunShiftFsm(h, dt, ref _fsmState, ref _fsmTimer, ref _pendingTargetGear, ref _currentGearAtStart);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			int num = h.TargetGear - 1;
			if (_fsmState != AmtFsmState.Steady)
			{
				gear = num;
				return false;
			}
			if (h.GearboxState == GearboxState.Burnout)
			{
				gear = num;
				return false;
			}
			float num2 = h.Velocity / Mathf.Max(h.DrivenWheelRadius, 0.01f);
			bool flag = Mathf.Abs(h.WDiff) > num2 * 1.35f + 1f;
			if (h.CurrentRpm >= (h.SportMode ? h.Redline : AtTransmission.WotUpshiftCeiling(h)) && !flag && num > 0 && num < h.NumGears && h.AccelWant > 0.5f)
			{
				gear = num + 1;
				return true;
			}
			gear = AtTransmission.GetOptimalGear(h, h.Velocity, h.WDiff, num, h.AccelWant);
			gear = AtTransmission.ApplyLowCap(h, gear, num);
			return gear != num;
		}
	}
}
