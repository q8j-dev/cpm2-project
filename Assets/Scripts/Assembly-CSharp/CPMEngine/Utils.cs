using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using EasyHaptic_EvilBurgers;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.NewUI;

namespace CPMEngine
{
	public static class Utils
	{
		public const float AirDensity = 1.29f;

		public const float MaxTireTemp = 250f;

		public const float SlippingTireDeteriorateRatio = 0.0001f;

		public const float RotationTireDeteriorateRatio = 5E-06f;

		public const float SlipVeloThreshold = 5f;

		public const float AirTemperature = 25f;

		public const float ReferenceDeltaTime = 0.01666667f;

		public const float HumanTemperature = 36.6f;

		public const int DefaultGearsLength = 14;

		public const float NaNTolerance = 1E-06f;

		public const float SpeedOfSound = 335f;

		public const int AvrPlayerMass = 70;

		public const float FuelDensity = 0.73722f;

		public const float AoG = 9.80665f;

		public const float EndRatioLimiterMinPercent = 0.85f;

		public const float EndRatioLimiterMaxPercent = 1.15f;

		public const float GearLimiterMinPercent = 0.9f;

		public const float GearLimiterMaxPercent = 1.1f;

		public static readonly int2 NotValid = new int2(50, 667);

		public static readonly int2 Valid = new int2(100, 667);

		private const float SportMultiplier = 1.2f;

		private static TyreWidth[] alltyres = (TyreWidth[])Enum.GetValues(typeof(TyreWidth));

		private const float L100kmToMpgUs = 235.21458f;

		private static readonly List<MeshRenderer> _lpAnchorRenderers = new List<MeshRenderer>();

		private const float Radian = (float)Math.PI * 20f;

		public const float Deg2Rad = (float)Math.PI / 180f;

		public const float Rad2Deg = 57.29578f;

		public const float PI = 3.14159f;

		public const float HalfPi = 1.570795f;

		private const float Tolerance = 0.0001f;

		private const float Kg2Lbs = 2.2046225f;

		private const float Nm2LbFt = 0.7376f;

		public const float AngularVelo2RPM = 30f / (float)Math.PI;

		public const float RPM2AngularVelo = 0.104719f;

		public const float HorsepowerToWatts = 735.4987f;

		private const string VibrateAvailablePrefsKey = "VibrateAvailablePrefs";

		public static float MaxTyreWidth
		{
			get
			{
				TyreWidth[] array = alltyres;
				return (float)array[array.Length - 1] / 1000f;
			}
		}

		public static string RPM => new LocalizedString("Utils.RPM");

		public static string GetTorquePostfix => PhysRuntimeFlags.Kph ? new LocalizedString("Utils.Nm") : new LocalizedString("Utils.lb-ft");

		public static string CurrentSpeedMetric => PhysRuntimeFlags.Kph ? new LocalizedString("Utils.km/h") : new LocalizedString("Utils.mph");

		public static string CurrentMetric => PhysRuntimeFlags.Kph ? new LocalizedString("Utils.km") : new LocalizedString("Utils.mi");

		public static string CurrentWeight => PhysRuntimeFlags.Kph ? new LocalizedString("Utils.kg") : new LocalizedString("Utils.lb");

		public static string CurrentDriftMetric => PhysRuntimeFlags.Kph ? new LocalizedString("Utils.m") : new LocalizedString("Utils.ft");

		public static float SpeedConversion
		{
			get
			{
				if (!PhysRuntimeFlags.Kph)
				{
					return 2.237f;
				}
				return 3.6f;
			}
		}

		public static bool VibrateIsOn => PlayerPrefs.GetInt("VibrateAvailablePrefs", 1) == 1;

		public static bool CheckShockAbsorberValid(float2 specs, TuningLimitsData limitData)
		{
			bool num = specs.x >= limitData.BumpMin && specs.x <= limitData.BumpMax;
			bool flag = specs.y >= limitData.ReboundMin && specs.y <= limitData.ReboundMax;
			return num & flag;
		}

		public static bool CheckSpringsValid(float2 specs, TuningLimitsData limitData)
		{
			bool num = specs.x >= Inch2Meter(limitData.SpringLengthMin) && specs.x <= Inch2Meter(limitData.SpringLengthMax);
			bool flag = specs.y >= Kgfmm2NM(limitData.SpringRateMin) && specs.y <= Kgfmm2NM(limitData.SpringRateMax);
			return num & flag;
		}

		public static SuspensionState GetSuspensionState(SuspensionData currentFront, SuspensionData currentRear, SuspensionData defaultFront, SuspensionData defaultRear)
		{
			bool num = IsEqual(currentFront.Bump, defaultFront.Bump) && IsEqual(currentFront.Rebound, defaultFront.Rebound);
			bool flag = IsEqual(currentRear.Bump, defaultRear.Bump) && IsEqual(currentRear.Rebound, defaultRear.Rebound);
			if (num & flag)
			{
				return SuspensionState.Comfort;
			}
			bool num2 = IsEqual(currentFront.Bump, defaultFront.Bump * 1.2f) && IsEqual(currentFront.Rebound, defaultFront.Rebound * 1.2f);
			bool flag2 = IsEqual(currentRear.Bump, defaultRear.Bump * 1.2f) && IsEqual(currentRear.Rebound, defaultRear.Rebound * 1.2f);
			if (num2 & flag2)
			{
				return SuspensionState.Sport;
			}
			return SuspensionState.Custom;
			static bool IsEqual(float a, float b)
			{
				return Mathf.Abs(a - b) <= 0.01f;
			}
		}

