using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CPM.Vehicle.Damage
{
	public static class DamageTopologyFactory
	{
		[ThreadStatic]
		private static List<Vector3> _vertexBuffer;

		[ThreadStatic]
		private static List<int> _indexBuffer;

		[ThreadStatic]
		private static List<int> _subMeshBuffer;

		public static DamageTopology FromMesh(Mesh mesh, in DamageSettings settings, Allocator allocator)
		{
			if (_vertexBuffer == null)
			{
				_vertexBuffer = new List<Vector3>();
			}
			if (_indexBuffer == null)
			{
				_indexBuffer = new List<int>();
			}
			if (_subMeshBuffer == null)
			{
				_subMeshBuffer = new List<int>();
			}
			_vertexBuffer.Clear();
			_indexBuffer.Clear();
			mesh.GetVertices(_vertexBuffer);
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				_subMeshBuffer.Clear();
				mesh.GetTriangles(_subMeshBuffer, i);
				_indexBuffer.AddRange(_subMeshBuffer);
			}
			NativeArray<float3> positions = new NativeArray<float3>(_vertexBuffer.Count, Allocator.Temp);
			NativeArray<int> triangles = new NativeArray<int>(_indexBuffer.Count, Allocator.Temp);
			try
			{
				for (int j = 0; j < _vertexBuffer.Count; j++)
				{
					positions[j] = _vertexBuffer[j];
				}
				for (int k = 0; k < _indexBuffer.Count; k++)
				{
					triangles[k] = _indexBuffer[k];
				}
				return DamageTopology.Build(positions, triangles, in settings, allocator);
			}
			finally
			{
				positions.Dispose();
				triangles.Dispose();
			}
		}
	}
}
