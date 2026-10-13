using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MTAssets.SkinnedMeshCombiner
{
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class CombinedMeshesManager : MonoBehaviour
	{
		public enum MergeMethod
		{
			OneMeshPerMaterial = 0,
			AllInOne = 1,
			JustMaterialColors = 2,
			OnlyAnima2dMeshes = 3
		}

		private const int MAX_VERTICES_FOR_16BITS_MESH = 50000;

		[HideInInspector]
		public GameObject rootGameObject;

		[HideInInspector]
		public MergeMethod mergeMethodUsed;

		[HideInInspector]
		public GameObject resultOfConversionToStaticMesh;

		[HideInInspector]
		public SkinnedMeshRenderer[] sourceMeshes;

		private void OnDestroy()
		{
			sourceMeshes = null;
			resultOfConversionToStaticMesh = null;
			rootGameObject = null;
		}

		public void ClearSourceMeshes()
		{
			sourceMeshes = null;
		}

		public void SplitCombinedMeshByMaterials()
		{
			SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			for (int i = 0; i < component.sharedMesh.subMeshCount; i++)
			{
				CombineInstance[] array = new CombineInstance[1];
				List<Transform> list = new List<Transform>();
				List<Matrix4x4> list2 = new List<Matrix4x4>();
				Transform[] bones = component.bones;
				for (int j = 0; j < bones.Length; j++)
				{
					list.Add(bones[j]);
					bool compatibilityMode = rootGameObject.GetComponent<SkinnedMeshCombiner>().compatibilityMode;
					if (compatibilityMode)
					{
						list2.Add(component.sharedMesh.bindposes[j] * component.transform.worldToLocalMatrix);
					}
					if (!compatibilityMode)
					{
						list2.Add(bones[j].worldToLocalMatrix * component.transform.worldToLocalMatrix);
					}
				}
				array[0].mesh = component.sharedMesh;
				array[0].subMeshIndex = i;
				array[0].transform = component.transform.localToWorldMatrix;
				Mesh mesh = new Mesh();
				if (component.sharedMesh.vertexCount <= 50000)
				{
					mesh.indexFormat = IndexFormat.UInt16;
				}
				if (component.sharedMesh.vertexCount > 50000)
				{
					mesh.indexFormat = IndexFormat.UInt32;
				}
				mesh.name = "Combined Mesh Splitted (Material " + i + ")";
				mesh.CombineMeshes(array, mergeSubMeshes: true, useMatrices: true);
				mesh.RecalculateBounds();
				GameObject obj = new GameObject("Combined Mesh Splitted (Material " + i + ")");
				obj.transform.SetParent(base.gameObject.transform);
				SkinnedMeshRenderer skinnedMeshRenderer = obj.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer.sharedMesh = mesh;
				skinnedMeshRenderer.bones = list.ToArray();
				skinnedMeshRenderer.sharedMesh.bindposes = list2.ToArray();
				skinnedMeshRenderer.rootBone = component.rootBone;
				skinnedMeshRenderer.sharedMaterials = new Material[1] { component.sharedMaterials[i] };
			}
			component.enabled = false;
		}
	}
}
