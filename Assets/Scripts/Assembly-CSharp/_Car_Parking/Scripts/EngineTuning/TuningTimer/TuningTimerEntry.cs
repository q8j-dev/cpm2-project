namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public readonly struct TuningTimerEntry
	{
		public readonly PartData Part;

		public readonly EngineKitData EngineKit;

		public readonly double WaitSeconds;

		public readonly double RemainingSeconds;

		public bool IsEngine => EngineKit != null;

		public TuningTimerEntry(PartData part, EngineKitData engineKit, double waitSeconds, double remainingSeconds)
		{
			Part = part;
			EngineKit = engineKit;
			WaitSeconds = waitSeconds;
			RemainingSeconds = remainingSeconds;
		}
	}
}
