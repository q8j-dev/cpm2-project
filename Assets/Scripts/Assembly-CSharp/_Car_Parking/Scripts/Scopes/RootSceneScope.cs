using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	public class RootSceneScope : LifetimeScope
	{
		private SceneScopeRegistry _registry;

		private Scene _registeredScene;

		protected override void Awake()
		{
			base.Awake();
			_registry = base.Container.Resolve<SceneScopeRegistry>();
			_registeredScene = base.gameObject.scene;
			_registry.Add(_registeredScene, base.Container);
		}

		protected override void OnDestroy()
		{
			_registry?.Remove(_registeredScene);
			_registry = null;
			base.OnDestroy();
		}
	}
}
