using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class DefaultDelegates
	{
		public static int GetRandomSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority)
		{
			return GetPossibleWaypoint(neighbors[Random.Range(0, neighbors.Count)], vehicleType, useWaypointPriority);
		}

		public static int GetForwardSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority)
		{
			Vector2Int selectedNeighbor = Vector2Int.zero;
			float num = 180f;
			for (int i = 0; i < neighbors.Count; i++)
			{
				float num2 = Vector3.Angle(API.GetGridData().GetCellPosition(neighbors[i]) - position, direction);
				if (num2 < num)
				{
					selectedNeighbor = neighbors[i];
					num = num2;
				}
			}
			return GetPossibleWaypoint(selectedNeighbor, vehicleType, useWaypointPriority);
		}

		private static int GetPossibleWaypoint(Vector2Int selectedNeighbor, VehicleTypes vehicleType, bool usePriority)
		{
			List<SpawnWaypoint> trafficSpawnWaypointsForCell = API.GetGridData().GetTrafficSpawnWaypointsForCell(selectedNeighbor, (int)vehicleType);
			if (trafficSpawnWaypointsForCell.Count > 0)
			{
				if (!usePriority)
				{
					return trafficSpawnWaypointsForCell[Random.Range(0, trafficSpawnWaypointsForCell.Count)].WaypointIndex;
				}
				int num = 0;
				foreach (SpawnWaypoint item in trafficSpawnWaypointsForCell)
				{
					num += item.Priority;
				}
				int num2 = Random.Range(1, num);
				num = 0;
				for (int i = 0; i < trafficSpawnWaypointsForCell.Count; i++)
				{
					num += trafficSpawnWaypointsForCell[i].Priority;
					if (num >= num2)
					{
						return trafficSpawnWaypointsForCell[i].WaypointIndex;
					}
				}
			}
			return -1;
		}

		public static void TrafficLightBehaviour(TrafficLightsColor currentRoadColor, GameObject[] redLightObjects, GameObject[] yellowLightObjects, GameObject[] greenLightObjects, string name)
		{
			switch (currentRoadColor)
			{
			case TrafficLightsColor.Red:
				SetLight(active: true, redLightObjects, name);
				SetLight(active: false, yellowLightObjects, name);
				SetLight(active: false, greenLightObjects, name);
				break;
			case TrafficLightsColor.YellowRed:
			case TrafficLightsColor.YellowGreen:
				SetLight(active: false, redLightObjects, name);
				SetLight(active: true, yellowLightObjects, name);
				SetLight(active: false, greenLightObjects, name);
				break;
			case TrafficLightsColor.Green:
				SetLight(active: false, redLightObjects, name);
				SetLight(active: false, yellowLightObjects, name);
				SetLight(active: true, greenLightObjects, name);
				break;
			}
		}

		private static void SetLight(bool active, GameObject[] lightObjects, string name)
		{
			for (int i = 0; i < lightObjects.Length; i++)
			{
				if (lightObjects[i] != null)
				{
					if (lightObjects[i].activeSelf != active)
					{
						lightObjects[i].SetActive(active);
					}
				}
				else
				{
					Debug.LogWarning("Intersection " + name + " has null red light objects");
				}
			}
		}

		public static void TriggerSizeModifier(float currentSpeed, BoxCollider frontCollider, float maxSpeed, float minTriggerLength, float maxTriggerLength)
		{
			float num = 20f;
			if (currentSpeed < num)
			{
				frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, minTriggerLength);
				frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, minTriggerLength / 2f);
			}
			else if (currentSpeed >= maxSpeed)
			{
				frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, maxTriggerLength);
				frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, maxTriggerLength / 2f);
			}
			else
			{
				float num2 = minTriggerLength + (currentSpeed - num) * ((maxTriggerLength - minTriggerLength) / (maxSpeed - num));
				frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, num2);
				frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, num2 / 2f);
			}
		}
	}
}
