using System;
using UnityEngine;

namespace Gley.TrafficSystem
{
	[Serializable]
	public class CarType
	{
		[SerializeField]
		private GameObject vehiclePrefab = default;

		[SerializeField]
		[Range(1f, 100f)]
		private int percent;

		[SerializeField]
		private bool ignore = default;

		public GameObject VehiclePrefab => vehiclePrefab;

		public int Percent => percent;

		public bool Ignore => ignore;

		public CarType()
		{
			percent = 1;
		}
	}
}
