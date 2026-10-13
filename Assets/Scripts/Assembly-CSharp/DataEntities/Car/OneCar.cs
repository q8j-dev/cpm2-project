using System;
using System.Collections.Generic;
using CPM.Database;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Persistence.Migration;

namespace DataEntities.Car
{
	[Serializable]
	[MigratableData]
	[Preserve]
	public class OneCar : ISaveableDataSuffix
	{
		public List<Vector3> vectors;

		public List<float> floats;

		public List<float> gears;

		public List<int> typeToInstall;

		public List<int> BoughtParts;

		public List<string> texts;

		public int CarID = -1;

		public VynilOneCar Vynils;

		public int flagID = -1;

		public int[] fsoData;

		public object dataVersion;

		public string Suffix => $"car{CarID}";

		public void Save(string path, bool deleteOld = false)
		{
			if (deleteOld)
			{
				EasySaveDataHandler.DeleteFile(path + "/" + Suffix);
			}
			EasySaveDataHandler.SaveValues(new Dictionary<string, object>
			{
				["vectors"] = vectors,
				["floats"] = floats,
				["gears"] = gears,
				["typeToInstall"] = typeToInstall,
				["BoughtParts"] = BoughtParts,
				["texts"] = texts,
				["CarID"] = CarID,
				["flagID"] = flagID,
				["fsoData"] = fsoData,
				["dataVersion"] = dataVersion
			}, path + "/" + Suffix);
		}
	}
}
