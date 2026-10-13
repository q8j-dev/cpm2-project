using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.GraphicsSettings;

namespace _Car_Parking.Scripts.Multiplayer.PhotonCustom
{
	public class VContainerPrefabPool : IPunPrefabPool
	{
		private const ulong LargeDistanceValue = 50000000000uL;

		private readonly int _maxPrefabPoolCount;

		private readonly IObjectResolver _resolver;

		private IDisposable _checkDistance;

		public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();

		private List<GameObject> CheckDistanceList = new List<GameObject>();

		public VContainerPrefabPool(IObjectResolver resolver)
		{
			_resolver = resolver;
			_maxPrefabPoolCount = (RamDevice.AllowHeavyPreload() ? 300 : 64);
			Init();
		}

		public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
		{
			GameObject value = null;
			if (!ResourceCache.TryGetValue(prefabId, out value))
			{
				value = Resources.Load<GameObject>(prefabId);
				if (value == null)
				{
					Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
				}
				else
				{
					if (ResourceCache.Count >= _maxPrefabPoolCount)
					{
						Clear();
					}
					ResourceCache.Add(prefabId, value);
				}
			}
			bool activeSelf = value.activeSelf;
			if (activeSelf)
			{
				value.SetActive(value: false);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(value, position, rotation);
			_resolver.InjectGameObject(gameObject);
			if (activeSelf)
			{
				value.SetActive(value: true);
			}
			AddToCheckLargeDistanceList(gameObject);
			return gameObject;
		}

		public async UniTask<GameObject> InstantiateAsync(string prefabId, Vector3 position, Quaternion rotation)
		{
			GameObject value = null;
			if (!ResourceCache.TryGetValue(prefabId, out value))
			{
				ResourceRequest loadResource = Resources.LoadAsync<GameObject>(prefabId);
				await loadResource;
				value = loadResource.asset as GameObject;
				if (value == null)
				{
					Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
					return null;
				}
				if (ResourceCache.Count >= _maxPrefabPoolCount)
				{
					Clear();
				}
				ResourceCache.Add(prefabId, value);
			}
			bool activeSelf = value.activeSelf;
			if (activeSelf)
			{
				value.SetActive(value: false);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(value, position, rotation);
			_resolver.InjectGameObject(gameObject);
			if (activeSelf)
			{
				value.SetActive(value: true);
			}
			AddToCheckLargeDistanceList(gameObject);
			return gameObject;
		}

		public async UniTask WarmShellsAsync(IReadOnlyList<string> prefabIds, CancellationToken ct)
		{
			if (prefabIds == null)
			{
				return;
			}
			for (int i = 0; i < prefabIds.Count; i++)
			{
				if (ct.IsCancellationRequested)
				{
					break;
				}
				string prefabId = prefabIds[i];
				if (!string.IsNullOrEmpty(prefabId) && !ResourceCache.ContainsKey(prefabId))
				{
					ResourceRequest op = Resources.LoadAsync<GameObject>(prefabId);
					await op;
					if (ct.IsCancellationRequested)
					{
						break;
					}
					if (op.asset is GameObject value && !ResourceCache.ContainsKey(prefabId))
					{
						ResourceCache.Add(prefabId, value);
					}
				}
			}
		}

		public void WarmInjectionGraph(string prefabId)
		{
			if (!string.IsNullOrEmpty(prefabId) && ResourceCache.TryGetValue(prefabId, out var value) && (bool)value)
			{
				bool activeSelf = value.activeSelf;
				if (activeSelf)
				{
					value.SetActive(value: false);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(value);
				if (activeSelf)
				{
					value.SetActive(value: true);
				}
				_resolver.InjectGameObject(gameObject);
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		public void Destroy(GameObject gameObject)
		{
			if (gameObject != null)
			{
				CheckDistanceList.Remove(gameObject);
			}
			UnityEngine.Object.Destroy(gameObject);
		}

		public void ClearResources(bool toDestroy = false)
		{
			_checkDistance?.Dispose();
			if (toDestroy)
			{
				foreach (GameObject checkDistance in CheckDistanceList)
				{
					UnityEngine.Object.Destroy(checkDistance);
				}
			}
			CheckDistanceList.Clear();
			ResourceCache.Clear();
		}

		private void AddToCheckLargeDistanceList(GameObject instance)
		{
			CheckDistanceList.Add(instance);
		}

		private void CheckDistance()
		{
			for (int i = 0; i < CheckDistanceList.Count; i++)
			{
				if ((bool)CheckDistanceList[i])
				{
					Transform transform = CheckDistanceList[i].transform;
					if (transform.position.sqrMagnitude > 5E+10f)
					{
						transform.position = Vector3.zero;
					}
				}
			}
		}

		private void Init()
		{
			_checkDistance?.Dispose();
			_checkDistance = Observable.Interval(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
			{
				CheckDistance();
			});
		}

		private void Clear()
		{
			ResourceCache.Clear();
		}
	}
}
