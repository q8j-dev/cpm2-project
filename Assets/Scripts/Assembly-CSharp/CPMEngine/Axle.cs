using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace CPMEngine
{
	[Serializable]
	public class Axle
	{
		private const float T0 = 400f;

		private const float T1 = 650f;

		private const float T2 = 900f;

		private const float T3 = 1300f;

		public WheelProvider LeftWheel;

		public WheelProvider RightWheel;

		public List<WheelProvider> Wheels;

		private float2 _suspensionRate = new float2(20000f, 20000f);

		private bool _airUpTrigger;

		private float _brakeDiskTemperature = 20f;

		private BrakeDiscType _brakeDiscType = BrakeDiscType.SOLID;

		private float _brakeDiskMass = 5f;

		private float _brakeDiskTorque = 1500f;

		private const float BrakeWetCoolingMul = 2.5f;

		public float CurrentSpringLength { get; set; } = 0.2f;

		public float SpringLength { get; set; } = 0.2f;

		public float TargetClearance { get; set; } = 0.2f;

		public float Sag { get; set; }

		public float HeightAdjustSpeed { get; set; } = 0.1f;

		public float MinSpringLenLimit { get; set; }

		public float MaxSpringLenLimit { get; set; } = 0.2f;

		private bool AirUpTrigger
		{
			get
			{
				return _airUpTrigger;
			}
			set
			{
				if (_airUpTrigger != value)
				{
					_airUpTrigger = value;
					if (_airUpTrigger && AudioWrapper.IsInitialized())
					{
						OnPneumaticUp?.Invoke();
					}
				}
			}
		}

		public float AirPressure { get; set; } = 100f;

		public float PressureLossRate { get; set; } = 0.1f;

		public float PressureRegainRate { get; set; } = 0.5f;

		public float GetAvrWheelsSpeed => CalculateAvrSpeed();

		public Color GetBrakeDiskHeatColor
		{
			get
			{
				float num = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(650f, 1300f, _brakeDiskTemperature));
				return TemperatureToColor(_brakeDiskTemperature) * num;
			}
		}

		public event Action<float> OnPneumaticDown;

		public event Action OnPneumaticUp;

		public void SetSpring(float3 specs, float sag, float minSpringLenLimit, float maxSpringLenLimit)
		{
			SpringLength = specs.x;
			Sag = sag;
			MinSpringLenLimit = minSpringLenLimit;
			MaxSpringLenLimit = maxSpringLenLimit;
			_suspensionRate.x = specs.y;
			_suspensionRate.y = specs.y;
			float num = AirSuspensionMath.SpringLenToClearance(maxSpringLenLimit, sag);
			TargetClearance = ((specs.z >= 0f) ? Mathf.Clamp(specs.z, 0f, num) : Mathf.Min(AirSuspensionMath.SpringLenToClearance(specs.x, sag), num));
			CurrentSpringLength = AirSuspensionMath.ClearanceToSpringLen(TargetClearance, Sag, maxSpringLenLimit);
		}

		public void UpdateSuspension(Rigidbody body)
		{
			UpdateAirPressure(body);
			float num = AirSuspensionMath.ClearanceToSpringLen(TargetClearance, Sag, MaxSpringLenLimit);
			CurrentSpringLength = ((num > CurrentSpringLength) ? Mathf.Lerp(CurrentSpringLength, num, HeightAdjustSpeed * Time.deltaTime) : Mathf.Lerp(CurrentSpringLength, num, HeightAdjustSpeed * Time.deltaTime * 5f));
			foreach (WheelProvider wheel in Wheels)
			{
				wheel.Wheel.MaxSpringLen = CurrentSpringLength;
				wheel.Wheel.SpringCoef = _suspensionRate.y;
			}
			HandlePneumaticDownSound(num);
		}

		private void HandlePneumaticDownSound(float targetSpringLength)
		{
			if (AudioWrapper.IsInitialized())
			{
				if (CurrentSpringLength - targetSpringLength > 0.001f)
				{
					float obj = Mathf.Clamp01(1f - targetSpringLength / CurrentSpringLength);
					OnPneumaticDown?.Invoke(obj);
					AirUpTrigger = false;
				}
				else
				{
					AirUpTrigger = true;
				}
			}
		}

		private void UpdateAirPressure(Rigidbody body)
		{
			if (body.velocity.magnitude > 1f)
			{
				AirPressure -= PressureLossRate * Time.deltaTime;
			}
			else
			{
				AirPressure += PressureRegainRate * Time.deltaTime;
			}
			AirPressure = Mathf.Clamp(AirPressure, 90f, 110f);
			_suspensionRate.y = _suspensionRate.x * (AirPressure / 100f);
		}

		public void SetBrakeDisk(BrakeDiscType type, float mass, float torq)
		{
			_brakeDiscType = type;
			_brakeDiskMass = mass;
			_brakeDiskTorque = torq;
		}

		public float CalculateAvrSpeed()
		{
			float num = 0f;
			foreach (WheelProvider wheel in Wheels)
			{
				if ((bool)wheel)
				{
					num += Utils.GetFixedVariable(Mathf.Abs(wheel.Wheel.AngularVelocity) * wheel.Wheel.Radius);
				}
			}
			if (Wheels.Count > 0)
			{
				num /= (float)Wheels.Count;
			}
			return num;
		}

		public void ApplyBrakingForce(float speedMps, float brakeBalance, float brakeInput)
		{
			if (Wheels.Count != 0)
			{
				float num = Mathf.Max(Wheels[0].Wheel.Radius, 0.1f);
				float angularSpeed = speedMps / num;
				_brakeDiskTemperature = PhysMath.BrakeHeatStep(_brakeDiskTemperature, _brakeDiskTorque, brakeBalance, brakeInput, angularSpeed, _brakeDiscType, _brakeDiskMass, 25f, Time.deltaTime, CalculateGripTorque(num));
			}
		}

		private float CalculateGripTorque(float radius)
		{
			float num = 0f;
			int num2 = 0;
			foreach (WheelProvider wheel in Wheels)
			{
				if (!(wheel == null) && wheel.Wheel != null)
				{
					num += wheel.Wheel.NormalForce;
					num2++;
				}
			}
			if (num2 == 0)
			{
				return 0f;
			}
			return 1.2f * (num / (float)num2) * radius;
		}

		public void CoolingToDefault()
		{
			_brakeDiskTemperature = 25f;
		}

		public void CoolDownBrake(float speed)
		{
			float wetCoolingMul = 1f + 1.5f * WeatherPhysics.Wetness;
			_brakeDiskTemperature = PhysMath.BrakeCoolStep(_brakeDiskTemperature, speed, _brakeDiscType, 25f, Time.deltaTime, wetCoolingMul);
		}

		private Color TemperatureToColor(float temperature)
		{
			Color rgbColor = new Color32(byte.MaxValue, 170, 0, byte.MaxValue);
			if (temperature <= 400f)
			{
				return Color.black;
			}
			if (temperature <= 650f)
			{
				float t = (temperature - 400f) / 250f;
				return Color.Lerp(Color.black, Color.red, t);
			}
			if (temperature <= 900f)
			{
				float t2 = (temperature - 650f) / 250f;
				Color.RGBToHSV(Color.red, out var H, out var S, out var V);
				Color.RGBToHSV(rgbColor, out var H2, out var S2, out var V2);
				float h = Mathf.LerpAngle(H, H2, t2);
				float s = Mathf.Lerp(S, S2, t2);
				float v = Mathf.Lerp(V, V2, t2);
				return Color.HSVToRGB(h, s, v);
			}
			if (temperature <= 1300f)
			{
				float t3 = (temperature - 900f) / 400f;
				Color.RGBToHSV(rgbColor, out var H3, out var S3, out var V3);
				Color.RGBToHSV(Color.white, out var H4, out var S4, out var V4);
				float h2 = Mathf.LerpAngle(H3, H4, t3);
				float s2 = Mathf.Lerp(S3, S4, t3);
				float v2 = Mathf.Lerp(V3, V4, t3);
				return Color.HSVToRGB(h2, s2, v2);
			}
			return Color.white;
		}

		public void UpdateTyreHealth(out float left, out float right)
		{
			left = 0f;
			right = 0f;
			if ((bool)LeftWheel)
			{
				left = LeftWheel.Wheel.TyreHealth;
			}
			if ((bool)RightWheel)
			{
				right = RightWheel.Wheel.TyreHealth;
			}
		}
	}
}
