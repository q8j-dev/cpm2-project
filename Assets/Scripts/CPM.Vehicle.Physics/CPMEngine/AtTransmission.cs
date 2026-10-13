using System;
using CPM.Transmission;
using UnityEngine;

namespace CPMEngine
{
	public static class AtTransmission
	{
		private const float BrakeDownshiftFracMin = 0.25f;

		private const float BrakeDownshiftFracMax = 0.55f;

		private const float ThrottleShapeExponent = 1.5f;

		private const float SportLimiterMargin = 0.01f;

		public const float DownshiftThrottleHysteresis = 0.15f;

		public const float LimiterUpshiftHoldSec = 0.15f;

		private const float LimiterUpshiftThrottle = 0.5f;

		private const float ShiftLagFloorSec = 0.12f;

		private const float ShiftLagMinSpeedFactor = 0.75f;

		public static void SimulateClutch(ITransmissionHost h, float subDelta)
		{
			h.TcMult = 1f;
			if (!h.AutoClutch || h.GearboxState == GearboxState.Burnout)
			{
				h.CurrentGear = h.TargetGear;
				h.TccLocked = false;
				return;
			}
			if (h.Handbrake > 0.05f && h.GearType != GearboxType.GearFwd)
			{
				h.Clutch = 1f;
				h.TccLocked = false;
				if (h.GearTimer == 0f)
				{
					h.CurrentGear = h.TargetGear;
				}
				return;
			}
			if ((h.CurrentGear == 1 || h.Ratio == 0f) && h.GearTimer == 0f)
			{
				h.Clutch = 1f;
				h.TccLocked = false;
				h.CurrentGear = h.TargetGear;
				return;
			}
			float num = Mathf.Clamp01(h.AccelWant);
			float num2 = Mathf.Max(h.WEngine, h.IdleRpm * 0.104719f * 0.5f);
			float num3 = h.WDiff * h.Ratio;
			float num4 = Mathf.Clamp01(Mathf.Abs(num3) / num2);
			float kStall = h.Profile.KStall;
			float num5 = Mathf.Clamp01(1f - num4);
			h.TcMult = 1f + (kStall - 1f) * num5 * num5;
			float num6 = num3 * (30f / (float)Math.PI);
			float num7 = Mathf.Abs(num6);
			if (h.GearTimer > 0f)
			{
				h.TccLocked = false;
			}
			if (num7 >= h.TcLockupRpm)
			{
				h.TcSlipLatched = false;
			}
			else if (num7 < h.TcLockupRpm * 0.8f)
			{
				h.TcSlipLatched = true;
			}
			float num8;
			if (h.Brake > 0.3f && (h.Velocity < 5f || h.CurrentRpm <= h.IdleRpm + 100f))
			{
				num8 = 1f;
				h.TccLocked = false;
			}
			else if (num < 0.05f && h.DynorunMode)
			{
				num8 = 1f;
				h.TccLocked = false;
			}
			else if (num < 0.05f && num6 < h.IdleRpm + 100f)
			{
				num8 = 1f;
				h.TccLocked = false;
			}
			else if (num < 0.05f)
			{
				num8 = Mathf.Lerp(0.6f, 0.15f, num4 / 0.85f);
				h.TccLocked = false;
			}
			else if (h.CurrentRpm < h.IdleRpm * 0.7f)
			{
				num8 = 1f;
				h.TccLocked = false;
			}
			else
			{
				float stallCoupling = h.Profile.StallCoupling;
				if (h.TccLocked)
				{
					if (num4 < h.Profile.TccLockExitRatio || h.Velocity < h.Profile.TccLockMinVeloExit)
					{
						h.TccLocked = false;
					}
				}
				else if (num4 >= h.Profile.TccLockEnterRatio && h.Velocity >= h.Profile.TccLockMinVeloEnter)
				{
					h.TccLocked = true;
				}
				if (h.TccLocked)
				{
					num8 = 0f;
				}
				else
				{
					float num9 = num4 / h.Profile.TccLockEnterRatio;
					num8 = Mathf.Lerp(stallCoupling, 0f, num9 * num9);
					if (num > 0.7f && h.TcSlipLatched && num8 < stallCoupling)
					{
						num8 = stallCoupling;
					}
					if (num >= 0.05f && num7 < h.IdleRpm + 600f)
					{
						float num10 = Mathf.Lerp(0.55f, stallCoupling, num);
						if (num8 < num10)
						{
							num8 = num10;
						}
					}
				}
			}
			if (h.AccelWant > 0.05f && !h.TccLocked && h.Velocity < 5f && h.WEngine > Mathf.Abs(num3))
			{
				num8 = Mathf.Max(num8, 1f - PhysMath.LaunchTorqueEngagementCap(h.EngineTorque, h.ClutchCapacity));
			}
			num8 = Mathf.Max(num8, h.ClutchPedalInput);
			float num11 = (h.IsActiveLaunchControlNow ? 6f : ((h.Velocity > h.Profile.TccLockMinVeloEnter) ? 5f : 2f));
			if (num8 < h.Clutch)
			{
				h.Clutch = Mathf.MoveTowards(h.Clutch, num8, num11 * subDelta);
			}
			else if (h.Velocity < 5f && num < 0.05f && h.Brake < 0.3f)
			{
				h.Clutch = num8;
			}
			else if (num < 0.05f && h.Brake < 0.3f && h.GearTimer == 0f && !h.IsActiveLaunchControlNow)
			{
				h.Clutch = Mathf.MoveTowards(h.Clutch, num8, 2f * subDelta);
			}
			else
			{
				h.Clutch = num8;
			}
			if (float.IsNaN(h.Clutch) || float.IsInfinity(h.Clutch))
			{
				h.Clutch = 1f;
			}
			h.Clutch = Mathf.Clamp01(h.Clutch);
			if (h.GearTimer == 0f)
			{
				h.CurrentGear = h.TargetGear;
			}
			else
			{
				float num12 = 1f - h.GearTimer / h.GearShiftingDelay;
				if (!(num12 < 0.3f))
				{
					if (num12 >= 0.7f)
					{
						float num13 = 1f - (num12 - 0.7f) / 0.3f;
						h.Clutch = ((h.Velocity < 5f && num < 0.05f) ? Mathf.Max(num13, num8) : num13);
						h.CurrentGear = h.TargetGear;
					}
					else
					{
						h.Clutch = 1f;
					}
				}
				else
				{
					h.Clutch = Mathf.Max(h.Clutch, num12 / 0.3f);
				}
				if (h.TargetGear < h.CurrentGear && h.TargetGear > 1)
				{
					if (h.CurrentRpm < h.WDiff * h.TargetGearTotalRatio * (30f / (float)Math.PI))
					{
						h.RevMatchBlip = 1f;
					}
				}
				else if (h.TargetGear > h.CurrentGear && num12 < 0.7f)
				{
					h.AccelWant = 0f;
				}
			}
			if (h.Starter > 0f)
			{
				h.Clutch = 1f;
			}
		}

