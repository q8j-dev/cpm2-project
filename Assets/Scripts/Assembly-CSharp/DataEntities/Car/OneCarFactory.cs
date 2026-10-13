using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.DataEntities.Car.CarEntities;
using _Car_Parking.Scripts.Other.FsoStrobeLights;

namespace DataEntities.Car
{
	public class OneCarFactory
	{
		public const int SizesFloats = 44;

		public const int SizesVectors = 18;

		public const int SizesGears = 8;

		public const int SizesBoughtParts = 71;

		public const int SizesTypeToInstall = 9;

		public const int SizeTexts = 3;

		public const int SizeFsoData = 8;

		private ES3File cacheFile;

		public OneCarFactory(string path)
		{
			if (!EasySaveDataHandler.TryGetFile(path, out cacheFile))
			{
				Debug.LogError("No save file found at path " + path);
			}
		}

		public OneCarFactory(int carId)
		{
			EasySaveDataHandler.TryGetFile(string.Format("{0}/car{1}", "cars", carId), out cacheFile);
		}

		public OneCarFactory()
		{
		}

		public OneCar Create()
		{
			if (cacheFile == null)
			{
				return null;
			}
			OneCar obj = new OneCar
			{
				vectors = EasySaveDataHandler.GetValue<List<Vector3>>(cacheFile, "vectors"),
				floats = EasySaveDataHandler.GetValue<List<float>>(cacheFile, "floats"),
				gears = EasySaveDataHandler.GetValue<List<float>>(cacheFile, "gears"),
				typeToInstall = EasySaveDataHandler.GetValue<List<int>>(cacheFile, "typeToInstall"),
				BoughtParts = EasySaveDataHandler.GetValue<List<int>>(cacheFile, "BoughtParts"),
				texts = EasySaveDataHandler.GetValue<List<string>>(cacheFile, "texts"),
				CarID = (int)EasySaveDataHandler.GetValue<object>(cacheFile, "CarID"),
				flagID = (int)EasySaveDataHandler.GetValue<object>(cacheFile, "flagID"),
				fsoData = EasySaveDataHandler.GetValue<int[]>(cacheFile, "fsoData"),
				dataVersion = 2
			};
			CheckLength(obj);
			return obj;
		}

		public OneCar Create(BoughtCarData boughtCarData)
		{
			List<Vector3> vectors = GetVectors(boughtCarData);
			List<float> floats = GetFloats(boughtCarData);
			List<float> gears = BuyCar.GetGears(boughtCarData.PackedData, 8);
			List<int> typesToInstall = GetTypesToInstall(boughtCarData);
			List<int> boughtParts = BuyCar.GetBoughtParts(boughtCarData.PackedData, 8, 71);
			List<string> texts = GetTexts(boughtCarData);
			int carId = boughtCarData.VehicleData.CarId;
			int flagID = (int)boughtCarData.VehicleData.photonView.InstantiationData[47];
			object[] array = (object[])boughtCarData.VehicleData.photonView.InstantiationData[49];
			int[] array2 = new int[array.Length];
			array2[0] = (int)array[0];
			array2[1] = (int)array[1];
			for (int i = 2; i < array.Length; i++)
			{
				array2[i] = Convert.ToInt32(array[i]);
			}
			OneCar obj = new OneCar
			{
				vectors = vectors,
				floats = floats,
				gears = gears,
				typeToInstall = typesToInstall,
				BoughtParts = boughtParts,
				texts = texts,
				CarID = carId,
				flagID = flagID,
				fsoData = array2,
				dataVersion = 2
			};
			CheckLength(obj);
			return obj;
		}

		public OneCar CreateDefaultCar(int carId)
		{
			OneCar oneCar = CreateDefaultOneCar(carId);
			oneCar.Save("cars");
			Debug.Log("DefaultCarFileWasCreated:" + carId);
			return oneCar;
		}

		public static OneCar CreateDefaultOneCar(int carId)
		{
			List<Vector3> vectors = Enumerable.Repeat(new Vector3(2f, 2f, 2f), 18).ToList();
			List<float> list = Enumerable.Repeat(0f, 44).ToList();
			list[35] = 0.5f;
			list[10] = 0.22f;
			list[11] = 0.22f;
			list[33] = 2f;
			list[34] = 2f;
			list[39] = 1f;
			list[40] = 1f;
			list[41] = 1f;
			list[42] = 1f;
			List<float> gears = Enumerable.Repeat(-10f, 8).ToList();
			List<int> typeToInstall = Enumerable.Repeat(-2, 9).ToList();
			List<int> list2 = Enumerable.Repeat(0, 71).ToList();
			list2[0] = -1;
			List<string> list3 = Enumerable.Repeat("", 3).ToList();
			string value = ObsoletePlayerDatas.CreateNewCarID(carId);
			ObscuredPrefs.SetString("carID" + carId, value);
			list3[2] = value;
			int carID = carId;
			int flagID = -1;
			int[] defaultFsoData = FsoStrobeLightsConfig.DefaultFsoData;
			OneCar obj = new OneCar
			{
				vectors = vectors,
				floats = list,
				gears = gears,
				typeToInstall = typeToInstall,
				BoughtParts = list2,
				texts = list3,
				CarID = carID,
				flagID = flagID,
				fsoData = defaultFsoData,
				dataVersion = 2
			};
			CheckLength(obj);
			return obj;
		}

