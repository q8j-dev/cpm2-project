using System;
using System.Collections.Generic;
using System.Linq;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class VehicleAI : IDestroyable
	{
		public delegate void NewWaypointRequested(int vehicleIndex);

		private readonly AllVehiclesData _allVehiclesData;

		private readonly TrafficWaypointsData _trafficWaypointsData;

		private readonly SoundManager _soundManager;

		private readonly TimeManager _timeManager;

		private readonly PlayerWaypointsManager _playerWaypointsManager;

		private readonly int _knownWaypoints;

		private bool _added;

		public static event NewWaypointRequested OnNewWaypointRequested;

		public static void TriggerNewWaypointRequestedEvent(int vehicleIndex)
		{
			OnNewWaypointRequested?.Invoke(vehicleIndex);
		}

		public VehicleAI(AllVehiclesData allVehiclesData, TrafficWaypointsData trafficWaypointsData, SoundManager soundManager, TimeManager timeManager, int knownWaypoints, PlayerWaypointsManager playerWaypointsManager)
		{
			_allVehiclesData = allVehiclesData;
			_trafficWaypointsData = trafficWaypointsData;
			_soundManager = soundManager;
			_timeManager = timeManager;
			_knownWaypoints = knownWaypoints;
			_playerWaypointsManager = playerWaypointsManager;
			Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Combine(Events.OnVehicleActivated, new Events.VehicleActivated(VehicleAddedHandler));
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Combine(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
			WaypointEvents.OnStopStateChanged += StopStateChangedHandler;
			WaypointEvents.OnGiveWayStateChanged += GiveWayStateChangedHandler;
			GiveWay.OnPassageGranted = (GiveWay.PassageGranted)Delegate.Combine(GiveWay.OnPassageGranted, new GiveWay.PassageGranted(PassageGrantedHandler));
			VehicleEvents.OnObstacleInTriggerAdded += ObstacleInTriggerAddedHandler;
			VehicleEvents.OnObstacleInTriggerRemoved += ObstacleInTriggerRemovedHandler;
			Assign();
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void Drive(int vehicleIndex, int currentGear, bool waypointRequested, VehicleTypes vehicleType)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetClosestObstacleAndSpeed(_allVehiclesData.AllVehicles[vehicleIndex].FrontTrigger.position);
			if (waypointRequested)
			{
				TriggerNewWaypointRequestedEvent(vehicleIndex);
				if (!_added)
				{
					UpdateKnownWaypointsList(vehicleIndex, vehicleType);
					_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.IncreaseActivePosition();
					RemoveOldWaypoint(vehicleIndex);
					_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.UpdateCoveredWaypoints(_allVehiclesData.AllVehicles[vehicleIndex].BackPosition.position, _allVehiclesData.AllVehicles[vehicleIndex].BackPosition.forward);
					Events.TriggerChangeDestinationEvent(vehicleIndex, _allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.GetFirstPosition());
				}
				_added = false;
			}
			_allVehiclesData.AllVehicles[vehicleIndex].UpdateVehicleScripts(_soundManager.MasterVolume, _timeManager.RealTimeSinceStartup, currentGear < 0);
		}

		public void AddWaypointAndClear(int waypointIndex, int vehicleIndex)
		{
			_added = true;
			TrafficWaypoint waypointFromIndex = _trafficWaypointsData.GetWaypointFromIndex(waypointIndex);
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.AddOtherLaneWaypoint(waypointIndex, _trafficWaypointsData.AllTrafficWaypoints[waypointIndex].Position, waypointFromIndex.MaxSpeed, waypointFromIndex.Stop, waypointFromIndex.GetGiveWayState(), 0);
			Events.TriggerChangeDestinationEvent(vehicleIndex, _trafficWaypointsData.AllTrafficWaypoints[waypointIndex].Position);
			UpdateKnownWaypointsList(vehicleIndex, _allVehiclesData.AllVehicles[vehicleIndex].VehicleType);
		}

		public bool AllPreviousWaypointsAreFree(int waypointIndex, int vehicleIndexThatMadeTheRequest, bool ignoreTime)
		{
			if (waypointIndex == -1)
			{
				return false;
			}
			if (IsThisWaypointAPlayerDestination(waypointIndex))
			{
				return false;
			}
			for (int i = 0; i < _allVehiclesData.AllVehicles.Length; i++)
			{
				if (i != vehicleIndexThatMadeTheRequest && _allVehiclesData.AllVehicles[i].MovementInfo.IsVehicleOnThisWaypoint(waypointIndex))
				{
					return false;
				}
			}
			if (ignoreTime)
			{
				return true;
			}
			VehicleComponent vehicleComponent = _allVehiclesData.AllVehicles[vehicleIndexThatMadeTheRequest];
			TrafficWaypoint trafficWaypoint = _trafficWaypointsData.AllTrafficWaypoints[waypointIndex];
			float f = Vector3.SqrMagnitude(vehicleComponent.FrontPosition.position - trafficWaypoint.Position);
			float timeToCoverDistance = vehicleComponent.GetTimeToCoverDistance(Mathf.Sqrt(f));
			for (int j = 0; j < _allVehiclesData.AllVehicles.Length; j++)
			{
				if (j == vehicleIndexThatMadeTheRequest || _allVehiclesData.AllVehicles[j].MovementInfo.GetWaypointIndex(0) != waypointIndex)
				{
					continue;
				}
				float f2 = Vector3.SqrMagnitude(_allVehiclesData.AllVehicles[j].FrontPosition.position - trafficWaypoint.Position);
				if (_allVehiclesData.AllVehicles[j].GetTimeToCoverDistance(Mathf.Sqrt(f2)) + 2f < timeToCoverDistance)
				{
					if (Enumerable.Contains(vehicleComponent.AllColliders, _allVehiclesData.AllVehicles[j].MovementInfo.ClosestObstacle.Collider))
					{
						return true;
					}
					return false;
				}
			}
			float maxDistance = vehicleComponent.MovementInfo.GetFirstWaypointSpeed() * timeToCoverDistance;
			if (ArePrevsWaypointsATarget(trafficWaypoint, maxDistance, vehicleIndexThatMadeTheRequest, out var closestVehicleIndex))
			{
				if (closestVehicleIndex == -1)
				{
					return false;
				}
				VehicleComponent vehicleComponent2 = _allVehiclesData.AllVehicles[closestVehicleIndex];
				float f3 = Vector3.SqrMagnitude(vehicleComponent2.FrontPosition.position - trafficWaypoint.Position);
				float timeToCoverDistance2 = vehicleComponent2.GetTimeToCoverDistance(Mathf.Sqrt(f3));
				if (timeToCoverDistance2 + 2f < timeToCoverDistance)
				{
					if (Enumerable.Contains(vehicleComponent.AllColliders, vehicleComponent2.MovementInfo.ClosestObstacle.Collider))
					{
						return true;
					}
					return false;
				}
				if (Vector3.Dot(vehicleComponent2.FrontPosition.position - vehicleComponent.FrontPosition.position, vehicleComponent2.transform.forward) > 0f)
				{
					return false;
				}
				if (vehicleComponent2.GetCurrentSpeedMS() > 5f && timeToCoverDistance2 - timeToCoverDistance < 2f && vehicleComponent2.GetCurrentSpeedMS() > vehicleComponent.GetCurrentSpeedMS())
				{
					return false;
				}
				return true;
			}
			return true;
		}

		public bool AllRequiredWaypointsAreFree(int waypointIndex, int vehicleIndex)
		{
			int[] giveWayList = _trafficWaypointsData.AllTrafficWaypoints[waypointIndex].GiveWayList;
			foreach (int num in giveWayList)
			{
				if (_playerWaypointsManager.IsThisWaypointIndexATarget(num))
				{
					return false;
				}
				VehicleComponent[] allVehicles = _allVehiclesData.AllVehicles;
				foreach (VehicleComponent vehicleComponent in allVehicles)
				{
					if (vehicleComponent.ListIndex != vehicleIndex && vehicleComponent.MovementInfo.GetCurrentWaypointIndex() == num)
					{
						return false;
					}
					if (vehicleComponent.ListIndex != vehicleIndex && vehicleComponent.MovementInfo.IsVehicleOnThisWaypoint(num))
					{
						return false;
					}
				}
				if (_trafficWaypointsData.AllTrafficWaypoints[num].Stop)
				{
					return true;
				}
			}
			return true;
		}

		public bool IsThisWaypointADestination(int waypointIndex, int vehicleIndexThatMadeTheRequest)
		{
			if (_playerWaypointsManager.IsThisWaypointIndexATarget(waypointIndex))
			{
				return true;
			}
			if (_trafficWaypointsData.AllTrafficWaypoints[waypointIndex].Stop)
			{
				return false;
			}
			VehicleComponent[] allVehicles = _allVehiclesData.AllVehicles;
			foreach (VehicleComponent vehicleComponent in allVehicles)
			{
				if (vehicleComponent.ListIndex != vehicleIndexThatMadeTheRequest && vehicleComponent.MovementInfo.GetCurrentWaypointIndex() == waypointIndex)
				{
					if (vehicleComponent.GetCurrentSpeedMS() < 2f && Enumerable.Contains(_allVehiclesData.AllVehicles[vehicleIndexThatMadeTheRequest].AllColliders, vehicleComponent.MovementInfo.ClosestObstacle.Collider))
					{
						return false;
					}
					return true;
				}
				if (vehicleComponent.ListIndex != vehicleIndexThatMadeTheRequest && vehicleComponent.MovementInfo.IsVehicleOnThisWaypoint(waypointIndex))
				{
					return true;
				}
			}
			return false;
		}

		private bool ArePrevsWaypointsATarget(Waypoint startWaypoint, float maxDistance, int vehicleIndexThatMadeTheRequest, out int closestVehicleIndex)
		{
			Queue<Waypoint> queue = new Queue<Waypoint>();
			HashSet<int> hashSet = new HashSet<int>();
			queue.Enqueue(startWaypoint);
			while (queue.Count > 0)
			{
				Waypoint waypoint = queue.Dequeue();
				if (!hashSet.Add(waypoint.ListIndex))
				{
					continue;
				}
				if (IsThisWaypointADestination(waypoint.ListIndex, vehicleIndexThatMadeTheRequest, out closestVehicleIndex))
				{
					return true;
				}
				_ = waypoint.Prevs;
				int[] prevs = waypoint.Prevs;
				foreach (int num in prevs)
				{
					TrafficWaypoint trafficWaypoint = _trafficWaypointsData.AllTrafficWaypoints[num];
					if (Vector3.SqrMagnitude(startWaypoint.Position - trafficWaypoint.Position) < maxDistance * maxDistance)
					{
						queue.Enqueue(trafficWaypoint);
					}
				}
			}
			closestVehicleIndex = -1;
			return false;
		}

		private bool IsThisWaypointADestination(int waypointIndex, int vehicleIndexThatMadeTheRequest, out int closestVehicleIndex)
		{
			float num = float.MaxValue;
			bool result = false;
			closestVehicleIndex = -1;
			for (int i = 0; i < _allVehiclesData.AllVehicles.Length; i++)
			{
				if (i != vehicleIndexThatMadeTheRequest && _allVehiclesData.AllVehicles[i].MovementInfo.WaypointCanBeReached(waypointIndex))
				{
					float num2 = Vector3.SqrMagnitude(_allVehiclesData.AllVehicles[i].FrontPosition.position - _trafficWaypointsData.AllTrafficWaypoints[waypointIndex].Position);
					if (num2 < num)
					{
						num = num2;
						closestVehicleIndex = i;
					}
					result = true;
				}
			}
			return result;
		}

		private void VehicleAddedHandler(int vehicleIndex, int waypointIndex)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetMaxVehicleSpeed(_allVehiclesData.AllVehicles[vehicleIndex].MaxSpeed);
			if (!_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.HasPath)
			{
				AddWaypointToPath(vehicleIndex, waypointIndex, 0);
			}
			UpdateKnownWaypointsList(vehicleIndex, _allVehiclesData.AllVehicles[vehicleIndex].VehicleType);
		}

		private void VehicleRemovedHandler(int vehicleIndex)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.ClearTarget();
		}

		private void StopStateChangedHandler(int waypointIndex, bool stopState)
		{
			for (int i = 0; i < _allVehiclesData.AllVehicles.Length; i++)
			{
				if (_allVehiclesData.AllVehicles[i].MovementInfo.TrySetStopWaypoint(waypointIndex, stopState))
				{
					RemoveOldWaypoint(i);
				}
			}
		}

		private void GiveWayStateChangedHandler(int waypointIndex, GiveWayType giveWayType)
		{
			for (int i = 0; i < _allVehiclesData.AllVehicles.Length; i++)
			{
				if (_allVehiclesData.AllVehicles[i].MovementInfo.TrySetGiveWayWaypoint(waypointIndex, giveWayType))
				{
					RemoveOldWaypoint(i);
				}
			}
		}

		private void PassageGrantedHandler(int vehicleIndex, int waypointIndex)
		{
			if (_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.TrySetGiveWayWaypoint(waypointIndex, GiveWayType.None))
			{
				RemoveOldWaypoint(vehicleIndex);
			}
			else
			{
				Debug.Log("Something is not good " + vehicleIndex);
			}
		}

		private void ObstacleInTriggerRemovedHandler(int vehicleIndex, Obstacle obstacleToRemove)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetObstacles(_allVehiclesData.AllVehicles[vehicleIndex].Obstacles, _allVehiclesData.AllVehicles[vehicleIndex].FrontTrigger.position);
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.RemoveObstacle(obstacleToRemove.ObstacleType);
		}

		private void ObstacleInTriggerAddedHandler(int vehicleIndex, Obstacle newObstacle)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetObstacles(_allVehiclesData.AllVehicles[vehicleIndex].Obstacles, _allVehiclesData.AllVehicles[vehicleIndex].FrontTrigger.position);
		}

		private void RemoveOldWaypoint(int vehicleIndex)
		{
			while (_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.IsAllowedToChange())
			{
				_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.TargetPassed();
				int oldWaypointIndex = _allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.OldWaypointIndex;
				if (oldWaypointIndex == -1)
				{
					continue;
				}
				TrafficWaypoint waypointFromIndex = _trafficWaypointsData.GetWaypointFromIndex(oldWaypointIndex);
				_allVehiclesData.AllVehicles[vehicleIndex].BlinkersController.UpdateBlinkers(waypointFromIndex.BlinkType);
				if (waypointFromIndex.Enter)
				{
					foreach (IIntersection associatedIntersection in _trafficWaypointsData.AllTrafficWaypoints[oldWaypointIndex].AssociatedIntersections)
					{
						associatedIntersection.VehicleEnter(vehicleIndex);
					}
				}
				if (waypointFromIndex.Exit)
				{
					foreach (IIntersection associatedIntersection2 in _trafficWaypointsData.AllTrafficWaypoints[oldWaypointIndex].AssociatedIntersections)
					{
						associatedIntersection2.VehicleLeft(vehicleIndex);
					}
				}
				if (waypointFromIndex.TriggerEvent)
				{
					Events.TriggerWaypointReachedEvent(vehicleIndex, oldWaypointIndex, _trafficWaypointsData.AllTrafficWaypoints[oldWaypointIndex].EventData);
				}
			}
		}

		private void AddWaypointToPath(int vehicleIndex, int waypointIndex, int angle)
		{
			TrafficWaypoint trafficWaypoint = _trafficWaypointsData.AllTrafficWaypoints[waypointIndex];
			_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.AddWaypointAsTarget(waypointIndex, trafficWaypoint.Position, trafficWaypoint.MaxSpeed, trafficWaypoint.Stop, trafficWaypoint.GetGiveWayState(), angle);
		}

		private bool IsThisWaypointAPlayerDestination(int waypointIndex)
		{
			return _playerWaypointsManager.IsThisWaypointIndexATarget(waypointIndex);
		}

		private void UpdateKnownWaypointsList(int vehicleIndex, VehicleTypes vehicleType)
		{
			bool flag = true;
			while (_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.PathLength <= _knownWaypoints && flag)
			{
				flag = ((!_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.HasPath) ? AddRandomWaypoint(vehicleIndex, vehicleType) : AddCustomPathWaypoint(vehicleIndex));
			}
		}

		private bool AddCustomPathWaypoint(int vehicleIndex)
		{
			if (_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.CustomPath != null && _allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.CustomPath.Count > 0)
			{
				TrafficWaypoint waypointFromIndex = _trafficWaypointsData.GetWaypointFromIndex(_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.CustomPath.Dequeue());
				AddWaypointToPath(vehicleIndex, waypointFromIndex.ListIndex, 0);
				return true;
			}
			if (_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.PathLength == 1 && _allVehiclesData.AllVehicles[vehicleIndex].GetCurrentSpeedMS() < 0.1f)
			{
				Events.TriggerDestinationReachedEvent(vehicleIndex);
			}
			return false;
		}

		public bool AddRandomWaypoint(int vehicleIndex, VehicleTypes vehicleType)
		{
			VehicleComponent vehicleComponent = _allVehiclesData.AllVehicles[vehicleIndex];
			int lastWaypointIndex = vehicleComponent.MovementInfo.GetLastWaypointIndex();
			if (lastWaypointIndex == -1)
			{
				Debug.LogWarning($"Vehicle {vehicleIndex} has no valid last waypoint.");
				return false;
			}
			List<NeighborStruct> neighborsWithConditions = _trafficWaypointsData.GetNeighborsWithConditions(lastWaypointIndex, vehicleType);
			if (neighborsWithConditions.Count > 0)
			{
				NeighborStruct neighborStruct = neighborsWithConditions[UnityEngine.Random.Range(0, neighborsWithConditions.Count)];
				AddWaypointToPath(vehicleIndex, neighborStruct.WaypointIndex, neighborStruct.Angle);
				return true;
			}
			List<int> otherLanesWithConditions = _trafficWaypointsData.GetOtherLanesWithConditions(lastWaypointIndex, vehicleType);
			if (otherLanesWithConditions.Count > 0)
			{
				int waypointIndex = otherLanesWithConditions[UnityEngine.Random.Range(0, otherLanesWithConditions.Count)];
				vehicleComponent.MovementInfo.SetGiveWayState(vehicleComponent.MovementInfo.PathLength - 1, GiveWayType.Standard);
				AddWaypointToPath(vehicleIndex, waypointIndex, 0);
				return true;
			}
			return false;
		}

		public void OnDestroy()
		{
			Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Remove(Events.OnVehicleActivated, new Events.VehicleActivated(VehicleAddedHandler));
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Remove(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
			WaypointEvents.OnStopStateChanged -= StopStateChangedHandler;
			WaypointEvents.OnGiveWayStateChanged -= GiveWayStateChangedHandler;
			GiveWay.OnPassageGranted = (GiveWay.PassageGranted)Delegate.Remove(GiveWay.OnPassageGranted, new GiveWay.PassageGranted(PassageGrantedHandler));
			VehicleEvents.OnObstacleInTriggerAdded -= ObstacleInTriggerAddedHandler;
			VehicleEvents.OnObstacleInTriggerRemoved -= ObstacleInTriggerRemovedHandler;
		}
	}
}
