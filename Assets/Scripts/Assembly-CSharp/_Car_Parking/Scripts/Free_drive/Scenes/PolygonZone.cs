using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Free_drive.Scenes
{
	[Serializable]
	public struct PolygonZone
	{
		public Vector2[] points;

		public float minY;

		public float maxY;

		public static PolygonZone FromCollider(Collider c)
		{
			if (!c)
			{
				return default;
			}
			if (c is BoxCollider box)
			{
				return FromBoxCollider(box);
			}
			if (c is MeshCollider meshCollider && (bool)meshCollider.sharedMesh)
			{
				return FromMeshCollider(meshCollider);
			}
			return FromBounds(c.bounds);
		}

		public static PolygonZone FromBoxCollider(BoxCollider box)
		{
			Vector3 center = box.center;
			Vector3 vector = box.size * 0.5f;
			Transform transform = box.transform;
			Vector3[] array = new Vector3[8];
			int num = 0;
			for (int i = -1; i <= 1; i += 2)
			{
				for (int j = -1; j <= 1; j += 2)
				{
					for (int k = -1; k <= 1; k += 2)
					{
						array[num++] = transform.TransformPoint(center + new Vector3((float)i * vector.x, (float)j * vector.y, (float)k * vector.z));
					}
				}
			}
			return BuildFromWorldPoints(array);
		}

		public static PolygonZone FromMeshCollider(MeshCollider mc)
		{
			Mesh sharedMesh = mc.sharedMesh;
			if (!sharedMesh)
			{
				return default;
			}
			Transform transform = mc.transform;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3[] array = new Vector3[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				array[i] = transform.TransformPoint(vertices[i]);
			}
			return BuildFromWorldPoints(array);
		}

		public static PolygonZone FromBounds(Bounds b)
		{
			Vector3 vector = b.size * 0.5f;
			return new PolygonZone
			{
				points = new Vector2[4]
				{
					new Vector2(b.center.x - vector.x, b.center.z - vector.z),
					new Vector2(b.center.x + vector.x, b.center.z - vector.z),
					new Vector2(b.center.x + vector.x, b.center.z + vector.z),
					new Vector2(b.center.x - vector.x, b.center.z + vector.z)
				},
				minY = b.center.y - vector.y,
				maxY = b.center.y + vector.y
			};
		}

		private static PolygonZone BuildFromWorldPoints(Vector3[] worldPoints)
		{
			float num = float.MaxValue;
			float num2 = float.MinValue;
			Vector2[] array = new Vector2[worldPoints.Length];
			for (int i = 0; i < worldPoints.Length; i++)
			{
				Vector3 vector = worldPoints[i];
				array[i] = new Vector2(vector.x, vector.z);
				if (vector.y < num)
				{
					num = vector.y;
				}
				if (vector.y > num2)
				{
					num2 = vector.y;
				}
			}
			return new PolygonZone
			{
				points = ConvexHullXZ(array),
				minY = num,
				maxY = num2
			};
		}

		private static Vector2[] ConvexHullXZ(Vector2[] points)
		{
			if (points == null || points.Length == 0)
			{
				return Array.Empty<Vector2>();
			}
			List<Vector2> list = new List<Vector2>(points.Length);
			for (int i = 0; i < points.Length; i++)
			{
				Vector2 item = points[i];
				bool flag = false;
				foreach (Vector2 item2 in list)
				{
					if (Mathf.Abs(item2.x - item.x) < 0.0001f && Mathf.Abs(item2.y - item.y) < 0.0001f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(item);
				}
			}
			if (list.Count <= 2)
			{
				return list.ToArray();
			}
			list.Sort((Vector2 a, Vector2 b) => (a.x == b.x) ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
			int count = list.Count;
			Vector2[] array = new Vector2[2 * count];
			int num = 0;
			for (int num2 = 0; num2 < count; num2++)
			{
				while (num >= 2 && Cross(array[num - 2], array[num - 1], list[num2]) <= 0f)
				{
					num--;
				}
				array[num++] = list[num2];
			}
			int num3 = num + 1;
			for (int num4 = count - 2; num4 >= 0; num4--)
			{
				while (num >= num3 && Cross(array[num - 2], array[num - 1], list[num4]) <= 0f)
				{
					num--;
				}
				array[num++] = list[num4];
			}
			Vector2[] array2 = new Vector2[num - 1];
			Array.Copy(array, array2, num - 1);
			return array2;
		}

		private static float Cross(Vector2 a, Vector2 b, Vector2 c)
		{
			return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
		}

		public float SqrDistance(Vector3 worldPoint)
		{
			if (points == null || points.Length < 3)
			{
				return float.MaxValue;
			}
			Vector2 p = new Vector2(worldPoint.x, worldPoint.z);
			bool flag = false;
			float num = float.MaxValue;
			int num2 = points.Length;
			int num3 = 0;
			int num4 = num2 - 1;
			while (num3 < num2)
			{
				Vector2 b = points[num3];
				Vector2 a = points[num4];
				if (b.y > p.y != a.y > p.y && p.x < (a.x - b.x) * (p.y - b.y) / (a.y - b.y) + b.x)
				{
					flag = !flag;
				}
				float num5 = SqrDistanceToSegment(p, a, b);
				if (num5 < num)
				{
					num = num5;
				}
				num4 = num3++;
			}
			float num6 = (flag ? 0f : num);
			float num7 = 0f;
			if (worldPoint.y < minY)
			{
				num7 = minY - worldPoint.y;
			}
			else if (worldPoint.y > maxY)
			{
				num7 = worldPoint.y - maxY;
			}
			return num6 + num7 * num7;
		}

		private static float SqrDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
		{
			Vector2 vector = b - a;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < 0.0001f)
			{
				return (p - a).sqrMagnitude;
			}
			float num = Mathf.Clamp01(Vector2.Dot(p - a, vector) / sqrMagnitude);
			Vector2 vector2 = a + vector * num;
			return (p - vector2).sqrMagnitude;
		}

		public Vector2 GetCenter()
		{
			if (points == null || points.Length == 0)
			{
				return Vector2.zero;
			}
			Vector2 zero = Vector2.zero;
			for (int i = 0; i < points.Length; i++)
			{
				zero += points[i];
			}
			return zero / points.Length;
		}

		public Vector3 GetCenter3D()
		{
			Vector2 center = GetCenter();
			return new Vector3(center.x, (minY + maxY) * 0.5f, center.y);
		}
	}
}
