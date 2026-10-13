using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class ActiveCellsManager
	{
		private readonly GridData _gridData;

		private List<Vector2Int> _activeCells;

		private List<Vector2Int> _currentCells;

		public ActiveCellsManager(NativeArray<float3> activeCameraPositions, GridData gridData, int level)
		{
			_gridData = gridData;
			_currentCells = new List<Vector2Int>();
			for (int i = 0; i < activeCameraPositions.Length; i++)
			{
				_currentCells.Add(default);
			}
			UpdateActiveCells(activeCameraPositions, level);
		}

		internal void UpdateGrid(int level, NativeArray<float3> activeCameraPositions)
		{
			UpdateActiveCells(activeCameraPositions, level);
		}

		private void UpdateActiveCells(NativeArray<float3> activeCameraPositions, int level)
		{
			if (_currentCells.Count != activeCameraPositions.Length)
			{
				_currentCells = new List<Vector2Int>();
				for (int i = 0; i < activeCameraPositions.Length; i++)
				{
					_currentCells.Add(default);
				}
			}
			bool flag = false;
			for (int j = 0; j < activeCameraPositions.Length; j++)
			{
				Vector2Int cellIndex = _gridData.GetCellIndex(activeCameraPositions[j]);
				if (_currentCells[j] != cellIndex)
				{
					_currentCells[j] = cellIndex;
					flag = true;
				}
			}
			if (flag)
			{
				_activeCells = new List<Vector2Int>();
				for (int k = 0; k < activeCameraPositions.Length; k++)
				{
					_activeCells.AddRange(_gridData.GetCellNeighbors(_currentCells[k].x, _currentCells[k].y, level, justEdgeCells: false));
				}
				GridEvents.TriggerActiveGridCellsChangedEvent(_gridData.GetCells(_activeCells));
			}
		}
	}
}
