using System;
using CPM.Database;
using CPM.Database.Constructors;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.Other.FsoStrobeLights;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	public class ReceiveFromDatabase
	{
		public enum DatabaseSourceType
		{
			WORLD_SALE = 0,
			USER = 1
		}

		public static void SaveOneCarFormDataBase(OneCarOld oneCar, ICarInfoService carInfoService, DatabaseSourceType databaseSourceType = DatabaseSourceType.USER)
		{
			CarInfo fromSlot = carInfoService.GetFromSlot(V.SlotID);
			FsoDataFileMediator fsoDataFileMediator = new FsoDataFileMediator(fromSlot);
			if (oneCar.CarID != 0)
			{
				ObscuredPrefs.SetInt("car" + oneCar.CarID, 1);
			}
			if (oneCar.floats == null)
			{
				return;
			}
			float[] array = new float[oneCar.floats.Count];
			int num = 0;
			foreach (float @float in oneCar.floats)
			{
				array[num] = @float;
				num++;
			}
			if (array.Length == 0)
			{
				return;
			}
			int carID = oneCar.CarID;
			if (databaseSourceType == DatabaseSourceType.WORLD_SALE)
			{
				int flagId = fromSlot.AdditionalData.FlagId;
				if (flagId > -1)
				{
					FlagConstructor.AddFlag(flagId);
				}
			}
			try
			{
				if (oneCar.fsoData != null && oneCar.fsoData.Length != 0)
				{
					fsoDataFileMediator.SetFsoIndex(oneCar.fsoData[0]);
					fsoDataFileMediator.SetFsoPosition(oneCar.fsoData[1]);
					Color color = new Color((float)(int)(byte)oneCar.fsoData[2] / 255f, (float)(int)(byte)oneCar.fsoData[3] / 255f, (float)(int)(byte)oneCar.fsoData[4] / 255f);
					Color colorRight = color;
					if (oneCar.fsoData.Length > 5)
					{
						colorRight = new Color((float)(int)(byte)oneCar.fsoData[5] / 255f, (float)(int)(byte)oneCar.fsoData[6] / 255f, (float)(int)(byte)oneCar.fsoData[7] / 255f);
					}
					fsoDataFileMediator.SetFsoColor(color, colorRight);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception: " + ex);
			}
			ObscuredPrefs.SetInt("car" + carID, 1);
			ObscuredPrefs.SetInt("CarPoliceBought" + carID, (int)array[0]);
			if (array[1] != 0f && array[1] < 1700f)
			{
				ObscuredPrefs.SetFloat("maxPower" + carID, array[1]);
			}
			if (array[2] != 0f)
			{
				ObscuredPrefs.SetFloat("maxPowerRpm" + carID, array[2]);
			}
			if (array[3] != 0f && array[3] < 2900f)
			{
				ObscuredPrefs.SetFloat("maxTorque" + carID, array[3]);
			}
			if (array[4] != 0f)
			{
				ObscuredPrefs.SetFloat("maxTorqueRPM" + carID, array[4]);
			}
			ObscuredPrefs.SetFloat("FOffset" + carID, array[5]);
			ObscuredPrefs.SetFloat("ROffset" + carID, array[6]);
			ObscuredPrefs.SetFloat("FforwardGripFactor" + carID, array[7]);
			ObscuredPrefs.SetFloat("GforwardGripFactor" + carID, array[8]);
			if (array[9] != 0f)
			{
				ObscuredPrefs.SetInt("wheelToSet" + carID, (int)array[9]);
			}
			ObscuredPrefs.SetFloat("FsuspensionTravel" + carID, array[10]);
			ObscuredPrefs.SetFloat("RsuspensionTravel" + carID, array[11]);
			ObscuredPrefs.SetFloat("FsuspensionRate" + carID, array[12]);
			ObscuredPrefs.SetFloat("RsuspensionRate" + carID, array[13]);
			ObscuredPrefs.SetFloat("Fcamber" + carID, array[14]);
			ObscuredPrefs.SetFloat("Rcamber" + carID, array[15]);
			if (array[16] != 0f)
			{
				ObscuredPrefs.SetFloat("shiftTime" + carID, array[16]);
			}
			ObscuredPrefs.SetInt("burble" + carID, (int)array[17]);
			ObscuredPrefs.SetInt("turboShot" + carID, (int)array[18]);
			if (array.Length > 19)
			{
				ObscuredPrefs.SetFloat("mileage" + carID, array[19]);
			}
			if (array.Length > 20)
			{
				ObscuredPrefs.SetFloat("MaxSteeringAngle" + carID, array[20]);
			}
			if (array.Length > 21)
			{
				ObscuredPrefs.SetFloat("_Shininess" + carID, array[21]);
				ObscuredPrefs.SetFloat("_Smoothness" + carID, array[22]);
				ObscuredPrefs.SetFloat("_WShininess" + carID, array[23]);
				ObscuredPrefs.SetFloat("_WSmoothness" + carID, array[24]);
				ObscuredPrefs.SetFloat("_W2Shininess" + carID, array[25]);
				ObscuredPrefs.SetFloat("_W2Smoothness" + carID, array[26]);
			}
			if (array.Length > 27)
			{
				ObscuredPrefs.SetFloat("camDistance" + carID, array[27]);
				ObscuredPrefs.SetFloat("camHeight" + carID, array[28]);
				ObscuredPrefs.SetFloat("camPitch" + carID, array[29]);
				ObscuredPrefs.SetFloat("camDamp" + carID, array[30]);
				ObscuredPrefs.SetFloat("camField" + carID, array[31]);
			}
			if (oneCar.vectors == null)
			{
				return;
			}
			Vector3[] array2 = oneCar.vectors.ToArray();
			if (array2.Length == 0)
			{
				return;
			}
			new Color(2f, 2f, 2f);
			if (array2[0].x != 2f)
			{
				PlayerDatas.SetColor("CarColor" + carID, PlayerDatas.VectorToColor(array2[0]));
			}
			if (array2[1].x != 2f)
			{
				PlayerDatas.SetColor("CarColorReflective" + carID, PlayerDatas.VectorToColor(array2[1]));
			}
			if (array2[2].x != 2f)
			{
				PlayerDatas.SetColor("Car" + carID + "W", PlayerDatas.VectorToColor(array2[2]));
			}
			if (array2[3].x != 2f)
			{
				PlayerDatas.SetColor("WheelColorReflective" + carID, PlayerDatas.VectorToColor(array2[3]));
			}
			if (array2[4].x != 2f)
			{
				Color color2 = PlayerDatas.VectorToColor(array2[4]);
				color2.a = array2[6].z;
				PlayerDatas.SetColor("CarColorWindow" + carID, color2);
			}
			if (array2[5].x != 2f)
			{
				PlayerDatas.SetColor("WindowSpecColor" + carID, PlayerDatas.VectorToColor(array2[5]));
			}
			if (array2[6].x != -2f)
			{
				ObscuredPrefs.SetFloat("COGx" + carID, array2[6].x);
				ObscuredPrefs.SetFloat("COGy" + carID, array2[6].y);
			}
			if (array2[7].x != 2f)
			{
				PlayerDatas.SetColor("type0_C_" + carID, PlayerDatas.VectorToColor(array2[7]));
			}
			if (array2[8].x != 2f)
			{
				PlayerDatas.SetColor("type5_C_" + carID, PlayerDatas.VectorToColor(array2[8]));
			}
			if (array2[9].x != 2f)
			{
				PlayerDatas.SetColor("type6_C_" + carID, PlayerDatas.VectorToColor(array2[9]));
			}
			if (array2[10].x != 2f)
			{
				PlayerDatas.SetColor("Diod_C_" + carID, PlayerDatas.VectorToColor(array2[10]));
			}
			if (array2.Length > 11 && array2[11].x != 2f)
			{
				PlayerDatas.SetColor("Smoke_C_" + carID, PlayerDatas.VectorToColor(array2[11]));
			}
			if (array2.Length > 12)
			{
				if (array2[12].x != 2f)
				{
					PlayerDatas.SetColor("CarColorSpecular" + carID, PlayerDatas.VectorToColor(array2[12]));
				}
				_ = array2[13].x;
				float y = array2[13].y;
				_ = -2f;
				_ = -2f;
				if (array2[14].x != 2f)
				{
					PlayerDatas.SetColor("ColorSpecularWheel" + carID, PlayerDatas.VectorToColor(array2[14]));
				}
				_ = array2[15].x;
				float y2 = array2[15].y;
				_ = -2f;
				_ = -2f;
			}
			if (array2.Length > 16)
			{
				ObscuredPrefs.SetInt("tireFont" + carID, (int)array2[16].x);
				ObscuredPrefs.SetInt("tireFontStyle" + carID, (int)array2[16].y);
				ObscuredPrefs.SetInt("rimRadius" + carID, (int)array2[16].z);
				PlayerDatas.SetColor("tireTextColor" + carID, PlayerDatas.VectorToColor(array2[17]));
			}
			if (array2.Length > 18)
			{
				PlayerDatas.SetColor("Car" + carID + "W2", PlayerDatas.VectorToColor(array2[18]));
				PlayerDatas.SetColor("WheelColorReflective2" + carID, PlayerDatas.VectorToColor(array2[19]));
				PlayerDatas.SetColor("ColorSpecularWheel2" + carID, PlayerDatas.VectorToColor(array2[20]));
			}
			float[] array3 = oneCar.gears.ToArray();
			for (int i = 0; i < 10; i++)
			{
				if (array3[i] != -10f)
				{
					PlayerPrefs.SetFloat(i + "GearRatio" + carID, array3[i]);
				}
			}
			if (array3[10] != -10f)
			{
				ObscuredPrefs.SetFloat("finalRatio" + carID, array3[10]);
			}
			if (oneCar.texts != null && oneCar.texts.Count > 0)
			{
				string[] array4 = oneCar.texts.ToArray();
				ObscuredPrefs.SetString("tireText" + carID, array4[0]);
				if (array4.Length > 1)
				{
					saveBodyKits(array4[1], carID);
				}
				if (array4.Length > 2)
				{
					string text = array4[2];
					if (text.Trim() != "")
					{
						ObscuredPrefs.SetString("carID" + carID, text);
					}
				}
			}
			int[] array5 = oneCar.typeToInstall.ToArray();
			int num2 = 0;
			int[] array6 = array5;
			foreach (int num3 in array6)
			{
				if (num3 != -2)
				{
					ObscuredPrefs.SetInt("type" + num2 + "_" + carID, num3);
				}
				num2++;
			}
			int[] array7 = oneCar.BoughtParts.ToArray();
			ObscuredPrefs.SetInt("hasTurbo" + carID, array7[0]);
			for (int k = 0; k < 35; k++)
			{
				PlayerPrefs.SetInt("isSet" + k + "Car" + carID, array7[k + 1]);
				PlayerPrefs.SetInt(k + "_" + carID, array7[k + 1]);
			}
		}

		public static void saveBodyKits(string input, int carID)
		{
			string[] array = input.Split("?"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split("_"[0]);
				if (array2.Length >= 2)
				{
					int num = int.Parse(array2[0]);
					int num2 = int.Parse(array2[1]);
					ObscuredPrefs.SetInt("type" + num + "_has_" + num2 + "_" + carID, 1);
				}
			}
		}
	}
}
