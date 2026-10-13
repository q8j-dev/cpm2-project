using System.Collections.Generic;
using CPM.Database;
using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	[CreateAssetMenu(fileName = "VinylsPreset", menuName = "Scriptable/VinylsPreset")]
	public class VinylsPreset : ScriptableObject
	{
		public List<VynilOneCar> vynilOneCarList;

		public int CarID;

		public List<int> Price = new List<int>();
	}
}
