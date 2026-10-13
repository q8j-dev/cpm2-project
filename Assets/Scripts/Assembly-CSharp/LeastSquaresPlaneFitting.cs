using System.Collections.Generic;
using UnityEngine;

public class LeastSquaresPlaneFitting : MonoBehaviour
{
	public List<Transform> Points;

	public GameObject PointDebug;

	private List<Transform> DebugInstances = new List<Transform>();

	public float smoothness = 1f;

	private void Start()
	{
		for (int i = 0; i < Points.Count; i++)
		{
			Transform component = Object.Instantiate(PointDebug).GetComponent<Transform>();
			DebugInstances.Add(component);
		}
	}

	public float NonLinearSquares3D(List<Vector3> points, Vector2 XZCoord)
	{
		if (points.Count == 0)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		int count = points.Count;
		foreach (Vector3 point in points)
		{
			float x = point.x;
			float z = point.z;
			float y = point.y;
			num += x;
			num2 += z;
			num3 += x * x;
			num4 += z * z;
			num5 += x * z;
			num6 += y;
			num7 += x * y;
			num8 += z * y;
		}
		_ = new float[3, 3]
		{
			{ count, num, num2 },
			{ num, num3, num5 },
			{ num2, num5, num4 }
		};
		float[] array = new float[3] { num6, num7, num8 };
		float num9 = (float)count * (num3 * num4 - num5 * num5) - num * (num * num4 - num2 * num5) + num2 * (num * num5 - num2 * num3);
		if ((double)Mathf.Abs(num9) < 1E-05)
		{
			Debug.LogWarning("Matrix is near singular. Results may not be accurate.");
			return 0f;
		}
		float[,] array2 = new float[3, 3]
		{
			{
				(num3 * num4 - num5 * num5) / num9,
				(0f - (num * num4 - num2 * num5)) / num9,
				(num * num5 - num2 * num3) / num9
			},
			{
				(0f - (num * num4 - num2 * num5)) / num9,
				((float)count * num4 - num2 * num2) / num9,
				(0f - ((float)count * num5 - num * num2)) / num9
			},
			{
				(num * num5 - num2 * num3) / num9,
				(0f - ((float)count * num5 - num * num2)) / num9,
				((float)count * num3 - num * num) / num9
			}
		};
		float num10 = array2[0, 0] * array[0] + array2[0, 1] * array[1] + array2[0, 2] * array[2];
		float num11 = array2[1, 0] * array[0] + array2[1, 1] * array[1] + array2[1, 2] * array[2];
		float num12 = array2[2, 0] * array[0] + array2[2, 1] * array[1] + array2[2, 2] * array[2];
		return num10 + num11 * XZCoord.x + num12 * XZCoord.y;
	}

	public static float LeastSquaresPlane(List<Vector3> points, Vector2 XZCoord)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		foreach (Vector3 point in points)
		{
			num += point.x;
			num2 += point.y;
			num3 += point.z;
		}
		num /= (float)points.Count;
		num2 /= (float)points.Count;
		num3 /= (float)points.Count;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		foreach (Vector3 point2 in points)
		{
			float num9 = point2.x - num;
			float num10 = point2.y - num2;
			float num11 = point2.z - num3;
			num4 += num9 * num9;
			num5 += num9 * num11;
			num6 += num11 * num11;
			num7 += num9 * num10;
			num8 += num11 * num10;
		}
		float num12 = num4 * num6 - num5 * num5;
		float num13 = (num6 * num7 - num5 * num8) / num12;
		float num14 = (num4 * num8 - num5 * num7) / num12;
		return num2 + num13 * (XZCoord.x - num) + num14 * (XZCoord.y - num3);
	}

	public List<Vector3> UnwrapPointsToPlaneNormal(List<Vector3> points, Vector3 origin, Vector3 up)
	{
		up.Normalize();
		Vector3 vector = Vector3.Cross(up, new Vector3(up.y, up.x, up.z));
		vector.Normalize();
		Vector3 normalized = Vector3.Cross(up, vector).normalized;
		up = Vector3.Cross(vector, normalized).normalized;
		Matrix4x4 matrix4x = default;
		matrix4x.SetColumn(0, normalized);
		matrix4x.SetColumn(1, up);
		matrix4x.SetColumn(2, vector);
		matrix4x.SetColumn(3, new Vector4(origin.x, origin.y, origin.z, 1f));
		matrix4x = matrix4x.inverse;
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 point = points[i] - origin;
			Vector3 vector2 = matrix4x.MultiplyPoint3x4(point);
			float magnitude = vector2.magnitude;
			vector2 = Vector3.Normalize(new Vector3(vector2.x, 0f, vector2.z)) * magnitude;
			points[i] = vector2;
		}
		return points;
	}

	private void Update()
	{
	}
}
