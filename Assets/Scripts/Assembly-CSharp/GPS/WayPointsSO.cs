using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GPS
{
	[CreateAssetMenu(fileName = "WayPoints", menuName = "Scriptable/WayPoints", order = 1)]
	public class WayPointsSO : SerializedScriptableObject
	{
		[NonSerialized]
		[OdinSerialize]
		public Dictionary<int, Grid> grid = new Dictionary<int, Grid>();

		public List<WayPointNode> nodes = new List<WayPointNode>();

		public WayPointNode this[uint u] => nodes[(int)u];

		public WayPointNode this[int u] => nodes[u];

		public void AddAreaID(uint areaID, int i)
		{
			if (!grid.ContainsKey(i))
			{
				grid[i] = new Grid();
			}
			grid[i].Add(areaID);
		}
	}
}
