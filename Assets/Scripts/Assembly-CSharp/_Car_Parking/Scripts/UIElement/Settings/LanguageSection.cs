using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class LanguageSection : SettingsSectionBase
	{
		[SerializeField]
		private UILanguageListElement[] LanguageListElements = default;

		private void Start()
		{
			UILanguageListElement[] languageListElements = LanguageListElements;
			foreach (UILanguageListElement uILanguageListElement in languageListElements)
			{
				uILanguageListElement.InitAnalytic(base.UserPreferenceAnalytic);
				if (LocalizationManager.Language == uILanguageListElement.Language)
				{
					uILanguageListElement.ToggleSetOn();
				}
			}
		}
	}
}
