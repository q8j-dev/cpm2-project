using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[Preserve]
	public class CPMCrowdManager : ICPMCrowdManager, IInitializable, IDisposable
	{
		private readonly Dictionary<int, Stack<CPMCrowdNpc>> _pool = new Dictionary<int, Stack<CPMCrowdNpc>>();

		private readonly List<CPMCrowdNpc> _activeNpcs = new List<CPMCrowdNpc>();

		private readonly CPMCrowdNpcCatalogSO _npcCatalog;

		private readonly IPrefabFactory _prefabFactory;

		private Transform _poolRoot;

		public CPMCrowdManager(CPMCrowdNpcCatalogSO npcCatalog, IPrefabFactory prefabFactory)
		{
			_npcCatalog = npcCatalog;
			_prefabFactory = prefabFactory;
		}

		public void Initialize()
		{
		}

		public CPMCrowdNpc Spawn(CPMCrowdNpcData data)
		{
			CPMCrowdNpc cPMCrowdNpc = TakeFromPool(data.PrefabIndex);
			if (!cPMCrowdNpc)
			{
				return null;
			}
			Transform transform = cPMCrowdNpc.transform;
			transform.SetParent(null, worldPositionStays: false);
			transform.SetPositionAndRotation(data.Position, data.Rotation);
			transform.localScale = data.Scale;
			cPMCrowdNpc.gameObject.SetActive(value: true);
			cPMCrowdNpc.OnSpawn(data.PrefabIndex);
			_activeNpcs.Add(cPMCrowdNpc);
			return cPMCrowdNpc;
		}

		public void SpawnAll(IReadOnlyList<CPMCrowdNpcData> datas)
		{
			if (datas != null)
			{
				for (int i = 0; i < datas.Count; i++)
				{
					Spawn(datas[i]);
				}
			}
		}

		public void Despawn(CPMCrowdNpc npc)
		{
			if ((bool)npc && _activeNpcs.Remove(npc))
			{
				ReturnToPool(npc);
			}
		}

		public void DespawnAll()
		{
			for (int num = _activeNpcs.Count - 1; num >= 0; num--)
			{
				ReturnToPool(_activeNpcs[num]);
			}
			_activeNpcs.Clear();
		}

		public void Dispose()
		{
			_activeNpcs.Clear();
			_pool.Clear();
			if ((bool)_poolRoot)
			{
				UnityEngine.Object.Destroy(_poolRoot.gameObject);
				_poolRoot = null;
			}
		}

		private CPMCrowdNpc TakeFromPool(int prefabIndex)
		{
			if (_pool.TryGetValue(prefabIndex, out var value) && value.Count > 0)
			{
				return value.Pop();
			}
			CPMCrowdNpc prefab = _npcCatalog.GetPrefab(prefabIndex);
			if (!prefab)
			{
				Debug.LogError($"[CPMCrowdManager] No crowd NPC prefab for index {prefabIndex}.");
				return null;
			}
			return _prefabFactory.Instantiate(prefab, Vector3.zero, Quaternion.identity, GetPoolRoot());
		}

		private void ReturnToPool(CPMCrowdNpc npc)
		{
			npc.OnDespawn();
			npc.gameObject.SetActive(value: false);
			npc.transform.SetParent(GetPoolRoot(), worldPositionStays: false);
			if (!_pool.TryGetValue(npc.PrefabIndex, out var value))
			{
				value = new Stack<CPMCrowdNpc>();
				_pool[npc.PrefabIndex] = value;
			}
			value.Push(npc);
		}

		private Transform GetPoolRoot()
		{
			if (!_poolRoot)
			{
				_poolRoot = new GameObject("CrowdPool").transform;
			}
			return _poolRoot;
		}
	}
}
