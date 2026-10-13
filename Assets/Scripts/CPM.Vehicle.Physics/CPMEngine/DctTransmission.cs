using System;
using UnityEngine;

namespace CPMEngine
{
	public static class DctTransmission
	{
		private static float BurnoutSlipFloor(ITransmissionHost h)
		{
			return Mathf.Lerp(0.8f, 0.3f, Mathf.Clamp01(h.CurrentRpm / Mathf.Max(h.Redline * 0.7f, 1f)));
		}

		public static void RunShiftFsm(ITransmissionHost h, float dt, ref DctFsmState state, ref float fsmTimer, ref int pendingTargetGear, ref int currentGearAtStart)
		{
			h.TcMult = 1f;
			h.TccLocked = false;
			switch (state)
			{
			case DctFsmState.Steady:
			{
				h.ThrottleOverride = 1f;
				bool flag4 = h.AccelWant < 0.02f && MtTransmission.IsAboutToStall(h);
				bool flag5 = h.Brake > 0.5f && h.Velocity < 1f && h.GearboxState != GearboxState.Burnout;
				if (h.GearboxState == GearboxState.Burnout)
				{
					h.Clutch = BurnoutSlipFloor(h);
					h.ClutchB = 1f;
				}
				else if (flag4 | flag5)
				{
					h.Clutch = 1f;
					h.ClutchB = 1f;
				}
				else
				{
					float num9 = Mathf.Abs(h.WDiff * h.Ratio);
					float num10 = Mathf.Clamp01((h.WEngine - num9) / Mathf.Max(h.WEngine, 1f));
					float f = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(num10 / 0.3f));
					float num11 = 1f - Mathf.Sqrt(f);
					num11 *= Mathf.Clamp01(1f - h.Velocity / 5f);
					if (h.AccelWant > 0.05f && h.Velocity < 5f && h.WEngine > num9)
					{
						num11 = Mathf.Max(num11, 1f - PhysMath.LaunchTorqueEngagementCap(h.EngineTorque, h.ClutchCapacity));
					}
					float tallGearScale = PhysMath.TallGearLaunchScale(h.GearRatio(h.CurrentGear), h.GearRatio(2), 0.3f);
					float launchTargetRpm = PhysMath.LaunchTargetRpm(h.IdleRpm, h.Redline, h.AccelWant, tallGearScale);
					if (h.AccelWant > 0.05f)
					{
						float a = BurnoutSlipFloor(h) * Mathf.Clamp01(1f - h.Velocity / 5f);
						float t2 = PhysMath.LaunchFlare(syncRpm: Mathf.Abs(h.WDiff * h.Ratio) * (30f / (float)Math.PI), engineRpm: h.CurrentRpm, idleRpm: h.IdleRpm, redline: h.Redline, throttle: h.AccelWant, tallGearScale: tallGearScale);
						float b = Mathf.Lerp(a, 0.85f, t2);
						num11 = Mathf.Max(num11, b);
					}
					float num12 = PhysMath.LaunchTakeUp01(h.CurrentRpm, launchTargetRpm, h.AccelWant);
					num11 *= 1f - num12;
					float num13 = Mathf.Lerp(4f, 20f, num12);
					h.Clutch = Mathf.MoveTowards(h.Clutch, num11, num13 * dt);
					h.ClutchB = 1f;
				}
				if (h.TargetGear != h.CurrentGear)
				{
					pendingTargetGear = Mathf.Clamp(h.TargetGear, 0, h.NumGears + 1);
					currentGearAtStart = h.CurrentGear;
					fsmTimer = 0f;
					if (pendingTargetGear == h.DormantGear)
					{
						state = DctFsmState.Handover;
						break;
					}
					h.DormantGear = pendingTargetGear;
					state = DctFsmState.PreShift;
				}
				break;
			}
			case DctFsmState.PreShift:
			{
				h.ThrottleOverride = 1f;
				bool num6 = h.AccelWant < 0.02f && MtTransmission.IsAboutToStall(h);
				bool flag3 = h.Brake > 0.5f && h.Velocity < 1f;
				if (num6 | flag3)
				{
					h.Clutch = 1f;
					h.ClutchB = 1f;
				}
				else
				{
					h.Clutch = 0f;
					h.ClutchB = 1f;
				}
				int num7 = Mathf.Clamp(h.TargetGear, 0, h.NumGears + 1);
				if (num7 != pendingTargetGear)
				{
					pendingTargetGear = num7;
				}
				h.DormantGear = pendingTargetGear;
				float num8 = h.Profile.DctPreShiftMs * 0.001f;
				fsmTimer += dt;
				if (fsmTimer >= num8)
				{
					state = DctFsmState.Handover;
					fsmTimer = 0f;
				}
				break;
			}
			case DctFsmState.Handover:
			{
				h.ThrottleOverride = 1f;
				float num = h.Profile.DctHandoverMs * 0.001f;
				float t = ((num > 0f) ? Mathf.Clamp01(fsmTimer / num) : 1f);
				bool num2 = h.AccelWant < 0.02f && MtTransmission.IsAboutToStall(h);
				bool flag = h.Brake > 0.5f && h.Velocity < 1f;
				if (num2 | flag)
				{
					h.Clutch = 1f;
					h.ClutchB = 1f;
				}
				else
				{
					h.Clutch = Mathf.Lerp(0f, 1f, t);
					float num3 = Mathf.Lerp(1f, 0f, t);
					h.ClutchB = ((h.Velocity < 3f) ? Mathf.MoveTowards(h.ClutchB, num3, 2f * dt) : num3);
				}
				fsmTimer += dt;
				bool flag2 = num2 | flag;
				if (fsmTimer >= num && ((h.ClutchB <= 0.01f) | flag2))
				{
					int num4 = (h.CurrentGear = h.DormantGear);
					int num5 = Math.Sign(num4 - currentGearAtStart);
					if (num5 == 0)
					{
						num5 = 1;
					}
					h.DormantGear = Mathf.Clamp(num4 + num5, 2, h.NumGears + 1);
					h.Clutch = h.ClutchB;
					h.ClutchB = 1f;
					state = DctFsmState.Steady;
					fsmTimer = 0f;
				}
				break;
			}
			}
			if (state != DctFsmState.Steady && pendingTargetGear < currentGearAtStart && h.AccelWant < 0.05f)
			{
				float num14 = PhysMath.RevMatchTargetRpm(Mathf.Abs(h.WDiff), h.GearRatio(pendingTargetGear), h.FinalDrive, h.Redline, 1.08f);
				if (h.CurrentRpm < num14 - 150f)
				{
					h.RevMatchBlip = 1f;
				}
			}
			float b2 = MtTransmission.AntiStallOpening(h);
			h.Clutch = Mathf.Max(h.Clutch, b2);
			h.ClutchB = Mathf.Max(h.ClutchB, b2);
			MtTransmission.ApplyStallPolicy(h);
		}
	}
}
