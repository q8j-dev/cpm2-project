using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IObstacleRaceZoneRule
	{
		string Name { get; }

		List<ObstacleRaceZoneInfo> Filter(List<ObstacleRaceZoneInfo> candidates, in ObstacleRaceSlotContext slot);
	}
}
