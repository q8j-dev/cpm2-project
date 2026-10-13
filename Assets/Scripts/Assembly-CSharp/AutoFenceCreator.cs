using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class AutoFenceCreator : MonoBehaviour
{
	public enum SplineFillMode
	{
		fixedNumPerSpan = 0,
		equiDistant = 1,
		angleDependent = 2
	}

	public enum FencePrefabType
	{
		postPrefab = 0,
		railPrefab = 1
	}

	public enum FenceSlopeMode
	{
		slope = 0,
		step = 1,
		shear = 2
	}

	private int objectsPerFolder = 100;

	public const float DEFAULT_RAIL_LENGTH = 3f;

	[Range(0.1f, 10f)]
	public float gs = 1f;

	public bool scaleInterpolationAlso = true;

	public Vector3 startPoint = Vector3.zero;

	public Vector3 endPoint = Vector3.zero;

	private Vector3 gapStart = Vector3.zero;

	private Vector3 gapEnd = Vector3.zero;

	private List<Vector3> gaps = new List<Vector3>();

	public bool allowGaps = true;

	public bool showDebugGapLine = true;

	private int defaultPoolSize = 100;

	public GameObject fencesFolder;

	public GameObject postsFolder;

	public GameObject railsFolder;

	public GameObject subsFolder;

	public List<GameObject> folderList = new List<GameObject>();

	private List<Transform> posts = new List<Transform>();

	public List<Transform> rails = new List<Transform>();

	private List<Transform> subs = new List<Transform>();

	private List<Transform> subJoiners = new List<Transform>();

	private List<Transform> markers = new List<Transform>();

	public List<Mesh> railMeshBuffers = new List<Mesh>();

	private List<Vector3> interPostPositions = new List<Vector3>();

	public List<Vector3> clickPoints = new List<Vector3>();

	public List<int> clickPointFlags = new List<int>();

	public List<Vector3> keyPoints = new List<Vector3>();

	public List<Vector3> allPostsPositions = new List<Vector3>();

	public List<Vector3> handles = new List<Vector3>();

	public List<GameObject> postPrefabs = new List<GameObject>();

	public List<GameObject> railPrefabs = new List<GameObject>();

	public List<GameObject> subPrefabs = new List<GameObject>();

	public List<GameObject> subJoinerPrefabs = new List<GameObject>();

	private GameObject clickMarkerObj;

	public List<string> presetNames = new List<string>();

	public List<string> postNames = new List<string>();

	public List<string> railNames = new List<string>();

	public List<string> subNames = new List<string>();

	public List<int> presetNums = new List<int>();

	public List<int> postNums = new List<int>();

	public List<int> railNums = new List<int>();

	public List<int> subNums = new List<int>();

	private int postCounter;

	private int subCounter;

	private int subJoinerCounter;

	public int railCounter;

	public int currentPostType;

	public int currentRailType;

	public int currentRailBType;

	public int currentSubType;

	public int currentSubJoinerType;

	[Range(0.2f, 10f)]
	public float fenceHeight = 2f;

	public Vector3 postSize = Vector3.one;

	[Range(-1f, 2f)]
	public float postHeightOffset;

	public Vector3 postRotation = Vector3.zero;

	private List<Material> originalPostMaterials = new List<Material>();

	[Range(0f, 12f)]
	public int numRails = 3;

	private bool useSecondaryRails = true;

	[Range(0.1f, 1f)]
	public float railGaps = 1f;

	[Range(0.1f, 1f)]
	public float minGap = 0.1f;

	[Range(0.1f, 1f)]
	public float maxGap = 1f;

	public Vector3 railPositionOffset = Vector3.zero;

	public Vector3 railSize = Vector3.one;

	public Vector3 railRotation = Vector3.zero;

	public bool centralizeRails;

	private List<Material> originalRailMaterials = new List<Material>();

	public bool autoHideBuriedRails = true;

	public bool showSubs;

	public int subsFixedOrProportionalSpacing = 1;

	[Range(0.1f, 20f)]
	public float subSpacing = 0.5f;

	public Vector3 subPositionOffset = Vector3.zero;

	public Vector3 subSize = Vector3.one;

	public Vector3 subRotation = Vector3.zero;

	public bool forceSubsToGroundContour;

	private List<Material> originalSubMaterials = new List<Material>();

	public bool useWave;

	[Range(0.01f, 10f)]
	public float frequency = 1f;

	[Range(0f, 2f)]
	public float amplitude = 0.25f;

	[Range((float)Math.PI * -4f, (float)Math.PI * 4f)]
	public float wavePosition = (float)Math.PI / 2f;

	public bool useSubJoiners;

	public bool interpolate = true;

	[Range(0.25f, 20f)]
	public float interPostDist = 4f;

	public bool smooth;

	[Range(0f, 1f)]
	public float tension;

	[Range(1f, 50f)]
	public int roundingDistance = 6;

	[Range(0f, 45f)]
	public float removeIfLessThanAngle = 4.5f;

	[Range(0.02f, 10f)]
	public float stripTooClose = 0.35f;

	public bool closeLoop;

	[Range(0f, 0.2f)]
	public float randomness;

	public bool showControls;

	public List<AutoFencePreset> presets = new List<AutoFencePreset>();

	public int currentPreset;

	private string presetFilePath = "Assets/Auto Fence Builder/Editor/AutoFencePresetFiles";

	public Vector3 lastDeletedPoint = Vector3.zero;

	public int lastDeletedIndex;

	public bool addColliders;

	private List<List<Vector3>> meshOrigVerts = new List<List<Vector3>>();

	private List<List<Vector3>> meshOrigNormals = new List<List<Vector3>>();

	private List<List<Vector4>> meshOrigTangents = new List<List<Vector4>>();

	private List<List<Vector2>> meshOrigUVs = new List<List<Vector2>>();

	private List<List<Vector2>> meshOrigUV2s = new List<List<Vector2>>();

	private List<List<int>> meshOrigTris = new List<List<int>>();

	private List<string> railMeshNames = new List<string>();

	private List<Mesh> origRailMeshes = new List<Mesh>();

	public FenceSlopeMode slopeMode;

	public int clearAllFencesWarning;

	public void Reset()
	{
		DestroyPools();
		keyPoints.Clear();
		LoadAllParts();
		SetupFolders();
		DestroyPools();
		CreatePools();
		fenceHeight = 2.4f;
		numRails = 2;
		railGaps = 0.3f;
		railPositionOffset.y = 0.36f;
		currentPostType = FindPrefabByName(FencePrefabType.postPrefab, "Angled_Post");
		currentRailType = FindPrefabByName(FencePrefabType.railPrefab, "CylinderSlim_Rail");
		currentSubType = FindPrefabByName(FencePrefabType.postPrefab, "SlimCylinder_Post");
		subSize.y = 0.36f;
		roundingDistance = 6;
		SetPostType(currentPostType, doRebuild: false);
		SetRailType(currentRailType, doRebuild: false);
		SetSubType(currentSubType, doRebuild: false);
		centralizeRails = true;
		slopeMode = FenceSlopeMode.slope;
		interpolate = true;
		interPostDist = 4f;
		autoHideBuriedRails = true;
		ReadPresetFiles();
		RebuildFenceFromPreset(0);
	}

	public void FinishAndStartNew()
	{
		GameObject gameObject = GameObject.Find("Current Fences Folder");
		if (gameObject != null)
		{
			gameObject.name = "Finished AutoFence";
		}
		DestroyUnused();
		gameObject.AddComponent<FenceMeshMerge>();
		clickPoints.Clear();
		clickPointFlags.Clear();
		keyPoints.Clear();
		posts.Clear();
		rails.Clear();
		subs.Clear();
		subJoiners.Clear();
		closeLoop = false;
		gaps.Clear();
		fencesFolder = null;
		SetupFolders();
		railMeshBuffers = new List<Mesh>();
		CreatePools();
	}

	public void ClearAllFences()
	{
		clickPoints.Clear();
		clickPointFlags.Clear();
		keyPoints.Clear();
		DestroyPools();
		CreatePools();
		DestroyMarkers();
		closeLoop = false;
	}

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("Current Fences Folder");
		if (gameObject != null)
		{
			if (Application.isEditor)
			{
				fencesFolder = gameObject;
				UnityEngine.Object.DestroyImmediate(gameObject);
				SetupFolders();
				LoadAllParts();
				DestroyPools();
				CreatePools();
				SetMarkersActiveStatus(showControls);
				ForceRebuildFromClickPoints();
			}
			else if (Application.isPlaying)
			{
				SetMarkersActiveStatus(newState: false);
			}
		}
	}

	public void SetupFolders()
	{
		if (fencesFolder == null)
		{
			fencesFolder = new GameObject("Current Fences Folder");
			folderList.Add(fencesFolder);
		}
		if (fencesFolder != null)
		{
			for (int num = fencesFolder.transform.childCount - 1; num >= 0; num--)
			{
				GameObject gameObject = fencesFolder.transform.GetChild(num).gameObject;
				for (int num2 = gameObject.transform.childCount - 1; num2 >= 0; num2--)
				{
					UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(num2).gameObject);
				}
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
		postsFolder = new GameObject("Posts");
		postsFolder.transform.parent = fencesFolder.transform;
		railsFolder = new GameObject("Rails");
		railsFolder.transform.parent = fencesFolder.transform;
		subsFolder = new GameObject("Subs");
		subsFolder.transform.parent = fencesFolder.transform;
	}

	public void CheckFolders()
	{
		if (fencesFolder == null)
		{
			SetupFolders();
			ClearAllFences();
			return;
		}
		if (postsFolder == null)
		{
			postsFolder = new GameObject("Posts");
			postsFolder.transform.parent = fencesFolder.transform;
			ResetPostPool();
		}
		if (railsFolder == null)
		{
			railsFolder = new GameObject("Rails");
			railsFolder.transform.parent = fencesFolder.transform;
			ResetRailPool();
		}
		if (subsFolder == null)
		{
			subsFolder = new GameObject("Subs");
			subsFolder.transform.parent = fencesFolder.transform;
			ResetSubPool();
		}
	}

	public void ReadPresetFiles()
	{
		string[] array = null;
		try
		{
			array = Directory.GetFiles(presetFilePath);
		}
		catch (Exception ex)
		{
			MonoBehaviour.print("Missing Presets Folder. Have you moved or renamed the Auto Fence Builder Folder or its contents?\n The Auto Fence Builder folder must be in the top level of your assets folder, with the scripts and its Editor folder inside. This Editor folder should contain the AutoFencePresetFiles folder.  " + ex.ToString());
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Contains("AutoFencePreset_") && text.EndsWith(".txt"))
			{
				string[] array3 = new string[50];
				array3 = File.ReadAllLines(text);
				CreatePresetFromStringValuesArray(array3);
			}
		}
		presets = presets.OrderBy((AutoFencePreset o) => o.name).ToList();
		CreatePresetStringsForMenus();
	}

	public void CreatePresetFromStringValuesArray(string[] readValues)
	{
		presets.Add(new AutoFencePreset(readValues[0], FindPrefabByName(FencePrefabType.postPrefab, readValues[1]), FindPrefabByName(FencePrefabType.railPrefab, readValues[2]), FindPrefabByName(FencePrefabType.postPrefab, readValues[3]), float.Parse(readValues[4]), float.Parse(readValues[5]), ParseVector3(readValues[6]), ParseVector3(readValues[7]), int.Parse(readValues[8]), float.Parse(readValues[9]), ParseVector3(readValues[10]), ParseVector3(readValues[11]), ParseVector3(readValues[12]), bool.Parse(readValues[13]), int.Parse(readValues[14]), float.Parse(readValues[15]), ParseVector3(readValues[16]), ParseVector3(readValues[17]), ParseVector3(readValues[18]), bool.Parse(readValues[19]), float.Parse(readValues[20]), float.Parse(readValues[21]), float.Parse(readValues[22]), bool.Parse(readValues[23]), bool.Parse(readValues[24]), float.Parse(readValues[25]), bool.Parse(readValues[26]), float.Parse(readValues[27]), int.Parse(readValues[28]), bool.Parse(readValues[29]), float.Parse(readValues[30]), float.Parse(readValues[31]), float.Parse(readValues[32])));
	}

	public void SavePresetFromCurrentSettings(string name)
	{
		if (presetNames.Contains(name))
		{
			name += "-";
		}
		presets.Add(new AutoFencePreset(name, currentPostType, currentRailType, currentSubType, fenceHeight, postHeightOffset, postSize, postRotation, numRails, railGaps, railPositionOffset, railSize, railRotation, showSubs, subsFixedOrProportionalSpacing, subSpacing, subPositionOffset, subSize, subRotation, useWave, frequency, amplitude, wavePosition, useSubJoiners, interpolate, interPostDist, smooth, tension, roundingDistance, forceSubsToGroundContour, randomness, removeIfLessThanAngle, stripTooClose));
		presets = presets.OrderBy((AutoFencePreset o) => o.name).ToList();
		SavePresetToPresetsFolder(name);
		if (presets.Count < 5)
		{
			presets.Clear();
			ReadPresetFiles();
		}
		CreatePresetStringsForMenus();
		currentPreset = FindPresetByName(name);
	}

	private int FindPresetByName(string name)
	{
		for (int i = 0; i < presetNames.Count; i++)
		{
			if (presetNames[i] == name)
			{
				return i;
			}
		}
		return -1;
	}

	private void CreatePresetStringsForMenus()
	{
		presetNames.Clear();
		presetNums.Clear();
		for (int i = 0; i < presets.Count; i++)
		{
			if (presetNames.Contains(presets[i].name))
			{
				presets[i].name += "+";
			}
			presetNames.Add(presets[i].name);
			presetNums.Add(i);
		}
	}

	public void SavePresetToPresetsFolder(string name)
	{
		List<string> list = new List<string>();
		list.Add(name);
		list.Add(postPrefabs[currentPostType].name);
		list.Add(railPrefabs[currentRailType].name);
		list.Add(subPrefabs[currentSubType].name);
		list.Add(fenceHeight.ToString("F3"));
		list.Add(postHeightOffset.ToString("F3"));
		list.Add(VectorToString(postSize));
		list.Add(VectorToString(postRotation));
		list.Add(numRails.ToString());
		list.Add(railGaps.ToString());
		list.Add(VectorToString(railPositionOffset));
		list.Add(VectorToString(railSize));
		list.Add(VectorToString(railRotation));
		list.Add(showSubs.ToString());
		list.Add(subsFixedOrProportionalSpacing.ToString());
		list.Add(subSpacing.ToString("F3"));
		list.Add(VectorToString(subPositionOffset));
		list.Add(VectorToString(subSize));
		list.Add(VectorToString(subRotation));
		list.Add(useWave.ToString());
		list.Add(frequency.ToString("F3"));
		list.Add(amplitude.ToString("F3"));
		list.Add(wavePosition.ToString("F3"));
		list.Add(useSubJoiners.ToString());
		list.Add(interpolate.ToString());
		list.Add(interPostDist.ToString());
		list.Add(smooth.ToString());
		list.Add(tension.ToString("F3"));
		list.Add(roundingDistance.ToString());
		list.Add(forceSubsToGroundContour.ToString());
		list.Add(randomness.ToString("F3"));
		list.Add(removeIfLessThanAngle.ToString("F3"));
		list.Add(stripTooClose.ToString("F3"));
		string text = "";
		for (int i = 0; i < list.Count; i++)
		{
			text = text + list[i] + "\r";
		}
		if (!Directory.Exists(presetFilePath))
		{
			Directory.CreateDirectory(presetFilePath);
		}
		string partialTimeString = GetPartialTimeString();
		File.WriteAllText(presetFilePath + "/AutoFencePreset_" + name + "_" + partialTimeString + ".txt", text);
	}

	private void LoadAllParts()
	{
		UnityEngine.Object[] array = Resources.LoadAll("FencePrefabs");
		foreach (UnityEngine.Object obj in array)
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject != null && gameObject.GetComponent<Renderer>() != null && gameObject.GetComponent<MeshFilter>() != null && gameObject.GetComponent<MeshFilter>().sharedMesh != null)
			{
				if (obj.name.EndsWith("_Post"))
				{
					postPrefabs.Add(gameObject);
					originalPostMaterials.Add(gameObject.GetComponent<Renderer>().sharedMaterial);
					subPrefabs.Add(gameObject);
					originalSubMaterials.Add(gameObject.GetComponent<Renderer>().sharedMaterial);
				}
				else if (obj.name.EndsWith("_Rail"))
				{
					railPrefabs.Add(gameObject);
					originalRailMaterials.Add(gameObject.GetComponent<Renderer>().sharedMaterial);
				}
				else if (obj.name.EndsWith("_SubJoiner"))
				{
					subJoinerPrefabs.Add(gameObject);
				}
			}
		}
		SaveRailMeshes();
		CreatePartStringsForMenus();
		string path = "FencePrefabs/ClickMarkerObj";
		clickMarkerObj = Resources.Load<GameObject>(path);
		if (clickMarkerObj == null)
		{
			MonoBehaviour.print("Can't load clickMarkerObj");
		}
	}

	private void CreatePartStringsForMenus()
	{
		postNames.Clear();
		postNums.Clear();
		int count = postPrefabs.Count;
		for (int i = 0; i < count; i++)
		{
			postNames.Add(postPrefabs[i].name);
			postNums.Add(i);
		}
		railNames.Clear();
		railNums.Clear();
		int count2 = railPrefabs.Count;
		for (int j = 0; j < count2; j++)
		{
			railNames.Add(railPrefabs[j].name);
			railNums.Add(j);
		}
		subNames.Clear();
		subNums.Clear();
		int count3 = subPrefabs.Count;
		for (int k = 0; k < count3; k++)
		{
			subNames.Add(subPrefabs[k].name);
			subNums.Add(k);
		}
	}

	public void RebuildFenceFromPreset(int presetIndex)
	{
		if (presetIndex < 0 || presetIndex >= presets.Count)
		{
			presets.Clear();
			ReadPresetFiles();
			if (presetIndex < 0 || presetIndex >= presets.Count)
			{
				MonoBehaviour.print("Presets missing. Have they been deleted or moved from Assets/Auto Fence Builder/Editor/AutoFencePresetFiles/ ?");
				return;
			}
		}
		AutoFencePreset autoFencePreset = presets[presetIndex];
		if (autoFencePreset != null)
		{
			currentPostType = autoFencePreset.postType;
			currentRailType = autoFencePreset.railType;
			currentSubType = autoFencePreset.subType;
			fenceHeight = autoFencePreset.fenceHeight;
			postHeightOffset = autoFencePreset.postHeightOffset;
			postSize = autoFencePreset.postSize;
			postRotation = autoFencePreset.postRotation;
			numRails = autoFencePreset.numRails;
			railGaps = autoFencePreset.railGaps;
			railPositionOffset = autoFencePreset.railPositionOffset;
			railSize = autoFencePreset.railSize;
			railRotation = autoFencePreset.railRotation;
			showSubs = autoFencePreset.showSubs;
			subsFixedOrProportionalSpacing = autoFencePreset.subsFixedOrProportionalSpacing;
			subSpacing = autoFencePreset.subSpacing;
			subSize = autoFencePreset.subSize;
			subPositionOffset = autoFencePreset.subPositionOffset;
			subRotation = autoFencePreset.subRotation;
			useWave = autoFencePreset.useWave;
			frequency = autoFencePreset.frequency;
			amplitude = autoFencePreset.amplitude;
			wavePosition = autoFencePreset.wavePosition;
			useSubJoiners = autoFencePreset.useJoiners;
			interPostDist = autoFencePreset.interPostDistance;
			forceSubsToGroundContour = autoFencePreset.forceSubsToGroundContour;
			randomness = autoFencePreset.randomness;
			SetPostType(currentPostType, doRebuild: false);
			SetRailType(currentRailType, doRebuild: false);
			SetSubType(currentSubType, doRebuild: false);
			ForceRebuildFromClickPoints();
		}
	}

	public void Randomize()
	{
		currentPostType = (int)(UnityEngine.Random.value * (float)postPrefabs.Count);
		currentRailType = (int)(UnityEngine.Random.value * (float)railPrefabs.Count);
		string text = railPrefabs[currentRailType].name;
		currentSubType = (int)(UnityEngine.Random.value * (float)subPrefabs.Count);
		fenceHeight = (UnityEngine.Random.Range(0.5f, 3.5f) + UnityEngine.Random.Range(0.5f, 4f) + UnityEngine.Random.Range(0.5f, 4f) + UnityEngine.Random.Range(0.5f, 4f)) / 4f;
		railSize.y = 1f;
		if (text.EndsWith("_Panel_Rail"))
		{
			numRails = 1;
			railSize.y = fenceHeight * 0.85f;
		}
		else
		{
			numRails = (int)((UnityEngine.Random.value * 4f + UnityEngine.Random.value * 4f + UnityEngine.Random.value * 4f) / 3f) + 1;
		}
		railGaps = UnityEngine.Random.value * 0.6f + 0.2f;
		float num = fenceHeight;
		if (numRails > 1)
		{
			num = fenceHeight / (float)(numRails - 1);
		}
		num *= railGaps;
		float maxInclusive = (1f - railGaps) * 0.9f;
		railPositionOffset.y = UnityEngine.Random.Range(0.1f, maxInclusive);
		if (text.EndsWith("_Panel_Rail"))
		{
			railPositionOffset.y = 0.51f;
		}
		showSubs = false;
		if (Mathf.Round(UnityEngine.Random.value) > 0.5f)
		{
			showSubs = true;
			subSpacing = UnityEngine.Random.Range(0.4f, 5f);
			subSize.y = UnityEngine.Random.Range(0.5f, 1f);
		}
		SetPostType(currentPostType, doRebuild: false);
		SetRailType(currentRailType, doRebuild: false);
		SetSubType(currentSubType, doRebuild: false);
		ForceRebuildFromClickPoints();
	}

	public void SetPostType(int type, bool doRebuild)
	{
		currentPostType = type;
		DeactivateEntirePool();
		ResetPostPool();
		if (doRebuild)
		{
			ForceRebuildFromClickPoints();
		}
	}

	public void SetRailType(int railType, bool doRebuild)
	{
		FenceSlopeMode fenceSlopeMode = slopeMode;
		currentRailType = railType;
		if (!railNames[currentRailType].EndsWith("Panel_Rail"))
		{
			slopeMode = FenceSlopeMode.slope;
		}
		else
		{
			slopeMode = FenceSlopeMode.shear;
			if (slopeMode != fenceSlopeMode)
			{
				HandleSlopeModeChange();
				doRebuild = true;
			}
		}
		DeactivateEntirePool();
		ResetRailPool();
		if (doRebuild)
		{
			ForceRebuildFromClickPoints();
		}
	}

	public void SetSubType(int type, bool doRebuild)
	{
		currentSubType = type;
		DeactivateEntirePool();
		ResetSubPool();
		if (doRebuild)
		{
			ForceRebuildFromClickPoints();
		}
	}

	private void SaveRailMeshes()
	{
		meshOrigVerts.Clear();
		meshOrigNormals.Clear();
		meshOrigTangents.Clear();
		meshOrigUVs.Clear();
		meshOrigUV2s.Clear();
		meshOrigTris.Clear();
		railMeshNames.Clear();
		origRailMeshes.Clear();
		for (int i = 0; i < railPrefabs.Count(); i++)
		{
			List<Vector3> list = new List<Vector3>();
			list.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.vertices);
			meshOrigVerts.Add(list);
			List<Vector3> list2 = new List<Vector3>();
			list2.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.normals);
			meshOrigNormals.Add(list2);
			List<Vector4> list3 = new List<Vector4>();
			list3.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.tangents);
			meshOrigTangents.Add(list3);
			List<Vector2> list4 = new List<Vector2>();
			list4.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.uv);
			meshOrigUVs.Add(list4);
			List<Vector2> list5 = new List<Vector2>();
			list5.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.uv2);
			meshOrigUV2s.Add(list5);
			List<int> list6 = new List<int>();
			list6.AddRange(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.triangles);
			meshOrigTris.Add(list6);
			railMeshNames.Add(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh.name);
			origRailMeshes.Add(railPrefabs[i].GetComponent<MeshFilter>().sharedMesh);
		}
	}

	public void HandleSlopeModeChange()
	{
		if (meshOrigVerts.Count == 0)
		{
			SaveRailMeshes();
		}
		for (int i = 0; i < railCounter; i++)
		{
			if (rails[i] != null)
			{
				rails[i].GetComponent<MeshFilter>().sharedMesh = origRailMeshes[currentRailType];
				rails[i].GetComponent<MeshFilter>().sharedMesh.vertices = meshOrigVerts[currentRailType].ToArray();
				rails[i].GetComponent<MeshFilter>().sharedMesh.normals = meshOrigNormals[currentRailType].ToArray();
				rails[i].GetComponent<MeshFilter>().sharedMesh.tangents = meshOrigTangents[currentRailType].ToArray();
				rails[i].GetComponent<MeshFilter>().sharedMesh.uv = meshOrigUVs[currentRailType].ToArray();
				rails[i].GetComponent<MeshFilter>().sharedMesh.uv2 = meshOrigUV2s[currentRailType].ToArray();
				rails[i].GetComponent<MeshFilter>().sharedMesh.triangles = meshOrigTris[currentRailType].ToArray();
			}
		}
	}

	public void ForceRebuildFromClickPoints()
	{
		if (clickPoints.Count == 0)
		{
			DeactivateEntirePool();
			return;
		}
		if (clickPoints.Count == 1)
		{
			DeactivateEntirePool();
			allPostsPositions.Clear();
			keyPoints.Clear();
			keyPoints.AddRange(clickPoints);
			AddNextPostAndInters(keyPoints[0], interpolateThisPost: false);
			return;
		}
		MergeClickPointGaps();
		DeactivateEntirePool();
		allPostsPositions.Clear();
		keyPoints.Clear();
		keyPoints.AddRange(clickPoints);
		MakeSplineFromClickPoints();
		startPoint = keyPoints[0];
		AddNextPostAndInters(keyPoints[0], interpolateThisPost: false, doRebuild: false);
		for (int i = 1; i < keyPoints.Count; i++)
		{
			endPoint = keyPoints[i];
			AddNextPostAndInters(keyPoints[i], interpolateThisPost: true, doRebuild: false);
			startPoint = keyPoints[i];
		}
		RemoveDiscontinuityBreaksFromAllPostPosition();
		RebuildFromFinalList();
		centralizeRails = false;
	}

	private void MergeClickPointGaps()
	{
		for (int i = 2; i < clickPointFlags.Count(); i++)
		{
			if (clickPointFlags[i] == 1 && clickPointFlags[i - 1] == 1)
			{
				DeletePost(i - 1, rebuild: false);
			}
		}
	}

	private void RemoveDiscontinuityBreaksFromAllPostPosition()
	{
		if (!allowGaps || clickPoints.Count < 3)
		{
			return;
		}
		_ = clickPoints[2];
		int num = 0;
		int num2 = -1;
		int num3 = 1;
		List<int> list = new List<int>();
		for (int i = 2; i < allPostsPositions.Count; i++)
		{
			Vector3 item = allPostsPositions[i];
			num = clickPoints.IndexOf(item);
			if (num == -1)
			{
				continue;
			}
			if (clickPointFlags[num] == 1)
			{
				num2 = i;
				for (int j = num3 + 1; j < num2; j++)
				{
					if (!list.Contains(j))
					{
						list.Add(j);
					}
				}
			}
			else
			{
				num3 = i;
			}
		}
		for (int num4 = list.Count - 1; num4 >= 0; num4--)
		{
			allPostsPositions.RemoveAt(list[num4]);
		}
	}

	private bool IsBreakPoint(Vector3 pos)
	{
		int num = clickPoints.IndexOf(pos);
		if (num != -1 && clickPointFlags[num] == 1)
		{
			return true;
		}
		return false;
	}

	public void RebuildFromFinalList()
	{
		postCounter = 0;
		railCounter = 0;
		subCounter = 0;
		subJoinerCounter = 0;
		Vector3 vector = Vector3.zero;
		CheckResizePools();
		SetMarkersActiveStatus(showControls);
		gaps.Clear();
		if (slopeMode != FenceSlopeMode.shear)
		{
			DestroyBufferedRailMeshes();
		}
		else
		{
			RereateRailMeshes();
		}
		for (int i = 0; i < allPostsPositions.Count; i++)
		{
			if (i > 0)
			{
				vector = allPostsPositions[i - 1];
			}
			Vector3 vector2 = allPostsPositions[i];
			SetupPost(i, vector2);
			if (i > 0)
			{
				if (vector == vector2)
				{
					MonoBehaviour.print("Warning: Posts A & B are in identical positions");
				}
				else if (!IsBreakPoint(allPostsPositions[i]) || !allowGaps)
				{
					CreateRailsAndSubPostsForSection(vector, vector2);
				}
				else
				{
					gaps.Add(vector);
					gaps.Add(vector2);
				}
			}
			postCounter++;
		}
		RotatePostsFinal();
		for (int j = 0; j < allPostsPositions.Count; j++)
		{
			if (posts[j] != null)
			{
				CreateCollider(posts[j].gameObject);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.1f, 0.1f, 1f, 0.4f);
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (showDebugGapLine && allowGaps)
		{
			for (int i = 0; i < gaps.Count(); i += 2)
			{
				zero = gaps[i];
				zero.y += 0.8f;
				zero2 = gaps[i + 1];
				zero2.y += 0.8f;
				Gizmos.DrawLine(zero, zero2);
				zero.y += 0.3f;
				zero2.y += 0.3f;
				Gizmos.DrawLine(zero, zero2);
				zero.y += 0.3f;
				zero2.y += 0.3f;
				Gizmos.DrawLine(zero, zero2);
			}
		}
	}

	public void CreateCollider(GameObject postA)
	{
		BoxCollider component = postA.GetComponent<BoxCollider>();
		if (component == null && !addColliders)
		{
			return;
		}
		if (component != null && (!addColliders || numRails > 0))
		{
			UnityEngine.Object.DestroyImmediate(component);
			return;
		}
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		component = (BoxCollider)postA.AddComponent(typeof(BoxCollider));
	}

	public void CreateRailCollider(GameObject rail)
	{
		BoxCollider component = rail.GetComponent<BoxCollider>();
		if (component == null && !addColliders)
		{
			return;
		}
		if (component != null && !addColliders)
		{
			UnityEngine.Object.DestroyImmediate(component);
			return;
		}
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		component = (BoxCollider)rail.AddComponent(typeof(BoxCollider));
		if (component != null)
		{
			component.enabled = true;
			Vector3 center = component.center;
			center.y = fenceHeight / 2f - railPositionOffset.y * fenceHeight;
			center.y *= gs;
			component.center = center;
			Vector3 size = component.size;
			size.y = fenceHeight * gs;
			component.size = size;
		}
	}

	public Mesh CombineRailMeshes()
	{
		CombineInstance[] array = new CombineInstance[railCounter];
		for (int i = 0; i < railCounter; i++)
		{
			GameObject gameObject = rails[i].gameObject;
			Mesh mesh = UnityEngine.Object.Instantiate(gameObject.GetComponent<MeshFilter>().sharedMesh);
			Vector3[] vertices = mesh.vertices;
			Vector3[] array2 = new Vector3[vertices.Length];
			for (int j = 0; j < vertices.Length; j++)
			{
				array2[j] = vertices[j];
			}
			mesh.vertices = array2;
			array[i].mesh = mesh;
			Transform transform = UnityEngine.Object.Instantiate(gameObject.transform);
			transform.position += gameObject.transform.parent.position;
			array[i].transform = transform.localToWorldMatrix;
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		Mesh mesh2 = new Mesh();
		mesh2.CombineMeshes(array);
		return mesh2;
	}

	public void CreateRailsAndSubPostsForSection(Vector3 A, Vector3 B)
	{
		Vector3 vector = CalculateDirection(A, B);
		Vector3 direction = A - B;
		float num = Vector3.Distance(A, B);
		float num2 = randomness;
		float num3 = fenceHeight * gs;
		Vector3 a = A;
		Vector3 b = B;
		a.y = (b.y = 0f);
		float num4 = Vector3.Distance(a, b);
		float num5 = A.y - B.y;
		if (numRails > 1)
		{
			num3 = fenceHeight * gs / (float)(numRails - 1);
		}
		num3 *= railGaps;
		float num6 = fenceHeight * railPositionOffset.y * gs;
		if (centralizeRails)
		{
			if (numRails > 1)
			{
				float num7 = fenceHeight * railGaps * gs;
				float y = (fenceHeight - num7) / 2f * gs;
				num6 = (railPositionOffset.y = y);
				railPositionOffset.y /= fenceHeight * gs;
			}
			else
			{
				railPositionOffset.y = 0.5f;
				num6 = fenceHeight * gs * railPositionOffset.y;
			}
		}
		else
		{
			num6 += 0.01f;
		}
		for (int i = 0; i < numRails; i++)
		{
			bool flag = false;
			GameObject gameObject = rails[railCounter].gameObject;
			if (gameObject == null)
			{
				MonoBehaviour.print("Missing Rail " + i + " Have you deleted one?");
				continue;
			}
			gameObject.gameObject.layer = 2;
			gameObject.hideFlags = HideFlags.None;
			gameObject.SetActive(value: true);
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.position = B + new Vector3(0f, num3 * (float)i + num6, 0f);
			if (slopeMode == FenceSlopeMode.step)
			{
				gameObject.transform.position += new Vector3(0f, num5, 0f);
			}
			gameObject.transform.Rotate(new Vector3(0f, -90f, 0f));
			gameObject.transform.Rotate(new Vector3(0f, vector.y, 0f));
			if (slopeMode == FenceSlopeMode.slope)
			{
				gameObject.transform.Rotate(new Vector3(0f, 0f, 0f - vector.x));
			}
			gameObject.transform.Translate(railPositionOffset.x, 0f, railPositionOffset.z * gs);
			float z = railRotation.z / num;
			gameObject.transform.Rotate(new Vector3(railRotation.x, railRotation.y, z), Space.Self);
			Vector3 one = Vector3.one;
			if (randomness > 0.0001f)
			{
				one += new Vector3(0f, UnityEngine.Random.Range(0f - num2, num2), UnityEngine.Random.Range(0f - num2, num2));
			}
			if (slopeMode == FenceSlopeMode.slope)
			{
				one.x *= num / 3f * railSize.x;
			}
			else
			{
				one.x *= num4 / 3f * railSize.x;
			}
			if (slopeMode != FenceSlopeMode.shear)
			{
				one.y *= railSize.y * gs;
			}
			if (railPrefabs[currentRailType].name.EndsWith("_Panel_Rail") && slopeMode != FenceSlopeMode.shear)
			{
				one.y *= fenceHeight * gs / 2f;
			}
			one.z *= railSize.z * gs;
			gameObject.transform.localScale = one;
			float num8 = num * railSize.x - num;
			if (railSize.x != 1f)
			{
				gameObject.transform.Translate(num8 / 2f, 0f, 0f);
			}
			gameObject.GetComponent<Renderer>().enabled = true;
			if (autoHideBuriedRails && Physics.Raycast(gameObject.transform.position, direction, out var hitInfo, num) && !hitInfo.collider.gameObject.name.StartsWith("Rail") && !hitInfo.collider.gameObject.name.StartsWith("Post") && !hitInfo.collider.gameObject.name.StartsWith("FenceManagerMarker"))
			{
				gameObject.hideFlags = HideFlags.HideInHierarchy;
				gameObject.SetActive(value: false);
				flag = true;
			}
			gameObject.name = "Rail " + railCounter;
			gameObject.transform.parent = railsFolder.transform;
			if (!flag)
			{
				if (slopeMode == FenceSlopeMode.shear)
				{
					MeshFilter component = gameObject.GetComponent<MeshFilter>();
					if (component == null)
					{
						MonoBehaviour.print("MeshFilter missing. Have you deleted one? Disable shear.");
						continue;
					}
					Mesh mesh = railMeshBuffers[railCounter];
					if (mesh == null)
					{
						MonoBehaviour.print("Mesh missing. Have you deleted one? Disable shear.");
						continue;
					}
					mesh.name = railMeshNames[currentRailType] + " sheared " + railCounter;
					if (meshOrigVerts.Count == 0)
					{
						SaveRailMeshes();
					}
					Vector3[] array = meshOrigVerts[currentRailType].ToArray();
					mesh.vertices = meshOrigVerts[currentRailType].ToArray();
					mesh.normals = meshOrigNormals[currentRailType].ToArray();
					mesh.uv = meshOrigUVs[currentRailType].ToArray();
					mesh.uv2 = meshOrigUV2s[currentRailType].ToArray();
					mesh.tangents = meshOrigTangents[currentRailType].ToArray();
					mesh.triangles = meshOrigTris[currentRailType].ToArray();
					Vector3[] vertices = mesh.vertices;
					for (int j = 0; j < vertices.Length; j++)
					{
						float num9 = Mathf.Abs(vertices[j].x) / 3f;
						vertices[j].y = array[j].y * railSize.y * fenceHeight / 2f * gs + num9 * num5;
					}
					mesh.vertices = vertices;
					component.sharedMesh = mesh;
				}
				if (i == 0)
				{
					CreateRailCollider(gameObject);
				}
				else
				{
					BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
					if (component2 != null)
					{
						UnityEngine.Object.DestroyImmediate(component2);
					}
				}
			}
			gameObject.gameObject.layer = 0;
			railCounter++;
			int num10 = railCounter / objectsPerFolder + 1;
			string text = "railsDividedFolder" + (num10 - 1);
			GameObject gameObject2 = GameObject.Find("Current Fences Folder/Rails/" + text);
			if (gameObject2 == null)
			{
				gameObject2 = new GameObject(text);
				gameObject2.transform.parent = railsFolder.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				CombineChildrenPlus combineChildrenPlus = gameObject2.AddComponent<CombineChildrenPlus>();
				if (combineChildrenPlus != null)
				{
					combineChildrenPlus.combineAtStart = true;
				}
			}
			gameObject.transform.parent = gameObject2.transform;
		}
		int num11 = 1;
		GameObject gameObject3 = null;
		float num12 = 1f;
		if (subsFixedOrProportionalSpacing == 1)
		{
			float num13 = subSpacing;
			num11 = (int)Mathf.Round(num / num13);
			if (num13 > num)
			{
				num11 = 1;
			}
			num12 = num / (float)(num11 + 1);
		}
		else if (subsFixedOrProportionalSpacing == 0)
		{
			num11 = (int)subSpacing;
			num12 = num / (float)(num11 + 1);
		}
		if (!showSubs)
		{
			return;
		}
		for (int k = 0; k < num11; k++)
		{
			GameObject gameObject4 = RequestSub(subCounter).gameObject;
			if (gameObject4 == null)
			{
				MonoBehaviour.print("Missing Sub " + k + " Have you deleted one?");
				continue;
			}
			gameObject4.hideFlags = HideFlags.None;
			gameObject4.SetActive(value: true);
			gameObject4.name = "Sub " + subCounter;
			gameObject4.transform.parent = subsFolder.transform;
			gameObject4.transform.position = B;
			gameObject4.transform.rotation = Quaternion.identity;
			gameObject4.transform.Rotate(new Vector3(0f, vector.y, 0f), Space.Self);
			gameObject4.transform.Translate(0f, 0f, subPositionOffset.z);
			gameObject4.transform.Rotate(new Vector3(subRotation.x, subRotation.y, subRotation.z), Space.Self);
			Vector3 vector2 = (B - A).normalized * num12 * (k + 1);
			gameObject4.transform.position += new Vector3(0f - vector2.x, 0f - vector2.y, 0f - vector2.z);
			gameObject4.transform.position += new Vector3(0f, subPositionOffset.y * fenceHeight * gs, 0f);
			float num14 = subSize.y * gs;
			if (useWave)
			{
				float num15 = Mathf.Sin((vector2.magnitude / num * (float)Math.PI * 2f + wavePosition) * frequency);
				num15 *= amplitude * gs;
				num14 = subSize.y * gs + num15 + amplitude * gs;
				if (k > 0 && useSubJoiners)
				{
					gameObject3 = RequestSubJoiner(subJoinerCounter++);
					if (gameObject3 != null)
					{
						gameObject3.transform.position = gameObject4.transform.position + new Vector3(0f, num14 * fenceHeight - 0.01f, 0f);
						gameObject3.transform.rotation = Quaternion.identity;
					}
				}
			}
			gameObject4.transform.Translate(subPositionOffset.x, 0f, 0f);
			Vector3 one2 = Vector3.one;
			float num16 = num2;
			if (num16 < 0.002f)
			{
				num16 = 0.002f;
			}
			one2 += new Vector3(UnityEngine.Random.Range(0f - num16, num16), UnityEngine.Random.Range(0f - num16, num16));
			one2.x *= subSize.x * gs;
			one2.y *= num14 * fenceHeight;
			one2.z *= subSize.z * gs;
			gameObject4.transform.localScale = one2;
			if (k > 0 && useSubJoiners && gameObject3 != null)
			{
				Vector3 vector3 = subs[subCounter].transform.position + new Vector3(0f, subs[subCounter].transform.localScale.y, 0f);
				Vector3 vector4 = subs[subCounter - 1].transform.position + new Vector3(0f, subs[subCounter - 1].transform.localScale.y, 0f);
				float x = Vector3.Distance(vector4, vector3);
				gameObject3.transform.localScale = new Vector3(x, gameObject3.transform.localScale.y, gameObject3.transform.localScale.z);
				Vector3 vector5 = CalculateDirection(vector3, vector4);
				gameObject3.transform.Rotate(new Vector3(0f, vector5.y - 90f, 0f - vector5.x + 180f));
				gameObject3.GetComponent<Renderer>().sharedMaterial = gameObject4.GetComponent<Renderer>().sharedMaterial;
			}
			if (forceSubsToGroundContour)
			{
				SetIgnoreColliders(inIgnore: true);
				Vector3 position = gameObject4.transform.position;
				float num17 = fenceHeight * 2f;
				position.y += num17;
				if (Physics.Raycast(position, Vector3.down, out var hitInfo2, 500f) && hitInfo2.collider.gameObject != null)
				{
					float num18 = hitInfo2.distance + 0.04f;
					gameObject4.transform.Translate(0f, 0f - (num18 - num17), 0f);
					one2.y += num18 - num17;
					gameObject4.transform.localScale = one2;
				}
				SetIgnoreColliders(inIgnore: false);
			}
			subCounter++;
			int num19 = subCounter / objectsPerFolder + 1;
			string text2 = "subsDividedFolder" + (num19 - 1);
			GameObject gameObject5 = GameObject.Find("Current Fences Folder/Subs/" + text2);
			if (gameObject5 == null)
			{
				gameObject5 = new GameObject(text2);
				gameObject5.transform.parent = subsFolder.transform;
				CombineChildrenPlus combineChildrenPlus2 = gameObject5.AddComponent<CombineChildrenPlus>();
				if (combineChildrenPlus2 != null)
				{
					combineChildrenPlus2.combineAtStart = true;
				}
			}
			gameObject4.transform.parent = gameObject5.transform;
		}
	}

	private void CreatePools()
	{
		CreatePostPool();
		CreateRailPool();
		CreateSubPool();
	}

	private void CreatePostPool(int n = 0, bool append = false)
	{
		if (currentPostType == -1 || currentPostType >= postPrefabs.Count || postPrefabs[currentPostType] == null)
		{
			currentPostType = 0;
		}
		if (n == 0)
		{
			n = defaultPoolSize;
		}
		int num = 0;
		if (append)
		{
			num = posts.Count;
			n = num + n;
		}
		for (int i = num; i < n; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(postPrefabs[currentPostType], Vector3.zero, Quaternion.identity);
			gameObject.SetActive(value: false);
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			posts.Add(gameObject.transform);
			gameObject.transform.parent = postsFolder.transform;
		}
		n = 8;
		if (clickPoints.Count() > 8)
		{
			n = (int)((float)clickPoints.Count() * 1.25f);
		}
		markers.Clear();
		if (clickMarkerObj == null)
		{
			MonoBehaviour.print("Reloading clickMarkerObj");
			clickMarkerObj = Resources.Load<GameObject>("FencePrefabs/ClickMarkerObj");
			if (clickMarkerObj == null)
			{
				MonoBehaviour.print("Can't load clickMarkerObj");
			}
		}
		if (clickMarkerObj != null)
		{
			for (int j = 0; j < n; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(clickMarkerObj, Vector3.zero, Quaternion.identity);
				gameObject2.SetActive(value: false);
				gameObject2.hideFlags = HideFlags.HideInHierarchy;
				markers.Add(gameObject2.transform);
				gameObject2.transform.parent = postsFolder.transform;
			}
		}
	}

	private void CreateRailPool(int n = 0, bool append = false)
	{
		if (currentRailType == -1 || currentRailType >= postPrefabs.Count || postPrefabs[currentRailType] == null)
		{
			currentRailType = 0;
		}
		if (n == 0)
		{
			n = defaultPoolSize * 2;
		}
		int num = 0;
		if (append)
		{
			num = rails.Count;
			n = num + n;
		}
		for (int i = num; i < n; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(railPrefabs[currentRailType], Vector3.zero, Quaternion.identity);
			gameObject.SetActive(value: false);
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			rails.Add(gameObject.transform);
			gameObject.transform.parent = railsFolder.transform;
		}
		RereateRailMeshes();
	}

	private void RereateRailMeshes()
	{
		DestroyBufferedRailMeshes();
		for (int i = 0; i < (allPostsPositions.Count - 1) * numRails; i++)
		{
			railMeshBuffers.Add(new Mesh());
		}
	}

	private void DestroyBufferedRailMeshes()
	{
		for (int i = 0; i < railMeshBuffers.Count; i++)
		{
			if (railMeshBuffers[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(railMeshBuffers[i]);
			}
		}
		railMeshBuffers.Clear();
	}

	private void CreateSubPool(int n = 0, bool append = false)
	{
		if (currentSubType == -1 || currentSubType >= postPrefabs.Count || postPrefabs[currentSubType] == null)
		{
			currentSubType = 0;
		}
		if (n == 0)
		{
			n = defaultPoolSize * 2;
		}
		int num = 0;
		if (append)
		{
			num = subs.Count;
			n = num + n;
		}
		for (int i = num; i < n; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(subPrefabs[currentSubType], Vector3.zero, Quaternion.identity);
			gameObject.SetActive(value: false);
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			subs.Add(gameObject.transform);
			gameObject.transform.parent = subsFolder.transform;
		}
		for (int j = num; j < n; j++)
		{
			if (!(subJoinerPrefabs[0] == null))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(subJoinerPrefabs[0], Vector3.zero, Quaternion.identity);
				gameObject2.SetActive(value: false);
				gameObject2.hideFlags = HideFlags.HideInHierarchy;
				subJoiners.Add(gameObject2.transform);
				gameObject2.transform.parent = subsFolder.transform;
			}
		}
	}

	public void CheckResizePools()
	{
		if (allPostsPositions.Count >= posts.Count - 1)
		{
			CreatePostPool((int)((float)allPostsPositions.Count * 1.25f) - posts.Count, append: true);
		}
		if (allPostsPositions.Count * numRails >= rails.Count - 1)
		{
			CreateRailPool((int)((float)(allPostsPositions.Count * numRails) * 1.25f - (float)rails.Count), append: true);
		}
		if (clickPoints.Count >= markers.Count - 1)
		{
			CreatePostPool((int)((float)allPostsPositions.Count * 1.25f) - posts.Count, append: true);
		}
	}

	private Transform RequestSub(int index)
	{
		if (index >= subs.Count - 1)
		{
			CreateSubPool((int)((float)subs.Count * 0.25f), append: true);
		}
		return subs[index];
	}

	private GameObject RequestSubJoiner(int index)
	{
		if (subJoiners[index] == null || subJoiners[index].gameObject == null)
		{
			return null;
		}
		GameObject obj = subJoiners[index].gameObject;
		obj.hideFlags = HideFlags.None;
		obj.SetActive(value: true);
		obj.name = "SubJoiner " + index;
		obj.transform.parent = subsFolder.transform;
		return obj;
	}

	private void ResetPostPool()
	{
		DestroyPosts();
		DestroyMarkers();
		CreatePostPool(posts.Count);
	}

	private void ResetRailPool()
	{
		DestroyRails();
		CreateRailPool(rails.Count);
	}

	private void ResetSubPool()
	{
		DestroySubs();
		CreateSubPool(subs.Count);
	}

	private void DestroyPosts()
	{
		for (int i = 0; i < posts.Count; i++)
		{
			if (posts[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(posts[i].gameObject);
			}
		}
		posts.Clear();
	}

	private void DestroyMarkers()
	{
		for (int i = 0; i < markers.Count; i++)
		{
			if (markers[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(markers[i].gameObject);
			}
		}
		markers.Clear();
	}

	private void DestroyRails()
	{
		for (int i = 0; i < rails.Count; i++)
		{
			if (rails[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(rails[i].gameObject);
			}
		}
		rails.Clear();
	}

	private void DestroySubs()
	{
		for (int i = 0; i < subs.Count; i++)
		{
			if (subs[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(subs[i].gameObject);
			}
			if (subJoiners[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(subJoiners[i].gameObject);
			}
		}
		subs.Clear();
		subJoiners.Clear();
	}

	private void DestroyPools()
	{
		DestroyPosts();
		DestroyMarkers();
		DestroyRails();
		DestroySubs();
	}

	public void DestroyUnused()
	{
		for (int i = 0; i < posts.Count; i++)
		{
			if (posts[i].gameObject != null && posts[i].gameObject.hideFlags == HideFlags.HideInHierarchy && !posts[i].gameObject.activeSelf)
			{
				UnityEngine.Object.DestroyImmediate(posts[i].gameObject);
			}
		}
		for (int j = 0; j < rails.Count; j++)
		{
			if (rails[j].gameObject != null && rails[j].gameObject.hideFlags == HideFlags.HideInHierarchy && !rails[j].gameObject.activeSelf)
			{
				UnityEngine.Object.DestroyImmediate(rails[j].gameObject);
			}
		}
		for (int k = 0; k < subs.Count; k++)
		{
			if (subs[k].gameObject != null && subs[k].gameObject.hideFlags == HideFlags.HideInHierarchy && !subs[k].gameObject.activeSelf)
			{
				UnityEngine.Object.DestroyImmediate(subs[k].gameObject);
				if (subJoiners[k].gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(subJoiners[k].gameObject);
				}
			}
		}
		DestroyMarkers();
	}

	public void CheckStatusOfAllClickPoints()
	{
		for (int i = 0; i < postCounter + 1; i++)
		{
			if (clickPoints.Contains(posts[i].position))
			{
				int index = clickPoints.IndexOf(posts[i].position);
				if (!posts[i].gameObject.activeInHierarchy)
				{
					DeletePost(index);
				}
			}
		}
	}

	public void DeactivateEntirePool()
	{
		for (int i = 0; i < posts.Count; i++)
		{
			if (posts[i] != null)
			{
				posts[i].gameObject.SetActive(value: false);
				posts[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
				posts[i].position = Vector3.zero;
			}
		}
		for (int j = 0; j < rails.Count; j++)
		{
			if (rails[j] != null)
			{
				rails[j].gameObject.SetActive(value: false);
				rails[j].gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
		}
		for (int k = 0; k < subs.Count; k++)
		{
			if (subs[k] != null)
			{
				subs[k].gameObject.SetActive(value: false);
				subs[k].gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			if (subJoiners[k] != null)
			{
				subJoiners[k].gameObject.SetActive(value: false);
				subJoiners[k].gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
		}
		for (int l = 0; l < markers.Count; l++)
		{
			if (markers[l] != null)
			{
				markers[l].gameObject.SetActive(value: false);
				markers[l].gameObject.hideFlags = HideFlags.HideInHierarchy;
				markers[l].position = Vector3.zero;
			}
		}
	}

	public void SetIgnoreClickMarkers(bool inIgnore)
	{
		int layer = 0;
		if (inIgnore)
		{
			layer = 2;
		}
		for (int i = 0; i < markers.Count; i++)
		{
			if (markers[i] != null)
			{
				markers[i].gameObject.layer = layer;
			}
		}
	}

	public void SetIgnoreColliders(bool inIgnore)
	{
		int layer = 0;
		if (inIgnore)
		{
			layer = 2;
		}
		for (int i = 0; i < posts.Count; i++)
		{
			if (posts[i] != null)
			{
				posts[i].gameObject.layer = layer;
			}
		}
		for (int j = 0; j < rails.Count; j++)
		{
			if (rails[j] != null)
			{
				rails[j].gameObject.layer = layer;
			}
		}
	}

	public void SetMarkersActiveStatus(bool newState)
	{
		for (int i = 0; i < clickPoints.Count; i++)
		{
			if (markers[i] != null)
			{
				markers[i].GetComponent<Renderer>().enabled = newState;
				markers[i].gameObject.SetActive(newState);
				if (newState)
				{
					markers[i].hideFlags = HideFlags.None;
				}
				else
				{
					markers[i].hideFlags = HideFlags.HideInHierarchy;
				}
			}
		}
	}

	public void ManageLoop(bool loop)
	{
		if (loop)
		{
			CloseLoop();
		}
		else
		{
			OpenLoop();
		}
	}

	public void CloseLoop()
	{
		if (clickPoints.Count < 3)
		{
			closeLoop = false;
		}
		if (clickPoints.Count >= 3 && clickPoints[clickPoints.Count - 1] != clickPoints[0])
		{
			clickPoints.Add(clickPoints[0]);
			clickPointFlags.Add(clickPointFlags[0]);
			ForceRebuildFromClickPoints();
		}
	}

	public void OpenLoop()
	{
		if (clickPoints.Count >= 3)
		{
			clickPoints.RemoveAt(clickPoints.Count - 1);
			ForceRebuildFromClickPoints();
		}
	}

	public void DeletePost(int index, bool rebuild = true)
	{
		if (clickPoints.Count > 0 && index < clickPoints.Count)
		{
			lastDeletedIndex = index;
			lastDeletedPoint = clickPoints[index];
			clickPoints.RemoveAt(index);
			clickPointFlags.RemoveAt(index);
			handles.RemoveAt(index);
			ForceRebuildFromClickPoints();
		}
	}

	public void InsertPost(Vector3 clickPosition)
	{
		float num = 1000000f;
		int num2 = -1;
		for (int i = 0; i < clickPoints.Count - 1; i++)
		{
			float num3 = CalcDistanceToLine(clickPoints[i], clickPoints[i + 1], clickPosition);
			if (num3 < num)
			{
				num = num3;
				num2 = i + 1;
			}
		}
		if (num2 != -1)
		{
			clickPoints.Insert(num2, clickPosition);
			clickPointFlags.Insert(num2, clickPointFlags[0]);
			ForceRebuildFromClickPoints();
			handles.Clear();
			for (int j = 0; j < clickPoints.Count; j++)
			{
				handles.Add(clickPoints[j]);
			}
		}
	}

	public float GetAngleAtPost(int i, List<Vector3> posts)
	{
		if (i >= posts.Count - 1 || i <= 0)
		{
			return 0f;
		}
		Vector3 vector = posts[i] - posts[i - 1];
		Vector3 to = posts[i + 1] - posts[i];
		return Vector3.Angle(vector, to);
	}

	private float CalcDistanceToLine(Vector3 lineStart, Vector3 lineEnd, Vector3 pt)
	{
		Vector3 direction = lineEnd - lineStart;
		Vector3 origin = lineStart;
		Ray ray = new Ray(origin, direction);
		float magnitude = Vector3.Cross(ray.direction, pt - ray.origin).magnitude;
		if (((lineStart.x > pt.x && lineEnd.x > pt.x) || (lineStart.x < pt.x && lineEnd.x < pt.x)) && ((lineStart.z > pt.z && lineEnd.z > pt.z) || (lineStart.z < pt.z && lineEnd.z < pt.z)))
		{
			return float.MaxValue;
		}
		return magnitude;
	}

	public void AddNextPostAndInters(Vector3 keyPoint, bool interpolateThisPost = true, bool doRebuild = true)
	{
		interPostPositions.Clear();
		float num = Vector3.Distance(startPoint, keyPoint);
		float num2 = interPostDist;
		if (scaleInterpolationAlso)
		{
			num2 *= gs;
		}
		if ((interpolate && num > num2) & interpolateThisPost)
		{
			int num3 = (int)Mathf.Round(num / num2);
			float num4 = 1f / (float)num3;
			float num5 = (keyPoint.x - startPoint.x) * num4;
			float num6 = (keyPoint.y - startPoint.y) * num4;
			float num7 = (keyPoint.z - startPoint.z) * num4;
			for (int i = 0; i < num3 - 1; i++)
			{
				float x = startPoint.x + num5 * (float)(i + 1);
				float y = startPoint.y + num6 * (float)(i + 1);
				float z = startPoint.z + num7 * (float)(i + 1);
				Vector3 item = new Vector3(x, y, z);
				interPostPositions.Add(item);
			}
			Ground(interPostPositions);
			allPostsPositions.AddRange(interPostPositions);
		}
		allPostsPositions.Add(keyPoint);
		if (doRebuild)
		{
			RebuildFromFinalList();
		}
	}

	private float CalculateGroundDistance(Vector3 a, Vector3 b)
	{
		a.y = 0f;
		b.y = 0f;
		return Vector3.Distance(a, b);
	}

	private void RotatePostsFinal()
	{
		if (postCounter < 2)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		if (!(posts[0] == null))
		{
			zero = posts[0].transform.position;
			zero2 = posts[1].transform.position;
			zero3 = CalculateDirection(zero, zero2);
			posts[0].transform.rotation = Quaternion.identity;
			posts[0].transform.Rotate(0f, zero3.y + 180f, 0f);
			posts[0].transform.Rotate(postRotation.x, postRotation.y, postRotation.z);
			for (int i = 1; i < postCounter - 1; i++)
			{
				zero = posts[i].transform.position;
				zero2 = posts[i - 1].transform.position;
				zero3 = CalculateDirection(zero, zero2);
				posts[i].transform.rotation = Quaternion.identity;
				posts[i].transform.Rotate(0f, zero3.y, 0f);
				float realAngle = GetRealAngle(posts[i].transform, posts[i + 1].transform);
				posts[i].transform.Rotate(0f, realAngle / 2f - 90f, 0f);
				posts[i].transform.Rotate(postRotation.x, postRotation.y, postRotation.z);
			}
			zero = posts[postCounter - 1].transform.position;
			zero2 = posts[postCounter - 2].transform.position;
			zero3 = CalculateDirection(zero, zero2);
			posts[postCounter - 1].transform.rotation = Quaternion.identity;
			posts[postCounter - 1].transform.Rotate(0f, zero3.y, 0f);
			posts[postCounter - 1].transform.Rotate(postRotation.x, postRotation.y, postRotation.z);
		}
	}

	private float GetRealAngle(Transform postA, Transform postB)
	{
		Vector3 forward = postA.forward;
		Vector3 vector = postB.position - postA.transform.position;
		float num = Vector3.Angle(vector, forward);
		float num2 = ((Vector3.Dot(vector, postA.right) > 0f) ? 1f : (-1f)) * num;
		if (num2 < 0f)
		{
			num2 += 360f;
		}
		return num2;
	}

	private void SetupPost(int n, Vector3 postPoint)
	{
		if (posts[n] == null)
		{
			return;
		}
		GameObject gameObject = posts[n].gameObject;
		bool flag = false;
		if (postNames[currentPostType] != "_None_Post")
		{
			gameObject.SetActive(value: true);
		}
		gameObject.hideFlags = HideFlags.None;
		gameObject.name = "Post " + n;
		if (clickPoints.Contains(postPoint))
		{
			gameObject.name += "_click";
			flag = true;
		}
		gameObject.layer = 8;
		float num = UnityEngine.Random.Range(0f - randomness, randomness);
		num = UnityEngine.Random.Range((0f - randomness) / 5f, randomness);
		gameObject.transform.position = postPoint;
		gameObject.transform.position += new Vector3(0f, postHeightOffset * gs, 0f);
		gameObject.transform.localScale = new Vector3(postSize.x * gs, fenceHeight * (1f + num) * postSize.y * gs, postSize.z * gs);
		int num2 = postCounter / objectsPerFolder + 1;
		string text = "postsDividedFolder" + (num2 - 1);
		GameObject gameObject2 = GameObject.Find("Current Fences Folder/Posts/" + text);
		if (gameObject2 == null)
		{
			gameObject2 = new GameObject(text);
			gameObject2.transform.parent = postsFolder.transform;
			CombineChildrenPlus combineChildrenPlus = gameObject2.AddComponent<CombineChildrenPlus>();
			if (combineChildrenPlus != null)
			{
				combineChildrenPlus.combineAtStart = true;
			}
		}
		gameObject.transform.parent = gameObject2.transform;
		if (flag)
		{
			int num3 = clickPoints.IndexOf(postPoint);
			if (num3 != -1)
			{
				GameObject obj = markers[num3].gameObject;
				obj.SetActive(value: true);
				obj.hideFlags = HideFlags.HideInHierarchy;
				Vector3 position = postPoint;
				position.y += gameObject.transform.localScale.y;
				obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				obj.transform.position = position;
				obj.name = "FenceManagerMarker_" + num3;
			}
		}
	}

	private void ThinByAngle(List<Vector3> posList)
	{
		if (removeIfLessThanAngle < 0.01f)
		{
			return;
		}
		float num = 180f;
		int num2 = -1;
		for (int i = 1; i < posList.Count - 1; i++)
		{
			Vector3 vector = posList[i] - posList[i - 1];
			Vector3 to = posList[i + 1] - posList[i];
			float num3 = Vector3.Angle(vector, to);
			if (!clickPoints.Contains(posList[i]) && num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		if (num2 != -1 && num < removeIfLessThanAngle)
		{
			posList.RemoveAt(num2);
			ThinByAngle(posList);
		}
	}

	private void ThinByDistance(List<Vector3> posList)
	{
		float num = 10000f;
		int num2 = -1;
		for (int i = 1; i < posList.Count - 1; i++)
		{
			if (IsClickPoint(posList[i]))
			{
				continue;
			}
			float num3 = Vector3.Distance(posList[i], posList[i + 1]);
			if (!(num3 < stripTooClose))
			{
				continue;
			}
			if (!IsClickPoint(posList[i + 1]))
			{
				float num4 = Vector3.Distance(posList[i], posList[i - 1]);
				float num5 = Vector3.Distance(posList[i + 1], posList[i + 2]);
				if (num4 < num5)
				{
					num = num3;
					num2 = i;
				}
				else
				{
					num = num3;
					num2 = i + 1;
				}
			}
			else
			{
				num = num3;
				num2 = i;
			}
		}
		if (num2 != -1 && num < stripTooClose)
		{
			posList.RemoveAt(num2);
			ThinByDistance(posList);
		}
	}

	private int FindClickPointIndex(Vector3 pos)
	{
		return clickPoints.IndexOf(pos);
	}

	private bool IsClickPoint(Vector3 pos)
	{
		if (clickPoints.Contains(pos))
		{
			return true;
		}
		return false;
	}

	private void MakeSplineFromClickPoints()
	{
		if (smooth && roundingDistance != 0 && clickPoints.Count >= 3)
		{
			List<Vector3> list = new List<Vector3>();
			Vector3 normalized = (clickPoints[1] - clickPoints[0]).normalized;
			Vector3 normalized2 = (clickPoints[clickPoints.Count - 1] - clickPoints[clickPoints.Count - 2]).normalized;
			if (closeLoop)
			{
				list.Add(clickPoints[clickPoints.Count - 3]);
				list.Add(clickPoints[clickPoints.Count - 2]);
			}
			else
			{
				list.Add(clickPoints[0] - 2f * normalized);
				list.Add(clickPoints[0] - 1f * normalized);
			}
			list.AddRange(clickPoints);
			if (closeLoop)
			{
				list.Add(clickPoints[1]);
				list.Add(clickPoints[2]);
			}
			else
			{
				list.Add(clickPoints[clickPoints.Count - 1] + 2f * normalized2);
				list.Add(clickPoints[clickPoints.Count - 1] + 1f * normalized2);
			}
			list = CreateCubicSpline3D(list, roundingDistance, SplineFillMode.equiDistant, tension);
			ThinByAngle(list);
			ThinByDistance(list);
			keyPoints.Clear();
			keyPoints.AddRange(list);
			Ground(keyPoints);
		}
	}

	public void Ground(List<Vector3> vec3List)
	{
		float num = 4f;
		SetIgnoreClickMarkers(inIgnore: true);
		SetIgnoreColliders(inIgnore: true);
		for (int i = 0; i < vec3List.Count; i++)
		{
			Vector3 vector;
			Vector3 origin = (vector = vec3List[i]);
			origin.y += num;
			if (Physics.Raycast(origin, Vector3.down, out var hitInfo, 500f))
			{
				if (hitInfo.collider.gameObject != null)
				{
					vector += new Vector3(0f, 0f - (hitInfo.distance - num), 0f);
				}
			}
			else if (Physics.Raycast(vector, Vector3.up, out hitInfo, 500f) && hitInfo.collider.gameObject != null)
			{
				vector += new Vector3(0f, hitInfo.distance, 0f);
			}
			vec3List[i] = vector;
		}
		SetIgnoreClickMarkers(inIgnore: false);
		SetIgnoreColliders(inIgnore: false);
	}

	public Vector3 CalculateDirection(Vector3 A, Vector3 B)
	{
		if (postCounter < 1)
		{
			return Vector3.zero;
		}
		return Quaternion.LookRotation(B - A).eulerAngles;
	}

	private List<Vector3> CreateCubicSpline3D(List<Vector3> inNodes, int numInters, SplineFillMode fillMode = SplineFillMode.fixedNumPerSpan, float tension = 0f, float bias = 0f, bool addInputNodesBackToList = true)
	{
		int count = inNodes.Count;
		if (count < 4)
		{
			return inNodes;
		}
		List<Vector3> list = new List<Vector3>((count - 1) * numInters);
		int num = numInters;
		for (int i = 2; i < count - 3; i++)
		{
			list.Add(inNodes[i]);
			Vector3 vector = inNodes[i - 1];
			Vector3 a = inNodes[i];
			Vector3 b = inNodes[i + 1];
			Vector3 vector2 = ((i >= count - 2) ? inNodes[count - 1] : inNodes[i + 2]);
			if (fillMode == SplineFillMode.equiDistant)
			{
				num = (int)Mathf.Round(Vector3.Distance(a, b) / (float)numInters);
				if (num < 1)
				{
					num = 1;
				}
			}
			float num2 = tension;
			if (IsBreakPoint(inNodes[i]) || IsBreakPoint(inNodes[i + 2]))
			{
				num2 = 1f;
			}
			for (int j = 0; j < num; j++)
			{
				float mu = 1f / ((float)num + 1f) * ((float)j + 1f);
				float x = HermiteInterpolate(vector.x, a.x, b.x, vector2.x, mu, num2, bias);
				float z = HermiteInterpolate(vector.z, a.z, b.z, vector2.z, mu, num2, bias);
				list.Add(new Vector3(x, a.y, z));
			}
		}
		if (addInputNodesBackToList)
		{
			list.Add(inNodes[count - 3]);
		}
		return list;
	}

	private float HermiteInterpolate(float y0, float y1, float y2, float y3, float mu, float tension, float bias)
	{
		float num = mu * mu;
		float num2 = num * mu;
		float num3 = (y1 - y0) * (1f + bias) * (1f - tension) / 2f;
		num3 += (y2 - y1) * (1f - bias) * (1f - tension) / 2f;
		float num4 = (y2 - y1) * (1f + bias) * (1f - tension) / 2f;
		num4 += (y3 - y2) * (1f - bias) * (1f - tension) / 2f;
		float num5 = 2f * num2 - 3f * num + 1f;
		float num6 = num2 - 2f * num + mu;
		float num7 = num2 - num;
		float num8 = -2f * num2 + 3f * num;
		return num5 * y1 + num6 * num3 + num7 * num4 + num8 * y2;
	}

	private int FindPrefabByName(FencePrefabType prefabType, string prefabName)
	{
		List<GameObject> list = postPrefabs;
		if (prefabType == FencePrefabType.railPrefab)
		{
			list = railPrefabs;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].name == prefabName)
			{
				return i;
			}
		}
		MonoBehaviour.print("Warning: Couldn't find prefab with name: " + prefabName + ". Have you deleted it, or is its mesh missing? Replace it, or reimport AutoFence to restore it.\nA default prefab will be used instead now.");
		return -1;
	}

	private string VectorToString(Vector3 vec)
	{
		return string.Concat(string.Concat("(" + vec.x.ToString("F3") + ", ", vec.y.ToString("F3"), ", "), vec.z.ToString("F3"), ")");
	}

	private Vector3 ParseVector3(string vecStr)
	{
		vecStr = vecStr.Trim('(', ')');
		string[] array = vecStr.Split(',');
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}

	public string GetPartialTimeString(bool includeDate = false)
	{
		string text = DateTime.Now.ToString();
		text = text.Replace("/", "-");
		text = text.Replace(":", "-");
		if (text.EndsWith(" AM") || text.EndsWith(" PM"))
		{
			text = text.Substring(0, text.Length - 3);
		}
		if (!includeDate)
		{
			text = text.Substring(text.Length - 8);
		}
		return text;
	}
}
