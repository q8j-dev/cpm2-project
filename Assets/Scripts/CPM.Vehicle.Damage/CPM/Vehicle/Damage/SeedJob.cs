using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct SeedJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<float3> Rest;

		[ReadOnly]
		public NativeArray<float> Stiffness;

		public NativeArray<float3> Current;

		public NativeArray<float> InvMass;

		public float3 ImpactPoint;

		public float3 ImpactDir;

		public float Depth;

		public float Radius;

		public float RimRatio;

		public float3 SlideAxis;

		public float SlideStretch;

		public void Execute(int i)
		{
			float3 x = Rest[i] - ImpactPoint;
			if (SlideStretch > 1f)
			{
				float num = math.dot(x, SlideAxis);
				x -= SlideAxis * (num * (1f - 1f / SlideStretch));
			}
			float num2 = math.length(x);
			float num3 = Radius * 1.5f;
			if (num2 > num3)
			{
				InvMass[i] = 0f;
				return;
			}
			InvMass[i] = 1f;
			if (!(Stiffness[i] >= 1f))
			{
				if (num2 < Radius)
				{
					float num4 = 1f - math.smoothstep(0f, Radius, num2);
					Current[i] += ImpactDir * (Depth * num4);
				}
				else if (RimRatio > 0f)
				{
					float num5 = math.sin((num2 - Radius) / math.max(1E-06f, num3 - Radius) * (float)Math.PI);
					Current[i] -= ImpactDir * (Depth * RimRatio * num5 * (1f - Stiffness[i]));
				}
			}
		}
	}
}
