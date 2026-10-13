using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class TrafficLightsCrossing : GenericIntersection
	{
		private readonly TrafficLightsCrossingData _trafficLightsCrossingData;

		private TrafficLightsBehaviour _trafficLightsBehaviour;

		private TrafficLightsColor _intersectionState;

		private float _currentTime;

		private float _currentTimer;

		private bool _doNotChange;

		public TrafficLightsCrossing(TrafficLightsCrossingData trafficLightsCrossingData, TrafficWaypointsData trafficWaypointsData, IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler, TrafficLightsBehaviour trafficLightsBehaviour)
		{
			_trafficLightsCrossingData = trafficLightsCrossingData;
			SetTrafficLightsBehaviour(trafficLightsBehaviour);
			GetPedestrianRoads(pedestrianWaypointsDataHandler);
			for (int i = 0; i < _trafficLightsCrossingData.ExitWaypoints.Length; i++)
			{
				trafficWaypointsData.AllTrafficWaypoints[_trafficLightsCrossingData.ExitWaypoints[i]].SetIntersection(this, giveWay: false, stop: false, enter: false, exit: true, intersectionGiveWay: false);
			}
			for (int j = 0; j < _trafficLightsCrossingData.StopWaypoints.Length; j++)
			{
				for (int k = 0; k < _trafficLightsCrossingData.StopWaypoints[j].roadWaypoints.Length; k++)
				{
					trafficWaypointsData.AllTrafficWaypoints[_trafficLightsCrossingData.StopWaypoints[j].roadWaypoints[k]].SetIntersection(this, giveWay: false, stop: true, enter: true, exit: false, intersectionGiveWay: false);
				}
			}
			_carsInIntersection = new List<int>();
			_intersectionState = TrafficLightsColor.Green;
			ApplyColorChanges();
			_currentTime = Random.Range(0, 10);
		}

		public override bool IsPathFree(int waypointIndex)
		{
			return false;
		}

		public override void PedestrianPassed(int pedestrianIndex)
		{
		}

		public void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
		{
			_trafficLightsBehaviour = trafficLightsBehaviour;
		}

		public override void UpdateIntersection(float realtimeSinceStartup)
		{
			if (_doNotChange)
			{
				return;
			}
			_currentTimer = realtimeSinceStartup - _currentTime;
			switch (_intersectionState)
			{
			case TrafficLightsColor.Green:
				if (_currentTimer > _trafficLightsCrossingData.GreenLightTime)
				{
					_intersectionState = TrafficLightsColor.YellowGreen;
					ApplyColorChanges();
					_currentTime = realtimeSinceStartup;
				}
				break;
			case TrafficLightsColor.YellowGreen:
				if (_currentTimer > _trafficLightsCrossingData.YellowLightTime)
				{
					_intersectionState = TrafficLightsColor.Red;
					ApplyColorChanges();
					_currentTime = realtimeSinceStartup;
				}
				break;
			case TrafficLightsColor.Red:
				if (_currentTimer > _trafficLightsCrossingData.RedLightTime)
				{
					_intersectionState = TrafficLightsColor.YellowRed;
					ApplyColorChanges();
					_currentTime = realtimeSinceStartup;
				}
				break;
			case TrafficLightsColor.YellowRed:
				if (_currentTimer > _trafficLightsCrossingData.YellowLightTime)
				{
					_intersectionState = TrafficLightsColor.Green;
					ApplyColorChanges();
					_currentTime = realtimeSinceStartup;
				}
				break;
			}
		}

		public void SetCrossingState(TrafficLightsColor newColor, bool doNotChangeAgain, float realtimeSinceStartup)
		{
			_intersectionState = newColor;
			ApplyColorChanges();
			_currentTime = realtimeSinceStartup;
			_doNotChange = doNotChangeAgain;
		}

		public override string GetName()
		{
			return _trafficLightsCrossingData.Name;
		}

		public LightsStopWaypoints[] GetWaypoints()
		{
			return _trafficLightsCrossingData.StopWaypoints;
		}

		public override List<int> GetStopWaypoints()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _trafficLightsCrossingData.StopWaypoints.Length; i++)
			{
				list.AddRange(_trafficLightsCrossingData.StopWaypoints[i].roadWaypoints);
			}
			return list;
		}

		public TrafficLightsColor GetCrossingState()
		{
			return _intersectionState;
		}

		public override int[] GetPedStopWaypoint()
		{
			return _trafficLightsCrossingData.PedestrianWaypoints;
		}

		private void ApplyColorChanges()
		{
			UpdateCurrentIntersectionWaypoints(0, _intersectionState != TrafficLightsColor.Green);
			TriggerPedestrianWaypointsUpdate(_intersectionState != TrafficLightsColor.Red);
			_trafficLightsBehaviour?.Invoke(_intersectionState, _trafficLightsCrossingData.StopWaypoints[0].redLightObjects, _trafficLightsCrossingData.StopWaypoints[0].yellowLightObjects, _trafficLightsCrossingData.StopWaypoints[0].greenLightObjects, _trafficLightsCrossingData.Name);
		}

		private void UpdateCurrentIntersectionWaypoints(int road, bool stop)
		{
			for (int i = 0; i < _trafficLightsCrossingData.StopWaypoints[road].roadWaypoints.Length; i++)
			{
				WaypointEvents.TriggerTrafficLightChangedEvent(_trafficLightsCrossingData.StopWaypoints[road].roadWaypoints[i], stop);
			}
		}

		private void GetPedestrianRoads(IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler)
		{
			if (_trafficLightsCrossingData.PedestrianWaypoints.Length != 0)
			{
				pedestrianWaypointsDataHandler.SetIntersection(_trafficLightsCrossingData.PedestrianWaypoints, this);
			}
		}

		private void TriggerPedestrianWaypointsUpdate(bool stop)
		{
		}
	}
}
