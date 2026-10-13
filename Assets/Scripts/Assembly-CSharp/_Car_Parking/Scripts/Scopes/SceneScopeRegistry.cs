using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	public class SceneScopeRegistry
	{
		private readonly Dictionary<Scene, IObjectResolver> _map = new Dictionary<Scene, IObjectResolver>();

		public IEnumerable<IObjectResolver> SceneContexts => _map.Values;

		public void Add(Scene scene, IObjectResolver container)
		{
			if (_map.ContainsKey(scene))
			{
				throw new InvalidOperationException("Scene '" + scene.name + "' is already registered.");
			}
			_map.Add(scene, container);
		}

		public IObjectResolver GetSceneContextForScene(string name)
		{
			Scene sceneByName = SceneManager.GetSceneByName(name);
			if (!sceneByName.IsValid())
			{
				throw new InvalidOperationException("Could not find scene with name '" + name + "'");
			}
			return GetSceneContextForScene(sceneByName);
		}

		public IObjectResolver GetSceneContextForScene(Scene scene)
		{
			return _map[scene];
		}

		public IObjectResolver TryGetSceneContextForScene(string name)
		{
			Scene sceneByName = SceneManager.GetSceneByName(name);
			if (!sceneByName.IsValid())
			{
				throw new InvalidOperationException("Could not find scene with name '" + name + "'");
			}
			return TryGetSceneContextForScene(sceneByName);
		}

		public IObjectResolver TryGetSceneContextForScene(Scene scene)
		{
			if (_map.TryGetValue(scene, out var value))
			{
				return value;
			}
			return null;
		}

		public IObjectResolver GetContainerForScene(Scene scene)
		{
			IObjectResolver objectResolver = TryGetContainerForScene(scene);
			if (objectResolver != null)
			{
				return objectResolver;
			}
			throw new InvalidOperationException("Unable to find DiContainer for scene '" + scene.name + "'");
		}

		public IObjectResolver TryGetContainerForScene(Scene scene)
		{
			if (scene == LifetimeScope.Find<A_PersistentScope>().gameObject.scene)
			{
				return LifetimeScope.Find<A_PersistentScope>().Container;
			}
			IObjectResolver objectResolver = TryGetSceneContextForScene(scene);
			if (objectResolver != null)
			{
				return objectResolver;
			}
			return null;
		}

		public void Remove(Scene scene)
		{
			if (!_map.Remove(scene))
			{
				Debug.LogWarning("Failed to remove SceneContext from SceneContextRegistry");
			}
		}
	}
}