		public static float4 GetSuspensionValues(SuspensionData currentFront, SuspensionData currentRear, SuspensionData defaultFront, SuspensionData defaultRear, SuspensionState needSuspensionState)
		{
			return needSuspensionState switch
			{
				SuspensionState.Comfort => new float4(defaultFront.Bump, defaultFront.Rebound, defaultRear.Bump, defaultRear.Rebound), 
				SuspensionState.Sport => new float4(defaultFront.Bump * 1.2f, defaultFront.Rebound * 1.2f, defaultRear.Bump * 1.2f, defaultRear.Rebound * 1.2f), 
				SuspensionState.Custom => new float4(currentFront.Bump, currentFront.Rebound, currentRear.Bump, currentRear.Rebound), 
				_ => new float4(defaultFront.Bump, defaultFront.Rebound, defaultRear.Bump, defaultRear.Rebound), 
			};
		}

		public static float GetFastBumpRebound(float input)
		{
			return Mathf.Round(input / 1.8f / 50f) * 50f;
		}

		public static float Round50(float input)
		{
			return Mathf.Round(input / 50f) * 50f;
		}

		public static float To01(float value)
		{
			return (value + 1f) * 0.5f;
		}

		public static float GetPedalMap(float val, int samples, float[] inputCurve, float[] remappedCurve)
		{
			float num = Mathf.Clamp01(val);
			if (samples < 2)
			{
				return (1f - num) * (1f - num);
			}
			int i;
			for (i = 0; i < samples - 1 && inputCurve[i + 1] <= num; i++)
			{
			}
			float num2 = inputCurve[i];
			float num3 = inputCurve[i + 1];
			float num4 = remappedCurve[i];
			float num5 = remappedCurve[i + 1];
			if (num < num2)
			{
				return num4;
			}
			return Mathf.Max(num4 + (num - num2) * (num5 - num4) / (num3 - num2), 0f);
		}

		public static string FormatTime(float totalSeconds)
		{
			int num = (int)(totalSeconds / 3600f);
			float num2 = totalSeconds % 3600f;
			int num3 = (int)(num2 / 60f);
			float num4 = num2 % 60f;
			int num5 = (int)num4;
			int num6 = (int)((num4 - (float)num5) * 1000f);
			if (num <= 0)
			{
				return $"{num3:D2}:{num5:D2}.{num6:D3}";
			}
			return $"{num:D2}:{num3:D2}:{num5:D2}.{num6:D3}";
		}

		public static string FormatTime1(float totalSeconds)
		{
			int num = (int)totalSeconds;
			int num2 = Mathf.RoundToInt((totalSeconds - (float)num) * 1000f);
			if (num2 == 1000)
			{
				num2 = 0;
				num++;
			}
			num2 /= 100;
			return $"{num}.{num2}";
		}

		public static string FormatTime2(float totalSeconds)
		{
			int num = (int)totalSeconds / 3600;
			int num2 = (int)totalSeconds % 3600 / 60;
			float num3 = totalSeconds % 60f;
			string text = new LocalizedString("Utils.Hours");
			string text2 = new LocalizedString("Utils.Minutes");
			string text3 = new LocalizedString("Utils.Seconds");
			if (num <= 0)
			{
				if (num2 <= 0)
				{
					return $"{num3:F2} {text3}";
				}
				return $"{num2} {num2} {num3:F2} {text3}";
			}
			return $"{num} {text} {num2} {text2} {num3:F2} {text3}";
		}

		public static string FormatTime3(float totalSeconds)
		{
			return totalSeconds.ToString("0.00", CultureInfo.InvariantCulture);
		}

		public static float GetHeatCapacity(BrakeDiscType type)
		{
			switch (type)
			{
			case BrakeDiscType.SOLID:
			case BrakeDiscType.VENTED:
			case BrakeDiscType.DRILLED:
			case BrakeDiscType.GROOVED:
				return 500f;
			case BrakeDiscType.CARBON:
				return 710f;
			case BrakeDiscType.CERAMIC:
				return 750f;
			default:
				return 500f;
			}
		}

		public static VehicleClass GetClassVehicle(int performanceScore, int carId)
		{
			if (carId == 155 || carId == 165)
			{
				return VehicleClass.F1;
			}
			if (performanceScore < 530)
			{
				if (performanceScore < 340)
				{
					if (performanceScore < 250)
					{
						return VehicleClass.A;
					}
					return VehicleClass.B;
				}
				if (performanceScore < 440)
				{
					return VehicleClass.C;
				}
				return VehicleClass.D;
			}
			if (performanceScore < 615)
			{
				return VehicleClass.S;
			}
			return VehicleClass.U;
		}

		public static Color GetClassColor(VehicleClass vehicleClass)
		{
			return vehicleClass switch
			{
				VehicleClass.A => new Color(0.443f, 0.749f, 0.89f), 
				VehicleClass.B => new Color(0.996f, 0.776f, 0.231f), 
				VehicleClass.C => new Color(0.941f, 0.388f, 0.11f), 
				VehicleClass.D => new Color(0.925f, 0.2f, 0.325f), 
				VehicleClass.S => new Color(0.663f, 0.361f, 0.776f), 
				VehicleClass.U => new Color(0.251f, 0.749f, 0.227f), 
				_ => new Color(0f, 0f, 0f), 
			};
		}

