using System.Collections.Generic;
using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylPresetDictionary : ScriptableObject
	{
		public List<oneCarVinylPresetDictionary> allCarDictionary;

		public int styleCount(int carID)
		{
			foreach (oneCarVinylPresetDictionary item in allCarDictionary)
			{
				if (item.carID == carID)
				{
					return item.PresetCount;
				}
			}
			return 0;
		}
	}
}
