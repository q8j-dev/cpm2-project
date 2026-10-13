using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Gley.TrafficSystem.Internal
{
	[BurstCompile]
	public struct WheelJob : IJobParallelFor
	{
		public NativeArray<float3> WheelSuspensionForce;

		[ReadOnly]
		public NativeArray<float3> WheelNormalDirection;

		[ReadOnly]
		public NativeArray<float3> WheelVelocity;

		[ReadOnly]
		public NativeArray<float> SpringForces;

		[ReadOnly]
		public NativeArray<float> WheelRayCastDistance;

		[ReadOnly]
		public NativeArray<float> WheelRadius;

		[ReadOnly]
		public NativeArray<float> WheelMaxSuspension;

		[ReadOnly]
		public NativeArray<float> SpringStiffness;

		[ReadOnly]
		public NativeArray<bool> ExcludedWheels;

		public void Execute(int i)
		{
			if (!ExcludedWheels[i])
			{
				if (WheelMaxSuspension[i] != 0f)
				{
					float compression = ((WheelRayCastDistance[i] != 0f) ? (1f - (WheelRayCastDistance[i] - WheelRadius[i]) / WheelMaxSuspension[i]) : 0f);
					WheelSuspensionForce[i] = ComputeSuspensionForce(SpringForces[i], compression, WheelNormalDirection[i], i);
				}
				else
				{
					WheelSuspensionForce[i] = ComputeSuspensionForce(SpringForces[i], 1f, WheelNormalDirection[i], i);
				}
			}
		}

		private float3 ComputeSuspensionForce(float springForce, float compression, float3 normalPoint, int index)
		{
			float num = WheelVelocity[index].y * springForce / 2f;
			float num2 = 0.5f - compression;
			if (num2 > -0.1f)
			{
				num2 = 0f;
			}
			float y = ((springForce * (compression / 0.5f) - springForce * SpringStiffness[index] * num2) * normalPoint).y - num;
			return new float3(0f, y, 0f);
		}
	}
}
