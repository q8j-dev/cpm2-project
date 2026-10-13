using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class WaypointSettingsBase : MonoBehaviour
	{
		public List<WaypointSettingsBase> neighbors;

		public List<WaypointSettingsBase> prev;

		public bool draw = true;

		public bool inView;

		public Vector3 position;

		public bool priorityLocked;

		public int priority;

		public bool triggerEvent;

		public string eventData;

		public List<int> distance;

		public bool penaltyLocked;

		public int penalty;

		public virtual void Initialize()
		{
			neighbors = new List<WaypointSettingsBase>();
			prev = new List<WaypointSettingsBase>();
		}

		public virtual void VerifyAssignments(bool showPrevsWarning)
		{
			if (neighbors == null)
			{
				neighbors = new List<WaypointSettingsBase>();
			}
			for (int num = neighbors.Count - 1; num >= 0; num--)
			{
				if (neighbors[num] == null)
				{
					neighbors.RemoveAt(num);
				}
			}
			if (prev == null)
			{
				prev = new List<WaypointSettingsBase>();
			}
			for (int num2 = prev.Count - 1; num2 >= 0; num2--)
			{
				if (prev[num2] == null)
				{
					prev.RemoveAt(num2);
				}
			}
			if (distance == null)
			{
				distance = new List<int>();
			}
			if (priority <= 0)
			{
				priority = 1;
			}
		}

		public void SetPriorityForAllNeighbors(int newPriority)
		{
			Queue<WaypointSettingsBase> queue = new Queue<WaypointSettingsBase>();
			HashSet<WaypointSettingsBase> hashSet = new HashSet<WaypointSettingsBase>();
			queue.Enqueue(this);
			hashSet.Add(this);
			priorityLocked = false;
			while (queue.Count > 0)
			{
				WaypointSettingsBase waypointSettingsBase = queue.Dequeue();
				if (waypointSettingsBase.priorityLocked)
				{
					continue;
				}
				waypointSettingsBase.priority = newPriority;
				foreach (WaypointSettingsBase neighbor in waypointSettingsBase.neighbors)
				{
					if (!hashSet.Contains(neighbor))
					{
						queue.Enqueue(neighbor);
						hashSet.Add(neighbor);
					}
				}
			}
			if (priority != 0)
			{
				priorityLocked = true;
			}
			Debug.Log("Done");
		}

		public void SetPenaltyForAllNeighbors(int newPenalty)
		{
			Queue<WaypointSettingsBase> queue = new Queue<WaypointSettingsBase>();
			HashSet<WaypointSettingsBase> hashSet = new HashSet<WaypointSettingsBase>();
			queue.Enqueue(this);
			hashSet.Add(this);
			penaltyLocked = false;
			while (queue.Count > 0)
			{
				WaypointSettingsBase waypointSettingsBase = queue.Dequeue();
				if (waypointSettingsBase.penaltyLocked)
				{
					continue;
				}
				waypointSettingsBase.penalty = newPenalty;
				foreach (WaypointSettingsBase neighbor in waypointSettingsBase.neighbors)
				{
					if (!hashSet.Contains(neighbor))
					{
						queue.Enqueue(neighbor);
						hashSet.Add(neighbor);
					}
				}
			}
			if (penalty != 0)
			{
				penaltyLocked = true;
			}
			Debug.Log("Done");
		}
	}
}