		public static void CalculateBotRating(MainCarValues carValues, GameObject vehicle, out VehicleClass carClass, out int ps)
		{
			if (!vehicle)
			{
				carClass = VehicleClass.A;
				ps = 0;
				return;
			}
			float num = Mathf.Max(carValues.bodyMass, 500f);
			float num2 = 100f;
			float maxHp = 0f;
			float maxTq = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			if (vehicle.TryGetComponent<Powertrain>(out var component))
			{
				num2 = component.CalcAccelTime(0f, 100f, Surface.Dryasphalt);
				component.CalcMaxPowerMaxTorque(out maxHp, out var _, out maxTq, out var _, out var _);
				num3 = component.MaxSideAccel * 0.001f;
				num4 = component.MaxBrakeAccel;
				num5 = Mathf.Clamp(component.CalcTopSpeed(), 0f, 100f);
			}
			float num6 = (maxHp + maxTq * 0.735f) / num * 500f;
			float num7 = 1f / num2 * 100f;
			float num8 = num5 / 3.6f;
			float num9 = (num3 + num4) * 10f;
			float num10 = (num6 + num7 + num8 + num9) * 1.1f;
			if (num3 > 1.5f)
			{
				num10 += 20f;
			}
			ps = Mathf.CeilToInt(num10);
			carClass = GetClassVehicle(ps, carValues.CarID);
		}

		public static void CalculateUserRating(int carId, GameObject vehicle, out VehicleClass carClass, out int ps)
		{
			VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId);
			if (!vehicle || !defaultCarTuningDataConfig)
			{
				carClass = VehicleClass.A;
				ps = 0;
				return;
			}
			float num = Mathf.Max(defaultCarTuningDataConfig.Tuning.ChassisData.Weight, 405f);
			float num2 = 100f;
			float maxHp = 0f;
			float maxTq = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			if (vehicle.TryGetComponent<Powertrain>(out var component))
			{
				num2 = component.CalcAccelTime(0f, 100f, Surface.Dryasphalt);
				component.CalcMaxPowerMaxTorque(out maxHp, out var _, out maxTq, out var _, out var _);
				num3 = component.MaxSideAccel * 0.001f;
				num4 = component.MaxBrakeAccel;
				num5 = Mathf.Clamp(component.CalcTopSpeed(), 0f, 100f);
			}
			float num6 = (maxHp + maxTq * 0.735f) / num * 500f;
			float num7 = 1f / num2 * 100f;
			float num8 = num5 / 3.6f;
			float num9 = (num3 + num4) * 10f;
			float num10 = (num6 + num7 + num8 + num9) * 1.1f;
			if (num3 > 1.5f)
			{
				num10 += 20f;
			}
			ps = Mathf.CeilToInt(num10);
			carClass = GetClassVehicle(ps, carId);
		}

		public static float CalcGasPressure(float rpm, float load, float pressure, float min, float max)
		{
			return pressure * (rpm / (max - min) * (load * 0.7f));
		}

		public static float GetEngineMaxTorqueRpm(float rpm, TorqueCurveSample[] curveSamples)
		{
			switch (curveSamples.Length)
			{
			case 0:
				return 0f;
			case 1:
				return curveSamples[0].Torque;
			default:
			{
				float num = 0f;
				for (int i = 0; i < curveSamples.Length - 1; i++)
				{
					if (rpm < curveSamples[i].Rpm)
					{
						return curveSamples[i].Torque;
					}
					num = curveSamples[i].Torque + (rpm - curveSamples[i].Rpm) * (curveSamples[i + 1].Torque - curveSamples[i].Torque) / (curveSamples[i + 1].Rpm - curveSamples[i].Rpm);
					if (curveSamples[i + 1].Rpm > rpm)
					{
						break;
					}
				}
				if (num < 0f)
				{
					num = 0f;
				}
				return num;
			}
			}
		}

		public static float GetDragMultiplier(TurboType turbo)
		{
			return turbo switch
			{
				TurboType.NA => 0f, 
				TurboType.SingleTurboMonoScroll => 0.06f, 
				TurboType.SingleTurboTwinScroll => 0.05f, 
				TurboType.TwinTurboSingleScroll => 0.09f, 
				TurboType.TwinTurboTwinScroll => 0.08f, 
				TurboType.BiTurbo => 0.1f, 
				TurboType.Supercharger => 0.05f, 
				TurboType.QuadTurboCharger => 0.14f, 
				_ => 0f, 
			};
		}

		public static TorqueCurveSample[] GenerateTorqueCurve(float maxTorque, float peakTorqueRPM, int turboType, float cutoffRpm, float maxRpm, float minForcedInductionRpm, float maxForcedInductionRpm, float idleRpm = 800f)
		{
			return PhysMath.GenerateTorqueCurve(maxTorque, peakTorqueRPM, turboType, cutoffRpm, maxRpm, minForcedInductionRpm, maxForcedInductionRpm, idleRpm);
		}

