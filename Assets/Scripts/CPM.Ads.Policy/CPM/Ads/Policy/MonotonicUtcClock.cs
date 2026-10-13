using System;

namespace CPM.Ads.Policy
{
	public sealed class MonotonicUtcClock : IUtcClock
	{
		private static readonly TimeSpan MaxAnchorLead = TimeSpan.FromHours(48.0);

		private readonly IRawUtcTime _raw;

		private readonly IAdPolicyStore _store;

		public MonotonicUtcClock(IRawUtcTime raw, IAdPolicyStore store)
		{
			_raw = raw;
			_store = store;
		}

		public DateTime UtcNow()
		{
			DateTime dateTime = _raw.UtcNow;
			if (TryGetLastSeen(_store.State.LastSeenUtcTicks, out var lastSeen) && dateTime < lastSeen && lastSeen - dateTime <= MaxAnchorLead)
			{
				dateTime = lastSeen;
			}
			dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			_store.State.LastSeenUtcTicks = dateTime.Ticks;
			return dateTime;
		}

		private static bool TryGetLastSeen(long ticks, out DateTime lastSeen)
		{
			DateTime minValue = DateTime.MinValue;
			if (ticks >= minValue.Ticks)
			{
				minValue = DateTime.MaxValue;
				if (ticks <= minValue.Ticks)
				{
					lastSeen = new DateTime(ticks, DateTimeKind.Utc);
					return true;
				}
			}
			lastSeen = default;
			return false;
		}
	}
}
