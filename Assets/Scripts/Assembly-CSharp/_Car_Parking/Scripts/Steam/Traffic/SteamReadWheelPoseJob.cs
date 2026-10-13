using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace _Car_Parking.Scripts.Steam.Traffic
{
	[BurstCompile]
	public struct SteamReadWheelPoseJob : IJobParallelForTransform
	{
		[ReadOnly]
		public NativeArray<bool> ExcludedWheels;

		public NativeArray<float3> Position;

		public NativeArray<float3> Right;

		public void Execute(int index, TransformAccess transform)
		{
			if (!ExcludedWheels[index] && transform.isValid)
			{
				Position[index] = transform.position;
				Right[index] = math.mul(transform.rotation, new float3(1f, 0f, 0f));
			}
		}
	}
}