		private static float[] GetTorqueCoefficient(TurboType turboType)
		{
			return turboType switch
			{
				TurboType.NA => new float[30]
				{
					0.26f, 0.33f, 0.47f, 0.66f, 0.79f, 0.89f, 0.96f, 0.99f, 1f, 0.98f,
					0.92f, 0.88f, 0.81f, 0.76f, 0.7f, 0.65f, 0.61f, 0.58f, 0.54f, 0.51f,
					0.48f, 0.42f, 0.36f, 0.31f, 0.27f, 0.22f, 0.18f, 0.15f, 0.11f, 0.06f
				}, 
				TurboType.SingleTurboMonoScroll => new float[30]
				{
					0.27f, 0.32f, 0.46f, 0.71f, 0.88f, 0.94f, 0.99f, 1f, 0.99f, 0.98f,
					0.97f, 0.95f, 0.9f, 0.86f, 0.82f, 0.76f, 0.71f, 0.66f, 0.62f, 0.58f,
					0.48f, 0.42f, 0.36f, 0.31f, 0.27f, 0.22f, 0.18f, 0.15f, 0.11f, 0.06f
				}, 
				TurboType.SingleTurboTwinScroll => new float[30]
				{
					0.26f, 0.33f, 0.47f, 0.68f, 0.86f, 0.94f, 0.99f, 1f, 1f, 0.99f,
					0.98f, 0.96f, 0.91f, 0.88f, 0.85f, 0.81f, 0.79f, 0.74f, 0.69f, 0.65f,
					0.59f, 0.54f, 0.48f, 0.43f, 0.39f, 0.33f, 0.29f, 0.23f, 0.19f, 0.13f
				}, 
				TurboType.TwinTurboSingleScroll => new float[30]
				{
					0.31f, 0.4f, 0.55f, 0.74f, 0.85f, 0.98f, 0.99f, 1f, 0.99f, 0.98f,
					0.97f, 0.96f, 0.94f, 0.91f, 0.87f, 0.83f, 0.78f, 0.75f, 0.7f, 0.66f,
					0.61f, 0.53f, 0.47f, 0.41f, 0.38f, 0.34f, 0.28f, 0.21f, 0.17f, 0.12f
				}, 
				TurboType.TwinTurboTwinScroll => new float[30]
				{
					0.3f, 0.41f, 0.54f, 0.72f, 0.84f, 0.98f, 0.99f, 1f, 0.99f, 0.98f,
					0.95f, 0.91f, 0.89f, 0.85f, 0.82f, 0.81f, 0.78f, 0.74f, 0.72f, 0.66f,
					0.6f, 0.54f, 0.48f, 0.42f, 0.37f, 0.33f, 0.27f, 0.22f, 0.16f, 0.12f
				}, 
				TurboType.BiTurbo => new float[30]
				{
					0.26f, 0.33f, 0.47f, 0.68f, 0.82f, 0.88f, 0.96f, 1f, 0.96f, 0.94f,
					0.91f, 0.88f, 0.87f, 0.84f, 0.83f, 0.82f, 0.77f, 0.75f, 0.73f, 0.67f,
					0.6f, 0.54f, 0.48f, 0.42f, 0.37f, 0.33f, 0.27f, 0.22f, 0.16f, 0.12f
				}, 
				TurboType.Supercharger => new float[30]
				{
					0.3f, 0.44f, 0.59f, 0.74f, 0.89f, 0.98f, 0.99f, 1f, 1f, 1f,
					1f, 0.98f, 0.98f, 0.97f, 0.96f, 0.9f, 0.88f, 0.84f, 0.82f, 0.76f,
					0.6f, 0.54f, 0.48f, 0.44f, 0.37f, 0.33f, 0.27f, 0.22f, 0.16f, 0.12f
				}, 
				TurboType.QuadTurboCharger => new float[30]
				{
					0.26f, 0.33f, 0.47f, 0.75f, 0.82f, 0.94f, 0.98f, 1f, 1.01f, 1f,
					0.99f, 0.99f, 0.98f, 0.98f, 0.96f, 0.91f, 0.88f, 0.84f, 0.82f, 0.76f,
					0.48f, 0.42f, 0.36f, 0.31f, 0.27f, 0.22f, 0.18f, 0.15f, 0.11f, 0.06f
				}, 
				_ => new float[30]
				{
					0.26f, 0.33f, 0.47f, 0.66f, 0.79f, 0.89f, 0.96f, 0.99f, 1f, 0.98f,
					0.92f, 0.88f, 0.81f, 0.76f, 0.7f, 0.65f, 0.61f, 0.58f, 0.54f, 0.51f,
					0.48f, 0.42f, 0.36f, 0.31f, 0.27f, 0.22f, 0.18f, 0.15f, 0.11f, 0.06f
				}, 
			};
		}

		public static float P2Bar(AdvancedInt input)
		{
			input.Get(out int value);
			return (float)value / 100000f;
		}

		public static int ExtractIntFromName(string name)
		{
			Match match = Regex.Match(name, "\\d+");
			if (match.Success)
			{
				return int.Parse(match.Value);
			}
			return -1;
		}

		public static string GetMass(float value, bool round = false)
		{
			if (round)
			{
				if (!PhysRuntimeFlags.Kph)
				{
					return string.Format("{0:F0} {1}", value * 2.2046225f, new LocalizedString("Utils.lbs"));
				}
				return string.Format("{0:F0} {1}", value, new LocalizedString("Utils.kg"));
			}
			if (!PhysRuntimeFlags.Kph)
			{
				return string.Format("{0:F1} {1}", value * 2.2046225f, new LocalizedString("Utils.lbs"));
			}
			return string.Format("{0:F1} {1}", value, new LocalizedString("Utils.kg"));
		}

		public static string GetMassNoLoc(float value, bool round = false)
		{
			if (round)
			{
				if (!PhysRuntimeFlags.Kph)
				{
					return $"{value * 2.2046225f:F0}LBS";
				}
				return $"{value:F0}KG";
			}
			if (!PhysRuntimeFlags.Kph)
			{
				return $"{value * 2.2046225f:F1}LBS";
			}
			return $"{value:F1}KG";
		}

