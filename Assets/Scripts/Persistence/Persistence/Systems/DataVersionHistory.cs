using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Persistence.Systems
{
	[CreateAssetMenu(menuName = "Scriptable/Persistence/DataVersionEntry")]
	public class DataVersionHistory : ScriptableObject
	{
		[Serializable]
		public class Entry
		{
			public string saveableType;

			public string migratorToType;

			public string version;
		}

		public string id;

		public string path;

		public Entry[] entries;

		public Dictionary<string, Entry> entriesDict;

		public bool ContainsType(string type)
		{
			Entry[] array = entries;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].saveableType == type)
				{
					return true;
				}
			}
			return false;
		}

		public void Initialize()
		{
			entriesDict = entries.ToDictionary((Entry a) => a.version, (Entry a) => a);
		}
	}
}
