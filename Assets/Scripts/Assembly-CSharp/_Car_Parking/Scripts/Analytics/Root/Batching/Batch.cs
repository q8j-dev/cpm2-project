using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Analytics.Root.Batching
{
	[Serializable]
	public class Batch
	{
		public readonly string Uuid = Guid.NewGuid().ToString();

		public List<Event> Events = new List<Event>();
	}
}
