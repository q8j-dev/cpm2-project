using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace Car_Parking.Scripts.Racing
{
	public class InjectableSceneObjectSpawner : MonoBehaviour
	{
		private IPrefabFactory _prefabFactory;

		private IGameFactManager _gameFactManager;

		[SerializeField]
		private GameObject Prefab;

		[SerializeField]
		private bool SpawnOnlyInMultiplayerMode;

		private bool _wasSpawn;

		private IPrefabFactory PrefabFactory
		{
			get
			{
				if (_prefabFactory != null)
				{
					return _prefabFactory;
				}
				if (!V.CoreGameManager)
				{
					return null;
				}
				return V.CoreGameManager._prefabFactory;
			}
		}

		[Inject]
		public void Construct(OptionalDependency<IPrefabFactory> prefabFactory, IGameFactManager gameFactManager)
		{
			_prefabFactory = prefabFactory.Value;
			_gameFactManager = gameFactManager;
		}

		private void OnEnable()
		{
			TrySpawn();
		}

		private void TrySpawn()
		{
			if (!_wasSpawn && (_gameFactManager == null || !_gameFactManager.IsTrue(GameFact.ArenaActive)) && (!SpawnOnlyInMultiplayerMode || FlagsStorage.IsMultiFree) && (SpawnOnlyInMultiplayerMode || FlagsStorage.IsFreeDriveMode))
			{
				PrefabFactory.Instantiate(Prefab, base.transform);
				_wasSpawn = true;
			}
		}
	}
}
