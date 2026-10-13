using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Serializable]
	public class BusinessPriceModel
	{
		public GlobalCost Cost;

		public ObscuredInt GetPrice
		{
			get
			{
				ObscuredInt price = Cost.Price;
				price.RandomizeCryptoKey();
				return price;
			}
		}

		public CostType PriceType => Cost.PriceType;
	}
}
