using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class FollowVehicle : VehicleBehaviour
	{
		private float _minFollowSpeed = 3f;

		private float _overtakeTime = 5f;

		private float _followTime;

		private bool _disableOvertake;

		private bool _stopped;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_followTime = 0f;
			_overtakeTime = 5f;
			_minFollowSpeed = 3f;
		}

		public void SetOvertakeTime(float time)
		{
			_overtakeTime = time;
		}

		public void SetMinFollowSpeed(float speed)
		{
			_minFollowSpeed = speed;
		}

		public void DisableOvertake(bool active)
		{
			_disableOvertake = active;
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			Obstacle closestObstacle = knownWaypointsList.ClosestObstacle;
			if (closestObstacle.Collider == null || closestObstacle.Collider.attachedRigidbody == null)
			{
				return behaviourResult;
			}
			ITrafficParticipant component = knownWaypointsList.ClosestObstacle.Collider.attachedRigidbody.GetComponent<ITrafficParticipant>();
			if (component == null)
			{
				return behaviourResult;
			}
			if (component.GetCurrentSpeedMS() < _minFollowSpeed)
			{
				_stopped = true;
			}
			float num;
			if (_stopped)
			{
				num = 0f;
				if (requiredBrakePower < 1f)
				{
					requiredBrakePower = 1f;
				}
				if (component.GetCurrentSpeedMS() > _minFollowSpeed)
				{
					_stopped = false;
				}
			}
			else
			{
				num = knownWaypointsList.GetFollowSpeed();
			}
			if (component.AlreadyCollidingWith(base.AllVehiclesData.AllVehicles[base.VehicleIndex].AllColliders) && component.GetCurrentSpeedMS() < 1f)
			{
				num = knownWaypointsList.GetFirstWaypointSpeed();
			}
			base.VehicleComponent.GetCurrentSpeedMS();
			_ = _minFollowSpeed;
			if (num < _minFollowSpeed)
			{
				num = 0f;
				_followTime = 0f;
				if (stopTargetReached)
				{
					requiredBrakePower = 5f;
				}
			}
			else
			{
				_followTime += Time.deltaTime;
			}
			if (stopTargetReached && requiredBrakePower < 1f)
			{
				requiredBrakePower = 1f;
				num *= 0.9f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), num, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.5f, base.VehicleComponent.distanceToStop);
			if (_followTime > _overtakeTime && !_disableOvertake)
			{
				Stop();
				API.StartVehicleBehaviour<Overtake>(base.VehicleIndex, Array.Empty<object>());
			}
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
