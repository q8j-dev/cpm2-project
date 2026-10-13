using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class WaypointSelector : IDestroyable
	{
		private readonly GridData _gridData;

		private readonly TrafficWaypointsData _trafficWaypointsData;

		private SpawnWaypointSelector _spawnWaypointSelector;

		private readonly List<Vector2Int> _steamNeighbors = new List<Vector2Int>(256);

		public WaypointSelector(GridData gridData, SpawnWaypointSelector spawnWaypointSelector, TrafficWaypointsData trafficWaypointsData)
		{
			Assign();
			_gridData = gridData;
			_spawnWaypointSelector = spawnWaypointSelector;
			_trafficWaypointsData = trafficWaypointsData;
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelector)
		{
			_spawnWaypointSelector = spawnWaypointSelector;
		}

		public TrafficWaypoint GetAFreeWaypoint(Vector3 cameraPosition, int depth, VehicleTypes carType, Vector3 playerPosition, Vector3 playerDirection, bool useWaypointPriority)
		{
			CellData cell = _gridData.GetCell(cameraPosition);
			List<Vector2Int> steamNeighbors = _steamNeighbors;
			_gridData.GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, depth, justEdgeCells: false, steamNeighbors);
			for (int num = steamNeighbors.Count - 1; num >= 0; num--)
			{
				if (!_gridData.HasTrafficSpawnWaypoints(steamNeighbors[num]))
				{
					steamNeighbors.RemoveAt(num);
				}
			}
			if (steamNeighbors.Count > 0)
			{
				int num2 = ApplyNeighborSelectorMethod(steamNeighbors, playerPosition, playerDirection, carType, useWaypointPriority);
				if (num2 != -1)
				{
					return _trafficWaypointsData.GetWaypointFromIndex(num2);
				}
			}
			return null;
		}

		private int ApplyNeighborSelectorMethod(List<Vector2Int> neighbors, Vector3 playerPosition, Vector3 playerDirection, VehicleTypes carType, bool useWaypointPriority)
		{
			try
			{
				return _spawnWaypointSelector(neighbors, playerPosition, playerDirection, carType, useWaypointPriority);
			}
			catch (Exception ex)
			{
				Debug.LogError(TrafficSystemErrors.NoNeighborSelectorMethod(ex.Message));
				return DefaultDelegates.GetRandomSpawnWaypoint(neighbors, playerPosition, playerDirection, carType, useWaypointPriority);
			}
		}

		public List<int> GetAreaWaypoints(Area area)
		{
			List<int> list = new List<int>();
			CellData cell = _gridData.GetCell(area.center);
			List<Vector2Int> cellNeighbors = _gridData.GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, Mathf.CeilToInt(area.radius * 2f / (float)_gridData.GridCellSize), justEdgeCells: false);
			for (int num = cellNeighbors.Count - 1; num >= 0; num--)
			{
				cell = _gridData.GetCell(cellNeighbors[num]);
				for (int i = 0; i < cell.TrafficWaypointsData.Waypoints.Count; i++)
				{
					int num2 = cell.TrafficWaypointsData.Waypoints[i];
					if (Vector3.SqrMagnitude(area.center - _trafficWaypointsData.AllTrafficWaypoints[num2].Position) < area.sqrRadius)
					{
						list.Add(num2);
					}
				}
			}
			return list;
		}

		public TrafficWaypoint GetClosestSpawnWaypoint(Vector3 position, VehicleTypes type)
		{
			Debug.Log($"[Traffic] GetClosestSpawnWaypoint, position - {position}");
			List<SpawnWaypoint> trafficSpawnWaypoipointsAroundPosition = _gridData.GetTrafficSpawnWaypoipointsAroundPosition(position, (int)type);
			if (trafficSpawnWaypoipointsAroundPosition.Count == 0)
			{
				return null;
			}
			float num = float.MaxValue;
			int num2 = -1;
			for (int i = 0; i < trafficSpawnWaypoipointsAroundPosition.Count; i++)
			{
				float num3 = Vector3.SqrMagnitude(_trafficWaypointsData.AllTrafficWaypoints[trafficSpawnWaypoipointsAroundPosition[i].WaypointIndex].Position - position);
				if (num3 < num)
				{
					num = num3;
					num2 = trafficSpawnWaypoipointsAroundPosition[i].WaypointIndex;
				}
			}
			if (num2 != -1)
			{
				return _trafficWaypointsData.AllTrafficWaypoints[num2];
			}
			return null;
		}

		public TrafficWaypoint GetClosestWaypoint(Vector3 position)
		{
			CellData cell = _gridData.GetCell(position);
			List<Vector2Int> cellNeighbors = _gridData.GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: false);
			List<TrafficWaypoint> list = new List<TrafficWaypoint>();
			for (int i = 0; i < cellNeighbors.Count; i++)
			{
				List<int> allTrafficWaypointsInCell = _gridData.GetAllTrafficWaypointsInCell(cellNeighbors[i]);
				for (int j = 0; j < allTrafficWaypointsInCell.Count; j++)
				{
					list.Add(_trafficWaypointsData.AllTrafficWaypoints[allTrafficWaypointsInCell[j]]);
				}
			}
			TrafficWaypoint trafficWaypoint = null;
			float num = float.PositiveInfinity;
			for (int k = 0; k < list.Count; k++)
			{
				float num2 = Vector3.SqrMagnitude(position - list[k].Position);
				if (num2 < num)
				{
					trafficWaypoint = list[k];
					num = num2;
				}
			}
			if (trafficWaypoint == null)
			{
				Debug.LogWarning($"No valid waypoint found for position {position}");
			}
			return trafficWaypoint;
		}

		public TrafficWaypoint GetClosestWaypointInDirection(Vector3 position, Vector3 direction)
		{
			CellData cell = _gridData.GetCell(position);
			List<Vector2Int> cellNeighbors = _gridData.GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: false);
			List<TrafficWaypoint> list = new List<TrafficWaypoint>();
			for (int i = 0; i < cellNeighbors.Count; i++)
			{
				List<int> allTrafficWaypointsInCell = _gridData.GetAllTrafficWaypointsInCell(cellNeighbors[i]);
				for (int j = 0; j < allTrafficWaypointsInCell.Count; j++)
				{
					list.Add(_trafficWaypointsData.AllTrafficWaypoints[allTrafficWaypointsInCell[j]]);
				}
			}
			TrafficWaypoint trafficWaypoint = null;
			float num = float.PositiveInfinity;
			for (int k = 0; k < list.Count; k++)
			{
				float num2 = Vector3.SqrMagnitude(position - list[k].Position);
				if (num2 < num && CheckOrientation(list[k], direction))
				{
					trafficWaypoint = list[k];
					num = num2;
				}
			}
			if (trafficWaypoint == null)
			{
				Debug.LogWarning($"No valid waypoint found for position {position}");
			}
			return trafficWaypoint;
		}

		private bool CheckOrientation(TrafficWaypoint waypoint, Vector3 direction)
		{
			if (waypoint.Neighbors.Length < 1)
			{
				return false;
			}
			TrafficWaypoint trafficWaypoint = _trafficWaypointsData.AllTrafficWaypoints[waypoint.Neighbors[0]];
			if (Mathf.Abs(Vector3.SignedAngle(direction, trafficWaypoint.Position - waypoint.Position, Vector3.up)) < 90f)
			{
				return true;
			}
			return false;
		}

		public void OnDestroy()
		{
		}
	}
}