		private static float UpshiftThreshold(ITransmissionHost h, float idle, float redline, float rpmRange, float throttle)
		{
			return UpshiftThreshold(h.SportMode, idle, redline, rpmRange, throttle);
		}

		public static float ThrottleShape(float throttle)
		{
			return Mathf.Pow(Mathf.Clamp01(throttle), 1.5f);
		}

		public static float UpshiftThreshold(bool sportMode, float idle, float redline, float rpmRange, float throttle)
		{
			float t = ThrottleShape(throttle);
			if (!sportMode)
			{
				return Mathf.Lerp(idle + rpmRange * 0.1f, redline - rpmRange * 0.05f, t);
			}
			return Mathf.Lerp(idle + rpmRange * 0.35f, redline - rpmRange * 0.01f, t);
		}

		public static float WotUpshiftCeiling(ITransmissionHost h)
		{
			return UpshiftThreshold(h, h.IdleRpm, h.Redline, h.Redline - h.IdleRpm, 1f);
		}

		public static float DownshiftThreshold(float idle, float upshiftBase)
		{
			return Mathf.Max(upshiftBase * 0.55f, idle + 200f);
		}

		public static float DownshiftThrottle(float throttle)
		{
			return Mathf.Max(0f, throttle - 0.15f);
		}

		public static bool IsEngineAtLimiter(float currentRpm, float idle, float redline)
		{
			return currentRpm >= redline - (redline - idle) * 0.05f;
		}

		public static bool IsEngineNearLimiter(float currentRpm, float idle, float redline)
		{
			return currentRpm >= redline - (redline - idle) * 0.1f;
		}

		public static float LimiterUpshiftHold(float throttle)
		{
			if (!(throttle > 0.5f))
			{
				return 0.5f;
			}
			return 0.15f;
		}

		public static bool LimiterCancelsShiftCooldown(float currentRpm, float idle, float redline, float throttle)
		{
			if (throttle > 0.5f)
			{
				return IsEngineNearLimiter(currentRpm, idle, redline);
			}
			return false;
		}

		private static float ShiftLagSpeedFactor(ITransmissionHost h)
		{
			float num = Mathf.Abs(h.Velocity);
			if (num < 1f)
			{
				return 1f;
			}
			float num2 = Mathf.Max(0f - h.LongAccel, 0f);
			if (num2 <= 0f)
			{
				return 1f;
			}
			float num3 = Mathf.Max(h.GearShiftingDelay, 0.12f);
			return Mathf.Clamp(1f - num2 * num3 / num, 0.75f, 1f);
		}

