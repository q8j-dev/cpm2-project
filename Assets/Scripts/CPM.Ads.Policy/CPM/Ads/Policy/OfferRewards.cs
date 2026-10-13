using System.Collections.Generic;
using CPM.Balance;

namespace CPM.Ads.Policy
{
	public static class OfferRewards
	{
		public static bool TryTimeSeconds(IReadOnlyList<ItemInBox> rewards, out int seconds)
		{
			seconds = 0;
			if (rewards == null)
			{
				return false;
			}
			for (int i = 0; i < rewards.Count; i++)
			{
				ItemInBox itemInBox = rewards[i];
				if (itemInBox != null && itemInBox.Type == ItemType.Time && itemInBox.Amount > 0)
				{
					seconds = itemInBox.Amount;
					return true;
				}
			}
			return false;
		}
	}
}
