using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceNoHardOpeningRule : IObstacleRaceOrderRule
	{
		public const int OpeningSlots = 2;

		public string Name => "no-hard-opening";

		public bool IsAllowed(IReadOnlyList<ObstacleRaceZoneDifficulty> slots)
		{
			int num = Math.Min(2, slots.Count);
			for (int i = 0; i < num; i++)
			{
				if (slots[i] == ObstacleRaceZoneDifficulty.Hard)
				{
					return false;
				}
			}
			return true;
		}
	}
}
