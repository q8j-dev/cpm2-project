using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class TrafficLightsIntersection : GenericIntersection
	{
		private readonly TrafficLightsIntersectionData _trafficLightsIntersectionData;

		private readonly TrafficLightsColor[] _intersectionState;

		private readonly float[] _roadGreenLightTime;

		private TrafficLightsBehaviour _trafficLightsBehaviour;

		private float _currentTime;

		private int _nrOfRoads;

		private int _currentRoad;

		private bool _yellowLight;

		private bool _stopUpdate;

		private bool _hasPedestrians;

		public TrafficLightsIntersection(TrafficLightsIntersectionData trafficLightsIntersectionData, TrafficWaypointsData trafficWaypointsData, IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler, TrafficLightsBehaviour trafficLightsBehaviour, float greenLightTime, float yellowLightTime)
		{
			_trafficLightsIntersectionData = trafficLightsIntersectionData;
			SetTrafficLightsBehaviour(trafficLightsBehaviour);
			_nrOfRoads = _trafficLightsIntersectionData.StopWaypoints.Length;
			GetPedestrianRoads(pedestrianWaypointsDataHandler);
			_roadGreenLightTime = new float[_nrOfRoads];
			for (int i = 0; i < _trafficLightsIntersectionData.StopWaypoints.Length; i++)
			{
				_roadGreenLightTime[i] = _trafficLightsIntersectionData.StopWaypoints[i].greenLightTime;
			}
			SetPedestrianGreenLightTime();
			if (_nrOfRoads == 0)
			{
				Debug.LogWarning("Intersection " + _trafficLightsIntersectionData.Name + " has some unassigned references");
				return;
			}
			_carsInIntersection = new List<int>();
			for (int j = 0; j < _trafficLightsIntersectionData.ExitWaypoints.Length; j++)
			{
				trafficWaypointsData.AllTrafficWaypoints[_trafficLightsIntersectionData.ExitWaypoints[j]].SetIntersection(this, giveWay: false, stop: false, enter: false, exit: true, intersectionGiveWay: false);
			}
			for (int k = 0; k < _trafficLightsIntersectionData.StopWaypoints.Length; k++)
			{
				for (int l = 0; l < _trafficLightsIntersectionData.StopWaypoints[k].roadWaypoints.Length; l++)
				{
					trafficWaypointsData.AllTrafficWaypoints[_trafficLightsIntersectionData.StopWaypoints[k].roadWaypoints[l]].SetIntersection(this, giveWay: false, stop: false, enter: true, exit: false, intersectionGiveWay: false);
				}
			}
			_intersectionState = new TrafficLightsColor[_nrOfRoads];
			_currentRoad = Random.Range(0, _nrOfRoads);
			ChangeCurrentRoadColors(_currentRoad, TrafficLightsColor.Green);
			ChangeAllRoadsExceptSelectd(_currentRoad, TrafficLightsColor.Red);
			ApplyColorChanges();
			_currentTime = 0f;
			if (greenLightTime >= 0f)
			{
				for (int m = 0; m < _roadGreenLightTime.Length; m++)
				{
					_roadGreenLightTime[m] = greenLightTime;
				}
			}
			if (yellowLightTime >= 0f)
			{
				_trafficLightsIntersectionData.YellowLightTime = yellowLightTime;
			}
			for (int n = 0; n < _roadGreenLightTime.Length; n++)
			{
				if (_roadGreenLightTime[n] == 0f)
				{
					_roadGreenLightTime[n] = _trafficLightsIntersectionData.GreenLightTime;
				}
			}
		}

		public override void PedestrianPassed(int pedestrianIndex)
		{
		}

		public override bool IsPathFree(int waypointIndex)
		{
			return false;
		}

		public override string GetName()
		{
			return _trafficLightsIntersectionData.Name;
		}

		public override int[] GetPedStopWaypoint()
		{
			return _trafficLightsIntersectionData.PedestrianWaypoints;
		}

		public override void UpdateIntersection(float realtimeSinceStartup)
		{
			if (_stopUpdate)
			{
				return;
			}
			if (!_yellowLight)
			{
				if (realtimeSinceStartup - _currentTime > _roadGreenLightTime[_currentRoad])
				{
					ChangeCurrentRoadColors(_currentRoad, TrafficLightsColor.YellowGreen);
					ChangeCurrentRoadColors(GetValidValue(_currentRoad + 1), TrafficLightsColor.YellowRed);
					ApplyColorChanges();
					_yellowLight = true;
					_currentTime = realtimeSinceStartup;
				}
			}
			else if (realtimeSinceStartup - _currentTime > _trafficLightsIntersectionData.YellowLightTime && (_carsInIntersection.Count == 0 || _trafficLightsIntersectionData.ExitWaypoints.Length == 0))
			{
				ChangeCurrentRoadColors(_currentRoad, TrafficLightsColor.Red);
				_currentRoad++;
				_currentRoad = GetValidValue(_currentRoad);
				ChangeCurrentRoadColors(_currentRoad, TrafficLightsColor.Green);
				_yellowLight = false;
				_currentTime = realtimeSinceStartup;
				ApplyColorChanges();
			}
		}

		public override List<int> GetStopWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _trafficLightsIntersectionData.StopWaypoints.Length; i++)
			{
				list.AddRange(_trafficLightsIntersectionData.StopWaypoints[i].roadWaypoints);
			}
			return list;
		}

		public void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
		{
			_trafficLightsBehaviour = trafficLightsBehaviour;
		}

		public void SetGreenRoad(int roadIndex, bool doNotChangeAgain, float realTimeSinceStartup)
		{
			_stopUpdate = doNotChangeAgain;
			_currentRoad = roadIndex;
			_currentTime = realTimeSinceStartup;
			ChangeCurrentRoadColors(roadIndex, TrafficLightsColor.Green);
			ChangeAllRoadsExceptSelectd(roadIndex, TrafficLightsColor.Red);
			ApplyColorChanges();
		}

		public void SetYellowRoad(int roadIndex, float realTimeSinceStartup)
		{
			_currentRoad = roadIndex - 1;
			_currentRoad = GetValidValue(_currentRoad);
			_currentTime = realTimeSinceStartup;
			_yellowLight = true;
			ChangeCurrentRoadColors(roadIndex, TrafficLightsColor.YellowRed);
			ChangeAllRoadsExceptSelectd(roadIndex, TrafficLightsColor.Red);
			ApplyColorChanges();
		}

		private void ApplyColorChanges()
		{
			for (int i = 0; i < _intersectionState.Length; i++)
			{
				UpdateCurrentIntersectionWaypoints(i, _intersectionState[i] != TrafficLightsColor.Green);
				if (i < _trafficLightsIntersectionData.StopWaypoints.Length)
				{
					_trafficLightsBehaviour?.Invoke(_intersectionState[i], _trafficLightsIntersectionData.StopWaypoints[i].redLightObjects, _trafficLightsIntersectionData.StopWaypoints[i].yellowLightObjects, _trafficLightsIntersectionData.StopWaypoints[i].greenLightObjects, _trafficLightsIntersectionData.Name);
				}
			}
		}

		private void UpdateCurrentIntersectionWaypoints(int road, bool stop)
		{
			if (_hasPedestrians && road >= _trafficLightsIntersectionData.StopWaypoints.Length)
			{
				TriggerPedestrianWaypointsUpdate(stop);
				return;
			}
			for (int i = 0; i < _trafficLightsIntersectionData.StopWaypoints[road].roadWaypoints.Length; i++)
			{
				WaypointEvents.TriggerTrafficLightChangedEvent(_trafficLightsIntersectionData.StopWaypoints[road].roadWaypoints[i], stop);
			}
		}

		private void ChangeCurrentRoadColors(int currentRoad, TrafficLightsColor newColor)
		{
			if (currentRoad < _intersectionState.Length)
			{
				_intersectionState[currentRoad] = newColor;
			}
			else
			{
				Debug.LogError(currentRoad + "is grated than the max number of roads for intersection " + _trafficLightsIntersectionData.Name);
			}
		}

		private void ChangeAllRoadsExceptSelectd(int currentRoad, TrafficLightsColor newColor)
		{
			for (int i = 0; i < _intersectionState.Length; i++)
			{
				if (i != currentRoad)
				{
					_intersectionState[i] = newColor;
				}
			}
		}

		private int GetValidValue(int roadNumber)
		{
			if (roadNumber >= _nrOfRoads)
			{
				roadNumber %= _nrOfRoads;
			}
			if (roadNumber < 0)
			{
				roadNumber = _nrOfRoads + roadNumber;
			}
			return roadNumber;
		}

		private void GetPedestrianRoads(IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			if (_trafficLightsIntersectionData.PedestrianWaypoints.Length != 0)
			{
				_hasPedestrians = true;
				_nrOfRoads++;
				pedestrianWaypointsDataHandler.SetIntersection(_trafficLightsIntersectionData.PedestrianWaypoints, this);
			}
		}

		private void SetPedestrianGreenLightTime()
		{
			if (_hasPedestrians)
			{
				_roadGreenLightTime[_roadGreenLightTime.Length - 1] = _trafficLightsIntersectionData.PedestrianGreenLightTime;
			}
		}

		private void TriggerPedestrianWaypointsUpdate(bool stop)
		{
		}
	}
}
