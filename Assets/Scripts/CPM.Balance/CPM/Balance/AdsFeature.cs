using System.Collections.Generic;

namespace CPM.Balance
{
	public class AdsFeature
	{
		public bool Enabled = true;

		public int GlobalDailyCap;

		public int GlobalSessionCap;

		public int SuccessCooldownSec;

		public int TechnicalCooldownSec;

		public int NoFillLockSec;

		public List<AdPlacementBalance> Placements = new List<AdPlacementBalance>();
	}
}
