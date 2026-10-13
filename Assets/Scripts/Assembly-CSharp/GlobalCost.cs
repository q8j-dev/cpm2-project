using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public struct GlobalCost
{
	public ObscuredInt Price;

	public CostType PriceType;

	public GlobalCost(int price, CostType priceType = CostType.Money)
	{
		Price = price;
		PriceType = priceType;
	}
}
