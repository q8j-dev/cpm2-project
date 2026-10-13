using Gley.TrafficSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public abstract class VehicleBehaviour
	{
		private bool _debugBehaviours;

		private bool _behaviourIsActive;

		private bool _startBraking;

		protected VehicleComponent VehicleComponent { get; private set; }

		protected TrafficWaypointsData TrafficWaypointsData { get; private set; }

		protected AllVehiclesData AllVehiclesData { get; private set; }

		protected int VehicleIndex { get; private set; }

		public string Name { get; }

		public abstract void OnDestroy();

		public abstract BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear);

		public VehicleBehaviour()
		{
			Name = GetType().Name;
		}

		public virtual void Initialize(int vehicleIndex, VehicleComponent vehicleComponent, TrafficWaypointsData trafficWaypointsData, AllVehiclesData allVehiclesData)
		{
			VehicleIndex = vehicleIndex;
			VehicleComponent = vehicleComponent;
			TrafficWaypointsData = trafficWaypointsData;
			AllVehiclesData = allVehiclesData;
		}

		public virtual void SetParams(object[] parameters)
		{
		}

		public void Start()
		{
			if (_debugBehaviours)
			{
				Debug.Log($"Can run {Name} for {VehicleIndex}");
			}
			if (!_behaviourIsActive)
			{
				_startBraking = false;
				_behaviourIsActive = true;
				OnBecomeActive();
				Events.TriggerBehaviourStartedEvent(VehicleIndex, this);
			}
		}

		public void Stop()
		{
			if (_debugBehaviours)
			{
				Debug.Log($"Can't run {Name} for {VehicleIndex}");
			}
			if (_behaviourIsActive)
			{
				_behaviourIsActive = false;
				OnBecameInactive();
				Events.TriggerBehaviourStoppedEvent(VehicleIndex, this);
			}
		}

		protected virtual void OnBecomeActive()
		{
			if (_debugBehaviours)
			{
				Debug.Log($"{Name} became active for {VehicleIndex}");
			}
		}

		protected virtual void OnBecameInactive()
		{
			if (_debugBehaviours)
			{
				Debug.Log($"{Name} became inactive for {VehicleIndex}");
			}
		}

		protected void Steer(ref BehaviourResult behaviourResult, float steerPercent)
		{
			behaviourResult.SteerPercent = steerPercent;
		}

		protected void PerformForwardMovement(ref BehaviourResult behaviourResult, float currentAllowedSpeed, float futureAllowedSpeed, float3 positionToReachTargetSpeed, float brakePercent, float minBrakePercent, float distanceToStop)
		{
			behaviourResult.Name = Name;
			if (brakePercent < 0.1f)
			{
				_startBraking = false;
			}
			if (!_startBraking && brakePercent >= minBrakePercent)
			{
				_startBraking = true;
			}
			if (!_startBraking)
			{
				brakePercent = 0f;
			}
			behaviourResult.BrakePercent = brakePercent;
			behaviourResult.MaxAllowedSpeed = currentAllowedSpeed;
			behaviourResult.SpeedInPoint = new TargetSpeedPoint(positionToReachTargetSpeed, futureAllowedSpeed, distanceToStop, Name);
		}
	}
}
