using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	public interface ISceneLoader
	{
		UniTask LoadSceneAsync(int sceneIndex, LoadSceneMode mode, Action<IContainerBuilder> extraBindings = null);
	}
}
