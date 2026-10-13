using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Database.Account
{
	public class AdditionalDataSaver
	{
		private const int PlayTimeCheckStep = 15;

		private const int PurchasedLength = 13;

		private const int MainLength = 11;

		private const int CameraLength = 5;

		public static void SaveMainAndOther(AdditionalPlayerProgress otherData)
		{
			SaveMain(otherData.Main);
			SavePurchased(otherData.Purchased);
			SaveCamera(otherData.Camera);
		}

		private static int RoundTime(float origin)
		{
			return (int)origin / 15 * 15;
		}

		private static void SaveMain(float[] array)
		{
			SafeArray<float> safeArray = new SafeArray<float>(array);
			if (safeArray.RealArray != null && safeArray.Length != 0)
			{
				ObscuredPrefs.SetInt("lastBoughtId", (int)safeArray[0]);
				if ((int)safeArray[1] != -1)
				{
					ObscuredPrefs.SetInt("LastCarId", (int)safeArray[1]);
				}
				AllTimeGamePlayUtil.Set((int)safeArray[2]);
				ObscuredPrefs.SetFloat("ana_multiFree", (int)safeArray[3]);
				ObscuredPrefs.SetFloat("ana_singleFree", (int)safeArray[4]);
				ObscuredPrefs.SetFloat("priceSold", Mathf.Clamp(safeArray[5], 0f, 50000000f));
				ObscuredPrefs.SetInt("RaceWin", (int)safeArray[6]);
				ObscuredPrefs.SetInt("RaceLose", (int)safeArray[7]);
				ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "_coinPurchaseCount", (int)safeArray[8]);
				ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, (int)safeArray[9]);
				PlayerTotalDistance.Set(safeArray[10]);
			}
		}

		public static float[] GenerateMain()
		{
			return new float[11]
			{
				ObscuredPrefs.GetInt("lastBoughtId"),
				ObscuredPrefs.GetInt("LastCarId", -1),
				RoundTime(AllTimeGamePlayUtil.Get()),
				RoundTime(ObscuredPrefs.GetFloat("ana_multiFree")),
				RoundTime(ObscuredPrefs.GetFloat("ana_singleFree")),
				ObscuredPrefs.GetFloat("priceSold"),
				ObscuredPrefs.GetInt("RaceWin"),
				ObscuredPrefs.GetInt("RaceLose"),
				ObscuredPrefs.GetInt(SystemInfo.deviceUniqueIdentifier + "_coinPurchaseCount"),
				ObscuredPrefs.GetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version),
				PlayerTotalDistance.Get()
			};
		}

		public static int[] GeneratePurchased()
		{
			return new int[13]
			{
				ObscuredPrefs.GetInt("endlessCrashTime"),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 1),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 2),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 3),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 4),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 5),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 6),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 7),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 8),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 9),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 10),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 11),
				ObscuredPrefs.GetInt("FastFoodBuilding" + 12)
			};
		}

		private static void SavePurchased(int[] array)
		{
			SafeArray<int> safeArray = new SafeArray<int>(array);
			if (safeArray.RealArray != null && safeArray.Length != 0)
			{
				ObscuredPrefs.SetInt("endlessCrashTime", safeArray[0]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 1, safeArray[1]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 2, safeArray[2]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 3, safeArray[3]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 4, safeArray[4]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 5, safeArray[5]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 6, safeArray[6]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 7, safeArray[7]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 8, safeArray[8]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 9, safeArray[9]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 10, safeArray[10]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 11, safeArray[11]);
				ObscuredPrefs.SetInt("FastFoodBuilding" + 12, safeArray[12]);
			}
		}

		private static void SaveCamera(float[] array)
		{
			SafeArray<float> safeArray = new SafeArray<float>(array);
			if (safeArray.RealArray != null && safeArray.Length != 0)
			{
				ObscuredPrefs.SetFloat("camDistance", safeArray[0]);
				ObscuredPrefs.SetFloat("camHeight", safeArray[1]);
				ObscuredPrefs.SetFloat("camPitch", safeArray[2]);
				ObscuredPrefs.SetFloat("camDamp", safeArray[3]);
				ObscuredPrefs.SetFloat("camField", safeArray[4]);
			}
		}

		public static float[] GenerateCamera()
		{
			return new float[5]
			{
				ObscuredPrefs.GetFloat("camDistance"),
				ObscuredPrefs.GetFloat("camHeight"),
				ObscuredPrefs.GetFloat("camPitch"),
				ObscuredPrefs.GetFloat("camDamp"),
				ObscuredPrefs.GetFloat("camField")
			};
		}
	}
}
