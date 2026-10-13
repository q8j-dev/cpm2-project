using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public static class EdgeBuilder
	{
		public static EdgeSet Build(NativeArray<int> triangles, in WeldMap weld, Allocator allocator)
		{
			int num = triangles.Length / 3;
			int num2 = math.max(1, num * 3);
			NativeParallelHashMap<int2, int> index = new NativeParallelHashMap<int2, int>(num2, Allocator.Temp);
			NativeList<int2> edges = new NativeList<int2>(num2, Allocator.Temp);
			NativeList<int2> tris = new NativeList<int2>(num2, Allocator.Temp);
			EdgeSet result = default;
			try
			{
				for (int i = 0; i < num; i++)
				{
					int num3 = weld.WeldedOf[triangles[i * 3]];
					int num4 = weld.WeldedOf[triangles[i * 3 + 1]];
					int num5 = weld.WeldedOf[triangles[i * 3 + 2]];
					AddEdge(num3, num4, i, ref index, ref edges, ref tris);
					AddEdge(num4, num5, i, ref index, ref edges, ref tris);
					AddEdge(num5, num3, i, ref index, ref edges, ref tris);
				}
				int num6 = (result.Count = edges.Length);
				result.Edges = new NativeArray<int2>(num6, allocator);
				result.RestLength = new NativeArray<float>(num6, allocator);
				result.Tris = new NativeArray<int2>(num6, allocator);
				result.Stiffness01 = new NativeArray<float>(num6, allocator);
				for (int j = 0; j < num6; j++)
				{
					int2 value = edges[j];
					result.Edges[j] = value;
					result.Tris[j] = tris[j];
					result.RestLength[j] = math.distance(weld.WeldedPositions[value.x], weld.WeldedPositions[value.y]);
				}
				return result;
			}
			catch
			{
				result.Dispose();
				throw;
			}
			finally
			{
				index.Dispose();
				edges.Dispose();
				tris.Dispose();
			}
		}

		private static void AddEdge(int u, int v, int triangle, ref NativeParallelHashMap<int2, int> index, ref NativeList<int2> edges, ref NativeList<int2> tris)
		{
			if (u == v)
			{
				return;
			}
			int2 value = ((u < v) ? new int2(u, v) : new int2(v, u));
			if (index.TryGetValue(value, out var item))
			{
				int2 value2 = tris[item];
				if (value2.y < 0 && value2.x != triangle)
				{
					value2.y = triangle;
					tris[item] = value2;
				}
			}
			else
			{
				index.Add(value, edges.Length);
				edges.Add(in value);
				tris.Add(new int2(triangle, -1));
			}
		}
	}
}
