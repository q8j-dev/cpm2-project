using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Economics.Export;

namespace _Car_Parking.Scripts.Economics
{
	[CreateAssetMenu(fileName = "EarnMoneyConfig", menuName = "Economics/EarnMoneyConfig")]
	public class EarnMoneyConfig : ExportableScriptableObject
	{
		public ObscuredInt MultiplayerRaceReward = 5000;

		public ObscuredInt MultiplayerPursuitReward = 10000;

		public ObscuredInt SpeedBannerReward = 50;

		public ObscuredInt GateCarReward = 50;

		public ObscuredInt GateMultiplayerCarReward = 100;

		public ObscuredInt TaxiRewardMin = 200;

		public ObscuredInt TaxiRewardMax = 250;

		public ObscuredFloat TaxiRewardExtraSafetyCoef = 0.5f;

		public ObscuredFloat TaxiRewardExtraTimeCoef = 1f;

		public ObscuredInt CaseReward = 1000;

		public ObscuredFloat BusinessProfitIntervalInSec = 0.15f;

		public ObscuredInt BusinessProfitMaxPerSession = 50000;
	}
}
