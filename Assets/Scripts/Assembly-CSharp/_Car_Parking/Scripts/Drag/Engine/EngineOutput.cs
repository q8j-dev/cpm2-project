using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public struct EngineOutput
	{
		public TorqueCurveSample[] Torque;

		public float Displacement;

		public float IdleRpm;

		public float RedlineRpm;

		public float MaxRpm;

		public float Inertia;

		public float BrakeInertia;

		public TurboType TurboType;

		public int TurboPressure;

		public float MinTurboRpm;

		public float MaxTurboRpm;

		public float ShiftDelay;

		public float MaxClutchF;

		public bool IsBroken;

		public bool CheckEngine;

		public bool isRichMixture;

		public float FuelConsumption;

		public float FuelConsumptionSpeed;

		public int SoundId;
	}
}
