using UnityEngine;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportMesh : MonoBehaviour
	{
		public TerrainData terrainData;

		public int vertexCountHorizontal = 100;

		public int vertexCountVertical = 100;

		private void Start()
		{
			if (!(terrainData == null))
			{
				Mesh sharedMesh = terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh);
				GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				Material sharedMaterial = new Material(Shader.Find(TerrainToMeshConstants.shaderUnityDefault));
				GetComponent<Renderer>().sharedMaterial = sharedMaterial;
			}
		}
	}
}
