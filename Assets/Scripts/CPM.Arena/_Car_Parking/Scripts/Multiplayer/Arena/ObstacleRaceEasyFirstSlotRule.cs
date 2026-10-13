using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceEasyFirstSlotRule : IObstacleRaceOrderRule
	{
		public string Name => "easy-first-slot";

		public bool IsAllowed(IReadOnlyList<ObstacleRaceZoneDifficulty> slots)
		{
			if (slots.Count != 0)
			{
				return slots[0] == ObstacleRaceZoneDifficulty.Easy;
			}
			return true;
		}
	}
}
