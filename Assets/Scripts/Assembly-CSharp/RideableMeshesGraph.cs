using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class RideableMeshesGraph
{
	[Preserve]
	private Dictionary<Collider, RideableMeshData> dict = new Dictionary<Collider, RideableMeshData>();

	private const float VertexBlendThreshold = 0.15f;

	public void AddSceneDict(Dictionary<Collider, RideableMeshData> sceneDict)
	{
		if (sceneDict == null)
		{
			return;
		}
		foreach (KeyValuePair<Collider, RideableMeshData> item in sceneDict)
		{
			dict.TryAdd(item.Key, item.Value);
		}
	}

	public void RemoveSceneDict(HashSet<Collider> colliders)
	{
		if (colliders == null)
		{
			return;
		}
		try
		{
			foreach (Collider collider in colliders)
			{
				if ((object)collider != null)
				{
					dict.Remove(collider);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public Surface GetSurface(Collider collider, int triangleIndex, Vector3 barycentric = default(Vector3))
	{
		if (!dict.TryGetValue(collider, out var value))
		{
			return Surface.Dryasphalt;
		}
		return GetSurfaceByTrianglesCount(triangleIndex, barycentric, value);
	}

	private Surface GetSurfaceByTrianglesCount(int triangleIndex, Vector3 barycentric, RideableMeshData meshData)
	{
		int num = meshData.submeshDatas.Length;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			RideableMeshData.SubmeshData submeshData = meshData.submeshDatas[i];
			num2 += submeshData.trianglesCount;
			if (triangleIndex < num2)
			{
				if (!submeshData.isVertexBlend)
				{
					return submeshData.surface;
				}
				if (meshData.meshVertexColors == null || submeshData.triangleIndices == null)
				{
					return submeshData.surfaceBase;
				}
				return SampleVertexBlendSurface(submeshData, meshData.meshVertexColors, triangleIndex, barycentric);
			}
		}
		return Surface.Dryasphalt;
	}

	private static Surface SampleVertexBlendSurface(RideableMeshData.SubmeshData sub, Color32[] vertexColors, int globalTriangleIndex, Vector3 bary)
	{
		int num = (globalTriangleIndex - sub.triangleStart) * 3;
		if (num < 0 || num + 2 >= sub.triangleIndices.Length)
		{
			return sub.surfaceBase;
		}
		if (vertexColors.Length == 0)
		{
			return sub.surfaceBase;
		}
		int num2 = sub.triangleIndices[num];
		int num3 = sub.triangleIndices[num + 1];
		int num4 = sub.triangleIndices[num + 2];
		int num5 = vertexColors.Length - 1;
		Color32 obj = ((num2 <= num5) ? vertexColors[num2] : default(Color32));
		Color32 color = ((num3 <= num5) ? vertexColors[num3] : default(Color32));
		Color32 color2 = ((num4 <= num5) ? vertexColors[num4] : default(Color32));
		float num6 = ((float)(int)obj.r * bary.x + (float)(int)color.r * bary.y + (float)(int)color2.r * bary.z) * (1f / 255f);
		float num7 = ((float)(int)obj.g * bary.x + (float)(int)color.g * bary.y + (float)(int)color2.g * bary.z) * (1f / 255f);
		float num8 = ((float)(int)obj.b * bary.x + (float)(int)color.b * bary.y + (float)(int)color2.b * bary.z) * (1f / 255f);
		if (Mathf.Max(num6, num7, num8) < 0.15f)
		{
			return sub.surfaceBase;
		}
		Surface result = sub.surfaceR;
		if (num7 > num6 && num7 >= num8)
		{
			result = sub.surfaceG;
		}
		else if (num8 > num6 && num8 > num7)
		{
			result = sub.surfaceB;
		}
		return result;
	}
}
