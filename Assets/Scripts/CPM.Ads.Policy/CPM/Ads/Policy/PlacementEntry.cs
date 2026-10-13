using System;
using System.Collections.Generic;

namespace CPM.Ads.Policy
{
	[Serializable]
	public class PlacementEntry
	{
		public AdPlacement Placement;

		public int DayCount;

		public long NoFillUntilTicks;

		public List<ContextEntry> Contexts = new List<ContextEntry>();
	}
}
