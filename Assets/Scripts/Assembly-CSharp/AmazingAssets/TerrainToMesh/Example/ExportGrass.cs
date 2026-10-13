using UnityEngine;
using UnityEngine.Rendering;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportGrass : MonoBehaviour
	{
		public TerrainData terrainData;

		public int vertexCountHorizontal = 100;

		public int vertexCountVertical = 100;

		private void Start()
		{
			if (terrainData == null)
			{
				return;
			}
			Mesh sharedMesh = terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh);
			GetComponent<MeshFilter>().sharedMesh = sharedMesh;
			Material sharedMaterial = new Material(Shader.Find(TerrainToMeshConstants.shaderUnityDefault));
			GetComponent<Renderer>().sharedMaterial = sharedMaterial;
			DetailPrototypesData[] array = terrainData.TerrainToMesh().ExportGrassData(vertexCountHorizontal, vertexCountVertical, 1, 1, 8, 1f);
			Mesh sharedMesh2 = TerrainToMeshUtilities.CreateGrassMesh(1);
			Shader shader = Shader.Find(TerrainToMeshConstants.shaderGrass);
			string materailPropTextureMainTex = TerrainToMeshConstants.materailPropTextureMainTex;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = new GameObject("Grass");
				gameObject.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
				Material material = new Material(shader);
				material.SetTexture(materailPropTextureMainTex, array[i].detailPrototype.prototypeTexture);
				for (int j = 0; j < array[i].position.Count; j++)
				{
					GameObject obj = new GameObject("Grass");
					obj.AddComponent<MeshFilter>().sharedMesh = sharedMesh2;
					obj.AddComponent<MeshRenderer>().sharedMaterial = material;
					obj.transform.SetParent(gameObject.transform, worldPositionStays: true);
					obj.transform.localPosition = array[i].position[j];
					obj.transform.localRotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
					obj.transform.localScale = array[i].scale[j];
				}
				TerrainToMeshUtilities.CombineGameObjects(gameObject, material, "Grass", array[i].detailPrototype.prototypeTexture.name, IndexFormat.UInt16);
			}
		}
	}
}
