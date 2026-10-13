using UnityEngine;

namespace _Car_Parking.Scripts.Analytics.Generic
{
	public static class AnalyticsHelper
	{
		public const string LastCarIdKey = "LastCarID";

		public static void SaveLastCarId(int lastCarId)
		{
			if (lastCarId != 0)
			{
				PlayerPrefs.SetInt("LastCarID", lastCarId);
				PlayerPrefs.Save();
			}
		}

		public static string GetCurrentCarId()
		{
			return V.CoreGameManager?.SlotService?.GetSlot(V.SlotID)?.CarFilePath ?? string.Empty;
		}
	}
}
