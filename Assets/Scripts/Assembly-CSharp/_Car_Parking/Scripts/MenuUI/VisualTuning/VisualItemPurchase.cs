using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public static class VisualItemPurchase
	{
		public static void Purchase(CurrencyManagerController wallet, SpendCoinsType category, int itemId, int price, bool alreadyOwned, Action onGranted, Action onFailed = null)
		{
			if (wallet == null)
			{
				Debug.LogError($"[VisualItemPurchase] No wallet for {category} item {itemId} — purchase refused.");
				onFailed?.Invoke();
			}
			else if (alreadyOwned)
			{
				onGranted?.Invoke();
			}
			else if (price <= 0)
			{
				Debug.LogError($"[VisualItemPurchase] {category} item {itemId} is not owned but has price {price} — purchase refused.");
				onFailed?.Invoke();
			}
			else
			{
				wallet.TryCoinsSpend(category, new AdvancedInt(price), onGranted, onFailed, allowZero: false).Forget();
			}
		}

		public static async UniTask<bool> PurchaseAsync(CurrencyManagerController wallet, SpendCoinsType category, int itemId, int price, bool alreadyOwned, Action onGranted, Action onFailed = null)
		{
			if (wallet == null)
			{
				Debug.LogError($"[VisualItemPurchase] No wallet for {category} item {itemId} — purchase refused.");
				onFailed?.Invoke();
				return false;
			}
			if (alreadyOwned)
			{
				onGranted?.Invoke();
				return true;
			}
			if (price <= 0)
			{
				Debug.LogError($"[VisualItemPurchase] {category} item {itemId} is not owned but has price {price} — purchase refused.");
				onFailed?.Invoke();
				return false;
			}
			(await wallet.TryCoinsSpend(category, new AdvancedInt(price), onGranted, onFailed, allowZero: false)).Get(out var value);
			return value;
		}
	}
}
