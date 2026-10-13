using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.MenuUI.LanguagesServers;

namespace _Car_Parking.Scripts.MenuUI
{
	[CreateAssetMenu(fileName = "LanguagesIconsConfig", menuName = "Configs/LanguagesIconsConfig")]
	public class LanguagesIconsConfig : ScriptableObject
	{
		[Serializable]
		public class LanguageIcon
		{
			public LanguageIconsProvider.CpmLanguages Language;

			public Sprite Icon;
		}

		public List<LanguageIcon> LanguageIcons;

		public Sprite TryGetIcon(LanguageIconsProvider.CpmLanguages language)
		{
			foreach (LanguageIcon languageIcon in LanguageIcons)
			{
				if (languageIcon.Language == language)
				{
					return languageIcon.Icon;
				}
			}
			return null;
		}
	}
}