		public static string GetFuelConsumption(float lPer100Km)
		{
			if (PhysRuntimeFlags.Kph)
			{
				return $"{lPer100Km:F1} l/100km";
			}
			float num = ((lPer100Km > 0f) ? (235.21458f / lPer100Km) : 0f);
			return $"{num:F1} mpg";
		}

		public static string GetTorque(float value)
		{
			if (value == 0f)
			{
				return new LocalizedString("Utils.N/A");
			}
			if (!PhysRuntimeFlags.Kph)
			{
				return $"{value * 0.7376f:F0}";
			}
			return $"{value:F0}";
		}

		public static string GetPower(float value)
		{
			if (value != 0f)
			{
				return string.Format("{0:F0} {1}", value, new LocalizedString("Utils.hp"));
			}
			return string.Format("{0} {1}", new LocalizedString("Utils.N/A"), new LocalizedString("Utils.hp"));
		}

		public static Transform SetLpAnchor(Transform anchor, GameObject obj, bool getCenter = false)
		{
			Transform transform = anchor;
			if ((bool)obj)
			{
				List<MeshRenderer> lpAnchorRenderers = _lpAnchorRenderers;
				obj.GetComponentsInChildren(lpAnchorRenderers);
				if (lpAnchorRenderers.Count == 0)
				{
					return transform;
				}
				if (getCenter)
				{
					Bounds bounds = lpAnchorRenderers[0].bounds;
					foreach (MeshRenderer item in lpAnchorRenderers)
					{
						bounds.Encapsulate(item.bounds);
					}
					transform = new GameObject("LightProbeAnchor").transform;
					transform.position = bounds.center;
				}
				foreach (MeshRenderer item2 in lpAnchorRenderers)
				{
					item2.probeAnchor = transform.transform;
				}
				lpAnchorRenderers.Clear();
			}
			return transform;
		}

		public static float SparkAngleTo4cycleTime(float ang)
		{
			return ang / 720f;
		}

		public static float Inch2Meter(float i)
		{
			return i * 0.0254f;
		}

		public static float Mm2Meter(float i)
		{
			return i * 0.001f;
		}

		public static float Mm2Meter(TyreWidth i)
		{
			return (float)i * 0.001f;
		}

		public static float Mm2Cm(float i)
		{
			return i * 0.1f;
		}

		public static float Cm2Mm(float i)
		{
			return i * 10f;
		}

		public static float Cm2Meter(float i)
		{
			return i * 0.01f;
		}

		public static float Inch2Cm(float i)
		{
			return i * 2.54f;
		}

		public static float Meter2Inch(float i)
		{
			return i * 39.37008f;
		}

		public static float Meter2Cm(float i)
		{
			return i * 100f;
		}

		public static float Meter2Mm(float i)
		{
			return i * 1000f;
		}

		public static float Meter2Mile(float i)
		{
			return i * 0.000621371f;
		}

		public static float Meter2Km(float i)
		{
			return i * 0.001f;
		}

		public static float Km2Mile(float i)
		{
			return i / 1.609f;
		}

		public static float Mps2Kmh(float i)
		{
			return i * 3.6f;
		}

		public static float Sqr(float i)
		{
			return i * i;
		}

		public static float Bar2kPa(float i)
		{
			return i * 100f;
		}

		public static int Bar2Pa(float i)
		{
			return Mathf.RoundToInt(i * 100000f);
		}

		public static float Pa2Bar(int i)
		{
			return (float)i / 100000f;
		}

		public static float kPa2Bar(float i)
		{
			return i / 100f;
		}

		public static float Kgfmm2NM(float input)
		{
			return input * 9806.65f;
		}

		public static float NM2Kgfmm(float input)
		{
			return input / 9806.65f;
		}

		public static float N2KN(float input)
		{
			return input / 1000f;
		}

		public static float KN2N(float input)
		{
			return input * 1000f;
		}

		public static float CalculateWheelRadius(TyreWidth width, TyreProfile profile, WheelDiameter wheelDiameter)
		{
			float num = (float)width * ((float)profile * 0.01f);
			return Mm2Meter((float)wheelDiameter * 25.4f + 2f * num) * 0.5f;
		}

		public static bool IsApproximatelyEqual(float a, float b)
		{
			return Mathf.Abs(a - b) < 0.0001f;
		}

		public static float MetersToFeet(float meter)
		{
			if (!PhysRuntimeFlags.Kph)
			{
				return meter * 3.28f;
			}
			return meter;
		}

		public static float GetConvertedMileage(float mileage)
		{
			if (!PhysRuntimeFlags.Kph)
			{
				return Km2Mile(mileage);
			}
			return mileage;
		}

		public static float RollTo(float val, float min, float max)
		{
			float num = max - min;
			float num2 = (val - min) % num;
			if (num2 < 0f)
			{
				num2 += num;
			}
			return min + num2;
		}

