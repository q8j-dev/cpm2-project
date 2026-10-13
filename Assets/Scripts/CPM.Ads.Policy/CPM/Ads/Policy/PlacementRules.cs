using System.Collections.Generic;
using CPM.Balance;

namespace CPM.Ads.Policy
{
	public readonly struct PlacementRules
	{
		public readonly bool Enabled;

		public readonly int DailyCap;

		public readonly int PerContextCap;

		public readonly IReadOnlyList<ItemInBox> Rewards;

		public PlacementRules(bool enabled, int dailyCap, int perContextCap, IReadOnlyList<ItemInBox> rewards)
		{
			Enabled = enabled;
			DailyCap = dailyCap;
			PerContextCap = perContextCap;
			Rewards = rewards;
		}
	}
}
