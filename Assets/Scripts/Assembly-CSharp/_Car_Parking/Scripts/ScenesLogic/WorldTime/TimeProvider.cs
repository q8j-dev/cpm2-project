using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.ScenesLogic.WorldTime
{
	public class TimeProvider
	{
		public static int skyboxSelectedRuntime;

		public static SkyboxBlender skyboxBlender;

		private static int _customHours = 12;

		public static int CustomHours
		{
			get
			{
				return _customHours;
			}
			set
			{
				CustomHoursPrefs = value;
				_customHours = value;
			}
		}

		public static int CustomHoursPrefs
		{
			get
			{
				return PlayerPrefs.GetInt("customTime", 12);
			}
			set
			{
				PlayerPrefs.SetInt("customTime", value);
			}
		}

		public static int SelectedSkyboxPrefs
		{
			get
			{
				return PlayerPrefs.GetInt("skyboxSelected", 0);
			}
			set
			{
				PlayerPrefs.SetInt("skyboxSelected", value);
			}
		}

		public static bool CustomTimeFormat24
		{
			get
			{
				return PlayerPrefs.GetInt("customTimeFormat24", DefaultTimeFormat24) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("customTimeFormat24", value ? 1 : 0);
			}
		}

		private static int DefaultTimeFormat24
		{
			get
			{
				if (!(LocalizationManager.Language == "English"))
				{
					return 1;
				}
				return 0;
			}
		}

		public static void Init()
		{
			_customHours = CustomHoursPrefs;
		}
	}
}
