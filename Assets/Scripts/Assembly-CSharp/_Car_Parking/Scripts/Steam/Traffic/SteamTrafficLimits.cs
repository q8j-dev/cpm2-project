using Traffic;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Traffic
{
	public static class SteamTrafficLimits
	{
		public const int MaxCars = 100;

		public const int MaxPeople = 100;

		public const int DefaultDensity = 50;

		public const int InitialCars = 30;

		public const float PeopleVisibleSpawnDistance = 90f;

		public const float PeopleSpawnRadius = 150f;

		private const string CarQuantityKey = "CarQuantity";

		private const string PeopleDensityKey = "peopleTrafficDensity";

		private const string DefaultsAppliedKey = "SteamTrafficDefaultsV5";

		private const string DensityChosenKey = "SteamTrafficDensityChosen";

		public static bool DensityChosenByUser => PlayerPrefs.GetInt("SteamTrafficDensityChosen", 0) == 1;

		public static void MarkDensityChosenByUser()
		{
			PlayerPrefs.SetInt("SteamTrafficDensityChosen", 1);
			PlayerPrefs.Save();
		}

		public static int DefaultForQuality(int quality)
		{
			return 50;
		}

		public static void EnsureDefaultsApplied()
		{
			if (PlayerPrefs.GetInt("SteamTrafficDefaultsV5", 0) == 1)
			{
				return;
			}
			if (DensityChosenByUser)
			{
				PlayerPrefs.SetInt("SteamTrafficDefaultsV5", 1);
				PlayerPrefs.Save();
				return;
			}
			CPMGraphicsSettingsManager instance = CPMGraphicsSettingsManager.Instance;
			if ((bool)instance)
			{
				SetDefaultsForQuality(instance.Quality);
			}
		}

		public static void ApplyPresetDefaults(int quality)
		{
			if (!DensityChosenByUser)
			{
				SetDefaultsForQuality(quality);
				int num = PeopleDefaultForQuality(quality);
				PeopleTraffic instance = PeopleTraffic.Instance;
				if ((bool)instance && instance.CurrentDensity != num)
				{
					instance.ChangeTrafficDensity(num);
				}
			}
		}

		public static int PeopleDefaultForQuality(int quality)
		{
			return Mathf.Min(DefaultForQuality(quality), 100);
		}

		public static void SetDefaultsForQuality(int quality)
		{
			PlayerPrefs.SetInt("CarQuantity", DefaultForQuality(quality));
			PlayerPrefs.SetInt("peopleTrafficDensity", PeopleDefaultForQuality(quality));
			PlayerPrefs.SetInt("SteamTrafficDefaultsV5", 1);
			PlayerPrefs.Save();
		}
	}
}
