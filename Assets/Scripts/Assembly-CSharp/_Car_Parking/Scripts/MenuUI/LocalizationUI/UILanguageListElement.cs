using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(Toggle))]
	public class UILanguageListElement : MonoBehaviour
	{
		[SerializeField]
		private Toggle LanguageToggle;

		public string Language;

		private UserPreferenceAnalytic _userPreferenceAnalytic;

		private void Reset()
		{
			if (!LanguageToggle)
			{
				LanguageToggle = GetComponent<Toggle>();
			}
		}

		public void InitAnalytic(UserPreferenceAnalytic userPreferenceAnalytic)
		{
			_userPreferenceAnalytic = userPreferenceAnalytic;
		}

		private void Start()
		{
			LanguageToggle.onValueChanged.AddListener(SelectLanguage);
		}

		private List<string> GetLanguageList()
		{
			LocalizationManager.Read();
			return LocalizationManager.LocalizedTexts.Keys.ToList();
		}

		private void SelectLanguage(bool isOn)
		{
			if (isOn && LocalizationManager.Language != Language)
			{
				_userPreferenceAnalytic?.LogUserPreference(UserPreferenceType.lang_change, LocalizationManager.Language, Language);
				LocalizationManager.Language = Language;
			}
		}

		public void ToggleSetOn()
		{
			LanguageToggle.isOn = true;
		}
	}
}
