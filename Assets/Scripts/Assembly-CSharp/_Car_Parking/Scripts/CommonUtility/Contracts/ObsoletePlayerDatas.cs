using System;
using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	public class ObsoletePlayerDatas
	{
		[Obsolete("No use.")]
		public static string CreateNewCarID(int carID)
		{
			string localId = ManagerKeysForPrefs.LocalId;
			string text = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
			string text2 = "";
			for (int i = 0; i < 2; i++)
			{
				text2 += text[UnityEngine.Random.Range(0, text.Length)];
			}
			for (int j = 0; j < 3; j++)
			{
				text2 += UnityEngine.Random.Range(0, 9);
			}
			return localId + "_" + carID + text2;
		}
	}
}
