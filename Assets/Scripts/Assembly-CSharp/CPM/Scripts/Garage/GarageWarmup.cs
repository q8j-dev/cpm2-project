using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.Scripts.Garage
{
	public sealed class GarageWarmup
	{
		private CancellationTokenSource _cts;

		private Scene _scene;

		private GameObject[] _roots;

		private bool[] _rootsWereActive;

		private LightmapData[] _garageLightmaps;

		private LightmapsMode _garageLightmapsMode;

		private LightProbes _garageLightProbes;

		private LightmapData[] _locationLightmaps;

		private LightmapsMode _locationLightmapsMode;

		private LightProbes _locationLightProbes;

		private ftLightmapsStorage _lightmapsStorage;

		public int ResidentSceneIndex { get; private set; } = -1;

		public Transform Owner { get; private set; }

		public bool IsResident
		{
			get
			{
				if (ResidentSceneIndex >= 0 && _roots != null)
				{
					return _scene.isLoaded;
				}
				return false;
			}
		}

		public bool IsPreparing { get; private set; }

		public bool IsResidentFor(int sceneIndex)
		{
			if (IsResident)
			{
				return ResidentSceneIndex == sceneIndex;
			}
			return false;
		}

		public void EnsureResident(int sceneIndex, object[] scopeInstances, Transform owner)
		{
			if (!IsPreparing && !IsResidentFor(sceneIndex))
			{
				Release();
				Owner = owner;
				IsPreparing = true;
				_cts = new CancellationTokenSource();
				PrepareAsync(sceneIndex, scopeInstances, _cts.Token).Forget();
			}
		}

		private async UniTaskVoid PrepareAsync(int sceneIndex, object[] scopeInstances, CancellationToken token)
		{
			_ = Time.realtimeSinceStartup;
			SteamLowLoadingPriority.Acquire();
			try
			{
				_locationLightmaps = CloneLightmaps(LightmapSettings.lightmaps);
				_locationLightmapsMode = LightmapSettings.lightmapsMode;
				_locationLightProbes = LightmapSettings.lightProbes;
				HashSet<int> existingHandles = CollectSceneHandles(sceneIndex);
				Scene scene;
				using (LifetimeScope.Enqueue((IContainerBuilder builder) =>
				{
					object[] array = scopeInstances;
					foreach (object instance in array)
					{
						builder.RegisterInstance(instance).AsSelf();
					}
				}))
				{
					AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
					if (asyncOperation == null)
					{
						return;
					}
					scene = FindNewScene(sceneIndex, existingHandles);
					await asyncOperation.ToUniTask(null, PlayerLoopTiming.Update, token).SuppressCancellationThrow();
				}
				if (!scene.IsValid())
				{
					scene = FindNewScene(sceneIndex, existingHandles);
				}
				if (!token.IsCancellationRequested && scene.isLoaded)
				{
					_scene = scene;
					CaptureGarageLightmaps();
					RestoreLocationLightmaps();
					_roots = scene.GetRootGameObjects();
					_lightmapsStorage = FindLightmapsStorage(_roots);
					SetRootsActive(active: false);
					ResidentSceneIndex = sceneIndex;
				}
			}
			finally
			{
				SteamLowLoadingPriority.Release();
				IsPreparing = false;
			}
		}

		public bool TryActivate()
		{
			if (!IsResident)
			{
				return false;
			}
			SetRootsActive(active: true);
			if (_garageLightmaps != null)
			{
				LightmapSettings.lightmapsMode = _garageLightmapsMode;
				LightmapSettings.lightmaps = _garageLightmaps;
				LightmapSettings.lightProbes = _garageLightProbes;
			}
			RefreshBakedLight();
			return true;
		}

		private void RefreshBakedLight()
		{
			if ((bool)_lightmapsStorage)
			{
				ftLightmaps.RefreshScene(_scene, _lightmapsStorage);
				ftLightmaps.RefreshScene2(_scene, _lightmapsStorage);
			}
		}

		private static ftLightmapsStorage FindLightmapsStorage(GameObject[] roots)
		{
			foreach (GameObject gameObject in roots)
			{
				if ((bool)gameObject && gameObject.TryGetComponent<ftLightmapsStorage>(out var component))
				{
					return component;
				}
			}
			return null;
		}

		public void Deactivate()
		{
			if (IsResident)
			{
				SetRootsActive(active: false);
			}
		}

		public void Release()
		{
			CancellationTokenSource cts = _cts;
			_cts = null;
			if (cts != null)
			{
				cts.Cancel();
				cts.Dispose();
			}
			if (_scene.isLoaded)
			{
				SceneManager.UnloadSceneAsync(_scene);
			}
			_scene = default;
			Owner = null;
			_roots = null;
			_rootsWereActive = null;
			_garageLightmaps = null;
			_garageLightProbes = null;
			_lightmapsStorage = null;
			ResidentSceneIndex = -1;
		}

		private void SetRootsActive(bool active)
		{
			if (_roots == null)
			{
				return;
			}
			if (!active)
			{
				_rootsWereActive = new bool[_roots.Length];
				for (int i = 0; i < _roots.Length; i++)
				{
					if ((bool)_roots[i])
					{
						_rootsWereActive[i] = _roots[i].activeSelf;
						_roots[i].SetActive(value: false);
					}
				}
				return;
			}
			for (int j = 0; j < _roots.Length; j++)
			{
				if ((bool)_roots[j])
				{
					bool active2 = _rootsWereActive == null || j >= _rootsWereActive.Length || _rootsWereActive[j];
					_roots[j].SetActive(active2);
				}
			}
		}

		private static HashSet<int> CollectSceneHandles(int sceneIndex)
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.buildIndex == sceneIndex)
				{
					hashSet.Add(sceneAt.handle);
				}
			}
			return hashSet;
		}

		private static Scene FindNewScene(int sceneIndex, HashSet<int> existingHandles)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.buildIndex == sceneIndex && !existingHandles.Contains(sceneAt.handle))
				{
					return sceneAt;
				}
			}
			return default;
		}

		private void CaptureGarageLightmaps()
		{
			_garageLightmaps = CloneLightmaps(LightmapSettings.lightmaps);
			_garageLightmapsMode = LightmapSettings.lightmapsMode;
			_garageLightProbes = LightmapSettings.lightProbes;
		}

		private static LightmapData[] CloneLightmaps(LightmapData[] source)
		{
			if (source == null)
			{
				return null;
			}
			LightmapData[] array = new LightmapData[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] != null)
				{
					array[i] = new LightmapData
					{
						lightmapColor = source[i].lightmapColor,
						lightmapDir = source[i].lightmapDir,
						shadowMask = source[i].shadowMask
					};
				}
			}
			return array;
		}

		private void RestoreLocationLightmaps()
		{
			LightmapSettings.lightmapsMode = _locationLightmapsMode;
			LightmapSettings.lightmaps = _locationLightmaps;
			LightmapSettings.lightProbes = _locationLightProbes;
		}
	}
}
