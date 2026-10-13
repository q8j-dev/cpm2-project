using CPM.Account;
using CPM.Connection;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class CircuitRacingRewardService : ICircuitRacingRewardService
	{
		private IWalletService _walletService;

		private IConnectionManager _connectionManager;

		[Inject]
		public CircuitRacingRewardService(IWalletService walletService, IConnectionManager connectionManager, IAccountController accountController)
		{
			_connectionManager = connectionManager;
			_walletService = walletService;
		}

		public async UniTask<WalletOperationResult> GetCircuitRaceReward(ObscuredInt trackId, AdvancedInt classId, ObscuredInt place, string key)
		{
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				return new WalletOperationResult(0, 0, "");
			}
			classId.Get(out int value2);
			string data = $"{trackId},{value2},{place}";
			return await _walletService.GetCircuitRaceReward(data, key);
		}
	}
}
