using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Localization
{
	[Serializable]
	public class LocalizedString
	{
		[SerializeField]
		private string LocalizationKey;

		public static string[] ReversedLanguages = new string[2] { "kz", "ko" };

		[SerializeField]
		private bool Localized = true;

		public string Value
		{
			get
			{
				if (!Localized)
				{
					return LocalizationKey;
				}
				return LocalizationManager.Localize(LocalizationKey);
			}
		}

		public static implicit operator string(LocalizedString localizedString)
		{
			return localizedString.Value;
		}

		public override string ToString()
		{
			return this;
		}

		public LocalizedString(string localizationKey)
		{
			LocalizationKey = localizationKey;
		}

		public LocalizedString(string localizationKey, string reversedAddString, bool newLineRequired = false)
		{
			string text = new LocalizedString(localizationKey);
			string text2 = (newLineRequired ? "\n" : "");
			if (!ReversedLanguages.Contains(LocalizationManager.Language))
			{
				LocalizationKey = text + text2 + reversedAddString;
			}
			else
			{
				LocalizationKey = reversedAddString + text2 + text;
			}
			Localized = false;
		}

		public static LocalizedString NonLocalized(string nonLocalizedMessage)
		{
			return new LocalizedString(nonLocalizedMessage)
			{
				Localized = false
			};
		}
	}
}
