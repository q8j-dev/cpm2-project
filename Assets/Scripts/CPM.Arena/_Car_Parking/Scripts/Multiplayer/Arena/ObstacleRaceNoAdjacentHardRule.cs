using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceNoAdjacentHardRule : IObstacleRaceOrderRule
	{
		public string Name => "no-adjacent-hard";

		public bool IsAllowed(IReadOnlyList<ObstacleRaceZoneDifficulty> slots)
		{
			for (int i = 1; i < slots.Count; i++)
			{
				if (slots[i] == ObstacleRaceZoneDifficulty.Hard && slots[i - 1] == ObstacleRaceZoneDifficulty.Hard)
				{
					return false;
				}
			}
			return true;
		}
	}
}
