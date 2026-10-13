using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventObjectPrefabSpawner : MonoBehaviour
	{
		[SerializeField]
		private EventType EventType;

		[SerializeField]
		private GameObject Prefab;

		[SerializeField]
		private bool SkipSubscribe;

		private IEventService _eventService;

		private bool _wasSpawn;

		private IPrefabFactory _prefabFactory;

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
		public void Construct(IEventService eventService, OptionalDependency<IPrefabFactory> prefabFactory)
		{
			_prefabFactory = prefabFactory.Value;
			_eventService = eventService;
		}

		private void OnEnable()
		{
			if (_prefabFactory == null && (bool)V.CoreGameManager && V.CoreGameManager._prefabFactory != null && _eventService != null)
			{
				_prefabFactory = V.CoreGameManager._prefabFactory;
			}
			TrySpawn();
			if (!SkipSubscribe)
			{
				_eventService.OnEventsRetrieved += EventsRetrievedCallback;
			}
		}

		private void OnDisable()
		{
			if (!SkipSubscribe)
			{
				_eventService.OnEventsRetrieved -= EventsRetrievedCallback;
			}
		}

		private void EventsRetrievedCallback()
		{
			TrySpawn();
		}

		public void TrySpawn()
		{
			if (!FlagsStorage.IsLevelScene && !_wasSpawn && (bool)Prefab && PrefabFactory != null && _eventService.GetEventByName(EventType) != null)
			{
				_wasSpawn = true;
				IEventGameObjectInit[] componentsInChildren = PrefabFactory.Instantiate(Prefab, base.transform.position, base.transform.rotation, base.transform).GetComponentsInChildren<IEventGameObjectInit>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Init(EventType);
				}
			}
		}
	}
}
