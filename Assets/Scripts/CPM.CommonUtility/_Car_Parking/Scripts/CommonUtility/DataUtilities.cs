using System.Collections.Generic;
using System.IO;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class DataUtilities
	{
		private static string DataPath => Path.Combine(Application.persistentDataPath, "49183427851348");

		private static string EasySavesPath => Path.Combine(Application.persistentDataPath, "es3");

		private static string GhostsPath => Path.Combine(Application.persistentDataPath, "ghosts");

		public static void DeleteAllUserData(List<IAllPrefsSaver> allPrefsSavers, List<IDeleteUserData> allDataDeleters)
		{
			Debug.Log("DeleteAllUserData");
			DeleteAllPrefsExclusions deleteAllPrefsExclusions = new DeleteAllPrefsExclusions();
			deleteAllPrefsExclusions.Save(allPrefsSavers);
			PlayerPrefs.DeleteAll();
			ObscuredPrefs.DeleteAll();
			if (allDataDeleters != null)
			{
				for (int i = 0; i < allDataDeleters.Count; i++)
				{
					if (allDataDeleters[i] != null)
					{
						allDataDeleters[i].DeleteData();
					}
				}
			}
			if (allPrefsSavers != null)
			{
				deleteAllPrefsExclusions.Load(allPrefsSavers);
			}
			if (Directory.Exists(DataPath))
			{
				foreach (string item in Directory.EnumerateFiles(DataPath))
				{
					File.Delete(item);
				}
			}
			if (Directory.Exists(EasySavesPath))
			{
				Directory.Delete(EasySavesPath, recursive: true);
			}
			if (Directory.Exists(GhostsPath))
			{
				Directory.Delete(GhostsPath, recursive: true);
			}
		}

		public static void DeleteAllSaves()
		{
			if (Directory.Exists(DataPath))
			{
				Directory.Delete(DataPath, recursive: true);
			}
			if (Directory.Exists(EasySavesPath))
			{
				Directory.Delete(EasySavesPath, recursive: true);
			}
			if (Directory.Exists(GhostsPath))
			{
				Directory.Delete(GhostsPath, recursive: true);
			}
		}
	}
}
