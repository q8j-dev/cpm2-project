using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Economics.Export;

namespace _Car_Parking.Scripts.Economics
{
	[CreateAssetMenu(fileName = "PricesConfig", menuName = "Economics/PricesConfig")]
	public class PricesConfig : ExportableScriptableObject
	{
		public ObscuredInt WheelPressure = 1010;

		public ObscuredInt ChangeTiresFree = 3000;

		public ObscuredInt ChangeTiresGarage = 1050;

		public ObscuredInt Oil = 1000;

		public ObscuredInt CarWash = 1100;

		public ObscuredInt Repair = 1100;

		public ObscuredInt Fuel95 = 50;

		public ObscuredInt Fuel98 = 60;

		public ObscuredInt Fuel100 = 80;

		public ObscuredInt FuelE85 = 100;

		public ObscuredInt FuelDiesel = 90;

		public ObscuredInt MultiplayerRaceLose = 2500;

		public ObscuredInt MultiplayerPursuitLose = 5000;

		public ObscuredInt Tall1 = 55;

		public ObscuredInt Tall2 = 75;

		public ObscuredInt Tall2SmallCar = 25;

		public ObscuredInt Tall2LargeCar = 55;

		public ObscuredInt WeightTallDefault = 50;

		public ObscuredInt WeightTallWeightCoef = 10;

		public ObscuredInt VinylsMin = 100;

		public ObscuredInt VinylsMax = 6000;

		public ObscuredInt VinylsStep = 100;

		public ObscuredInt VinylsText = 1000;

		public ObscuredInt WheelRimsMin = 1000;

		public ObscuredInt WheelRimsMax = 20000;

		public ObscuredInt WheelRimsStep = 300;

		public ObscuredInt BodyKitsMoneyPrice = 5000;
	}
}
