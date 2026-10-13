using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class LayoutSectionPlan
	{
		public static List<List<string>> Build(IReadOnlyList<string> maps, Func<string, string> mergeInto)
		{
			List<List<string>> list = new List<List<string>>();
			if (maps == null)
			{
				return list;
			}
			HashSet<string> onPage = new HashSet<string>(maps, StringComparer.Ordinal);
			List<string> list2 = new List<string>(maps.Count);
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>(StringComparer.Ordinal);
			foreach (string map in maps)
			{
				string text = RootOf(map, onPage, mergeInto);
				list2.Add(text);
				if (text == map && !dictionary.ContainsKey(map))
				{
					List<string> item = (dictionary[map] = new List<string> { map });
					list.Add(item);
				}
			}
			for (int i = 0; i < maps.Count; i++)
			{
				if (list2[i] != maps[i] && !dictionary[list2[i]].Contains(maps[i]))
				{
					dictionary[list2[i]].Add(maps[i]);
				}
			}
			return list;
		}

		private static string RootOf(string map, HashSet<string> onPage, Func<string, string> mergeInto)
		{
			if (mergeInto == null)
			{
				return map;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal) { map };
			string text = map;
			while (true)
			{
				string text2 = mergeInto(text);
				if (string.IsNullOrEmpty(text2) || !onPage.Contains(text2))
				{
					return text;
				}
				if (!hashSet.Add(text2))
				{
					break;
				}
				text = text2;
			}
			return map;
		}
	}
}
