using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public static class TireFitment
{
	[Serializable]
	public struct MeshReference
	{
		public float InnerDiameter;

		public float OuterDiameter;

		public float InnerWidth;

		public float OuterWidth;

		public float CarcassDiameter;

		public bool IsValid
		{
			get
			{
				if (InnerDiameter > 0f && OuterDiameter > InnerDiameter && InnerWidth > 0f)
				{
					return OuterWidth > 0f;
				}
				return false;
			}
		}
	}

	private const float TreadStrip = 0.7f;

	private const int MinTreadSamples = 32;

	private const float CarcassPercentile = 0.1f;

	public static readonly MeshReference DefaultReference = new MeshReference
	{
		InnerDiameter = 0.431f,
		OuterDiameter = 0.64f,
		InnerWidth = 0.22f,
		OuterWidth = 0.19f
	};

	public static float MeasureCarcassRadius(Vector3[] vertices, float minRadius, float maxRadius, float treadHalfWidth)
	{
		if (vertices == null || vertices.Length == 0 || treadHalfWidth <= 0.0001f)
		{
			return maxRadius;
		}
		float num = minRadius + (maxRadius - minRadius) * 0.5f;
		float num2 = treadHalfWidth * 0.7f;
		List<float> list = new List<float>();
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			if (!(Mathf.Abs(vector.x) > num2))
			{
				float num3 = Mathf.Sqrt(vector.y * vector.y + vector.z * vector.z);
				if (num3 >= num)
				{
					list.Add(num3);
				}
			}
		}
		if (list.Count < 32)
		{
			return maxRadius;
		}
		list.Sort();
		int index = Mathf.Clamp(Mathf.RoundToInt((float)(list.Count - 1) * 0.1f), 0, list.Count - 1);
		return list[index];
	}

	private static float Normalize(float x)
	{
		return math.rsqrt(math.dot(x, x)) * x;
	}

	private static Mesh CopyMesh(Mesh origin)
	{
		Mesh mesh = new Mesh
		{
			vertices = origin.vertices,
			triangles = origin.triangles,
			uv = origin.uv,
			uv2 = origin.uv2,
			uv3 = origin.uv3,
			normals = origin.normals,
			colors = origin.colors,
			tangents = origin.tangents,
			subMeshCount = origin.subMeshCount
		};
		for (int i = 0; i < origin.subMeshCount; i++)
		{
			mesh.SetSubMesh(i, origin.GetSubMesh(i));
		}
		return mesh;
	}

	public static Mesh GetRecalculateTire(Mesh original, float tyreDiameter, float tyreWidth, float rimWidth, float rimDiameter, MeshReference reference)
	{
		if (!reference.IsValid)
		{
			reference = DefaultReference;
		}
		Mesh mesh = CopyMesh(original);
		List<Vector3> list = new List<Vector3>();
		mesh.GetVertices(list);
		Color[] array = new Color[list.Count];
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			float x = Mathf.Abs(list[i].x) * 2f;
			float a = Mathf.Lerp(list[i].x, rimWidth / reference.InnerWidth * list[i].x, Mathf.Clamp01(math.remap(0f, reference.InnerWidth, 0f, 1f, x)));
			float b = Mathf.Lerp(list[i].x, tyreWidth / reference.OuterWidth * list[i].x, Mathf.Clamp01(math.remap(0f, reference.OuterWidth, 0f, 1f, x)));
			Vector2 vector = new Vector2(list[i].y, list[i].z);
			float t = Mathf.Clamp01(math.remap(reference.InnerDiameter, reference.OuterDiameter, 0f, 1f, vector.magnitude * 2f));
			float x2 = Mathf.Lerp(a, b, t);
			Vector2 vector2 = Vector2.Lerp(vector / reference.InnerDiameter * rimDiameter, vector / reference.OuterDiameter * tyreDiameter, t);
			list[i] = new Vector3(x2, vector2.x, vector2.y);
			array[i] = new Color(list[i].x, list[i].x, list[i].x, 1f);
		}
		mesh.SetVertices(list);
		mesh.SetColors(array);
		mesh.RecalculateBounds();
		return mesh;
	}

	public static async UniTask<Mesh> GetFittedTire(Mesh original, float tyreDiameter, float tyreWidth, float rimWidth, float rimDiameter, MeshReference reference)
	{
		if (!reference.IsValid)
		{
			reference = DefaultReference;
		}
		Mesh mesh = CopyMesh(original);
		List<Vector3> vert = new List<Vector3>();
		mesh.GetVertices(vert);
		Color[] colors = new Color[vert.Count];
		Stopwatch budget = Stopwatch.StartNew();
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			if (budget.Elapsed.TotalMilliseconds > 2.0)
			{
				await UniTask.NextFrame();
				budget.Restart();
			}
			float x = Mathf.Abs(vert[i].x) * 2f;
			float a = Mathf.Lerp(vert[i].x, rimWidth / reference.InnerWidth * vert[i].x, Mathf.Clamp01(math.remap(0f, reference.InnerWidth, 0f, 1f, x)));
			float b = Mathf.Lerp(vert[i].x, tyreWidth / reference.OuterWidth * vert[i].x, Mathf.Clamp01(math.remap(0f, reference.OuterWidth, 0f, 1f, x)));
			Vector2 vector = new Vector2(vert[i].y, vert[i].z);
			float t = Mathf.Clamp01(math.remap(reference.InnerDiameter, reference.OuterDiameter, 0f, 1f, vector.magnitude * 2f));
			float x2 = Mathf.Lerp(a, b, t);
			Vector2 vector2 = Vector2.Lerp(vector / reference.InnerDiameter * rimDiameter, vector / reference.OuterDiameter * tyreDiameter, t);
			vert[i] = new Vector3(x2, vector2.x, vector2.y);
			colors[i] = new Color(vert[i].x, vert[i].x, vert[i].x, 1f);
		}
		mesh.SetVertices(vert);
		mesh.SetColors(colors);
		mesh.RecalculateBounds();
		return mesh;
	}
}
