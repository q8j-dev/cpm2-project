using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceMaxSameDifficultyRunRule : IObstacleRaceOrderRule
	{
		public const int MaxRun = 2;

		public string Name => "max-same-difficulty-run";

		public bool IsAllowed(IReadOnlyList<ObstacleRaceZoneDifficulty> slots)
		{
			int num = 0;
			for (int i = 0; i < slots.Count; i++)
			{
				num = ((i <= 0 || slots[i] != slots[i - 1]) ? 1 : (num + 1));
				if (num > 2)
				{
					return false;
				}
			}
			return true;
		}
	}
}
