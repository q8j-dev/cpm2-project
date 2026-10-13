using System;
using UnityEngine;

namespace CPMEngine
{
	public static class PhysMath
	{
		public readonly struct EspCorrection
		{
			public readonly bool Active;

			public readonly bool RearAxle;

			public readonly bool RightSide;

			public readonly float Intensity01;

			public EspCorrection(bool active, bool rearAxle, bool rightSide, float intensity01)
			{
				Active = active;
				RearAxle = rearAxle;
				RightSide = rightSide;
				Intensity01 = intensity01;
			}
		}

		public const float AirDensity = 1.29f;

		public const float NaNTolerance = 1E-06f;

		public const float AoG = 9.80665f;

		private const float Radian = (float)Math.PI * 20f;

		public const float Rad2Deg = 57.29578f;

		public const float PI = 3.14159f;

		public const float HalfPi = 1.570795f;

		public const float AngularVelo2RPM = 30f / (float)Math.PI;

		public const float RPM2AngularVelo = 0.104719f;

		public const float HorsepowerToWatts = 735.4987f;

		public const float RevMatchMarginRpm = 150f;

		public const float RevMatchOvershoot = 1.08f;

		public const float LaunchClutchTorqueMargin = 1.3f;

		public const float LaunchTorqueCapSpeed = 5f;

		public const float TurboDumpTau = 0.08f;

		public const float TurboBleedScale = 3f;

		public const float TallGearLaunchFloor = 0.3f;

		public const float RimBumpStiffness = 800000f;

		public const float RimBumpEngageFraction = 0.65f;

		public const float SuspensionUsableTravelFraction = 0.45f;

		public const float SuspensionMaxStaticShare = 0.5f;

		public const float SuspensionBumpEngageFraction = 0.6f;

		public const float SuspensionBumpRatio = 8f;

		public const float SuspensionBumpMaxStiffness = 500000f;

		public const float SuspensionBumpDampingRatio = 3f;

		public const float TyreDampingRatioMin = 0.15f;

		public const float TyreDampingRatioMax = 0.5f;

		public const float AbsMinBrakeTorque = 100f;

		public const float AbsMinReferenceAngVel = 2f;

		public const float AirborneBrakeStopSec = 0.5f;

		public const float AirborneBrakeMinTorque = 100f;

		public const float EspCharacteristicSpeed = 22f;

		public const float EspSpinBrakeDeadZone = 5f;

		public const float EspSpinBrakeFullOverspeed = 25f;

		public const float EspSecondWheelSideslip = 15f;

		public const float EspSecondWheelFullSideslip = 25f;

		public const float EspSideslipFloor = 2f;

		public const float EspSideslipCertain = 8f;

		public const float EspSideslipRate = 15f;

		public const float BrakeHeatGain = 5f;

		public const float BrakeGripCoefficient = 1.2f;

		public const float TyreHeatEfficiency = 0.85f;

		public const float TyreSurfaceHeatCapPerKg = 12f;

		public const float TyreSurfaceConductance = 100f;

		public const float TyreSmokeTemperature = 160f;

		public const float TyreSurfaceMaxTemperature = 350f;

		public const float TyreSmokeSlipSpeed = 4f;

		private const float SlideShareOnset = 0.12f;

		private const float SlideShareFull = 0.3f;

		public const float TyreSmokeSlideShare = 0.5f;

		public const int WheelContactRayCount = 5;

		public const int MobileWheelContactRayCount = 1;

		public const float LateralRayFraction = 0.8f;

		public const float LongitudinalRayFraction = 0.3f;

		public const float ContactRayStickyMargin = 0.005f;

		public const float VisualContactNormalMinDot = 0.57f;

		public const float VisualTreadShearGain = 2.2f;

		public const float LowEndTorqueDeficit = 0.35f;

		public const float PumpingMepPa = 80000f;

		public const float FrictionMepPa = 160000f;

		private const float BmepEstimatePa = 1000000f;

		private const float MinEstimatedDisplacementCc = 800f;

		private const float MaxEstimatedDisplacementCc = 6500f;

		public const float MinInertiaPerLitre = 0.04f;

		public const float AbsoluteMinEngineInertia = 0.05f;

