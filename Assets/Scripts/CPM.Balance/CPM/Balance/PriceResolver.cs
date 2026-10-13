using System.Collections.Generic;

namespace CPM.Balance
{
	public static class PriceResolver
	{
		public static PriceBalance ResolvePayment(List<PriceBalance> prices)
		{
			if (prices == null || prices.Count == 0)
			{
				return null;
			}
			return prices[0];
		}
	}
}
