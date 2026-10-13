using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public interface IDragRacingRewardService
	{
		UniTask<WalletOperationResult> GetCoinForDrag(ObscuredInt classId, ObscuredInt place, int gearboxType);
	}
}