		public const float LegacyBrakeTorqueRatio = 0.07f;

		public static int ActiveWheelContactRayCount => 5;

		public static float CalcGasPressure(float rpm, float load, float pressure, float min, float max)
		{
			return pressure * (rpm / (max - min) * (load * 0.7f));
		}

		public static float CalcTurboTargetPressure(float rpm, float load, float maxPressure, float minRpm, float maxRpm)
		{
			float num = maxRpm - minRpm;
			if (maxPressure <= 0f || num <= 0f)
			{
				return 0f;
			}
			float x = Mathf.Clamp01((rpm - minRpm) / num);
			float num2 = Mathf.Clamp01(load / 0.7f);
			return maxPressure * SmoothStep(x) * num2;
		}

		public static float SpoolTowards(float current, float target, float dt, float tauUp, float tauDown)
		{
			float num = ((target >= current) ? tauUp : tauDown);
			if (num <= 0f)
			{
				return target;
			}
			return current + (target - current) * (1f - Mathf.Exp((0f - dt) / num));
		}

		public static float LaunchTargetRpm(float idleRpm, float redline, float throttle)
		{
			return LaunchTargetRpm(idleRpm, redline, throttle, 1f);
		}

		public static float LaunchTargetRpm(float idleRpm, float redline, float throttle, float tallGearScale)
		{
			float num = Mathf.Lerp(idleRpm + 400f, Mathf.Min(3200f, redline * 0.45f), Mathf.Clamp01(throttle));
			return idleRpm + (num - idleRpm) * Mathf.Clamp01(tallGearScale);
		}

		public static float LaunchFlare01(float currentRpm, float launchTargetRpm)
		{
			return Mathf.Clamp01((launchTargetRpm - currentRpm) / 600f);
		}

		public static float LaunchFlareDemand01(float throttle)
		{
			return Mathf.Clamp01((throttle - 0.3f) / 0.4f);
		}

		public static float LaunchFlare(float engineRpm, float syncRpm, float idleRpm, float redline, float throttle)
		{
			return LaunchFlare(engineRpm, syncRpm, idleRpm, redline, throttle, 1f);
		}

		public static float AntiStallOpening01(float engineRpm, float idleRpm, float floorRpm)
		{
			if (engineRpm >= idleRpm)
			{
				return 0f;
			}
			float num = idleRpm - floorRpm;
			if (num <= 0f)
			{
				return 1f;
			}
			return Mathf.Clamp01((idleRpm - engineRpm) / num);
		}

		public static float LaunchTakeUp01(float currentRpm, float launchTargetRpm, float throttle)
		{
			return Mathf.Clamp01((currentRpm - (launchTargetRpm - 300f)) / 600f) * LaunchFlareDemand01(throttle);
		}

		public static float LaunchFlare(float engineRpm, float syncRpm, float idleRpm, float redline, float throttle, float tallGearScale)
		{
			float launchTargetRpm = LaunchTargetRpm(idleRpm, redline, throttle, tallGearScale);
			return Mathf.Min(LaunchFlare01(engineRpm, launchTargetRpm), LaunchFlare01(syncRpm + 600f, launchTargetRpm)) * LaunchFlareDemand01(throttle);
		}

		public static float RevMatchTargetRpm(float wDiffAbs, float gearRatio, float finalDrive, float redline, float overshoot)
		{
			return Mathf.Min(wDiffAbs * gearRatio * finalDrive * (30f / (float)Math.PI) * overshoot, redline);
		}

		public static float LaunchTorqueFrac(float engineTorque, float clutchCapacity)
		{
			if (clutchCapacity <= 1f)
			{
				return 1f;
			}
			return Mathf.Clamp01(Mathf.Abs(engineTorque) * 1.3f / clutchCapacity);
		}

		public static float LaunchTorqueEngagementCap(float engineTorque, float clutchCapacity)
		{
			return Mathf.Sqrt(LaunchTorqueFrac(engineTorque, clutchCapacity));
		}

