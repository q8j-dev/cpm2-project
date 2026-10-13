using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility.Contracts;

namespace _Car_Parking.Scripts.Database.Cars
{
	public static class CarDataGenerator
	{
		public const float ShininessDef = 0.7f;

		public const float SmoothnessDef = 1f;

		public static List<Vector3> GetCarVectors(int carID)
		{
			List<Vector3> list = new List<Vector3>();
			Color defaultValue = new Color(2f, 2f, 2f);
			Color color = GetColor("CarColor" + carID, defaultValue);
			Color color2 = GetColor("CarColorReflective" + carID, defaultValue);
			Color color3 = PlayerDatas.GetColor("CarColorSpecular" + carID, defaultValue);
			Color color4 = GetColor("Car" + carID + "W", defaultValue);
			Color color5 = GetColor("WheelColorReflective" + carID, defaultValue);
			Color color6 = GetColor("ColorSpecularWheel" + carID, defaultValue);
			Color color7 = GetColor("Car" + carID + "W2", defaultValue);
			Color color8 = GetColor("WheelColorReflective2" + carID, defaultValue);
			Color color9 = GetColor("ColorSpecularWheel2" + carID, defaultValue);
			Color color10 = GetColor("Car" + carID + "WB", Color.red);
			Color color11 = GetColor("CarColorWindow" + carID, defaultValue);
			Color color12 = GetColor("WindowSpecColor" + carID, defaultValue);
			float x = ObscuredPrefs.GetFloat("COGx" + carID, -2f);
			float y = ObscuredPrefs.GetFloat("COGy" + carID, -2f);
			int num = ObscuredPrefs.GetInt("rimRadius" + carID, 2);
			int num2 = ObscuredPrefs.GetInt("tireFontStyle" + carID);
			int num3 = ObscuredPrefs.GetInt("tireFont" + carID);
			Color color13 = GetColor("tireTextColor" + carID, Color.white);
			list.Add(ColorToVector3(color));
			list.Add(ColorToVector3(color2));
			list.Add(ColorToVector3(color4));
			list.Add(ColorToVector3(color5));
			list.Add(ColorToVector3(color11));
			list.Add(ColorToVector3(color12));
			list.Add(new Vector3(x, y, color11.a));
			Color color14 = GetColor("type0_C_" + carID, defaultValue);
			list.Add(new Vector3(color14.r, color14.g, color14.b));
			color14 = GetColor("type5_C_" + carID, defaultValue);
			list.Add(new Vector3(color14.r, color14.g, color14.b));
			color14 = GetColor("type6_C_" + carID, defaultValue);
			list.Add(new Vector3(color14.r, color14.g, color14.b));
			color14 = GetColor("Diod_C_" + carID, defaultValue);
			list.Add(new Vector3(color14.r, color14.g, color14.b));
			color14 = GetColor("Smoke_C_" + carID, defaultValue);
			list.Add(new Vector3(color14.r, color14.g, color14.b));
			list.Add(ColorToVector3(color3));
			list.Add(new Vector3(-2f, -2f, 0f));
			list.Add(ColorToVector3(color6));
			list.Add(new Vector3(-2f, -2f, 0f));
			list.Add(new Vector3(num3, num2, num));
			list.Add(ColorToVector3(color13));
			list.Add(ColorToVector3(color7));
			list.Add(ColorToVector3(color8));
			list.Add(ColorToVector3(color9));
			list.Add(ColorToVector3(color10));
			return list;
		}

		public static List<string> GetCarTexts(int carID)
		{
			List<string> list = new List<string>();
			string item = ObscuredPrefs.GetString("tireText" + carID);
			list.Add(item);
			string item2 = GenerateBodyKitsString(carID);
			list.Add(item2);
			string text = ObscuredPrefs.GetString("carID" + carID);
			if (text == "")
			{
				text = ObsoletePlayerDatas.CreateNewCarID(carID);
				ObscuredPrefs.SetString("carID" + carID, text);
			}
			list.Add(text);
			return list;
		}

