using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class AsyncSceneSwitcher
	{
		private static bool _inProgress;

		public static bool InProgress => _inProgress;

		public static void Load(int buildIndex)
		{
			if (_inProgress)
			{
				return;
			}
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(buildIndex);
			if (asyncOperation != null)
			{
				_inProgress = true;
				LoadingPriorityBoost.Acquire();
				SceneLoadTracker.Begin();
				asyncOperation.completed += (AsyncOperation _) =>
				{
					SceneLoadTracker.End();
					LoadingPriorityBoost.Release();
					_inProgress = false;
				};
			}
		}
	}
}
