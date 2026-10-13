using System;
using System.Collections.Generic;
using CPM.Database;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.Persistence.Migration;

namespace Legacy.Database
{
	[Serializable]
	[MigratableData]
	[Preserve]
	public class OneCarLegacy
	{
		public List<Vector3> vectors;

		public List<float> floats;

		public List<float> gears;

		public List<int> typeToInstall;

		public List<int> BoughtParts;

		public List<string> texts;

		public int CarID;

		public VynilOneCar Vynils;

		public int flagID;

		public int[] fsoData;

		public OneCarLegacy()
		{
		}

		public OneCarLegacy(int carID)
		{
			CarID = carID;
			vectors = CarDataGenerator.GetCarVectors(carID);
			floats = CarDataGenerator.GetCarFloats(carID);
			gears = CarDataGenerator.GetCarGears(carID);
			BoughtParts = CarDataGenerator.GetBoughtParts(carID);
			texts = CarDataGenerator.GetCarTexts(carID);
			flagID = ObscuredPrefs.GetInt("flagToSet" + carID, -1);
			fsoData = GetFsoData(carID);
		}

		private static int[] GetFsoData(int carID)
		{
			int num = FsoStrobeLightsConfig.LegacyGetFsoPrefs(carID);
			int num2 = FsoStrobeLightsConfig.LegacyGetFsoPositionPrefs(carID);
			Color32 color = FsoStrobeLightsConfig.LegacyGetFsoColorPrefsLeft(carID);
			Color32 color2 = FsoStrobeLightsConfig.LegacyGetFsoColorPrefsRight(carID);
			return new int[8] { num, num2, color.r, color.g, color.b, color2.r, color2.g, color2.b };
		}
	}
}
