using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	[BurstCompile]
	public struct DriveJob : IJobParallelFor
	{
		public NativeArray<float3> ForwardForce;

		public NativeArray<float3> LateralForce;

		public NativeArray<float3> TrailerForce;

		public NativeArray<float> WheelRotation;

		public NativeArray<float> TurnAngle;

		public NativeArray<float> VehicleRotationAngle;

		public NativeArray<int> CurrentGear;

		public NativeArray<int> TargetGear;

		public NativeArray<bool> ReadyToRemove;

		public NativeArray<bool> NeedsWaypoint;

		public NativeArray<bool> IsBraking;

		public NativeArray<float> RequiredBrakePower;

		public NativeArray<bool> TargetReached;

		[ReadOnly]
		public NativeArray<float3> TargetWaypointPosition;

		[ReadOnly]
		public NativeArray<float3> NextWaypointPosition;

		[ReadOnly]
		public NativeArray<float3> StopPosition;

		[ReadOnly]
		public NativeArray<float3> AllBotsPosition;

		[ReadOnly]
		public NativeArray<float3> GroundDirection;

		[ReadOnly]
		public NativeArray<float3> ForwardDirection;

		[ReadOnly]
		public NativeArray<float3> RightDirection;

		[ReadOnly]
		public NativeArray<float3> TrailerRightDirection;

		[ReadOnly]
		public NativeArray<float3> CarVelocity;

		[ReadOnly]
		public NativeArray<float3> TrailerVelocity;

		[ReadOnly]
		public NativeArray<float3> CameraPositions;

		[ReadOnly]
		public NativeArray<float> WheelCircumferences;

		[ReadOnly]
		public NativeArray<float> MaxSteer;

		[ReadOnly]
		public NativeArray<float> PowerStep;

		[ReadOnly]
		public NativeArray<float> BrakeStep;

		[ReadOnly]
		public NativeArray<float> Drag;

		[ReadOnly]
		public NativeArray<float> TrailerDrag;

		[ReadOnly]
		public NativeArray<float> MaximumAllowedSpeed;

		[ReadOnly]
		public NativeArray<float> WheelDistance;

		[ReadOnly]
		public NativeArray<float> SteeringStep;

		[ReadOnly]
		public NativeArray<float> MassDifference;

		[ReadOnly]
		public NativeArray<float> DistanceToStop;

		[ReadOnly]
		public NativeArray<float> AccelerationPercent;

		[ReadOnly]
		public NativeArray<float> BrakePercent;

		[ReadOnly]
		public NativeArray<float> SteerPercent;

		[ReadOnly]
		public NativeArray<float> TargetSpeed;

		[ReadOnly]
		public NativeArray<int> NrOfWheels;

		[ReadOnly]
		public NativeArray<int> TrailerNrOfWheels;

		[ReadOnly]
		public NativeArray<int> GroundedWheels;

		[ReadOnly]
		public NativeArray<bool> ExcludedVehicles;

		[ReadOnly]
		public float3 WorldUp;

		[ReadOnly]
		public float DistanceToRemove;

		[ReadOnly]
		public float FixedDeltaTime;

		public void Execute(int index)
		{
			if (ExcludedVehicles[index])
			{
				return;
			}
			RemoveVehicle(index);
			if (GroundDirection[index].Equals(float3.zero))
			{
				return;
			}
			IsBraking[index] = false;
			float3 float5 = math.normalize(GroundDirection[index] / GroundedWheels[index]);
			float3 float6 = ForwardDirection[index] - float5 * math.dot(ForwardDirection[index], float5);
			float3 obj = math.dot(CarVelocity[index], float6) * float6;
			float3 float7 = RightDirection[index] - float5 * math.dot(RightDirection[index], float5);
			float3 float8 = math.dot(CarVelocity[index], float7) * float7;
			float num = math.sign(Vector3.Dot(obj, ForwardDirection[index]));
			float num2 = math.length(obj);
			float num3 = num2 * num;
			float num4 = CalculateMinToChangeWaypoint(num2);
			float num5 = math.distance(TargetWaypointPosition[index], AllBotsPosition[index]);
			float3 float9 = TargetWaypointPosition[index] - AllBotsPosition[index];
			float num6 = math.dot(float9, ForwardDirection[index]);
			if (!StopPosition[index].Equals(TrafficSystemConstants.DEFAULT_POSITION))
			{
				float num7 = math.distance(StopPosition[index], AllBotsPosition[index]) - DistanceToStop[index];
				float num8 = math.dot(StopPosition[index] - AllBotsPosition[index], ForwardDirection[index]);
				if (TargetSpeed[index] < num2)
				{
					float num9 = (num2 * num2 - TargetSpeed[index] * TargetSpeed[index]) / (2f * num7) * FixedDeltaTime;
					if (num9 < 0f)
					{
						RequiredBrakePower[index] = 1f;
					}
					else
					{
						RequiredBrakePower[index] = num9 / BrakeStep[index] * 1f;
					}
				}
				else
				{
					RequiredBrakePower[index] = 0f;
				}
				if ((num8 < 0f && TargetGear[index] != -1) || num7 < 0f)
				{
					TargetReached[index] = true;
				}
				else
				{
					TargetReached[index] = false;
				}
			}
			else
			{
				TargetReached[index] = false;
				RequiredBrakePower[index] = 0f;
			}
			if (num5 < num4 || (num6 < 0f && num5 < num4 * 5f))
			{
				NeedsWaypoint[index] = true;
			}
			float maxSpeed = ((CurrentGear[index] != TargetGear[index]) ? 0f : ((CurrentGear[index] != -1) ? MaximumAllowedSpeed[index] : (MaximumAllowedSpeed[index] / 5f)));
			float3 float10 = AllBotsPosition[index];
			float3 float11 = TargetWaypointPosition[index];
			float3 float12 = NextWaypointPosition[index];
			_ = (float3)GetQuadraticBezierPoint(0.1f, float10, float11, float12) - AllBotsPosition[index];
			float num10 = ((SteerPercent[index] != -2.1474836E+09f) ? (SteerPercent[index] * MaxSteer[index]) : SignedAngle(ForwardDirection[index], float9, WorldUp));
			bool canTurn = CanTurn(index, num5, num2, num10);
			float targetSpeed = GetTargetSpeed(index, num2, maxSpeed, canTurn);
			if (targetSpeed == 0f && (double)num2 < 0.0001)
			{
				CurrentGear[index] = TargetGear[index];
			}
			float3 value = -float8 / FixedDeltaTime / NrOfWheels[index];
			float velocity = GetVelocity(index, num3, targetSpeed * (float)CurrentGear[index]);
			ForwardForce[index] = float6 * velocity;
			LateralForce[index] = value;
			if (TrailerNrOfWheels[index] > 0)
			{
				TrailerForce[index] = -TrailerRightDirection[index] * Vector3.Dot(TrailerVelocity[index], TrailerRightDirection[index]) / TrailerNrOfWheels[index];
			}
			WheelRotation[index] += 360f * (num3 / WheelCircumferences[index]) * FixedDeltaTime;
			ComputeSteerAngle(index, MaxSteer[index], targetSpeed, num10);
		}

		private float CalculateMinToChangeWaypoint(float currentSpeed)
		{
			if (currentSpeed.ToKMH() < 50f)
			{
				return 1.5f;
			}
			return 4f + (currentSpeed.ToKMH() - 50f) * 0.02f;
		}

		private bool CanTurn(int index, float waypointDistance, float targetVelocity, float angle)
		{
			float num = waypointDistance / (targetVelocity * FixedDeltaTime);
			if (num > 5f && math.abs(angle - TurnAngle[index]) / SteeringStep[index] > num)
			{
				return false;
			}
			return true;
		}

		private void ComputeSteerAngle(int index, float maxSteer, float targetVelocity, float angleToNextWaypoint)
		{
			float num = TurnAngle[index];
			float num2 = SteeringStep[index];
			num = math.clamp(num + math.clamp(angleToNextWaypoint - num, 0f - num2, num2), 0f - maxSteer, maxSteer);
			if (math.abs(angleToNextWaypoint) < math.abs(num))
			{
				num = angleToNextWaypoint;
			}
			float num3 = WheelDistance[index] / math.tan(math.radians(num));
			VehicleRotationAngle[index] = 180f * targetVelocity * FixedDeltaTime / ((float)Math.PI * num3) * (float)CurrentGear[index];
			TurnAngle[index] = num;
		}

		private Vector3 GetQuadraticBezierPoint(float t, Vector3 P0, Vector3 P1, Vector3 P2)
		{
			float num = 1f - t;
			float num2 = t * t;
			return num * num * P0 + 2f * num * t * P1 + num2 * P2;
		}

		private float GetVelocity(int index, float currentSpeed, float targetSpeed)
		{
			float num = targetSpeed - currentSpeed;
			float num2 = num + GetDrag(targetSpeed, Drag[index], FixedDeltaTime);
			if (TrailerNrOfWheels[index] > 0 && targetSpeed != 0f)
			{
				num2 += num * MassDifference[index] + GetDrag(targetSpeed, TrailerDrag[index], FixedDeltaTime);
			}
			return num2 / FixedDeltaTime / (float)NrOfWheels[index];
		}

		private float GetTargetSpeed(int index, float currentSpeed, float maxSpeed, bool canTurn)
		{
			if (!canTurn)
			{
				maxSpeed = 0f;
			}
			float num = currentSpeed;
			float num2 = math.abs(num - maxSpeed);
			if (num2 < PowerStep[index] * AccelerationPercent[index])
			{
				return maxSpeed;
			}
			if (num > maxSpeed)
			{
				if (num2 > 1f)
				{
					IsBraking[index] = true;
				}
				if (BrakePercent[index] == 0f)
				{
					return math.max(maxSpeed, num - BrakeStep[index]);
				}
				return math.max(maxSpeed, num - BrakeStep[index] * BrakePercent[index]);
			}
			return num + PowerStep[index] * AccelerationPercent[index];
		}

		private void RemoveVehicle(int index)
		{
			bool value = true;
			for (int i = 0; i < CameraPositions.Length; i++)
			{
				if (math.distancesq(AllBotsPosition[index], CameraPositions[i]) < DistanceToRemove)
				{
					value = false;
					break;
				}
			}
			ReadyToRemove[index] = value;
		}

		private float SignedAngle(float3 dir1, float3 dir2, float3 normal)
		{
			if (dir1.Equals(float3.zero))
			{
				return 0f;
			}
			dir1 = math.normalize(dir1);
			return math.degrees(math.atan2(math.dot(math.cross(dir1, dir2), normal), math.dot(dir1, dir2)));
		}

		private float GetDrag(float targetSpeed, float drag, float fixedDeltaTime)
		{
			return targetSpeed / (1f - drag * fixedDeltaTime) - targetSpeed;
		}
	}
}
