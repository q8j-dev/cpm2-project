using System;
using UnityEngine;

namespace CPMEngine
{
	public static class CenterDiffSimulator
	{
		public const float LockedViscousCoef = 200f;

		public const float OpenViscousCoef = 25f;

		public const float OpenMaxCouplingNm = 80f;

		public const float CenterLockStiffness = 500f;

		public const float ViscousCenterDeadZone = 1f;

		public const float MaxGeometrySteer = (float)Math.PI / 4f;

		public const float SlipTransferDeadZone = 1.5f;

		public const float SlipTransferGain = 0.05f;

		public static float CouplingTorque(bool centerOpen, float rearDiffSpeed, float frontDiffSpeed)
		{
			float num = rearDiffSpeed - frontDiffSpeed;
			if (!centerOpen)
			{
				return num * 200f;
			}
			return Mathf.Clamp(num * 25f, -80f, 80f);
		}

		public static float LockedCenterDeadZone(float rearAxleSpeed, float frontAxleSpeed, float steerAngleRad)
		{
			float f = Mathf.Min(Mathf.Abs(steerAngleRad), (float)Math.PI / 4f);
			float num = 1f / Mathf.Cos(f) - 1f;
			return Mathf.Min(Mathf.Abs(rearAxleSpeed), Mathf.Abs(frontAxleSpeed)) * num;
		}

		public static float ContractedAxleDelta(float delta, float deadZone, float effInertia, float dt)
		{
			float num = Mathf.Abs(delta);
			if (num <= deadZone)
			{
				return delta;
			}
			float num2 = Mathf.Max(effInertia, 0.01f);
			float num3 = 1f / (1f + 1000f * dt / num2);
			float num4 = num - deadZone;
			return (deadZone + num4 * num3) * Mathf.Sign(delta);
		}

		public static float FrontTorqueShare(float torqueSplit, float frontAxleSlipVel, float rearAxleSlipVel)
		{
			float f = Mathf.Abs(rearAxleSlipVel) - Mathf.Abs(frontAxleSlipVel);
			float num = Mathf.Max(Mathf.Abs(f) - 1.5f, 0f) * Mathf.Sign(f) * 0.05f;
			return Mathf.Clamp01(torqueSplit + num);
		}

		public static void SplitDriveTorque(float driveTorque, float frontShare, float couplingTorque, out float frontAxleTorque, out float rearAxleTorque)
		{
			frontAxleTorque = driveTorque * frontShare + couplingTorque;
			rearAxleTorque = driveTorque * (1f - frontShare) - couplingTorque;
		}
	}
}
