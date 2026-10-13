using System;
using System.Collections.Generic;
using System.Linq;

namespace Persistence.Systems
{
	[Serializable]
	[SaveableData]
	public class DataClassesVersionStorage
	{
		[Serializable]
		public class Entry
		{
			public string id;

			public string version;
		}

		public List<Entry> entries;

		public Dictionary<string, string> VersionsDict { get; set; }

		public void SetEntry(string id, string version)
		{
			if (VersionsDict.ContainsKey(id))
			{
				VersionsDict[id] = version;
				entries.Single((Entry n) => n.id == id).version = version;
			}
			else
			{
				entries.Add(new Entry
				{
					id = id,
					version = version
				});
				VersionsDict.Add(id, version);
			}
		}

		public void Initialize()
		{
			if (entries != null && entries.Count > 0)
			{
				VersionsDict = entries.ToDictionary((Entry a) => a.id, (Entry a) => a.version);
			}
			else
			{
				entries = new List<Entry>();
				VersionsDict = new Dictionary<string, string>();
			}
		}
	}
}
