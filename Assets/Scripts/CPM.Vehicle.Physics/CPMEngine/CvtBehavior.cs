using System;
using CPM.Transmission;
using UnityEngine;

namespace CPMEngine
{
	public sealed class CvtBehavior : ITransmissionBehavior
	{
		private int _vGear;

		public void Tick(ITransmissionHost h, float dt)
		{
			AtTransmission.SimulateClutch(h, dt);
			if (h.TargetGear < 2)
			{
				h.RatioOverride = float.NaN;
				_vGear = 0;
				return;
			}
			GearboxProfile profile = h.Profile;
			int num = Mathf.Max(2, profile.CvtVirtualGears);
			float num2 = h.GearRatio(2) * h.FinalDrive;
			float num3 = h.GearRatio(h.NumGears + 1) * h.FinalDrive;
			_vGear = Mathf.Clamp(_vGear, 0, num - 1);
			if (h.LowMode)
			{
				_vGear = Mathf.Clamp(h.LowMaxGear - 1, 0, num - 1);
				h.RatioOverride = VirtualRatio(_vGear, num, num2, num3);
				return;
			}
			float a = (h.SportMode ? profile.CvtSportIdleFrac : profile.CvtUpshiftIdleFrac);
			float b = (h.SportMode ? profile.CvtSportUpshiftPowerFrac : profile.CvtUpshiftPowerFrac);
			float num4 = Mathf.Min(b: Mathf.Pow(num3 / num2, 1f / (float)Mathf.Max(num - 1, 1)) * 0.9f, a: h.SportMode ? profile.CvtSportDownshiftHyst : profile.CvtDownshiftHysteresis);
			float t = Mathf.Clamp01(h.Throttle);
			float num5 = Mathf.Lerp(a, b, t) * h.Redline;
			float num6 = num5 * num4;
			float num7 = Mathf.Abs(h.WDiff);
			float num8 = num7 * VirtualRatio(_vGear, num, num2, num3) * (30f / (float)Math.PI);
			float num9 = h.Velocity / Mathf.Max(h.DrivenWheelRadius, 0.01f);
			bool flag = num7 > num9 * 1.35f + 1f;
			int num10 = num - 1;
			if (num8 >= num5 && _vGear < num10 && !flag)
			{
				_vGear++;
			}
			else if (num8 <= num6 && _vGear > 0)
			{
				_vGear--;
			}
			h.RatioOverride = VirtualRatio(_vGear, num, num2, num3);
		}

		private static float VirtualRatio(int gear, int steps, float ratioMax, float ratioMin)
		{
			if (steps <= 1 || ratioMax <= 0f)
			{
				return ratioMax;
			}
			float p = (float)gear / (float)(steps - 1);
			return ratioMax * Mathf.Pow(ratioMin / ratioMax, p);
		}

		public bool WantsShift(ITransmissionHost h, out int gear)
		{
			gear = h.TargetGear - 1;
			return false;
		}
	}
}
