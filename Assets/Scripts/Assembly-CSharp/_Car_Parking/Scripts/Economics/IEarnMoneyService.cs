using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Economics
{
	public interface IEarnMoneyService
	{
		ObscuredInt MultiplayerRaceWin { get; }

		ObscuredInt MultiplayerPursuitWin { get; }

		ObscuredInt SpeedBannerReward { get; }

		ObscuredInt GateTrafficCarReward { get; }

		ObscuredInt GateMultiplayerCarReward { get; }

		ObscuredInt TaxiRewardMin { get; }

		ObscuredInt TaxiRewardMax { get; }

		ObscuredFloat TaxiRewardExtraSafetyCoef { get; }

		ObscuredFloat TaxiRewardExtraTimeCoef { get; }

		ObscuredInt CaseReward { get; }

		ObscuredFloat BusinessProfitIntervalInSec { get; }

		ObscuredInt BusinessProfitMaxPerSession { get; }
	}
}
