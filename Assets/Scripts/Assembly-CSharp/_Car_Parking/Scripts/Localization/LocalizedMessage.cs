using System;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Localization
{
	[Serializable]
	public class LocalizedMessage
	{
		[SerializeField]
		private string Pattern;

		[SerializeField]
		private LocalizedString[] Entries;

		[SerializeField]
		private bool Localized = true;

		public string Value
		{
			get
			{
				if (!Localized)
				{
					return Pattern;
				}
				return string.Format(Pattern, Entries.ToList().ConvertAll((Converter<LocalizedString, object>)((LocalizedString localizedString) => localizedString.ToString())).ToArray());
			}
		}

		public static implicit operator string(LocalizedMessage localizedMessage)
		{
			return localizedMessage.Value;
		}

		public override string ToString()
		{
			return Value;
		}

		private LocalizedMessage(string pattern, params LocalizedString[] entries)
		{
			Pattern = pattern;
			Entries = entries;
		}

		private LocalizedMessage(string nonLocalizedMessage)
		{
			Localized = false;
			Pattern = nonLocalizedMessage;
		}

		public static LocalizedMessage NonLocalized(string nonLocalizedMessage)
		{
			return new LocalizedMessage(nonLocalizedMessage);
		}

		public static LocalizedMessage FromKeys(string pattern, params string[] keys)
		{
			return new LocalizedMessage(pattern, keys.Select((string k) => new LocalizedString(k)).ToArray());
		}

		public static LocalizedMessage FromLocalizedStrings(string pattern, params LocalizedString[] keys)
		{
			return new LocalizedMessage(pattern, keys);
		}

		public static LocalizedMessage FromLocalizedString(LocalizedString localizedString)
		{
			return new LocalizedMessage("{0}", localizedString);
		}

		public static LocalizedMessage FromKey(string key)
		{
			return new LocalizedMessage("{0}", new LocalizedString(key));
		}
	}
}
