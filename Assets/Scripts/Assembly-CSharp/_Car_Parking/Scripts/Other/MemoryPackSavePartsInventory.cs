using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackSavePartsInventory : MemoryPackDictionaryBase
	{
		private const string Inventory = "Inventory";

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			return new Dictionary<string, ushort> { ["Inventory"] = 0 };
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			return new Dictionary<string, Type> { ["Inventory"] = typeof(List<PartData>) };
		}
	}
}
