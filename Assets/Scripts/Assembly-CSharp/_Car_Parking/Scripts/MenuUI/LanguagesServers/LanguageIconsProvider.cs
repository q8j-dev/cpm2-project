using System.Collections.Generic;
using TMPro;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.LanguagesServers
{
	public class LanguageIconsProvider
	{
		public enum CpmLanguages
		{
			Afrikaans = 0,
			Arabic = 1,
			Basque = 2,
			Belarusian = 3,
			Bulgarian = 4,
			Catalan = 5,
			Chinese = 6,
			Czech = 7,
			Danish = 8,
			Dutch = 9,
			English = 10,
			Estonian = 11,
			Faroese = 12,
			Finnish = 13,
			French = 14,
			German = 15,
			Greek = 16,
			Hebrew = 17,
			Hungarian = 18,
			Icelandic = 19,
			Indonesian = 20,
			Italian = 21,
			Japanese = 22,
			Korean = 23,
			Latvian = 24,
			Lithuanian = 25,
			Norwegian = 26,
			Polish = 27,
			Portuguese = 28,
			Romanian = 29,
			Russian = 30,
			SerboCroatian = 31,
			Slovak = 32,
			Slovenian = 33,
			Spanish = 34,
			Swedish = 35,
			Thai = 36,
			Turkish = 37,
			Ukrainian = 38,
			Vietnamese = 39,
			ChineseSimplified = 40,
			ChineseTraditional = 41,
			Unknown = 42,
			Malay = 43,
			Kazakh = 44,
			Albanian = 45,
			Algerian = 46,
			Armenian = 47,
			Argentinian = 48,
			Australian = 49,
			Austrian = 50,
			Azerbaijani = 51,
			Bangladesh = 52,
			Belgium = 53,
			Bosnian = 54,
			Brazilian = 55,
			Cambodian = 56,
			Canadian = 57,
			Chile = 58,
			Colombian = 59,
			Croatian = 60,
			Ecuadorian = 61,
			Egyptian = 62,
			Georgian = 63,
			Indian = 64,
			Iranian = 65,
			Iraqi = 66,
			Irish = 67,
			Jamaican = 68,
			Jordan = 69,
			Kuwaiti = 70,
			Kyrgyz = 71,
			Lebanon = 72,
			Mexican = 73,
			Moroccan = 74,
			Pakistani = 75,
			Philippines = 76,
			Qatar = 77,
			Singapore = 78,
			Switzerland = 79,
			Tunisian = 80,
			Uzbek = 81,
			Zimbabwean = 82,
			Nigerian = 83,
			Turkmen = 84
		}

		public const string LanguageIndexPrefKey = "languageIndex";

		private LanguagesIconsConfig _config;

		private List<CpmLanguages> _languagesInDropdown = new List<CpmLanguages>();

		private const string LanguageKeyPrefix = "Region.";

		public CpmLanguages Current { get; set; }

		public LanguageIconsProvider(LanguagesIconsConfig config)
		{
			_config = config;
		}

		public List<TMP_Dropdown.OptionData> GetLanguagesDropdownList()
		{
			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
			_languagesInDropdown.Clear();
			for (int i = 0; i < _config.LanguageIcons.Count; i++)
			{
				LanguagesIconsConfig.LanguageIcon languageIcon = _config.LanguageIcons[i];
				if (languageIcon != null && !IsHiddenInDropdown(languageIcon.Language))
				{
					list.Add(new TMP_Dropdown.OptionData
					{
						text = LocalizedName(languageIcon.Language),
						image = languageIcon.Icon
					});
					_languagesInDropdown.Add(languageIcon.Language);
				}
			}
			return list;
		}

		private static bool IsHiddenInDropdown(CpmLanguages language)
		{
			if (language != CpmLanguages.Unknown && language != CpmLanguages.ChineseSimplified)
			{
				return language == CpmLanguages.ChineseTraditional;
			}
			return true;
		}

		public static string LocalizedName(CpmLanguages language)
		{
			string text = "Region." + language;
			string text2 = LocalizationManager.Localize(text);
			if (!(text2 == text))
			{
				return text2;
			}
			return language.ToString();
		}

		public CpmLanguages GetLanguage(int dropDownIndex)
		{
			return _languagesInDropdown[dropDownIndex];
		}

		public int IndexOfLanguage(int languages)
		{
			int num = _languagesInDropdown.IndexOf((CpmLanguages)languages);
			if (num >= 0)
			{
				return num;
			}
			return 0;
		}
	}
}
