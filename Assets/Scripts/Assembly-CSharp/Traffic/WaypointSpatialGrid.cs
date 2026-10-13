using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
	public class WaypointSpatialGrid
	{
		private readonly Dictionary<long, List<int>> _cells = new Dictionary<long, List<int>>();

		private Vector3[] _points;

		private float _invCellSize = 0.01f;

		public void Build(Vector3[] points, float cellSize)
		{
			_points = points;
			_invCellSize = 1f / cellSize;
			foreach (List<int> value2 in _cells.Values)
			{
				value2.Clear();
			}
			if (points == null)
			{
				return;
			}
			for (int i = 0; i < points.Length; i++)
			{
				long key = PackKey(CellCoord(points[i].x), CellCoord(points[i].z));
				if (!_cells.TryGetValue(key, out var value))
				{
					value = new List<int>();
					_cells[key] = value;
				}
				value.Add(i);
			}
		}

		public void QueryRing(Vector3 center, float minDistance, float maxDistance, List<PointWithIndex> results)
		{
			results.Clear();
			if (_points == null)
			{
				return;
			}
			float num = minDistance * minDistance;
			float num2 = maxDistance * maxDistance;
			int num3 = CellCoord(center.x);
			int num4 = CellCoord(center.z);
			int num5 = Mathf.CeilToInt(maxDistance * _invCellSize);
			for (int i = num3 - num5; i <= num3 + num5; i++)
			{
				for (int j = num4 - num5; j <= num4 + num5; j++)
				{
					if (!_cells.TryGetValue(PackKey(i, j), out var value))
					{
						continue;
					}
					for (int k = 0; k < value.Count; k++)
					{
						int num6 = value[k];
						float sqrMagnitude = (center - _points[num6]).sqrMagnitude;
						if (sqrMagnitude < num2 && sqrMagnitude >= num)
						{
							results.Add(new PointWithIndex(_points[num6], num6));
						}
					}
				}
			}
		}

		public void QueryWithin(Vector3 center, float maxDistance, List<Vector3> results)
		{
			results.Clear();
			if (_points == null)
			{
				return;
			}
			float num = maxDistance * maxDistance;
			int num2 = CellCoord(center.x);
			int num3 = CellCoord(center.z);
			int num4 = Mathf.CeilToInt(maxDistance * _invCellSize);
			for (int i = num2 - num4; i <= num2 + num4; i++)
			{
				for (int j = num3 - num4; j <= num3 + num4; j++)
				{
					if (!_cells.TryGetValue(PackKey(i, j), out var value))
					{
						continue;
					}
					for (int k = 0; k < value.Count; k++)
					{
						int num5 = value[k];
						if ((center - _points[num5]).sqrMagnitude < num)
						{
							results.Add(_points[num5]);
						}
					}
				}
			}
		}

		private int CellCoord(float worldCoord)
		{
			return Mathf.FloorToInt(worldCoord * _invCellSize);
		}

		private static long PackKey(int x, int z)
		{
			return ((long)x << 32) ^ (uint)z;
		}
	}
}
