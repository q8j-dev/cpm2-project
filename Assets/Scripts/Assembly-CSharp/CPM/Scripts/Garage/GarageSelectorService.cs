using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace CPM.Scripts.Garage
{
	[UnityEngine.Scripting.Preserve]
	public class GarageSelectorService : IGarageSelectorService, ITickable
	{
		private readonly Dictionary<int, int> _garageScenesByID = new Dictionary<int, int>
		{
			{ 0, 33 },
			{ 1, 42 }
		};

		private readonly ISceneLoader _sceneLoader;

		private ProgressContainer _progressContainer;

		private bool _inLoading;

		private float _loadProgress;

		private AsyncOperation _asyncLoadOperation;

		public bool IsGarageSwitching { get; private set; }

		public float LoadProgress => _loadProgress;

		public int CurrentSceneIndex => _garageScenesByID[CurrentGarageIndex];

		public int CurrentGarageIndex => ObscuredPrefs.GetInt("SelectedGarage");

		public event Action<int> OnCurrentGarageIndexChanged;

		public GarageSelectorService(ISceneLoader sceneLoader)
		{
			_sceneLoader = sceneLoader;
		}

		public void Register(ProgressContainer progressContainer)
		{
			_progressContainer = progressContainer;
		}

		public void Tick()
		{
			if (_inLoading)
			{
				_loadProgress += _asyncLoadOperation.progress;
				if (_progressContainer != null)
				{
					_progressContainer.SetProgress(_loadProgress, "Loading...");
				}
			}
		}

		public async UniTask<bool> ChangeMenuGarage(int id, bool withScreen = false, bool checkCurrentIndex = true)
		{
			if (!_garageScenesByID.ContainsKey(id))
			{
				return false;
			}
			if ((CurrentGarageIndex == id) & checkCurrentIndex)
			{
				return false;
			}
			if (withScreen)
			{
				V.CoreGameManager.ManagerUI.ShowProgressWindow("").Forget();
				if (_progressContainer != null)
				{
					_progressContainer.SetProgress(0.17f, "Loading...");
				}
				await UniTask.Delay(1000);
			}
			IsGarageSwitching = true;
			_asyncLoadOperation = SceneManager.LoadSceneAsync(1);
			_inLoading = withScreen;
			await _asyncLoadOperation;
			ChangeGarageIndex(id);
			_inLoading = false;
			_loadProgress = 0f;
			IsGarageSwitching = false;
			return true;
		}

		public async UniTask LoadCurrentGarageAsync(params object[] instances)
		{
			await _sceneLoader.LoadSceneAsync(CurrentSceneIndex, LoadSceneMode.Additive, (IContainerBuilder builder) =>
			{
				object[] array = instances;
				foreach (object instance in array)
				{
					builder.RegisterInstance(instance).AsSelf();
				}
			});
		}

		public async UniTask ActivateWarmedGarageAsync(AsyncOperation warmedOperation, params object[] instances)
		{
			SceneLoadTracker.Begin();
			try
			{
				using (LifetimeScope.Enqueue((IContainerBuilder builder) =>
				{
					object[] array = instances;
					foreach (object instance in array)
					{
						builder.RegisterInstance(instance).AsSelf();
					}
				}))
				{
					warmedOperation.allowSceneActivation = true;
					await warmedOperation;
				}
			}
			finally
			{
				SceneLoadTracker.End();
			}
		}

		public void ChangeGarageIndex(int garageIndex)
		{
			ObscuredPrefs.SetInt("SelectedGarage", garageIndex);
		}

		public int GetGarageSceneIndex(int garageId)
		{
			if (!_garageScenesByID.TryGetValue(garageId, out var value))
			{
				return -1;
			}
			return value;
		}
	}
}
