using System;
using CPM.Transmission;
using Unity.Mathematics;
using _Car_Parking.Scripts.Drag.Engine;

namespace CPMEngine
{
	[Serializable]
	public class DynoData
	{
		[Serializable]
		public class Body
		{
			public float Mass;

			public float FrontPercent;

			public float UpPercent;

			public float Width;

			public float Height;

			public float Length;

			public int BrakeBalance;
		}

		[Serializable]
		public class AeroDynamics
		{
			public float Cx;

			public float Sx;

			public float FrontDownforce;

			public float RearDownforce;
		}

		[Serializable]
		public class CarEngine
		{
			public bool IsBroken;

			public CarOptionsValues Options;

			public EngineOrientation Orientation;

			public TorqueCurveSample[] TorqueCurveSamples;

			public float MaxTorque;

			public float MaxTorqueRpm;

			public float Displacement;

			public bool IsRotary;

			public float IdleRpm;

			public float RedlineRpm;

			public float MaxRpm;

			public float InertiaRatio;

			public float BrakeTorqueRatio;

			public TurboType TurboType;

			public int TurboPressure;

			public float2 TurboRange;

			public float FuelConsumptionSpeed;

			public float FuelConsumptionAtCostantSpeed;

			public float LcRpm;

			public float LcTime;
		}

		[Serializable]
		public class Gearbox
		{
			public float[] Gears;

			public float FinalDrive;

			public float GearShiftingDelay;

			public float InertiaRatio;
		}

		[Serializable]
		public class SteerSuspension : Suspension
		{
			public float Caster;

			public float Ackerman;

			public float MaxSteerAngle;
		}

		[Serializable]
		public class Suspension
		{
			public float AddSteerAngle;

			public SuspensionType SuspensionType;

			public float SpringLength;

			public float Stiffness;

			public PneumaticBagSystem PneumaticBagSystem;

			public float Bump;

			public float Rebound;

			public float AntirollBar;

			public float Camber;

			public float ToeIn;

			public float Spacers;

			public float BrakeDiskTorque;

			public float BrakeDiskMass;

			public BrakeDiscType BrakeDiscType;
		}

		[Serializable]
		public class PneumaticBagSystem
		{
			public float CurrentSpringLength;

			public float2 SuspensionHeight;

			public float TargetHeight;

			public float HeightAdjustSpeed;

			public float Sag;

			public float MinSpringLen;

			public float MaxSpringLen;
		}

		[Serializable]
		public class Tranny
		{
			public GearboxType GearType;

			public GearboxKind Kind;

			public AutoShifterModes AutoModes;

			public int LowMaxGear;

			public float TorqueSplit;

			public float DiffLossCoef;

			public float TransmissionLossCoef;

			public float ClutchMaxTorque;
		}

		[Serializable]
		public class LSD
		{
			public LSDValues FrontLSD = new LSDValues();

			public LSDValues RearLSD = new LSDValues();
		}

		[Serializable]
		public class Tyre
		{
			public TyreType TyreType;

			public WheelDiameter DiscDiam;

			public RimWidth DiscWidth;

			public TyreWidth Width;

			public TyreProfile Profile;

			public float Pressure;

			public float Mass;

			public float FrictionMultiplier;
		}

		public Body Chassis;

		public CarEngine Engine;

		public Tranny Transmission;

		public Gearbox GearBox;

		public LSD Lsd;

		public SteerSuspension FrontSuspension;

		public Suspension RearSuspension;

		public Tyre FrontTyre;

		public Tyre RearTyre;

		public AeroDynamics Aero;

		public DynoData()
		{
			Chassis = new Body();
			GearBox = new Gearbox();
			Transmission = new Tranny();
			Lsd = new LSD();
			Engine = new CarEngine();
			FrontTyre = new Tyre();
			RearTyre = new Tyre();
			FrontSuspension = new SteerSuspension
			{
				PneumaticBagSystem = new PneumaticBagSystem()
			};
			RearSuspension = new Suspension
			{
				PneumaticBagSystem = new PneumaticBagSystem()
			};
			Aero = new AeroDynamics();
			Reset();
		}

