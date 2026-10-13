using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class WaypointSettings : WaypointSettingsBase
	{
		public List<WaypointSettings> otherLanes;

		public List<WaypointSettings> giveWayList;

		public List<VehicleTypes> allowedCars;

		[SerializeField]
		private Vector3 _left;

		public float laneWidth;

		public int maxSpeed;

		public bool giveWay;

		public bool enter;

		public bool exit;

		public bool speedLocked;

		public bool carsLocked;

		public bool complexGiveWay;

		public bool zipperGiveWay;

		public Vector3 Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			otherLanes = new List<WaypointSettings>();
			giveWayList = new List<WaypointSettings>();
		}

		public void ResetProperties()
		{
			enter = false;
			exit = false;
		}

		public override void VerifyAssignments(bool showPrevsWarning)
		{
			base.VerifyAssignments(showPrevsWarning);
			if (showPrevsWarning && prev.Count == 0)
			{
				Debug.LogWarning(UrbanSystemErrors.NoPrevs(base.name), base.gameObject);
			}
			if (otherLanes == null)
			{
				otherLanes = new List<WaypointSettings>();
			}
			for (int num = otherLanes.Count - 1; num >= 0; num--)
			{
				if (otherLanes[num] == null)
				{
					otherLanes.RemoveAt(num);
				}
			}
			if (giveWayList == null)
			{
				giveWayList = new List<WaypointSettings>();
			}
			for (int num2 = giveWayList.Count - 1; num2 >= 0; num2--)
			{
				if (giveWayList[num2] == null)
				{
					giveWayList.RemoveAt(num2);
				}
			}
			if (allowedCars == null)
			{
				allowedCars = new List<VehicleTypes>();
			}
			for (int num3 = allowedCars.Count - 1; num3 >= 0; num3--)
			{
				if (!IsValid((int)allowedCars[num3]))
				{
					allowedCars.RemoveAt(num3);
				}
			}
			if (laneWidth == 0f)
			{
				if (base.name.Contains("Connect"))
				{
					laneWidth = 4f;
				}
				else
				{
					laneWidth = base.transform.parent.parent.parent.GetComponent<Road>().laneWidth;
				}
			}
			if (!_left.Equals(Vector3.zero))
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			foreach (WaypointSettingsBase neighbor in neighbors)
			{
				zero += neighbor.transform.position - base.transform.position;
			}
			zero.Normalize();
			Left = Vector3.Cross(Vector3.up, zero).normalized;
		}

		public void SetVehicleTypesForAllNeighbors(List<VehicleTypes> allowedVehicles)
		{
			Queue<WaypointSettings> queue = new Queue<WaypointSettings>();
			HashSet<WaypointSettingsBase> hashSet = new HashSet<WaypointSettingsBase>();
			queue.Enqueue(this);
			hashSet.Add(this);
			carsLocked = false;
			while (queue.Count > 0)
			{
				WaypointSettings waypointSettings = queue.Dequeue();
				if (waypointSettings.carsLocked)
				{
					continue;
				}
				waypointSettings.allowedCars = allowedVehicles;
				foreach (WaypointSettingsBase neighbor in waypointSettings.neighbors)
				{
					if (!hashSet.Contains(neighbor))
					{
						queue.Enqueue((WaypointSettings)neighbor);
						hashSet.Add(neighbor);
					}
				}
			}
			if (allowedVehicles.Count > 0)
			{
				carsLocked = true;
			}
			Debug.Log("Done");
		}

		public void SetSpeedForAllNeighbors(int newSpeed)
		{
			Queue<WaypointSettings> queue = new Queue<WaypointSettings>();
			HashSet<WaypointSettingsBase> hashSet = new HashSet<WaypointSettingsBase>();
			queue.Enqueue(this);
			hashSet.Add(this);
			speedLocked = false;
			while (queue.Count > 0)
			{
				WaypointSettings waypointSettings = queue.Dequeue();
				if (waypointSettings.speedLocked)
				{
					continue;
				}
				waypointSettings.maxSpeed = newSpeed;
				foreach (WaypointSettingsBase neighbor in waypointSettings.neighbors)
				{
					if (!hashSet.Contains(neighbor))
					{
						queue.Enqueue((WaypointSettings)neighbor);
						hashSet.Add(neighbor);
					}
				}
			}
			if (newSpeed != 0)
			{
				speedLocked = true;
			}
			Debug.Log("Done");
		}

		private bool IsValid(int value)
		{
			return Enum.IsDefined(typeof(VehicleTypes), value);
		}
	}
}
