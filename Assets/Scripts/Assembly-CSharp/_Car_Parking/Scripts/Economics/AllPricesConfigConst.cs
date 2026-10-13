using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Economics
{
	[Preserve]
	public class AllPricesConfigConst : IPricesService, IEarnMoneyService
	{
		public ObscuredInt OilPrice => 995;

		public ObscuredInt WheelPressurePrice => 1110;

		public ObscuredInt ChangeTiresFreePrice => 3000;

		public ObscuredInt ChangeTiresGaragePrice => 1050;

		public ObscuredInt CarWashGaragePrice => 1100;

		public ObscuredInt CarWashFreePrice => 1100;

		public ObscuredInt RepairPrice => 1100;

		public ObscuredInt Fuel95Price => 50;

		public ObscuredInt Fuel98Price => 60;

		public ObscuredInt Fuel100Price => 80;

		public ObscuredInt FuelE85Price => 100;

		public ObscuredInt FuelDieselPrice => 90;

		public ObscuredInt MultiplayerRaceLose => 2500;

		public ObscuredInt MultiplayerPursuitLose => 5000;

		public ObscuredInt WeightTallDefault => 50;

		public ObscuredInt WeightTallWeightCoef => 10;

		public ObscuredInt Tall1 => 55;

		public ObscuredInt Tall2 => 75;

		public ObscuredInt Tall2SmallCar => 25;

		public ObscuredInt Tall2LargeCar => 55;

		public ObscuredInt VinylsMin => 100;

		public ObscuredInt VinylsMax => 6000;

		public ObscuredInt VinylsStep => 100;

		public ObscuredInt VinylsText => 1000;

		public ObscuredInt RimsMin => 1000;

		public ObscuredInt RimsMax => 20000;

		public ObscuredInt RimsStep => 300;

		public ObscuredInt BodyKitsMoneyPrice => 5000;

		public ObscuredInt MultiplayerRaceWin => 5000;

		public ObscuredInt MultiplayerPursuitWin => 10000;

		public ObscuredInt SpeedBannerReward => 50;

		public ObscuredInt GateTrafficCarReward => 5;

		public ObscuredInt GateMultiplayerCarReward => 100;

		public ObscuredInt CaseReward => 1000;

		public ObscuredInt TaxiRewardMin => 200;

		public ObscuredInt TaxiRewardMax => 250;

		public ObscuredFloat TaxiRewardExtraSafetyCoef => 0.5f;

		public ObscuredFloat TaxiRewardExtraTimeCoef => 1f;

		public ObscuredFloat BusinessProfitIntervalInSec => 0.15f;

		public ObscuredInt BusinessProfitMaxPerSession => 50000;
	}
}
