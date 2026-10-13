using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceDifficultyRule : IObstacleRaceZoneRule
	{
		public string Name => "difficulty";

		public List<ObstacleRaceZoneInfo> Filter(List<ObstacleRaceZoneInfo> candidates, in ObstacleRaceSlotContext slot)
		{
			List<ObstacleRaceZoneInfo> list = new List<ObstacleRaceZoneInfo>(candidates.Count);
			foreach (ObstacleRaceZoneInfo candidate in candidates)
			{
				if (candidate.Difficulty == slot.RequiredDifficulty)
				{
					list.Add(candidate);
				}
			}
			return list;
		}

		List<ObstacleRaceZoneInfo> IObstacleRaceZoneRule.Filter(List<ObstacleRaceZoneInfo> candidates, in ObstacleRaceSlotContext slot)
		{
			return Filter(candidates, in slot);
		}
	}
}
