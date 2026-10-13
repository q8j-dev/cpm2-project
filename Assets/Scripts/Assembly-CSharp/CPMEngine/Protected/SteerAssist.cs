using System;
using UnityEngine;

namespace CPMEngine.Protected
{
	internal class SteerAssist
	{
		private const float MinSpeed = 1f;

		private const float AngularVelocityFactor = 0.1f;

		private const float SpeedDampingFactor = 0.05f;

		private const float SpeedDampingBase = 15f;

		private const float MaxSteeringRate = 3f;

		private const float CounterSteerThreshold = 1.2f;

		private const float DriftSteeringMultiplier = 10.5f;

		private const float SteeringSmoothness = 2f;

		private PhysEngine _car;

		private float _steerAng;

		private IWheel _wheelFl;

		private IWheel _wheelFr;

		private RB _rb;

		public void AttachCar(PhysEngine car)
		{
			_car = car;
			_steerAng = 0f;
			_rb = _car.RigidBody;
			_car.GetFront(out var left, out var right);
			_wheelFl = _car.GetWheel(left);
			_wheelFr = _car.GetWheel(right);
		}

		private void GetFrontAxleVelocities(out Vector3 leftPosInv, out Vector3 rightPosInv)
		{
			Vector3 topPos = _wheelFl.TopPos;
			Vector3 topPos2 = _wheelFr.TopPos;
			Vector3 pos = _rb.Matrix.MultiplyPoint3x4(topPos);
			Vector3 pos2 = _rb.Matrix.MultiplyPoint3x4(topPos2);
			Vector3 velocity = _rb.GetVelocity(pos);
			Vector3 velocity2 = _rb.GetVelocity(pos2);
			leftPosInv = _rb.InvMatrix.MultiplyVector(velocity);
			rightPosInv = _rb.InvMatrix.MultiplyVector(velocity2);
		}

		private float GetFrontAxleForwardSpeed()
		{
			GetFrontAxleVelocities(out var leftPosInv, out var rightPosInv);
			return ((leftPosInv + rightPosInv) * 0.5f).z;
		}

		private void GetGripEnvelope(out float center, out float peak)
		{
			float caster = _wheelFl.Caster;
			float caster2 = _wheelFr.Caster;
			GetFrontAxleVelocities(out var leftPosInv, out var rightPosInv);
			float a = Mathf.Abs(_wheelFl.BestSlipAngle);
			float b = Mathf.Abs(_wheelFr.BestSlipAngle);
			float a2 = Mathf.Max(a, b);
			peak = Mathf.Max(a2, 6f) * ((float)Math.PI / 180f) * 1.2f;
			float num = leftPosInv.z;
			float num2 = leftPosInv.x;
			float num3 = rightPosInv.z;
			float num4 = rightPosInv.x;
			if (num < 0f)
			{
				num2 = 0f - num2;
				num = 0f - num;
			}
			if (num3 < 0f)
			{
				num4 = 0f - num4;
				num3 = 0f - num3;
			}
			float num5 = (0f - Mathf.Atan2(num2, num)) / Mathf.Cos(caster * ((float)Math.PI / 180f));
			float num6 = (0f - Mathf.Atan2(num4, num3)) / Mathf.Cos(caster2 * ((float)Math.PI / 180f));
			center = (num5 + num6) * 0.5f;
		}

		private void GetOptimalSteerRange(out float angMin, out float angMax)
		{
			GetGripEnvelope(out var center, out var peak);
			if (Mathf.Abs(GetFrontAxleForwardSpeed()) >= 5f && _car.Gear != -1)
			{
				angMin = center - peak;
				angMax = center + peak;
			}
			else
			{
				angMin = 0f - _car.MaxSteerAngle;
				angMax = _car.MaxSteerAngle;
			}
		}

