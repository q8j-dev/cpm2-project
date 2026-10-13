using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ObstacleRaceSlotContext
	{
		public readonly int SlotIndex;

		public readonly int SlotCount;

		public readonly ObstacleRaceZoneDifficulty RequiredDifficulty;

		public readonly IReadOnlyList<ObstacleRaceZoneInfo> Placed;

		public ObstacleRaceSlotContext(int slotIndex, int slotCount, ObstacleRaceZoneDifficulty requiredDifficulty, IReadOnlyList<ObstacleRaceZoneInfo> placed)
		{
			SlotIndex = slotIndex;
			SlotCount = slotCount;
			RequiredDifficulty = requiredDifficulty;
			Placed = placed;
		}
	}
}
