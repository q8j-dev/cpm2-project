using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct ClampAndNoiseJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<float3> Rest;

		[ReadOnly]
		public NativeArray<float> Stiffness;

		[ReadOnly]
		public NativeArray<float> InvMass;

		public NativeArray<float3> Current;

		public NativeArray<bool> WeldedDirty;

		public float MaxDeform;

		public float NoiseAmplitude;

		public float DirtyEpsilon;

		public void Execute(int i)
		{
			if (InvMass[i] <= 0f)
			{
				return;
			}
			float3 float5 = Current[i] - Rest[i];
			float num = math.length(float5);
			float num2 = MaxDeform * (1f - Stiffness[i]);
			if (num < 1E-08f || num2 <= 1E-06f)
			{
				Current[i] = Rest[i];
				WeldedDirty[i] = false;
				return;
			}
			float num3 = num * num2 * math.rsqrt(num2 * num2 + num * num);
			float3 float6 = float5 * (num3 / num);
			if (NoiseAmplitude > 0f)
			{
				float6 += Hash(i) * (NoiseAmplitude * math.saturate(num3 / num2));
			}
			float num4 = math.length(float6);
			if (num4 > num2)
			{
				float6 *= num2 / num4;
			}
			Current[i] = Rest[i] + float6;
			WeldedDirty[i] = math.lengthsq(float6) > DirtyEpsilon * DirtyEpsilon;
		}

		private static float3 Hash(int i)
		{
			int num = i * -1640531535 + -1640531527;
			int num2 = (int)((uint)num ^ ((uint)num >> 15)) * -2048144777;
			int num3 = (int)((uint)num2 ^ ((uint)num2 >> 13)) * -1028477379;
			int num4 = num3 ^ (int)((uint)num3 >> 16);
			uint num5 = (uint)(num4 * 1664525 + 1013904223);
			uint num6 = num5 * 1664525 + 1013904223;
			return new float3((uint)(num4 & 0xFFFF), num5 & 0xFFFF, num6 & 0xFFFF) * (2f / 65535f) - 1f;
		}
	}
}
