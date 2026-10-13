using System;
using UnityEngine;

namespace CPMEngine
{
	public static class AmtTransmission
	{
		private const float RevMatchMarginRpm = 150f;

		public static float LaunchEngagement(ITransmissionHost h)
		{
			float a = Mathf.Abs(h.Velocity) / h.Profile.AmtLaunchSpeed;
			float num = Mathf.Clamp01(h.Throttle);
			float syncRpm = Mathf.Abs(h.WDiff * h.Ratio) * (30f / (float)Math.PI);
			float tallGearScale = PhysMath.TallGearLaunchScale(h.GearRatio(h.CurrentGear), h.GearRatio(2), 0.3f);
			float launchTargetRpm = PhysMath.LaunchTargetRpm(h.IdleRpm, h.Redline, num, tallGearScale);
			float num2 = PhysMath.LaunchFlare(h.CurrentRpm, syncRpm, h.IdleRpm, h.Redline, num, tallGearScale);
			float b = num * h.Profile.AmtLaunchBite;
			float a2 = Mathf.Clamp01(Mathf.Max(a, b)) * (1f - num2);
			float num3 = Mathf.Abs(h.WDiff * h.Ratio);
			if (h.AccelWant > 0.05f && h.WEngine > num3)
			{
				a2 = Mathf.Min(a2, PhysMath.LaunchTorqueEngagementCap(h.EngineTorque, h.ClutchCapacity));
			}
			return Mathf.Max(a2, PhysMath.LaunchTakeUp01(h.CurrentRpm, launchTargetRpm, num));
		}

		public static void RunShiftFsm(ITransmissionHost h, float dt, ref AmtFsmState state, ref float fsmTimer, ref int pendingTargetGear, ref int currentGearAtStart)
		{
			h.TcMult = 1f;
			h.TccLocked = false;
			float amtClutchOpenMs = h.Profile.AmtClutchOpenMs;
			float amtGearEngageMs = h.Profile.AmtGearEngageMs;
			float amtClutchCloseMs = h.Profile.AmtClutchCloseMs;
			bool amtThrottleCutsOnEngage = h.Profile.AmtThrottleCutsOnEngage;
			if (state != AmtFsmState.Steady)
			{
				int num = Mathf.Clamp(h.TargetGear, 0, h.NumGears + 1);
				if (num != pendingTargetGear)
				{
					int num2 = Math.Sign(pendingTargetGear - currentGearAtStart);
					if (Math.Sign(num - currentGearAtStart) == num2 && num2 != 0)
					{
						pendingTargetGear = num;
					}
					else
					{
						pendingTargetGear = num;
						currentGearAtStart = h.CurrentGear;
						float num3 = amtClutchOpenMs * 0.001f;
						fsmTimer = ((num3 > 0f) ? (h.Clutch * num3) : 0f);
						state = AmtFsmState.OpeningClutch;
					}
				}
			}
			switch (state)
			{
			case AmtFsmState.Steady:
			{
				h.ThrottleOverride = 1f;
				bool num5 = h.AccelWant < 0.1f && MtTransmission.IsAboutToStall(h);
				bool flag = h.Brake > 0.5f && h.Velocity < 1f && h.GearboxState != GearboxState.Burnout;
				bool flag2 = h.AccelWant > 0.1f && h.Brake > 0.5f && h.Velocity < 3f;
				if (num5 | flag)
				{
					h.Clutch = 1f;
				}
				else if (flag2)
				{
					h.Clutch = 0.3f;
				}
				else if (Mathf.Abs(h.Velocity) < h.Profile.AmtLaunchSpeed && Mathf.Abs(h.Ratio) > 0.0001f)
				{
					h.Clutch = 1f - LaunchEngagement(h);
				}
				else
				{
					h.Clutch = 0f;
				}
				if (h.TargetGear != h.CurrentGear)
				{
					pendingTargetGear = Mathf.Clamp(h.TargetGear, 0, h.NumGears + 1);
					currentGearAtStart = h.CurrentGear;
					state = AmtFsmState.OpeningClutch;
					float num6 = amtClutchOpenMs * 0.001f;
					fsmTimer = ((num6 > 0f) ? (h.Clutch * num6) : 0f);
				}
				break;
			}
			case AmtFsmState.OpeningClutch:
			{
				float num10 = amtClutchOpenMs * 0.001f;
				float t2 = ((num10 > 0f) ? Mathf.Clamp01(fsmTimer / num10) : 1f);
				h.Clutch = Mathf.Lerp(0f, 1f, t2);
				h.ThrottleOverride = (amtThrottleCutsOnEngage ? Mathf.Lerp(1f, 0f, t2) : 1f);
				fsmTimer += dt;
				if (fsmTimer >= num10)
				{
					h.CurrentGear = pendingTargetGear;
					state = AmtFsmState.EngagingGear;
					fsmTimer = 0f;
				}
				break;
			}
			case AmtFsmState.EngagingGear:
			{
				h.Clutch = 1f;
				float num7 = amtGearEngageMs * 0.001f;
				bool flag3 = pendingTargetGear < currentGearAtStart;
				if (amtThrottleCutsOnEngage & flag3)
				{
					float num8 = Mathf.Min(Mathf.Abs(h.WDiff) * h.GearRatio(pendingTargetGear) * h.FinalDrive * (30f / (float)Math.PI), h.Redline);
					bool flag4 = h.CurrentRpm >= num8 - 150f;
					h.ThrottleOverride = (flag4 ? 0f : 1f);
					if (!flag4)
					{
						h.RevMatchBlip = 1f;
					}
					fsmTimer += dt;
					float num9 = h.Profile.AmtRevMatchMaxMs * 0.001f;
					if (((fsmTimer >= num7) & flag4) || fsmTimer >= num9)
					{
						state = AmtFsmState.ClosingClutch;
						fsmTimer = 0f;
					}
				}
				else
				{
					h.ThrottleOverride = (amtThrottleCutsOnEngage ? 0f : 1f);
					fsmTimer += dt;
					if (fsmTimer >= num7)
					{
						state = AmtFsmState.ClosingClutch;
						fsmTimer = 0f;
					}
				}
				break;
			}
			case AmtFsmState.ClosingClutch:
			{
				float num4 = amtClutchCloseMs * 0.001f;
				float t = ((num4 > 0f) ? Mathf.Clamp01(fsmTimer / num4) : 1f);
				float b = ((h.AccelWant < 0.1f && MtTransmission.IsAboutToStall(h)) ? 1f : 0f);
				h.Clutch = Mathf.Lerp(1f, b, t);
				h.ThrottleOverride = (amtThrottleCutsOnEngage ? Mathf.Lerp(0f, 1f, t) : 1f);
				fsmTimer += dt;
				if (fsmTimer >= num4)
				{
					state = AmtFsmState.Steady;
					fsmTimer = 0f;
				}
				break;
			}
			}
			h.Clutch = Mathf.Max(h.Clutch, MtTransmission.AntiStallOpening(h));
			MtTransmission.ApplyStallPolicy(h);
		}
	}
}
