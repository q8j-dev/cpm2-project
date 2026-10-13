namespace _Car_Parking.Scripts
{
	public static class DirectoryHelper
	{
		public const string CarDefaultTuningDataPath = "Configs/CarTuningData/";

		public const string SellCarDataPath = "sellcars/";

		public const string CarSaveDataPath = "cars/";

		public const string PlayerDataSaveDataPath = "PlayerData";

		public const string PlatesSaveDataPath = "plates/";

		public const string CarVinylsSuffix = "/carVinyls";

		public const string CarInfoSuffix = "/mainData";

		public const string CarWindowVinylsSuffix = "/windowVinyls";

		public const string PlatesDataFilePath = "plates/allData";

		public const string TuningTimersSaveDataPath = "TuningTimers/";

		public const string EngineTuningTimersSaveDataPath = "TuningTimers/EngineTuning/";

		public const string PurchaseCrashliticsPath = "PurchaseCrashlitics/Timers";

		public const string ShowCaseEnginePath = "engine_0";

		public const string ShowCaseIntercoolerPath = "Intercooler_0";

		public const string ShowCaseBlockPath = "Block_0";

		public const string ShowCaseGearboxPath = "Gearbox_0";

		public const string ShowCaseFuelRailPath = "Fuel_Rail_0";

		public const string ShowCaseExhaustPipePath = "ExhaustPipe_0";

		public const string ShowCaseEngineHeadPath = "EngineHead_0";

		public static string GetCarInfoPath(string carInstanceId)
		{
			return "cars/" + carInstanceId + "/mainData";
		}

		public static string GetCarVinylsPath(string carInstanceId)
		{
			return "cars/" + carInstanceId + "/carVinyls";
		}

		public static string GetWindowVinylsPath(string carInstanceId)
		{
			return "cars/" + carInstanceId + "/windowVinyls";
		}

		public static string GetPlateVinylsPath(string plateId)
		{
			return "plates/" + plateId;
		}
	}
}
