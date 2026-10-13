using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MTAssets.SkinnedMeshCombiner
{
	public static class SMCMeshClassExtension
	{
		public class Vertices
		{
			private List<Vector3> verts;

			private List<Vector2> uv1;

			private List<Vector2> uv2;

			private List<Vector2> uv3;

			private List<Vector2> uv4;

			private List<Vector3> normals;

			private List<Vector4> tangents;

			private List<Color32> colors;

			private List<BoneWeight> boneWeights;

			public Vertices()
			{
				verts = new List<Vector3>();
			}

			public Vertices(Mesh aMesh)
			{
				verts = CreateList(aMesh.vertices);
				uv1 = CreateList(aMesh.uv);
				uv2 = CreateList(aMesh.uv2);
				uv3 = CreateList(aMesh.uv3);
				uv4 = CreateList(aMesh.uv4);
				normals = CreateList(aMesh.normals);
				tangents = CreateList(aMesh.tangents);
				colors = CreateList(aMesh.colors32);
				boneWeights = CreateList(aMesh.boneWeights);
			}

			private List<T> CreateList<T>(T[] aSource)
			{
				if (aSource == null || aSource.Length == 0)
				{
					return null;
				}
				return new List<T>(aSource);
			}

			private void Copy<T>(ref List<T> aDest, List<T> aSource, int aIndex)
			{
				if (aSource != null)
				{
					if (aDest == null)
					{
						aDest = new List<T>();
					}
					aDest.Add(aSource[aIndex]);
				}
			}

			public int Add(Vertices aOther, int aIndex)
			{
				int count = verts.Count;
				Copy(ref verts, aOther.verts, aIndex);
				Copy(ref uv1, aOther.uv1, aIndex);
				Copy(ref uv2, aOther.uv2, aIndex);
				Copy(ref uv3, aOther.uv3, aIndex);
				Copy(ref uv4, aOther.uv4, aIndex);
				Copy(ref normals, aOther.normals, aIndex);
				Copy(ref tangents, aOther.tangents, aIndex);
				Copy(ref colors, aOther.colors, aIndex);
				Copy(ref boneWeights, aOther.boneWeights, aIndex);
				return count;
			}

			public void AssignTo(Mesh aTarget)
			{
				if (verts.Count > 65535)
				{
					aTarget.indexFormat = IndexFormat.UInt32;
				}
				aTarget.SetVertices(verts);
				if (uv1 != null)
				{
					aTarget.SetUVs(0, uv1);
				}
				if (uv2 != null)
				{
					aTarget.SetUVs(1, uv2);
				}
				if (uv3 != null)
				{
					aTarget.SetUVs(2, uv3);
				}
				if (uv4 != null)
				{
					aTarget.SetUVs(3, uv4);
				}
				if (normals != null)
				{
					aTarget.SetNormals(normals);
				}
				if (tangents != null)
				{
					aTarget.SetTangents(tangents);
				}
				if (colors != null)
				{
					aTarget.SetColors(colors);
				}
				if (boneWeights != null)
				{
					aTarget.boneWeights = boneWeights.ToArray();
				}
			}
		}

		public static Mesh SMCGetSubmesh(this Mesh aMesh, int aSubMeshIndex)
		{
			if (aSubMeshIndex < 0 || aSubMeshIndex >= aMesh.subMeshCount)
			{
				return null;
			}
			int[] triangles = aMesh.GetTriangles(aSubMeshIndex);
			Vertices aOther = new Vertices(aMesh);
			Vertices vertices = new Vertices();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int[] array = new int[triangles.Length];
			for (int i = 0; i < triangles.Length; i++)
			{
				int num = triangles[i];
				if (!dictionary.TryGetValue(num, out var value))
				{
					value = vertices.Add(aOther, num);
					dictionary.Add(num, value);
				}
				array[i] = value;
			}
			Mesh mesh = new Mesh();
			vertices.AssignTo(mesh);
			mesh.triangles = array;
			return mesh;
		}
	}
}
