namespace _Car_Parking.Scripts.GooglePlayServices
{
	public readonly struct AchievementServerState
	{
		public string Id { get; }

		public bool Completed { get; }

		public double PercentCompleted { get; }

		public AchievementServerState(string id, bool completed, double percentCompleted)
		{
			Id = id;
			Completed = completed;
			PercentCompleted = percentCompleted;
		}
	}
}
