using System;
using System.Collections.Generic;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class LoadAndSpawnPrefab : IDisposable
	{
		private IPrefabFactory _prefabFactory;

		private readonly Dictionary<string, CancellationTokenSource> _cancellations = new Dictionary<string, CancellationTokenSource>();

		public LoadAndSpawnPrefab(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		public void Dispose()
		{
			foreach (KeyValuePair<string, CancellationTokenSource> cancellation in _cancellations)
			{
				cancellation.Value?.Cancel();
				cancellation.Value?.Dispose();
			}
			_cancellations.Clear();
		}

		public async UniTask<GameObject> Spawn(string key, string path, TransformData localTransformData, Transform parent, CancellationToken cancellationTokenGlobal = default(CancellationToken))
		{
			if (_cancellations.TryGetValue(key, out var value))
			{
				value?.Cancel();
				value?.Dispose();
			}
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			_cancellations[key] = cancellationTokenSource;
			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationTokenGlobal);
			return await Spawn(path, parent, localTransformData, linkedCts.Token);
		}

		private async UniTask<GameObject> Spawn(string path, Transform parent, TransformData localTransformData, CancellationToken cancellationToken)
		{
			if (!AddressableExtensions.AddressableResourceExists(path, typeof(GameObject)))
			{
				Debug.LogError("Addressable " + path + " not found");
				return null;
			}
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(path);
			try
			{
				GameObject gameObject = await handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
				if (!gameObject || !parent)
				{
					Addressables.Release(handle);
					return null;
				}
				GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, parent);
				gameObject2.transform.localPosition = localTransformData.Position;
				gameObject2.transform.localRotation = localTransformData.Rotation;
				gameObject2.transform.localScale = localTransformData.Scale;
				_ = handle.BindTo(gameObject2);
				return gameObject2;
			}
			catch (Exception)
			{
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
				return null;
			}
		}
	}
}
