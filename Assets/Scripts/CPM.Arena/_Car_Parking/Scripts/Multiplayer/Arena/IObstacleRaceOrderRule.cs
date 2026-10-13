using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IObstacleRaceOrderRule
	{
		string Name { get; }

		bool IsAllowed(IReadOnlyList<ObstacleRaceZoneDifficulty> slots);
	}
}