		public void CopyFrom(DynoData from)
		{
			Chassis.Mass = from.Chassis.Mass;
			Chassis.FrontPercent = from.Chassis.FrontPercent;
			Chassis.UpPercent = from.Chassis.UpPercent;
			Chassis.Length = from.Chassis.Length;
			Chassis.Width = from.Chassis.Width;
			Chassis.Height = from.Chassis.Height;
			Chassis.BrakeBalance = from.Chassis.BrakeBalance;
			GearBox.FinalDrive = from.GearBox.FinalDrive;
			GearBox.Gears = new float[from.GearBox.Gears.Length];
			from.GearBox.Gears.CopyTo(GearBox.Gears, 0);
			GearBox.GearShiftingDelay = from.GearBox.GearShiftingDelay;
			GearBox.InertiaRatio = from.GearBox.InertiaRatio;
			Transmission.GearType = from.Transmission.GearType;
			Transmission.Kind = from.Transmission.Kind;
			Transmission.AutoModes = from.Transmission.AutoModes;
			Transmission.LowMaxGear = from.Transmission.LowMaxGear;
			Transmission.TorqueSplit = from.Transmission.TorqueSplit;
			Transmission.DiffLossCoef = from.Transmission.DiffLossCoef;
			Transmission.TransmissionLossCoef = from.Transmission.TransmissionLossCoef;
			Transmission.ClutchMaxTorque = from.Transmission.ClutchMaxTorque;
			Lsd.FrontLSD = from.Lsd.FrontLSD;
			Lsd.RearLSD = from.Lsd.RearLSD;
			Engine.BrakeTorqueRatio = from.Engine.BrakeTorqueRatio;
			Engine.InertiaRatio = from.Engine.InertiaRatio;
			Engine.Orientation = from.Engine.Orientation;
			Engine.TorqueCurveSamples = new TorqueCurveSample[from.Engine.TorqueCurveSamples.Length];
			from.Engine.TorqueCurveSamples.CopyTo(Engine.TorqueCurveSamples, 0);
			Engine.IsBroken = from.Engine.IsBroken;
			Engine.MaxTorque = from.Engine.MaxTorque;
			Engine.MaxTorqueRpm = from.Engine.MaxTorqueRpm;
			Engine.Displacement = from.Engine.Displacement;
			Engine.IsRotary = from.Engine.IsRotary;
			Engine.IdleRpm = from.Engine.IdleRpm;
			Engine.RedlineRpm = from.Engine.RedlineRpm;
			Engine.MaxRpm = from.Engine.MaxRpm;
			Engine.TurboType = from.Engine.TurboType;
			Engine.TurboPressure = from.Engine.TurboPressure;
			Engine.TurboRange = from.Engine.TurboRange;
			Engine.Options = from.Engine.Options;
			Engine.FuelConsumptionSpeed = from.Engine.FuelConsumptionSpeed;
			Engine.FuelConsumptionAtCostantSpeed = from.Engine.FuelConsumptionAtCostantSpeed;
			Engine.LcRpm = from.Engine.LcRpm;
			Engine.LcTime = from.Engine.LcTime;
			FrontTyre.TyreType = from.FrontTyre.TyreType;
			FrontTyre.Width = from.FrontTyre.Width;
			FrontTyre.Profile = from.FrontTyre.Profile;
			FrontTyre.Pressure = from.FrontTyre.Pressure;
			FrontTyre.DiscDiam = from.FrontTyre.DiscDiam;
			FrontTyre.DiscWidth = from.FrontTyre.DiscWidth;
			FrontTyre.Mass = from.FrontTyre.Mass;
			FrontTyre.FrictionMultiplier = from.FrontTyre.FrictionMultiplier;
			RearTyre.TyreType = from.RearTyre.TyreType;
			RearTyre.Width = from.RearTyre.Width;
			RearTyre.Profile = from.RearTyre.Profile;
			RearTyre.Pressure = from.RearTyre.Pressure;
			RearTyre.DiscDiam = from.RearTyre.DiscDiam;
			RearTyre.DiscWidth = from.RearTyre.DiscWidth;
			RearTyre.Mass = from.RearTyre.Mass;
			RearTyre.FrictionMultiplier = from.RearTyre.FrictionMultiplier;
			FrontSuspension.AntirollBar = from.FrontSuspension.AntirollBar;
			FrontSuspension.AddSteerAngle = from.FrontSuspension.AddSteerAngle;
			FrontSuspension.SuspensionType = from.FrontSuspension.SuspensionType;
			FrontSuspension.SpringLength = from.FrontSuspension.SpringLength;
			FrontSuspension.Stiffness = from.FrontSuspension.Stiffness;
			FrontSuspension.PneumaticBagSystem = from.FrontSuspension.PneumaticBagSystem;
			FrontSuspension.Bump = from.FrontSuspension.Bump;
			FrontSuspension.Rebound = from.FrontSuspension.Rebound;
			FrontSuspension.ToeIn = from.FrontSuspension.ToeIn;
			FrontSuspension.Camber = from.FrontSuspension.Camber;
			FrontSuspension.Caster = from.FrontSuspension.Caster;
			FrontSuspension.Ackerman = from.FrontSuspension.Ackerman;
			FrontSuspension.MaxSteerAngle = from.FrontSuspension.MaxSteerAngle;
			FrontSuspension.Spacers = from.FrontSuspension.Spacers;
			FrontSuspension.BrakeDiskTorque = from.FrontSuspension.BrakeDiskTorque;
			FrontSuspension.BrakeDiscType = from.FrontSuspension.BrakeDiscType;
			FrontSuspension.BrakeDiskMass = from.FrontSuspension.BrakeDiskMass;
			RearSuspension.AntirollBar = from.RearSuspension.AntirollBar;
			RearSuspension.AddSteerAngle = from.RearSuspension.AddSteerAngle;
			RearSuspension.SuspensionType = from.RearSuspension.SuspensionType;
			RearSuspension.SpringLength = from.RearSuspension.SpringLength;
			RearSuspension.Stiffness = from.RearSuspension.Stiffness;
			RearSuspension.PneumaticBagSystem = from.RearSuspension.PneumaticBagSystem;
			RearSuspension.Bump = from.RearSuspension.Bump;
			RearSuspension.Rebound = from.RearSuspension.Rebound;
			RearSuspension.ToeIn = from.RearSuspension.ToeIn;
			RearSuspension.Camber = from.RearSuspension.Camber;
			RearSuspension.Spacers = from.RearSuspension.Spacers;
			RearSuspension.BrakeDiskTorque = from.RearSuspension.BrakeDiskTorque;
			RearSuspension.BrakeDiscType = from.RearSuspension.BrakeDiscType;
			RearSuspension.BrakeDiskMass = from.RearSuspension.BrakeDiskMass;
			Aero.Cx = from.Aero.Cx;
			Aero.Sx = from.Aero.Sx;
			Aero.FrontDownforce = from.Aero.FrontDownforce;
			Aero.RearDownforce = from.Aero.RearDownforce;
		}

