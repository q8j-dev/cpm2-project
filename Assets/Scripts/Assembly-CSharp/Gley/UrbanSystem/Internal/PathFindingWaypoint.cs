using System;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class PathFindingWaypoint : IHeapItem<PathFindingWaypoint>, IComparable<PathFindingWaypoint>
	{
		[SerializeField]
		private int[] _allowedAgents;

		[SerializeField]
		private int[] _neighbours;

		[SerializeField]
		private int[] _movementPenalty;

		[SerializeField]
		private Vector3 _worldPosition;

		[SerializeField]
		private int _listIndex;

		internal int[] Neighbours => _neighbours;

		internal int[] AllowedAgents => _allowedAgents;

		internal int[] MovementPenalty => _movementPenalty;

		internal Vector3 WorldPosition => _worldPosition;

		internal int ListIndex => _listIndex;

		internal int FCost => GCost + HCost;

		internal int Parent { get; set; }

		internal int GCost { get; set; }

		internal int HCost { get; set; }

		public int HeapIndex { get; set; }

		public PathFindingWaypoint(int listIndex, Vector3 worldPosition, int gCost, int hCost, int parent, int[] neighbours, int[] movementPenalty, int[] allowedAgents)
		{
			_listIndex = listIndex;
			_worldPosition = worldPosition;
			_neighbours = neighbours;
			_movementPenalty = movementPenalty;
			_allowedAgents = allowedAgents;
			GCost = gCost;
			HCost = hCost;
			Parent = parent;
		}

		public int CompareTo(PathFindingWaypoint nodeToCompare)
		{
			int num = FCost.CompareTo(nodeToCompare.FCost);
			if (num == 0)
			{
				num = HCost.CompareTo(nodeToCompare.HCost);
			}
			return -num;
		}
	}
}
