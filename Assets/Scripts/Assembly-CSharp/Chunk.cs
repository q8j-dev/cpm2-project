using System.Collections.Generic;
using System.Threading;
using CPM.Chunking;
using CPM.NightLighting;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public Vector3 Center;

	public Vector2 Size;

	public float activationDistance = 15f;

	public List<GameObject> allGOinChunk = new List<GameObject>();

	[Header("Collection Settings")]
	[Tooltip("Layers to ignore when collecting objects")]
	public LayerMask ignoreLayers;

	[Tooltip("Objects that will always be added to this chunk")]
	public List<GameObject> alwaysInclude = new List<GameObject>();

	public bool lastState = true;

	private bool _isDestroyed;

	private CancellationTokenSource _ctsUpdateState;

	private bool _updateStateWasCompleted;

	public Color chunkColor = Color.green;

	[HideInInspector]
	public bool showGizmos = true;

	private void Awake()
	{
		activationDistance = Size.x;
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
		allGOinChunk.Clear();
		alwaysInclude.Clear();
		CommonExtensions.CancelAndDisposeToken(ref _ctsUpdateState);
	}

	public async UniTask CollectFromAllGameObjects(GameObject[] allObjectsInScene, List<GameObject> objectsToIgnore = null, List<GameObject> managerAlwaysInclude = null)
	{
		await UniTask.Delay(1, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		if (_isDestroyed)
		{
			return;
		}
		allGOinChunk.Clear();
		int num = 0;
		foreach (GameObject item in alwaysInclude)
		{
			if (item != null && !allGOinChunk.Contains(item))
			{
				allGOinChunk.Add(item);
				num++;
			}
		}
		if (managerAlwaysInclude != null)
		{
			foreach (GameObject item2 in managerAlwaysInclude)
			{
				if (item2 != null && !allGOinChunk.Contains(item2) && IsInsideBounds(item2.transform.position))
				{
					allGOinChunk.Add(item2);
					num++;
				}
			}
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		if (objectsToIgnore != null)
		{
			foreach (GameObject item3 in objectsToIgnore)
			{
				if (!(item3 == null))
				{
					hashSet.Add(item3);
					Transform[] componentsInChildren = item3.GetComponentsInChildren<Transform>(includeInactive: true);
					foreach (Transform transform in componentsInChildren)
					{
						hashSet.Add(transform.gameObject);
					}
				}
			}
		}
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in allObjectsInScene)
		{
			if (allGOinChunk.Contains(gameObject) || !gameObject.activeSelf || hashSet.Contains(gameObject) || (bool)gameObject.GetComponent<VehicleData>() || (bool)gameObject.GetComponent<MainPerson>() || gameObject.TryGetComponent<EnableOnNight>(out var _) || gameObject.layer == LayerMask.NameToLayer("Road") || gameObject.layer == LayerMask.NameToLayer("map") || ((int)ignoreLayers != 0 && ((1 << gameObject.layer) & (int)ignoreLayers) != 0))
			{
				continue;
			}
			MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
			Collider component3 = gameObject.GetComponent<Collider>();
			ParticleSystem component4 = gameObject.GetComponent<ParticleSystem>();
			if (component2 == null && component3 == null && component4 == null)
			{
				continue;
			}
			float num2 = 0f;
			if (component2 != null)
			{
				Bounds bounds = component2.bounds;
				num2 = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
			}
			else if (component3 != null)
			{
				Bounds bounds2 = component3.bounds;
				num2 = Mathf.Max(bounds2.size.x, bounds2.size.y, bounds2.size.z);
			}
			if (!(num2 > Size.x) || !(component4 == null))
			{
				Vector3 position = ((component2 != null) ? component2.bounds.center : ((!(component3 != null)) ? gameObject.transform.position : component3.bounds.center));
				if (IsInsideBounds(position))
				{
					list.Add(gameObject);
				}
			}
		}
		HashSet<GameObject> hashSet2 = new HashSet<GameObject>(list);
		foreach (GameObject item4 in list)
		{
			bool flag = false;
			Transform parent = item4.transform.parent;
			while (parent != null)
			{
				if (hashSet2.Contains(parent.gameObject))
				{
					flag = true;
					break;
				}
				parent = parent.parent;
			}
			if (!flag)
			{
				allGOinChunk.Add(item4);
			}
		}
		MonoBehaviour.print($"Chunk {base.name}: collected {allGOinChunk.Count} objects ({num} always-include)");
	}

	public bool IsInsideBounds(Vector3 position)
	{
		float num = Size.x * 0.5f;
		float num2 = Size.y * 0.5f;
		if (position.x >= Center.x - num && position.x <= Center.x + num && position.z >= Center.z - num2)
		{
			return position.z <= Center.z + num2;
		}
		return false;
	}

	public void RemoveGameObject(GameObject go)
	{
		if (!(go == null))
		{
			allGOinChunk.Remove(go);
		}
	}

	public bool AddGameObject(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		if (!allGOinChunk.Contains(go))
		{
			allGOinChunk.Add(go);
			return true;
		}
		return false;
	}

	public bool ContainsGameObject(GameObject go)
	{
		if (go != null)
		{
			return allGOinChunk.Contains(go);
		}
		return false;
	}

	public void CleanupNullReferences()
	{
		allGOinChunk.RemoveAll((GameObject g) => g == null);
	}

	public void UpdateState(Vector3 cameraPosition, Vector3 cameraForward, int forceState, Camera mainCamera)
	{
		if (_isDestroyed)
		{
			return;
		}
		bool flag = ChunkVisibilityRule.ShouldEnable(forceState, base.transform.position, cameraPosition, mainCamera.farClipPlane, Size.x);
		if (flag == lastState && _updateStateWasCompleted)
		{
			return;
		}
		lastState = flag;
		if (flag)
		{
			CommonExtensions.CancelAndDisposeToken(ref _ctsUpdateState);
			foreach (GameObject item in allGOinChunk)
			{
				if ((bool)item && !item.activeSelf)
				{
					item.SetActive(value: true);
				}
			}
			_updateStateWasCompleted = true;
		}
		else
		{
			CommonExtensions.CancelAndDisposeToken(ref _ctsUpdateState);
			_ctsUpdateState = new CancellationTokenSource();
			_updateStateWasCompleted = false;
			DeactivateAsync(_ctsUpdateState.Token).Forget();
		}
	}

	private async UniTask DeactivateAsync(CancellationToken cancellationToken)
	{
		int num = 0;
		foreach (GameObject item in allGOinChunk)
		{
			if (_isDestroyed)
			{
				return;
			}
			if ((bool)item)
			{
				if (item.activeSelf)
				{
					item.SetActive(value: false);
					num++;
				}
				if (num > 20)
				{
					await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken);
					num = 0;
				}
			}
		}
		_updateStateWasCompleted = true;
	}
}
