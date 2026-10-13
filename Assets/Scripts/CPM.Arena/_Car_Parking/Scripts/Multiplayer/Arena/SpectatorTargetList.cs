using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class SpectatorTargetList
	{
		public const int NoTarget = 0;

		private readonly List<int> _running = new List<int>();

		private readonly List<int> _reordered = new List<int>();

		private int _current;

		private int _lastIndex;

		public int Current => _current;

		public int Count => _running.Count;

		public bool HasTarget => _current != 0;

		public int PositionOfCurrent
		{
			get
			{
				if (_current != 0)
				{
					return _running.IndexOf(_current) + 1;
				}
				return 0;
			}
		}

		public bool Contains(int actor)
		{
			return _running.Contains(actor);
		}

		public bool TrySelect(int actor)
		{
			int num = _running.IndexOf(actor);
			if (num < 0)
			{
				return false;
			}
			_current = actor;
			_lastIndex = num;
			return true;
		}

		public void Refresh(IReadOnlyList<int> participants, SurvivalEliminationBoard board)
		{
			_running.Clear();
			if (participants != null)
			{
				for (int i = 0; i < participants.Count; i++)
				{
					int num = participants[i];
					if (board == null || !board.Contains(num))
					{
						_running.Add(num);
					}
				}
			}
			if (_running.Count == 0)
			{
				_current = 0;
				_lastIndex = 0;
				return;
			}
			int num2 = _running.IndexOf(_current);
			if (num2 >= 0)
			{
				_lastIndex = num2;
				return;
			}
			if (_lastIndex >= _running.Count)
			{
				_lastIndex = 0;
			}
			_current = _running[_lastIndex];
		}

		public void Reorder(IReadOnlyList<int> order)
		{
			if (order == null || _running.Count < 2)
			{
				return;
			}
			_reordered.Clear();
			for (int i = 0; i < order.Count; i++)
			{
				int item = order[i];
				if (_running.Contains(item) && !_reordered.Contains(item))
				{
					_reordered.Add(item);
				}
			}
			for (int j = 0; j < _running.Count; j++)
			{
				int item2 = _running[j];
				if (!_reordered.Contains(item2))
				{
					_reordered.Add(item2);
				}
			}
			_running.Clear();
			_running.AddRange(_reordered);
			int num = _running.IndexOf(_current);
			if (num >= 0)
			{
				_lastIndex = num;
			}
		}

		public bool Next()
		{
			return Step(1);
		}

		public bool Prev()
		{
			return Step(-1);
		}

		private bool Step(int delta)
		{
			if (_running.Count == 0)
			{
				return false;
			}
			int num = _running.IndexOf(_current);
			if (num < 0)
			{
				num = 0;
			}
			num = (num + delta + _running.Count) % _running.Count;
			_current = _running[num];
			_lastIndex = num;
			return true;
		}
	}
}
