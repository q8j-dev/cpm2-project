using System.Collections.Generic;

namespace CPM.Balance
{
	public class ShopItem
	{
		public string Id;

		public List<PriceBalance> Prices = new List<PriceBalance>();

		public List<ItemInBox> Contents = new List<ItemInBox>();
	}
}
