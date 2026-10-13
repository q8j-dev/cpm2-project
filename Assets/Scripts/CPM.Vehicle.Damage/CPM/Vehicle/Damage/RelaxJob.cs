using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct RelaxJob : IJob
	{
		public NativeArray<float3> Current;

		[ReadOnly]
		public NativeArray<int2> Edges;

		public NativeArray<float> RestLength;

		[ReadOnly]
		public NativeArray<float> Stiffness;

		[ReadOnly]
		public NativeArray<float> InvMass;

		public int EdgeCount;

		public int Iterations;

		public float YieldStrain;

		public float PlasticRate;

		public void Execute()
		{
			for (int i = 0; i < Iterations; i++)
			{
				for (int j = 0; j < EdgeCount; j++)
				{
					int2 int5 = Edges[j];
					float num = InvMass[int5.x];
					float num2 = InvMass[int5.y];
					float num3 = num + num2;
					if (num3 <= 0f)
					{
						continue;
					}
					float3 float5 = Current[int5.y] - Current[int5.x];
					float num4 = math.length(float5);
					if (num4 < 1E-08f)
					{
						continue;
					}
					float num5 = RestLength[j];
					if (PlasticRate > 0f)
					{
						float num6 = math.abs(num4 - num5) - YieldStrain * num5;
						if (num6 > 0f)
						{
							num5 += math.sign(num4 - num5) * num6 * PlasticRate * (1f - Stiffness[j]);
							RestLength[j] = num5;
						}
					}
					float num7 = math.lerp(0.2f, 1f, Stiffness[j]);
					float3 float6 = float5 * ((num4 - num5) * num7 / (num4 * num3));
					if (num > 0f)
					{
						Current[int5.x] += float6 * num;
					}
					if (num2 > 0f)
					{
						Current[int5.y] -= float6 * num2;
					}
				}
			}
		}
	}
}
