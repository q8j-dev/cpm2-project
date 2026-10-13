using System;

namespace CPMEngine
{
	[Serializable]
	public struct FuelMap
	{
		public float RPM;

		public float pressure;

		public float Time;

		public float Fuel;

		public FuelMap(float rpm)
		{
			RPM = rpm;
			pressure = 2f;
			Time = 0f;
			Fuel = 0f;
		}
	}
}
