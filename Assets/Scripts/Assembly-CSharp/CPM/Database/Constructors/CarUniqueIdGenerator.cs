using System;
using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace CPM.Database.Constructors
{
	public static class CarUniqueIdGenerator
	{
		public static string CreateNewCarID(int carID)
		{
			string localId = ManagerKeysForPrefs.LocalId;
			string text = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
			string text2 = ((int)DateTime.Now.TimeOfDay.TotalSeconds).ToString();
			string text3 = "";
			for (int i = 0; i < 2; i++)
			{
				text3 += text[UnityEngine.Random.Range(0, text.Length)];
			}
			for (int j = 0; j < 3; j++)
			{
				text3 += UnityEngine.Random.Range(0, 9);
			}
			return "car" + carID + "_" + localId + "_" + text3 + "_" + text2;
		}

		public static bool IsCarInstanceIdCorrect(string carInstanceId, int carID)
		{
			return true;
		}
	}
}
