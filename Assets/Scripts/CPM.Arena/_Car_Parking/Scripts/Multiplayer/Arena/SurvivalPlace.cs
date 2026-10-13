using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalPlace
	{
		public const int Undecided = -1;

		public static int For(SurvivalEliminationBoard board, IReadOnlyList<int> participants, int actor)
		{
			if (board == null || participants == null || participants.Count == 0)
			{
				return -1;
			}
			IReadOnlyList<int> eliminationOrder = board.EliminationOrder;
			for (int i = 0; i < eliminationOrder.Count; i++)
			{
				if (eliminationOrder[i] == actor)
				{
					return i + 1;
				}
			}
			return -1;
		}
	}
}
