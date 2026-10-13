using UnityEngine;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportMeshAndSplatmap : MonoBehaviour
	{
		public TerrainData terrainData;

		public int vertexCountHorizontal = 100;

		public int vertexCountVertical = 100;

		public bool terrainHasHoles;

		public bool createFallbackTextures;

		private void Start()
		{
			if (!(terrainData == null))
			{
				Mesh sharedMesh = terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh);
				GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				Material material = terrainData.TerrainToMesh().ExportSplatmapMaterial(terrainHasHoles);
				GetComponent<Renderer>().sharedMaterial = material;
				if (createFallbackTextures)
				{
					Texture2D value = terrainData.TerrainToMesh().ExportBasemapDiffuseTexture(1024, terrainHasHoles, unpack: false);
					Texture2D value2 = terrainData.TerrainToMesh().ExportBasemapNormalTexture(1024, unpack: false);
					material.SetTexture(TerrainToMeshConstants.materailPropTextureMainTex, value);
					material.SetTexture(TerrainToMeshConstants.materailPropTextureBumpMap, value2);
				}
			}
		}
	}
}
