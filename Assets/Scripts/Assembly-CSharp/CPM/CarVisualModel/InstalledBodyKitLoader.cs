using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	public static class InstalledBodyKitLoader
	{
		public const string MountsRootName = "BodyKitMounts";

		public static string MountName(BodyKitType type, int id)
		{
			return $"KitMount_{type}_{id}";
		}

		private static string SimpleKey(int carId, BodyKitType type, int id)
		{
			string text = type switch
			{
				BodyKitType.Hood => "hood", 
				BodyKitType.FrontBumper => "fbumper", 
				BodyKitType.RearBumper => "rbumper", 
				BodyKitType.Skirt => "skirts", 
				BodyKitType.Fender => "fender", 
				_ => null, 
			};
			if (text != null)
			{
				return $"bodyKits/kit{carId}/{text}{id}.prefab";
			}
			return null;
		}

		private static string ComplexKey(BodyKitType type, int id)
		{
			switch (type)
			{
			case BodyKitType.Spoiler:
				return $"bodyKits/spoiler{id}.prefab";
			case BodyKitType.AirIntakeHood:
			case BodyKitType.AirIntakeRoof:
				return $"bodyKits/airintake{id}.prefab";
			case BodyKitType.Trim:
				return $"bodyKits/trims/trim{id}.prefab";
			default:
				return null;
			}
		}

		public static async UniTask<LoadedKits> LoadAsync(int carId, GameObject root, GameObject body, CarModelEntry entry, InstalledBodyKits kits, CancellationToken token)
		{
			List<LoadedKitInstance> instances = new List<LoadedKitInstance>();
			List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();
			HideBuiltInSpoilerIfReplaced(body, kits);
			List<BodyKitRequest> requests = BodyKitPlan.BuildLoadList(kits);
			for (int i = 0; i < requests.Count; i++)
			{
				BodyKitRequest bodyKitRequest = requests[i];
				try
				{
					if (bodyKitRequest.Class != BodyKitClass.Simple)
					{
						await LoadComplexAsync(bodyKitRequest.Type, bodyKitRequest.Id, root.transform, entry, instances, handles, token);
					}
					else
					{
						await LoadSimpleAsync(carId, bodyKitRequest.Type, bodyKitRequest.Id, body.transform, instances, handles, token);
					}
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
			return new LoadedKits(instances, handles);
		}

		private static async UniTask LoadSimpleAsync(int carId, BodyKitType type, int id, Transform bodyParent, List<LoadedKitInstance> instances, List<AsyncOperationHandle> handles, CancellationToken token)
		{
			string text = SimpleKey(carId, type, id);
			if (text == null || !AddressableExtensions.AddressableResourceExists(text, typeof(GameObject)))
			{
				return;
			}
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(text);
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
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, bodyParent);
				ApplyKitLocalTransform(gameObject2);
				MarkKitType(gameObject2, type);
				instances.Add(new LoadedKitInstance(gameObject2, type));
			}
		}

		private static async UniTask LoadComplexAsync(BodyKitType type, int id, Transform root, CarModelEntry entry, List<LoadedKitInstance> instances, List<AsyncOperationHandle> handles, CancellationToken token)
		{
			if (!BodyKitPlan.TryGetMount(entry.BodyKitMounts, type, id, out var mount))
			{
				return;
			}
			string text = ComplexKey(type, id);
			if (text == null || !AddressableExtensions.AddressableResourceExists(text, typeof(GameObject)))
			{
				return;
			}
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(text);
			handles.Add(handle);
			GameObject gameObject = await handle.ToUniTask(null, PlayerLoopTiming.Update, token);
			if (!gameObject)
			{
				handles.Remove(handle);
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
				return;
			}
			GameObject gameObject2 = new GameObject(MountName(type, id));
			if (mount.RawLocal)
			{
				gameObject2.transform.SetParent(GetOrCreateMountsRoot(root, entry), worldPositionStays: false);
				gameObject2.transform.localPosition = mount.LocalPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(mount.LocalEuler);
				gameObject2.transform.localScale = mount.LocalScale;
			}
			else
			{
				gameObject2.transform.SetParent(root, worldPositionStays: false);
				gameObject2.transform.localPosition = mount.LocalPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(mount.LocalEuler);
			}
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject, gameObject2.transform);
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale = Vector3.one;
			MarkKitType(gameObject3, type);
			instances.Add(new LoadedKitInstance(gameObject3, type));
		}

		private static void MarkKitType(GameObject instance, BodyKitType type)
		{
			if (instance.TryGetComponent<bodyKitValues>(out var component))
			{
				component.Type = (int)type;
			}
		}

		private static Transform GetOrCreateMountsRoot(Transform root, CarModelEntry entry)
		{
			Transform transform = root.Find("BodyKitMounts");
			if ((bool)transform)
			{
				return transform;
			}
			Transform transform2 = new GameObject("BodyKitMounts").transform;
			transform2.SetParent(root, worldPositionStays: false);
			transform2.localPosition = entry.BodyAnchorLocal;
			transform2.localRotation = Quaternion.identity;
			return transform2;
		}

		private static void HideBuiltInSpoilerIfReplaced(GameObject body, InstalledBodyKits kits)
		{
			if (kits != null && kits.SpoilerId != 0 && (bool)body && body.TryGetComponent<BodyMarker>(out var component) && (bool)component.spoiler)
			{
				component.spoiler.gameObject.SetActive(value: false);
			}
		}

		private static void ApplyKitLocalTransform(GameObject kit)
		{
			if (kit.TryGetComponent<bodyKitValues>(out var component))
			{
				kit.transform.localPosition = component.localPosition;
				kit.transform.localRotation = Quaternion.Euler(component.localEuler);
				kit.transform.localScale = Vector3.one;
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