		public static float TurboSpoolStep(float current, float rpm, float load, bool onThrottle, float maxPressure, float minRpm, float maxRpm, TurboType turbo, float dt)
		{
			float target = (onThrottle ? CalcTurboTargetPressure(rpm, load, maxPressure, minRpm, maxRpm) : 0f);
			float num = TurboSpoolTau(turbo);
			float tauDown = (onThrottle ? (num * 3f) : 0.08f);
			return Mathf.Clamp(SpoolTowards(current, target, dt, num, tauDown), 0f, maxPressure);
		}

		public static float TurboSpoolTau(TurboType turbo)
		{
			return turbo switch
			{
				TurboType.SingleTurboMonoScroll => 0.45f, 
				TurboType.SingleTurboTwinScroll => 0.4f, 
				TurboType.TwinTurboSingleScroll => 0.32f, 
				TurboType.TwinTurboTwinScroll => 0.28f, 
				TurboType.BiTurbo => 0.3f, 
				TurboType.QuadTurboCharger => 0.55f, 
				_ => 0f, 
			};
		}

		public static float TorqueConverterStallRpm(float torquePeakRpm, float upperRpm, float idleRpm, float peakFraction, float idleMultiplier)
		{
			return Mathf.Max(Mathf.Lerp(torquePeakRpm, upperRpm, 0.5f) * peakFraction, idleRpm * idleMultiplier);
		}

		public static float TallGearLaunchStallScale(float totalRatio, float firstGearTotalRatio, float wheelMatchedRpm, float lockupRpm, float minScale)
		{
			float b = TallGearLaunchScale(totalRatio, firstGearTotalRatio, minScale);
			float t = 1f - Mathf.Clamp01(wheelMatchedRpm / Mathf.Max(lockupRpm, 1f));
			return Mathf.Lerp(1f, b, t);
		}

		public static float TallGearLaunchScale(float totalRatio, float firstGearTotalRatio, float minScale)
		{
			float num = Mathf.Abs(firstGearTotalRatio);
			float num2 = Mathf.Abs(totalRatio);
			if (num < 0.0001f || num2 < 0.0001f)
			{
				return 1f;
			}
			return Mathf.Clamp(num2 / num, minScale, 1f);
		}

		public static float WheelSpinFromTravel(float distance, float elapsed, float radius)
		{
			if (elapsed <= 0f || radius <= 0f)
			{
				return 0f;
			}
			return distance / elapsed / radius;
		}

		public static float SuspensionUsableTravel(float maxSpringLen, float staticSag)
		{
			float a = Mathf.Max(maxSpringLen, 0f) * 0.45f;
			float b = Mathf.Max(staticSag, 0f) / 0.5f;
			return Mathf.Max(a, b);
		}

		public static float SuspensionBumpEngage(float maxSpringLen, float staticSag)
		{
			float num = SuspensionUsableTravel(maxSpringLen, staticSag);
			float num2 = Mathf.Clamp(staticSag, 0f, num);
			return num2 + (num - num2) * 0.6f;
		}

		public static float SuspensionBumpDampingScale(float compression, float maxSpringLen, float staticSag)
		{
			float num = SuspensionUsableTravel(maxSpringLen, staticSag);
			float num2 = SuspensionBumpEngage(maxSpringLen, staticSag);
			float num3 = num - num2;
			if (num3 <= 0f)
			{
				return 3f;
			}
			float num4 = Mathf.Clamp01(Mathf.Max(Mathf.Max(compression, 0f) - num2, 0f) / num3);
			return Mathf.Lerp(1f, 3f, num4 * num4);
		}

		public static float SuspensionSpringForce(float compression, float maxSpringLen, float springCoef, float staticSag)
		{
			float num = Mathf.Max(compression, 0f);
			float num2 = SuspensionBumpEngage(maxSpringLen, staticSag);
			float num3 = Mathf.Min(num, num2);
			float num4 = Mathf.Max(num - num2, 0f);
			float num5 = Mathf.Min(springCoef * 8f, 500000f);
			return springCoef * num3 + num5 * num4;
		}

		public static float TyreVerticalSpringForce(float deflection, float tyreThickness, float sidewallStiffness)
		{
			float num = tyreThickness * 0.65f;
			float num2 = Mathf.Min(deflection, num);
			float num3 = Mathf.Max(deflection - num, 0f);
			return sidewallStiffness * num2 + 800000f * num3;
		}

