using System;
using System.Collections.Generic;
using UnityEngine;

public static class NormalSolver
{
	private struct VertexKey
	{
		private readonly long _x;

		private readonly long _y;

		private readonly long _z;

		private const int Tolerance = 100000;

		private const long FNV32Init = 2166136261L;

		private const long FNV32Prime = 16777619L;

		public VertexKey(Vector3 position)
		{
			_x = (long)Mathf.Round(position.x * 100000f);
			_y = (long)Mathf.Round(position.y * 100000f);
			_z = (long)Mathf.Round(position.z * 100000f);
		}

		public override bool Equals(object obj)
		{
			VertexKey vertexKey = (VertexKey)obj;
			if (_x == vertexKey._x && _y == vertexKey._y)
			{
				return _z == vertexKey._z;
			}
			return false;
		}

		public override int GetHashCode()
		{
			long num = 2166136261L;
			num ^= _x;
			num *= 16777619;
			num ^= _y;
			num *= 16777619;
			num ^= _z;
			return (num * 16777619).GetHashCode();
		}
	}

	private struct VertexEntry
	{
		public int MeshIndex;

		public int TriangleIndex;

		public int VertexIndex;

		public VertexEntry(int meshIndex, int triIndex, int vertIndex)
		{
			MeshIndex = meshIndex;
			TriangleIndex = triIndex;
			VertexIndex = vertIndex;
		}
	}

	public static void UnweldVertices(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		List<Vector3> list = new List<Vector3>();
		int[][] array = new int[mesh.subMeshCount][];
		List<Vector2> list2 = new List<Vector2>();
		int num = 0;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int[] triangles = mesh.GetTriangles(i);
			Vector3[] array2 = new Vector3[triangles.Length];
			int[] array3 = new int[triangles.Length];
			Vector2[] array4 = new Vector2[array2.Length];
			for (int j = 0; j < triangles.Length; j++)
			{
				array2[j] = vertices[triangles[j]];
				array4[j] = uv[triangles[j]];
				array3[j] = num;
				num++;
			}
			list.AddRange(array2);
			array[i] = array3;
			list2.AddRange(array4);
		}
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		for (int k = 0; k < mesh.subMeshCount; k++)
		{
			mesh.SetTriangles(array[k], k, calculateBounds: false);
		}
		RecalculateTangents(mesh);
	}

	public static void RecalculateNormals(this Mesh mesh, float angle)
	{
		UnweldVertices(mesh);
		float num = Mathf.Cos(angle * ((float)Math.PI / 180f));
		Vector3[] vertices = mesh.vertices;
		Vector3[] array = new Vector3[vertices.Length];
		Vector3[][] array2 = new Vector3[mesh.subMeshCount][];
		Dictionary<VertexKey, List<VertexEntry>> dictionary = new Dictionary<VertexKey, List<VertexEntry>>(vertices.Length);
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int[] triangles = mesh.GetTriangles(i);
			array2[i] = new Vector3[triangles.Length / 3];
			for (int j = 0; j < triangles.Length; j += 3)
			{
				int num2 = triangles[j];
				int num3 = triangles[j + 1];
				int num4 = triangles[j + 2];
				Vector3 lhs = vertices[num3] - vertices[num2];
				Vector3 rhs = vertices[num4] - vertices[num2];
				Vector3 vector = Vector3.Cross(lhs, rhs);
				float magnitude = vector.magnitude;
				if (magnitude > 0f)
				{
					vector /= magnitude;
				}
				int num5 = j / 3;
				array2[i][num5] = vector;
				VertexKey key = new VertexKey(vertices[num2]);
				if (!dictionary.TryGetValue(key, out var value))
				{
					value = new List<VertexEntry>(4);
					dictionary.Add(key, value);
				}
				value.Add(new VertexEntry(i, num5, num2));
				key = new VertexKey(vertices[num3]);
				if (!dictionary.TryGetValue(key, out value))
				{
					value = new List<VertexEntry>();
					dictionary.Add(key, value);
				}
				value.Add(new VertexEntry(i, num5, num3));
				key = new VertexKey(vertices[num4]);
				if (!dictionary.TryGetValue(key, out value))
				{
					value = new List<VertexEntry>();
					dictionary.Add(key, value);
				}
				value.Add(new VertexEntry(i, num5, num4));
			}
		}
		foreach (List<VertexEntry> value2 in dictionary.Values)
		{
			for (int k = 0; k < value2.Count; k++)
			{
				Vector3 vector2 = default;
				VertexEntry vertexEntry = value2[k];
				for (int l = 0; l < value2.Count; l++)
				{
					VertexEntry vertexEntry2 = value2[l];
					if (vertexEntry.VertexIndex == vertexEntry2.VertexIndex)
					{
						vector2 += array2[vertexEntry2.MeshIndex][vertexEntry2.TriangleIndex];
					}
					else if (Vector3.Dot(array2[vertexEntry.MeshIndex][vertexEntry.TriangleIndex], array2[vertexEntry2.MeshIndex][vertexEntry2.TriangleIndex]) >= num)
					{
						vector2 += array2[vertexEntry2.MeshIndex][vertexEntry2.TriangleIndex];
					}
				}
				array[vertexEntry.VertexIndex] = vector2.normalized;
			}
		}
		mesh.normals = array;
	}

	public static void RecalculateTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		int num = triangles.Length;
		int num2 = vertices.Length;
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector4[] array3 = new Vector4[num2];
		for (int i = 0; i < num; i += 3)
		{
			int num3 = triangles[i];
			int num4 = triangles[i + 1];
			int num5 = triangles[i + 2];
			Vector3 vector = vertices[num3];
			Vector3 vector2 = vertices[num4];
			Vector3 vector3 = vertices[num5];
			Vector2 vector4 = uv[num3];
			Vector2 vector5 = uv[num4];
			Vector2 vector6 = uv[num5];
			float num6 = vector2.x - vector.x;
			float num7 = vector3.x - vector.x;
			float num8 = vector2.y - vector.y;
			float num9 = vector3.y - vector.y;
			float num10 = vector2.z - vector.z;
			float num11 = vector3.z - vector.z;
			float num12 = vector5.x - vector4.x;
			float num13 = vector6.x - vector4.x;
			float num14 = vector5.y - vector4.y;
			float num15 = vector6.y - vector4.y;
			float num16 = num12 * num15 - num13 * num14;
			float num17 = ((num16 == 0f) ? 0f : (1f / num16));
			Vector3 vector7 = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
			Vector3 vector8 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
			array[num3] += vector7;
			array[num4] += vector7;
			array[num5] += vector7;
			array2[num3] += vector8;
			array2[num4] += vector8;
			array2[num5] += vector8;
		}
		for (int j = 0; j < num2; j++)
		{
			Vector3 normal = normals[j];
			Vector3 tangent = array[j];
			Vector3.OrthoNormalize(ref normal, ref tangent);
			array3[j].x = tangent.x;
			array3[j].y = tangent.y;
			array3[j].z = tangent.z;
			array3[j].w = ((Vector3.Dot(Vector3.Cross(normal, tangent), array2[j]) < 0f) ? (-1f) : 1f);
		}
		mesh.tangents = array3;
	}
}
