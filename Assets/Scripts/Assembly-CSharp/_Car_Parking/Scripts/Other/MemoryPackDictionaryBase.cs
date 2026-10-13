using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Other
{
	public abstract class MemoryPackDictionaryBase
	{
		public string Serialize(IDictionary<string, object> changedData, string accountID)
		{
			return MemoryPackHelper.SerializeDictionary(changedData, GetKeyIndexMap(), accountID);
		}

		public IDictionary<string, object> Deserialize(string packed, string accountID)
		{
			Dictionary<string, ushort> keyIndexMap = GetKeyIndexMap();
			Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();
			foreach (KeyValuePair<string, ushort> item in keyIndexMap)
			{
				dictionary[item.Value] = item.Key;
			}
			return MemoryPackHelper.DeserializeDictionary(packed, GetTypesMap(), dictionary, accountID);
		}

		protected abstract Dictionary<string, ushort> GetKeyIndexMap();

		protected abstract Dictionary<string, Type> GetTypesMap();
	}
}
