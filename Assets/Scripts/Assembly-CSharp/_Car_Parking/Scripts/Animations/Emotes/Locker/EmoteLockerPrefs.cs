using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public static class EmoteLockerPrefs
	{
		public const string PresetKey = "emoteLockerPreset";

		public const string LastTabKey = "emoteLockerLastTab";

		public const string LastMovesFilterKey = "emoteLockerLastMovesFilter";

		public const string LastEmojiFilterKey = "emoteLockerLastEmojiFilter";

		public const int LegacyWheelSlots = 8;

		public static List<int> GetPreset()
		{
			if (!PlayerPrefs.HasKey("emoteLockerPreset"))
			{
				return AnimationShop.GetSelectedAnimationsByPrefs();
			}
			return ParseList(PlayerPrefs.GetString("emoteLockerPreset"));
		}

		public static void SetPreset(List<int> values)
		{
			PlayerPrefs.SetString("emoteLockerPreset", JoinList(values));
			MirrorToLegacyWheel(values);
			PlayerPrefs.Save();
		}

		public static void DeletePreset()
		{
			PlayerPrefs.DeleteKey("emoteLockerPreset");
			PlayerPrefs.Save();
		}

		public static List<int> ParseList(string raw)
		{
			List<int> list = new List<int>();
			if (string.IsNullOrEmpty(raw))
			{
				return list;
			}
			string[] array = raw.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], out var result))
				{
					list.Add(result);
				}
			}
			return list;
		}

		public static string JoinList(IReadOnlyList<int> values)
		{
			if (values == null || values.Count == 0)
			{
				return string.Empty;
			}
			return string.Join(",", values);
		}

		private static void MirrorToLegacyWheel(List<int> values)
		{
			if (values != null && values.Count != 0)
			{
				int count = Mathf.Min(values.Count, 8);
				AnimationShop.SetSelectedAnimationsByPrefs(values.GetRange(0, count));
			}
		}
	}
}
