using CPM.Connection;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseCheckerHashCheck : IPurchaseCheckProductToBuy
	{
		private readonly IConnectionManager _connectionManager;

		public PurchaseCheckerHashCheck(IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		public async UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			_connectionManager.CorrectHash.Get(out var value);
			if (!value && (bool)V.CoreGameManager)
			{
				V.CoreGameManager.SetMessage("error: 0x4", 3f, NotificationType.Error);
			}
			return value ? PreBuyActionResult.Success : PreBuyActionResult.HashError;
		}
	}
}
