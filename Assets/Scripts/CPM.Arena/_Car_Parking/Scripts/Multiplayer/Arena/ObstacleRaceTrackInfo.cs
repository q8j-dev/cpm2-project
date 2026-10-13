namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceTrackInfo
	{
		public int? Seed { get; }

		public string[] Plan { get; }

		public int ParticipantCount { get; }

		public ObstacleRaceTrackInfo(int? seed, string[] plan, int participantCount)
		{
			Seed = seed;
			Plan = plan;
			ParticipantCount = participantCount;
		}
	}
}
