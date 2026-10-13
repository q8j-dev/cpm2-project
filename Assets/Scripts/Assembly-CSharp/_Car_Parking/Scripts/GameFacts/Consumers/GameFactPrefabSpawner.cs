using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.GameFacts.Consumers
{
	public class GameFactPrefabSpawner : MonoBehaviour
	{
		[SerializeField]
		private ConditionState State = default;

		[Tooltip("Addressable key/path of the prefab to spawn on this transform.")]
		[SerializeField]
		private string AddressableKey = default;

		[SerializeField]
		private bool CanDespawn = true;

		[Tooltip("Seconds to keep the instance alive after the condition stops holding. 0 — destroy immediately.")]
		[SerializeField]
		private float DespawnDelay = default;

		private LoadAndSpawnPrefab _loader;

		private GameObject _instance;

		private CancellationTokenSource _cts;

		private CancellationTokenSource _despawnCts;

		private int _generation;

		private bool IsActive => State?.Current ?? false;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_loader = new LoadAndSpawnPrefab(prefabFactory);
		}

		private void Start()
		{
			if ((bool)State)
			{
				State.OnStatusChanged += OnStateStatusChanged;
			}
			Apply();
		}

		private void OnDestroy()
		{
			if ((bool)State)
			{
				State.OnStatusChanged -= OnStateStatusChanged;
			}
			Despawn();
			_loader?.Dispose();
		}

		private void OnStateStatusChanged(bool met)
		{
			Apply();
		}

		private void Apply()
		{
			if (!State)
			{
				return;
			}
			if (IsActive)
			{
				BaseCommonExtensions.CancelAndDisposeToken(ref _despawnCts);
				if (!_instance && _cts == null)
				{
					SpawnAsync().Forget();
				}
			}
			else if (CanDespawn && _despawnCts == null)
			{
				if (DespawnDelay > 0f)
				{
					DespawnDelayedAsync().Forget();
				}
				else
				{
					Despawn();
				}
			}
		}

		private async UniTaskVoid DespawnDelayedAsync()
		{
			_despawnCts = new CancellationTokenSource();
			if (!(await UniTask.Delay(TimeSpan.FromSeconds(DespawnDelay), ignoreTimeScale: false, PlayerLoopTiming.Update, _despawnCts.Token).SuppressCancellationThrow()))
			{
				Despawn();
			}
		}

		private async UniTaskVoid SpawnAsync()
		{
			int gen = ++_generation;
			_cts = new CancellationTokenSource();
			TransformData localTransformData = new TransformData(Vector3.zero, Quaternion.identity, Vector3.one);
			GameObject gameObject = await _loader.Spawn(AddressableKey, AddressableKey, localTransformData, base.transform, _cts.Token);
			if (gen != _generation)
			{
				if ((bool)gameObject)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			else if (!IsActive && _despawnCts == null)
			{
				if ((bool)gameObject)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			}
			else if (!gameObject)
			{
				Debug.LogError("GameFactPrefabSpawner on '" + base.name + "': failed to load '" + AddressableKey + "'", this);
				BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			}
			else
			{
				_instance = gameObject;
				BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			}
		}

		private void Despawn()
		{
			_generation++;
			BaseCommonExtensions.CancelAndDisposeToken(ref _despawnCts);
			BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			if ((bool)_instance)
			{
				UnityEngine.Object.Destroy(_instance);
				_instance = null;
			}
		}
	}
}
