using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[Preserve]
	public class RallyPropManager : IRallyPropManager, IInitializable, IDisposable
	{
		private readonly Dictionary<int, Stack<PooledProp>> _pool = new Dictionary<int, Stack<PooledProp>>();

		private readonly List<PooledProp> _active = new List<PooledProp>();

		private readonly RallyPropCatalogSO _catalog;

		private readonly IPrefabFactory _prefabFactory;

		private Transform _poolRoot;

		public RallyPropManager(RallyPropCatalogSO catalog, IPrefabFactory prefabFactory)
		{
			_catalog = catalog;
			_prefabFactory = prefabFactory;
		}

		public void Initialize()
		{
		}

		public PooledProp Spawn(RallyPropData data)
		{
			PooledProp pooledProp = TakeFromPool(data.PrefabIndex);
			if (!pooledProp)
			{
				return null;
			}
			Transform transform = pooledProp.transform;
			transform.SetParent(null, worldPositionStays: false);
			transform.SetPositionAndRotation(data.Position, data.Rotation);
			transform.localScale = data.Scale;
			pooledProp.gameObject.SetActive(value: true);
			_active.Add(pooledProp);
			return pooledProp;
		}

		public void SpawnAll(IReadOnlyList<RallyPropData> datas)
		{
			if (datas != null)
			{
				for (int i = 0; i < datas.Count; i++)
				{
					Spawn(datas[i]);
				}
			}
		}

		public void Despawn(PooledProp prop)
		{
			if ((bool)prop && _active.Remove(prop))
			{
				ReturnToPool(prop);
			}
		}

		public void DespawnAll()
		{
			for (int num = _active.Count - 1; num >= 0; num--)
			{
				ReturnToPool(_active[num]);
			}
			_active.Clear();
		}

		public void Dispose()
		{
			_active.Clear();
			_pool.Clear();
			if ((bool)_poolRoot)
			{
				UnityEngine.Object.Destroy(_poolRoot.gameObject);
				_poolRoot = null;
			}
		}

		private PooledProp TakeFromPool(int prefabIndex)
		{
			if (_pool.TryGetValue(prefabIndex, out var value) && value.Count > 0)
			{
				return value.Pop();
			}
			GameObject prefab = _catalog.GetPrefab(prefabIndex);
			if (!prefab)
			{
				Debug.LogError($"[RallyPropManager] No prop prefab for index {prefabIndex}.");
				return null;
			}
			GameObject gameObject = _prefabFactory.Instantiate(prefab, Vector3.zero, Quaternion.identity, GetPoolRoot());
			PooledProp pooledProp = gameObject.GetComponent<PooledProp>();
			if (!pooledProp)
			{
				pooledProp = gameObject.AddComponent<PooledProp>();
			}
			pooledProp.SetIndex(prefabIndex);
			return pooledProp;
		}

		private void ReturnToPool(PooledProp prop)
		{
			prop.gameObject.SetActive(value: false);
			prop.transform.SetParent(GetPoolRoot(), worldPositionStays: false);
			if (!_pool.TryGetValue(prop.PrefabIndex, out var value))
			{
				value = new Stack<PooledProp>();
				_pool[prop.PrefabIndex] = value;
			}
			value.Push(prop);
		}

		private Transform GetPoolRoot()
		{
			if (!_poolRoot)
			{
				_poolRoot = new GameObject("RallyPropPool").transform;
			}
			return _poolRoot;
		}
	}
}
