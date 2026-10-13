using System.Collections.Generic;

namespace CPM.TuningTimer.Integrity
{
	public readonly struct TuningTimerFacts
	{
		public readonly int OwnerId;

		public readonly double TimeOfStart;

		public readonly double DurationSeconds;

		public readonly IReadOnlyList<int> PayloadIds;

		public TuningTimerFacts(int ownerId, double timeOfStart, double durationSeconds, IReadOnlyList<int> payloadIds)
		{
			OwnerId = ownerId;
			TimeOfStart = timeOfStart;
			DurationSeconds = durationSeconds;
			PayloadIds = payloadIds;
		}
	}
}
