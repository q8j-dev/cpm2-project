using System;
using Gley.UrbanSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class Overtake : VehicleBehaviour
	{
		private const float _minOvertakeSpeed = 2f;

		private bool _checkWaypoint;

		private bool _ignoreTime;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_ignoreTime = false;
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}

		public void SetIgnoreTime(bool ignoreTime)
		{
			_ignoreTime = ignoreTime;
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			float num = knownWaypointsList.GetFollowSpeed();
			if (num < 2f)
			{
				num = 0f;
				Stop();
				API.StartVehicleBehaviour<FollowVehicle>(base.VehicleIndex, Array.Empty<object>());
			}
			if (stopTargetReached && requiredBrakePower < 1f)
			{
				requiredBrakePower = 1f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), num, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.5f, base.VehicleComponent.distanceToStop);
			if (_checkWaypoint && !knownWaypointsList.HasPath)
			{
				_checkWaypoint = false;
				TrafficWaypoint waypointFromIndex = base.TrafficWaypointsData.GetWaypointFromIndex(knownWaypointsList.GetWaypointIndex(0));
				if (waypointFromIndex.OtherLanes.Length != 0)
				{
					int overtakeIndex = GetOvertakeIndex(waypointFromIndex.OtherLanes, base.VehicleComponent.VehicleType);
					if (overtakeIndex != -1)
					{
						Blink(overtakeIndex);
						if (API.AllPreviousWaypointsAreFree(overtakeIndex, base.VehicleIndex, _ignoreTime))
						{
							API.AddWaypointAndClear(overtakeIndex, base.VehicleIndex);
							Stop();
							if (!_ignoreTime)
							{
								API.StartVehicleBehaviour<FollowVehicle>(base.VehicleIndex, Array.Empty<object>());
							}
						}
					}
				}
			}
			return behaviourResult;
		}

		private void Blink(int index)
		{
			float num = Vector3.SignedAngle(base.VehicleComponent.GetForwardVector(), base.TrafficWaypointsData.GetWaypointFromIndex(index).Position - base.VehicleComponent.transform.position, Vector3.up);
			if (num > 5f)
			{
				base.VehicleComponent.SetBlinker(BlinkType.Right);
			}
			if (num < -5f)
			{
				base.VehicleComponent.SetBlinker(BlinkType.Left);
			}
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (vehicleIndex == base.VehicleIndex)
			{
				_checkWaypoint = true;
			}
		}

		private int GetOvertakeIndex(int[] otherLanes, VehicleTypes vehicleType)
		{
			float num = 0f;
			int result = -1;
			for (int i = 0; i < otherLanes.Length; i++)
			{
				TrafficWaypoint waypointFromIndex = base.TrafficWaypointsData.GetWaypointFromIndex(otherLanes[i]);
				bool flag = false;
				VehicleTypes[] allowedVehicles = waypointFromIndex.AllowedVehicles;
				for (int j = 0; j < allowedVehicles.Length; j++)
				{
					if (allowedVehicles[j] == vehicleType)
					{
						flag = true;
						break;
					}
				}
				if (flag && (float)waypointFromIndex.MaxSpeed > num)
				{
					num = waypointFromIndex.MaxSpeed;
					result = otherLanes[i];
				}
			}
			return result;
		}

		public override void OnDestroy()
		{
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}
	}
}
