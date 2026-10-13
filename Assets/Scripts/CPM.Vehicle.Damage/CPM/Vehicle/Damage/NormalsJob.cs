using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct NormalsJob : IJob
	{
		[ReadOnly]
		public NativeArray<int> Triangles;

		[ReadOnly]
		public NativeArray<float3> RawPositions;

		[ReadOnly]
		public NativeArray<bool> RawDirty;

		public NativeArray<float3> Normals;

		public void Execute()
		{
			for (int i = 0; i < RawDirty.Length; i++)
			{
				if (RawDirty[i])
				{
					Normals[i] = float3.zero;
				}
			}
			int num = Triangles.Length / 3;
			for (int j = 0; j < num; j++)
			{
				int index = Triangles[j * 3];
				int index2 = Triangles[j * 3 + 1];
				int index3 = Triangles[j * 3 + 2];
				bool flag = RawDirty[index];
				bool flag2 = RawDirty[index2];
				bool flag3 = RawDirty[index3];
				if (flag || flag2 || flag3)
				{
					float3 float5 = math.cross(RawPositions[index2] - RawPositions[index], RawPositions[index3] - RawPositions[index]);
					if (flag)
					{
						Normals[index] += float5;
					}
					if (flag2)
					{
						Normals[index2] += float5;
					}
					if (flag3)
					{
						Normals[index3] += float5;
					}
				}
			}
			for (int k = 0; k < RawDirty.Length; k++)
			{
				if (RawDirty[k])
				{
					float num2 = math.length(Normals[k]);
					if (num2 > 1E-12f)
					{
						Normals[k] /= num2;
					}
				}
			}
		}
	}
}
