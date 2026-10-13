using System;
using System.Collections.Generic;
using CPM.Database;
using DataEntities.Vinyl;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDictionarySaveCar : MemoryPackDictionaryBase
	{
		private const string CarID = "CarID";

		private const string CarInstanceId = "CarInstanceId";

		private const string BoughtBodyKits = "BoughtBodyKits";

		private const string Colors = "Colors";

		private const string TuningData = "TuningData";

		private const string AdditionalData = "AdditionalData";

		private const string InstalledBodyKits = "InstalledBodyKits";

		private const string Vinyls = "Vinyls";

		private const string Window = "Window";

		private const string TuningDataFrontWheelDataLimits = "TuningData.FrontWheelData.Limits";

		private const string TuningDataRearWheelDataLimits = "TuningData.RearWheelData.Limits";

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			return new Dictionary<string, ushort>
			{
				["CarID"] = 0,
				["CarInstanceId"] = 1,
				["BoughtBodyKits"] = 2,
				["Colors"] = 3,
				["TuningData"] = 4,
				["AdditionalData"] = 5,
				["InstalledBodyKits"] = 6,
				["Vinyls"] = 7,
				["Window"] = 8,
				["TuningData.FrontWheelData.Limits"] = 9,
				["TuningData.RearWheelData.Limits"] = 10
			};
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			return new Dictionary<string, Type>
			{
				["CarID"] = typeof(int),
				["CarInstanceId"] = typeof(string),
				["BoughtBodyKits"] = typeof(BoughtBodyKits),
				["Colors"] = typeof(Colors),
				["TuningData"] = typeof(CarTuningData),
				["AdditionalData"] = typeof(AdditionalData),
				["InstalledBodyKits"] = typeof(InstalledBodyKits),
				["Vinyls"] = typeof(List<VinylData>),
				["Window"] = typeof(List<VinylData>),
				["TuningData.FrontWheelData.Limits"] = typeof(string),
				["TuningData.RearWheelData.Limits"] = typeof(string)
			};
		}
	}
}
