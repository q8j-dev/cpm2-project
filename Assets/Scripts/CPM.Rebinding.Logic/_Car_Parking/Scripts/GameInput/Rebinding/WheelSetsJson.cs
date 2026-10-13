using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class WheelSetsJson
	{
		[Serializable]
		private sealed class WheelSet
		{
			public string layout;

			public string overrides;
		}

		[Serializable]
		private sealed class WheelSetList
		{
			public List<WheelSet> sets = new List<WheelSet>();
		}

		public static string Get(string json, string layout)
		{
			foreach (WheelSet set in Parse(json).sets)
			{
				if (string.Equals(set.layout, layout, StringComparison.OrdinalIgnoreCase))
				{
					return set.overrides;
				}
			}
			return null;
		}

		public static string Set(string json, string layout, string overrides)
		{
			WheelSetList wheelSetList = Without(Parse(json), layout);
			wheelSetList.sets.Add(new WheelSet
			{
				layout = layout,
				overrides = overrides
			});
			return JsonUtility.ToJson(wheelSetList);
		}

		public static string Remove(string json, string layout)
		{
			return JsonUtility.ToJson(Without(Parse(json), layout));
		}

		private static WheelSetList Without(WheelSetList list, string layout)
		{
			WheelSetList wheelSetList = new WheelSetList();
			foreach (WheelSet set in list.sets)
			{
				if (!string.Equals(set.layout, layout, StringComparison.OrdinalIgnoreCase))
				{
					wheelSetList.sets.Add(set);
				}
			}
			return wheelSetList;
		}

		private static WheelSetList Parse(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return new WheelSetList();
			}
			try
			{
				WheelSetList obj = JsonUtility.FromJson<WheelSetList>(json) ?? new WheelSetList();
				WheelSetList wheelSetList = obj;
				if (wheelSetList.sets == null)
				{
					wheelSetList.sets = new List<WheelSet>();
				}
				return obj;
			}
			catch (ArgumentException)
			{
				return new WheelSetList();
			}
		}
	}
}
