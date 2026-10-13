using System.Collections.Generic;

namespace CPM.RouteStandings
{
	public sealed class StandingsCalculator
	{
		private readonly RouteMetrics _metrics;

		private readonly Dictionary<int, int> _cursors = new Dictionary<int, int>();

		private readonly Dictionary<int, float> _remaining = new Dictionary<int, float>();

		private readonly List<RacerStanding> _standings = new List<RacerStanding>();

		private readonly List<int> _departed = new List<int>();

		public IReadOnlyList<RacerStanding> Standings => _standings;

		public StandingsCalculator(RouteMetrics metrics)
		{
			_metrics = metrics;
		}

		public void Update(IReadOnlyList<RacerProgress> racers)
		{
			_standings.Clear();
			_remaining.Clear();
			int num = ((_metrics != null && _metrics.IsValid) ? (racers?.Count ?? 0) : 0);
			for (int i = 0; i < num; i++)
			{
				RacerProgress racerProgress = racers[i];
				int cursor = (_cursors.TryGetValue(racerProgress.Id, out var value) ? value : (-1));
				float num2 = _metrics.RemainingFrom(racerProgress.Position, ref cursor);
				_cursors[racerProgress.Id] = cursor;
				_remaining[racerProgress.Id] = num2;
				_standings.Add(new RacerStanding(racerProgress.Id, 0, num2));
			}
			DropDepartedCursors();
			_standings.Sort(CompareStandings);
			for (int j = 0; j < _standings.Count; j++)
			{
				_standings[j] = new RacerStanding(_standings[j].Id, j, _standings[j].Remaining);
			}
		}

		public bool TryGetRemaining(int id, out float meters)
		{
			return _remaining.TryGetValue(id, out meters);
		}

		public bool TryGetGap(int fromId, int toId, out float meters)
		{
			meters = 0f;
			if (!_remaining.TryGetValue(fromId, out var value))
			{
				return false;
			}
			if (!_remaining.TryGetValue(toId, out var value2))
			{
				return false;
			}
			meters = value - value2;
			return true;
		}

		private static int CompareStandings(RacerStanding a, RacerStanding b)
		{
			float remaining = a.Remaining;
			int num = remaining.CompareTo(b.Remaining);
			if (num == 0)
			{
				int id = a.Id;
				return id.CompareTo(b.Id);
			}
			return num;
		}

		private void DropDepartedCursors()
		{
			_departed.Clear();
			foreach (KeyValuePair<int, int> cursor in _cursors)
			{
				if (!_remaining.ContainsKey(cursor.Key))
				{
					_departed.Add(cursor.Key);
				}
			}
			for (int i = 0; i < _departed.Count; i++)
			{
				_cursors.Remove(_departed[i]);
			}
		}
	}
}
