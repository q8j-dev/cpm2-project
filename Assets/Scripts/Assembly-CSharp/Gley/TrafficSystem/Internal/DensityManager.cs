using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Gley.TrafficSystem.Internal
{
	public class DensityManager
	{
		private class VehicleRequest
		{
			public UnityAction<VehicleComponent, int> CompleteMethod { get; set; }

			public List<int> Path { get; set; }

			public VehicleComponent Vehicle { get; set; }

			public VehicleTypes Type { get; set; }

			public Category Category { get; set; }

			public TrafficWaypoint Waypoint { get; set; }

			public bool IgnoreLOS { get; set; }

			public Quaternion TrailerRotation { get; set; }

			internal VehicleRequest(TrafficWaypoint waypoint, VehicleTypes type, Category category, VehicleComponent vehicle, UnityAction<VehicleComponent, int> completeMethod, List<int> path, bool ignoreLos)
			{
				Waypoint = waypoint;
				Type = type;
				Category = category;
				Vehicle = vehicle;
				CompleteMethod = completeMethod;
				Path = path;
				IgnoreLOS = ignoreLos;
				TrailerRotation = Quaternion.identity;
			}
		}

		private enum Category
		{
			Random = 0,
			User = 1
		}

		private readonly List<VehicleRequest> _requestedVehicles;

		private readonly AllVehiclesData _allVehiclesData;

		private readonly IdleVehiclesData _idleVehiclesData;

		private readonly PositionValidator _positionValidator;

		private readonly WaypointSelector _waypointSelector;

		private readonly TrafficWaypointsData _trafficWaypointsData;

		private readonly bool _useWaypointPriority;

		private readonly bool _debugDensity;

		private int _maxNrOfVehicles;

		private int _currentNrOfVehicles;

		private int _activeSquaresLevel;

		private int _newVehiclesNeeded;

		private Vector3 _lastPlayerPosition;

		private Vector3 _lastPlayerDirection;

		private Vector3 _lastCameraPosition;

		public DensityManager(AllVehiclesData allVehiclesData, TrafficWaypointsData trafficWaypointsData, PositionValidator positionValidator, NativeArray<float3> activeCameraPositions, int maxNrOfVehicles, Vector3 playerPosition, Vector3 playerDirection, int activeSquaresLevel, bool useWaypointPriority, int initialDensity, bool debugDensity, WaypointSelector waypointSelector)
		{
			_positionValidator = positionValidator;
			_allVehiclesData = allVehiclesData;
			_activeSquaresLevel = activeSquaresLevel;
			_maxNrOfVehicles = maxNrOfVehicles;
			_useWaypointPriority = useWaypointPriority;
			_trafficWaypointsData = trafficWaypointsData;
			_requestedVehicles = new List<VehicleRequest>();
			_debugDensity = debugDensity;
			_waypointSelector = waypointSelector;
			List<VehicleComponent> list = new List<VehicleComponent>();
			for (int i = 0; i < maxNrOfVehicles; i++)
			{
				VehicleComponent vehicle = _allVehiclesData.GetVehicle(i);
				if (!vehicle.Ignored)
				{
					list.Add(vehicle);
				}
			}
			_idleVehiclesData = new IdleVehiclesData(list);
			if (initialDensity >= 0)
			{
				SetTrafficDensity(initialDensity);
			}
			for (int j = 0; j < _maxNrOfVehicles; j++)
			{
				RequestRandomVehicle(playerPosition, playerDirection, ignorLOS: true, activeCameraPositions[UnityEngine.Random.Range(0, activeCameraPositions.Length)]);
			}
			UpdateVehicleDensity(default, default, default);
			ClearUninstantiatedRequests();
		}

		public void UpdateVehicleDensity(Vector3 playerPosition, Vector3 playerDirection, Vector3 activeCameraPosition)
		{
			_lastPlayerPosition = playerPosition;
			_lastPlayerDirection = playerDirection;
			_lastCameraPosition = activeCameraPosition;
			_newVehiclesNeeded = _maxNrOfVehicles - _currentNrOfVehicles;
			if (_newVehiclesNeeded <= 0)
			{
				return;
			}
			if (_newVehiclesNeeded > _requestedVehicles.Count)
			{
				RequestRandomVehicle(playerPosition, playerDirection, ignorLOS: false, activeCameraPosition);
			}
			for (int num = _requestedVehicles.Count - 1; num >= 0; num--)
			{
				if (RequestIsValid(_requestedVehicles[num]))
				{
					InstantiateVehicle(_requestedVehicles[num]);
				}
			}
		}

		public void RequestIgnoredVehicle(int vehicleIndex, Vector3 position, UnityAction<VehicleComponent, int> completeMethod)
		{
			if (position == Vector3.zero)
			{
				return;
			}
			if (!_allVehiclesData.AllVehicles[vehicleIndex].Ignored)
			{
				Debug.LogWarning($"vehicleIndex {vehicleIndex} is not marked as ignored, it will not be instantiated");
				return;
			}
			VehicleTypes vehicleType = _allVehiclesData.AllVehicles[vehicleIndex].VehicleType;
			TrafficWaypoint closestSpawnWaypoint = _waypointSelector.GetClosestSpawnWaypoint(position, vehicleType);
			if (closestSpawnWaypoint != null)
			{
				_requestedVehicles.Add(new VehicleRequest(closestSpawnWaypoint, vehicleType, Category.User, _allVehiclesData.AllVehicles[vehicleIndex], completeMethod, null, ignoreLos: true));
			}
			else
			{
				Debug.LogWarning("No waypoint found!");
			}
		}

		public void RequestVehicleAtPosition(Vector3 position, VehicleTypes type, UnityAction<VehicleComponent, int> completeMethod, List<int> path)
		{
			TrafficWaypoint closestSpawnWaypoint = _waypointSelector.GetClosestSpawnWaypoint(position, type);
			if (closestSpawnWaypoint == null)
			{
				Debug.LogWarning("There are no free waypoints in the current cell");
			}
			else
			{
				_requestedVehicles.Add(new VehicleRequest(closestSpawnWaypoint, type, Category.User, null, completeMethod, path, ignoreLos: true));
			}
		}

		public void InstantiateTrafficVehicle(int vehicleIndex, Vector3 vehiclePosition, Quaternion vehicleRotation, Vector3 initialVelocity, Vector3 initialAngularVelocity, int nextWaypointIndex)
		{
			if (_allVehiclesData.IsVehicleIndexValid(vehicleIndex))
			{
				RemoveVehicle(vehicleIndex, force: true);
				InstantiateVehicle(vehicleIndex, nextWaypointIndex, vehiclePosition, vehicleRotation, initialVelocity, initialAngularVelocity);
			}
			else
			{
				Debug.LogError($"Vehicle index {vehicleIndex} is invalid. It should be between 0 and {_allVehiclesData.AllVehicles.Length}");
			}
		}

		public void UpdateCameraPositions(Transform[] activeCameras)
		{
			_positionValidator.UpdateCamera(activeCameras);
		}

		public void IgnoreVehicle(int vehicleIndex)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].Ignored = true;
			_idleVehiclesData.RemoveVehicle(_allVehiclesData.GetVehicle(vehicleIndex));
		}

		public void RestoreIgnoredVehicle(int vehicleIndex)
		{
			_allVehiclesData.AllVehicles[vehicleIndex].Ignored = false;
			_idleVehiclesData.AddVehicle(_allVehiclesData.GetVehicle(vehicleIndex));
		}

		public void RemoveVehicle(GameObject vehicle)
		{
			int vehicleIndex = _allVehiclesData.GetVehicleIndex(vehicle);
			if (vehicleIndex != -1)
			{
				RemoveVehicle(vehicleIndex, force: true);
			}
			else
			{
				Debug.Log($"Vehicle {vehicle} not found");
			}
		}

		public void RemoveVehicle(int vehicleIndex, bool force)
		{
			if ((!_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.HasPath && !_allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.DontRemove) || force)
			{
				_allVehiclesData.RemoveVehicle(vehicleIndex);
				_idleVehiclesData.AddVehicle(_allVehiclesData.GetVehicle(vehicleIndex));
				_currentNrOfVehicles--;
				Events.TriggerVehicleDisabledEvent(vehicleIndex);
			}
		}

		public void ClearTrafficOnArea(Vector3 center, float radius, Func<VehicleComponent, bool> condition = null)
		{
			float num = radius * radius;
			for (int i = 0; i < _allVehiclesData.AllVehicles.Length; i++)
			{
				VehicleComponent vehicleComponent = _allVehiclesData.AllVehicles[i];
				if (vehicleComponent.gameObject.activeSelf && math.distancesq(center, vehicleComponent.transform.position) < num && (condition == null || condition(vehicleComponent)))
				{
					RemoveVehicle(i, force: true);
				}
			}
		}

		public void SetTrafficDensity(int nrOfVehicles)
		{
			_maxNrOfVehicles = nrOfVehicles;
			if (_currentNrOfVehicles <= _maxNrOfVehicles)
			{
				return;
			}
			for (int num = _requestedVehicles.Count - 1; num >= 0; num--)
			{
				ClearRequest(_requestedVehicles[num]);
			}
			int num2 = _allVehiclesData.AllVehicles.Length - 1;
			while (num2 >= 0 && _currentNrOfVehicles > _maxNrOfVehicles)
			{
				if (_allVehiclesData.AllVehicles[num2].gameObject.activeSelf && !_allVehiclesData.AllVehicles[num2].Ignored)
				{
					RemoveVehicle(num2, force: true);
				}
				num2--;
			}
		}

		public void UpdateActiveSquaresLevel(int newLevel)
		{
			_activeSquaresLevel = newLevel;
		}

		private void InstantiateVehicle(VehicleRequest request)
		{
			_requestedVehicles.Remove(request);
			request.CompleteMethod?.Invoke(request.Vehicle, request.Waypoint.ListIndex);
			if (request.Path != null)
			{
				_allVehiclesData.AllVehicles[request.Vehicle.ListIndex].MovementInfo.SetPath(request.Path);
			}
			_currentNrOfVehicles++;
			request.Vehicle.ActivateVehicle(request.Waypoint.Position, _trafficWaypointsData.GetNextOrientation(request.Waypoint), request.TrailerRotation);
			_idleVehiclesData.RemoveVehicle(request.Vehicle);
			Events.TriggerVehicleActivatedEvent(request.Vehicle.ListIndex, request.Waypoint.ListIndex);
		}

		private void InstantiateVehicle(int vehicleIndex, int targetWaypointIndex, Vector3 position, Quaternion rotation, Vector3 initialVelocity, Vector3 initialAngularVelocity)
		{
			VehicleComponent vehicle = _allVehiclesData.GetVehicle(vehicleIndex);
			vehicle.ActivateVehicle(position, rotation, Quaternion.identity);
			vehicle.SetVelocity(initialVelocity, initialAngularVelocity);
			_idleVehiclesData.RemoveVehicle(vehicle);
			Events.TriggerVehicleActivatedEvent(vehicleIndex, targetWaypointIndex);
		}

		private void ClearUninstantiatedRequests()
		{
			for (int num = _requestedVehicles.Count - 1; num >= 0; num--)
			{
				ClearRequest(_requestedVehicles[num]);
			}
		}

		private void ClearRequest(VehicleRequest request)
		{
			_idleVehiclesData.AddVehicle(request.Vehicle);
			_requestedVehicles.Remove(request);
		}

		private bool RequestIsValid(VehicleRequest request)
		{
			if (request.Vehicle == null)
			{
				VehicleComponent randomVehicleOfType = _idleVehiclesData.GetRandomVehicleOfType(request.Type);
				if (randomVehicleOfType == null)
				{
					if (_debugDensity)
					{
						Debug.Log($"Density: No vehicle of type {request.Type} is idle");
					}
					return false;
				}
				request.Vehicle = randomVehicleOfType;
				_idleVehiclesData.RemoveVehicle(randomVehicleOfType);
			}
			if (request.Vehicle.gameObject.activeSelf)
			{
				if (_debugDensity)
				{
					Debug.Log("Density: already active");
				}
				return false;
			}
			if (request.Waypoint.TemporaryDisabled)
			{
				if (_debugDensity)
				{
					Debug.Log("Density: waypoint is disabled");
				}
				return false;
			}
			if (!_positionValidator.IsValid(request.Waypoint.Position, request.Vehicle.length * 2f, request.Vehicle.coliderHeight, request.Vehicle.ColliderWidth, request.IgnoreLOS, request.Vehicle.frontTrigger.localPosition.z, _trafficWaypointsData.GetNextOrientation(request.Waypoint)))
			{
				if (request.Category == Category.Random)
				{
					ClearRequest(request);
				}
				return false;
			}
			if (request.Vehicle.trailer != null)
			{
				if (request.TrailerRotation == Quaternion.identity)
				{
					request.TrailerRotation = _trafficWaypointsData.GetPrevOrientation(request.Waypoint);
					if (request.TrailerRotation == Quaternion.identity)
					{
						request.TrailerRotation = _trafficWaypointsData.GetNextOrientation(request.Waypoint);
					}
				}
				if (!_positionValidator.CheckTrailerPosition(request.Waypoint.Position, _trafficWaypointsData.GetNextOrientation(request.Waypoint), request.TrailerRotation, request.Vehicle))
				{
					return false;
				}
			}
			return true;
		}

		private void RequestRandomVehicle(Vector3 position, Vector3 direction, bool ignorLOS, Vector3 cameraPosition)
		{
			VehicleComponent randomVehicle = _idleVehiclesData.GetRandomVehicle();
			if (randomVehicle == null)
			{
				if (_debugDensity)
				{
					Debug.Log("Density: No idle vehicle found");
				}
				return;
			}
			TrafficWaypoint aFreeWaypoint = _waypointSelector.GetAFreeWaypoint(cameraPosition, _activeSquaresLevel, randomVehicle.VehicleType, position, direction, _useWaypointPriority);
			if (aFreeWaypoint == null)
			{
				if (_debugDensity)
				{
					Debug.Log("Density: No free waypoint found");
				}
			}
			else if (aFreeWaypoint.TemporaryDisabled)
			{
				if (_debugDensity)
				{
					Debug.Log("Density: waypoint is disabled");
				}
			}
			else
			{
				if (!_positionValidator.IsValid(aFreeWaypoint.Position, randomVehicle.length * 2f, randomVehicle.coliderHeight, randomVehicle.ColliderWidth, ignorLOS, randomVehicle.frontTrigger.localPosition.z, _trafficWaypointsData.GetNextOrientation(aFreeWaypoint)))
				{
					return;
				}
				Quaternion identity = Quaternion.identity;
				if (randomVehicle.trailer != null)
				{
					identity = _trafficWaypointsData.GetPrevOrientation(aFreeWaypoint);
					if (identity == Quaternion.identity)
					{
						identity = _trafficWaypointsData.GetNextOrientation(aFreeWaypoint);
					}
					if (!_positionValidator.CheckTrailerPosition(aFreeWaypoint.Position, _trafficWaypointsData.GetNextOrientation(aFreeWaypoint), identity, randomVehicle))
					{
						return;
					}
				}
				_idleVehiclesData.RemoveVehicle(randomVehicle);
				_requestedVehicles.Add(new VehicleRequest(aFreeWaypoint, randomVehicle.VehicleType, Category.Random, randomVehicle, null, null, ignorLOS));
			}
		}
	}
}
