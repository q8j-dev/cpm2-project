using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace _Car_Parking.Scripts.Database.Cars
{
	public abstract class UtilsVehicles
	{
		private const int MaxPlayerNameLength = 20;

		private const bool UpdatedBlackList = false;

		public static void AddDeletedVehicleID(string carGeneratedID)
		{
			if (carGeneratedID == "" || ExistsInDeletedList(carGeneratedID))
			{
				return;
			}
			for (int i = 0; i < 50; i++)
			{
				if (!ObscuredPrefs.HasKey("deletedGeneratedCarID" + i))
				{
					ObscuredPrefs.SetString($"deletedGeneratedCarID{i}", carGeneratedID);
					return;
				}
			}
			for (int j = 0; j < 50; j++)
			{
				ObscuredPrefs.DeleteKey("deletedGeneratedCarID" + j);
			}
			ObscuredPrefs.SetString("deletedGeneratedCarID" + 0, carGeneratedID);
		}

		private static bool ExistsInDeletedList(string carGeneratedID)
		{
			for (int i = 0; i < 50; i++)
			{
				if (ObscuredPrefs.GetString($"deletedGeneratedCarID{i}") == carGeneratedID)
				{
					return true;
				}
			}
			return false;
		}

		public static List<int> FavoriteVehicleList()
		{
			List<int> list = new List<int>();
			int item = PlayerPrefs.GetInt("ShowFavorite", 0);
			list.Add(item);
			for (int i = 0; i < 200; i++)
			{
				if (PlayerPrefs.GetInt("Favorite" + i, 0) == 1)
				{
					list.Add(i);
				}
			}
			return list;
		}

		public static bool IsBlackVehicle()
		{
			return false;
		}
	}
}
