using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct NearbyCarJob : IJobParallelFor
{
	public const int MaxNearbyCarsPerCar = 10;

	[ReadOnly]
	public NativeArray<float3> Positions;

	[ReadOnly]
	public NativeArray<float3> Forwards;

	[ReadOnly]
	public NativeArray<float3> Rights;

	[ReadOnly]
	public NativeArray<float3> Ups;

	[ReadOnly]
	public NativeArray<float3> Velocities;

	[ReadOnly]
	public NativeArray<float> Speeds;

	[ReadOnly]
	public int CarCount;

	[NativeDisableParallelForRestriction]
	public NativeArray<NearbyCarResult> Results;

	[NativeDisableParallelForRestriction]
	public NativeArray<int> ResultCounts;

	public void Execute(int observerIndex)
	{
		float3 float5 = Positions[observerIndex];
		float3 float6 = Forwards[observerIndex];
		float3 y = Rights[observerIndex];
		_ = Ups[observerIndex];
		float num = math.clamp(Speeds[observerIndex] * 2.5f, 10f, 150f) + 20f;
		float num2 = num * num;
		int num3 = observerIndex * 10;
		int num4 = 0;
		for (int i = 0; i < CarCount; i++)
		{
			if (num4 >= 10)
			{
				break;
			}
			if (i == observerIndex)
			{
				continue;
			}
			float3 float7 = Positions[i];
			float3 float8 = float7 - float5;
			if (!(float8.x * float8.x + float8.z * float8.z > num2) && !(math.dot(float6, float8) < 0f - num))
			{
				float forwardDist = math.dot(float8, float6);
				float sideDist = math.dot(float8, y);
				float3 float9 = Velocities[i];
				float num5 = math.lengthsq(float9);
				bool sameDirection;
				if (num5 > 4f)
				{
					float num6 = math.dot(float6, float9);
					sameDirection = num6 > 0f && num6 * num6 > 0.25f * num5;
				}
				else
				{
					sameDirection = true;
				}
				Results[num3 + num4] = new NearbyCarResult
				{
					CandidateIndex = i,
					ForwardDist = forwardDist,
					SideDist = sideDist,
					WorldPosX = float7.x,
					WorldPosY = float7.y,
					WorldPosZ = float7.z,
					SameDirection = sameDirection
				};
				num4++;
			}
		}
		for (int j = num4; j < 10; j++)
		{
			Results[num3 + j] = new NearbyCarResult
			{
				CandidateIndex = -1
			};
		}
		ResultCounts[observerIndex] = num4;
	}
}
