using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Analytics.Generic
{
	public static class CurrencyConverter
	{
		private static readonly Dictionary<CurrencyItemType, CurrencyType> _currencies = new Dictionary<CurrencyItemType, CurrencyType>
		{
			{
				CurrencyItemType.coins,
				CurrencyType.HARD
			},
			{
				CurrencyItemType.money,
				CurrencyType.SOFT
			}
		};

		public static CurrencyType Convert(CurrencyItemType from)
		{
			return _currencies[from];
		}
	}
}
