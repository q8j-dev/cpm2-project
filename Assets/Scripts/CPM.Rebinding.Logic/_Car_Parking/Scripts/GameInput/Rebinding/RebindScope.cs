using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class RebindScope
	{
		public static HashSet<string> CoActiveMaps(string targetMap, IEnumerable<IEnumerable<string>> groupMapSets)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal) { targetMap };
			if (groupMapSets == null)
			{
				return hashSet;
			}
			List<string> list = new List<string>(16);
			foreach (IEnumerable<string> groupMapSet in groupMapSets)
			{
				if (groupMapSet == null)
				{
					continue;
				}
				list.Clear();
				list.AddRange(groupMapSet);
				if (!list.Contains(targetMap))
				{
					continue;
				}
				foreach (string item in list)
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		public static List<InputActionMap> ScopeMaps(InputActionAsset asset, InputAction target, IReadOnlyList<InputAction> followers, IEnumerable<IEnumerable<string>> groupMapSets)
		{
			HashSet<string> names = CoActiveMaps(target.actionMap.name, groupMapSets);
			if (followers != null)
			{
				foreach (InputAction follower in followers)
				{
					if (follower?.actionMap != null)
					{
						names.UnionWith(CoActiveMaps(follower.actionMap.name, groupMapSets));
					}
				}
			}
			List<InputActionMap> list = new List<InputActionMap>(names.Count);
			list.AddRange(asset.actionMaps.Where((InputActionMap map) => names.Contains(map.name)));
			return list;
		}

		public static bool CanFireTogether(BindingSlot a, BindingSlot b, IReadOnlyList<string[]> groupMapSets, Func<BindingSlot, int, bool> isUnusedIn)
		{
			string text = a.Action?.actionMap?.name;
			string text2 = b.Action?.actionMap?.name;
			if (text == null || text2 == null)
			{
				return false;
			}
			bool flag = false;
			if (groupMapSets != null)
			{
				for (int i = 0; i < groupMapSets.Count; i++)
				{
					string[] array = groupMapSets[i];
					if (array != null && Array.IndexOf(array, text) >= 0 && Array.IndexOf(array, text2) >= 0)
					{
						flag = true;
						if (isUnusedIn == null || (!isUnusedIn(a, i) && !isUnusedIn(b, i)))
						{
							return true;
						}
					}
				}
			}
			if (!flag)
			{
				return text == text2;
			}
			return false;
		}
	}
}
