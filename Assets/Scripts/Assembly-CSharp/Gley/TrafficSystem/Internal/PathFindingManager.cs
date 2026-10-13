using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class PathFindingManager
	{
		private readonly GridData _gridData;

		private readonly PathFindingData _trafficPathFindingData;

		private readonly AStar _aStar;

		public PathFindingManager(GridData gridData, PathFindingData trafficPathFindingData)
		{
			_gridData = gridData;
			_trafficPathFindingData = trafficPathFindingData;
			_aStar = new AStar();
		}

		public List<int> GetPathToDestination(int vehicleIndex, int currentWaypointIndex, Vector3 position, VehicleTypes vehicleType)
		{
			if (currentWaypointIndex < 0)
			{
				Debug.LogWarning($"Cannot find route to destination. Vehicle at index {vehicleIndex} is disabled or has an invalid target waypoint");
				return null;
			}
			int closestPathFindingWaypoint = GetClosestPathFindingWaypoint(position, (int)vehicleType);
			if (closestPathFindingWaypoint < 0)
			{
				Debug.LogWarning("No waypoint found closer to destination");
				return null;
			}
			List<int> list = _aStar.FindPath(currentWaypointIndex, closestPathFindingWaypoint, (int)vehicleType, _trafficPathFindingData.AllPathFindingWaypoints);
			if (list != null)
			{
				return list;
			}
			Debug.LogWarning($"No path found for vehicle {vehicleIndex} to {position}");
			return null;
		}

		public List<int> GetPath(Vector3 startPosition, Vector3 endPosition, VehicleTypes vehicleType)
		{
			int closestPathFindingWaypoint = GetClosestPathFindingWaypoint(startPosition, (int)vehicleType);
			if (closestPathFindingWaypoint == -1)
			{
				Debug.LogWarning($"No traffic waypoint found close to {startPosition}");
				return null;
			}
			int closestPathFindingWaypoint2 = GetClosestPathFindingWaypoint(endPosition, (int)vehicleType);
			if (closestPathFindingWaypoint2 == -1)
			{
				Debug.LogWarning($"No traffic waypoint found closed to {endPosition}");
				return null;
			}
			List<int> list = _aStar.FindPath(closestPathFindingWaypoint, closestPathFindingWaypoint2, (int)vehicleType, _trafficPathFindingData.AllPathFindingWaypoints);
			if (list == null)
			{
				Debug.LogWarning($"No path found from {startPosition} to {endPosition}");
			}
			return list;
		}

		private int GetClosestPathFindingWaypoint(Vector3 position, int type)
		{
			List<int> trafficWaypointsAroundPosition = _gridData.GetTrafficWaypointsAroundPosition(position);
			if (trafficWaypointsAroundPosition.Count == 0)
			{
				return -1;
			}
			float num = float.MaxValue;
			int result = -1;
			foreach (int item in trafficWaypointsAroundPosition)
			{
				if (_trafficPathFindingData.GetAllowedAgents(item).Contains(type))
				{
					float num2 = Vector3.SqrMagnitude(_trafficPathFindingData.AllPathFindingWaypoints[item].WorldPosition - position);
					if (num2 < num)
					{
						num = num2;
						result = item;
					}
				}
			}
			return result;
		}
	}
}
