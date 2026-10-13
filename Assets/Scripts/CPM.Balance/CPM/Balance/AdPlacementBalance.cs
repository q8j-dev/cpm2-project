using System.Collections.Generic;

namespace CPM.Balance
{
	public class AdPlacementBalance
	{
		public string Id;

		public bool Enabled = true;

		public int DailyCap;

		public int PerContextCap;

		public List<ItemInBox> Rewards = new List<ItemInBox>();
	}
}
