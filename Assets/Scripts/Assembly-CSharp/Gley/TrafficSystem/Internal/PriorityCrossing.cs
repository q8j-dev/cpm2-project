using System.Collections.Generic;
using System.Linq;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class PriorityCrossing : GenericIntersection, IDestroyable
	{
		private List<PedestrianCrossing> _pedestriansCrossing;

		private PriorityCrossingData _priorityCrossingData;

		private Vector3 _position;

		private Color _waypointColor;

		private bool _stopCars;

		public PriorityCrossing(PriorityCrossingData priorityCrossingData, TrafficWaypointsData trafficWaypointsData, IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			_priorityCrossingData = priorityCrossingData;
			for (int i = 0; i < _priorityCrossingData.ExitWaypoints.Length; i++)
			{
				trafficWaypointsData.AllTrafficWaypoints[_priorityCrossingData.ExitWaypoints[i]].SetIntersection(this, giveWay: false, stop: false, enter: false, exit: true, intersectionGiveWay: false);
			}
			int num = 0;
			for (int j = 0; j < _priorityCrossingData.StopWaypoints.Length; j++)
			{
				for (int k = 0; k < _priorityCrossingData.StopWaypoints[j].roadWaypoints.Length; k++)
				{
					trafficWaypointsData.AllTrafficWaypoints[_priorityCrossingData.StopWaypoints[j].roadWaypoints[k]].SetIntersection(this, giveWay: true, stop: false, enter: true, exit: false, intersectionGiveWay: true);
					_position += trafficWaypointsData.AllTrafficWaypoints[_priorityCrossingData.StopWaypoints[j].roadWaypoints[k]].Position;
					num++;
				}
			}
			_position /= (float)num;
			InitializePedestrianWaypoints(pedestrianWaypointsDataHandler);
			_carsInIntersection = new List<int>();
			_waypointColor = Color.green;
			Assign();
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public override bool IsPathFree(int waypointIndex)
		{
			_stopCars = IsPedestrianCrossing(0);
			if (_stopCars)
			{
				if (_waypointColor != Color.red)
				{
					_waypointColor = Color.red;
					CheckColor();
				}
				return false;
			}
			if (_waypointColor != Color.green)
			{
				_waypointColor = Color.green;
			}
			return true;
		}

		public override string GetName()
		{
			return _priorityCrossingData.Name;
		}

		public override void PedestrianPassed(int pedestrianIndex)
		{
		}

		public int[] GetWaypointsToCkeck()
		{
			return _priorityCrossingData.StopWaypoints[0].roadWaypoints;
		}

		public Color GetWaypointColors()
		{
			return _waypointColor;
		}

		public override List<int> GetStopWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _priorityCrossingData.StopWaypoints.Length; i++)
			{
				list.AddRange(_priorityCrossingData.StopWaypoints[i].roadWaypoints);
			}
			return list;
		}

		public void SetPriorityCrossingState(bool stop, bool stopUpdate)
		{
			_stopCars = stop;
			IsPathFree(0);
		}

		public bool GetPriorityCrossingState()
		{
			return _waypointColor == Color.red;
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

		public Vector3 GetPosition()
		{
			return _position;
		}

		public override void ResetIntersection()
		{
			base.ResetIntersection();
			_pedestriansCrossing = new List<PedestrianCrossing>();
		}

		private void InitializePedestrianWaypoints(IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			_pedestriansCrossing = new List<PedestrianCrossing>();
		}

		private void MakePedestriansCross(int road)
		{
		}

		private void PedestrianWantsToCross(int pedestrianIndex, IIntersection intersection, int waypointIndex)
		{
			if (intersection == this)
			{
				int roadToCross = GetRoadToCross(waypointIndex);
				_pedestriansCrossing.Add(new PedestrianCrossing(pedestrianIndex, roadToCross));
				CheckColor();
			}
		}

		private void CheckColor()
		{
			if (_pedestriansCrossing.Count > 0)
			{
				if (_waypointColor == Color.red)
				{
					MakePedestriansCross(0);
				}
				else
				{
					IsPathFree(0);
				}
			}
		}

		private int GetRoadToCross(int waypoint)
		{
			for (int i = 0; i < _priorityCrossingData.StopWaypoints.Length; i++)
			{
				for (int j = 0; j < _priorityCrossingData.StopWaypoints[i].pedestrianWaypoints.Length; j++)
				{
					if (_priorityCrossingData.StopWaypoints[i].pedestrianWaypoints[j] == waypoint)
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
