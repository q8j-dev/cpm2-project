using System;
using UnityEngine;

namespace MTAssets.SkinnedMeshCombiner
{
	public class SkinnedMeshConverter : MonoBehaviour
	{
		[HideInInspector]
		public GameObject resultOfConversion;

		[HideInInspector]
		public string resultOfConversionAssetSaved = "";

		[HideInInspector]
		public Transform boneThatWillMove;

		[HideInInspector]
		public bool flipNormals;

		[HideInInspector]
		public bool saveDataInAssets = true;

		private string ValidateThisComponentAndGetCauseIfIsNotValid(bool sendLogsOnConsole)
		{
			string text = "";
			MeshRenderer component = GetComponent<MeshRenderer>();
			MeshFilter component2 = GetComponent<MeshFilter>();
			if (component2 == null || component == null)
			{
				text = "Apparently the GameObject the Skinned Mesh Converter is in does not contain a valid static mesh. Please make sure that the component is in a GameObject that contains a Mesh Renderer and Mesh Filter with mesh.";
			}
			if (component2 != null && component2.sharedMesh == null)
			{
				text = "Apparently the GameObject the Skinned Mesh Converter is in does not contain a valid static mesh. Please make sure that the component is in a GameObject that contains a Mesh Renderer and Mesh Filter with mesh.";
			}
			Animator componentInParent = GetComponentInParent<Animator>();
			if (GetComponentInParent<Animation>() == null && componentInParent == null)
			{
				text = "Apparently the Skinned Mesh Converter is not in a character with an Animator or Animation. The Skinned Mesh Converter must be inside a character, in the GameObject of a static mesh (sword, hat, glasses, etc.) derived from a character's bone.";
			}
			if (GetComponentInParent<SkinnedMeshCombiner>() == null)
			{
				text = "The Skinned Mesh Converter must be inside a GameObject that contains a static mesh (sword, hat, glasses, etc.) that is the child of a character's bone. This character must contain an Animator or Animation and must also contain the Skinned Mesh Combiner component at its root.";
			}
			if (sendLogsOnConsole && text != "")
			{
				Debug.LogError(text);
			}
			return text;
		}

