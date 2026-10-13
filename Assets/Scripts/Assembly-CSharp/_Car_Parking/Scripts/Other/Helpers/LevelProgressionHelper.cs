using System.Collections.Generic;
using CPM.Database;
using CPM.Rating.Entities;
using CodeStage.AntiCheat.Storage;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class LevelProgressionHelper
	{
		private const int MaxLevelsAmount = 200;

		private const string PlayerPrefLevelBestResult = "bestResultID";

		public static int LevelCompletedAmount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 200; i++)
				{
					if (LevelIsCompleted(i))
					{
						num++;
					}
				}
				return num;
			}
		}

		public static float GetLevelBestResult(int index)
		{
			return ObscuredPrefs.GetFloat("bestResultID" + index, -1f);
		}

		public static void SetLevelBestResult(int index, float value)
		{
			ObscuredPrefs.SetFloat("bestResultID" + index, value);
		}

		public static bool LevelIsCompleted(int index)
		{
			return GetLevelBestResult(index) > 0f;
		}

		public static void SendLevelsProgressionToDataBase(bool forceSend, FreeDriveProvider freeDriveProvider)
		{
			if ((bool)freeDriveProvider.FreeDriveDB)
			{
				FreeDriveDB freeDriveDB = freeDriveProvider.FreeDriveDB;
				int? levels = LevelCompletedAmount;
				freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, levels), forceSend).Forget();
			}
		}

		public static Dictionary<int, float> GetLevelProgressionData()
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < 200; i++)
			{
				if (LevelIsCompleted(i))
				{
					dictionary[i] = GetLevelBestResult(i);
				}
			}
			return dictionary;
		}

		public static void ApplyLevelProgressionDataIfNeed(Dictionary<int, float> levelProgressionData, FreeDriveProvider freeDriveProvider)
		{
			bool flag = false;
			foreach (int key in levelProgressionData.Keys)
			{
				if (!LevelIsCompleted(key) || levelProgressionData[key] < GetLevelBestResult(key))
				{
					SetLevelBestResult(key, levelProgressionData[key]);
					flag = true;
				}
			}
			if (flag)
			{
				SendLevelsProgressionToDataBase(forceSend: true, freeDriveProvider);
			}
		}

		public static void DeleteAllLevelProgressionData()
		{
			for (int i = 0; i < 200; i++)
			{
				ObscuredPrefs.DeleteKey("bestResultID" + i);
			}
		}
	}
}
