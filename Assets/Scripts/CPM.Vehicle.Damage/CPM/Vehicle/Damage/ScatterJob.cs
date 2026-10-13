using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	[BurstCompile]
	public struct ScatterJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<int> WeldedOf;

		[ReadOnly]
		public NativeArray<float3> Welded;

		[ReadOnly]
		public NativeArray<bool> WeldedDirty;

		public NativeArray<float3> RawPositions;

		public NativeArray<bool> RawDirty;

		public void Execute(int i)
		{
			int index = WeldedOf[i];
			RawPositions[i] = Welded[index];
			RawDirty[i] = WeldedDirty[index];
		}
	}
}
