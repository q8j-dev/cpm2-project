using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Serializable]
	public class CombinedPrice
	{
		public RealСurrencyPrice RealСurrencyPrice;

		[SerializeField]
		private ObscuredInt _coin;

		[SerializeField]
		private ObscuredInt _money;

		public ObscuredInt Coin
		{
			get
			{
				if (!IsRealСurrency)
				{
					return _coin;
				}
				return 0;
			}
			set
			{
				_coin = value;
			}
		}

		public ObscuredInt Money
		{
			get
			{
				if (!IsRealСurrency)
				{
					return _money;
				}
				return 0;
			}
			set
			{
				_money = value;
			}
		}

		public bool IsHaveCoin => (int)Coin > 0;

		public bool IsHaveMoney => (int)Money > 0;

		public bool IsHaveMoneyAndCoin
		{
			get
			{
				if (IsHaveCoin)
				{
					return IsHaveMoney;
				}
				return false;
			}
		}

		public bool IsZero
		{
			get
			{
				if ((int)Money == 0)
				{
					return (int)Coin == 0;
				}
				return false;
			}
		}

		public bool IsRealСurrency
		{
			get
			{
				if (RealСurrencyPrice != null)
				{
					return !string.IsNullOrEmpty(RealСurrencyPrice.ProductId);
				}
				return false;
			}
		}

		public CombinedPrice()
		{
		}

		public CombinedPrice(RealСurrencyPrice realСurrencyPrice)
		{
			RealСurrencyPrice = realСurrencyPrice;
		}

		public CombinedPrice(ObscuredInt coin, ObscuredInt money)
		{
			Coin = coin;
			Money = money;
		}

		public static CombinedPrice operator +(CombinedPrice a, CombinedPrice b)
		{
			CombinedPrice combinedPrice = new CombinedPrice();
			combinedPrice.Coin = (int)a.Coin + (int)b.Coin;
			combinedPrice.Money = (int)a.Money + (int)b.Money;
			if (a.RealСurrencyPrice != null)
			{
				combinedPrice.RealСurrencyPrice = a.RealСurrencyPrice;
			}
			if (b.RealСurrencyPrice != null)
			{
				combinedPrice.RealСurrencyPrice = b.RealСurrencyPrice;
			}
			return combinedPrice;
		}

		public static bool operator ==(CombinedPrice a, CombinedPrice b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			if ((int)a.Money == (int)b.Money)
			{
				return (int)a.Coin == (int)b.Coin;
			}
			return false;
		}

		public static bool operator !=(CombinedPrice a, CombinedPrice b)
		{
			return !(a == b);
		}

		public void AddCoin(int amount)
		{
			Coin = (int)Coin + amount;
		}

		public void AddMoney(int amount)
		{
			Money = (int)Money + amount;
		}

		public void Add(CostType priceType, int amount)
		{
			switch (priceType)
			{
			case CostType.Coins:
				AddCoin(amount);
				break;
			case CostType.Money:
				AddMoney(amount);
				break;
			case CostType.RealWorldCash:
				break;
			}
		}

		public override string ToString()
		{
			return $"({Coin}, {Money})";
		}
	}
}