		public static float TyreDampingRatio(float deflection, float tyreThickness)
		{
			float num = tyreThickness * 0.65f;
			if (num <= 0f)
			{
				return 0.5f;
			}
			float num2 = Mathf.Clamp01(Mathf.Max(deflection, 0f) / num);
			return Mathf.Lerp(0.15f, 0.5f, num2 * num2);
		}

		public static float TyreVerticalDamping(float sidewallStiffness, float deflection, float tyreThickness, float unsprungMass, float dt)
		{
			deflection = Mathf.Max(deflection, 0f);
			float num = tyreThickness * 0.65f;
			float num2 = TyreDamping((deflection > num) ? 800000f : sidewallStiffness, TyreSupportedMass(sidewallStiffness, deflection, tyreThickness, unsprungMass), TyreDampingRatio(deflection, tyreThickness));
			if (dt <= 0f)
			{
				return num2;
			}
			return Mathf.Min(num2, Mathf.Max(unsprungMass, 1f) / dt);
		}

		public static float TyreSupportedMass(float sidewallStiffness, float deflection, float tyreThickness, float unsprungMass)
		{
			return Mathf.Max(TyreVerticalSpringForce(Mathf.Max(deflection, 0f), tyreThickness, sidewallStiffness) / 9.80665f, Mathf.Max(unsprungMass, 0f));
		}

		public static float TyreDamping(float axisStiffness, float supportedMass, float dampingRatio)
		{
			return 2f * dampingRatio * Mathf.Sqrt(Mathf.Max(axisStiffness, 0f) * Mathf.Max(supportedMass, 0f));
		}

		public static bool AbsIsArmed(bool wheelOnGround, int absLevel, bool parkingGear, float brakeRequest, float referenceAngularVelocity)
		{
			if (((!wheelOnGround || absLevel <= 0) | parkingGear) || brakeRequest < 100f)
			{
				return false;
			}
			return Mathf.Abs(referenceAngularVelocity) >= 2f;
		}

		public static float AirborneBrakeCap(float wheelInertia, float angularVelocity)
		{
			return Mathf.Max(Mathf.Abs(wheelInertia) * Mathf.Abs(angularVelocity) / 0.5f, 100f);
		}

		public static float EspReferenceYawRate(float speed, float steerAngleRad, float wheelBase, float maxLateralAccel)
		{
			if (wheelBase <= 0f || speed <= 0.1f)
			{
				return 0f;
			}
			float num = speed / 22f;
			float value = speed * steerAngleRad / (wheelBase * (1f + num * num));
			float num2 = Mathf.Max(maxLateralAccel, 0.1f) / speed;
			return Mathf.Clamp(value, 0f - num2, num2);
		}

		public static float EspSideslipDegrees(float forwardSpeed, float lateralSpeed)
		{
			float num = Mathf.Abs(forwardSpeed);
			if (num <= 0.01f && Mathf.Abs(lateralSpeed) <= 0.01f)
			{
				return 0f;
			}
			return Mathf.Atan2(lateralSpeed, num) * 57.29578f;
		}

		public static float EspBrakeTorqueLimit(float verticalLoad, float loadedRadius, float maxLateralAccel)
		{
			float num = Mathf.Max(maxLateralAccel, 0.1f) / 9.80665f;
			return Mathf.Max(verticalLoad, 0f) * Mathf.Max(loadedRadius, 0.05f) * num;
		}

		public static float EspSpinBrakeShare(float overspeedRadPerSec)
		{
			return Mathf.Clamp01((overspeedRadPerSec - 5f) / 20f);
		}

		public static float EspSecondWheelShare(float sideslipDeg)
		{
			return Mathf.Clamp01((Mathf.Abs(sideslipDeg) - 15f) / 10f);
		}

		public static bool EspBodyIsSliding(float sideslipDeg, float sideslipRateDegPerSec)
		{
			float num = Mathf.Abs(sideslipDeg);
			if (num < 2f)
			{
				return false;
			}
			if (num >= 8f)
			{
				return true;
			}
			return sideslipRateDegPerSec * Mathf.Sign(sideslipDeg) >= 15f;
		}

