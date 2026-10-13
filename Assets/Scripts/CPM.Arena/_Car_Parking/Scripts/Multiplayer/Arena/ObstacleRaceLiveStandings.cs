using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceLiveStandings
	{
		public static void Build(List<ObstacleRaceStandingRow> result, SurvivalEliminationBoard board, IReadOnlyList<int> participants, IReadOnlyList<int> runnersClosestFirst, int localActor, Func<int, string> nameOf, Func<int, double?> claimOf, Func<int, float?> remainingOf)
		{
			result.Clear();
			if (board == null || participants == null)
			{
				return;
			}
			IReadOnlyList<int> eliminationOrder = board.EliminationOrder;
			double?[] array = SurvivalEliminationTimes.Build(eliminationOrder, claimOf);
			for (int i = 0; i < eliminationOrder.Count; i++)
			{
				Add(result, eliminationOrder[i], localActor, nameOf, isRunning: false, array[i], null);
			}
			if (runnersClosestFirst != null)
			{
				for (int j = 0; j < runnersClosestFirst.Count; j++)
				{
					int num = runnersClosestFirst[j];
					if (!board.Contains(num))
					{
						Add(result, num, localActor, nameOf, isRunning: true, null, remainingOf?.Invoke(num));
					}
				}
			}
			for (int k = 0; k < participants.Count; k++)
			{
				int actor = participants[k];
				if (!board.Contains(actor))
				{
					Add(result, actor, localActor, nameOf, isRunning: true, null, null);
				}
			}
		}

		public static int LocalPlace(IReadOnlyList<ObstacleRaceStandingRow> rows)
		{
			if (rows == null)
			{
				return 0;
			}
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].IsLocal)
				{
					return rows[i].Place;
				}
			}
			return 0;
		}

		private static void Add(List<ObstacleRaceStandingRow> result, int actor, int localActor, Func<int, string> nameOf, bool isRunning, double? finishTime, float? remaining)
		{
			if (!Contains(result, actor))
			{
				result.Add(new ObstacleRaceStandingRow(actor, result.Count + 1, nameOf?.Invoke(actor), actor == localActor, isRunning, finishTime, remaining));
			}
		}

		private static bool Contains(List<ObstacleRaceStandingRow> rows, int actor)
		{
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].Actor == actor)
				{
					return true;
				}
			}
			return false;
		}
	}
}
