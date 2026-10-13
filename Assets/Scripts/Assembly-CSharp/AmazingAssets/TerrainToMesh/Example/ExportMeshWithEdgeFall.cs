using UnityEngine;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportMeshWithEdgeFall : MonoBehaviour
	{
		public TerrainData terrainData;

		public int vertexCountHorizontal = 100;

		public int vertexCountVertical = 100;

		public EdgeFall edgeFall = new EdgeFall(0f, true);

		public Texture2D edgeFallTexture;

		private void Start()
		{
			if (!(terrainData == null))
			{
				Mesh sharedMesh = terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh, edgeFall);
				GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				string shaderUnityDefault = TerrainToMeshConstants.shaderUnityDefault;
				string materailPropTextureMainTex = TerrainToMeshConstants.materailPropTextureMainTex;
				Material material = new Material(Shader.Find(shaderUnityDefault));
				Material material2 = new Material(Shader.Find(shaderUnityDefault));
				material2.SetTexture(materailPropTextureMainTex, edgeFallTexture);
				GetComponent<Renderer>().sharedMaterials = new Material[2] { material, material2 };
			}
		}
	}
}