		private void DoConvertMesh()
		{
			if (ValidateThisComponentAndGetCauseIfIsNotValid(sendLogsOnConsole: true) != "")
			{
				return;
			}
			if (isMeshConverted())
			{
				Debug.LogError("The mesh could not be converted to Skinned, the mesh is already converted.");
				return;
			}
			resultOfConversion = new GameObject(base.gameObject.name + " (Converted To Skinned)");
			resultOfConversion.transform.parent = base.gameObject.transform;
			SkinnedMeshRenderer skinnedMeshRenderer = resultOfConversion.AddComponent<SkinnedMeshRenderer>();
			MeshRenderer component = GetComponent<MeshRenderer>();
			MeshFilter component2 = GetComponent<MeshFilter>();
			Mesh mesh = null;
			if (component2 != null)
			{
				mesh = component2.sharedMesh;
			}
			Mesh mesh2 = new Mesh();
			mesh2.name = mesh.name + " (Converted To Skinned)";
			Vector3[] array = new Vector3[mesh.vertices.Length];
			Vector2[] array2 = new Vector2[mesh.uv.Length];
			Vector2[] array3 = new Vector2[mesh.uv2.Length];
			Vector2[] array4 = new Vector2[mesh.uv3.Length];
			Vector2[] array5 = new Vector2[mesh.uv4.Length];
			Vector2[] array6 = new Vector2[mesh.uv5.Length];
			Vector2[] array7 = new Vector2[mesh.uv6.Length];
			Vector2[] array8 = new Vector2[mesh.uv7.Length];
			Vector2[] array9 = new Vector2[mesh.uv8.Length];
			int[] array10 = new int[mesh.triangles.Length];
			Vector3[] array11 = new Vector3[mesh.normals.Length];
			Vector4[] array12 = new Vector4[mesh.tangents.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = mesh.vertices[i];
			}
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = mesh.uv[j];
			}
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k] = mesh.uv2[k];
			}
			for (int l = 0; l < array4.Length; l++)
			{
				array4[l] = mesh.uv3[l];
			}
			for (int m = 0; m < array5.Length; m++)
			{
				array5[m] = mesh.uv4[m];
			}
			for (int n = 0; n < array6.Length; n++)
			{
				array6[n] = mesh.uv5[n];
			}
			for (int num = 0; num < array7.Length; num++)
			{
				array7[num] = mesh.uv6[num];
			}
			for (int num2 = 0; num2 < array8.Length; num2++)
			{
				array8[num2] = mesh.uv7[num2];
			}
			for (int num3 = 0; num3 < array9.Length; num3++)
			{
				array9[num3] = mesh.uv8[num3];
			}
			if (!flipNormals)
			{
				for (int num4 = 0; num4 < array10.Length; num4++)
				{
					array10[num4] = mesh.triangles[num4];
				}
				for (int num5 = 0; num5 < array11.Length; num5++)
				{
					array11[num5] = mesh.normals[num5];
				}
			}
			if (flipNormals)
			{
				for (int num6 = 0; num6 < array10.Length; num6++)
				{
					array10[num6] = mesh.triangles[array10.Length - 1 - num6];
				}
				for (int num7 = 0; num7 < array11.Length; num7++)
				{
					array11[num7] = mesh.normals[num7] * -1f;
				}
			}
			for (int num8 = 0; num8 < array12.Length; num8++)
			{
				array12[num8] = mesh.tangents[num8];
			}
			BoneWeight[] array13 = new BoneWeight[array.Length];
			for (int num9 = 0; num9 < array13.Length; num9++)
			{
				array13[num9].boneIndex0 = 0;
				array13[num9].weight0 = 1f;
			}
			Transform[] array14 = new Transform[1] { boneThatWillMove };
			Matrix4x4[] bindposes = new Matrix4x4[1] { array14[0].worldToLocalMatrix * base.transform.localToWorldMatrix };
			mesh2.vertices = array;
			mesh2.uv = array2;
			mesh2.uv2 = array3;
			mesh2.uv3 = array4;
			mesh2.uv4 = array5;
			mesh2.uv5 = array6;
			mesh2.uv6 = array7;
			mesh2.uv7 = array8;
			mesh2.uv8 = array9;
			mesh2.triangles = array10;
			mesh2.normals = array11;
			mesh2.tangents = array12;
			mesh2.bindposes = bindposes;
			mesh2.boneWeights = array13;
			skinnedMeshRenderer.sharedMesh = mesh2;
			skinnedMeshRenderer.bones = array14;
			skinnedMeshRenderer.sharedMaterials = component.sharedMaterials;
			skinnedMeshRenderer.rootBone = boneThatWillMove;
			resultOfConversion.AddComponent<ConvertedMeshesManager>().converterOriginalGameObject = base.gameObject;
			component.enabled = false;
			if (!Application.isPlaying)
			{
				Debug.Log("The mesh was successfully converted to a Skinned version.");
			}
		}

		private void DoUndoConvertMesh(bool runUnityGc, bool runMonoIl2CppGc)
		{
			SkinnedMeshCombiner[] componentsInParent = GetComponentsInParent<SkinnedMeshCombiner>();
			if (componentsInParent.Length == 0)
			{
				Debug.LogError("It was not possible to undo the mesh conversion back to static as it was not possible to find a Skinned Mesh Combiner component linked to this hierarchy of bones or character.");
				return;
			}
			bool flag = false;
			SkinnedMeshCombiner[] array = componentsInParent;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isMeshesCombined())
				{
					flag = true;
				}
			}
			if (flag)
			{
				Debug.LogError("It was not possible to undo the mesh conversion back to static. Before undoing the conversion, you need to undo any blends that are currently working on this character's Skinned Mesh Combiner component.");
				return;
			}
			if (!isMeshConverted())
			{
				Debug.LogError("It was not possible to undo the conversion of the mesh to static. The mesh has not yet been converted.");
				return;
			}
			MeshRenderer component = GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = true;
			}
			UnityEngine.Object.Destroy(resultOfConversion);
			if (runMonoIl2CppGc)
			{
				GC.Collect();
			}
			if (!Application.isPlaying)
			{
				Debug.Log("The conversion of the mesh, to Skinned, was undone.");
			}
		}

		public bool isMeshConverted()
		{
			if (resultOfConversion != null)
			{
				return true;
			}
			return false;
		}

		public void ConvertMesh()
		{
			DoConvertMesh();
		}

		public void UndoMeshConversion(bool runUnityGc, bool runMonoIl2CppGc)
		{
			DoUndoConvertMesh(runUnityGc, runMonoIl2CppGc);
		}

		public SkinnedMeshRenderer GetMeshConvertedToSkinned()
		{
			if (!isMeshConverted())
			{
				ConvertMesh();
			}
			return resultOfConversion.GetComponent<SkinnedMeshRenderer>();
		}
	}
}
