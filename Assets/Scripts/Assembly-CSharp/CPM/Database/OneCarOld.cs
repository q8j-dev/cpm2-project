using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Database
{
	[Serializable]
	[Preserve]
	public class OneCarOld
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

		public OneCarOld()
		{
		}

		public OneCarOld(int carID, DataBase dataBase)
		{
			CarID = carID;
		}
	}
}
