using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public static class MeshWelder
	{
		public const float WeldEpsilon = 0.0001f;

		public static WeldMap Build(NativeArray<float3> positions, Allocator allocator)
		{
			int length = positions.Length;
			NativeArray<int> weldedOf = new NativeArray<int>(length, allocator);
			NativeList<float3> nativeList = new NativeList<float3>(math.max(1, length), Allocator.Temp);
			NativeParallelMultiHashMap<int3, int> nativeParallelMultiHashMap = new NativeParallelMultiHashMap<int3, int>(math.max(1, length), Allocator.Temp);
			float num = 10000f;
			float num2 = 9.999999E-09f;
			try
			{
				for (int i = 0; i < length; i++)
				{
					float3 value = positions[i];
					int3 int5 = (int3)math.floor(value * num);
					int num3 = -1;
					for (int j = -1; j <= 1; j++)
					{
						if (num3 >= 0)
						{
							break;
						}
						for (int k = -1; k <= 1; k++)
						{
							if (num3 >= 0)
							{
								break;
							}
							for (int l = -1; l <= 1; l++)
							{
								if (num3 >= 0)
								{
									break;
								}
								int3 key = int5 + new int3(l, k, j);
								if (!nativeParallelMultiHashMap.TryGetFirstValue(key, out var item, out var it))
								{
									continue;
								}
								do
								{
									if (math.distancesq(nativeList[item], value) <= num2)
									{
										num3 = item;
										break;
									}
								}
								while (nativeParallelMultiHashMap.TryGetNextValue(out item, ref it));
							}
						}
					}
					if (num3 < 0)
					{
						num3 = nativeList.Length;
						nativeList.Add(in value);
						nativeParallelMultiHashMap.Add(int5, num3);
					}
					weldedOf[i] = num3;
				}
				WeldMap result = new WeldMap
				{
					WeldedOf = weldedOf,
					WeldedCount = nativeList.Length,
					WeldedPositions = new NativeArray<float3>(nativeList.Length, allocator)
				};
				for (int m = 0; m < nativeList.Length; m++)
				{
					result.WeldedPositions[m] = nativeList[m];
				}
				return result;
			}
			catch
			{
				if (weldedOf.IsCreated)
				{
					weldedOf.Dispose();
				}
				throw;
			}
			finally
			{
				nativeList.Dispose();
				nativeParallelMultiHashMap.Dispose();
			}
		}
	}
}
