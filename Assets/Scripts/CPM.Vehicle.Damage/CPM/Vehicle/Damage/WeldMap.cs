using System;
using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public struct WeldMap : IDisposable
	{
		public NativeArray<int> WeldedOf;

		public NativeArray<float3> WeldedPositions;

		public int WeldedCount;

		public bool IsCreated => WeldedOf.IsCreated;

		public void Dispose()
		{
			if (WeldedOf.IsCreated)
			{
				WeldedOf.Dispose();
			}
			if (WeldedPositions.IsCreated)
			{
				WeldedPositions.Dispose();
			}
			WeldedCount = 0;
		}
	}
}
