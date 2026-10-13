using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Economics
{
	public interface IPricesService
	{
		ObscuredInt OilPrice { get; }

		ObscuredInt WheelPressurePrice { get; }

		ObscuredInt ChangeTiresFreePrice { get; }

		ObscuredInt ChangeTiresGaragePrice { get; }

		ObscuredInt CarWashGaragePrice { get; }

		ObscuredInt CarWashFreePrice { get; }

		ObscuredInt RepairPrice { get; }

		ObscuredInt Fuel95Price { get; }

		ObscuredInt Fuel98Price { get; }

		ObscuredInt Fuel100Price { get; }

		ObscuredInt FuelE85Price { get; }

		ObscuredInt FuelDieselPrice { get; }

		ObscuredInt MultiplayerRaceLose { get; }

		ObscuredInt MultiplayerPursuitLose { get; }

		ObscuredInt Tall1 { get; }

		ObscuredInt Tall2 { get; }

		ObscuredInt Tall2SmallCar { get; }

		ObscuredInt Tall2LargeCar { get; }

		ObscuredInt WeightTallDefault { get; }

		ObscuredInt WeightTallWeightCoef { get; }

		ObscuredInt VinylsMin { get; }

		ObscuredInt VinylsMax { get; }

		ObscuredInt VinylsStep { get; }

		ObscuredInt VinylsText { get; }

		ObscuredInt RimsMin { get; }

		ObscuredInt RimsMax { get; }

		ObscuredInt RimsStep { get; }

		ObscuredInt BodyKitsMoneyPrice { get; }
	}
}
