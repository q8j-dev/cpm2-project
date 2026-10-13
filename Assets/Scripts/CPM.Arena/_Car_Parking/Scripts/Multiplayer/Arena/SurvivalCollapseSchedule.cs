using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class SurvivalCollapseSchedule
	{
		private double _deadline;

		public bool IsArmed => _deadline > 0.0;

		public double Deadline => _deadline;

		public bool TryArm(double deadlineServerTime)
		{
			if (_deadline > 0.0)
			{
				return false;
			}
			if (deadlineServerTime <= 0.0)
			{
				return false;
			}
			_deadline = deadlineServerTime;
			return true;
		}

		public bool TryHasten(double deadlineServerTime)
		{
			if (_deadline <= 0.0)
			{
				return false;
			}
			if (deadlineServerTime <= 0.0)
			{
				return false;
			}
			if (deadlineServerTime >= _deadline)
			{
				return false;
			}
			_deadline = deadlineServerTime;
			return true;
		}

		public bool Adopt(double deadlineServerTime)
		{
			if (!TryArm(deadlineServerTime))
			{
				return TryHasten(deadlineServerTime);
			}
			return true;
		}

		public bool HasCollapsed(double serverTime)
		{
			if (_deadline > 0.0)
			{
				return serverTime >= _deadline;
			}
			return false;
		}

		public double Remaining(double serverTime)
		{
			if (!(_deadline > 0.0))
			{
				return 0.0;
			}
			return Math.Max(0.0, _deadline - serverTime);
		}

		public void Reset()
		{
			_deadline = 0.0;
		}
	}
}
