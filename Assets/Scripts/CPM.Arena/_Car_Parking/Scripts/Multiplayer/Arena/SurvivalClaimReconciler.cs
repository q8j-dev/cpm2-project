using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalClaimReconciler
	{
		public static IReadOnlyList<int> Pending(IReadOnlyList<int> participants, Func<int, object> readClaim, SurvivalEliminationBoard board)
		{
			if (participants == null || readClaim == null)
			{
				return Array.Empty<int>();
			}
			List<SurvivalClaim> list = null;
			for (int i = 0; i < participants.Count; i++)
			{
				int num = participants[i];
				if ((board == null || !board.Contains(num)) && SurvivalClaim.TryParse(readClaim(num), out var elapsed))
				{
					if (list == null)
					{
						list = new List<SurvivalClaim>();
					}
					list.Add(new SurvivalClaim(num, elapsed));
				}
			}
			if (list == null)
			{
				return Array.Empty<int>();
			}
			list.Sort(CompareFallThenActor);
			int[] array = new int[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				array[j] = list[j].Actor;
			}
			return array;
		}

		private static int CompareFallThenActor(SurvivalClaim a, SurvivalClaim b)
		{
			double elapsed = a.Elapsed;
			int num = elapsed.CompareTo(b.Elapsed);
			if (num == 0)
			{
				int actor = a.Actor;
				return actor.CompareTo(b.Actor);
			}
			return num;
		}
	}
}
