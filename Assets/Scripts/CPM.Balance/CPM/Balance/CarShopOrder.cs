using System;

namespace CPM.Balance
{
	public readonly struct CarShopOrder : IComparable<CarShopOrder>
	{
		private readonly int _group;

		private readonly double _amount;

		private readonly int _carId;

		private CarShopOrder(int group, double amount, int carId)
		{
			_group = group;
			_amount = amount;
			_carId = carId;
		}

		public static CarShopOrder ForSoftPrice(int carId, bool isCoinPrice, int amount)
		{
			return new CarShopOrder(isCoinPrice ? 1 : 0, amount, carId);
		}

		public static CarShopOrder ForRealPrice(int carId, float? localizedPrice)
		{
			if (!localizedPrice.HasValue)
			{
				return new CarShopOrder(1, 0.0, carId);
			}
			return new CarShopOrder(0, localizedPrice.Value, carId);
		}

		public int CompareTo(CarShopOrder other)
		{
			int num = _group;
			int num2 = num.CompareTo(other._group);
			if (num2 != 0)
			{
				return num2;
			}
			double amount = _amount;
			int num3 = amount.CompareTo(other._amount);
			if (num3 != 0)
			{
				return num3;
			}
			num = _carId;
			return num.CompareTo(other._carId);
		}
	}
}
