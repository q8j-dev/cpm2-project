using UnityEngine;

namespace CPMEngine
{
	public class CustomMaterial : MonoBehaviour
	{
		[SerializeField]
		private Surface SurfaceType;

		[SerializeField]
		private Surface VertexRsurfaceType;

		[SerializeField]
		private Surface VertexGsurfaceType;

		[SerializeField]
		private Surface VertexBsurfaceType;

		[SerializeField]
		private Surface VertexAsurfaceType;

		[SerializeField]
		private bool LogActive = true;

		private MeshCollider _meshCollider;

		private Mesh _mesh;

		private Color[] _vertexColors;

		private int[] _triangles;

		public Surface Surface => SurfaceType;

		private void Awake()
		{
		}

		public Surface GetSurfaceFromHit(RaycastHit hit)
		{
			if (_mesh == null || _vertexColors == null || _vertexColors.Length == 0)
			{
				return SurfaceType;
			}
			int num = hit.triangleIndex * 3;
			int num2 = _triangles[num];
			int num3 = _triangles[num + 1];
			int num4 = _triangles[num + 2];
			Color color = _vertexColors[num2];
			Color color2 = _vertexColors[num3];
			Color color3 = _vertexColors[num4];
			Vector3 barycentricCoordinate = hit.barycentricCoordinate;
			Color color4 = color * barycentricCoordinate.x + color2 * barycentricCoordinate.y + color3 * barycentricCoordinate.z;
			return DecodeSurface(color4);
		}

		private Surface DecodeSurface(Color color)
		{
			float num = Mathf.Max(color.r, color.g, color.b);
			if (num < 0.5f)
			{
				return VertexAsurfaceType;
			}
			if (Mathf.Approximately(num, color.r))
			{
				return VertexRsurfaceType;
			}
			if (Mathf.Approximately(num, color.g))
			{
				return VertexGsurfaceType;
			}
			if (!Mathf.Approximately(num, color.b))
			{
				return VertexAsurfaceType;
			}
			return VertexBsurfaceType;
		}

		private void Log(string log, bool isError = false)
		{
		}
	}
}