		public static EspCorrection EspCorrectionFor(float yawRate, float referenceYawRate, float deadZone, float fullAuthority)
		{
			float num = yawRate - referenceYawRate;
			float num2 = Mathf.Abs(num);
			if (num2 <= deadZone)
			{
				return default;
			}
			float num3 = Mathf.Max(fullAuthority - deadZone, 0.0001f);
			float intensity = Mathf.Clamp01((num2 - deadZone) / num3);
			bool flag = Mathf.Abs(yawRate) > Mathf.Abs(referenceYawRate);
			return new EspCorrection(active: true, !flag, num < 0f, intensity);
		}

		public static float BrakeHeatCapacity(BrakeDiscType type)
		{
			return type switch
			{
				BrakeDiscType.CARBON => 710f, 
				BrakeDiscType.CERAMIC => 750f, 
				_ => 500f, 
			};
		}

		public static float BrakeMaxTemperature(BrakeDiscType type)
		{
			return type switch
			{
				BrakeDiscType.SOLID => 500f, 
				BrakeDiscType.VENTED => 700f, 
				BrakeDiscType.DRILLED => 750f, 
				BrakeDiscType.GROOVED => 800f, 
				BrakeDiscType.CARBON => 1000f, 
				BrakeDiscType.CERAMIC => 1300f, 
				_ => 500f, 
			};
		}

		public static float BrakeCoolingRate(BrakeDiscType type)
		{
			return type switch
			{
				BrakeDiscType.CARBON => 1.5f, 
				BrakeDiscType.CERAMIC => 1.8f, 
				_ => 2f, 
			};
		}

		public static float BrakeHeatStep(float temperature, float torque, float balance, float input, float angularSpeed, BrakeDiscType type, float mass, float airTemperature, float dt, float gripTorque)
		{
			float num = Mathf.Min(torque * balance * input, gripTorque);
			float num2 = 5f * num * angularSpeed * dt / (BrakeHeatCapacity(type) * Mathf.Max(mass, 0.1f));
			return Mathf.Clamp(temperature + num2, airTemperature, BrakeMaxTemperature(type));
		}

		public static float BrakeCoolStep(float temperature, float speedMps, BrakeDiscType type, float airTemperature, float dt, float wetCoolingMul = 1f)
		{
			float num = Mathf.Lerp(1f, 10f, Mathf.InverseLerp(0f, 60f, speedMps));
			float num2 = temperature - airTemperature;
			return Mathf.Max(temperature - BrakeCoolingRate(type) * num * wetCoolingMul * (num2 / 100f) * dt, airTemperature);
		}

		public static float TyreThermalStep(float temperature, float heatPowerIn, float coolingRate, float airTemperature, float heatCapacity, float maxTemperature, float dt)
		{
			float num = coolingRate * (temperature - airTemperature);
			float num2 = (heatPowerIn * 0.85f - num) / Mathf.Max(heatCapacity, 1E-06f);
			float num3 = temperature + num2 * dt;
			if (float.IsNaN(num3) || float.IsInfinity(num3))
			{
				num3 = ((float.IsNaN(temperature) || float.IsInfinity(temperature)) ? airTemperature : temperature);
			}
			return Mathf.Clamp(num3, airTemperature, maxTemperature);
		}

		public static float ContactSlipSpeed(float angularVelocity, float radiusLoaded, float forwardVelocity, float sideVelocity)
		{
			return Mathf.Sqrt(Sqr(angularVelocity * radiusLoaded - forwardVelocity) + Sqr(sideVelocity));
		}

		private static float SlideShareRamp(float kappa)
		{
			return Mathf.Clamp01((kappa - 0.12f) / 0.18f);
		}

		public static float TyreLongSlidingShare(float angularVelocity, float radiusLoaded, float forwardVelocity)
		{
			float num = angularVelocity * radiusLoaded;
			float num2 = Mathf.Abs(num - forwardVelocity);
			float num3 = Mathf.Max(Mathf.Abs(num), Mathf.Abs(forwardVelocity), 1f);
			return SlideShareRamp(num2 / num3);
		}

