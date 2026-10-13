using Addler.Runtime.Core.LifetimeBinding;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking.Scripts.Car;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	public static class SteamPreviewInteriorLoader
	{
		public static async UniTask LoadInto(GameObject body, CarBodyReferences refs, int carId)
		{
			if (!body)
			{
				return;
			}
			string key = $"{carId}i";
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key).BindTo(body);
			await handle.Task;
			if ((bool)body && handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && (bool)handle.Result)
			{
				GameObject gameObject = Object.Instantiate(handle.Result, body.transform, worldPositionStays: false);
				gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				SetLayerRecursive(gameObject.transform, body.layer);
				if ((bool)refs && (bool)refs.OldInterior)
				{
					refs.OldInterior.SetActive(active: false);
				}
			}
		}

		private static void SetLayerRecursive(Transform t, int layer)
		{
			t.gameObject.layer = layer;
			foreach (Transform item in t)
			{
				SetLayerRecursive(item, layer);
			}
		}
	}
}
