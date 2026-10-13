using UnityEngine;

namespace CPM.CarVisualModel
{
	public static class WheelArchProbe
	{
		public static bool TryMeasureGap(Vector3[] vertices, int[] triangles, ArchFootprint footprint, out float gap)
		{
			gap = 0f;
			if (vertices == null || triangles == null || triangles.Length < 3)
			{
				return false;
			}
			Vector3 dir = ((footprint.Up.sqrMagnitude > 0f) ? footprint.Up.normalized : Vector3.up);
			int num = Mathf.Max(footprint.Samples, 1);
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				float num3 = ((num == 1) ? 0f : Mathf.Lerp(-1f, 1f, (float)i / (float)(num - 1)));
				for (int j = 0; j < num; j++)
				{
					float num4 = ((num == 1) ? 0f : Mathf.Lerp(-1f, 1f, (float)j / (float)(num - 1)));
					Vector3 origin = footprint.TyreTop + footprint.Right * (num3 * footprint.HalfWidth) + footprint.Forward * (num4 * footprint.HalfLength);
					for (int k = 0; k + 2 < triangles.Length; k += 3)
					{
						if (TryRayTriangle(origin, dir, vertices[triangles[k]], vertices[triangles[k + 1]], vertices[triangles[k + 2]], out var distance) && distance < num2)
						{
							num2 = distance;
						}
					}
				}
			}
			if (num2 == float.MaxValue)
			{
				return false;
			}
			gap = num2;
			return true;
		}

		public static float ResolveMountY(float currentMountY, float currentGap, float targetGap)
		{
			return currentMountY + currentGap - targetGap;
		}

		public static float SnapToCentimeters(float value)
		{
			return Mathf.Round(value * 100f) / 100f;
		}

		private static bool TryRayTriangle(Vector3 origin, Vector3 dir, Vector3 a, Vector3 b, Vector3 c, out float distance)
		{
			distance = 0f;
			Vector3 vector = b - a;
			Vector3 vector2 = c - a;
			Vector3 rhs = Vector3.Cross(dir, vector2);
			float num = Vector3.Dot(vector, rhs);
			if (Mathf.Abs(num) < 1E-09f)
			{
				return false;
			}
			float num2 = 1f / num;
			Vector3 lhs = origin - a;
			float num3 = Vector3.Dot(lhs, rhs) * num2;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			Vector3 rhs2 = Vector3.Cross(lhs, vector);
			float num4 = Vector3.Dot(dir, rhs2) * num2;
			if (num4 < 0f || num3 + num4 > 1f)
			{
				return false;
			}
			float num5 = Vector3.Dot(vector2, rhs2) * num2;
			if (num5 < 0f)
			{
				return false;
			}
			distance = num5;
			return true;
		}
	}
}
