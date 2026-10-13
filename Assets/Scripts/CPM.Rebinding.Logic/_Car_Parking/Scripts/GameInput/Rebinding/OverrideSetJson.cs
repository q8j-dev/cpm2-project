using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class OverrideSetJson
	{
		[Serializable]
		private sealed class Entry
		{
			public string action = default;

			public string id = default;

			public string path = default;

			public string interactions = default;

			public string processors = default;
		}

		[Serializable]
		private sealed class EntryList
		{
			public List<Entry> bindings = new List<Entry>();
		}

		public const string EmptySet = "{\"bindings\":[]}";

		public static bool Split(string json, Predicate<string> includeId, out string included, out string excluded)
		{
			included = "{\"bindings\":[]}";
			excluded = "{\"bindings\":[]}";
			if (!TryParse(json, out var list))
			{
				return false;
			}
			EntryList entryList = new EntryList();
			EntryList entryList2 = new EntryList();
			foreach (Entry binding in list.bindings)
			{
				(includeId(binding.id) ? entryList : entryList2).bindings.Add(binding);
			}
			included = JsonUtility.ToJson(entryList);
			excluded = JsonUtility.ToJson(entryList2);
			return true;
		}

		public static bool IsEmpty(string json)
		{
			if (TryParse(json, out var list))
			{
				return list.bindings.Count == 0;
			}
			return true;
		}

		private static bool TryParse(string json, out EntryList list)
		{
			list = null;
			if (string.IsNullOrEmpty(json))
			{
				return false;
			}
			try
			{
				list = JsonUtility.FromJson<EntryList>(json);
			}
			catch (ArgumentException)
			{
				return false;
			}
			if (list == null)
			{
				list = new EntryList();
			}
			EntryList entryList = list;
			if (entryList.bindings == null)
			{
				entryList.bindings = new List<Entry>();
			}
			return true;
		}
	}
}
