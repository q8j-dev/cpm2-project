using System;
using CPM.Transmission;
using UnityEngine;

namespace CPMEngine
{
	public static class MtTransmission
	{
		public const float StopVelocity = 2f;

		public const float StopClutchRpmMargin = 300f;

		public static float WheelMatchedRpm(ITransmissionHost h)
		{
			return Mathf.Abs(h.WDiff * h.Ratio * (30f / (float)Math.PI));
		}

		public static bool IsAboutToStall(ITransmissionHost h)
		{
			if (Mathf.Abs(h.Velocity) < 2f)
			{
				return true;
			}
			if (Mathf.Abs(h.Ratio) > 0.0001f)
			{
				return WheelMatchedRpm(h) < h.IdleRpm + 300f;
			}
			return false;
		}

		public static float AntiStallOpening(ITransmissionHost h)
		{
			return PhysMath.AntiStallOpening01(h.WEngine * (30f / (float)Math.PI), h.IdleRpm, h.StallFloorRpm);
		}

		public static void ApplyStallPolicy(ITransmissionHost h)
		{
			GearboxProfile profile = h.Profile;
			h.IdleGovernorScale = 1f;
			if (!(Mathf.Abs(h.Ratio) < 0.0001f) && !h.DriveDecoupled)
			{
				float num = Mathf.Clamp01(h.ClutchEngagement);
				float num2 = h.IdleRpm * 0.104719f * profile.StallFraction;
				if (h.EngineWorks && num > profile.StallEngMin && h.WEngine < num2)
				{
					h.StallEngine();
				}
			}
		}
	}
}
