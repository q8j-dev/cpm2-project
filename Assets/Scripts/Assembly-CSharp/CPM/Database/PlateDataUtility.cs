using System;
using System.Collections.Generic;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts;

namespace CPM.Database
{
	public static class PlateDataUtility
	{
		public const int StartingPlateCount = 6;

		private static PlateData[] _platesCache;

		private static ES3File _platesCacheSource;

		private static PlateData[] GetPlatesTable(ES3File file)
		{
			if (_platesCache != null && _platesCacheSource == file)
			{
				return _platesCache;
			}
			PlateData[] array = new PlateData[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = file.Load<PlateData>((i + 1).ToString());
			}
			_platesCache = array;
			_platesCacheSource = file;
			return array;
		}

		private static void InvalidateCache()
		{
			_platesCache = null;
			_platesCacheSource = null;
		}

		public static bool TryGetCarUidByPlateId(int plateId, out string carUid, bool isFrontPlate = false)
		{
			carUid = string.Empty;
			CheckAndCreateSaveFile();
			if (!EasySaveDataHandler.TryGetFile("plates/allData", out var file))
			{
				return false;
			}
			PlateData[] platesTable = GetPlatesTable(file);
			foreach (PlateData plateData in platesTable)
			{
				if (plateData.plateId == plateId)
				{
					string text = (isFrontPlate ? plateData.frontCarUId : plateData.rearCarUId);
					if (!string.IsNullOrEmpty(text))
					{
						carUid = text;
						return true;
					}
					return false;
				}
			}
			return false;
		}

		public static int TryGetPlateIdForCar(string carUId, bool isFrontPlate = false)
		{
			CheckAndCreateSaveFile();
			if (!EasySaveDataHandler.TryGetFile("plates/allData", out var file))
			{
				return 0;
			}
			PlateData[] platesTable = GetPlatesTable(file);
			foreach (PlateData plateData in platesTable)
			{
				if ((isFrontPlate ? plateData.frontCarUId : plateData.rearCarUId) == carUId)
				{
					return plateData.plateId;
				}
			}
			return 0;
		}

		public static void UpdatePlateData(int plateId, string dataToSet, bool isFrontPlate = false)
		{
			CheckAndCreateSaveFile();
			if (!EasySaveDataHandler.TryGetFile("plates/allData", out var file))
			{
				throw new Exception("Plates data file could not be found!");
			}
			if (!string.IsNullOrEmpty(dataToSet))
			{
				DisownOtherPlates(file, dataToSet, isFrontPlate);
			}
			PlateData plateData = null;
			string key = "";
			string[] keys = file.GetKeys();
			foreach (string text in keys)
			{
				PlateData plateData2 = file.Load<PlateData>(text);
				if (plateData2.plateId == plateId)
				{
					plateData = plateData2;
					key = text;
					break;
				}
			}
			if (plateData == null)
			{
				Debug.LogError("Null plate data");
				return;
			}
			if (isFrontPlate)
			{
				plateData.frontCarUId = dataToSet;
			}
			else
			{
				plateData.rearCarUId = dataToSet;
			}
			EasySaveDataHandler.SaveValue(plateData, file, key);
			EasySaveDataHandler.SaveFile("plates/allData");
			InvalidateCache();
		}

		private static void DisownOtherPlates(ES3File file, string dataToCheck, bool isFrontPlate)
		{
			for (int i = 1; i < 7; i++)
			{
				PlateData plateData = file.Load<PlateData>(i.ToString());
				if (isFrontPlate)
				{
					if (plateData.frontCarUId == dataToCheck)
					{
						plateData.frontCarUId = null;
						EasySaveDataHandler.SaveValue(plateData, file, i.ToString());
					}
				}
				else if (plateData.rearCarUId == dataToCheck)
				{
					plateData.rearCarUId = null;
					EasySaveDataHandler.SaveValue(plateData, file, i.ToString());
				}
			}
		}

		private static void CheckAndCreateSaveFile()
		{
			if (!EasySaveDataHandler.FileExists("plates/allData"))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				for (int i = 1; i < 7; i++)
				{
					dictionary.Add(i.ToString(), new PlateData(i));
				}
				EasySaveDataHandler.SaveValues(dictionary, "plates/allData");
				InvalidateCache();
			}
		}

		public static void SavePlates(PlatesDataForDatabase platesDataForDatabase)
		{
			if (platesDataForDatabase == null)
			{
				Debug.LogError("Empty plates data for database, So no to save.");
				return;
			}
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate1, DirectoryHelper.GetPlateVinylsPath("1"));
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate2, DirectoryHelper.GetPlateVinylsPath("2"));
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate3, DirectoryHelper.GetPlateVinylsPath("3"));
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate4, DirectoryHelper.GetPlateVinylsPath("4"));
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate5, DirectoryHelper.GetPlateVinylsPath("5"));
			PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(platesDataForDatabase.Plate6, DirectoryHelper.GetPlateVinylsPath("6"));
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (platesDataForDatabase.PlateData.Count != 6)
			{
				Debug.LogError("Plates data count is not correct, So no to save plates data");
				return;
			}
			for (int i = 1; i < 7; i++)
			{
				dictionary.Add(i.ToString(), platesDataForDatabase.PlateData[i]);
			}
			EasySaveDataHandler.SaveValues(dictionary, "plates/allData");
			InvalidateCache();
		}
	}
}
