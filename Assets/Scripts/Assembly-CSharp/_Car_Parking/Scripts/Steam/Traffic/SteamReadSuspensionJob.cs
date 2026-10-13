using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace _Car_Parking.Scripts.Steam.Traffic
{
	[BurstCompile]
	public struct SteamReadSuspensionJob : IJobParallelForTransform
	{
		[ReadOnly]
		public NativeArray<bool> ExcludedWheels;

		[ReadOnly]
		public NativeArray<int> WheelVehicle;

		[ReadOnly]
		public NativeArray<float3> BodyVelocity;

		[ReadOnly]
		public NativeArray<float3> BodyAngularVelocity;

		[ReadOnly]
		public NativeArray<float3> BodyCenterOfMass;

		public NativeArray<float3> Position;

		public NativeArray<float3> PointVelocity;

		public void Execute(int index, TransformAccess transform)
		{
			if (!ExcludedWheels[index] && transform.isValid)
			{
				float3 float5 = transform.position;
				Position[index] = float5;
				int index2 = WheelVehicle[index];
				PointVelocity[index] = BodyVelocity[index2] + math.cross(BodyAngularVelocity[index2], float5 - BodyCenterOfMass[index2]);
			}
		}
	}
}
