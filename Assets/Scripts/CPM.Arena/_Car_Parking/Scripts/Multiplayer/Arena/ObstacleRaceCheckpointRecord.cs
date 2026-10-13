namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceCheckpointRecord
	{
		public int Id { get; }

		public double? Time { get; }

		public int Restarts { get; }

		public bool Missed { get; }

		public ObstacleRaceCheckpointRecord(int id, double? time, int restarts, bool missed)
		{
			Id = id;
			Time = time;
			Restarts = restarts;
			Missed = missed;
		}
	}
}
