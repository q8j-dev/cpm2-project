using System;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
	[Serializable]
	public enum Role
	{
		No = 0,
		Police = 1
	}

	private const float ShoesBoundsExtentYPadding = 0.5f;

	[Header("IDs")]
	public int typeID;

	public int id;

	[Header("Settings")]
	public bool HidesBodyCompletely;

	public bool HideBeard;

	public int hairToSet;

	public PersonElements.Pack pack;

	public SkinnedMeshRenderer skinnedMeshRendererPerson;

	public Role role;

	[Header("Temporary")]
	public Mesh defaultMesh;

	public Mesh originalMesh;

	public Material[] defaultMats;

	public Material[] originalMats;

	private void Start()
	{
		MergeBones();
		ExpandShoesBounds();
	}

	private void ExpandShoesBounds()
	{
		if (typeID == 9 && TryGetComponent<SkinnedMeshRenderer>(out var component))
		{
			Bounds localBounds = component.localBounds;
			localBounds.extents += new Vector3(0f, 0.5f, 0f);
			component.localBounds = localBounds;
		}
	}

	[ContextMenu("MergeBones")]
	private void MergeBones()
	{
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		Transform[] bones = skinnedMeshRendererPerson.bones;
		foreach (Transform transform in bones)
		{
			if (!dictionary.ContainsKey(transform.name))
			{
				dictionary[transform.name] = transform;
			}
		}
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		Transform[] bones2 = component.bones;
		Transform[] array = new Transform[bones2.Length];
		Transform transform2 = null;
		string text = component.rootBone.name;
		for (int j = 0; j < bones2.Length; j++)
		{
			string text2 = bones2[j].name;
			dictionary.TryGetValue(text2, out array[j]);
			if (text2 == text && transform2 == null)
			{
				transform2 = array[j];
			}
		}
		component.bones = array;
		component.rootBone = transform2;
		base.transform.parent = skinnedMeshRendererPerson.transform.parent;
	}
}