		public void UpdateSteering(float inputSteer, float dt, bool driftMode = false)
		{
			if (_car == null)
			{
				return;
			}
			float num = 1f;
			float num2 = 1f;
			bool flag = false;
			if (inputSteer > 0f && inputSteer < 1f)
			{
				num = inputSteer;
				inputSteer = 1f;
				flag = true;
			}
			if (inputSteer < 0f && inputSteer > -1f)
			{
				num2 = 0f - inputSteer;
				inputSteer = -1f;
				flag = true;
			}
			float steerAng = _steerAng;
			float num3 = 0f - _rb.AngularVelocity.y;
			float num4 = Mathf.Max(_rb.Velocity.magnitude, 1f);
			float num5 = 7f / (num4 * 0.05f + 15f);
			float num6 = num4 / (num4 * num4 * 0.1f);
			GetOptimalSteerRange(out var angMin, out var angMax);
			if (flag)
			{
				angMin -= num3 * 0.1f;
				angMax -= num3 * 0.1f;
			}
			float num7 = (angMax - angMin) * 0.5f;
			float num8 = (angMax + angMin) * 0.5f;
			angMax = num8 + num7 * num;
			angMin = num8 - num7 * num2;
			float num9 = num6 * inputSteer;
			float num10 = 0f - _car.AngAccel;
			float value = 0f;
			if (Mathf.Abs(num10) > 0.001f)
			{
				value = (num9 - num3) / num10;
			}
			value = Mathf.Clamp(value, 0f, 0.2f);
			float num11 = num3 + num10 * value;
			float num12 = Mathf.Abs(num9 - num11);
			float num13 = 1f + num12 * 1f;
			if (num13 > 3f)
			{
				num13 = 3f;
			}
			float num14 = Mathf.Abs(inputSteer);
			if (inputSteer > 0f)
			{
				if (num11 < num9)
				{
					float num15 = num5 * num13;
					num5 = num14 * num15 + (1f - num14) * num5;
				}
				if (num11 > num9 * 1.2f)
				{
					angMax = Mathf.Min(angMax, num7 * 0.8f);
				}
			}
			if (inputSteer < 0f)
			{
				if (num11 > num9)
				{
					float num16 = num5 * num13;
					num5 = num14 * num16 + (1f - num14) * num5;
				}
				if (num11 < num9 * 1.2f)
				{
					angMin = Mathf.Max(angMin, (0f - num7) * 0.8f);
				}
			}
			inputSteer *= num5;
			float num17 = 2f;
			if (!driftMode)
			{
				float num18 = Mathf.Sqrt(Mathf.Abs((num9 - num3) / num6));
				if (num18 > 1f)
				{
					inputSteer *= num18;
				}
				if (num18 < 1.5f)
				{
					inputSteer *= 0.5f;
					if (_steerAng < angMax * 0.5f && _steerAng > angMin * 0.5f)
					{
						num17 = 1f;
					}
				}
			}
			else
			{
				num17 = num5 * 10.5f;
			}
			_steerAng += inputSteer * dt;
			if (Mathf.Abs(inputSteer) < 0.001f)
			{
				float num19 = num17 * dt;
				float num20 = num8 * 0.3f;
				if (_steerAng > num19 + num20)
				{
					_steerAng -= num19;
				}
				else if (_steerAng >= 0f - num19 + num20)
				{
					_steerAng = num20;
				}
				else
				{
					_steerAng += num19;
				}
			}
			if (flag)
			{
				float steerAng2 = ((inputSteer > 0f) ? (num8 + num7 * num) : ((!(inputSteer < 0f)) ? _steerAng : (num8 - num7 * num2)));
				_steerAng = steerAng2;
			}
			_steerAng = Mathf.Clamp(_steerAng, angMin, angMax);
			_steerAng = Mathf.Clamp(_steerAng, 0f - _car.MaxSteerAngle, _car.MaxSteerAngle);
			float num21 = 2f * dt;
			if (flag)
			{
				num21 *= 2f;
			}
			_steerAng = Mathf.Clamp(_steerAng, steerAng - num21, steerAng + num21);
			SteerAbsolute(_steerAng);
		}

		private void SteerAbsolute(float ang)
		{
			if (_car != null)
			{
				_steerAng = ang;
				_car.SteerAngle = _steerAng;
			}
		}

		public void SetNormalizedSteering(float par)
		{
			if (_car != null)
			{
				SteerAbsolute(par * _car.MaxSteerAngle);
			}
		}
	}
}
