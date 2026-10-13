using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Analytics.Root.Batching
{
	public interface IBatchSource
	{
		event Action<List<Event>> EventsReadyForBatch;
	}
}
