using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	public static class GhostTogglePrefs
	{
		private const string Key = "ghost_race_enabled";

		public static bool Enabled
		{
			get
			{
				return PlayerPrefs.GetInt("ghost_race_enabled", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("ghost_race_enabled", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}
	}
}
