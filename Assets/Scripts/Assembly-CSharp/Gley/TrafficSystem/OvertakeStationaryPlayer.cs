using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class OvertakeStationaryPlayer : VehicleBehaviour
	{
		private enum State
		{
			Stopped = 0,
			TurningTowardOvertake = 1,
			MovingForward = 2,
			TurningAway = 3,
			Reversing = 4,
			WaitingForClear = 5,
			Exiting = 6
		}

		private float _forwardDistance = 1.5f;

		private float _backwardDistance = 2.5f;

		private float _movementSpeed = 4f;

		private List<int> _originalWaypointIndexes;

		private TrafficWaypoint _newWaypoint;

		private State _currentState;

		private float _oldDistance;

		private float _maxSteerReached;

		private int _direction;

		private int _overtakeDirection;

		private bool _waypointSet;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_oldDistance = float.PositiveInfinity;
			_waypointSet = false;
			_maxSteerReached = base.VehicleComponent.MaxSteer - base.VehicleComponent.SteerStep;
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (!_waypointSet)
			{
				SetNewWaypoint(knownWaypointsList);
			}
			BehaviourResult behaviourResult = new BehaviourResult();
			float num = Vector3.Distance(knownWaypointsList.ClosestObstaclePoint, base.VehicleComponent.FrontPosition.position);
			float steerAngle = base.VehicleComponent.SteerAngle;
			switch (_currentState)
			{
			case State.Stopped:
				requiredBrakePower = 5f;
				behaviourResult.TargetGear = 0;
				if (num != float.PositiveInfinity)
				{
					ChangeState(State.TurningTowardOvertake);
				}
				break;
			case State.TurningTowardOvertake:
				requiredBrakePower = 1f;
				Steer(ref behaviourResult, -_direction);
				if ((0f - steerAngle) * (float)_direction >= _maxSteerReached)
				{
					ChangeState(State.MovingForward);
				}
				break;
			case State.MovingForward:
				behaviourResult.TargetGear = 1;
				requiredBrakePower = 0f;
				Steer(ref behaviourResult, -_direction);
				if (num > _forwardDistance)
				{
					if (num == float.PositiveInfinity)
					{
						ChangeState(State.WaitingForClear);
					}
					else if (num - _oldDistance > 0.01f)
					{
						ChangeState(State.TurningAway);
					}
				}
				else
				{
					Steer(ref behaviourResult, _direction);
					ChangeState(State.TurningAway);
				}
				break;
			case State.TurningAway:
				requiredBrakePower = 1f;
				Steer(ref behaviourResult, _direction);
				if (steerAngle * (float)_direction >= _maxSteerReached)
				{
					ChangeState(State.Reversing);
				}
				break;
			case State.Reversing:
				behaviourResult.TargetGear = -1;
				requiredBrakePower = 0f;
				Steer(ref behaviourResult, _direction);
				if (num > _backwardDistance)
				{
					ChangeState(State.TurningTowardOvertake);
				}
				break;
			case State.WaitingForClear:
				Steer(ref behaviourResult, -_direction);
				requiredBrakePower = 1f;
				Go();
				break;
			case State.Exiting:
				Stop();
				break;
			}
			_oldDistance = num;
			float num2 = ((requiredBrakePower == 0f) ? _movementSpeed : 0f);
			PerformForwardMovement(ref behaviourResult, num2, num2, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.5f, base.VehicleComponent.distanceToStop);
			return behaviourResult;
		}

		public void SetForwardDistance(float distance)
		{
			_forwardDistance = distance;
		}

		public void SetBackwardDistance(float distance)
		{
			_backwardDistance = distance;
		}

		public void SetMovementSpeed(float speed)
		{
			_movementSpeed = speed;
		}

		private void SetNewWaypoint(MovementInfo knownWaypointsList)
		{
			ChangeState(State.Stopped);
			TrafficWaypoint waypointFromIndex = base.TrafficWaypointsData.GetWaypointFromIndex(knownWaypointsList.GetWaypointIndex(0));
			if (waypointFromIndex.OtherLanes == null || waypointFromIndex.OtherLanes.Length == 0)
			{
				SwitchToFollowPlayer();
				return;
			}
			int overtakeIndex = GetOvertakeIndex(waypointFromIndex.OtherLanes, base.VehicleComponent.VehicleType);
			if (overtakeIndex == -1)
			{
				SwitchToFollowPlayer();
				return;
			}
			if (_originalWaypointIndexes != null && _originalWaypointIndexes.Contains(overtakeIndex) && _newWaypoint != null)
			{
				_waypointSet = true;
				_backwardDistance *= 1.05f;
				ChangeState(State.TurningAway);
				return;
			}
			_newWaypoint = API.GetWaypointFromIndex(overtakeIndex);
			while (_newWaypoint.HasPrevs)
			{
				TrafficWaypoint waypointFromIndex2 = API.GetWaypointFromIndex(_newWaypoint.Prevs[0]);
				float f = Vector3.SignedAngle(base.VehicleComponent.GetForwardVector(), base.VehicleComponent.GetFrontAxlePosition() - waypointFromIndex2.Position, Vector3.up);
				if (!(Mathf.Abs(f) > 100f))
				{
					break;
				}
				_overtakeDirection = (int)Mathf.Sign(f);
				_direction = _overtakeDirection;
				_newWaypoint = waypointFromIndex2;
			}
			Blink(_direction);
			_originalWaypointIndexes = new List<int>(knownWaypointsList.WaypointIndexes);
			_waypointSet = true;
		}

		private void SwitchToFollowPlayer()
		{
			API.StartVehicleBehaviour<FollowPlayer>(base.VehicleIndex, Array.Empty<object>());
			Stop();
		}

		private void Go()
		{
			if (API.AllPreviousWaypointsAreFree(_newWaypoint.ListIndex, base.VehicleIndex))
			{
				ChangeState(State.Exiting);
				API.AddWaypointAndClear(_newWaypoint.ListIndex, base.VehicleIndex);
				Stop();
			}
		}

		private void ChangeState(State newState)
		{
			if (_currentState != newState)
			{
				_currentState = newState;
			}
		}

		private void Blink(int direction)
		{
			if (direction == 1)
			{
				base.VehicleComponent.SetBlinker(BlinkType.Left);
			}
			else
			{
				base.VehicleComponent.SetBlinker(BlinkType.Right);
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
		}
	}
}
