using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Free_drive
{
	public class FreeDrivePrefabSpawner : MonoBehaviour
	{
		private IPrefabFactory _prefabFactory;

		private IGameFactManager _gameFactManager;

		private bool _injected;

		[SerializeField]
		private GameObject[] Prefabs;

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

		public GameObject[] PrefabsToSpawn => Prefabs;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, IGameFactManager gameFactManager)
		{
			if (!_injected)
			{
				_prefabFactory = prefabFactory;
				_gameFactManager = gameFactManager;
				_injected = true;
			}
		}

		private void Start()
		{
			if (!FlagsStorage.IsFreeDriveMode || _gameFactManager.IsTrue(GameFact.ArenaActive))
			{
				return;
			}
			for (int i = 0; i < Prefabs.Length; i++)
			{
				if ((bool)Prefabs[i])
				{
					PrefabFactory.Instantiate(Prefabs[i], base.transform.position, base.transform.rotation, base.transform);
				}
			}
		}
	}
}
