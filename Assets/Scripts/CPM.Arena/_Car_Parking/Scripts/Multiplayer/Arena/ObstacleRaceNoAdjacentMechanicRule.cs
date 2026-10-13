using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceNoAdjacentMechanicRule : IObstacleRaceZoneRule
	{
		public string Name => "no-adjacent-mechanic";

		public List<ObstacleRaceZoneInfo> Filter(List<ObstacleRaceZoneInfo> candidates, in ObstacleRaceSlotContext slot)
		{
			if (slot.Placed.Count == 0)
			{
				return candidates;
			}
			IReadOnlyList<ObstacleRaceMechanicType> mechanics = slot.Placed[slot.Placed.Count - 1].Mechanics;
			if (mechanics == null || mechanics.Count == 0)
			{
				return candidates;
			}
			List<ObstacleRaceZoneInfo> list = new List<ObstacleRaceZoneInfo>(candidates.Count);
			foreach (ObstacleRaceZoneInfo candidate in candidates)
			{
				if (!SharesAny(candidate.Mechanics, mechanics))
				{
					list.Add(candidate);
				}
			}
			return list;
		}

		private static bool SharesAny(IReadOnlyList<ObstacleRaceMechanicType> mechanics, IReadOnlyList<ObstacleRaceMechanicType> previous)
		{
			if (mechanics == null)
			{
				return false;
			}
			for (int i = 0; i < mechanics.Count; i++)
			{
				for (int j = 0; j < previous.Count; j++)
				{
					if (mechanics[i] == previous[j])
					{
						return true;
					}
				}
			}
			return false;
		}

		List<ObstacleRaceZoneInfo> IObstacleRaceZoneRule.Filter(List<ObstacleRaceZoneInfo> candidates, in ObstacleRaceSlotContext slot)
		{
			return Filter(candidates, in slot);
		}
	}
}
