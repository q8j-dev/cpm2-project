using System;
using CPM.Transmission;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public struct VehicleDataResult
	{
		public ObscuredInt Points;

		public ObscuredInt EngineType;

		public int EngineId;

		public TorqueCurveSample[] Torque;

		public MaintenanceInfo MaintenanceData;

		public FrontWheelData FrontWheel;

		public RearWheelData RearWheel;

		public GearboxType GearBoxType;

		public TurboType TurboType;

		public int ExhaustType;

		public int BlowOffType;

		public int SuperChargerType;

		public int BurbleType;

		public bool CheckEngine;

		public bool IsBroken;

		public float Displacement;

		public float IdleRpm;

		public float RedlineRpm;

		public float MaxRpm;

		public float EngineInertia;

		public float EngineBrakeInertia;

		public GearboxKind Kind;

		public AutoShifterModes AutoModes;

		public int LowMaxGear;

		public float[] GearRatios;

		public LSDValues FrontLsd;

		public LSDValues RearLsd;

		public float GearShiftDelay;

		public float EndRatio;

		public float GearboxInertia;

		public float TorqueSplit;

		public float MaxClutchF;

		public float MaxSteeringAngle;

		public float CurrentFrontSuspensionHeight;

		public float CurrentRearSuspensionHeight;

		public float MinFrontSuspensionHeight;

		public float MaxFrontSuspensionHeight;

		public float MinRearSuspensionHeight;

		public float MaxRearSuspensionHeight;

		public float HeightAdjustSpeed;

		public bool HaveLc;

		public float LcRpm;

		public float LcTime;

		public int TurboPressure;

		public float MinTurboRpm;

		public float MaxTurboRpm;

		public float FuelConsumptionAtCostantSpeed;

		public float FuelConsumptionSpeed;

		public float Weight;

		public override string ToString()
		{
			return " " + Torque;
		}
	}
}
