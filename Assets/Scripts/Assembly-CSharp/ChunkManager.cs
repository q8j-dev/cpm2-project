using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking;

public class ChunkManager : MonoBehaviour
{
	public Vector2 chunkSize = new Vector2(10f, 10f);

	public int rows = 10;

	public int columns = 10;

	public Transform origin;

	public List<Chunk> chunks = new List<Chunk>();

	public Vector2 lastPos;

	public int defaultState = 2;

	public GameObject allChunkParent;

	public int allObjectCount;

	public List<GameObject> ObjectsToIgnore;

	public List<GameObject> AlwaysIncludeInChunk = new List<GameObject>();

	private Quaternion lastRotation;

	private bool _isDestroyed;

	public int place;

	public static bool DisableChunkUpdates;

	private Camera mainCamera;

	public static bool IsHighQuality => CPMGraphicsSettingsManager.Instance.Quality > 1;

	private void Start()
	{
		if (FlagsStorage.IsLevelScene)
		{
			Debug.Log("Chunks:Is Level so return");
			return;
		}
		ObscuredPrefs.GetInt("chunks", 1);
		if (1 == 0)
		{
			Debug.Log("Chunk: is 0 so return");
			return;
		}
		Debug.Log("Chunk: is 1. So enable.");
		mainCamera = V.CoreGameManager.GetComponent<Camera>();
		UpdateState().Forget();
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		CommonExtensions.ClearGOCollection(ObjectsToIgnore);
		CommonExtensions.ClearGOCollection(chunks);
		AlwaysIncludeInChunk?.Clear();
		mainCamera = null;
		origin = null;
		allChunkParent = null;
	}

	private async UniTaskVoid UpdateState()
	{
		await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		while (true)
		{
			await UniTask.Delay(500, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (_isDestroyed)
			{
				break;
			}
			if (!DisableChunkUpdates && (bool)V.CoreGameManager)
			{
				Transform transform = V.CoreGameManager.transform;
				UpdateChunks(transform.position, transform.forward, transform.rotation);
			}
		}
	}

	public async UniTask CreateChunksAll()
	{
		if (allChunkParent == null)
		{
			allChunkParent = new GameObject("all chunks");
		}
		allChunkParent.transform.localPosition = Vector3.zero;
		allChunkParent.transform.SetParent(base.transform);
		allObjectCount = 0;
		foreach (Chunk chunk in chunks)
		{
			if (!chunk)
			{
				continue;
			}
			foreach (GameObject item in chunk.allGOinChunk)
			{
				if ((bool)item)
				{
					item.SetActive(value: true);
				}
			}
			Object.DestroyImmediate(chunk.gameObject);
		}
		chunks.Clear();
		foreach (GameObject item2 in ObjectsToIgnore)
		{
			if ((bool)item2)
			{
				item2.SetActive(value: false);
			}
		}
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				GameObject gameObject = new GameObject($"Chunk_{i}_{j}");
				gameObject.transform.parent = allChunkParent.transform;
				Chunk chunkComponent = gameObject.AddComponent<Chunk>();
				Vector3 vector = new Vector3((float)i * chunkSize.x, 0f, (float)j * chunkSize.y) + origin.position;
				chunkComponent.transform.position = vector;
				chunkComponent.Center = vector;
				chunkComponent.Size = chunkSize;
				GameObject[] allObjectsInScene = Object.FindObjectsOfType<GameObject>();
				await chunkComponent.CollectFromAllGameObjects(allObjectsInScene, ObjectsToIgnore, AlwaysIncludeInChunk);
				chunks.Add(chunkComponent);
				allObjectCount += chunkComponent.allGOinChunk.Count;
			}
		}
		foreach (Chunk chunk2 in chunks)
		{
			if (chunk2.allGOinChunk.Count == 0)
			{
				Object.DestroyImmediate(chunk2.gameObject);
			}
		}
		chunks.RemoveAll((Chunk item) => item == null);
		foreach (GameObject item3 in ObjectsToIgnore)
		{
			if ((bool)item3)
			{
				item3.SetActive(value: true);
			}
		}
		Debug.Log($"Created {chunks.Count} chunks with {allObjectCount} total objects");
	}

	public void UpdateChunks(Vector3 cameraPosition, Vector3 cameraForward, Quaternion cameraRotation)
	{
		Vector2 b = new Vector2(cameraPosition.x, cameraPosition.z);
		if (Vector2.Distance(lastPos, b) < 20f)
		{
			return;
		}
		lastPos = b;
		lastRotation = cameraRotation;
		foreach (Chunk chunk in chunks)
		{
			if ((bool)chunk)
			{
				chunk.UpdateState(cameraPosition, cameraForward, defaultState, mainCamera);
			}
		}
	}
}
