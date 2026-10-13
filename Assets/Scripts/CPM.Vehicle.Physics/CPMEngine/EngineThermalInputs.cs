namespace CPMEngine
{
	public readonly struct EngineThermalInputs
	{
		public readonly float AmbientC;

		public readonly float EnginePowerKw;

		public readonly float Rpm;

		public readonly float DisplacementCc;

		public readonly float SpeedMps;

		public readonly float CoolingCapacity;

		public readonly bool EngineRunning;

		public EngineThermalInputs(float ambientC, float enginePowerKw, float rpm, float displacementCc, float speedMps, float coolingCapacity, bool engineRunning)
		{
			AmbientC = ambientC;
			EnginePowerKw = enginePowerKw;
			Rpm = rpm;
			DisplacementCc = displacementCc;
			SpeedMps = speedMps;
			CoolingCapacity = coolingCapacity;
			EngineRunning = engineRunning;
		}
	}
}
