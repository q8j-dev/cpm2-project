using UnityEngine;

namespace CPMEngine
{
	public class EngineThermal
	{
		public const float ThermostatOpensC = 85f;

		public const float ThermostatFullOpenC = 95f;

		public const float OperatingC = 90f;

		public const float FanOnC = 95f;

		public const float FanOffC = 90f;

		public const float MaxC = 130f;

		public const float HeatToCoolant = 1f;

		public const float IdleHeatKwPerLitrePer1000Rpm = 1.9f;

		public const float AirflowBase = 1f;

		public const float AirflowPerMps = 0.15f;

		public const float AirflowFan = 1.5f;

		public const float BlockCoolingWPerK = 8f;

		public const float HeatCapacityKjPerKPerLitre = 9f;

		public const float MaxTickSeconds = 0.1f;

		public const float MinDisplacementCc = 1000f;

		public const float CoolingCapacityPerNm = 0.25f;

		public const float CoolingCapacityUnrated = 200f;

		public float TemperatureC { get; private set; } = 90f;

		public bool FanOn { get; private set; }

		public void Reset(float ambientC)
		{
			TemperatureC = ambientC;
			FanOn = false;
		}

		public void Tick(in EngineThermalInputs inputs, float dt)
		{
			if (!(dt <= 0f))
			{
				dt = Mathf.Min(dt, 0.1f);
				float num = Mathf.Max(inputs.DisplacementCc, 1000f) * 0.001f;
				float num2 = 9f * num * 1000f;
				float num3 = 0f;
				if (inputs.EngineRunning)
				{
					float num4 = Mathf.Max(inputs.EnginePowerKw, 0f) * 1000f;
					float num5 = 1.9f * num * (Mathf.Max(inputs.Rpm, 0f) / 1000f) * 1000f;
					num3 = 1f * num4 + num5;
				}
				UpdateFan(inputs.EngineRunning);
				float num6 = 1f + 0.15f * Mathf.Abs(inputs.SpeedMps) + (FanOn ? 1.5f : 0f);
				float num7 = Mathf.InverseLerp(85f, 95f, TemperatureC);
				float num8 = (Mathf.Max(inputs.CoolingCapacity, 0f) * num7 * num6 + 8f) * (TemperatureC - inputs.AmbientC);
				TemperatureC += (num3 - num8) / num2 * dt;
				TemperatureC = Mathf.Clamp(TemperatureC, inputs.AmbientC, 130f);
			}
		}

		public static bool NextFanState(bool fanOn, float temperatureC, bool engineRunning)
		{
			if (!engineRunning)
			{
				return false;
			}
			if (temperatureC >= 95f)
			{
				return true;
			}
			if (temperatureC <= 90f)
			{
				return false;
			}
			return fanOn;
		}

		private void UpdateFan(bool engineRunning)
		{
			FanOn = NextFanState(FanOn, TemperatureC, engineRunning);
		}

		public float Gauge01(float ambientC)
		{
			if (TemperatureC <= 90f)
			{
				return 0.5f * Mathf.InverseLerp(ambientC, 90f, TemperatureC);
			}
			return Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(90f, 130f, TemperatureC));
		}

		public static float CoolingCapacityFromRating(float ratingNm)
		{
			if (!(ratingNm < 0f))
			{
				return Mathf.Max(ratingNm, 0f) * 0.25f;
			}
			return 200f;
		}
	}
}
