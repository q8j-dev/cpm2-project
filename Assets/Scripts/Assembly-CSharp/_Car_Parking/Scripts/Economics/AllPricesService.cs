using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Economics
{
	public class AllPricesService : IPricesService, IEarnMoneyService
	{
		private PricesConfig _pricesConfig;

		private EarnMoneyConfig _earnMoneyConfig;

		public ObscuredInt OilPrice => _pricesConfig.Oil;

		public ObscuredInt WheelPressurePrice => _pricesConfig.WheelPressure;

		public ObscuredInt ChangeTiresFreePrice => _pricesConfig.ChangeTiresFree;

		public ObscuredInt ChangeTiresGaragePrice => _pricesConfig.ChangeTiresGarage;

		public ObscuredInt CarWashGaragePrice => _pricesConfig.CarWash;

		public ObscuredInt CarWashFreePrice => _pricesConfig.CarWash;

		public ObscuredInt RepairPrice => _pricesConfig.Repair;

		public ObscuredInt Fuel95Price => _pricesConfig.Fuel95;

		public ObscuredInt Fuel98Price => _pricesConfig.Fuel98;

		public ObscuredInt Fuel100Price => _pricesConfig.Fuel100;

		public ObscuredInt FuelE85Price => _pricesConfig.FuelE85;

		public ObscuredInt FuelDieselPrice => _pricesConfig.FuelDiesel;

		public ObscuredInt MultiplayerRaceLose => _pricesConfig.MultiplayerRaceLose;

		public ObscuredInt MultiplayerPursuitLose => _pricesConfig.MultiplayerPursuitLose;

		public ObscuredInt MultiplayerRaceWin => _earnMoneyConfig.MultiplayerRaceReward;

		public ObscuredInt MultiplayerPursuitWin => _earnMoneyConfig.MultiplayerPursuitReward;

		public ObscuredInt SpeedBannerReward => _earnMoneyConfig.SpeedBannerReward;

		public ObscuredInt GateTrafficCarReward => _earnMoneyConfig.GateCarReward;

		public ObscuredInt GateMultiplayerCarReward => _earnMoneyConfig.GateMultiplayerCarReward;

		public ObscuredInt CaseReward => _earnMoneyConfig.CaseReward;

		public ObscuredInt TaxiRewardMin => _earnMoneyConfig.TaxiRewardMin;

		public ObscuredInt TaxiRewardMax => _earnMoneyConfig.TaxiRewardMax;

		public ObscuredFloat TaxiRewardExtraSafetyCoef => _earnMoneyConfig.TaxiRewardExtraSafetyCoef;

		public ObscuredFloat TaxiRewardExtraTimeCoef => _earnMoneyConfig.TaxiRewardExtraTimeCoef;

		public ObscuredFloat BusinessProfitIntervalInSec => _earnMoneyConfig.BusinessProfitIntervalInSec;

		public ObscuredInt BusinessProfitMaxPerSession => _earnMoneyConfig.BusinessProfitMaxPerSession;

		public ObscuredInt Tall1 => _pricesConfig.Tall1;

		public ObscuredInt Tall2 => _pricesConfig.Tall2;

		public ObscuredInt Tall2SmallCar => _pricesConfig.Tall2SmallCar;

		public ObscuredInt Tall2LargeCar => _pricesConfig.Tall2LargeCar;

		public ObscuredInt WeightTallDefault => _pricesConfig.WeightTallDefault;

		public ObscuredInt WeightTallWeightCoef => _pricesConfig.WeightTallWeightCoef;

		public ObscuredInt VinylsMin => _pricesConfig.VinylsMin;

		public ObscuredInt VinylsMax => _pricesConfig.VinylsMax;

		public ObscuredInt VinylsStep => _pricesConfig.VinylsStep;

		public ObscuredInt VinylsText => _pricesConfig.VinylsText;

		public ObscuredInt RimsMin => _pricesConfig.WheelRimsMin;

		public ObscuredInt RimsMax => _pricesConfig.WheelRimsMax;

		public ObscuredInt RimsStep => _pricesConfig.WheelRimsStep;

		public ObscuredInt BodyKitsMoneyPrice => _pricesConfig.BodyKitsMoneyPrice;

		public AllPricesService(PricesConfig pricesConfig, EarnMoneyConfig earnMoneyConfig)
		{
			_earnMoneyConfig = earnMoneyConfig;
			_pricesConfig = pricesConfig;
		}
	}
}
