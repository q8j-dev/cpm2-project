using System;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class DefaultBehaviourImplementation : IBehaviourImplementation, IDestroyable
	{
		private VehicleComponent[] _allVehicles;

		private int _knownWaypoints;

		public IBehaviourImplementation Initialize(params object[] parameters)
		{
			Assign();
			_allVehicles = (VehicleComponent[])parameters[0];
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
			MovementInfo.OnStopWaypointsUpdated += StopWaypointsUpdatedHandler;
			Events.OnObstaclesUpdated += OnObstaclesUpdatedHandler;
			Events.OnObstacleRemoved += ObstacleRemovedHandler;
			MovementInfo.OnGiveWayWaypointsUpdated += GiveWayWaypointsUpdatedHandler;
			MovementInfo.OnSlowDownWaypointsUpdated += SlowDownWaypointsUpdatedHandler;
			Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Combine(Events.OnVehicleActivated, new Events.VehicleActivated(VehicleAddedHandler));
			Events.OnVehicleCrashed += VehicleCrashedHandler;
			_knownWaypoints = (int)parameters[1];
			return this;
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (_allVehicles[vehicleIndex].MovementInfo.RemainingPathLength < _knownWaypoints)
			{
				if (!_allVehicles[vehicleIndex].MovementInfo.NoWaypoints)
				{
					_allVehicles[vehicleIndex].MovementInfo.NoWaypoints = true;
					API.StartVehicleBehaviour<NoWaypoints>(vehicleIndex, Array.Empty<object>());
				}
			}
			else if (_allVehicles[vehicleIndex].MovementInfo.NoWaypoints)
			{
				_allVehicles[vehicleIndex].MovementInfo.NoWaypoints = false;
				API.StopVehicleBehaviour<NoWaypoints>(vehicleIndex);
			}
		}

		private void StopWaypointsUpdatedHandler(int vehicleIndex)
		{
			if (_allVehicles[vehicleIndex].MovementInfo.HasStopWaypoints())
			{
				API.StartVehicleBehaviour<StopInPoint>(vehicleIndex, Array.Empty<object>());
			}
			else
			{
				API.StopVehicleBehaviour<StopInPoint>(vehicleIndex);
			}
		}

		private void OnObstaclesUpdatedHandler(int vehicleIndex)
		{
			switch (_allVehicles[vehicleIndex].MovementInfo.ClosestObstacle.ObstacleType)
			{
			case ObstacleTypes.TrafficVehicle:
				if (ShouldFollow(vehicleIndex, _allVehicles[vehicleIndex].MovementInfo.ClosestObstacle))
				{
					API.StartVehicleBehaviour<FollowVehicle>(vehicleIndex, Array.Empty<object>());
				}
				break;
			case ObstacleTypes.Player:
				PlayerSeen(vehicleIndex, _allVehicles[vehicleIndex].MovementInfo.ClosestObstacle);
				break;
			case ObstacleTypes.DynamicObject:
				API.StartVehicleBehaviour<StopInDistance>(vehicleIndex, Array.Empty<object>());
				break;
			case ObstacleTypes.StaticObject:
				API.StartVehicleBehaviour<AvoidReverse>(vehicleIndex, Array.Empty<object>());
				break;
			}
		}

		private void PlayerSeen(int vehicleIndex, Obstacle closestObstacle)
		{
			if (closestObstacle.Collider == null || closestObstacle.Collider.attachedRigidbody == null)
			{
				API.StartVehicleBehaviour<StopInDistance>(vehicleIndex, Array.Empty<object>());
			}
			else if (closestObstacle.Collider.attachedRigidbody.GetComponent<ITrafficParticipant>() == null)
			{
				Debug.LogWarning(closestObstacle.Collider.name + " is a vehicle but does not implement the ITrafficParticipant interface. If this is your player, attach the PlayerComponent or your own implementation of the interface.");
			}
			else
			{
				API.StartVehicleBehaviour<FollowPlayer>(vehicleIndex, Array.Empty<object>());
			}
		}

		private bool IsSameOrientation(Vector3 heading1, Vector3 heading2)
		{
			if (Vector3.Dot(heading1.normalized, heading2.normalized) > 0f)
			{
				return true;
			}
			return false;
		}

		private void ObstacleRemovedHandler(int vehicleIndex, ObstacleTypes obstacleType)
		{
			switch (obstacleType)
			{
			case ObstacleTypes.Player:
				API.StopVehicleBehaviour<FollowPlayer>(vehicleIndex);
				API.StopVehicleBehaviour<OvertakePlayer>(vehicleIndex);
				break;
			case ObstacleTypes.StaticObject:
				API.StopVehicleBehaviour<AvoidReverse>(vehicleIndex);
				break;
			case ObstacleTypes.DynamicObject:
				API.StopVehicleBehaviour<StopInDistance>(vehicleIndex);
				break;
			case ObstacleTypes.TrafficVehicle:
				API.StopVehicleBehaviour<FollowVehicle>(vehicleIndex);
				API.StopVehicleBehaviour<Overtake>(vehicleIndex);
				break;
			default:
				Debug.Log($" Obstacle type {obstacleType} not implemented");
				break;
			case ObstacleTypes.Other:
			case ObstacleTypes.Road:
				break;
			}
		}

		private void GiveWayWaypointsUpdatedHandler(int vehicleIndex)
		{
			if (_allVehicles[vehicleIndex].MovementInfo.HasGiveWayWaypoints())
			{
				API.StartVehicleBehaviour<GiveWay>(vehicleIndex, Array.Empty<object>());
			}
			else
			{
				API.StopVehicleBehaviour<GiveWay>(vehicleIndex);
			}
		}

		private void SlowDownWaypointsUpdatedHandler(int vehicleIndex)
		{
			if (_allVehicles[vehicleIndex].MovementInfo.HasSlowDownWaypoints())
			{
				API.StartVehicleBehaviour<Decelerate>(vehicleIndex, Array.Empty<object>());
			}
			else
			{
				API.StopVehicleBehaviour<Decelerate>(vehicleIndex);
			}
		}

		private void VehicleAddedHandler(int vehicleIndex, int waypointIndex)
		{
			API.StartVehicleBehaviour<Forward>(vehicleIndex, Array.Empty<object>());
			API.StartVehicleBehaviour<CurveSlowDown>(vehicleIndex, Array.Empty<object>());
		}

		private void VehicleCrashedHandler(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
		{
			switch (obstacleType)
			{
			case ObstacleTypes.Player:
			case ObstacleTypes.DynamicObject:
			case ObstacleTypes.StaticObject:
				API.StartVehicleBehaviour<TempStop>(vehicleIndex, Array.Empty<object>());
				break;
			case ObstacleTypes.TrafficVehicle:
				CheckWhoIsInFront(vehicleIndex, other);
				break;
			}
		}

		private void CheckWhoIsInFront(int vehicleIndex, Collider other)
		{
			if (!(other.attachedRigidbody != null))
			{
				return;
			}
			VehicleComponent component = other.attachedRigidbody.GetComponent<VehicleComponent>();
			if (component != null)
			{
				VehicleComponent vehicleComponent = _allVehicles[vehicleIndex];
				if (Vector3.Dot(component.FrontPosition.position - vehicleComponent.FrontPosition.position, vehicleComponent.transform.forward) > 0f)
				{
					API.StartVehicleBehaviour<TempStop>(vehicleIndex, Array.Empty<object>());
				}
			}
		}

		private bool ShouldFollow(int vehicleIndex, Obstacle closestObstacle)
		{
			if (closestObstacle.Collider == null)
			{
				return false;
			}
			if (closestObstacle.Collider.attachedRigidbody == null)
			{
				return false;
			}
			ITrafficParticipant component = closestObstacle.Collider.attachedRigidbody.GetComponent<ITrafficParticipant>();
			if (component.AlreadyCollidingWith(_allVehicles[vehicleIndex].AllColliders))
			{
				return false;
			}
			if (!IsSameOrientation(_allVehicles[vehicleIndex].GetHeading(), component.GetHeading()))
			{
				return false;
			}
			return true;
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void OnDestroy()
		{
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
			MovementInfo.OnStopWaypointsUpdated -= StopWaypointsUpdatedHandler;
			Events.OnObstaclesUpdated -= OnObstaclesUpdatedHandler;
			Events.OnObstacleRemoved -= ObstacleRemovedHandler;
			MovementInfo.OnGiveWayWaypointsUpdated -= GiveWayWaypointsUpdatedHandler;
			MovementInfo.OnSlowDownWaypointsUpdated -= SlowDownWaypointsUpdatedHandler;
			Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Remove(Events.OnVehicleActivated, new Events.VehicleActivated(VehicleAddedHandler));
			Events.OnVehicleCrashed -= VehicleCrashedHandler;
		}
	}
}
