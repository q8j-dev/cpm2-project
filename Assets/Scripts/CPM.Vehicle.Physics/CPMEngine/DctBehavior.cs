using UnityEngine;

namespace CPMEngine
{
	public sealed class DctBehavior : ITransmissionBehavior
	{
		private DctFsmState _fsmState;

		private float _fsmTimer;

		private int _pendingTargetGear = 2;

		private int _currentGearAtStart = 2;

		public void Tick(ITransmissionHost h, float dt)
		{
			DctTransmission.RunShiftFsm(h, dt, ref _fsmState, ref _fsmTimer, ref _pendingTargetGear, ref _currentGearAtStart);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			int num = h.TargetGear - 1;
			if (_fsmState != DctFsmState.Steady)
			{
				gear = num;
				return false;
			}
			if (h.GearboxState == GearboxState.Burnout)
			{
				gear = num;
				return false;
			}
			bool flag = h.IsActiveLaunchControlNow || h.Velocity < 1f;
			float num2 = h.Velocity / Mathf.Max(h.DrivenWheelRadius, 0.01f);
			bool flag2 = h.WDiff > num2 * 1.35f && h.Velocity > 0.5f;
			if (h.CurrentRpm >= (h.SportMode ? h.Redline : AtTransmission.WotUpshiftCeiling(h)) && num > 0 && num < h.NumGears && h.AccelWant > 0.5f && !flag2 && !flag)
			{
				gear = num + 1;
				return true;
			}
			gear = AtTransmission.GetOptimalGear(h, h.Velocity, h.WDiff, num, h.AccelWant);
			if (flag && gear > num)
			{
				gear = num;
			}
			gear = AtTransmission.ApplyLowCap(h, gear, num);
			return gear != num;
		}
	}
}
