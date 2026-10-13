using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

namespace CPMEngine
{
	public static class LsdSimulator
	{
		private const float Min1WayBias = 1.2f;

		private const float Max1WayBias = 2f;

		private const float Min15WayAccBias = 1.2f;

		private const float Max15WayAccBias = 2f;

		private const float Min15WayDeAccBias = 1.1f;

		private const float Max15WayDeAccBias = 1.5f;

		private const float Min2WayBias = 1.2f;

		private const float Max2WayBias = 2f;

		private const float MinElectronicBias = 1f;

		internal const float LockPercentScale = 30f;

		internal const float DefaultLockDeadZone = 4f;

		internal const float FullLockPercent = 100f;

		private const float ClutchSlipDeadZone = 2.5f;

		private const float ClutchSlipRamp = 8f;

		private const float TorsenSlipDeadZone = 2f;

		private const float TorsenSlipRamp = 8f;

		private const float ActiveMDeadZone = 4f;

		public static void UpdateLSD(float load, float axleTorque, float leftWheel, float rightWheel, LSDValues lsdValues, out float leftTd, out float rightTd)
		{
			if (lsdValues.LockPercent >= 100f)
			{
				leftTd = 0f;
				rightTd = 0f;
				return;
			}
			switch (lsdValues.Type)
			{
			case LSDType.Open:
				OpenDifferential(leftWheel, rightWheel, out leftTd, out rightTd);
				break;
			case LSDType.Way1:
				Lsd1Way(load, axleTorque, leftWheel, rightWheel, out leftTd, out rightTd, lsdValues.AccelBias);
				break;
			case LSDType.Way15:
				Lsd1_5Way(load, axleTorque, leftWheel, rightWheel, out leftTd, out rightTd, lsdValues.AccelBias, lsdValues.BrakeBias);
				break;
			case LSDType.Way2:
				Lsd2Way(axleTorque, leftWheel, rightWheel, out leftTd, out rightTd, lsdValues.AccelBias);
				break;
			case LSDType.Electronic:
				ElectronicLsd(axleTorque, leftWheel, rightWheel, out leftTd, out rightTd, lsdValues.MaxBias);
				break;
			case LSDType.Locked:
				LockedDifferential(out leftTd, out rightTd);
				break;
			case LSDType.Torsen:
				TorsenLSD(leftWheel, rightWheel, axleTorque, out leftTd, out rightTd, lsdValues.TorqueBiasRatio);
				break;
			case LSDType.ActiveM:
				ActiveMLSD(leftWheel, rightWheel, out leftTd, out rightTd, lsdValues.MaxResistance);
				break;
			default:
				OpenDifferential(leftWheel, rightWheel, out leftTd, out rightTd);
				break;
			}
		}

		internal static float LockDeadZone(LSDValues lsd)
		{
			if (lsd.Type != LSDType.Locked && !(lsd.LockPercent >= 100f))
			{
				return 4f;
			}
			return 0f;
		}

		internal static float ContractedDiff(float diff, LSDValues lsd, float effInertia, float dt)
		{
			if (lsd == null || lsd.LockPercent <= 0f)
			{
				return diff;
			}
			float num = LockDeadZone(lsd);
			float num2 = Mathf.Abs(diff);
			if (num2 <= num)
			{
				return diff;
			}
			float num3 = num2 - num;
			float num4 = lsd.LockPercent * 30f;
			float num5 = Mathf.Max(effInertia, 0.01f);
			float num6 = 1f / (1f + 2f * num4 * dt / num5);
			return (num + num3 * num6) * Mathf.Sign(diff);
		}

		private static void OpenDifferential(float left, float right, out float resistanceLeft, out float resistanceRight)
		{
			resistanceRight = (resistanceLeft = Mathf.Clamp((right - left) * 0.3f, -0.3f, 0.3f));
		}

		private static void LockedDifferential(out float resistanceLeft, out float resistanceRight)
		{
			resistanceLeft = 0f;
			resistanceRight = 0f;
		}

		private static float ClutchTransfer(float axleTorque, float bias, float diff)
		{
			float num = Mathf.Max(bias, 1f);
			float num2 = Mathf.Abs(axleTorque) * 0.5f * (num - 1f) / (num + 1f);
			float num3 = Mathf.Clamp01((Mathf.Abs(diff) - 2.5f) / 8f);
			return num2 * num3 * Mathf.Sign(diff);
		}

		private static void Lsd1Way(float load, float axleTorque, float left, float right, out float resistanceLeft, out float resistanceRight, float bias = 1.5f)
		{
			float bias2 = ((load > 0f) ? Mathf.Clamp(bias, 1.2f, 2f) : 1f);
			resistanceRight = (resistanceLeft = ClutchTransfer(axleTorque, bias2, right - left));
		}

		private static void Lsd1_5Way(float load, float axleTorque, float left, float right, out float resistanceLeft, out float resistanceRight, float biasAccel = 1.5f, float biasDecel = 1.2f)
		{
			float bias = ((load > 0f) ? Mathf.Clamp(biasAccel, 1.2f, 2f) : Mathf.Clamp(biasDecel, 1.1f, 1.5f));
			resistanceRight = (resistanceLeft = ClutchTransfer(axleTorque, bias, right - left));
		}

		private static void Lsd2Way(float axleTorque, float left, float right, out float resistanceLeft, out float resistanceRight, float bias = 1.5f)
		{
			float bias2 = Mathf.Clamp(bias, 1.2f, 2f);
			resistanceRight = (resistanceLeft = ClutchTransfer(axleTorque, bias2, right - left));
		}

		private static void ElectronicLsd(float axleTorque, float left, float right, out float resistanceLeft, out float resistanceRight, float maxBias = 2f)
		{
			float num = right - left;
			float bias = Mathf.Lerp(1f, maxBias, Mathf.Clamp01(Mathf.Abs(num) / 6f));
			resistanceRight = (resistanceLeft = ClutchTransfer(axleTorque, bias, num));
		}

		private static void TorsenLSD(float left, float right, float axleTorque, out float torqueLeft, out float torqueRight, float torqueBiasRatio = 3f)
		{
			float num = Mathf.Max(torqueBiasRatio, 1f);
			float num2 = Mathf.Abs(axleTorque) * 0.5f * (num - 1f) / (num + 1f);
			float f = right - left;
			float num3 = Mathf.Clamp01((Mathf.Abs(f) - 2f) / 8f);
			torqueRight = (torqueLeft = num2 * num3 * Mathf.Sign(f));
		}

		private static void ActiveMLSD(float left, float right, out float resistanceLeft, out float resistanceRight, float maxResistance = 30f)
		{
			float f = right - left;
			float num = Mathf.Abs(f);
			if (num <= 4f)
			{
				resistanceLeft = 0f;
				resistanceRight = 0f;
			}
			else
			{
				resistanceRight = (resistanceLeft = Mathf.Clamp((num - 4f) * Mathf.Sign(f) * 8f, 0f - maxResistance, maxResistance));
			}
		}
	}
}
