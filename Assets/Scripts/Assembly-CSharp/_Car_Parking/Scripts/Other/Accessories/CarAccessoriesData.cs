using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Accessories
{
	[CreateAssetMenu(fileName = "CarAccessoriesData", menuName = "Scriptable/TuningLists/CarAccessoriesData", order = 1)]
	public class CarAccessoriesData : ScriptableObject
	{
		public const string WasAccessoryBoughtPrefsKey = "WasAccessoryBought";

		public const string ToSetPrefsKey = "AccessoryToSet";

		public const string CarAccessoriesPath = "Configs/ExteriorTuning/CarAccessoriesData";

		public List<OneAccessoryData> accessoriesData = new List<OneAccessoryData>();

		public int[] paidAccessorsByDefault;
	}
}
