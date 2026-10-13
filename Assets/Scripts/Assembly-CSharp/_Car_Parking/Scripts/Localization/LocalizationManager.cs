using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace _Car_Parking.Scripts.Localization
{
	public static class LocalizationManager
	{
		private static string _language = "English";

		private const string Path = "Localization/LocalizationSettings";

		public static Dictionary<string, Dictionary<string, string>> LocalizedTexts { get; } = new Dictionary<string, Dictionary<string, string>>();

		public static string Language
		{
			get
			{
				_language = PlayerPrefs.GetString("InGameLanguage", GetSystemLanguage());
				return _language;
			}
			set
			{
				_language = value;
				PlayerPrefs.SetString("InGameLanguage", value);
				LocalizationChanged?.Invoke();
			}
		}

		public static event Action LocalizationChanged;

		private static string GetSystemLanguage()
		{
			return Application.systemLanguage switch
			{
				SystemLanguage.English => "English", 
				SystemLanguage.German => "German", 
				SystemLanguage.Japanese => "Japanese", 
				SystemLanguage.Russian => "Russian", 
				SystemLanguage.Spanish => "Spanish", 
				SystemLanguage.Turkish => "Turkish", 
				SystemLanguage.Unknown => "English", 
				_ => "English", 
			};
		}

		public static void Read()
		{
			if (LocalizedTexts.Count > 0)
			{
				return;
			}
			LocalizationSettings localizationSettings = Resources.Load<LocalizationSettings>("Localization/LocalizationSettings");
			if (!localizationSettings)
			{
				Debug.LogError("LocalizationSettings not found at Resources/Localization/LocalizationSettings.");
				return;
			}
			List<string> list = new List<string>();
			foreach (Sheet sheet in localizationSettings.Sheets)
			{
				TextAsset textAsset = sheet.TextAsset;
				if (!textAsset)
				{
					Debug.LogError("Localization sheet `" + sheet.Name + "` TextAsset is missing or was unloaded, skipping.");
					continue;
				}
				List<string> lines = GetLines(textAsset.text);
				List<string> list2 = (from i in lines[0].Split(',')
					select i.Trim()).ToList();
				List<string> list3 = list2.Where((string l) => !string.IsNullOrEmpty(l)).ToList();
				if (list3.Count != list3.Distinct().Count())
				{
					Debug.LogError("Duplicated languages found in `" + sheet.Name + "`. This sheet is not loaded.");
					continue;
				}
				for (int num = 1; num < list2.Count; num++)
				{
					if (!LocalizedTexts.ContainsKey(list2[num]))
					{
						LocalizedTexts.Add(list2[num], new Dictionary<string, string>());
					}
				}
				for (int num2 = 1; num2 < lines.Count; num2++)
				{
					List<string> columns = GetColumns(lines[num2]);
					string text = columns[0];
					if (text == "" || list.Contains(text))
					{
						continue;
					}
					list.Add(text);
					for (int num3 = 1; num3 < list2.Count; num3++)
					{
						if (!LocalizedTexts[list2[num3]].ContainsKey(text))
						{
							LocalizedTexts[list2[num3]].Add(text, columns[num3]);
						}
					}
				}
			}
		}

		public static bool HasKey(string localizationKey)
		{
			if (LocalizedTexts.Count > 0)
			{
				return LocalizedTexts[Language].ContainsKey(localizationKey);
			}
			return false;
		}

		public static string Localize(string localizationKey, GameObject gameObject = null)
		{
			if (LocalizedTexts.Count <= 0)
			{
				Read();
			}
			if (!LocalizedTexts.ContainsKey(Language))
			{
				throw new KeyNotFoundException("Language not found: " + Language);
			}
			if (localizationKey == null)
			{
				localizationKey = "";
				LocalizedTexts[Language].Add(localizationKey, "");
			}
			if (!LocalizedTexts[Language].ContainsKey(localizationKey) || LocalizedTexts[Language][localizationKey] == "")
			{
				if (!LocalizedTexts["English"].ContainsKey(localizationKey))
				{
					return localizationKey;
				}
				return LocalizedTexts["English"][localizationKey];
			}
			return LocalizedTexts[Language][localizationKey].Replace("\\n", "\n");
		}

		public static List<string> GetLines(string text)
		{
			string text2 = text.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");
			foreach (Match item in Regex.Matches(text2, "\"[\\s\\S]+?\""))
			{
				text2 = text2.Replace(item.Value, item.Value.Replace("\"", null).Replace(",", "[_comma_]").Replace("\n", "[_newline_]"));
			}
			text2 = text2.Replace("。", "。 ").Replace("、", "、 ").Replace("：", "： ")
				.Replace("！", "！ ")
				.Replace("（", " （")
				.Replace("）", "） ")
				.Trim();
			return (from i in text2.Split('\n')
				where i != ""
				select i).ToList();
		}

		public static List<string> GetColumns(string line)
		{
			return (from j in line.Split(',')
				select j.Trim() into j
				select j.Replace("[_quote_]", "\"").Replace("[_comma_]", ",").Replace("[_newline_]", "\n")).ToList();
		}
	}
}
