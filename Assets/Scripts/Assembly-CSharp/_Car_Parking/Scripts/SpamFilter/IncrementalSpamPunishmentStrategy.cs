using System;

namespace _Car_Parking.Scripts.SpamFilter
{
	public class IncrementalSpamPunishmentStrategy : ISpamPunishmentStrategy
	{
		private int _count;

		private readonly TimeSpan _initial;

		private const double InitialSeconds = 60.0;

		private const double IncrementSeconds = 60.0;

		public IncrementalSpamPunishmentStrategy()
		{
			_initial = TimeSpan.FromSeconds(60.0);
		}

		public TimeSpan GetNextPunishment()
		{
			return _initial + TimeSpan.FromSeconds(60.0 * (double)_count++);
		}
	}
}
