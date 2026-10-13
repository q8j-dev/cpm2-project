using UnityEngine;

public class UVPointSampler : MonoBehaviour
{
	public int Width;

	public int Height;

	private Mesh MainMesh;

	private void Start()
	{
		MainMesh = GetComponent<MeshFilter>().sharedMesh;
	}

	private void Update()
	{
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				Vector3 position = Vector3.zero;
				Vector3 normal = Vector3.up;
				if (InterpolatePositionAndNormalFromUV(MainMesh, new Vector2((float)i / (float)Width, (float)j / (float)Height), ref position, ref normal))
				{
					Debug.DrawRay(base.transform.TransformPoint(position), base.transform.TransformDirection(normal));
				}
			}
		}
	}

	private bool InterpolatePositionAndNormalFromUV(Mesh mesh, Vector2 uv, ref Vector3 position, ref Vector3 normal)
	{
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv2 = mesh.uv;
		int[] triangles = mesh.triangles;
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector2 p = uv2[triangles[i]];
			Vector2 p2 = uv2[triangles[i + 1]];
			Vector2 p3 = uv2[triangles[i + 2]];
			if (IsPointInTriangle(uv, p, p2, p3))
			{
				Vector3 vector = vertices[triangles[i]];
				Vector3 vector2 = vertices[triangles[i + 1]];
				Vector3 vector3 = vertices[triangles[i + 2]];
				Vector3 vector4 = CalculateBarycentricCoordinates(uv, p, p2, p3);
				position = vector4.x * vector + vector4.y * vector2 + vector4.z * vector3;
				Vector3 vector5 = normals[triangles[i]];
				Vector3 vector6 = normals[triangles[i + 1]];
				Vector3 vector7 = normals[triangles[i + 2]];
				normal = vector4.x * vector5 + vector4.y * vector6 + vector4.z * vector7;
				normal.Normalize();
				return true;
			}
		}
		position = Vector3.zero;
		normal = Vector3.zero;
		return false;
	}

	private bool IsPointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		float num = (p0.x - p2.x) * (p1.y - p2.y) - (p1.x - p2.x) * (p0.y - p2.y);
		float num2 = ((p.x - p2.x) * (p1.y - p2.y) - (p1.x - p2.x) * (p.y - p2.y)) / num;
		float num3 = ((p0.x - p2.x) * (p.y - p2.y) - (p.x - p2.x) * (p0.y - p2.y)) / num;
		float num4 = 1f - num2 - num3;
		if (num2 >= 0f && num3 >= 0f)
		{
			return num4 >= 0f;
		}
		return false;
	}

	private Vector3 CalculateBarycentricCoordinates(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		float num = (p0.x - p2.x) * (p1.y - p2.y) - (p1.x - p2.x) * (p0.y - p2.y);
		float num2 = ((p.x - p2.x) * (p1.y - p2.y) - (p1.x - p2.x) * (p.y - p2.y)) / num;
		float num3 = ((p0.x - p2.x) * (p.y - p2.y) - (p.x - p2.x) * (p0.y - p2.y)) / num;
		float z = 1f - num2 - num3;
		return new Vector3(num2, num3, z);
	}
}
