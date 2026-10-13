using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MeshMerger : MonoBehaviour
{
	public void Merge()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(includeInactive: true);
		List<CombineInstance> list = new List<CombineInstance>();
		List<Material> list2 = new List<Material>();
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
			if (component == null || meshFilter.sharedMesh == null || meshFilter.transform == base.transform)
			{
				continue;
			}
			for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
			{
				list.Add(new CombineInstance
				{
					mesh = meshFilter.sharedMesh,
					subMeshIndex = j,
					transform = base.transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix
				});
				if (j < component.sharedMaterials.Length)
				{
					list2.Add(component.sharedMaterials[j]);
				}
				else
				{
					list2.Add(component.sharedMaterial);
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning("Nothing to merge");
			return;
		}
		Mesh mesh = new Mesh();
		mesh.name = base.gameObject.name + "_Merged";
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.CombineMeshes(list.ToArray(), mergeSubMeshes: false, useMatrices: true);
		MeshFilter meshFilter2 = GetComponent<MeshFilter>();
		if (!meshFilter2)
		{
			meshFilter2 = base.gameObject.AddComponent<MeshFilter>();
		}
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (!meshRenderer)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		meshFilter2.sharedMesh = mesh;
		meshRenderer.sharedMaterials = list2.ToArray();
		Debug.Log($"Merged {list.Count} meshes into 1");
	}

	public void MergeAndClean()
	{
		Merge();
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in base.transform)
		{
			list.Add(item.gameObject);
		}
		foreach (GameObject item2 in list)
		{
			Object.DestroyImmediate(item2);
		}
	}
}
