using System;

namespace CPM.Checkpoints
{
	public sealed class CheckpointTracker
	{
		private readonly int _total;

		public int Total => _total;

		public int Last { get; private set; } = -1;

		public int Missed { get; private set; }

		public bool IsSealed { get; private set; }

		public int Passed => Last + 1 - Missed;

		public event Action<int> Skipped;

		public event Action Advanced;

		public CheckpointTracker(int total)
		{
			_total = ((total > 0) ? total : 0);
		}

		public void Touch(int index)
		{
			if (!IsSealed && index >= 0 && index < _total && index > Last)
			{
				int num = index - Last - 1;
				Last = index;
				if (num > 0)
				{
					Missed += num;
					Skipped?.Invoke(num);
				}
				Advanced?.Invoke();
			}
		}

		public int Seal()
		{
			if (IsSealed)
			{
				return Missed;
			}
			IsSealed = true;
			int num = _total - 1 - Last;
			if (num <= 0)
			{
				return Missed;
			}
			Missed += num;
			Skipped?.Invoke(num);
			return Missed;
		}
	}
}