		public static void ResetCar(int carID, GameObject car)
		{
		}

		private List<string> GetTexts(BoughtCarData boughtCarData)
		{
			List<string> obj = new List<string>
			{
				boughtCarData.Mpcc.tireText,
				""
			};
			string text = boughtCarData.VehicleData.generatedID;
			if (string.IsNullOrEmpty(text))
			{
				text = ObsoletePlayerDatas.CreateNewCarID(boughtCarData.VehicleData.CarId);
			}
			obj.Add(text);
			return obj;
		}

		private static List<int> GetTypesToInstall(BoughtCarData boughtCarData)
		{
			return boughtCarData.BodyKits.TypesToIntall.ToList();
		}

		private List<float> GetFloats(BoughtCarData boughtCarData)
		{
			List<float> list = new List<float>();
			list.Add(boughtCarData.Mpcc.Shininess);
			list.Add(boughtCarData.Mpcc.Smoothness);
			list.Add(boughtCarData.Mpcc.WShininess);
			list.Add(boughtCarData.Mpcc.WSmoothness);
			list.Add(boughtCarData.Mpcc.W2Shininess);
			list.Add(boughtCarData.Mpcc.W2Smoothness);
			list.AddRange(new float[5]);
			list.Add(boughtCarData.VehicleData.sizeWheelScale);
			list.Add(boughtCarData.Mpcc.cog.x);
			list.Add(boughtCarData.Mpcc.cog.y);
			list.Add(boughtCarData.Mpcc.WindowColor.a);
			float item = boughtCarData.Mpcc.tireFont;
			float item2 = boughtCarData.Mpcc.tireFontStyle;
			float item3 = boughtCarData.Mpcc.rimRadius;
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(boughtCarData.Axles.FrontAxle.RightWheel.Wheel.TyreHealth);
			list.Add(boughtCarData.Axles.FrontAxle.LeftWheel.Wheel.TyreHealth);
			list.Add(boughtCarData.Axles.RearAxle.RightWheel.Wheel.TyreHealth);
			list.Add(boughtCarData.Axles.RearAxle.LeftWheel.Wheel.TyreHealth);
			return list;
		}

		private List<Vector3> GetVectors(BoughtCarData boughtCarData)
		{
			return new List<Vector3>
			{
				boughtCarData.Mpcc.CarColor.ToVector3(),
				boughtCarData.Mpcc.CarColorReflective.ToVector3(),
				boughtCarData.Mpcc.WheelColor.ToVector3(),
				boughtCarData.Mpcc.WheelColorReflective.ToVector3(),
				boughtCarData.Mpcc.WindowColor.ToVector3(),
				boughtCarData.Mpcc.WindowColorReflective.ToVector3(),
				boughtCarData.BodyKits.colors[0].ToVector3(),
				boughtCarData.BodyKits.colors[1].ToVector3(),
				boughtCarData.BodyKits.colors[2].ToVector3(),
				boughtCarData.Mpcc.diodColor.ToVector3()
			};
		}

		public static void CheckLength(OneCar oneCar)
		{
			CheckListLength(oneCar.floats, 44);
			CheckListLength(oneCar.gears, 8);
			CheckListLength(oneCar.texts, 3);
			CheckListLength(oneCar.vectors, 18);
			CheckListLength(oneCar.BoughtParts, 71);
			CheckListLength(oneCar.typeToInstall, 9);
		}

		private static void CheckListLength<T>(List<T> list, int size)
		{
			if (list == null)
			{
				list = new List<T>();
			}
			list.Resize2(size);
		}

		public static void CheckFirstCar(int carId)
		{
			if (!CheckCarFileExists(carId))
			{
				new OneCarFactory().CreateDefaultCar(carId);
			}
		}

		public static bool CheckCarFileExists(int carId)
		{
			ES3File file;
			return EasySaveDataHandler.TryGetFile(string.Format("{0}/car{1}", "cars", carId), out file);
		}
	}
}
