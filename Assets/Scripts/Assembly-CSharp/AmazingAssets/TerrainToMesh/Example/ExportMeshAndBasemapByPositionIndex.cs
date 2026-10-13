using UnityEngine;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportMeshAndBasemapByPositionIndex : MonoBehaviour
	{
		public TerrainData terrainData;

		public int vertexCountHorizontal = 100;

		public int vertexCountVertical = 100;

		public int mapsResolution = 512;

		public bool exportHoles;

		private int chunkCountHorizontal = 4;

		private int chunkCountVertical = 4;

		[Header("Chunk count is 4x4")]
		[Range(0f, 3f)]
		public int positionX;

		[Range(0f, 3f)]
		public int positionY;

		private void Start()
		{
			if (!(terrainData == null))
			{
				Mesh sharedMesh = terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, chunkCountHorizontal, chunkCountVertical, positionX, positionY, perChunkUV: true, Normal.CalculateFromMesh);
				GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				Texture2D value = terrainData.TerrainToMesh().ExportBasemapDiffuseTexture(mapsResolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY, exportHoles, unpack: false);
				Texture2D texture2D = terrainData.TerrainToMesh().ExportBasemapNormalTexture(mapsResolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY, unpack: false);
				Texture2D texture2D2 = null;
				if (TerrainToMeshUtilities.GetCurrentRenderPipeline() != TerrainToMeshUtilities.RenderPipeline.Builtin)
				{
					texture2D2 = terrainData.TerrainToMesh().ExportBasemapMaskTexture(mapsResolution, chunkCountHorizontal, chunkCountVertical, positionX, positionY, unpack: false);
				}
				string shaderUnityDefault = TerrainToMeshConstants.shaderUnityDefault;
				string materailPropTextureMainTex = TerrainToMeshConstants.materailPropTextureMainTex;
				string materailPropTextureBumpMap = TerrainToMeshConstants.materailPropTextureBumpMap;
				string materailPropTextureMetallicSmoothnessMap = TerrainToMeshConstants.materailPropTextureMetallicSmoothnessMap;
				Material material = new Material(Shader.Find(shaderUnityDefault));
				InitializeMaterial(material);
				material.SetTexture(materailPropTextureMainTex, value);
				if (texture2D != null)
				{
					material.SetTexture(materailPropTextureBumpMap, texture2D);
					material.EnableKeyword("_NORMALMAP");
				}
				if (texture2D2 != null)
				{
					material.SetTexture(materailPropTextureMetallicSmoothnessMap, texture2D2);
					material.EnableKeyword("_METALLICGLOSSMAP");
				}
				if (exportHoles)
				{
					SetupAlphaTest(material);
				}
				GetComponent<Renderer>().sharedMaterial = material;
			}
		}

		private void InitializeMaterial(Material material)
		{
			if (TerrainToMeshUtilities.GetCurrentRenderPipeline() == TerrainToMeshUtilities.RenderPipeline.HighDefinition)
			{
				if (material.HasProperty("_DistortionSrcBlend"))
				{
					material.SetFloat("_DistortionSrcBlend", 1f);
				}
				if (material.HasProperty("_DistortionDstBlend"))
				{
					material.SetFloat("_DistortionDstBlend", 1f);
				}
				if (material.HasProperty("_DistortionBlurSrcBlend"))
				{
					material.SetFloat("_DistortionBlurSrcBlend", 1f);
				}
				if (material.HasProperty("_DistortionBlurDstBlend"))
				{
					material.SetFloat("_DistortionBlurDstBlend", 1f);
				}
				if (material.HasProperty("_StencilWriteMask"))
				{
					material.SetFloat("_StencilWriteMask", 6f);
				}
				if (material.HasProperty("_StencilRefGBuffer"))
				{
					material.SetFloat("_StencilRefGBuffer", 10f);
				}
				if (material.HasProperty("_StencilWriteMaskGBuffer"))
				{
					material.SetFloat("_StencilWriteMaskGBuffer", 14f);
				}
				if (material.HasProperty("_StencilRefDepth"))
				{
					material.SetFloat("_StencilRefDepth", 8f);
				}
				if (material.HasProperty("_StencilRefMV"))
				{
					material.SetFloat("_StencilRefMV", 40f);
				}
				if (material.HasProperty("_StencilWriteMaskMV"))
				{
					material.SetFloat("_StencilWriteMaskMV", 40f);
				}
				if (material.HasProperty("_ZTestDepthEqualForOpaque"))
				{
					material.SetFloat("_ZTestDepthEqualForOpaque", 3f);
				}
				if (material.HasProperty("_ZTestModeDistortion"))
				{
					material.SetFloat("_ZTestModeDistortion", 4f);
				}
				material.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
			}
		}

		private void SetupAlphaTest(Material material)
		{
			material.EnableKeyword("_ALPHATEST_ON");
			material.renderQueue = 2450;
			switch (TerrainToMeshUtilities.GetCurrentRenderPipeline())
			{
			case TerrainToMeshUtilities.RenderPipeline.Builtin:
				if (material.HasProperty("_Mode"))
				{
					material.SetFloat("_Mode", 1f);
				}
				break;
			case TerrainToMeshUtilities.RenderPipeline.Universal:
				if (material.HasProperty("_AlphaClip"))
				{
					material.SetFloat("_AlphaClip", 1f);
				}
				break;
			case TerrainToMeshUtilities.RenderPipeline.HighDefinition:
				if (!material.IsKeywordEnabled("_DISABLE_SSR_TRANSPARENT"))
				{
					material.EnableKeyword("_DISABLE_SSR_TRANSPARENT");
				}
				if (material.HasProperty("_AlphaCutoffEnable"))
				{
					material.SetFloat("_AlphaCutoffEnable", 1f);
				}
				if (material.HasProperty("_ZTestGBuffer"))
				{
					material.SetFloat("_ZTestGBuffer", 3f);
				}
				break;
			}
		}
	}
}
