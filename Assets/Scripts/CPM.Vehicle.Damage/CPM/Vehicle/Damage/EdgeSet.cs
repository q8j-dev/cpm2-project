using System;
using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public struct EdgeSet : IDisposable
	{
		public NativeArray<int2> Edges;

		public NativeArray<float> RestLength;

		public NativeArray<int2> Tris;

		public NativeArray<float> Stiffness01;

		public int Count;

		public bool IsCreated => Edges.IsCreated;

		public void Dispose()
		{
			if (Edges.IsCreated)
			{
				Edges.Dispose();
			}
			if (RestLength.IsCreated)
			{
				RestLength.Dispose();
			}
			if (Tris.IsCreated)
			{
				Tris.Dispose();
			}
			if (Stiffness01.IsCreated)
			{
				Stiffness01.Dispose();
			}
			Count = 0;
		}
	}
}