		public static int String2Int(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return -1;
			}
			if (int.TryParse(Regex.Replace(input, "[^\\d]+", ""), out var result))
			{
				return result;
			}
			return -1;
		}

		public static float RoundTo(float value, int precision)
		{
			float num = Mathf.Pow(10f, precision);
			return Mathf.Round(value * num) / num;
		}

		public static float CalcEngineBoostMultiplier(float curPressure, TurboType turboType)
		{
			if (turboType == TurboType.NA)
			{
				return 1f;
			}
			if (curPressure <= 0f)
			{
				return 1f;
			}
			return 1f + curPressure * TurboEfficiency(turboType);
		}

		public static bool QuaternionIsNaN(Quaternion q)
		{
			if (!float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z))
			{
				return float.IsNaN(q.w);
			}
			return true;
		}

		public static bool Vector3IsNaN(Vector3 q)
		{
			if (!float.IsNaN(q.x) && !float.IsNaN(q.y))
			{
				return float.IsNaN(q.z);
			}
			return true;
		}

		public static float GetFixedVariable(float value)
		{
			if (float.IsInfinity(value) || float.IsNaN(value))
			{
				value = 0f;
			}
			return value;
		}

		public static Vector3 FixedVector(Vector3 vec)
		{
			if (float.IsInfinity(vec.x) || float.IsNaN(vec.x))
			{
				vec.x = 0f;
			}
			if (float.IsInfinity(vec.y) || float.IsNaN(vec.y))
			{
				vec.y = 0f;
			}
			if (float.IsInfinity(vec.z) || float.IsNaN(vec.z))
			{
				vec.z = 0f;
			}
			return vec;
		}

		internal static Matrix4x4 CrossProductMatrix(Vector3 v)
		{
			return new Matrix4x4
			{
				m00 = 0f,
				m10 = 0f - v.z,
				m20 = v.y,
				m30 = 0f,
				m01 = v.z,
				m11 = 0f,
				m21 = 0f - v.x,
				m31 = 0f,
				m02 = 0f - v.y,
				m12 = v.x,
				m22 = 0f,
				m32 = 0f,
				m03 = 0f,
				m13 = 0f,
				m23 = 0f,
				m33 = 0f
			};
		}

		internal static Vector3 MultiplyPoint3x4(Matrix4x4 m, Vector3 v)
		{
			return m.MultiplyPoint3x4(v);
		}

		internal static void MatrixTransform(ref Matrix4x4 m, Vector3 origin, Vector3 ox, Vector3 oy, Vector3 oz)
		{
			m.m00 = ox.x;
			m.m10 = ox.y;
			m.m20 = ox.z;
			m.m30 = 0f;
			m.m01 = oy.x;
			m.m11 = oy.y;
			m.m21 = oy.z;
			m.m31 = 0f;
			m.m02 = oz.x;
			m.m12 = oz.y;
			m.m22 = oz.z;
			m.m32 = 0f;
			m.m03 = origin.x;
			m.m13 = origin.y;
			m.m23 = origin.z;
			m.m33 = 1f;
		}

		public static void MatrixGetInverse(Matrix4x4 m, ref Matrix4x4 res)
		{
			float m2 = 0f - (m.m00 * m.m03 + m.m10 * m.m13 + m.m20 * m.m23);
			float m3 = 0f - (m.m01 * m.m03 + m.m11 * m.m13 + m.m21 * m.m23);
			float m4 = 0f - (m.m02 * m.m03 + m.m12 * m.m13 + m.m22 * m.m23);
			res.m00 = m.m00;
			res.m01 = m.m10;
			res.m02 = m.m20;
			res.m03 = m2;
			res.m10 = m.m01;
			res.m11 = m.m11;
			res.m12 = m.m21;
			res.m13 = m3;
			res.m20 = m.m02;
			res.m21 = m.m12;
			res.m22 = m.m22;
			res.m23 = m4;
			res.m30 = 0f;
			res.m31 = 0f;
			res.m32 = 0f;
			res.m33 = 1f;
		}

		public static void MatrixMultiply3x3(Matrix4x4 lhs, Matrix4x4 rhs, ref Matrix4x4 res)
		{
			res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20;
			res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21;
			res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22;
			res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20;
			res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
			res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
			res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20;
			res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
			res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
		}

		internal static void MatrixMultiply4x4(Matrix4x4 lhs, Matrix4x4 rhs, ref Matrix4x4 res)
		{
			res = lhs * rhs;
		}

		internal static void MatrixAdd3x3(ref Matrix4x4 a, Matrix4x4 m)
		{
			a.m00 += m.m00;
			a.m10 += m.m10;
			a.m20 += m.m20;
			a.m01 += m.m01;
			a.m11 += m.m11;
			a.m21 += m.m21;
			a.m02 += m.m02;
			a.m12 += m.m12;
			a.m22 += m.m22;
		}

		internal static void MatrixOrthonormalize(ref Matrix4x4 m)
		{
			Vector3 v = new Vector3(m.m00, m.m10, m.m20);
			Vector3 v2 = new Vector3(m.m01, m.m11, m.m21);
			Vector3 vector = new Vector3(m.m02, m.m12, m.m22);
			Vector3 origin = new Vector3(m.m03, m.m13, m.m23);
			Vector3Normalize(ref v);
			v2 -= v * Vector3.Dot(v, v2);
			Vector3Normalize(ref v2);
			vector = Vector3.Cross(v, v2);
			MatrixTransform(ref m, origin, v, v2, vector);
		}

		internal static Vector3 GetForward(Matrix4x4 m)
		{
			return new Vector3(m.m02, m.m12, m.m22);
		}

		internal static Vector3 GetOrigin(Matrix4x4 m)
		{
			return new Vector3(m.m03, m.m13, m.m23);
		}

		internal static Vector3 GetRight(Matrix4x4 m)
		{
			return new Vector3(m.m00, m.m10, m.m20);
		}

		internal static Vector3 GetUp(Matrix4x4 m)
		{
			return new Vector3(m.m01, m.m11, m.m21);
		}

		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 euler = angles * ((float)Math.PI / 180f);
			Vector3 vector = point - pivot;
			return Quaternion.Euler(euler) * vector + pivot;
		}

		internal static void MatrixTranslate(ref Matrix4x4 m, Vector3 v)
		{
			m.m03 += v.x;
			m.m13 += v.y;
			m.m23 += v.z;
		}

		public static void MatrixSetTranslate(ref Matrix4x4 m, Vector3 v)
		{
			m.m03 = v.x;
			m.m13 = v.y;
			m.m23 = v.z;
		}

		internal static void MatrixRotateFast(ref Matrix4x4 m, float angle, Vector3 axis, bool isDegree = true)
		{
			if (isDegree)
			{
				angle *= (float)Math.PI / 180f;
			}
			axis.Normalize();
			float num = Mathf.Cos(angle);
			float num2 = Mathf.Sin(angle);
			float num3 = 1f - num;
			float x = axis.x;
			float y = axis.y;
			float z = axis.z;
			float num4 = x * y * num3;
			float num5 = x * z * num3;
			float num6 = y * z * num3;
			float num7 = x * num2;
			float num8 = y * num2;
			float num9 = z * num2;
			MatrixMultiply4x4(new Matrix4x4
			{
				m00 = x * x * num3 + num,
				m01 = num4 - num9,
				m02 = num5 + num8,
				m03 = 0f,
				m10 = num4 + num9,
				m11 = y * y * num3 + num,
				m12 = num6 - num7,
				m13 = 0f,
				m20 = num5 - num8,
				m21 = num6 + num7,
				m22 = z * z * num3 + num,
				m23 = 0f,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			}, m, ref m);
		}

		internal static void Vector3NormalizeSafe(ref Vector3 v)
		{
			if (v != Vector3.zero)
			{
				v.Normalize();
			}
		}

		internal static void Vector3Normalize(ref Vector3 v)
		{
			float num = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			num = 1f / num;
			v.x *= num;
			v.y *= num;
			v.z *= num;
		}

		internal static void NormalizeAngles(ref float ang, ref float ang2)
		{
			if (ang > (float)Math.PI * 20f && ang2 > (float)Math.PI * 20f)
			{
				ang -= (float)Math.PI * 20f;
				ang2 -= (float)Math.PI * 20f;
			}
			if (ang < 0f && ang2 < 0f)
			{
				ang += (float)Math.PI * 20f;
				ang2 += (float)Math.PI * 20f;
			}
		}

		internal static void NormalizeRadAngle(ref float ang)
		{
			if (ang > (float)Math.PI * 20f)
			{
				ang -= (float)Math.PI * 20f;
			}
			if (ang < 0f)
			{
				ang += (float)Math.PI * 20f;
			}
		}

		internal static void NormalizeDegAngle(ref float angle)
		{
			angle %= 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
		}

		internal static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			Quaternion result = default;
			float num = m.m00 + m.m11 + m.m22;
			float num2 = 2f * Mathf.Sqrt(1f + m.m22 - m.m00 - m.m11);
			if (num > 0f)
			{
				num2 = 0.5f / Mathf.Sqrt(num + 1f);
				result.w = 0.25f / num2;
				result.x = (m.m21 - m.m12) * num2;
				result.y = (m.m02 - m.m20) * num2;
				result.z = (m.m10 - m.m01) * num2;
				return result;
			}
			if (m.m00 > m.m11 && m.m00 > m.m22)
			{
				num2 = 2f * Mathf.Sqrt(1f + m.m00 - m.m11 - m.m22);
				result.w = (m.m21 - m.m12) / num2;
				result.x = 0.25f * num2;
				result.y = (m.m01 + m.m10) / num2;
				result.z = (m.m02 + m.m20) / num2;
				return result;
			}
			if (m.m11 > m.m22)
			{
				num2 = 2f * Mathf.Sqrt(1f + m.m11 - m.m00 - m.m22);
				result.w = (m.m02 - m.m20) / num2;
				result.x = (m.m01 + m.m10) / num2;
				result.y = 0.25f * num2;
				result.z = (m.m12 + m.m21) / num2;
				return result;
			}
			result.w = (m.m10 - m.m01) / num2;
			result.x = (m.m02 + m.m20) / num2;
			result.y = (m.m12 + m.m21) / num2;
			result.z = 0.25f * num2;
			return result;
		}

		public static float KgToInertia(float kg)
		{
			return kg * 0.1f * 0.1f;
		}

		public static float FtLbf2Nm(float f)
		{
			return f * 1.3558179f;
		}

		public static void GetEngineParams(TorqueCurveSample[] samples, TurboType turboType, float maxTurboPressure, out float maxTorque, out float maxTorqueRpm)
		{
			if (samples.Length == 0)
			{
				maxTorque = 0f;
				maxTorqueRpm = 0f;
				return;
			}
			maxTorque = 0f;
			maxTorqueRpm = 0f;
			for (int i = 0; i < samples.Length; i++)
			{
				if (samples[i].Torque > maxTorque)
				{
					maxTorque = samples[i].Torque;
					maxTorqueRpm = samples[i].Rpm;
				}
			}
			maxTorque *= CalcEngineBoostMultiplier(maxTurboPressure, turboType);
		}

		public static string FormatOdometer(float km)
		{
			float num = km % 10000f;
			if (num < 0f)
			{
				num += 10000f;
			}
			return num.ToString("0000.0", CultureInfo.InvariantCulture);
		}

		public static string FormatMileage(float mileage)
		{
			int num = (int)mileage;
			mileage -= (float)num;
			if (mileage > 0.95f)
			{
				num++;
			}
			return string.Format("{0:D4}{1}", num, mileage.ToString("F1").Remove(0, 1));
		}

		public static float TurboEfficiency(TurboType turbo)
		{
			return turbo switch
			{
				TurboType.SingleTurboMonoScroll => 0.5f, 
				TurboType.SingleTurboTwinScroll => 0.55f, 
				TurboType.TwinTurboSingleScroll => 0.6f, 
				TurboType.TwinTurboTwinScroll => 0.65f, 
				TurboType.BiTurbo => 0.63f, 
				TurboType.Supercharger => 0.7f, 
				TurboType.QuadTurboCharger => 0.9f, 
				_ => 0f, 
			};
		}

		public static string GetRankNameByIndex(int index)
		{
			return index switch
			{
				0 => "Beginner", 
				1 => "Driver", 
				2 => "Racer", 
				3 => "an Expert", 
				4 => "Professional", 
				5 => "Supreme", 
				6 => "The King", 
				7 => "Developer", 
				8 => "YouTuber", 
				9 => "TikToker", 
				10 => "IG Blogger", 
				_ => "", 
			};
		}

		public static string CorrectFormatString(string format, int argumentCount)
		{
			string pattern = "\\{(\\d+)\\}";
			return Regex.Replace(format, pattern, (Match match) => (int.Parse(match.Groups[1].Value) >= argumentCount) ? "" : match.Value);
		}

		public static Color ConvertHDRToLDR(Color hdrColor)
		{
			float num = Mathf.Max(hdrColor.r, Mathf.Max(hdrColor.g, hdrColor.b));
			if (!(num > 1f))
			{
				return hdrColor;
			}
			return new Color(hdrColor.r / num, hdrColor.g / num, hdrColor.b / num, hdrColor.a);
		}

		public static string GetLocalizedMarketFilter(MarketInventoryFilters filter)
		{
			string result = "";
			switch (filter)
			{
			case MarketInventoryFilters.All:
				result = new LocalizedString("Enginefilterwindow.All");
				break;
			case MarketInventoryFilters.Compatible:
				result = new LocalizedString("Enginefilterwindow.Compatible");
				break;
			case MarketInventoryFilters.InCompatible:
				result = new LocalizedString("Enginefilterwindow.Incompatible");
				break;
			}
			return result;
		}

		public static string ConvertToTime(float value)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(value);
			return ((value < 0f) ? "-" : "") + timeSpan.ToString("mm\\:ss");
		}

		public static string ConvertToTimeMiliSecs(float value)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Abs(value));
			return ((value < 0f) ? "-" : "") + timeSpan.ToString("mm\\:ss\\:fff");
		}

		public static string SecondsToText(int value)
		{
			value = Mathf.Max(0, value);
			TimeSpan timeSpan = TimeSpan.FromSeconds(value);
			if (timeSpan.Days > 0)
			{
				return $"{timeSpan.Days}d:{timeSpan.Hours}h";
			}
			if (timeSpan.Hours > 0)
			{
				return $"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			}
			if (timeSpan.Minutes > 0)
			{
				return $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
			}
			return $"{timeSpan.Seconds}";
		}

		public static string GetCameraTypeName(DragRacingCamera.CameraType cameraType)
		{
			return cameraType switch
			{
				DragRacingCamera.CameraType.driverView => new LocalizedString("DragRacingCamera.DriverView"), 
				DragRacingCamera.CameraType.garageView => new LocalizedString("DragRacingCamera.GarageView"), 
				DragRacingCamera.CameraType.close => new LocalizedString("DragRacingCamera.close"), 
				DragRacingCamera.CameraType.middle => new LocalizedString("DragRacingCamera.middle"), 
				DragRacingCamera.CameraType.far => new LocalizedString("DragRacingCamera.far"), 
				DragRacingCamera.CameraType.spectator => new LocalizedString("DragRacingCamera.spectator"), 
				DragRacingCamera.CameraType.freeCamera => "Free", 
				DragRacingCamera.CameraType.directionCamera => "Direction", 
				_ => cameraType.ToString(), 
			};
		}

		public static string GetPhoneCarControlTitle(PhoneCarControlUIPanelType carControlUIPanelType)
		{
			return carControlUIPanelType switch
			{
				PhoneCarControlUIPanelType.Pneumatics => new LocalizedString("PhoneSuspensionWindow.Pneumatic"), 
				PhoneCarControlUIPanelType.ShockAbsorbers => new LocalizedString("Suspensionuiwindow.ShockAbsorber"), 
				_ => "", 
			};
		}

		public static string GetFormattedResultWithSpaces(float value)
		{
			return ((int)value).ToString("N0").Replace(",", " ") + " DP";
		}

		public static void Vibrate(float duration, float intensity, bool unscaledTime = false)
		{
			if (VibrateIsOn)
			{
				CustomVibrationData customData = new CustomVibrationData
				{
					amplitude = intensity * 100f,
					durationInSeconds = duration,
					sharpness = 59f
				};
				if (unscaledTime)
				{
					VibrationProvider.TryPlayUnscaled(customData);
				}
				else
				{
					VibrationProvider.TryPlay(customData);
				}
			}
		}

		public static void SetVibrateIsOn(bool isOn)
		{
			PlayerPrefs.SetInt("VibrateAvailablePrefs", isOn ? 1 : 0);
		}
	}
}
