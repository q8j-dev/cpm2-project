using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public interface ICircuitRacingRewardService
	{
		UniTask<WalletOperationResult> GetCircuitRaceReward(ObscuredInt trackId, AdvancedInt classId, ObscuredInt place, string key);
	}
}
