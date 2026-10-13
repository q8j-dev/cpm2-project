namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class RaceFinishContext : IEventContext
	{
		public int RacePosition;

		public float RaceFinishTime;

		public float BestLap;

		public float ModeDuration;

		public float Penalty;

		public int CountAwards;

		public int RewardSoft;

		public int RewardHard;

		public string ExitReason;
	}
}
