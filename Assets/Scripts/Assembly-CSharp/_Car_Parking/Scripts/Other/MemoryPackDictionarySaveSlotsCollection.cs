using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDictionarySaveSlotsCollection : MemoryPackDictionaryBase
	{
		public const ushort ForceSaveSlotsKey = 9999;

		private const ushort MaxGarageSlots = 20;

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			Dictionary<string, ushort> dictionary = new Dictionary<string, ushort>();
			for (ushort num = 0; num < 2; num++)
			{
				for (ushort num2 = 0; num2 < 20; num2++)
				{
					ushort value = (ushort)(num * 1000 + num2);
					dictionary.Add(value.ToString(), value);
				}
			}
			if (PlayerPrefs.GetInt("ForceSaveSlots", 0) == 1)
			{
				dictionary.Add(((ushort)9999).ToString(), 9999);
			}
			return dictionary;
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			for (ushort num = 0; num < 2; num++)
			{
				for (ushort num2 = 0; num2 < 20; num2++)
				{
					dictionary.Add(((ushort)(num * 1000 + num2)).ToString(), typeof(string));
				}
			}
			return dictionary;
		}
	}
}
