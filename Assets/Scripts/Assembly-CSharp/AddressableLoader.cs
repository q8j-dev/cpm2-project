using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableLoader
{
	public struct CachedHandle
	{
		public AsyncOperationHandle Handle;

		public HashSet<GameObject> Targets;
	}

	private static Dictionary<string, CachedHandle> Handles = new Dictionary<string, CachedHandle>();

	public static async UniTask<Addressable<T>> LoadAssetAsync<T>(string path)
	{
		SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
		Addressable<T> addressable = new Addressable<T>();
		await semaphoreSlim.WaitAsync();
		return addressable;
	}

	public static async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken token, GameObject target)
	{
		if (!target)
		{
			Debug.Log("AddressableLoader.LoadAssetAsync: null target for key '" + key + "'. Skipping load to avoid leaking handle.");
			return default;
		}
		AsyncOperationHandle handle;
		if (Handles.TryGetValue(key, out var value) && value.Handle.IsValid())
		{
			handle = value.Handle;
		}
		else
		{
			if (Handles.ContainsKey(key))
			{
				Handles.Remove(key);
			}
			handle = Addressables.LoadAssetAsync<T>(key);
			Handles.Add(key, new CachedHandle
			{
				Handle = handle,
				Targets = new HashSet<GameObject>()
			});
		}
		AddTarget(key, target);
		await handle.Task.AsUniTask().AttachExternalCancellation(token);
		if (!handle.IsValid())
		{
			throw new OperationCanceledException(token);
		}
		return (T)handle.Result;
	}

	public static void AddTarget(string key, GameObject target)
	{
		if ((bool)target && Handles[key].Targets.Add(target))
		{
			target.OnDestroyAsObservable().Subscribe(TryReleaseHandle);
		}
		void TryReleaseHandle(Unit _)
		{
			if (Handles.TryGetValue(key, out var value))
			{
				value.Targets.Remove(target);
				if (value.Targets.Count != 0)
				{
					return;
				}
				if (value.Handle.IsValid())
				{
					value.Handle.Release();
				}
			}
			Handles.Remove(key);
		}
	}

	private static void LoadAsync<T>()
	{
	}

	private static void Load<T>()
	{
	}
}
