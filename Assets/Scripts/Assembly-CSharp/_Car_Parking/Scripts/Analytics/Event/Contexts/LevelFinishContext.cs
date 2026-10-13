namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public struct LevelFinishContext : IEventContext
	{
		public string LevelId;

		public float PlayDuration;

		public float ModeDuration;

		public int RewardSoft;

		public float BestCompletionTime;

		public int AttemptNumber;

		public string FinishReason;
	}
}
