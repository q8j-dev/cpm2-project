using CPM.Transmission;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public struct TransmissionValues
	{
		public float TorqueSplit;

		public float[] GearRatio;

		public float FinalDrive;

		public bool LaunchControl;

		public float LaunchRpm;

		public float LaunchTime;

		public float Inertia;

		public GearboxKind Kind;

		public AutoShifterModes AutoModes;

		public int LowMaxGear;

		public int Type;
	}
}
