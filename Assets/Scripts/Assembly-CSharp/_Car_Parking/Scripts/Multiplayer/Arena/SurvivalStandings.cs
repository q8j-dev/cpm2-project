using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.Multiplayer.Arena.UI;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalStandings
	{
		public static IReadOnlyList<ObstacleRaceResultRow> Build(SurvivalEliminationBoard board, IReadOnlyList<int> participants, int localActor, LobbyRoster roster, Func<int, double?> claimOf)
		{
			IReadOnlyList<int> eliminationOrder = board.EliminationOrder;
			double?[] array = SurvivalEliminationTimes.Build(eliminationOrder, claimOf);
			List<ObstacleRaceResultRow> list = new List<ObstacleRaceResultRow>();
			for (int i = 0; i < eliminationOrder.Count; i++)
			{
				int num = eliminationOrder[i];
				ObstacleRacePlayerStatus status = ((i != 0) ? ObstacleRacePlayerStatus.Eliminated : ObstacleRacePlayerStatus.Won);
				list.Add(new ObstacleRaceResultRow(roster.GetDisplayName(num), num == localActor, status, array[i]));
			}
			for (int j = 0; j < participants.Count; j++)
			{
				int num2 = participants[j];
				if (!board.Contains(num2))
				{
					list.Add(new ObstacleRaceResultRow(roster.GetDisplayName(num2), num2 == localActor, ObstacleRacePlayerStatus.Playing, null));
				}
			}
			return list;
		}
	}
}
