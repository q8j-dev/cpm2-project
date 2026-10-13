using UnityEngine;

namespace CPMEngine
{
	public static class SequentialTransmission
	{
		public static void RunShiftFsm(ITransmissionHost h, float dt, ref SequentialFsmState state, ref float fsmTimer, ref int pendingGear, ref int currentGearAtStart)
		{
			h.TcMult = 1f;
			h.TccLocked = false;
			float num = h.Profile.SeqShiftMs * 0.001f;
			switch (state)
			{
			case SequentialFsmState.Steady:
			{
				h.ThrottleOverride = 1f;
				float num3 = ((h.AccelWant < 0.05f && Mathf.Abs(h.Velocity) < h.Profile.AmtLaunchSpeed) ? 0f : AmtTransmission.LaunchEngagement(h));
				h.Clutch = 1f - num3;
				if (h.TargetGear != h.CurrentGear)
				{
					pendingGear = Mathf.Clamp(h.TargetGear, 0, h.NumGears + 1);
					currentGearAtStart = h.CurrentGear;
					fsmTimer = 0f;
					state = SequentialFsmState.Shifting;
				}
				break;
			}
			case SequentialFsmState.Shifting:
			{
				bool flag = pendingGear < currentGearAtStart;
				h.ThrottleOverride = (flag ? 1f : 0f);
				if (flag && h.AccelWant < 0.05f)
				{
					float num2 = PhysMath.RevMatchTargetRpm(Mathf.Abs(h.WDiff), h.GearRatio(pendingGear), h.FinalDrive, h.Redline, 1.08f);
					if (h.CurrentRpm < num2 - 150f)
					{
						h.RevMatchBlip = 1f;
					}
				}
				fsmTimer += dt;
				if (h.CurrentGear != pendingGear && fsmTimer >= num * 0.5f)
				{
					h.CurrentGear = pendingGear;
				}
				if (fsmTimer >= num)
				{
					h.ThrottleOverride = 1f;
					state = SequentialFsmState.Steady;
				}
				break;
			}
			}
		}
	}
}
