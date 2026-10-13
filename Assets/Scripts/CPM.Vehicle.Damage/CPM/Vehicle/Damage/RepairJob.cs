using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct RepairJob : IJob
	{
		[ReadOnly]
		public NativeArray<float3> Rest;

		[ReadOnly]
		public NativeArray<bool> WeldedDirty;

		public NativeArray<float3> Current;

		public NativeArray<int> AnyDirty;

		public float Step;

		public float SleepSqr;

		public void Execute()
		{
			int value = 0;
			for (int i = 0; i < Current.Length; i++)
			{
				if (WeldedDirty[i])
				{
					float3 float5 = Rest[i] - Current[i];
					if (math.lengthsq(float5) < SleepSqr)
					{
						Current[i] = Rest[i];
						continue;
					}
					Current[i] += float5 * Step;
					value = 1;
				}
			}
			AnyDirty[0] = value;
		}
	}
}
