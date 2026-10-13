namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceOrderRules
	{
		public static readonly IObstacleRaceOrderRule[] Default = new IObstacleRaceOrderRule[4]
		{
			new ObstacleRaceEasyFirstSlotRule(),
			new ObstacleRaceNoHardOpeningRule(),
			new ObstacleRaceNoAdjacentHardRule(),
			new ObstacleRaceMaxSameDifficultyRunRule()
		};
	}
}
