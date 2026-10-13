namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public readonly struct EngineTimerInfo
	{
		public readonly string TimerId;

		public readonly double RemainingSeconds;

		public readonly double AdReductionSeconds;

		public readonly bool IsEngineInstall;

		public readonly bool HasGearBox;

		public EngineTimerInfo(string timerId, double remainingSeconds, double adReductionSeconds, bool isEngineInstall, bool hasGearBox)
		{
			TimerId = timerId;
			RemainingSeconds = remainingSeconds;
			AdReductionSeconds = adReductionSeconds;
			IsEngineInstall = isEngineInstall;
			HasGearBox = hasGearBox;
		}
	}
}
