using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDictionarySaveWalletData : MemoryPackDictionaryBase
	{
		private const string Money = "Money";

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			return new Dictionary<string, ushort> { ["Money"] = 0 };
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			return new Dictionary<string, Type> { ["Money"] = typeof(int) };
		}
	}
}
