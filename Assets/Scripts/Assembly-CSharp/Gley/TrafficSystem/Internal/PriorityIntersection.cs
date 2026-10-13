using System.Collections.Generic;
using System.Linq;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class PriorityIntersection : GenericIntersection, IDestroyable
	{
		private readonly List<int> _waypointsToCkeck;

		private readonly List<Color> _waypointColor;

		private readonly float _requiredTime;

		private List<PedestrianCrossing> _pedestriansCrossing;

		private PriorityIntersectionData _priorityIntersectionData;

		private Vector3 _position;

		private float _currentTime;

		private int _currentRoadIndex;

		private int _nrOfRoads;

		private bool _currentRoadIsActive = true;

		public PriorityIntersection(PriorityIntersectionData priorityIntersectionData, TrafficWaypointsData trafficWaypointsData, IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			_priorityIntersectionData = priorityIntersectionData;
			for (int i = 0; i < _priorityIntersectionData.ExitWaypoints.Length; i++)
			{
				trafficWaypointsData.AllTrafficWaypoints[_priorityIntersectionData.ExitWaypoints[i]].SetIntersection(this, giveWay: false, stop: false, enter: false, exit: true, intersectionGiveWay: false);
			}
			int num = 0;
			for (int j = 0; j < _priorityIntersectionData.StopWaypoints.Length; j++)
			{
				for (int k = 0; k < _priorityIntersectionData.StopWaypoints[j].roadWaypoints.Length; k++)
				{
					trafficWaypointsData.AllTrafficWaypoints[_priorityIntersectionData.StopWaypoints[j].roadWaypoints[k]].SetIntersection(this, giveWay: true, stop: false, enter: true, exit: false, intersectionGiveWay: true);
					_position += trafficWaypointsData.AllTrafficWaypoints[_priorityIntersectionData.StopWaypoints[j].roadWaypoints[k]].Position;
					num++;
				}
			}
			_position /= (float)num;
			InitializePedestrianWaypoints(pedestrianWaypointsDataHandler);
			_carsInIntersection = new List<int>();
			_requiredTime = 3f;
			_waypointsToCkeck = new List<int>();
			_waypointColor = new List<Color>();
			_nrOfRoads = _priorityIntersectionData.StopWaypoints.Length;
			_currentRoadIndex = Random.Range(0, _nrOfRoads);
			_currentTime = Time.timeSinceLevelLoad;
			Assign();
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public override bool IsPathFree(int waypointIndex)
		{
			if (IsPedestrianCrossing(_currentRoadIndex))
			{
				return false;
			}
			if (_currentRoadIsActive)
			{
				if (_priorityIntersectionData.StopWaypoints[_currentRoadIndex].roadWaypoints.Contains(waypointIndex))
				{
					_currentTime = Time.timeSinceLevelLoad;
					return true;
				}
				if (Time.timeSinceLevelLoad - _currentTime > _requiredTime && _carsInIntersection.Count <= 0)
				{
					_currentRoadIsActive = false;
					_currentTime = Time.timeSinceLevelLoad;
				}
			}
			else
			{
				for (int i = 0; i < _priorityIntersectionData.StopWaypoints.Length; i++)
				{
					if (_priorityIntersectionData.StopWaypoints[i].roadWaypoints.Contains(waypointIndex))
					{
						_currentRoadIndex = i;
						_currentRoadIsActive = true;
					}
				}
			}
			return false;
		}

		public override void PedestrianPassed(int pedestrianIndex)
		{
			PedestrianCrossing pedestrianCrossing = _pedestriansCrossing.FirstOrDefault((PedestrianCrossing cond) => cond.PedestrianIndex == pedestrianIndex);
			if (pedestrianCrossing != null)
			{
				if (!pedestrianCrossing.Crossing)
				{
					pedestrianCrossing.Crossing = true;
				}
				else
				{
					_pedestriansCrossing.Remove(pedestrianCrossing);
				}
			}
		}

		public override string GetName()
		{
			return _priorityIntersectionData.Name;
		}

		public override void ResetIntersection()
		{
			base.ResetIntersection();
			_pedestriansCrossing = new List<PedestrianCrossing>();
		}

		public Vector3 GetPosition()
		{
			return _position;
		}

		public List<int> GetWaypointsToCkeck()
		{
			return _waypointsToCkeck;
		}

		public List<Color> GetWaypointColors()
		{
			return _waypointColor;
		}

		public override List<int> GetStopWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _priorityIntersectionData.StopWaypoints.Length; i++)
			{
				list.AddRange(_priorityIntersectionData.StopWaypoints[i].roadWaypoints);
			}
			return list;
		}

		public override void UpdateIntersection(float realtimeSinceStartup)
		{
		}

		public int GetCarsInIntersection()
		{
			return _carsInIntersection.Count;
		}

		public List<PedestrianCrossing> GetPedestriansCrossing()
		{
			return _pedestriansCrossing;
		}

		public override int[] GetPedStopWaypoint()
		{
			return new int[0];
		}

		private void InitializePedestrianWaypoints(IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			_pedestriansCrossing = new List<PedestrianCrossing>();
		}

		private void PedestrianWantsToCross(int pedestrianIndex, IIntersection intersection, int waypointIndex)
		{
			if (intersection == this)
			{
				int roadToCross = GetRoadToCross(waypointIndex);
				_pedestriansCrossing.Add(new PedestrianCrossing(pedestrianIndex, roadToCross));
			}
		}

		private int GetRoadToCross(int waypoint)
		{
			for (int i = 0; i < _priorityIntersectionData.StopWaypoints.Length; i++)
			{
				for (int j = 0; j < _priorityIntersectionData.StopWaypoints[i].pedestrianWaypoints.Length; j++)
				{
					if (_priorityIntersectionData.StopWaypoints[i].pedestrianWaypoints[j] == waypoint)
					{
						return i;
					}
				}
			}
			Debug.LogError("Not Good - verify pedestrians assignments in priority intersection");
			return -1;
		}

		private bool IsPedestrianCrossing(int road)
		{
			if (_pedestriansCrossing.Count == 0)
			{
				return false;
			}
			return _pedestriansCrossing.FirstOrDefault((PedestrianCrossing cond) => cond.Road == road) != null;
		}

		public void OnDestroy()
		{
		}
	}
}
