using CPM.Account;
using CPM.Connection;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class DragRacingRewardService : IDragRacingRewardService
	{
		private IWalletService _walletService;

		private IConnectionManager _connectionManager;

		[Inject]
		public DragRacingRewardService(IWalletService walletService, IConnectionManager connectionManager, IAccountController accountController)
		{
			_connectionManager = connectionManager;
			_walletService = walletService;
		}

		public async UniTask<WalletOperationResult> GetCoinForDrag(ObscuredInt classId, ObscuredInt place, int gearboxType)
		{
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				return new WalletOperationResult(0, 0);
			}
			string[] array = new string[5];
			ObscuredInt obscuredInt = classId;
			array[0] = obscuredInt.ToString();
			array[1] = ",";
			obscuredInt = place;
			array[2] = obscuredInt.ToString();
			array[3] = ",";
			array[4] = gearboxType.ToString();
			string data = string.Concat(array);
			return await _walletService.GetCoinForDrag(data);
		}
	}
}
