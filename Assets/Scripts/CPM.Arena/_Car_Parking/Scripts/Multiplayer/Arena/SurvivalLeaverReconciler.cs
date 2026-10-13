using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalLeaverReconciler
	{
		public static IEnumerable<int> Missing(IReadOnlyList<int> participants, ICollection<int> present, SurvivalEliminationBoard board)
		{
			if (participants == null)
			{
				yield break;
			}
			for (int i = 0; i < participants.Count; i++)
			{
				int num = participants[i];
				if ((present == null || !present.Contains(num)) && (board == null || !board.Contains(num)))
				{
					yield return num;
				}
			}
		}
	}
}