		public static float TyreLatSlidingShare(float forwardVelocity, float sideVelocity)
		{
			return SlideShareRamp(Mathf.Abs(sideVelocity) / Mathf.Max(Mathf.Abs(forwardVelocity), 1f));
		}

		public static float TyreSurfaceThermalStep(float surfaceTemp, float heatPowerIn, float carcassTemp, float airCooling, float airTemperature, float heatCapacity, float dt)
		{
			float num = 100f * (surfaceTemp - carcassTemp) + airCooling * (surfaceTemp - airTemperature);
			float num2 = surfaceTemp + (heatPowerIn - num) / Mathf.Max(heatCapacity, 1E-06f) * dt;
			if (float.IsNaN(num2) || float.IsInfinity(num2))
			{
				num2 = ((!float.IsNaN(surfaceTemp) && !float.IsInfinity(surfaceTemp)) ? surfaceTemp : ((float.IsNaN(carcassTemp) || float.IsInfinity(carcassTemp)) ? airTemperature : carcassTemp));
			}
			return Mathf.Clamp(num2, airTemperature, 350f);
		}

		public static void WheelContactRayOffset(int index, float width, float radius, out float lateral, out float longitudinal)
		{
			if (index <= 0)
			{
				lateral = 0f;
				longitudinal = 0f;
				return;
			}
			float num = width * 0.5f * 0.8f;
			float num2 = radius * 0.3f;
			lateral = ((index == 1 || index == 3) ? (0f - num) : num);
			longitudinal = ((index == 1 || index == 2) ? num2 : (0f - num2));
		}

		public static float WheelContactRaySagitta(float longitudinal, float radius)
		{
			float num = Mathf.Max(radius, 0.0001f);
			float num2 = longitudinal * longitudinal;
			float num3 = num * num - num2;
			if (!(num3 <= 0f))
			{
				return num - Mathf.Sqrt(num3);
			}
			return num;
		}

		public static int SelectWheelContactRay(float[] roadDistances, bool[] hits, float[] sagittas, int count, int previousBest = -1)
		{
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < count; i++)
			{
				if (hits[i])
				{
					float num3 = roadDistances[i] + sagittas[i];
					if (num < 0 || !(num3 >= num2))
					{
						num = i;
						num2 = num3;
					}
				}
			}
			if (num < 0)
			{
				return -1;
			}
			if (previousBest >= 0 && previousBest < count && hits[previousBest] && roadDistances[previousBest] + sagittas[previousBest] <= num2 + 0.005f)
			{
				return previousBest;
			}
			return num;
		}

		public static Vector3 ClampVisualContactNormal(Vector3 roadNormal, Vector3 wheelUp, float minDot)
		{
			Vector3 vector = ((wheelUp.sqrMagnitude > 1E-12f) ? wheelUp.normalized : Vector3.up);
			if (roadNormal.sqrMagnitude <= 1E-12f)
			{
				return vector;
			}
			Vector3 normalized = roadNormal.normalized;
			float num = Vector3.Dot(normalized, vector);
			if (num >= minDot)
			{
				return normalized;
			}
			Vector3 vector2 = normalized - vector * num;
			if (vector2.sqrMagnitude <= 1E-12f)
			{
				return vector;
			}
			vector2.Normalize();
			float num2 = Mathf.Sqrt(Mathf.Max(0f, 1f - minDot * minDot));
			return (vector * minDot + vector2 * num2).normalized;
		}

		public static float TreadShearMagnitude(float force, float sidewallStiffness, float tyreThickness)
		{
			if (force <= 1E-06f)
			{
				return 0f;
			}
			return Mathf.Min(force * 2.2f / Mathf.Max(sidewallStiffness, 1f), tyreThickness);
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
			TorqueCurveSample[] array = new TorqueCurveSample[30];
			float torqueDropOff = GetTorqueDropOff((TurboType)turboType);
			float num = Mathf.Max(peakTorqueRPM, 1f);
			float num2 = Mathf.Max(maxRpm - num, 1f);
			float num3 = 14500f / (float)(array.Length - 1);
			for (int i = 0; i < array.Length; i++)
			{
				float num4 = 500f + (float)i * num3;
				float a;
				if (num4 <= num)
				{
					float num5 = 1f - num4 / num;
					a = 1f - 0.35f * num5 * num5;
				}
				else if (num4 <= maxRpm)
				{
					float num6 = (num4 - num) / num2;
					a = 1f - num6 * num6 * num6 * (1f - torqueDropOff);
				}
				else
				{
					a = 0f;
				}
				array[i].Rpm = num4;
				array[i].Torque = maxTorque * Mathf.Max(a, 0f);
			}
			return array;
		}

