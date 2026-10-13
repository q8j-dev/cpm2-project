using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class VContainerSceneLoader : ISceneLoader
	{
		public async UniTask LoadSceneAsync(int sceneIndex, LoadSceneMode mode, Action<IContainerBuilder> extraBindings = null)
		{
			SceneLoadTracker.Begin();
			try
			{
				if (extraBindings != null)
				{
					using (LifetimeScope.Enqueue(extraBindings))
					{
						await SceneManager.LoadSceneAsync(sceneIndex, mode);
					}
				}
				else
				{
					await SceneManager.LoadSceneAsync(sceneIndex, mode);
				}
			}
			finally
			{
				SceneLoadTracker.End();
			}
		}
	}
}
