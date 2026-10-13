using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Gley.TrafficSystem.Internal
{
	[BurstCompile]
	public struct UpdateWheelJob : IJobParallelForTransform
	{
		[ReadOnly]
		public NativeArray<float3> WheelsOrigin;

		[ReadOnly]
		public NativeArray<float3> DownDirection;

		[ReadOnly]
		public NativeArray<float> WheelRotation;

		[ReadOnly]
		public NativeArray<float> TurnAngle;

		[ReadOnly]
		public NativeArray<float> WheelRadius;

		[ReadOnly]
		public NativeArray<float> RayCastDistance;

		[ReadOnly]
		public NativeArray<float> MaxSuspension;

		[ReadOnly]
		public NativeArray<int> VehicleIndex;

		[ReadOnly]
		public NativeArray<bool> CanSteer;

		[ReadOnly]
		public NativeArray<bool> ExcludedWheels;

		[ReadOnly]
		public int NrOfVehicles;

		public void Execute(int index, TransformAccess transform)
		{
			if (!ExcludedWheels[index])
			{
				if (RayCastDistance[index] != 0f)
				{
					transform.position = WheelsOrigin[index] + DownDirection[VehicleIndex[index]] * (RayCastDistance[index] - WheelRadius[index]);
				}
				else
				{
					transform.position = WheelsOrigin[index] + DownDirection[VehicleIndex[index]] * MaxSuspension[VehicleIndex[index]];
				}
				if (CanSteer[index])
				{
					transform.localRotation = quaternion.EulerXYZ(math.radians(WheelRotation[VehicleIndex[index]]), math.radians(TurnAngle[VehicleIndex[index]]), 0f);
				}
				else
				{
					transform.localRotation = quaternion.EulerXYZ(math.radians(WheelRotation[VehicleIndex[index]]), 0f, 0f);
				}
			}
		}
	}
}
