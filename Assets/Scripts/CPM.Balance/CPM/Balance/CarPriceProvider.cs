using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;

namespace CPM.Balance
{
	public static class CarPriceProvider
	{
		private const int DefaultPrice = 10000;

		private static Dictionary<int, ShopItem> _carIndex = new Dictionary<int, ShopItem>();

		private static bool _initialized;

		private static bool _warnedNotInitialized;

		public static int Count => _carIndex.Count;

		public static void Init(BalanceDocument document)
		{
			_carIndex = BuildIndex(document);
			_initialized = true;
		}

		public static bool TryInit(BalanceDocument document)
		{
			Dictionary<int, ShopItem> dictionary = BuildIndex(document);
			if (dictionary.Count == 0)
			{
				return false;
			}
			_carIndex = dictionary;
			_initialized = true;
			return true;
		}

		private static Dictionary<int, ShopItem> BuildIndex(BalanceDocument document)
		{
			Dictionary<int, ShopItem> dictionary = new Dictionary<int, ShopItem>();
			List<ShopItem> list = document?.Shop?.Items;
			if (list != null)
			{
				foreach (ShopItem item in list)
				{
					if (item?.Contents == null)
					{
						continue;
					}
					foreach (ItemInBox content in item.Contents)
					{
						if (content != null && content.Type == ItemType.Car && content.Id.HasValue)
						{
							int value = content.Id.Value;
							if (dictionary.ContainsKey(value))
							{
								Debug.LogError($"CarPriceProvider.Init: duplicate carId {value} in balance document. First-wins.");
							}
							else
							{
								dictionary[value] = item;
							}
						}
					}
				}
			}
			return dictionary;
		}

		private static void WarnIfNotInitialized(string caller)
		{
			if (!_initialized && !_warnedNotInitialized)
			{
				_warnedNotInitialized = true;
				Debug.LogWarning("CarPriceProvider." + caller + " called before Init(); returning parity defaults. Ensure PersistentConfigScope.Configure runs before any price consumer.");
			}
		}

		public static void Get(int carId, out AdvancedInt price, out AdvancedBoolean isCoinPrice)
		{
			WarnIfNotInitialized("Get");
			if (_carIndex.TryGetValue(carId, out var value))
			{
				PriceBalance priceBalance = PriceResolver.ResolvePayment(value.Prices);
				if (priceBalance != null && priceBalance.Type != CostType.RealWorldCash)
				{
					price = new AdvancedInt(priceBalance.Amount);
					isCoinPrice = new AdvancedBoolean(priceBalance.Type == CostType.Coins);
					return;
				}
			}
			price = new AdvancedInt(10000);
			isCoinPrice = new AdvancedBoolean(v: true);
		}

		public static bool IsRealCurrencyCar(int carId)
		{
			WarnIfNotInitialized("IsRealCurrencyCar");
			if (!_carIndex.TryGetValue(carId, out var value))
			{
				return false;
			}
			PriceBalance priceBalance = PriceResolver.ResolvePayment(value.Prices);
			if (priceBalance != null)
			{
				return priceBalance.Type == CostType.RealWorldCash;
			}
			return false;
		}
	}
}
