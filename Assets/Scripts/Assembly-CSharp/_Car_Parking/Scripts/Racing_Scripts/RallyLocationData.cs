using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public class RallyLocationData
	{
		public int Id;

		public string NameKey;

		public string AnalyticsId;

		public Sprite LocationLogo;

		public V.placeToSpawn PlaceToSpawn;

		public RallyMapTrajectorySO TrajectoryData;
	}
}
