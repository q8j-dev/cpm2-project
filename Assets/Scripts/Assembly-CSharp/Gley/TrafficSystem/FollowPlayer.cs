using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class FollowPlayer : VehicleBehaviour
	{
		private const float _minFollowSpeed = 2f;

		private const float _overtakeTime = 2f;

		private float _followTime;

		private float _stationaryTime;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_followTime = 0f;
			_stationaryTime = 0f;
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			float num = knownWaypointsList.GetFollowSpeed();
			if (num < 2f)
			{
				num = 0f;
				_followTime = 0f;
				if (stopTargetReached)
				{
					requiredBrakePower = 10f;
				}
				if (base.VehicleComponent.GetCurrentSpeedMS() < 0.1f && _stationaryTime > 2f)
				{
					if (!knownWaypointsList.HasStopWaypoints() && !knownWaypointsList.HasGiveWayWaypoints())
					{
						API.StartVehicleBehaviour<OvertakeStationaryPlayer>(base.VehicleIndex, Array.Empty<object>());
						Stop();
					}
					else
					{
						_stationaryTime = 0f;
					}
				}
			}
			else
			{
				_followTime += Time.deltaTime;
			}
			_stationaryTime += Time.deltaTime;
			if (stopTargetReached && requiredBrakePower < 1f)
			{
				requiredBrakePower = 1f;
				num *= 0.9f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), num, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.5f, base.VehicleComponent.distanceToStop);
			if (_followTime > 2f)
			{
				Stop();
				API.StartVehicleBehaviour<OvertakePlayer>(base.VehicleIndex, Array.Empty<object>());
			}
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
