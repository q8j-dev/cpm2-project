using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class BehaviourManager : IDestroyable
	{
		private readonly Dictionary<string, VehicleBehaviour>[] _supportedBehaviours;

		private readonly Dictionary<string, VehicleBehaviour>[] _activeBehaviours;

		private readonly List<VehicleBehaviour>[] _behavioursToAdd;

		private readonly List<VehicleBehaviour>[] _behavioursToRemove;

		private readonly BehaviourResult[] _appliedBehaviour;

		private readonly List<BehaviourResult>[] _possibleBehaviours;

		private readonly TrafficWaypointsData _trafficWaypointsData;

		private readonly AllVehiclesData _allVehiclesData;

		public BehaviourManager(int nrOfVehicles, IBehaviourList defaultBehaviours, TrafficWaypointsData trafficWaypointsData, AllVehiclesData allVehiclesData)
		{
			_trafficWaypointsData = trafficWaypointsData;
			_allVehiclesData = allVehiclesData;
			Events.OnBehaviourStarted += BehaviourStartedHandler;
			Events.OnBehaviourStopped += BehaviourStoppedHandler;
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Combine(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
			_behavioursToAdd = new List<VehicleBehaviour>[nrOfVehicles];
			_behavioursToRemove = new List<VehicleBehaviour>[nrOfVehicles];
			_activeBehaviours = new Dictionary<string, VehicleBehaviour>[nrOfVehicles];
			_supportedBehaviours = new Dictionary<string, VehicleBehaviour>[nrOfVehicles];
			_appliedBehaviour = new BehaviourResult[nrOfVehicles];
			_possibleBehaviours = new List<BehaviourResult>[nrOfVehicles];
			for (int i = 0; i < nrOfVehicles; i++)
			{
				_behavioursToAdd[i] = new List<VehicleBehaviour>();
				_behavioursToRemove[i] = new List<VehicleBehaviour>();
				SetVehicleBehaviours(i, defaultBehaviours.GetBehaviours());
			}
			Assign();
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void SetAllVehiclesBehaviours(IBehaviourList vehicleBehaviours)
		{
			if (vehicleBehaviours == null)
			{
				Debug.LogError("VehicleBehaviours not implemented");
				return;
			}
			for (int i = 0; i < _activeBehaviours.Length; i++)
			{
				SetVehicleBehaviours(i, vehicleBehaviours.GetBehaviours());
			}
		}

		public void SetVehicleBehaviours(int vehicleIndex, VehicleBehaviour[] vehicleBehaviours)
		{
			if (vehicleBehaviours == null || vehicleBehaviours.Length == 0)
			{
				Debug.LogError("Behaviours list has no elements");
				return;
			}
			if (_supportedBehaviours[vehicleIndex] != null)
			{
				foreach (KeyValuePair<string, VehicleBehaviour> item in _supportedBehaviours[vehicleIndex])
				{
					if (item.Value != null)
					{
						item.Value.OnDestroy();
					}
				}
			}
			_supportedBehaviours[vehicleIndex] = new Dictionary<string, VehicleBehaviour>();
			_activeBehaviours[vehicleIndex] = new Dictionary<string, VehicleBehaviour>();
			for (int i = 0; i < vehicleBehaviours.Length; i++)
			{
				if (_supportedBehaviours[vehicleIndex].TryAdd(vehicleBehaviours[i].Name, vehicleBehaviours[i]))
				{
					vehicleBehaviours[i].Initialize(vehicleIndex, API.GetVehicleComponent(vehicleIndex), _trafficWaypointsData, _allVehiclesData);
				}
			}
		}

		public void StartBehaviour(int vehicleIndex, string behaviourName, object[] parameters)
		{
			VehicleBehaviour behaviour = GetBehaviour(vehicleIndex, behaviourName);
			if (behaviour != null)
			{
				behaviour.Start();
				behaviour.SetParams(parameters);
			}
			else
			{
				Debug.LogError(behaviourName + " not found");
			}
		}

		public BehaviourResult ExecuteBehaviour(MovementInfo knownWaypointsList, int vehicleIndex, float requiredBrakePower, bool stopTargetReached, int currentGear, Vector3 frontTriggerPosition, float3 stopPosition)
		{
			UpdateActiveBehaviours(vehicleIndex);
			BehaviourResult behaviourResult = new BehaviourResult();
			if (_activeBehaviours[vehicleIndex].Count > 0)
			{
				if (_possibleBehaviours[vehicleIndex] == null)
				{
					_possibleBehaviours[vehicleIndex] = new List<BehaviourResult>();
				}
				else
				{
					_possibleBehaviours[vehicleIndex].Clear();
				}
				foreach (KeyValuePair<string, VehicleBehaviour> item in _activeBehaviours[vehicleIndex])
				{
					BehaviourResult behaviourResult2 = item.Value.Execute(knownWaypointsList, requiredBrakePower, stopTargetReached, stopPosition, currentGear);
					_possibleBehaviours[vehicleIndex].Add(behaviourResult2);
					behaviourResult.Append(behaviourResult2, frontTriggerPosition);
				}
			}
			_appliedBehaviour[vehicleIndex] = behaviourResult;
			return behaviourResult;
		}

		public BehaviourResult GetVehicleBehaviour(int vehicleIndex)
		{
			return _appliedBehaviour[vehicleIndex];
		}

		public List<BehaviourResult> GetPossibleBehaviours(int vehicleIndex)
		{
			return _possibleBehaviours[vehicleIndex];
		}

		public void StopBehaviour(int vehicleIndex, string behaviourName)
		{
			GetBehaviour(vehicleIndex, behaviourName)?.Stop();
		}

		private void VehicleRemovedHandler(int vehicleIndex)
		{
			foreach (KeyValuePair<string, VehicleBehaviour> item in _supportedBehaviours[vehicleIndex])
			{
				item.Value.Stop();
			}
			_behavioursToAdd[vehicleIndex] = new List<VehicleBehaviour>();
			_behavioursToRemove[vehicleIndex] = new List<VehicleBehaviour>();
			_activeBehaviours[vehicleIndex] = new Dictionary<string, VehicleBehaviour>();
		}

		public VehicleBehaviour GetBehaviour(int vehicleIndex, string behaviourName)
		{
			if (_supportedBehaviours[vehicleIndex].TryGetValue(behaviourName, out var value))
			{
				return value;
			}
			Debug.LogWarning($"Behaviour {behaviourName} not found on vehicle index {vehicleIndex}");
			return null;
		}

		public Dictionary<string, VehicleBehaviour>[] GetActiveBehaviours()
		{
			return _activeBehaviours;
		}

		private void BehaviourStoppedHandler(int vehicleIndex, VehicleBehaviour behaviour)
		{
			if (_activeBehaviours[vehicleIndex].ContainsKey(behaviour.Name))
			{
				_behavioursToRemove[vehicleIndex].Add(behaviour);
			}
			else
			{
				_behavioursToAdd[vehicleIndex].Remove(behaviour);
			}
		}

		private void BehaviourStartedHandler(int vehicleIndex, VehicleBehaviour behaviour)
		{
			if (!_activeBehaviours[vehicleIndex].ContainsKey(behaviour.Name))
			{
				_behavioursToAdd[vehicleIndex].Add(behaviour);
			}
			else
			{
				_behavioursToRemove[vehicleIndex].Remove(behaviour);
			}
		}

		private void UpdateActiveBehaviours(int vehicleIndex)
		{
			while (_behavioursToAdd[vehicleIndex].Count > 0)
			{
				_activeBehaviours[vehicleIndex].Add(_behavioursToAdd[vehicleIndex][0].Name, _behavioursToAdd[vehicleIndex][0]);
				_behavioursToAdd[vehicleIndex].RemoveAt(0);
			}
			while (_behavioursToRemove[vehicleIndex].Count > 0)
			{
				_activeBehaviours[vehicleIndex].Remove(_behavioursToRemove[vehicleIndex][0].Name);
				_behavioursToRemove[vehicleIndex].RemoveAt(0);
			}
		}

		public void OnDestroy()
		{
			Events.OnBehaviourStarted -= BehaviourStartedHandler;
			Events.OnBehaviourStopped -= BehaviourStoppedHandler;
			Events.OnVehicleDisabled = (Events.VehicleDisabled)Delegate.Remove(Events.OnVehicleDisabled, new Events.VehicleDisabled(VehicleRemovedHandler));
			for (int i = 0; i < _supportedBehaviours.Length; i++)
			{
				if (_supportedBehaviours[i] == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, VehicleBehaviour> item in _supportedBehaviours[i])
				{
					if (_supportedBehaviours[i].Values != null)
					{
						item.Value.OnDestroy();
					}
				}
			}
		}
	}
}
