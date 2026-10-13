using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.Trade
{
	public class TradeItemMoney : TradeItem
	{
		public AdvancedInt Price { get; private set; }

		public override TradeItemType ItemType => TradeItemType.Money;

		public TradeItemMoney(AdvancedInt price)
		{
			Price = price;
		}
	}
}
