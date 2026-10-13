using System;
using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public struct DamageTopology : IDisposable
	{
		public WeldMap Weld;

		public EdgeSet Edges;

		public NativeArray<float> VertexStiffness;

		public NativeArray<int> Triangles;

		public int RawCount;

		public bool IsCreated => Weld.IsCreated;

		public static DamageTopology Build(NativeArray<float3> positions, NativeArray<int> triangles, in DamageSettings settings, Allocator allocator)
		{
			DamageTopology result = new DamageTopology
			{
				RawCount = positions.Length
			};
			try
			{
				result.Weld = MeshWelder.Build(positions, allocator);
				result.Edges = EdgeBuilder.Build(triangles, in result.Weld, allocator);
				result.VertexStiffness = EdgeStiffness.Fill(ref result.Edges, in result.Weld, triangles, settings.FlatAngleDeg, settings.RidgeAngleDeg, allocator);
				result.Triangles = new NativeArray<int>(triangles.Length, allocator);
				result.Triangles.CopyFrom(triangles);
				if (result.Edges.Tris.IsCreated)
				{
					result.Edges.Tris.Dispose();
				}
				return result;
			}
			catch
			{
				result.Dispose();
				throw;
			}
		}

		public void Dispose()
		{
			Weld.Dispose();
			Edges.Dispose();
			if (VertexStiffness.IsCreated)
			{
				VertexStiffness.Dispose();
			}
			if (Triangles.IsCreated)
			{
				Triangles.Dispose();
			}
			RawCount = 0;
		}
	}
}
