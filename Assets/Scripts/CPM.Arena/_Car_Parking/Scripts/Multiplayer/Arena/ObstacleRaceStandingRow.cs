namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceStandingRow
	{
		public readonly int Actor;

		public readonly int Place;

		public readonly string UserName;

		public readonly bool IsLocal;

		public readonly bool IsRunning;

		public readonly double? FinishTime;

		public readonly float? RemainingMeters;

		public ObstacleRaceStandingRow(int actor, int place, string userName, bool isLocal, bool isRunning, double? finishTime, float? remainingMeters)
		{
			Actor = actor;
			Place = place;
			UserName = userName;
			IsLocal = isLocal;
			IsRunning = isRunning;
			FinishTime = finishTime;
			RemainingMeters = remainingMeters;
		}
	}
}
