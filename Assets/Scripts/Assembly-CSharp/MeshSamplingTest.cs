using System.Collections.Generic;
using UnityEngine;

public class MeshSamplingTest : MonoBehaviour
{
	private Mesh msh;

	public int id = 1;

	public int iterations = 1;

	private void GetRelatedTriangles(Mesh mesh, int VertexIndex, ref int[] ConnectedTriangles, ref int[] ConnectedVertexIndex)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			int num = -1;
			if (mesh.triangles[i] == VertexIndex)
			{
				num = 0;
			}
			if (mesh.triangles[i + 1] == VertexIndex)
			{
				num = 1;
			}
			if (mesh.triangles[i + 2] == VertexIndex)
			{
				num = 2;
			}
			if (num >= 0)
			{
				list2.Add(num);
				list.Add(i / 3);
			}
		}
		ConnectedTriangles = list.ToArray();
		ConnectedVertexIndex = list2.ToArray();
	}

	private void SampleBarycentric(Mesh mesh, int TriangleIndex, Vector3 barycentricCoordinate, ref Vector3 SampledPosition, ref Vector3 SampledNormal)
	{
		Vector3 vector = mesh.vertices[mesh.triangles[TriangleIndex]];
		Vector3 vector2 = mesh.vertices[mesh.triangles[TriangleIndex + 1]];
		Vector3 vector3 = mesh.vertices[mesh.triangles[TriangleIndex + 2]];
		SampledPosition = barycentricCoordinate.x * vector + barycentricCoordinate.y * vector2 + barycentricCoordinate.z * vector3;
		Vector3 vector4 = mesh.normals[mesh.triangles[TriangleIndex]];
		Vector3 vector5 = mesh.normals[mesh.triangles[TriangleIndex + 1]];
		Vector3 vector6 = mesh.normals[mesh.triangles[TriangleIndex + 2]];
		SampledNormal = (barycentricCoordinate.x * vector4 + barycentricCoordinate.y * vector5 + barycentricCoordinate.z * vector6).normalized;
	}

	private void SampleBarycentric(Mesh mesh, int TriangleIndex, int FirstVertID, Vector3 barycentricCoordinate, ref Vector3 SampledPosition, ref Vector3 SampledNormal)
	{
		Vector3 barycentricCoordinate2 = default;
		switch (FirstVertID)
		{
		case 0:
			barycentricCoordinate2 = new Vector3(barycentricCoordinate.x, barycentricCoordinate.y, barycentricCoordinate.z);
			break;
		case 1:
			barycentricCoordinate2 = new Vector3(barycentricCoordinate.y, barycentricCoordinate.x, barycentricCoordinate.z);
			break;
		case 2:
			barycentricCoordinate2 = new Vector3(barycentricCoordinate.y, barycentricCoordinate.z, barycentricCoordinate.x);
			break;
		}
		SampleBarycentric(mesh, TriangleIndex, barycentricCoordinate2, ref SampledPosition, ref SampledNormal);
	}

	private float HaltonSequenceValue(int index, int prime)
	{
		float num = 0f;
		float num2 = 1f / (float)prime;
		while (index > 0)
		{
			num += (float)(index % prime) * num2;
			index /= prime;
			num2 /= (float)prime;
		}
		return num;
	}

	private Vector3[] StarfieldSample(int amount)
	{
		int num = Mathf.CeilToInt(Mathf.Sqrt(amount));
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num - i; j++)
			{
				float num2 = ((float)i + HaltonSequenceValue(i * j, 2)) / (float)num;
				float num3 = ((float)j + HaltonSequenceValue(i * j, 3)) / (float)num;
				if (num2 + num3 > 1f)
				{
					num2 = 1f - num2;
					num3 = 1f - num3;
				}
				float z = 1f - num2 - num3;
				list.Add(new Vector3(num2, num3, z));
			}
		}
		return list.ToArray();
	}

	private Vector2 GetHaltonSequence2D(int seed)
	{
		if (seed != 0)
		{
			return new Vector2(HaltonSequenceValue(seed - 1, 2), HaltonSequenceValue(seed - 1, 3));
		}
		return new Vector2(0.3f, 0.3f);
	}

	private Vector3 GetBarycentricPoint(int index)
	{
		float num = 1f - Mathf.Sqrt(HaltonSequenceValue(index, 2));
		float num2 = HaltonSequenceValue(index, 3) * (1f - num);
		float z = 1f - num - num2;
		return new Vector3(num, num2, z);
	}

	private Vector3[] GetBarycentricArray(int amount)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < amount; i++)
		{
			list.Add(GetBarycentricPoint(i));
		}
		return list.ToArray();
	}

	private void Start()
	{
		msh = GetComponent<MeshFilter>().mesh;
	}

	private void Update()
	{
		Vector3[] barycentricArray = GetBarycentricArray(iterations);
		for (int i = 0; i < msh.triangles.Length; i += 3)
		{
			for (int j = 0; j < iterations; j++)
			{
				Vector3 SampledPosition = default;
				Vector3 SampledNormal = default;
				GetBarycentricPoint(j);
				SampleBarycentric(msh, i, barycentricArray[j], ref SampledPosition, ref SampledNormal);
				Debug.DrawRay(base.transform.TransformPoint(SampledPosition), base.transform.TransformDirection(SampledNormal));
			}
		}
	}
}
