using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceBlock
	{
		public readonly IReadOnlyList<ObstacleRaceZoneDifficulty> Slots;

		public readonly bool KeepOrder;

		public int Count => Slots?.Count ?? 0;

		public ObstacleRaceBlock(IReadOnlyList<ObstacleRaceZoneDifficulty> slots, bool keepOrder = false)
		{
			Slots = slots;
			KeepOrder = keepOrder;
		}
	}
}
