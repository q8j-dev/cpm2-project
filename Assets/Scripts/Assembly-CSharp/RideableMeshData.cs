using System;
using UnityEngine;

[Serializable]
public class RideableMeshData
{
	[Serializable]
	public class SubmeshData
	{
		public int trianglesCount;

		public Surface surface;

		public bool isVertexBlend;

		public Surface surfaceBase;

		public Surface surfaceR;

		public Surface surfaceG;

		public Surface surfaceB;

		public int submeshIndex;

		public int triangleStart;

		[NonSerialized]
		public int[] triangleIndices;
	}

	public SubmeshData[] submeshDatas;

	public Mesh mesh;

	[NonSerialized]
	public Color32[] meshVertexColors;
}
