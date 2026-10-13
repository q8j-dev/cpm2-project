using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts;

public class CutData
{
	public readonly GameObject gameObject;

	public readonly Transform transform;

	public Mesh mesh;

	private List<Vector3> vertices;

	private List<Vector2> uvs;

	private List<int> triangles;

	private List<int> subtriangles;

	public List<Vector3> edgeVertices;

	public CutData(string name = "new cut object")
	{
		gameObject = new GameObject(name);
		gameObject.layer = LayerViaName.ShortDistance;
		transform = gameObject.transform;
		vertices = new List<Vector3>();
		uvs = new List<Vector2>();
		triangles = new List<int>();
		subtriangles = new List<int>();
		edgeVertices = new List<Vector3>();
	}

	public int AddVertex(Vector3 vertex)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			if (vertices[i] == vertex)
			{
				return i;
			}
		}
		Vector2 item = new Vector2(0.5f, 0.5f);
		vertices.Add(vertex);
		uvs.Add(item);
		return vertices.Count - 1;
	}

	public int AddVertex(Vector3 vertex, Vector2 uv)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			if (vertices[i] == vertex && uvs[i] == uv)
			{
				return i;
			}
		}
		vertices.Add(vertex);
		uvs.Add(uv);
		return vertices.Count - 1;
	}

	public void AddTriangle(int a, int b, int c)
	{
		triangles.Add(a);
		triangles.Add(b);
		triangles.Add(c);
	}

	public void AddSubTriangle(int a, int b, int c)
	{
		subtriangles.Add(a);
		subtriangles.Add(b);
		subtriangles.Add(c);
	}

	public void AddEdgeVertex(Vector3 vertex)
	{
		edgeVertices.Add(vertex);
	}

	public void MoveVertex(int id, Vector3 pos)
	{
		vertices[id] = pos;
	}

	public Vector3 GetCapCenter()
	{
		if (edgeVertices.Count > 0)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < edgeVertices.Count; i++)
			{
				num += edgeVertices[i].x;
				num2 += edgeVertices[i].y;
				num3 += edgeVertices[i].z;
			}
			float x = num / (float)edgeVertices.Count;
			float y = num2 / (float)edgeVertices.Count;
			float z = num3 / (float)edgeVertices.Count;
			return new Vector3(x, y, z);
		}
		return Vector3.zero;
	}

	public void BuildMesh(string n = "new mesh")
	{
		mesh = new Mesh();
		mesh.name = n;
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetTriangles(triangles, 0);
		mesh.SetTriangles(subtriangles, 1);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}
