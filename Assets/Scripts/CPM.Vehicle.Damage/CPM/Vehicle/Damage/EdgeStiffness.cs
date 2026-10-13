using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public static class EdgeStiffness
	{
		public static NativeArray<float> Fill(ref EdgeSet edges, in WeldMap weld, NativeArray<int> triangles, float flatAngleDeg, float ridgeAngleDeg, Allocator allocator)
		{
			float xMin = math.radians(flatAngleDeg);
			float xMax = math.radians(ridgeAngleDeg);
			NativeArray<float> result = new NativeArray<float>(weld.WeldedCount, allocator);
			NativeArray<int> nativeArray = new NativeArray<int>(weld.WeldedCount, Allocator.Temp);
			try
			{
				for (int i = 0; i < edges.Count; i++)
				{
					int2 int5 = edges.Tris[i];
					float num;
					if (int5.y < 0)
					{
						num = 1f;
					}
					else
					{
						float3 x = TriangleNormal(int5.x, triangles, in weld);
						float3 float5 = TriangleNormal(int5.y, triangles, in weld);
						if (math.lengthsq(x) < 1E-12f || math.lengthsq(float5) < 1E-12f)
						{
							num = 1f;
						}
						else
						{
							float x2 = math.acos(math.clamp(math.dot(x, float5), -1f, 1f));
							num = math.smoothstep(xMin, xMax, x2);
						}
					}
					edges.Stiffness01[i] = num;
					int2 int6 = edges.Edges[i];
					result[int6.x] += num;
					nativeArray[int6.x]++;
					result[int6.y] += num;
					nativeArray[int6.y]++;
				}
				for (int j = 0; j < result.Length; j++)
				{
					result[j] = ((nativeArray[j] > 0) ? (result[j] / (float)nativeArray[j]) : 1f);
				}
				return result;
			}
			catch
			{
				if (result.IsCreated)
				{
					result.Dispose();
				}
				throw;
			}
			finally
			{
				nativeArray.Dispose();
			}
		}

		private static float3 TriangleNormal(int triangle, NativeArray<int> triangles, in WeldMap weld)
		{
			float3 float5 = weld.WeldedPositions[weld.WeldedOf[triangles[triangle * 3]]];
			float3 obj = weld.WeldedPositions[weld.WeldedOf[triangles[triangle * 3 + 1]]];
			float3 float6 = math.cross(y: weld.WeldedPositions[weld.WeldedOf[triangles[triangle * 3 + 2]]] - float5, x: obj - float5);
			float num = math.length(float6);
			if (!(num > 1E-12f))
			{
				return float3.zero;
			}
			return float6 / num;
		}
	}
}
