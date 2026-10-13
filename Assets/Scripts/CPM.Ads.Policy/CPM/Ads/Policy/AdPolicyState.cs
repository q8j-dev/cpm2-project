using System;
using System.Collections.Generic;

namespace CPM.Ads.Policy
{
	[Serializable]
	public class AdPolicyState
	{
		public long DayWindowStartTicks;

		public long LastSeenUtcTicks;

		public long LastSuccessTicks;

		public long LastNoRewardCloseTicks;

		public int GlobalDayCount;

		public List<PlacementEntry> Placements = new List<PlacementEntry>();
	}
}
