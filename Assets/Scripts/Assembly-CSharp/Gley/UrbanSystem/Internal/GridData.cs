using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class GridData : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField]
		private int _gridCellSize;

		[SerializeField]
		private Vector3 _gridCorner;

		[SerializeField]
		private int _gridRows;

		[SerializeField]
		private int _gridColumns;

		[SerializeField]
		private CellData[] _sparseCells;

		private RowData[] _grid;

		public RowData[] Grid => _grid;

		public int GridCellSize => _gridCellSize;

		public Vector3 GridCorner
		{
			get
			{
				return _gridCorner;
			}
			set
			{
				_gridCorner = value;
			}
		}

		public void SetGridData(RowData[] grid, Vector3 gridCorner, int gridCellSize)
		{
			_grid = grid;
			_gridCorner = gridCorner;
			_gridCellSize = gridCellSize;
			_gridRows = ((grid != null) ? grid.Length : 0);
			_gridColumns = ((grid != null && grid.Length != 0) ? grid[0].Row.Length : 0);
		}

		public void OnBeforeSerialize()
		{
			if (_grid == null || _grid.Length == 0)
			{
				return;
			}
			_gridRows = _grid.Length;
			_gridColumns = _grid[0].Row.Length;
			List<CellData> list = new List<CellData>();
			for (int i = 0; i < _grid.Length; i++)
			{
				for (int j = 0; j < _grid[i].Row.Length; j++)
				{
					CellData cellData = _grid[i].Row[j];
					if (cellData.TrafficWaypointsData.HasWaypoints || cellData.TrafficWaypointsData.HasSpawnWaypoints || cellData.PedestrianWaypointsData.HasWaypoints || cellData.PedestrianWaypointsData.HasSpawnWaypoints || cellData.IntersectionsInCell.Count > 0)
					{
						list.Add(cellData);
					}
				}
			}
			_sparseCells = list.ToArray();
		}

		public void OnAfterDeserialize()
		{
			if (_gridRows <= 0 || _gridColumns <= 0)
			{
				return;
			}
			_grid = new RowData[_gridRows];
			for (int i = 0; i < _gridRows; i++)
			{
				CellData[] array = new CellData[_gridColumns];
				for (int j = 0; j < _gridColumns; j++)
				{
					Vector3 center = new Vector3(_gridCorner.x + (float)(j * _gridCellSize) + (float)_gridCellSize / 2f, 0f, _gridCorner.z + (float)(i * _gridCellSize) + (float)_gridCellSize / 2f);
					array[j] = new CellData(new CellProperties(j, i, center), new CellWaypointsData(new List<int>(), new List<SpawnWaypoint>(), hasWaypoints: false, hasSpawnWaypoints: false), new CellWaypointsData(new List<int>(), new List<SpawnWaypoint>(), hasWaypoints: false, hasSpawnWaypoints: false), new List<int>());
				}
				_grid[i] = new RowData(array);
			}
			if (_sparseCells == null)
			{
				return;
			}
			for (int k = 0; k < _sparseCells.Length; k++)
			{
				CellData cellData = _sparseCells[k];
				int row = cellData.CellProperties.Row;
				int column = cellData.CellProperties.Column;
				if (row >= 0 && row < _gridRows && column >= 0 && column < _gridColumns)
				{
					_grid[row].Row[column] = cellData;
					cellData.CellProperties.Center = new Vector3(_gridCorner.x + (float)(column * _gridCellSize) + (float)_gridCellSize / 2f, 0f, _gridCorner.z + (float)(row * _gridCellSize) + (float)_gridCellSize / 2f);
				}
			}
			_sparseCells = null;
		}

		public bool IsValid(out string error)
		{
			error = string.Empty;
			if (_grid == null)
			{
				error = UrbanSystemErrors.SceneDataIsNull;
				return false;
			}
			if (_grid.Length == 0)
			{
				error = UrbanSystemErrors.SceneGridIsNull;
				return false;
			}
			return true;
		}

		public CellData GetCell(Vector2Int cellIndex)
		{
			return Grid[cellIndex.x].Row[cellIndex.y];
		}

		public CellData GetCell(Vector3 position)
		{
			return GetCell(position.x, position.z);
		}

		public CellData GetCell(float xPoz, float zPoz)
		{
			int value = Mathf.FloorToInt(Mathf.Abs((GridCorner.z - zPoz) / (float)GridCellSize));
			int value2 = Mathf.FloorToInt(Mathf.Abs((GridCorner.x - xPoz) / (float)GridCellSize));
			value = Mathf.Clamp(value, 0, Grid.Length - 1);
			value2 = Mathf.Clamp(value2, 0, Grid[value].Row.Length - 1);
			return Grid[value].Row[value2];
		}

		public Vector2Int GetCellIndex(Vector3 position)
		{
			int num = Mathf.FloorToInt(Mathf.Abs((GridCorner.z - position.z) / (float)GridCellSize));
			int num2 = Mathf.FloorToInt(Mathf.Abs((GridCorner.x - position.x) / (float)GridCellSize));
			if (num < 0 || num >= Grid.Length)
			{
				num = Mathf.Clamp(num, 0, Grid.Length - 1);
			}
			if (num2 < 0 || num2 >= Grid[num].Row.Length)
			{
				num2 = Mathf.Clamp(num2, 0, Grid[num].Row.Length - 1);
			}
			return new Vector2Int(Grid[num].Row[num2].CellProperties.Row, Grid[num].Row[num2].CellProperties.Column);
		}

		internal Vector3 GetCellPosition(Vector2Int vector2Int)
		{
			return GetCell(vector2Int).CellProperties.Center;
		}

		internal CellData[] GetCells(List<Vector2Int> activeCells)
		{
			CellData[] array = new CellData[activeCells.Count];
			for (int i = 0; i < activeCells.Count; i++)
			{
				array[i] = GetCell(activeCells[i]);
			}
			return array;
		}

		public int GetGridLength()
		{
			return Grid.Length;
		}

		public int GetRowLength(int row)
		{
			return Grid[row].Row.Length;
		}

		internal List<Vector2Int> GetCellNeighbors(int row, int column, int depth, bool justEdgeCells)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			GetCellNeighbors(row, column, depth, justEdgeCells, result);
			return result;
		}

		internal void GetCellNeighbors(int row, int column, int depth, bool justEdgeCells, List<Vector2Int> result)
		{
			result.Clear();
			int num = row - depth;
			if (num < 0)
			{
				num = 0;
			}
			int num2 = row + depth;
			if (num2 >= GetGridLength())
			{
				num2 = GetGridLength() - 1;
			}
			int num3 = column - depth;
			if (num3 < 0)
			{
				num3 = 0;
			}
			int num4 = column + depth;
			if (num4 >= GetRowLength(row))
			{
				num4 = GetRowLength(row) - 1;
			}
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					if (justEdgeCells)
					{
						if (i == row + depth || i == row - depth || j == column + depth || j == column - depth)
						{
							result.Add(new Vector2Int(i, j));
						}
					}
					else
					{
						result.Add(new Vector2Int(i, j));
					}
				}
			}
		}

		public bool HasTrafficSpawnWaypoints(Vector2Int cellIndex)
		{
			return Grid[cellIndex.x].Row[cellIndex.y].TrafficWaypointsData.HasSpawnWaypoints;
		}

		public List<SpawnWaypoint> GetTrafficSpawnWaypoipointsAroundPosition(Vector3 position, int agentType)
		{
			List<SpawnWaypoint> list = GetCell(position.x, position.z).TrafficWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList();
			if (list.Count == 0)
			{
				CellData cell = GetCell(position.x, position.z);
				foreach (Vector2Int cellNeighbor in GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: true))
				{
					list.AddRange(GetCell(cellNeighbor).TrafficWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList());
				}
			}
			return list;
		}

		public List<int> GetTrafficWaypointsAroundPosition(Vector3 position)
		{
			List<int> waypoints = GetCell(position.x, position.z).TrafficWaypointsData.Waypoints;
			if (waypoints.Count == 0)
			{
				CellData cell = GetCell(position.x, position.z);
				foreach (Vector2Int cellNeighbor in GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: true))
				{
					waypoints.AddRange(GetCell(cellNeighbor).TrafficWaypointsData.Waypoints);
				}
			}
			return waypoints;
		}

		public List<int> GetAllTrafficPlayModeWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < Grid.Length; i++)
			{
				for (int j = 0; j < Grid[i].Row.Length; j++)
				{
					list.AddRange(Grid[i].Row[j].TrafficWaypointsData.Waypoints);
				}
			}
			return list;
		}

		public List<SpawnWaypoint> GetAllTrafficSpawnWaypoints()
		{
			List<SpawnWaypoint> list = new List<SpawnWaypoint>();
			for (int i = 0; i < Grid.Length; i++)
			{
				for (int j = 0; j < Grid[i].Row.Length; j++)
				{
					list.AddRange(Grid[i].Row[j].TrafficWaypointsData.SpawnWaypoints);
				}
			}
			return list;
		}

		public List<int> GetAllTrafficWaypointsInCell(Vector2Int cellIndex)
		{
			return Grid[cellIndex.x].Row[cellIndex.y].TrafficWaypointsData.Waypoints;
		}

		public List<SpawnWaypoint> GetTrafficSpawnWaypointsForCell(Vector2Int cellIndex, int agentType)
		{
			_ = Grid[cellIndex.x].Row[cellIndex.y].TrafficWaypointsData.SpawnWaypoints;
			return Grid[cellIndex.x].Row[cellIndex.y].TrafficWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList();
		}

		public void AddTrafficWaypoint(CellData cellData, int waypointIndex)
		{
			cellData.TrafficWaypointsData.Waypoints.Add(waypointIndex);
			cellData.TrafficWaypointsData.HasWaypoints = true;
		}

		public void AddTrafficSpawnWaypoint(CellData cellData, int waypointIndex, int[] allowedVehicles, int priority)
		{
			cellData.TrafficWaypointsData.SpawnWaypoints.Add(new SpawnWaypoint(waypointIndex, allowedVehicles, priority));
			cellData.TrafficWaypointsData.HasSpawnWaypoints = true;
		}

		public void AddIntersection(CellData cellData, int intersectionIndex)
		{
			cellData.IntersectionsInCell.Add(intersectionIndex);
		}

		public bool HasPedestrianSpawnWaypoints(Vector2Int cellIndex)
		{
			return Grid[cellIndex.x].Row[cellIndex.y].PedestrianWaypointsData.HasSpawnWaypoints;
		}

		internal List<int> GetPedestrianWaypointsInArea(Area area)
		{
			List<int> list = new List<int>();
			CellData cell = GetCell(area.Center);
			List<Vector2Int> cellNeighbors = GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, Mathf.CeilToInt(area.Radius * 2f / (float)GridCellSize), justEdgeCells: false);
			for (int num = cellNeighbors.Count - 1; num >= 0; num--)
			{
				cell = GetCell(cellNeighbors[num]);
				list.AddRange(cell.PedestrianWaypointsData.Waypoints);
			}
			return list;
		}

		public List<int> GetPedestrianWaypointsAroundPosition(Vector3 position)
		{
			List<int> waypoints = GetCell(position.x, position.z).PedestrianWaypointsData.Waypoints;
			if (waypoints.Count == 0)
			{
				CellData cell = GetCell(position.x, position.z);
				foreach (Vector2Int cellNeighbor in GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: true))
				{
					waypoints.AddRange(GetCell(cellNeighbor).PedestrianWaypointsData.Waypoints);
				}
			}
			return waypoints;
		}

		internal List<SpawnWaypoint> GetPedestrianSpawnWaypoipointsAroundPosition(Vector3 position, int agentType)
		{
			List<SpawnWaypoint> list = GetCell(position.x, position.z).PedestrianWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList();
			if (list.Count == 0)
			{
				CellData cell = GetCell(position.x, position.z);
				foreach (Vector2Int cellNeighbor in GetCellNeighbors(cell.CellProperties.Row, cell.CellProperties.Column, 1, justEdgeCells: true))
				{
					list.AddRange(GetCell(cellNeighbor).PedestrianWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList());
				}
			}
			return list;
		}

		public List<int> GetAllPedestrianPlayModeWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < Grid.Length; i++)
			{
				for (int j = 0; j < Grid[i].Row.Length; j++)
				{
					list.AddRange(Grid[i].Row[j].PedestrianWaypointsData.Waypoints);
				}
			}
			return list;
		}

		public List<SpawnWaypoint> GetAllPedestrianSpawnWaypoints()
		{
			List<SpawnWaypoint> list = new List<SpawnWaypoint>();
			for (int i = 0; i < Grid.Length; i++)
			{
				for (int j = 0; j < Grid[i].Row.Length; j++)
				{
					list.AddRange(Grid[i].Row[j].PedestrianWaypointsData.SpawnWaypoints);
				}
			}
			return list;
		}

		public List<SpawnWaypoint> GetPedestrianSpawnWaypointsForCell(Vector2Int cellIndex, int agentType)
		{
			return Grid[cellIndex.x].Row[cellIndex.y].PedestrianWaypointsData.SpawnWaypoints.Where((SpawnWaypoint cond1) => cond1.AllowedAgents.Contains(agentType)).ToList();
		}

		public void AddPedestrianWaypoint(CellData cellData, int waypointIndex)
		{
			cellData.PedestrianWaypointsData.Waypoints.Add(waypointIndex);
			cellData.PedestrianWaypointsData.HasWaypoints = true;
		}

		public void AddPedestrianSpawnWaypoint(CellData cellData, int waypointIndex, int[] allowedPedestrians, int priority)
		{
			cellData.PedestrianWaypointsData.SpawnWaypoints.Add(new SpawnWaypoint(waypointIndex, allowedPedestrians, priority));
			cellData.PedestrianWaypointsData.HasSpawnWaypoints = true;
		}
	}
}
