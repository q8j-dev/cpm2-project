using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Wallet
{
	public interface IWalletService
	{
		UniTask<WalletOperationResult> TrySpendCoins(SpendCoinsType spendCoinsType, ObscuredInt price);

		UniTask<WalletOperationResult> SyncCoinsFromDatabase();

		UniTask<WalletOperationResult> PurchaseItem(ItemData data);

		UniTask<WalletOperationResult> RestorePurchase(ItemData data);

		UniTask<WalletOperationResult> SyncMoneyFromDatabase();

		UniTask<WalletOperationResult> GetCoinForDrag(string data);

		UniTask<WalletOperationResult> GetCircuitRaceReward(string data, string key);

		UniTask<WalletOperationResult> PurchaseItemSteam(ItemData data);
	}
}
