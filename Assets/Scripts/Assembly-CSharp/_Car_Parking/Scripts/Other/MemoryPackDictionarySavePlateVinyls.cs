using System;
using System.Collections.Generic;
using CPM.Database;
using DataEntities.Vinyl;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDictionarySavePlateVinyls : MemoryPackDictionaryBase
	{
		private const string PlateData = "PlateData";

		private const string Plate1 = "Plate1";

		private const string Plate2 = "Plate2";

		private const string Plate3 = "Plate3";

		private const string Plate4 = "Plate4";

		private const string Plate5 = "Plate5";

		private const string Plate6 = "Plate6";

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			return new Dictionary<string, ushort>
			{
				["PlateData"] = 0,
				["Plate1"] = 1,
				["Plate2"] = 2,
				["Plate3"] = 3,
				["Plate4"] = 4,
				["Plate5"] = 5,
				["Plate6"] = 6
			};
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			return new Dictionary<string, Type>
			{
				["PlateData"] = typeof(Dictionary<int, PlateData>),
				["Plate1"] = typeof(List<VinylData>),
				["Plate2"] = typeof(List<VinylData>),
				["Plate3"] = typeof(List<VinylData>),
				["Plate4"] = typeof(List<VinylData>),
				["Plate5"] = typeof(List<VinylData>),
				["Plate6"] = typeof(List<VinylData>)
			};
		}
	}
}
