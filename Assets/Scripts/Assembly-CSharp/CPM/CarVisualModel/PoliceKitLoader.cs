using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;

namespace CPM.CarVisualModel
{
	public static class PoliceKitLoader
	{
		public static async UniTask<List<AsyncOperationHandle>> LoadAsync(int carId, GameObject root, PoliceData police, PoliceSystemConfig config, CancellationToken token)
		{
			List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();
			if (police == null || !police.Installed || config == null || config.PoliceCarConfigs == null)
			{
				return handles;
			}
			PoliceCarBodyKitsConfig carConfig = config.PoliceCarConfigs.GetConfig(carId);
			if (!carConfig)
			{
				return handles;
			}
			List<PoliceKitRequest> requests = PolicePlan.BuildLoadList(police);
			for (int i = 0; i < requests.Count; i++)
			{
				PoliceKitRequest policeKitRequest = requests[i];
				try
				{
					await LoadKitAsync(policeKitRequest.Type, policeKitRequest.Index, carId, root.transform, carConfig, config, handles, token);
				}
				catch (OperationCanceledException)
				{
					ReleaseAll(handles);
					throw;
				}
				catch (Exception)
				{
				}
			}
			return handles;
		}

		private static async UniTask LoadKitAsync(PoliceBodyKitType type, int index, int carId, Transform root, PoliceCarBodyKitsConfig carConfig, PoliceSystemConfig config, List<AsyncOperationHandle> handles, CancellationToken token)
		{
			PoliceCarBodyKitsConfig.BodyKitData bodyKitData = carConfig.GetBodyKit(type, index);
			if (bodyKitData == null)
			{
				return;
			}
			PoliceBodyKitSettingsConfig bodyKitsSettingsConfig = config.GetBodyKitsSettingsConfig(type);
			if (!bodyKitsSettingsConfig)
			{
				return;
			}
			string addressablePath = bodyKitsSettingsConfig.GetAddressablePath(index);
			if (string.IsNullOrEmpty(addressablePath) || !AddressableExtensions.AddressableResourceExists(addressablePath, typeof(GameObject)))
			{
				return;
			}
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(addressablePath);
			handles.Add(handle);
			GameObject gameObject = await handle.ToUniTask(null, PlayerLoopTiming.Update, token);
			if (!gameObject)
			{
				handles.Remove(handle);
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, root);
				TransformData transformData = bodyKitData.TransformData;
				gameObject2.transform.localPosition = transformData.Position;
				gameObject2.transform.localRotation = transformData.Rotation;
				gameObject2.transform.localScale = transformData.Scale;
			}
		}

		private static void ReleaseAll(List<AsyncOperationHandle> handles)
		{
			for (int i = 0; i < handles.Count; i++)
			{
				if (handles[i].IsValid())
				{
					Addressables.Release(handles[i]);
				}
			}
		}
	}
}
