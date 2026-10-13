using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class SurvivalEliminationBoard
	{
		private readonly List<int> _order = new List<int>();

		private readonly HashSet<int> _seen = new HashSet<int>();

		public int Count => _order.Count;

		public IReadOnlyList<int> EliminationOrder => _order;

		public bool Contains(int actor)
		{
			return _seen.Contains(actor);
		}

		public bool TryAdd(int actor)
		{
			if (!_seen.Add(actor))
			{
				return false;
			}
			_order.Add(actor);
			return true;
		}

		public object ToProperty()
		{
			return _order.ToArray();
		}

		public static SurvivalEliminationBoard FromProperty(object raw)
		{
			SurvivalEliminationBoard survivalEliminationBoard = new SurvivalEliminationBoard();
			if (raw is int[] array)
			{
				for (int i = 0; i < array.Length; i++)
				{
					survivalEliminationBoard.TryAdd(array[i]);
				}
			}
			return survivalEliminationBoard;
		}
	}
}
