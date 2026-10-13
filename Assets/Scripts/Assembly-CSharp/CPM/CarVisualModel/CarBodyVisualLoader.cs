using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CPM.CarVisualModel
{
	public static class CarBodyVisualLoader
	{
		public static string InstanceName(int carId)
		{
			return $"b{carId}(Clone)";
		}

		public static async UniTask<LoadedBody> LoadAsync(int carId, Transform parent, Vector3 anchorLocalPos, CancellationToken token)
		{
			string key = $"body/b{carId}.prefab";
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
			GameObject gameObject;
			try
			{
				gameObject = await handle.ToUniTask(null, PlayerLoopTiming.Update, token);
			}
			catch
			{
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
				throw;
			}
			if (!gameObject)
			{
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
				return new LoadedBody(null, default, valid: false);
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, parent);
			gameObject2.name = InstanceName(carId);
			gameObject2.transform.localPosition = anchorLocalPos;
			gameObject2.transform.localRotation = Quaternion.identity;
			return new LoadedBody(gameObject2, handle, valid: true);
		}
	}
}
