using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineChildrenPlus : MonoBehaviour
{
	public bool generateTriangleStrips = true;

	public List<GameObject> currentMeshes;

	public List<Renderer> allRenderers;

	public bool combineAtStart = true;

	public bool destroyOnDisable;

	[HideInInspector]
	public bool areaCombine;

	public bool removeColliders;

	private void Start()
	{
		if (combineAtStart)
		{
			Combine();
		}
	}

	public void DisableCombine()
	{
		if (currentMeshes != null)
		{
			for (int i = 0; i < currentMeshes.Count; i++)
			{
				if (currentMeshes[i] != null)
				{
					if (Application.isEditor)
					{
						UnityEngine.Object.DestroyImmediate(currentMeshes[i]);
					}
					else
					{
						UnityEngine.Object.Destroy(currentMeshes[i]);
					}
				}
			}
			currentMeshes.Clear();
		}
		if (allRenderers != null)
		{
			for (int j = 0; j < allRenderers.Count; j++)
			{
				if (allRenderers[j] != null)
				{
					allRenderers[j].enabled = true;
				}
			}
			allRenderers.Clear();
		}
		if (GetComponent(typeof(MeshFilter)) != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(GetComponent(typeof(MeshFilter)));
			}
			else
			{
				UnityEngine.Object.Destroy(GetComponent(typeof(MeshFilter)));
			}
		}
		if ((bool)GetComponent("MeshRenderer"))
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(GetComponent(typeof(MeshRenderer)));
			}
			else
			{
				UnityEngine.Object.Destroy(GetComponent("MeshRenderer"));
			}
		}
	}

	public void Combine()
	{
		if (!Application.isEditor)
		{
			DisableCombine();
		}
		currentMeshes = new List<GameObject>();
		allRenderers = new List<Renderer>();
		Component[] componentsInChildren = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		MeshFilter component = GetComponent<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)componentsInChildren[i];
			if (component != null && component == meshFilter)
			{
				continue;
			}
			if (removeColliders && meshFilter.GetComponent<Collider>() != null)
			{
				if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(meshFilter.GetComponent<Collider>());
				}
				else
				{
					UnityEngine.Object.Destroy(meshFilter.GetComponent<Collider>());
				}
			}
			Renderer component2 = componentsInChildren[i].GetComponent<Renderer>();
			MeshCombineUtilityPlus.MeshInstance meshInstance = new MeshCombineUtilityPlus.MeshInstance
			{
				mesh = meshFilter.sharedMesh
			};
			if (!(component2 != null) || !component2.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
			Material[] sharedMaterials = component2.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)hashtable[sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
					continue;
				}
				arrayList = new ArrayList();
				arrayList.Add(meshInstance);
				hashtable.Add(sharedMaterials[j], arrayList);
			}
			allRenderers.Add(component2);
			component2.enabled = false;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			MeshCombineUtilityPlus.MeshInstance[] combines = (MeshCombineUtilityPlus.MeshInstance[])((ArrayList)item.Value).ToArray(typeof(MeshCombineUtilityPlus.MeshInstance));
			if (hashtable.Count == 1)
			{
				if (GetComponent(typeof(MeshFilter)) == null)
				{
					base.gameObject.AddComponent(typeof(MeshFilter));
				}
				if (!GetComponent("MeshRenderer"))
				{
					base.gameObject.AddComponent<MeshRenderer>();
				}
				((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = MeshCombineUtilityPlus.Combine(combines, generateTriangleStrips);
				GetComponent<Renderer>().material = (Material)item.Key;
				GetComponent<Renderer>().enabled = true;
			}
			else
			{
				GameObject gameObject = new GameObject("Combined mesh");
				currentMeshes.Add(gameObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent(typeof(MeshFilter));
				gameObject.AddComponent<MeshRenderer>();
				gameObject.GetComponent<Renderer>().material = (Material)item.Key;
				((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).mesh = MeshCombineUtilityPlus.Combine(combines, generateTriangleStrips);
			}
		}
	}
}