		public static List<float> GetCarFloats(int carID)
		{
			List<float> list = new List<float>();
			float defaultValue = ObscuredPrefs.GetFloat("FsuspensionTravel" + carID, 0.22f);
			float defaultValue2 = ObscuredPrefs.GetFloat("RsuspensionTravel" + carID, 0.22f);
			float defaultValue3 = ObscuredPrefs.GetFloat("FsuspensionRate" + carID, 40000f);
			float defaultValue4 = ObscuredPrefs.GetFloat("RsuspensionRate" + carID, 40000f);
			float defaultValue5 = ObscuredPrefs.GetFloat("Fcamber" + carID);
			float defaultValue6 = ObscuredPrefs.GetFloat("Rcamber" + carID);
			float item = ObscuredPrefs.GetFloat("maxTorqueRPM" + carID);
			float item2 = ObscuredPrefs.GetFloat("maxTorque" + carID);
			float item3 = ObscuredPrefs.GetFloat("maxPower" + carID);
			float item4 = ObscuredPrefs.GetFloat("maxPowerRpm" + carID);
			float item5 = ObscuredPrefs.GetFloat("FOffset" + carID);
			float item6 = ObscuredPrefs.GetFloat("ROffset" + carID);
			float item7 = ObscuredPrefs.GetInt("CarPoliceBought" + carID);
			list.Add(item7);
			list.Add(item3);
			list.Add(item4);
			list.Add(item2);
			list.Add(item);
			list.Add(item5);
			list.Add(item6);
			item7 = ObscuredPrefs.GetFloat("FforwardGripFactor" + carID, 1f);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("GforwardGripFactor" + carID, 1f);
			list.Add(item7);
			item7 = ObscuredPrefs.GetInt("wheelToSet" + carID);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("FsuspensionTravel" + carID, defaultValue);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("RsuspensionTravel" + carID, defaultValue2);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("FsuspensionRate" + carID, defaultValue3);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("RsuspensionRate" + carID, defaultValue4);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("Fcamber" + carID, defaultValue5);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("Rcamber" + carID, defaultValue6);
			list.Add(item7);
			item7 = ObscuredPrefs.GetInt("burble" + carID);
			list.Add(item7);
			item7 = ObscuredPrefs.GetInt("turboShot" + carID);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("mileage" + carID, 2.4f);
			list.Add(item7);
			item7 = ObscuredPrefs.GetFloat("MaxSteeringAngle" + carID, 33f);
			list.Add(item7);
			float item8 = ObscuredPrefs.GetFloat("_Shininess" + carID, 0.7f);
			list.Add(item8);
			float item9 = ObscuredPrefs.GetFloat("_Smoothness" + carID, 1f);
			list.Add(item9);
			float item10 = ObscuredPrefs.GetFloat("_WShininess" + carID, 0.1f);
			list.Add(item10);
			float item11 = ObscuredPrefs.GetFloat("_WSmoothness" + carID, 0.8f);
			list.Add(item11);
			float item12 = ObscuredPrefs.GetFloat("_W2Shininess" + carID, 0.1f);
			list.Add(item12);
			float item13 = ObscuredPrefs.GetFloat("_W2Smoothness" + carID, 0.8f);
			list.Add(item13);
			list.Add(ObscuredPrefs.GetFloat("camDistance" + carID));
			list.Add(ObscuredPrefs.GetFloat("camHeight" + carID));
			list.Add(ObscuredPrefs.GetFloat("camPitch" + carID));
			list.Add(ObscuredPrefs.GetFloat("camDamp" + carID));
			list.Add(ObscuredPrefs.GetFloat("camField" + carID));
			float item14 = ObscuredPrefs.GetFloat("sizeWheelScale" + carID);
			list.Add(item14);
			return list;
		}

		public static List<float> GetCarGears(int carID)
		{
			List<float> list = new List<float>();
			for (int i = 0; i < 7; i++)
			{
				float item = PlayerPrefs.GetFloat(i + "GearRatio" + carID, -10f);
				list.Add(item);
			}
			list.Add(ObscuredPrefs.GetFloat("finalRatio" + carID, -10f));
			return list;
		}

		public static List<int> GetBoughtParts(int carID)
		{
			List<int> list = new List<int>();
			int item = ObscuredPrefs.GetInt("hasTurbo" + carID);
			list.Add(item);
			for (int i = 0; i < 35; i++)
			{
				item = PlayerPrefs.GetInt("isSet" + i + "Car" + carID, 0);
				list.Add(item);
			}
			return list;
		}

		public static Vector3 ColorToVector3(Color color)
		{
			return new Vector3(color.r, color.g, color.b);
		}

		public static Color GetColor(string name, Color defaultValue)
		{
			Color result = new Color(0f, 0f, 0f, 0f);
			result.r = PlayerPrefs.GetFloat(name + ".r", defaultValue.r);
			result.g = PlayerPrefs.GetFloat(name + ".g", defaultValue.g);
			result.b = PlayerPrefs.GetFloat(name + ".b", defaultValue.b);
			result.a = PlayerPrefs.GetFloat(name + ".a", defaultValue.a);
			return result;
		}

		public static string GenerateBodyKitsString(int carID)
		{
			int num = 300;
			string text = "";
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (ObscuredPrefs.GetInt("type" + i + "_has_" + j + "_" + carID) == 1)
					{
						text = i + "_" + j + "?" + text;
					}
				}
			}
			return text;
		}
	}
}
