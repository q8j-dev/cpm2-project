using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Gley
{
	public class TrafficLoader : MonoBehaviour
	{
		private const float GreenSeconds = 10f;

		private const float YellowSeconds = 2f;

		private const float RedSeconds = 10f;

		public List<SemaphoresEnabler.SemaphoreGroup> SemaphoreGroups = new List<SemaphoresEnabler.SemaphoreGroup>();

		private SceneControl _sceneControl;

		private SemaphoresEnabler _semaphoresEnabler;

		private CancellationTokenSource _bulbCycleCts;

		public void Init(SceneControl sceneControl)
		{
			_sceneControl = sceneControl;
			_semaphoresEnabler = SemaphoresEnabler.GetSemaphoresEnabler();
			_sceneControl.OnSceneLoadedEvent += SceneControlOnOnSceneLoadedEvent;
			foreach (SemaphoresEnabler.SemaphoreGroup semaphoreGroup in SemaphoreGroups)
			{
				semaphoreGroup.SemaphoreParent.transform.SetParent(_semaphoresEnabler.transform);
				semaphoreGroup.SemaphoreParent.transform.localPosition = Vector3.zero;
			}
			Scene[] allLoadedScenes = ScenesHelper.GetAllLoadedScenes();
			foreach (Scene scene in allLoadedScenes)
			{
				SceneControlOnOnSceneLoadedEvent(scene.buildIndex, isLoaded: true);
			}
		}

		public void StartSemaphoreCycle()
		{
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			List<GameObject> list3 = new List<GameObject>();
			foreach (SemaphoresEnabler.SemaphoreGroup semaphoreGroup in SemaphoreGroups)
			{
				if (!semaphoreGroup.SemaphoreParent)
				{
					continue;
				}
				Transform[] componentsInChildren = semaphoreGroup.SemaphoreParent.GetComponentsInChildren<Transform>(includeInactive: true);
				foreach (Transform transform in componentsInChildren)
				{
					switch (transform.name.Replace("_", "").ToLowerInvariant())
					{
					case "redon":
						list.Add(transform.gameObject);
						break;
					case "yellowon":
						list2.Add(transform.gameObject);
						break;
					case "greenon":
						list3.Add(transform.gameObject);
						break;
					}
				}
			}
			if (list.Count != 0 || list2.Count != 0 || list3.Count != 0)
			{
				CommonExtensions.CancelAndDisposeToken(ref _bulbCycleCts);
				_bulbCycleCts = new CancellationTokenSource();
				CycleBulbsAsync(list, list2, list3, _bulbCycleCts.Token).Forget();
			}
		}

		private static async UniTaskVoid CycleBulbsAsync(List<GameObject> red, List<GameObject> yellow, List<GameObject> green, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				SetBulbs(red, active: false);
				SetBulbs(yellow, active: false);
				SetBulbs(green, active: true);
				await UniTask.Delay(TimeSpan.FromSeconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				SetBulbs(green, active: false);
				SetBulbs(yellow, active: true);
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				SetBulbs(yellow, active: false);
				SetBulbs(red, active: true);
				await UniTask.Delay(TimeSpan.FromSeconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}

		private static void SetBulbs(List<GameObject> bulbs, bool active)
		{
			foreach (GameObject bulb in bulbs)
			{
				if ((bool)bulb && bulb.activeSelf != active)
				{
					bulb.SetActive(active);
				}
			}
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _bulbCycleCts);
			if ((bool)_sceneControl)
			{
				_sceneControl.OnSceneLoadedEvent -= SceneControlOnOnSceneLoadedEvent;
			}
		}

		private void SceneControlOnOnSceneLoadedEvent(int sceneIndex, bool isLoaded)
		{
			SemaphoreGroups.Find((SemaphoresEnabler.SemaphoreGroup x) => x.SceneIndex == sceneIndex)?.SemaphoreParent.SetActive(isLoaded);
		}

		public void RemoveLights()
		{
			CommonExtensions.CancelAndDisposeToken(ref _bulbCycleCts);
			foreach (SemaphoresEnabler.SemaphoreGroup semaphoreGroup in SemaphoreGroups)
			{
				UnityEngine.Object.Destroy(semaphoreGroup.SemaphoreParent);
			}
			SemaphoreGroups.Clear();
		}
	}
}
