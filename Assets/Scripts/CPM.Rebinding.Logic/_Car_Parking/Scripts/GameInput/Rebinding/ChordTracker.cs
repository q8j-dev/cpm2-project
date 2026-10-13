using System;
using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class ChordTracker
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		private readonly List<string> _held = new List<string>();

		private readonly List<string> _candidate = new List<string>();

		private readonly List<string> _ignored = new List<string>();

		public IReadOnlyList<string> Candidate => _candidate;

		public bool IsComplete { get; private set; }

		public int Version { get; private set; }

		public bool IsHolding => _held.Count > 0;

		public void Reset(IEnumerable<string> alreadyHeld = null)
		{
			_held.Clear();
			_candidate.Clear();
			_ignored.Clear();
			if (alreadyHeld != null)
			{
				foreach (string item in alreadyHeld)
				{
					if (!string.IsNullOrEmpty(item) && IndexOf(_ignored, item) < 0)
					{
						_ignored.Add(item);
					}
				}
			}
			IsComplete = false;
			Version++;
		}

		public bool Press(string path)
		{
			if (IsComplete || string.IsNullOrEmpty(path) || Overlaps(_ignored, path) || Overlaps(_held, path))
			{
				return false;
			}
			_held.Add(path);
			_candidate.Clear();
			_candidate.AddRange(_held);
			Version++;
			return true;
		}

		public void Release(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			int num = IndexOf(_ignored, path);
			if (num >= 0)
			{
				_ignored.RemoveAt(num);
				return;
			}
			int num2 = IndexOf(_held, path);
			if (num2 >= 0)
			{
				_held.RemoveAt(num2);
				if (_held.Count == 0 && _candidate.Count > 0)
				{
					IsComplete = true;
				}
			}
		}

		public bool IsTracking(string path)
		{
			return IndexOf(_held, path) >= 0;
		}

		private static bool Overlaps(List<string> list, string path)
		{
			return list.Any((string other) => string.Equals(other, path, StringComparison.OrdinalIgnoreCase) || BindingPaths.Overlap(other, path));
		}

		private static int IndexOf(List<string> list, string path)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (string.Equals(list[i], path, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
