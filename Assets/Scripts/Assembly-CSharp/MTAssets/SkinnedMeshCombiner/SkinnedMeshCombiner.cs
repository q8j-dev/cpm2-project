using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace MTAssets.SkinnedMeshCombiner
{
	public class SkinnedMeshCombiner : MonoBehaviour
	{
		public enum LogTypeOf
		{
			Assert = 0,
			Error = 1,
			Exception = 2,
			Log = 3,
			Warning = 4
		}

		public enum MergeMethod
		{
			OneMeshPerMaterial = 0,
			AllInOne = 1,
			JustMaterialColors = 2,
			OnlyAnima2dMeshes = 3
		}

		public enum AtlasSize
		{
			Pixels32x32 = 0,
			Pixels64x64 = 1,
			Pixels128x128 = 2,
			Pixels256x256 = 3,
			Pixels512x512 = 4,
			Pixels1024x1024 = 5,
			Pixels2048x2048 = 6,
			Pixels4096x4096 = 7,
			Pixels8192x8192 = 8
		}

		public enum AnimQuality
		{
			UseQualitySettings = 0,
			Bad = 1,
			Good = 2,
			VeryGood = 3
		}

		public enum MipMapEdgesSize
		{
			Pixels0x0 = 0,
			Pixels16x16 = 1,
			Pixels32x32 = 2,
			Pixels64x64 = 3,
			Pixels128x128 = 4,
			Pixels256x256 = 5,
			Pixels512x512 = 6,
			Pixels1024x1024 = 7
		}

		public enum AtlasPadding
		{
			Pixels0x0 = 0,
			Pixels2x2 = 1,
			Pixels4x4 = 2,
			Pixels8x8 = 3,
			Pixels16x16 = 4
		}

		public enum MergeTiledTextures
		{
			SkipAll = 0,
			ImprovedMode = 1,
			LegacyMode = 2
		}

		public enum BlendShapesSupport
		{
			Disabled = 0,
			Enabled = 1,
			FullSupport = 2
		}

		public enum CombineOnStart
		{
			Disabled = 0,
			OnStart = 1,
			OnAwake = 2
		}

		public enum RootBoneToUse
		{
			Automatic = 0,
			Manual = 1
		}

		[Serializable]
		public class LogOfMerge
		{
			public string content;

			public LogTypeOf logType;

			public LogOfMerge(string content, LogTypeOf logType)
			{
				this.content = content;
				this.logType = logType;
			}
		}

		[Serializable]
		public class OneMeshPerMaterialParams
		{
			public bool mergeOnlyEqualRootBones;
		}

		[Serializable]
		public class JustMaterialColorsParams
		{
			public Material materialToUse;

			public bool mergeOnlyEqualsRootBones;

			public bool useDefaultColorProperty = true;

			public string colorPropertyToFind = "_Color";

			public string mainTexturePropertyToInsert = "_MainTex";
		}

		[Serializable]
		public class AllInOneParams
		{
			public Material materialToUse;

			public AtlasSize atlasResolution = AtlasSize.Pixels512x512;

			public MipMapEdgesSize mipMapEdgesSize = MipMapEdgesSize.Pixels64x64;

			public AtlasPadding atlasPadding;

			public MergeTiledTextures mergeTiledTextures = MergeTiledTextures.LegacyMode;

			public bool mergeOnlyEqualsRootBones;

			public bool useDefaultMainTextureProperty = true;

			public string mainTexturePropertyToFind = "_MainTex";

			public string mainTexturePropertyToInsert = "_MainTex";

			public bool metallicMapSupport;

			public string metallicMapPropertyToFind = "_MetallicGlossMap";

			public string metallicMapPropertyToInsert = "_MetallicGlossMap";

			public bool specularMapSupport;

			public string specularMapPropertyToFind = "_SpecGlossMap";

			public string specularMapPropertyToInsert = "_SpecGlossMap";

			public bool normalMapSupport;

			public string normalMapPropertyToFind = "_BumpMap";

			public string normalMapPropertyToInsert = "_BumpMap";

			public bool normalMap2Support;

			public string normalMap2PropertyFind = "_DetailNormalMap";

			public string normalMap2PropertyToInsert = "_DetailNormalMap";

			public bool heightMapSupport;

			public string heightMapPropertyToFind = "_ParallaxMap";

			public string heightMapPropertyToInsert = "_ParallaxMap";

			public bool occlusionMapSupport;

			public string occlusionMapPropertyToFind = "_OcclusionMap";

			public string occlusionMapPropertyToInsert = "_OcclusionMap";

			public bool detailAlbedoMapSupport;

			public string detailMapPropertyToFind = "_DetailAlbedoMap";

			public string detailMapPropertyToInsert = "_DetailAlbedoMap";

			public bool detailMaskSupport;

			public string detailMaskPropertyToFind = "_DetailMask";

			public string detailMaskPropertyToInsert = "_DetailMask";

			public bool pinkNormalMapsFix = true;
		}

		[Serializable]
		public class OnlyAnima2dMeshes
		{
			public AtlasSize atlasResolution = AtlasSize.Pixels512x512;
		}

		private class BlendShapeData
		{
			public string blendShapeFrameName = "";

			public int blendShapeFrameIndex = -1;

			public float blendShapeCurrentValue;

			public List<Vector3> startDeltaVertices = new List<Vector3>();

			public List<Vector3> startDeltaNormals = new List<Vector3>();

			public List<Vector3> startDeltaTangents = new List<Vector3>();

			public List<Vector3> finalDeltaVertices = new List<Vector3>();

			public List<Vector3> finalDeltaNormals = new List<Vector3>();

			public List<Vector3> finalDeltaTangents = new List<Vector3>();
		}

		private class SubMeshToCombine
		{
			public Transform transform;

			public SkinnedMeshRenderer skinnedMeshRenderer;

			public int subMeshIndex;

			public SubMeshToCombine(Transform transform, SkinnedMeshRenderer skinnedMeshRenderer, int subMeshIndex)
			{
				this.transform = transform;
				this.skinnedMeshRenderer = skinnedMeshRenderer;
				this.subMeshIndex = subMeshIndex;
			}
		}

		private class SubMeshesCombined
		{
			public Matrix4x4 localToWorldMatrix;

			public Mesh subMeshesMerged;

			public Transform[] bonesToMerge;

			public Matrix4x4[] bindPosesToMerge;

			public Material thisMaterial;

			public SubMeshesCombined(Matrix4x4 localToWorldMatrix, Mesh subMeshesMerged, Transform[] bonesToMerge, Matrix4x4[] bindPosesToMerge, Material thisMaterial)
			{
				this.localToWorldMatrix = localToWorldMatrix;
				this.subMeshesMerged = subMeshesMerged;
				this.bonesToMerge = bonesToMerge;
				this.bindPosesToMerge = bindPosesToMerge;
				this.thisMaterial = thisMaterial;
			}
		}

		private class TexturesSubMeshes
		{
			public class UvBounds
			{
				public float majorX;

				public float majorY;

				public float minorX;

				public float minorY;

				public float spaceMinorX;

				public float spaceMajorX;

				public float spaceMinorY;

				public float spaceMajorY;

				public float edgesUseX;

				public float edgesUseY;

				public float Round(float value, int places)
				{
					return float.Parse(value.ToString("F" + places));
				}

				public void RoundBoundsValuesAndCalculateSpaceNeededToTiling()
				{
					majorX = Round(majorX, 4);
					majorY = Round(majorY, 4);
					minorX = Round(minorX, 4);
					minorY = Round(minorY, 4);
					if (minorX >= 0f)
					{
						spaceMinorX = 0f;
					}
					if (minorX < 0f)
					{
						spaceMinorX = minorX * -1f;
					}
					if (minorY >= 0f)
					{
						spaceMinorY = 0f;
					}
					if (minorY < 0f)
					{
						spaceMinorY = minorY * -1f;
					}
					if (majorY >= 1f)
					{
						spaceMajorY = majorY - 1f;
					}
					if (majorX >= 1f)
					{
						spaceMajorX = majorX - 1f;
					}
				}
			}

			public class UserSubMeshes
			{
				public UvBounds uvBoundsOfThisSubMesh = new UvBounds();

				public int startOfUvVerticesInIndex;

				public Vector2[] originalUvVertices;
			}

			public Material material;

			public Texture2D mainTexture;

			public Texture2D metallicMap;

			public Texture2D specularMap;

			public Texture2D normalMap;

			public Texture2D normalMap2;

			public Texture2D heightMap;

			public Texture2D occlusionMap;

			public Texture2D detailMap;

			public Texture2D detailMask;

			public bool isTiledTexture;

			public Vector2Int mainTextureResolution;

			public Vector2Int mainTextureResolutionWithEdges;

			public List<UserSubMeshes> userSubMeshes = new List<UserSubMeshes>();

			public Vector2 GetEdgesPercentUsageOfThisTextures()
			{
				return new Vector2(userSubMeshes[0].uvBoundsOfThisSubMesh.edgesUseX, userSubMeshes[0].uvBoundsOfThisSubMesh.edgesUseY);
			}

			public void ConvertAllSubMeshsVerticesToPositive()
			{
				foreach (UserSubMeshes userSubMesh in userSubMeshes)
				{
					float[] array = new float[userSubMesh.originalUvVertices.Length];
					float[] array2 = new float[userSubMesh.originalUvVertices.Length];
					for (int i = 0; i < userSubMesh.originalUvVertices.Length; i++)
					{
						array[i] = userSubMesh.originalUvVertices[i].x;
						array2[i] = userSubMesh.originalUvVertices[i].y;
					}
					Vector2 vector = new Vector2(Mathf.Min(array), Mathf.Min(array2));
					for (int j = 0; j < userSubMesh.originalUvVertices.Length; j++)
					{
						Vector2 vector2 = userSubMesh.originalUvVertices[j];
						Vector2 zero = Vector2.zero;
						if (vector2.x >= 0f)
						{
							zero.x = vector2.x + ((vector.x < 0f) ? (vector.x * -1f) : 0f);
						}
						if (vector2.y >= 0f)
						{
							zero.y = vector2.y + ((vector.y < 0f) ? (vector.y * -1f) : 0f);
						}
						if (vector2.x < 0f)
						{
							zero.x = vector.x * -1f - vector2.x * -1f;
						}
						if (vector2.y < 0f)
						{
							zero.y = vector.y * -1f - vector2.y * -1f;
						}
						userSubMesh.originalUvVertices[j] = zero;
					}
				}
			}
		}

		private class AtlasData
		{
			public Texture2D mainTextureAtlas = new Texture2D(16, 16);

			public Texture2D metallicMapAtlas = new Texture2D(16, 16);

			public Texture2D specularMapAtlas = new Texture2D(16, 16);

			public Texture2D normalMapAtlas = new Texture2D(16, 16);

			public Texture2D normalMap2Atlas = new Texture2D(16, 16);

			public Texture2D heightMapAtlas = new Texture2D(16, 16);

			public Texture2D occlusionMapAtlas = new Texture2D(16, 16);

			public Texture2D detailMapAtlas = new Texture2D(16, 16);

			public Texture2D detailMaskAtlas = new Texture2D(16, 16);

			public Rect[] atlasRects = new Rect[0];

			public Texture2D[] originalMainTexturesUsedAndOrdenedAccordingToAtlasRect = new Texture2D[0];

			public int GetRectIndexOfThatMainTexture(Texture2D texture)
			{
				int num = -1;
				Texture2D[] array = originalMainTexturesUsedAndOrdenedAccordingToAtlasRect;
				foreach (Texture2D obj in array)
				{
					num++;
					if (obj == texture)
					{
						break;
					}
				}
				return num;
			}
		}

		private enum TextureType
		{
			MainTexture = 0,
			MetallicMap = 1,
			SpecularMap = 2,
			NormalMap = 3,
			HeightMap = 4,
			OcclusionMap = 5,
			DetailMap = 6,
			DetailMask = 7
		}

		private class ColorData
		{
			public string colorName;

			public Color color;

			public ColorData(string colorName, Color color)
			{
				this.colorName = colorName;
				this.color = color;
			}
		}

		private class UvDataAndColorOfThisSubmesh
		{
			public Texture2D textureColor;

			public int startOfUvVerticesIndex;

			public Vector2[] originalUvVertices;
		}

		private class ColorAtlasData
		{
			public Texture2D colorAtlas = new Texture2D(16, 16);

			public Rect[] atlasRects = new Rect[0];

			public Texture2D[] originalTexturesUsedAndOrdenedAccordingToAtlasRect = new Texture2D[0];

			public int GetRectIndexOfThatMainTexture(Texture2D texture)
			{
				int num = -1;
				Texture2D[] array = originalTexturesUsedAndOrdenedAccordingToAtlasRect;
				foreach (Texture2D obj in array)
				{
					num++;
					if (obj == texture)
					{
						break;
					}
				}
				return num;
			}
		}

		private const int MAX_VERTICES_FOR_16BITS_MESH = 50000;

		[HideInInspector]
		public GameObject[] resultMergeOriginalGameObjects;

		[HideInInspector]
		public GameObject resultMergeGameObject;

		[HideInInspector]
		public string resultMergeTextStats = "";

		[HideInInspector]
		public List<LogOfMerge> resultMergeLogs = new List<LogOfMerge>();

		[HideInInspector]
		public List<string> resultMergeAssetsSaved = new List<string>();

		[HideInInspector]
		public MergeMethod mergeMethod;

		[HideInInspector]
		public OneMeshPerMaterialParams oneMeshPerMaterialParams = new OneMeshPerMaterialParams();

		[HideInInspector]
		public JustMaterialColorsParams justMaterialColorsParams = new JustMaterialColorsParams();

		[HideInInspector]
		public AllInOneParams allInOneParams = new AllInOneParams();

		[HideInInspector]
		public OnlyAnima2dMeshes onlyAnima2dMeshes = new OnlyAnima2dMeshes();

		[HideInInspector]
		public BlendShapesSupport blendShapesSupport;

		[HideInInspector]
		public float blendShapesMultiplier = 1f;

		[HideInInspector]
		public RootBoneToUse rootBoneToUse;

		[HideInInspector]
		public Transform manualRootBoneToUse;

		[HideInInspector]
		public bool compatibilityMode = true;

		[HideInInspector]
		public bool combineInactives;

		[HideInInspector]
		public CombineOnStart combineOnStart;

		[HideInInspector]
		public bool convertCombinedMeshToStaticOnStart;

		[HideInInspector]
		public bool legacyAnimationSupport;

		[HideInInspector]
		public string nameOfThisMerge = "Combined Meshes";

		[HideInInspector]
		public List<GameObject> gameObjectsToIgnore = new List<GameObject>();

		[HideInInspector]
		public bool saveDataInAssets = true;

		[HideInInspector]
		public bool savePrefabOfMerge;

		[HideInInspector]
		public string nameOfPrefabOfMerge = "";

		[HideInInspector]
		public bool launchConsoleLogs = true;

		[HideInInspector]
		public bool highlightUvVertices;

		public UnityEvent onCombineMeshs;

		public UnityEvent onUndoCombineMeshs;

		private void Awake()
		{
			ValidateAllVariables();
			if (combineOnStart == CombineOnStart.OnAwake)
			{
				if (Application.isPlaying)
				{
					CombineMeshes();
				}
				if (convertCombinedMeshToStaticOnStart && Application.isPlaying)
				{
					ConvertCombinedMeshToStaticMesh();
				}
			}
		}

		private void Start()
		{
			if (combineOnStart == CombineOnStart.OnStart)
			{
				if (Application.isPlaying)
				{
					CombineMeshes();
				}
				if (convertCombinedMeshToStaticOnStart && Application.isPlaying)
				{
					ConvertCombinedMeshToStaticMesh();
				}
			}
		}

		public bool isMeshesCombined()
		{
			if (resultMergeGameObject != null)
			{
				return true;
			}
			return false;
		}

		public void CombineMeshes()
		{
			switch (mergeMethod)
			{
			case MergeMethod.OneMeshPerMaterial:
				DoCombineMeshs_OneMeshPerMaterial();
				break;
			case MergeMethod.AllInOne:
				DoCombineMeshs_AllInOne();
				break;
			case MergeMethod.JustMaterialColors:
				DoCombineMeshs_JustMaterialColors();
				break;
			case MergeMethod.OnlyAnima2dMeshes:
				DoCombineMeshs_OnlyAnima2dMeshs();
				break;
			}
		}

		public void UndoCombineMeshes(bool runUnityGc, bool runMonoIl2CppGc)
		{
			if (isCombinedMeshesConvertedToStatic())
			{
				Debug.LogError("Before undoing the meshes merge, please undo the conversion to static mesh by calling the \"UndoConvertCombinedMeshToStaticMesh()\" method.");
			}
			else
			{
				DoUndoCombineMeshs(runMonoIl2CppGc, runUnityGc, errorOcurred: false);
			}
		}

		public SkinnedMeshRenderer GetCombinedMeshSkinnedMeshRenderer()
		{
			if (!isMeshesCombined())
			{
				CombineMeshes();
			}
			return resultMergeGameObject.GetComponent<SkinnedMeshRenderer>();
		}

		public bool isCombinedMeshesConvertedToStatic()
		{
			if (resultMergeGameObject == null)
			{
				return false;
			}
			CombinedMeshesManager component = resultMergeGameObject.GetComponent<CombinedMeshesManager>();
			if (component.resultOfConversionToStaticMesh == null)
			{
				return false;
			}
			if (component.resultOfConversionToStaticMesh != null)
			{
				return true;
			}
			return false;
		}

		public GameObject ConvertCombinedMeshToStaticMesh()
		{
			if (resultMergeGameObject == null)
			{
				Debug.LogWarning("Could not convert the combined mesh from \"" + base.gameObject.name + "\" to static. It is necessary that the meshes of this model are merged by the Skinned Mesh Combiner, before converting the combined mesh to a static mesh. Please combine the stitches first.");
				return null;
			}
			SkinnedMeshRenderer combinedMeshSkinnedMeshRenderer = GetCombinedMeshSkinnedMeshRenderer();
			CombinedMeshesManager component = combinedMeshSkinnedMeshRenderer.gameObject.GetComponent<CombinedMeshesManager>();
			if (component.resultOfConversionToStaticMesh != null)
			{
				Debug.LogWarning("The combined mesh has already been converted to a static mesh.");
				return component.resultOfConversionToStaticMesh;
			}
			if (component.mergeMethodUsed == CombinedMeshesManager.MergeMethod.OnlyAnima2dMeshes)
			{
				Debug.LogWarning("It is not possible to convert a mesh to static, mesh that was combined using the \"Only Anima2D Meshes\" method.");
				return null;
			}
			Mesh mesh = new Mesh();
			mesh.name = "Static Mesh";
			mesh.indexFormat = IndexFormat.UInt32;
			combinedMeshSkinnedMeshRenderer.BakeMesh(mesh);
			GameObject gameObject = new GameObject("Static Mesh");
			gameObject.transform.SetParent(combinedMeshSkinnedMeshRenderer.gameObject.transform);
			component.resultOfConversionToStaticMesh = gameObject;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			meshRenderer.sharedMaterials = combinedMeshSkinnedMeshRenderer.sharedMaterials;
			combinedMeshSkinnedMeshRenderer.enabled = false;
			return gameObject;
		}

		public void UndoConvertCombinedMeshToStaticMesh()
		{
			if (resultMergeGameObject == null)
			{
				Debug.LogWarning("It is not possible to undo the conversion to static mesh, because the meshes of the \"" + base.gameObject.name + "\" model have not yet been combined.");
				return;
			}
			SkinnedMeshRenderer combinedMeshSkinnedMeshRenderer = GetCombinedMeshSkinnedMeshRenderer();
			CombinedMeshesManager component = combinedMeshSkinnedMeshRenderer.gameObject.GetComponent<CombinedMeshesManager>();
			if (component.resultOfConversionToStaticMesh == null)
			{
				Debug.LogWarning("It is not possible to undo the conversion to static mesh, as no conversion to static mesh has been done yet.");
				return;
			}
			UnityEngine.Object.Destroy(component.resultOfConversionToStaticMesh);
			combinedMeshSkinnedMeshRenderer.enabled = true;
		}

		public GameObject GetCombinedMeshConvertedToStatic()
		{
			return resultMergeGameObject.GetComponent<CombinedMeshesManager>().resultOfConversionToStaticMesh;
		}

		private void DoCombineMeshs_OneMeshPerMaterial()
		{
			if (resultMergeGameObject != null)
			{
				LaunchLog("Currently, this character's meshes are already merged. Please, before making a new merge, undo the merge previously done.", LogTypeOf.Warning);
				return;
			}
			try
			{
				ShowProgressBar("Merging...", show: true, 1f);
				resultMergeLogs.Clear();
				ValidateAllVariables();
				Animator animator = null;
				AnimatorCullingMode cullingMode = AnimatorCullingMode.AlwaysAnimate;
				if (!legacyAnimationSupport)
				{
					animator = GetComponent<Animator>();
					cullingMode = animator.cullingMode;
					animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				Animation animation = null;
				AnimationCullingType cullingType = AnimationCullingType.AlwaysAnimate;
				if (legacyAnimationSupport)
				{
					animation = GetComponent<Animation>();
					cullingType = animation.cullingType;
					animation.cullingType = AnimationCullingType.AlwaysAnimate;
				}
				DateTime now = DateTime.Now;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int originalUvLenght = 0;
				GameObject[] allItemsForCombine = GetAllItemsForCombine(includeItemsRegisteredToBeIgnored: false, launchLogs: true);
				SkinnedMeshRenderer[] allSkinnedMeshsValidatedToCombine = GetAllSkinnedMeshsValidatedToCombine(allItemsForCombine);
				if (allSkinnedMeshsValidatedToCombine == null || allSkinnedMeshsValidatedToCombine.Length < 1)
				{
					LaunchLog("The merge has been canceled. There may not be enough meshes to be combined. At least 1 valid mesh is required for the merge process to be possible. Also, there is the possibility that all the meshes found, are invalid or have been ignored during the merge process.", LogTypeOf.Warning);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (ExistsDifferentRootBones(allSkinnedMeshsValidatedToCombine, launchLogs: true) && oneMeshPerMaterialParams.mergeOnlyEqualRootBones)
				{
					StopMergeByErrorWhileMerging(null);
					return;
				}
				num = CountVerticesInAllMeshes(allSkinnedMeshsValidatedToCombine);
				Dictionary<Material, List<SubMeshToCombine>> dictionary = new Dictionary<Material, List<SubMeshToCombine>>();
				SkinnedMeshRenderer[] array = allSkinnedMeshsValidatedToCombine;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					for (int j = 0; j < skinnedMeshRenderer.sharedMesh.subMeshCount; j++)
					{
						if (dictionary.ContainsKey(skinnedMeshRenderer.sharedMaterials[j]))
						{
							dictionary[skinnedMeshRenderer.sharedMaterials[j]].Add(new SubMeshToCombine(skinnedMeshRenderer.transform, skinnedMeshRenderer, j));
						}
						if (!dictionary.ContainsKey(skinnedMeshRenderer.sharedMaterials[j]))
						{
							dictionary.Add(skinnedMeshRenderer.sharedMaterials[j], new List<SubMeshToCombine>
							{
								new SubMeshToCombine(skinnedMeshRenderer.transform, skinnedMeshRenderer, j)
							});
						}
						num5++;
					}
				}
				List<SubMeshesCombined> list = new List<SubMeshesCombined>();
				GameObject gameObject = new GameObject("Cache Obj");
				SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject.AddComponent<SkinnedMeshRenderer>();
				foreach (KeyValuePair<Material, List<SubMeshToCombine>> item3 in dictionary)
				{
					List<SubMeshToCombine> value = item3.Value;
					num2 += value.Count;
					num3 += value.Count - 1;
					num4++;
					List<Transform> list2 = new List<Transform>();
					List<Matrix4x4> list3 = new List<Matrix4x4>();
					List<CombineInstance> list4 = new List<CombineInstance>();
					foreach (SubMeshToCombine item4 in value)
					{
						Transform[] bones = item4.skinnedMeshRenderer.bones;
						for (int k = 0; k < bones.Length; k++)
						{
							if (k < item4.skinnedMeshRenderer.sharedMesh.bindposes.Length)
							{
								list2.Add(bones[k]);
								if (compatibilityMode)
								{
									list3.Add(item4.skinnedMeshRenderer.sharedMesh.bindposes[k] * item4.skinnedMeshRenderer.transform.worldToLocalMatrix);
								}
								if (!compatibilityMode)
								{
									list3.Add(bones[k].worldToLocalMatrix * item4.skinnedMeshRenderer.transform.worldToLocalMatrix);
								}
							}
						}
						list4.Add(new CombineInstance
						{
							mesh = item4.skinnedMeshRenderer.sharedMesh,
							subMeshIndex = item4.subMeshIndex,
							transform = item4.transform.localToWorldMatrix
						});
					}
					Mesh mesh = new Mesh();
					if (num <= 50000)
					{
						mesh.indexFormat = IndexFormat.UInt16;
					}
					if (num > 50000)
					{
						mesh.indexFormat = IndexFormat.UInt32;
					}
					mesh.CombineMeshes(list4.ToArray(), mergeSubMeshes: true, useMatrices: true);
					skinnedMeshRenderer2.sharedMesh = mesh;
					skinnedMeshRenderer2.sharedMesh.bindposes = list3.ToArray();
					skinnedMeshRenderer2.bones = list2.ToArray();
					skinnedMeshRenderer2.sharedMaterial = item3.Key;
					list.Add(new SubMeshesCombined(skinnedMeshRenderer2.transform.worldToLocalMatrix, skinnedMeshRenderer2.sharedMesh, skinnedMeshRenderer2.bones, skinnedMeshRenderer2.sharedMesh.bindposes, skinnedMeshRenderer2.sharedMaterial));
				}
				DestroyGameObject(gameObject);
				List<Transform> list5 = new List<Transform>();
				List<Matrix4x4> list6 = new List<Matrix4x4>();
				List<CombineInstance> list7 = new List<CombineInstance>();
				List<Material> list8 = new List<Material>();
				foreach (SubMeshesCombined item5 in list)
				{
					list7.Add(new CombineInstance
					{
						mesh = item5.subMeshesMerged,
						subMeshIndex = 0,
						transform = item5.localToWorldMatrix
					});
					Transform[] bonesToMerge = item5.bonesToMerge;
					foreach (Transform item in bonesToMerge)
					{
						list5.Add(item);
					}
					Matrix4x4[] bindPosesToMerge = item5.bindPosesToMerge;
					foreach (Matrix4x4 item2 in bindPosesToMerge)
					{
						list6.Add(item2);
					}
					list8.Add(item5.thisMaterial);
				}
				Mesh mesh2 = new Mesh();
				if (num <= 50000)
				{
					mesh2.indexFormat = IndexFormat.UInt16;
				}
				if (num > 50000)
				{
					mesh2.indexFormat = IndexFormat.UInt32;
				}
				mesh2.name = "Combined Meshes (One Mesh Per Material)";
				mesh2.CombineMeshes(list7.ToArray(), mergeSubMeshes: false);
				mesh2.RecalculateBounds();
				resultMergeGameObject = new GameObject(nameOfThisMerge);
				resultMergeGameObject.transform.SetParent(base.gameObject.transform);
				SkinnedMeshRenderer skinnedMeshRenderer3 = resultMergeGameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer3.sharedMesh = mesh2;
				skinnedMeshRenderer3.bones = list5.ToArray();
				skinnedMeshRenderer3.sharedMesh.bindposes = list6.ToArray();
				skinnedMeshRenderer3.sharedMaterials = list8.ToArray();
				skinnedMeshRenderer3.rootBone = GetCorrectRootBoneFromAllOriginalSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine);
				if (blendShapesSupport == BlendShapesSupport.FullSupport)
				{
					MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine, mesh2, skinnedMeshRenderer3);
				}
				CombinedMeshesManager combinedMeshesManager = resultMergeGameObject.AddComponent<CombinedMeshesManager>();
				combinedMeshesManager.rootGameObject = base.gameObject;
				combinedMeshesManager.mergeMethodUsed = CombinedMeshesManager.MergeMethod.OneMeshPerMaterial;
				combinedMeshesManager.sourceMeshes = allSkinnedMeshsValidatedToCombine;
				resultMergeOriginalGameObjects = allItemsForCombine;
				array = allSkinnedMeshsValidatedToCombine;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				if (!legacyAnimationSupport)
				{
					animator.cullingMode = cullingMode;
				}
				if (legacyAnimationSupport)
				{
					animation.cullingType = cullingType;
				}
				DateTime now2 = DateTime.Now;
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Meshes", skinnedMeshRenderer3.sharedMesh, base.gameObject.name, "asset");
				}
				if (savePrefabOfMerge)
				{
					SaveMergeAsPrefab("Prefabs", nameOfPrefabOfMerge, base.gameObject);
				}
				GenerateStatsTextAndFillIt(nameOfThisMerge, "One Mesh Per Material", num, now, now2, num2, num4, num3, num5, originalUvLenght, 0);
				ShowProgressBar("", show: false, 1f);
				if (!Application.isPlaying)
				{
					Debug.Log("The merging of the meshes was completed successfully!");
				}
				LaunchLog("The merging of the meshes was completed successfully! The name of this merge is \"" + nameOfThisMerge + "\"!", LogTypeOf.Log);
				if (Application.isPlaying && onCombineMeshs != null)
				{
					onCombineMeshs.Invoke();
				}
				ShowLogsOfMergeTabAndClearUndoHistory();
			}
			catch (Exception exception)
			{
				StopMergeByErrorWhileMerging(exception);
			}
		}

		private void DoCombineMeshs_JustMaterialColors()
		{
			if (resultMergeGameObject != null)
			{
				LaunchLog("Currently, this character's meshes are already merged. Please, before making a new merge, undo the merge previously done.", LogTypeOf.Warning);
				return;
			}
			try
			{
				ShowProgressBar("Merging...", show: true, 1f);
				resultMergeLogs.Clear();
				ValidateAllVariables();
				Animator animator = null;
				AnimatorCullingMode cullingMode = AnimatorCullingMode.AlwaysAnimate;
				if (!legacyAnimationSupport)
				{
					animator = GetComponent<Animator>();
					cullingMode = animator.cullingMode;
					animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				Animation animation = null;
				AnimationCullingType cullingType = AnimationCullingType.AlwaysAnimate;
				if (legacyAnimationSupport)
				{
					animation = GetComponent<Animation>();
					cullingType = animation.cullingType;
					animation.cullingType = AnimationCullingType.AlwaysAnimate;
				}
				DateTime now = DateTime.Now;
				int num = 0;
				int num2 = 0;
				int drawCallReduction = 0;
				int meshCountAfter = 0;
				int num3 = 0;
				int num4 = 0;
				GameObject[] allItemsForCombine = GetAllItemsForCombine(includeItemsRegisteredToBeIgnored: false, launchLogs: true);
				SkinnedMeshRenderer[] allSkinnedMeshsValidatedToCombine = GetAllSkinnedMeshsValidatedToCombine(allItemsForCombine);
				if (justMaterialColorsParams.materialToUse == null)
				{
					LaunchLog("You must provide a material to be used by the mesh resulting from the merge. This material will be a copy of the material you provide, and it will hold the merge atlas for example. Please provide a material for use, so that merge is possible.", LogTypeOf.Error);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (allSkinnedMeshsValidatedToCombine == null || allSkinnedMeshsValidatedToCombine.Length < 1)
				{
					LaunchLog("The merge has been canceled. There may not be enough meshes to be combined. At least 1 valid mesh is required for the merge process to be possible. Also, there is the possibility that all the meshes found, are invalid or have been ignored during the merge process.", LogTypeOf.Warning);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (ExistsDifferentRootBones(allSkinnedMeshsValidatedToCombine, launchLogs: true) && justMaterialColorsParams.mergeOnlyEqualsRootBones)
				{
					StopMergeByErrorWhileMerging(null);
					return;
				}
				num = CountVerticesInAllMeshes(allSkinnedMeshsValidatedToCombine);
				List<CombineInstance> list = new List<CombineInstance>();
				List<Transform> list2 = new List<Transform>();
				List<Matrix4x4> list3 = new List<Matrix4x4>();
				List<UvDataAndColorOfThisSubmesh> list4 = new List<UvDataAndColorOfThisSubmesh>();
				int num5 = 0;
				SkinnedMeshRenderer[] array = allSkinnedMeshsValidatedToCombine;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					for (int j = 0; j < skinnedMeshRenderer.sharedMesh.subMeshCount; j++)
					{
						ShowProgressBar("Reading Mesh...", show: true, 1f);
						Transform[] bones = skinnedMeshRenderer.bones;
						for (int k = 0; k < bones.Length; k++)
						{
							list2.Add(bones[k]);
							if (compatibilityMode)
							{
								list3.Add(skinnedMeshRenderer.sharedMesh.bindposes[k] * skinnedMeshRenderer.transform.worldToLocalMatrix);
							}
							if (!compatibilityMode)
							{
								list3.Add(bones[k].worldToLocalMatrix * skinnedMeshRenderer.transform.worldToLocalMatrix);
							}
						}
						CombineInstance item = new CombineInstance
						{
							mesh = skinnedMeshRenderer.sharedMesh,
							subMeshIndex = j,
							transform = skinnedMeshRenderer.transform.localToWorldMatrix
						};
						list.Add(item);
						int vertexCount = item.mesh.GetSubMesh(item.subMeshIndex).vertexCount;
						UvDataAndColorOfThisSubmesh uvDataAndColorOfThisSubmesh = new UvDataAndColorOfThisSubmesh();
						uvDataAndColorOfThisSubmesh.startOfUvVerticesIndex = num5;
						uvDataAndColorOfThisSubmesh.originalUvVertices = new Vector2[vertexCount];
						uvDataAndColorOfThisSubmesh.textureColor = GetTextureFilledWithColorOfMaterial(skinnedMeshRenderer.sharedMaterials[j], justMaterialColorsParams.colorPropertyToFind, 64, 64);
						list4.Add(uvDataAndColorOfThisSubmesh);
						num2++;
						meshCountAfter = 1;
						drawCallReduction = list4.Count - 1;
						num3++;
						num4 += vertexCount;
						num5 += vertexCount;
					}
				}
				ShowProgressBar("Merging...", show: true, 1f);
				Mesh mesh = new Mesh();
				if (num <= 50000)
				{
					mesh.indexFormat = IndexFormat.UInt16;
				}
				if (num > 50000)
				{
					mesh.indexFormat = IndexFormat.UInt32;
				}
				mesh.name = "Combined Meshes (Just Material Colors)";
				mesh.CombineMeshes(list.ToArray(), mergeSubMeshes: true, useMatrices: true);
				mesh.RecalculateBounds();
				resultMergeGameObject = new GameObject(nameOfThisMerge);
				resultMergeGameObject.transform.SetParent(base.gameObject.transform);
				SkinnedMeshRenderer skinnedMeshRenderer2 = resultMergeGameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer2.sharedMesh = mesh;
				skinnedMeshRenderer2.bones = list2.ToArray();
				skinnedMeshRenderer2.sharedMesh.bindposes = list3.ToArray();
				skinnedMeshRenderer2.rootBone = GetCorrectRootBoneFromAllOriginalSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine);
				skinnedMeshRenderer2.sharedMaterials = new Material[1] { GetValidatedCopyOfMaterial(justMaterialColorsParams.materialToUse, copyPropertiesOfTargetMaterial: true, clearAllTextures: true) };
				skinnedMeshRenderer2.sharedMaterials[0].name = "Combined Materials (Just Material Colors)";
				if (blendShapesSupport == BlendShapesSupport.FullSupport)
				{
					MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine, mesh, skinnedMeshRenderer2);
				}
				ColorAtlasData colorAtlasData = CreateColorAtlas(list4.ToArray(), 256, 0, showProgress: true);
				ShowProgressBar("Creating New UV Map...", show: true, 1f);
				if (skinnedMeshRenderer2.sharedMesh.uv.Length == 0)
				{
					LaunchLog("It was not possible to create a UV map for the combined mesh. Originally, this character's meshes do not have a UV mapping. Create a UV mapping for this character or try using a blending method that does not work with UV mapping, such as One Mesh Per Material.", LogTypeOf.Error);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (skinnedMeshRenderer2.sharedMesh.uv.Length != skinnedMeshRenderer2.sharedMesh.vertexCount)
				{
					LaunchLog("It was not possible to create a new UV map for the combianda mesh. The amount of vertices in the original UV map is different from the amount of vertices that the original meshes have, that is, not all vertices of this character are in the UV map. Make sure that all the vertices of this character are on the UV map or try using a blending method that does not work with UV mapping, such as One Mesh Per Material.", LogTypeOf.Error);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				Vector2[] array2 = new Vector2[skinnedMeshRenderer2.sharedMesh.uv.Length];
				foreach (UvDataAndColorOfThisSubmesh item2 in list4)
				{
					for (int l = 0; l < item2.originalUvVertices.Length; l++)
					{
						if (item2.originalUvVertices[l].x < 0f)
						{
							item2.originalUvVertices[l].x = item2.originalUvVertices[l].x * -1f;
						}
						if (item2.originalUvVertices[l].y < 0f)
						{
							item2.originalUvVertices[l].y = item2.originalUvVertices[l].y * -1f;
						}
					}
					Vector2 vector = Vector2.zero;
					for (int m = 0; m < item2.originalUvVertices.Length; m++)
					{
						vector = new Vector2(Mathf.Max(item2.originalUvVertices[m].x, vector.x), Mathf.Max(item2.originalUvVertices[m].y, vector.y));
					}
					Vector2 vector2 = new Vector2(0.8f, 0.8f);
					int rectIndexOfThatMainTexture = colorAtlasData.GetRectIndexOfThatMainTexture(item2.textureColor);
					for (int n = 0; n < item2.originalUvVertices.Length; n++)
					{
						Vector2 zero = Vector2.zero;
						if (vector.x <= 1f)
						{
							zero.x = Mathf.Lerp(colorAtlasData.atlasRects[rectIndexOfThatMainTexture].xMin, colorAtlasData.atlasRects[rectIndexOfThatMainTexture].xMax, Mathf.Lerp(vector2.x, 1f - vector2.x, item2.originalUvVertices[n].x));
						}
						if (vector.y <= 1f)
						{
							zero.y = Mathf.Lerp(colorAtlasData.atlasRects[rectIndexOfThatMainTexture].yMin, colorAtlasData.atlasRects[rectIndexOfThatMainTexture].yMax, Mathf.Lerp(vector2.y, 1f - vector2.y, item2.originalUvVertices[n].y));
						}
						if (vector.x > 1f)
						{
							zero.x = Mathf.Lerp(colorAtlasData.atlasRects[rectIndexOfThatMainTexture].xMin, colorAtlasData.atlasRects[rectIndexOfThatMainTexture].xMax, Mathf.Lerp(vector2.x, 1f - vector2.x, item2.originalUvVertices[n].x / vector.x));
						}
						if (vector.y > 1f)
						{
							zero.y = Mathf.Lerp(colorAtlasData.atlasRects[rectIndexOfThatMainTexture].yMin, colorAtlasData.atlasRects[rectIndexOfThatMainTexture].yMax, Mathf.Lerp(vector2.y, 1f - vector2.y, item2.originalUvVertices[n].y / vector.y));
						}
						array2[n + item2.startOfUvVerticesIndex] = zero;
					}
				}
				ShowProgressBar("Finishing...", show: true, 1f);
				skinnedMeshRenderer2.sharedMesh.uv = array2;
				ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer2.sharedMaterials[0], justMaterialColorsParams.mainTexturePropertyToInsert, colorAtlasData.colorAtlas);
				if (highlightUvVertices)
				{
					for (int num6 = 0; num6 < skinnedMeshRenderer2.sharedMesh.uv.Length; num6++)
					{
						colorAtlasData.colorAtlas.SetPixel((int)((float)colorAtlasData.colorAtlas.width * skinnedMeshRenderer2.sharedMesh.uv[num6].x), (int)((float)colorAtlasData.colorAtlas.height * skinnedMeshRenderer2.sharedMesh.uv[num6].y), Color.yellow);
					}
					colorAtlasData.colorAtlas.Apply();
				}
				CombinedMeshesManager combinedMeshesManager = resultMergeGameObject.AddComponent<CombinedMeshesManager>();
				combinedMeshesManager.rootGameObject = base.gameObject;
				combinedMeshesManager.mergeMethodUsed = CombinedMeshesManager.MergeMethod.JustMaterialColors;
				combinedMeshesManager.sourceMeshes = allSkinnedMeshsValidatedToCombine;
				resultMergeOriginalGameObjects = allItemsForCombine;
				array = allSkinnedMeshsValidatedToCombine;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				if (!legacyAnimationSupport)
				{
					animator.cullingMode = cullingMode;
				}
				if (legacyAnimationSupport)
				{
					animation.cullingType = cullingType;
				}
				DateTime now2 = DateTime.Now;
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Meshes", skinnedMeshRenderer2.sharedMesh, base.gameObject.name, "asset");
				}
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Atlases", colorAtlasData.colorAtlas, base.gameObject.name + " (ColorAtlas)", "asset");
				}
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Materials", skinnedMeshRenderer2.sharedMaterials[0], base.gameObject.name, "mat");
				}
				if (savePrefabOfMerge)
				{
					SaveMergeAsPrefab("Prefabs", nameOfPrefabOfMerge, base.gameObject);
				}
				GenerateStatsTextAndFillIt(nameOfThisMerge, "Just Material Colors", num, now, now2, num2, meshCountAfter, drawCallReduction, num3, num4, array2.Length);
				ShowProgressBar("", show: false, 1f);
				if (!Application.isPlaying)
				{
					Debug.Log("The merging of the meshes was completed successfully!");
				}
				LaunchLog("The merging of the meshes was completed successfully! The name of this merge is \"" + nameOfThisMerge + "\"!", LogTypeOf.Log);
				if (Application.isPlaying && onCombineMeshs != null)
				{
					onCombineMeshs.Invoke();
				}
				ShowLogsOfMergeTabAndClearUndoHistory();
			}
			catch (Exception exception)
			{
				StopMergeByErrorWhileMerging(exception);
			}
		}

		private void DoCombineMeshs_AllInOne()
		{
			if (resultMergeGameObject != null)
			{
				LaunchLog("Currently, this character's meshes are already merged. Please, before making a new merge, undo the merge previously done.", LogTypeOf.Warning);
				return;
			}
			try
			{
				ShowProgressBar("Merging...", show: true, 1f);
				resultMergeLogs.Clear();
				ValidateAllVariables();
				Animator animator = null;
				AnimatorCullingMode cullingMode = AnimatorCullingMode.AlwaysAnimate;
				if (!legacyAnimationSupport)
				{
					animator = GetComponent<Animator>();
					cullingMode = animator.cullingMode;
					animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				Animation animation = null;
				AnimationCullingType cullingType = AnimationCullingType.AlwaysAnimate;
				if (legacyAnimationSupport)
				{
					animation = GetComponent<Animation>();
					cullingType = animation.cullingType;
					animation.cullingType = AnimationCullingType.AlwaysAnimate;
				}
				DateTime now = DateTime.Now;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int meshCountAfter = 0;
				int materialCount = 0;
				int num4 = 0;
				GameObject[] allItemsForCombine = GetAllItemsForCombine(includeItemsRegisteredToBeIgnored: false, launchLogs: true);
				SkinnedMeshRenderer[] allSkinnedMeshsValidatedToCombine = GetAllSkinnedMeshsValidatedToCombine(allItemsForCombine);
				if (allInOneParams.materialToUse == null)
				{
					LaunchLog("You must provide a material to be used by the mesh resulting from the merge. This material will be a copy of the material you provide, and it will hold the merge atlas for example. Please provide a material for use, so that merge is possible.", LogTypeOf.Error);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (allSkinnedMeshsValidatedToCombine == null || allSkinnedMeshsValidatedToCombine.Length < 1)
				{
					LaunchLog("The merge has been canceled. There may not be enough meshes to be combined. At least 1 valid mesh is required for the merge process to be possible. Also, there is the possibility that all the meshes found, are invalid or have been ignored during the merge process.", LogTypeOf.Warning);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				if (ExistsDifferentRootBones(allSkinnedMeshsValidatedToCombine, launchLogs: true) && allInOneParams.mergeOnlyEqualsRootBones)
				{
					StopMergeByErrorWhileMerging(null);
					return;
				}
				num = CountVerticesInAllMeshes(allSkinnedMeshsValidatedToCombine);
				List<CombineInstance> list = new List<CombineInstance>();
				List<Transform> list2 = new List<Transform>();
				List<Matrix4x4> list3 = new List<Matrix4x4>();
				List<TexturesSubMeshes> list4 = new List<TexturesSubMeshes>();
				int num5 = 0;
				SkinnedMeshRenderer[] array = allSkinnedMeshsValidatedToCombine;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					num5 += skinnedMeshRenderer.sharedMesh.subMeshCount;
				}
				int num6 = 0;
				int num7 = 0;
				array = allSkinnedMeshsValidatedToCombine;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in array)
				{
					for (int j = 0; j < skinnedMeshRenderer2.sharedMesh.subMeshCount; j++)
					{
						float progress = (float)num6 / ((float)num5 + 1f);
						ShowProgressBar("Reading Mesh...", show: true, progress);
						Transform[] bones = skinnedMeshRenderer2.bones;
						for (int k = 0; k < bones.Length; k++)
						{
							list2.Add(bones[k]);
							if (compatibilityMode)
							{
								list3.Add(skinnedMeshRenderer2.sharedMesh.bindposes[k] * skinnedMeshRenderer2.transform.worldToLocalMatrix);
							}
							if (!compatibilityMode)
							{
								list3.Add(bones[k].worldToLocalMatrix * skinnedMeshRenderer2.transform.worldToLocalMatrix);
							}
						}
						CombineInstance item = new CombineInstance
						{
							mesh = skinnedMeshRenderer2.sharedMesh,
							subMeshIndex = j,
							transform = skinnedMeshRenderer2.transform.localToWorldMatrix
						};
						list.Add(item);
						Vector2[] uv = item.mesh.SMCGetSubmesh(j).uv;
						TexturesSubMeshes.UvBounds boundValuesOfSubMeshUv = GetBoundValuesOfSubMeshUv(uv);
						if (allInOneParams.mergeTiledTextures == MergeTiledTextures.LegacyMode)
						{
							boundValuesOfSubMeshUv.majorX = 1f;
							boundValuesOfSubMeshUv.majorY = 1f;
							boundValuesOfSubMeshUv.minorX = 0f;
							boundValuesOfSubMeshUv.minorY = 0f;
						}
						boundValuesOfSubMeshUv.RoundBoundsValuesAndCalculateSpaceNeededToTiling();
						if (isTiledTexture(boundValuesOfSubMeshUv))
						{
							TexturesSubMeshes texturesSubMeshes = new TexturesSubMeshes();
							Texture2D texture2D = (Texture2D)skinnedMeshRenderer2.sharedMaterials[j].GetTexture(allInOneParams.mainTexturePropertyToFind);
							Vector2Int mainTextureResolution = Vector2Int.zero;
							Vector2Int zero = Vector2Int.zero;
							if (texture2D == null)
							{
								mainTextureResolution = new Vector2Int(64, 64);
							}
							if (texture2D != null)
							{
								mainTextureResolution = new Vector2Int(texture2D.width, texture2D.height);
							}
							zero = new Vector2Int(mainTextureResolution.x + GetEdgesSizeForTextures() * 2, mainTextureResolution.y + GetEdgesSizeForTextures() * 2);
							texturesSubMeshes.material = skinnedMeshRenderer2.sharedMaterials[j];
							texturesSubMeshes.isTiledTexture = true;
							texturesSubMeshes.mainTextureResolution = mainTextureResolution;
							texturesSubMeshes.mainTextureResolutionWithEdges = zero;
							texturesSubMeshes.mainTexture = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.mainTexturePropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.MainTexture, showProgress: true, progress);
							if (allInOneParams.metallicMapSupport)
							{
								texturesSubMeshes.metallicMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.metallicMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.MetallicMap, showProgress: true, progress);
							}
							if (allInOneParams.specularMapSupport)
							{
								texturesSubMeshes.specularMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.specularMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.SpecularMap, showProgress: true, progress);
							}
							if (allInOneParams.normalMapSupport)
							{
								texturesSubMeshes.normalMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.normalMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.NormalMap, showProgress: true, progress);
							}
							if (allInOneParams.normalMap2Support)
							{
								texturesSubMeshes.normalMap2 = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.normalMap2PropertyFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.NormalMap, showProgress: true, progress);
							}
							if (allInOneParams.heightMapSupport)
							{
								texturesSubMeshes.heightMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.heightMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.HeightMap, showProgress: true, progress);
							}
							if (allInOneParams.occlusionMapSupport)
							{
								texturesSubMeshes.occlusionMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.occlusionMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.OcclusionMap, showProgress: true, progress);
							}
							if (allInOneParams.detailAlbedoMapSupport)
							{
								texturesSubMeshes.detailMap = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.detailMapPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.DetailMap, showProgress: true, progress);
							}
							if (allInOneParams.detailMaskSupport)
							{
								texturesSubMeshes.detailMask = GetValidatedCopyOfTexture(texturesSubMeshes.material, allInOneParams.detailMaskPropertyToFind, texturesSubMeshes.mainTextureResolutionWithEdges.x, texturesSubMeshes.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.DetailMask, showProgress: true, progress);
							}
							TexturesSubMeshes.UserSubMeshes userSubMeshes = new TexturesSubMeshes.UserSubMeshes();
							userSubMeshes.uvBoundsOfThisSubMesh = boundValuesOfSubMeshUv;
							userSubMeshes.startOfUvVerticesInIndex = num7;
							userSubMeshes.originalUvVertices = new Vector2[uv.Length];
							for (int l = 0; l < userSubMeshes.originalUvVertices.Length; l++)
							{
								userSubMeshes.originalUvVertices[l] = uv[l];
							}
							texturesSubMeshes.userSubMeshes.Add(userSubMeshes);
							list4.Add(texturesSubMeshes);
						}
						if (!isTiledTexture(boundValuesOfSubMeshUv))
						{
							TexturesSubMeshes theTextureSubMeshesOfMaterial = GetTheTextureSubMeshesOfMaterial(skinnedMeshRenderer2.sharedMaterials[j], list4);
							if (theTextureSubMeshesOfMaterial == null)
							{
								TexturesSubMeshes texturesSubMeshes2 = new TexturesSubMeshes();
								Texture2D texture2D2 = (Texture2D)skinnedMeshRenderer2.sharedMaterials[j].GetTexture(allInOneParams.mainTexturePropertyToFind);
								Vector2Int mainTextureResolution2 = Vector2Int.zero;
								Vector2Int zero2 = Vector2Int.zero;
								if (texture2D2 == null)
								{
									mainTextureResolution2 = new Vector2Int(64, 64);
								}
								if (texture2D2 != null)
								{
									mainTextureResolution2 = new Vector2Int(texture2D2.width, texture2D2.height);
								}
								zero2 = new Vector2Int(mainTextureResolution2.x + GetEdgesSizeForTextures() * 2, mainTextureResolution2.y + GetEdgesSizeForTextures() * 2);
								texturesSubMeshes2.material = skinnedMeshRenderer2.sharedMaterials[j];
								texturesSubMeshes2.isTiledTexture = false;
								texturesSubMeshes2.mainTextureResolution = mainTextureResolution2;
								texturesSubMeshes2.mainTextureResolutionWithEdges = zero2;
								texturesSubMeshes2.mainTexture = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.mainTexturePropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.MainTexture, showProgress: true, progress);
								if (allInOneParams.metallicMapSupport)
								{
									texturesSubMeshes2.metallicMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.metallicMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.MetallicMap, showProgress: true, progress);
								}
								if (allInOneParams.specularMapSupport)
								{
									texturesSubMeshes2.specularMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.specularMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.SpecularMap, showProgress: true, progress);
								}
								if (allInOneParams.normalMapSupport)
								{
									texturesSubMeshes2.normalMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.normalMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.NormalMap, showProgress: true, progress);
								}
								if (allInOneParams.normalMap2Support)
								{
									texturesSubMeshes2.normalMap2 = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.normalMap2PropertyFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.NormalMap, showProgress: true, progress);
								}
								if (allInOneParams.heightMapSupport)
								{
									texturesSubMeshes2.heightMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.heightMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.HeightMap, showProgress: true, progress);
								}
								if (allInOneParams.occlusionMapSupport)
								{
									texturesSubMeshes2.occlusionMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.occlusionMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.OcclusionMap, showProgress: true, progress);
								}
								if (allInOneParams.detailAlbedoMapSupport)
								{
									texturesSubMeshes2.detailMap = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.detailMapPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.DetailMap, showProgress: true, progress);
								}
								if (allInOneParams.detailMaskSupport)
								{
									texturesSubMeshes2.detailMask = GetValidatedCopyOfTexture(texturesSubMeshes2.material, allInOneParams.detailMaskPropertyToFind, texturesSubMeshes2.mainTextureResolutionWithEdges.x, texturesSubMeshes2.mainTextureResolutionWithEdges.y, boundValuesOfSubMeshUv, TextureType.DetailMask, showProgress: true, progress);
								}
								TexturesSubMeshes.UserSubMeshes userSubMeshes2 = new TexturesSubMeshes.UserSubMeshes();
								userSubMeshes2.uvBoundsOfThisSubMesh = boundValuesOfSubMeshUv;
								userSubMeshes2.startOfUvVerticesInIndex = num7;
								userSubMeshes2.originalUvVertices = new Vector2[uv.Length];
								for (int m = 0; m < userSubMeshes2.originalUvVertices.Length; m++)
								{
									userSubMeshes2.originalUvVertices[m] = uv[m];
								}
								texturesSubMeshes2.userSubMeshes.Add(userSubMeshes2);
								list4.Add(texturesSubMeshes2);
							}
							if (theTextureSubMeshesOfMaterial != null)
							{
								TexturesSubMeshes.UserSubMeshes userSubMeshes3 = new TexturesSubMeshes.UserSubMeshes();
								userSubMeshes3.uvBoundsOfThisSubMesh = boundValuesOfSubMeshUv;
								userSubMeshes3.startOfUvVerticesInIndex = num7;
								userSubMeshes3.originalUvVertices = new Vector2[uv.Length];
								for (int n = 0; n < userSubMeshes3.originalUvVertices.Length; n++)
								{
									userSubMeshes3.originalUvVertices[n] = uv[n];
								}
								theTextureSubMeshesOfMaterial.userSubMeshes.Add(userSubMeshes3);
							}
						}
						num2++;
						meshCountAfter = 1;
						num3++;
						materialCount = list4.Count;
						num4 += uv.Length;
						num7 += uv.Length;
						num6++;
					}
				}
				ShowProgressBar("Merging...", show: true, 1f);
				Mesh mesh = new Mesh();
				if (num <= 50000)
				{
					mesh.indexFormat = IndexFormat.UInt16;
				}
				if (num > 50000)
				{
					mesh.indexFormat = IndexFormat.UInt32;
				}
				mesh.name = "Combined Meshes (All In One)";
				mesh.CombineMeshes(list.ToArray(), mergeSubMeshes: true, useMatrices: true);
				mesh.RecalculateBounds();
				resultMergeGameObject = new GameObject(nameOfThisMerge);
				resultMergeGameObject.transform.SetParent(base.gameObject.transform);
				SkinnedMeshRenderer skinnedMeshRenderer3 = resultMergeGameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer3.sharedMesh = mesh;
				skinnedMeshRenderer3.bones = list2.ToArray();
				skinnedMeshRenderer3.sharedMesh.bindposes = list3.ToArray();
				skinnedMeshRenderer3.rootBone = GetCorrectRootBoneFromAllOriginalSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine);
				skinnedMeshRenderer3.sharedMaterials = new Material[1] { GetValidatedCopyOfMaterial(allInOneParams.materialToUse, copyPropertiesOfTargetMaterial: true, clearAllTextures: true) };
				skinnedMeshRenderer3.sharedMaterials[0].name = "Combined Materials (All In One)";
				if (blendShapesSupport == BlendShapesSupport.FullSupport)
				{
					MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(allSkinnedMeshsValidatedToCombine, mesh, skinnedMeshRenderer3);
				}
				AtlasData atlasData = CreateAllAtlas(list4, GetAtlasMaxResolution(), GetAtlasPadding(), showProgress: true);
				ShowProgressBar("Creating New UV Map...", show: true, 1f);
				if (skinnedMeshRenderer3.sharedMesh.uv.Length == 0)
				{
					LaunchLog("It was not possible to create a UV map for the combined mesh. Originally, this character's meshes do not have a UV mapping. Create a UV mapping for this character or try using a blending method that does not work with UV mapping, such as One Mesh Per Material.", LogTypeOf.Error);
					StopMergeByErrorWhileMerging(null);
					return;
				}
				Vector2[] array2 = new Vector2[skinnedMeshRenderer3.sharedMesh.uv.Length];
				foreach (TexturesSubMeshes item2 in list4)
				{
					if (item2.isTiledTexture)
					{
						item2.ConvertAllSubMeshsVerticesToPositive();
					}
					foreach (TexturesSubMeshes.UserSubMeshes userSubMesh in item2.userSubMeshes)
					{
						if (!item2.isTiledTexture)
						{
							for (int num8 = 0; num8 < userSubMesh.originalUvVertices.Length; num8++)
							{
								if (userSubMesh.originalUvVertices[num8].x < 0f)
								{
									userSubMesh.originalUvVertices[num8].x = userSubMesh.originalUvVertices[num8].x * -1f;
								}
								if (userSubMesh.originalUvVertices[num8].y < 0f)
								{
									userSubMesh.originalUvVertices[num8].y = userSubMesh.originalUvVertices[num8].y * -1f;
								}
							}
							Vector2 vector = Vector2.zero;
							for (int num9 = 0; num9 < userSubMesh.originalUvVertices.Length; num9++)
							{
								vector = new Vector2(Mathf.Max(userSubMesh.originalUvVertices[num9].x, vector.x), Mathf.Max(userSubMesh.originalUvVertices[num9].y, vector.y));
							}
							Vector2 edgesPercentUsageOfThisTextures = item2.GetEdgesPercentUsageOfThisTextures();
							int rectIndexOfThatMainTexture = atlasData.GetRectIndexOfThatMainTexture(item2.mainTexture);
							for (int num10 = 0; num10 < userSubMesh.originalUvVertices.Length; num10++)
							{
								Vector2 zero3 = Vector2.zero;
								if (vector.x <= 1f)
								{
									zero3.x = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture].xMin, atlasData.atlasRects[rectIndexOfThatMainTexture].xMax, Mathf.Lerp(edgesPercentUsageOfThisTextures.x, 1f - edgesPercentUsageOfThisTextures.x, userSubMesh.originalUvVertices[num10].x));
								}
								if (vector.y <= 1f)
								{
									zero3.y = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture].yMin, atlasData.atlasRects[rectIndexOfThatMainTexture].yMax, Mathf.Lerp(edgesPercentUsageOfThisTextures.y, 1f - edgesPercentUsageOfThisTextures.y, userSubMesh.originalUvVertices[num10].y));
								}
								if (vector.x > 1f)
								{
									zero3.x = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture].xMin, atlasData.atlasRects[rectIndexOfThatMainTexture].xMax, Mathf.Lerp(edgesPercentUsageOfThisTextures.x, 1f - edgesPercentUsageOfThisTextures.x, userSubMesh.originalUvVertices[num10].x / vector.x));
								}
								if (vector.y > 1f)
								{
									zero3.y = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture].yMin, atlasData.atlasRects[rectIndexOfThatMainTexture].yMax, Mathf.Lerp(edgesPercentUsageOfThisTextures.y, 1f - edgesPercentUsageOfThisTextures.y, userSubMesh.originalUvVertices[num10].y / vector.y));
								}
								array2[num10 + userSubMesh.startOfUvVerticesInIndex] = zero3;
							}
						}
						if (item2.isTiledTexture)
						{
							Vector2 vector2 = Vector2.zero;
							for (int num11 = 0; num11 < userSubMesh.originalUvVertices.Length; num11++)
							{
								vector2 = new Vector2(Mathf.Max(userSubMesh.originalUvVertices[num11].x, vector2.x), Mathf.Max(userSubMesh.originalUvVertices[num11].y, vector2.y));
							}
							Vector2 edgesPercentUsageOfThisTextures2 = item2.GetEdgesPercentUsageOfThisTextures();
							int rectIndexOfThatMainTexture2 = atlasData.GetRectIndexOfThatMainTexture(item2.mainTexture);
							for (int num12 = 0; num12 < userSubMesh.originalUvVertices.Length; num12++)
							{
								Vector2 zero4 = Vector2.zero;
								zero4.x = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture2].xMin, atlasData.atlasRects[rectIndexOfThatMainTexture2].xMax, Mathf.Lerp(edgesPercentUsageOfThisTextures2.x, 1f - edgesPercentUsageOfThisTextures2.x, userSubMesh.originalUvVertices[num12].x / vector2.x));
								zero4.y = Mathf.Lerp(atlasData.atlasRects[rectIndexOfThatMainTexture2].yMin, atlasData.atlasRects[rectIndexOfThatMainTexture2].yMax, Mathf.Lerp(edgesPercentUsageOfThisTextures2.y, 1f - edgesPercentUsageOfThisTextures2.y, userSubMesh.originalUvVertices[num12].y / vector2.y));
								array2[num12 + userSubMesh.startOfUvVerticesInIndex] = zero4;
							}
						}
					}
				}
				ShowProgressBar("Finishing and Saving Assets...", show: true, 1f);
				skinnedMeshRenderer3.sharedMesh.uv = array2;
				ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.mainTexturePropertyToInsert, atlasData.mainTextureAtlas);
				if (allInOneParams.metallicMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.metallicMapPropertyToInsert, atlasData.metallicMapAtlas);
				}
				if (allInOneParams.specularMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.specularMapPropertyToInsert, atlasData.specularMapAtlas);
				}
				if (allInOneParams.normalMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.normalMapPropertyToInsert, atlasData.normalMapAtlas);
				}
				if (allInOneParams.normalMap2Support)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.normalMap2PropertyToInsert, atlasData.normalMap2Atlas);
				}
				if (allInOneParams.heightMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.heightMapPropertyToInsert, atlasData.heightMapAtlas);
				}
				if (allInOneParams.occlusionMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.occlusionMapPropertyToInsert, atlasData.occlusionMapAtlas);
				}
				if (allInOneParams.detailAlbedoMapSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.detailMapPropertyToInsert, atlasData.detailMapAtlas);
				}
				if (allInOneParams.detailMaskSupport)
				{
					ApplyAtlasInPropertyOfMaterial(skinnedMeshRenderer3.sharedMaterials[0], allInOneParams.detailMaskPropertyToInsert, atlasData.detailMaskAtlas);
				}
				if (highlightUvVertices)
				{
					for (int num13 = 0; num13 < skinnedMeshRenderer3.sharedMesh.uv.Length; num13++)
					{
						atlasData.mainTextureAtlas.SetPixel((int)((float)atlasData.mainTextureAtlas.width * skinnedMeshRenderer3.sharedMesh.uv[num13].x), (int)((float)atlasData.mainTextureAtlas.height * skinnedMeshRenderer3.sharedMesh.uv[num13].y), Color.yellow);
					}
					atlasData.mainTextureAtlas.Apply();
				}
				CombinedMeshesManager combinedMeshesManager = resultMergeGameObject.AddComponent<CombinedMeshesManager>();
				combinedMeshesManager.rootGameObject = base.gameObject;
				combinedMeshesManager.mergeMethodUsed = CombinedMeshesManager.MergeMethod.AllInOne;
				combinedMeshesManager.sourceMeshes = allSkinnedMeshsValidatedToCombine;
				resultMergeOriginalGameObjects = allItemsForCombine;
				array = allSkinnedMeshsValidatedToCombine;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				if (!legacyAnimationSupport)
				{
					animator.cullingMode = cullingMode;
				}
				if (legacyAnimationSupport)
				{
					animation.cullingType = cullingType;
				}
				DateTime now2 = DateTime.Now;
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Meshes", skinnedMeshRenderer3.sharedMesh, base.gameObject.name, "asset");
				}
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Atlases", atlasData.mainTextureAtlas, base.gameObject.name + " (MainTexture)", "asset");
				}
				if (saveDataInAssets && allInOneParams.metallicMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.metallicMapAtlas, base.gameObject.name + " (MetallicMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.specularMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.specularMapAtlas, base.gameObject.name + " (SpecularMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.normalMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.normalMapAtlas, base.gameObject.name + " (NormalMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.normalMap2Support)
				{
					SaveAssetAsFile("Atlases", atlasData.normalMap2Atlas, base.gameObject.name + " (NormalMap2x)", "asset");
				}
				if (saveDataInAssets && allInOneParams.heightMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.heightMapAtlas, base.gameObject.name + " (HeightMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.occlusionMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.occlusionMapAtlas, base.gameObject.name + " (OcclusionMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.detailAlbedoMapSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.detailMapAtlas, base.gameObject.name + " (DetailMap)", "asset");
				}
				if (saveDataInAssets && allInOneParams.detailMaskSupport)
				{
					SaveAssetAsFile("Atlases", atlasData.detailMaskAtlas, base.gameObject.name + " (DetailMask)", "asset");
				}
				if (saveDataInAssets)
				{
					SaveAssetAsFile("Materials", skinnedMeshRenderer3.sharedMaterials[0], base.gameObject.name, "mat");
				}
				if (savePrefabOfMerge)
				{
					SaveMergeAsPrefab("Prefabs", nameOfPrefabOfMerge, base.gameObject);
				}
				GenerateStatsTextAndFillIt(nameOfThisMerge, "All In One", num, now, now2, num2, meshCountAfter, num3, materialCount, num4, skinnedMeshRenderer3.sharedMesh.uv.Length);
				ShowProgressBar("", show: false, 1f);
				if (!Application.isPlaying)
				{
					Debug.Log("The merging of the meshes was completed successfully!");
				}
				LaunchLog("The merging of the meshes was completed successfully! The name of this merge is \"" + nameOfThisMerge + "\"!", LogTypeOf.Log);
				if (Application.isPlaying && onCombineMeshs != null)
				{
					onCombineMeshs.Invoke();
				}
				ShowLogsOfMergeTabAndClearUndoHistory();
			}
			catch (Exception exception)
			{
				StopMergeByErrorWhileMerging(exception);
			}
		}

		private void DoCombineMeshs_OnlyAnima2dMeshs()
		{
		}

		private void DoUndoCombineMeshs(bool runMonoIl2CppGc, bool runUnityGc, bool errorOcurred)
		{
			if (resultMergeGameObject == null)
			{
				LaunchLog("Currently the character's meshes are not merged. To undo a merge, perform one first.", LogTypeOf.Warning);
				return;
			}
			if (!errorOcurred)
			{
				ShowProgressBar("Undoing...", show: true, 1f);
			}
			if (resultMergeOriginalGameObjects != null)
			{
				GameObject[] array = resultMergeOriginalGameObjects;
				foreach (GameObject gameObject in array)
				{
					if (gameObject != null)
					{
						gameObject.SetActive(value: true);
					}
				}
			}
			DestroyGameObject(resultMergeGameObject);
			resultMergeOriginalGameObjects = null;
			resultMergeTextStats = "";
			resultMergeAssetsSaved.Clear();
			if (!errorOcurred)
			{
				resultMergeLogs.Clear();
			}
			if (runMonoIl2CppGc)
			{
				GC.Collect();
			}
			if (!errorOcurred)
			{
				ShowProgressBar("", show: false, 1f);
			}
			if (!errorOcurred && !Application.isPlaying)
			{
				Debug.Log("The last merge performed on this character was undone.");
			}
			if (Application.isPlaying && onUndoCombineMeshs != null)
			{
				onUndoCombineMeshs.Invoke();
			}
		}

		private void LaunchLog(string content, LogTypeOf logType)
		{
			if (launchConsoleLogs && Application.isPlaying)
			{
				if (logType == LogTypeOf.Assert || logType == LogTypeOf.Error || logType == LogTypeOf.Exception)
				{
					Debug.LogError(content);
				}
				_ = 3;
				_ = 4;
			}
		}

		private void ValidateAllVariables()
		{
			if (savePrefabOfMerge)
			{
				saveDataInAssets = true;
			}
			if (allInOneParams.specularMapSupport && allInOneParams.metallicMapSupport)
			{
				allInOneParams.metallicMapSupport = false;
				allInOneParams.specularMapSupport = false;
			}
			if (string.IsNullOrEmpty(nameOfThisMerge))
			{
				nameOfThisMerge = "Combined Meshes";
			}
			if (CurrentRP.haveAnotherSrpPackages && allInOneParams.useDefaultMainTextureProperty)
			{
				allInOneParams.mainTexturePropertyToFind = "_BaseMap";
				allInOneParams.mainTexturePropertyToInsert = "_BaseMap";
			}
			if (CurrentRP.haveAnotherSrpPackages && justMaterialColorsParams.useDefaultColorProperty)
			{
				justMaterialColorsParams.mainTexturePropertyToInsert = "_BaseMap";
				justMaterialColorsParams.colorPropertyToFind = "_BaseColor";
			}
			if (!CurrentRP.haveAnotherSrpPackages && allInOneParams.useDefaultMainTextureProperty)
			{
				allInOneParams.mainTexturePropertyToFind = "_MainTex";
				allInOneParams.mainTexturePropertyToInsert = "_MainTex";
			}
			if (!CurrentRP.haveAnotherSrpPackages && justMaterialColorsParams.useDefaultColorProperty)
			{
				justMaterialColorsParams.mainTexturePropertyToInsert = "_MainTex";
				justMaterialColorsParams.colorPropertyToFind = "_Color";
			}
		}

		private GameObject[] GetAllItemsForCombine(bool includeItemsRegisteredToBeIgnored, bool launchLogs)
		{
			List<GameObject> list = new List<GameObject>();
			if (mergeMethod != MergeMethod.OnlyAnima2dMeshes)
			{
				SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(combineInactives);
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					if (skinnedMeshRenderer.gameObject.GetComponent<CombinedMeshesManager>() == null)
					{
						list.Add(skinnedMeshRenderer.gameObject);
					}
				}
			}
			_ = mergeMethod;
			_ = 3;
			if (includeItemsRegisteredToBeIgnored)
			{
				return list.ToArray();
			}
			List<GameObject> list2 = new List<GameObject>();
			for (int j = 0; j < list.Count; j++)
			{
				bool flag = false;
				foreach (GameObject item in gameObjectsToIgnore)
				{
					if (item == list[j])
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (launchLogs)
					{
						LaunchLog("GameObject \"" + list[j].name + "\" was registered to be ignored during the merge, so it was not included in the merge processing.", LogTypeOf.Warning);
					}
				}
				else
				{
					list2.Add(list[j]);
				}
			}
			return list2.ToArray();
		}

		private void StopMergeByErrorWhileMerging(Exception exception)
		{
			if (exception != null)
			{
				LaunchLog("An error occurred while performing this merge. Read on for more details.\n\n" + exception.Message + "\n\n" + exception.StackTrace, LogTypeOf.Error);
				ShowProgressBar("", show: false, 1f);
				ShowLogsOfMergeTabAndClearUndoHistory();
				Debug.LogError("An error occurred during this merge. Check the log or console for more details.");
				DoUndoCombineMeshs(runMonoIl2CppGc: true, runUnityGc: true, errorOcurred: true);
			}
			if (exception == null)
			{
				ShowProgressBar("", show: false, 1f);
				Debug.LogError("An error occurred during this merge. Check the log or console for more details.");
				ShowLogsOfMergeTabAndClearUndoHistory();
				DoUndoCombineMeshs(runMonoIl2CppGc: true, runUnityGc: true, errorOcurred: true);
			}
		}

		private void ShowLogsOfMergeTabAndClearUndoHistory()
		{
		}

		private void ShowProgressBar(string message, bool show, float progress)
		{
		}

		private void SaveAssetAsFile(string folderNameToSave, UnityEngine.Object assetToSave, string fileName, string fileExtension)
		{
		}

		private void SaveMergeAsPrefab(string folderNameToSave, string prefabName, GameObject targetGo)
		{
		}

		private void GenerateStatsTextAndFillIt(string nameOfThisMerge, string mergeMethod, int processedVertices, DateTime mergeStart, DateTime mergeEnd, int meshCountBefore, int meshCountAfter, int drawCallReduction, int materialCount, int originalUvLenght, int modifyiedUvLenght)
		{
		}

		private SkinnedMeshRenderer[] GetAllSkinnedMeshsValidatedToCombine(GameObject[] gameObjectsToCombine)
		{
			List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>();
			for (int i = 0; i < gameObjectsToCombine.Length; i++)
			{
				SkinnedMeshRenderer component = gameObjectsToCombine[i].GetComponent<SkinnedMeshRenderer>();
				if (!(component != null))
				{
					continue;
				}
				if (!component.enabled)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: The Skinned Mesh Renderer is disabled", LogTypeOf.Log);
					continue;
				}
				if (component.sharedMesh == null)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: The mesh is null.", LogTypeOf.Log);
					continue;
				}
				if (component.sharedMesh.blendShapeCount > 0 && blendShapesSupport == BlendShapesSupport.Disabled)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: The mesh contains Blendshapes, and the \"Ignore Blendshapes\" option is enabled.", LogTypeOf.Log);
					continue;
				}
				if (component.sharedMaterials == null)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: This mesh has no materials, and materials list is null.", LogTypeOf.Log);
					continue;
				}
				if (component.sharedMaterials.Length == 0)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: This mesh has no materials.", LogTypeOf.Log);
					continue;
				}
				if (component.sharedMaterials.Length != component.sharedMesh.subMeshCount)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: The amount of materials in this mesh does not match the number of sub-meshes.", LogTypeOf.Log);
					continue;
				}
				bool flag = false;
				Material[] sharedMaterials = component.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j] == null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					LaunchLog("The mesh present in GameObject " + component.gameObject.name + " was ignored during the merge process. Reason: Null materials were found in this mesh.", LogTypeOf.Log);
					continue;
				}
				if (mergeMethod == MergeMethod.AllInOne && allInOneParams.mergeTiledTextures == MergeTiledTextures.SkipAll)
				{
					bool flag2 = false;
					Vector2[] uv = component.sharedMesh.uv;
					for (int j = 0; j < uv.Length; j++)
					{
						Vector2 vector = uv[j];
						if (vector.x > 1f || vector.y > 1f)
						{
							flag2 = true;
						}
						if (vector.x < 0f || vector.y < 0f)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						LaunchLog("The mesh present in \"" + component.transform.name + "\" has a larger UV map than the texture (tiled texture). If the \"Merge Tiled Texture\" option is disabled, this mesh was ignored during the merge process. Keep in mind that if this mesh uses a higher UV than its texture (tiled texture), the texture will have to be adapted to fit in an atlas, and this can end up untwisting the way the texture is rendered in this mesh.", LogTypeOf.Log);
						continue;
					}
				}
				list.Add(component);
			}
			return list.ToArray();
		}

		private bool ExistsDifferentRootBones(SkinnedMeshRenderer[] skinnedMeshRenderers, bool launchLogs)
		{
			Transform rootBone = skinnedMeshRenderers[0].rootBone;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
			{
				if (rootBone != skinnedMeshRenderer.rootBone)
				{
					if (launchLogs)
					{
						LaunchLog("Different root bones were found in your character's meshes. Combining meshes with different root bones can cause IK animation problems for example, but in general, the merge works without problems. If you activate the \"Only Equal Root Bones\" option, the Skinned Mesh Combiner will not combine meshes with different root bones in your character.", LogTypeOf.Log);
					}
					return true;
				}
			}
			return false;
		}

		private int CountVerticesInAllMeshes(SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			int num = 0;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
			{
				num += skinnedMeshRenderer.sharedMesh.vertexCount;
			}
			return num;
		}

		private void DestroyGameObject(GameObject gameObject)
		{
			UnityEngine.Object.Destroy(gameObject);
		}

		private Material GetValidatedCopyOfMaterial(Material targetMaterial, bool copyPropertiesOfTargetMaterial, bool clearAllTextures)
		{
			Material material = new Material(targetMaterial.shader);
			if (copyPropertiesOfTargetMaterial)
			{
				material.CopyPropertiesFromMaterial(targetMaterial);
			}
			if (clearAllTextures)
			{
				if (material.HasProperty("_MainTex"))
				{
					material.SetTexture("_MainTex", null);
				}
				if (material.HasProperty("_BaseMap"))
				{
					material.SetTexture("_BaseMap", null);
				}
				if (material.HasProperty("_MetallicGlossMap"))
				{
					material.SetTexture("_MetallicGlossMap", null);
				}
				if (material.HasProperty("_SpecGlossMap"))
				{
					material.SetTexture("_SpecGlossMap", null);
				}
				if (material.HasProperty("_BumpMap"))
				{
					material.SetTexture("_BumpMap", null);
				}
				if (material.HasProperty("_DetailNormalMap"))
				{
					material.SetTexture("_DetailNormalMap", null);
				}
				if (material.HasProperty("_ParallaxMap"))
				{
					material.SetTexture("_ParallaxMap", null);
				}
				if (material.HasProperty("_OcclusionMap"))
				{
					material.SetTexture("_OcclusionMap", null);
				}
				if (material.HasProperty("_DetailMapSupport"))
				{
					material.SetTexture("_DetailMapSupport", null);
				}
				if (material.HasProperty("_DetailMask"))
				{
					material.SetTexture("_DetailMask", null);
				}
				if (material.HasProperty("_Color"))
				{
					material.SetColor("_Color", Color.white);
				}
				if (material.HasProperty("_BaseColor"))
				{
					material.SetColor("_BaseColor", Color.white);
				}
			}
			return material;
		}

		private void MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(SkinnedMeshRenderer[] skinnedMeshesToMerge, Mesh finalMesh, SkinnedMeshRenderer finalSkinnedMeshRenderer)
		{
			List<BlendShapeData> list = new List<BlendShapeData>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 0;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshesToMerge)
			{
				string[] array = new string[skinnedMeshRenderer.sharedMesh.blendShapeCount];
				for (int j = 0; j < skinnedMeshRenderer.sharedMesh.blendShapeCount; j++)
				{
					array[j] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(j);
				}
				for (int k = 0; k < array.Length; k++)
				{
					BlendShapeData blendShapeData = new BlendShapeData();
					blendShapeData.blendShapeFrameName = array[k];
					blendShapeData.blendShapeFrameIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(array[k]);
					blendShapeData.blendShapeCurrentValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeData.blendShapeFrameIndex);
					Vector3[] array2 = new Vector3[skinnedMeshRenderer.sharedMesh.vertexCount];
					Vector3[] array3 = new Vector3[skinnedMeshRenderer.sharedMesh.vertexCount];
					Vector3[] array4 = new Vector3[skinnedMeshRenderer.sharedMesh.vertexCount];
					Vector3[] collection = new Vector3[finalMesh.vertexCount];
					Vector3[] collection2 = new Vector3[finalMesh.vertexCount];
					Vector3[] collection3 = new Vector3[finalMesh.vertexCount];
					blendShapeData.startDeltaVertices.AddRange(collection);
					blendShapeData.startDeltaNormals.AddRange(collection2);
					blendShapeData.startDeltaTangents.AddRange(collection3);
					blendShapeData.finalDeltaVertices.AddRange(collection);
					blendShapeData.finalDeltaNormals.AddRange(collection2);
					blendShapeData.finalDeltaTangents.AddRange(collection3);
					if (skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(array[k]) != -1)
					{
						skinnedMeshRenderer.sharedMesh.GetBlendShapeFrameVertices(blendShapeData.blendShapeFrameIndex, skinnedMeshRenderer.sharedMesh.GetBlendShapeFrameCount(blendShapeData.blendShapeFrameIndex) - 1, array2, array3, array4);
					}
					if (skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(array[k]) == -1)
					{
						LaunchLog("Mesh data could not be found in Blendshape \"" + array[k] + "\". This Blendshape may not work on the mesh resulting from the merge.", LogTypeOf.Warning);
					}
					for (int l = 0; l < array2.Length; l++)
					{
						blendShapeData.finalDeltaVertices[l + num] = array2[l] * blendShapesMultiplier;
					}
					for (int m = 0; m < array3.Length; m++)
					{
						blendShapeData.finalDeltaNormals[m + num] = array3[m] * blendShapesMultiplier;
					}
					for (int n = 0; n < array4.Length; n++)
					{
						blendShapeData.finalDeltaTangents[n + num] = array4[n] * blendShapesMultiplier;
					}
					list.Add(blendShapeData);
				}
				num += skinnedMeshRenderer.sharedMesh.vertexCount;
			}
			foreach (BlendShapeData item in list)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(item.blendShapeFrameName);
				if (dictionary.ContainsKey(item.blendShapeFrameName))
				{
					stringBuilder.Append(" (");
					stringBuilder.Append(dictionary[item.blendShapeFrameName]);
					stringBuilder.Append(")");
					LaunchLog("The Blendshape with the name of \"" + item.blendShapeFrameName + "\" was found in more than one mesh. This would generate duplicates of the same Blendshape in the mesh resulting from the merge, so Blendshape \"" + item.blendShapeFrameName + "\" (duplicate) received a duplicate counter (for example \"" + item.blendShapeFrameName + " (0)\"). This will keep all Blendshapes working.", LogTypeOf.Warning);
				}
				finalMesh.AddBlendShapeFrame(stringBuilder.ToString(), 0f, item.startDeltaVertices.ToArray(), item.startDeltaNormals.ToArray(), item.startDeltaTangents.ToArray());
				finalMesh.AddBlendShapeFrame(stringBuilder.ToString(), 100f, item.finalDeltaVertices.ToArray(), item.finalDeltaNormals.ToArray(), item.finalDeltaTangents.ToArray());
				if (dictionary.ContainsKey(item.blendShapeFrameName))
				{
					dictionary[item.blendShapeFrameName]++;
				}
				if (!dictionary.ContainsKey(item.blendShapeFrameName))
				{
					dictionary.Add(item.blendShapeFrameName, 0);
				}
			}
			if (blendShapesSupport != BlendShapesSupport.FullSupport)
			{
				return;
			}
			foreach (BlendShapeData item2 in list)
			{
				if (item2.blendShapeCurrentValue > 0f)
				{
					finalSkinnedMeshRenderer.SetBlendShapeWeight(finalMesh.GetBlendShapeIndex(item2.blendShapeFrameName), item2.blendShapeCurrentValue);
				}
			}
		}

		private Transform GetCorrectRootBoneFromAllOriginalSkinnedMeshRenderers(SkinnedMeshRenderer[] skinnedMeshesToMerge)
		{
			if (rootBoneToUse == RootBoneToUse.Manual)
			{
				return manualRootBoneToUse;
			}
			if (rootBoneToUse == RootBoneToUse.Automatic)
			{
				Transform result = null;
				Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshesToMerge)
				{
					if (skinnedMeshRenderer != null && skinnedMeshRenderer.rootBone != null)
					{
						if (dictionary.ContainsKey(skinnedMeshRenderer.rootBone))
						{
							dictionary[skinnedMeshRenderer.rootBone]++;
						}
						if (!dictionary.ContainsKey(skinnedMeshRenderer.rootBone))
						{
							dictionary.Add(skinnedMeshRenderer.rootBone, 1);
						}
					}
				}
				int num = 0;
				{
					foreach (KeyValuePair<Transform, int> item in dictionary)
					{
						if (item.Value > num)
						{
							result = item.Key;
							num = item.Value;
						}
					}
					return result;
				}
			}
			return null;
		}

		private ColorData GetDefaultAndNeutralColorForThisTexture(TextureType textureType)
		{
			return textureType switch
			{
				TextureType.MainTexture => new ColorData("RED", Color.red), 
				TextureType.MetallicMap => new ColorData("BLACK", Color.black), 
				TextureType.SpecularMap => new ColorData("BLACK", Color.black), 
				TextureType.NormalMap => new ColorData("PURPLE", new Color(128f / 255f, 128f / 255f, 1f, 1f)), 
				TextureType.HeightMap => new ColorData("BLACK", Color.black), 
				TextureType.OcclusionMap => new ColorData("WHITE", Color.white), 
				TextureType.DetailMap => new ColorData("GRAY", Color.gray), 
				TextureType.DetailMask => new ColorData("WHITE", Color.white), 
				_ => new ColorData("RED", Color.red), 
			};
		}

		private int UvBoundToPixels(float uvSize, int textureSize)
		{
			return (int)(uvSize * (float)textureSize);
		}

		private float[] UvBoundSplitted(float uvSize)
		{
			if (uvSize < 0f)
			{
				uvSize *= -1f;
			}
			float[] array = new float[2];
			string[] array2 = uvSize.ToString().Split(',');
			array[0] = float.Parse(array2[0]);
			array[1] = 0f;
			if (array2.Length > 1)
			{
				array[1] = float.Parse("0," + array2[1]);
			}
			return array;
		}

		private Texture2D GetValidatedCopyOfTexture(Material materialToFindTexture, string propertyToFindTexture, int widthOfCorrespondentMainTexture, int heightOfCorrespondentMainTexture, TexturesSubMeshes.UvBounds boundsUvValues, TextureType textureType, bool showProgress, float progress)
		{
			if (showProgress)
			{
				ShowProgressBar("Copying " + propertyToFindTexture, show: true, progress);
			}
			Texture2D texture2D = null;
			materialToFindTexture.EnableKeyword(propertyToFindTexture);
			if (materialToFindTexture.HasProperty(propertyToFindTexture) && materialToFindTexture.GetTexture(propertyToFindTexture) != null)
			{
				texture2D = (Texture2D)materialToFindTexture.GetTexture(propertyToFindTexture);
			}
			if (!materialToFindTexture.HasProperty(propertyToFindTexture) || materialToFindTexture.GetTexture(propertyToFindTexture) == null)
			{
				ColorData defaultAndNeutralColorForThisTexture = GetDefaultAndNeutralColorForThisTexture(textureType);
				LaunchLog("It was not possible to find the texture stored in property \"" + propertyToFindTexture + "\" of material \"" + materialToFindTexture.name + "\", so this Texture/Map was replaced by a " + defaultAndNeutralColorForThisTexture.colorName + " texture. This can affect how the texture or effect maps (such as Normal Maps, etc.) are displayed in the combined model. This can result in some small differences in the combined mesh when compared to the separate original meshes.", LogTypeOf.Warning);
				texture2D = new Texture2D(widthOfCorrespondentMainTexture, heightOfCorrespondentMainTexture);
				Color[] array = new Color[widthOfCorrespondentMainTexture * heightOfCorrespondentMainTexture];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = defaultAndNeutralColorForThisTexture.color;
				}
				texture2D.SetPixels(0, 0, widthOfCorrespondentMainTexture, heightOfCorrespondentMainTexture, array, 0);
			}
			Texture2D texture2D2 = null;
			try
			{
				int edgesSizeForTextures = GetEdgesSizeForTextures();
				int num = 0;
				int num2 = 0;
				int num3 = 16384;
				bool flag = false;
				if (!isTiledTexture(boundsUvValues))
				{
					num = edgesSizeForTextures + texture2D.width + edgesSizeForTextures;
					num2 = edgesSizeForTextures + texture2D.height + edgesSizeForTextures;
				}
				if (isTiledTexture(boundsUvValues))
				{
					num = edgesSizeForTextures + UvBoundToPixels(boundsUvValues.spaceMinorX, texture2D.width) + texture2D.width + UvBoundToPixels(boundsUvValues.spaceMajorX, texture2D.width) + edgesSizeForTextures;
					num2 = edgesSizeForTextures + UvBoundToPixels(boundsUvValues.spaceMinorY, texture2D.height) + texture2D.height + UvBoundToPixels(boundsUvValues.spaceMajorY, texture2D.height) + edgesSizeForTextures;
				}
				if (num >= num3 || num2 >= num3)
				{
					flag = true;
				}
				if (flag)
				{
					ColorData defaultAndNeutralColorForThisTexture2 = GetDefaultAndNeutralColorForThisTexture(textureType);
					if (!string.IsNullOrEmpty(texture2D.name))
					{
						LaunchLog("It was not possible to process the \"" + texture2D.name + "\" texture, as its size during processing was greater than Unity's " + num3 + " pixel limitation. This may have happened because its texture is larger than " + num3 + " pixels, or because the tiling of any of its meshes is too extensive. Try to use a texture smaller than " + num3 + " pixels and if that persists, skip any meshes that land a very large tile. During processing this texture reached the size of " + num + "x" + num2 + " pixels. This texture has been replaced by a simple texture of color " + defaultAndNeutralColorForThisTexture2.colorName + ".", LogTypeOf.Warning);
					}
					num = texture2D.width;
					num2 = texture2D.height;
				}
				texture2D2 = new Texture2D(num, num2, TextureFormat.ARGB32, mipChain: false, linear: false);
				Color32[] pixels = texture2D.GetPixels32(0);
				if (allInOneParams.pinkNormalMapsFix && textureType == TextureType.NormalMap && texture2D.format == TextureFormat.DXT5)
				{
					for (int j = 0; j < pixels.Length; j++)
					{
						Color color = pixels[j];
						color.r = color.a * 2f - 1f;
						color.g = color.g * 2f - 1f;
						Vector2 vector = new Vector2(color.r, color.g);
						color.b = Mathf.Sqrt(1f - Mathf.Clamp01(Vector2.Dot(vector, vector)));
						pixels[j] = new Color(color.r * 0.5f + 0.5f, color.g * 0.5f + 0.5f, color.b * 0.5f + 0.5f);
					}
				}
				if (flag)
				{
					ColorData defaultAndNeutralColorForThisTexture3 = GetDefaultAndNeutralColorForThisTexture(textureType);
					Color[] array2 = new Color[texture2D.width * texture2D.height];
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k] = defaultAndNeutralColorForThisTexture3.color;
					}
					texture2D2.SetPixels(0, 0, texture2D.width, texture2D.height, array2, 0);
				}
				if (!isTiledTexture(boundsUvValues) && !flag)
				{
					texture2D2.SetPixels32(edgesSizeForTextures, edgesSizeForTextures, texture2D.width, texture2D.height, pixels, 0);
				}
				if (isTiledTexture(boundsUvValues) && !flag)
				{
					if (showProgress)
					{
						ShowProgressBar("Copying and Generating Tiles for " + propertyToFindTexture, show: true, progress);
					}
					Color[] array3 = null;
					array3 = texture2D.GetPixels(texture2D.width - (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), 0, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), texture2D.height, 0);
					for (int l = 0; (float)l < UvBoundSplitted(boundsUvValues.spaceMinorY)[0] + UvBoundSplitted(boundsUvValues.spaceMajorY)[0] + 1f; l++)
					{
						texture2D2.SetPixels(edgesSizeForTextures, edgesSizeForTextures + UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorY)[1], texture2D.height) + l * texture2D.height, UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorX)[1], texture2D.width), texture2D.height, array3, 0);
					}
					array3 = texture2D.GetPixels(0, 0, texture2D.width, texture2D.height, 0);
					for (int m = 0; (float)m < UvBoundSplitted(boundsUvValues.spaceMinorX)[0] + UvBoundSplitted(boundsUvValues.spaceMajorX)[0] + 1f; m++)
					{
						for (int n = 0; (float)n < UvBoundSplitted(boundsUvValues.spaceMinorY)[0] + UvBoundSplitted(boundsUvValues.spaceMajorY)[0] + 1f; n++)
						{
							texture2D2.SetPixels(edgesSizeForTextures + (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.spaceMinorX)[1]) + m * texture2D.width, edgesSizeForTextures + (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.spaceMinorY)[1]) + n * texture2D.height, texture2D.width, texture2D.height, array3, 0);
						}
					}
					array3 = texture2D.GetPixels(0, 0, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), texture2D.height, 0);
					for (int num4 = 0; (float)num4 < UvBoundSplitted(boundsUvValues.spaceMinorY)[0] + UvBoundSplitted(boundsUvValues.spaceMajorY)[0] + 1f; num4++)
					{
						texture2D2.SetPixels(edgesSizeForTextures + (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.spaceMinorX)[1]) + ((int)UvBoundSplitted(boundsUvValues.spaceMinorX)[0] + (int)UvBoundSplitted(boundsUvValues.spaceMajorX)[0] + 1) * texture2D.width, edgesSizeForTextures + UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorY)[1], texture2D.height) + num4 * texture2D.height, UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMajorX)[1], texture2D.width), texture2D.height, array3, 0);
					}
					array3 = texture2D.GetPixels(0, texture2D.height - (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), texture2D.width, (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), 0);
					for (int num5 = 0; (float)num5 < UvBoundSplitted(boundsUvValues.spaceMinorX)[0] + UvBoundSplitted(boundsUvValues.spaceMajorX)[0] + 1f; num5++)
					{
						texture2D2.SetPixels(edgesSizeForTextures + UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorX)[1], texture2D.width) + num5 * texture2D.width, edgesSizeForTextures, texture2D.width, UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorY)[1], texture2D.height), array3, 0);
					}
					array3 = texture2D.GetPixels(0, 0, texture2D.width, (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), 0);
					for (int num6 = 0; (float)num6 < UvBoundSplitted(boundsUvValues.spaceMinorX)[0] + UvBoundSplitted(boundsUvValues.spaceMajorX)[0] + 1f; num6++)
					{
						texture2D2.SetPixels(edgesSizeForTextures + UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMinorX)[1], texture2D.width) + num6 * texture2D.width, edgesSizeForTextures + (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.spaceMinorY)[1]) + ((int)UvBoundSplitted(boundsUvValues.spaceMinorY)[0] + (int)UvBoundSplitted(boundsUvValues.spaceMajorY)[0] + 1) * texture2D.height, texture2D.width, UvBoundToPixels(UvBoundSplitted(boundsUvValues.spaceMajorY)[1], texture2D.height), array3, 0);
					}
					array3 = texture2D.GetPixels(texture2D.width - (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), texture2D.height - (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), 0);
					texture2D2.SetPixels(edgesSizeForTextures, edgesSizeForTextures, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), array3, 0);
					array3 = texture2D.GetPixels(0, texture2D.height - (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), 0);
					texture2D2.SetPixels(texture2D2.width - edgesSizeForTextures - (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), edgesSizeForTextures, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.minorY)[1]), array3, 0);
					array3 = texture2D.GetPixels(texture2D.width - (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), 0, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), 0);
					texture2D2.SetPixels(edgesSizeForTextures, texture2D2.height - edgesSizeForTextures - (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.minorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), array3, 0);
					array3 = texture2D.GetPixels(0, 0, (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), 0);
					texture2D2.SetPixels(texture2D2.width - edgesSizeForTextures - (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), texture2D2.height - edgesSizeForTextures - (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), (int)((float)texture2D.width * UvBoundSplitted(boundsUvValues.majorX)[1]), (int)((float)texture2D.height * UvBoundSplitted(boundsUvValues.majorY)[1]), array3, 0);
				}
				if (edgesSizeForTextures <= texture2D.width && edgesSizeForTextures <= texture2D.height && !flag)
				{
					Color[] array4 = null;
					array4 = texture2D2.GetPixels(texture2D2.width - edgesSizeForTextures - edgesSizeForTextures, 0, edgesSizeForTextures, texture2D2.height, 0);
					texture2D2.SetPixels(0, 0, edgesSizeForTextures, texture2D2.height, array4, 0);
					array4 = texture2D2.GetPixels(edgesSizeForTextures, 0, edgesSizeForTextures, texture2D2.height, 0);
					texture2D2.SetPixels(texture2D2.width - edgesSizeForTextures, 0, edgesSizeForTextures, texture2D2.height, array4, 0);
					array4 = texture2D2.GetPixels(0, edgesSizeForTextures, texture2D2.width, edgesSizeForTextures, 0);
					texture2D2.SetPixels(0, texture2D2.height - edgesSizeForTextures, texture2D2.width, edgesSizeForTextures, array4, 0);
					array4 = texture2D2.GetPixels(0, texture2D2.height - edgesSizeForTextures - edgesSizeForTextures, texture2D2.width, edgesSizeForTextures, 0);
					texture2D2.SetPixels(0, 0, texture2D2.width, edgesSizeForTextures, array4, 0);
				}
				if (edgesSizeForTextures > texture2D.width || (edgesSizeForTextures > texture2D.height && !flag))
				{
					LaunchLog("You have selected a texture border size (" + edgesSizeForTextures + "px), where the border size is larger than this texture (\"" + texture2D.name + "\" " + texture2D.width + "x" + texture2D.height + "px) size itself, causing this texture to repeat in the atlas. This increased the merging time due to the need for a new algorithm for creating the borders. It is recommended that the size of the edges of the textures in the atlas, does not exceed the size of the textures themselves.", LogTypeOf.Warning);
					for (int num7 = 0; num7 < edgesSizeForTextures; num7++)
					{
						for (int num8 = 0; num8 < texture2D2.height; num8++)
						{
							texture2D2.SetPixel(num7, num8, texture2D.GetPixel(texture2D.width - edgesSizeForTextures - edgesSizeForTextures + num7, num8));
						}
					}
					for (int num9 = texture2D2.width - edgesSizeForTextures; num9 < texture2D2.width; num9++)
					{
						for (int num10 = 0; num10 < texture2D2.height; num10++)
						{
							texture2D2.SetPixel(num9, num10, texture2D.GetPixel(texture2D.width - num9, num10));
						}
					}
					for (int num11 = 0; num11 < texture2D2.width; num11++)
					{
						for (int num12 = 0; num12 < edgesSizeForTextures; num12++)
						{
							texture2D2.SetPixel(num11, num12, texture2D.GetPixel(num11, texture2D.width - edgesSizeForTextures + num12));
						}
					}
					for (int num13 = 0; num13 < texture2D2.width; num13++)
					{
						for (int num14 = texture2D2.height - edgesSizeForTextures; num14 < texture2D2.height; num14++)
						{
							texture2D2.SetPixel(num13, num14, texture2D.GetPixel(num13, edgesSizeForTextures - (texture2D.height - num14)));
						}
					}
				}
			}
			catch (UnityException ex)
			{
				if (ex.Message.StartsWith("Texture '" + texture2D.name + "' is not readable"))
				{
					ColorData defaultAndNeutralColorForThisTexture4 = GetDefaultAndNeutralColorForThisTexture(textureType);
					texture2D2 = new Texture2D(widthOfCorrespondentMainTexture, heightOfCorrespondentMainTexture, TextureFormat.ARGB32, mipChain: false, linear: false);
					Color[] array5 = new Color[widthOfCorrespondentMainTexture * heightOfCorrespondentMainTexture];
					for (int num15 = 0; num15 < array5.Length; num15++)
					{
						array5[num15] = defaultAndNeutralColorForThisTexture4.color;
					}
					texture2D2.SetPixels(0, 0, widthOfCorrespondentMainTexture, heightOfCorrespondentMainTexture, array5, 0);
					LaunchLog("It was not possible to combine texture \"" + texture2D.name + "\" within an atlas, as it is not marked as \"Readable\" in the import settings (\"Read/Write Enabled\"). The texture has been replaced with a " + defaultAndNeutralColorForThisTexture4.colorName + " one.", LogTypeOf.Error);
				}
			}
			if (textureType == TextureType.MainTexture)
			{
				boundsUvValues.edgesUseX = (float)GetEdgesSizeForTextures() / (float)texture2D2.width;
				boundsUvValues.edgesUseY = (float)GetEdgesSizeForTextures() / (float)texture2D2.height;
			}
			if (texture2D2.width != widthOfCorrespondentMainTexture || texture2D2.height != heightOfCorrespondentMainTexture)
			{
				SMCTextureResizer.Bilinear(texture2D2, widthOfCorrespondentMainTexture, heightOfCorrespondentMainTexture);
			}
			return texture2D2;
		}

		private int GetAtlasMaxResolution()
		{
			if (mergeMethod == MergeMethod.AllInOne)
			{
				switch (allInOneParams.atlasResolution)
				{
				case AtlasSize.Pixels32x32:
					return 32;
				case AtlasSize.Pixels64x64:
					return 64;
				case AtlasSize.Pixels128x128:
					return 128;
				case AtlasSize.Pixels256x256:
					return 256;
				case AtlasSize.Pixels512x512:
					return 512;
				case AtlasSize.Pixels1024x1024:
					return 1024;
				case AtlasSize.Pixels2048x2048:
					return 2048;
				case AtlasSize.Pixels4096x4096:
					return 4096;
				case AtlasSize.Pixels8192x8192:
					return 8192;
				}
			}
			return 16;
		}

		private AtlasData CreateAllAtlas(List<TexturesSubMeshes> copyiedTextures, int maxResolution, int paddingBetweenTextures, bool showProgress)
		{
			AtlasData atlasData = new AtlasData();
			List<Texture2D> list = new List<Texture2D>();
			if (showProgress)
			{
				ShowProgressBar("Creating Base Atlas...", show: true, 1f);
			}
			list.Clear();
			foreach (TexturesSubMeshes copyiedTexture in copyiedTextures)
			{
				list.Add(copyiedTexture.mainTexture);
			}
			atlasData.originalMainTexturesUsedAndOrdenedAccordingToAtlasRect = list.ToArray();
			atlasData.atlasRects = atlasData.mainTextureAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			if (allInOneParams.metallicMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Metallic Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture2 in copyiedTextures)
				{
					list.Add(copyiedTexture2.metallicMap);
				}
				atlasData.metallicMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.specularMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Specular Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture3 in copyiedTextures)
				{
					list.Add(copyiedTexture3.specularMap);
				}
				atlasData.specularMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.normalMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Normal Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture4 in copyiedTextures)
				{
					list.Add(copyiedTexture4.normalMap);
				}
				atlasData.normalMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.normalMap2Support)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Normal Map 2 Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture5 in copyiedTextures)
				{
					list.Add(copyiedTexture5.normalMap2);
				}
				atlasData.normalMap2Atlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.heightMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Height Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture6 in copyiedTextures)
				{
					list.Add(copyiedTexture6.heightMap);
				}
				atlasData.heightMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.occlusionMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Occlusion Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture7 in copyiedTextures)
				{
					list.Add(copyiedTexture7.occlusionMap);
				}
				atlasData.occlusionMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.detailAlbedoMapSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Detail Map Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture8 in copyiedTextures)
				{
					list.Add(copyiedTexture8.detailMap);
				}
				atlasData.detailMapAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			if (allInOneParams.detailMaskSupport)
			{
				if (showProgress)
				{
					ShowProgressBar("Creating Detail Mask Atlas...", show: true, 1f);
				}
				list.Clear();
				foreach (TexturesSubMeshes copyiedTexture9 in copyiedTextures)
				{
					list.Add(copyiedTexture9.detailMask);
				}
				atlasData.detailMaskAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			}
			return atlasData;
		}

		private void ApplyAtlasInPropertyOfMaterial(Material targetMaterial, string propertyToInsertTexture, Texture2D atlasTexture)
		{
			if (targetMaterial.HasProperty(propertyToInsertTexture))
			{
				if (!targetMaterial.IsKeywordEnabled(propertyToInsertTexture))
				{
					targetMaterial.EnableKeyword(propertyToInsertTexture);
				}
				targetMaterial.SetTexture(propertyToInsertTexture, atlasTexture);
				if (!targetMaterial.IsKeywordEnabled(propertyToInsertTexture))
				{
					targetMaterial.EnableKeyword(propertyToInsertTexture);
				}
				if (propertyToInsertTexture == "_MetallicGlossMap" && !targetMaterial.IsKeywordEnabled("_METALLICGLOSSMAP") && allInOneParams.metallicMapSupport)
				{
					targetMaterial.EnableKeyword("_METALLICGLOSSMAP");
				}
				if (propertyToInsertTexture == "_SpecGlossMap" && !targetMaterial.IsKeywordEnabled("_SPECGLOSSMAP") && allInOneParams.specularMapSupport)
				{
					targetMaterial.EnableKeyword("_SPECGLOSSMAP");
				}
				if (propertyToInsertTexture == "_BumpMap" && !targetMaterial.IsKeywordEnabled("_NORMALMAP") && allInOneParams.normalMapSupport)
				{
					targetMaterial.EnableKeyword("_NORMALMAP");
				}
				if (propertyToInsertTexture == "_ParallaxMap" && !targetMaterial.IsKeywordEnabled("_PARALLAXMAP") && allInOneParams.heightMapSupport)
				{
					targetMaterial.EnableKeyword("_PARALLAXMAP");
				}
				if (propertyToInsertTexture == "_OcclusionMap" && !targetMaterial.IsKeywordEnabled("_OcclusionMap") && allInOneParams.occlusionMapSupport)
				{
					targetMaterial.EnableKeyword("_OcclusionMap");
				}
				if (propertyToInsertTexture == "_DetailAlbedoMap" && !targetMaterial.IsKeywordEnabled("_DETAIL_MULX2") && allInOneParams.detailAlbedoMapSupport)
				{
					targetMaterial.EnableKeyword("_DETAIL_MULX2");
				}
				if (propertyToInsertTexture == "_DetailNormalMap" && !targetMaterial.IsKeywordEnabled("_DETAIL_MULX2") && allInOneParams.normalMap2Support)
				{
					targetMaterial.EnableKeyword("_DETAIL_MULX2");
				}
			}
			if (!targetMaterial.HasProperty(propertyToInsertTexture))
			{
				LaunchLog("It was not possible to find and apply the atlas on property \"" + propertyToInsertTexture + "\" of the material to use (\"" + targetMaterial.name + "\"). Therefore, no atlas was applied to this property.", LogTypeOf.Error);
			}
		}

		private int GetEdgesSizeForTextures()
		{
			if (mergeMethod == MergeMethod.AllInOne)
			{
				switch (allInOneParams.mipMapEdgesSize)
				{
				case MipMapEdgesSize.Pixels0x0:
					return 0;
				case MipMapEdgesSize.Pixels16x16:
					return 16;
				case MipMapEdgesSize.Pixels32x32:
					return 32;
				case MipMapEdgesSize.Pixels64x64:
					return 64;
				case MipMapEdgesSize.Pixels128x128:
					return 128;
				case MipMapEdgesSize.Pixels256x256:
					return 256;
				case MipMapEdgesSize.Pixels512x512:
					return 512;
				case MipMapEdgesSize.Pixels1024x1024:
					return 1024;
				}
			}
			return 2;
		}

		private int GetAtlasPadding()
		{
			if (mergeMethod == MergeMethod.AllInOne)
			{
				switch (allInOneParams.atlasPadding)
				{
				case AtlasPadding.Pixels0x0:
					return 0;
				case AtlasPadding.Pixels2x2:
					return 2;
				case AtlasPadding.Pixels4x4:
					return 4;
				case AtlasPadding.Pixels8x8:
					return 8;
				case AtlasPadding.Pixels16x16:
					return 16;
				}
			}
			return 0;
		}

		private TexturesSubMeshes.UvBounds GetBoundValuesOfSubMeshUv(Vector2[] subMeshUv)
		{
			TexturesSubMeshes.UvBounds uvBounds = new TexturesSubMeshes.UvBounds();
			float[] array = new float[subMeshUv.Length];
			float[] array2 = new float[subMeshUv.Length];
			for (int i = 0; i < subMeshUv.Length; i++)
			{
				array[i] = subMeshUv[i].x;
				array2[i] = subMeshUv[i].y;
			}
			uvBounds.majorX = Mathf.Max(array);
			uvBounds.majorY = Mathf.Max(array2);
			uvBounds.minorX = Mathf.Min(array);
			uvBounds.minorY = Mathf.Min(array2);
			return uvBounds;
		}

		private TexturesSubMeshes GetTheTextureSubMeshesOfMaterial(Material material, List<TexturesSubMeshes> listOfTexturesAndSubMeshes)
		{
			foreach (TexturesSubMeshes listOfTexturesAndSubMesh in listOfTexturesAndSubMeshes)
			{
				if (listOfTexturesAndSubMesh.material == material && !listOfTexturesAndSubMesh.isTiledTexture)
				{
					return listOfTexturesAndSubMesh;
				}
			}
			return null;
		}

		private bool isTiledTexture(TexturesSubMeshes.UvBounds bounds)
		{
			if (bounds.minorX < 0f || bounds.minorY < 0f || bounds.majorX > 1f || bounds.majorY > 1f)
			{
				return true;
			}
			if (bounds.minorX >= 0f && bounds.minorY >= 0f && bounds.majorX <= 1f)
			{
				_ = bounds.majorY;
				_ = 1f;
				return false;
			}
			return false;
		}

		private Texture2D GetTextureFilledWithColorOfMaterial(Material targetMaterial, string colorPropertyToFind, int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false, linear: false);
			Color color = Color.white;
			if (targetMaterial.HasProperty(colorPropertyToFind))
			{
				color = targetMaterial.GetColor(colorPropertyToFind);
			}
			if (!targetMaterial.HasProperty(colorPropertyToFind))
			{
				LaunchLog("It was not possible to find the color stored in property \"" + colorPropertyToFind + "\" of material \"" + targetMaterial.name + "\", so this Color was replaced by a GRAY texture.", LogTypeOf.Warning);
				color = Color.gray;
			}
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			texture2D.SetPixels(0, 0, width, height, array, 0);
			return texture2D;
		}

		private ColorAtlasData CreateColorAtlas(UvDataAndColorOfThisSubmesh[] uvDatasAndColors, int maxResolution, int paddingBetweenTextures, bool showProgress)
		{
			ColorAtlasData colorAtlasData = new ColorAtlasData();
			List<Texture2D> list = new List<Texture2D>();
			if (showProgress)
			{
				ShowProgressBar("Creating Colors Atlas...", show: true, 1f);
			}
			list.Clear();
			foreach (UvDataAndColorOfThisSubmesh uvDataAndColorOfThisSubmesh in uvDatasAndColors)
			{
				list.Add(uvDataAndColorOfThisSubmesh.textureColor);
			}
			colorAtlasData.originalTexturesUsedAndOrdenedAccordingToAtlasRect = list.ToArray();
			colorAtlasData.atlasRects = colorAtlasData.colorAtlas.PackTextures(list.ToArray(), paddingBetweenTextures, maxResolution);
			return colorAtlasData;
		}

		private int GetAtlasMaxResolutionAnima2D()
		{
			if (mergeMethod == MergeMethod.OnlyAnima2dMeshes)
			{
				switch (onlyAnima2dMeshes.atlasResolution)
				{
				case AtlasSize.Pixels32x32:
					return 32;
				case AtlasSize.Pixels64x64:
					return 64;
				case AtlasSize.Pixels128x128:
					return 128;
				case AtlasSize.Pixels256x256:
					return 256;
				case AtlasSize.Pixels512x512:
					return 512;
				case AtlasSize.Pixels1024x1024:
					return 1024;
				case AtlasSize.Pixels2048x2048:
					return 2048;
				case AtlasSize.Pixels4096x4096:
					return 4096;
				case AtlasSize.Pixels8192x8192:
					return 8192;
				}
			}
			return 16;
		}
	}
}
