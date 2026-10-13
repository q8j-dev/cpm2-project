using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	internal class AStar
	{
		internal List<int> FindPath(int startNodeIndex, int targetNodeIndex, int pedestraianTypes, PathFindingWaypoint[] allPathFindingWaypoints)
		{
			PathFindingWaypoint pathFindingWaypoint = allPathFindingWaypoints[startNodeIndex];
			PathFindingWaypoint pathFindingWaypoint2 = allPathFindingWaypoints[targetNodeIndex];
			Heap<PathFindingWaypoint> heap = new Heap<PathFindingWaypoint>(allPathFindingWaypoints.Length);
			HashSet<PathFindingWaypoint> hashSet = new HashSet<PathFindingWaypoint>();
			heap.Add(pathFindingWaypoint);
			while (heap.Count > 0)
			{
				PathFindingWaypoint pathFindingWaypoint3 = heap.RemoveFirst();
				hashSet.Add(pathFindingWaypoint3);
				if (pathFindingWaypoint3 == pathFindingWaypoint2)
				{
					return RetracePath(pathFindingWaypoint, pathFindingWaypoint2, allPathFindingWaypoints);
				}
				for (int i = 0; i < pathFindingWaypoint3.Neighbours.Length; i++)
				{
					PathFindingWaypoint pathFindingWaypoint4 = allPathFindingWaypoints[pathFindingWaypoint3.Neighbours[i]];
					if (!pathFindingWaypoint4.AllowedAgents.Contains(pedestraianTypes) || hashSet.Contains(pathFindingWaypoint4))
					{
						continue;
					}
					int num = pathFindingWaypoint3.GCost + GetDistance(pathFindingWaypoint3, pathFindingWaypoint4) + pathFindingWaypoint3.MovementPenalty[i];
					if (num < pathFindingWaypoint4.GCost || !heap.Contains(pathFindingWaypoint4))
					{
						pathFindingWaypoint4.GCost = num;
						pathFindingWaypoint4.HCost = GetDistance(pathFindingWaypoint4, pathFindingWaypoint2);
						pathFindingWaypoint4.Parent = pathFindingWaypoint3.ListIndex;
						if (!heap.Contains(pathFindingWaypoint4))
						{
							heap.Add(pathFindingWaypoint4);
						}
						else
						{
							heap.UpdateItem(pathFindingWaypoint4);
						}
					}
				}
			}
			return null;
		}

		private List<int> RetracePath(PathFindingWaypoint startNode, PathFindingWaypoint endNode, PathFindingWaypoint[] allPathFindingWaypoints)
		{
			List<int> list = new List<int>();
			for (PathFindingWaypoint pathFindingWaypoint = endNode; pathFindingWaypoint != startNode; pathFindingWaypoint = allPathFindingWaypoints[pathFindingWaypoint.Parent])
			{
				list.Add(pathFindingWaypoint.ListIndex);
			}
			list.Reverse();
			return list;
		}

		private int GetDistance(PathFindingWaypoint nodeA, PathFindingWaypoint nodeB)
		{
			float num = Mathf.Abs(nodeA.WorldPosition.x - nodeB.WorldPosition.x);
			float num2 = Mathf.Abs(nodeA.WorldPosition.z - nodeB.WorldPosition.z);
			if (num > num2)
			{
				return (int)(14f * num2 + 10f * (num - num2));
			}
			return (int)(14f * num + 10f * (num2 - num));
		}
	}
}