		private void Reset()
		{
			Chassis.Mass = 1100f;
			Chassis.FrontPercent = 60f;
			Chassis.UpPercent = 25f;
			Chassis.Length = 4.625f;
			Chassis.Width = 1.6f;
			Chassis.Height = 1.3f;
			Chassis.BrakeBalance = 60;
			GearBox.FinalDrive = 3.9f;
			GearBox.Gears = new float[7] { 3.5f, 0f, 3.63f, 1.95f, 1.36f, 0.94f, 0.78f };
			GearBox.GearShiftingDelay = 0.25f;
			GearBox.InertiaRatio = 0.1f;
			Transmission.GearType = GearboxType.GearRwd;
			Transmission.Kind = GearboxKind.AT;
			Transmission.TorqueSplit = 50f;
			Transmission.DiffLossCoef = 0f;
			Transmission.TransmissionLossCoef = 0.1f;
			Transmission.ClutchMaxTorque = 500f;
			Lsd.FrontLSD.Type = LSDType.Open;
			Lsd.FrontLSD.AccelBias = 1.2f;
			Lsd.FrontLSD.BrakeBias = 1.2f;
			Lsd.FrontLSD.MaxBias = 1f;
			Lsd.FrontLSD.TorqueBiasRatio = 3f;
			Lsd.FrontLSD.LockPercent = 10f;
			Lsd.RearLSD.Type = LSDType.Open;
			Lsd.RearLSD.AccelBias = 1.2f;
			Lsd.RearLSD.BrakeBias = 1.2f;
			Lsd.RearLSD.MaxBias = 1f;
			Lsd.RearLSD.TorqueBiasRatio = 3f;
			Lsd.RearLSD.LockPercent = 10f;
			Engine.BrakeTorqueRatio = 0.07f;
			Engine.InertiaRatio = 1f;
			Engine.Orientation = EngineOrientation.Longitudinally;
			Engine.TorqueCurveSamples = Array.Empty<TorqueCurveSample>();
			Engine.IsBroken = false;
			Engine.MaxTorque = 120f;
			Engine.MaxTorqueRpm = 3400f;
			Engine.Displacement = 0f;
			Engine.IsRotary = false;
			Engine.IdleRpm = 1000f;
			Engine.RedlineRpm = 8000f;
			Engine.MaxRpm = 10000f;
			Engine.TurboType = TurboType.NA;
			Engine.TurboPressure = 0;
			Engine.TurboRange = new float2(2500f, 4000f);
			Engine.Options.AbsLevel = 5;
			Engine.Options.TrcLevel = 0;
			Engine.Options.EspLevel = 0;
			Engine.Options.Lc = true;
			Engine.Options.MTAssist = false;
			Engine.FuelConsumptionSpeed = 130f;
			Engine.FuelConsumptionAtCostantSpeed = 10f;
			Engine.LcRpm = 4000f;
			Engine.LcTime = 5f;
			FrontTyre.TyreType = TyreType.Touring;
			FrontTyre.Width = TyreWidth.W225;
			FrontTyre.Profile = TyreProfile.P45;
			FrontTyre.Pressure = 240000f;
			FrontTyre.DiscDiam = WheelDiameter.R17;
			FrontTyre.DiscWidth = RimWidth.W6;
			FrontTyre.Mass = 0f;
			FrontTyre.FrictionMultiplier = 1f;
			RearTyre.TyreType = TyreType.Touring;
			RearTyre.Width = TyreWidth.W225;
			RearTyre.Profile = TyreProfile.P45;
			RearTyre.Pressure = 240000f;
			RearTyre.DiscDiam = WheelDiameter.R17;
			RearTyre.DiscWidth = RimWidth.W6;
			RearTyre.Mass = 0f;
			RearTyre.FrictionMultiplier = 1f;
			FrontSuspension.BrakeDiskTorque = 2500f;
			FrontSuspension.BrakeDiscType = BrakeDiscType.SOLID;
			FrontSuspension.BrakeDiskMass = 5f;
			FrontSuspension.AntirollBar = 10000f;
			FrontSuspension.AddSteerAngle = 0f;
			FrontSuspension.SuspensionType = SuspensionType.Mcpherson;
			FrontSuspension.SpringLength = 0.15f;
			FrontSuspension.Stiffness = 45000f;
			FrontSuspension.Bump = 4000f;
			FrontSuspension.Rebound = 5000f;
			FrontSuspension.PneumaticBagSystem = new PneumaticBagSystem
			{
				CurrentSpringLength = 0.15f,
				SuspensionHeight = new float2(-1f, -1f),
				TargetHeight = -1f,
				HeightAdjustSpeed = 0.1f,
				Sag = 0f,
				MinSpringLen = 0f,
				MaxSpringLen = 0f
			};
			FrontSuspension.ToeIn = 0f;
			FrontSuspension.Camber = 0f;
			FrontSuspension.Caster = 4f;
			FrontSuspension.Ackerman = 0f;
			FrontSuspension.MaxSteerAngle = 34f;
			FrontSuspension.Spacers = 0f;
			RearSuspension.BrakeDiskTorque = 1800f;
			RearSuspension.BrakeDiscType = BrakeDiscType.SOLID;
			RearSuspension.BrakeDiskMass = 5f;
			RearSuspension.AntirollBar = 10000f;
			RearSuspension.AddSteerAngle = 0f;
			RearSuspension.SuspensionType = SuspensionType.Mcpherson;
			RearSuspension.SpringLength = 0.15f;
			RearSuspension.Stiffness = 45000f;
			RearSuspension.PneumaticBagSystem = new PneumaticBagSystem
			{
				CurrentSpringLength = 0.15f,
				SuspensionHeight = new float2(-1f, -1f),
				TargetHeight = -1f,
				HeightAdjustSpeed = 0.1f,
				Sag = 0f,
				MinSpringLen = 0f,
				MaxSpringLen = 0f
			};
			RearSuspension.Bump = 4000f;
			RearSuspension.Rebound = 5000f;
			RearSuspension.ToeIn = 0f;
			RearSuspension.Camber = 0f;
			RearSuspension.Spacers = 0f;
			Aero.Cx = 0.35f;
			Aero.Sx = 2.2f;
			Aero.FrontDownforce = 10f;
			Aero.RearDownforce = 10f;
		}
	}
}
