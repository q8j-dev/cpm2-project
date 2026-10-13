using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamPurchaseRestoreService
	{
		bool HasUnrestoredPurchaseNoSlot { get; }

		UniTask RestoreIfNeeded();

		void ClearUnrestoredPurchaseFlag();
	}
}
