using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoStrobeLightsAdditionalPoints : MonoBehaviour
	{
		public bool disableThisPoints;

		public List<FsoStrobeLights.SpawnPoint> spawnPoints = new List<FsoStrobeLights.SpawnPoint>();

		private void Start()
		{
			if (disableThisPoints)
			{
				DisablePoints();
			}
			else
			{
				EnablePoints();
			}
		}

		private void EnablePoints()
		{
			if (GetComponentInParent<SellCarValues>() != null)
			{
				MonoBehaviour.print(base.name + ": it's sell car.");
				return;
			}
			VehicleData componentInParent = GetComponentInParent<VehicleData>();
			GameObject gameObject = ((!(componentInParent == null)) ? componentInParent.gameObject : V.Car);
			if (gameObject != null && gameObject.TryGetComponentInChildren<FsoStrobeLights>(out var component, includeInactive: true))
			{
				component.BodyKitSpawnPoints = spawnPoints;
			}
			else
			{
				MonoBehaviour.print(base.name + ": no fso strobe lights on start");
			}
		}

		private static void DisablePoints()
		{
			GameObject car = V.Car;
			if (car != null && car.TryGetComponentInChildren<FsoStrobeLights>(out var component, includeInactive: true))
			{
				component.BodyKitSpawnPoints = null;
			}
			else
			{
				MonoBehaviour.print("no fso destroy");
			}
		}
	}
}
