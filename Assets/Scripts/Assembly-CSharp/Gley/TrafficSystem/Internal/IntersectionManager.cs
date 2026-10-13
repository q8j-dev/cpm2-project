using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class IntersectionManager : IDestroyable
	{
		private readonly GenericIntersection[] _allIntersections;

		private readonly TrafficLightsIntersection[] _trafficLightsIntersections;

		private readonly PriorityIntersection[] _priorityIntersections;

		private readonly TrafficLightsCrossing[] _trafficLightsCrossings;

		private readonly PriorityCrossing[] _priorityCrossings;

		private List<GenericIntersection> _activeIntersections;

		private TimeManager _timeManager;

		private readonly HashSet<int> _steamCellIntersections = new HashSet<int>();

		private readonly HashSet<int> _steamPreviousIntersections = new HashSet<int>();

		private readonly List<int> _steamIntersectionIndexes = new List<int>();

		public GenericIntersection[] AllIntersections => _allIntersections;

		public TrafficLightsIntersection[] TrafficLightsIntersections => _trafficLightsIntersections;

		public PriorityIntersection[] PriorityIntersections => _priorityIntersections;

		public TrafficLightsCrossing[] TrafficLightsCrossings => _trafficLightsCrossings;

		public PriorityCrossing[] PriorityCrossings => _priorityCrossings;

		public IntersectionManager(IntersectionsData intersectionsData, TrafficWaypointsData trafficWaypointsData, IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler, TrafficLightsBehaviour trafficLightsBehaviour, float greenLightTime, float yellowLightTime, TimeManager timeManager)
		{
			Assign();
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Combine(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
			IntersectionEvents.OnActiveIntersectionsChanged += ActiveIntersectionChangedHandler;
			_activeIntersections = new List<GenericIntersection>();
			GridEvents.OnActiveGridCellsChanged += ActiveGridCellsChangedHandler;
			ActiveIntersectionChangedHandler(_activeIntersections);
			IntersectionDataType[] allIntersections = intersectionsData.AllIntersections;
			_allIntersections = new GenericIntersection[allIntersections.Length];
			List<TrafficLightsIntersection> list = new List<TrafficLightsIntersection>();
			List<PriorityIntersection> list2 = new List<PriorityIntersection>();
			List<TrafficLightsCrossing> list3 = new List<TrafficLightsCrossing>();
			List<PriorityCrossing> list4 = new List<PriorityCrossing>();
			for (int i = 0; i < allIntersections.Length; i++)
			{
				switch (allIntersections[i].Type)
				{
				case IntersectionType.TrafficLights:
					list.Add(new TrafficLightsIntersection(intersectionsData.AllLightsIntersections[allIntersections[i].OtherListIndex], trafficWaypointsData, pedestrianWaypointsDataHandler, trafficLightsBehaviour, greenLightTime, yellowLightTime));
					_allIntersections[i] = list[list.Count - 1];
					break;
				case IntersectionType.Priority:
					list2.Add(new PriorityIntersection(intersectionsData.AllPriorityIntersections[allIntersections[i].OtherListIndex], trafficWaypointsData, pedestrianWaypointsDataHandler));
					_allIntersections[i] = list2[list2.Count - 1];
					break;
				case IntersectionType.LightsCrossing:
					list3.Add(new TrafficLightsCrossing(intersectionsData.AllLightsCrossings[allIntersections[i].OtherListIndex], trafficWaypointsData, pedestrianWaypointsDataHandler, trafficLightsBehaviour));
					_allIntersections[i] = list3[list3.Count - 1];
					break;
				case IntersectionType.PriorityCrossing:
					list4.Add(new PriorityCrossing(intersectionsData.AllPriorityCrossings[allIntersections[i].OtherListIndex], trafficWaypointsData, pedestrianWaypointsDataHandler));
					_allIntersections[i] = list4[list4.Count - 1];
					break;
				}
			}
			_priorityCrossings = list4.ToArray();
			_priorityIntersections = list2.ToArray();
			_trafficLightsCrossings = list3.ToArray();
			_trafficLightsIntersections = list.ToArray();
			_timeManager = timeManager;
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void UpdateIntersections(float realTimeSinceStartup)
		{
			for (int i = 0; i < _activeIntersections.Count; i++)
			{
				_activeIntersections[i].UpdateIntersection(realTimeSinceStartup);
			}
		}

		private void VehicleRemovedHandler(int index)
		{
			for (int i = 0; i < _activeIntersections.Count; i++)
			{
				_activeIntersections[i].RemoveVehicle(index);
			}
		}

		public void SetCrossingState(string crossingName, TrafficLightsColor newColor, bool doNotChangeAgain, float realtimeSinceStartup)
		{
			for (int i = 0; i < _trafficLightsCrossings.Length; i++)
			{
				if (_trafficLightsCrossings[i].GetName() == crossingName)
				{
					_trafficLightsCrossings[i].SetCrossingState(newColor, doNotChangeAgain, realtimeSinceStartup);
					return;
				}
			}
			Debug.LogWarning(crossingName + " not found");
		}

		public TrafficLightsColor GetTrafficLightsCrossingState(string crossingName)
		{
			for (int i = 0; i < _trafficLightsCrossings.Length; i++)
			{
				if (_trafficLightsCrossings[i].GetName() == crossingName)
				{
					return _trafficLightsCrossings[i].GetCrossingState();
				}
			}
			Debug.LogWarning(crossingName + " not found");
			return TrafficLightsColor.Red;
		}

		public bool IsPriorityCrossingRed(string crossingName)
		{
			for (int i = 0; i < _priorityCrossings.Length; i++)
			{
				if (_priorityCrossings[i].GetName() == crossingName)
				{
					return _priorityCrossings[i].GetPriorityCrossingState();
				}
			}
			Debug.LogWarning(crossingName + " not found");
			return false;
		}

		public void SetPriorityCrossingState(string crossingName, bool stop, bool stopUpdate)
		{
			for (int i = 0; i < _priorityCrossings.Length; i++)
			{
				if (_priorityCrossings[i].GetName() == crossingName)
				{
					_priorityCrossings[i].SetPriorityCrossingState(stop, stopUpdate);
				}
			}
		}

		public void SetRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain)
		{
			for (int i = 0; i < _trafficLightsIntersections.Length; i++)
			{
				if (_trafficLightsIntersections[i].GetName() == intersectionName)
				{
					_trafficLightsIntersections[i].SetGreenRoad(roadIndex, doNotChangeAgain, _timeManager.RealTimeSinceStartup);
					return;
				}
			}
			Debug.LogWarning(intersectionName + " not found. Make sure it is a Traffic Lights Intersection.");
		}

		public void SetRoadToYellow(string intersectionName, int roadIndex)
		{
			for (int i = 0; i < _trafficLightsIntersections.Length; i++)
			{
				if (_trafficLightsIntersections[i].GetName() == intersectionName)
				{
					_trafficLightsIntersections[i].SetYellowRoad(roadIndex, _timeManager.RealTimeSinceStartup);
					return;
				}
			}
			Debug.LogWarning(intersectionName + " not found. Make sure it is a Traffic Lights Intersection.");
		}

		public void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
		{
			for (int i = 0; i < _trafficLightsIntersections.Length; i++)
			{
				_trafficLightsIntersections[i].SetTrafficLightsBehaviour(trafficLightsBehaviour);
			}
			for (int j = 0; j < _trafficLightsCrossings.Length; j++)
			{
				_trafficLightsCrossings[j].SetTrafficLightsBehaviour(trafficLightsBehaviour);
			}
		}

		public List<GenericIntersection> GetIntersections(List<int> intersectionIndexes)
		{
			List<GenericIntersection> list = new List<GenericIntersection>();
			for (int i = 0; i < intersectionIndexes.Count; i++)
			{
				list.Add(_allIntersections[intersectionIndexes[i]]);
			}
			return list;
		}

		private void PedestrianRemovedHandler(int pedestrianIndex)
		{
			for (int i = 0; i < _activeIntersections.Count; i++)
			{
				_activeIntersections[i].PedestrianPassed(pedestrianIndex);
			}
		}

		private void ActiveGridCellsChangedHandler(CellData[] activeCells)
		{
			_steamCellIntersections.Clear();
			for (int i = 0; i < activeCells.Length; i++)
			{
				List<int> intersectionsInCell = activeCells[i].IntersectionsInCell;
				for (int j = 0; j < intersectionsInCell.Count; j++)
				{
					_steamCellIntersections.Add(intersectionsInCell[j]);
				}
			}
			if (_steamCellIntersections.SetEquals(_steamPreviousIntersections))
			{
				return;
			}
			_steamPreviousIntersections.Clear();
			_steamIntersectionIndexes.Clear();
			foreach (int steamCellIntersection in _steamCellIntersections)
			{
				_steamPreviousIntersections.Add(steamCellIntersection);
				_steamIntersectionIndexes.Add(steamCellIntersection);
			}
			IntersectionEvents.TriggerActiveIntersectionsChangedEvent(GetIntersections(_steamIntersectionIndexes));
		}

		private void ActiveIntersectionChangedHandler(List<GenericIntersection> activeIntersections)
		{
			for (int i = 0; i < activeIntersections.Count; i++)
			{
				if (_activeIntersections != null && !_activeIntersections.Contains(activeIntersections[i]))
				{
					activeIntersections[i].ResetIntersection();
				}
			}
			_activeIntersections = activeIntersections;
		}

		public void OnDestroy()
		{
			IntersectionEvents.OnActiveIntersectionsChanged -= ActiveIntersectionChangedHandler;
			GridEvents.OnActiveGridCellsChanged -= ActiveGridCellsChangedHandler;
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Remove(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
		}
	}
}
