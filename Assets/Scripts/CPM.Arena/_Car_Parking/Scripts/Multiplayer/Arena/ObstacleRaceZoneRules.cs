namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceZoneRules
	{
		public static readonly IObstacleRaceZoneRule[] Default = new IObstacleRaceZoneRule[2]
		{
			new ObstacleRaceDifficultyRule(),
			new ObstacleRaceNoAdjacentMechanicRule()
		};
	}
}
