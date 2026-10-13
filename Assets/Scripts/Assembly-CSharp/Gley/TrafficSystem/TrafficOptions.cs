using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class TrafficOptions
	{
		public float MinDistanceToAdd = -1f;

		public float DistanceToRemove = -1f;

		public float MasterVolume = 1f;

		public bool UseWaypointPriority;

		public float GreenLightTime = -1f;

		public float YellowLightTime = -1f;

		public float MinOffset = -0.25f;

		public float MaxOffset = 0.25f;

		public int ActiveSquaresLevel = 1;

		public int InitialDensity = -1;

		public int DefaultPathLength = 5;

		public bool LightsOn;

		public List<Color> ColorPool;

		public Area DisableWaypointsArea;

		public IBehaviourImplementation BehaviourImplementation;

		public Func<GameObject, Transform, GameObject> InstantiateVehicle;

		private TrafficLightsBehaviour trafficLightsBehaviour;

		private SpawnWaypointSelector spawnWaypointSelector;

		private ModifyTriggerSize modifyTriggerSize;

		private IBehaviourList _vehicleBehaviours;

		public TrafficLightsBehaviour TrafficLightsBehaviour
		{
			get
			{
				if (trafficLightsBehaviour == null)
				{
					trafficLightsBehaviour = DefaultDelegates.TrafficLightBehaviour;
				}
				return trafficLightsBehaviour;
			}
			set
			{
				trafficLightsBehaviour = value;
			}
		}

		public SpawnWaypointSelector SpawnWaypointSelector
		{
			get
			{
				if (spawnWaypointSelector == null)
				{
					spawnWaypointSelector = DefaultDelegates.GetRandomSpawnWaypoint;
				}
				return spawnWaypointSelector;
			}
			set
			{
				spawnWaypointSelector = value;
			}
		}

		public ModifyTriggerSize ModifyTriggerSize
		{
			get
			{
				if (modifyTriggerSize == null)
				{
					modifyTriggerSize = DefaultDelegates.TriggerSizeModifier;
				}
				return modifyTriggerSize;
			}
			set
			{
				modifyTriggerSize = value;
			}
		}

		public IBehaviourList VehicleBehaviours
		{
			get
			{
				if (_vehicleBehaviours == null)
				{
					_vehicleBehaviours = new DefaultVehicleBehaviours();
				}
				return _vehicleBehaviours;
			}
			set
			{
				_vehicleBehaviours = value;
			}
		}
	}
}
