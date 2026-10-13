using UnityEngine;

namespace AmazingAssets.TerrainToMesh.Example
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ExportDetailMesh : MonoBehaviour
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
			DetailPrototypesData[] array = terrainData.TerrainToMesh().ExportDetailMeshData(vertexCountHorizontal, vertexCountVertical, 1, 1, 8, 1f);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].position.Count; j++)
				{
					GameObject obj = Object.Instantiate(array[i].detailPrototype.prototype);
					obj.transform.position = array[i].position[j];
					obj.transform.localScale = array[i].scale[j];
					obj.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
				}
			}
		}
	}
}
