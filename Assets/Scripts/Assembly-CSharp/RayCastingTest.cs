using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class RayCastingTest : MonoBehaviour
{
	public struct TriangleBounds
	{
		public Bounds bounds;

		public int triangleIndex;
	}

	public class BvhNode
	{
		public Bounds bounds;

		public BvhNode left;

		public BvhNode right;

		public List<int> triangleIndices;

		public bool IsLeaf => triangleIndices != null;
	}

	public struct Triangle
	{
		public Vector3 pos0;

		public Vector3 pos1;

		public Vector3 pos2;

		public Vector3 normal;
	}

	public struct BvhData
	{
		public Vector3 min;

		public Vector3 max;

		public int leftIdx;

		public int rightIdx;

		public int triangleIdx;

		public int triangleCount;

		public bool IsLeaf => triangleIdx >= 0;
	}

	private struct RayData
	{
		public Vector3 origin;

		public Vector3 direction;

		public float length;
	}

	public GameObject EnviroRoot;

	private Transform target;

	private Mesh CastingMesh;

	private MeshFilter targetfilter;

	public ComputeShader computeShader;

	private ComputeBuffer BVHBuffer;

	private ComputeBuffer triangleBuffer;

	private ComputeBuffer rayBuffer;

	private ComputeBuffer resultBuffer;

	private int kernelIndex;

	public float distance = 3f;

	public int SplitCount = 64;

	public bool DebugGizmo;

	public int GizmoDepth;

	public bool GizmoOnlyLeafNode;

	public int GecursiveCount;

	private List<Triangle> TriangleList;

	private List<BvhData> BVHList;

	private static List<Triangle> CreateTriangles(GameObject rootMeshObject)
	{
		return rootMeshObject.GetComponentsInChildren<MeshFilter>().SelectMany((MeshFilter mf) =>
		{
			Mesh sharedMesh = mf.sharedMesh;
			int[] triangles = sharedMesh.triangles;
			Transform trans = mf.transform;
			List<Vector3> worldVertices = sharedMesh.vertices.Select((Vector3 vtx) => trans.TransformPoint(vtx)).ToList();
			return Enumerable.Range(0, triangles.Length / 3).Select((int i) =>
			{
				Vector3 vector = worldVertices[triangles[i * 3]];
				Vector3 vector2 = worldVertices[triangles[i * 3 + 1]];
				Vector3 vector3 = worldVertices[triangles[i * 3 + 2]];
				Vector3 normal = -Vector3.Cross(vector - vector2, vector3 - vector2).normalized;
				return new Triangle
				{
					pos0 = vector,
					pos1 = vector2,
					pos2 = vector3,
					normal = normal
				};
			});
		}).ToList();
	}

	public static (List<BvhData> bvhDatas, List<Triangle> triangles) BuildBvh(GameObject rootMeshObject, int splitCount)
	{
		List<Triangle> triangles = CreateTriangles(rootMeshObject);
		(List<BvhData>, List<int>) tuple = CreatteBvhDatas(CreateBvh(triangles, splitCount));
		List<BvhData> item = tuple.Item1;
		List<Triangle> item2 = tuple.Item2.Select((int idx) => triangles[idx]).ToList();
		return (bvhDatas: item, triangles: item2);
	}

	private static BvhNode CreateBvh(List<Triangle> triangles, int splitCount)
	{
		NativeArray<TriangleBounds> nativeArray = new NativeArray<TriangleBounds>(triangles.Count, Allocator.Temp);
		for (int i = 0; i < triangles.Count; i++)
		{
			Triangle triangle = triangles[i];
			Vector3 min = Vector3.Min(Vector3.Min(triangle.pos0, triangle.pos1), triangle.pos2);
			Vector3 max = Vector3.Max(Vector3.Max(triangle.pos0, triangle.pos1), triangle.pos2);
			TriangleBounds value = new TriangleBounds
			{
				bounds = new Bounds
				{
					min = min,
					max = max
				},
				triangleIndex = i
			};
			nativeArray[i] = value;
		}
		BvhNode result = CreateBvhRecursive(nativeArray, splitCount);
		nativeArray.Dispose();
		return result;
	}

	private static (List<BvhData>, List<int>) CreatteBvhDatas(BvhNode node)
	{
		List<BvhData> list = new List<BvhData>();
		List<int> list2 = new List<int>();
		CreatteBvhDatasRecursive(node, list, list2);
		return (list, list2);
	}

	private static BvhNode CreateBvhRecursive(NativeSlice<TriangleBounds> triangleBoundsArray, int splitCount, int recursiveCount = 0)
	{
		float split = 0f;
		int num = -1;
		if (triangleBoundsArray.Length >= 4)
		{
			(Bounds, float) tuple = CalcBoundsAndSAH(triangleBoundsArray);
			Bounds item = tuple.Item1;
			float num2 = tuple.Item2;
			Vector3 size = item.size;
			NativeArray<TriangleBounds> leftBuf = new NativeArray<TriangleBounds>(triangleBoundsArray.Length, Allocator.Temp);
			NativeArray<TriangleBounds> rightBuf = new NativeArray<TriangleBounds>(triangleBoundsArray.Length, Allocator.Temp);
			for (int i = 0; i < 3; i++)
			{
				if ((double)size[i] < 0.001)
				{
					continue;
				}
				float num3 = size[i] / (float)(splitCount / (recursiveCount + 1));
				float num4 = item.min[i] + num3;
				float num5 = item.max[i] - num3;
				for (float num6 = num4; num6 < num5; num6 += num3)
				{
					var (triangleBoundsArray2, triangleBoundsArray3) = SplitLR(triangleBoundsArray, i, num6, ref leftBuf, ref rightBuf);
					if (triangleBoundsArray2.Length > 1 && triangleBoundsArray3.Length > 1)
					{
						float item2 = CalcBoundsAndSAH(triangleBoundsArray2).Item2;
						float item3 = CalcBoundsAndSAH(triangleBoundsArray3).Item2;
						float num7 = item2 + item3;
						if (num7 < num2)
						{
							num2 = num7;
							num = i;
							split = num6;
						}
					}
				}
			}
			rightBuf.Dispose();
			leftBuf.Dispose();
		}
		BvhNode result;
		if (num < 0)
		{
			result = CreateBvhNodeLeaf(triangleBoundsArray);
		}
		else
		{
			NativeArray<TriangleBounds> leftBuf2 = new NativeArray<TriangleBounds>(triangleBoundsArray.Length, Allocator.Temp);
			NativeArray<TriangleBounds> rightBuf2 = new NativeArray<TriangleBounds>(triangleBoundsArray.Length, Allocator.Temp);
			(NativeSlice<TriangleBounds> left, NativeSlice<TriangleBounds> right) tuple3 = SplitLR(triangleBoundsArray, num, split, ref leftBuf2, ref rightBuf2);
			NativeSlice<TriangleBounds> item4 = tuple3.left;
			NativeSlice<TriangleBounds> item5 = tuple3.right;
			BvhNode bvhNode = CreateBvhRecursive(item4, splitCount, recursiveCount + 1);
			BvhNode bvhNode2 = CreateBvhRecursive(item5, splitCount, recursiveCount + 1);
			Bounds bounds = bvhNode.bounds;
			bounds.Encapsulate(bvhNode2.bounds);
			result = new BvhNode
			{
				bounds = bounds,
				left = bvhNode,
				right = bvhNode2
			};
			rightBuf2.Dispose();
			leftBuf2.Dispose();
		}
		return result;
		static BvhNode CreateBvhNodeLeaf(NativeSlice<TriangleBounds> nativeSlice)
		{
			return new BvhNode
			{
				bounds = CalcBounds(nativeSlice),
				triangleIndices = nativeSlice.Select((TriangleBounds n) => n.triangleIndex).ToList()
			};
		}
	}

	private static Bounds CalcBounds(NativeSlice<TriangleBounds> triangleBoundsArray)
	{
		Vector3 vector = Vector3.one * float.MaxValue;
		Vector3 vector2 = Vector3.one * float.MinValue;
		for (int i = 0; i < triangleBoundsArray.Length; i++)
		{
			Bounds bounds = triangleBoundsArray[i].bounds;
			vector = Vector3.Min(vector, bounds.min);
			vector2 = Vector3.Max(vector2, bounds.max);
		}
		return new Bounds
		{
			min = vector,
			max = vector2
		};
	}

	private static (Bounds, float) CalcBoundsAndSAH(NativeSlice<TriangleBounds> triangleBoundsArray)
	{
		Bounds item = CalcBounds(triangleBoundsArray);
		Vector3 size = item.size;
		float item2 = (float)triangleBoundsArray.Length * (size.x * size.y + size.x * size.y + size.y * size.z);
		return (item, item2);
	}

	private static void CreatteBvhDatasRecursive(BvhNode node, List<BvhData> datas, List<int> triangleIndexes)
	{
		BvhData bvhData = new BvhData
		{
			min = node.bounds.min,
			max = node.bounds.max,
			leftIdx = -1,
			rightIdx = -1,
			triangleIdx = -1,
			triangleCount = 0
		};
		if (node.IsLeaf)
		{
			int count = triangleIndexes.Count;
			triangleIndexes.AddRange(node.triangleIndices);
			bvhData.triangleIdx = count;
			bvhData.triangleCount = node.triangleIndices.Count;
			datas.Add(bvhData);
		}
		else
		{
			bvhData.triangleIdx = -1;
			int count2 = datas.Count;
			datas.Add(default);
			bvhData.leftIdx = datas.Count;
			CreatteBvhDatasRecursive(node.left, datas, triangleIndexes);
			bvhData.rightIdx = datas.Count;
			CreatteBvhDatasRecursive(node.right, datas, triangleIndexes);
			datas[count2] = bvhData;
		}
	}

	private static (NativeSlice<TriangleBounds> left, NativeSlice<TriangleBounds> right) SplitLR(NativeSlice<TriangleBounds> triBoundsArray, int axis, float split, ref NativeArray<TriangleBounds> leftBuf, ref NativeArray<TriangleBounds> rightBuf)
	{
		int length = 0;
		int length2 = 0;
		for (int i = 0; i < triBoundsArray.Length; i++)
		{
			TriangleBounds value = triBoundsArray[i];
			if (value.bounds.center[axis] < split)
			{
				leftBuf[length++] = value;
			}
			else
			{
				rightBuf[length2++] = value;
			}
		}
		return (left: leftBuf.Slice(0, length), right: rightBuf.Slice(0, length2));
	}

	private void Start()
	{
		kernelIndex = computeShader.FindKernel("CSMain");
		TriangleList = new List<Triangle>();
		BVHList = new List<BvhData>();
		(List<BvhData>, List<Triangle>) tuple = BuildBvh(EnviroRoot, SplitCount);
		BVHList = tuple.Item1;
		TriangleList = tuple.Item2;
		BVHBuffer = new ComputeBuffer(BVHList.Count, 40);
		triangleBuffer = new ComputeBuffer(TriangleList.Count, 48);
		BVHBuffer.SetData(BVHList);
		triangleBuffer.SetData(TriangleList);
		computeShader.SetBuffer(kernelIndex, "triangleBuffer", triangleBuffer);
		computeShader.SetBuffer(kernelIndex, "bvhBuffer", BVHBuffer);
		target = GetComponent<Transform>();
		targetfilter = target.GetComponent<MeshFilter>();
		CastingMesh = targetfilter.mesh;
	}

	private void Update()
	{
		int vertexCount = CastingMesh.vertexCount;
		List<RayData> list = new List<RayData>(vertexCount);
		for (int i = 0; i < vertexCount; i++)
		{
			Vector3 origin = target.transform.TransformPoint(CastingMesh.vertices[i]);
			Vector3 direction = target.transform.TransformDirection(CastingMesh.normals[i]);
			float length = distance;
			list.Add(new RayData
			{
				origin = origin,
				direction = direction,
				length = length
			});
		}
		rayBuffer = new ComputeBuffer(vertexCount, 28);
		rayBuffer.SetData(list);
		resultBuffer = new ComputeBuffer(vertexCount, 4);
		computeShader.SetBuffer(kernelIndex, "rays", rayBuffer);
		computeShader.SetBuffer(kernelIndex, "results", resultBuffer);
		computeShader.Dispatch(kernelIndex, vertexCount, 1, 1);
		int[] array = new int[vertexCount];
		resultBuffer.GetData(array);
		Color[] array2 = new Color[vertexCount];
		for (int j = 0; j < vertexCount; j++)
		{
			array2[j] = ((array[j] > 0) ? Color.red : Color.green);
		}
		CastingMesh.colors = array2;
		rayBuffer.Release();
		resultBuffer.Release();
	}

	private void OnDrawGizmos()
	{
		if (DebugGizmo && TriangleList.Count > 0 && BVHList.Count > 0)
		{
			DrawBvhGizmo(0, GizmoDepth, GizmoOnlyLeafNode);
		}
	}

	private void DrawBvhGizmo(int idx, int gizmoDepth, bool gizmoOnlyLeafNode, int recursiveCount = 0)
	{
		if (idx < 0 || BVHList.Count <= idx)
		{
			return;
		}
		BvhData bvhData = BVHList[idx];
		if (recursiveCount == gizmoDepth)
		{
			if (bvhData.IsLeaf)
			{
				Gizmos.color = Color.red;
				for (int i = 0; i < bvhData.triangleCount; i++)
				{
					Triangle triangle = TriangleList[i + bvhData.triangleIdx];
					Gizmos.DrawLine(triangle.pos0, triangle.pos1);
					Gizmos.DrawLine(triangle.pos0, triangle.pos2);
					Gizmos.DrawLine(triangle.pos1, triangle.pos2);
				}
			}
			if (!gizmoOnlyLeafNode || bvhData.IsLeaf)
			{
				Bounds bounds = new Bounds
				{
					min = bvhData.min,
					max = bvhData.max
				};
				Gizmos.color = (bvhData.IsLeaf ? Color.cyan : Color.green);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}
		else if (!bvhData.IsLeaf)
		{
			DrawBvhGizmo(bvhData.leftIdx, gizmoDepth, gizmoOnlyLeafNode, recursiveCount + 1);
			DrawBvhGizmo(bvhData.rightIdx, gizmoDepth, gizmoOnlyLeafNode, recursiveCount + 1);
		}
	}
}
