using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class ReflectionCubemapLoader
{
	private string activeGuid;

	private string pendingGuid;

	private string prefetchGuid;

	private AsyncOperationHandle<Cubemap> activeHandle;

	private AsyncOperationHandle<Cubemap> pendingHandle;

	private AsyncOperationHandle<Cubemap> prefetchHandle;

	private HashSet<string> failedGuids;

	public Texture ActiveTexture
	{
		get
		{
			if (!activeHandle.IsValid() || activeHandle.Status != AsyncOperationStatus.Succeeded)
			{
				return null;
			}
			return activeHandle.Result;
		}
	}

	public Texture Request(AssetReferenceT<Cubemap> reference, out bool isRequested)
	{
		isRequested = false;
		if (!WeatherPreset.IsValidRef(reference))
		{
			return ActiveTexture;
		}
		string assetGUID = reference.AssetGUID;
		if (assetGUID == activeGuid)
		{
			RecordIfFailed(pendingGuid, in pendingHandle);
			ReleaseSlot(ref pendingGuid, ref pendingHandle);
			isRequested = true;
			return ActiveTexture;
		}
		if (failedGuids != null && failedGuids.Contains(assetGUID))
		{
			return ActiveTexture;
		}
		if (assetGUID != pendingGuid)
		{
			RecordIfFailed(pendingGuid, in pendingHandle);
			ReleaseSlot(ref pendingGuid, ref pendingHandle);
			if (assetGUID == prefetchGuid)
			{
				pendingGuid = prefetchGuid;
				pendingHandle = prefetchHandle;
				prefetchGuid = null;
				prefetchHandle = default;
			}
			else
			{
				pendingGuid = assetGUID;
				pendingHandle = Addressables.LoadAssetAsync<Cubemap>(reference.RuntimeKey);
			}
		}
		if (pendingHandle.IsValid() && pendingHandle.IsDone)
		{
			if (pendingHandle.Status == AsyncOperationStatus.Succeeded && pendingHandle.Result != null)
			{
				ReleaseSlot(ref activeGuid, ref activeHandle);
				activeGuid = pendingGuid;
				activeHandle = pendingHandle;
				pendingGuid = null;
				pendingHandle = default;
			}
			else
			{
				RecordIfFailed(pendingGuid, in pendingHandle);
				ReleaseSlot(ref pendingGuid, ref pendingHandle);
			}
		}
		isRequested = assetGUID == activeGuid;
		return ActiveTexture;
	}

	public void Prefetch(AssetReferenceT<Cubemap> reference)
	{
		if (!WeatherPreset.IsValidRef(reference))
		{
			return;
		}
		string assetGUID = reference.AssetGUID;
		if (!(assetGUID == activeGuid) && !(assetGUID == pendingGuid) && !(assetGUID == prefetchGuid))
		{
			RecordIfFailed(prefetchGuid, in prefetchHandle);
			ReleaseSlot(ref prefetchGuid, ref prefetchHandle);
			if (failedGuids == null || !failedGuids.Contains(assetGUID))
			{
				prefetchGuid = assetGUID;
				prefetchHandle = Addressables.LoadAssetAsync<Cubemap>(reference.RuntimeKey);
			}
		}
	}

	public void ReleaseActive()
	{
		ReleaseSlot(ref activeGuid, ref activeHandle);
	}

	public void ReleaseTransient()
	{
		RecordIfFailed(pendingGuid, in pendingHandle);
		ReleaseSlot(ref pendingGuid, ref pendingHandle);
		RecordIfFailed(prefetchGuid, in prefetchHandle);
		ReleaseSlot(ref prefetchGuid, ref prefetchHandle);
	}

	public void ReleaseAll()
	{
		ReleaseSlot(ref activeGuid, ref activeHandle);
		ReleaseSlot(ref pendingGuid, ref pendingHandle);
		ReleaseSlot(ref prefetchGuid, ref prefetchHandle);
		failedGuids = null;
	}

	private void RecordIfFailed(string guid, in AsyncOperationHandle<Cubemap> handle)
	{
		if (guid != null && handle.IsValid() && handle.IsDone && (handle.Status != AsyncOperationStatus.Succeeded || !(handle.Result != null)))
		{
			Debug.LogError("ReflectionCubemapLoader: reflection cubemap " + guid + " failed to load, keeping the previous cubemap. Are the cubemaps addressable and the Addressables content built? " + handle.OperationException);
			(failedGuids ?? (failedGuids = new HashSet<string>())).Add(guid);
		}
	}

	private static void ReleaseSlot(ref string guid, ref AsyncOperationHandle<Cubemap> handle)
	{
		if (handle.IsValid())
		{
			Addressables.Release(handle);
		}
		guid = null;
		handle = default;
	}
}
