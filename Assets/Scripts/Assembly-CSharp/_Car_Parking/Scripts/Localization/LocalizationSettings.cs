using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Localization
{
	[CreateAssetMenu(fileName = "LocalizationSettings", menuName = "◆ Localization/Settings")]
	public class LocalizationSettings : ScriptableObject
	{
		public List<Sheet> Sheets;

		public string SaveFolderPath = "Assets/Resources/Localization/SavedFiles/";

		public static string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

		private static DateTime _timestamp;

		private static LocalizationSettings _instance;

		public static LocalizationSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = LoadSettings();
				}
				return _instance;
			}
		}

		private static LocalizationSettings LoadSettings()
		{
			LocalizationSettings localizationSettings = Resources.Load<LocalizationSettings>("Localization/LocalizationSettings");
			if (localizationSettings != null)
			{
				return localizationSettings;
			}
			throw new Exception("Localization settings not found: Localization/LocalizationSettings");
		}
	}
}
