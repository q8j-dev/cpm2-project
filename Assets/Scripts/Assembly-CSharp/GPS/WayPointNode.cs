using System;
using UnityEngine;

namespace GPS
{
	[Serializable]
	public class WayPointNode
	{
		public uint id;

		public NodePosition position;

		public uint[] next;

		private float d;

		public static implicit operator Vector3(WayPointNode wayPointNode)
		{
			return wayPointNode.position;
		}

		public WayPointNode()
		{
			Init();
		}

		public WayPointNode(uint id, params uint[] ids)
		{
			Init();
			this.id = id;
			next = ids;
		}

		private void Init()
		{
		}

		public float CalcHeuristic(Vector3 destination)
		{
			return GPSTracker.Distance(destination, new Vector3(position.x, position.y, position.z));
		}

		public int ClosestPath(Vector3 destination)
		{
			WayPointNode wayPointNode = this;
			uint[] array = next;
			while (array.Length == 1)
			{
				WayPointNode wayPointNode2 = GPSTracker.nodes[(int)array[0]];
				try
				{
					if (GPSTracker.distance.ContainsKey(wayPointNode.id))
					{
						d = GPSTracker.distance[wayPointNode.id];
					}
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				float num = d + GPSTracker.Distance(new Vector3(wayPointNode2.position.x, wayPointNode2.position.y, wayPointNode2.position.z), new Vector3(wayPointNode.position.x, wayPointNode.position.y, wayPointNode.position.z));
				if (GPSTracker.cameFrom.ContainsKey(array[0]))
				{
					if (GPSTracker.distance[array[0]] > num)
					{
						GPSTracker.distance[array[0]] = num;
						GPSTracker.cameFrom[array[0]] = wayPointNode.id;
					}
				}
				else
				{
					GPSTracker.distance[array[0]] = num;
					GPSTracker.cameFrom[array[0]] = wayPointNode.id;
				}
				wayPointNode = wayPointNode2;
				array = wayPointNode2.next;
			}
			uint[] array2 = array;
			foreach (uint num2 in array2)
			{
				WayPointNode wayPointNode3 = GPSTracker.nodes[(int)num2];
				try
				{
					if (GPSTracker.distance.ContainsKey(wayPointNode.id))
					{
						d = GPSTracker.distance[wayPointNode.id];
					}
				}
				catch (Exception message2)
				{
					Debug.LogError(message2);
				}
				float num3 = d + GPSTracker.Distance(new Vector3(wayPointNode3.position.x, wayPointNode3.position.y, wayPointNode3.position.z), new Vector3(wayPointNode.position.x, wayPointNode.position.y, wayPointNode.position.z));
				if (GPSTracker.cameFrom.ContainsKey(num2))
				{
					if (GPSTracker.distance[num2] > num3)
					{
						GPSTracker.distance[num2] = num3;
						GPSTracker.cameFrom[num2] = id;
					}
				}
				else
				{
					GPSTracker.distance[num2] = num3;
					GPSTracker.cameFrom[num2] = id;
					GPSTracker.children[num2] = d + wayPointNode3.CalcHeuristic(destination);
				}
			}
			return 1;
		}
	}
}
