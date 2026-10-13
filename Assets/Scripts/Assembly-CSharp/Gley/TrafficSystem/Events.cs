using UnityEngine;

namespace Gley.TrafficSystem
{
	public class Events
	{
		public delegate void BehaviourStarted(int vehicleIndex, VehicleBehaviour behaviour);

		public delegate void BehaviourStopped(int vehicleIndex, VehicleBehaviour behaviour);

		public delegate void ChangeDestination(int vehicleIndex, Vector3 newPosition);

		public delegate void DestinationReached(int vehicleIndex);

		public delegate void ObstacleRemoved(int vehicleIndex, ObstacleTypes obstacleType);

		public delegate void ObstaclesUpdated(int vehicleIndex);

		public delegate void VehicleActivated(int vehicleIndex, int waypointIndex);

		public delegate void VehicleCrash(int vehicleIndex, ObstacleTypes obstacleType, Collider other);

		public delegate void VehicleDisabled(int vehicleIndex);

		public delegate void WaypointReached(int vehicleIndex, int waypointIndex, string data);

		public static ChangeDestination OnChangeDestination;

		public static DestinationReached OnDestinationReached;

		public static VehicleActivated OnVehicleActivated;

		public static VehicleDisabled OnVehicleDisabled;

		public static WaypointReached OnWaypointReached;

		public static event BehaviourStarted OnBehaviourStarted;

		public static event BehaviourStopped OnBehaviourStopped;

		public static event ObstacleRemoved OnObstacleRemoved;

		public static event ObstaclesUpdated OnObstaclesUpdated;

		public static event VehicleCrash OnVehicleCrashed;

		public static void TriggerBehaviourStartedEvent(int vehicleIndex, VehicleBehaviour behaviour)
		{
			OnBehaviourStarted?.Invoke(vehicleIndex, behaviour);
		}

		public static void TriggerBehaviourStoppedEvent(int vehicleIndex, VehicleBehaviour behaviour)
		{
			OnBehaviourStopped?.Invoke(vehicleIndex, behaviour);
		}

		public static void TriggerChangeDestinationEvent(int vehicleIndex, Vector3 newPosition)
		{
			OnChangeDestination?.Invoke(vehicleIndex, newPosition);
		}

		public static void TriggerDestinationReachedEvent(int vehicleIndex)
		{
			OnDestinationReached?.Invoke(vehicleIndex);
		}

		public static void TriggerObstacleRemovedEvent(int vehicleIndex, ObstacleTypes obstacleType)
		{
			OnObstacleRemoved?.Invoke(vehicleIndex, obstacleType);
		}

		public static void TriggerObstaclesUpdatedEvent(int vehicleIndex)
		{
			OnObstaclesUpdated?.Invoke(vehicleIndex);
		}

		public static void TriggerVehicleActivatedEvent(int vehicleIndex, int waypointIndex)
		{
			OnVehicleActivated?.Invoke(vehicleIndex, waypointIndex);
		}

		public static void TriggerVehicleCrashEvent(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
		{
			OnVehicleCrashed?.Invoke(vehicleIndex, obstacleType, other);
		}

		public static void TriggerVehicleDisabledEvent(int vehicleIndex)
		{
			OnVehicleDisabled?.Invoke(vehicleIndex);
		}

		public static void TriggerWaypointReachedEvent(int vehicleIndex, int waypointIndex, string data)
		{
			OnWaypointReached?.Invoke(vehicleIndex, waypointIndex, data);
		}
	}
}
