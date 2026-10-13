using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Racing_Scripts;

namespace RacingScripts.RaceControllers
{
	public class RaceModeController : MonoBehaviour
	{
		private RaceModeDataSO _raceModeDataSO;

		private AsyncOperationHandle<GameObject> _handle;

		private IPrefabFactory _prefabFactory;

		[HideInInspector]
		public GameObject SpawnedObject;

		public Action ShowMainMenuAction;

		[Inject]
		public void Construct(RaceModeDataSO raceModeDataSO, IPrefabFactory prefabFactory)
		{
			_raceModeDataSO = raceModeDataSO;
			_prefabFactory = prefabFactory;
		}

		public void ShowMainMenu()
		{
			ShowMainMenuAction?.Invoke();
		}

		private void Start()
		{
			InitializePrefabs();
		}

		private void InitializePrefabs()
		{
			SpawnedObject = _prefabFactory.Instantiate(_raceModeDataSO.RaceModeData.NeedToInstantiatePrefabForMode, base.transform);
		}

		public async UniTask InitializeScene(V.placeToSpawn sceneIndex)
		{
			await SceneManager.LoadSceneAsync(GetSceneIndex(sceneIndex), LoadSceneMode.Additive);
			if (sceneIndex != V.placeToSpawn.JapanSuburban)
			{
				return;
			}
			GlobalParent[] array = UnityEngine.Object.FindObjectsByType<GlobalParent>(FindObjectsSortMode.None);
			Transform transform = null;
			GlobalParent[] array2 = array;
			foreach (GlobalParent globalParent in array2)
			{
				if (globalParent.gameObject.scene.buildIndex == GetSceneIndex(sceneIndex))
				{
					transform = globalParent.transform;
					break;
				}
			}
			if ((bool)transform)
			{
				GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("FreeParts/JapanPartInFree"), transform);
				obj.transform.localPosition = new Vector3(0f, 0f, 0f);
				obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}

		public async UniTask UnloadScene(V.placeToSpawn sceneIndex)
		{
			int sceneIndex2 = GetSceneIndex(sceneIndex);
			if (SceneManager.GetSceneByBuildIndex(sceneIndex2).isLoaded)
			{
				await SceneManager.UnloadSceneAsync(sceneIndex2);
			}
		}

		private void OnDestroy()
		{
			if (_handle.IsValid())
			{
				Addressables.Release(_handle);
			}
		}

		public int GetSceneIndex(V.placeToSpawn placeToSpawn)
		{
			return placeToSpawn switch
			{
				V.placeToSpawn.MainCity => 14, 
				V.placeToSpawn.Highway => 27, 
				V.placeToSpawn.Desert => 15, 
				V.placeToSpawn.Mountain => 16, 
				V.placeToSpawn.Racing => 23, 
				V.placeToSpawn.City2 => 26, 
				V.placeToSpawn.JapanSerpentine => 35, 
				V.placeToSpawn.JapanSuburban => 36, 
				V.placeToSpawn.Island => 17, 
				V.placeToSpawn.TropicalIsland => 43, 
				V.placeToSpawn.Nurburgring => 44, 
				V.placeToSpawn.Italy => 47, 
				V.placeToSpawn.StraightRoad => 24, 
				V.placeToSpawn.Offroad => 29, 
				_ => 23, 
			};
		}
	}
}
