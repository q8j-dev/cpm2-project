using System;
using UnityEngine;

namespace CPM.Scripts.Garage
{
	[Serializable]
	public class GarageSpot
	{
		public int id;

		[HideInInspector]
		public bool isOccupied;

		public Transform spawnTransform;
	}
}
