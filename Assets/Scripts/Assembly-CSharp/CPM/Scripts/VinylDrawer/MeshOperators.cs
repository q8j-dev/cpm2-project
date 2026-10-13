using System.Collections.Generic;
using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	public static class MeshOperators
	{
		internal struct TrianglePosNorm
		{
			public Vector3 position;

			public Vector3 normal;
		}

		internal static TrianglePosNorm[] UvTo3D(Vector2[] uvArray, MeshCollider[] meshColliders)
		{
			int num = uvArray.Length;
			bool[] array = new bool[num];
			TrianglePosNorm[] array2 = new TrianglePosNorm[num];
			List<int> list = new List<int>();
			List<Vector2> list2 = new List<Vector2>();
			List<Vector3> list3 = new List<Vector3>();
			List<Vector3> list4 = new List<Vector3>();
			foreach (MeshCollider meshCollider in meshColliders)
			{
				list.Clear();
				list2.Clear();
				list3.Clear();
				list4.Clear();
				Mesh sharedMesh = meshCollider.sharedMesh;
				if (!sharedMesh)
				{
					continue;
				}
				sharedMesh.GetTriangles(list, 0);
				sharedMesh.GetUVs(0, list2);
				sharedMesh.GetVertices(list3);
				sharedMesh.GetNormals(list4);
				for (int j = 0; j < list.Count; j += 3)
				{
					Vector2 vector = list2[list[j]];
					Vector2 vector2 = list2[list[j + 1]];
					Vector2 vector3 = list2[list[j + 2]];
					float num2 = Area(vector, vector2, vector3);
					if (num2 == 0f)
					{
						continue;
					}
					for (int k = 0; k < uvArray.Length && !array[k]; k++)
					{
						Vector2 p = uvArray[k];
						float num3 = Area(vector2, vector3, p) / num2;
						if (num3 < 0f)
						{
							continue;
						}
						float num4 = Area(vector3, vector, p) / num2;
						if (!(num4 < 0f))
						{
							float num5 = Area(vector, vector2, p) / num2;
							if (!(num5 < 0f))
							{
								Vector3 position = num3 * list3[list[j]] + num4 * list3[list[j + 1]] + num5 * list3[list[j + 2]];
								Vector3 normal = (list4[list[j]] + list4[list[j + 1]] + list4[list[j + 2]]) / 3f;
								array[k] = true;
								num--;
								array2[k].position = meshCollider.transform.TransformPoint(position);
								array2[k].normal = normal;
							}
						}
					}
					if (num == 0)
					{
						return array2;
					}
				}
			}
			return array2;
			static float Area(Vector2 vector5, Vector2 vector8, Vector2 vector6)
			{
				Vector2 vector4 = vector5 - vector6;
				Vector2 vector7 = vector8 - vector6;
				return (vector4.x * vector7.y - vector4.y * vector7.x) / 2f;
			}
		}
	}
}