		public static int GetOptimalGear(ITransmissionHost h, float vel, float mwDiff, int curGear, float throttle)
		{
			float idleRpm = h.IdleRpm;
			float redline = h.Redline;
			float num = redline - idleRpm;
			float drivenWheelRadius = h.DrivenWheelRadius;
			float num2 = vel / drivenWheelRadius;
			bool flag = mwDiff > num2 * 1.35f && vel > 0.5f;
			mwDiff = Mathf.Min(mwDiff, num2 * 1.5f);
			num2 = Mathf.Max(num2, mwDiff);
			float num3 = num2 * h.GearRatio(curGear + 1) * h.FinalDrive * (30f / (float)Math.PI);
			if (vel < 1.5f && curGear > 1)
			{
				return 1;
			}
			float num4 = UpshiftThreshold(h, idleRpm, redline, num, throttle);
			float num5 = Mathf.Max(throttle, h.ThrottleIntent);
			if (num5 > 0.8f && curGear > 1 && vel >= 5f && !flag)
			{
				float num6 = UpshiftThreshold(h, idleRpm, redline, num, num5) - num * 0.15f;
				if (num3 < num6)
				{
					int num7 = curGear;
					for (int num8 = curGear - 1; num8 >= 1; num8--)
					{
						float num9 = num2 * h.GearRatio(num8 + 1) * h.FinalDrive * (30f / (float)Math.PI);
						if (num9 <= num6 && num9 >= idleRpm)
						{
							num7 = num8;
						}
						else if (num9 > num6)
						{
							break;
						}
					}
					if (num7 < curGear && num2 * h.GearRatio(num7 + 1) * h.FinalDrive * (30f / (float)Math.PI) >= num3 * 1.1f)
					{
						return num7;
					}
				}
			}
			bool flag2 = h.ThrottleRecentMax - throttle > 0.15f;
			float num10 = (((h.ThrottleDropTimer > 0f || throttle < 0.05f) | flag2) ? float.MaxValue : num4);
			bool flag3 = IsEngineAtLimiter(h.CurrentRpm, idleRpm, redline);
			bool flag4 = IsEngineNearLimiter(h.CurrentRpm, idleRpm, redline);
			if (curGear < h.NumGears && !flag && h.ThrottleDropTimer == 0f && ((flag3 && h.EngineRevLimitedSustained > LimiterUpshiftHold(throttle)) || (flag4 && h.EngineRevLimitedSustained > 1.5f)))
			{
				return curGear + 1;
			}
			if (num3 >= num10 && curGear < h.NumGears && !flag)
			{
				float num11 = num2 * h.GearRatio(curGear + 2) * h.FinalDrive * (30f / (float)Math.PI);
				float num12 = Mathf.Max(idleRpm + 100f, DownshiftThreshold(idleRpm, num4) * 1.15f);
				if (num11 >= num12)
				{
					return curGear + 1;
				}
			}
			if (curGear > 1 && !flag)
			{
				if (throttle < 0.05f)
				{
					bool flag5 = h.Brake > 0.2f;
					float num13 = (flag5 ? (idleRpm + num * Mathf.Lerp(0.25f, 0.55f, Mathf.Clamp01(h.Brake))) : (idleRpm + num * 0.1f));
					if ((h.SportMode & flag5) || num3 < num13)
					{
						float num14 = num2 * h.GearRatio(curGear) * h.FinalDrive * (30f / (float)Math.PI);
						if (num14 * ShiftLagSpeedFactor(h) <= redline - 300f && num14 <= redline)
						{
							return curGear - 1;
						}
					}
				}
				else
				{
					float num15 = DownshiftThreshold(idleRpm, UpshiftThreshold(h, idleRpm, redline, num, DownshiftThrottle(throttle)));
					if (num3 < num15)
					{
						float num16 = num2 * h.GearRatio(curGear) * h.FinalDrive * (30f / (float)Math.PI);
						if (num16 <= redline - num * 0.05f && num16 <= num4)
						{
							return curGear - 1;
						}
					}
				}
			}
			return curGear;
		}

		public static int StandstillGear(bool lowMode, int lowMaxGear)
		{
			if (!lowMode)
			{
				return 1;
			}
			return Mathf.Max(1, lowMaxGear);
		}

		public static bool AppliesStandstillGoverning(GearboxKind kind)
		{
			return kind != GearboxKind.Sequential;
		}

		public static int ApplyLowCap(ITransmissionHost h, int desiredGear, int curGear)
		{
			if (!h.LowMode)
			{
				return desiredGear;
			}
			int num = Mathf.Max(1, h.LowMaxGear);
			if (curGear <= num)
			{
				return num;
			}
			float num2 = Mathf.Abs(h.WDiff) * h.GearRatio(curGear) * h.FinalDrive * (30f / (float)Math.PI);
			float num3 = h.Redline - h.IdleRpm;
			if (!(num2 <= h.Redline - num3 * 0.05f))
			{
				return curGear;
			}
			return Mathf.Min(curGear - 1, Mathf.Max(desiredGear, num));
		}
	}
}