		private static float SmoothStep(float x)
		{
			return x * x * (3f - 2f * x);
		}

		private static float GetTorqueDropOff(TurboType turboType)
		{
			return turboType switch
			{
				TurboType.SingleTurboMonoScroll => 0.35f, 
				TurboType.SingleTurboTwinScroll => 0.4f, 
				TurboType.TwinTurboSingleScroll => 0.32f, 
				TurboType.TwinTurboTwinScroll => 0.38f, 
				TurboType.BiTurbo => 0.36f, 
				TurboType.QuadTurboCharger => 0.55f, 
				TurboType.Supercharger => 0.5f, 
				TurboType.NA => 0.25f, 
				_ => 0.3f, 
			};
		}

		public static float Mm2Meter(float i)
		{
			return i * 0.001f;
		}

		public static float Mm2Meter(TyreWidth i)
		{
			return (float)i * 0.001f;
		}

		public static float Sqr(float i)
		{
			return i * i;
		}

		public static float CalculateWheelRadius(TyreWidth width, TyreProfile profile, WheelDiameter wheelDiameter)
		{
			float num = (float)width * ((float)profile * 0.01f);
			return Mm2Meter((float)wheelDiameter * 25.4f + 2f * num) * 0.5f;
		}

		public static Vector3 Vector3Mul(Vector3 a, float d)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
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

		public static float CalcPumpingBrakeTorque(float rpm, float rpmPeak, float displacementCc, bool isRotary)
		{
			if (rpm < 10f || displacementCc <= 0f)
			{
				return 0f;
			}
			float num = Mathf.Max(rpmPeak, 1f);
			float num2 = 80000f + 160000f * rpm / num;
			float num3 = (isRotary ? 2f : 4f) * (float)Math.PI;
			return num2 * (displacementCc * 1E-06f) / num3;
		}

		public static float EstimateDisplacementCc(float atmoPeakTorque)
		{
			if (atmoPeakTorque <= 0f)
			{
				return 800f;
			}
			return Mathf.Clamp(atmoPeakTorque * 4f * (float)Math.PI / 1000000f * 1000000f, 800f, 6500f);
		}

		public static float MinEngineInertia(float displacementCc)
		{
			if (displacementCc <= 0f)
			{
				return 0.05f;
			}
			return Mathf.Max(0.05f, 0.04f * displacementCc / 1000f);
		}

		public static float NormalizeTorqueSample(float rawTorque, float boostMultiplier, float legacyBrake, float pumpingBrake)
		{
			float num = Mathf.Max(boostMultiplier, 1f);
			return Mathf.Max(((rawTorque + legacyBrake) * num - legacyBrake + pumpingBrake) / num - pumpingBrake, 0f);
		}

		public static float DynoTorqueAtRpm(float normalizedTorque, float boostMultiplier, float pumpingBrake)
		{
			float num = Mathf.Max(boostMultiplier, 1f);
			return (normalizedTorque + pumpingBrake) * num - pumpingBrake;
		}

		public static float DynoTorqueFromRawSample(float rawTorque, float boostMultiplier, float legacyBrake, float pumpingBrake)
		{
			return DynoTorqueAtRpm(NormalizeTorqueSample(rawTorque, boostMultiplier, legacyBrake, pumpingBrake), boostMultiplier, pumpingBrake);
		}

		public static float LegacyBrakeTorque(float rpm, float rpmPeak, float boostedMaxTorque)
		{
			if (rpm < 10f)
			{
				return 0f;
			}
			return 0.07f * (0.5f + 1.5f * rpm / Mathf.Max(rpmPeak, 1f)) * boostedMaxTorque;
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
	}
}
