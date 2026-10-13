namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class OpenWorldFinishContext : IEventContext
	{
		public float ModeDuration;

		public int TaxiOrdersCompleted;

		public int TaxiOrdersEarned;

		public int CheckpointEarned;

		public string ExitReason;
	}
}
