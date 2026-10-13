using System;
using System.Collections.Generic;
using Gley.TrafficSystem.Internal;
using Gley.UrbanSystem.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Gley.TrafficSystem
{
	public static class API
	{
		public static void AddNeighbor(int waypointIndex, int waypointIndexToAdd)
		{
			TrafficManager.Instance.TrafficWaypointsData.AddNeighbor(waypointIndex, waypointIndexToAdd);
		}

		public static void AddOtherLane(int waypointIndex, int waypointIndexToAdd)
		{
			TrafficManager.Instance.TrafficWaypointsData.AddOtherLane(waypointIndex, waypointIndexToAdd);
		}

		public static void AddPrevs(int waypointIndex, int waypointIndexToAdd)
		{
			TrafficManager.Instance.TrafficWaypointsData.AddPrevs(waypointIndex, waypointIndexToAdd);
		}

		public static void AddWaypointAndClear(int waypointIndex, int vehicleIndex)
		{
			TrafficManager.Instance.VehicleAI.AddWaypointAndClear(waypointIndex, vehicleIndex);
		}

		public static void AddWaypointEvent(int waypointIndex, string data)
		{
			if ((bool)TrafficManager.Instance.TrafficWaypointsData)
			{
				TrafficManager.Instance.TrafficWaypointsData.SetEventData(waypointIndex, data);
			}
		}

		public static bool AllPreviousWaypointsAreFree(int waypointIndex, int vehicleIndex, bool ignoreTime = false)
		{
			if (TrafficManager.Instance.VehicleAI != null)
			{
				return TrafficManager.Instance.VehicleAI.AllPreviousWaypointsAreFree(waypointIndex, vehicleIndex, ignoreTime);
			}
			return false;
		}

		public static bool AllRequiredWaypointsAreFree(int waypointIndex, int vehicleIndex)
		{
			if (TrafficManager.Instance.VehicleAI != null)
			{
				return TrafficManager.Instance.VehicleAI.AllRequiredWaypointsAreFree(waypointIndex, vehicleIndex);
			}
			return false;
		}

		public static void ClearTrafficOnArea(Vector3 center, float radius, Func<VehicleComponent, bool> condition = null)
		{
			TrafficManager.Instance.DensityManager?.ClearTrafficOnArea(center, radius, condition);
		}

		public static void DisableAreaWaypoints(Vector3 center, float radius)
		{
			TrafficManager.Instance.DisabledWaypointsManager?.DisableAreaWaypoints(new Area(center, radius));
		}

		public static void DontRemoveVehicle(int vehicleIndex, bool dontRemove)
		{
			TrafficManager.Instance.AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.DontRemoveVehicle(dontRemove);
		}

		public static void EnableAllWaypoints()
		{
			TrafficManager.Instance.DisabledWaypointsManager?.EnableAllWaypoints();
		}

		public static void ExcludeVehicleFromSystem(int vehicleIndex)
		{
			TrafficManager.Instance.ExcludeVehicleFromSystem(vehicleIndex);
		}

		public static Dictionary<string, VehicleBehaviour> GetActiveBehaviours(int vehicleIndex)
		{
			BehaviourManager behaviourManager = TrafficManager.Instance.BehaviourManager;
			if (behaviourManager != null)
			{
				return behaviourManager.GetActiveBehaviours()[vehicleIndex];
			}
			return null;
		}

		public static VehicleComponent[] GetAllVehicles()
		{
			return TrafficManager.Instance.AllVehiclesData?.AllVehicles;
		}

		public static TrafficWaypoint GetClosestWaypoint(Vector3 position)
		{
			return TrafficManager.Instance.WaypointSelector?.GetClosestWaypoint(position);
		}

		public static TrafficWaypoint GetClosestWaypointInDirection(Vector3 position, Vector3 direction)
		{
			return TrafficManager.Instance.WaypointSelector?.GetClosestWaypointInDirection(position, direction);
		}

		public static int GetIgnoredVehicleIndex(GameObject vehicle)
		{
			if (TrafficManager.Instance.AllVehiclesData != null)
			{
				return TrafficManager.Instance.AllVehiclesData.GetIgnoredVehicleIndex(vehicle);
			}
			return -1;
		}

		public static List<VehicleComponent> GetIgnoredVehicleList()
		{
			return TrafficManager.Instance.AllVehiclesData?.GetIgnoredVehicleList();
		}

		public static List<int> GetPath(Vector3 startPosition, Vector3 endPosition, VehicleTypes vehicleType)
		{
			return TrafficManager.Instance.PathFindingManager?.GetPath(startPosition, endPosition, vehicleType);
		}

		public static bool GetPriorityCrossingStopState(string crossingName)
		{
			if (TrafficManager.Instance.IntersectionManager != null)
			{
				return TrafficManager.Instance.IntersectionManager.IsPriorityCrossingRed(crossingName);
			}
			return false;
		}

		public static float GetSteeringAngle(int vehicleIndex)
		{
			return TrafficManager.Instance.GetSteeringAngle(vehicleIndex);
		}

		public static TrafficLightsColor GetTrafficLightsCrossingState(string crossingName)
		{
			if (TrafficManager.Instance.IntersectionManager != null)
			{
				return TrafficManager.Instance.IntersectionManager.GetTrafficLightsCrossingState(crossingName);
			}
			return TrafficLightsColor.Red;
		}

		public static VehicleBehaviour GetVehicleBehaviourOfType<T>(int vehicleIndex) where T : VehicleBehaviour
		{
			return TrafficManager.Instance.BehaviourManager?.GetBehaviour(vehicleIndex, typeof(T).Name);
		}

		public static VehicleComponent GetVehicleComponent(int vehicleIndex)
		{
			if (TrafficManager.Instance.AllVehiclesData != null)
			{
				return TrafficManager.Instance.AllVehiclesData.GetVehicle(vehicleIndex);
			}
			return null;
		}

		public static int GetVehicleIndex(GameObject vehicle)
		{
			if (TrafficManager.Instance.AllVehiclesData != null)
			{
				return TrafficManager.Instance.AllVehiclesData.GetVehicleIndex(vehicle);
			}
			return -1;
		}

		public static TrafficWaypoint GetWaypointFromIndex(int waypointIndex)
		{
			TrafficWaypointsData trafficWaypointsData = TrafficManager.Instance.TrafficWaypointsData;
			if (trafficWaypointsData == null)
			{
				return null;
			}
			return trafficWaypointsData.GetWaypointFromIndex(waypointIndex);
		}

		public static void IgnoreVehicle(int vehicleIndex)
		{
			TrafficManager.Instance.DensityManager?.IgnoreVehicle(vehicleIndex);
		}

		public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool vehiclePool)
		{
			Initialize(activeCamera, nrOfVehicles, vehiclePool, new TrafficOptions());
		}

		public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
		{
			Initialize(new Transform[1] { activeCamera }, nrOfVehicles, vehiclePool, trafficOptions);
		}

		public static void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
		{
			TrafficManager.Instance.Initialize(activeCameras, nrOfVehicles, vehiclePool, trafficOptions);
		}

		public static void InitializeBehaviourImplementation(params object[] parameters)
		{
			TrafficManager.Instance.InitializeBehaviourImplementation(parameters);
		}

		public static void InstantiateIgnoredVehicle(int vehicleIndex, Vector3 position)
		{
			InstantiateIgnoredVehicle(vehicleIndex, position, null);
		}

		public static void InstantiateIgnoredVehicle(int vehicleIndex, Vector3 position, UnityAction<VehicleComponent, int> completeMethod)
		{
			TrafficManager.Instance.DensityManager?.RequestIgnoredVehicle(vehicleIndex, position, completeMethod);
		}

		public static void InstantiateVehicle(Vector3 position, VehicleTypes vehicleType)
		{
			InstantiateVehicle(position, vehicleType, null);
		}

		public static void InstantiateVehicle(Vector3 position, VehicleTypes vehicleType, UnityAction<VehicleComponent, int> completeMethod)
		{
			TrafficManager.Instance.DensityManager?.RequestVehicleAtPosition(position, vehicleType, completeMethod, null);
		}

		public static void InstantiateVehicleOnTheSpot(int vehicleIndex, Vector3 frontWheelsPosition, Quaternion vehicleRotation, Vector3 initialVelocity, Vector3 initialAngularVelocity, int nextWaypointIndex)
		{
			TrafficManager.Instance.DensityManager.InstantiateTrafficVehicle(vehicleIndex, frontWheelsPosition, vehicleRotation, initialVelocity, initialAngularVelocity, nextWaypointIndex);
		}

		public static void InstantiateVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination)
		{
			InstantiateVehicleWithPath(position, vehicleType, destination, null);
		}

		public static void InstantiateVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination, UnityAction<VehicleComponent, int> completeMethod)
		{
			TrafficManager.Instance.InstantiateVehicleWithPath(position, vehicleType, destination, completeMethod);
		}

		public static bool IsInitialized()
		{
			if (TrafficManager.Exists)
			{
				return TrafficManager.Instance.Initialized;
			}
			return false;
		}

		public static bool IsThisWaypointADestination(int waypointIndex, int vehicleIndex)
		{
			if (TrafficManager.Instance.VehicleAI != null)
			{
				return TrafficManager.Instance.VehicleAI.IsThisWaypointADestination(waypointIndex, vehicleIndex);
			}
			return false;
		}

		public static void RemoveVehicle(GameObject vehicle)
		{
			TrafficManager.Instance.DensityManager.RemoveVehicle(vehicle);
		}

		public static void RemoveVehicle(int vehicleIndex)
		{
			TrafficManager.Instance.DensityManager.RemoveVehicle(vehicleIndex, force: true);
		}

		public static void RemoveVehiclePath(int vehicleIndex)
		{
			AllVehiclesData allVehiclesData = TrafficManager.Instance.AllVehiclesData;
			if (allVehiclesData != null)
			{
				allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.RemovePath();
			}
		}

		public static void RemoveWaypointEvent(int waypointIndex)
		{
			if (TrafficManager.Instance.TrafficWaypointsData != null)
			{
				TrafficManager.Instance.TrafficWaypointsData.SetEventData(waypointIndex, null);
			}
		}

		public static void RestoreExcludedVehicleToSystem(int vehicleIndex)
		{
			TrafficManager.Instance.RestoreRemovedVehicleToSystem(vehicleIndex);
		}

		public static void RestoreIgnoredVehicle(int vehicleIndex)
		{
			TrafficManager.Instance.DensityManager?.RestoreIgnoredVehicle(vehicleIndex);
		}

		public static void SetActiveSquares(int level)
		{
			TrafficManager.Instance.SetActiveSquaresLevel(level);
		}

		public static void SetAllVehiclesBehaviours(IBehaviourList vehicleBehaviours)
		{
			TrafficManager.Instance.BehaviourManager?.SetAllVehiclesBehaviours(vehicleBehaviours);
		}

		public static void SetCamera(Transform activeCamera)
		{
			SetCameras(new Transform[1] { activeCamera });
		}

		public static void SetCameras(Transform[] activeCameras)
		{
			TrafficManager.Instance.UpdateCamera(activeCameras);
		}

		public static void SetDestination(int vehicleIndex, Vector3 position)
		{
			TrafficManager.Instance.SetDestination(vehicleIndex, position);
		}

		public static void SetEngineVolume(float volume)
		{
			TrafficManager.Instance.SoundManager?.UpdateMasterVolume(volume);
		}

		public static void SetGiveWayProperty(int waypointIndex, bool giveWayValue)
		{
			TrafficWaypointsData trafficWaypointsData = TrafficManager.Instance.TrafficWaypointsData;
			if (!(trafficWaypointsData == null))
			{
				trafficWaypointsData.GetWaypointFromIndex(waypointIndex)?.SetGiveWayValue(giveWayValue);
			}
		}

		public static void SetHazardLights(int vehicleIndex, bool activate)
		{
			TrafficManager.Instance.AllVehiclesData.AllVehicles[vehicleIndex].BlinkersController.SetHazardLights(activate);
		}

		public static void SetIntersectionRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain = false)
		{
			TrafficManager.Instance.IntersectionManager?.SetRoadToGreen(intersectionName, roadIndex, doNotChangeAgain);
		}

		public static void SetIntersectionRoadToYellow(string intersectionName, int roadIndex)
		{
			TrafficManager.Instance.IntersectionManager?.SetRoadToYellow(intersectionName, roadIndex);
		}

		public static void SetPriorityCrossingStopState(string crossingName, bool stop, bool stopUpdate = true)
		{
			TrafficManager.Instance.IntersectionManager.SetPriorityCrossingState(crossingName, stop, stopUpdate);
		}

		public static void SetOffset(int vehicleIndex, float offset)
		{
			TrafficManager.Instance.AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetOffset(offset);
		}

		public static void SetSpeedVariationPercentage(int vehicleIndex, float speedReductionPercentage, float speedIncreasePercentage)
		{
			TrafficManager.Instance.AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetSpeedVariationPercent(speedReductionPercentage, speedIncreasePercentage);
		}

		public static void SetStopProperty(int waypointIndex, bool stopValue)
		{
			TrafficWaypointsData trafficWaypointsData = TrafficManager.Instance.TrafficWaypointsData;
			if (!(trafficWaypointsData == null))
			{
				trafficWaypointsData.GetWaypointFromIndex(waypointIndex)?.SetStopValue(stopValue);
			}
		}

		public static void SetTrafficDensity(int nrOfVehicles)
		{
			TrafficManager.Instance.DensityManager?.SetTrafficDensity(nrOfVehicles);
		}

		public static void SetTrafficLightsCrossingState(string crossingName, TrafficLightsColor newColor, bool stopUpdate = false)
		{
			TrafficManager.Instance.IntersectionManager.SetCrossingState(crossingName, newColor, stopUpdate, TrafficManager.Instance.TimeManager.RealTimeSinceStartup);
		}

		public static void SetVehicleBehaviours(int vehicleIndex, IBehaviourList vehicleBehaviours)
		{
			TrafficManager.Instance.BehaviourManager?.SetVehicleBehaviours(vehicleIndex, vehicleBehaviours.GetBehaviours());
		}

		public static void SetVehiclePath(int vehicleIndex, List<int> pathWaypoints)
		{
			AllVehiclesData allVehiclesData = TrafficManager.Instance.AllVehiclesData;
			if (allVehiclesData != null)
			{
				allVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetPath(pathWaypoints);
			}
		}

		public static void StartVehicleBehaviour<T>(int vehicleIndex, params object[] parameters) where T : VehicleBehaviour
		{
			TrafficManager.Instance.BehaviourManager?.StartBehaviour(vehicleIndex, typeof(T).Name, parameters);
		}

		public static void StopVehicleBehaviour<T>(int vehicleIndex) where T : VehicleBehaviour
		{
			TrafficManager.Instance.BehaviourManager?.StopBehaviour(vehicleIndex, typeof(T).Name);
		}

		public static void StopVehicleDriving(GameObject vehicle)
		{
			TrafficManager.Instance.StopVehicleDriving(vehicle);
		}

		public static void ResumeVehicleDriving(GameObject vehicle)
		{
			TrafficManager.Instance.ResumeVehicleDriving(vehicle);
		}

		public static void TriggerColliderRemovedEvent(Collider collider)
		{
			TriggerColliderRemovedEvent(new Collider[1] { collider });
		}

		public static void TriggerColliderRemovedEvent(Collider[] collider)
		{
			TrafficManager.Instance.AllVehiclesData?.TriggerColliderRemovedEvent(collider);
		}

		public static void UpdateVehicleLights(bool on)
		{
			TrafficManager.Instance.AllVehiclesData?.UpdateVehicleLights(on);
		}

		internal static GridData GetGridData()
		{
			return TrafficManager.Instance.GridData;
		}
	}
}
