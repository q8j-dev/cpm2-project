using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren3 : MonoBehaviour
{
	public bool isAddMeshCollider;

	public bool CastShadow = true;

	private void Awake()
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Dictionary<Material, List<CombineInstance>> dictionary = new Dictionary<Material, List<CombineInstance>>();
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material != null && !dictionary.ContainsKey(material))
				{
					dictionary.Add(material, new List<CombineInstance>());
				}
			}
		}
		MeshFilter[] componentsInChildren2 = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren2)
		{
			if (!(meshFilter.sharedMesh == null))
			{
				CombineInstance item = new CombineInstance
				{
					mesh = meshFilter.sharedMesh,
					transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix
				};
				dictionary[meshFilter.GetComponent<Renderer>().sharedMaterial].Add(item);
				meshFilter.GetComponent<Renderer>().enabled = false;
				Object.Destroy(meshFilter.gameObject);
			}
		}
		foreach (Material key in dictionary.Keys)
		{
			GameObject gameObject = new GameObject("Combined mesh");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.AddComponent<MeshFilter>().mesh.CombineMeshes(dictionary[key].ToArray(), mergeSubMeshes: true, useMatrices: true);
			if (isAddMeshCollider)
			{
				gameObject.AddComponent<MeshCollider>();
			}
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = key;
			_ = key.shader.name == "";
			if (!CastShadow)
			{
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			}
		}
	}
}
